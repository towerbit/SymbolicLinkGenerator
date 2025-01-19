using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace FtpPlayer
{
    public partial class frmFtpPlayer : Form
    {
        public frmFtpPlayer()
        {
            InitializeComponent();
            tscSrc.Dock = DockStyle.Fill;

            spcSrc.SplitterDistance = spcSrc.Width / 4;
            spcSrc.FixedPanel = FixedPanel.Panel1;
            spcSrc.Panel1MinSize = 100;
            spcSrc.Panel2MinSize = 100;
            spcSrc.SplitterMoved += (s, e) => _client?.ResizeListViewColumns(lvwSrc);

            btnSrc.Image = Properties.Resources.treeview.ToBitmap();
            btnSrc.CheckedChanged += (s, e) =>
            {
                spcSrc.Panel1Collapsed = !btnSrc.Checked;
                _client?.ResizeListViewColumns(lvwSrc);
            };

            //txtSrcPath.KeyPress += txtDirKeyPressEventHandler;
            //txtSrcPath.GotFocus += txtDirGotFocusEventHandler;

            tvwSrc.HideSelection = false;
            tvwSrc.BackColor = SystemColors.ControlLight;
            tvwSrc.AfterSelect += tvwAfterSelectEventHandler;

            lvwSrc.FullRowSelect = true;
            lvwSrc.MultiSelect = false;
            lvwSrc.View = View.Details;
            lvwSrc.BackColor = SystemColors.ControlLight;
            lvwSrc.DoubleClick += lvwDoubleClickEventHandler;
            //lvwSrc.ItemDrag += lvwSrcItemDragEventHandler;
            //lvwSrc.SelectedIndexChanged += lvwSelectedIndexChangedEventHandler;
            lvwSrc.KeyUp += lvwKeyUpEventHandler;
            //lvwSrc.GotFocus += (s, e) => _activeListView = lvwSrc;
            //lvwSrc.LostFocus += (s, e) => { if (_activeListView == lvwSrc) _activeListView = null; };
        }

        private void lvwKeyUpEventHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                lvwDoubleClickEventHandler(sender, e);
        }

        private void lvwDoubleClickEventHandler(object sender, EventArgs e)
        {
            var selectItem = lvwSrc.SelectedItems[0];
            if (null != selectItem)
                if (selectItem.Name.StartsWith("File"))
                {
                    
                    var uidStr = "";
                    if (Properties.Settings.Default.username != "anonymous")
                    {
                        uidStr = Properties.Settings.Default.username;
                        if (Properties.Settings.Default.password != "")
                            uidStr += $":{Properties.Settings.Default.password}";
                    }
                    if(!string.IsNullOrEmpty(uidStr))
                        uidStr += "@";

                    var info = selectItem.Tag as FtpFileInfo;
                    var selectedNode = tvwSrc.SelectedNode;
                    var url = string.Format("ftp://{0}{1}{2}/{3}",
                                            uidStr,
                                            Properties.Settings.Default.server,
                                            selectedNode.FullPath, 
                                            info.Name)
                                    .Replace("/\\", "/")
                                    .Replace("\\", "/");
                    
                    var player = Properties.Settings.Default.vlc;
                    if (!string.IsNullOrEmpty(player)
                        && File.Exists(player))
                    {
                        var process = new Process();
                        process.StartInfo.FileName = player;
                        process.StartInfo.Arguments = $"\"{url}\""; // 兼容路径中有空格的文件名
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                        process.Start();
                    }
                    else
                        MessageBox.Show($"{player} FILE NOT FOUND", this.Text, 
                            MessageBoxButtons.OK , MessageBoxIcon.Error); 
                }
                else
                {
                    // 打开子目录
                    var node = tvwSrc.Nodes.Find(selectItem.Name, true)[0];
                    if (node != null)
                    {
                        tvwSrc.SelectedNode = node;
                        _client.AddSubFolders(node);
                        _client.ReloadListView(node, lvwSrc);
                        lvwItemCountUpdate(lvwSrc);
                        txtSrcPath.Text = node.FullPath.Replace("/\\","/").Replace("\\","/");
                    }
                }
            else
                SystemSounds.Beep.Play();
        }

        private void tvwAfterSelectEventHandler(object sender, TreeViewEventArgs e)
        {
            loadTreeNodes(e.Node, !e.Node.IsExpanded);
        }

        private void loadTreeNodes(TreeNode currNode, bool clearall)
        {
            if (currNode.Parent != null)
            {
                _client.AddSubFolders(currNode, clearall);
                txtSrcPath.Text = currNode.FullPath.Replace("/\\","/").Replace("\\","/");
            }
            else
            {
                txtSrcPath.Text = "/";
            }
            _client.ReloadListView(currNode, lvwSrc);
            lvwItemCountUpdate(lvwSrc);
        }

        private void lvwItemCountUpdate(ListView lvw)
        {
            if (lvw.Items[0].Text == "..")
                lblSrcCount.Text = $"Total {lvw.Items.Count - 1} items";
            else
                lblSrcCount.Text = $"Total {lvw.Items.Count} items";
            lblSrcSelCount.Text = "";
        }

        private FtpClient _client;
        private const int SPRING_BORDER = 48;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            txtSrcPath.Width = spcSrc.Width - btnSrc.Width - lblSrcPath.Width - SPRING_BORDER;
            lblSrcCount.Text = $"Total {lvwSrc.Items.Count} items";
            lblSrcSelCount.Text = "";

            _client = new FtpClient();
            _client.ftpServer = Properties.Settings.Default.server;
            _client.username = Properties.Settings.Default.username;
            _client.password = Properties.Settings.Default.password;
            
            _client.AddRootFolders(tvwSrc);
            _client.ReloadListView(tvwSrc.Nodes[0], lvwSrc);
        }
    }
}
