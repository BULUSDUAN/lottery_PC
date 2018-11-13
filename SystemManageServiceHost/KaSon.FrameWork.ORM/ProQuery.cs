using KaSon.FrameWork.ORM.Provider;
using KaSon.FrameWork.Services.ORM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal class ProQuery : IProQuery
    {
        private Provider.DbProvider _provider;
        /// <summary>
        /// 表达式树
        /// </summary>
        private readonly System.Linq.Expressions.Expression _expression;

        /// <summary>
        /// 查询SQL
        /// </summary>
        public string ProName { get; set; }
        public KaSon.FrameWork.Services.ORM.DbParameterCollection ParameterCollection { get; set; }
        public object[] ParaValues { get; set; }
        public ProQuery(Provider.DbProvider provider)
        {
            // TODO: Complete member initialization

            var Context = provider.QueryContext;


            this._provider = provider;
            this._expression = System.Linq.Expressions.Expression.Constant(this);
        }

        public ProQuery(ORM.Provider.DbProvider dbProvider, string name, KaSon.FrameWork.Services.ORM.DbParameterCollection collet = null)
        {
            // TODO: Complete member initialization
            this._provider = dbProvider;
            this.ProName = name;
            this.ParameterCollection = collet;

        }
        public ProQuery(ORM.Provider.DbProvider dbProvider, string name, params object[] paraValues)
        {
            // TODO: Complete member initialization
            this._provider = dbProvider;
            this.ProName = name;
            this.ParaValues = paraValues;

        }





        public DbProvider DbProvider
        {
            get
            {
                return this._provider;
            }
        }



        public Type ReturnType { get; set; }


        /// <summary>
        ///  反射对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> List<T>()
        {



            Func<DbDataReader, IList<T>> func;

            //映射委托
            func = EntityHelper.EntityAssign<T>();
            string ProName = this.ProName;

            if (this.ParameterCollection == null)
            {
                return this.DbProvider.Factory.CreateDbHelper().ExcuteProcReader(ProName, func,this.ParaValues);
            }
            else
            {
                return this.DbProvider.Factory.CreateDbHelper().ExcuteProcReader(ProName, func,
                this.ParameterCollection);
            }


        }


        /// <summary>
        ///  返回第一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T First<T>()
        {
            Func<DbDataReader, IList<T>> func;


            func = EntityHelper.EntityAssign<T>();

            string ProName = this.ProName;

            IList<T> list = this.DbProvider.Factory.CreateDbHelper().ExcuteProcReader(ProName, func,
              this.ParaValues);
            if (this.ParameterCollection == null)
            {
                list = this.DbProvider.Factory.CreateDbHelper().ExcuteProcReader(ProName, func,
            this.ParaValues);
            }
            else
            {
                list = this.DbProvider.Factory.CreateDbHelper().ExcuteProcReader(ProName, func,
                this.ParameterCollection);
            }
            if (list.Count > 0)
            {
                return list[0];
            }


            return default(T);

        }

        /// <summary>
        ///  执行非查询 存储过程
        /// </summary>
        /// <returns></returns>
        public int Excute()
        {
            int result = 0;
            if (this.ParameterCollection == null)
            {
                result = this.DbProvider.Factory.CreateDbHelper().ExcuteProcNonQuery(ProName,
            this.ParaValues);
            }
            else
            {
                result = this.DbProvider.Factory.CreateDbHelper().ExcuteProcNonQuery(ProName,
                this.ParameterCollection);
            }
            return result;
        }
    }
}
