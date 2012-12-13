using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

// For serialization
using System.Runtime.Serialization.Formatters.Binary;

// For file access
using System.IO;
using MySql.Data.MySqlClient;
namespace ProjectTime
{
    static class Program
    {
        public static string ConnexionString = "Database=he;DataSource=192.168.0.1;UserId=he;Password=mySqlUserPassword";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var con = new MySqlConnection(ConnexionString);
            try
            {
                con.Open(); // connection must be openned for command
                
                var cmd = new MySqlCommand("select * from e_architect", con);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetString("id") + ": " + reader.GetString("firstname"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: "+ex);
            }
            finally
            {
                con.Close();
            }
            return;
            // Use serialization to files until real DB
            var bformatter = new BinaryFormatter();

            // Simulation of DB of Architects
            var architectsDb = new List<Architect>();
            architectsDb.Add(new Architect("Marie", "Hascoët", "Histoire d'Espace"));
            architectsDb.Add(new Architect("Delphine", "Aillaud", "Delphine Aillaud Architecte"));
            architectsDb.Add(new Architect("Vincent", "Gourvil", "O2 Concept Architecture"));
            architectsDb.Add(new Architect("Hugues", "Launay", "Couasnon et Launay Architectes"));

            // Serialization of DB Architects
            using(var architectsFile = File.Open("Architects.osl", FileMode.Create))
            {
                bformatter.Serialize(architectsFile, architectsDb);
            }
         
            // De-serialization of DB Architects
            List<Architect> architects;
            using(var architectsFile = File.Open("Architects.osl", FileMode.Open))
            {
                architects = (List<Architect>)bformatter.Deserialize(architectsFile);
            }
          

            // Simulation of DB of Projects
            var projectsDb = new List<Project>();
            projectsDb.Add(new Project("Plédran"));
            projectsDb.Add(new Project("Onno"));
            projectsDb.Add(new Project("Rondeau"));
            projectsDb.Add(new Project("BH"));
            projectsDb.Add(new Project("Leprètre"));

            // Serialization of DB Projects
            using(var projectsFile = File.Open("Projects.osl", FileMode.Create))
            {
                bformatter.Serialize(projectsFile, projectsDb);
            }

            // De-serialization of DB Projects
            List<Project> projects;
            using(var projectsFile = File.Open("Projects.osl", FileMode.Open))
            {
                projects = (List<Project>)bformatter.Deserialize(projectsFile);
            }
            

            // Simulation of DB of Phases
            var phasesDb = new List<Phase>();
            phasesDb.Add(new Phase("Esquisse"));
            phasesDb.Add(new Phase("APS/APD"));
            phasesDb.Add(new Phase("PC"));
            phasesDb.Add(new Phase("Pro"));
            phasesDb.Add(new Phase("DCE"));
            phasesDb.Add(new Phase("AO"));
            phasesDb.Add(new Phase("Chantier"));

            // Serialization of DB Phases
            using(var phasesFile = File.Open("Phases.osl", FileMode.Create))
            {
                bformatter.Serialize(phasesFile, phasesDb);
            }

            // De-serialization of DB Phases
            List<Phase> phases;
            using(var phasesFile = File.Open("Phases.osl", FileMode.Open))
            {
                phases = (List<Phase>)bformatter.Deserialize(phasesFile);
            }


            // Create time recording window and launch application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mainWindow = new RecordWindow(architects, projects, phases);
            mainWindow.FormBorderStyle = FormBorderStyle.FixedSingle;
            Application.Run(mainWindow);
        }

        public static bool IsInternetConnexionAvailable()
        {
            var req = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingReply rep;
            try
            {
                rep = req.Send("www.google.com");
            }
            catch (System.Net.NetworkInformation.PingException e)
            {
                using (var logFile = new System.IO.StreamWriter("log.txt", true))
                {
                    logFile.WriteLine(System.DateTime.Now + ": User tried to add an entry to the database but no Internet was available.");
                }
                return false;
            }
            return rep != null && (rep.Status == System.Net.NetworkInformation.IPStatus.Success);
            
        }

        public static void VarDump(object obj)
        {
            /*if ( obj.ToString() != string.Empty )
            {
                Console.Write(obj.ToString());
                return;
            }*/

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
