using System.Diagnostics;

namespace TicTac
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecordWindow));
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.comboBoxArchitects = new System.Windows.Forms.ComboBox();
            this.comboBoxProjects = new System.Windows.Forms.ComboBox();
            this.comboBoxPhases = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIconMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.trayMenuItemConfigure = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenuItemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenuItemReduce = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIconMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(90, 119);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.ButtonStartClick);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(171, 119);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 1;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.ButtonStopClick);
            // 
            // comboBoxArchitects
            // 
            this.comboBoxArchitects.FormattingEnabled = true;
            this.comboBoxArchitects.Location = new System.Drawing.Point(83, 13);
            this.comboBoxArchitects.Name = "comboBoxArchitects";
            this.comboBoxArchitects.Size = new System.Drawing.Size(163, 21);
            this.comboBoxArchitects.Sorted = true;
            this.comboBoxArchitects.TabIndex = 7;
            this.comboBoxArchitects.SelectionChangeCommitted += new System.EventHandler(this.ComboBoxArchitectsSelectionChangeCommited);
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
            this.label2.Location = new System.Drawing.Point(12, 70);
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
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(100, 97);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(58, 13);
            this.labelTime.TabIndex = 8;
            this.labelTime.Text = "Au boulot !";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(44, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Total :";
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.BalloonTipText = "TicTac est ouvert et prêt à enregistrer vos exploits !";
            this.notifyIcon.BalloonTipTitle = "TicTac";
            this.notifyIcon.ContextMenuStrip = this.notifyIconMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "TicTac";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // notifyIconMenu
            // 
            this.notifyIconMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trayMenuItemConfigure,
            this.trayMenuItemOpen,
            this.trayMenuItemReduce,
            this.trayMenuItemExit});
            this.notifyIconMenu.Name = "notifyIconMenu";
            this.notifyIconMenu.Size = new System.Drawing.Size(160, 92);
            // 
            // trayMenuItemConfigure
            // 
            this.trayMenuItemConfigure.Image = global::TicTac.Properties.Resources.configure;
            this.trayMenuItemConfigure.Name = "trayMenuItemConfigure";
            this.trayMenuItemConfigure.Size = new System.Drawing.Size(159, 22);
            this.trayMenuItemConfigure.Text = "Configure";
            this.trayMenuItemConfigure.Click += new System.EventHandler(this.trayMenuItemConfigure_Click);
            // 
            // trayMenuItemOpen
            // 
            this.trayMenuItemOpen.Name = "trayMenuItemOpen";
            this.trayMenuItemOpen.Size = new System.Drawing.Size(159, 22);
            this.trayMenuItemOpen.Text = "Open";
            this.trayMenuItemOpen.ToolTipText = "Open TicTac record window";
            this.trayMenuItemOpen.Click += new System.EventHandler(this.trayMenuItemOpen_Click);
            // 
            // trayMenuItemReduce
            // 
            this.trayMenuItemReduce.Name = "trayMenuItemReduce";
            this.trayMenuItemReduce.Size = new System.Drawing.Size(159, 22);
            this.trayMenuItemReduce.Text = "Minimize in tray";
            this.trayMenuItemReduce.ToolTipText = "Minimize in system tray (keep running in background)";
            this.trayMenuItemReduce.Click += new System.EventHandler(this.trayMenuItemMinimize_Click);
            // 
            // trayMenuItemExit
            // 
            this.trayMenuItemExit.Image = global::TicTac.Properties.Resources.power;
            this.trayMenuItemExit.Name = "trayMenuItemExit";
            this.trayMenuItemExit.Size = new System.Drawing.Size(159, 22);
            this.trayMenuItemExit.Text = "Exit";
            this.trayMenuItemExit.ToolTipText = "Close TicTac";
            this.trayMenuItemExit.Click += new System.EventHandler(this.notifyIconMenuExit_Click);
            // 
            // RecordWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(258, 151);
            this.ContextMenuStrip = this.notifyIconMenu;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.comboBoxArchitects);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxPhases);
            this.Controls.Add(this.comboBoxProjects);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "RecordWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TicTac (v 0.1)";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RecordWindowFormClosed);
            this.Resize += new System.EventHandler(this.RecordWindow_Resize);
            this.notifyIconMenu.ResumeLayout(false);
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
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip notifyIconMenu;
        private System.Windows.Forms.ToolStripMenuItem trayMenuItemExit;
        private System.Windows.Forms.ToolStripMenuItem trayMenuItemOpen;
        private System.Windows.Forms.ToolStripMenuItem trayMenuItemReduce;
        private System.Windows.Forms.ToolStripMenuItem trayMenuItemConfigure;
    }
}

