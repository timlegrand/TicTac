using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TicTac
{
    public partial class EditProject : Form
    {
        private readonly Service _service;
        private readonly DatabaseViewer _parent;
        private bool _updateOnly;
        private Project _currentProject;

        public EditProject(DatabaseViewer parent)
        {
            InitializeComponent();
            _parent = parent;
        }

        public EditProject(DatabaseViewer parent, Project currentProject)
        {
            InitializeComponent();
            this.Text = "Modifier un Projet";
            _service = Service.Instance;
            _parent = parent;
            textBoxProjectName.Text = currentProject.Name;
            textBoxDescription.Text = currentProject.Description;
            this._currentProject = currentProject;
            this._updateOnly = true;
        }

        private void SaveClick(object sender, EventArgs e)
        {
            var n = textBoxProjectName.Text;
            var d = textBoxDescription.Text;
            if (n.Length == 0)
            {
                MessageBox.Show("\"Name\" field is required", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var updatedProject = new Project() { Id = _currentProject.Id, Name = n, Description = d };
            var id = (_updateOnly == true) ?
                _service.EditProject((int)this._currentProject.Id, updatedProject) :
                _service.AddProject(updatedProject);
            Console.WriteLine("inserted id = {0}", id);

            _service.ProjectList.Single(p => ((int)p.Id) == (int)id).CopyIn(updatedProject);
            _parent.UpdateData();
            Hide();
        }
    }
}
