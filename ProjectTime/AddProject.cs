using System;
using System.Windows.Forms;

namespace TicTac
{
    public partial class AddProject : Form
    {
        private readonly DAOClient _db;
        private readonly DatabaseViewer _parent;

        public AddProject(DatabaseViewer parent)
        {
            InitializeComponent();
            _db = new DAOClient();
            _parent = parent;
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

            var id = _db.InsertProject(new Project() { Name = n, Description = d });
            Console.WriteLine("Project inserted id = {0}", id);

            _parent.UpdateData();
            Hide();
        }
    }
}
