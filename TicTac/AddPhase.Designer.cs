namespace TicTac
{
    partial class AddPhase
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
            this.labelDescription = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.labelPhaseName = new System.Windows.Forms.Label();
            this.textBoxPhaseName = new System.Windows.Forms.TextBox();
            this.Save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(12, 41);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(60, 13);
            this.labelDescription.TabIndex = 14;
            this.labelDescription.Text = "Description";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(107, 41);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(141, 46);
            this.textBoxDescription.TabIndex = 13;
            // 
            // labelPhaseName
            // 
            this.labelPhaseName.AutoSize = true;
            this.labelPhaseName.Location = new System.Drawing.Point(12, 15);
            this.labelPhaseName.Name = "labelPhaseName";
            this.labelPhaseName.Size = new System.Drawing.Size(87, 13);
            this.labelPhaseName.TabIndex = 12;
            this.labelPhaseName.Text = "Nom de la phase";
            // 
            // textBoxPhaseName
            // 
            this.textBoxPhaseName.Location = new System.Drawing.Point(107, 15);
            this.textBoxPhaseName.Name = "textBoxPhaseName";
            this.textBoxPhaseName.Size = new System.Drawing.Size(141, 20);
            this.textBoxPhaseName.TabIndex = 11;
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(173, 93);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(75, 23);
            this.Save.TabIndex = 10;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.SaveClick);
            // 
            // AddPhase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(260, 125);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.labelPhaseName);
            this.Controls.Add(this.textBoxPhaseName);
            this.Controls.Add(this.Save);
            this.Name = "AddPhase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ajouter une phase";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label labelPhaseName;
        private System.Windows.Forms.TextBox textBoxPhaseName;
        private System.Windows.Forms.Button Save;

    }
}