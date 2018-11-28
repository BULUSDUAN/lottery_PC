using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Database.DbAccess
{
    /// <summary>
    /// 错误消息定义，辅助类
    /// </summary>
    internal static class ErrorMessages
    {
        internal const string NotHandledException = "未处理的异常";
        internal const string PreconditionAssertFailed = "前置条件断言失败";
        internal const string DBConnectionStringIsNullOrEmpty = "数据库连接字符串为null或空串";
        internal const string CommandTextIsNullOrEmpty = "要执行的SQL命令为null或空串";
        internal const string CommandTextIsErrorMessage = "要执行的SQL命令错误，不能正确执行";
        internal const string TableNameIsEmpty = "表名为空";
        internal const string SqlParameterNotMatchValues = "提供的参数类表与SQL语句需要的参数不匹配";
        internal const string NotSupportedDatabaseType = "提供的数据库类型不支持";
        internal const string NullReferenceException = "提供的参数对象为null或空字符串";
        internal const string AddEntityErrorMessage = "添加实体对象失败";
        internal const string SelectEntityErrorMessage = "查询实体对象失败";
        internal const string ExecDbCommandErrorMessage = "操作数据库失败";
        internal const string PrimaryKeyConflict = "主键冲突";
        internal const string IndexConflict = "索引冲突";
        internal const string PrimaryKeyIsNull = "主键为空";
        internal const string CommiteError = "事务提交失败";
        internal const string ResultNotUniqueMessage = "主键查询结果不唯一";
        internal const string EntityMappingError = "实体映射错误";
        internal const string EntityReadOnly = "实体是只读";
        internal const string TransactionCommited = "事务已提交";
    }
}
