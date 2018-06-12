using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Data.OleDb;

/// <summary>
///Comm_StaticVariables 的摘要说明
/// </summary>
public static class PublishJobTable
{
    public static int num = 0;

    public static DataTable PublishJobTable_Excel;

    public static void GetAllPublishedJobTableByHour()
    {
        int hours = 30;
        //判断表格的列是否为空
        if (PublishJobTable_Excel == null)
        {
            PublishJobTable_Excel = new DataTable();
        }
        ////收集Excel文件名
        //List<string> excelList = new List<string>();
        ////判断Excel文件名和时间的列是否存在
        //if (PublishJobTable_Excel.Columns["Excel文件"]!=null&&PublishJobTable_Excel.Columns["Excel时间"]!=null)
        //{

        //}

        //检索Excel文件
        FileInfo[] files = new DirectoryInfo(@"\\128.1.30.112\Downloads").GetFiles("*.xls");
        foreach (FileInfo file in files)
        {
            //判断文件名是否存在
            string excelFileName = Path.GetFileNameWithoutExtension(file.FullName);
            bool isExists = false;
            foreach (DataRow row in PublishJobTable_Excel.Rows)
            {
                string rowExcelFile = row["Excel文件"].ToString();
                if (excelFileName.IndexOf(rowExcelFile
                    , StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    isExists = true;
                    break;
                }

            }
            if (isExists)
            {
                continue;
            }
            //指定时间内的Excel文件
            if (file.LastWriteTime.AddHours(hours) >= DateTime.Now)
            {
                //读取Excel里面的作业信息，并且合并到同一个Table里面
                DataTable dt_excel = GetPublishDataTableFromExcelFile(file.FullName);
                if (dt_excel != null)
                {
                    PublishJobTable_Excel.Merge(dt_excel);
                }
            }
        }
        //删除超过日期的信息
        List<DataRow> drList = new List<DataRow>();
        foreach (DataRow row in PublishJobTable_Excel.Rows)
        {
            DateTime dt_row = DateTime.Parse(row["Excel时间"].ToString());
            if (dt_row.AddHours(hours + 72) < DateTime.Now)
            {
                drList.Add(row);
            }
        }
        foreach (DataRow row in drList)
        {
            PublishJobTable_Excel.Rows.Remove(row);
        }
    }



    public static void GetAllPublishExcel()
    {
        //检索Excel文件
        FileInfo[] files = new DirectoryInfo(@"\\128.1.30.112\Downloads").GetFiles("*.xls");
        DataTable excelTable = SQLiteDbHelper.ExecuteDataTable("select Excel文件 from job group by excel文件");
        foreach (FileInfo file in files)
        {
            //判断Excel文件名是否存在
            string excelFileName = Path.GetFileNameWithoutExtension(file.FullName);
            DataRow[] rows = excelTable.Select("[Excel文件]='" + excelFileName+"'");
            //如果不存在，则添加
            if (rows == null || rows.Length == 0)
            {
                //读取Excel里面的作业信息，并返回一个DataTable
                DataTable dt_excel = GetPublishDataTableFromExcelFile(file.FullName);
                if (dt_excel != null)
                {
                    //添加
                    JobService.Add(dt_excel);
                }
            }
        }
    }


    //根据excle的路径把第一个sheel中的内容放入datatable
    private static DataTable ReadExcelToTable(string path)//excel存放的路径
    {
        //Office2003（Microsoft.Jet.Oledb.4.0）
        //string strConn = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'", strExcelFileName);
        //Office2007（Microsoft.ACE.OLEDB.12.0）
        //string strConn = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'", strExcelFileName);
        //Office2010（Microsoft.ACE.OLEDB.12.0）
        //string strConn = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'", strExcelFileName);
        //还要一个就是“HDR=Yes”这个问题，如果HDR的值是Yes,那么第一行是被当做列名的，不会被导入。只有为“No”才可以被当做数据导入。

        //连接字符串
        //string connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';"; // Office 07及以上版本 不能出现多余的空格 而且分号注意
        try
        {
            string connstring = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=NO;IMEX=1;'", path);
            using (OleDbConnection conn = new OleDbConnection(connstring))
            {
                conn.Open();
                DataTable sheetsName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" }); //得到所有sheet的名字
                string firstSheetName = sheetsName.Rows[0][2].ToString(); //得到第一个sheet的名字
                string sql = string.Format("SELECT * FROM [{0}]", firstSheetName); //查询字符串
                //string sql = string.Format("SELECT * FROM [{0}] WHERE [日期] is not null", firstSheetName); //查询字符串

                OleDbDataAdapter ada = new OleDbDataAdapter(sql, connstring);
                DataSet set = new DataSet();
                ada.Fill(set);
                set.Tables[0].TableName = Path.GetFileNameWithoutExtension(path);
                return set.Tables[0];
            }
        }
        catch 
        {
            return null;
        }


    }

    private static DataTable GetPublishDataTableFromExcelFile(string excelFileFullName)
    {
        DataTable dt = ReadExcelToTable(excelFileFullName);
        if (dt == null || dt.Rows.Count == 0)
        {
            try
            {
                if (new FileInfo(excelFileFullName).Length < 5 * 1024)
                {
                    File.Delete(excelFileFullName);
                }
            }
            catch
            {
            }
            return null;
        }
        //****删除空白行******
        for (int i = dt.Rows.Count - 1; i >= 0; i--)
        {
            bool isEmptyRow = true;
            foreach (DataColumn dc in dt.Columns)
            {
                object obj = dt.Rows[i][dc];
                if (obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
                {
                    isEmptyRow = false; break;
                }
            }
            if (isEmptyRow)
            {
                dt.Rows.RemoveAt(i);
            }
            else
            {
                //***删除有“打印人”、“印版房”、“打印日期”的列，基本是最后一列了

                string rowText = "";
                foreach (DataColumn dc in dt.Columns)
                {
                    rowText += dt.Rows[i][dc];
                }
                if (rowText.Contains("打印人")
                    && rowText.Contains("印版房")
                    && rowText.Contains("打印日期"))
                {
                    dt.Rows.RemoveAt(i);
                }
            }
        }
        //****删除空白列******
        for (int i = dt.Columns.Count - 1; i >= 0; i--)
        {
            bool isEmptyColumn = true;
            foreach (DataRow dr in dt.Rows)
            {
                object obj = dr[i];
                if (obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
                {
                    isEmptyColumn = false; break;
                }
            }
            if (isEmptyColumn)
            {
                dt.Columns.RemoveAt(i);
            }
        }



        //dt.Rows.RemoveAt(0);//删除首行
        //dt.Columns.RemoveAt(0);//删除首列
        //dt.Columns.RemoveAt(dt.Columns.Count - 1);//删除末列
        //for (int i = 0; i < 2; i++)//删除最后2行
        //{
        //    dt.Rows.RemoveAt(dt.Rows.Count - 1);
        //}
        //设置列名
        for (int i = 0; i < dt.Columns.Count; i++)
        {
            object obj = dt.Rows[0][i];
            if (string.IsNullOrWhiteSpace(obj.ToString()))
            {
                dt.Columns[i].ColumnName = "色数2";
            }
            else
            {
                dt.Columns[i].ColumnName = obj.ToString();
            }
            if (dt.Columns[i].ColumnName == "色数")
            {
                dt.Columns[i].ColumnName = "色数1";
            }
        }
        ////将列名改为小写字母
        //foreach (DataColumn col in dt.Columns)
        //{
        //    col.ColumnName = PinYinConverter.GetFirst(col.ColumnName).ToLower();
        //}
        //删除首行（就是把列名的那行删除了）
        dt.Rows.RemoveAt(0);

        //添加Excel文件名和时间
        string[] columns = { "Excel文件", "Excel时间" };
        foreach (string col in columns)
        {
            if (!dt.Columns.Contains(col))
            {
                dt.Columns.Add(col);
            }
        }
        DateTime excelTime = File.GetLastWriteTime(excelFileFullName);
        foreach (DataRow row in dt.Rows)
        {
            row["Excel文件"] = dt.TableName;
            row["Excel时间"] = excelTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        return dt;
    }


    /// <summary>
    /// 读取Excel中数据
    /// </summary>
    /// <param name="strExcelPath"></param>
    /// <param name="tableName"></param>
    /// <returns></returns>
    private static DataTable GetExcelTableByOleDB(string strExcelPath, string tableName)
    {
        try
        {
            DataTable dtExcel = new DataTable();
            //数据表
            DataSet ds = new DataSet();
            //获取文件扩展名
            string strExtension = System.IO.Path.GetExtension(strExcelPath);
            string strFileName = System.IO.Path.GetFileName(strExcelPath);
            //Excel的连接
            OleDbConnection objConn = null;
            switch (strExtension)
            {
                case ".xls":
                    objConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strExcelPath + ";" + "Extended Properties=\"Excel 8.0;HDR=NO;IMEX=1;\"");
                    break;
                case ".xlsx":
                    objConn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strExcelPath + ";" + "Extended Properties=\"Excel 12.0;HDR=NO;IMEX=1;\"");
                    break;
                default:
                    objConn = null;
                    break;
            }
            if (objConn == null)
            {
                return null;
            }
            objConn.Open();
            //获取Excel中所有Sheet表的信息
            //System.Data.DataTable schemaTable = objConn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
            //获取Excel的第一个Sheet表名
            // string tableName1 = schemaTable.Rows[0][2].ToString().Trim();
            string strSql = "select * from [$" + tableName + "]";
            //获取Excel指定Sheet表中的信息
            OleDbCommand objCmd = new OleDbCommand(strSql, objConn);
            OleDbDataAdapter myData = new OleDbDataAdapter(strSql, objConn);
            myData.Fill(ds, tableName);//填充数据
            objConn.Close();
            //dtExcel即为excel文件中指定表中存储的信息
            dtExcel = ds.Tables[tableName];
            return dtExcel;
        }
        catch
        {
            return null;
        }

    }
}