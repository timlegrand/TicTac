using System;
using System.Windows.Forms;


namespace TicTac
{
    static class Program
    {
        public static string ApplicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TicTac\\";
        public const bool IsDBPrefStorageAvailable = false;
        public static WallClock clk;

        [STAThread]
        static void Main()
        {
            clk = new WallClock();
            Program.clk.Probe("PROGRAM START");

            // Check if an Internet connection is available
            ConnectionPing.CheckDatabaseConnexionAvailable(null);
            if (!ConnectionPing.DatabaseConnexionAvailable)
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
    }
}
