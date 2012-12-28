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
        private readonly DbConnection _db;
        private static List<Project> _projectList;
        private static List<Phase> _phaseList;
        private static List<Architect> _architectsList;
        private Session _ws;
        

        public RecordWindow()
        {
            InitializeComponent();

            // Retrieve data from server
            _db = new DbConnection();
            _architectsList = _db.SelectAllArchitects();
            _projectList = _db.SelectAllProjects();
            _phaseList = _db.SelectAllPhases();

            // Fill in the ComboBoxes
            if (_architectsList != null && _architectsList.Count() != 0) comboBoxArchitects.Items.AddRange(_architectsList.ToArray());
            if (_projectList != null && _projectList.Count() != 0) comboBoxProjects.Items.AddRange(_projectList.ToArray());
            if (_phaseList != null && _phaseList.Count() != 0) comboBoxPhases.Items.AddRange(_phaseList.ToArray());
            comboBoxProjects.SelectedItem = comboBoxProjects.Items[0];
            comboBoxPhases.SelectedItem = comboBoxPhases.Items[0];
            comboBoxArchitects.SelectedItem = comboBoxArchitects.Items[0]; // Must be done LAST
            
            InitButtons(); // Actually useless since called above by "comboBoxArchitects.SelectedItem changed" events
        }

        private Session RestoreSession(Architect archi)
        {
            // 1- Try to retrieve one single open Session in DB
            var sessions = _db.StartedWorkSessions(archi);
            var session = sessions.Count == 1 ? sessions[0] : null;
            if (session == null)
            {
                //TODO 2- If none try to retrieve last work Session info from XML
                comboBoxProjects.Enabled = true;
                comboBoxPhases.Enabled = true;
            }
            else if(session.IsValid())
            {
                Console.WriteLine("Running session found:");
                Program.VarDump(session);
                var matchingProjects = from proj in _projectList where proj.Id == session.Project.Id select proj;
                if (matchingProjects.Count() != 1) throw new DataException();
                comboBoxProjects.SelectedItem = matchingProjects.First();
                var matchingPhases = from phase in _phaseList where phase.Id == session.Phase.Id select phase;
                if (matchingPhases.Count() != 1) throw new DataException();
                comboBoxPhases.SelectedItem = matchingPhases.First();
                comboBoxProjects.Enabled = false;
                comboBoxPhases.Enabled = false;
            }
            // 3- If not let set all to defaults //TODO to make sure
            InitButtons();
            return session;
        }


        private void InitButtons()
        {
            // Search for any already-started session for a given Architect
            var sessions = _db.StartedWorkSessions((Architect)comboBoxArchitects.SelectedItem);
            var session = sessions.Count == 1 ? sessions[0] : null;
            if (session != null)
            {
                _ws = session;
                buttonStart.Enabled = false;
                buttonStop.Enabled = true;
            }
            else // hope that return value is 0 (big problem otherwise)
            {
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
            }
        }
        

        // ComboBox selection
        private void ComboBoxArchitectsSelectedIndexChanged(object sender, EventArgs e)
        {
            _ws = RestoreSession((Architect)comboBoxArchitects.SelectedItem) ?? new Session()
                {
                    Architect = (Architect)comboBoxArchitects.SelectedItem,
                    Project = (Project)comboBoxProjects.SelectedItem,
                    Phase = (Phase)comboBoxPhases.SelectedItem
                };
        }
        
        private void ComboBoxProjectsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ws == null) return;
            _ws.Project = (Project)comboBoxProjects.SelectedItem;
        }

        private void ComboBoxPhaseSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ws == null) return;
            _ws.Phase = (Phase)comboBoxPhases.SelectedItem;
        }


        // Time events handling
        private void ButtonStartClick(object sender, EventArgs e)
        {
            if (!(comboBoxProjects.SelectedIndex > -1) || !(comboBoxPhases.SelectedIndex > -1))
            {
                MessageBox.Show(@"Choisissez un projet et une phase", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            _ws.StartTime = DateTime.Now;

            _db.StartWorkSession(_ws);
            comboBoxProjects.Enabled = false;
            comboBoxPhases.Enabled = false;
            
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }
        
        private void ButtonStopClick(object sender, EventArgs e)
        {
            _ws.StopTime = DateTime.Now;
            var elapsedTime = _ws.StopTime - _ws.StartTime;
            labelTime.Text = string.Format("{0:00}:{1:00}:{2:00}", 
                elapsedTime.Hours, 
                elapsedTime.Minutes,
                elapsedTime.Seconds);
            
            _db.EndWorkSession(_ws);

            comboBoxProjects.Enabled = true;
            comboBoxPhases.Enabled = true;
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }


        // Termination
        private void RecordWindowFormClosed(object sender, FormClosedEventArgs e)
        {
            if (!_ws.IsValid()) return;
            /*
            Console.WriteLine(@"Writing " + Session.SessionFileName + @"...");
            _ws.SaveToXml();
            Console.WriteLine(Session.SessionFileName + @" written.");
            */
        }

        private void ButtonConsultClick(object sender, EventArgs e)
        {
            var consultForm = new Consult(this)
                                  { FormBorderStyle = FormBorderStyle.FixedSingle };
            consultForm.Show();
        }
    }
}
