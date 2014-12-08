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
        public string Server    { get; private set; }
        public string Database  { get; private set; }
        public string Uid       { get; private set; }
        public string Password  { get; private set; }
        private MySqlConnection _connection;
        private string _connectionString;

        //Constructor
        public DbClient()
        {
            Server =    TicTac.Database.ServerAddress;
            Database =  TicTac.Database.Name;
            Uid =       TicTac.Database.UserName;
            Password =  TicTac.Database.Password;

            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            _connectionString = "SERVER=" + Server + ";" + "DATABASE=" + Database + ";" + "UID=" + Uid + ";" + "PASSWORD=" + Password + ";";
            _connection = new MySqlConnection(_connectionString);
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
                        Logger.Write(String.Format("{0}: \"{1}\"", ex.Message, Server));
                        MessageBox.Show(@"Cannot connect to server. Contact administrator.");
                        break;

                    case 1045:
                        Logger.Write(String.Format("{0}: with login \"{1}\" and password \"{2}\"", ex.Message, Uid, Password));
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
                Logger.Write(ex);
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        //InsertWorkSession statement
        public override void InsertWorkSession(WorkSession s)
        {
#if (DEBUG)
            Console.WriteLine(@"Debug mode: row will be inserted with field ""test""=1.");
#endif

            if (!OpenConnection()) return;

            var cmd = new MySqlCommand(null, _connection)
                {
                    CommandText = "INSERT INTO r_worked " +
                                  "VALUES ('', NULL, TIMESTAMP(NOW()), @archi, @project, @phase, @test, @company)"
                };
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@archi", s.Architect.Id);
            cmd.Parameters.AddWithValue("@project", s.Project.Id);
            cmd.Parameters.AddWithValue("@phase", s.Phase.Id);
#if (DEBUG)
            cmd.Parameters.AddWithValue("@test", 1);
#else
            cmd.Parameters.AddWithValue("@test", 0);
#endif
            cmd.Parameters.AddWithValue("@company", s.Architect.Company);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        //Update statement
        public override void UpdateWorkSession(WorkSession s)
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
        public override List<WorkSession> SelectStartedWorkSessions(Architect archi)
        {
            if (!OpenConnection() || archi == null || archi.Id == null) return null;

            var li = new List<WorkSession>();

            var cmdString = "SELECT * FROM r_worked " +
                            "WHERE " +
                            "enddate IS NULL" + " AND " +
                            "archi=" + archi.Id + " AND " +
#if (DEBUG)
                            "test=1";
#else
                            "test=0";
#endif

            var cmd = new MySqlCommand(null, _connection)
            {
                CommandText = cmdString
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
                    li.Add(new WorkSession
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


        public override List<WorkSession> SelectWorkSessions(Architect archi, DateRange dr = null)
        {
            if (!OpenConnection() || archi == null || archi.Id == null)
            {
                throw new ArgumentException();
            }


            var li = new List<WorkSession>();

            var startDate = dr != null ? dr.StartDate : null;
            var endDate = dr != null ? dr.EndDate : null;
            var startString = startDate != null ? "startdate>='" + ((DateTime) startDate).ToString("yyyy-MM-dd HH:mm:ss") + "' AND " : "";
            var endString = endDate != null ? "enddate<='" + ((DateTime) endDate).ToString("yyyy-MM-dd HH:mm:ss") + "' AND " : "";

            var cmdString = "SELECT * FROM r_worked " +
                            "WHERE " +
                            "archi=" + archi.Id + " AND " +
                            startString + endString +
#if (DEBUG)
                            "test=1";
#else
                            "test=0";
#endif

            var cmd = new MySqlCommand(null, _connection)
            {
                CommandText = cmdString
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
                    var stopTime = DateTime.Parse(reader["enddate"].ToString());
                    var tempConnexion = new DbClient();
                    li.Add(new WorkSession
                    {
                        Architect = tempConnexion.SelectArchitectFromId(archiId),
                        Project = tempConnexion.SelectProjectFromId(projectId),
                        Phase = tempConnexion.SelectPhaseFromId(phaseId),
                        RunningSessionId = id,
                        StartTime = startTime,
                        StopTime = stopTime
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
                    var de = (string)dataReader["description"];
                    projects.Add(new Project{ Id = id, Name = na, Description = de });
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

        internal int UpdateArchitect(int id, Architect a)
        {
            if (a == null) return -1;

            var insertedRowId = -1;
            if (!a.IsValidWithoutId() || !OpenConnection()) return insertedRowId;

            using (var cmd = new MySqlCommand(null, _connection)
            {
                CommandText = "UPDATE e_architect SET " +
                                "firstname='" + a.FirstName + "', " +
                                "lastName='" + a.LastName + "', " +
                                "company='" + a.Company + "' " +
                                "WHERE ID='" + id + "'"
            })
            {
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }

            CloseConnection();
            return id;
        }

        internal object UpdateProject(int id, Project p)
        {
            if (p == null) return -1;

            var insertedRowId = -1;
            if (!p.IsValidWithoutId() || !OpenConnection()) return insertedRowId;

            using (var cmd = new MySqlCommand(null, _connection)
            {
                CommandText = "UPDATE e_project SET " +
                                "name='" + p.Name + "', " +
                                "description='" + p.Description + "' " +
                                "WHERE ID='" + id + "'"
            })
            {
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }

            CloseConnection();
            return id;
        }

        internal object UpdatePhase(int id, Phase p)
        {
            if (p == null) return -1;

            var insertedRowId = -1;
            if (!p.IsValidWithoutId() || !OpenConnection()) return insertedRowId;

            using (var cmd = new MySqlCommand(null, _connection)
            {
                CommandText = "UPDATE e_phase SET " +
                                "name='" + p.Name + "', " +
                                "description='" + p.Description + "' " +
                                "WHERE ID='" + id + "'"
            })
            {
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }

            CloseConnection();
            return id;
        }

        internal DataTable GetWorkSessionDataTable(int? archi_id, int? project_id, int? phase_id)
        {
            if (OpenConnection())
            {
                var query = "SELECT id, startdate, enddate, TIMEDIFF(enddate, startdate) AS 'duration' FROM r_worked " +
                           "WHERE " +
                           "enddate IS NOT NULL";
                query += archi_id != null ? " AND archi=" + archi_id : "";
                query += project_id != null ? " AND project=" + project_id : "";
                query += phase_id != null ? " AND phase=" + phase_id : "";
                Console.WriteLine(query);
                var cmd = new MySqlCommand(query, _connection);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable data = new DataTable();
                adapter.Fill(data);

                CloseConnection();
                return data;
            }

            return null;
        }
    }
}
