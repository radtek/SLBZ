using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using HanDe_ClassLibrary.PrepressFile.Adobe.Acrobat;
using HanDe_ClassLibrary.LogCommon;

namespace HanDe_ClassLibrary.PrinergyEvoFile.Preps
{
    /// <summary>
    /// 此类提供了job文件的读取,包括连接的pdf和可能需要印刷的颜色
    /// </summary>
    public  class JobFile
    {
        public FileInfo JobFileInfo { get; set; }

        /// <summary>
        /// 初始化一个JobFile实例
        /// </summary>
        /// <param name="fileInfo"></param>
        public JobFile(FileInfo fileInfo)
        {
            if (fileInfo.Exists
                && fileInfo.Extension.ToLower() == ".job"
                && fileInfo.Length < 10 * 1024 * 1024)
            {
                this.JobFileInfo = fileInfo;
            }
        }

        /// <summary>
        /// 获取job里面的pdf文件列表
        /// </summary>
        /// <returns></returns>
        public List<Acrobat8> GetPdfList()
        {
            FileStream fs = null;
            StreamReader sr = null;
            List<Acrobat8> acrobat8List = new List<Acrobat8>();

            try
            {
                fs = new FileStream(this.JobFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader(fs);

                string allText = sr.ReadToEnd();
                //释放流
                {
                    sr.Close();
                    fs.Close();
                }

                Regex regex = new Regex("//.*.pdf",RegexOptions.IgnoreCase);
                foreach (Match item in regex.Matches(allText))
                {
                    string temp =Uri.UnescapeDataString( item.Value);
                    acrobat8List.Add(new Acrobat8(new FileInfo(temp)));
                }

            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());
                
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                } 
            }
            return acrobat8List;
        }


        /// <summary>
        /// 获取job里面的颜色列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetColorList()
        {
            FileStream fs = null;
            StreamReader sr = null;
            List<string> colorList = new List<string>();

            try
            {
                fs = new FileStream(this.JobFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader(fs);

                string allText = sr.ReadToEnd();
                //释放流
                {
                    sr.Close();
                    fs.Close();
                }

                Regex regex = new Regex("%SSiJobColor: '.*'");
                foreach (Match item in regex.Matches(allText))
                {
                    if (item.Value == @"%SSiJobColor: 'Composite'")
                    {
                        continue;
                    }
                    Regex reg = new Regex("'.*'");
                    string temp = reg.Match(item.Value).Value.Trim('\'');
                    if (colorList.IndexOf(temp) < 0)
                    {
                        colorList.Add(temp);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());

            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return colorList;
        }
    }
}
