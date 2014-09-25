using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TicTac
{
    public partial class EditArchitect : Form
    {
        private readonly DatabaseViewer parentWindow;
        private Architect currentArchitect; 
        private bool updateOnly;

        public EditArchitect(DatabaseViewer parent)
        {
            InitializeComponent();
            this.parentWindow = parent;
            this.comboBoxCompany.Items.AddRange(Service.Instance.GetAllCompanies().ToArray());
            this.comboBoxCompany.SelectedItem = this.comboBoxCompany.Items[0];
            this.updateOnly = false;
        }

        public EditArchitect(DatabaseViewer parent, Architect architect) : this(parent)
        {
            this.Text = "Modifier un architecte";
            this.textBoxFirstName.Text = architect.FirstName;
            this.textBoxLastName.Text = architect.LastName;
            this.comboBoxCompany.SelectedItem = Service.Instance.SelectCompanyFromId(architect.Company);
            this.currentArchitect = architect;
            this.updateOnly = true;
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

            var updatedArchitect = new Architect() { Id = currentArchitect.Id, Company = (int)cp.Id, FirstName = fn, LastName = ln };
            var id = (updateOnly == true) ?
                Service.Instance.EditArchitect((int)currentArchitect.Id, updatedArchitect) :
                Service.Instance.AddArchitect(updatedArchitect);
            Console.WriteLine("inserted id = {0}", id);

            parentWindow.UpdateData();
            Hide();
        }
    }
}
