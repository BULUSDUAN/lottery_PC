using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Common.Database.DbAccess
{
    internal class DbAccessHelper
    {
        internal static string GetDbCommandErrorMsg(DbConnection db, params DbCommand[] cmdList)
        {
            StringBuilder errorMsgBuilder = new StringBuilder();
            foreach (DbCommand cmd in cmdList)
            {
                if (cmd == null) continue;

                errorMsgBuilder.AppendLine("Type: " + cmd.GetType().FullName);
                if (db == null)
                {
                    errorMsgBuilder.AppendLine("Connection String: Null");
                }
                else
                {
                    SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(db.ConnectionString);
                    if (!string.IsNullOrEmpty(sb.Password)) sb.Password = "******";
                    errorMsgBuilder.AppendLine("Connection String: " + sb.ConnectionString);
                }
                errorMsgBuilder.AppendLine("Command Type: " + cmd.CommandType.ToString());
                errorMsgBuilder.AppendLine("Command Text: " + cmd.CommandText);
                if (cmd.Transaction != null)
                {
                    errorMsgBuilder.AppendLine("Transaction: " + cmd.Transaction.IsolationLevel.ToString());
                }
                else
                {
                    errorMsgBuilder.AppendLine("Transaction: None");
                }
                if (cmd.Parameters != null && cmd.Parameters.Count > 0)
                {
                    errorMsgBuilder.AppendLine("Parameters: " + cmd.Parameters.Count);
                    for (int i = 0; i < cmd.Parameters.Count; i++)
                    {
                        DbParameter p = cmd.Parameters[i];
                        errorMsgBuilder.Append("[" + i + "]" + "");
                        errorMsgBuilder.Append(p.ParameterName);
                        errorMsgBuilder.Append("(" + p.Direction.ToString() + ")");
                        errorMsgBuilder.Append(p.DbType.ToString());
                        errorMsgBuilder.Append("(" + p.Size.ToString() + ")");
                        errorMsgBuilder.AppendLine();
                        errorMsgBuilder.AppendLine("    Value: " + (p.Value == null ? "NULL" : p.Value.ToString()));
                    }
                }
                else
                {
                    errorMsgBuilder.AppendLine("Parameters: None");
                }
                errorMsgBuilder.AppendLine();
            }
            return errorMsgBuilder.ToString();
        }

        /// <summary>
        /// 组合字符串格式化的信息，并返回，不引发异常
        /// </summary>
        internal static string GetFormatErrorMsg(string str, params object[] values)
        {
            StringBuilder errorMsgBuilder = new StringBuilder();
            errorMsgBuilder.AppendLine(str);
            if (values != null && values.Length > 0)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    object o = values[i];
                    errorMsgBuilder.Append("[p" + i + "]" + "");
                    if (o != null)
                    {
                        errorMsgBuilder.AppendLine(o.ToString());
                    }
                    else
                    {
                        errorMsgBuilder.AppendLine("null");
                    }
                }
            }
            else
            {
                errorMsgBuilder.AppendLine("Parameters: None");
            }
            return errorMsgBuilder.ToString();
        }
    }
}
