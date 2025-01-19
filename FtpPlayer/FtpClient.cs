using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace FtpPlayer
{
    internal class FtpClient
    {
        private static Dictionary<string, int> _dicIcons = new Dictionary<string, int>();

        private const int IDX_FILE = 0;
        private const int IDX_FOLDER = 1;
        private const int IDX_COMPUTER = 2;

        private static ImageList _imageList;

        static FtpClient()
        {
            _imageList = new ImageList();
            _imageList.ImageSize = new Size(16, 16);
            _imageList.ColorDepth = ColorDepth.Depth32Bit;

            //var driveIcon = GetSystemIcon("C:"); // 获取通用磁盘的图标
            //_imageList.Images.Add(driveIcon);
            //_dicIcons["Drive"] = IDX_DRIVE; // 磁盘图标的索引
            var fileIcon = GetFileTypeIcon();
            //var fileIcon = GetSystemIcon("C:\\Windows\\bootstat.dat");
            _imageList.Images.Add(fileIcon);
            _dicIcons["File"] = IDX_FILE;

            var folderIcon = GetSystemIcon("."); // 获取通用的文件夹图标
            _imageList.Images.Add(folderIcon);
            _dicIcons["Folder"] = IDX_FOLDER; // 文件夹图标的索引
        }

        public string ftpServer { get; set; }
        public string username { get; set; }
        public string password { get; set; }


        private List<FtpFileInfo> ListDirectory(string directory = "/")
        {
            var ret = new List<FtpFileInfo>();
            try
            {
                // 创建FTP请求
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri($"ftp://{ftpServer}{directory}"));
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(username, password);

                // 获取FTP响应
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        ret.Add (new FtpFileInfo(line));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return ret;
        }

        

        public void AddRootFolders(TreeView tvw)
        {
            if (tvw.ImageList == null && tvw.StateImageList == null)
            {
                ModifyTreeViewStyle(tvw.Handle);

                tvw.ImageList = new ImageList();
                tvw.ImageList.ImageSize = new Size(16, 16);
                tvw.ImageList.ColorDepth = ColorDepth.Depth32Bit;

                var driveIcon = GetSystemIcon("C:"); // 获取通用磁盘的图标
                var folderIcon = GetSystemIcon("."); // 获取通用的文件夹图标
                //var computerIcon = SystemIcons.WinLogo;
                Icon computerIcon = Properties.Resources.computer;
                Icon openedFolderIcon = Properties.Resources.opend_folder;
                tvw.ImageList.Images.Add(driveIcon);    // 0
                tvw.ImageList.Images.Add(folderIcon);   // 1
                tvw.ImageList.Images.Add(computerIcon); // 2
                tvw.ImageList.Images.Add(openedFolderIcon); // 3
                tvw.StateImageList = tvw.ImageList;

                tvw.AfterExpand += (s, e) =>
                {
                    if (e.Node.Name.StartsWith("Folder"))
                    {
                        e.Node.ImageIndex = 3;
                        e.Node.SelectedImageIndex = 3;
                    }
                };
                tvw.AfterCollapse += (s, e) =>
                {
                    if (e.Node.Name.StartsWith("Folder"))
                    {
                        e.Node.ImageIndex = 1;
                        e.Node.SelectedImageIndex = 1;
                    }
                };
            }

            try
            {
                // 清除TreeView中的现有节点
                tvw.Nodes.Clear();
                // 创建根节点
                var path = "/";
                //path = "/movie/cartoon";

                TreeNode rootNode = new TreeNode(path, IDX_COMPUTER, IDX_COMPUTER);
                rootNode.Tag = new FtpFileInfo() { Name = "/", IsFile = false, Length = 0 };
                tvw.Nodes.Add(rootNode);
                
                var rootList = ListDirectory(path);
                var folders = rootList.Where(f => !f.IsFile).ToList();
                folders.Sort((a, b) => a.Name.CompareTo(b.Name));
                var files = rootList.Where(f => f.IsFile).ToList();
                files.Sort((a,b)=> a.Name.CompareTo(b.Name));
                // 遍历根目录
                foreach (FtpFileInfo folder in folders)
                {
                    var folderNode = new TreeNode($"{folder.Name}", IDX_FOLDER, IDX_FOLDER);
                    folderNode.Name = $"Drive{Guid.NewGuid().ToString("N")}";
                    folderNode.Tag = folder;
                    rootNode.Nodes.Add(folderNode);
                }
                rootNode.Expand();
            }
            catch (Exception ex)
            {
                // 处理任何可能发生的异常，例如路径不存在或没有访问权限等
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void AddSubFolders(TreeNode parentNode, bool clear = false)
        {
            if (parentNode == null)
                throw new ArgumentNullException("parentNode");

            if (clear) parentNode.Nodes.Clear();

            if (parentNode.Nodes.Count == 0)
            {
                string parentPath = parentNode.FullPath;
                parentNode.TreeView.Cursor = Cursors.WaitCursor;
                parentNode.TreeView.BeginUpdate();

                try
                {
                    // 获取指定路径下的所有子目录
                    var subList = ListDirectory(parentPath);
                    var folders = subList.Where(f => !f.IsFile).ToList();
                    folders.Sort((a, b) => a.Name.CompareTo(b.Name));

                    // 遍历子目录并为每个子目录创建一个节点
                    foreach (FtpFileInfo folder in folders)
                    {
                        // 创建节点并设置其文本为目录名
                        TreeNode node = new TreeNode(folder.Name , IDX_FOLDER, IDX_FOLDER);
                        node.Name = $"Folder{Guid.NewGuid().ToString("N")}";
                        node.Tag = folder;
                    
                        // 将节点添加到父节点中
                        parentNode.Nodes.Add(node);
                    }
                }
                catch (Exception ex)
                {
                    // 可以在这里处理特定于目录访问的异常，或者简单地忽略它们以防止中断整个树结构的构建
                    Console.WriteLine("Error accessing directory: " + parentPath + "\n" + ex.Message);
                }
                parentNode.TreeView.EndUpdate();
                parentNode.TreeView.Cursor = Cursors.Default;
                parentNode.Expand();
            }
        }

        public void ReloadListView(TreeNode node, ListView lvw)
        {
            if (lvw.SmallImageList == null)
                lvw.SmallImageList = _imageList;

            lvw.Cursor = Cursors.WaitCursor;
            lvw.BeginUpdate();
            // 清除ListView中的现有项
            lvw.Items.Clear();
            try
            {
                var path = node.FullPath;
                if (!string.IsNullOrEmpty(path))
                {
                    lvw.Columns[2].Text = "Modified";
                    //lvw.Tag = path; // 保存当前目录
                    if (node.Parent != null)
                    { 
                        var parentDir = new ListViewItem("..", IDX_FOLDER);
                        parentDir.Name = node.Parent.Name;
                        parentDir.Tag = node.Parent.Tag;
                        lvw.Items.Add(parentDir);
                    }

                    // 将目录添加到ListView中
                    foreach (TreeNode subNode in node.Nodes)
                    {
                        ListViewItem item = new ListViewItem(subNode.Text, IDX_FOLDER);
                        item.Name = subNode.Name;
                        item.Tag = subNode.Tag;
                        item.SubItems.Add("");
                      
                        item.SubItems.Add((subNode.Tag as FtpFileInfo).Date.ToString("G"));
                        lvw.Items.Add(item);
                    }

                    // 将文件添加到ListView中
                    var subList = ListDirectory(path);
                    var files = subList.Where(f => f.IsFile).ToList();
                    files.Sort((a, b) => a.Name.CompareTo(b.Name));

                    foreach (FtpFileInfo file in files)
                    {
                        int iconIndex = IDX_FILE;
                        var ext = Path.GetExtension(file.Name);
                        if (Properties.Settings.Default.ext.Contains(ext))
                        {
                            if (!_dicIcons.TryGetValue(ext, out iconIndex))
                            {
                                //iconIndex = IDX_FILE;
                                // 尝试查找指定扩展名的图标
                                var icon = GetFileTypeIcon(ext);
                                lvw.SmallImageList.Images.Add(icon);
                                iconIndex = lvw.SmallImageList.Images.Count - 1;
                                _dicIcons[ext] = iconIndex;
                            }
                            ListViewItem item = new ListViewItem(file.Name, iconIndex);
                            item.Name = $"File{Guid.NewGuid().ToString("N")}";

                            item.SubItems.Add(ConvertBytesToFileSize(file.Length)); // 添加文件大小列
                            item.SubItems.Add(file.Date.ToString("G"));
                            item.Tag = file;
                            lvw.Items.Add(item);
                        }
                        else
                            Debug.Print($"skip *{ext} file");
                    }
                }
            }
            catch (Exception ex)
            {
                // 处理任何可能发生的异常，例如路径不存在或没有访问权限等
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ResizeListViewColumns(lvw);
            lvw.EndUpdate();
            lvw.Cursor = Cursors.Default;
        }

        public void ResizeListViewColumns(ListView lvw)
        {
            // 设置ListView的列宽以适应内容
            lvw.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            int iWidth = 20; // 留出滚动条宽度
            using (var g = lvw.CreateGraphics())
                for (int i = 1; i < lvw.Columns.Count; i++)
                {
                    var col = lvw.Columns[i];
                    // 设置宽度不小于标题宽度
                    var width = (int)(g.MeasureString(col.Text, lvw.Font).Width + 10);
                    if (col.Width < width) col.Width = width;
                    iWidth += lvw.Columns[i].Width;
                }

            // 设置第一列的宽度
            lvw.Columns[0].Width = lvw.Width - iWidth;
        }

        /// <summary>
        /// 换算文件大小
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string ConvertBytesToFileSize(ulong bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.#}{sizes[order]}"; // Adjust numeric format if needed
        }

        #region 修改 TreeView 为系统样式
        // Win32 API 函数
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        // 消息常量
        private const int WM_SETREDRAW = 0x000B;
        private const int TVS_LINESATROOT = 0x0001;
        private const int TVS_HASLINES = 0x0002;
        private const int TVS_HASBUTTONS = 0x0004;
        private const int TVS_TRACKSELECT = 0x0008;

        private void ModifyTreeViewStyle(IntPtr handle)
        {
            // 发送消息以修改 TreeView 样式
            SendMessage(handle, 0x0001, (IntPtr)(TVS_HASLINES | TVS_HASBUTTONS | TVS_LINESATROOT), IntPtr.Zero);
        }
        #endregion

        #region 提取文件夹图标
        private const uint SHGFI_ICON = 0x100;    // 获取图标
        private const uint SHGFI_SMALLICON = 0x1; // 小图标
        private const uint SHGFI_LARGEICON = 0x0; // 大图标
        private const uint SHGFI_USEFILEATTRIBUTES = 0x10;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        
        const uint SHGFI_DISPLAYNAME = 0x000000200;     // 获取显示名
        const uint SHGFI_TYPENAME = 0x000000400;     // 获取类型名

        /// <summary>
        /// 调用 SHGetFileInfo 的 DLLImport 属性定义
        /// </summary>
        /// <param name="pszPath"></param>
        /// <param name="dwFileAttributes"></param>
        /// <param name="psfi"></param>
        /// <param name="cbSizeFileInfo"></param>
        /// <param name="uFlags"></param>
        /// <returns></returns>
        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbSizeFileInfo,
            uint uFlags);

        /// <summary>
        /// SHFILEINFO 结构，用于接收 SHGetFileInfo 返回的信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon; // 图标的句柄
            public IntPtr iIcon; // 图标的系统索引号
            public uint dwAttributes; // 文件的属性
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName; // 文件的显示名
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName; // 文件的类型名
        }

        private static Icon GetSystemIcon(string path = ".")
        {
            var shinfo = new SHFILEINFO();
            const uint flags = SHGFI_ICON | SHGFI_SMALLICON; // 可以根据需要更改为大图标 SHGFI_LARGEICON
            var hIcon = SHGetFileInfo(path, 
                                      FILE_ATTRIBUTE_DIRECTORY, 
                                      ref shinfo, 
                                      (uint)Marshal.SizeOf(shinfo), 
                                      flags);
            if (hIcon == IntPtr.Zero)
                return null;

            var icon = Icon.FromHandle(shinfo.hIcon); // 转换图标句柄为Icon对象，注意这里不需要手动Dispose，因为Icon是从系统获取的句柄转换的。
            //DestroyIcon(shinfo.hIcon);
            return icon;
        }

        /// <summary>
        /// 获取文件类型的图标
        /// </summary>
        /// <param name="ext">文件扩展名，默认为使用一个不存在的扩展名</param>
        /// <returns></returns>
        private static Icon GetFileTypeIcon(string ext=".unknown")
        {
            var shinfo = new SHFILEINFO();
            const uint flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES | SHGFI_SMALLICON;//SHGFI_ICON | SHGFI_USEFILEATTRIBUTES | SHGFI_LARGEICON
            IntPtr hIcon = SHGetFileInfo(ext, 
                                         FILE_ATTRIBUTE_NORMAL,
                                         ref shinfo,
                                         (uint)Marshal.SizeOf(typeof(SHFILEINFO)),
                                         flags);
            if (hIcon == IntPtr.Zero)
                return null;

            // 从句柄创建 Icon 对象
            Icon icon = Icon.FromHandle(shinfo.hIcon);
            return icon;
        }

        //private Icon GetComputerIcon(out string computerName)
        //{
        //    var shinfo = new SHFILEINFO();
        //    IntPtr result = SHGetFileInfo("::{20D04FE0-3AEA-1069-A2D8-08002B30309D}", FILE_ATTRIBUTE_DIRECTORY, ref shinfo,
        //                             (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_DISPLAYNAME);
        //    computerName = shinfo.szDisplayName;
        //    // 将图标句柄转换为 Bitmap
        //    Icon icon = Icon.FromHandle(shinfo.hIcon);
        //    //DestroyIcon(shinfo.hIcon);
        //    return icon;
        //}

        //[DllImport("shell32.dll", CharSet = CharSet.Auto)]
        //private extern int SHGetKnownFolderPath(ref Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);

        //[DllImport("user32.dll")]
        //private extern bool DestroyIcon(IntPtr hIcon);
        #endregion

    }
}
