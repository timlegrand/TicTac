using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ProjectTime
{
    static class Program
    {
        public static readonly string ServerIp = "82.240.213.167";

        [STAThread]
        static void Main()
        {
            // Create time recording window and launch application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var centerPoint = new Point(Screen.PrimaryScreen.WorkingArea.Width/2,
                                        Screen.PrimaryScreen.WorkingArea.Height/2);
            var mainWindow = new RecordWindow
                                 {
                                     FormBorderStyle = FormBorderStyle.FixedSingle
                                 };
            Application.Run(mainWindow);
        }


        public static bool IsDatabaseConnexionAvailable()
        {
            var req = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingReply rep;
            try
            {
                rep = req.Send(ServerIp);
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
                    Console.WriteLine("  [\"{0}\"] => ({1}) \"{2}\"", p.Name, p.GetValue(obj, null).GetType()/*"unknown"*/, p.GetValue(obj, null));
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
