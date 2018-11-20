using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Common.Database.SqlClient
{
    public class DataAccessor : IDisposable
    {
        private readonly IDbConnection _conn;
        private readonly DbProviderFactory _factory;
        public DataAccessor(IDbConnection conn)
        {
            _conn = conn;
            if (conn is SqlConnection)
            {
                _factory = System.Data.SqlClient.SqlClientFactory.Instance;
            }
            else
            {
                throw new NotSupportedException("不支持的数据库链接类型 - " + conn);
            }
        }
        public DataTable GetDataTable(string sql, string tableName = "DefaultTable")
        {
            var dt = new DataTable(tableName);
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = sql;
                using (var adap = _factory.CreateDataAdapter())
                {
                    adap.SelectCommand = cmd as DbCommand;
                    adap.Fill(dt);
                }
            }
            return dt;
        }
        private bool _isDisposed = false;
        public void Dispose()
        {
            if (!_isDisposed && _conn != null)
            {
                _conn.Dispose();
                _isDisposed = true;
            }
        }
    }
}
