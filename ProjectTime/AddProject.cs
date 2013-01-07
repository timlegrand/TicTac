using System;
using System.Windows.Forms;

namespace ProjectTime
{
    public partial class AddProject : Form
    {
        private readonly DbConnection _db;
        private readonly Consult _parent;

        public AddProject(Consult parent)
        {
            InitializeComponent();
            _db = new DbConnection(Program.ServerIp, "he", "he", "mySqlUserPassword");
            _parent = parent;
        }

        private void SaveClick(object sender, EventArgs e)
        {
            var n = textBoxProjectName.Text;
            if (n.Length == 0)
            {
                MessageBox.Show(@"Please fill-in every fields", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var id = _db.InsertProject(new Project() { Name = n });
            Console.WriteLine("Project inserted id = {0}", id);

            _parent.UpdateData();
            Hide();
        }
    }
}
