using System.Diagnostics;

namespace ProjectTime
{
    partial class RecordWindow
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
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.comboBoxProjects = new System.Windows.Forms.ComboBox();
            this.comboBoxPhases = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxArchitects = new System.Windows.Forms.ComboBox();
            this.labelTime = new System.Windows.Forms.Label();
            this.addArchitect = new System.Windows.Forms.Button();
            this.addProject = new System.Windows.Forms.Button();
            this.addPhase = new System.Windows.Forms.Button();
            this._bgTimer = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(94, 109);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.ButtonStartClick);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(187, 109);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 1;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.ButtonStopClick);
            // 
            // comboBoxProjects
            // 
            this.comboBoxProjects.FormattingEnabled = true;
            this.comboBoxProjects.Location = new System.Drawing.Point(83, 40);
            this.comboBoxProjects.Name = "comboBoxProjects";
            this.comboBoxProjects.Size = new System.Drawing.Size(163, 21);
            this.comboBoxProjects.TabIndex = 2;
            this.comboBoxProjects.SelectedIndexChanged += new System.EventHandler(this.ComboBoxProjectsSelectedIndexChanged);
            // 
            // comboBoxPhases
            // 
            this.comboBoxPhases.FormattingEnabled = true;
            this.comboBoxPhases.Location = new System.Drawing.Point(83, 67);
            this.comboBoxPhases.Name = "comboBoxPhases";
            this.comboBoxPhases.Size = new System.Drawing.Size(163, 21);
            this.comboBoxPhases.TabIndex = 3;
            this.comboBoxPhases.SelectedIndexChanged += new System.EventHandler(this.ComboBoxPhaseSelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Projet";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Phase";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Architecte";
            // 
            // comboBoxArchitects
            // 
            this.comboBoxArchitects.FormattingEnabled = true;
            this.comboBoxArchitects.Location = new System.Drawing.Point(83, 13);
            this.comboBoxArchitects.Name = "comboBoxArchitects";
            this.comboBoxArchitects.Size = new System.Drawing.Size(163, 21);
            this.comboBoxArchitects.TabIndex = 7;
            this.comboBoxArchitects.SelectedIndexChanged += new System.EventHandler(this.ComboBoxArchitectsSelectedIndexChanged);
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(12, 114);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(58, 13);
            this.labelTime.TabIndex = 8;
            this.labelTime.Text = "Au boulot !";
            // 
            // addArchitect
            // 
            this.addArchitect.Location = new System.Drawing.Point(253, 13);
            this.addArchitect.Name = "addArchitect";
            this.addArchitect.Size = new System.Drawing.Size(20, 21);
            this.addArchitect.TabIndex = 9;
            this.addArchitect.Text = "+";
            this.addArchitect.UseVisualStyleBackColor = true;
            this.addArchitect.Visible = false;
            this.addArchitect.Click += new System.EventHandler(this.AddArchitectClick);
            // 
            // addProject
            // 
            this.addProject.Location = new System.Drawing.Point(253, 40);
            this.addProject.Name = "addProject";
            this.addProject.Size = new System.Drawing.Size(20, 21);
            this.addProject.TabIndex = 10;
            this.addProject.Text = "+";
            this.addProject.UseVisualStyleBackColor = true;
            this.addProject.Visible = false;
            this.addProject.Click += new System.EventHandler(this.AddProjectClick);
            // 
            // addPhase
            // 
            this.addPhase.Location = new System.Drawing.Point(253, 67);
            this.addPhase.Name = "addPhase";
            this.addPhase.Size = new System.Drawing.Size(20, 21);
            this.addPhase.TabIndex = 11;
            this.addPhase.Text = "+";
            this.addPhase.UseVisualStyleBackColor = true;
            this.addPhase.Visible = false;
            this.addPhase.Click += new System.EventHandler(this.AddPhaseClick);
            // 
            // _bgTimer
            // 
            this._bgTimer.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BgTimerRunWorkerCompleted);
            // 
            // RecordWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 157);
            this.Controls.Add(this.addPhase);
            this.Controls.Add(this.addProject);
            this.Controls.Add(this.addArchitect);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.comboBoxArchitects);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxPhases);
            this.Controls.Add(this.comboBoxProjects);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Name = "RecordWindow";
            this.Text = "ProjectTime";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RecordWindowFormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxProjects;
        private System.Windows.Forms.ComboBox comboBoxPhases;
        private System.Windows.Forms.ComboBox comboBoxArchitects;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Button addArchitect;
        private System.Windows.Forms.Button addProject;
        private System.Windows.Forms.Button addPhase;
    }
}

