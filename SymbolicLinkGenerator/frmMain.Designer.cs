namespace SymbolicLinkGenerator
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.spcMain = new System.Windows.Forms.SplitContainer();
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
            this.tscDst = new System.Windows.Forms.ToolStripContainer();
            this.ssDst = new System.Windows.Forms.StatusStrip();
            this.lblDstCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblDstSelCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.spcDst = new System.Windows.Forms.SplitContainer();
            this.tvwDst = new System.Windows.Forms.TreeView();
            this.lvwDst = new System.Windows.Forms.ListView();
            this.colDstName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDstSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDtsMemo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tsDst = new System.Windows.Forms.ToolStrip();
            this.btnDst = new System.Windows.Forms.ToolStripButton();
            this.lblDstPath = new System.Windows.Forms.ToolStripLabel();
            this.txtDstPath = new System.Windows.Forms.ToolStripTextBox();
            this.msMain = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShowTarget = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileBar1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShowLog = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.spcMain)).BeginInit();
            this.spcMain.Panel1.SuspendLayout();
            this.spcMain.Panel2.SuspendLayout();
            this.spcMain.SuspendLayout();
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
            this.tscDst.BottomToolStripPanel.SuspendLayout();
            this.tscDst.ContentPanel.SuspendLayout();
            this.tscDst.TopToolStripPanel.SuspendLayout();
            this.tscDst.SuspendLayout();
            this.ssDst.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcDst)).BeginInit();
            this.spcDst.Panel1.SuspendLayout();
            this.spcDst.Panel2.SuspendLayout();
            this.spcDst.SuspendLayout();
            this.tsDst.SuspendLayout();
            this.msMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // spcMain
            // 
            this.spcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcMain.Location = new System.Drawing.Point(0, 25);
            this.spcMain.Name = "spcMain";
            // 
            // spcMain.Panel1
            // 
            this.spcMain.Panel1.Controls.Add(this.tscSrc);
            // 
            // spcMain.Panel2
            // 
            this.spcMain.Panel2.Controls.Add(this.tscDst);
            this.spcMain.Size = new System.Drawing.Size(1264, 736);
            this.spcMain.SplitterDistance = 592;
            this.spcMain.TabIndex = 0;
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
            this.tscSrc.ContentPanel.Size = new System.Drawing.Size(521, 560);
            this.tscSrc.Location = new System.Drawing.Point(29, 55);
            this.tscSrc.Name = "tscSrc";
            this.tscSrc.Size = new System.Drawing.Size(521, 607);
            this.tscSrc.TabIndex = 0;
            this.tscSrc.Text = "tscSrc";
            // 
            // tscSrc.TopToolStripPanel
            // 
            this.tscSrc.TopToolStripPanel.Controls.Add(this.tsSrc);
            // 
            // ssSrc
            // 
            this.ssSrc.Dock = System.Windows.Forms.DockStyle.None;
            this.ssSrc.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblSrcCount,
            this.lblSrcSelCount});
            this.ssSrc.Location = new System.Drawing.Point(0, 0);
            this.ssSrc.Name = "ssSrc";
            this.ssSrc.Size = new System.Drawing.Size(521, 22);
            this.ssSrc.SizingGrip = false;
            this.ssSrc.TabIndex = 0;
            // 
            // lblSrcCount
            // 
            this.lblSrcCount.Name = "lblSrcCount";
            this.lblSrcCount.Size = new System.Drawing.Size(51, 17);
            this.lblSrcCount.Text = "0 Items";
            // 
            // lblSrcSelCount
            // 
            this.lblSrcSelCount.Name = "lblSrcSelCount";
            this.lblSrcSelCount.Size = new System.Drawing.Size(104, 17);
            this.lblSrcSelCount.Text = "Selected 0 Items";
            // 
            // spcSrc
            // 
            this.spcSrc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcSrc.Location = new System.Drawing.Point(0, 0);
            this.spcSrc.Name = "spcSrc";
            // 
            // spcSrc.Panel1
            // 
            this.spcSrc.Panel1.Controls.Add(this.tvwSrc);
            // 
            // spcSrc.Panel2
            // 
            this.spcSrc.Panel2.Controls.Add(this.lvwSrc);
            this.spcSrc.Size = new System.Drawing.Size(521, 560);
            this.spcSrc.SplitterDistance = 242;
            this.spcSrc.TabIndex = 0;
            // 
            // tvwSrc
            // 
            this.tvwSrc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwSrc.Location = new System.Drawing.Point(0, 0);
            this.tvwSrc.Name = "tvwSrc";
            this.tvwSrc.Size = new System.Drawing.Size(242, 560);
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
            this.lvwSrc.Name = "lvwSrc";
            this.lvwSrc.ShowItemToolTips = true;
            this.lvwSrc.Size = new System.Drawing.Size(275, 560);
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
            this.tsSrc.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSrc,
            this.lblSrcPath,
            this.txtSrcPath});
            this.tsSrc.Location = new System.Drawing.Point(0, 0);
            this.tsSrc.Name = "tsSrc";
            this.tsSrc.Size = new System.Drawing.Size(521, 25);
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
            this.btnSrc.Size = new System.Drawing.Size(23, 22);
            this.btnSrc.Text = "Treeview";
            // 
            // lblSrcPath
            // 
            this.lblSrcPath.Name = "lblSrcPath";
            this.lblSrcPath.Size = new System.Drawing.Size(46, 22);
            this.lblSrcPath.Text = "Target";
            // 
            // txtSrcPath
            // 
            this.txtSrcPath.AutoSize = false;
            this.txtSrcPath.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.txtSrcPath.Name = "txtSrcPath";
            this.txtSrcPath.Size = new System.Drawing.Size(100, 25);
            // 
            // tscDst
            // 
            // 
            // tscDst.BottomToolStripPanel
            // 
            this.tscDst.BottomToolStripPanel.Controls.Add(this.ssDst);
            // 
            // tscDst.ContentPanel
            // 
            this.tscDst.ContentPanel.Controls.Add(this.spcDst);
            this.tscDst.ContentPanel.Size = new System.Drawing.Size(521, 560);
            this.tscDst.Location = new System.Drawing.Point(70, 55);
            this.tscDst.Name = "tscDst";
            this.tscDst.Size = new System.Drawing.Size(521, 607);
            this.tscDst.TabIndex = 1;
            this.tscDst.Text = "tscDst";
            // 
            // tscDst.TopToolStripPanel
            // 
            this.tscDst.TopToolStripPanel.Controls.Add(this.tsDst);
            // 
            // ssDst
            // 
            this.ssDst.Dock = System.Windows.Forms.DockStyle.None;
            this.ssDst.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblDstCount,
            this.lblDstSelCount});
            this.ssDst.Location = new System.Drawing.Point(0, 0);
            this.ssDst.Name = "ssDst";
            this.ssDst.Size = new System.Drawing.Size(521, 22);
            this.ssDst.SizingGrip = false;
            this.ssDst.TabIndex = 0;
            // 
            // lblDstCount
            // 
            this.lblDstCount.Name = "lblDstCount";
            this.lblDstCount.Size = new System.Drawing.Size(51, 17);
            this.lblDstCount.Text = "0 Items";
            // 
            // lblDstSelCount
            // 
            this.lblDstSelCount.Name = "lblDstSelCount";
            this.lblDstSelCount.Size = new System.Drawing.Size(104, 17);
            this.lblDstSelCount.Text = "Selected 0 Items";
            // 
            // spcDst
            // 
            this.spcDst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcDst.Location = new System.Drawing.Point(0, 0);
            this.spcDst.Name = "spcDst";
            // 
            // spcDst.Panel1
            // 
            this.spcDst.Panel1.Controls.Add(this.tvwDst);
            // 
            // spcDst.Panel2
            // 
            this.spcDst.Panel2.Controls.Add(this.lvwDst);
            this.spcDst.Size = new System.Drawing.Size(521, 560);
            this.spcDst.SplitterDistance = 243;
            this.spcDst.TabIndex = 0;
            // 
            // tvwDst
            // 
            this.tvwDst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwDst.Location = new System.Drawing.Point(0, 0);
            this.tvwDst.Name = "tvwDst";
            this.tvwDst.Size = new System.Drawing.Size(243, 560);
            this.tvwDst.TabIndex = 0;
            // 
            // lvwDst
            // 
            this.lvwDst.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDstName,
            this.colDstSize,
            this.colDtsMemo});
            this.lvwDst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwDst.HideSelection = false;
            this.lvwDst.Location = new System.Drawing.Point(0, 0);
            this.lvwDst.Name = "lvwDst";
            this.lvwDst.ShowItemToolTips = true;
            this.lvwDst.Size = new System.Drawing.Size(274, 560);
            this.lvwDst.TabIndex = 0;
            this.lvwDst.UseCompatibleStateImageBehavior = false;
            this.lvwDst.View = System.Windows.Forms.View.Details;
            // 
            // colDstName
            // 
            this.colDstName.Text = "Name";
            this.colDstName.Width = 160;
            // 
            // colDstSize
            // 
            this.colDstSize.Text = "Size";
            this.colDstSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // colDtsMemo
            // 
            this.colDtsMemo.Text = "Memo";
            // 
            // tsDst
            // 
            this.tsDst.Dock = System.Windows.Forms.DockStyle.None;
            this.tsDst.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsDst.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDst,
            this.lblDstPath,
            this.txtDstPath});
            this.tsDst.Location = new System.Drawing.Point(0, 0);
            this.tsDst.Name = "tsDst";
            this.tsDst.Size = new System.Drawing.Size(521, 25);
            this.tsDst.Stretch = true;
            this.tsDst.TabIndex = 0;
            // 
            // btnDst
            // 
            this.btnDst.Checked = true;
            this.btnDst.CheckOnClick = true;
            this.btnDst.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnDst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDst.Image = ((System.Drawing.Image)(resources.GetObject("btnDst.Image")));
            this.btnDst.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDst.Name = "btnDst";
            this.btnDst.Size = new System.Drawing.Size(23, 22);
            this.btnDst.Text = "Treeview";
            // 
            // lblDstPath
            // 
            this.lblDstPath.Name = "lblDstPath";
            this.lblDstPath.Size = new System.Drawing.Size(57, 22);
            this.lblDstPath.Text = "Location";
            // 
            // txtDstPath
            // 
            this.txtDstPath.AutoSize = false;
            this.txtDstPath.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.txtDstPath.Name = "txtDstPath";
            this.txtDstPath.Size = new System.Drawing.Size(100, 25);
            // 
            // msMain
            // 
            this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuHelp});
            this.msMain.Location = new System.Drawing.Point(0, 0);
            this.msMain.Name = "msMain";
            this.msMain.Size = new System.Drawing.Size(1264, 25);
            this.msMain.TabIndex = 1;
            this.msMain.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuShowLog,
            this.mnuShowTarget,
            this.mnuFileBar1,
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(39, 21);
            this.mnuFile.Text = "&File";
            // 
            // mnuShowTarget
            // 
            this.mnuShowTarget.Name = "mnuShowTarget";
            this.mnuShowTarget.Size = new System.Drawing.Size(180, 22);
            this.mnuShowTarget.Text = "Show &Target";
            // 
            // mnuFileBar1
            // 
            this.mnuFileBar1.Name = "mnuFileBar1";
            this.mnuFileBar1.Size = new System.Drawing.Size(177, 6);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Size = new System.Drawing.Size(180, 22);
            this.mnuFileExit.Text = "&Exit";
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(47, 21);
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(111, 22);
            this.mnuHelpAbout.Text = "&About";
            // 
            // mnuShowLog
            // 
            this.mnuShowLog.Name = "mnuShowLog";
            this.mnuShowLog.Size = new System.Drawing.Size(180, 22);
            this.mnuShowLog.Text = "Show &Log";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1264, 761);
            this.Controls.Add(this.spcMain);
            this.Controls.Add(this.msMain);
            this.MainMenuStrip = this.msMain;
            this.Name = "frmMain";
            this.Text = "Symbolic Link Generator";
            this.spcMain.Panel1.ResumeLayout(false);
            this.spcMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcMain)).EndInit();
            this.spcMain.ResumeLayout(false);
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
            this.tscDst.BottomToolStripPanel.ResumeLayout(false);
            this.tscDst.BottomToolStripPanel.PerformLayout();
            this.tscDst.ContentPanel.ResumeLayout(false);
            this.tscDst.TopToolStripPanel.ResumeLayout(false);
            this.tscDst.TopToolStripPanel.PerformLayout();
            this.tscDst.ResumeLayout(false);
            this.tscDst.PerformLayout();
            this.ssDst.ResumeLayout(false);
            this.ssDst.PerformLayout();
            this.spcDst.Panel1.ResumeLayout(false);
            this.spcDst.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcDst)).EndInit();
            this.spcDst.ResumeLayout(false);
            this.tsDst.ResumeLayout(false);
            this.tsDst.PerformLayout();
            this.msMain.ResumeLayout(false);
            this.msMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer spcMain;
        private System.Windows.Forms.ToolStripContainer tscSrc;
        private System.Windows.Forms.StatusStrip ssSrc;
        private System.Windows.Forms.ToolStripStatusLabel lblSrcCount;
        private System.Windows.Forms.ToolStripStatusLabel lblSrcSelCount;
        private System.Windows.Forms.ToolStrip tsSrc;
        private System.Windows.Forms.ToolStripButton btnSrc;
        private System.Windows.Forms.ToolStripLabel lblSrcPath;
        private System.Windows.Forms.ToolStripTextBox txtSrcPath;
        private System.Windows.Forms.ToolStripContainer tscDst;
        private System.Windows.Forms.StatusStrip ssDst;
        private System.Windows.Forms.ToolStripStatusLabel lblDstCount;
        private System.Windows.Forms.ToolStripStatusLabel lblDstSelCount;
        private System.Windows.Forms.SplitContainer spcDst;
        private System.Windows.Forms.TreeView tvwDst;
        private System.Windows.Forms.ListView lvwDst;
        private System.Windows.Forms.ToolStrip tsDst;
        private System.Windows.Forms.ToolStripButton btnDst;
        private System.Windows.Forms.ToolStripLabel lblDstPath;
        private System.Windows.Forms.ToolStripTextBox txtDstPath;
        private System.Windows.Forms.SplitContainer spcSrc;
        private System.Windows.Forms.TreeView tvwSrc;
        private System.Windows.Forms.ListView lvwSrc;
        private System.Windows.Forms.ColumnHeader colSrcName;
        private System.Windows.Forms.ColumnHeader colSrcSize;
        private System.Windows.Forms.ColumnHeader colSrcMemo;
        private System.Windows.Forms.ColumnHeader colDstName;
        private System.Windows.Forms.ColumnHeader colDstSize;
        private System.Windows.Forms.ColumnHeader colDtsMemo;
        private System.Windows.Forms.MenuStrip msMain;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStripMenuItem mnuShowTarget;
        private System.Windows.Forms.ToolStripSeparator mnuFileBar1;
        private System.Windows.Forms.ToolStripMenuItem mnuShowLog;
    }
}

