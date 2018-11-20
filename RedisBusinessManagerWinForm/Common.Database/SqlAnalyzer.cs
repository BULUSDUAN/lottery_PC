using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Common.Database.DbAccess;

namespace Common.Database
{
    /// <summary>
    /// sql解析器
    /// </summary>
    public static class SqlAnalyzer
    {
        public static void ForeachSQLFromFile(string fileName, IDbAccess dbAccess, string separator, Action beginAction = null, Action finishAction = null, Action<string> beginEachAction = null, Action<string> finishEachAction = null)
        {
            ForeachSQLFromStream(File.OpenRead(fileName), dbAccess
                , (n) =>
                {
                    string baseDir = Path.GetDirectoryName(fileName);
                    n = Path.Combine(baseDir, n);
                    return File.OpenRead(n);
                }
                , beginAction, finishAction, beginEachAction, finishEachAction);
        }
        public static void ForeachSQLFromFile(string fileName, IDbAccess dbAccess, Action beginAction = null, Action finishAction = null, Action<string> beginEachAction = null, Action<string> finishEachAction = null)
        {
            string separator = "go";
            ForeachSQLFromFile(fileName, dbAccess, separator, beginAction, finishAction);
        }
        public static void ForeachSQLFromStream(Stream stream, IDbAccess dbAccess, string separator, Func<string, Stream> getStreamByNameHandler = null, Action beginAction = null, Action finishAction = null, Action<string> beginEachAction = null, Action<string> finishEachAction = null)
        {
            if (beginAction != null)
            {
                beginAction();
            }
            using (StreamReader sr = new StreamReader(stream, Encoding.Default))
            {
                string line = sr.ReadLine();
                StringBuilder sbuilder = new StringBuilder();
                while (line != null)
                {
                    string sql = line.Trim();
                    if (sql.Length > 0)
                    {
                        if (sql.StartsWith("#split<", StringComparison.OrdinalIgnoreCase)
                            && sql.EndsWith(">"))
                        {
                            DoActionSql(ref sbuilder, dbAccess, beginEachAction, finishEachAction);
                            separator = sql.Substring("#split<".Length, sql.Length - "#split<".Length - 1);
                        }
                        else if (sql.StartsWith("#include<", StringComparison.OrdinalIgnoreCase)
                            && sql.EndsWith(">"))
                        {
                            DoActionSql(ref sbuilder, dbAccess, beginEachAction, finishEachAction);
                            string subFileName = sql.Substring("#include<".Length, sql.Length - "#include<".Length - 1);
                            var subStream = getStreamByNameHandler(subFileName);
                            ForeachSQLFromStream(subStream, dbAccess, getStreamByNameHandler, beginAction, finishAction, beginEachAction, finishEachAction);
                        }
                        else
                        {
                            if (separator.Equals("\\N", StringComparison.OrdinalIgnoreCase))
                            {
                                sbuilder.AppendLine(line);
                                DoActionSql(ref sbuilder, dbAccess, beginEachAction, finishEachAction);
                            }
                            else
                            {
                                if (!sql.Equals(separator, StringComparison.OrdinalIgnoreCase))
                                {
                                    sbuilder.AppendLine(line);
                                }
                                else
                                {
                                    DoActionSql(ref sbuilder, dbAccess, beginEachAction, finishEachAction);
                                }
                            }
                        }
                    }
                    line = sr.ReadLine();
                }
                DoActionSql(ref sbuilder, dbAccess, beginEachAction, finishEachAction);
            }
            if (finishAction != null)
            {
                finishAction();
            }
        }
        public static void ForeachSQLFromStream(Stream stream, IDbAccess dbAccess, Func<string, Stream> getStreamByNameHandler = null, Action beginAction = null, Action finishAction = null, Action<string> beginEachAction = null, Action<string> finishEachAction = null)
        {
            string separator = "go";
            ForeachSQLFromStream(stream, dbAccess, separator, getStreamByNameHandler, beginAction, finishAction, beginEachAction, finishEachAction);
        }

        private static void DoActionSql(ref StringBuilder sqlBuilder, IDbAccess dbAccess, Action<string> beginAction, Action<string> finishAction)
        {
            if (sqlBuilder.ToString().Length > 0)
            {
                try
                {
                    if (beginAction != null)
                    {
                        beginAction(sqlBuilder.ToString());
                    }
                    dbAccess.ExecSQL(sqlBuilder.ToString());
                    if (finishAction != null)
                    {
                        finishAction(sqlBuilder.ToString());
                    }
                    sqlBuilder = new StringBuilder();
                }
                catch (Exception ex)
                {
                    throw new Exception("执行SQL失败：" + sqlBuilder, ex);
                }
            }
        }
    }
}
