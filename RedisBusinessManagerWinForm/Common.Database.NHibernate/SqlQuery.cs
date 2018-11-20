using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NHibernate;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Common.Database.NHibernate
{
    /// <summary>
    /// 包含输出参数的查询
    /// </summary>
    public class OutputQuery
    {
        private DbProviderFactory dbFactory = null;
        private DbConnection dbConn = null;
        private DbCommand dbCmd = null;
        private Dictionary<string, DbParameter> outputParameters = new Dictionary<string, DbParameter>();

        private SqlConnection sqlCon = null;

        internal OutputQuery(ISession session, IQuery query)
        {
            dbConn = session.Connection as DbConnection;
            sqlCon = session.Connection as SqlConnection;
            dbFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            dbCmd = dbConn.CreateCommand();
            if (session.Transaction != null)
            {
                session.Transaction.Enlist(dbCmd);
            }
            dbCmd.CommandText = query.QueryString.Replace(':', '@');
        }
        /// <summary>
        /// 添加输入参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>查询</returns>
        public OutputQuery AddInParameter(string name, object value)
        {
            var p = dbCmd.CreateParameter();
            if (value.GetType().IsEnum)
            {
                p.DbType = DbType.Int32;
            }
            else
            {
                p.DbType = (DbType)Enum.Parse(typeof(DbType), value.GetType().Name);
            }
            p.ParameterName = "@" + name;
            p.Direction = ParameterDirection.Input;
            p.Value = value;
            dbCmd.Parameters.Add(p);

            return this;
        }
        /// <summary>
        /// 添加输出参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="typeName">参数类型</param>
        /// <returns>查询</returns>
        public OutputQuery AddOutParameter(string name, string typeName)
        {
            var p = dbCmd.CreateParameter();
            p.DbType = (DbType)Enum.Parse(typeof(DbType), typeName);
            p.ParameterName = "@" + name;
            p.Direction = ParameterDirection.Output;
            dbCmd.Parameters.Add(p);
            outputParameters.Add(name, p);

            return this;
        }
        public DataTable GetDataTable()
        {
            var dt = new DataTable();
            var adap = dbFactory.CreateDataAdapter();
            adap.SelectCommand = dbCmd;
            adap.Fill(dt);
            return dt;
        }
        public DataTable GetDataTable(out Dictionary<string, object> outputs)
        {
            if (outputParameters.Count <= 0)
            {
                throw new ArgumentException(string.Format("输出参数定义有{0}个，必须返回对应数量的值", outputParameters.Count));
            }
            var dt = new DataTable();
            var adap = dbFactory.CreateDataAdapter();
            adap.SelectCommand = dbCmd;
            adap.Fill(dt);

            outputs = new Dictionary<string, object>();
            foreach (var item in outputParameters)
            {
                outputs.Add(item.Key, item.Value.Value);
            }
            return dt;
        }
        public DataSet GetDataSet()
        {
            var dt = new DataSet();
            var adap = dbFactory.CreateDataAdapter();
            adap.SelectCommand = dbCmd;
            adap.Fill(dt);
            return dt;
        }

        public DataSet GetDataSet2(string storedProcName)
        {
            //var fa = System.Data.SqlClient.SqlClientFactory.Instance;
            //var con = fa.CreateConnection();
            //con.ConnectionString = "Data Source=192.168.0.180;Initial Catalog=ECP_CORE_1210;UID=sa;PWD=Xunti2014;";
            //var cmd = fa.CreateCommand();
            //cmd.Connection = con;
            //cmd.CommandText = storedProcName;
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = 60;
            //foreach (DbParameter item in dbCmd.Parameters)
            //{
            //    var p = cmd.CreateParameter();
            //    p.DbType = item.DbType;
            //    p.ParameterName = item.ParameterName;
            //    p.Direction = item.Direction;
            //    p.Value = item.Value;

            //    cmd.Parameters.Add(p);
            //}

            //if (sqlCon.State != ConnectionState.Open)
            //    sqlCon.Open();

            //var ds = new DataSet();
            //var da = fa.CreateDataAdapter();
            //da.SelectCommand = cmd;
            //da.Fill(ds);
            //return ds;


            //以DataReader方式
            dbCmd.CommandType = CommandType.StoredProcedure;
            dbCmd.CommandText = storedProcName;
            var ds = new DataSet();
            var reader = dbCmd.ExecuteReader();
            do
            {
                var initColumns = false;
                var dt = new DataTable();
                while (reader.Read())
                {
                    var array = new object[reader.FieldCount];
                    reader.GetValues(array);
                    if (!initColumns)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dt.Columns.Add(reader.GetName(i), array[i].GetType());
                        }
                        initColumns = true;
                    }
                    dt.Rows.Add(array);
                }
                ds.Tables.Add(dt);
            }
            while (reader.NextResult());

            return ds;

            //dbCmd.CommandType = CommandType.StoredProcedure;
            //dbCmd.CommandText = storedProcName;

            //var dt = new DataSet();
            //var sqlDA = new System.Data.SqlClient.SqlDataAdapter();
            //sqlDA.SelectCommand = dbCmd as System.Data.SqlClient.SqlCommand;
            //sqlDA.Fill(dt);

            //var adap = dbFactory.CreateDataAdapter();
            //adap.SelectCommand = dbCmd;
            //adap.Fill(dt);
            //return dt;
        }

        public DataSet GetDataSet(out Dictionary<string, object> outputs)
        {
            if (outputParameters.Count <= 0)
            {
                throw new ArgumentException(string.Format("输出参数定义有{0}个，必须返回对应数量的值", outputParameters.Count));
            }
            var dt = new DataSet();
            var adap = dbFactory.CreateDataAdapter();
            adap.SelectCommand = dbCmd;
            adap.Fill(dt);

            outputs = new Dictionary<string, object>();
            foreach (var item in outputParameters)
            {
                outputs.Add(item.Key, item.Value.Value);
            }
            return dt;
        }
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="outputs">输出值</param>
        /// <returns>查询的结果列表</returns>
        public IList List(out Dictionary<string, object> outputs)
        {
            if (outputParameters.Count <= 0)
            {
                throw new ArgumentException(string.Format("输出参数定义有{0}个，必须返回对应数量的值", outputParameters.Count));
            }
            var dt = new DataTable();
            var adap = dbFactory.CreateDataAdapter();
            adap.SelectCommand = dbCmd;
            adap.Fill(dt);

            outputs = new Dictionary<string, object>();
            foreach (var item in outputParameters)
            {
                outputs.Add(item.Key, item.Value.Value);
            }
            return GetDataTableList(dt);
        }
        /// <summary>
        /// 以分页形式返回列表
        /// </summary>
        /// <param name="totalCount">记录总条数</param>
        /// <returns>查询的结果列表</returns>
        public IList ToListByPaging(out int totalCount)
        {
            var ds = new DataSet();
            var adap = dbFactory.CreateDataAdapter();
            adap.SelectCommand = dbCmd;
            adap.Fill(ds);

            totalCount = int.Parse(ds.Tables[1].Rows[0]["rowCount"].ToString());
            ds.Tables[2].Columns.RemoveAt(ds.Tables[2].Columns.Count - 1);
            var result = new List<object>();
            foreach (DataRow row in ds.Tables[2].Rows)
            {
                result.Add(row.ItemArray);
            }
            return result;
        }

        private IList GetDataTableList(DataTable dt)
        {
            IList list = new List<object[]>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(row.ItemArray);
            }
            return list;
        }
    }
}
