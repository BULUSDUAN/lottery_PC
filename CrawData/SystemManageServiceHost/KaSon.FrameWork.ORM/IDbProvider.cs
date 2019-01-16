using KaSon.FrameWork.Services.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
  public  interface IDbProvider: IDisposable
    {
        IDbProvider Init(string dbkey, bool isLLink = false);
         bool SetLink { set; }
        
            IDbProvider Begin();
        ICommonQuery CreateComQuery();
        IQueryable<T> CreateQuery<T>() where T : class, new();
        IDal<T> GetDal<T>() where T : class, new();
        IQuery CreateSQLQuery(string sql);
        void Commit();
        void Rollback();
        bool HasDbKey();
        void Dispose();

    }
}
