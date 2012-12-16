using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProjectTime
{
    public partial class Consult : Form
    {
        private readonly DBConnect _db;
        private readonly List<Project> _projectDb;
        private readonly List<Phase> _phaseDb;
        private readonly List<Architect> _architectsDb;
        private Architect _currentArchitect;
        private Project _currentProject;
        private Phase _currentPhase;
        private readonly RecordWindow _mainWindow;

        public Consult(RecordWindow mainWindow)
        {
            InitializeComponent();
            _db = new DBConnect();
            _architectsDb = _db.SelectAllArchitects();
            _projectDb = _db.SelectAllProjects();
            _phaseDb = _db.SelectAllPhases();

            _currentArchitect = null;
            _currentProject = null;
            _currentPhase = null;
            
            // Fill in the Architect, Project and Phases ComboBox
            comboBoxArchitects.Items.Add("Tous");
            comboBoxArchitects.Items.AddRange(_architectsDb.ToArray());
            comboBoxArchitects.SelectedItem = comboBoxArchitects.Items[0];
            comboBoxProjects.Items.Add("Tous");
            comboBoxProjects.Items.AddRange(_projectDb.ToArray());
            comboBoxProjects.SelectedItem = comboBoxProjects.Items[0];
            comboBoxPhases.Items.Add("Tous");
            comboBoxPhases.Items.AddRange(_phaseDb.ToArray());
            comboBoxPhases.SelectedItem = comboBoxPhases.Items[0];

            _mainWindow = mainWindow;
        }

        private void ButtonValidateClick(object sender, EventArgs e)
        {
            var archiId = (_currentArchitect != null) ? _currentArchitect.Id : null;
            var projectId = (_currentProject != null) ? _currentProject.Id : null;
            var phaseId = (_currentPhase != null) ? _currentPhase.Id : null;

            var countSeconds = _db.GetTimeCount(archiId, projectId, phaseId);
            textBoxCountHours.Text = String.Format("{0:0.00}", (countSeconds / 3600));
            textBoxCountManMonth.Text = String.Format("{0:0.00}", (countSeconds / (3600*7*20)));
        }

        private void AddArchitectClick(object sender, EventArgs e)
        {
            var addForm = new AddArchitect(_architectsDb, _mainWindow);
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
            if (comboBoxProjects.SelectedItem == "Tous")
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
