using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
//using System.Reflection;
//using System.Threading;
using System.Timers;
using System.Windows.Forms;

// For timers
using Timer = System.Timers.Timer;


namespace ProjectTime
{
    public partial class RecordWindow : Form
    {
        //private string _title;
        private readonly DbConnect _db;
        private static List<Project> _projectList;
        private static List<Phase> _phaseList;
        private static List<Architect> _architectsList;
        private Architect _currentArchitect;
        private Project _currentProject;
        private Phase _currentPhase;
        private DateTime _startTime;
        private DateTime _stopTime;
        private TimeSpan _elapsedTime;
        private static Timer _timer;
        private BackgroundWorker _bgTimer;
        private readonly Config _config;
        
        // This delegate enables asynchronous calls for setting the text property on a TextBox control.
        //delegate void SetTextCallback(string text);


        public RecordWindow()
        {
            InitializeComponent();

            _db = new DbConnect();
            _architectsList = _db.SelectAllArchitects();
            _projectList = _db.SelectAllProjects();
            _phaseList = _db.SelectAllPhases();

            _config = new Config(_db) {Architect = null, Project = null, Phase = null};

            buttonStop.Enabled = false;

            // Fill in the Architect, Project and Phases ComboBox
            comboBoxArchitects.Items.AddRange(_architectsList.ToArray());
            comboBoxArchitects.SelectedItem = comboBoxArchitects.Items[0];
            comboBoxProjects.Items.AddRange(_projectList.ToArray());
            comboBoxProjects.SelectedItem = comboBoxProjects.Items[0];
            comboBoxPhases.Items.AddRange(_phaseList.ToArray());
            comboBoxPhases.SelectedItem = comboBoxPhases.Items[0];

            // Create a timer with a one-second interval
            _timer = new Timer(1000);
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Enabled = true;
            // If the timer is declared in a long-running method, use KeepAlive to prevent garbage collection from occurring before the method ends.
            GC.KeepAlive(_timer);

            // Create a backgroung worker for inter-thread communications
            //this._bgTimer = new System.ComponentModel.BackgroundWorker();

            _config.LoadConfig();
            comboBoxArchitects.SelectedItem = _config.Architect;
            comboBoxProjects.SelectedItem = _config.Project;
            comboBoxPhases.SelectedItem = _config.Phase;
        }

        // Accessors
        public Architect CurrentArchitect { get; set; }
        public Project CurrentProject { get; set; }
        public Phase CurrentPhase { get; set; }
        public uint? RunningProjectId { get; set; }

        // Database queries
        public static Architect GetArchitectFromId(int id)
        {
            var matchingArchitects = from arch in _architectsList where arch.Id == id select arch;
            if (matchingArchitects.Count() != 1) throw new DataException("Aucun ou plusieurs architectes ont l'id désiré");
            return matchingArchitects.First();
        }

        public static Project GetProjectFromId(int id)
        {
            var matchingProjects = from proj in _projectList where proj.Id == id select proj;
            if (matchingProjects.Count() != 1) throw new DataException("Aucun ou plusieurs projets ont le nom désiré");
            return matchingProjects.First();
        }

        public static Phase GetPhaseFromId(int id)
        {
            var matchingPhases = from phase in _phaseList where phase.Id == id select phase;
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
                MessageBox.Show(@"Choisissez un projet et une phase", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            if (Program.IsInternetConnexionAvailable())
            {
                _db.InsertWorked(_elapsedTime.TotalSeconds, _currentArchitect, _currentProject, _currentPhase);
            }
            else
            {
                MessageBox.Show(@"Vous devez être connecté à Internet pour ajouter des entrées dans la base de données.", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _config.RealTimeElapsed = string.Format("{0:00}:{1:00}:{2:00}", e.SignalTime.Hour, e.SignalTime.Minute, e.SignalTime.Second);
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


        // Termination
        private void RecordWindowFormClosed(object sender, FormClosedEventArgs e)
        {
            Debug.Assert(_currentArchitect != null && _currentProject != null && _currentPhase != null);
            Console.WriteLine(@"Writing " + Config.ConfigFileName + @"...");
            _config.SaveConfig(_currentArchitect, _currentProject, _currentPhase);
            Console.WriteLine(Config.ConfigFileName + @" written.");
        }

        private void ButtonConsultClick(object sender, EventArgs e)
        {
            var consultForm = new Consult(this)
                                  { FormBorderStyle = FormBorderStyle.FixedSingle };
            consultForm.Show();
        }
    }
}
