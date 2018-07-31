using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YBF.WinForm.Ywj;
using YBF.WinForm.ChuBan;
using System.IO;
using HanDe_ToolBox_Form;

namespace YBF
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        FormYwj ywj = null;
        private void tsmiYwj_Click(object sender, EventArgs e)
        {
            ywj = new FormYwj();
            ywj.WindowState = FormWindowState.Maximized;
            ywj.MdiParent = this;
            ywj.Show();
        }

        FormChuBan chuban = null;
        private void tsmiChuban_Click(object sender, EventArgs e)
        {
            if (chuban == null
               || chuban.IsDisposed)
            {
                chuban = new FormChuBan();
                chuban.MdiParent = this;
            }
            chuban.Show();
            chuban.WindowState = FormWindowState.Maximized;
            chuban.Focus();
        }

        private void tsmiMovePublishedPdf_Click(object sender, EventArgs e)
        {
            string[] topPaths ={@"\\128.1.30.144\historical_data\processes"
                              ,@"\\128.1.130.31\historical_data\processes"};

            foreach (string topPath in topPaths)
            {
                if (!Directory.Exists(topPath))
                {
                    continue;
                }
                foreach (string guidPath in Directory.EnumerateDirectories(topPath))
                {
                    //bool bl1 = Directory.GetCreationTime(guidPath).AddHours(24) > DateTime.Now;
                    //bool bl2 = File.Exists(topPath + "\\jt-trail\\1-3{printing-to-device}.out.jtk");
                    //bool bl3 = File.Exists(topPath + "\\ProcessCreationInfo.txt");
                    if (Directory.GetCreationTime(guidPath).AddHours(24) > DateTime.Now
                        && File.Exists(guidPath + "\\jt-trail\\1-3{printing-to-device}.out.jtk")
                        && File.Exists(guidPath + "\\ProcessCreationInfo.txt"))
                    {
                        string ProcessCreationInfo = guidPath + "\\ProcessCreationInfo.txt";
                        string[] allLines = File.ReadAllLines(ProcessCreationInfo);
                        for (int i = 12; i < allLines.Length; i++)
                        {
                            string fileFullName = allLines[i];
                            if (Path.GetDirectoryName(fileFullName)
                                .Equals(@"\\ev08382-01\JobData\pdf\已下单PDF"
                                ,StringComparison.CurrentCultureIgnoreCase)
                                && File.Exists(fileFullName))
                            {
                                string toPath = Path.GetDirectoryName(fileFullName)
                                    + "\\" +
                                    (DateTime.Now.Hour < 5 ? DateTime.Now.AddDays(-1).ToString("M-d") : DateTime.Now.ToString("M-d"));
                                if (!Directory.Exists(toPath))
                                {
                                    Directory.CreateDirectory(toPath);
                                }
                                File.Move(fileFullName,
                                    toPath + "\\" + Path.GetFileName(fileFullName));
                            }
                        }
                    }
                }
            }
        }

        private void tsmiWindows_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            t.DropDownItems.Clear();
            Form[] windows = this.MdiChildren;
            if (windows.Length > 0)
            {
                ToolStripMenuItem tool = new ToolStripMenuItem("全部关闭");
                tool.Click += new EventHandler(tool_Click);
                t.DropDownItems.Add(tool);
                tool = new ToolStripMenuItem("全部最小化");
                tool.Click += new EventHandler(tool_Click);
                t.DropDownItems.Add(tool);
                t.DropDownItems.Add(new ToolStripSeparator());
            }
            foreach (Form f in windows)
            {
                ToolStripMenuItem m = new ToolStripMenuItem(f.Text);
                m.Click += new EventHandler(tsmi_Click);
                t.DropDownItems.Add(m);
            }
        }
        private void tool_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            switch (tsmi.Text)
            {
                case "全部关闭":
                    foreach (Form f in this.MdiChildren)
                    {
                        f.Dispose();
                    }
                    break;
                case "全部最小化":
                    foreach (Form f in this.MdiChildren)
                    {
                        f.WindowState = FormWindowState.Minimized;
                    }
                    break;
                default:
                    break;
            }
        }
        private void tsmi_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            foreach (Form f in this.MdiChildren)
            {
                if (tsmi.Text == f.Text)
                {
                    f.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    f.WindowState = FormWindowState.Minimized;
                }
            }
        }

        FormFindOld findOld=null;
        private void tsmiOldPlant_Click(object sender, EventArgs e)
        {
            if (findOld == null
                || findOld.IsDisposed)
            {
                findOld = new FormFindOld(null);
                findOld.MdiParent = this;
            }
            findOld.Show();
            findOld.WindowState = FormWindowState.Maximized;
            findOld.Focus();
        }

        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    break;
                case WatcherChangeTypes.Deleted:
                    break;
                case WatcherChangeTypes.Renamed:
                    break;
            }   
        }


        FileSystemWatcher watcher_2009_2015 = null;
        FileSystemWatcher watcher_2016_2018 = null;
        FileSystemWatcher watcher_PDFok = null;
        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private FileSystemWatcher GetFileSystemWatcher(string path)
        {
            FileSystemWatcher watcher = null;
            if (Directory.Exists(path))
            {
                watcher = new FileSystemWatcher(path, "*.pdf");
                watcher.EnableRaisingEvents = false;
                watcher.IncludeSubdirectories = true;
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            }
            return watcher;
        }

        //统计出版记录
        FormPublishProcess publishProcess = null;
        private void tsmiProcess_Click(object sender, EventArgs e)
        {
            if (publishProcess == null
                || publishProcess.IsDisposed)
            {
                publishProcess = new FormPublishProcess();
                publishProcess.MdiParent = this;
            }
            publishProcess.Show();
            publishProcess.WindowState = FormWindowState.Maximized;
            publishProcess.Focus();
        }
    }
}
