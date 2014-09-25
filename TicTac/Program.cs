using System;
using System.Reflection;
using System.Windows.Forms;


namespace TicTac
{
    static class Program
    {
        public static bool DatabaseConnexionAvailable { get; private set; }
        public const bool IsDBPrefStorageAvailable = false;
        public static WallClock clk;

        [STAThread]
        static void Main()
        {
            clk = new WallClock();

            // Check if an Internet connection is available
            CheckDatabaseConnexionAvailable(null);
            if (!DatabaseConnexionAvailable)
            {
                const string msg = @"Il est préférable d'être connecté à Internet pour utiliser TitTacTeam. Certaines fonctionnalités peuvent ne pas fonctionner correctement.";
                MessageBox.Show(msg, @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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


        public static bool CheckDatabaseConnexionAvailable(string serverAddress)
        {
            var server = serverAddress ?? Database.DbServerIp;
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
            return DatabaseConnexionAvailable;
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
            Console.WriteLine(@"{");
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
            Console.WriteLine(@"}");
            Console.WriteLine();
        }
    }
}
