using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ProjectTime
{
    class DBConnect
    {
        private MySqlConnection _connection;
        private string _server;
        private string _database;
        private string _uid;
        private string _password;

        //Constructor
        public DBConnect()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            _server = "82.240.213.167";
            _database = "he";
            _uid = "he";
            _password = "mySqlUserPassword";
            string connectionString = "SERVER=" + _server + ";" + "DATABASE=" + _database + ";" + "UID=" + _uid + ";" + "PASSWORD=" + _password + ";";

            _connection = new MySqlConnection(connectionString);
        }

        //open connection to database
        private bool OpenConnection()
        {
            if (!Program.IsInternetConnexionAvailable())
            {
                MessageBox.Show("Vous devez être connecté à Internet pour ajouter des entrées dans la base de données.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                        MessageBox.Show("Cannot connect to server. Contact administrator.");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again.");
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

        //InsertWorked statement
        public void InsertWorked(double elapsedTime, Architect archi, Project project, Phase phase)
        {
            if (OpenConnection())
            {
                var cmd = new MySqlCommand(null, _connection);
                cmd.CommandText = "INSERT INTO r_worked VALUES (TIMESTAMP(NOW()), TIMESTAMP(NOW()), @archi, @project, @phase, @time, @test)";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@archi", archi.Id);
                cmd.Parameters.AddWithValue("@project", project.Id);
                cmd.Parameters.AddWithValue("@phase", phase.Id);
                cmd.Parameters.AddWithValue("@time", elapsedTime);
                var test = 0;
#if (DEBUG)
                Console.WriteLine("Debug mode: row inserted with field \"test\"=1.");
                test = 1;
#endif
                cmd.Parameters.AddWithValue("@test", test);

                cmd.ExecuteNonQuery();
                CloseConnection();
            }
        }

        //Update statement
        public void Update()
        {
            //TODO
        }

        //Delete statement
        public void Delete()
        {
            //TODO
        }

        public double GetTimeCount(int? archiId, int? projectId, int? phaseId)
        {
            string where = "";
            if ((archiId != null) || (projectId != null) || (phaseId != null))
            {
                where += " WHERE ";
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
                    count += (double)dataReader["time"];
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
                    count += (double) dataReader["time"];
                }

                dataReader.Close();
                CloseConnection();
            }
            return count;
        }

        public string GetArchitectFullNameFromId(int id)
        {
            string query = "SELECT * FROM e_architect WHERE ID=\"" + id + "\"";
            string name = null;

            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    var fn = (string)dataReader["firstname"];
                    var ln = (string)dataReader["lastname"];
                    name = fn + " " + ln;
                }
                dataReader.Close();
                CloseConnection();
            }

            return name;
        }

        public string GetProjectNameFromId(int id)
        {
            string query = "SELECT * FROM e_project WHERE ID=\"" + id + "\"";
            string name = null;

            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    name = (string)dataReader["name"];
                }
                dataReader.Close();
                CloseConnection();
            }

            return name;
        }

        public string GetPhaseNameFromId(int id)
        {
            string query = "SELECT * FROM e_phase WHERE ID=\"" + id + "\"";
            string name = null;

            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    name = (string)dataReader["name"];
                }
                dataReader.Close();
                CloseConnection();
            }

            return name;
        }

        public string GetCompanyNameFromId(int id)
        {
            string query = "SELECT * FROM e_company WHERE ID=\"" + id + "\"";
            string name = null;

            if (OpenConnection())
            {
                var cmd = new MySqlCommand(query, _connection);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    name = (string) dataReader["name"];
                }
                dataReader.Close();
                CloseConnection();
            }

            return name;
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
