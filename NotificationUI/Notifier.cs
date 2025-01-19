using System;
using System.Drawing;
using System.Windows.Forms;

namespace NotificationUI
{
    public class Notifier
    {
        #region Properties

        private const int _defInterval = 6000;
        //private readonly Color _defBgColor = Color.FromArgb(83, 92, 104);
        private readonly Color _defBgColor = Color.FromArgb(36, 104, 188);// (44, 111, 229);

        /// <summary>
        /// 显示范围，默认为主屏工作区，可以设置成主窗体的 Bounds
        /// </summary>
        public Rectangle WorkingArea { get; set; } = Screen.PrimaryScreen.WorkingArea;

        ///// <summary>
        ///// 显示区域，默认为屏幕的右下角
        ///// </summary>
        //public NotifyLocation Location { get; set; } = NotifyLocation.BottomRight;

        #endregion

        #region Sucess

        public EventHandler MessageAreaClick;

        
        public void ShowSucess(string message)
        {
            ShowSucess(message, _defInterval);
        }

        public void ShowSucess(string message, int interval)
        {
            var frm = new frmNotifier();
            frm.WorkingArea = this.WorkingArea;
            frm.lblMessage.Click += (s, e) => { MessageAreaClick?.Invoke(s, e); };
            frm.ShowAlert(message, ToolTipIcon.Info, interval);
        }

        #endregion

        #region Information

        public void ShowInformation(string message)
        {
            ShowInformation(message, _defInterval);
        }

        public void ShowInformation(string message, int interval)
        {
            var frm = new frmNotifier();
            frm.WorkingArea = this.WorkingArea;
            frm.lblMessage.Click += (s, e) => { MessageAreaClick?.Invoke(s, e); };
            frm.ShowAlert(message, ToolTipIcon.None, interval);
        }

        #endregion

        #region Warning

        public void ShowWarning(string message)
        {
            ShowWarning(message, _defInterval);
        }

        public void ShowWarning(string message, int interval)
        {
            var frm = new frmNotifier();
            frm.WorkingArea = this.WorkingArea;
            frm.lblMessage.Click += (s, e) => { MessageAreaClick?.Invoke(s, e); };
            frm.ShowAlert(message, ToolTipIcon.Warning, interval);
        }

        #endregion

        #region Error

        public void ShowError(string message)
        {
            ShowError(message, _defInterval);
        }

        public void ShowError(string message, int interval)
        {
            var frm = new frmNotifier();
            frm.WorkingArea = this.WorkingArea;
            frm.lblMessage.Click += (s, e) => { MessageAreaClick?.Invoke(s, e); };
            frm.ShowAlert(message, ToolTipIcon.Error, interval);
        }

        #endregion

        #region Custom

        public void ShowCustom(string message, Image image = null, Color color = default)
        {
            ShowCustom(message, image, color, _defInterval );
        }

        public void ShowCustom(string message, Image image = null, Color color = default, int interval= _defInterval)
        {
            var frm = new frmNotifier();
            frm.WorkingArea = this.WorkingArea;
            frm.lblMessage.Click += (s, e) => { MessageAreaClick?.Invoke(s, e); };
            frm.ShowCustom(message, image, color == default ? _defBgColor : color, interval );
        }

        public void Show(string message, ToolTipIcon icon, int interval = _defInterval)
        {
            var frm = new frmNotifier();
            frm.WorkingArea = this.WorkingArea;
            frm.lblMessage.Click += (s, e) => { MessageAreaClick?.Invoke(s, e); };
            frm.ShowAlert(message, icon, interval, _defBgColor);
        }

        #endregion
    }

    public enum NotifyAction
    {
        Start,
        Wait,
        Close
    }

    //public enum NotifyLocation
    //{
    //    TopLeft,
    //    TopCenter,
    //    TopRight,
    //    MiddleLeft,
    //    MiddleCenter,
    //    MiddleRight,
    //    BottomLeft,
    //    BottomCenter,
    //    BottomRight
    //}
}
