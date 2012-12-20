using System;
using System.IO;
using System.Xml;

namespace ProjectTime
{
    class Config
    {
        private readonly DbConnect _db;
        private uint? RunningDatabaseEntryId { get; set; }
        public Architect Architect { get; set; }
        public Project Project { get; set; }
        public Phase Phase { get; set; }
        public const string ConfigFileName = "config.xml";
        public string ConfigFilePathAndName { get; set; }
        public string RealTimeElapsed{ get; set; }
        
        
        public Config(DbConnect db)
        {
            _db = db;
            RunningDatabaseEntryId = null;
            Architect = null;
            Project = null;
            Phase = null;

            // ConfigFile should be saved in "Appdata" folder
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ProjectTime\\";
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Directory \"" + path + "\"does not exist, creating...");
                Directory.CreateDirectory(path);
            }
            ConfigFilePathAndName = Path.Combine(path, ConfigFileName);
        }

        // Last selection memorization
        public void SaveConfig(Architect archi, Project pro, Phase ph)
        {
            //Program.VarDump(archi);
            //Program.VarDump(pro);
            //Program.VarDump(ph);
            var writer = new XmlTextWriter(ConfigFilePathAndName, System.Text.Encoding.UTF8) { Formatting = Formatting.Indented };

            writer.WriteStartDocument(false);
            writer.WriteComment("Fichier de sauvegarde de la dernière configuration de ProjectTime.");
            writer.WriteStartElement("config");

            writer.WriteStartElement("architect");
            writer.WriteElementString("id", archi.Id.ToString());
            writer.WriteElementString("firstname", archi.FirstName);
            writer.WriteElementString("lastname", archi.LastName);
            writer.WriteElementString("company", _db.GetCompanyNameFromId(archi.Company));
            writer.WriteEndElement();

            writer.WriteStartElement("project");
            writer.WriteElementString("id", pro.Id.ToString());
            writer.WriteElementString("name", pro.Name);
            writer.WriteEndElement();

            writer.WriteStartElement("phase");
            writer.WriteElementString("id", ph.Id.ToString());
            writer.WriteElementString("name", ph.Name);
            writer.WriteEndElement();

            writer.WriteEndElement(); // End "config"

            //TODO
            if (false/*stillRunning*/)
            {
                writer.WriteStartElement("runningdatabaseentry");
                //myXmlTextWriter.WriteElementString("id", ?);
                writer.WriteEndElement();
            }

            writer.Flush();
            writer.Close();
        }

        public void LoadConfig()
        {
            if (!(File.Exists(ConfigFilePathAndName) && (new FileInfo(ConfigFilePathAndName).Length > 100)))
            {
                return;
            }

            var settings = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                IgnoreWhitespace = true,
                IgnoreComments = true
            };
            var reader = XmlReader.Create(ConfigFilePathAndName, settings);

            reader.Read();
            reader.ReadStartElement("config");

            reader.ReadStartElement("architect");
            var archiId = int.Parse(reader.ReadElementString("id"));
            reader.ReadElementString("firstname");
            reader.ReadElementString("lastname");
            reader.ReadElementString("company");
            Architect = RecordWindow.GetArchitectFromId(archiId);
            reader.ReadEndElement();

            reader.ReadStartElement("project");
            var projectId = int.Parse(reader.ReadElementString("id"));
            reader.ReadElementString("name");
            Project = RecordWindow.GetProjectFromId(projectId);
            reader.ReadEndElement();

            reader.ReadStartElement("phase");
            var phaseId = int.Parse(reader.ReadElementString("id"));
            reader.ReadElementString("name");
            Phase = RecordWindow.GetPhaseFromId(phaseId);
            reader.ReadEndElement();

            reader.ReadEndElement(); // End "config"

            //TODO
            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "runningdatabaseentry"))
                {
                    RunningDatabaseEntryId = uint.Parse(reader.ReadElementString("id"));
                }
            }

            reader.Close();
        }
    }
}
