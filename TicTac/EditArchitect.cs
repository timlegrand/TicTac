using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TicTac
{
    public partial class EditArchitect : Form
    {
        private readonly DatabaseViewer _parentWindow;
        private readonly Architect _currentArchitect; 
        private readonly bool _updateOnly;

        public EditArchitect(DatabaseViewer parent)
        {
            InitializeComponent();
            _parentWindow = parent;
            comboBoxCompany.Items.AddRange(Service.Instance.GetAllCompanies().ToArray());
            comboBoxCompany.SelectedItem = comboBoxCompany.Items[0];
            _updateOnly = false;
        }

        public EditArchitect(DatabaseViewer parent, Architect architect) : this(parent)
        {
            Text = "Modifier un architecte";
            textBoxFirstName.Text = architect.FirstName;
            textBoxLastName.Text = architect.LastName;
            comboBoxCompany.SelectedItem = Service.Instance.SelectCompanyFromId(architect.Company);
            _currentArchitect = architect;
            _updateOnly = true;
        }

        private void SaveClick(object sender, EventArgs e)
        {
            var fn = textBoxFirstName.Text;
            var ln = textBoxLastName.Text;
            var cp = (Company) comboBoxCompany.SelectedItem;
            if ( fn.Length == 0 || ln.Length == 0 || cp == null || cp.Id == null )
            {
                MessageBox.Show(@"Please fill-in every fields", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var updatedArchitect = new Architect() { Id = _currentArchitect.Id, Company = (int)cp.Id, FirstName = fn, LastName = ln };
            var id = (_updateOnly == true && _currentArchitect.Id != null) ?
                Service.Instance.EditArchitect((int)_currentArchitect.Id, updatedArchitect) :
                Service.Instance.AddArchitect(updatedArchitect);
            Console.WriteLine("inserted id = {0}", id);

            _parentWindow.UpdateData();
            Hide();
        }
    }
}
