using NotificationUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SymbolicLinkGenerator
{
    public partial class frmMain : Form
    {
        private const int SPRING_BORDER = 48;
        private ListView _activeListView;
        private Notifier _notifier;
        public frmMain()
        {
            InitializeComponent();
            _notifier = new Notifier();

            this.Icon = Properties.Resources.form;
            this.KeyPreview = true;

            // 设计器里 Dock = Fill 会显示错乱
            tscSrc.Dock = DockStyle.Fill;
            tscDst.Dock = DockStyle.Fill;

            #region 主面板
            spcMain.SplitterDistance = spcMain.Width / 2;
            spcMain.Panel1MinSize = spcMain.Width / 3;
            spcMain.Panel2MinSize = spcMain.Width / 3;
            spcMain.SplitterMoved += (s, e) =>
            {
                //Debug.Print("【spcMain.SplitterMoved】");
                spcMain.SuspendLayout();
                txtSrcPath.Width = spcMain.Panel1.Width - btnSrc.Width - lblSrcPath.Width - SPRING_BORDER;
                ExplorerHelper.ResizeListViewColumns(lvwSrc);
                txtDstPath.Width = spcMain.Panel2.Width - btnDst.Width - lblDstPath.Width - SPRING_BORDER;
                ExplorerHelper.ResizeListViewColumns(lvwDst);
                spcMain.ResumeLayout();
            };
            
            spcMain.Resize += (s, e) =>
            {
                //Debug.Print("【spcMain.Resize】");
                spcMain.SuspendLayout();
                ExplorerHelper.ResizeListViewColumns(lvwSrc);
                ExplorerHelper.ResizeListViewColumns(lvwDst);
                spcMain.ResumeLayout();
            };
            // 是否折叠目标列表界面
            spcMain.Panel1Collapsed = !Properties.Settings.Default.ShowTarget;
            #endregion

            #region 左面板
            spcSrc.SplitterDistance = spcSrc.Width / 3;
            spcSrc.FixedPanel = FixedPanel.Panel1;
            spcSrc.Panel1MinSize = 100;
            spcSrc.Panel2MinSize = 100;
            spcSrc.SplitterMoved += (s, e) => ExplorerHelper.ResizeListViewColumns(lvwSrc);

            btnSrc.Image = Properties.Resources.treeview.ToBitmap();
            btnSrc.CheckedChanged += (s, e) =>
            {
                spcSrc.Panel1Collapsed = !btnSrc.Checked;
                ExplorerHelper.ResizeListViewColumns(lvwSrc);
            };

            txtSrcPath.KeyPress += txtDirKeyPressEventHandler;
            txtSrcPath.GotFocus += txtDirGotFocusEventHandler;

            //tvwSrc.HideSelection = false;
            tvwSrc.BackColor = SystemColors.ControlLight;
            tvwSrc.AfterSelect += tvwAfterSelectEventHandler;
            tvwSrc.BeforeExpand += tvwBeforeExpandEventHandler;

            //lvwSrc.FullRowSelect = true;
            lvwSrc.MultiSelect = true;
            lvwSrc.View = View.Details;
            lvwSrc.BackColor = SystemColors.ControlLight;
            lvwSrc.DoubleClick += lvwDoubleClickEventHandler;
            lvwSrc.ItemDrag += lvwSrcItemDragEventHandler;
            lvwSrc.SelectedIndexChanged += lvwSelectedIndexChangedEventHandler;
            lvwSrc.KeyDown += lvwSrcKeyDownEventHandler;
            lvwSrc.GotFocus += (s, e) => _activeListView = lvwSrc;
            lvwSrc.LostFocus += (s, e) => { if (_activeListView == lvwSrc) _activeListView = null; };
            //lvwSrc.LabelEdit = true;
            //lvwSrc.AfterLabelEdit += lvwAfterLabelEditEventHandler;
            #endregion

            #region 右面板
            spcDst.SplitterDistance = spcSrc.Width / 3;
            spcDst.FixedPanel = FixedPanel.Panel1;
            spcDst.Panel1MinSize = 100;
            spcDst.Panel2MinSize = 100;
            spcDst.SplitterMoved += (s, e) => ExplorerHelper.ResizeListViewColumns(lvwDst);

            btnDst.Image = Properties.Resources.treeview.ToBitmap();
            btnDst.CheckedChanged += (s, e) =>
            {
                spcDst.Panel1Collapsed = !btnDst.Checked;
                ExplorerHelper.ResizeListViewColumns(lvwDst);
            };

            txtDstPath.KeyPress += txtDirKeyPressEventHandler;
            txtDstPath.GotFocus += txtDirGotFocusEventHandler;

            //tvwDst.HideSelection = false;
            tvwDst.AfterSelect += tvwAfterSelectEventHandler;
            //tvwDst.NodeMouseDoubleClick += tvwNodeMouseDoubleClickEventHnadler;
            tvwDst.BeforeExpand += tvwBeforeExpandEventHandler;

            //lvwDst.AllowDrop = false;
            lvwDst.AllowDrop = true;
            //lvwDst.FullRowSelect = true;
            lvwDst.MultiSelect = true;
            lvwDst.View = View.Details;
            lvwDst.DoubleClick += lvwDoubleClickEventHandler;
            lvwDst.DragEnter += lvwDstDragEnterEventHandler;
            //lvwDst.DragOver += lvwDestDragOverEventHandler;
            lvwDst.DragDrop += lvwDstDragDropEventHandler;
            lvwDst.KeyDown += lvwDstKeyDownEventHandler;
            //lvwDst.ItemSelectionChanged += (s, e) => 
            //{
            //    var lvw = (ListView)s;
            //    var item = e.Item;
            //    //lblDstPath.Text = item.ToolTipText;
            //};
            lvwDst.SelectedIndexChanged += lvwSelectedIndexChangedEventHandler;
            lvwDst.GotFocus += (s, e) => _activeListView = lvwDst;
            lvwDst.LostFocus += (s, e) => { if (_activeListView == lvwDst) _activeListView = null; };
            lvwDst.LabelEdit = true;
            lvwDst.BeforeLabelEdit += lvwDstBeforeLabelEditEventHandler;
            lvwDst.AfterLabelEdit += lvwDstAfterLabelEditEventHandler;
            #endregion

            #region 主菜单
            mnuShowLog.Checked = Properties.Settings.Default.ShowLog;
            mnuShowLog.Click += (s, e) =>
            {
                mnuShowLog.Checked = !mnuShowLog.Checked;
            };
            mnuShowTarget.Checked = Properties.Settings.Default.ShowTarget;
            mnuShowTarget.Click += (s, e) =>
            {
                mnuShowTarget.Checked = !mnuShowTarget.Checked;
                spcMain.Panel1Collapsed = !mnuShowTarget.Checked;
                txtDstPath.Width = spcMain.Panel2.Width - btnDst.Width - lblDstPath.Width - SPRING_BORDER;
                tsDst.Update();
            };
            mnuFileExit.Click += (s, e) => this.Close();
            mnuHelpAbout.Click += (s, e) => showHelp();
            msMain.Visible = false;
            #endregion
        }
        
        #region EVENTHANDLER

        private void lvwDstBeforeLabelEditEventHandler(object sender, LabelEditEventArgs e)
        {
            var lvw = (ListView)sender;
            var selectItem = lvw.Items[e.Item];
            var path = selectItem.Tag.ToString();
            if (ExplorerHelper.IsSymbolicLink(path, out _))
                e.CancelEdit = false;
            else
            {
                e.CancelEdit = true;
                SystemSounds.Beep.Play();
            }
        }

        private void lvwDstAfterLabelEditEventHandler(object sender, LabelEditEventArgs e)
        {
            e.CancelEdit = true; // 默认取消编辑
            // 验证编辑结果
            if (string.IsNullOrEmpty(e.Label))
                return;

            var lvw = (ListView)sender;
            var selectItem = lvw.Items[e.Item];
            var path = selectItem.Tag.ToString();
            //if (ExplorerHelper.IsSymbolicLink(path, out _))
            //{
            // 仅限符号链接可以改名
            if (ExplorerHelper.IsInvalidFileName(e.Label))
            {
                // 文件名包含非法字符
                MessageBox.Show("Invalid filename with special chars", this.Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                // 保存修改
                try
                {
                    var newPath = Path.Combine(Path.GetDirectoryName(path), e.Label);
                    File.Move(path, newPath);
                    Debug.Print($"Item {e.Item} text changed to: {e.Label}");
                    selectItem.Tag = newPath;
                    e.CancelEdit = false; // 确认修改

                    //new frmToast(this, "Rename symbolic link file success", 3000);
                    _notifier.ShowSucess("Rename success");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Exception: {ex.Message}", this.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            //}
            //else
            //{
            //    MessageBox.Show("Support rename a symbolic link file only", this.Text,
            //                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
        }


        private void txtDirGotFocusEventHandler(object sender, EventArgs e)
        {
            var txt = (ToolStripTextBox)sender;
            txt.SelectionStart = 0;
            txt.SelectionLength = txt.TextLength;
        }

        private void txtDirKeyPressEventHandler(object sender, KeyPressEventArgs e)
        {
            var txt = (ToolStripTextBox)sender;
            if (e.KeyChar == '\r')
            {
                var tvw = txt.Name.Contains("Src") ? tvwSrc : tvwDst;
                var lvw = txt.Name.Contains("Src") ? lvwSrc : lvwDst;
                //var lblCount = txt.Name.Contains("Src") ? lblSrcCount : lblDstCount;
                //var lblSelCount = txt.Name.Contains("Src") ? lblSrcSelCount : lblDstSelCount;
                ExplorerHelper.AddFolder(txt.Text, tvw, lvw);
                lvwItemCountUpdate(lvw);
                if (tvw == tvwDst)
                    lvw.AllowDrop = tvw.SelectedNode != null 
                                    && tvw.SelectedNode.Parent != null;
                e.Handled = true;
            }
        }

        private void lvwSrcKeyDownEventHandler(object sender, KeyEventArgs e)
        {
            var lvw = (ListView)sender;
            if (e.Control && e.KeyCode == Keys.C)
            {
                copySelectedItems(lvw);
            }
        }

        private void lvwSrcItemDragEventHandler(object sender, ItemDragEventArgs e)
        {
            var lvw = (ListView)sender;

            // 支持多选拖拽
            if (lvw.SelectedItems[0].Name.StartsWith("Drive"))
            {
                SystemSounds.Beep.Play();
                Debug.Print("磁盘不支持创建软连接");
            }
            else
                lvw.DoDragDrop(lvw.SelectedItems, DragDropEffects.Copy);
        }

        private void lvwDstKeyDownEventHandler(object sender, KeyEventArgs e)
        {
            if (lvwDst.SelectedItems.Count > 0)
            {
                if (e.KeyCode == Keys.F2)
                    lvwDst.SelectedItems[0].BeginEdit();
                else if (e.KeyCode == Keys.F3)
                    explorerSelectItem();
                else if (e.KeyCode == Keys.F5)
                    loadTreeNodes(tvwDst.SelectedNode, true);
                else if (e.KeyCode == Keys.Enter)
                    lvwDoubleClickEventHandler(sender, e);
                else if (e.KeyCode == Keys.Delete)
                    deleteSelectItems();
                else if (e.Control)
                {
                    if (e.KeyCode == Keys.C)
                        copySelectedItems(lvwDst); // 复制
                    // 无选中项时，不会触发 KeyDown 事件，
                    // 所以以下按键要在 OnKeyDown 中也处理一下
                    else if (e.KeyCode == Keys.V)
                        pasteFromClipboard(); // 粘贴
                    else if (e.KeyCode == Keys.A)
                        foreach (ListViewItem item in lvwDst.Items)
                            item.Selected = !"..".Equals(item.Text);
                    else if (e.KeyCode == Keys.R)
                        foreach (ListViewItem item in lvwDst.Items)
                            item.Selected = !"..".Equals(item.Text) && !item.Selected;
                }
            }
        }

        private void lvwDstDragDropEventHandler(object sender, DragEventArgs e)
        {
            var lvw = (ListView)sender;
            var tvw = tvwDst;

            if (null == tvw.SelectedNode
               || null == tvw.SelectedNode.Parent)
            {
                Debug.Print("无法在当前位置拖放创建软连接");
                return;
            }

            bool needReload = false;
            var items = new List<dtoSLGItem>();
            if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)))
            {
                // 拖入的是内部的 ListViewItem，支持多选拖放
                var draggedItems = (ListView.SelectedListViewItemCollection)e.Data.GetData(typeof(ListView.SelectedListViewItemCollection));
                foreach (ListViewItem draggedItem in draggedItems)
                {
                    //var link = Path.Combine(txtDstPath.Text, Path.GetFileName(draggedItem.Tag.ToString()));
                    var currPath = tvw.SelectedNode.Tag.ToString();
                    var link = Path.Combine(currPath, Path.GetFileName(draggedItem.Tag.ToString()));
                    var target = draggedItem.Tag.ToString();
                    //if (ExplorerHelper.TryMakeLinkByCore(link, target, mnuShowLog.Checked))
                    //    needReload = true;
                    items.Add(new dtoSLGItem() { Link = link, SourcePath = target });
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // 直接从资源管理器拖入文件
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    // 在这里可以处理拖放的文件
                    foreach (string file in files)
                    {
                        var currPath = tvw.SelectedNode.Tag.ToString();
                        var link = Path.Combine(currPath, Path.GetFileName(file));
                        var target = file;
                        //if (ExplorerHelper.TryMakeLinkByCore(link, target, mnuShowLog.Checked))
                        //    needReload = true;
                        items.Add(new dtoSLGItem() { Link = link, SourcePath = target });
                    }
                }
            }

            needReload = items.Count > 0 && 
                ExplorerHelper.TryMakeLinkByCore(items, mnuShowLog.Checked);
            if (needReload)
            {
                //ExplorerHelper.AddSubForlders(tvw.SelectedNode, true);
                //ExplorerHelper.ReloadListView(tvw.SelectedNode, lvw);
                //lblDstCount.Text = $"Total {lvw.Items.Count - 1} items";
                //lblDstSelCount.Text = "";
                loadTreeNodes(tvw.SelectedNode, true);
                //new frmToast(this, "Create symbolic link file success", 3000);
                _notifier.ShowSucess("Create symbolic link success");
            }
        }

        //private void lvwDestDragOverEventHandler(object sender, DragEventArgs e)
        //{
        //    if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection))
        //        || e.Data.GetDataPresent(DataFormats.FileDrop))
        //        e.Effect = DragDropEffects.Copy; // 在拖拽过程中持续显示移动效果
        //}

        private void lvwDstDragEnterEventHandler(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection))
                || e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy; // 设置拖拽效果为移动
            else
                e.Effect = DragDropEffects.None; // 不允许拖放
        }

        private void lvwSelectedIndexChangedEventHandler(object sender, EventArgs e)
        {
            var lvw = (ListView)sender;
            var lbl = lvw.Name.Contains("Src") ? lblSrcSelCount : lblDstSelCount;
            lbl.Text = $"Selected {lvw.SelectedItems.Count} items";
        }

        private void lvwDoubleClickEventHandler(object sender, EventArgs e)
        {
            var lvw = (ListView)sender;
            var tvw = lvw.Name.Contains("Src") ? tvwSrc: tvwDst;
            var txt = lvw.Name.Contains("Src") ? txtSrcPath : txtDstPath;
            //var lblCount = lvw.Name.Contains("Src") ? lblSrcCount : lblDstCount;
            //var lblSelCount = lvw.Name.Contains("Src") ? lblSrcSelCount : lblDstSelCount;
            var selectItem = lvw.SelectedItems[0];
            if (null != selectItem)
                if (selectItem.Name.StartsWith("File"))
                {
                    try
                    {
                        var process = new Process();
                        process.StartInfo.FileName = selectItem.Tag.ToString();
                        process.StartInfo.UseShellExecute = true;
                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        _notifier.ShowWarning(ex.Message);
                    }
                }
                else
                {
                    var node = tvw.Nodes.Find(selectItem.Name, true).First();
                    if (node != null)
                    {
                        tvw.SelectedNode = node;
                        ExplorerHelper.AddSubFolders(node);
                        ExplorerHelper.ReloadFolderFiles(node, lvw);
                        lvwItemCountUpdate(lvw);
                        if (tvw == tvwDst)
                            lvw.AllowDrop = node.Parent != null;
                        txt.Text = node.Tag.ToString();
                    }
                }
            else
                SystemSounds.Beep.Play();
        }

        private void tvwAfterSelectEventHandler(object sender, TreeViewEventArgs e)
        {
            loadTreeNodes(e.Node, !e.Node.IsExpanded);
        }

        private void tvwNodeMouseDoubleClickEventHnadler(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 这里不能用 e.Node， 因为测试发现 e.Node 为双击结束后鼠标停留位置的节点
                var currNode = e.Node.TreeView.SelectedNode;
                Debug.Print(currNode.FullPath);
                loadTreeNodes(currNode, !currNode.IsExpanded);
            }
        }

        private void tvwBeforeExpandEventHandler(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && 
                e.Node.Nodes[0].Text == "Loading...")
            {
                e.Node.Nodes.Clear();
                ExplorerHelper.AddSubFolders(e.Node);
            }
        }
        #endregion

        private void lvwItemCountUpdate(ListView lvw)
        {
            var lbl = lvw.Name.Contains("Src") ? lblSrcCount : lblDstCount;
            if (lvw.Items[0].Text == "..")
                lbl.Text = $"Total {lvw.Items.Count - 1} items";
            else
                lbl.Text = $"Total {lvw.Items.Count} items";

            var lblSel = lvw.Name.Contains("Src") ? lblSrcSelCount : lblDstSelCount;
            lblSel.Text = "";
            //lblDstPath.Text = "";
        }

        private void loadTreeNodes(TreeNode currNode, bool clearall)
        {
            var tvw = currNode.TreeView;
            var lvw = tvw.Name.Contains("Src") ? lvwSrc : lvwDst;
            var txt = tvw.Name.Contains("Src") ? txtSrcPath : txtDstPath;
            //var lblCount = tvw.Name.Contains("Src") ? lblSrcCount : lblDstCount;
            //var lblSelCount = tvw.Name.Contains("Src") ? lblSrcSelCount : lblDstSelCount;

            if (currNode.Parent != null)
            {
                ExplorerHelper.AddSubFolders(currNode, clearall);
                ExplorerHelper.ReloadFolderFiles(currNode, lvw);
                lvwItemCountUpdate(lvw);
                txt.Text = currNode.Tag.ToString();
            }
            else
            {
                //ExplorerHelper.AddDrives(tvw);
                ExplorerHelper.ReloadFolderFiles(currNode, lvw);
                lvwItemCountUpdate(lvw);
                txt.Text = "";
            }

            //lvwDst.AllowDrop = (null != tvwDst.SelectedNode
            //                    && null != tvwDst.SelectedNode.Parent);
            //Debug.Print($"lvwDst.AllowDrop={lvwDst.AllowDrop}");
        }

        /// <summary>
        /// 通过资源管理器打开路径, 符号文件定位源路径
        /// </summary>
        private void explorerSelectItem()
        {
            var path = lvwDst.SelectedItems[0].Tag.ToString();
            if ((File.Exists(path) || Directory.Exists(path)))
            {
                if (ExplorerHelper.IsSymbolicLink(path, out string targetPath))
                {
                    if(File.Exists(targetPath)|| Directory.Exists(targetPath))
                        Process.Start("explorer.exe", $"/select, {targetPath}");
                    else
                        SystemSounds.Beep.Play();
                }
                else
                    // 非符号链接文件，直接打开
                    Process.Start("explorer.exe", $"/select, {path}");
            }
            else
                SystemSounds.Beep.Play();
        }

        private void deleteSelectItems()
        {
            bool allSymbolicLink = true;
            foreach(ListViewItem selectItem in lvwDst.SelectedItems)
                if (!ExplorerHelper.IsSymbolicLink(selectItem.Tag.ToString(), out _))
                {
                    allSymbolicLink = false;
                    break;
                }

            if (allSymbolicLink)
            {
                var msg = lvwDst.SelectedItems.Count > 1
                   ? $"Are you sure you want to delete the {lvwDst.SelectedItems.Count} selected items, including '{lvwDst.SelectedItems[0].Text}' and others?"
                   : $"Are you sure you want to delete the selected item '{lvwDst.SelectedItems[0].Text}'?";

                if (MessageBox.Show(msg, this.Text,
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question) == DialogResult.OK)
                {
                    TreeNode pNode = null;
                    foreach (ListViewItem selectItem in lvwDst.SelectedItems)
                    {
                        var path = selectItem.Tag.ToString();
                        if(ExplorerHelper.MoveToRecycle(path))
                        {
                            Debug.Print($"{path} 已删除");
                            lvwDst.Items.Remove(selectItem);
                            if (selectItem.Name.StartsWith("Folder"))
                                if (null == pNode)
                                {
                                    var fNode = tvwDst.Nodes.Find(selectItem.Name, true)
                                                            .First();
                                    if (fNode != null)
                                        pNode = fNode.Parent;
                                }
                        }
                        else
                            Debug.Print($"{path} 删除失败");
                    }
                    if (null != pNode)
                        ExplorerHelper.AddSubFolders(pNode, true);
                }
            }
            else
            {
                Debug.Print("选中项目中存在非符号链接文件，不支持删除操作");
                SystemSounds.Beep.Play();
            }
        }

        private void copySelectedItems(ListView lvw)
        {
            // 检查是否有选中的项
            if (lvw.SelectedItems.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (ListViewItem item in lvw.SelectedItems)
                    sb.AppendLine(item.Tag.ToString());
                Clipboard.SetText(sb.ToString());
                Debug.Print($"已复制到剪切板: {lvw.SelectedItems.Count}项");
            }
        }

        private void pasteFromClipboard()
        {
            if (tvwDst.SelectedNode != null && tvwDst.SelectedNode.Parent != null)
            {
                var currPath = tvwDst.SelectedNode.Tag as string;
                if (!string.IsNullOrEmpty(currPath))
                {
                    bool needReload = pasteFromClipboard(lvwDst, currPath, mnuShowLog.Checked);
                    // 刷新界面
                    if (needReload)
                    {
                        loadTreeNodes(tvwDst.SelectedNode, true);
                        //new frmToast(this, "Create symbolic link file success", 3000);
                        _notifier.ShowSucess("Create symbolic link success");
                    }
                }
            }
            else
                Debug.Print("当前位置无法粘贴创建软连接");
        }

        private bool pasteFromClipboard(ListView lvw, string path, bool showLog)
        {
            // 获取剪切板数据对象
            IDataObject dataObject = Clipboard.GetDataObject();
            bool ret = false;

            var items = new List<dtoSLGItem>();
            if (dataObject.GetDataPresent(DataFormats.Text))
            {
                var clipboardText = (string)dataObject.GetData(DataFormats.Text);
                var paths = clipboardText.Split(new[] { "\r\n", "\n" },
                                                StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var fileOrFolder in paths)
                {
                    if (Directory.Exists(fileOrFolder) || File.Exists(fileOrFolder))
                    {
                        var link = Path.Combine(path, Path.GetFileName(fileOrFolder));
                        //ExplorerHelper.TryMakeLinkByCore(link, fileOrFolder, showLog);
                        items.Add(new dtoSLGItem() { Link = link, SourcePath = fileOrFolder });
                        //ret = true;
                    }
                    else
                        Debug.Print($"路径或文件不存在: {fileOrFolder}");
                }
            }
            else if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                // 获取剪切板中的文件路径
                string[] paths = (string[])dataObject.GetData(DataFormats.FileDrop);


                // 将每个文件路径添加为 ListViewItem
                foreach (string fileOrFolder in paths)
                {
                    // 检查路径是否存在
                    if (Directory.Exists(fileOrFolder) || File.Exists(fileOrFolder))
                    {
                        var link = Path.Combine(path, Path.GetFileName(fileOrFolder));
                        //ExplorerHelper.TryMakeLinkByCore(link, fileOrFolder, showLog);
                        items.Add(new dtoSLGItem() { Link = link, SourcePath = fileOrFolder });
                        //ret = true;
                    }
                    else
                        Debug.Print($"路径或文件不存在: {fileOrFolder}");
                }
            }

            ret = items.Count > 0 && ExplorerHelper.TryMakeLinkByCore(items, showLog);
            return ret;
        }

        private void showHelp() =>
            MessageBox.Show(@"Just drag the target file or folder and drop it to where you like to create a symbolic link.

Shortcut keys:
=============
F1     to show this help,
F2     to rename, 
F3     to open in Explorer,
F5     to refresh,
Del    to delete, 
Enter  to execute，
Ctrl+C to copy, 
Ctrl+V to paste as symbolic link, 
Ctrl+A to select all,
Ctrl+R to select inverse.
",
                                $"{Application.ProductName} v{Application.ProductVersion}",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

        #region FORM OVERRIDE

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ExplorerHelper.SetupAeroStyle(lvwSrc);
            ExplorerHelper.SetupAeroStyle(lvwDst);
            ExplorerHelper.SetupAeroStyle(tvwSrc);
            ExplorerHelper.SetupAeroStyle(tvwDst);

            ExplorerHelper.AddDrives(tvwSrc);
            ExplorerHelper.ReloadFolderFiles(tvwSrc.Nodes[0], lvwSrc);
            txtSrcPath.Width = spcSrc.Width - btnSrc.Width - lblSrcPath.Width - SPRING_BORDER;
            lblSrcCount.Text = $"Total {lvwSrc.Items.Count} items";
            lblSrcSelCount.Text = "";

            ExplorerHelper.AddDrives(tvwDst);
            ExplorerHelper.ReloadFolderFiles(tvwDst.Nodes[0], lvwDst);
            txtDstPath.Width = spcDst.Width - btnDst.Width - lblDstPath.Width - SPRING_BORDER;
            lblDstCount.Text = $"Total {lvwDst.Items.Count} items";
            lblDstSelCount.Text = "";
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Alt)
                msMain.Visible = !msMain.Visible;
            if (e.KeyCode == Keys.F1)
                showHelp();
            else if (e.KeyCode == Keys.F5)
            {
                if (_activeListView == lvwSrc)
                    loadTreeNodes(tvwSrc.SelectedNode, true);
                else
                    loadTreeNodes(tvwDst.SelectedNode, true);
            }
            else if (e.Control && _activeListView != null)
            {
                if (e.KeyCode == Keys.A)
                {
                    foreach (ListViewItem item in _activeListView.Items)
                        item.Selected = !item.Text.Equals("..");
                }
                else if (e.KeyCode == Keys.V)
                    pasteFromClipboard();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Properties.Settings.Default.ShowLog = mnuShowLog.Checked;
            Properties.Settings.Default.ShowTarget = mnuShowTarget.Checked;
            Properties.Settings.Default.Save();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            ExplorerHelper.KillProcessCore();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            spcMain.Dock = DockStyle.Fill;
            txtSrcPath.Width = spcSrc.Width - btnSrc.Width - lblSrcPath.Width - SPRING_BORDER;
            txtDstPath.Width = spcDst.Width - btnDst.Width - lblDstPath.Width - SPRING_BORDER;
            if (_notifier != null)
                _notifier.WorkingArea = this.Bounds;
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            if (_notifier != null)
                _notifier.WorkingArea = this.Bounds;
        }
        #endregion
    }
}
