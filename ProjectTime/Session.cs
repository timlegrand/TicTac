using System;
using System.IO;
using System.Xml;

namespace ProjectTime
{
    class Session
    {
        private const string Version = "0.2";
        public const string SessionFileName = "session.xml";
        public static string SessionFilePathAndName { get; set; }

        public Architect Architect { get; set; }
        public Project Project { get; set; }
        public Phase Phase { get; set; }
        public uint? RunningSessionId { get; set; }
        public DateTime StartTime { get; set; } //in DateTime format
        public DateTime StopTime { get; set; } //in DateTime format

        public Session()
        {
            Architect = null;
            Project = null;
            Phase = null;
            RunningSessionId = null;
            StartTime = DateTime.MinValue;
            StopTime = DateTime.MinValue;

            // ConfigFile should be saved in "AppData" folder
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ProjectTime\\";
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Directory \"" + path + "\"does not exist, creating...");
                Directory.CreateDirectory(path);
            }
            SessionFilePathAndName = Path.Combine(path, SessionFileName);
        }
        /*
        public bool Validate()
        {
            if (Architect.Id == null || Project.Id == null || Phase.Id == null)
            {
                return false;
            }
            Architect = _db.GetArchitectFromId((int)Architect.Id);
            Project = _db.GetProjectFromId((int)Project.Id);
            Phase = _db.GetPhaseFromId((int)Phase.Id);
            return IsValid();
        }
        */
        public bool IsValid()
        {
            return (Architect != null && Project != null && Phase != null);
        }

        public bool IsTerminated()
        {
            return (StartTime != DateTime.MinValue && StopTime != DateTime.MinValue);
        }

        /*
        public static Session Load()
        {
            if (Program.IsDatabaseConnexionAvailable())
            {
                return LoadFromDatabase();
            }
            else
            {
                return LoadFromXml();
            }
        }*/

        /*
        private static Session LoadFromDatabase()
        {
            var sessions = _db.StartedWorkSessions();
            if (sessions.Count == 1)
            {
                return sessions[0];
            }
            throw new Exception();
        }*/

        /*
        // Last selection memorization
        public void SaveToXml()
        {
            var writer = new XmlTextWriter(SessionFilePathAndName, System.Text.Encoding.UTF8) { Formatting = Formatting.Indented };

            writer.WriteStartDocument(false);
            writer.WriteComment("Fichier de sauvegarde de la dernière configuration de ProjectTime.");
            writer.WriteStartElement("config");
            writer.WriteAttributeString("version", "0.1");

            writer.WriteStartElement("architect");
            writer.WriteAttributeString("id", Architect.Id.ToString());
            writer.WriteAttributeString("firstname", Architect.FirstName);
            writer.WriteAttributeString("lastname", Architect.LastName);
            writer.WriteAttributeString("company", _db.GetCompanyFromId(Architect.Company).Name);
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

        public Session LoadFromXml()
        {
            if (!(File.Exists(SessionFilePathAndName) && (new FileInfo(SessionFilePathAndName).Length > 100)))
            {
                Console.WriteLine(@"No Session file found or size too low (seems to be empty).");
                return null;
            }

            var settings = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                IgnoreWhitespace = true,
                IgnoreComments = true
            };
            var reader = XmlReader.Create(SessionFilePathAndName, settings);

            reader.Read();
            reader.ReadToFollowing("config");
            var version = reader.GetAttribute("version");
            if (version != Version)
            {
                Console.WriteLine(@"Session file version (" + version + @") doesn't match with current supported version (" + Version + @").");
                reader.Close();
                return null;
            }
            Console.WriteLine(@"Session file version " + version);

            reader.ReadToFollowing("architect");
            var archiId = int.Parse(reader.GetAttribute("id") ?? "-1");
            //reader.GetAttribute("firstname");
            //reader.GetAttribute("lastname");
            //reader.GetAttribute("company");
            Architect = RecordWindow.GetArchitectFromId(archiId);

            reader.ReadToFollowing("project");
            var projectId = int.Parse(reader.GetAttribute("id") ?? "-1");
            reader.GetAttribute("name");
            Project = RecordWindow.GetProjectFromId(projectId);

            reader.ReadToFollowing("phase");
            var phaseId = int.Parse(reader.GetAttribute("id") ?? "-1");
            reader.GetAttribute("name");
            Phase = RecordWindow.GetPhaseFromId(phaseId);

            reader.Close();

            return new Session(_db)
                        {
                            Architect = _db.GetArchitectFromId(archiId),
                            Project = _db.GetProjectFromId(projectId),
                            Phase = _db.GetPhaseFromId(phaseId)
                        };
        }
        */
    }
}
