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
        private readonly DbConnect _db;
        private static List<Project> _projectList;
        private static List<Phase> _phaseList;
        private static List<Architect> _architectsList;
        private DateTime _startTime;
        private DateTime _stopTime;
        private readonly Config _config;
        

        public RecordWindow()
        {
            InitializeComponent();

            // Retrieve data from server
            _db = new DbConnect();
            _architectsList = _db.SelectAllArchitects();
            _projectList = _db.SelectAllProjects();
            _phaseList = _db.SelectAllPhases();
            
            // Fill in the ComboBoxes
            comboBoxArchitects.Items.AddRange(_architectsList.ToArray());
            comboBoxProjects.Items.AddRange(_projectList.ToArray());
            comboBoxPhases.Items.AddRange(_phaseList.ToArray());
            
            // Create config
            _config = new Config(_db);
            _config.LoadFromXml();

            // Select default items
            if (_config.IsValid())
            {
                comboBoxArchitects.SelectedItem = _config.Architect;
                comboBoxProjects.SelectedItem = _config.Project;
                comboBoxPhases.SelectedItem = _config.Phase;
            }
            else
            {
                comboBoxArchitects.SelectedItem = comboBoxArchitects.Items[0];
                comboBoxProjects.SelectedItem = comboBoxProjects.Items[0];
                comboBoxPhases.SelectedItem = comboBoxPhases.Items[0];
                _config.Architect = (Architect) comboBoxArchitects.Items[0];
                _config.Project = (Project) comboBoxProjects.Items[0];
                _config.Phase = (Phase) comboBoxPhases.Items[0];
            }

            Program.VarDump(_config);
            return;
            InitStartStopButtons();
        }

        // Accessors
        public Architect CurrentArchitect { get; set; }
        public Project CurrentProject { get; set; }
        public Phase CurrentPhase { get; set; }
        public uint? RunningProjectId { get; set; }

        private void InitStartStopButtons()
        {
            if (!_config.IsValid()) return;

            // Search for any already-started session
            if (_db.StartedWorkSessions(_config) == 1)
            {
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
            _config.Architect = (Architect) comboBoxArchitects.SelectedItem;
            InitStartStopButtons();
        }
        
        private void ComboBoxProjectsSelectedIndexChanged(object sender, EventArgs e)
        {
            _config.Project = (Project) comboBoxProjects.SelectedItem;
            InitStartStopButtons();
        }

        private void ComboBoxPhaseSelectedIndexChanged(object sender, EventArgs e)
        {
            _config.Phase = (Phase) comboBoxPhases.SelectedItem;
            InitStartStopButtons();
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

            if (Program.IsInternetConnexionAvailable())
            {
                _db.StartWorkSession(_config);
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
            if (Program.IsInternetConnexionAvailable())
            {
                _db.EndWorkSession(_config);
            }
            else
            {
                MessageBox.Show(@"Vous devez être connecté à Internet pour ajouter des entrées dans la base de données.", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }


        // Termination
        private void RecordWindowFormClosed(object sender, FormClosedEventArgs e)
        {
            if (!_config.IsValid()) return;

            Console.WriteLine(@"Writing " + Config.ConfigFileName + @"...");
            _config.SaveToXml();
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
