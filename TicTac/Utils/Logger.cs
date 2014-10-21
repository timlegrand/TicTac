using System;
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

            // If log file size is greater than limit, backup and restart logging
            if (File.Exists(FilePathAndName) && new FileInfo(FilePathAndName).Length > 1000000)
            {
                string backupName = FilePathAndName + ".old";
                if (File.Exists(backupName))
                {
                    File.Delete(backupName);
                }

                File.Move(FilePathAndName, backupName);
            }

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
            var sb = new StringBuilder();

            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            string log = String.Format("{0}: {1}", now, message ?? "");

            sb.Append(log);

            using (var outfile = new StreamWriter(FilePathAndName, true))
            {
                outfile.WriteLine(sb.ToString());
            }
        }

        public static void Print()
        {
            string text = File.ReadAllText(FilePathAndName);
            Console.WriteLine(text);
        }
    }
}
