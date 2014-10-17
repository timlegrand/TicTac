using System.ComponentModel;
using System.Drawing;
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

            InitializeComponent();
            this.PerformLayout();
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

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(CommonForm));

            this.SuspendLayout();

            // Busy animation
            this.busyAnimation = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.busyAnimation)).BeginInit();

            this.busyAnimation.Image = global::TicTac.Properties.Resources.loading;
            this.busyAnimation.Location = new System.Drawing.Point(146, 102); // Default
            this.busyAnimation.Name = "busyAnimation";
            this.busyAnimation.Size = new System.Drawing.Size(21, 21);
            this.busyAnimation.SizeMode = PictureBoxSizeMode.StretchImage;
            this.busyAnimation.TabIndex = 15;
            this.busyAnimation.TabStop = false;
            this.Controls.Add(this.busyAnimation);
            ((System.ComponentModel.ISupportInitialize)(this.busyAnimation)).EndInit();
            busyAnimation.Hide();

            // Icon
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));

            this.ResumeLayout(false);
        }

        protected Point BoundLocation(Point p)
        {
                return BoundLocation(p.X, p.Y);
        }

        protected Point BoundLocation(int x, int y)
        {
            if (x != -1 && y != -1)
            {
                x = (x < Screen.PrimaryScreen.Bounds.Width - this.Width) ? x : Screen.PrimaryScreen.Bounds.Width - this.Width;
                x = (x >= 0) ? x : 0;
                y = (y < Screen.PrimaryScreen.Bounds.Height - this.Height) ? y : Screen.PrimaryScreen.Bounds.Height - this.Height;
                x = (x >= 0) ? x : 0;
                return new Point(x, y);
            }

            // Default
            return new Point(
                (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2,
                (Screen.PrimaryScreen.Bounds.Height - this.Width) / 2);
        }

    }
}
