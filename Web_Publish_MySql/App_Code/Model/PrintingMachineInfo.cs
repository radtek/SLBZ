using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
///印刷机信息的实例类
/// </summary>
public class PrintingMachineInfo
{
    
    /// <summary>
    /// ID
    /// </summary>
    public int ID { get; set; }
    /// <summary>
    /// 印刷机名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 板材的长边
    /// </summary>
    public int PlantSize_L { get; set; }
    /// <summary>
    /// 板材的短边
    /// </summary>
    public int PlantSize_S { get; set; }
    /// <summary>
    /// 正常咬口
    /// </summary>
    public int Bite { get; set; }
    /// <summary>
    /// 最大过纸尺寸的长边
    /// </summary>
    public int MaxPaper_L { get; set; }
    /// <summary>
    /// 最大过纸尺寸的短边
    /// </summary>
    public int MaxPaper_S { get; set; }
    /// <summary>
    /// 最大印刷尺寸的长边
    /// </summary>
    public int MaxPrinting_L { get; set; }
    /// <summary>
    /// 最大印刷尺寸的短边
    /// </summary>
    public int MaxPrinting_S { get; set; }
    /// <summary>
    /// 最小过纸尺寸的长边
    /// </summary>
    public int MinPaper_L { get; set; }
    /// <summary>
    /// 最小过纸尺寸的短边
    /// </summary>
    public int MinPaper_S{ get; set; }

	public PrintingMachineInfo()
	{
        //[ID],[Name],[PlantSize],[Bite],[MaxPaper],[MaxPrinting],[MinPaper]	
	}
    public PrintingMachineInfo(DataRow row)
    {
        //[ID],[Name],[PlantSize],[Bite],[MaxPaper],[MaxPrinting],[MinPaper]	
        this.ID = Convert.ToInt32(row["ID"]);
        this.Name = row["Name"].ToString();
        this.Bite = Convert.ToInt32(row["Bite"]);
        int[] strArray = SplitString(row["PlantSize"].ToString());
        this.PlantSize_L = strArray[0];
        this.PlantSize_S = strArray[1];
        strArray = SplitString(row["MaxPaper"].ToString());
        this.MaxPaper_L = strArray[0];
        this.MaxPaper_S = strArray[1];
        strArray = SplitString(row["MaxPrinting"].ToString());
        this.MaxPrinting_L = strArray[0];
        this.MaxPrinting_S = strArray[1];
        strArray = SplitString(row["MinPaper"].ToString());
        this.MinPaper_L = strArray[0];
        this.MinPaper_S = strArray[1];
    }

    private int[] SplitString(string str)
    {
        int[] returnArray = new int[2];
        string[] plantArray = str.Split('*');
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
            returnArray[0] = side_L;
            returnArray[1] = side_S;
        }
        return returnArray;
    }
}