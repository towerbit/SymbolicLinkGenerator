using System.Drawing;
using System.Windows.Forms;

namespace SymbolicLinkGenerator
{
    public partial class frmToast : Form
    {
        
        public frmToast()
        {
            InitializeComponent();
            lblMessage.SizeChanged += (s, e) =>
            {
                if (lblMessage.Width > this.Width)
                {
                    this.Width = this.Padding.Left + lblMessage.Width + this.Padding.Right;
                }
                else
                {
                    lblMessage.Left = this.Width / 2 - lblMessage.Width / 2;
                }
            };
        }

        public frmToast(Form parent, string message, int duration) : this()
        {
            lblMessage.Text = message;
            this.StartPosition = FormStartPosition.Manual;
            //this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - Width - 10, Screen.PrimaryScreen.WorkingArea.Height - Height - 10);
            this.Location = new Point(parent.Left + (parent.Width - this.Width )/2, parent.Top + (parent.Height - this.Height)/2 );
            var tmrClose = new Timer();
            tmrClose.Interval = duration;
            tmrClose.Tick += (s, e) => this.Close();
            tmrClose.Start();
            this.Show();
        }
    }
}
