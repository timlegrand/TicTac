using System;
using System.Windows.Forms;

namespace TicTac
{
    public partial class EditPhase : Form
    {
        private readonly DatabaseViewer _parentWindow;
        private readonly Phase _currentPhase;
        private readonly bool _updateOnly;

        public EditPhase(DatabaseViewer parent)
        {
            InitializeComponent();
            _parentWindow = parent;
        }

        public EditPhase(DatabaseViewer parent, Phase currentPhase) : this(parent)
        {
            Text = "Modifier une Phase";
            textBoxPhaseName.Text = currentPhase.Name;
            textBoxDescription.Text = currentPhase.Description;
            _currentPhase = currentPhase;
            _updateOnly = true;
        }

        private void SaveClick(object sender, EventArgs e)
        {
            var n = textBoxPhaseName.Text;
            var d = textBoxDescription.Text;
            if (n.Length == 0)
            {
                MessageBox.Show("\"Name\" field is required", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var updatedPhase = new Phase() { Id = _currentPhase.Id, Name = n, Description = d };
            var id = (_updateOnly == true && _currentPhase.Id != null) ?
                Service.Instance.EditPhase((int)_currentPhase.Id, updatedPhase) :
                Service.Instance.AddPhase(updatedPhase);
            Console.WriteLine("inserted id = {0}", id);

            _parentWindow.UpdateData();
            Hide();
        }
    }
}
