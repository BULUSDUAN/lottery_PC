
using KaSon.FrameWork.ORM.Builder;
using KaSon.FrameWork.ORM.Dal;
using KaSon.FrameWork.ORM.DbHelper;
using KaSon.FrameWork.ORM.Extension;
using KaSon.FrameWork.ORM.Provider;
using KaSon.FrameWork.Services.Enum;
using KaSon.FrameWork.Services.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Factory
{
    public class OracleFactory : ConstructionFactory
    {



        internal override IQueryable<T> CreateQuery<T>(Provider.DbProvider provider)
        {

            return new Query<T>(provider);
        }

        internal override IBuilder.IDbBuilder CreateDbBuilder()
        {
            return new OracleBuilder();
        }
        
        internal override IDal<T> CreateDal<T>(DbProvider provider)
        {
            return new Dal<T>(provider,this.IsLLink);
        }

        internal override DalHelper CreateDalHelper(DbProvider provider)
        {
            return new OracleDalHelper(provider);
        }

        //internal override ICommonQuery CreateDbExtension(DbProvider provider)
        //{
        //    return new SqlServerExtension(provider);
        //}

        public override IDbHelper CreateDbHelper()
        {
           // return new OracleHelper(base.ConfigInfo);
            throw new DllNotFoundException();
        }

        //public IDbHelperExtend CreateDbHelperExtend()
        //{
        //    return new SqlServerHelperExtend(base.ConfigInfo);
        //}

        internal override IDbParamterBuilder CreateDbParamterBuilder()
        {
            return new OracleParamterBuilder();
        }


       
        public override ProviderInfo GetProviderInfo()
        {
            return ProviderInfo.Oracle;
        }

        /// <summary>
        ///  执行SQL 语句
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        internal override IQuery CreateSQLQuery(DbProvider provider,string sql)
        {
            return new Query(provider, sql);
        }

        internal override IProQuery ProQuery(DbProvider provider, string name, params object[] paraValues)
        {

            return new ProQuery(provider, name, paraValues);
        }

        internal override IProQuery ProQuery(DbProvider provider, string name, DbParameterCollection parameters)
        {
            return new ProQuery(provider, name, parameters);
        }

        internal override ICommonQuery CreateDbExtension(DbProvider provider)
        {
            return new MySqlExtension(provider);
        }
    }
}
