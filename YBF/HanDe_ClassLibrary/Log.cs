using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace HanDe_ClassLibrary.LogCommon
{
    /// <summary>
    /// 日志
    /// </summary>
    public static class Log
    {
       // private static readonly string logFile = "Log\\" + DateTime.Now.ToString("yyyyMMdd") + "_Log.xml";
        private static readonly string logFile = "Log\\Log.xml";

        public static void WriteErrorLog(string Mess)
        {
            //建立文件夹
            if (!Directory.Exists("Log"))
            {
                Directory.CreateDirectory("Log");
            }

            XmlDocument xmlDoc = new XmlDocument();//定义XML文档
            //建立xml文件
            if (!File.Exists(logFile))
            {                
                XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);//定义声明
                xmlDoc.AppendChild(declaration);//添加声明
                //添加根节点
                XmlElement rootElement = xmlDoc.CreateElement("Errors");
                xmlDoc.AppendChild(rootElement);
                xmlDoc.Save(logFile);
            }

            DateTime dt = DateTime.Now;
            xmlDoc.Load(logFile);
            XmlElement childElement = xmlDoc.CreateElement("event");
            childElement.SetAttribute("DateTime", dt.ToString());
            childElement.SetAttribute("Description", Mess);
            xmlDoc.DocumentElement.AppendChild(childElement);
            xmlDoc.Save(logFile);
        }
    }
}
