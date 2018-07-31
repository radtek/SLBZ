namespace YBF
{
    partial class MainForm
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmiYwj = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiChuban = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOldPlant = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMovePublishedPdf = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiProcess = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiYwj,
            this.tsmiChuban,
            this.tsmiOldPlant,
            this.tsmiMovePublishedPdf,
            this.tsmiProcess,
            this.tsmiWindows});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(609, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmiYwj
            // 
            this.tsmiYwj.Name = "tsmiYwj";
            this.tsmiYwj.Size = new System.Drawing.Size(53, 20);
            this.tsmiYwj.Text = "源文件";
            this.tsmiYwj.Click += new System.EventHandler(this.tsmiYwj_Click);
            // 
            // tsmiChuban
            // 
            this.tsmiChuban.Name = "tsmiChuban";
            this.tsmiChuban.Size = new System.Drawing.Size(41, 20);
            this.tsmiChuban.Text = "出版";
            this.tsmiChuban.Click += new System.EventHandler(this.tsmiChuban_Click);
            // 
            // tsmiOldPlant
            // 
            this.tsmiOldPlant.Name = "tsmiOldPlant";
            this.tsmiOldPlant.Size = new System.Drawing.Size(41, 20);
            this.tsmiOldPlant.Text = "旧版";
            this.tsmiOldPlant.Click += new System.EventHandler(this.tsmiOldPlant_Click);
            // 
            // tsmiMovePublishedPdf
            // 
            this.tsmiMovePublishedPdf.Name = "tsmiMovePublishedPdf";
            this.tsmiMovePublishedPdf.Size = new System.Drawing.Size(113, 20);
            this.tsmiMovePublishedPdf.Text = "一键移动出版文件";
            this.tsmiMovePublishedPdf.Click += new System.EventHandler(this.tsmiMovePublishedPdf_Click);
            // 
            // tsmiWindows
            // 
            this.tsmiWindows.Name = "tsmiWindows";
            this.tsmiWindows.Size = new System.Drawing.Size(41, 20);
            this.tsmiWindows.Text = "窗口";
            this.tsmiWindows.DropDownOpening += new System.EventHandler(this.tsmiWindows_DropDownOpening);
            // 
            // tsmiProcess
            // 
            this.tsmiProcess.Name = "tsmiProcess";
            this.tsmiProcess.Size = new System.Drawing.Size(65, 20);
            this.tsmiProcess.Text = "出版记录";
            this.tsmiProcess.Click += new System.EventHandler(this.tsmiProcess_Click);
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 487);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "森林包装印版房作业管理系统";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiYwj;
        private System.Windows.Forms.ToolStripMenuItem tsmiChuban;
        private System.Windows.Forms.ToolStripMenuItem tsmiMovePublishedPdf;
        private System.Windows.Forms.ToolStripMenuItem tsmiWindows;
        private System.Windows.Forms.ToolStripMenuItem tsmiOldPlant;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.ToolStripMenuItem tsmiProcess;
    }
}

