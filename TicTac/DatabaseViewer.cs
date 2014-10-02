using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.ComponentModel;

namespace TicTac
{
    public partial class DatabaseViewer : Form
    {
        private Architect _currentArchitect;
        private Project _currentProject;
        private Phase _currentPhase;
        public readonly RecordWindow _parent;

        public DatabaseViewer(RecordWindow mainWindow)
        {
            InitializeComponent();
            InitializeData();

            busyAnimation.Hide();
            _parent = mainWindow;
            ;

            int id = (_currentArchitect != null && _currentArchitect.Id != null) ? (int)_currentArchitect.Id : 0;
            DataTable data = Service.Instance.GetWorkSessionDataTable(id);

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = data;

            dataGridView1.Sort(dataGridView1.Columns[0], System.ComponentModel.ListSortDirection.Descending);
        }

        private void InitializeData()
        {
            _currentArchitect = null;
            _currentProject = null;
            _currentPhase = null;

            // Fill in the Architect, Project and Phases ComboBox
            comboBoxArchitects.Items.Clear();
            comboBoxArchitects.Items.Add("Tous");
            comboBoxArchitects.Items.AddRange(Service.Instance.ArchitectList.ToArray());
            comboBoxArchitects.SelectedItem = comboBoxArchitects.Items[0];
            comboBoxProjects.Items.Clear();
            comboBoxProjects.Items.Add("Tous");
            comboBoxProjects.Items.AddRange(Service.Instance.ProjectList.ToArray());
            comboBoxProjects.SelectedItem = comboBoxProjects.Items[0];
            comboBoxPhases.Items.Clear();
            comboBoxPhases.Items.Add("Tous");
            comboBoxPhases.Items.AddRange(Service.Instance.PhaseList.ToArray());
            comboBoxPhases.SelectedItem = comboBoxPhases.Items[0];
        }

        public void UpdateData()
        {
            InitializeData();
            _parent.InitComboboxes();
        }

        delegate void ShowBusyAnimationDelegate();
        private void ShowBusyAnimation(object sender, DoWorkEventArgs e)
        {
            this.BeginInvoke(new ShowBusyAnimationDelegate(busyAnimation.Show), null);
        }

        delegate void LongOperationDelegate();
        private void LongOperation(object sender, DoWorkEventArgs e)
        {            
            var archiId = (_currentArchitect != null) ? _currentArchitect.Id : null;
            var projectId = (_currentProject != null) ? _currentProject.Id : null;
            var phaseId = (_currentPhase != null) ? _currentPhase.Id : null;

            var timeSpan = Service.Instance.SelectTimeCount(archiId, projectId, phaseId);

            this.BeginInvoke(new UpdateTextBoxCountDelegate(UpdateTextBoxCount), timeSpan);
        }

        delegate void UpdateTextBoxCountDelegate(TimeSpan timeSpan);
        private void UpdateTextBoxCount(TimeSpan timeSpan)
        {
            this.textBoxCountHours.Text = String.Format("{0:0.00}", timeSpan.TotalHours);
            this.textBoxCountManMonth.Text = String.Format("{0:0.00}", timeSpan.TotalDays);
        }
                    
        delegate void HideBusyAnimationDelegate();
        private void HideBusyAnimation(object sender, RunWorkerCompletedEventArgs e)
        {
            _parent.BeginInvoke(new HideBusyAnimationDelegate(busyAnimation.Hide), null);
        }

        private void ButtonValidateClick(object sender, EventArgs e)
        {
            var bgw = new BackgroundWorker();
            bgw.DoWork += ShowBusyAnimation;
            bgw.DoWork += LongOperation;
            bgw.RunWorkerCompleted += HideBusyAnimation;
            bgw.RunWorkerAsync();
        }

        private void AddArchitectClick(object sender, EventArgs e)
        {
            var form = new EditArchitect(this);
            form.Show();
        }

        private void AddProjectClick(object sender, EventArgs e)
        {
            var form = new EditProject(this);
            form.Show();
        }

        private void AddPhaseClick(object sender, EventArgs e)
        {
            var form = new EditPhase(this);
            form.Show();
        }

        private void EditArchitectButtonClick(object sender, EventArgs e)
        {
            if (_currentArchitect == null) return;
            var form = new EditArchitect(this, _currentArchitect);
            form.Show();
        }

        private void EditProjectButtonClick(object sender, EventArgs e)
        {
            if (_currentProject == null) return;
            var form = new EditProject(this, _currentProject);
            form.Show();
        }

        private void EditPhaseButtonClick(object sender, EventArgs e)
        {
            if (_currentPhase == null) return;
            var form = new EditPhase(this, _currentPhase);
            form.Show();
        }

        private void DeleteArchitectButtonClick(object sender, EventArgs e)
        {
            if (
                MessageBox.Show(
                    "Êtes vous sûr de vouloir supprimer l'achitecte " + _currentArchitect.ToString() + " ? Tous les enregistrements le concernant seront perdus.",
                    "Confirmer la suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var success = Service.Instance.DeleteArchitect(_currentArchitect);
                if (!success) throw new DataException();
                UpdateData();
            }
        }

        private void DeleteProjectButtonClick(object sender, EventArgs e)
        {
            if (
                MessageBox.Show("Êtes vous sûr de vouloir supprimer le projet " + _currentProject.ToString() + "? Tous les enregistrements le concernant seront perdus.",
                                "Confirmer la suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var success = Service.Instance.DeleteProject(_currentProject);
                if (!success) throw new DataException();
                UpdateData();
            }
        }

        private void DeletePhaseButtonClick(object sender, EventArgs e)
        {
            if (
                MessageBox.Show("Êtes vous sûr de vouloir supprimer la phase " + _currentPhase.ToString() + "? Tous les enregistrements la concernant seront perdus.",
                                "Confirmer la suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var success = Service.Instance.DeletePhase(_currentPhase);
                if (!success) throw new DataException();
                UpdateData();
            }
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

        private bool closeEnough(DatabaseViewer consult, RecordWindow mainWindow)
        {
            var rwCenter = new Point((mainWindow.Top + mainWindow.Bottom)/2, (mainWindow.Left + mainWindow.Right)/2);
            var cwCenter = new Point((consult.Top + consult.Bottom)/2, (consult.Left + consult.Right)/2);
            var dist = Math.Sqrt(Math.Pow(rwCenter.X - cwCenter.X, 2) + Math.Pow(rwCenter.Y - cwCenter.Y, 2));
            return dist <= 400;
        }

        private void EditDatabaseButtonClick(object sender, EventArgs e)
        {
            var configureForm = new ConfigureDatabase(this) { FormBorderStyle = FormBorderStyle.FixedSingle };
            configureForm.Show();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {

        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {

        }
    }
}
