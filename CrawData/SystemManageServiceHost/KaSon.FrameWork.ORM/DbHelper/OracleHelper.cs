

namespace KaSon.FrameWork.ORM.DbHelper
{
    using KaSon.FrameWork.Services.Enum;
    using KaSon.FrameWork.Services.ORM;
   // 
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
   // using Oracle.DataAccess.Client;
  
    internal class OracleHelper : IDbHelper
    {
        //private readonly OrmConfigInfo _connInfo;
        //protected readonly OracleConnection Conn = new OracleConnection();
        //protected string Identity;
        //protected OracleTransaction Tran = null;

        //public OracleHelper(OrmConfigInfo info)
        //{
        //    this._connInfo = info;
        //    this.Conn.ConnectionString = this._connInfo.ConnectionString;
        //}

        //public void Begin(string identity)
        //{
        //    this.Open();
        //    if (this.Tran == null)
        //    {
        //        this.Tran = this.Conn.BeginTransaction();
        //        this.Identity = identity;
        //    }
        //}
        ///// <summary>
        ///// 得到一个DataTable
        ///// </summary>
        ///// <param name="sql">sql语句</param>
        ///// <returns></returns>
        ///// 
    
      
        //public DataTable GetDataTable(string sql)
        //{
            
        //        try
        //        {

        //            this.Open();
        //            using (OracleCommand cmd = new OracleCommand(sql))
        //            {
        //                OracleDataReader dr = cmd.ExecuteReader();
        //                DataTable dt = new DataTable();
        //                dt.Load(dr);
        //                dr.Close();
        //                dr.Dispose();
        //                cmd.Prepare();
        //                return dt;
        //            }
        //        }
        //        catch (Exception exception)
        //        {
                   
        //            throw new Exception("执行SQL过程出错" + exception.Message, exception);
        //        }
        //        //finally
        //        //{
        //        //    this.Close();
        //        //}
            
        //}

       

        // //<summary>
        // //高效批量录入
        // //</summary>
        // //<param name="dataSource"></param>
        // //<param name="param"></param>
        // //<returns></returns>
        //public int BulkAdd(DataTable dataSource, BulkAddParam param)
        //{
        //    int count;
        //    try
        //    {
        //        this.Open();
        //        using (OracleBulkCopy copy = new OracleBulkCopy(this.Conn))
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

        //public void Close()
        //{
        //    if ((this.Tran == null) && (this.Conn.State != ConnectionState.Closed))
        //    {
        //        this.Conn.Close();
        //    }
        //}

        //public void CloseHelper(string identity)
        //{
        //    if ((this.Tran != null) && (this.Identity == identity))
        //    {
        //        this.Rollback(identity);
        //    }
        //    this.Close();
        //}

        //public void Commit(string identity)
        //{
        //    if (this.Tran == null)
        //    {
        //        throw new Exception("事务没有开启");
        //    }
        //    if ((this.Tran != null) && (this.Identity == identity))
        //    {
        //        this.Tran.Commit();
        //        this.Tran = null;
        //        this.Identity = "";
        //        this.Close();
        //    }
        //}

        //public int ExcuteNonQuery(string sql,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        //{
        //    int num2;
        //    try
        //    {
        //        this.Open();
        //        OracleCommand cmd = this.GetCommand(sql, parameters);
        //        int num = cmd.ExecuteNonQuery();
        //        SetParameters(cmd, parameters);
        //        num2 = num;
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception(sql + "错误信息：" + exception.Message, exception);
        //    }
        //    //finally
        //    //{
        //    //    this.Close();
        //    //}
        //    return num2;
        //}
        ///// <summary>执行存储过程
        ///// </summary>
        ///// <param name="procName"></param>
        ///// <param name="collection"></param>
        ///// <returns></returns>
        //public int ExcuteProcNonQuery(string name, KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        //{
        //    int num2;
        //    try
        //    {
        //        this.Open();
        //        OracleCommand cmd = this.GetCommand(name, parameters);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        int num = cmd.ExecuteNonQuery();
        //        SetParameters(cmd, parameters);
        //        num2 = num;
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception(string.Concat(new object[] { "执行存储过程 ", name, "出错", exception }), exception);
        //    }
        //    //finally
        //    //{
        //    //    this.Close();
        //    //}
        //    return num2;
        //}

       

        ///// <summary>执行存储过程
        ///// </summary>
        ///// <param name="procName"></param>
        ///// <param name="collection"></param>
        ///// <returns></returns>
        //public DataSet ExcuteProcQuery(string name,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        //{
        //    DataSet set2;
        //    try
        //    {
        //        this.Open();
        //        OracleCommand selectCommand = this.GetCommand(name, parameters);
        //        OracleDataAdapter adapter = new OracleDataAdapter(selectCommand);
        //        selectCommand.CommandType = CommandType.StoredProcedure;
        //        DataSet dataSet = new DataSet();
        //        adapter.Fill(dataSet);
        //        SetParameters(selectCommand, parameters);
        //        set2 = dataSet;
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception(string.Concat(new object[] { "执行存储过程 ", name, "出错", exception }), exception);
        //    }
        //    //finally
        //    //{
        //    //    this.Close();
        //    //}
        //    return set2;
        //}
        ///// <summary>执行存储过程
        ///// </summary>
        ///// <param name="procName"></param>
        ///// <param name="collection"></param>
        ///// <returns></returns>
        //public TResult ExcuteProcReader<TResult>(string name, Func<DbDataReader, TResult> func,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        //{
        //    OracleDataReader arg = null;
        //    TResult local2;
        //    try
        //    {
        //        this.Open();
        //        OracleCommand cmd = this.GetCommand(name, parameters);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        arg = cmd.ExecuteReader();
        //        SetParameters(cmd, parameters);
        //        TResult local = func(arg);
        //        arg.Close();
        //        local2 = local;
        //    }
        //    catch (Exception exception)
        //    {
        //        if (arg != null)
        //        {
        //            arg.Close();
        //        }
        //        throw new Exception("执行存储过程 " + name + "出错" + exception.Message, exception);
        //    }
        //    //finally
        //    //{
        //    //    this.Close();
        //    //}
        //    return local2;
        //}

     


        //public DataSet ExcuteQuery(string sql,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        //{
        //    DataSet set2;
        //    try
        //    {
        //        this.Open();
        //        OracleCommand selectCommand = this.GetCommand(sql, parameters);
        //        OracleDataAdapter adapter = new OracleDataAdapter(selectCommand);
        //        DataSet dataSet = new DataSet();
        //        adapter.Fill(dataSet);
        //        SetParameters(selectCommand, parameters);
        //        set2 = dataSet;
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception(sql + "错误信息：" + exception.Message, exception);
        //    }
        //    //finally
        //    //{
        //    //    this.Close();
        //    //}
        //    return set2;
        //}

        //public TResult ExecuteReader<TResult>(string sql, Func<DbDataReader, TResult> func,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        //{


        //    Console.WriteLine(sql);

        //    OracleDataReader arg = null;
        //    TResult local2;
        //    try
        //    {
        //        this.Open();
        //        OracleCommand cmd = this.GetCommand(sql, parameters);
        //        arg = cmd.ExecuteReader();
        //        SetParameters(cmd, parameters);
        //        TResult local = func(arg);
        //        arg.Close();
        //        local2 = local;
        //    }
        //    catch (Exception exception)
        //    {
        //        if (arg != null)
        //        {
        //            arg.Close();
        //        }
        //        throw new Exception(sql + "错误信息：" + exception.Message, exception);
        //    }
        //    //finally
        //    //{
        //    //    this.Close();
        //    //}
        //    return local2;
        //}
     
        //public object ExecuteScalar(string sql,KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        //{
        //    object obj3;
        //    try
        //    {
        //        this.Open();
        //        OracleCommand cmd = this.GetCommand(sql, parameters);
        //        object obj2 = cmd.ExecuteScalar();
        //        SetParameters(cmd, parameters);
        //        obj3 = obj2;
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception(sql + "错误信息：" + exception.Message, exception);
        //    }
        //    //finally
        //    //{
        //    //    this.Close();
        //    //}
        //    return obj3;
        //}
        //private OracleCommand GetCommand(string sql, KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        //{
        //    OracleCommand command = new OracleCommand(sql)
        //    {
        //        Connection = this.Conn,
        //        CommandTimeout = this._connInfo.CommandTimeout,
        //        Transaction = this.Tran
        //    };
        //    if (parameters != null)
        //    {
        //        foreach (KaSon.FrameWork.Services.ORM.DbParameter parameter in parameters)
        //        {
        //            OracleParameter parameter2 = new OracleParameter
        //            {
        //                ParameterName = parameter.Name,
        //                Direction = parameter.Direction,
        //                Value = parameter.Value
        //            };
        //            if (parameter.Length.HasValue)
        //            {
        //                parameter2.Size = parameter.Length.Value;
        //            }
        //            if (parameter.DbType.HasValue)
        //            {
        //                parameter2.DbType = parameter.DbType.Value;
        //            }
        //            if (parameter.SqlType.HasValue)
        //            {
        //                OracleDataType valueOrDefault = parameter.OracleType.GetValueOrDefault();
        //                if (parameter.SqlType.HasValue)
        //                {
        //                    switch (valueOrDefault)
        //                    {
        //                        case OracleDataType.Char:
        //                            parameter2.OracleDbType = OracleDbType.Char;
        //                            break;

        //                        case OracleDataType.Varchar2:
        //                            parameter2.OracleDbType = OracleDbType.Varchar2;
        //                            break;

        //                        case OracleDataType.Nchar:
        //                            parameter2.OracleDbType = OracleDbType.NChar;
        //                            break;

        //                        case OracleDataType.Nvarchar2:
        //                            parameter2.OracleDbType = OracleDbType.NVarchar2;
        //                            break;

        //                        case OracleDataType.Date:
        //                            parameter2.OracleDbType = OracleDbType.Date;
        //                            break;

        //                        case OracleDataType.Long:
        //                            parameter2.OracleDbType = OracleDbType.Long;
        //                            break;

        //                        case OracleDataType.Raw:
        //                            parameter2.OracleDbType = OracleDbType.Raw;
        //                            break;

        //                        case OracleDataType.Blob:
        //                            parameter2.OracleDbType = OracleDbType.Blob;
        //                            break;

        //                        case OracleDataType.Clob:
        //                            parameter2.OracleDbType = OracleDbType.Clob;
        //                            break;

        //                        case OracleDataType.Nclob:
        //                            parameter2.OracleDbType = OracleDbType.NClob;
        //                            break;

        //                        case OracleDataType.Number:
        //                            parameter2.OracleDbType = OracleDbType.Decimal;
        //                            break;

        //                        case OracleDataType.Timestamp:
        //                            parameter2.OracleDbType = OracleDbType.TimeStamp;
        //                            break;
        //                    }
        //                }
        //            }
        //            command.Parameters.Add(parameter2);
        //        }
        //    }
        //    return command;
        //}

     

        //protected void Open()
        //{
        //    try
        //    {
        //        if (this.Conn.State != ConnectionState.Open)
        //        {
        //            this.Conn.Open();
        //        }
        //    }
        //    catch
        //    {
        //        if (this.Conn.State != ConnectionState.Closed)
        //        {
        //            this.Conn.Close();
        //        }
        //        this.Conn.ConnectionString = OperateCommon.GetConnInfo(this._connInfo.DbKey).ConnectionString;
        //        this.Conn.Open();
        //    }
        //}

        //public void Rollback(string identity)
        //{
        //    if (this.Tran == null)
        //    {
        //        throw new Exception("事务没有开启");
        //    }
        //    if ((this.Tran != null) && (this.Identity == identity))
        //    {
        //        this.Tran.Rollback();
        //        this.Tran = null;
        //        this.Identity = "";
        //        this.Close();
        //    }
        //}

        //private static void SetParameters(OracleCommand cmd,KaSon.FrameWork.Services.ORM.DbParameterCollection paramters)
        //{
        //    foreach (System.Data.Common.DbParameter parameter in cmd.Parameters)
        //    {
        //        if (parameter.Direction == ParameterDirection.Output)
        //        {
        //            paramters[parameter.ParameterName].Value = parameter.Value;
        //        }
        //        if ((parameter.Direction == ParameterDirection.ReturnValue) && (paramters[parameter.ParameterName] != null))
        //        {
        //            paramters[parameter.ParameterName].Value = parameter.Value;
        //        }
        //    }
        //}
        //protected void DealParamters(KaSon.FrameWork.Services.ORM.DbParameterCollection paramters)
        //{
        //    foreach ( KaSon.FrameWork.Services.ORM.DbParameter parameter in paramters)
        //    {
        //        if (parameter.Value is bool)
        //        {
        //            parameter.Value = Convert.ToBoolean(parameter.Value) ? 1 : 0;
        //        }
        //        if ((parameter.Value == null) || (parameter.Value is DBNull))
        //        {
        //            parameter.Value = null;
        //        }
        //        if ((parameter.Value != null) && parameter.Value.GetType().IsEnum)
        //        {
        //            parameter.Value = Convert.ToInt32(parameter.Value);
        //        }
        //    }
        //}
        //protected OracleDbType GetOracleDbType(DbType dbType)
        //{
        //    switch (dbType)
        //    {
        //        case DbType.AnsiString:
        //        case DbType.Guid:
        //        case DbType.String:
        //        case DbType.AnsiStringFixedLength:
        //            return OracleDbType.NVarchar2;

        //        case DbType.Date:
        //        case DbType.DateTime:
        //        case DbType.DateTime2:
        //        case DbType.DateTimeOffset:
        //            return OracleDbType.Date;

        //        case DbType.Decimal:
        //            return OracleDbType.Decimal;

        //        case DbType.Double:
        //            return OracleDbType.Double;

        //        case DbType.Int16:
        //        case DbType.UInt16:
        //            return OracleDbType.Int16;

        //        case DbType.Int32:
        //        case DbType.UInt32:
        //            return OracleDbType.Int32;

        //        case DbType.Int64:
        //        case DbType.UInt64:
        //            return OracleDbType.Int64;

        //        case DbType.Object:
        //            return OracleDbType.RefCursor;

        //        case DbType.Single:
        //            return OracleDbType.Single;
        //    }
        //    return OracleDbType.NVarchar2;
        //}
        //public int ExcuteProcNonQuery(string name, params object[] parameters)
        //{
        //    OracleCommand command = this.CreateCommandByProcedureName(name, parameters);
        //    int local2 = 0;
        //    try
        //    {
        //     local2= command.ExecuteNonQuery();
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception("执行存储过程失败！" + exception.Message, exception);
        //    }
        //    //finally
        //    //{
        //    //    this.Close();
        //    //}
        //    return local2; 
        //}
        ///// <summary>
        ///// 执行SQL语  单个结果
        ///// </summary>
        ///// <typeparam name="T">泛型，约束继承DataTable和new()</typeparam>
        ///// <param name="commandString">SQL语句</param>
        ///// <param name="parameters">参数</param>
        ///// <returns></returns>
        //public TResult ExcuteProcReader<TResult>(string name, Func<DbDataReader, TResult> func, params object[] paraValues)
        //{
        //    return this.ExecuteProcedureByKeyValuePairs<TResult>(name, func, paraValues);
        //}
       
       
      

        ///// <summary>
        ///// 执行SQL语句，返回指定泛型的数据行，参数params KeyValuePair
        ///// </summary>
        ///// <typeparam name="T">泛型，约束继承DataTable和new()</typeparam>
        ///// <param name="commandString">SQL语句</param>
        ///// <param name="parameters">参数</param>
        ///// <returns></returns>
        //public TResult ExecuteProcedureByKeyValuePairs<TResult>(string procedureName, Func<DbDataReader, TResult> func, params object[] parameters)
        //{
           
        //    OracleCommand command = this.CreateCommandByProcedureName(procedureName, parameters);

        //    TResult local2;
        //    try
        //    {
        //        OracleDataReader reader = null;
        //        command.ExecuteNonQuery();
        //        SortedList<string, object> list = new SortedList<string, object>();
        //        foreach (OracleParameter parameter in command.Parameters)
        //        {
                  
        //            if ((parameter.Direction == ParameterDirection.InputOutput) || (parameter.Direction == ParameterDirection.Output) || parameter.Direction == ParameterDirection.ReturnValue)
        //            {
        //                if (parameter.OracleDbType == OracleDbType.RefCursor)
        //                {
        //                    reader = parameter.Value as OracleDataReader;
                          
                        

        //                    break;

        //                }
                       
        //            }
                   
        //        }
        //        TResult local = func(reader);
        //        reader.Close();
        //        local2 = local;

                
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception("执行存储过程失败！" + exception.Message, exception);
        //    }
           
        //    return local2; 
        //}


        // //TResult local2;
        // //   try
        // //   {
        // //       this.Open();
        // //       OracleCommand cmd = this.GetCommand(name, parameters);
        // //       cmd.CommandType = CommandType.StoredProcedure;
        // //       arg = cmd.ExecuteReader();
        // //       SetParameters(cmd, parameters);
        // //       TResult local = func(arg);
        // //       arg.Close();
        // //       local2 = local;
        // //   }
        // //   catch (Exception exception)
        // //   {
        // //       if (arg != null)
        // //       {
        // //           arg.Close();
        // //       }
        // //       throw new Exception("执行存储过程 " + name + "出错" + exception.Message, exception);
        // //   }
        // //   //finally
        // //   //{
        // //   //    this.Close();
        // //   //}
        // //   return local2;

        ///// <summary>
        ///// 执行SQL语句，返回指定泛型的数据行，参数params KeyValuePair
        ///// </summary>
        ///// <typeparam name="T">泛型，约束继承DataTable和new()</typeparam>
        ///// <param name="commandString">SQL语句</param>
        ///// <param name="parameters">参数</param>
        ///// <returns></returns>
        //public IDictionary<string, object> ExecuteProcedureByKeyValuePairs(string procedureName, params KeyValuePair<string, object>[] parameters)
        //{
        //    IDictionary<string, object> dictionary;
        //    OracleCommand command = this.CreateCommandByProcedureName(procedureName, parameters);
        //    try
        //    {
        //        command.ExecuteNonQuery();
        //        SortedList<string, object> list = new SortedList<string, object>();
        //        foreach (OracleParameter parameter in command.Parameters)
        //        {
        //            OracleDataReader reader;
        //            if ((parameter.Direction == ParameterDirection.InputOutput) || (parameter.Direction == ParameterDirection.Output))
        //            {
        //                if (parameter.OracleDbType == OracleDbType.RefCursor)
        //                {
        //                    reader = parameter.Value as OracleDataReader;
        //                    list.Add(parameter.ParameterName, reader.DataReaderToDataSet());
        //                }
        //                else
        //                {
        //                    list.Add(parameter.ParameterName, parameter.Value);
        //                }
        //            }
        //            if (parameter.Direction == ParameterDirection.ReturnValue)
        //            {
        //                if (parameter.OracleDbType == OracleDbType.RefCursor)
        //                {
        //                    reader = parameter.Value as OracleDataReader;
        //                    list.Add(parameter.ParameterName, reader.DataReaderToDataSet());
        //                }
        //                else
        //                {
        //                    list.Add(parameter.ParameterName, parameter.Value);
        //                }
        //            }
        //        }
        //        dictionary = list;
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception("执行存储过程失败！" + exception.Message, exception);
        //    }
        //    finally
        //    {
        //        this.Close();
        //    }
        //    return dictionary;
        //}
      
        ///// <summary>
        ///// 建立参数
        ///// </summary>
        ///// <param name="procedureName"></param>
        ///// <param name="Parameters"></param>
        ///// <returns></returns>
        //private OracleCommand CreateCommandByProcedureName(string procedureName, KeyValuePair<string, object>[] Parameters)
        //{
        //    OracleCommand command = new OracleCommand
        //    {
        //        CommandType = CommandType.StoredProcedure,
        //        CommandText = procedureName
        //    };
        //    this.InitCommand(command);
        //    OracleCommandBuilder.DeriveParameters(command);

        //    if (Parameters != null)
        //    {
        //        int index = 0;
        //        SortedList<string, object> list = Parameters.KeyValuePairsToSortedList<string, object>();
        //        foreach (OracleParameter parameter in command.Parameters)
        //        {
        //            if ((parameter.Direction == ParameterDirection.Input) )
        //            {
        //                command.Parameters[parameter.ParameterName].Value = list[parameter.ParameterName];
        //                index++;
        //            }
        //        }
        //    }
        //    command.BindByName = true;
        //    return command;
        //}


        ///// <summary>
        ///// 录入 输入类型的参数就可以，不关心参数名称 
        ///// </summary>
        ///// <param name="procedureName"></param>
        ///// <param name="Parameters"></param>
        ///// <returns></returns>
        //private OracleCommand CreateCommandByProcedureName(string procedureName, params object[] Parameters)
        //{
        //    OracleCommand command = new OracleCommand
        //    {
        //        CommandType = CommandType.StoredProcedure,
        //        CommandText = procedureName
        //    };
        //    this.InitCommand(command);
        //    OracleCommandBuilder.DeriveParameters(command);

        //    if (Parameters != null)
        //    {
        //        int index = 0;
              
        //        foreach (OracleParameter parameter in command.Parameters)
        //        {
        //            if ((parameter.Direction == ParameterDirection.Input))
        //            {
        //                command.Parameters[parameter.ParameterName].Value = Parameters[index];
        //                index++;
        //            }
        //        }
        //    }
        //    command.BindByName = true;
        //    return command;
        //}

        //private void InitCommand(OracleCommand command)
        //{
        //    this.Open();
        //    command.Connection = this.Conn;
        //    if (this.Tran != null)
        //    {
        //        command.Transaction = this.Tran;
        //    }
        //    if (command.Connection.State == ConnectionState.Closed)
        //    {
        //        command.Connection.Open();
        //    }
        //}

        //private OracleCommand CreateCommandByProcedureName(string procedureName, KaSon.FrameWork.Services.ORM.DbParameterCollection commandParameters)
        //{
        //    OracleCommand command = new OracleCommand
        //    {
        //        CommandType = CommandType.StoredProcedure,
        //        CommandText = procedureName
        //    };
        //    this.InitCommand(command);
        //    if (commandParameters != null)
        //    {
        //        this.DealParamters(commandParameters);
        //        foreach (KaSon.FrameWork.Services.ORM.DbParameter parameter in commandParameters)
        //        {
        //            OracleParameter param = new OracleParameter
        //            {
        //                ParameterName = parameter.Name,
        //                Direction = parameter.Direction,
        //                Value = parameter.Value
        //            };
        //            if (parameter.Length.HasValue)
        //            {
        //                param.Size = parameter.Length.Value;
        //            }
        //            if (parameter.DbType.HasValue)
        //            {
        //                param.DbType = parameter.DbType.Value;
        //                param.OracleDbType = this.GetOracleDbType(parameter.DbType.Value);
        //            }
        //            if (!((parameter.Direction != ParameterDirection.ReturnValue) || parameter.DbType.HasValue))
        //            {
        //                throw new Exception("返回类型的参数必须指定DbType!");
        //            }
        //            command.Parameters.Add(param);
        //        }
        //    }
        //    command.BindByName = true;
        //    return command;
        //}
        public void Begin(string identity)
        {
            throw new NotImplementedException();
        }

        public void CloseHelper(string identity)
        {
            throw new NotImplementedException();
        }

        public void Commit(string identity)
        {
            throw new NotImplementedException();
        }

        public int ExcuteNonQuery(string sql, Services.ORM.DbParameterCollection parameters = null)
        {
            throw new NotImplementedException();
        }

        public int ExcuteProcNonQuery(string name, Services.ORM.DbParameterCollection parameters = null)
        {
            throw new NotImplementedException();
        }

        public int ExcuteProcNonQuery(string name, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public DataSet ExcuteProcQuery(string name, Services.ORM.DbParameterCollection parameters = null)
        {
            throw new NotImplementedException();
        }

        public TResult ExcuteProcReader<TResult>(string name, Func<DbDataReader, TResult> func, Services.ORM.DbParameterCollection parameters = null)
        {
            throw new NotImplementedException();
        }

        public TResult ExcuteProcReader<TResult>(string name, Func<DbDataReader, TResult> func, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public DataSet ExcuteQuery(string sql, Services.ORM.DbParameterCollection parameters = null)
        {
            throw new NotImplementedException();
        }

        public TResult ExecuteReader<TResult>(string sql, Func<DbDataReader, TResult> func, Services.ORM.DbParameterCollection parameters = null)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar(string sql, Services.ORM.DbParameterCollection parameters = null)
        {
            throw new NotImplementedException();
        }

        public void Rollback(string identity)
        {
            throw new NotImplementedException();
        }


        public object ExcuteNonQueryWithID(string sql, Services.ORM.DbParameterCollection parameters = null)
        {
            throw new NotImplementedException();
        }
    }
}
