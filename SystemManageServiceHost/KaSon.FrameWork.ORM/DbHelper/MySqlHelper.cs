

namespace KaSon.FrameWork.ORM.DbHelper
{
    using KaSon.FrameWork.Services.Enum;
    using KaSon.FrameWork.Services.ORM;
    using MySql.Data.MySqlClient;
   
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.IO;

  
    internal class MySqlHelper : IDbHelper
    {
        private readonly OrmConfigInfo _connInfo;
        protected readonly MySqlConnection Conn = new MySqlConnection();
        protected string Identity;
        protected MySqlTransaction Tran = null;

        public MySqlHelper(OrmConfigInfo info)
        {
            this._connInfo = info;
            this.Conn.ConnectionString = this._connInfo.ConnectionString;
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
        /// 
    
      
        public DataTable GetDataTable(string sql)
        {
            
                try
                {

                    this.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql))
                    {
                        MySqlDataReader dr = cmd.ExecuteReader();
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
                this.Close();
            }

        }

       

         //<summary>
         //高效批量录入
         //</summary>
         //<param name="dataSource"></param>
         //<param name="param"></param>
         //<returns></returns>
        //public int BulkAdd(DataTable dataSource,  SqlServerBulkAddParam param)
        //{
        //    int count;
        //    try
        //    {
        //        this.Open();
        //        using (MySqlBulkLoader copy = new SqlBulkCopy(this.Conn))
        //        {
        //            copy.BatchSize = 0x2710;
        //            copy.BulkCopyTimeout = 60;
        //            copy.DestinationTableName = param.TableName;
        //            foreach (ColumnMapping mapping in param.ColumnMappings)
        //            {
        //                copy.ColumnMappings.Add(mapping.DataTableColumnName, mapping.DBColumnName);
        //            }
        //            copy.WriteToServer(dataSource);
        //            count = dataSource.Rows.Count;
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception("错误信息：" + exception.Message, exception);
        //    }
        //    finally
        //    {
        //        this.Close();
        //    }
        //    return count;
        //}

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
        /// <summary>
        /// 非查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExcuteNonQuery(string sql,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        {
            int num2=0;
            try
            {
                this.Open();
                MySqlCommand cmd = this.GetCommand(sql, parameters);
                int num = cmd.ExecuteNonQuery();
                // SetParameters(cmd, parameters);
                // num2 = num;
            }
            catch (Exception exception)
            {
                throw new Exception(sql + "错误信息：" + exception.Message, exception);
            }
            finally
            {
                this.Close();
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
            object num2 =null;
            try
            {
                this.Open();
                MySqlCommand cmd = this.GetCommand(sql, parameters);
                 cmd.ExecuteNonQuery();

                num2= cmd.LastInsertedId;

                // SetParameters(cmd, parameters);
                // num2 = num;
            }
            catch (Exception exception)
            {
                throw new Exception(sql + "错误信息：" + exception.Message, exception);
            }
            finally
            {
                this.Close();
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
                MySqlCommand cmd = this.GetCommand(name, parameters);
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
                this.Close();
            }
            return num2;
        }

        /// <summary>执行存储过程 ,输入参数值 paraValues 参数值
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
                MySqlCommand cmd = this.CreateCommandByProcedureName(name, paraValues);
                cmd.CommandType = CommandType.StoredProcedure;
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
                this.Close();
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
                MySqlCommand selectCommand = this.GetCommand(name, parameters);
                MySqlDataAdapter adapter = new MySqlDataAdapter(selectCommand);
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
                this.Close();
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
            MySqlDataReader arg = null;
            TResult local2;
            try
            {
                this.Open();
                MySqlCommand cmd = this.GetCommand(name, parameters);
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
                this.Close();
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
            MySqlDataReader arg = null;
            TResult local2;
            try
            {
                this.Open();
                MySqlCommand cmd = this.CreateCommandByProcedureName(name, paraValues);
                cmd.CommandType = CommandType.StoredProcedure;
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
        
            return local2;
        }

        //为存储过程参数赋值
        private void addInParaValue(string _name, MySqlCommand comm, params object[] paraValues)
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
        private  List<string> GetParas(string name)
        {
            List<string> al = new List<string>();
            using (MySqlConnection con =this.Conn)
            {
                MySqlCommand comm = new MySqlCommand("sp_sproc_columns", con);
                comm.CommandTimeout = 600;
                comm.CommandType = CommandType.StoredProcedure;

               // MySqlCommandBuilder.DeriveParameters

                comm.Parameters.AddWithValue("@procedure_name", name); //查找存储过程的参数列表

                MySqlDataAdapter sda = new MySqlDataAdapter(comm);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                for (int i = 1; i < dt.Rows.Count; i++)//记得从１开始算起(添回存储过程的参数名)
                {
                    al.Add(dt.Rows[i][3].ToString());
                }
            }
            return al;
        }
        private void InitCommand(MySqlCommand command)
        {
            this.Open();
            command.Connection = this.Conn;
            if (this.Tran != null)
            {
                command.Transaction = this.Tran;
            }
            if (command.Connection.State == ConnectionState.Closed)
            {
                command.Connection.Open();
            }
        }
        /// <summary>
        /// 录入 输入类型的参数就可以，不关心参数名称 ,并且赋值
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        private MySqlCommand CreateCommandByProcedureName(string procedureName, params object[] Parameters)
        {
            MySqlCommand command = new MySqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = procedureName
            };
            this.InitCommand(command);
            MySqlCommandBuilder.DeriveParameters(command);

            if (Parameters != null)
            {
                int index = 0;

                foreach (MySqlParameter parameter in command.Parameters)
                {
                    if ((parameter.Direction == ParameterDirection.Input))
                    {
                        command.Parameters[parameter.ParameterName].Value = Parameters[index];
                        index++;
                    }
                }
            }
         
            return command;
        }


        public DataSet ExcuteQuery(string sql,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        {
            DataSet set2;
            try
            {
                this.Open();
                MySqlCommand selectCommand = this.GetCommand(sql, parameters);
                MySqlDataAdapter adapter = new MySqlDataAdapter(selectCommand);
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
                this.Close();
            }
            return set2;
        }
        public static void Write(string s)
        {

            string Type = "MySQL_";
            return;
            StreamWriter writer = null;
            try
            {

                // 写入日志
                string year = DateTime.Now.Year.ToString();
                string month = DateTime.Now.Month.ToString();
                string path = string.Empty;
                string filename = DateTime.Now.Day.ToString() + ".log";
                string str = "";
                string logfile = null;


                if (!string.IsNullOrWhiteSpace(logfile))
                {
                    str = @"Global_Log\" + Type + logfile;
                }
                else
                {
                    str = @"Global_Log\exp_" + Type + DateTime.Now.ToString("yyyyMMddHH") + "_" + AppDomain.CurrentDomain.Id.ToString() + ".txt";
                }
                str = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, str);
                string directoryName = Path.GetDirectoryName(str);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                using (writer = new StreamWriter(str, true, System.Text.Encoding.Default))
                {

                    writer.WriteLine(s);

                }
                //writer.Close();

            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }

        }
        public TResult ExecuteReader<TResult>(string sql, Func<DbDataReader, TResult> func,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        {

            Write("开始SQL:" + sql);
          //  Console.WriteLine(sql);

            MySqlDataReader arg = null;
            TResult local2;
            try
            {
                this.Open();
                MySqlCommand cmd = this.GetCommand(sql, parameters);
              
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
                throw new Exception(sql + "错误信息：" + exception.Message, exception);
            }
            finally
            {
                this.Close();
            }
            return local2;
        }

    

        public object ExecuteScalar(string sql,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        {
            Write("开始SQL:" + sql);
            object obj3;
            try
            {
                this.Open();
                MySqlCommand cmd = this.GetCommand(sql, parameters);
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
                this.Close();
            }
            return obj3;
        }

        private MySqlCommand GetCommand(string sql,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        {
            MySqlCommand command = new MySqlCommand(sql)
            {
                Connection = this.Conn,
                CommandTimeout = this._connInfo.CommandTimeout,
                Transaction = this.Tran
            };
            if (parameters != null)
            {
                foreach (KaSon.FrameWork.Services.ORM.DbParameter parameter in parameters)
                {
                    MySqlParameter parameter2 = new MySqlParameter
                    {
                        ParameterName = parameter.Name,
                        Direction = parameter.Direction,
                        Value = parameter.Value
                    };
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
                        MySqlDataType valueOrDefault = parameter.MySqlType.GetValueOrDefault();
                        if (parameter.SqlType.HasValue)
                        {
                            switch (valueOrDefault)
                            {
                                case MySqlDataType.Char:
                                case MySqlDataType.Nvarchar:
                                case MySqlDataType.Varchar:
                                    parameter2.MySqlDbType = MySqlDbType.VarChar;
                                    break;

                                case MySqlDataType.Text:
                                    parameter2.MySqlDbType = MySqlDbType.Text;
                                    break;
                                case MySqlDataType.LongText:
                                    parameter2.MySqlDbType = MySqlDbType.LongText;
                                    break;
                                case MySqlDataType.MediumText:
                                    parameter2.MySqlDbType = MySqlDbType.MediumText;
                                    break;
                                case MySqlDataType.TinyText:
                                    parameter2.MySqlDbType = MySqlDbType.TinyText;
                                    break;
                                case MySqlDataType.Bit:
                                    parameter2.MySqlDbType = MySqlDbType.Bit;
                                    break;

                                case MySqlDataType.Int:
                                    parameter2.MySqlDbType = MySqlDbType.Int64;
                                    break;

                               

                                case MySqlDataType.Decimal:
                                    parameter2.MySqlDbType = MySqlDbType.Decimal;
                                    break;


                                case MySqlDataType.Float:
                                    parameter2.MySqlDbType = MySqlDbType.Float;
                                    break;



                                case MySqlDataType.Blob:
                                    parameter2.MySqlDbType = MySqlDbType.Blob;
                                    break;

                                case MySqlDataType.LongBlob:
                                    parameter2.MySqlDbType = MySqlDbType.LongBlob;
                                    break;

                                case MySqlDataType.MediumBlob:
                                    parameter2.MySqlDbType = MySqlDbType.MediumBlob;
                                    break;
                                case MySqlDataType.TinyBlob:
                                    parameter2.MySqlDbType = MySqlDbType.TinyBlob;
                                    break;

                                case MySqlDataType.Date:
                                    parameter2.MySqlDbType = MySqlDbType.Date;
                                    break;

                                case MySqlDataType.Datetime:
                                    parameter2.MySqlDbType = MySqlDbType.DateTime;
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

        private static void SetParameters(MySqlCommand cmd,KaSon.FrameWork.Services.ORM.DbParameterCollection paramters)
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
