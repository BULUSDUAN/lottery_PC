using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Database.DbAccess;
using System.Data;
using System.Reflection;
using Common.Utilities;
using System.IO;

namespace Common.Database
{
    public class SqlInitHelper
    {
        private IDbAccess _dbAccess = null;
        public SqlInitHelper(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }
        public void ClearTableData(params string[] tableNames)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var tableName in tableNames)
            {
                sb.AppendLine(string.Format("DELETE FROM [{0}]", tableName));
            }
            _dbAccess.ExecSQL(sb.ToString());
        }
        public void DropAllTable()
        {
            _dbAccess.ExecSQL("EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
            TryExecSql("EXEC sp_MSForEachTable 'DROP TABLE ?'", 1, 20);
        }
        public void ClearAllData()
        {
            _dbAccess.ExecSQL("EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
            _dbAccess.ExecSQL("EXEC sp_MSForEachTable 'ALTER TABLE ? DISABLE TRIGGER ALL'");
            _dbAccess.ExecSQL("EXEC sp_MSForEachTable 'DELETE FROM ?'");
            _dbAccess.ExecSQL("EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'");
            _dbAccess.ExecSQL("EXEC sp_MSForEachTable 'ALTER TABLE ? ENABLE TRIGGER ALL'");
        }
        public void ExecResourceSqls(string pattern, Assembly assem = null, string innerNameSpace = null)
        {
            if (assem == null)
            {
                assem = Assembly.GetCallingAssembly();
            }
            var names = assem.GetManifestResourceNames();
            var sorted = names.OrderBy((k) => k);
            foreach (var name in sorted)
            {
                if (RegHelper.IsReg(name, pattern))
                {
                    SqlAnalyzer.ForeachSQLFromStream(assem.GetManifestResourceStream(name), _dbAccess
                        , (n) =>
                        {
                            n = string.Format("{0}.{1}", innerNameSpace, n);
                            var stream = assem.GetManifestResourceStream(n);
                            if (stream == null)
                            {
                                throw new FileNotFoundException("未找到sql资源文件 - " + n);
                            }
                            return stream;
                        });
                }
            }
        }
        private int TryExecSql(string sql, int currentTime, int maxTimes)
        {
            try
            {
                return _dbAccess.ExecSQL(sql);
            }
            catch
            {
                if (currentTime >= maxTimes)
                {
                    throw;
                }
                return TryExecSql(sql, currentTime + 1, maxTimes);
            }
        }
    }
}
