using System;
using System.Windows.Forms;

namespace TicTac
{
    public partial class AddProject : Form
    {
        private readonly DbConnection _db;
        private readonly Consult _parent;

        public AddProject(Consult parent)
        {
            InitializeComponent();
            _db = new DbConnection();
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
