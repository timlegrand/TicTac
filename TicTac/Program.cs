using System;
using System.Deployment.Application;
using System.Reflection;
using System.Windows.Forms;


namespace TicTac
{
    internal class AbortSignalException : Exception { }

    static class Program
    {
        public static string ApplicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TicTac\\";
        public const bool IsDBPrefStorageAvailable = false;
        public static WallClock clk;

        public static string CurrentVersion
        {
            get
            {
                string v = ApplicationDeployment.IsNetworkDeployed
                       ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                       : Assembly.GetExecutingAssembly().GetName().Version.ToString();

                // Remove extra build version
                if (v.Split('.').Length == 4 && v[v.Length - 2] == '.' && v[v.Length - 1] == '0')
                {
                    v = v.Substring(0, v.Length - 2);
                }

                return v;
            }
        }

        [STAThread]
        static void Main()
        {
            // Prologue
            clk = new WallClock();

            // Create time recording window and launch application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RecordWindow mainWindow;
            try
            {
                mainWindow = new RecordWindow
                                 {
                                     FormBorderStyle = FormBorderStyle.FixedSingle
                                 };
                Application.Run(mainWindow);
            }
            catch (AbortSignalException)
            {
                Logger.Write("Abort signal raised");
                Application.Exit();
            }

            // Epilogue
            clk.Print();
            Logger.Write("PROGRAM END");
            Logger.Print();
        }
    }
}
