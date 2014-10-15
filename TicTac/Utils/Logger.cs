using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TicTac
{
    public static class Logger
    {
        public static readonly string FileName = "log.txt";
        public static string FilePathAndName { get; set; }

        static Logger()
        {
            // Log file should be saved in "AppData/Roaming" folder
            var path = Program.ApplicationDataFolder;
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Directory \"" + path + "\"does not exist, creating...");
                Directory.CreateDirectory(path);
            }
            FilePathAndName = Path.Combine(path, FileName);

            // Log file header
            Write("Logger started.");
            Write(String.Format("TicTac version: {0}", Program.CurrentVersion));

            
        }

        public static void Write(Exception ex)
        {
            Write(String.Format("{0}", ex.Message));
        }

        public static void Write(string message)
        {
            StringBuilder sb = new StringBuilder();

            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            string log = String.Format("{0}: {1}", now, message ?? "");

            sb.Append(log);

            using (StreamWriter outfile = new StreamWriter(FilePathAndName, true))
            {
                outfile.WriteLine(sb.ToString());
            }
        }

        public static void Print()
        {
            string text = System.IO.File.ReadAllText(FilePathAndName);
            Console.WriteLine(text);
        }
    }
}
