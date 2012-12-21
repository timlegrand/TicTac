using System;
using System.IO;
using System.Xml;

namespace ProjectTime
{
    class Config
    {
        private const string Version = "0.1";
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
            RealTimeElapsed = "";

            // ConfigFile should be saved in "Appdata" folder
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ProjectTime\\";
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Directory \"" + path + "\"does not exist, creating...");
                Directory.CreateDirectory(path);
            }
            ConfigFilePathAndName = Path.Combine(path, ConfigFileName);
        }

        public bool IsValid()
        {
            return (Architect != null && Project != null && Phase != null);
        }

        // Last selection memorization
        public void SaveToXml()
        {
            var writer = new XmlTextWriter(ConfigFilePathAndName, System.Text.Encoding.UTF8) { Formatting = Formatting.Indented };

            writer.WriteStartDocument(false);
            writer.WriteComment("Fichier de sauvegarde de la dernière configuration de ProjectTime.");
            writer.WriteStartElement("config");
            writer.WriteAttributeString("version", "0.1");

            writer.WriteStartElement("architect");
            writer.WriteAttributeString("id", Architect.Id.ToString());
            writer.WriteAttributeString("firstname",Architect.FirstName);
            writer.WriteAttributeString("lastname", Architect.LastName);
            writer.WriteAttributeString("company", _db.GetCompanyNameFromId(Architect.Company));
            writer.WriteEndElement();

            writer.WriteStartElement("project");
            writer.WriteAttributeString("id", Project.Id.ToString());
            writer.WriteAttributeString("name", Project.Name);
            writer.WriteEndElement();

            writer.WriteStartElement("phase");
            writer.WriteAttributeString("id", Phase.Id.ToString());
            writer.WriteAttributeString("name", Phase.Name);
            writer.WriteEndElement();

            writer.WriteEndElement(); // End "config"

            writer.Flush();
            writer.Close();
        }

        public void LoadFromXml()
        {
            if (!(File.Exists(ConfigFilePathAndName) && (new FileInfo(ConfigFilePathAndName).Length > 100)))
            {
                Console.WriteLine(@"No config file found or size too low (seems to be empty).");
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
            reader.ReadToFollowing("config");
            var version = reader.GetAttribute("version");
            if (version != Config.Version)
            {
                Console.WriteLine(@"Config file version (" + version + ") doesn't match with current supported version (" + Config.Version + ").");
                reader.Close();
                return;
            }
            Console.WriteLine(@"Config file version " + version);

            reader.ReadToFollowing("architect");
            var archiId = int.Parse(reader.GetAttribute("id"));
            //reader.GetAttribute("firstname");
            //reader.GetAttribute("lastname");
            //reader.GetAttribute("company");
            Architect = RecordWindow.GetArchitectFromId(archiId);

            reader.ReadToFollowing("project");
            var projectId = int.Parse(reader.GetAttribute("id"));
            reader.GetAttribute("name");
            Project = RecordWindow.GetProjectFromId(projectId);

            reader.ReadToFollowing("phase");
            var phaseId = int.Parse(reader.GetAttribute("id"));
            reader.GetAttribute("name");
            Phase = RecordWindow.GetPhaseFromId(phaseId);

            reader.Close();
        }
    }
}
