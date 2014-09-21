using System;
using System.Windows.Forms;
using TicTac.DAO;
using System.Security.Cryptography;
using System.Text;

namespace TicTac
{
    public partial class ConfigureDatabase : Form
    {
        private readonly DatabaseViewer _parent;
        private string hPassword;

        public ConfigureDatabase(DatabaseViewer consult)
        {
            InitializeComponent();

            _parent = consult;
            textBoxServer.Text   = Database.DbServerIp;
            textBoxDatabase.Text = Database.DbName;
            textBoxUserName.Text = Database.DbUserName;
            textBoxPassword.Text = "*********";
            hPassword = null;
        }

        private void ButtonSaveClick(object sender, EventArgs e)
        {
            hPassword = SHA1Util.SHA1HashStringForUTF8String(textBoxPassword.Text);
            ;

            if (!IsValid())
            {
                MessageBox.Show(@"Veuillez renseigner tous les champs", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!Program.CheckDatabaseConnexionAvailable(textBoxServer.Text))
            {
                MessageBox.Show(@"Base de données inaccessible. Vérifiez l'adresse ou le nom du serveur, ou bien vérifiez que celui-ci est démarré.", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            PropagateDatabaseConfiguration();
            Hide();
        }
       
        private void PropagateDatabaseConfiguration()
        {
            Database.DbServerIp = textBoxServer.Text;
            Database.DbName     = textBoxDatabase.Text;
            Database.DbUserName = textBoxUserName.Text;
            Database.DbPassword = hPassword;
        }

        private bool IsValid()
        {
            return textBoxServer.Text   != string.Empty &&
                   textBoxDatabase.Text != string.Empty &&
                   textBoxUserName.Text != string.Empty &&
                   textBoxPassword.Text != string.Empty;
        }
    }
}
