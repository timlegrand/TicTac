﻿using System;
using System.Windows.Forms;

namespace ProjectTime
{
    public partial class AddPhase : Form
    {
        private readonly DbConnection _db;
        private readonly Consult _parent;

        public AddPhase(Consult parent)
        {
            InitializeComponent();
            _db = new DbConnection();
            _parent = parent;
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

            var id = _db.InsertPhase(new Phase() { Name = n, Description = d });
            Console.WriteLine("Project inserted id = {0}", id);

            _parent.UpdateData();
            Hide();
        }
    }
}