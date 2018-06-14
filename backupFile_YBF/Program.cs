using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.InteropServices;

namespace backupFile_YBF
{
    class Program
    {
        [DllImport("User32.dll ", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll ", EntryPoint = "GetSystemMenu")]
        extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);

        [DllImport("user32.dll ", EntryPoint = "RemoveMenu")]
        extern static int RemoveMenu(IntPtr hMenu, int nPos, int flags);


        private static int num = 0;
        //  private static string[] backupPdfList;
        private static List<FileInfo> localFileList = new List<FileInfo>(1000);
        private static List<FileInfo> backupFileList = new List<FileInfo>(1000);
        static void Main(string[] args)
        {
            try
            {
                DateTime dt_s = DateTime.Now;


                //使用映射获取执行程序集路径
                string appFullPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                //根据控制台标题找控制台
                int WINDOW_HANDLER = FindWindow(null, appFullPath);
                //找关闭按钮
                IntPtr CLOSE_MENU = GetSystemMenu((IntPtr)WINDOW_HANDLER, IntPtr.Zero);
                int SC_CLOSE = 0xF060;
                //关闭按钮禁用
                RemoveMenu(CLOSE_MENU, SC_CLOSE, 0x0);

                Console.WriteLine("正在加载备份盘的pdf文件列表...");               
                Console.WriteLine(@"F:\CTP己输出2009-2014");
                localFileList.AddRange(new DirectoryInfo(@"F:\CTP己输出2009-2014")
                    .GetFiles("*.pdf", System.IO.SearchOption.AllDirectories));
                Console.WriteLine(@"G:\CTP己输出2015备份");
                localFileList.AddRange(new DirectoryInfo(@"G:\CTP己输出2015备份")
                    .GetFiles("*.pdf", System.IO.SearchOption.AllDirectories));
                Console.WriteLine(@"\\128.1.30.86\老厂彩印ctp");
                backupFileList.AddRange(new DirectoryInfo(@"\\128.1.30.86\老厂彩印ctp")
                    .GetFiles("*.pdf", System.IO.SearchOption.AllDirectories));




                Directory.CreateDirectory("Log");

                Dictionary<string, string> pathDic = new Dictionary<string, string>();
                pathDic.Add(@"F:\CTP己输出2009-2014\09年12月", @"\\128.1.30.86\老厂彩印ctp\09年12月");
                pathDic.Add(@"F:\CTP己输出2009-2014\10年PDF", @"\\128.1.30.86\老厂彩印ctp");
                pathDic.Add(@"F:\CTP己输出2009-2014\11年PDF", @"\\128.1.30.86\老厂彩印ctp");
                pathDic.Add(@"F:\CTP己输出2009-2014\12年PDF", @"\\128.1.30.86\老厂彩印ctp\12年PDF");
                pathDic.Add(@"F:\CTP己输出2009-2014\13年PDF", @"\\128.1.30.86\老厂彩印ctp");
                pathDic.Add(@"F:\CTP己输出2009-2014\14年PDF", @"\\128.1.30.86\老厂彩印ctp");
                pathDic.Add(@"F:\CTP己输出2009-2014\15年PDF", @"\\128.1.30.86\老厂彩印ctp");
                pathDic.Add(@"G:\CTP己输出2015备份\16年PDF", @"\\128.1.30.86\老厂彩印ctp");
                pathDic.Add(@"G:\CTP己输出2015备份\17年PDF", @"\\128.1.30.86\老厂彩印ctp");
                pathDic.Add(@"G:\CTP己输出2015备份\18年PDF", @"\\128.1.30.86\老厂彩印ctp");

                foreach (FileInfo localPdf in localFileList)
                {
                    foreach (string key in pathDic.Keys)
                    {
                        if (localPdf.FullName.Contains(key))
                        {
                            string toFile = localPdf.FullName.Replace(key, pathDic[key]);
                            BackupFile(localPdf, toFile);
                            break;
                        }
                    }
                }

                //CTP输出记录表
                BackupFile(new FileInfo(@"E:\SOFTWARE\CTP输出记录表.xls")
                    ,@"\\128.1.30.86\老厂彩印ctp\CTP输出记录表.xls");




                //foreach (string localPath in pathDic.Keys)
                //{
                //    string backFile_Path = pathDic[localPath];

                //    foreach (string pdfFile in Directory.EnumerateFiles(localPath, "*.pdf"
                //        , System.IO.SearchOption.AllDirectories))
                //    {
                //        string toFile = pdfFile.Replace(localPath, backFile_Path);
                //        BackupFile(pdfFile, toFile);
                //    }

                //}





                Console.WriteLine("完成！");
                Console.WriteLine("耗时：{0}", DateTime.Now - dt_s);
                Console.WriteLine("按 Esc 结束并退出！");
                while (Console.ReadKey(true).Key != ConsoleKey.Escape)
                {

                }

            }
            catch (Exception)
            {

                throw;
            }
        }







        private static bool BackupFile(FileInfo fromFile, string toFile)
        {
            bool returnBool = false;

            Console.WriteLine();
            Console.WriteLine(++num);
            Console.WriteLine("准备备份");
            Console.WriteLine(fromFile.FullName);
            //判断目标文件是否存在。
            //如果存在，则按照修改时间来替换
            //如果不存在，则直接拷贝
            FileInfo toFileInfo = backupFileList.Find(f => f.FullName == toFile);
            if (toFileInfo != null)
            {
                Console.WriteLine("文件已经存在");
                Console.WriteLine("准备判断修改时间是否一样");
                if (fromFile.LastWriteTime > toFileInfo.LastWriteTime)
                {
                    Console.WriteLine("本地修改时间大于备份修改时间，执行拷贝操作");
                    returnBool = CopyFile(fromFile.FullName, toFileInfo.FullName, true);

                }
                else
                {
                    Console.WriteLine("备份修改时间大于本地修改时间，不执行拷贝操作\n");
                }
            }
            else
            {
                Console.WriteLine("文件不存在，直接执行拷贝操作！");
                returnBool = CopyFile(fromFile.FullName, toFile, false);
            }

            backupFileList.Remove(toFileInfo);
            return returnBool;
        }


        private static bool CopyFile(string fromFile, string toFile, bool overwrite)
        {
            string overStr = overwrite ? "_(替换)" : "";
            bool returnBool = false;
            try
            {
                Console.WriteLine(toFile);
                Console.WriteLine("正在复制......");
                //if (!Directory.Exists(Path.GetDirectoryName(toFile)))
                //{
                //    Directory.CreateDirectory(Path.GetDirectoryName(toFile));
                //}
                FileSystem.CopyFile(fromFile, toFile, true);
                // File.Copy(fromFile, toFile, true);
                Console.WriteLine("完成复制！");
                File.AppendAllText(string.Format("Log\\copyLog_{0}.txt",
                    DateTime.Now.ToString("yyyy-MM-dd")),
                    string.Format("{3}{2}{0}{2}{1}{2}{2}", fromFile, toFile,
                    Environment.NewLine, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + overStr));
                returnBool = true;
            }
            catch (Exception)
            {
                returnBool = false;
            }
            return returnBool;
        }
    }
}
