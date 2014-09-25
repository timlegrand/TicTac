using System;
using System.Windows.Forms;

namespace TicTac
{
    public partial class EditProject : Form
    {
        private readonly DatabaseViewer parentWindow;
        private Project currentProject; 
        private bool updateOnly;

        public EditProject(DatabaseViewer parent)
        {
            InitializeComponent();
            this.parentWindow = parent;
        }

        public EditProject(DatabaseViewer parent, Project currentProject) : this(parent)
        {
            this.Text = "Modifier un Projet";
            this.textBoxProjectName.Text = currentProject.Name;
            this.textBoxDescription.Text = currentProject.Description;
            this.currentProject = currentProject;
            this.updateOnly = true;
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

            var updatedProject = new Project() { Id = currentProject.Id, Name = n, Description = d };
            var id = (updateOnly == true) ?
                Service.Instance.EditProject((int)currentProject.Id, updatedProject) :
                Service.Instance.AddProject(updatedProject);
            Console.WriteLine("inserted id = {0}", id);

            this.parentWindow.UpdateData();
            Hide();
        }
    }
}
