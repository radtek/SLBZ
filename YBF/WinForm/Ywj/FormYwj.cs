﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HandeJobManager.DAL;
using YBF.Class.Model;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using Illustrator;
using YBF.Properties;
using YBF.Class.Comm;

namespace YBF.WinForm.Ywj
{
    public partial class FormYwj : Form
    {
        public FormYwj()
        {
            InitializeComponent();
        }

        private void FormYwj_Load(object sender, EventArgs e)
        {
            //关闭多余的窗体
            foreach (Form f in this.ParentForm.MdiChildren)
            {
                if (f.Name == this.Name && f.Handle != this.Handle)
                {
                    f.Dispose();
                }
            }

            InitListView();
        }

        private void InitListView()
        {
            this.listViewYwj.Items.Clear();
            DataTable dt = SQLiteList.Ybf.ExecuteDataTable("select * from Ywj");
            if (dt.Rows.Count > 0)
            {
                //排除名单
                List<string> pcmdList = new List<string>();
                foreach (DataRow md in SQLiteList.Ybf.ExecuteDataTable("select * from Ywj_PaiChu").Rows)
                {
                    pcmdList.Add(md["FileFullName"].ToString());
                }


                foreach (DataRow dr in dt.Rows)
                {
                    YwjInfo ywj = new YwjInfo();
                    ywj.ID = Convert.ToInt32(dr["ID"]);
                    ywj.Name = dr["Name"].ToString();
                    ywj.Path = dr["Path"].ToString();
                    ywj.PathMove = dr["PathMove"].ToString();

                    foreach (string file in Directory.EnumerateFiles(ywj.Path, "."))
                    {
                        //排除名单
                        if (pcmdList.Contains(file))
                        {
                            continue;
                        }
                        //符合要求的正则表达式
                        Regex regex = new Regex(".+\\.pdf|.+\\.ai|.+\\.cdr", RegexOptions.IgnoreCase);
                        if (regex.IsMatch(file))
                        {
                            //添加
                            ListViewItem lvi = new ListViewItem
                                (
                               new string[]{
                       Path.GetFileName(file),
                               File.GetLastWriteTime(file).ToString("yyyy-MM-dd HH:mm:ss"),
                               ywj.Name
                           }
                                );
                            lvi.Tag = ywj;
                            this.listViewYwj.Items.Add(lvi);
                        }
                    }
                }
            }
            this.listViewYwj.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void tsmiSetting_Click(object sender, EventArgs e)
        {
            new FormYwjSetting().ShowDialog();
        }

        //true表示无效.false表示有效.
        //此函数为全局更改(慎用!!!)
        protected override bool ProcessCmdKey(ref　Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)//刷新
            {
                InitListView();
            }


            return false;

        }

        private void tsmiRefresh_Click(object sender, EventArgs e)
        {
            InitListView();
        }

        private void tsmiCopy_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            string Path_CopyTo = string.Format(@"H:\输出原文件\{0}年{1}月\{1}-{2}",
                dt.ToString("yy"), dt.Month, dt.Day);
            ListView.SelectedListViewItemCollection coll = this.listViewYwj.SelectedItems;
            if (coll.Count > 0)
            {
                foreach (ListViewItem item in coll)
                {
                    YwjInfo ywj = item.Tag as YwjInfo;
                    string sourceFileName = ywj.Path + "\\" + item.Text;
                    string destinationFileName = Path_CopyTo + "\\" + item.Text;
                    if (Path.GetExtension(sourceFileName).LastIndexOf(".pdf"
                       , StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        destinationFileName = @"\\128.1.30.144\HotFolders\RefineToPDF\"
                            + Path.GetFileName(destinationFileName);
                    }
                    FileSystem.CopyFile(sourceFileName, destinationFileName
                        , UIOption.AllDialogs, UICancelOption.DoNothing);
                   
                    if (File.Exists(destinationFileName))
                    {
                        FileSystem.MoveFile(sourceFileName, ywj.PathMove + "\\" + item.Text
                       , UIOption.AllDialogs, UICancelOption.DoNothing);
                        if (Path.GetExtension(destinationFileName).LastIndexOf(".ai", StringComparison.OrdinalIgnoreCase) >-1)
                        {
                            Comm_Method.AiFileList.Add(destinationFileName);
                        }
                    }
                }
                InitListView();
                if (!Directory.Exists(Path_CopyTo + "\\ok"))
                {
                    Directory.CreateDirectory(Path_CopyTo + "\\ok");
                }
                Process.Start("Explorer.exe", Path_CopyTo);

            }

        }
        ///// <summary>
        ///// 转存为PDF
        ///// </summary>
        ///// <param name="fileList"></param>
        //public  void AutoSavePdf(List<string> fileList)
        //{

        //    if (fileList.Count == 0)
        //    {
        //        return;
        //    }
        //    //***开始自动转PDF
        //    //判断Adobe Illustrator CS6 (64 Bit)是否运行
        //    string aiexe = @"C:\Program Files\Adobe\AdobeIllustratorCS6_x64\Support Files\Contents\Windows\Illustrator.exe";
        //    if (File.Exists(aiexe))
        //    {
        //        Illustrator.ApplicationClass app = new Illustrator.ApplicationClass();
        //        foreach (string item in fileList)
        //        {
        //            app.DoJavaScript(Resources.AutoSavePdf.Replace
        //                ("*文件名*", item.Replace('\\', '/')));
        //        }
        //    }
        //}


        private void tsmiPaiChu_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection coll = this.listViewYwj.SelectedItems;
            List<string> sqlList = new List<string>();
            if (coll.Count > 0)
            {
                foreach (ListViewItem item in coll)
                {
                    YwjInfo ywj = item.Tag as YwjInfo;
                    string fileFullName = ywj.Path + "\\" + item.Text;
                    sqlList.Add(
                        "INSERT INTO [Ywj_PaiChu]([FileFullName])VALUES('"
                        + fileFullName + "');");
                }
                if (SQLiteList.Ybf.ExecuteSqlTran(sqlList))
                {
                    InitListView();
                }

            }
        }
    }
}
