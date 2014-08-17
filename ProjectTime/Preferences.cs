using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using TicTac.DAO;

namespace TicTac
{
    class Preferences
    {
        private const string Version = "0.5";
        public const string ConfigFileName = "preferences.xml";
        public static string PreferencesFilePathAndName { get; set; }

        public Point? LastStartPosition { get; set; }
        public Architect LastArchitect { get; set; }
        public DbClient LastDb { get; set; }
        private readonly RecordWindow _parent;

        // Constructor
        public Preferences(RecordWindow recordWindow)
        {
            _parent = recordWindow;
            LastStartPosition = new Point(recordWindow.Location.X, recordWindow.Location.Y);
            LastArchitect = null;
            LastDb = null;

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
            if (Program.DatabasePreferencesAvailable)
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

        // Last selection memorization
        public void SaveToXml()
        {
            var writer = new XmlTextWriter(PreferencesFilePathAndName, Encoding.UTF8) { Formatting = Formatting.Indented };

            writer.WriteStartDocument(false);
            writer.WriteComment("Fichier de sauvegarde des préférences de TicTac.");
            writer.WriteStartElement("preferences");

            // Revision of current format of document
            writer.WriteAttributeString("version", Version);

            // Last working architect
            writer.WriteStartElement("architect");
            writer.WriteAttributeString("id", LastArchitect.Id.ToString());
            writer.WriteAttributeString("firstname", LastArchitect.FirstName);
            writer.WriteAttributeString("lastname", LastArchitect.LastName);
            writer.WriteAttributeString("company", LastArchitect.Company.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            // Last prefered window position
            writer.WriteStartElement("position");
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
            }
#if (DEBUG)
            Console.WriteLine(@"Preferences file version " + version);
#endif
            // Last working architect
            reader.ReadToFollowing("architect");
            var archiId = Int32.Parse(reader.GetAttribute("id") ?? "-1");
            var firstname = reader.GetAttribute("firstname") ?? "Unknown";
            var lastname = reader.GetAttribute("lastname") ?? "Unknown";
            var company = Int32.Parse(reader.GetAttribute("company") ?? "-1");
            LastArchitect = new Architect() { Id = archiId, FirstName = firstname, LastName = lastname, Company = company };

            // Last prefered window position
            reader.ReadToFollowing("position");
            var x = Int32.Parse(reader.GetAttribute("x") ?? "-1");
            var y = Int32.Parse(reader.GetAttribute("y") ?? "-1");
            LastStartPosition = (x == -1 || y == -1) ? (Point?) null : new Point(x,y);

            reader.Close();
        }

        public bool IsValid()
        {
            return LastStartPosition != null && LastArchitect != null; //&& LastDb != null 
        }
    }
}
