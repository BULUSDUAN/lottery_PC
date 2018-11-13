using KaSon.FrameWork.ORM.Provider;
using KaSon.FrameWork.Services.ORM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal class Query : IQuery
    {
        private Provider.DbProvider _provider;
        /// <summary>
        /// 表达式树
        /// </summary>
        private readonly System.Linq.Expressions.Expression _expression;

        /// <summary>
        /// 查询SQL
        /// </summary>
        public string QuerySQL { get; set; }
        public Query(Provider.DbProvider provider)
        {
            // TODO: Complete member initialization
          
            var Context = provider.QueryContext;

           
            this._provider = provider;
            this._expression = System.Linq.Expressions.Expression.Constant(this);
        }

      
        public Query(ORM.Provider.DbProvider dbProvider, string sql)
        {
            // TODO: Complete member initialization
            this._provider = dbProvider;
            this._provider.QueryProviderContext = new QueryProviderContext();
            this._provider.QueryProviderContext.QuerySQL = sql;
           // this._provider.QueryProviderContext.Parameters


            this.QuerySQL = sql;
        }

    


     
        public DbProvider DbProvider
        {
            get
            {
                return this._provider;
            }
        }

       

        public Type ReturnType { get; set; }
        public IQuery SetString(string name, string value)
        {
            string sql =this.QuerySQL;
            sql = sql.Replace(":" + name, "@" + name);
            this.QuerySQL = sql;
            this.DbProvider.QueryProviderContext.Parameters.Add(new KaSon.FrameWork.Services.ORM.DbParameter() { Name = name, Value = value });

                return this;
        }
        public IQuery OutInt(string name, out int Num)
        {
            Num = 0;
            string sql = this.QuerySQL;
            sql = sql.Replace(":" + name, "@" + name);
            this.QuerySQL = sql;
            this.DbProvider.QueryProviderContext.Parameters.Add(new KaSon.FrameWork.Services.ORM.DbParameter() { Name = name,Direction= ParameterDirection.Output, Length=10, DbType= DbType.Int32,Value=0 });

            return this;
        }
        public IQuery OutString(string name, out string Num)
        {
            Num = "";
            string sql = this.QuerySQL;
            sql = sql.Replace(":" + name, "@" + name);
            this.QuerySQL = sql;
            this.DbProvider.QueryProviderContext.Parameters.Add(new KaSon.FrameWork.Services.ORM.DbParameter() { Name = name, Direction = ParameterDirection.Output });

            return this;
        }
        public IQuery OutDecimal(string name, out decimal Num)
        {
            Num = 0m;
            string sql = this.QuerySQL;
            sql = sql.Replace(":" + name, "@" + name);
            this.QuerySQL = sql;
            this.DbProvider.QueryProviderContext.Parameters.Add(new KaSon.FrameWork.Services.ORM.DbParameter() { Name = name, Direction = ParameterDirection.Output });

            return this;
        }
        public IQuery SetDecimal(string name, decimal value)
        {
            string sql = this.QuerySQL;
            sql = sql.Replace(":" + name, "@" + name);
            this.QuerySQL = sql;
            this.DbProvider.QueryProviderContext.Parameters.Add(new KaSon.FrameWork.Services.ORM.DbParameter() { Name = name, Value = value });

            return this;
        }


        public IQuery SetInt(string name, int value)
        {
            string sql = this.QuerySQL;
            sql = sql.Replace(":" + name, "@" + name);
            this.QuerySQL = sql;
            this.DbProvider.QueryProviderContext.Parameters.Add(new KaSon.FrameWork.Services.ORM.DbParameter() { Name = name, Value = value });

            return this;
        }


        public IList<T>  List<T>(bool isLink = false)
        {

           
           
           Func<DbDataReader, IList<T>> func;


           func = EntityHelper.EntityAssign<T>();
           string sql = this.QuerySQL;

         return  this.DbProvider.Factory.CreateDbHelper().ExecuteReader(sql, func, this.DbProvider.QueryProviderContext.Parameters);
        }

      
       

        public IQuery SetEntity(Type type)
        {
            this.ReturnType = type;
            return this;
        }


        public T First<T>()
        {
           Func<DbDataReader, IList<T>> func;


           func = EntityHelper.EntityAssign<T>();
           string sql = this.QuerySQL;

         //  sql = sql.ToLower().Replace("select","select top 1 ");

         var list=  this.DbProvider.Factory.CreateDbHelper().ExecuteReader(sql, func, this.DbProvider.QueryProviderContext.Parameters);

         if (list.Count > 0)
         {
             return list[0];
         }

          
          return default(T);
           
        }

        public int Excute()
        {
            string sql = this.QuerySQL;
             return this.DbProvider.GetDbHelper().ExcuteNonQuery(sql, this.DbProvider.QueryProviderContext.Parameters);

            //ExcuteNonQuery
           // throw new NotImplementedException();
        }
    }
}
