using KaSon.FrameWork.ORM.Factory;
using KaSon.FrameWork.Services.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Provider
{
  public  class DbProvider:IDisposable
    {
      /// <summary>
      /// Db key
      /// </summary>
      private string _dbKey;
      /// <summary>
      /// 抽象工厂
      /// </summary>
    
      private ConstructionFactory _factory;

      /// <summary>
      /// 唯一标识   回话唯一标识
      /// </summary>
      private readonly string _id = Guid.NewGuid().ToString();

      /// <summary>
      /// 查询上下文
      /// </summary>
      private QueryContext _queryContext;

        public static bool IsShowOneSQL = false;
      /// <summary>
      /// 查询上下文
      /// </summary>
      private QueryProviderContext _queryProviderContext;

        public static bool InitConfigJson(List<OrmConfigInfo> list) {

           return OperateCommon.SetConfigInfo(list);
        }

      private bool _isTran;

        public bool HasDbKey() {

            if (String.IsNullOrEmpty(this._dbKey))
            {

                return false;
            }
            else if (!GlobalCache.DbHelperPool.Keys.Contains(this._dbKey)) {
                return false;
            }

            return true;
        }
      public DbProvider Init(string dbkey)
      {
         // Console.WriteLine("_factory1" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
          this._dbKey = dbkey;
          this._factory = ConstructionFactory.GetInstance(dbkey);
          if (!GlobalCache.DbHelperPool.Keys.Contains(dbkey))
          {
              IDbHelper helper = this._factory.CreateDbHelper();
              GlobalCache.DbHelperPool.Add(dbkey, helper);
          }
       
        //  this._factory = ConstructionFactory.GetInstance(dbkey);
         //  _dbHelper = this.Factory.CreateDbHelper();
         // Console.WriteLine("_factory2" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
          return this;
      }
      public KaSon.FrameWork.Services.ORM.IDbHelper CreateDbHelper()
      {
          return this._factory.CreateExtend<KaSon.FrameWork.Services.ORM.IDbHelper>();
      }
      public virtual IQueryable<T> CreateQuery<T>() where T : class, new()
      {
          return this._factory.CreateQuery<T>(this);
      }
      public IQuery CreateSQLQuery(string sql)
      {

          return this._factory.CreateSQLQuery(this, sql);

      }

      #region 存储过程
      /// <summary>
      /// 存储过程
      /// </summary>
      /// <param name="name"></param>
      /// <param name="paraValues"></param>
      /// <returns></returns>
      public IProQuery ProQuery(string name, params object[] paraValues)
      {

          return this._factory.ProQuery(this,name, paraValues);

      }
      public IProQuery ProQuery(string name, DbParameterCollection parameters = null)
      {

          return this._factory.ProQuery(this, name, parameters);

      }
      public int ProNonQuery(string name, params object[] paraValues)
      {

          return this._factory.ProQuery(this, name, paraValues).Excute();

      }
      public int ProNonQuery(string name, DbParameterCollection parameters = null)
      {

          return this._factory.ProQuery(this, name, parameters).Excute();

      }
      #endregion
      #region 通用查询

      public ICommonQuery CreateComQuery()
      {


          return this._factory.CreateDbExtension(this);
      }


      #endregion

      internal ConstructionFactory Factory
      {
          get
          {
              return this._factory;
          }
      }

      public KaSon.FrameWork.Services.ORM.IDbHelper GetDbHelper()
      {
          if (!KaSon.FrameWork.ORM.GlobalCache.DbHelperPool.Keys.Contains(this._dbKey))
          {
              throw new ApplicationException("找不到当前dbKey，请确定当前是否存在dbKey 或 是否缓存DbProvider！");
          }
          return KaSon.FrameWork.ORM.GlobalCache.DbHelperPool[this._dbKey];
      }
      public DbProvider Begin()
      {
          if (!this._isTran)
          {
              this.GetDbHelper().Begin(this._id);
              this._isTran = true;
          }
          return this;
      }
      internal string Id
      {
          get
          {
              return this._id;
          }
      }
      public void ClearCache()
      {
          GlobalCache.Clear();
      }
      public void Rollback()
      {
          try
          {
              this.GetDbHelper().Rollback(this._id);
          }
          finally
          {
              this._isTran = false;
          }
      }
      public void Commit()
      {
          try
          {
              this.GetDbHelper().Commit(this._id);
          }
          finally
          {
              this._isTran = false;
          }
      }
      public bool IsTran
      {
          get
          {
              return this._isTran;
          }
      }
    
      public KaSon.FrameWork.Services.ORM.ConnInfo ConnInfo
      {
          get
          {
              return new KaSon.FrameWork.Services.ORM.ConnInfo { CommandTimeout =
                  this.Factory.ConfigInfo.CommandTimeout, ConnectionString = 
                  this.Factory.ConfigInfo.ConnectionString, 
                  Provider =  this.Factory.ConfigInfo.Provider,
                  FactoryType =
                  this.Factory.ConfigInfo.FactoryType };
          }
      }
      public IDal<T> GetDal<T>() where T : class, new()
      {
          return this._factory.CreateDal<T>(this);
      }
    
      internal QueryContext QueryContext
      {
          get
          {
              return (this._queryContext ?? (this._queryContext = new QueryContext(this)));
          }
      }

      internal QueryProviderContext QueryProviderContext
      {
            set {
                this._queryProviderContext = value;
            }
          get
          {
              return (this._queryProviderContext ?? (this._queryProviderContext = new QueryProviderContext()));
          }
      }

      public void Dispose()
      {
          if (this._queryContext != null)
          {
              this._queryContext.Parameters.Remove(this._id);
          }
            if (KaSon.FrameWork.ORM.GlobalCache.DbHelperPool.Keys.Contains(this._dbKey))
            {
                IDbHelper dbHelper = this.GetDbHelper();
                if (dbHelper != null)
                {
                    dbHelper.CloseHelper(this._id);
                }
            }
           
          
      }
    }
}
