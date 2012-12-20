using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProjectTime
{
    public partial class Consult : Form
    {
        private readonly DbConnect _db;
        private readonly List<Project> _projectList;
        private readonly List<Phase> _phaseList;
        private readonly List<Architect> _architectsList;
        private Architect _currentArchitect;
        private Project _currentProject;
        private Phase _currentPhase;
        private readonly RecordWindow _mainWindow;

        public Consult(RecordWindow mainWindow)
        {
            InitializeComponent();
            _db = new DbConnect();
            _architectsList = _db.SelectAllArchitects();
            _projectList = _db.SelectAllProjects();
            _phaseList = _db.SelectAllPhases();

            _currentArchitect = null;
            _currentProject = null;
            _currentPhase = null;
            
            // Fill in the Architect, Project and Phases ComboBox
            comboBoxArchitects.Items.Add("Tous");
            comboBoxArchitects.Items.AddRange(_architectsList.ToArray());
            comboBoxArchitects.SelectedItem = comboBoxArchitects.Items[0];
            comboBoxProjects.Items.Add("Tous");
            comboBoxProjects.Items.AddRange(_projectList.ToArray());
            comboBoxProjects.SelectedItem = comboBoxProjects.Items[0];
            comboBoxPhases.Items.Add("Tous");
            comboBoxPhases.Items.AddRange(_phaseList.ToArray());
            comboBoxPhases.SelectedItem = comboBoxPhases.Items[0];

            pictureBox.Hide();

            _mainWindow = mainWindow;
        }

        private const int SnapDist = 200;
        private bool DoSnap(int pos1, int pos2)
        {
            int delta = Math.Abs(pos1 - pos2);
            return delta <= SnapDist;
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            Screen scn = Screen.FromPoint(this.Location);
            /*
            if (DoSnap(this.Left, scn.WorkingArea.Left)) this.Left = scn.WorkingArea.Left;
            if (DoSnap(this.Top, scn.WorkingArea.Top)) this.Top = scn.WorkingArea.Top;
            if (DoSnap(scn.WorkingArea.Right, this.Right)) this.Left = scn.WorkingArea.Right - this.Width;
            if (DoSnap(scn.WorkingArea.Bottom, this.Bottom)) this.Top = scn.WorkingArea.Bottom - this.Height;
             * */
            if (DoSnap(this.Top, _mainWindow.Bottom)) this.Top = _mainWindow.Bottom;
            if (DoSnap(this.Bottom, _mainWindow.Top)) this.Top = _mainWindow.Top - this.Height;
            if (DoSnap(this.Left, _mainWindow.Right)) this.Left = _mainWindow.Right;
            if (DoSnap(this.Right, _mainWindow.Left)) this.Left = _mainWindow.Left - this.Width;
            
        }

        private void ButtonValidateClick(object sender, EventArgs e)
        {
            pictureBox.Show();
            //System.Threading.Thread.Sleep(2000);
            var archiId = (_currentArchitect != null) ? _currentArchitect.Id : null;
            var projectId = (_currentProject != null) ? _currentProject.Id : null;
            var phaseId = (_currentPhase != null) ? _currentPhase.Id : null;

            var countSeconds = _db.GetTimeCount(archiId, projectId, phaseId);
            textBoxCountHours.Text = String.Format("{0:0.00}", (countSeconds / 3600));
            textBoxCountManMonth.Text = String.Format("{0:0.00}", (countSeconds / (3600*7*20)));
            pictureBox.Hide();
        }

        private void AddArchitectClick(object sender, EventArgs e)
        {
            var addForm = new AddArchitect(_architectsList, _mainWindow);
            addForm.Show();
        }

        private void AddProjectClick(object sender, EventArgs e)
        {
            //TODO
        }

        private void AddPhaseClick(object sender, EventArgs e)
        {
            //TODO
        }

        private void ComboBoxArchitectsSelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBoxArchitects.SelectedItem == "Tous")
            {
                _currentArchitect = null;
            }
            else
            {
                _currentArchitect = (Architect) comboBoxArchitects.SelectedItem;
            }
        }

        private void ComboBoxProjectsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxProjects.SelectedItem.ToString() == "Tous")
            {
                _currentProject = null;
            }
            else
            {
                _currentProject = (Project) comboBoxProjects.SelectedItem;
            }
        }

        private void ComboBoxPhasesSelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPhases.SelectedItem == "Tous")
            {
                _currentPhase = null;
            }
            else
            {
                _currentPhase = (Phase) comboBoxPhases.SelectedItem;
            }
        }
    }
}
