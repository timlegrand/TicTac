using System;
using System.Windows.Forms;

namespace ProjectTime
{
    public partial class ConfigureDatabase : Form
    {
        private DbConnection _db;
        private readonly Consult _parent;

        public ConfigureDatabase(Consult consult)
        {
            InitializeComponent();

            _parent = consult;
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
            if (!Program.IsDatabaseConnexionAvailable(textBoxServer.Text))
            {
                MessageBox.Show(@"Base de données inaccessible. Vérifiez l'adresse ou le nom du serveur, ou bien vérifiez que celui-ci est démarré.", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            PropagateDatabaseConfiguration();
            Hide();
        }

        private void PropagateDatabaseConfiguration()
        {
            Program.DbServerIp = textBoxServer.Text;
            Program.DbName = textBoxDatabase.Text;
            Program.DbUserName = textBoxUserName.Text;
            Program.DbPassword = textBoxPassword.Text;
            _parent.UpdateDb();
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
