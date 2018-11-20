using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Common.Database.DbAccess
{
    /// <summary>
    /// 访问MS SQL的数据库访问对象
    /// </summary>
    [Serializable]
    internal class DbMsSql : DbAccessBase
    {
        /// <summary>
        /// 根据数据库连接字符串初始化对象
        /// </summary>
        /// <param name="constr">提供数据库连接字符串，不能为null或空串</param>
        public DbMsSql(string constr)
            : base(constr)
        {
        }
        protected override DbProviderFactory Factory
        {
            get { return SqlClientFactory.Instance; }
        }
        protected override string GetParamPlaceHolder(int paramindex)
        {
            return "@p" + paramindex.ToString();
        }
        protected override DbAccessException HandleDbAccessException(DbConnection db, DbException ex, params DbCommand[] cmdList)
        {
            var sqlEx = ex as SqlException;
            string errorMsg;
            switch (sqlEx.Number)
            {
                case 2627:
                    errorMsg = "主键冲突" + "\n" + DbAccessHelper.GetDbCommandErrorMsg(db, cmdList);
                    return new DbAccessException(errorMsg, ex);
                case 2601:
                    errorMsg = "索引冲突" + "\n" + DbAccessHelper.GetDbCommandErrorMsg(db, cmdList);
                    return new DbAccessException(errorMsg, ex);
                default:
                    errorMsg = "抛出执行SQL命令错误异常" + "\n" + DbAccessHelper.GetDbCommandErrorMsg(db, cmdList);
                    return new DbAccessException(errorMsg, ex);
            }
        }
    }
}
