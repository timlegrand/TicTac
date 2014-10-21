using System;
using System.Windows.Forms;

namespace TicTac
{
    public partial class EditProject : Form
    {
        private readonly DatabaseViewer _parentWindow;
        private readonly Project _currentProject; 
        private readonly bool _updateOnly;

        public EditProject(DatabaseViewer parent)
        {
            InitializeComponent();
            _parentWindow = parent;
        }

        public EditProject(DatabaseViewer parent, Project currentProject) : this(parent)
        {
            Text = "Modifier un Projet";
            textBoxProjectName.Text = currentProject.Name;
            textBoxDescription.Text = currentProject.Description;
            _currentProject = currentProject;
            _updateOnly = true;
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
            var id = (_updateOnly == true && _currentProject.Id != null) ?
                Service.Instance.EditProject((int)_currentProject.Id, updatedProject) :
                Service.Instance.AddProject(updatedProject);
            Console.WriteLine("inserted id = {0}", id);

            _parentWindow.UpdateData();
            Hide();
        }
    }
}
