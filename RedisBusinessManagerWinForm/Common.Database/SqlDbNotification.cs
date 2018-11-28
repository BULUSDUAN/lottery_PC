using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;

namespace Common.Database
{
    /// <summary>
    /// Sql server 数据库通知
    /// </summary>
    public class SqlDbNotification
    {
        public static void RegisterNotificationSql(DbConnection conn, string dependencySql, Action<SqlCommand> cmdAction, OnChangeEventHandler notificationHandler)
        {
            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = dependencySql;

            RegisterNotificationCommand(cmd, cmdAction, notificationHandler);
        }
        private static void RegisterNotificationCommand(DbCommand cmd, Action<SqlCommand> cmdAction, OnChangeEventHandler notificationHandler)
        {
            if (!(cmd is SqlCommand))
            {
                throw new ArgumentException("传入的DbCommand实例只支持SqlCommand类型", "DbCommand");
            }
            using (SqlCommand sqlCmd = cmd as SqlCommand)
            {
                if (cmd.Connection.State == System.Data.ConnectionState.Closed)
                {
                    cmd.Connection.Open();
                }
                SqlDependency dependency = new SqlDependency(sqlCmd);
                dependency.OnChange += notificationHandler;

                cmdAction(sqlCmd);

                cmd.Connection.Close();
            }
        }
    }
}
