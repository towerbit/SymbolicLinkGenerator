using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
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
            tscSrc.Dock= DockStyle.Fill;
            tscDst.Dock= DockStyle.Fill;

            spcMain.SplitterDistance = spcMain.Width / 2;
            spcMain.Panel1MinSize = spcMain.Width / 3;
            spcMain.Panel2MinSize = spcMain.Width / 3;
            spcMain.SplitterMoved += (s, e) =>
            {
                Debug.Print("【spcMain.SplitterMoved】");
                spcMain.SuspendLayout();
                txtSrcPath.Width = spcMain.Panel1.Width - btnSrc.Width - lblSrcPath.Width - SPRING_BORDER;
                ExplorerHelper.ResizeListViewColumns(lvwSrc);
                txtDstPath.Width = spcMain.Panel2.Width - btnDst.Width - lblDstPath.Width - SPRING_BORDER;
                ExplorerHelper.ResizeListViewColumns(lvwDst);
                spcMain.ResumeLayout();
            };
            spcMain.Resize += (s, e) =>
            {
                Debug.Print("【spcMain.Resize】");
                spcMain.SuspendLayout();
                ExplorerHelper.ResizeListViewColumns(lvwSrc);
                ExplorerHelper.ResizeListViewColumns(lvwDst);
                spcMain.ResumeLayout();
            };
            spcSrc.SplitterDistance = spcSrc.Width / 3;
            spcSrc.FixedPanel = FixedPanel.Panel1;
            spcSrc.Panel1MinSize = 100;
            spcSrc.Panel2MinSize = 100;
            spcSrc.SplitterMoved+=(s, e) => ExplorerHelper.ResizeListViewColumns(lvwSrc);
            
            btnSrc.Image = Properties.Resources.treeview.ToBitmap();
            btnSrc.CheckedChanged += (s, e) =>
            {
                spcSrc.Panel1Collapsed = !btnSrc.Checked;
                ExplorerHelper.ResizeListViewColumns(lvwSrc);
            };

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

            lvwSrc.FullRowSelect = true;
            lvwSrc.MultiSelect = true;
            lvwSrc.View = View.Details;
            lvwSrc.BackColor = SystemColors.ControlLight;
            lvwSrc.DoubleClick += lvwDoubleClickEventHandler;
            lvwSrc.ItemDrag += lvwSrcItemDragEventHandler;
            lvwSrc.SelectedIndexChanged += lvwSelectedIndexChangedEventHandler;
            lvwSrc.GotFocus += (s, e) => _activeListView = lvwSrc;
            lvwSrc.LostFocus += (s, e) => { if (_activeListView == lvwSrc) _activeListView = null; };

            lvwDst.AllowDrop = false;
            lvwDst.FullRowSelect = true;
            lvwDst.MultiSelect = true;
            lvwDst.View = View.Details;
            lvwDst.DoubleClick += lvwDoubleClickEventHandler;
            lvwDst.DragEnter += lvwDestDragEnterEventHandler;
            lvwDst.DragOver += lvwDestDragOverEventHandler;
            lvwDst.DragDrop += lvwDestDragDropEventHandler;
            lvwDst.KeyDown += lvwDestKeyDownEventHandler;
            lvwDst.ItemSelectionChanged += (s, e) => 
            {
                var lvw = (ListView)s;
                var item = e.Item;
                lblDstTargetPath.Text = item.ToolTipText;
            };
            lvwDst.SelectedIndexChanged += lvwSelectedIndexChangedEventHandler;
            lvwDst.GotFocus += (s, e) => _activeListView = lvwDst;
            lvwDst.LostFocus +=(s, e) => { if (_activeListView == lvwDst) _activeListView = null; };

            tvwSrc.HideSelection = false;
            tvwSrc.BackColor = SystemColors.ControlLight;
            tvwSrc.AfterSelect += tvwAfterSelectEventHandler;
            
            tvwDst.HideSelection = false;
            tvwDst.AfterSelect += tvwAfterSelectEventHandler;
            //tvwDst.NodeMouseDoubleClick += tvwNodeMouseDoubleClickEventHnadler;

            txtSrcPath.KeyPress += txtDirKeyPressEventHandler;
            txtSrcPath.GotFocus += txtDirGotFocusEventHandler;

            txtDstPath.KeyPress += txtDirKeyPressEventHandler;
            txtDstPath.GotFocus += txtDirGotFocusEventHandler;

            mnuFileExit.Click += (s, e) => this.Close();
            mnuHelpAbout.Click += (s, e) => 
                MessageBox.Show(this, Application.ProductName, Application.ProductVersion, 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            msMain.Visible = false;
        }

        private void lvwSelectedIndexChangedEventHandler(object sender, EventArgs e)
        {
            var lvw = (ListView)sender;
            var lbl = lvw.Name.Contains("Src")? lblSrcSelCount: lblDstSelCount;
            lbl.Text = $"Selected {lvw.SelectedItems.Count} items";
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
                ExplorerHelper.AddForlder(txt.Text, tvw, lvw);
                lvwItemCountUpdate(lvw);
                if (tvw == tvwDst)
                    lvw.AllowDrop = tvw.SelectedNode.Parent != null;
                    
                e.Handled = true;
            }
        }

        private void lvwItemCountUpdate(ListView lvw)
        {
            var lbl = lvw.Name.Contains("Src") ? lblSrcCount : lblDstCount;
            if (lvw.Items[0].Text == "..")
                lbl.Text = $"Total {lvw.Items.Count - 1} items";
            else
                lbl.Text = $"Total {lvw.Items.Count} items";

            var lblSel = lvw.Name.Contains("Src") ? lblSrcSelCount : lblDstSelCount;
            lblSel.Text = "";
            lblDstTargetPath.Text = "";
        }

        private void lvwDestKeyDownEventHandler(object sender, KeyEventArgs e)
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
                            Debug.Print("SymbolicLink delete only.");
                        }
                    }
                    if (null != pNode)
                        ExplorerHelper.AddSubForlders(pNode, true);
                }
            }
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
            loadTreeNodes(e.Node);
        }

        private void loadTreeNodes(TreeNode currNode)
        {
            var tvw = currNode.TreeView;
            var lvw = tvw.Name.Contains("Src") ? lvwSrc : lvwDst;
            var txt = tvw.Name.Contains("Src") ? txtSrcPath : txtDstPath;
            //var lblCount = tvw.Name.Contains("Src") ? lblSrcCount : lblDstCount;
            //var lblSelCount = tvw.Name.Contains("Src") ? lblSrcSelCount : lblDstSelCount;

            if (currNode.Parent != null)
            {               
                ExplorerHelper.AddSubForlders(currNode, !currNode.IsExpanded);
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

            lvwDst.AllowDrop = (null!= tvwDst.SelectedNode
                                && null!= tvwDst.SelectedNode.Parent);
        }

        private void tvwNodeMouseDoubleClickEventHnadler(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 这里不能用 e.Node， 因为测试发现 e.Node 为双击结束后，鼠标停留位置的节点
                var currNode = e.Node.TreeView.SelectedNode;
                Debug.Print(currNode.FullPath);
                loadTreeNodes(currNode);
            }
        }

        private void lvwDestDragDropEventHandler(object sender, DragEventArgs e)
        {
            var lvw = (ListView)sender;
            var tvw = tvwDst;

            // 支持多选拖放
            if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)))
            {
                var draggedItems = (ListView.SelectedListViewItemCollection)e.Data.GetData(typeof(ListView.SelectedListViewItemCollection));
                bool needReload = false;
                foreach (ListViewItem draggedItem in draggedItems)
                {
                    //var link = Path.Combine(txtDstPath.Text, Path.GetFileName(draggedItem.Tag.ToString()));
                    var currPath = tvw.SelectedNode.Tag.ToString();
                    var link = Path.Combine(currPath, Path.GetFileName(draggedItem.Tag.ToString()));

                    var target = draggedItem.Tag.ToString();
                    if (ExplorerHelper.TryMakeLink(link, target))
                        needReload = true;
                }
                if (needReload)
                {
                    ExplorerHelper.AddSubForlders(tvw.SelectedNode, true);
                    ExplorerHelper.ReloadListView(tvw.SelectedNode, lvw);
                    lblDstCount.Text = $"Total {lvw.Items.Count - 1} items";
                    lblDstSelCount.Text = "";
                }
            }

        }

        private void lvwDestDragOverEventHandler(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy; // 在拖拽过程中持续显示移动效果
        }

        private void lvwDestDragEnterEventHandler(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy; // 设置拖拽效果为移动
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
        }
    }
}
