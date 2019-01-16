

namespace KaSon.FrameWork.ORM.DbHelper
{
    using KaSon.FrameWork.ORM.Provider;
    using KaSon.FrameWork.Services.Enum;
    using KaSon.FrameWork.Services.ORM;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    internal class SqlServerHelper : IDbHelper, ISqlServerSpecial
    {
        private readonly OrmConfigInfo _connInfo;
        protected readonly SqlConnection Conn = new SqlConnection();
        protected string Identity;
        protected SqlTransaction Tran = null;
        private bool _isLink = false;
        public SqlServerHelper(OrmConfigInfo info,bool isLink)
        {
            this._connInfo = info;
            this.Conn.ConnectionString = this._connInfo.ConnectionString;

            _isLink = isLink;



        }

        public void Begin(string identity)
        {
            this.Open();
            if (this.Tran == null)
            {
                this.Tran = this.Conn.BeginTransaction();
                this.Identity = identity;
            }
        }
        /// <summary>
        /// 得到一个DataTable
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {
            
                try
                {

                    this.Open();
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(dr);
                        dr.Close();
                        dr.Dispose();
                        cmd.Prepare();
                        return dt;
                    }
                }
                catch (Exception exception)
                {
                   
                    throw new Exception("执行SQL过程出错" + exception.Message, exception);
                }
                finally
                {
                if (!_isLink)
                {
                    this.Close();
                }
                   
                }
            
        }

       

        /// <summary>
        /// 高效批量录入
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int BulkAdd(DataTable dataSource, BulkAddParam param)
        {
            int count;
            try
            {
                this.Open();
                
                using (SqlBulkCopy copy = new SqlBulkCopy(this.Conn, SqlBulkCopyOptions.Default,this.Tran))
                {
     
                    //copy.BatchSize = 0x2710;
                    copy.BulkCopyTimeout = 60;
                    copy.DestinationTableName = param.TableName;
                    foreach (ColumnMapping mapping in param.ColumnMappings)
                    {
                        copy.ColumnMappings.Add(mapping.DataTableColumnName, mapping.DBColumnName);
                    }
                    copy.WriteToServer(dataSource);
                    count = dataSource.Rows.Count;
                }
            }
            catch (Exception exception)
            {
                throw new Exception("错误信息：" + exception.Message, exception);
            }
            finally
            {
                if (!_isLink)
                {
                    this.Close();
                }


            }
            return count;
        }

        public void Close()
        {
            if ((this.Tran == null) && (this.Conn.State != ConnectionState.Closed))
            {
                this.Conn.Close();
            }
        }

        public void CloseHelper(string identity)
        {
            if ((this.Tran != null) && (this.Identity == identity))
            {
                this.Rollback(identity);
            }
            this.Close();
        }

        public void Commit(string identity)
        {
            if (this.Tran == null)
            {
                throw new Exception("事务没有开启");
            }
            if ((this.Tran != null) && (this.Identity == identity))
            {
                this.Tran.Commit();
                this.Tran = null;
                this.Identity = "";
                this.Close();
            }
        }

        public int ExcuteNonQuery(string sql,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        {
            //Write("开始SQL:" + sql,"Add");
            int num2;
            try
            {
                this.Open();
                SqlCommand cmd = this.GetCommand(sql, parameters);
                int num = cmd.ExecuteNonQuery();
                SetParameters(cmd, parameters);
                num2 = num;


            }
            catch (Exception exception)
            {
                throw new Exception(sql + "错误信息：" + exception.Message, exception);
            }
            finally
            {
                if (!_isLink)
                {
                    this.Close();
                }
                //  this.Close();
            }
            return num2;
        }

        /// <summary>
        /// 非查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExcuteNonQueryWithID(string sql, KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        {
            object num2 = null;
            try
            {
                this.Open();
                SqlCommand cmd = this.GetCommand(sql, parameters);
                num2=cmd.ExecuteNonQuery();

              //  num2 = cmd.LastInsertedId;

                // SetParameters(cmd, parameters);
                // num2 = num;
            }
            catch (Exception exception)
            {
                throw new Exception(sql + "错误信息：" + exception.Message, exception);
            }
            finally
            {
                if (!_isLink)
                {
                    this.Close();
                }
            }
            return num2;
        }


        /// <summary>执行存储过程
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public int ExcuteProcNonQuery(string name, KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        {
            int num2;
            try
            {
                this.Open();
                SqlCommand cmd = this.GetCommand(name, parameters);
                cmd.CommandType = CommandType.StoredProcedure;
                int num = cmd.ExecuteNonQuery();
                SetParameters(cmd, parameters);
                num2 = num;
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat(new object[] { "执行存储过程 ", name, "出错", exception }), exception);
            }
            finally
            {
                if (!_isLink)
                {
                    this.Close();
                }
            }
            return num2;
        }

        /// <summary>执行存储过程
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public int ExcuteProcNonQuery(string name, params object[] paraValues)
        {
            int num2;
            try
            {
                this.Open();
                SqlCommand cmd = this.GetCommand(name, null);
                cmd.CommandType = CommandType.StoredProcedure;
                addInParaValue(name, cmd, paraValues);

                int num = cmd.ExecuteNonQuery();
              //  SetParameters(cmd, parameters);
                num2 = num;
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat(new object[] { "执行存储过程 ", name, "出错", exception }), exception);
            }
            finally
            {
                if (!_isLink)
                {
                    this.Close();
                }
            }
            return num2;
        }

        /// <summary>执行存储过程
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public DataSet ExcuteProcQuery(string name,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        {
            DataSet set2;
            try
            {
                this.Open();
                SqlCommand selectCommand = this.GetCommand(name, parameters);
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                selectCommand.CommandType = CommandType.StoredProcedure;
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                SetParameters(selectCommand, parameters);
                set2 = dataSet;
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat(new object[] { "执行存储过程 ", name, "出错", exception }), exception);
            }
            finally
            {
                if (!_isLink)
                {
                    this.Close();
                }
            }
            return set2;
        }
        /// <summary>执行存储过程
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public TResult ExcuteProcReader<TResult>(string name, Func<DbDataReader, TResult> func,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        {
            SqlDataReader arg = null;
            TResult local2;
            try
            {
                this.Open();
                SqlCommand cmd = this.GetCommand(name, parameters);
                cmd.CommandType = CommandType.StoredProcedure;
                arg = cmd.ExecuteReader();
                SetParameters(cmd, parameters);
                TResult local = func(arg);
                arg.Close();
                local2 = local;
            }
            catch (Exception exception)
            {
                if (arg != null)
                {
                    arg.Close();
                }
                throw new Exception("执行存储过程 " + name + "出错" + exception.Message, exception);
            }
            finally
            {
                if (!_isLink)
                {
                    this.Close();
                }
            }
            return local2;
        }

        /// <summary>执行存储过程
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public TResult ExcuteProcReader<TResult>(string name, Func<DbDataReader, TResult> func, params object[] paraValues)
        {
            SqlDataReader arg = null;
            TResult local2;
            try
            {
                this.Open();
                SqlCommand cmd = GetCommand(name, null);
                cmd.CommandType = CommandType.StoredProcedure;
                addInParaValue(name, cmd, paraValues);
                arg = cmd.ExecuteReader();
                TResult local = func(arg);
                arg.Close();
                local2 = local;
            }
            catch (Exception exception)
            {
                if (arg != null)
                {
                    arg.Close();
                }
                throw new Exception("执行存储过程 " + name + "出错" + exception.Message, exception);
            }
            finally
            {
                if (!_isLink)
                {
                    this.Close();
                }
            }
            return local2;
        }

        //为存储过程参数赋值
        private  void addInParaValue(string _name, SqlCommand comm, params object[] paraValues)
        {
            if (paraValues != null)
            {
                List<string> al = GetParas(_name);
                try
                {
                    for (int i = 0; i < paraValues.Length; i++)
                    {
                        //这是从i算起,注意啦......
                        comm.Parameters.AddWithValue(al[i], paraValues[i]);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }


        //获取存储过程的参数列表
        //exec sp_sproc_columns  @procedure_name='sel_website'(存储过程名)
        private List<string> GetParas(string name)
        {
            List<string> al = new List<string>();
            //  using (SqlConnection con =this.Conn)
            //   {
            SqlCommand comm = new SqlCommand("sp_sproc_columns", this.Conn);
            comm.CommandTimeout = 600;
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@procedure_name", name); //查找存储过程的参数列表

            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            for (int i = 1; i < dt.Rows.Count; i++)//记得从１开始算起(添回存储过程的参数名)
            {
                al.Add(dt.Rows[i][3].ToString());
            }
            //  }
            return al;
        }



        public DataSet ExcuteQuery(string sql,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        {

         

            DataSet set2;
            try
            {
                this.Open();
                SqlCommand selectCommand = this.GetCommand(sql, parameters);
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                SetParameters(selectCommand, parameters);
                set2 = dataSet;
            }
            catch (Exception exception)
            {
                throw new Exception(sql + "错误信息：" + exception.Message, exception);
            }
            finally
            {
                if (!_isLink)
                {
                    this.Close();
                }
            }
            return set2;
        }

        private static object LockObj = new object();

        public static void Write(string name= "SQL_", string smsg="")
        {

            // 写入日志
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string path = string.Empty;
            string filename = DateTime.Now.Day.ToString() + ".log";
           // string str = "";
            string logfile = null;
            //StreamWriter writer = null;
            try
            {


                lock (LockObj)
                {
                    string str = "";
                    if (!string.IsNullOrEmpty(logfile))
                    {
                        str = @"Log_Log\SQLInfo\" + name + logfile;
                    }
                    else
                    {
                        str = @"Log_Log\SQLInfo\" + name + DateTime.Now.ToString("yyyyMMddHH") + "_" + AppDomain.CurrentDomain.Id.ToString() + ".log";
                    }
                    str = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, str);
                    string directoryName = Path.GetDirectoryName(str);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    string str3 = string.Format("{0}：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), smsg);
                    using (StreamWriter writer = new StreamWriter(str, true, Encoding.Default))
                    {
                        writer.WriteLine(str3);
                    }
                }

            } catch(Exception ex) {
                throw ex;
            }

                //if (!string.IsNullOrWhiteSpace(logfile))
                //{
                //    str = @"Global_Log\" + Type + logfile;
                //}
                //else
                //{
                //    str = @"Global_Log\exp_" + Type + DateTime.Now.ToString("yyyyMMddHH") + "_" + AppDomain.CurrentDomain.Id.ToString() + ".txt";
                //}
                //str = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, str);
                //string directoryName = Path.GetDirectoryName(str);
                //if (!Directory.Exists(directoryName))
                //{
                //    Directory.CreateDirectory(directoryName);
                //}

                //using (writer = new StreamWriter(str, true, Encoding.Default))
                //{

                //    writer.WriteLine(s);

                //}
                //writer.Close();

            //}
            //finally
            //{
            //    if (writer != null)
            //        writer.Close();
            //}

        }
        public TResult ExecuteReader<TResult>(string sql, Func<DbDataReader, TResult> func,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        {
         
            SqlDataReader arg = null;
            TResult local2;
#if LogInfo
            Stopwatch watch = new Stopwatch();
            Console.WriteLine("Stopwatch");
#endif
            try
            {
                //时间
#if LogInfo
                watch.Start();
#endif
                this.Open();
#if LogInfo
                watch.Stop();
                Double opt = watch.Elapsed.TotalMilliseconds;
                watch.Start();
#endif

                SqlCommand cmd = this.GetCommand(sql, parameters);
              
                SetParameters(cmd, parameters);
                arg = cmd.ExecuteReader();
                //时间
#if LogInfo
                watch.Stop();
                Double opt1 = watch.Elapsed.TotalMilliseconds;

                watch.Start();
#endif
                // string str = command.Parameters[1].Value.ToString();
                TResult local = func(arg);
                arg.Close();
                local2 = local;
#if LogInfo
                watch.Stop();
                Double opt2 = watch.Elapsed.TotalMilliseconds;
               // watch.Start();

                Write("LogTime_", string.Format("sql:{0}\r\n,Open打开时间:{1}\r\n读出时间:{2}\r\n时间:{3}\r\n", sql, opt.ToString(), opt1.ToString(), opt2.ToString()));
#endif
#if LogInfo
                if (DbProvider.IsShowOneSQL)
                {
                    StringBuilder sb = new StringBuilder(); ;
                    foreach (var item in parameters)
                    {
                        sb.Append(string.Format("name:{0},value:{1} \r\n", item.Name, item.Value));
                        // str = str + ;
                    }
                    sb.Append(string.Format("sql:{0}\r\n", sql));
                    Write("Log_",sb.ToString());
                    DbProvider.IsShowOneSQL = false;
                }
#endif
            }
            catch (Exception exception)
            {
               // #if LogInfo
                if (arg != null)
                {
                    arg.Close();
                }

                StringBuilder sb = new StringBuilder(); ;
                sb.Append(exception.ToString());
                foreach (var item in parameters)
                {
                    sb.Append(string.Format("name:{0},value:{1} \r\n", item.Name, item.Value));
                    // str = str + ;
                }
                sb.Append(string.Format("sql:{0}\r\n", sql));
                Write("SQL_", sb.ToString());
                //    DbProvider.IsShowOneSQL = false;
                throw new Exception("错误信息：" + exception.Message, exception);
            }
            finally
            {
                if (!_isLink)
                {
                    this.Close();
                }
            }
            return local2;
        }


    

        public object ExecuteScalar(string sql,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        {
            //Write("开始SQL:" + sql);
            object obj3;
            try
            {
                this.Open();
                SqlCommand cmd = this.GetCommand(sql, parameters);
                object obj2 = cmd.ExecuteScalar();
                SetParameters(cmd, parameters);
                obj3 = obj2;
            }
            catch (Exception exception)
            {
                throw new Exception(sql + "错误信息：" + exception.Message, exception);
            }
            finally
            {
                if (!_isLink)
                {
                    this.Close();
                }
            }
            return obj3;
        }

        private SqlCommand GetCommand(string sql,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        {
            SqlCommand command = new SqlCommand(sql)
            {
                Connection = this.Conn,
                CommandTimeout = this._connInfo.CommandTimeout,
                Transaction = this.Tran
            };
            if (parameters != null)
            {
                foreach (KaSon.FrameWork.Services.ORM.DbParameter parameter in parameters)
                {
                    SqlParameter parameter2 = new SqlParameter
                    {
                        ParameterName = parameter.Name,
                        Direction = parameter.Direction,
                        Value = parameter.Value
                    };
                    if (parameter.Value == null) {
                        parameter2.SqlDbType = SqlDbType.VarChar;
                    }
                    if (parameter.Length.HasValue)
                    {
                        parameter2.Size = parameter.Length.Value;
                    }
                    if (parameter.DbType.HasValue)
                    {
                        parameter2.DbType = parameter.DbType.Value;
                    }
                    if (parameter.SqlType.HasValue)
                    {
                        SqlServerDataType valueOrDefault = parameter.SqlType.GetValueOrDefault();
                        if (parameter.SqlType.HasValue)
                        {
                            switch (valueOrDefault)
                            {
                                case SqlServerDataType.Char:
                                    parameter2.SqlDbType = SqlDbType.Char;
                                    break;

                                case SqlServerDataType.Nchar:
                                    parameter2.SqlDbType = SqlDbType.NChar;
                                    break;

                                case SqlServerDataType.Ntext:
                                    parameter2.SqlDbType = SqlDbType.NText;
                                    break;

                                case SqlServerDataType.Nvarchar:
                                    parameter2.SqlDbType = SqlDbType.NVarChar;
                                    break;

                                case SqlServerDataType.Text:
                                    parameter2.SqlDbType = SqlDbType.Text;
                                    break;

                                case SqlServerDataType.Varchar:
                                    parameter2.SqlDbType = SqlDbType.VarChar;
                                    break;

                                case SqlServerDataType.Bit:
                                    parameter2.SqlDbType = SqlDbType.Bit;
                                    break;

                                case SqlServerDataType.Tinyint:
                                    parameter2.SqlDbType = SqlDbType.TinyInt;
                                    break;

                                case SqlServerDataType.Smallint:
                                    parameter2.SqlDbType = SqlDbType.SmallInt;
                                    break;

                                case SqlServerDataType.Int:
                                    parameter2.SqlDbType = SqlDbType.Int;
                                    break;

                                case SqlServerDataType.Bigint:
                                    parameter2.SqlDbType = SqlDbType.BigInt;
                                    break;

                                case SqlServerDataType.Decimal:
                                    parameter2.SqlDbType = SqlDbType.Decimal;
                                    break;

                                case SqlServerDataType.Money:
                                    parameter2.SqlDbType = SqlDbType.Money;
                                    break;

                                case SqlServerDataType.Smallmoney:
                                    parameter2.SqlDbType = SqlDbType.SmallMoney;
                                    break;

                                case SqlServerDataType.Float:
                                    parameter2.SqlDbType = SqlDbType.Float;
                                    break;

                                case SqlServerDataType.Real:
                                    parameter2.SqlDbType = SqlDbType.Real;
                                    break;

                                case SqlServerDataType.Binary:
                                    parameter2.SqlDbType = SqlDbType.Binary;
                                    break;

                                case SqlServerDataType.Image:
                                    parameter2.SqlDbType = SqlDbType.Image;
                                    break;

                                case SqlServerDataType.Varbinary:
                                    parameter2.SqlDbType = SqlDbType.VarBinary;
                                    break;

                                case SqlServerDataType.Date:
                                    parameter2.SqlDbType = SqlDbType.Date;
                                    break;

                                case SqlServerDataType.Datetime:
                                    parameter2.SqlDbType = SqlDbType.DateTime;
                                    break;
                            }
                        }
                    }
                    command.Parameters.Add(parameter2);
                }
            }
            return command;
        }

        protected void Open()
        {
            try
            {
                if (this.Conn.State != ConnectionState.Open)
                {
                    this.Conn.Open();
                }
            }
            catch
            {
                if (this.Conn.State != ConnectionState.Closed)
                {
                    this.Conn.Close();
                }
                this.Conn.ConnectionString = OperateCommon.GetConnInfo(this._connInfo.DbKey).ConnectionString;
                this.Conn.Open();
            }
        }
        protected void OpenEx()
        {
            try
            {
                if (this.Conn.State != ConnectionState.Open)
                {
                    this.Conn.Open();
                }
            }
            catch
            {
                if (this.Conn.State != ConnectionState.Closed)
                {
                    this.Conn.Close();
                }
                this.Conn.ConnectionString = OperateCommon.GetConnInfo(this._connInfo.DbKey).ConnectionString;
                this.Conn.Open();
            }
        }

        public void Rollback(string identity)
        {
            if (this.Tran == null)
            {
                throw new Exception("事务没有开启");
            }
            if ((this.Tran != null) && (this.Identity == identity))
            {
                this.Tran.Rollback();
                this.Tran = null;
                this.Identity = "";
                this.Close();
            }
        }

        private static void SetParameters(SqlCommand cmd,KaSon.FrameWork.Services.ORM.DbParameterCollection paramters)
        {
            foreach (System.Data.Common.DbParameter parameter in cmd.Parameters)
            {
                if (parameter.Direction == ParameterDirection.Output)
                {
                 paramters[parameter.ParameterName].Value = parameter.Value;
                }
                if ((parameter.Direction == ParameterDirection.ReturnValue) && (paramters[parameter.ParameterName] != null))
                {
                    paramters[parameter.ParameterName].Value = parameter.Value;
                }
            }
        }

    
    }
}
