﻿using MySql.Data.MySqlClient;
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

        // Static constructor
        static Database()
        {
            Abort = false;
        }

        public static bool CheckConnexion(string serverAddress, string dbName, string userName, string hPassword)
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
                Logger.Write(String.Format("Database authentication test failed ({0}): \"{1}\"", serverAddress, ex.Message));
                MessageBox.Show(ex.Message, @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            Logger.Write(String.Format("Database authentication test succeeded ({0})", serverAddress));
           
            DatabaseConnexionAvailable = true;
            LastServerChecked = serverAddress;

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
                ServerAddress != null && ServerAddress != string.Empty &&
                Name != null && Name != string.Empty &&
                UserName != null && UserName != string.Empty &&
                Password != null && Password != string.Empty;
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
    }
}
