using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ProjectTime
{
    static class Program
    {
        public static string DbServerIp = "82.240.213.167";
        public static string DbName = "he";
        public static string DbUserName = "he";
        public static string DbPassword = "mySqlUserPassword";
        public static DbConnection Db { get; set; }

        [STAThread]
        static void Main()
        {
            // Check if an Internet connection is available
            if (!Program.IsDatabaseConnexionAvailable(null))
            {
                MessageBox.Show(@"Vous devez être connecté à Internet pour ajouter des entrées dans la base de données.", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // Create time recording window and launch application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mainWindow = new RecordWindow
                                 {
                                     FormBorderStyle = FormBorderStyle.FixedSingle
                                 };
            Application.Run(mainWindow);
        }


        public static bool IsDatabaseConnexionAvailable(string serverAddress)
        {
            var server = serverAddress ?? Program.DbServerIp;
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
                    logFile.WriteLine(System.DateTime.Now + ": Remote database unreachable (" + e.Message + ")");
                }
                return false;
            }
            return rep != null && (rep.Status == System.Net.NetworkInformation.IPStatus.Success);
            
        }

        public static void VarDump(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException();
            }
            // If native (atomic) type
            if (obj.GetType().GetProperties().Length == 0)
            {
                Console.WriteLine("({0}) \"{1}\"", obj.GetType().ToString(), obj.ToString());
                return;
            }

            // Else if complex / user-defined type
            Console.WriteLine(obj.GetType().ToString());
            Console.WriteLine("{");
            PropertyInfo[] props = obj.GetType().GetProperties();
            foreach (PropertyInfo p in props)
            {
                try
                {
                    Console.WriteLine("  [\"{0}\"] => ({1}) \"{2}\"",
                        p.Name,
                        (p.GetValue(obj, null) != null) ? p.GetValue(obj, null).GetType().ToString() : "unknown",
                        p.GetValue(obj, null) ?? "null");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);   
                }
            }
            Console.WriteLine("}");
            Console.WriteLine();
        }
    }
}
