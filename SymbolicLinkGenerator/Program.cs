using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace SymbolicLinkGenerator
{
    internal static class Program
    {
        private static Mutex mutex;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 创建一个互斥体，确保只有一个实例能够运行
            bool createdNew;
            mutex = new Mutex(true, Assembly.GetExecutingAssembly().FullName, out createdNew);

            if (!createdNew)
            {
                // 如果互斥体已经存在，说明应用程序已经在运行
                MessageBox.Show("应用程序已经在运行。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // 退出程序
            }

            try
            {
                ExplorerHelper.KillProcessCore();
                System.IO.File.Delete("SlgCore.exe"); // 确保每次释放版本匹配的 SlgCore.exe
            }
            catch (Exception ex)
            {
                Debug.Print($"当前目录下的 SlgCore.exe 未删除, {ex.Message}");
            }
            SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());

            // 释放互斥体
            mutex.ReleaseMutex();
        }

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
