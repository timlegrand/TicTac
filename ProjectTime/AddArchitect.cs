using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProjectTime
{
    public partial class AddArchitect : Form
    {
        private readonly DbConnection _db;
        private readonly List<Company> _companyList;
        private readonly Consult _parent;

        public AddArchitect(Consult parent)
        {
            InitializeComponent();
            _db = new DbConnection(Program.ServerIp, "he", "he", "mySqlUserPassword");
            _parent = parent;
            _companyList = _db.SelectAllCompanies();
            if (_companyList != null && _companyList.Count != 0) comboBoxCompany.Items.AddRange(_companyList.ToArray());
            comboBoxCompany.SelectedItem = comboBoxCompany.Items[0];
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
            
            var id = _db.InsertArchitect(new Architect(){Company = (int) cp.Id, FirstName = fn, LastName = ln});
            Console.WriteLine("inserted id = {0}", id);

            _parent.UpdateData();
            Hide();
        }
    }
}
