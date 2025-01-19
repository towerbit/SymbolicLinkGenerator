using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace NotificationUI
{
    internal partial class frmNotifier : Form
    {
        #region Properties

        private NotifyAction _action;
        private int _interval;
        private int positionX;
        private int positionY;

        public Rectangle WorkingArea { get; set; } = Screen.PrimaryScreen.WorkingArea;
        
        protected override bool ShowWithoutActivation => true;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE 显示但不激活获取焦点
                return createParams;
            }
        }
        #endregion

        #region Constructor

        public frmNotifier()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void CloseAlert_Click(object sender, EventArgs e)
        {
            timer.Interval = 1;
            _action = NotifyAction.Close;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            switch (_action)
            {
                case NotifyAction.Start:
                    timer.Interval = 1;
                    Opacity += 0.1;

                    if (positionX < Location.X) Left--;
                    else if (Opacity == 1.0) _action = NotifyAction.Wait;

                    break;
                case NotifyAction.Wait:
                    timer.Interval = _interval;
                    _action = NotifyAction.Close;
                    break;
                case NotifyAction.Close:
                    timer.Interval = 1;
                    Opacity -= 0.1;
                    Left -= 3;
                    if (Opacity == 0.0) Close();
                    break;
            }
        }

        #endregion

        #region Methods

        internal void ShowCustom(string message, Image icon, Color bgColor, int interval)
        {
            Opacity = 0.0;
            StartPosition = FormStartPosition.Manual;

            for (int i = 1; i < 10; i++)
            {
                var formName = "alert" + i;
                var frm = (frmNotifier)Application.OpenForms[formName];

                if (frm == null)
                {
                    Name = formName;
                    positionX = WorkingArea.Left + WorkingArea.Width - Width - 10;
                    positionY = WorkingArea.Top + WorkingArea.Height - Height * i - 10 * i;
                    Location = new Point(positionX, positionY);
                    break;
                }
            }

            ptbLogo.Image = icon;
            BackColor = bgColor;

            lblMessage.Text = message;
            _interval = interval;
            _action = NotifyAction.Start;
            timer.Interval = 1;
            timer.Start();
            
            this.Show();
        }

        internal void ShowAlert(string message, ToolTipIcon icon, int interval, Color bgColor=default)
        {
            Image imgIcon = imageList1.Images["information48px.png"];//Resources.information48px;
            bgColor = bgColor == default? Color.RoyalBlue: bgColor;
            switch (icon)
            {
                case ToolTipIcon.Info:
                    imgIcon = imageList1.Images["sucess48px.png"];//Resources.sucess48px
                    bgColor = Color.SeaGreen;
                    break;
                case  ToolTipIcon.Warning:
                    imgIcon = imageList1.Images["warning48px.png"];//Resources.warning48px;
                    bgColor = Color.FromArgb(230, 126, 34);
                    break;
                case ToolTipIcon.Error:
                    imgIcon = imageList1.Images["error48px.png"];//Resources.error48px;
                    bgColor = Color.FromArgb(231, 76, 60);
                    break;
                default: //None
                    break;
            }

            ShowCustom(message, imgIcon, bgColor, interval);
        }
        #endregion
    }
}
