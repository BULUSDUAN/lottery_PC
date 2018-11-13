using KaSon.FrameWork.ORM.Dal;
using KaSon.FrameWork.ORM.IBuilder;
using KaSon.FrameWork.ORM.Provider;
using KaSon.FrameWork.Services.Enum;
using KaSon.FrameWork.Services.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KaSon.FrameWork.ORM.Factory
{
   public abstract class ConstructionFactory
   {
       /// <summary>
       /// /配置信息
       /// </summary>
       private OrmConfigInfo _configInfo = new OrmConfigInfo();
       /// <summary>
       /// 初始化不同工厂
       /// </summary>
       /// <param name="dbKey"></param>
       /// <returns></returns>
       public static ConstructionFactory GetInstance(string dbKey)
       {
           OrmConfigInfo connInfo = OperateCommon.GetConnInfo(dbKey);
           ConstructionFactory factory = null ;
           switch (connInfo.Provider)
           {

                   ///SQLServer 工厂
               case ProviderInfo.SqlServer:

                   factory = new SQLServerFactory();

                   connInfo.Provider = factory.GetProviderInfo();
                   factory.ConfigInfo = connInfo;

                   break;
               ///MySql  工厂
               case ProviderInfo.MySql:

                   factory = new MySqlFactory();

                   connInfo.Provider = factory.GetProviderInfo();
                   factory.ConfigInfo = connInfo;

                   break;
               ///Oracle  工厂
               case ProviderInfo.Oracle:

                   factory = new OracleFactory();

                   connInfo.Provider = factory.GetProviderInfo();
                   factory.ConfigInfo = connInfo;

                   break;
               default:
                   break;
           }

        
           //选择工厂开始
           return factory;
       }

       /// <summary>
       /// 创建扩展
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <returns></returns>
       public virtual T CreateExtend<T>()
       {
           Type type = base.GetType();
           foreach (MethodInfo info in type.GetMethods())
           {
               if (info.ReturnType == typeof(T))
               {
                   object[] parameters = new object[0];
                   return (T)info.Invoke(this, parameters);
               }
           }
           return Activator.CreateInstance<T>();
       }
       internal abstract IDbParamterBuilder CreateDbParamterBuilder();
       public abstract IDbHelper CreateDbHelper();
       internal abstract IDal<T> CreateDal<T>(DbProvider provider) where T : class, new();
       internal abstract IDbBuilder CreateDbBuilder();
       internal abstract IQueryable<T> CreateQuery<T>(DbProvider provider);
       internal abstract ICommonQuery CreateDbExtension(DbProvider provider);
       internal abstract IQuery CreateSQLQuery(DbProvider provider, string SQL);
       internal abstract IProQuery ProQuery(DbProvider provider, string name,params object[] values);
       internal abstract IProQuery ProQuery(DbProvider provider, string name, DbParameterCollection parameters);
       public abstract ProviderInfo GetProviderInfo();
       internal abstract DalHelper CreateDalHelper(DbProvider provider);
       public OrmConfigInfo ConfigInfo
       {
           get
           {
               return this._configInfo;
           }
           private set
           {
               this._configInfo = value;
           }
       }
    }
}
