using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjectTime
{
    public partial class AddArchitect : Form
    {
        private  List<Architect> _architectDb;
        private RecordWindow _mainWindow;

        public AddArchitect(List<Architect> architectDb, RecordWindow mainWindow)
        {
            _architectDb = architectDb;
            _mainWindow = mainWindow;
            InitializeComponent();
        }

        private void SaveClick(object sender, EventArgs e)
        {
            var fn = this.firstName.Text;
            var ln = this.lastName.Text;
            var cp = this.company.Text;
            if ( fn.Length == 0 || ln.Length == 0 || cp.Length == 0 )
            {
                MessageBox.Show("Please fill-in every fields", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            /*
            _architectDb.Add(new Architect(fn, ln, cp));
            */
            // TODO
            _mainWindow.Update();
            this.Hide();
        }
    }
}
