using System;
using System.Drawing;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using TicTac.Properties;


namespace TicTac
{
    public partial class RecordWindow : CommonForm
    {
        private Service _service;
        private readonly Preferences _preferences;
        public WorkSession WorkSession { get; private set; }
        private readonly TicTimer _ticTimer;
        private readonly TicTimer _recallTimer;

        public RecordWindow()
        {
            _preferences = new DualModePreferences();
            _preferences.Load();

            Database.WaitForConnectivity();
            Service.StartAsync();

            _ticTimer = new TicTimer(OnTimerTick, 1000, true);
            _recallTimer = new TicTimer(OnRecallTimerAlarm, 900000); // Every 15 minutes

            InitializeComponent();
            Initialize();
        }

        public RecordWindow(Point p)
            : this()
        {
            busyAnimation.Location = p;
        }

        private void Initialize()
        {
            Text += @" (v " + Program.CurrentVersion + @")";

            notifyIcon.Icon = Icon;

            StartPosition = FormStartPosition.Manual;
            Location = BoundLocation(_preferences.StartLocation);

            // Following needs Service to be initialized
            _service = Service.Ready();
            Program.Clk.Probe("SERVICE READY");

            SuspendLayout();
            InitComboboxes();
            InitButtons();
            ResumeLayout();

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
                int id = _preferences.LastArchitect != null ? _preferences.LastArchitect.Id ?? 0 : 0;
                var item = comboBoxArchitects.Items.Cast<Architect>().FirstOrDefault(archi => archi.Id == id);
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
                _service.ProjectList.Sort((p1,p2)=>String.Compare(p1.Name, p2.Name, StringComparison.Ordinal));
                _service.ProjectList.Reverse();
                comboBoxProjects.DataSource = _service.ProjectList;
                //comboBoxProjects.DisplayMember = "Name";
                //comboBoxProjects.ValueMember = "Id";

                // Reselect last project
                var id = _preferences.LastProject != null ? _preferences.LastProject.Id ?? 0 : 0;
                var item = comboBoxProjects.Items.Cast<Project>().FirstOrDefault(project => project.Id == id);
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
                _service.PhaseList.Sort((p1, p2) => String.Compare(p1.Name, p2.Name, StringComparison.Ordinal));
                comboBoxPhases.DataSource = _service.PhaseList;
                //comboBoxProjects.DisplayMember = "Name";
                //comboBoxProjects.ValueMember = "Id";

                // Reselect last phase
                var id = _preferences.LastPhase != null ? _preferences.LastPhase.Id ?? 0 : 0;
                var item = comboBoxPhases.Items.Cast<Phase>().FirstOrDefault(phase => phase.Id == id);
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
                notifyIcon.Icon = Resources.tictac_on;
            }
            else // hope that return value is 0 (big problem otherwise)
            {
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
                notifyIcon.Icon = Resources.tictac;
            }
        }

        private WorkSession RestoreSession(Architect archi)
        {
            // 1- Try to retrieve one single open Session in DB
            var session = _service.GetStartedWorkSessions(archi).SingleOrDefault();
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

                _ticTimer.Start(DateTime.Now - session.StartTime);
                _recallTimer.Start();

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
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                notifyIcon = null;
            }
        }


        // Time events handling
        private delegate void SetLabelTimeTextDelegate(string newLabel);
        private void SetLabelTimeText(string newLabel)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SetLabelTimeTextDelegate(SetLabelTimeText), new object[] { newLabel });
                return;
            }

            labelTime.Text = newLabel;
        }

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            var s = string.Format("{0:0} jour(s) et {1:00}:{2:00}:{3:00}",
                _ticTimer.Elapsed.TotalDays,
                _ticTimer.Elapsed.Hours,
                _ticTimer.Elapsed.Minutes,
                _ticTimer.Elapsed.Seconds);

            SetLabelTimeText(s);
        }

        private delegate void SetAndShowBallonTipDelegate(string title, string message);
        private void SetAndShowBallonTip(string title, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new SetAndShowBallonTipDelegate(SetAndShowBallonTip), new object[] { title, message });
                return;
            }

            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = message;
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.ShowBalloonTip(10000);
        }

        private void OnRecallTimerAlarm(object sender, ElapsedEventArgs e)
        {
            if (WorkSession == null || WorkSession.IsTerminated())
            {
                return;
            }

            string previousTitle = notifyIcon.BalloonTipTitle;
            string previousMessage = notifyIcon.BalloonTipText;

            string title = String.Format("Hey, {0}!", WorkSession.Architect.FirstName);
            string message = String.Format("Êtes-vous toujours en train de travailler sur la phase {0} du projet {1} ?",
                WorkSession.Phase.ToString(),
                WorkSession.Project.ToString());

            SetAndShowBallonTip(title, message);

            notifyIcon.BalloonTipTitle = previousTitle;
            notifyIcon.BalloonTipText = previousMessage;
        }


        // User events handling
        private void ButtonStartClick(object sender, EventArgs e)
        {
            if (!(comboBoxProjects.SelectedIndex > -1) || !(comboBoxPhases.SelectedIndex > -1))
            {
                MessageBox.Show(@"Choisissez un projet et une phase", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            _ticTimer.Start();
            _recallTimer.Start();

            WorkSession.StartTime = DateTime.Now;
            _service.StartWorkSession(WorkSession);

            comboBoxProjects.Enabled = false;
            comboBoxPhases.Enabled = false;
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;

            notifyIcon.Icon = Resources.tictac_on;
        }
        
        private void ButtonStopClick(object sender, EventArgs e)
        {
            _ticTimer.Pause();
            _recallTimer.Stop();

            WorkSession.StopTime = DateTime.Now;
            _service.EndWorkSession(WorkSession);

            comboBoxProjects.Enabled = true;
            comboBoxPhases.Enabled = true;
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;

            notifyIcon.Icon = Resources.tictac;
        }

        private void RecordWindow_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                notifyIcon.ShowBalloonTip(500);
                Hide();
            }
            else
            {
                Show();
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
            if (WindowState == FormWindowState.Normal)
            {
                Hide();
                WindowState = FormWindowState.Minimized;
                notifyIcon.ShowBalloonTip(500);
            }
            // Show
            else if (WindowState == FormWindowState.Minimized)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
        }

        private void notifyIconMenuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void trayMenuItemMinimize_Click(object sender, EventArgs e)
        {
            // Hide
            if (WindowState == FormWindowState.Normal)
            {
                Hide();
                WindowState = FormWindowState.Minimized;
                notifyIcon.ShowBalloonTip(500);
            }
        }

        private void trayMenuItemOpen_Click(object sender, EventArgs e)
        {
            // Show
            if (WindowState == FormWindowState.Minimized)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
        }

        private void trayMenuItemConfigure_Click(object sender, EventArgs e)
        {
            var configureForm = new ConfigureDatabase(new Point(144, 120)) { FormBorderStyle = FormBorderStyle.FixedSingle };
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
