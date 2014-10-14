using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;


namespace TicTac
{
    public class BackupPreferences : Preferences
    {
        public static readonly string PreferencesFileName = "preferences.xml";
        public static string PreferencesFilePathAndName { get; set; }
        private static readonly new string Version = "0.7";

        static BackupPreferences()
        {
            // Preferences file should be saved in "AppData/Roaming" folder
            var path = Program.ApplicationDataFolder;
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Directory \"" + path + "\"does not exist, creating...");
                Directory.CreateDirectory(path);
            }
            PreferencesFilePathAndName = Path.Combine(path, PreferencesFileName);
        }

        // Constructors
        public BackupPreferences() : base() {}
        public BackupPreferences(RecordWindow recordWindow) : base(recordWindow) {}

        public override void Save()
        {
#if (DEBUG)
            Console.WriteLine(@"Writing " + PreferencesFileName + @"...");
#endif
            var writer = new XmlTextWriter(PreferencesFilePathAndName, Encoding.UTF8) { Formatting = Formatting.Indented };

            writer.WriteStartDocument(false);
            writer.WriteComment("Fichier de sauvegarde des préférences de TicTac.");
            writer.WriteStartElement("preferences");

            // Revision of current format of document
            writer.WriteAttributeString("version", Version);

            // Database info
            if (Database.Password != null)
            {
                writer.WriteStartElement("database");
                writer.WriteAttributeString("name", Database.Name);
                writer.WriteAttributeString("address", Database.ServerAddress);
                writer.WriteAttributeString("userName", Database.UserName);
                writer.WriteAttributeString("password", Database.Password);
                writer.WriteEndElement();
            }

            // Last working architect
            if (LastArchitect != null && LastArchitect.IsValid())
            {
                writer.WriteStartElement("architect");
                writer.WriteAttributeString("id", LastArchitect.Id.ToString());
                writer.WriteAttributeString("firstname", LastArchitect.FirstName);
                writer.WriteAttributeString("lastname", LastArchitect.LastName);
                writer.WriteAttributeString("company", LastArchitect.Company.ToString(CultureInfo.InvariantCulture));
                writer.WriteEndElement();
            }

            // Last project
            if (LastProject != null && LastProject.IsValid())
            {
                writer.WriteStartElement("project");
                writer.WriteAttributeString("id", LastProject.Id.ToString());
                writer.WriteAttributeString("name", LastProject.Name);
                writer.WriteAttributeString("description", LastProject.Description);
                writer.WriteEndElement();
            }

            // Last phase
            if (LastPhase != null && LastPhase.IsValid())
            {
                writer.WriteStartElement("phase");
                writer.WriteAttributeString("id", LastPhase.Id.ToString());
                writer.WriteAttributeString("name", LastPhase.Name);
                writer.WriteAttributeString("description", LastPhase.Description);
                writer.WriteEndElement();
            }

            // Last prefered window location
            writer.WriteStartElement("location");
            writer.WriteAttributeString("x", StartLocation.X.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("y", StartLocation.Y.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            writer.WriteEndElement(); // End "preferences"

            writer.Flush();
            writer.Close();
#if (DEBUG)
            Console.WriteLine(PreferencesFileName + @" written.");
#endif
        }

        public override void Load()
        {
            if (!(File.Exists(PreferencesFilePathAndName) && (new FileInfo(PreferencesFilePathAndName).Length > 100)))
            {
                throw new FileLoadException(@"No Preferences file found or size too low (file seems to be empty).");
            }

            var settings = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                IgnoreWhitespace = true,
                IgnoreComments = true
            };
            var reader = XmlReader.Create(PreferencesFilePathAndName, settings);

            reader.Read();
            reader.ReadToFollowing("preferences");

            // Revision of current format of document
            var version = reader.GetAttribute("version");
            if (version != Version)
            {
                Console.WriteLine(@"Cannot read preferences file (version " + version + @") doesn't match with current supported version (" + Version + @").");
                Console.WriteLine(@"File is going to be overwritten.");
                //Console.WriteLine(@"Please edit file manually or simply delete it (no worries, it will appear again at next launch). You may find it at '" + PreferencesFilePathAndName + @"'.");
                reader.Close();

                throw new Exception(@"Cannot read preferences file (version " + version + @") doesn't match with current supported version (" + Version + @").");
            }
#if (DEBUG)
            Console.WriteLine(@"Preferences file version " + version);
#endif

            // Database info
            if (reader.ReadToFollowing("database"))
            {
                Database.Name = reader.GetAttribute("name");
                Database.ServerAddress = reader.GetAttribute("address");
                Database.UserName = reader.GetAttribute("userName");
                Database.Password = reader.GetAttribute("password");
            }

            // Last working architect
            if (reader.ReadToFollowing("architect"))
            {
                var id = Int32.Parse(reader.GetAttribute("id") ?? "-1");
                var firstname = reader.GetAttribute("firstname") ?? "Unknown";
                var lastname = reader.GetAttribute("lastname") ?? "Unknown";
                var company = Int32.Parse(reader.GetAttribute("company") ?? "-1");
                LastArchitect = new Architect() { Id = id, FirstName = firstname, LastName = lastname, Company = company };
            }

            // Last project
            if (reader.ReadToFollowing("project"))
            {
                var id = Int32.Parse(reader.GetAttribute("id") ?? "-1");
                var name = reader.GetAttribute("name") ?? "Unknown";
                var description = reader.GetAttribute("description") ?? "Unknown";
                LastProject = new Project() { Id = id, Name = name, Description = description };
            }

            // Last phase
            if (reader.ReadToFollowing("phase"))
            {
                var id = Int32.Parse(reader.GetAttribute("id") ?? "-1");
                var name = reader.GetAttribute("name") ?? "Unknown";
                var description = reader.GetAttribute("description") ?? "Unknown";
                LastPhase = new Phase() { Id = id, Name = name, Description = description };
            }

            // Last prefered window location
            if (reader.ReadToFollowing("location"))
            {
                var x = Int32.Parse(reader.GetAttribute("x") ?? "-1");
                var y = Int32.Parse(reader.GetAttribute("y") ?? "-1");
                StartLocation = BoundLocation(x, y);
            }

            reader.Close();
        }

        public override bool IsValid()
        {
            return (StartLocation != null &&
                    LastArchitect != null && LastArchitect != null &&
                    LastProject != null && LastProject != null &&
                    LastPhase != null && LastPhase != null);
        }
    }
}
