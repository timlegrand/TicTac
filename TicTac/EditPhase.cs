using System;
using System.Windows.Forms;

namespace TicTac
{
    public partial class EditPhase : Form
    {
        private readonly DatabaseViewer parentWindow;
        private Phase _currentPhase;
        private bool _updateOnly;

        public EditPhase(DatabaseViewer parent)
        {
            InitializeComponent();
            this.parentWindow = parent;
        }

        public EditPhase(DatabaseViewer parent, Phase currentPhase) : this(parent)
        {
            this.Text = "Modifier une Phase";
            this.textBoxPhaseName.Text = currentPhase.Name;
            this.textBoxDescription.Text = currentPhase.Description;
            this._currentPhase = currentPhase;
            this._updateOnly = true;
        }

        private void SaveClick(object sender, System.EventArgs e)
        {
            var n = textBoxPhaseName.Text;
            var d = textBoxDescription.Text;
            if (n.Length == 0)
            {
                MessageBox.Show("\"Name\" field is required", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var updatedPhase = new Phase() { Id = _currentPhase.Id, Name = n, Description = d };
            var id = (_updateOnly == true) ?
                Service.Instance.EditPhase((int)_currentPhase.Id, updatedPhase) :
                Service.Instance.AddPhase(updatedPhase);
            Console.WriteLine("inserted id = {0}", id);

            parentWindow.UpdateData();
            Hide();
        }
    }
}
