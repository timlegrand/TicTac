using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

// For serialization
using System.Runtime.Serialization.Formatters.Binary;

// For file access
using System.IO;

namespace ProjectTime
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Use serialization to files until real DB
            BinaryFormatter bformatter = new BinaryFormatter();

            // Simulation of DB of Architects
            var architectsDb = new List<Architect>();
            architectsDb.Add(new Architect("Marie", "Hascoët", "Histoire d'Espace"));
            architectsDb.Add(new Architect("Delphine", "Aillaud", "Delphine Aillaud Architecte"));
            architectsDb.Add(new Architect("Vincent", "Gourvil", "O2 Concept Architecture"));
            architectsDb.Add(new Architect("Hugues", "Launay", "Couasnon et Launay Architectes"));

            // Serialization of DB Architects
            using(var architectsFile = File.Open("Architects.osl", FileMode.Create))
            {
                //Console.WriteLine("Ecriture de la table des architectes (dans un fichier)");
                bformatter.Serialize(architectsFile, architectsDb);
            }
         
            // De-serialization of DB Architects
            List<Architect> architects;
            using(var architectsFile = File.Open("Architects.osl", FileMode.Open))
            {
                //Console.WriteLine("Lecture de la table des architectes (depuis un fichier)");
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
                //Console.WriteLine("Ecriture de la table des projets (dans un fichier)");
                bformatter.Serialize(projectsFile, projectsDb);
            }

            // De-serialization of DB Projects
            List<Project> projects;
            using(var projectsFile = File.Open("Projects.osl", FileMode.Open))
            {
                //Console.WriteLine("Lecture de la table des projets (depuis un fichier)");
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
                //Console.WriteLine("Ecriture de la table des phases (dans un fichier)");
                bformatter.Serialize(phasesFile, phasesDb);
            }

            // De-serialization of DB Phases
            List<Phase> phases;
            using(var phasesFile = File.Open("Phases.osl", FileMode.Open))
            {
                //Console.WriteLine("Lecture de la table des phases (depuis un fichier)");
                phases = (List<Phase>)bformatter.Deserialize(phasesFile);
            }


            // Create time recording window and launch application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mainWindow = new RecordWindow(architects, projects, phases);
            mainWindow.FormBorderStyle = FormBorderStyle.FixedSingle;
            Application.Run(mainWindow);
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
