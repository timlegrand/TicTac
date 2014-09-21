using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using TicTac.DAO;
using System.Windows.Forms;

namespace TicTac
{
    class Preferences
    {
        private const string Version = "0.7";
        public const string ConfigFileName = "preferences.xml";
        public static string PreferencesFilePathAndName { get; set; }

        public Point StartLocation { get; set; }
        public Point DefaultLocation { get; set; }
        public Architect LastArchitect { get; set; }
        public Project LastProject { get; set; }
        public Phase LastPhase { get; set; }
        private readonly RecordWindow _parent;

        // Constructor
        public Preferences(RecordWindow recordWindow)
        {
            _parent = recordWindow;

            DefaultLocation = new Point(Screen.PrimaryScreen.Bounds.Width / 2 - recordWindow.Width / 2,
                                        Screen.PrimaryScreen.Bounds.Height / 2 - recordWindow.Height / 2);
            StartLocation = DefaultLocation;
            LastArchitect = null;
            LastProject = null;
            LastPhase = null;

            // Preferences file should be saved in "AppData/Roaming" folder
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TicTac\\";
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Directory \"" + path + "\"does not exist, creating...");
                Directory.CreateDirectory(path);
            }
            PreferencesFilePathAndName = Path.Combine(path, ConfigFileName);
        }

        public void Load()
        {
            if (Program.IsDBPrefStorageAvailable)
            {
                LoadFromDatabase();
            }
            else
            {
                LoadFromXml();
            }
        }

        private void LoadFromDatabase()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            if (Program.IsDBPrefStorageAvailable)
            {
                SaveToDatabase();
            }
            else
            {
                SaveToXml();
            }
        }

        private void SaveToDatabase()
        {
            throw new NotImplementedException();
        }


        // Last selection memorization
        public void SaveToXml()
        {
            var writer = new XmlTextWriter(PreferencesFilePathAndName, Encoding.UTF8) { Formatting = Formatting.Indented };

            writer.WriteStartDocument(false);
            writer.WriteComment("Fichier de sauvegarde des préférences de TicTac.");
            writer.WriteStartElement("preferences");

            // Revision of current format of document
            writer.WriteAttributeString("version", Version);

            // Database info
            if (Database.DbPassword != null)
            {
                writer.WriteStartElement("database");
                writer.WriteAttributeString("name", Database.DbName);
                writer.WriteAttributeString("address", Database.DbServerIp);
                writer.WriteAttributeString("userName", Database.DbUserName);
                writer.WriteAttributeString("password", Database.DbPassword);
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
            writer.WriteAttributeString("x", _parent.Location.X.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("y", _parent.Location.Y.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            writer.WriteEndElement(); // End "preferences"

            writer.Flush();
            writer.Close();
        }

        public void LoadFromXml()
        {
            if (!(File.Exists(PreferencesFilePathAndName) && (new FileInfo(PreferencesFilePathAndName).Length > 100)))
            {
                Console.WriteLine(@"No Preferences file found or size too low (file seems to be empty).");
                return;
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
                return;
            }
#if (DEBUG)
            Console.WriteLine(@"Preferences file version " + version);
#endif

            // Database info
            if (reader.ReadToFollowing("database"))
            {
                Database.DbName = reader.GetAttribute("name");
                Database.DbServerIp = reader.GetAttribute("address");
                Database.DbUserName = reader.GetAttribute("userName");
                Database.DbPassword = reader.GetAttribute("password");
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
                BoundAndSetLocation(x, y);
            }

            reader.Close();
        }

        private void BoundAndSetLocation(int x, int y)
        {
            if (x != -1 && y != -1)
            {
                x = (x < Screen.PrimaryScreen.Bounds.Width - _parent.Width) ? x : Screen.PrimaryScreen.Bounds.Width - _parent.Width;
                x = (x >= 0) ? x : 0;
                y = (y < Screen.PrimaryScreen.Bounds.Height - _parent.Height) ? y : Screen.PrimaryScreen.Bounds.Height - _parent.Height;
                x = (x >= 0) ? x : 0;
                StartLocation = new Point(x, y);
            }
        }

        public bool IsValid()
        {
            return (StartLocation != null &&
                    LastArchitect != null && LastArchitect.IsValid() &&
                    LastProject != null && LastProject.IsValid() &&
                    LastPhase != null && LastPhase.IsValid());
        }
    }
}
