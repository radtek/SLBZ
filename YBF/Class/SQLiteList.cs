using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandeJobManager.DAL
{
   public static class SQLiteList
    {
       /// <summary>
       /// 主数据
       /// </summary>
       public static SQLiteDbHelper YbfSQLite;
       /// <summary>
       /// 副数据库列表
       /// (主要用于在需要的时候读取)
       /// </summary>
       public static List<SQLiteDbHelper> ViceSQliteList;
       /// <summary>
       /// 备份的数据库列表
       /// (主要用于在需要的时候读取)
       /// </summary>
       public static List<SQLiteDbHelper> BackupSQliteList;
    }
}
