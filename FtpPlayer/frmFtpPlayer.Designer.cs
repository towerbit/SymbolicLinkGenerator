namespace FtpPlayer
{
    partial class frmFtpPlayer
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFtpPlayer));
            this.tscSrc = new System.Windows.Forms.ToolStripContainer();
            this.ssSrc = new System.Windows.Forms.StatusStrip();
            this.lblSrcCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblSrcSelCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.spcSrc = new System.Windows.Forms.SplitContainer();
            this.tvwSrc = new System.Windows.Forms.TreeView();
            this.lvwSrc = new System.Windows.Forms.ListView();
            this.colSrcName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSrcSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSrcMemo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tsSrc = new System.Windows.Forms.ToolStrip();
            this.btnSrc = new System.Windows.Forms.ToolStripButton();
            this.lblSrcPath = new System.Windows.Forms.ToolStripLabel();
            this.txtSrcPath = new System.Windows.Forms.ToolStripTextBox();
            this.tscSrc.BottomToolStripPanel.SuspendLayout();
            this.tscSrc.ContentPanel.SuspendLayout();
            this.tscSrc.TopToolStripPanel.SuspendLayout();
            this.tscSrc.SuspendLayout();
            this.ssSrc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcSrc)).BeginInit();
            this.spcSrc.Panel1.SuspendLayout();
            this.spcSrc.Panel2.SuspendLayout();
            this.spcSrc.SuspendLayout();
            this.tsSrc.SuspendLayout();
            this.SuspendLayout();
            // 
            // tscSrc
            // 
            // 
            // tscSrc.BottomToolStripPanel
            // 
            this.tscSrc.BottomToolStripPanel.Controls.Add(this.ssSrc);
            // 
            // tscSrc.ContentPanel
            // 
            this.tscSrc.ContentPanel.Controls.Add(this.spcSrc);
            this.tscSrc.ContentPanel.Margin = new System.Windows.Forms.Padding(4);
            this.tscSrc.ContentPanel.Size = new System.Drawing.Size(1083, 499);
            this.tscSrc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tscSrc.Location = new System.Drawing.Point(0, 0);
            this.tscSrc.Margin = new System.Windows.Forms.Padding(4);
            this.tscSrc.Name = "tscSrc";
            this.tscSrc.Size = new System.Drawing.Size(1083, 563);
            this.tscSrc.TabIndex = 1;
            this.tscSrc.Text = "tscSrc";
            // 
            // tscSrc.TopToolStripPanel
            // 
            this.tscSrc.TopToolStripPanel.Controls.Add(this.tsSrc);
            // 
            // ssSrc
            // 
            this.ssSrc.Dock = System.Windows.Forms.DockStyle.None;
            this.ssSrc.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ssSrc.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblSrcCount,
            this.lblSrcSelCount});
            this.ssSrc.Location = new System.Drawing.Point(0, 0);
            this.ssSrc.Name = "ssSrc";
            this.ssSrc.Size = new System.Drawing.Size(1083, 31);
            this.ssSrc.SizingGrip = false;
            this.ssSrc.TabIndex = 0;
            // 
            // lblSrcCount
            // 
            this.lblSrcCount.Name = "lblSrcCount";
            this.lblSrcCount.Size = new System.Drawing.Size(73, 24);
            this.lblSrcCount.Text = "0 Items";
            // 
            // lblSrcSelCount
            // 
            this.lblSrcSelCount.Name = "lblSrcSelCount";
            this.lblSrcSelCount.Size = new System.Drawing.Size(151, 24);
            this.lblSrcSelCount.Text = "Selected 0 Items";
            // 
            // spcSrc
            // 
            this.spcSrc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcSrc.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.spcSrc.Location = new System.Drawing.Point(0, 0);
            this.spcSrc.Margin = new System.Windows.Forms.Padding(4);
            this.spcSrc.Name = "spcSrc";
            // 
            // spcSrc.Panel1
            // 
            this.spcSrc.Panel1.Controls.Add(this.tvwSrc);
            // 
            // spcSrc.Panel2
            // 
            this.spcSrc.Panel2.Controls.Add(this.lvwSrc);
            this.spcSrc.Size = new System.Drawing.Size(1083, 499);
            this.spcSrc.SplitterDistance = 298;
            this.spcSrc.SplitterWidth = 6;
            this.spcSrc.TabIndex = 0;
            // 
            // tvwSrc
            // 
            this.tvwSrc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwSrc.Location = new System.Drawing.Point(0, 0);
            this.tvwSrc.Margin = new System.Windows.Forms.Padding(4);
            this.tvwSrc.Name = "tvwSrc";
            this.tvwSrc.Size = new System.Drawing.Size(298, 499);
            this.tvwSrc.TabIndex = 0;
            // 
            // lvwSrc
            // 
            this.lvwSrc.BackColor = System.Drawing.SystemColors.Window;
            this.lvwSrc.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSrcName,
            this.colSrcSize,
            this.colSrcMemo});
            this.lvwSrc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwSrc.HideSelection = false;
            this.lvwSrc.Location = new System.Drawing.Point(0, 0);
            this.lvwSrc.Margin = new System.Windows.Forms.Padding(4);
            this.lvwSrc.Name = "lvwSrc";
            this.lvwSrc.ShowItemToolTips = true;
            this.lvwSrc.Size = new System.Drawing.Size(779, 499);
            this.lvwSrc.TabIndex = 0;
            this.lvwSrc.UseCompatibleStateImageBehavior = false;
            this.lvwSrc.View = System.Windows.Forms.View.Details;
            // 
            // colSrcName
            // 
            this.colSrcName.Text = "Name";
            this.colSrcName.Width = 160;
            // 
            // colSrcSize
            // 
            this.colSrcSize.Text = "Size";
            this.colSrcSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // colSrcMemo
            // 
            this.colSrcMemo.Text = "Memo";
            // 
            // tsSrc
            // 
            this.tsSrc.Dock = System.Windows.Forms.DockStyle.None;
            this.tsSrc.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsSrc.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.tsSrc.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSrc,
            this.lblSrcPath,
            this.txtSrcPath});
            this.tsSrc.Location = new System.Drawing.Point(0, 0);
            this.tsSrc.Name = "tsSrc";
            this.tsSrc.Size = new System.Drawing.Size(1083, 33);
            this.tsSrc.Stretch = true;
            this.tsSrc.TabIndex = 0;
            // 
            // btnSrc
            // 
            this.btnSrc.Checked = true;
            this.btnSrc.CheckOnClick = true;
            this.btnSrc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnSrc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSrc.Image = ((System.Drawing.Image)(resources.GetObject("btnSrc.Image")));
            this.btnSrc.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSrc.Name = "btnSrc";
            this.btnSrc.Size = new System.Drawing.Size(34, 28);
            this.btnSrc.Text = "Treeview";
            // 
            // lblSrcPath
            // 
            this.lblSrcPath.Name = "lblSrcPath";
            this.lblSrcPath.Size = new System.Drawing.Size(66, 28);
            this.lblSrcPath.Text = "Target";
            // 
            // txtSrcPath
            // 
            this.txtSrcPath.AutoSize = false;
            this.txtSrcPath.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.txtSrcPath.Name = "txtSrcPath";
            this.txtSrcPath.ReadOnly = true;
            this.txtSrcPath.Size = new System.Drawing.Size(100, 25);
            // 
            // frmFtpPlayer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1083, 563);
            this.Controls.Add(this.tscSrc);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmFtpPlayer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FtpPlayer";
            this.tscSrc.BottomToolStripPanel.ResumeLayout(false);
            this.tscSrc.BottomToolStripPanel.PerformLayout();
            this.tscSrc.ContentPanel.ResumeLayout(false);
            this.tscSrc.TopToolStripPanel.ResumeLayout(false);
            this.tscSrc.TopToolStripPanel.PerformLayout();
            this.tscSrc.ResumeLayout(false);
            this.tscSrc.PerformLayout();
            this.ssSrc.ResumeLayout(false);
            this.ssSrc.PerformLayout();
            this.spcSrc.Panel1.ResumeLayout(false);
            this.spcSrc.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcSrc)).EndInit();
            this.spcSrc.ResumeLayout(false);
            this.tsSrc.ResumeLayout(false);
            this.tsSrc.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer tscSrc;
        private System.Windows.Forms.StatusStrip ssSrc;
        private System.Windows.Forms.ToolStripStatusLabel lblSrcCount;
        private System.Windows.Forms.ToolStripStatusLabel lblSrcSelCount;
        private System.Windows.Forms.SplitContainer spcSrc;
        private System.Windows.Forms.TreeView tvwSrc;
        private System.Windows.Forms.ListView lvwSrc;
        private System.Windows.Forms.ColumnHeader colSrcName;
        private System.Windows.Forms.ColumnHeader colSrcSize;
        private System.Windows.Forms.ColumnHeader colSrcMemo;
        private System.Windows.Forms.ToolStrip tsSrc;
        private System.Windows.Forms.ToolStripButton btnSrc;
        private System.Windows.Forms.ToolStripLabel lblSrcPath;
        private System.Windows.Forms.ToolStripTextBox txtSrcPath;
    }
}

