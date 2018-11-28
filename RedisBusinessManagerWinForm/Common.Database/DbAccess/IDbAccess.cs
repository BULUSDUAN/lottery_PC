using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Common.Database.DbAccess
{
    /// <summary>
    /// 数据库连接类型枚举
    /// </summary>
    public enum RDatabaseType
    {
        /// <summary>
        /// MSSql
        /// </summary>
        MSSQL,
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 9,
    }
    /// <summary>
    /// 数据库访问接口
    /// </summary>
    public interface IDbAccess
    {
        /// <summary>
        /// 获取对应数据库的对象名称。如对应MS Sql server，输入TableName，则返回[TableName]
        /// </summary>
        /// <param name="objectName">对象名称。可以为表名，字段名</param>
        /// <returns>添加数据库标识以后的数据库安全名称</returns>
        string GetDbObjectName(string objectName);
        /// <summary>
        /// 连接超时间时间，以秒为单位，默认为60。
        /// 如果设置的时间为负数，则自动重置为默认值。
        /// </summary>
        int ConnectionTimeout { get; set; }
        /// <summary>
        /// 命令执行超时时间。以秒为单位，默认为60。
        /// 如果设置的时间为负数，则自动重置为默认值。
        /// </summary>
        int CommandTimeout { get; set; }
        /// <summary>
        /// 创建数据库链接
        /// </summary>
        /// <returns></returns>
        DbConnection CreateDbConnection();
        /// <summary>
        /// 创建一个DbCommand对象实例，不引发异常
        /// </summary>
        /// <returns>DbCommand对象实例</returns>
        DbCommand CreateDbCommand();
        /// <summary>
        /// 创建一个DbParameter对象实例，不引发异常
        /// </summary>
        /// <returns>DbParameter对象实例</returns>
        DbParameter CreateDbParameter();
        /// <summary>
        /// 执行一个数据库命令，并返回受影响的行数
        /// </summary>
        /// <param name="cmd">需要被执行的数据库命令对象，不允许为null</param>
        /// <returns>返回受影响的行数</returns>
        int ExecCommand(DbCommand cmd);
        /// <summary>
        /// 执行一个不返回结果集的SQL语句，并返回受影响的行数。
        /// </summary>
        /// <param name="sqlstr">
        /// 要执行的SQL语句。不允许为null或空串。
        /// 如果是一个带参数的SQL语句，那么此SQL需要是有有效格式化标识的字符串。例如：SELECT * FROM [Table] WHERE [Name]={0}
        /// </param>
        /// <param name="values">
        /// SQL语句中参数的值，对应到SQL语句中的参数序号。如果提供的SQL不需要参数，则不需要传入此参数。
        /// </param>
        /// <returns>返回受影响的行数</returns>
        int ExecSQL(string sqlstr, params object[] values);
        /// <summary>
        /// 执行SQL语句，并返回查询到的DataTable结果集。
        /// </summary>
        /// <param name="sqlstr">
        /// 要执行的SQL语句。不允许为null或空串。
        /// 如果是一个带参数的SQL语句，那么此SQL需要是有有效格式化标识的字符串。例如：SELECT * FROM [Table] WHERE [Name]={0}
        /// </param>
        /// <param name="values">
        /// SQL语句中参数的值，对应到SQL语句中的参数序号。如果提供的SQL不需要参数，则不需要传入此参数。
        /// </param>
        /// <returns>返回查询到的DataTable结果集。如果没有查到任何数据，则返回没有数据的DataTable对象。</returns>
        DataTable GetDataTableBySQL(string sqlstr, params object[] values);
        /// <summary>
        /// 执行DbCommand数据库命令，并返回查询到的DataTable结果集。
        /// </summary>
        /// <param name="cmd">
        /// 要执行的DbCommand数据库命令。不允许为nul。
        /// </param>
        /// <returns>返回查询到的DataTable结果集。如果没有查到任何数据，则返回没有数据的DataTable对象。</returns>
        DataTable GetDataTableByCommand(DbCommand cmd);
        /// <summary>
        /// 执行DbCommand数据库命令，并返回查询到的DataSet结果集。
        /// </summary>
        /// <param name="cmd">
        /// 要执行的DbCommand数据库命令。不允许为nul。
        /// </param>
        /// <returns>返回查询到的DataTable结果集。如果没有查到任何数据，则返回没有数据的DataTable对象。</returns>
        DataSet GetDataSetByCommand(DbCommand cmd);
        /// <summary>
        /// 执行SQL语句，并返回查询到的DataTable结果集。
        /// </summary>
        /// <param name="sqlstr">
        /// 要执行的SQL语句。不允许为null或空串。
        /// 如果是一个带参数的SQL语句，那么此SQL需要是有有效格式化标识的字符串。例如：SELECT * FROM [Table] WHERE [Name]={0}
        /// </param>
        /// <param name="values">
        /// SQL语句中参数的值，对应到SQL语句中的参数序号。如果提供的SQL不需要参数，则不需要传入此参数。
        /// </param>
        /// <returns>返回查询到的DataTable结果集。如果没有查到任何数据，则返回没有数据的DataTable对象。</returns>
        DataSet GetDataSetBySQL(string sqlstr, params object[] values);
        /// <summary>
        /// 将一个DataTable中的数据保存到数据库中
        /// </summary>
        /// <param name="datatb">
        /// 要更新到数据库的DataTable内存表。
        /// 此表必须包含一个表名称。
        /// 如果传入表为null或没有任何改动，则不做任何操作。
        /// </param>
        void SaveDataTable(DataTable datatb);
        /// <summary>
        /// 将一个内存中的数据行保存到数据库中
        /// </summary>
        /// <param name="tablename">表名，不允许为null或空字符串</param>
        /// <param name="row">
        /// 包含要更新到数据库中的数据的数据据行。
        /// 如果为null，或状态为未改变(Unchanged)或分离(Detached)状态，则不做任何操作。
        /// </param>
        void SaveDataRow(string tablename, DataRow row);
        /// <summary>
        /// 执行一个SQL语句，并返回其结果集中第一行第一列的值
        /// </summary>
        /// <param name="sqlstr">SQL语句，不允许为null或空字符串</param>
        /// <param name="values">SQL语句中参数的值，对应到SQL语句中的每一个参数</param>
        /// <returns>其结果集中第一行第一列的值。如果返回的结果集没有数据，则返回null。</returns>
        object GetRC1BySQL(string sqlstr, params object[] values);
        /// <summary>
        /// 执行一个SQL语句，并返回其结果集中第一行第一列的值
        /// </summary>
        /// <returns>其结果集中第一行第一列的值。如果返回的结果集没有数据，则返回null。</returns>
        object GetRC1ByCommand(DbCommand cmd);
    }
    public interface IDbDoAccess : IDbAccess
    {
        void DoTransaction(Action action);
    }
    public interface IDbTranAccess : IDbAccess, IDisposable
    {
        void Commit();

        void Rollback();
    }
}
