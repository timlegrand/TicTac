using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

// tried to implement second method from http://msdn.microsoft.com/en-us/library/ms171728(v=vs.80).aspx
// DOES NOT WORK
// (first method works)

namespace ProjectTime
{
    public partial class RecordWindow : Form
    {
        //private string _title;
        private readonly Database _db;
        private static List<Project> _projectList;
        private static List<Phase> _phaseList;
        private static List<Architect> _architectsList;
        private DateTime _startTime;
        private DateTime _stopTime;
        private readonly Session _cfg;
        //public Architect CurrentArchitect { get; set; }
        //public Project CurrentProject { get; set; }
        //public Phase CurrentPhase { get; set; }
        //public uint? RunningProjectId { get; set; }
        

        public RecordWindow()
        {
            InitializeComponent();

            // Retrieve data from server
            _db = new Database();
            _architectsList = _db.SelectAllArchitects();
            _projectList = _db.SelectAllProjects();
            _phaseList = _db.SelectAllPhases();
            
            // Fill in the ComboBoxes
            if (_architectsList != null) comboBoxArchitects.Items.AddRange(_architectsList.ToArray());
            if (_projectList != null) comboBoxProjects.Items.AddRange(_projectList.ToArray());
            if (_phaseList != null) comboBoxPhases.Items.AddRange(_phaseList.ToArray());

            // Try to retreive Architect, Project and Phase
            _cfg = new Session(_db);
            _cfg = Load();
            
            // Select default items
            InitComboBoxes();
            InitButtons(); // Actually useless since called above by "comboBoxArchitects.SelectedItem changed" events
        }


        // Initializers
        private void InitComboBoxes()
        {
            if (_cfg.IsValid())
            {
                comboBoxArchitects.SelectedItem = _cfg.Architect;
                comboBoxProjects.SelectedItem = _cfg.Project;
                comboBoxPhases.SelectedItem = _cfg.Phase;
            }
            else
            {
                comboBoxArchitects.SelectedItem = comboBoxArchitects.Items[0];
                comboBoxProjects.SelectedItem = comboBoxProjects.Items[0];
                comboBoxPhases.SelectedItem = comboBoxPhases.Items[0];
                _cfg.Architect = (Architect)comboBoxArchitects.Items[0];
                _cfg.Project = (Project)comboBoxProjects.Items[0];
                _cfg.Phase = (Phase)comboBoxPhases.Items[0];
            }
        }

        private void InitButtons()
        {
            if (!_cfg.IsValid()) return;

            //TODO
            // Search for any already-started session for a given Architect
            if (_db.StartedWorkSessions(_cfg.Architect).Count == 1)
            {
                _startTime = new DateTime(_cfg.StartTime);
                buttonStart.Enabled = false;
                buttonStop.Enabled = true;
            }
            else // hope that return value is 0 (big problem otherwise)
            {
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
            }
        }


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
            _cfg.Architect = (Architect) comboBoxArchitects.SelectedItem;
            InitButtons();
        }
        
        private void ComboBoxProjectsSelectedIndexChanged(object sender, EventArgs e)
        {
            _cfg.Project = (Project) comboBoxProjects.SelectedItem;
            InitButtons();
        }

        private void ComboBoxPhaseSelectedIndexChanged(object sender, EventArgs e)
        {
            _cfg.Phase = (Phase) comboBoxPhases.SelectedItem;
            InitButtons();
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

            if (Program.IsDatabaseConnexionAvailable())
            {
                _db.StartWorkSession(_cfg);
                comboBoxProjects.Enabled = false;
                comboBoxPhases.Enabled = false;
            }
            else
            {
                MessageBox.Show(@"Vous devez être connecté à Internet pour ajouter des entrées dans la base de données.", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }
        
        private void ButtonStopClick(object sender, EventArgs e)
        {
            _stopTime = DateTime.Now;
            var elapsedTime = _stopTime - _startTime;
            labelTime.Text = string.Format("{0:00}:{1:00}:{2:00}", elapsedTime.Hours, elapsedTime.Minutes,
                                           elapsedTime.Seconds);
            if (Program.IsDatabaseConnexionAvailable())
            {
                _db.EndWorkSession(_cfg);
                comboBoxProjects.Enabled = true;
                comboBoxPhases.Enabled = true;
            }
            else
            {
                MessageBox.Show(@"Base de données inaccessible.", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }


        // Termination
        private void RecordWindowFormClosed(object sender, FormClosedEventArgs e)
        {
            if (!_cfg.IsValid()) return;

            Console.WriteLine(@"Writing " + Session.ConfigFileName + @"...");
            _cfg.SaveToXml();
            Console.WriteLine(Session.ConfigFileName + @" written.");
        }

        private void ButtonConsultClick(object sender, EventArgs e)
        {
            var consultForm = new Consult(this)
                                  { FormBorderStyle = FormBorderStyle.FixedSingle };
            consultForm.Show();
        }
    }
}
