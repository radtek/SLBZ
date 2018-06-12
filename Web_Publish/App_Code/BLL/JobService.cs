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
        SQLiteDbHelper.ExecuteNonQuery(
         "   CREATE TABLE IF NOT EXISTS [Job] ("
    + "[ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT, "
    + "[稿袋号] nvarchar(254) NOT NULL DEFAULT '', "
    + "[上机机台] nvarchar(254) NOT NULL DEFAULT '', "
    + "[客户简称] nvarchar(254) NOT NULL DEFAULT '', "
    + "[产品名称] nvarchar(254) NOT NULL DEFAULT '', "
    + "[制造尺寸] nvarchar(254) NOT NULL DEFAULT '', "
    + "[面纸尺寸] nvarchar(254) NOT NULL DEFAULT '', "
    + "[色数1] nvarchar(254) NOT NULL DEFAULT '', "
    + "[色数2] nvarchar(254) NOT NULL DEFAULT '', "
    + "[晒版数] nvarchar(254) NOT NULL DEFAULT '', "
    + "[备注] nvarchar(254) NOT NULL DEFAULT '', "
    + "[咬口] nvarchar(254) NOT NULL DEFAULT '', "
    + "[Excel文件] nvarchar(254) NOT NULL DEFAULT '', "
    + "[Excel时间] nvarchar(254) NOT NULL DEFAULT '');");
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
       return  SQLiteDbHelper.ExecuteNonQuery(
             string.Format("INSERT INTO [Job]\n({0})\nVALUES\n{1}"
             , fields_sb.ToString(), values_sb.ToString().Trim().TrimEnd(','))) > 0;

      
    }
}