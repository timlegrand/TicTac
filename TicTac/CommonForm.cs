using System.Windows.Forms;

namespace TicTac
{
    public class CommonForm : Form
    {
        public CommonForm()
            : base()
        {
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownHandler);
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
