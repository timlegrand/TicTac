using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;


namespace TicTac
{
    internal static class Database
    {
        public static string ServerAddress = "";
        public static string Name = "";
        public static string UserName = "";
        public static string Password = "";

        public static bool DatabaseConnexionAvailable { get; private set; }
        public static bool DatabaseConnexionChecked { get; private set; }
        public static string LastServerChecked { get; private set; }

        public static bool Abort { get; set; }

        // Static constructor
        static Database()
        {
            Abort = false;
        }

        public static bool CheckServerReachable(string serverAddress)
        {
            if (DatabaseConnexionChecked &&
                serverAddress != null && serverAddress != string.Empty &&
                LastServerChecked == serverAddress)
            {
                return DatabaseConnexionAvailable;
            }

            string server = "";
            if (serverAddress != null && serverAddress != string.Empty)
            {
                server = serverAddress;
            }
            else if(Database.ServerAddress != null && Database.ServerAddress != string.Empty)
            {
                server = Database.ServerAddress;
            }

            return CheckConnexion(server);
        }

        public static bool CheckConnexion(string serverAddress)
        {
            if (serverAddress == null)
            {
                throw new ArgumentNullException("serverAddress cannot be empty");
            }

            if (serverAddress == string.Empty)
            {
                return false;
            }

            var req = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingReply rep;
            try
            {
                rep = req.Send(serverAddress);
            }
            catch (System.Net.NetworkInformation.PingException e)
            {
                using (var logFile = new System.IO.StreamWriter("log.txt", true))
                {
                    logFile.WriteLine(DateTime.Now + ": Remote database unreachable (" + e.Message + ")");
                }
                return false;
            }

            DatabaseConnexionAvailable = (rep != null && (rep.Status == System.Net.NetworkInformation.IPStatus.Success));
            LastServerChecked = serverAddress;

            return DatabaseConnexionAvailable;
        }

        public static bool CheckDatabaseConnexion(string serverAddress, string dbName, string userName, string hPassword)
        {
            if (serverAddress == null)
            {
                throw new ArgumentNullException("serverAddress cannot be empty");
            }

            if (DatabaseConnexionChecked &&
                serverAddress != null && serverAddress != string.Empty &&
                LastServerChecked == serverAddress)
            {
                return DatabaseConnexionAvailable;
            }

            if (serverAddress == string.Empty)
            {
                return false;
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
                MessageBox.Show(ex.Message, @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
           
            DatabaseConnexionAvailable = true;
            LastServerChecked = serverAddress;

            return DatabaseConnexionAvailable;
        }

        internal static bool CheckDatabaseConnexion(string serverAddress)
        {
            return CheckDatabaseConnexion(serverAddress, Database.Name, Database.UserName, Database.Password);
        }

        public static bool IsValid()
        {
            return
                ServerAddress != null && ServerAddress != string.Empty &&
                Name != null && Name != string.Empty &&
                UserName != null && UserName != string.Empty &&
                Password != null && Password != string.Empty;
        }


        internal static void WaitForConnectivity()
        {
            
            while (!CheckDatabaseConnexion(ServerAddress) && !Abort)
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
    }
}
