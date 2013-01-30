using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ProjectTime
{
    class Config
    {
        private const string Version = "0.3";
        public const string ConfigFileName = "config.xml";
        public static string ConfigFilePathAndName { get; set; }

        public Point? StartPosition { get; set; }
        public Architect Architect { get; set; }
        public DbConnection Db { get; set; }
        private readonly RecordWindow _parent;

        public Config(RecordWindow recordWindow)
        {
            _parent = recordWindow;
            StartPosition = new Point(recordWindow.Location.X, recordWindow.Location.Y);
            Architect = null;
            Db = null;

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
            writer.WriteAttributeString("id", Architect.Id.ToString());
            writer.WriteAttributeString("firstname", Architect.FirstName);
            writer.WriteAttributeString("lastname", Architect.LastName);
            writer.WriteAttributeString("company", Db.GetCompanyFromId(Architect.Company).Name);
            writer.WriteEndElement();

            writer.WriteStartElement("position");
            writer.WriteAttributeString("x", _parent.Location.X.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("y", _parent.Location.Y.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();
            
            
            writer.WriteStartElement("database");
            writer.WriteAttributeString("server", Db.Server);
            writer.WriteAttributeString("database", Db.Database);
            writer.WriteAttributeString("uid", Db.Uid);
            writer.WriteAttributeString("password", Db.Password);
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
            //TODO: else configure Db BEFORE USING IT
            //...

            reader.ReadToFollowing("position");
            var x = Int32.Parse(reader.GetAttribute("x") ?? "-1");
            var y = Int32.Parse(reader.GetAttribute("y") ?? "-1");
            StartPosition = (x == -1 || y == -1) ? (Point?) null : new Point(x,y);

            reader.ReadToFollowing("database");
            var server =    reader.GetAttribute("server");
            var database =  reader.GetAttribute("database");
            var uid =       reader.GetAttribute("uid");
            var password =  reader.GetAttribute("password");
            Db = new DbConnection
                {
                    Server = server,
                    Database = database,
                    Uid = uid,
                    Password = password
                };

            // Now Db is initialized, we can use it
            Architect = Db.GetArchitectFromId(archiId);

            reader.Close();

            return true;
        }

        public bool IsValid()
        {
            return StartPosition != null && Db != null && Architect != null;
        }
    }
}
