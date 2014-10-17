using System.ComponentModel;
using System.Windows.Forms;

namespace TicTac
{
    public class CommonForm : Form
    {
        protected PictureBox busyAnimation;
        protected readonly Form _parent;

        public CommonForm()
            : base()
        {
            // Escape key closes Form
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(this.KeyDownHandler);

            // Busy animation
            this.busyAnimation = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.busyAnimation)).BeginInit();
            this.SuspendLayout();
            this.busyAnimation.Image = global::TicTac.Properties.Resources.loading;
            this.busyAnimation.Location = new System.Drawing.Point(146, 102); // Default
            this.busyAnimation.Name = "busyAnimation";
            this.busyAnimation.Size = new System.Drawing.Size(21, 21);
            this.busyAnimation.SizeMode = PictureBoxSizeMode.StretchImage;
            this.busyAnimation.TabIndex = 15;
            this.busyAnimation.TabStop = false;
            this.Controls.Add(this.busyAnimation);
            ((System.ComponentModel.ISupportInitialize)(this.busyAnimation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            busyAnimation.Hide();
        }

        public CommonForm(Form mainWindow)
            : this()
        {
            _parent = mainWindow;
        }

        public CommonForm(System.Drawing.Point animationLocation)
            : this()
        {
            this.busyAnimation.Location = animationLocation;
        }

        public CommonForm(Form mainWindow, System.Drawing.Point animationLocation)
            : this(mainWindow)
        {
            this.busyAnimation.Location = animationLocation;
        }

        delegate void ShowBusyAnimationDelegate();
        protected void ShowBusyAnimation(object sender, DoWorkEventArgs e)
        {
            this.BeginInvoke(new ShowBusyAnimationDelegate(busyAnimation.Show), null);
        }

        delegate void HideBusyAnimationDelegate();
        protected void HideBusyAnimation(object sender, RunWorkerCompletedEventArgs e)
        {
            _parent.BeginInvoke(new HideBusyAnimationDelegate(busyAnimation.Hide), null);
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
