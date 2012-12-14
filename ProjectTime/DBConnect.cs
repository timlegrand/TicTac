using System.Collections.Generic;
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

        //Insert statement
        public void Insert()
        {
            //TODO
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
                    var fn = (string) dataReader["firstname"];
                    var ln = (string) dataReader["lastname"];
                    var co = (string) dataReader["company"].ToString();

                    architects.Add(new Architect(fn, ln, co));
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
                    var n = (string)dataReader["name"];
                    projects.Add(new Project(n));
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
                    var n = (string)dataReader["name"];
                    phases.Add(new Phase(n));
                }

                dataReader.Close();
                CloseConnection();
            }
            return phases;
        }

        public int Count()
        {
            //TODO
            return 0;
        }
    }
}
