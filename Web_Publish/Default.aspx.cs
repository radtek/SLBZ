using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Text;
using HanDe_ClassLibrary.PrepressFile.Adobe.Acrobat;
using HanDe_ClassLibrary.Common.SizeBox;
using System.Threading;

public partial class PublishProcess : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        string[] values = Request.QueryString.GetValues("id");
        bool isShowGdh = false;
        if (values != null && (new string[] { "1", "true" }.Contains(values[0])))
        {
            isShowGdh = true;
        }
        ShowProcess(isShowGdh);
    }


    internal static int CompareProcessByCompletionTime(EvoProcessInfo x, EvoProcessInfo y)
    {
        if (x == null)
        {
            if (y == null)
            {
                //如果x为空并且y也没空，则判断相等
                return 0;
            }
            else
            {
                //如果x为空并且y不为空，则判断y大
                return -1;
            }
        }
        else
        {
            //如果x不为空
            if (y == null)
            // ...同时y为空，则判断x大
            {
                return 1;
            }
            else
            {
                // ...同时y也不为空，则判断两者的提交时间
                int retval = x.CompletionTime.CompareTo(y.CompletionTime);

                if (retval != 0)
                {
                    //如果x和y的提交时间不一样，则时间晚的大
                    return retval;
                }
                else
                {
                    // 如果x和y的提交时间完全一样，
                    // 则按照他们的guid来排序
                    return x.Guid.CompareTo(y.Guid);
                }
            }
        }
    }
    internal string GetFileNameWithoutExtension(string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }


    internal void Println(string s)
    {
        Response.Write(s + "<br/>\r\n");
    }





    private string GetString(string str)
    {
        return str.Replace(" ", "").Replace("（", "(").Replace("）", ")")
            .Replace("/", "-").Replace("*", "x").ToLower();
    }

    private bool IsEachContain(string str1, string str2)
    {

        str1 = GetString(str1);
        str2 = GetString(str2);
        return str1.IndexOf(str2) > -1 || str2.IndexOf(str1) > -1;
    }

    protected void ButtonSubmit_Click(object sender, EventArgs e)
    {
        ShowProcess(CheckBoxGdh.Checked);
    }
    protected void Timer1_Tick(object sender, EventArgs e)
    {
        this.Timer1.Enabled = false;
        ShowProcess(CheckBoxGdh.Checked);
        this.Timer1.Enabled = true;
    }

    private void ShowProcess(bool isShowGdh)
    {
        //  //出版的Excel文件
        DataTable dt_gdh_cpmc = new DataTable();
        if (isShowGdh)
        {
            CheckBoxGdh.Checked = true;
        }
        if (CheckBoxGdh.Checked)
        {
            // PublishJobTable.GetAllPublishedJobTableByHour();
            PublishJobTable.GetPublishedJobTable_All();

            //***读取稿袋号和产品名称
            dt_gdh_cpmc = SQLiteDbHelper.ExecuteDataTable(
                "SELECT *FROM [Job] order by [Excel时间]desc limit 1000");
        }

        //****历史记录****
        //加载指定小时以内的完成出版的记录	
        int hour = 2;
        List<EvoProcessInfo> hisProcessList = new List<EvoProcessInfo>();

        foreach (String fileFullName in EvoProcess.Get_historical_data_Files_hours(hour))
        {
            EvoProcessInfo process = new EvoProcessInfo(fileFullName);
            if (process != null)
            {
                //判断GUID是否已经存在,并确定整个的完成时间
                bool cunzai = false;
                int index = -1;
                for (int i = 0; i < hisProcessList.Count; i++)
                {
                    if (process.Guid == hisProcessList[i].Guid)
                    {
                        cunzai = true;
                        index = i;
                        break;
                    }
                }

                if (cunzai)
                {
                    if (hisProcessList[index].CompletionTime.CompareTo(process.CompletionTime) < 0)
                    {
                        hisProcessList[index].CompletionTime = process.CompletionTime;
                    }
                }
                else
                {
                    hisProcessList.Add(process);
                }

            }

        }
        //************

        if (hisProcessList != null && hisProcessList.Count > 0)
        {
            //需要绑定到GridView中的DataTable
            DataTable gridVies_dt = new DataTable();
            gridVies_dt.Columns.Add("文件名");
            gridVies_dt.Columns.Add("板材咬口");
            gridVies_dt.Columns.Add("颜色");
            gridVies_dt.Columns.Add("加网信息");


            //按照完成时间排序(晚的时间靠前)
            hisProcessList.Sort(CompareProcessByCompletionTime);


            //遍历整个整理好的列表,并且将数据添加到表格中
            foreach (EvoProcessInfo proInfo in hisProcessList)
            {
                DataRow dr = gridVies_dt.NewRow();
                StringBuilder sb = new StringBuilder();
                foreach (String fileName in proInfo.FileList)
                {
                    //稿袋号
                    int num = 0;
                    string gaodaihao = "";
                    if (isShowGdh)
                    {
                        foreach (DataRow row in dt_gdh_cpmc.Rows)
                        {
                            if (IsEachContain(Path.GetFileNameWithoutExtension(fileName), row["产品名称"].ToString())
                                && DateTime.Parse(row["Excel时间"].ToString()).AddDays(3) > DateTime.Now)
                            {
                                if (!gaodaihao.Equals(row["稿袋号"].ToString(), StringComparison.OrdinalIgnoreCase))
                                {
                                    num++;
                                    gaodaihao = row["稿袋号"].ToString();
                                }
                                if (num >= 2)
                                {
                                    break;
                                }

                            }
                        }
                    }
                    sb.AppendLine(Path.GetFileNameWithoutExtension(fileName) + "</br>");
                    if (num == 1 && !string.IsNullOrWhiteSpace(gaodaihao))
                    {
                        sb.AppendLine("(" + gaodaihao + ")</br>");
                    }
                }
                dr["文件名"] = sb.ToString();

                dr["板材咬口"] = "板材：" + proInfo.Plant + "<br/>咬口：" + proInfo.OffsetY + "<br/>";

                sb.Clear();
                foreach (String color in proInfo.ColorList)
                {
                    sb.AppendLine(color + "</br>");
                }
                dr["颜色"] = sb.ToString();
                dr["加网信息"] = "线数：" + proInfo.RulingOrFeatureSize
                    + "</br>网点：" + proInfo.DotShape
                    + "</br>曲线：" + proInfo.CalibrationTarget;
                gridVies_dt.Rows.Add(dr);
            }
            this.GridViewHistorical.Visible = true;
            this.GridViewHistorical.DataSource = gridVies_dt;
            this.GridViewHistorical.DataBind();
        }
        else
        {
            this.GridViewHistorical.Visible = false;
        }


        //*****当前出版记录
        //加载所有的完成出版的记录
        List<EvoProcessInfo> dynProcessList = new List<EvoProcessInfo>();

        foreach (String fileFullName in EvoProcess.Get_dynamic_data_Files_All())
        {
            EvoProcessInfo process = new EvoProcessInfo(fileFullName);
            if (process.FileList.Count > 0)
            {
                //判断GUID是否已经存在,并确定整个的完成时间
                bool cunzai = false;
                int index = -1;
                for (int i = 0; i < dynProcessList.Count; i++)
                {
                    if (process.Guid == dynProcessList[i].Guid)
                    {
                        cunzai = true;
                        index = i;
                        break;
                    }
                }

                if (cunzai)
                {
                    if (dynProcessList[index].CompletionTime.CompareTo(process.CompletionTime) < 0)
                    {
                        dynProcessList[index].CompletionTime = process.CompletionTime;
                    }
                }
                else
                {
                    dynProcessList.Add(process);
                }

            }

        }

        //************

        if (dynProcessList != null && dynProcessList.Count > 0)
        {
            //需要绑定到GridView中的DataTable
            DataTable gridVies_dt = new DataTable();
            gridVies_dt.Columns.Add("文件名");
            gridVies_dt.Columns.Add("板材咬口");
            gridVies_dt.Columns.Add("颜色");
            gridVies_dt.Columns.Add("加网信息");


            //按照完成时间排序(晚的时间靠前)
            dynProcessList.Sort(CompareProcessByCompletionTime);


            //遍历整个整理好的列表,并且将数据添加到表格中
            foreach (EvoProcessInfo proInfo in dynProcessList)
            {
                DataRow dr = gridVies_dt.NewRow();
                StringBuilder sb = new StringBuilder();
                foreach (String fileName in proInfo.FileList)
                {
                    //稿袋号
                    int num = 0;
                    string gaodaihao = "";
                    if (isShowGdh)
                    {
                        foreach (DataRow row in dt_gdh_cpmc.Rows)
                        {
                            if (IsEachContain(Path.GetFileNameWithoutExtension(fileName), row["产品名称"].ToString())
                                && DateTime.Parse(row["Excel时间"].ToString()).AddDays(3) > DateTime.Now)
                            {
                                if (!gaodaihao.Equals(row["稿袋号"].ToString(), StringComparison.OrdinalIgnoreCase))
                                {
                                    num++;
                                    gaodaihao = row["稿袋号"].ToString();
                                }
                                if (num >= 2)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    sb.Append(Path.GetFileNameWithoutExtension(fileName) + "</br>");
                    if (num == 1 && !string.IsNullOrWhiteSpace(gaodaihao))
                    {
                        sb.AppendLine("(" + gaodaihao + ")</br>");
                    }

                }
                dr["文件名"] = sb.ToString();

                dr["板材咬口"] = "板材：" + proInfo.Plant + "</br>咬口：" + proInfo.OffsetY;

                sb.Clear();
                foreach (String color in proInfo.ColorList)
                {
                    sb.AppendLine(color + "</br>");
                }
                dr["颜色"] = sb.ToString();
                dr["加网信息"] = "线数：" + proInfo.RulingOrFeatureSize
                    + "</br>网点：" + proInfo.DotShape
                    + "</br>曲线：" + proInfo.CalibrationTarget;
                gridVies_dt.Rows.Add(dr);
            }
            this.GridViewDynamic.Visible = true;
            this.GridViewDynamic.DataSource = gridVies_dt;
            this.GridViewDynamic.DataBind();
        }
        else
        {
            this.GridViewDynamic.Visible = false;
        }
        try
        {
            //***错误信息
            if (true)
            {
                DataTable dt_error = new DataTable();
                dt_error.Columns.Add("文件名");
                dt_error.Columns.Add("错误");
                //[ID],[Name],[PlantSize],[Bite],[MaxPaper],[MaxPrinting],[MinPaper]	
                List<PrintingMachineInfo> pmList = new List<PrintingMachineInfo>();
                foreach (DataRow row in
                    SQLiteDbHelper.ExecuteDataTable("SELECT*FROM[PrintingMachine]").Rows)
                {
                    pmList.Add(new PrintingMachineInfo(row));
                }
                List<EvoProcessInfo> allList = new List<EvoProcessInfo>();
                allList.AddRange(hisProcessList);
                allList.AddRange(dynProcessList);
                foreach (EvoProcessInfo pro in allList)
                {
                    PrintingMachineInfo pmInfo = pmList.Find(p => p.PlantSize_L == pro.Plant_L && p.PlantSize_S == pro.Plant_S);
                    if (pmInfo == null)
                    {
                        continue;
                    }
                    foreach (string fileName in pro.FileList)
                    {
                        StringBuilder sb = new StringBuilder();
                        //***咬口错误 
                        if (pro.OffsetY > pmInfo.Bite || pro.OffsetY < 10)
                        {
                            sb.Append("咬口错误<br />");
                        }
                        //***最大过纸
                        //***最大印刷尺寸
                        if (sb.Length == 0)
                        {
                            if (pro.ImagingPosition != null)
                            {
                                CREO_TrimBox_MilliMetre imaging = pro.ImagingPosition.GetCREO_TrimBox_MilliMetre();
                                if (pmInfo.MaxPrinting_S + pmInfo.Bite < imaging.High.Length + pro.OffsetY - 10
                                    || imaging.Left.Length < 0)
                                {
                                    sb.Append("超出最大印刷面积<br />");
                                }
                            }
                        }

                        //***最小过纸
                        //***重复提交
                        if (sb.Length == 0)
                        {
                            bool isRepeat = false;
                            foreach (EvoProcessInfo pro_child in allList)
                            {
                                if (pro == pro_child)
                                {
                                    continue;
                                }
                                if (pro_child.Equals(pro))
                                {
                                    isRepeat = true;
                                    break;
                                }
                            }
                            if (isRepeat)
                            {
                                sb.Append("重复提交<br />");
                            }
                        }
                        //***汇总信息
                        if (sb.Length > 0)
                        {
                            DataRow row = dt_error.NewRow();
                            row["文件名"] = Path.GetFileNameWithoutExtension(fileName);
                            row["错误"] = sb.ToString();
                            dt_error.Rows.Add(row);
                        }
                    }
                }
                if (dt_error.Rows.Count > 0)
                {
                    this.GridViewError.Visible = true;
                    this.LableError.Visible = true;
                    this.GridViewError.DataSource = dt_error;
                    this.GridViewError.DataBind();
                }
                else
                {
                    this.GridViewError.Visible = false;
                    this.LableError.Visible = false;
                }
            }
            //else
            //{
            //    this.GridViewError.Visible = false;
            //    this.LableError.Visible = false;
            //}
        }
        catch
        {
        }
    }
}