using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;


namespace TicTac
{
    public static class Database
    {
        public static string ServerAddress = "";
        public static string Name = "";
        public static string UserName = "";
        public static string Password = "";

        public static bool DatabaseConnexionAvailable { get; private set; }
        public static bool DatabaseConnexionChecked { get; private set; }
        public static string LastServerChecked { get; private set; }

        public static bool Abort { get; set; }

        private static object syncRoot = new Object();

        // Static constructor
        static Database()
        {
            Abort = false;
        }

        public static bool CheckConnexion(string serverAddress, string dbName, string userName, string hPassword)
        {
            if (serverAddress == null || dbName == null || userName == null || hPassword == null)
            {
                throw new ArgumentNullException();
            }
            else if (serverAddress == String.Empty || dbName == String.Empty || userName == String.Empty || hPassword == String.Empty)
            {
                return false;
            }

            // Prevent from redundant, simultaneous requests from other threads
            lock (syncRoot)
            {
                if (DatabaseConnexionChecked && LastServerChecked == serverAddress)
                {
                    return DatabaseConnexionAvailable;
                }

                string _connectionString =
                    "SERVER='" + serverAddress + "';" +
                    "DATABASE='" + dbName + "';" +
                    "UID='" + userName + "';" +
                    "PASSWORD='" + hPassword + "';";
                var _connection = new MySqlConnection(_connectionString);

                try
                {
                    _connection.Open();
                    _connection.Close();
                }
                catch (MySqlException ex)
                {
                    Logger.Write(String.Format("Database authentication test failed ({0}): \"{1}\"", serverAddress, ex.Message));
                    MessageBox.Show(ex.Message, @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                Logger.Write(String.Format("Database authentication test succeeded ({0})", serverAddress));

                DatabaseConnexionChecked = true;
                DatabaseConnexionAvailable = true;
                LastServerChecked = serverAddress;
            }

            return DatabaseConnexionAvailable;
        }

        public static bool CheckConnexion(string serverAddress)
        {
            return CheckConnexion(serverAddress, Database.Name, Database.UserName, Database.Password);
        }

        public static bool CheckConnexion()
        {
            return CheckConnexion(Database.ServerAddress);
        }

        public static bool IsValid()
        {
            return
                !string.IsNullOrEmpty(ServerAddress) &&
                !string.IsNullOrEmpty(Name) &&
                !string.IsNullOrEmpty(UserName) &&
                !string.IsNullOrEmpty(Password);
        }


        public static void WaitForConnectivity()
        {
            while (!CheckConnexion(ServerAddress) && !Abort)
            {
                // Feed Database properties
                var configureForm = new ConfigureDatabase() { FormBorderStyle = FormBorderStyle.FixedSingle };
                configureForm.ShowDialog();
            }

            if (Database.Abort)
            {
                throw new AbortSignalException();
            }
        }

        public new static string ToString()
        {
            string s = String.Format("{0}, {1}, {2}, {3}",
                ServerAddress, Name, UserName, Password);
            return s;
        }

    }
}
