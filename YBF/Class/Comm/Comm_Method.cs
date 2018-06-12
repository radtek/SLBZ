using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using excelToTable_NPOI;

namespace YBF.Class.Comm
{
    //常用的通用类
    public static class Comm_Method
    {
        public static DataTable Table_Excel = new DataTable();
        private static DateTime LastTime_Excel = DateTime.MinValue;
        public static List<string> PdfFileList = new List<string>();
        public static void Init_Tabel_Excel()
        {
            string ExcelFile = "E:\\SOFTWARE\\CTP输出记录表.xls";
            // string ExcelFile = "E:\\SOFTWARE\\CTP输出记录表000.xlsx";
            DateTime ExcelFileLastTime = File.GetLastWriteTime(ExcelFile);
            if (ExcelFileLastTime > LastTime_Excel)
            {
                Table_Excel = new ExcelHelper(ExcelFile)
                .ExcelToDataTable(null, true, true);
                LastTime_Excel = ExcelFileLastTime;
            }
        }
        public static void Init_PdfFileList()
        {
            PdfFileList.Clear();
            string[] OldFilePaths = { "F:\\CTP己输出2009-2014"
                                        , "G:\\CTP己输出2015备份" 
                                    ,@"\\128.1.30.144\JobData\pdf\已下单PDF\PDF-ok"};
            foreach (string path in OldFilePaths)
            {
                PdfFileList.AddRange(Directory.GetFiles(path, "*.pdf", SearchOption.AllDirectories));
            }
            PdfFileList.AddRange(Directory.GetFiles(@"\\128.1.30.144\JobData\pdf\已下单PDF", "*.pdf"));
            
        }


        /// <summary>
        /// 显示错误弹窗
        /// </summary>
        /// <param name="mess"></param>
        public static void ShowErrorMessage(string mess)
        {
            MessageBox.Show(mess, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //根据excle的路径把第一个sheel中的内容放入datatable
        public static DataTable ReadExcelToTable(string path)//excel存放的路径
        {
            try
            {

                //连接字符串
                string connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';"; // Office 07及以上版本 不能出现多余的空格 而且分号注意
                //string connstring = Provider=Microsoft.JET.OLEDB.4.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';"; //Office 07以下版本 
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
            catch (Exception)
            {
                return null;
            }

        }

        public static DataTable GetPublishDataTableFromExcelFile(string excelFileFullName)
        {
            DataTable dt = Comm_Method.ReadExcelToTable(excelFileFullName);
            if (dt == null || dt.Rows.Count == 0)
            {
                return dt;
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
            //删除第一列和最后一列，因为是空白的
            dt.Columns.RemoveAt(0);
            dt.Columns.RemoveAt(dt.Columns.Count - 1);

            ////****删除空白列******
            //for (int i = dt.Columns.Count-1; i >= 0; i--)
            //{
            //    bool isEmptyColumn = true;
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        object obj = dr[i];
            //        if (obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
            //        {
            //            isEmptyColumn = false; break;
            //        }
            //    }
            //    if (isEmptyColumn)
            //    {
            //        dt.Columns.RemoveAt(i);
            //    }
            //}



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

            return dt;
        }


        /// <summary>
        /// 读取Excel中数据
        /// </summary>
        /// <param name="strExcelPath"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable GetExcelTableByOleDB(string strExcelPath, string tableName)
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }


        }

        #region 全角转换半角以及半角转换为全角
        /// <summary>
        /// 转全角的函数(SBC case)
        ///<para>全角空格为12288，半角空格为32</para>
        ///<para>其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248</para>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSBC(string input)
        {
            // 半角转全角：
            char[] array = input.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 32)
                {
                    array[i] = (char)12288;
                    continue;
                }
                if (array[i] < 127)
                {
                    array[i] = (char)(array[i] + 65248);
                }
            }
            return new string(array);
        }

        /// <summary>
        ///转半角的函数(DBC case)
        ///<para>全角空格为12288，半角空格为32</para>
        ///<para>其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248</para>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToDBC(string input)
        {
            char[] array = input.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 12288)
                {
                    array[i] = (char)32;
                    continue;
                }
                if (array[i] > 65280 && array[i] < 65375)
                {
                    array[i] = (char)(array[i] - 65248);
                }
            }
            return new string(array);
        }
        #endregion
    }
}
