using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml;

// For timers
using Timer = System.Timers.Timer;


namespace ProjectTime
{
    public partial class RecordWindow : Form
    {
        //private string _title;
        private readonly DBConnect _db;
        private readonly List<Project> _projectDb;
        private readonly List<Phase> _phaseDb;
        private readonly List<Architect> _architectsDb;
        private Architect _currentArchitect;
        private Project _currentProject;
        private Phase _currentPhase;
        private DateTime _startTime;
        private DateTime _stopTime;
        private TimeSpan _elapsedTime;
        private static Timer _timer;
        private BackgroundWorker _bgTimer;
        private const string ConfigFileName = "config.xml";
        private string ConfigFile;
        private string _realTimeElapsed;

        // This delegate enables asynchronous calls for setting the text property on a TextBox control.
        delegate void SetTextCallback(string text);


        public RecordWindow()
        {
            InitializeComponent();
            _db = new DBConnect();
            _architectsDb = _db.SelectAllArchitects();
            _projectDb = _db.SelectAllProjects();
            _phaseDb = _db.SelectAllPhases();
            
            _currentArchitect = null;
            _currentProject = null;
            _currentPhase = null;
            buttonStop.Enabled = false;

            // Fill in the Architect, Project and Phases ComboBox
            comboBoxArchitects.Items.AddRange(_architectsDb.ToArray());
            comboBoxArchitects.SelectedItem = comboBoxArchitects.Items[0];
            comboBoxProjects.Items.AddRange(_projectDb.ToArray());
            comboBoxProjects.SelectedItem = comboBoxProjects.Items[0];
            comboBoxPhases.Items.AddRange(_phaseDb.ToArray());
            comboBoxPhases.SelectedItem = comboBoxPhases.Items[0];

            // Create a timer with a one-second interval
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Enabled = true;
            // If the timer is declared in a long-running method, use KeepAlive to prevent garbage collection from occurring before the method ends.
            GC.KeepAlive(_timer);

            // Create a backgroung worker for inter-thread communications
            //this._bgTimer = new System.ComponentModel.BackgroundWorker();

            // ConfigFile should be saved in "My Document" folder
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ProjectTime\\";
            if (!Directory.Exists(path)) 
            {
                Directory.CreateDirectory(path);
            }
            ConfigFile = path + ConfigFileName;
            Console.WriteLine(ConfigFile);
            if (File.Exists(ConfigFile) && (new FileInfo(ConfigFile).Length > 100))
            {
                LoadConfig();
                comboBoxArchitects.SelectedItem = _currentArchitect;
                comboBoxProjects.SelectedItem = _currentProject;
                comboBoxPhases.SelectedItem = _currentPhase;
            }
        }

        // Accessors
        public Architect CurrentArchitect { get; set; }
        public Project CurrentProject { get; set; }
        public Phase CurrentPhase { get; set; }

        // Database queries
        private Architect GetArchitectFromId(int id)
        {
            var matchingArchitects = from arch in _architectsDb where arch.Id == id select arch;
            if (matchingArchitects.Count() != 1) throw new DataException("Aucun ou plusieurs architectes ont l'id désiré");
            return matchingArchitects.First();
        }

        private Project GetProjectFromId(int id)
        {
            var matchingProjects = from proj in _projectDb where proj.Id == id select proj;
            if (matchingProjects.Count() != 1) throw new DataException("Aucun ou plusieurs projets ont le nom désiré");
            return matchingProjects.First();
        }

        private Phase GetPhaseFromId(int id)
        {
            var matchingPhases = from phase in _phaseDb where phase.Id == id select phase;
            if (matchingPhases.Count() != 1) throw new DataException();
            return matchingPhases.First();
        }


        // ComboBox selection
        private void ComboBoxArchitectsSelectedIndexChanged(object sender, EventArgs e)
        {
            _currentArchitect = (Architect) comboBoxArchitects.SelectedItem;
        }
        
        private void ComboBoxProjectsSelectedIndexChanged(object sender, EventArgs e)
        {
            _currentProject = (Project) comboBoxProjects.SelectedItem;
        }

        private void ComboBoxPhaseSelectedIndexChanged(object sender, EventArgs e)
        {
            _currentPhase = (Phase) comboBoxPhases.SelectedItem;
        }


        // Time events handling
        private void ButtonStartClick(object sender, EventArgs e)
        {
            if (!(comboBoxProjects.SelectedIndex > -1) || !(comboBoxPhases.SelectedIndex > -1))
            {
                MessageBox.Show("Choisissez un projet et une phase", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
                //throw new Exception("Choisissez un projet et une phase");
            }
            _startTime = DateTime.Now;
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }

        private void ButtonStopClick(object sender, EventArgs e)
        {
            _stopTime = DateTime.Now;
            _elapsedTime = _stopTime - _startTime;
            labelTime.Text = string.Format("{0:00}:{1:00}:{2:00}", _elapsedTime.Hours, _elapsedTime.Minutes,
                                           _elapsedTime.Seconds);
            _db.InsertWorked(_elapsedTime.TotalSeconds, _currentArchitect, _currentProject, _currentPhase);
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _realTimeElapsed = string.Format("{0:00}:{1:00}:{2:00}", e.SignalTime.Hour, e.SignalTime.Minute, e.SignalTime.Second);
            Debug.Assert(_bgTimer != null);
            //_bgTimer.RunWorkerAsync();
        }
        
        private void BgTimerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // tried to implement second method from http://msdn.microsoft.com/en-us/library/ms171728(v=vs.80).aspx
            // DOES NOT WORK
            // (first method works)
            //this.labelTime.Text = _realTimeElapsed;
        }


        // Last selection memorization
        private void SaveConfig(Architect archi, Project pro, Phase ph)
        {
            Program.VarDump(archi);
            Program.VarDump(pro);
            Program.VarDump(ph);

            var myXmlTextWriter = new XmlTextWriter(ConfigFile, System.Text.Encoding.UTF8)
                                      {Formatting = Formatting.Indented};

            myXmlTextWriter.WriteStartDocument(false);
            myXmlTextWriter.WriteComment("Fichier de sauvegarde de la dernière configuration de ProjectTime.");
            myXmlTextWriter.WriteStartElement("config");

            myXmlTextWriter.WriteStartElement("architect");
            myXmlTextWriter.WriteElementString("id", archi.Id.ToString());
            myXmlTextWriter.WriteElementString("firstname", archi.FirstName);
            myXmlTextWriter.WriteElementString("lastname", archi.LastName);
            myXmlTextWriter.WriteElementString("company", _db.GetCompanyNameFromId(archi.Company));
            myXmlTextWriter.WriteEndElement();

            myXmlTextWriter.WriteStartElement("project");
            myXmlTextWriter.WriteElementString("id", pro.Id.ToString());
            myXmlTextWriter.WriteElementString("name", pro.Name);
            myXmlTextWriter.WriteEndElement();

            myXmlTextWriter.WriteStartElement("phase");
            myXmlTextWriter.WriteElementString("id", ph.Id.ToString());
            myXmlTextWriter.WriteElementString("name", ph.Name);
            myXmlTextWriter.WriteEndElement();

            myXmlTextWriter.WriteEndElement();
            
            myXmlTextWriter.Flush();
            myXmlTextWriter.Close();
        }

        private void LoadConfig()
        {
            var settings = new XmlReaderSettings
                               {
                                   ConformanceLevel = ConformanceLevel.Fragment,
                                   IgnoreWhitespace = true,
                                   IgnoreComments = true
                               };
            var reader = XmlReader.Create(ConfigFile, settings);

            reader.Read();
            reader.ReadStartElement("config");

            reader.ReadStartElement("architect");
            var archiId = int.Parse(reader.ReadElementString("id"));
            reader.ReadElementString("firstname");
            reader.ReadElementString("lastname");
            reader.ReadElementString("company");
            _currentArchitect = GetArchitectFromId(archiId);
            reader.ReadEndElement();

            reader.ReadStartElement("project");
            var projectId = int.Parse(reader.ReadElementString("id"));
            reader.ReadElementString("name");
            _currentProject = GetProjectFromId(projectId);
            reader.ReadEndElement();

            reader.ReadStartElement("phase");
            var phaseId = int.Parse(reader.ReadElementString("id"));
            reader.ReadElementString("name");
            _currentPhase = GetPhaseFromId(phaseId);
            reader.ReadEndElement();

            reader.ReadEndElement(); // End "config"

            reader.Close();
        }

        // Termination
        private void RecordWindowFormClosed(object sender, FormClosedEventArgs e)
        {
            Debug.Assert(_currentArchitect != null && _currentProject != null && _currentPhase != null);
            Console.WriteLine("Writing " + ConfigFile + "...");
            SaveConfig(_currentArchitect, _currentProject, _currentPhase);
            Console.WriteLine(ConfigFile + " written.");
        }

        private void ButtonConsultClick(object sender, EventArgs e)
        {
            var consultForm = new Consult(this)
                                  { FormBorderStyle = FormBorderStyle.FixedSingle };
            consultForm.Show();
        }
    }
}
