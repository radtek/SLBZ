using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

public partial class GaoDaiHao : System.Web.UI.Page
{
    private string sqlSelect="SELECT[稿袋号],[上机机台],[客户简称],[产品名称],[制造尺寸],[面纸尺寸],[色数1],[色数2],[晒版数],[备注],[Excel文件]"
        +"FROM [Job]";
    protected void Page_Load(object sender, EventArgs e)
    {
        PublishJobTable.GetPublishedJobTable_All();
        this.DgvGdh.DataSource = SQLiteDbHelper.ExecuteDataTable(
            sqlSelect+"	ORDER BY [Excel时间] DESC LIMIT 300");
        this.DgvGdh.DataBind();
    }
    protected void ButtonSearch_Click(object sender, EventArgs e)
    {
        PublishJobTable.GetPublishedJobTable_All();
        string searchTxt = this.TextBoxSearch.Text.Trim();
        if (!string.IsNullOrWhiteSpace(searchTxt))
        {
        //    StringBuilder sb = new StringBuilder();
        //    DataTable tempDt = SQLiteDbHelper.ExecuteDataTable("SELECT [ID],[稿袋号],[客户简称],[产品名称]FROM[Job]");
        //    foreach (DataRow row in tempDt.Select(string.Format("[客户简称] LIKE '%{0}%' OR[产品名称] LIKE '%{0}%'OR[稿袋号] LIKE '%{0}%'", searchTxt.Trim())))
        //    {
        //        sb.Append(row["ID"].ToString() + ",");
        //    }
        //    sb.Append(0);
        //    this.DgvGdh.DataSource = SQLiteDbHelper.ExecuteDataTable(
        //    sqlSelect + "WHERE[ID] IN (" + sb.ToString() + ")");
            this.DgvGdh.DataSource = SQLiteDbHelper.ExecuteDataTable(
            string.Format(sqlSelect+" WHERE[客户简称] LIKE '%{0}%' OR[产品名称] LIKE '%{0}%' "
            + "OR[稿袋号] LIKE '%{0}%' ORDER BY [Excel时间] DESC LIMIT 300", searchTxt.Trim()));
            this.DgvGdh.DataBind();
        }
        

    }
    
}