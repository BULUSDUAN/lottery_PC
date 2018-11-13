using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace KaSon.FrameWork.Services.ORM
{
    /// <summary>
    /// DB参数
    /// </summary>
    public interface IDbHelper
    {
        void Begin(string identity);
        void CloseHelper(string identity);
        void Commit(string identity);

        /// <summary>
        /// 一定会返回自增编号
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object ExcuteNonQueryWithID(string sql, DbParameterCollection parameters = null);

        int ExcuteNonQuery(string sql, DbParameterCollection parameters = null);
        int ExcuteProcNonQuery(string name, DbParameterCollection parameters = null);
        int ExcuteProcNonQuery(string name, params object[] parameters);
        DataSet ExcuteProcQuery(string name, DbParameterCollection parameters = null);
        /// <summary>
        /// reader完成操作后调用Close关闭连接
        /// </summary>
        /// <param name="name"></param>
        /// <param name="func"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        TResult ExcuteProcReader<TResult>(string name, Func<DbDataReader, TResult> func, DbParameterCollection parameters = null);
        TResult ExcuteProcReader<TResult>(string name, Func<DbDataReader, TResult> func, params object [] parameters );
        DataSet ExcuteQuery(string sql, DbParameterCollection parameters = null);
        TResult ExecuteReader<TResult>(string sql, Func<DbDataReader, TResult> func, DbParameterCollection parameters = null);

        
        object ExecuteScalar(string sql, DbParameterCollection parameters = null);
        void Rollback(string identity);
    }
}
