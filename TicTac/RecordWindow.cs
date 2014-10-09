using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Linq;
using System.Windows.Forms;


namespace TicTac
{
    public partial class RecordWindow : Form
    {
        private Service _service;
        private Preferences _prefs;
        public WorkSession WorkSession { get; private set; }
        private TicTimer _ticTimer;

        //Constructor
        public RecordWindow()
        {
            // Load configuration, including Default information
            _prefs = new DualModePreferences(this);
            _prefs.Load();

            Service.StartAsync();
            _ticTimer = new TicTimer(OnTimerTickEvent, 1000, true);

            InitializeComponent();
            Initialize();

            Program.clk.Print();
        }

        private delegate void SetLabelTimeTextDelegate(string newLabel);
        private void SetLabelTimeText(string newLabel)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new SetLabelTimeTextDelegate(SetLabelTimeText), new object[] {newLabel});
                return;
            }

            this.labelTime.Text = newLabel;
        }

        void OnTimerTickEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            var s = string.Format("{0:0} jour(s) et {1:00}:{2:00}:{3:00}",
                _ticTimer.Elapsed.TotalDays,
                _ticTimer.Elapsed.Hours,
                _ticTimer.Elapsed.Minutes,
                _ticTimer.Elapsed.Seconds);

            SetLabelTimeText(s);
        }

        private void Initialize()
        {
            this.notifyIcon.Icon = this.Icon;

            StartPosition = FormStartPosition.Manual;
            Location = _prefs.StartLocation;

            // Following needs Service to be initialized
            Service.Ready();
            _service = Service.Instance;
            Program.clk.Probe("SERVICE READY");

            this.SuspendLayout();
            InitComboboxes();
            InitButtons();
            this.ResumeLayout();

            // Retrieve current WS if any
            ComboBoxArchitectsSelectionChangeCommited(null, null);
        }

        public void InitComboboxes()
        {
            InitComboboxArchitects();
            InitComboboxProjects();
            InitComboboxPhases();
        }

        public void InitComboboxArchitects()
        {
            comboBoxArchitects.Items.Clear();
         
            if (_service.ArchitectList != null && _service.ArchitectList.Count() != 0)
            {
                comboBoxArchitects.Items.AddRange(_service.ArchitectList.ToArray());
                int id = _prefs.LastArchitect != null ? _prefs.LastArchitect.Id ?? 0 : 0;
                var item = comboBoxArchitects.Items.Cast<Architect>().Where(archi => archi.Id == id).FirstOrDefault();
                comboBoxArchitects.SelectedItem = item ?? comboBoxArchitects.Items[0];
            }
        }

        public void InitComboboxProjects()
        {
            comboBoxProjects.DataSource = null;
            comboBoxProjects.Items.Clear();

            if (_service.ProjectList != null && _service.ProjectList.Count() != 0)
            {
                // Sort descending
                _service.ProjectList.Sort((p1,p2)=>p1.Name.CompareTo(p2.Name));
                _service.ProjectList.Reverse();
                comboBoxProjects.DataSource = _service.ProjectList;
                //comboBoxProjects.DisplayMember = "Name";
                //comboBoxProjects.ValueMember = "Id";

                // Reselect last project
                var id = _prefs.LastProject != null ? _prefs.LastProject.Id ?? 0 : 0;
                var item = comboBoxProjects.Items.Cast<Project>().Where(project => project.Id == id).FirstOrDefault();
                comboBoxProjects.SelectedItem = item ?? comboBoxProjects.Items[0];
            }
        }

        public void InitComboboxPhases()
        {
            comboBoxPhases.DataSource = null;
            comboBoxPhases.Items.Clear();    

            if (_service.PhaseList != null && _service.PhaseList.Count() != 0)
            {
                // Sort ascending
                _service.PhaseList.Sort((p1, p2) => p1.Name.CompareTo(p2.Name));
                comboBoxPhases.DataSource = _service.PhaseList;
                //comboBoxProjects.DisplayMember = "Name";
                //comboBoxProjects.ValueMember = "Id";

                // Reselect last phase
                var id = _prefs.LastPhase != null ? _prefs.LastPhase.Id ?? 0 : 0;
                var item = comboBoxPhases.Items.Cast<Phase>().Where(phase => phase.Id == id).FirstOrDefault();
                comboBoxPhases.SelectedItem = item ?? comboBoxPhases.Items[0];
            }
        }

        private WorkSession RestoreSession(Architect archi)
        {
            if (!ConnectionPing.DatabaseConnexionAvailable)
            {
                throw new NotImplementedException(@"file-based session not yet implemented");
            }

            // 1- Try to retrieve one single open Session in DB
            var session = _service.GetStartedWorkSessions(archi).SingleOrDefault<WorkSession>();
            if (session == null)
            {
                comboBoxProjects.Enabled = true;
                comboBoxPhases.Enabled = true;
            }
            else
            {
                //TODO 2- If none try to retrieve last work Session info from XML
            }

            if(session != null && session.IsValid())
            {
                Console.WriteLine(@"Running session found:");
                Utils.Vardump.dump(session);
                var matchingProjects = (from proj in _service.ProjectList where proj.Id == session.Project.Id select proj).ToList();
                if (matchingProjects.Count() != 1) throw new DataException();
                comboBoxProjects.SelectedItem = matchingProjects.First();
                var matchingPhases = (from phase in _service.PhaseList where phase.Id == session.Phase.Id select phase).ToList();
                if (matchingPhases.Count() != 1) throw new DataException();
                comboBoxPhases.SelectedItem = matchingPhases.First();

                _ticTimer.Start(DateTime.Now - session.StartTime);

                comboBoxProjects.Enabled = false;
                comboBoxPhases.Enabled = false;
            }

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
                WorkSession = session;
                buttonStart.Enabled = false;
                buttonStop.Enabled = true;
                notifyIcon.Icon = Properties.Resources.tictac_on;
            }
            else // hope that return value is 0 (big problem otherwise)
            {
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
                notifyIcon.Icon = Properties.Resources.tictac;
            }
        }
        

        // ComboBox selection
        private void ComboBoxArchitectsSelectionChangeCommited(object sender, EventArgs e)
        {
            var archi = (Architect)comboBoxArchitects.SelectedItem;

            var daily = _service.GetDailyWorkSessions(archi);
            foreach (WorkSession ws in daily)
            {
                Console.WriteLine(ws.ToString());
            }

            WorkSession = RestoreSession(archi) ?? new WorkSession
                {
                    Architect = archi,
                    Project = (Project)comboBoxProjects.SelectedItem,
                    Phase = (Phase)comboBoxPhases.SelectedItem
                };
        }
        
        private void ComboBoxProjectsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (WorkSession == null) return;
            WorkSession.Project = (Project)comboBoxProjects.SelectedItem;
        }

        private void ComboBoxPhaseSelectedIndexChanged(object sender, EventArgs e)
        {
            if (WorkSession == null) return;
            WorkSession.Phase = (Phase)comboBoxPhases.SelectedItem;
        }


        // Time events handling
        private void ButtonStartClick(object sender, EventArgs e)
        {
            if (!(comboBoxProjects.SelectedIndex > -1) || !(comboBoxPhases.SelectedIndex > -1))
            {
                MessageBox.Show(@"Choisissez un projet et une phase", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            _ticTimer.Start();

            WorkSession.StartTime = DateTime.Now;
            _service.StartWorkSession(WorkSession);

            comboBoxProjects.Enabled = false;
            comboBoxPhases.Enabled = false;
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;

            notifyIcon.Icon = Properties.Resources.tictac_on;
        }
        
        private void ButtonStopClick(object sender, EventArgs e)
        {
            _ticTimer.Stop();

            WorkSession.StopTime = DateTime.Now;
            _service.EndWorkSession(WorkSession);

            comboBoxProjects.Enabled = true;
            comboBoxPhases.Enabled = true;
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;

            notifyIcon.Icon = Properties.Resources.tictac_on;
        }


        // Termination
        private void RecordWindowFormClosed(object sender, FormClosedEventArgs e)
        {
            var prefs = new DualModePreferences(this)
                {
                    StartLocation = Location,
                    LastArchitect = (Architect) comboBoxArchitects.SelectedItem,
                    LastProject = (Project)comboBoxProjects.SelectedItem,
                    LastPhase = (Phase)comboBoxPhases.SelectedItem,
                };
            if (!prefs.IsValid()) return;
            prefs.Save();

            // Serialize and save comboboxes content in files
            _service.SaveAllArchitects();
            _service.SaveAllProjects();
            _service.SaveAllPhases();

            // System tray icon
            if (notifyIcon != null)
            {
                this.notifyIcon.Visible = false;
                this.notifyIcon.Dispose();
                this.notifyIcon = null;
            }
        }

        //this.notifyIcon.Icon = this.Icon; // L'icône de not_zero est celle de l'application.
        //this.notifyIcon.Icon = SystemIcons.Application; // Affiche l'icône par défaut.
        //this.notifyIcon.Icon = SystemIcons.Error; // Affiche l'icône d'erreur.
        //this.notifyIcon.Icon = SystemIcons.Warning; // Affiche l'icône de danger.
        //this.notifyIcon.Icon = SystemIcons.Question; // Affiche l'icône de question.
        //this.notifyIcon.Icon = SystemIcons.Shield; // Affiche l'icône du bouclier Windows.
        //this.notifyIcon.BalloonTipIcon = ToolTipIcon.Info; // Icône information de Windows.
        //this.notifyIcon.ShowBalloonTip(3000); // On affiche le message indéfiniment.

        private void RecordWindow_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon.ShowBalloonTip(500);
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            // Left click only to hide/show GUI
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            // Hide
            if (this.WindowState == FormWindowState.Normal)
            {
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
                notifyIcon.ShowBalloonTip(500);
            }
            // Show
            else if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void notifyIconMenuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void trayMenuItemMinimize_Click(object sender, EventArgs e)
        {
            // Hide
            if (this.WindowState == FormWindowState.Normal)
            {
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
                notifyIcon.ShowBalloonTip(500);
            }
        }

        private void trayMenuItemOpen_Click(object sender, EventArgs e)
        {
            // Show
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void trayMenuItemConfigure_Click(object sender, EventArgs e)
        {
            WorkSession ws = new WorkSession
                                    {
                                        Architect = (Architect)comboBoxArchitects.SelectedItem,
                                        Project = (Project)comboBoxProjects.SelectedItem,
                                        Phase = (Phase)comboBoxPhases.SelectedItem
                                    };
            var consultForm = new DatabaseViewer(this, ws) { FormBorderStyle = FormBorderStyle.FixedSingle };
            consultForm.Show();
        }
    }
}
