using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

using System.Linq;
using System.Windows.Forms;

// tried to implement second method from http://msdn.microsoft.com/en-us/library/ms171728(v=vs.80).aspx
// DOES NOT WORK
// (first method works)

namespace TicTac
{
    public partial class RecordWindow : Form
    {
        private DAOClient _dao; // Remains for save&close only
        private Service _service;
        public Architect LastArchitect { get; private set; }
        private WorkSession _ws;

        //Constructor
        public RecordWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            // Load configuration, including Default information
            var prefs = new Preferences(this);
            prefs.Load();
            StartPosition = FormStartPosition.Manual;
            Location = prefs.StartLocation;
            LastArchitect = prefs.LastArchitect;

            _service = new Service();
            _dao = new DAOClient();

            InitComboboxes();
            InitButtons(); // Actually useless since called above by "comboBoxArchitects.SelectedItem changed" events
        }

        // Retrieve Comboboxes data
        private void InitComboboxes()
        {
            // Fill in the ComboBoxes
            if (_service.ProjectList != null && _service.ProjectList.Count() != 0)
            {
                comboBoxProjects.Items.AddRange(_service.ProjectList.ToArray());
                comboBoxProjects.SelectedItem = comboBoxProjects.Items[0];
            }

            if (_service.PhaseList != null && _service.PhaseList.Count() != 0)
            {
                comboBoxPhases.Items.AddRange(_service.PhaseList.ToArray());
                comboBoxPhases.SelectedItem = comboBoxPhases.Items[0];
            }

            if (_service.ArchitectList != null && _service.ArchitectList.Count() != 0)
            {
                comboBoxArchitects.Items.AddRange(_service.ArchitectList.ToArray());
                // Must be done LAST because of event management
                if (LastArchitect != null)
                {
                    // TODO make it work with Linq
                    //var item = (ComboBox.ObjectCollection) from Architect elem in comboBoxArchitects.Items
                    //           where elem.Id == LastArchitect.Id
                    //           select elem;
                    var i = 0;
                    while (!LastArchitect.Equals((Architect)comboBoxArchitects.Items[i]))
                    {
                        i++;
                    }
                    comboBoxArchitects.SelectedItem = comboBoxArchitects.Items[i];
                }
                else
                {
                    comboBoxArchitects.SelectedItem = comboBoxArchitects.Items[0];
                }
            }
        }

        private WorkSession RestoreSession(Architect archi)
        {
            if (!Program.IsDatabaseConnexionAvailable(null)) return null;
            // 1- Try to retrieve one single open Session in DB
            var sessions = _service.GetStartedWorkSessions(archi);
            var session = (sessions != null && sessions.Count == 1) ? sessions[0] : null;
            //var session = (Session) from s in sessions where s.Architect.Id == LastArchitect.Id select s;
            if (session == null)
            {
                //TODO 2- If none try to retrieve last work Session info from XML
                comboBoxProjects.Enabled = true;
                comboBoxPhases.Enabled = true;
            }
            else if(session.IsValid())
            {
                Console.WriteLine(@"Running session found:");
                Program.VarDump(session);
                var matchingProjects = (from proj in _service.ProjectList where proj.Id == session.Project.Id select proj).ToList();
                if (matchingProjects.Count() != 1) throw new DataException();
                comboBoxProjects.SelectedItem = matchingProjects.First();
                var matchingPhases = (from phase in _service.PhaseList where phase.Id == session.Phase.Id select phase).ToList();
                if (matchingPhases.Count() != 1) throw new DataException();
                comboBoxPhases.SelectedItem = matchingPhases.First();
                comboBoxProjects.Enabled = false;
                comboBoxPhases.Enabled = false;
            }
            // 3- If not let set all to defaults //TODO to make sure about behavior
            InitButtons();
            return session;
        }


        private void InitButtons()
        {
            // Search for any already-started session for a given Architect
            var sessions = _service.GetStartedWorkSessions((Architect)comboBoxArchitects.SelectedItem);
            var session = (sessions != null && sessions.Count == 1) ? sessions[0] : null;
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
            _ws = RestoreSession((Architect)comboBoxArchitects.SelectedItem) ?? new WorkSession
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

            _service.StartWorkSession(_ws);
            comboBoxProjects.Enabled = false;
            comboBoxPhases.Enabled = false;
            
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }
        
        private void ButtonStopClick(object sender, EventArgs e)
        {
            _ws.StopTime = DateTime.Now;
            var elapsedTime = _ws.StopTime - _ws.StartTime;
            labelTime.Text =
                string.Format("{0:0}j {1:00}:{2:00}:{3:00}",
                elapsedTime.TotalDays,
                elapsedTime.Hours, 
                elapsedTime.Minutes,
                elapsedTime.Seconds);
            
            _service.EndWorkSession(_ws);

            comboBoxProjects.Enabled = true;
            comboBoxPhases.Enabled = true;
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }


        // Termination
        private void RecordWindowFormClosed(object sender, FormClosedEventArgs e)
        {
            var prefs = new Preferences(this)
                {
                    StartLocation = Location,
                    LastArchitect = (Architect) comboBoxArchitects.SelectedItem,
                };
            if (!prefs.IsValid()) return;
#if (DEBUG)            
            Console.WriteLine(@"Writing " + Preferences.ConfigFileName + @"...");
#endif
            prefs.Save();
#if (DEBUG)
            Console.WriteLine(Preferences.ConfigFileName + @" written.");
#endif

            // Serialize and save comboboxes content in files
            _service.SaveAllArchitects();
            _service.SaveAllProjects();
            _service.SaveAllPhases();
        }

        private void ButtonConsultClick(object sender, EventArgs e)
        {
            var consultForm = new DatabaseViewer(this)
                                  { FormBorderStyle = FormBorderStyle.FixedSingle };
            consultForm.Show();
        }
    }
}
