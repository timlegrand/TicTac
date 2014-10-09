using System;


namespace TicTac
{
    internal sealed class ConnectionPing
    {
        public static bool DatabaseConnexionAvailable { get; private set; }
        public static bool DatabaseConnexionChecked { get; private set; }
        public static string LastServerChecked { get; private set; }

        private static volatile ConnectionPing instance;
        private static object syncRoot = new Object();

        // Static constructor
        static ConnectionPing()
        {
            ConnectionPing.DatabaseConnexionAvailable = false;
            ConnectionPing.DatabaseConnexionChecked = false;
        }

        // Singleton getter & lazy constructor
        public static ConnectionPing Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ConnectionPing();
                    }
                }

                return instance;
            }
        }

        // Instance constructor
        private ConnectionPing()
        {
            CheckDatabaseConnexionAvailable("www.google.com");
        }

        public static bool CheckDatabaseConnexionAvailable(string serverAddress)
        {
            if (DatabaseConnexionChecked &&
                serverAddress != null && serverAddress != string.Empty &&
                LastServerChecked == serverAddress)
            {
                return DatabaseConnexionAvailable;
            }

            string server;
            if (serverAddress != null && serverAddress != string.Empty)
            {
                server = serverAddress;
            }
            else if(Database.ServerAddress != null && Database.ServerAddress != string.Empty)
            {
                server = Database.ServerAddress;
            }
            else
            {
                server = "www.google.com"; // Default
            }

            return CheckConnexion(server);
        }

        public static bool CheckConnexion(string serverAddress)
        {
            if (serverAddress == null || serverAddress == string.Empty)
            {
                throw new ArgumentNullException("serverAddress cannot be empty");
            }

            var server = serverAddress ?? Database.ServerAddress ?? "www.google.com";

            var req = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingReply rep;
            try
            {
                rep = req.Send(server);
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
            LastServerChecked = server;

            return DatabaseConnexionAvailable;
        }
    }
}
