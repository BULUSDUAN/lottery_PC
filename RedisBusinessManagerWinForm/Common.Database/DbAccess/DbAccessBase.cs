using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Common.Utilities;

namespace Common.Database.DbAccess
{
    /// <summary>
    /// 数据库访问的抽象类。
    /// 此类提供对数据库读写的基础函数。
    /// </summary>
    [Serializable]
    internal abstract class DbAccessBase : IDbDoAccess
    {
        internal DbEngine _engine = null;

        /// <summary>
        /// 根据数据库连接字符串初始化对象
        /// </summary>
        /// <param name="constr">提供数据库连接字符串，不能为null或空串</param>
        protected DbAccessBase(string constr)
        {
            // 断言连接字符串不为null或空串
            PreconditionAssert.IsFalse(string.IsNullOrEmpty(constr), ErrorMessages.DBConnectionStringIsNullOrEmpty);

            _engine = new DbEngine(Factory, constr);

            ConnectionTimeout = 60;
            CommandTimeout = 60;
        }

        #region 抽象方法定义

        protected abstract DbProviderFactory Factory { get; }

        /// <summary>
        /// 返回一个参数占位符
        /// </summary>
        /// <param name="paramindex">参数位置</param>
        /// <returns>参数占位符</returns>
        protected abstract string GetParamPlaceHolder(int paramindex);

        protected abstract DbAccessException HandleDbAccessException(DbConnection db, DbException ex, params DbCommand[] cmdList);

        #endregion

        #region 接口 IDBAccess 成员

        /// <summary>
        /// 连接超时间时间，以秒为单位，默认为60。
        /// 如果设置的时间为负数，则自动重置为默认值。
        /// </summary>
        public int ConnectionTimeout { get; set; }
        /// <summary>
        /// 命令执行超时时间。以秒为单位，默认为60。
        /// 如果设置的时间为负数，则自动重置为默认值。
        /// </summary>
        public int CommandTimeout { get; set; }
        /// <summary>
        /// 创建数据库链接
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateDbConnection()
        {
            return _engine.CreateConnection();
        }
        /// <summary>
        /// 创建一个DbCommand对象实例，不引发异常
        /// </summary>
        /// <returns>DbCommand对象实例</returns>
        public DbCommand CreateDbCommand()
        {
            DbCommand cmd = _engine.CreateCommand();
            _engine.InitDbCommand(cmd);
            cmd.CommandTimeout = CommandTimeout;
            return cmd;
        }
        /// <summary>
        /// 创建一个DbParameter对象实例，不引发异常
        /// </summary>
        /// <returns>DbParameter对象实例</returns>
        public DbParameter CreateDbParameter()
        {
            return _engine.CreateParameter();
        }
        /// <summary>
        /// 获取对应数据库的对象名称。如对应MS Sql server，输入TableName，则返回[TableName]
        /// </summary>
        /// <param name="objectName">对象名称。可以为表名，字段名</param>
        /// <returns>添加数据库标识以后的数据库安全名称</returns>
        public string GetDbObjectName(string objectName)
        {
            using (DbCommandBuilder cmdBuilder = _engine.CreateCommandBuilder())
            {
                return cmdBuilder.QuotePrefix + objectName + cmdBuilder.QuoteSuffix;
            }
        }
        public void DoTransaction(Action action)
        {
            try
            {
                _engine.BeginAction();
                bool succeed = false;
                try
                {
                    _engine.BeginTransaction();
                    action();
                    succeed = true;
                }
                finally
                {
                    _engine.FinishTransaction(succeed);
                }
            }
            finally
            {
                _engine.FinishAction();
            }
        }
        public int ExecSQL(string sqlstr, params object[] values)
        {
            ValidateSqlStrAndParams(sqlstr, values);

            int result = 0;
            _engine.DoCommand((cmd) =>
            {
                InitDbCommand(cmd, sqlstr, values);
                result = this.DoExecCommand(cmd);
            });
            return result;
        }
        public int ExecCommand(DbCommand cmd)
        {
            PreconditionAssert.IsNotNull(cmd, ErrorMessages.CommandTextIsNullOrEmpty);

            try
            {
                _engine.BeginAction();
                return this.DoExecCommand(cmd);
            }
            finally
            {
                _engine.FinishAction();
            }
        }
        public DataTable GetDataTableBySQL(string sqlstr, params object[] values)
        {
            ValidateSqlStrAndParams(sqlstr, values);

            DataTable result = null;
            _engine.DoCommand((cmd) =>
            {
                InitDbCommand(cmd, sqlstr, values);
                result = this.DoGetDataTableByCommand(cmd);
            });
            return result;
        }
        public DataTable GetDataTableByCommand(DbCommand cmd)
        {
            PreconditionAssert.IsNotNull(cmd, ErrorMessages.CommandTextIsNullOrEmpty);
            try
            {
                _engine.BeginAction();
                return this.DoGetDataTableByCommand(cmd);
            }
            finally
            {
                _engine.FinishAction();
            }
        }
        public DataSet GetDataSetBySQL(string sqlstr, params object[] values)
        {
            ValidateSqlStrAndParams(sqlstr, values);

            DataSet result = null;
            _engine.DoCommand((cmd) =>
            {
                InitDbCommand(cmd, sqlstr, values);
                result = this.DoGetDataSetByCommand(cmd);
            });
            return result;
        }
        public DataSet GetDataSetByCommand(DbCommand cmd)
        {
            PreconditionAssert.IsNotNull(cmd, ErrorMessages.CommandTextIsNullOrEmpty);
            try
            {
                _engine.BeginAction();
                return this.DoGetDataSetByCommand(cmd);
            }
            finally
            {
                _engine.FinishAction();
            }
        }
        public void SaveDataTable(DataTable dt)
        {
            PreconditionAssert.IsNotNull(dt, ErrorMessages.CommandTextIsNullOrEmpty);
            PreconditionAssert.IsNotEmptyString(dt.TableName, ErrorMessages.TableNameIsEmpty);
            DoTransaction(() =>
            {
                DoSaveDataTable(dt);
            });
        }
        public void SaveDataRow(string tableName, DataRow row)
        {
            PreconditionAssert.IsNotNull(row, ErrorMessages.CommandTextIsNullOrEmpty);
            PreconditionAssert.IsNotEmptyString(tableName, ErrorMessages.TableNameIsEmpty);
            _engine.DoCommand((cmd) =>
            {
                using (DbDataAdapter adapter = _engine.CreateDataAdapter())
                {
                    adapter.SelectCommand = cmd;
                    using (DbCommandBuilder dbCmdBuilder = _engine.CreateCommandBuilder())
                    {
                        this.DoSaveDataRow(adapter, dbCmdBuilder, tableName, row);
                    }
                }
            });
        }
        public object GetRC1BySQL(string sqlstr, params object[] values)
        {
            ValidateSqlStrAndParams(sqlstr, values);

            object value = null;
            _engine.DoCommand((cmd) =>
            {
                InitDbCommand(cmd, sqlstr, values);
                value = DoGetRC1Value(cmd);
            });
            return value;
        }

        public object GetRC1ByCommand(DbCommand cmd)
        {
            PreconditionAssert.IsNotNull(cmd, ErrorMessages.CommandTextIsNullOrEmpty);
            try
            {
                _engine.BeginAction();
                return this.DoGetRC1Value(cmd);
            }
            finally
            {
                _engine.FinishAction();
            }
        }

        #endregion

        #region 内部 Do Command 函数

        private int DoExecCommand(DbCommand cmd)
        {
            try
            {
                int result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (DbException ex)
            {
                throw HandleDbAccessException(cmd.Connection, ex, cmd);
            }
            catch (Exception ex)
            {
                string errorMsg = "执行DbCommand失败" + "\r\n" + DbAccessHelper.GetDbCommandErrorMsg(cmd.Connection, cmd);
                throw new DbAccessException(errorMsg, ex);
            }
        }
        private DataTable DoGetDataTableByCommand(DbCommand cmd)
        {
            using (DbDataAdapter adp = _engine.CreateDataAdapter())
            {
                try
                {
                    adp.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    string errorMsg = "获取DataTable失败" + "\r\n" + DbAccessHelper.GetDbCommandErrorMsg(cmd.Connection, cmd);
                    throw new DbAccessException(errorMsg, ex);
                }
            }
        }
        private DataSet DoGetDataSetByCommand(DbCommand cmd)
        {
            using (DbDataAdapter adp = _engine.CreateDataAdapter())
            {
                try
                {
                    adp.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    adp.Fill(ds);
                    return ds;
                }
                catch (Exception ex)
                {
                    string errorMsg = "获取DataSet失败" + "\r\n" + DbAccessHelper.GetDbCommandErrorMsg(cmd.Connection, cmd);
                    throw new DbAccessException(errorMsg, ex);
                }
            }
        }
        private object DoGetRC1Value(DbCommand cmd)
        {
            try
            {
                return cmd.ExecuteScalar();
            }
            catch (DbException ex)
            {
                throw HandleDbAccessException(cmd.Connection, ex, cmd);
            }
            catch (Exception e)
            {
                string errorMsg = "获取RC1值失败" + "\r\n" + DbAccessHelper.GetDbCommandErrorMsg(cmd.Connection, cmd);
                throw new DbAccessException(errorMsg, e);
            }
        }

        private void DoSaveDataTable(DataTable dataTable)
        {
            if (dataTable == null || dataTable.GetChanges() == null) { return; }

            DataTable dtChanges = dataTable.GetChanges();
            if (dtChanges == null || dtChanges.Rows.Count == 0) { return; }

            _engine.DoCommand((cmd) =>
            {
                using (DbDataAdapter adap = _engine.CreateDataAdapter())
                {
                    using (DbCommandBuilder dbCmdBuilder = _engine.CreateCommandBuilder())
                    {
                        try
                        {
                            dbCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                            dbCmdBuilder.DataAdapter = adap;

                            adap.SelectCommand = cmd;
                            adap.SelectCommand.CommandText = string.Format("SELECT TOP 1 * FROM [{0}]", dataTable.TableName);
                            adap.Update(dtChanges);
                            dataTable.AcceptChanges();
                        }
                        catch (DbException ex)
                        {
                            throw HandleDbAccessException(adap.SelectCommand.Connection, ex, adap.SelectCommand, adap.InsertCommand, adap.UpdateCommand, adap.DeleteCommand);
                        }
                        catch (Exception ex)
                        {
                            throw new DbAccessException("执行保存DataTable操作失败", ex);
                        }
                    }
                }
            });
        }

        private void DoSaveDataRow(DbDataAdapter adap, DbCommandBuilder dbCmdBuilder, string tablename, DataRow row)
        {
            if (row == null
                || row.RowState == DataRowState.Unchanged
                || row.RowState == DataRowState.Detached)
            {
                return;
            }
            try
            {
                dbCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                dbCmdBuilder.DataAdapter = adap;

                adap.SelectCommand.CommandText = string.Format("SELECT TOP 1 * FROM [{0}]", tablename);
                adap.Update(new DataRow[] { row });
            }
            catch (DbException ex)
            {
                throw HandleDbAccessException(adap.SelectCommand.Connection, ex, adap.SelectCommand, adap.InsertCommand, adap.UpdateCommand, adap.DeleteCommand);
            }
            catch (Exception ex)
            {
                throw new DbAccessException("执行保存DataRow操作失败", ex);
            }
        }

        #endregion

        #region 辅助性内部函数

        /// <summary>
        /// 获取参数名称
        /// </summary>
        private string GetParamName(int paramindex)
        {
            return GetParamPlaceHolder(paramindex);
        }
        /// <summary>
        /// 验证SQL与传入参数是否匹配
        /// </summary>
        private void ValidateSqlStrAndParams(string str, params object[] values)
        {
            // 断言传入SQL语句为null或不为空串
            PreconditionAssert.IsNotEmptyString(str, ErrorMessages.CommandTextIsNullOrEmpty);
            // 断言传入的SQL参数与提供的参数值列表匹配
            string sqlDetail = ErrorMessages.SqlParameterNotMatchValues + "\n" + DbAccessHelper.GetFormatErrorMsg(str, values);
            PreconditionAssert.CanFormatString(str, values, sqlDetail);
        }
        /// <summary>
        /// 初始化DbCommand
        /// </summary>
        private void InitDbCommand(DbCommand cmd, string formatedSql, object[] values)
        {
            cmd.CommandTimeout = CommandTimeout;

            string[] parameters = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                parameters[i] = GetParamPlaceHolder(i);

                DbParameter p = CreateDbParameter();
                p.ParameterName = GetParamName(i);
                p.Value = values[i] ?? DBNull.Value;
                cmd.Parameters.Add(p);
            }
            cmd.CommandText = string.Format(formatedSql, parameters);
        }

        #endregion
    }
}
