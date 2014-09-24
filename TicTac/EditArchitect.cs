using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TicTac
{
    public partial class EditArchitect : Form
    {
        private readonly Service _service;
        private readonly List<Company> _companyList;
        private readonly DatabaseViewer _parent;
        private bool _updateOnly;
        private Architect _currentArchitect;

        public EditArchitect(DatabaseViewer parent)
        {
            InitializeComponent();
            _service = Service.Instance;
            _parent = parent;
            _companyList = _service.GetAllCompanies();
            if (_companyList != null && _companyList.Count != 0) comboBoxCompany.Items.AddRange(_companyList.ToArray());
            comboBoxCompany.SelectedItem = comboBoxCompany.Items[0];
            this._updateOnly = false;
        }

        public EditArchitect(DatabaseViewer parent, Architect architect)
        {
            InitializeComponent();
            this.Text = "Modifier un architecte";
            _service = Service.Instance;
            _parent = parent;
            _companyList = _service.GetAllCompanies();
            if (_companyList != null && _companyList.Count != 0) comboBoxCompany.Items.AddRange(_companyList.ToArray());
            comboBoxCompany.SelectedItem = comboBoxCompany.Items[0];
            textBoxFirstName.Text = architect.FirstName;
            textBoxLastName.Text = architect.LastName;
            comboBoxCompany.SelectedItem = _service.SelectCompanyFromId(architect.Company);
            this._currentArchitect = architect;
            this._updateOnly = true;
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
            var id = (_updateOnly == true) ?
                _service.EditArchitect((int)this._currentArchitect.Id, updatedArchitect) :
                _service.AddArchitect(updatedArchitect);
            Console.WriteLine("inserted id = {0}", id);

            _service.ArchitectList.Single(a => a.Id == id).CopyIn(updatedArchitect);
            _parent.UpdateData();
            Hide();
        }
    }
}
