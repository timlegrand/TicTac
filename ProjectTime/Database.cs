using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ProjectTime
{
    class Database
    {
        private MySqlConnection _connection;
        private string _server;
        private string _database;
        private string _uid;
        private string _password;

        //Constructor
        public Database()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            _server = Program.ServerIp;
            _database = "he";
            _uid = "he";
            _password = "mySqlUserPassword";
            string connectionString = "SERVER=" + _server + ";" + "DATABASE=" + _database + ";" + "UID=" + _uid + ";" + "PASSWORD=" + _password + ";";

            _connection = new MySqlConnection(connectionString);
        }

        //open connection to database
        private bool OpenConnection()
        {
            if (!Program.IsDatabaseConnexionAvailable())
            {
                MessageBox.Show(@"Vous devez être connecté à Internet pour ajouter des entrées dans la base de données.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (_connection.State == ConnectionState.Open)
            {
                return true;
            }
            
            try
            {
                _connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                // The two most common error numbers when connecting are as follows
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show(@"Cannot connect to server. Contact administrator.");
                        break;

                    case 1045:
                        MessageBox.Show(@"Invalid username/password, please try again.");
                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                _connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        //StartWorkSession statement
        public void StartWorkSession(Session cfg)
        {
            var test = 0;
#if (DEBUG)
            Console.WriteLine(@"Debug mode: row will be inserted with field ""test""=1.");
            test = 1;
#endif

            if (!OpenConnection()) return;

            var cmd = new MySqlCommand(null, _connection)
                {
                    CommandText = "INSERT INTO r_worked " +
                                  "VALUES ('', NULL, TIMESTAMP(NOW()), @archi, @project, @phase, @test)"
                };
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@archi", cfg.Architect.Id);
            cmd.Parameters.AddWithValue("@project", cfg.Project.Id);
            cmd.Parameters.AddWithValue("@phase", cfg.Phase.Id);
            cmd.Parameters.AddWithValue("@test", test);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        //Update statement
        public void EndWorkSession(Session cfg)
        {
            var sessions = StartedWorkSessions(cfg.Architect);
            if (sessions.Count != 1)
            {
                string msg = @"La base de données contient " + sessions.Count + @" occurence(s) pour la combinaison choisie.";
                Console.WriteLine(msg);
                //MessageBox.Show(msg, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!OpenConnection()) return;

            var cmd = new MySqlCommand(null, _connection)
            {
                CommandText = "UPDATE r_worked " +
                              "SET enddate=TIMESTAMP(NOW()) " +
                              "WHERE " +
                              "enddate IS NULL"         + " AND " +
                              "archi=" + cfg.Architect.Id + " AND " +
                              "project=" + cfg.Project.Id + " AND " +
                              "phase=" + cfg.Phase.Id + " AND " +
#if (DEBUG)
                              "test=1"
#else
                              "test=0"
#endif
            };

            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        //TODO: must return info for a given (or not?) Architect
        //Search statement
        public List<Session> StartedWorkSessions(Architect archi)
        {
            Program.VarDump(archi);
            var li = new List<Session>();
            
            if (!OpenConnection() || !archi.IsValid()) return null;

            var cmd = new MySqlCommand(null, _connection)
            {
                CommandText = "SELECT * FROM r_worked " +
                              "WHERE " +
                              "enddate IS NULL" + " AND " +
                              "archi=" + archi.Id + " AND " +
                              //"project=" + cfg.Project.Id + " AND " +
                              //"phase=" + cfg.Phase.Id + " AND " +
#if (DEBUG)
                              "test=1"
#else
                              "test=0"
#endif
            };

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var archiId = (int)reader["archi"];
                    var projectId = (int)reader["project"];
                    var phaseId = (int)reader["phase"];
                    li.Add(new Session(archiId, projectId, phaseId));
                }
            }

            if (li.Count == 1)
            {
                //TODO
            }

            CloseConnection();
            return li; 
        }


        public double GetTimeCount(int? archiId, int? projectId, int? phaseId)
        {
            string where = " WHERE enddate IS NOT NULL";
            if ((archiId != null) || (projectId != null) || (phaseId != null))
            {
                where += " AND ";
            }
            if (archiId != null)
            {
                where += "archi=\"" + archiId + "\"";
            }
            if (projectId != null)
            {
                if (archiId != null)
                {
                    where += " AND ";
                }
                where += "project=\"" + projectId + "\"";
            }
            if (phaseId != null)
            {
                if (projectId != null)
                {
                    where += " AND ";
                }
                where += "phase=\"" + phaseId + "\"";
            }
            
            string query = "SELECT * FROM r_worked" + where;
            double count = 0;
            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    var start = System.DateTime.Parse(dataReader["startdate"].ToString());
                    var end = System.DateTime.Parse(dataReader["enddate"].ToString());
                    if (end >= start)
                    {
                        count += (end - start).TotalSeconds;
                    }
                }

                dataReader.Close();
                CloseConnection();
            }
            return count;
        }

        public double GetTimeCountFromProjectId(int? id)
        {
            Debug.Assert(id != null);
                
            string query = "SELECT * FROM r_worked WHERE project=\"" + id + "\"";
            double count=0;
            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    var start = System.DateTime.Parse(dataReader["startdate"].ToString());
                    var end = System.DateTime.Parse(dataReader["enddate"].ToString());
                    if (end >= start)
                    {
                        count += (end - start).TotalSeconds;
                    }
                }

                dataReader.Close();
                CloseConnection();
            }
            return count;
        }

        public Architect GetArchitectFromId(int id)
        {
            string query = "SELECT * FROM e_architect WHERE ID=\"" + id + "\"";
            var archi = new Architect();

            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    archi.FirstName = (string)dataReader["firstname"];
                    archi.LastName = (string)dataReader["lastname"];
                }
                dataReader.Close();
                CloseConnection();
            }

            return archi;
        }

        public Project GetProjectFromId(int id)
        {
            string query = "SELECT * FROM e_project WHERE ID=\"" + id + "\"";
            var pro = new Project();

            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    pro.Id = (int)dataReader["id"];
                    pro.Name = (string)dataReader["name"];
                }
                dataReader.Close();
                CloseConnection();
            }

            return pro;
        }

        public Phase GetPhaseFromId(int id)
        {
            string query = "SELECT * FROM e_phase WHERE ID=\"" + id + "\"";
            var phase = new Phase();

            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    phase.Id = (int)dataReader["id"];
                    phase.Name = (string)dataReader["name"];
                }
                dataReader.Close();
                CloseConnection();
            }

            return phase;
        }

        public Company GetCompanyFromId(int id)
        {
            string query = "SELECT * FROM e_company WHERE ID=\"" + id + "\"";
            var co = new Company();

            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    co.Id = (int)dataReader["id"]; 
                    co.Name = (string)dataReader["name"];
                }
                dataReader.Close();
                CloseConnection();
            }

            return co;
        }

        public List<Architect> SelectAllArchitects()
        {
            const string query = "SELECT * FROM e_architect";
            var architects = new List<Architect>();

            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    var id = (int) dataReader["id"];
                    var fn = (string) dataReader["firstname"];
                    var ln = (string) dataReader["lastname"];
                    var co = (int) dataReader["company"];
                    architects.Add(new Architect(id, fn, ln, co));
                }
                dataReader.Close();
                CloseConnection();
            }

            return architects;
        }

        public List<Project> SelectAllProjects()
        {
            const string query = "SELECT * FROM e_project";
            var projects = new List<Project>();
            
            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    var id = (int) dataReader["id"];
                    var na = (string)dataReader["name"];
                    projects.Add(new Project(id, na));
                }
                dataReader.Close();
                CloseConnection();
            }

            return projects;
        }

        public List<Phase> SelectAllPhases()
        {
            const string query = "SELECT * FROM e_phase";
            var phases = new List<Phase>();
            
            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    var id = (int)dataReader["id"];
                    var na = (string)dataReader["name"];
                    phases.Add(new Phase(id, na));
                }
                dataReader.Close();
                CloseConnection();
            }

            return phases;
        }

    }
}
