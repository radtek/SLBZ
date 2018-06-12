using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using HanDe_ClassLibrary.Common.SizeBox;
using HanDe_ClassLibrary.Common.Unit;

/// <summary>
///印能捷作业记录的实例
/// </summary>
public class EvoPrintingToDeviceProcessInfo
{
    /// <summary>
    /// 唯一标识(GUID)
    /// </summary>
    public string Guid;
    /// <summary>
    /// 稿袋号
    /// </summary>
    public string Gaodaihao;
    /// <summary>
    /// 作业提交时间
    /// </summary>
    public DateTime SubmissionDate;
    /// <summary>
    /// 作业完成时间
    /// </summary>
    public DateTime CompletionTime;
    /// <summary>
    ///作业的文件列表
    /// </summary>
    public List<String> FileList;
    /// <summary>
    ///板材
    /// </summary>
    public String Plant;
    /// <summary>
    ///颜色列表
    /// </summary>
    public List<String> ColorList;
    /// <summary>
    ///颜色数量
    /// </summary>
    public int ColorNumber;
    /// <summary>
    ///垂直向下偏移晾
    /// </summary>
    public double OffsetY;
    /// <summary>
    ///线数
    /// </summary>
    public double RulingOrFeatureSize;
    /// <summary>
    ///校准曲线
    /// </summary>
    public String CalibrationTarget;
    /// <summary>
    ///网点形状
    /// </summary>
    public String DotShape;
    /// <summary>
    /// 板材长边
    /// </summary>
    public int Plant_L;
    /// <summary>
    /// 板材短边
    /// </summary>
    public int Plant_S;
    /// <summary>
    /// 成像位置
    /// </summary>
    public CREO_TrimBox_Point ImagingPosition;
    /// <summary>
    /// 实例化 印能捷作业记录
    /// </summary>
    public EvoPrintingToDeviceProcessInfo()
    {
        this.FileList = new List<String>();
        this.CompletionTime = new DateTime();
        this.SubmissionDate = new DateTime();
        this.ColorList = new List<String>();

    }

    /// <summary>
    /// 实例化 印能捷作业记录
    /// </summary>
    public EvoPrintingToDeviceProcessInfo(string fileFullName)
    {
        this.FileList = new List<String>();
        this.CompletionTime = new DateTime();
        this.SubmissionDate = new DateTime();
        this.ColorList = new List<String>();

        FileStream fs = null;
        StreamReader sr = null;
        try
        {
            //读取里面的所有的内容,保存到字符串
            fs = new FileStream(fileFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            sr = new StreamReader(fs);
            string allText = sr.ReadToEnd();
            sr.Dispose();
            fs.Dispose();
            //如果不存在“打印到设备（printing-to-device）”的标记，则退出（即返回空）
            if ((fileFullName + allText).IndexOf("printing-to-device") == -1) return;


            FileInfo file_printingToDevice = new FileInfo(fileFullName);
            String pubPath = Path.GetDirectoryName(file_printingToDevice.DirectoryName);

            // 判断ProcessCreationInfo.txt是否存在
            String ProcessCreationInfo = pubPath + "\\ProcessCreationInfo.txt";
            if (!File.Exists(ProcessCreationInfo))
            {
                return;
            }

            // 读取文本文件的所有行
            string[] AllLine = File.ReadAllLines(ProcessCreationInfo);
            int index = AllLine[3].IndexOf(':') + 1; ;
            //提取出时间                    
            string str = AllLine[3].Substring(index).Trim();
            char[] split = new char[] { '-', '.' };
            string[] dateStr = str.Split(split, 8);
            DateTime dateTime = new DateTime(Convert.ToInt32(dateStr[0])
                , Convert.ToInt32(dateStr[1])
                , Convert.ToInt32(dateStr[2])
                , Convert.ToInt32(dateStr[3])
                , Convert.ToInt32(dateStr[4])
                , Convert.ToInt32(dateStr[5])
                );

            this.SubmissionDate = dateTime;
            //**完成时间
            this.CompletionTime = File.GetLastWriteTime(fileFullName);

            //***提取出GUID
            index = AllLine[2].IndexOf(":") + 1;
            str = AllLine[2].Substring(index).Trim();
            this.Guid = str;


            // ***提取文件名
            for (int i = 12; i < AllLine.Length; i++)
            {
                String processFilename = AllLine[i];
                this.FileList.Add(processFilename);
            }

            // ****提取板材和提取颜色*******

            int lastIndex;
            // ***提取板材

            index = allText.IndexOf("/Ct (");
            lastIndex = allText.IndexOf("\n", index);
            str = allText.Substring(index, lastIndex - index);
            // 定位下划线
            index = str.LastIndexOf('_');
            // 定位符合')'
            lastIndex = str.LastIndexOf(')');
            str = str.Substring(index + 1, lastIndex - index);
            Regex regex = new Regex("\\d+");
            MatchCollection matchs = regex.Matches(str);
            this.Plant = matchs[0].Value + "*" + matchs[1].Value;

            // ***提取颜色数量和颜色列表
            index = allText.LastIndexOf("\n/CO [\n");
            lastIndex = allText.IndexOf(" ]/CP [\n", index);
            str = allText.Substring(index + 7, lastIndex - index);
            // 去除str最后的字符
            str = str.Replace("]/CP [", "");
            // 取出str最前面的符号'/'
            str = str.TrimStart('/').Trim();
            // 对颜色进行拆分
            String[] colors = str.Split('/');
            this.ColorNumber = colors.Length * this.FileList.Count;

            foreach (String color in colors)
            {
                this.ColorList.Add(color.Replace("#3d", "=").Replace("#20", " ").Replace("\n", "").Trim());
            }

            // ***垂直偏移
            index = allText.IndexOf("/CPC_MediaOffset ");
            lastIndex = allText.IndexOf("\n", index);
            str = allText.Substring(index, lastIndex - index);
            str = str.Replace("/CPC_MediaOffset ", "");
            str = str.Trim();
            // 定位要相应数据的位置
            str = "\n" + str.Replace("R", "obj\n");
            index = allText.IndexOf(str);
            index = allText.IndexOf("[", index);
            lastIndex = allText.IndexOf("]", index);
            str = allText.Substring(index, lastIndex - index);
            //识别数字
            matchs = new Regex("\\d+\\.\\d+").Matches(str);
            this.OffsetY = Math.Round(Double.Parse(matchs[1].Value) * ConversionConstant.MM_PER_PT);
            // ***线数
            index = allText.IndexOf("/CPC_RulingOrFeatureSize");
            lastIndex = allText.IndexOf("\n", index);
            str = allText.Substring(index, lastIndex - index);
            str = str.Replace("/CPC_RulingOrFeatureSize", "");
            str = str.Trim();
            this.RulingOrFeatureSize = Double.Parse(str);

            // ***校准曲线
            index = allText.IndexOf("/CPC_CalibrationTarget");
            lastIndex = allText.IndexOf("\n", index);
            str = allText.Substring(index, lastIndex - index);
            // 定位左右圆括号
            index = str.IndexOf("(");
            lastIndex = str.LastIndexOf(")");
            str = str.Substring(index + 1, lastIndex - index - 1);
            this.CalibrationTarget = str;

            // ***网点形状
            index = allText.IndexOf("/CPC_DotShape");
            lastIndex = allText.IndexOf("\n", index);
            str = allText.Substring(index, lastIndex - index);
            // 定位左右圆括号
            index = str.IndexOf("(");
            lastIndex = str.LastIndexOf(")");
            str = str.Substring(index + 1, lastIndex - index - 1);
            this.DotShape = str;

            //***板材长边和短边
            string[] plantArray = this.Plant.Split('*');
            if (plantArray.Length == 2)
            {
                int side_L = Convert.ToInt32(plantArray[1]);
                int side_S = Convert.ToInt32(plantArray[0]);
                if (side_L < side_S)
                {
                    side_L = side_L + side_S;
                    side_S = side_L - side_S;
                    side_L = side_L - side_S;
                }
                this.Plant_L = side_L;
                this.Plant_S = side_S;
            }
            //****成像位置
            string txt = allText.Replace('\n', '\0');
            regex = new Regex(@"\]/Cl \[.*\]/D");
            if (regex.IsMatch(txt))
            {
                MatchCollection mc = new Regex(@"-?\d+(\.\d+)?")
                    .Matches(regex.Match(txt).Value);
                if (mc != null && mc.Count == 4)
                {
                    double left = Convert.ToDouble(mc[0].Value);
                    double down = Convert.ToDouble(mc[1].Value);
                    double right = Convert.ToDouble(mc[2].Value);
                    double top = Convert.ToDouble(mc[3].Value);
                    ImagingPosition = new CREO_TrimBox_Point(
                        new Point_Unit(top), new Point_Unit(down), new Point_Unit(left), new Point_Unit(right));
                }
            }
            //***稿袋号

        }
        finally
        {
            if (sr != null)
            {
                sr.Dispose();
            }
            if (fs != null)
            {
                fs.Dispose();
            }
        }
    }


}