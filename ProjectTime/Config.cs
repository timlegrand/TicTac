using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using TicTac.DAO;

namespace TicTac
{
    class Config
    {
        private const string Version = "0.4";
        public const string ConfigFileName = "config.xml";
        public static string ConfigFilePathAndName { get; set; }

        public Point? LastStartPosition { get; set; }
        public Architect LastArchitect { get; set; }
        public DbClient LastDb { get; set; }
        private readonly RecordWindow _parent;

        public Config(RecordWindow recordWindow)
        {
            _parent = recordWindow;
            LastStartPosition = new Point(recordWindow.Location.X, recordWindow.Location.Y);
            LastArchitect = null;
            LastDb = null;

            // ConfigFile should be saved in "AppData" folder
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TicTac\\";
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Directory \"" + path + "\"does not exist, creating...");
                Directory.CreateDirectory(path);
            }
            ConfigFilePathAndName = Path.Combine(path, ConfigFileName);
        }

        public bool Load()
        {
            return Program.IsDatabaseConfigAvailable() ? LoadFromDatabase() : LoadFromXml();
        }

        private bool LoadFromDatabase()
        {
            throw new NotImplementedException();
        }

        // Last selection memorization
        public void SaveToXml()
        {
            var writer = new XmlTextWriter(ConfigFilePathAndName, Encoding.UTF8) { Formatting = Formatting.Indented };

            writer.WriteStartDocument(false);
            writer.WriteComment("Fichier de sauvegarde de la dernière configuration de TicTac.");
            writer.WriteStartElement("config");
            writer.WriteAttributeString("version", Version);

            writer.WriteStartElement("architect");
            writer.WriteAttributeString("id", LastArchitect.Id.ToString());
            writer.WriteAttributeString("firstname", LastArchitect.FirstName);
            writer.WriteAttributeString("lastname", LastArchitect.LastName);
            writer.WriteAttributeString("company", LastDb.SelectCompanyFromId(LastArchitect.Company).Name);
            writer.WriteEndElement();

            writer.WriteStartElement("position");
            writer.WriteAttributeString("x", _parent.Location.X.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("y", _parent.Location.Y.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();
            
            
            writer.WriteStartElement("database");
            writer.WriteAttributeString("server", LastDb.Server);
            writer.WriteAttributeString("database", LastDb.Database);
            writer.WriteAttributeString("uid", LastDb.Uid);
            writer.WriteAttributeString("password", LastDb.Password);
            writer.WriteEndElement();

            writer.WriteEndElement(); // End "config"

            writer.Flush();
            writer.Close();
        }

        public bool LoadFromXml()
        {
            if (!(File.Exists(ConfigFilePathAndName) && (new FileInfo(ConfigFilePathAndName).Length > 100)))
            {
                Console.WriteLine(@"No Session file found or size too low (file seems to be empty).");
                return false;
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
            if (version != Version)
            {
                Console.WriteLine(@"Session file version (" + version + @") doesn't match with current supported version (" + Version + @").");
                reader.Close();
                return false;
            }
            //Console.WriteLine(@"Session file version " + version);

            reader.ReadToFollowing("architect");
            var archiId = Int32.Parse(reader.GetAttribute("id") ?? "-1");
            //TODO: if no internet connection available
            //reader.GetAttribute("firstname");
            //reader.GetAttribute("lastname");
            //reader.GetAttribute("company");
            //TODO: else configure DAO BEFORE USING IT
            //...

            reader.ReadToFollowing("position");
            var x = Int32.Parse(reader.GetAttribute("x") ?? "-1");
            var y = Int32.Parse(reader.GetAttribute("y") ?? "-1");
            LastStartPosition = (x == -1 || y == -1) ? (Point?) null : new Point(x,y);

            reader.ReadToFollowing("database");
            var server =    reader.GetAttribute("server");
            var database =  reader.GetAttribute("database");
            var uid =       reader.GetAttribute("uid");
            var password =  reader.GetAttribute("password");
            LastDb = new DbClient
                {
                    Server = server,
                    Database = database,
                    Uid = uid,
                    Password = password
                };

            // Now DAO is initialized, we can use it
            LastArchitect = LastDb.SelectArchitectFromId(archiId);

            reader.Close();

            return true;
        }

        public bool IsValid()
        {
            return LastStartPosition != null && LastDb != null && LastArchitect != null;
        }
    }
}
