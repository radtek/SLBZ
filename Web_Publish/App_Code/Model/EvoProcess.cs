using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;


/// <summary>
///提供用于读取印能捷作业信息的静态方法
/// </summary>
public static class EvoProcess
{

    private static String GetEvoPath()
    {
        String name = "";

        name = ComputerComm.GetComputerName();
        if (name.Equals("evoaoguang", StringComparison.CurrentCultureIgnoreCase))
        {
            return "C:\\Program Files\\Kodak\\Prinergy Evo 5.1.6.9";
        }
        else if (name.Equals("evo8382-01", StringComparison.CurrentCultureIgnoreCase))
        {
            return "C:\\Program Files\\Kodak\\Prinergy Evo 5.1.6.5";
        }
        else
        {
            return "C:\\Program Files\\Kodak\\Prinergy Evo 5.1.6.5";
        }
    }

    public static List<String> Get_historical_data_Files_hours(int hour)
    {

        String path = GetEvoPath() + "\\historical_data\\processes";
		// String path = "\\\\128.1.30.144\\historical_data\\processes";
		String searchPattern = "1-3{printing-to-device}.out.jtk";
		List<String> fileList = new List<string>();

        foreach (string guidPath in Directory.EnumerateDirectories(path))
        {
            if (Directory.GetLastWriteTime(guidPath).AddHours(hour + 10) >= DateTime.Now)
            {
                foreach (string file in Directory.EnumerateFiles
                    (guidPath,searchPattern,SearchOption.AllDirectories))
                {
                    if (File.GetLastWriteTime(file).AddHours(hour) >= DateTime.Now)
                    {
                        fileList.Add(file);
                    }                    
                }
            }
        }

		return fileList;
    }


    public static List<String> Get_dynamic_data_Files_All()
    {
        String path = GetEvoPath() + "\\dynamic_data\\processes";
        // String path = "\\\\128.1.30.144\\historical_data\\processes";
        String searchPattern = "1-2{applying-geometry}.out.jtk";
        List<String> fileList = new List<string>();
        FileStream fs = null;
        StreamReader sr = null;

        foreach (string guidPath in Directory.EnumerateDirectories(path))
        {
            if (Directory.GetLastWriteTime(guidPath).AddHours(48)<DateTime.Now)
            {
                continue;
            }
                foreach (string file in Directory.EnumerateFiles
                    (guidPath, searchPattern, SearchOption.AllDirectories))
                {
                    //读取里面的所有的内容,保存到字符串
                    try
                    {
                        fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        sr = new StreamReader(fs);
                        string allText = sr.ReadToEnd();
                        sr.Dispose();
                        fs.Dispose();                   
                    if (allText.IndexOf("printing-to-device",StringComparison.CurrentCultureIgnoreCase)>=0)
                    {
                        fileList.Add(file);
                    }
                    }
                    catch
                    {
                        if (sr!=null)
                        {
                            sr.Dispose();
                        }
                        if (fs!=null)
                        {
                            fs.Dispose();
                        }
                    }
                }
        }

        return fileList;
    }

}