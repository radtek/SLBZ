using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HandeJobManager.DAL;
using System.Diagnostics;

namespace YBF
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
           

            SQLiteList.Ybf=new SQLiteDbHelper(
                @"Data Source=" + Application.StartupPath + "\\Data\\ybf.db;Version=3;");
            SQLiteList.BackupProcess = new SQLiteDbHelper(
                @"Data Source=\\128.1.30.144\Backup_ev08382-01\processes\BackupIndex.db;Version=3;");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
