using System;
using System.Windows.Forms;
using TicTac.DAO;
using System.Security.Cryptography;
using System.Text;

namespace TicTac
{
    public partial class ConfigureDatabase : CommonForm
    {
        private string hPassword;


        public ConfigureDatabase()
            : base()
        {
            InitializeComponent();

            textBoxServer.Text   = Database.ServerAddress ?? "";
            textBoxDatabase.Text = Database.Name ?? "";
            textBoxUserName.Text = Database.UserName ?? "";
            textBoxPassword.Text = Database.Password != null && Database.Password != string.Empty ? "*********" : "";
            hPassword = null;
        }

        public ConfigureDatabase(System.Drawing.Point p)
            : this()
        {
            this.busyAnimation.Location = p;
        }

        private void ButtonSaveClick(object sender, EventArgs e)
        {
            buttonSave.Enabled = false;
            hPassword = SHA1Util.SHA1HashStringForUTF8String(textBoxPassword.Text);
            ;

            if (!IsValid())
            {
                MessageBox.Show("Veuillez renseigner tous les champs",
                    @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                buttonSave.Enabled = true;
                return;
            }
            if (!Database.CheckConnexion(textBoxServer.Text, textBoxDatabase.Text, textBoxUserName.Text, hPassword))
            {
                MessageBox.Show("Base de données inaccessible. Vérifiez vos informations, notamment " +
                    "l'adresse du serveur (qui peut être une url ou une IP) ou vérifiez que ce dernier est démarré.",
                    @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                buttonSave.Enabled = true;
                return;
            }

            PropagateDatabaseConfiguration();
            Hide();
        }
       
        private void PropagateDatabaseConfiguration()
        {
            Database.ServerAddress = textBoxServer.Text;
            Database.Name     = textBoxDatabase.Text;
            Database.UserName = textBoxUserName.Text;
            Database.Password = hPassword;
        }

        private bool IsValid()
        {
            return textBoxServer.Text   != string.Empty &&
                   textBoxDatabase.Text != string.Empty &&
                   textBoxUserName.Text != string.Empty &&
                   textBoxPassword.Text != string.Empty;
        }

        private void textBoxServer_Enter(object sender, EventArgs e)
        {
            // Kick off SelectAll asyncronously so that it occurs after Click
            BeginInvoke((Action)delegate
            {
                textBoxServer.SelectAll();
            });
        }

        private void textBoxDatabase_Enter(object sender, EventArgs e)
        {
            // Kick off SelectAll asyncronously so that it occurs after Click
            BeginInvoke((Action)delegate
            {
                textBoxDatabase.SelectAll();
            });
        }

        private void textBoxUserName_Enter(object sender, EventArgs e)
        {
            // Kick off SelectAll asyncronously so that it occurs after Click
            BeginInvoke((Action)delegate
            {
                textBoxUserName.SelectAll();
            });
        }

        private void textBoxPassword_Enter(object sender, EventArgs e)
        {
            // Kick off SelectAll asyncronously so that it occurs after Click
            BeginInvoke((Action)delegate
            {
                textBoxPassword.SelectAll();
            });
        }

        private void ConfigureDatabase_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.None)
            {
                // Resume closing
                return;
            }

            // Confirm user wants to close
            DialogResult dr = MessageBox.Show(this, "Il n'est pas encore possible d'utiliser TicTac sans configurer l'accès à la base de données." 
            + "\n\nSouhaitez-vous quitter TicTac ?"
            , "Impossible", MessageBoxButtons.OKCancel);
            switch (dr)
            {
                case DialogResult.OK:
                    Database.Abort = true;
                    this.Dispose();
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
                default:
                    break;
            }
        }
    }
}
