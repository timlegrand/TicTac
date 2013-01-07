using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ProjectTime
{
    public partial class Consult : Form
    {
        private readonly DbConnection _db;
        private List<Project> _projectList;
        private List<Phase> _phaseList;
        private List<Architect> _architectsList;
        private Architect _currentArchitect;
        private Project _currentProject;
        private Phase _currentPhase;
        private readonly RecordWindow _mainWindow;

        public Consult(RecordWindow mainWindow)
        {
            InitializeComponent();
            _db = new DbConnection(Program.ServerIp, "he", "he", "mySqlUserPassword");
            InitializeData();

            pictureBox.Hide();
            _mainWindow = mainWindow;
        }

        private void InitializeData()
        {
            
            _architectsList = _db.SelectAllArchitects();
            _projectList = _db.SelectAllProjects();
            _phaseList = _db.SelectAllPhases();

            _currentArchitect = null;
            _currentProject = null;
            _currentPhase = null;

            // Fill in the Architect, Project and Phases ComboBox
            comboBoxArchitects.Items.Clear();
            comboBoxArchitects.Items.Add("Tous");
            comboBoxArchitects.Items.AddRange(_architectsList.ToArray());
            comboBoxArchitects.SelectedItem = comboBoxArchitects.Items[0];
            comboBoxProjects.Items.Clear();
            comboBoxProjects.Items.Add("Tous");
            comboBoxProjects.Items.AddRange(_projectList.ToArray());
            comboBoxProjects.SelectedItem = comboBoxProjects.Items[0];
            comboBoxPhases.Items.Clear();
            comboBoxPhases.Items.Add("Tous");
            comboBoxPhases.Items.AddRange(_phaseList.ToArray());
            comboBoxPhases.SelectedItem = comboBoxPhases.Items[0];
        }

        public void UpdateData()
        {
            InitializeData();
        }

        private const int SnapDist = 20;
        private bool DoSnap(int pos1, int pos2)
        {
            int delta = Math.Abs(pos1 - pos2);
            return delta <= SnapDist;
        }

        private void ButtonValidateClick(object sender, EventArgs e)
        {
            pictureBox.Show();
            //System.Threading.Thread.Sleep(2000);
            var archiId = (_currentArchitect != null) ? _currentArchitect.Id : null;
            var projectId = (_currentProject != null) ? _currentProject.Id : null;
            var phaseId = (_currentPhase != null) ? _currentPhase.Id : null;

            var timeSpan = _db.GetTimeCount(archiId, projectId, phaseId);
            textBoxCountHours.Text = String.Format("{0:0.00}",timeSpan.TotalHours);
            textBoxCountManMonth.Text = String.Format("{0:0.00}",timeSpan.TotalDays);
            pictureBox.Hide();
        }

        private void AddArchitectClick(object sender, EventArgs e)
        {
            var form = new AddArchitect(this);
            form.Show();
        }

        private void AddProjectClick(object sender, EventArgs e)
        {
            var form = new AddProject(this);
            form.Show();
        }

        private void AddPhaseClick(object sender, EventArgs e)
        {
            var form = new AddPhase(this);
            form.Show();
        }

        private void ComboBoxArchitectsSelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBoxArchitects.SelectedItem.ToString() == "Tous")
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
            if (comboBoxPhases.SelectedItem.ToString() == "Tous")
            {
                _currentPhase = null;
            }
            else
            {
                _currentPhase = (Phase) comboBoxPhases.SelectedItem;
            }
        }

        private void OnMove(object sender, EventArgs e)
        {
            /*
            Screen scn = Screen.FromPoint(this.Location);
            if (closeEnough(this, _mainWindow))
            {
                if (DoSnap(this.Top, _mainWindow.Bottom)) this.Top = _mainWindow.Bottom;
                if (DoSnap(this.Bottom, _mainWindow.Top)) this.Top = _mainWindow.Top - this.Height;
                if (DoSnap(this.Left, _mainWindow.Right)) this.Left = _mainWindow.Right;
                if (DoSnap(this.Right, _mainWindow.Left)) this.Left = _mainWindow.Left - this.Width;
                if (DoSnap(this.Top, _mainWindow.Top)) this.Top = _mainWindow.Top;
                if (DoSnap(this.Bottom, _mainWindow.Bottom)) this.Top = _mainWindow.Bottom - this.Height;
                if (DoSnap(this.Left, _mainWindow.Left)) this.Left = _mainWindow.Left;
                if (DoSnap(this.Right, _mainWindow.Right)) this.Left = _mainWindow.Right - this.Width;
            }
            */
        }

        private bool closeEnough(Consult consult, RecordWindow mainWindow)
        {
            var rwCenter = new Point((mainWindow.Top + mainWindow.Bottom)/2, (mainWindow.Left + mainWindow.Right)/2);
            var cwCenter = new Point((consult.Top + consult.Bottom) / 2, (consult.Left + consult.Right) / 2);
            var dist = Math.Sqrt(Math.Pow(rwCenter.X - cwCenter.X, 2) + Math.Pow(rwCenter.Y - cwCenter.Y, 2));
            return dist <= 400;
        }

        private void DeleteArchitectButtonClick(object sender, EventArgs e)
        {
            if (MessageBox.Show("Êtes vous sûr de vouloir supprimer l'achitecte " + _currentArchitect.ToString() + " ?", "Confirmer la suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var success = _db.DeleteArchitect(_currentArchitect);
                if (!success) throw new DataException();
                UpdateData();
            }
        }

        private void DeleteProjectButtonClick(object sender, EventArgs e)
        {
            if (MessageBox.Show("Êtes vous sûr de vouloir supprimer le projet " + _currentProject.ToString() + "?", "Confirmer la suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var success = _db.DeleteProject(_currentProject);
                if (!success) throw new DataException();
                UpdateData();
            }
        }

        private void DeletePhaseButtonClick(object sender, EventArgs e)
        {
            if (MessageBox.Show("Êtes vous sûr de vouloir supprimer la phase " + _currentPhase.ToString() + "?", "Confirmer la suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var success = _db.DeletePhase(_currentPhase);
                if (!success) throw new DataException();
                UpdateData();
            }
        }
    }
}
