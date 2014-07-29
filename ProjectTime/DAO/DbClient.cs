using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using TicTac.DAO;

namespace TicTac.DAO
{
    class DbClient : DAOClientClass
    {
        private MySqlConnection _connection;
        public string Server    { get; set; } //TODO: WANT A PRIVATE SET
        public string Database  { get; set; } //TODO: WANT A PRIVATE SET
        public string Uid       { get; set; } //TODO: WANT A PRIVATE SET
        public string Password  { get; set; } //TODO: WANT A PRIVATE SET

        //Constructor
        public DbClient()
        {
            Server = null;
            Database = null;
            Uid = null;
            Password = null;
            
            //If not initialized
            if (Server == null)
            {
                Server =    TicTac.Database.DbServerIp;
                Database =  TicTac.Database.DbName;
                Uid =       TicTac.Database.DbUserName;
                Password =  TicTac.Database.DbPassword;
            }
            
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            string connectionString = "SERVER=" + Server + ";" + "DATABASE=" + Database + ";" + "UID=" + Uid + ";" + "PASSWORD=" + Password + ";";
            _connection = new MySqlConnection(connectionString);
        }

        //open connection to database
        private bool OpenConnection()
        {
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
        public override void StartWorkSession(Session cfg)
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
        public override void EndWorkSession(Session s)
        {
            if (s == null) throw new ArgumentNullException("s");
            if (!s.IsValid() || !s.IsTerminated()) throw new Exception();
            if (!OpenConnection()) return;
            //var formattedDate = String.Format("{0:yyyy/MM/dd HH:mm:ss}", s.StopTime);
            
            var cmd = new MySqlCommand(null, _connection)
            {
                CommandText = "UPDATE r_worked " +
                              "SET enddate=TIMESTAMP(NOW()) " +
                              "WHERE " +
                              "enddate IS NULL"         + " AND " +
                              "archi=" + s.Architect.Id + " AND " +
                              "project=" + s.Project.Id + " AND " +
                              "phase=" + s.Phase.Id + " AND " +
#if (DEBUG)
                              "test=1"
#else
                              "test=0"
#endif
            };

            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        //TODO: must return info for a given Architect
        public override List<Session> SelectStartedWorkSessions(Architect archi)
        {
            if (!OpenConnection() || archi == null || archi.Id == null) return null;

            var li = new List<Session>();
            var cmd = new MySqlCommand(null, _connection)
            {
                CommandText = "SELECT * FROM r_worked " +
                              "WHERE " +
                              "enddate IS NULL" + " AND " +
                              "archi=" + archi.Id + " AND " +
                              //"project=" + s.Project.Id + " AND " +
                              //"phase=" + s.Phase.Id + " AND " +
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
                    var id = uint.Parse(reader["id"].ToString());
                    var archiId = int.Parse(reader["archi"].ToString());
                    var projectId = int.Parse(reader["project"].ToString());
                    var phaseId = int.Parse(reader["phase"].ToString());
                    var startTime = DateTime.Parse(reader["startdate"].ToString());
                    var tempConnexion = new DbClient();
                    li.Add(new Session
                        {
                            Architect = tempConnexion.SelectArchitectFromId(archiId),
                            Project = tempConnexion.SelectProjectFromId(projectId),
                            Phase = tempConnexion.SelectPhaseFromId(phaseId),
                            RunningSessionId = id,
                            StartTime = startTime
                        });
                }
            }

            CloseConnection();
            return li;
        }


        public override TimeSpan SelectTimeCount(int? archiId, int? projectId, int? phaseId)
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
            var count = TimeSpan.Zero;
            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    var start = DateTime.Parse(dataReader["startdate"].ToString());
                    var end = DateTime.Parse(dataReader["enddate"].ToString());
                    if (end >= start)
                    {
                        count += (end - start);
                    }
                }

                dataReader.Close();
                CloseConnection();
            }
            return count;
        }

        public override double SelectTimeCountFromProjectId(int? id)
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
                    var start = DateTime.Parse(dataReader["startdate"].ToString());
                    var end = DateTime.Parse(dataReader["enddate"].ToString());
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

        public override Architect SelectArchitectFromId(int id)
        {
            string query = "SELECT * FROM e_architect WHERE ID=\"" + id + "\"";
            var archi = new Architect();

            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    archi.Id = int.Parse(dataReader["id"].ToString()); 
                    archi.FirstName = (string)dataReader["firstname"];
                    archi.LastName = (string)dataReader["lastname"];
                    archi.Company = int.Parse(dataReader["id"].ToString());
                }
                dataReader.Close();
                CloseConnection();
            }

            return archi;
        }

        public override Project SelectProjectFromId(int id)
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

        public override Phase SelectPhaseFromId(int id)
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

        public override Company SelectCompanyFromId(int id)
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

        public override List<Architect> SelectAllArchitects()
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
                    architects.Add(new Architect{ Id = id, FirstName = fn, LastName = ln, Company = co });
                }
                dataReader.Close();
                CloseConnection();
            }

            return architects;
        }

        public override List<Project> SelectAllProjects()
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
                    projects.Add(new Project{ Id = id, Name = na });
                }
                dataReader.Close();
                CloseConnection();
            }

            return projects;
        }

        public override List<Phase> SelectAllPhases()
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
                    var d = dataReader["description"].ToString();
                    phases.Add(new Phase{ Id = id, Name = na, Description = d });
                }
                dataReader.Close();
                CloseConnection();
            }

            return phases;
        }

        public override List<Company> SelectAllCompanies()
        {
            const string query = "SELECT * FROM e_company";
            var l = new List<Company>();

            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    var id = (int)dataReader["id"];
                    var na = (string)dataReader["name"];
                    l.Add(new Company(id, na));
                }
                dataReader.Close();
                CloseConnection();
            }

            return l;
        }

        public override int InsertArchitect(Architect a)
        {
            var insertedRowId = -1;
            if (!a.IsValid() || !OpenConnection()) return insertedRowId;

            using (var cmd = new MySqlCommand(null, _connection)
                                {
                                    CommandText = "INSERT INTO e_architect " +
                                                    "VALUES ('', @firstname, @lastname, @company)"
                                })
            {
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@firstname", a.FirstName);
                cmd.Parameters.AddWithValue("@lastname", a.LastName);
                cmd.Parameters.AddWithValue("@company", a.Company);
                cmd.ExecuteNonQuery();

                // Get the last inserted id
                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT @@IDENTITY";
                insertedRowId = Convert.ToInt32(cmd.ExecuteScalar());    
            }
            
            CloseConnection();
            return insertedRowId;
        }

        public override int InsertProject(Project p)
        {
            var insertedRowId = -1;
            if (!p.IsValid() || !OpenConnection()) return insertedRowId;

            using (var cmd = new MySqlCommand(null, _connection)
            {
                CommandText = "INSERT INTO e_project " +
                                "VALUES ('', @name, @description)"
            })
            {
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@name", p.Name);
                cmd.Parameters.AddWithValue("@description", p.Description);
                // TODO: possibility to add a project with an non null working time
                //cmd.Parameters.AddWithValue("@totaltime", /*TODO*/);
                cmd.ExecuteNonQuery();

                // Get the last inserted id
                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT @@IDENTITY";
                insertedRowId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            CloseConnection();
            return insertedRowId;
        }

        public override int InsertPhase(Phase p)
        {
            var insertedRowId = -1;
            if (!p.IsValid() || !OpenConnection()) return insertedRowId;

            using (var cmd = new MySqlCommand(null, _connection)
            {
                CommandText = "INSERT INTO e_phase " +
                                "VALUES ('', @name, @description)"
            })
            {
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@name", p.Name);
                cmd.Parameters.AddWithValue("@description", p.Description);
                cmd.ExecuteNonQuery();

                // Get the last inserted id
                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT @@IDENTITY";
                insertedRowId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            CloseConnection();
            return insertedRowId;
        }

        public override bool DeleteArchitect(Architect a)
        {
            if (a == null || a.Id == null) return false;
            string query = "DELETE FROM e_architect WHERE ID=\"" + a.Id + "\"";
            
            if (OpenConnection())
            {
                using (var cmd = new MySqlCommand(query, _connection))
                {
                    cmd.ExecuteNonQuery();
                }
                
                CloseConnection();
            }
            return true;
        }

        public override bool DeleteProject(Project p)
        {
            if (p == null || p.Id == null) return false;
            string query = "DELETE FROM e_project WHERE ID=\"" + p.Id + "\"";

            if (OpenConnection())
            {
                using (var cmd = new MySqlCommand(query, _connection))
                {
                    cmd.ExecuteNonQuery();
                }

                CloseConnection();
            }
            return true;
        }

        public override bool DeletePhase(Phase p)
        {
            if (p == null || p.Id == null) return false;
            string query = "DELETE FROM e_phase WHERE ID=\"" + p.Id + "\"";

            if (OpenConnection())
            {
                using (var cmd = new MySqlCommand(query, _connection))
                {
                    cmd.ExecuteNonQuery();
                }

                CloseConnection();
            }
            return true;
        }
    }
}
