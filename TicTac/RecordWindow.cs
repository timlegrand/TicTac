using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Linq;
using System.Windows.Forms;


namespace TicTac
{
    public partial class RecordWindow : CommonForm
    {
        private Service _service;
        private Preferences preferences;
        public WorkSession WorkSession { get; private set; }
        private TicTimer ticTimer;
        private TicTimer recallTimer;

        public RecordWindow()
            : base()
        {
            this.preferences = new DualModePreferences();
            this.preferences.Load();

            Database.WaitForConnectivity();
            Service.StartAsync();

            this.ticTimer = new TicTimer(OnTimerTick, 1000, true);
            this.recallTimer = new TicTimer(OnRecallTimerAlarm, 900000); // Every 15 minutes

            InitializeComponent();
            Initialize();
        }

        public RecordWindow(System.Drawing.Point p)
            : this()
        {
            this.busyAnimation.Location = p;
        }

        private void Initialize()
        {
            this.Text += " (v " + Program.CurrentVersion + ")";

            this.notifyIcon.Icon = this.Icon;

            StartPosition = FormStartPosition.Manual;
            Location = BoundLocation(preferences.StartLocation);

            // Following needs Service to be initialized
            _service = Service.Ready();
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
                int id = preferences.LastArchitect != null ? preferences.LastArchitect.Id ?? 0 : 0;
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
                var id = preferences.LastProject != null ? preferences.LastProject.Id ?? 0 : 0;
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
                var id = preferences.LastPhase != null ? preferences.LastPhase.Id ?? 0 : 0;
                var item = comboBoxPhases.Items.Cast<Phase>().Where(phase => phase.Id == id).FirstOrDefault();
                comboBoxPhases.SelectedItem = item ?? comboBoxPhases.Items[0];
            }
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

        private WorkSession RestoreSession(Architect archi)
        {
            // 1- Try to retrieve one single open Session in DB
            var session = _service.GetStartedWorkSessions(archi).SingleOrDefault<WorkSession>();
            if (session == null)
            {
                comboBoxProjects.Enabled = true;
                comboBoxPhases.Enabled = true;
            }
            else if (session.IsValid())
            {
                Console.WriteLine(@"Running session found:");
                session.PrettyPrint();

                comboBoxProjects.SelectedItem = (from proj in _service.ProjectList
                                                 where proj.Id == session.Project.Id
                                                 select proj).Single();
                comboBoxPhases.SelectedItem = (from phase in _service.PhaseList
                                               where phase.Id == session.Phase.Id
                                               select phase).Single();

                ticTimer.Start(DateTime.Now - session.StartTime);
                recallTimer.Start();

                comboBoxProjects.Enabled = false;
                comboBoxPhases.Enabled = false;
            }

            InitButtons();
            return session;
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


        // Termination
        private void RecordWindowFormClosed(object sender, FormClosedEventArgs e)
        {
            var prefs = new DualModePreferences(this)
            {
                StartLocation = Location,
                LastArchitect = (Architect)comboBoxArchitects.SelectedItem,
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


        // Time events handling
        private delegate void SetLabelTimeTextDelegate(string newLabel);
        private void SetLabelTimeText(string newLabel)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new SetLabelTimeTextDelegate(SetLabelTimeText), new object[] { newLabel });
                return;
            }

            this.labelTime.Text = newLabel;
        }

        private void OnTimerTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            var s = string.Format("{0:0} jour(s) et {1:00}:{2:00}:{3:00}",
                ticTimer.Elapsed.TotalDays,
                ticTimer.Elapsed.Hours,
                ticTimer.Elapsed.Minutes,
                ticTimer.Elapsed.Seconds);

            SetLabelTimeText(s);
        }

        private delegate void SetAndShowBallonTipDelegate(string title, string message);
        private void SetAndShowBallonTip(string title, string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetAndShowBallonTipDelegate(SetAndShowBallonTip), new object[] { title, message });
                return;
            }

            this.notifyIcon.BalloonTipTitle = title;
            this.notifyIcon.BalloonTipText = message;
            this.notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            this.notifyIcon.ShowBalloonTip(10000);
        }

        private void OnRecallTimerAlarm(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (WorkSession == null || WorkSession.IsTerminated())
            {
                return;
            }

            string previousTitle = this.notifyIcon.BalloonTipTitle;
            string previousMessage = this.notifyIcon.BalloonTipText;

            string title = String.Format("Hey, {0}!", WorkSession.Architect.FirstName);
            string message = String.Format("Êtes-vous toujours en train de travailler sur la phase {0} du projet {1} ?",
                WorkSession.Phase.ToString(),
                WorkSession.Project.ToString());

            SetAndShowBallonTip(title, message);

            this.notifyIcon.BalloonTipTitle = previousTitle;
            this.notifyIcon.BalloonTipText = previousMessage;
        }


        // User events handling
        private void ButtonStartClick(object sender, EventArgs e)
        {
            if (!(comboBoxProjects.SelectedIndex > -1) || !(comboBoxPhases.SelectedIndex > -1))
            {
                MessageBox.Show(@"Choisissez un projet et une phase", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            ticTimer.Start();
            recallTimer.Start();

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
            ticTimer.Pause();
            recallTimer.Stop();

            WorkSession.StopTime = DateTime.Now;
            _service.EndWorkSession(WorkSession);

            comboBoxProjects.Enabled = true;
            comboBoxPhases.Enabled = true;
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;

            notifyIcon.Icon = Properties.Resources.tictac;
        }

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
            var configureForm = new ConfigureDatabase(new System.Drawing.Point(144, 120)) { FormBorderStyle = FormBorderStyle.FixedSingle };
            configureForm.Show();
        }

        private void trayMenuItemViewData_Click(object sender, EventArgs e)
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
