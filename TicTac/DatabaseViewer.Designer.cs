﻿namespace TicTac
{
    partial class DatabaseViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxArchitects = new System.Windows.Forms.ComboBox();
            this.comboBoxProjects = new System.Windows.Forms.ComboBox();
            this.comboBoxPhases = new System.Windows.Forms.ComboBox();
            this.labelArchitect = new System.Windows.Forms.Label();
            this.labelProject = new System.Windows.Forms.Label();
            this.labelPhase = new System.Windows.Forms.Label();
            this.buttonValidate = new System.Windows.Forms.Button();
            this.labelTotalCountHours = new System.Windows.Forms.Label();
            this.textBoxCountHours = new System.Windows.Forms.TextBox();
            this.textBoxCountManMonth = new System.Windows.Forms.TextBox();
            this.labelTotalCountManMonth = new System.Windows.Forms.Label();
            this.addArchitect = new System.Windows.Forms.Button();
            this.addProject = new System.Windows.Forms.Button();
            this.addPhase = new System.Windows.Forms.Button();
            this.deletePhaseButton = new System.Windows.Forms.Button();
            this.deleteProjectButton = new System.Windows.Forms.Button();
            this.deleteArchitectButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.enddate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startdate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.duration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxArchitects
            // 
            this.comboBoxArchitects.FormattingEnabled = true;
            this.comboBoxArchitects.Location = new System.Drawing.Point(76, 18);
            this.comboBoxArchitects.Name = "comboBoxArchitects";
            this.comboBoxArchitects.Size = new System.Drawing.Size(158, 21);
            this.comboBoxArchitects.TabIndex = 0;
            this.comboBoxArchitects.SelectedIndexChanged += new System.EventHandler(this.ComboBoxArchitectsSelectedIndexChanged);
            // 
            // comboBoxProjects
            // 
            this.comboBoxProjects.FormattingEnabled = true;
            this.comboBoxProjects.Location = new System.Drawing.Point(76, 46);
            this.comboBoxProjects.Name = "comboBoxProjects";
            this.comboBoxProjects.Size = new System.Drawing.Size(158, 21);
            this.comboBoxProjects.TabIndex = 1;
            this.comboBoxProjects.SelectedIndexChanged += new System.EventHandler(this.ComboBoxProjectsSelectedIndexChanged);
            // 
            // comboBoxPhases
            // 
            this.comboBoxPhases.FormattingEnabled = true;
            this.comboBoxPhases.Location = new System.Drawing.Point(76, 74);
            this.comboBoxPhases.Name = "comboBoxPhases";
            this.comboBoxPhases.Size = new System.Drawing.Size(158, 21);
            this.comboBoxPhases.TabIndex = 2;
            this.comboBoxPhases.SelectedIndexChanged += new System.EventHandler(this.ComboBoxPhasesSelectedIndexChanged);
            // 
            // labelArchitect
            // 
            this.labelArchitect.AutoSize = true;
            this.labelArchitect.Location = new System.Drawing.Point(14, 21);
            this.labelArchitect.Name = "labelArchitect";
            this.labelArchitect.Size = new System.Drawing.Size(55, 13);
            this.labelArchitect.TabIndex = 3;
            this.labelArchitect.Text = "Architecte";
            // 
            // labelProject
            // 
            this.labelProject.AutoSize = true;
            this.labelProject.Location = new System.Drawing.Point(14, 49);
            this.labelProject.Name = "labelProject";
            this.labelProject.Size = new System.Drawing.Size(34, 13);
            this.labelProject.TabIndex = 4;
            this.labelProject.Text = "Projet";
            // 
            // labelPhase
            // 
            this.labelPhase.AutoSize = true;
            this.labelPhase.Location = new System.Drawing.Point(14, 77);
            this.labelPhase.Name = "labelPhase";
            this.labelPhase.Size = new System.Drawing.Size(37, 13);
            this.labelPhase.TabIndex = 5;
            this.labelPhase.Text = "Phase";
            // 
            // buttonValidate
            // 
            this.buttonValidate.Location = new System.Drawing.Point(173, 101);
            this.buttonValidate.Name = "buttonValidate";
            this.buttonValidate.Size = new System.Drawing.Size(75, 23);
            this.buttonValidate.TabIndex = 6;
            this.buttonValidate.Text = "Valider";
            this.buttonValidate.UseVisualStyleBackColor = true;
            this.buttonValidate.Click += new System.EventHandler(this.ButtonValidateClick);
            // 
            // labelTotalCountHours
            // 
            this.labelTotalCountHours.AutoSize = true;
            this.labelTotalCountHours.Location = new System.Drawing.Point(97, 412);
            this.labelTotalCountHours.Name = "labelTotalCountHours";
            this.labelTotalCountHours.Size = new System.Drawing.Size(37, 13);
            this.labelTotalCountHours.TabIndex = 7;
            this.labelTotalCountHours.Text = "Total :";
            // 
            // textBoxCountHours
            // 
            this.textBoxCountHours.Location = new System.Drawing.Point(140, 409);
            this.textBoxCountHours.Name = "textBoxCountHours";
            this.textBoxCountHours.ReadOnly = true;
            this.textBoxCountHours.Size = new System.Drawing.Size(92, 20);
            this.textBoxCountHours.TabIndex = 8;
            this.textBoxCountHours.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxCountManMonth
            // 
            this.textBoxCountManMonth.Location = new System.Drawing.Point(140, 439);
            this.textBoxCountManMonth.Name = "textBoxCountManMonth";
            this.textBoxCountManMonth.ReadOnly = true;
            this.textBoxCountManMonth.Size = new System.Drawing.Size(92, 20);
            this.textBoxCountManMonth.TabIndex = 10;
            this.textBoxCountManMonth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelTotalCountManMonth
            // 
            this.labelTotalCountManMonth.AutoSize = true;
            this.labelTotalCountManMonth.Location = new System.Drawing.Point(109, 442);
            this.labelTotalCountManMonth.Name = "labelTotalCountManMonth";
            this.labelTotalCountManMonth.Size = new System.Drawing.Size(25, 13);
            this.labelTotalCountManMonth.TabIndex = 9;
            this.labelTotalCountManMonth.Text = "ou :";
            // 
            // addArchitect
            // 
            this.addArchitect.Location = new System.Drawing.Point(261, 19);
            this.addArchitect.Name = "addArchitect";
            this.addArchitect.Size = new System.Drawing.Size(20, 21);
            this.addArchitect.TabIndex = 12;
            this.addArchitect.Text = "+";
            this.addArchitect.UseVisualStyleBackColor = true;
            this.addArchitect.Click += new System.EventHandler(this.AddArchitectClick);
            // 
            // addProject
            // 
            this.addProject.Location = new System.Drawing.Point(261, 46);
            this.addProject.Name = "addProject";
            this.addProject.Size = new System.Drawing.Size(20, 21);
            this.addProject.TabIndex = 13;
            this.addProject.Text = "+";
            this.addProject.UseVisualStyleBackColor = true;
            this.addProject.Click += new System.EventHandler(this.AddProjectClick);
            // 
            // addPhase
            // 
            this.addPhase.Location = new System.Drawing.Point(261, 73);
            this.addPhase.Name = "addPhase";
            this.addPhase.Size = new System.Drawing.Size(20, 21);
            this.addPhase.TabIndex = 14;
            this.addPhase.Text = "+";
            this.addPhase.UseVisualStyleBackColor = true;
            this.addPhase.Click += new System.EventHandler(this.AddPhaseClick);
            // 
            // deletePhaseButton
            // 
            this.deletePhaseButton.Location = new System.Drawing.Point(283, 73);
            this.deletePhaseButton.Name = "deletePhaseButton";
            this.deletePhaseButton.Size = new System.Drawing.Size(20, 21);
            this.deletePhaseButton.TabIndex = 18;
            this.deletePhaseButton.Text = "-";
            this.deletePhaseButton.UseVisualStyleBackColor = true;
            this.deletePhaseButton.Click += new System.EventHandler(this.DeletePhaseButtonClick);
            // 
            // deleteProjectButton
            // 
            this.deleteProjectButton.Location = new System.Drawing.Point(283, 46);
            this.deleteProjectButton.Name = "deleteProjectButton";
            this.deleteProjectButton.Size = new System.Drawing.Size(20, 21);
            this.deleteProjectButton.TabIndex = 17;
            this.deleteProjectButton.Text = "-";
            this.deleteProjectButton.UseVisualStyleBackColor = true;
            this.deleteProjectButton.Click += new System.EventHandler(this.DeleteProjectButtonClick);
            // 
            // deleteArchitectButton
            // 
            this.deleteArchitectButton.Location = new System.Drawing.Point(283, 19);
            this.deleteArchitectButton.Name = "deleteArchitectButton";
            this.deleteArchitectButton.Size = new System.Drawing.Size(20, 21);
            this.deleteArchitectButton.TabIndex = 16;
            this.deleteArchitectButton.Text = "-";
            this.deleteArchitectButton.UseVisualStyleBackColor = true;
            this.deleteArchitectButton.Click += new System.EventHandler(this.DeleteArchitectButtonClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(238, 442);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "homme * jour";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(238, 412);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "heures";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(239, 73);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(20, 21);
            this.button1.TabIndex = 24;
            this.button1.Text = "e";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.EditPhaseButtonClick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(239, 46);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(20, 21);
            this.button2.TabIndex = 23;
            this.button2.Text = "e";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.EditProjectButtonClick);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(239, 19);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(20, 21);
            this.button3.TabIndex = 22;
            this.button3.Text = "e";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.EditArchitectButtonClick);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.enddate,
            this.startdate,
            this.duration});
            this.dataGridView1.Location = new System.Drawing.Point(13, 130);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 25;
            this.dataGridView1.Size = new System.Drawing.Size(288, 273);
            this.dataGridView1.TabIndex = 25;
            // 
            // enddate
            // 
            this.enddate.DataPropertyName = "enddate";
            this.enddate.FillWeight = 73.85786F;
            this.enddate.HeaderText = "Fin";
            this.enddate.Name = "enddate";
            this.enddate.Width = 21;
            // 
            // startdate
            // 
            this.startdate.DataPropertyName = "startdate";
            this.startdate.FillWeight = 73.85786F;
            this.startdate.HeaderText = "Début";
            this.startdate.Name = "startdate";
            this.startdate.Width = 21;
            // 
            // duration
            // 
            this.duration.DataPropertyName = "duration";
            this.duration.FillWeight = 152.2843F;
            this.duration.HeaderText = "Durée";
            this.duration.Name = "duration";
            this.duration.Width = 21;
            // 
            // DatabaseViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 472);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.deletePhaseButton);
            this.Controls.Add(this.deleteProjectButton);
            this.Controls.Add(this.deleteArchitectButton);
            this.Controls.Add(this.addPhase);
            this.Controls.Add(this.addProject);
            this.Controls.Add(this.addArchitect);
            this.Controls.Add(this.textBoxCountManMonth);
            this.Controls.Add(this.labelTotalCountManMonth);
            this.Controls.Add(this.textBoxCountHours);
            this.Controls.Add(this.labelTotalCountHours);
            this.Controls.Add(this.buttonValidate);
            this.Controls.Add(this.labelPhase);
            this.Controls.Add(this.labelProject);
            this.Controls.Add(this.labelArchitect);
            this.Controls.Add(this.comboBoxPhases);
            this.Controls.Add(this.comboBoxProjects);
            this.Controls.Add(this.comboBoxArchitects);
            this.MaximizeBox = false;
            this.Name = "DatabaseViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Consulter / éditer la base";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxArchitects;
        private System.Windows.Forms.ComboBox comboBoxProjects;
        private System.Windows.Forms.ComboBox comboBoxPhases;
        private System.Windows.Forms.Label labelArchitect;
        private System.Windows.Forms.Label labelProject;
        private System.Windows.Forms.Label labelPhase;
        private System.Windows.Forms.Button buttonValidate;
        private System.Windows.Forms.Label labelTotalCountHours;
        private System.Windows.Forms.TextBox textBoxCountHours;
        private System.Windows.Forms.TextBox textBoxCountManMonth;
        private System.Windows.Forms.Label labelTotalCountManMonth;
        private System.Windows.Forms.Button addArchitect;
        private System.Windows.Forms.Button addProject;
        private System.Windows.Forms.Button addPhase;
        private System.Windows.Forms.Button deletePhaseButton;
        private System.Windows.Forms.Button deleteProjectButton;
        private System.Windows.Forms.Button deleteArchitectButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn enddate;
        private System.Windows.Forms.DataGridViewTextBoxColumn startdate;
        private System.Windows.Forms.DataGridViewTextBoxColumn duration;
    }
}