using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TicTac
{
    public partial class EditPhase : Form
    {
        private readonly Service _service;
        private readonly DatabaseViewer _parent;
        private Phase _currentPhase;
        private bool _updateOnly;

        public EditPhase(DatabaseViewer parent)
        {
            InitializeComponent();
            _parent = parent;
        }

        public EditPhase(DatabaseViewer parent, Phase currentPhase)
        {
            InitializeComponent();
            this.Text = "Modifier une Phase";
            _service = Service.Instance;
            _parent = parent;
            textBoxPhaseName.Text = currentPhase.Name;
            textBoxDescription.Text = currentPhase.Description;
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
                _service.EditPhase((int)this._currentPhase.Id, updatedPhase) :
                _service.AddPhase(updatedPhase);
            Console.WriteLine("inserted id = {0}", id);

            _service.PhaseList.Single(p => ((int)p.Id) == (int)id).CopyIn(updatedPhase);
            _parent.UpdateData();
            Hide();
        }
    }
}
