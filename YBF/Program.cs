using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HandeJobManager.DAL;

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
            SQLiteList.YbfSQLite=new SQLiteDbHelper(
                @"Data Source=" + Application.StartupPath + "\\Data\\ybf.db;Version=3;");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
