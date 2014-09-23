﻿using System;
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
        private Preferences _prefs;
        private WorkSession _ws;

        //Constructor
        public RecordWindow()
        {
            InitializeComponent();
Program.clk.Probe("InitializeComponent");
            Initialize();
Program.clk.Probe("Initialize");
Program.clk.Print();
        }

        private void Initialize()
        {
Program.clk.Probe();
            this.notifyIcon.Icon = this.Icon;

            // Load configuration, including Default information
            _prefs = new Preferences(this);
            _prefs.Load();
            StartPosition = FormStartPosition.Manual;
            Location = _prefs.StartLocation;
Program.clk.Probe();

            _service = new Service();
Program.clk.Probe();
            _dao = new DAOClient();
Program.clk.Probe();
            InitComboboxes();
Program.clk.Probe();
            InitButtons(); // Actually useless since called above by "comboBoxArchitects.SelectedItem changed" events
        }

        // Retrieve Comboboxes data
        private void InitComboboxes()
        {
            // Fill in the ComboBoxes
            if (_service.ProjectList != null && _service.ProjectList.Count() != 0)
            {
                // Sort descending
                _service.ProjectList.Sort((p1,p2)=>p1.Name.CompareTo(p2.Name));
                _service.ProjectList.Reverse();
                comboBoxProjects.Items.AddRange(_service.ProjectList.ToArray());
                if (_prefs.LastProject != null)
                {
                    int i;
                    for ( i = 0; i < comboBoxProjects.Items.Count; i++)
                    {
                        var p = (Project)comboBoxProjects.Items[i];
                        if (_prefs.LastProject.Equals(p))
                        {
                            comboBoxProjects.SelectedItem = comboBoxProjects.Items[i];
                            break;
                        }
                    }
                }
                else
                {
                    comboBoxProjects.SelectedItem = comboBoxProjects.Items[0];
                }
            }

            if (_service.PhaseList != null && _service.PhaseList.Count() != 0)
            {
                comboBoxPhases.Items.AddRange(_service.PhaseList.ToArray());
                if (_prefs.LastPhase != null)
                {
                    int i;
                    for ( i = 0; i < comboBoxPhases.Items.Count; i++)
                    {
                        if (_prefs.LastPhase.Equals((Phase)comboBoxPhases.Items[i]))
                        {
                            comboBoxPhases.SelectedItem = comboBoxPhases.Items[i];
                            break;
                        }
                    }
                }
                else
                {
                    comboBoxPhases.SelectedItem = comboBoxPhases.Items[0];
                }
            }

            if (_service.ArchitectList != null && _service.ArchitectList.Count() != 0)
            {
                comboBoxArchitects.Items.AddRange(_service.ArchitectList.ToArray());
                // Must be done LAST because of event management
                if (_prefs.LastArchitect != null)
                {
                    // TODO make it work with Linq (see http://msdn.microsoft.com/fr-fr/library/vstudio/system.windows.forms.combobox.objectcollection.aspx)
                    //var item = (ComboBox.ObjectCollection) from Architect elem in comboBoxArchitects.Items
                    //           where elem.Id == LastArchitect.Id
                    //           select elem;
                    var i = 0;
                    while (!_prefs.LastArchitect.Equals((Architect)comboBoxArchitects.Items[i]))
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
            // TODO make file-based session available !!
            if (!Program.DatabaseConnexionAvailable) return null;
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
        private void ComboBoxArchitectsSelectedIndexChanged(object sender, EventArgs e)
        {
            var archi = (Architect)comboBoxArchitects.SelectedItem;

            var daily = _service.GetDailyWorkSessions(archi);
            foreach (WorkSession ws in daily)
            {
                Console.WriteLine(ws.ToString());
            }

            _ws = RestoreSession(archi) ?? new WorkSession
                {
                    Architect = archi,
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

            notifyIcon.Icon = Properties.Resources.tictac_on;
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

            notifyIcon.Icon = Properties.Resources.tictac_on;
        }


        // Termination
        private void RecordWindowFormClosed(object sender, FormClosedEventArgs e)
        {
            var prefs = new Preferences(this)
                {
                    StartLocation = Location,
                    LastArchitect = (Architect) comboBoxArchitects.SelectedItem,
                    LastProject = (Project)comboBoxProjects.SelectedItem,
                    LastPhase = (Phase)comboBoxPhases.SelectedItem,
                };
            if (!prefs.IsValid()) return;
#if (DEBUG)            
            Console.WriteLine(@"Writing " + Preferences.PreferencesFileName + @"...");
#endif
            prefs.Save();
#if (DEBUG)
            Console.WriteLine(Preferences.PreferencesFileName + @" written.");
#endif

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
            var consultForm = new DatabaseViewer(this) { FormBorderStyle = FormBorderStyle.FixedSingle };
            consultForm.Show();
        }
    }
}
