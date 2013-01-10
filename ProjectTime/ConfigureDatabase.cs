using System;
using System.Windows.Forms;

namespace ProjectTime
{
    public partial class ConfigureDatabase : Form
    {
        private DbConnection _db;

        public ConfigureDatabase()
        {
            InitializeComponent();

            textBoxServer.Text = Program.DbServerIp;
            textBoxDatabase.Text = Program.DbName;
            textBoxUserName.Text = Program.DbUserName;
            textBoxPassword.Text = Program.DbPassword;
        }

        private void ButtonSaveClick(object sender, EventArgs e)
        {
            if (!IsValid())
            {
                MessageBox.Show(@"Veuillez renseigner tous les champs", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            /*_db = new DbConnection(
                labelServerNameOrIp.Text,
                labelDatabaseName.Text,
                labelUserName.Text,
                labelPassword.Text);
            Program.VarDump(_db);*/

            PropagateDatabaseConfiguration();
            Hide();
        }

        private void PropagateDatabaseConfiguration()
        {
            //Program.Db = _db;
            Program.DbServerIp = textBoxServer.Text;
            Program.DbName = textBoxDatabase.Text;
            Program.DbUserName = textBoxUserName.Text;
            Program.DbPassword = textBoxPassword.Text;
        }

        private bool IsValid()
        {
            return textBoxServer.Text != string.Empty &&
                   textBoxDatabase.Text != string.Empty &&
                   textBoxUserName.Text != string.Empty &&
                   textBoxPassword.Text != string.Empty;
        }
    }
}
