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
        private const string ConfigFile = "config.xml";
        private string _realTimeElapsed;

        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
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

            // Fill in the Architects ComboBox
            //var queryArchitects = from arch in _architectsDb orderby arch.LastName ascending select new { arch.FirstName, arch.LastName };
            var queryArchitects = from arch in _architectsDb orderby arch.LastName ascending select arch.FirstName;
            comboBoxArchitects.Items.AddRange(queryArchitects.ToArray());
            comboBoxArchitects.SelectedItem = comboBoxArchitects.Items[0];
            
            // Fill in the Projects ComboBox
            var queryProjects = from proj in _projectDb orderby proj.Name ascending select proj.Name;
            comboBoxProjects.Items.AddRange(queryProjects.ToArray());
            comboBoxProjects.SelectedItem = comboBoxProjects.Items[0];

            // Fill in the Phase ComboBox
            var queryPhases = from phase in _phaseDb select phase.Name;
            comboBoxPhases.Items.AddRange(queryPhases.ToArray());
            comboBoxPhases.SelectedItem = comboBoxPhases.Items[0];

            // Create a timer with a one-second interval
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Enabled = true;
            // If the timer is declared in a long-running method, use KeepAlive to prevent garbage collection from occurring before the method ends.
            GC.KeepAlive(_timer);

            // Create a backgroung worker for inter-thread communications
            //this._bgTimer = new System.ComponentModel.BackgroundWorker();

            // Read config file if any
            if (File.Exists(ConfigFile))
            {
                LoadConfig();
                comboBoxArchitects.SelectedItem = _currentArchitect.FirstName;
                comboBoxProjects.SelectedItem = _currentProject.Name;
                comboBoxPhases.SelectedItem = _currentPhase.Name;
            }
        }

        // Accessors
        public Architect CurrentArchitect { get; set; }
        public Project CurrentProject { get; set; }
        public Phase CurrentPhase { get; set; }

        // Database queries
        private Architect GetArchitectFromName(string firstName)
        {
            var matchingArchitects = from arch in _architectsDb where arch.FirstName == firstName select arch;
            if (matchingArchitects.Count() != 1) throw new DataException("Aucun ou plusieurs architectes ont le nom désiré");
            return matchingArchitects.First();
        }

        private Project GetProjectFromName(string name)
        {
            var matchingProjects = from proj in _projectDb where proj.Name == name select proj;
            if (matchingProjects.Count() != 1) throw new DataException("Aucun ou plusieurs projets ont le nom désiré");
            return matchingProjects.First();
        }

        private Phase GetPhaseFromName(string name)
        {
            var matchingPhases = from phase in _phaseDb where phase.Name == name select phase;
            if (matchingPhases.Count() != 1) throw new DataException();
            return matchingPhases.First();
        }


        // Events handling
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

        private void ComboBoxProjectsSelectedIndexChanged(object sender, EventArgs e)
        {
            _currentProject = GetProjectFromName((string) comboBoxProjects.SelectedItem);
        }

        private void ComboBoxPhaseSelectedIndexChanged(object sender, EventArgs e)
        {
            _currentPhase = GetPhaseFromName((string) comboBoxPhases.SelectedItem);
        }

        private void ComboBoxArchitectsSelectedIndexChanged(object sender, EventArgs e)
        {
            _currentArchitect = GetArchitectFromName((string) comboBoxArchitects.SelectedItem.ToString());
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


        private void AddArchitectClick(object sender, EventArgs e)
        {
            if (!Program.IsInternetConnexionAvailable())
            {
                MessageBox.Show("Vous devez être connecté à Internet pour ajouter des entrées dans la base de données.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
                
            var addForm = new AddArchitect(_architectsDb, this);
            addForm.Show();
            }

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
            myXmlTextWriter.WriteElementString("lastname", archi.LastName);
            myXmlTextWriter.WriteElementString("firstname", archi.FirstName);
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.WriteStartElement("project");
            myXmlTextWriter.WriteElementString("name", pro.Name);
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.WriteStartElement("phase");
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
            reader.ReadStartElement("lastname");
            var ln = reader.ReadString();
            reader.ReadEndElement();
            reader.ReadStartElement("firstname");
            var fn = reader.ReadString();
            reader.ReadEndElement();
            _currentArchitect = GetArchitectFromName(fn);
            reader.ReadEndElement();

            reader.ReadStartElement("project");
            reader.ReadStartElement("name");
            _currentProject = GetProjectFromName(reader.ReadString());
            reader.ReadEndElement();
            reader.ReadEndElement();

            reader.ReadStartElement("phase");
            reader.ReadStartElement("name");
            _currentPhase = GetPhaseFromName(reader.ReadString());
            reader.ReadEndElement();
            reader.ReadEndElement();

            reader.ReadEndElement();

            reader.Close();
        }

        private void RecordWindowFormClosed(object sender, FormClosedEventArgs e)
        {
            Debug.Assert(_currentArchitect != null && _currentProject != null && _currentPhase != null);
            Console.WriteLine("Writing " + ConfigFile + "...");
            SaveConfig(_currentArchitect, _currentProject, _currentPhase);
            Console.WriteLine(ConfigFile + " written.");
        }

        private void AddProjectClick(object sender, EventArgs e)
        {
            if (!Program.IsInternetConnexionAvailable())
            {
                MessageBox.Show("Vous devez être connecté à Internet pour ajouter des entrées dans la base de données.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //TODO
        }

        private void AddPhaseClick(object sender, EventArgs e)
        {
            if (!Program.IsInternetConnexionAvailable())
            {
                MessageBox.Show("Vous devez être connecté à Internet pour ajouter des entrées dans la base de données.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //TODO
        }
    }
}
