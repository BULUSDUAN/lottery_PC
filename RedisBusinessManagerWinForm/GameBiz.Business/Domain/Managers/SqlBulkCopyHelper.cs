using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace GameBiz.Business.Domain.Managers
{
    /// <summary>
    /// 批量执行sql
    /// </summary>
    public class SqlBulkCopyHelper
    {
        public static int NotifyCount = 0;
        public static int TableCount = 0;
        public static int NotifyAfterCount
        {
            get { return 5000; }
        }
        public static void WriteTableToDataBase(DataTable table, SqlConnection conn, SqlRowsCopiedEventHandler eventHandler = null)
        {
            using (var bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.SqlRowsCopied += (obj, arg) =>
                {
                    SqlBulkCopyHelper.NotifyCount++;
                };
                if (eventHandler != null)
                {
                    bulkCopy.SqlRowsCopied += eventHandler;
                }
                bulkCopy.NotifyAfter = SqlBulkCopyHelper.NotifyAfterCount;
                bulkCopy.BatchSize = 20000;
                bulkCopy.DestinationTableName = table.TableName;

                bulkCopy.WriteToServer(table);
            }
            TableCount++;
        }

        public static void WriteTableToDataBase(DataTable table, SqlConnection conn, SqlRowsCopiedEventHandler eventHandler = null, params string[] columns)
        {
            using (var bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.SqlRowsCopied += (obj, arg) =>
                {
                    SqlBulkCopyHelper.NotifyCount++;
                };
                if (eventHandler != null)
                {
                    bulkCopy.SqlRowsCopied += eventHandler;
                }
                bulkCopy.NotifyAfter = SqlBulkCopyHelper.NotifyAfterCount;
                bulkCopy.BatchSize = 20000;
                bulkCopy.DestinationTableName = table.TableName;

                //通过列映射，可以使外部的table顺序任意变化
                foreach (var item in columns)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    bulkCopy.ColumnMappings.Add(item, item);
                }

                bulkCopy.WriteToServer(table, DataRowState.Added);
            }
            TableCount++;
        }

        public static void UpdateTable(DataTable dt, string strTblName, SqlConnection conn)
        {
            SqlDataAdapter myAdapter = new SqlDataAdapter();
            SqlCommand myCommand = new SqlCommand("select * from " + strTblName, conn);
            myAdapter.SelectCommand = myCommand;
            SqlCommandBuilder myCommandBuilder = new SqlCommandBuilder(myAdapter);
            myAdapter.Update(dt);
        }

        public static void WriteTableToDataBase(DataTable table, string conn, SqlRowsCopiedEventHandler eventHandler = null)
        {
            using (var bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.SqlRowsCopied += (obj, arg) =>
                {
                    SqlBulkCopyHelper.NotifyCount++;
                };
                if (eventHandler != null)
                {
                    bulkCopy.SqlRowsCopied += eventHandler;
                }
                bulkCopy.NotifyAfter = SqlBulkCopyHelper.NotifyAfterCount;
                bulkCopy.BatchSize = 20000;
                bulkCopy.DestinationTableName = table.TableName;
                bulkCopy.WriteToServer(table);
            }
            TableCount++;
        }
        public static void WriteTableToDataBaseByReader(IDataReader reader, string tableName, string conn, SqlRowsCopiedEventHandler eventHandler = null, Dictionary<string, string> mapping = null)
        {
            Console.WriteLine("开始写数据库");
            using (var bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.BulkCopyTimeout = 300;
                bulkCopy.SqlRowsCopied += (obj, arg) =>
                {
                    SqlBulkCopyHelper.NotifyCount++;
                };
                if (eventHandler != null)
                {
                    bulkCopy.SqlRowsCopied += eventHandler;
                }
                bulkCopy.NotifyAfter = SqlBulkCopyHelper.NotifyAfterCount;
                bulkCopy.BatchSize = 20000;
                bulkCopy.DestinationTableName = tableName;
                if (mapping != null)
                {
                    foreach (var item in mapping)
                    {
                        bulkCopy.ColumnMappings.Add(item.Key, item.Value);
                    }
                }

                bulkCopy.WriteToServer(reader);
            }
            TableCount++;
            Console.WriteLine("写数据库结束");
        }
    }
}
