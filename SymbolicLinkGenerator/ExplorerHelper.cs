using SymbolicLinkGenerator.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SymbolicLinkGenerator
{
    internal static class ExplorerHelper
    {
        private static Dictionary<string, int> _dicIcons = new Dictionary<string, int>();

        private const int IDX_DRIVE = 0;
        private const int IDX_FOLDER = 1;
        private const int IDX_COMPUTER = 2;

        private static string _cumputerName = "This PC"; // "此电脑";

        private static ImageList _imageList;
        private const int LVW_ICON_BORDER = 4;

        static ExplorerHelper()
        {
            _imageList = new ImageList();
            _imageList.ImageSize = new Size(16 + LVW_ICON_BORDER * 2,
                                            16 + LVW_ICON_BORDER * 2);
            _imageList.ColorDepth = ColorDepth.Depth32Bit;

            var driveIcon = GetSystemIcon("C:"); // 获取通用磁盘的图标
            var borderedDriveIcon = AddTransparentBorder(driveIcon, LVW_ICON_BORDER); // 添加2像素边框
            _imageList.Images.Add(borderedDriveIcon);
            _dicIcons["Drive"] = IDX_DRIVE; // 磁盘图标的索引

            var folderIcon = GetSystemIcon("."); // 获取通用的文件夹图标
            var borderedFolderIcon = AddTransparentBorder(folderIcon, LVW_ICON_BORDER); // 添加2像素边框
            _imageList.Images.Add(borderedFolderIcon);
            _dicIcons["Folder"] = IDX_FOLDER; // 文件夹图标的索引
        }

        /// <summary>
        /// 为 Icon 添加透明边框，支持指定目标尺寸
        /// </summary>
        /// <param name="originalIcon">原始图标</param>
        /// <param name="borderSize">边框大小（像素）</param>
        /// <param name="targetSize">目标尺寸，如果不指定则自动计算</param>
        /// <returns>带透明边框的新图标</returns>
        private static Icon AddTransparentBorder(Icon originalIcon, int borderSize, Size? targetSize = null)
        {
            if (originalIcon == null)
                throw new ArgumentNullException(nameof(originalIcon));

            if (borderSize <= 0)
                return originalIcon;

            // 计算新尺寸
            Size newSize = targetSize ?? new Size(originalIcon.Width + borderSize * 2, 
                                                 originalIcon.Height + borderSize * 2);

            // 创建新的位图
            using (Bitmap bitmap = new Bitmap(newSize.Width, newSize.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    // 设置高质量绘图
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            
                    // 清除为透明背景
                    g.Clear(Color.Transparent);
            
                    // 计算居中绘制的位置
                    Rectangle destRect = new Rectangle(
                        (newSize.Width - originalIcon.Width) / 2,
                        (newSize.Height - originalIcon.Height) / 2,
                        originalIcon.Width,
                        originalIcon.Height);
            
                    // 将图标转换为位图并绘制
                    using (Bitmap iconBitmap = originalIcon.ToBitmap())
                    {
                        g.DrawImage(iconBitmap, destRect);
                    }
                }
        
                // 将位图转换为图标
                return Icon.FromHandle(bitmap.GetHicon());
            }
        }

        /// <summary>
        /// 判断路径是否为符号链接
        /// </summary>
        /// <param name="path"></param>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static bool IsSymbolicLink(string path, out string targetPath)
        {
            // 检查文件是否存在
            if (!File.Exists(path) && !Directory.Exists(path))
            {
                // 指定的路径不存在。
                throw new FileNotFoundException("The specified path does not exist.", path);
            }

            // 创建 FileInfo 对象
            FileInfo fileInfo = new FileInfo(path);

            // 判断是否为软链接
            bool ret = fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
            if (ret)
                targetPath = GetTargetPath(path);
            else
                targetPath = string.Empty;
            return ret;
        }

        #region 获取符号链接的目标地址
        private static string GetTargetPath(string linkPath)
        {
            // 使用 Windows API 获取符号链接目标路径
            var targetPath = new System.Text.StringBuilder(260);
            int targetLength = targetPath.Capacity;

            // 调用 GetFinalPathNameByHandle
            //IntPtr handle = CreateFile(linkPath, FileAccess.Read, FileShare.Read, IntPtr.Zero,
            //    FileMode.Open, FileAttributes.Normal, IntPtr.Zero);

            const uint FILE_SHARE_READ = 1;
            const uint OPEN_EXISTING = 3;
            const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
            IntPtr handle = CreateFile(linkPath, 0, FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero);

            if (handle == IntPtr.Zero)
            {
                //throw new IOException("无法打开符号链接。");
                Debug.Print($"{linkPath} 无法打开符号链接。");
                return string.Empty;
            }

            try
            {
                int result = GetFinalPathNameByHandle(handle, targetPath, targetLength, 0);
                if (result == 0)
                {
                    //throw new IOException("无法获取符号链接的目标路径。");
                    Debug.Print($"{linkPath} 无法获取符号链接的目标路径。");
                    return string.Empty;
                }
                return targetPath.ToString();
            }
            finally
            {
                CloseHandle(handle);
            }
        }

        // P/Invoke 声明
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateFile(string lpFileName, FileAccess dwDesiredAccess, FileShare dwShareMode, IntPtr lpSecurityAttributes,
            FileMode dwCreationDisposition, FileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateFile(string lpFileName,
                                                uint dwDesiredAccess,
                                                uint dwShareMode,
                                                IntPtr lpSecurityAttributes,
                                                uint dwCreationDisposition,
                                                uint dwFlagsAndAttributes,
                                                IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetFinalPathNameByHandle(IntPtr hFile, System.Text.StringBuilder lpszFilePath, int cchFilePath, int dwFlags);
        #endregion

        public static bool IsInvalidFileName(string label)
        {
            return label.Any(c => Path.GetInvalidFileNameChars()
                                      .Contains(c));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="lvw"></param>
        public static void ReloadFolderFiles(TreeNode node, ListView lvw)
        {
           
            if (lvw.SmallImageList == null)
                lvw.SmallImageList = _imageList;

            lvw.Cursor = Cursors.WaitCursor;
            lvw.BeginUpdate();
            // 清除ListView中的现有项
            lvw.Items.Clear();
            try
            {
                var path = node.Tag.ToString();
                if (!string.IsNullOrEmpty(path))
                {
                    lvw.Columns[2].Text = "Modified";
                    lvw.Tag = path; // 保存当前目录

                    var parentDir = new ListViewItem("..", IDX_FOLDER);
                    parentDir.Name = node.Parent.Name;
                    parentDir.Tag = Path.GetDirectoryName(path) ?? "";
                    lvw.Items.Add(parentDir);

                    // 将目录添加到ListView中
                    foreach (TreeNode subNode in node.Nodes)
                    {
                        ListViewItem item = new ListViewItem(subNode.Text, IDX_FOLDER);
                        item.Name = subNode.Name;
                        item.SubItems.Add("");
                        var directory = subNode.Tag.ToString();
                        if (IsSymbolicLink(directory, out string targetPath))
                        {
                            // 蓝色标记为符号链接，红色标记为失效的符号链接
                            item.ForeColor = Directory.Exists(targetPath) ? Color.Blue : Color.Red; 
                            item.ToolTipText = targetPath;
                        }
                        var dirInfo = new DirectoryInfo(directory);
                        item.SubItems.Add(dirInfo.LastWriteTime.ToString("G"));
                        item.Tag = directory; // 可以将完整路径存储在Tag属性中，以便后续使用
                        lvw.Items.Add(item);
                    }

                    // 将文件添加到ListView中
                    string[] files = Directory.GetFiles(path);
                    foreach (string file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file);

                        bool bSoftlink = fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);

                        int iconIndex = -1;
                        if (!_dicIcons.TryGetValue(fileInfo.Extension, out iconIndex))
                        {
                            var icon = Icon.ExtractAssociatedIcon(file);
                            var borderedIcon = AddTransparentBorder(icon, LVW_ICON_BORDER);
                            lvw.SmallImageList.Images.Add(borderedIcon);
                            iconIndex = lvw.SmallImageList.Images.Count - 1;
                            _dicIcons[fileInfo.Extension] = iconIndex; // 存储文件图标在ImageList中的索引
                            //icon.Dispose(); // 释放图标资源
                        }

                        ListViewItem item = new ListViewItem(fileInfo.Name, iconIndex);
                        item.Name = $"File{Guid.NewGuid().ToString("N")}";
                        if (bSoftlink)
                        {
                            var targetPath = GetTargetPath(file);
                            item.ForeColor = File.Exists(targetPath) ? Color.Blue : Color.Red;
                            item.ToolTipText = targetPath;
                        }
                        item.SubItems.Add(ConvertBytesToFileSize(fileInfo.Length)); // 添加文件大小列
                        item.SubItems.Add(fileInfo.LastWriteTime.ToString("G"));
                        item.Tag = file; // 可以将完整路径存储在Tag属性中，以便后续使用
                        lvw.Items.Add(item);
                    }
                }
                else
                {
                    // 遍历每个驱动器添加盘符
                    // 将驱动器添加到ListView中
                    lvw.Columns[2].Text = "Available";
                    foreach (TreeNode subNode in node.Nodes)
                    {
                        ListViewItem item = new ListViewItem(subNode.Text);
                        item.Name = subNode.Name;
                        if (subNode.Name.StartsWith("Drive"))
                        {
                            item.ImageIndex = IDX_DRIVE;
                            var drive = subNode.Tag.ToString();
                            var driveInfo = new DriveInfo(drive);
                            item.SubItems.Add(ConvertBytesToFileSize(driveInfo.TotalSize)); // 添加磁盘大小列
                            item.SubItems.Add(ConvertBytesToFileSize(driveInfo.AvailableFreeSpace)); // 可用大小
                        }
                        else
                        {
                            item.ImageIndex = IDX_FOLDER;
                            //item.SubItems.Add("");
                            //var dirInfo = new DirectoryInfo(subNode.Tag.ToString());
                            //item.SubItems.Add(dirInfo.LastWriteTime.ToString("G"));
                        }
                        item.Tag = subNode.Tag; // 可以将完整路径存储在Tag属性中，以便后续使用
                        lvw.Items.Add(item);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lvw"></param>
        public static void ResizeListViewColumns(ListView lvw)
        {
            // 设置ListView的列宽以适应内容
            lvw.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            int iWidth = 30; // 留出滚动条宽度
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
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ConvertBytesToFileSize(long bytes)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tvw"></param>
        public static void AddDrives(TreeView tvw)
        {
            if (tvw.ImageList == null && tvw.StateImageList ==null)
            {
                //ModifyTreeViewStyle(tvw.Handle);
                tvw.ImageList = new ImageList();
                tvw.ImageList.ImageSize = new Size(16, 16);
                tvw.ImageList.ColorDepth = ColorDepth.Depth32Bit;

                var driveIcon = GetSystemIcon("C:"); // 获取通用磁盘的图标
                var folderIcon = GetSystemIcon("."); // 获取通用的文件夹图标
                //var computerIcon = SystemIcons.WinLogo;
                Icon computerIcon = Properties.Resources.computer;
                Icon openedFolderIcon = Properties.Resources.opend_folder;
                tvw.ImageList.Images.Add(driveIcon);        // 0
                tvw.ImageList.Images.Add(folderIcon);       // 1
                tvw.ImageList.Images.Add(computerIcon);     // 2
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
                // 创建我的电脑作为根节点
                TreeNode rootNode = new TreeNode(_cumputerName, IDX_COMPUTER, IDX_COMPUTER);
                rootNode.Tag = "";
                tvw.Nodes.Add(rootNode);

                // 添加特殊目录
                string[] specilFolders = new string[]
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    //Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                };
                foreach (string specilFolder in specilFolders)
                {
                    var folderNode = new TreeNode(Path.GetFileName(specilFolder), IDX_FOLDER, IDX_FOLDER);
                    folderNode.Name = $"Folder{Guid.NewGuid().ToString("N")}";
                    folderNode.Tag = specilFolder;
                    rootNode.Nodes.Add(folderNode);
                    AddSubFolders(folderNode); // 预加载子文件夹
                }

                //string[] volumes = Environment.GetLogicalDrives();

                // 获取计算机上所有的驱动器信息
                DriveInfo[] drives = DriveInfo.GetDrives();

                // 遍历每个驱动器添加盘符
                foreach (DriveInfo drive in drives)
                {
                    if (drive.IsReady)
                    {
                        var driveLabel = drive.VolumeLabel;
                        
                        if (string.IsNullOrWhiteSpace(driveLabel))
                            driveLabel = "Local Drive"; // 如果没有标签，则使用默认名称:本地磁盘
                        var driveNode = new TreeNode($"{driveLabel}({drive.Name.Substring(0, 2)})", IDX_DRIVE, IDX_DRIVE);

                        driveNode.Name = $"Drive{Guid.NewGuid().ToString("N")}";//$"Drive{driveNode}";
                        driveNode.Tag = drive.Name; // 可以将完整路径存储在Tag属性中，以便后续使用
                        rootNode.Nodes.Add(driveNode);
                        AddSubFolders(driveNode);
                    }
                }
                rootNode.Expand();
            }
            catch (Exception ex)
            {
                // 处理任何可能发生的异常，例如路径不存在或没有访问权限等
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="clear"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddSubFolders_old(TreeNode parentNode, bool clear = false)
        {
            if (parentNode == null)
                throw new ArgumentNullException("parentNode");

            if (clear) parentNode.Nodes.Clear();

            if (parentNode.Nodes.Count == 0)
            {
                string parentPath = parentNode.Tag.ToString();
                parentNode.TreeView.Cursor = Cursors.WaitCursor;
                parentNode.TreeView.BeginUpdate();

                try
                {
                    // 获取指定路径下的所有子目录
                    string[] directories = Directory.GetDirectories(parentPath);

                    // 遍历子目录并为每个子目录创建一个节点
                    foreach (string directory in directories)
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(directory);
                        if (!dirInfo.Name.StartsWith("$"))
                        {
                            // 创建节点并设置其文本为目录名
                            TreeNode node = new TreeNode(Path.GetFileName(directory), IDX_FOLDER, IDX_FOLDER);
                            node.Name = $"Folder{Guid.NewGuid().ToString("N")}";
                            node.Tag = directory; // 可以将完整路径存储在Tag属性中，以便后续使用
                            if (IsSymbolicLink(directory, out var targetPath))
                            {
                                // 标记符号链接为蓝色字体，目标不存在的标记为红色字体
                                node.ForeColor = Directory.Exists(targetPath) ? Color.Blue : Color.Red;
                                node.ToolTipText = targetPath;
                            }
                            // 将节点添加到父节点中
                            parentNode.Nodes.Add(node);
                        }
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
        
        public static void AddSubFolders(TreeNode node, bool reload = false)
        {
            if (node.Nodes.Count > 0 && !reload)
                return;

            string path = node.Tag.ToString();

            try
            {
                // 清除现有子节点（如果是重新加载）
                if (reload)
                    node.Nodes.Clear();

                // 获取当前目录下的所有子目录
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                DirectoryInfo[] subDirs = dirInfo.GetDirectories();

                foreach (DirectoryInfo dir in subDirs)
                {
                    // 检查是否有访问权限
                    try
                    {
                        // 创建子节点
                        TreeNode subNode = new TreeNode(dir.Name)
                        {
                            Name = "Folder." + dir.FullName.GetHashCode(),
                            Tag = dir.FullName,
                            ImageIndex = 1,      // 文件夹图标索引
                            SelectedImageIndex = 1
                        };

                        // 关键部分：检查此子目录是否还有子目录
                        if (hasSubdirectories(dir))
                        {
                            // 添加一个虚拟子节点以显示展开符号
                            subNode.Nodes.Add(new TreeNode() { Text = "Loading..." });
                        }

                        node.Nodes.Add(subNode);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // 跳过没有权限访问的目录
                    }
                }
            }
            catch (Exception ex)
            {
                // 处理异常情况
                Console.WriteLine($"Error loading directory structure: {ex.Message}");
            }

            bool hasSubdirectories(DirectoryInfo dir)
            {
                try
                {
                    return dir.GetDirectories().Length > 0;
                }
                catch (Exception)
                {

                    return false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="tvw"></param>
        /// <param name="lvw"></param>
        public static void AddFolder(string path, TreeView tvw, ListView lvw)
        {
            if (Directory.Exists(path))
            {
                string[] pathArray = path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                TreeNode node = null;
                var rootNode = tvw.Nodes[0];
                foreach (TreeNode driveNode in rootNode.Nodes)
                {
                    var driveText = driveNode.Text;
                    if (driveText.EndsWith($"({pathArray[0]})", StringComparison.OrdinalIgnoreCase))
                    {
                        node = driveNode;
                        break;
                    }
                }

                // 递推添加子节点
                if (node != null)
                {
                    AddSubFolders(node);
                    for (int i = 1; i < pathArray.Length; i++)
                    {
                        node = findNodeByText(node.Nodes, pathArray[i]);
                        if (null != node)
                            AddSubFolders(node);
                    }

                    tvw.SelectedNode = node;
                    ReloadFolderFiles(node, lvw);
                }
            }
            else
            {
                SystemSounds.Beep.Play();
            }
        }

        private static TreeNode findNodeByText(TreeNodeCollection nodes, string text)
        {
            foreach (TreeNode node in nodes)
                if (node.Text.Equals(text, StringComparison.OrdinalIgnoreCase))
                    return node;
            return null;
        }

        #region 提取文件夹图标
        private const uint SHGFI_ICON = 0x100;    // 获取图标
        private const uint SHGFI_SMALLICON = 0x1; // 小图标
        private const uint SHGFI_LARGEICON = 0x0; // 大图标
        private const uint SHGFI_USEFILEATTRIBUTES = 0x10;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;

        const uint SHGFI_DISPLAYNAME = 0x000000200;  // 获取显示名
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
            SHGetFileInfo(path, FILE_ATTRIBUTE_DIRECTORY, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
            var icon = Icon.FromHandle(shinfo.hIcon); // 转换图标句柄为Icon对象，注意这里不需要手动Dispose，因为Icon是从系统获取的句柄转换的。
            //DestroyIcon(shinfo.hIcon);
            return icon;
        }

        //private static Icon GetComputerIcon(out string computerName)
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
//private static extern int SHGetKnownFolderPath(ref Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);

//[DllImport("user32.dll")]
//private static extern bool DestroyIcon(IntPtr hIcon);
#endregion

#if false
        /// <summary>
        /// 尝试创建符号软链接
        /// </summary>
        /// <param name="link">创建软链接的保存位置</param>
        /// <param name="target">软链接的连接目标来源</param>
        /// <returns></returns>
        [Obsolete("由于管理员权限问题停用，改用 TryMakeLinkByCore()")]
        public static bool TryMakeLink(string link, string target)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = $"/c mklink {(Directory.Exists(target) ? "/d " : "")}\"{link}\" \"{target}\"",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            bool ret = false;
            try
            {
                var process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                string stdOut = process.StandardOutput.ReadToEnd();
                string stdErr = process.StandardError.ReadToEnd();
                process.WaitForExit();

                ret = process.ExitCode == 0;
                process.Dispose();
                if (ret)
                    Debug.Print(stdOut);
                else
                    Debug.Print(stdErr);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
            
            return ret;
        }
#endif

        private static void extractEmbeddedResource(string resourceName, string outputPath)
        {
            // 获取当前程序集
            Assembly assembly = Assembly.GetExecutingAssembly();

            // 使用流获取嵌入的资源
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    Debug.Print($"找不到嵌入的资源: {resourceName}");
                    return;
                }

                // 创建输出文件流
                using (FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    // 将嵌入的资源写入到文件
                    stream.CopyTo(fileStream);
                }
            }

            Console.WriteLine($"资源已提取到: {outputPath}");
        }

        //public static bool TryMakeLinkByCore(string link, string target, bool showLog)
        public static bool TryMakeLinkByCore(List<dtoSLGItem> items, bool showLog)
        {
            bool ret = false;
            // 检查 SlgCore.exe 文件是否存在
            //var outputFile = Path.Combine(Path.GetTempPath(), "SlgCore.exe");
            var outputFile = Path.Combine(Application.StartupPath, "SlgCore.exe");
            if (!File.Exists("SlgCore.exe"))
            {
                // 从嵌入的资源中释放文件：名字空间.文件名.扩展名
                var resourceName = "SymbolicLinkGenerator.SlgCore.exe";
                extractEmbeddedResource(resourceName, outputFile);
            }

            // 检查 SlgCore 进程是否已存在, 没有则需要启动它
            if (Process.GetProcessesByName("SlgCore").Length == 0)
            {
                var p = new Process();
                p.StartInfo.FileName = outputFile;
                p.StartInfo.Verb = "runas"; 

                if (showLog)
                { 
                    p.StartInfo.CreateNoWindow = false;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                }
                else
                {
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                }

                p.StartInfo.UseShellExecute = true;
                p.Start();
            }

            NamedPipeClientStream pipe = null;
            try
            {
                pipe = new NamedPipeClientStream(".", "Global\\SlgFilePipe", PipeDirection.InOut);
                pipe.Connect(1000); // 连接到管道
                Debug.Print("连接 SlgCore 管道成功");

                //using (var writer = new StreamWriter(pipe))
                //    writer.WriteLine($"{link},{target}");

                var helper = new DataHelper() { Items = items.ToArray() };
                var json = helper.ToJson();
                byte[] buffer = Encoding.UTF8.GetBytes(json);

                // 分块发送
                int offset = 0;
                int chunkSize = 1024; // 每次发送1024字节
                while (offset < buffer.Length)
                {
                    int bytesToSend = Math.Min(chunkSize, buffer.Length - offset);
                    pipe.Write(buffer, offset, bytesToSend);
                    offset += bytesToSend;
                }
                pipe.Flush();
                Debug.Print("向 SlgCore 管道发送数据完毕");
                //ret = true;

                buffer = new byte[4];
                int iRead =  pipe.Read(buffer, 0, buffer.Length);
                // 服务端返回成功创建的软连接数
                var response = BitConverter.ToInt32(buffer, 0);
                Debug.Print($"收到服务端应答:{response}");
                ret = response == items.Count;
            }
            catch (TimeoutException)
            {
                Debug.Print("与 SlgCore 管道通讯超时");
            }
            catch (Exception ex)
            {
                Debug.Print($"与 SlgCore 管道通讯异常: {ex.Message}");
            }
            finally
            {
                Debug.Print("与 SlgCore 管道通讯结束");
                pipe?.Dispose();
            }
            return ret;
        }

        public static void KillProcessCore()
        {
            foreach(var p in Process.GetProcessesByName("SlgCore"))
                p.Kill();
            var exePath = Path.Combine(Path.GetTempPath(), "SlgCore.exe");
            //if(File.Exists(exePath))
            //    File.Delete(exePath); 
        }

        #region 删除文件到回收站
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public uint wFunc;
            public string pFrom;
            public string pTo;
            public ushort fFlags;
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        private const int FO_DELETE = 0x0003;
        private const int FOF_ALLOWUNDO = 0x0040; // 64

        /// <summary>
        /// 删除到回收站
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool MoveToRecycle(string filePath)
        {
            try
            {
                SHFILEOPSTRUCT fileOp = new SHFILEOPSTRUCT();
                fileOp.wFunc = FO_DELETE;
                fileOp.pFrom = filePath + "\0"; // 文件路径以 null 结尾
                fileOp.pTo = null;
                fileOp.fFlags = FOF_ALLOWUNDO; // 允许撤销
                fileOp.hwnd = IntPtr.Zero;
                fileOp.fAnyOperationsAborted = false;
                fileOp.hNameMappings = IntPtr.Zero;
                fileOp.lpszProgressTitle = null;

                int result = SHFileOperation(ref fileOp);
                if (result != 0)
                    MessageBox.Show($"删除操作失败，错误代码: {result}");
                return result == 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生异常: {ex.Message}");
            }
            return false;
        }
        #endregion

        #region 修改 TreeView 为系统样式
        /*
        // Win32 API 函数
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        // 消息常量
        private const int WM_SETREDRAW = 0x000B;
        private const int TVS_LINESATROOT = 0x0001;
        private const int TVS_HASLINES = 0x0002;
        private const int TVS_HASBUTTONS = 0x0004;
        private const int TVS_TRACKSELECT = 0x0008;

        private static void ModifyTreeViewStyle(IntPtr handle)
        {
            // 发送消息以修改 TreeView 样式
            SendMessage(handle, 0x0001, (IntPtr)(TVS_HASLINES | TVS_HASBUTTONS | TVS_LINESATROOT), IntPtr.Zero);
        }
        */
        internal static void SetupAeroStyle(TreeView tvw)
        {
            // 设置 TreeView 样式以支持 Aero 效果
            tvw.HotTracking = true;           // 启用热追踪效果
            tvw.ShowLines = false;            // 隐藏传统连接线
            tvw.ShowPlusMinus = true;         // 显示展开/折叠按钮
            tvw.ShowRootLines = false;        // 隐藏根节点连线
            //tvw.BorderStyle = BorderStyle.None; // 无边框样式
            tvw.FullRowSelect = true;
            tvw.HideSelection = false;
            tvw.ItemHeight = 24;
            tvw.Indent = 20;
            SetWindowTheme(tvw.Handle, "explorer", null);
        }

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        internal static void SetupAeroStyle(ListView lvw)
        {
            lvw.View = View.Details;
            lvw.FullRowSelect = true;
            lvw.GridLines = false;
            lvw.MultiSelect = true;
            lvw.HideSelection = false;
            //lvw.DoubleBuffered = true;

            // 设置头部样式
            lvw.OwnerDraw = false; // 让系统处理绘制
            SetWindowTheme(lvw.Handle, "explorer", null);
        }
        #endregion
    }
}
