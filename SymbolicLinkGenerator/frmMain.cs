using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;

namespace SymbolicLinkGenerator
{
    public partial class frmMain : Form
    {
        private const int SPRING_BORDER = 48;
        private ListView _activeListView;
        public frmMain()
        {
            InitializeComponent();

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

            tvwSrc.HideSelection = false;
            tvwSrc.BackColor = SystemColors.ControlLight;
            tvwSrc.AfterSelect += tvwAfterSelectEventHandler;

            lvwSrc.FullRowSelect = true;
            lvwSrc.MultiSelect = true;
            lvwSrc.View = View.Details;
            lvwSrc.BackColor = SystemColors.ControlLight;
            lvwSrc.DoubleClick += lvwDoubleClickEventHandler;
            lvwSrc.ItemDrag += lvwSrcItemDragEventHandler;
            lvwSrc.SelectedIndexChanged += lvwSelectedIndexChangedEventHandler;
            lvwSrc.KeyDown += lvwSrcKeyDownEventHandler;
            lvwSrc.GotFocus += (s, e) => _activeListView = lvwSrc;
            lvwSrc.LostFocus += (s, e) => { if (_activeListView == lvwSrc) _activeListView = null; };
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

            tvwDst.HideSelection = false;
            tvwDst.AfterSelect += tvwAfterSelectEventHandler;
            //tvwDst.NodeMouseDoubleClick += tvwNodeMouseDoubleClickEventHnadler;

            //lvwDst.AllowDrop = false;
            lvwDst.AllowDrop = true;
            lvwDst.FullRowSelect = true;
            lvwDst.MultiSelect = true;
            lvwDst.View = View.Details;
            lvwDst.DoubleClick += lvwDoubleClickEventHandler;
            lvwDst.DragEnter += lvwDstDragEnterEventHandler;
            //lvwDst.DragOver += lvwDestDragOverEventHandler;
            lvwDst.DragDrop += lvwDstDragDropEventHandler;
            lvwDst.KeyDown += lvwDstKeyDownEventHandler;
            lvwDst.ItemSelectionChanged += (s, e) => 
            {
                var lvw = (ListView)s;
                var item = e.Item;
                lblDstPath.Text = item.ToolTipText;
            };
            lvwDst.SelectedIndexChanged += lvwSelectedIndexChangedEventHandler;
            lvwDst.GotFocus += (s, e) => _activeListView = lvwDst;
            lvwDst.LostFocus += (s, e) => { if (_activeListView == lvwDst) _activeListView = null; };
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
            mnuHelpAbout.Click += (s, e) =>
                MessageBox.Show("Just drag the target file or folder and drop it to where you like.",
                                $"{Application.ProductName} v{Application.ProductVersion}", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            msMain.Visible = false;
            #endregion
        }

        #region EVENTHANDLER

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
                ExplorerHelper.AddForlder(txt.Text, tvw, lvw);
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
            var lvw = (ListView)sender;

            if (e.KeyCode == Keys.Delete
                && lvwDst.SelectedItems.Count > 0)
            {
                var msg = lvwDst.SelectedItems.Count > 1
                   ? $"Are you sure you want to delete the {lvwDst.SelectedItems.Count} selected items, including '{lvwDst.SelectedItems[0].Text}' and others?"
                   : $"Are you sure you want to delete the selected item '{lvwDst.SelectedItems[0].Text}'?";

                if (MessageBox.Show(msg, this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    TreeNode pNode = null;
                    foreach (ListViewItem selectItem in lvwDst.SelectedItems)
                    {
                        var path = selectItem.Tag.ToString();
                        if (ExplorerHelper.IsSymbolicLink(path, out _)
                            && ExplorerHelper.MoveToRecycle(path))
                        {
                            lvw.Items.Remove(selectItem);
                            if (selectItem.Name.StartsWith("Folder"))
                                if (null == pNode)
                                {
                                    var fNode = tvwDst.Nodes.Find(selectItem.Name, true).First();
                                    if (fNode != null)
                                        pNode = fNode.Parent;
                                }
                        }
                        else
                        {
                            SystemSounds.Beep.Play();
                            Debug.Print("仅支持软连接删除");
                        }
                    }
                    if (null != pNode)
                        ExplorerHelper.AddSubForlders(pNode, true);
                }
            }
            else if (e.Control)
            {
                if (e.KeyCode == Keys.C)
                {
                    // 复制
                    copySelectedItems(lvw);
                }
                else if (e.KeyCode == Keys.V)
                {
                    // 粘贴
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
                                new frmToast(this, "创建成功", 3000);
                            }
                        }
                    }
                    else
                        Debug.Print("当前位置无法粘贴创建软连接");
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
                    if (ExplorerHelper.TryMakeLinkByCore(link, target, mnuShowLog.Checked))
                        needReload = true;
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
                        if (ExplorerHelper.TryMakeLinkByCore(link, target, mnuShowLog.Checked))
                            needReload = true;
                    }
                }
            }

            if (needReload)
            {
                //ExplorerHelper.AddSubForlders(tvw.SelectedNode, true);
                //ExplorerHelper.ReloadListView(tvw.SelectedNode, lvw);
                //lblDstCount.Text = $"Total {lvw.Items.Count - 1} items";
                //lblDstSelCount.Text = "";
                loadTreeNodes(tvw.SelectedNode, true);
                new frmToast(this, "创建成功", 3000);
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
                    var process = new Process();
                    process.StartInfo.FileName = selectItem.Tag.ToString();
                    process.StartInfo.UseShellExecute = true;
                    process.Start();
                }
                else
                {
                    var node = tvw.Nodes.Find(selectItem.Name, true).First();
                    if (node != null)
                    {
                        tvw.SelectedNode = node;
                        ExplorerHelper.AddSubForlders(node);
                        ExplorerHelper.ReloadListView(node, lvw);
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
                // 这里不能用 e.Node， 因为测试发现 e.Node 为双击结束后，鼠标停留位置的节点
                var currNode = e.Node.TreeView.SelectedNode;
                Debug.Print(currNode.FullPath);
                loadTreeNodes(currNode, !currNode.IsExpanded);
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
                ExplorerHelper.AddSubForlders(currNode, clearall);
                ExplorerHelper.ReloadListView(currNode, lvw);
                lvwItemCountUpdate(lvw);
                txt.Text = currNode.Tag.ToString();
            }
            else
            {
                //ExplorerHelper.AddDrives(tvw);
                ExplorerHelper.ReloadListView(currNode, lvw);
                lvwItemCountUpdate(lvw);
                txt.Text = "";
            }

            //lvwDst.AllowDrop = (null != tvwDst.SelectedNode
            //                    && null != tvwDst.SelectedNode.Parent);
            //Debug.Print($"lvwDst.AllowDrop={lvwDst.AllowDrop}");
        }

        private static void copySelectedItems(ListView lvw)
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

        private static bool pasteFromClipboard(ListView lvw, string path, bool showLog)
        {
            // 获取剪切板数据对象
            IDataObject dataObject = Clipboard.GetDataObject();
            bool ret = false;
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
                        ExplorerHelper.TryMakeLinkByCore(link, fileOrFolder, showLog);
                        ret = true;
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
                        ExplorerHelper.TryMakeLinkByCore(link, fileOrFolder, showLog);
                        ret = true;
                    }
                    else
                        Debug.Print($"路径或文件不存在: {fileOrFolder}");
                }
            }
            return ret;
        }

        #region FORM OVERRIDE

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ExplorerHelper.AddDrives(tvwSrc);
            ExplorerHelper.ReloadListView(tvwSrc.Nodes[0], lvwSrc);
            txtSrcPath.Width = spcSrc.Width - btnSrc.Width - lblSrcPath.Width - SPRING_BORDER;
            lblSrcCount.Text = $"Total {lvwSrc.Items.Count} items";
            lblSrcSelCount.Text = "";

            ExplorerHelper.AddDrives(tvwDst);
            ExplorerHelper.ReloadListView(tvwDst.Nodes[0], lvwDst);
            txtDstPath.Width = spcDst.Width - btnDst.Width - lblDstPath.Width - SPRING_BORDER;
            lblDstCount.Text = $"Total {lvwDst.Items.Count} items";
            lblDstSelCount.Text = "";
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Alt)
                msMain.Visible = !msMain.Visible;
            else if (e.Control
                && e.KeyCode == Keys.A
                && _activeListView != null)
                foreach (ListViewItem item in _activeListView.Items)
                    item.Selected = true;
            else if (e.KeyCode == Keys.F5)
            {
                // 强制清空重新加载
                loadTreeNodes(tvwDst.SelectedNode, true);
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
        
        #endregion
    }
}
