using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace HanDe_ClassLibrary.PrinergyEvoFile.Preps
{
    public static class PrepsConfig
    {
        public static bool SetOpenJobPath(string path)
        {
            try
            {
                //string defaultFile=@"C:\Preps 5.3.2\Profiles\Default\default.cfg";

                string text = File.ReadAllText(path);

                //替换输出路径
                //Regex regex = new Regex(@"-WINOUTPATH\:.*");
                //Match match = regex.Match(text);
                //string oldStr=match.Value;
                //string newStr = @"-WINOUTPATH:" + path;
                //text = text.Replace(oldStr, newStr);

                //替换打开路径
                Regex regex = new Regex(@"-WINOPENJOBPATH\:.*");
                Match match = regex.Match(text);
                string oldStr = match.Value;
                string newStr = @"-WINOPENJOBPATH:" + path;
                text = text.Replace(oldStr, newStr);

                //写入文件
                File.WriteAllText(path, text);


                return true;
            }
            catch
            {
                return false;
            }


        }
    }
}
