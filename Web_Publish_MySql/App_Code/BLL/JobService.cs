using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;

/// <summary>
/// 作业的数据操作方法
/// </summary>
public class JobService
{
    private static void IsNotExistCreate()
    {
        MySqlDbHelper.ExecuteNonQuery(
        " CREATE TABLE `PublishJob`.`job` (" +
" `ID` bigint(20) NOT NULL AUTO_INCREMENT," +
 " `出版` bit(1) NOT NULL," +
 " `稿袋号` varchar(10) NOT NULL," +
 " `上机机台` varchar(20) NOT NULL," +
 " `客户简称` varchar(20) NOT NULL," +
  "`产品名称` varchar(200) NOT NULL," +
"  `制造尺寸` varchar(30) NOT NULL," +
 " `面纸尺寸` varchar(20) NOT NULL," +
 " `色数1` varchar(10) NOT NULL," +
 " `色数2` varchar(10) NOT NULL," +
 " `晒版数` varchar(10) NOT NULL," +
 " `备注` varchar(254) NOT NULL," +
 " `咬口` varchar(10) NOT NULL," +
 " `Excel文件` varchar(100) NOT NULL," +
 " `Excel时间` datetime NOT NULL," +
 " PRIMARY KEY (`ID`)" +
") ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;");
    }

    public static bool Add(DataTable dt)
    {
        if (dt==null||dt.Rows.Count==0||dt.Columns.Count==0)
        {
            return false;
        }

        //INSERT INTO [Job]()VALUES();
        StringBuilder fields_sb = new StringBuilder();
        StringBuilder values_sb = new StringBuilder();
        StringBuilder value_sb = new StringBuilder();

        //先确定字段
        foreach (DataColumn col in dt.Columns)
        {
            fields_sb.Append(col.ColumnName + ",");
        }
        fields_sb.Remove(fields_sb.Length-1,1);
        //确定值
        foreach (DataRow row in dt.Rows)
        {
            value_sb.Clear();
            foreach (DataColumn col in dt.Columns)
            {
                value_sb.Append("'"+row[col].ToString() + "',");
            }
            value_sb.Remove(value_sb.Length - 1, 1);
            values_sb.AppendLine("("+value_sb+"),");
        }
        return MySqlDbHelper.ExecuteNonQuery(
             string.Format("INSERT INTO `Job`\n({0})\nVALUES\n{1}"
             , fields_sb.ToString(), values_sb.ToString().Trim().TrimEnd(','))) > 0;

      
    }
}