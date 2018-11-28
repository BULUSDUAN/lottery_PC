using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Database.DbAccess;

namespace Common.Database.ORM
{
    public class EntityManagement : IDisposable
    {
        public ObjectPersistence Persistence { get; private set; }
        public EntityManagement(IDbAccess dbAccess)
        {
            Persistence = new ObjectPersistence(dbAccess);
        }
        public void Add<T>(T t)
        {
            Persistence.Add(t);
        }
        public void Add<T>(params T[] arr)
        {
            foreach (var t in arr)
            {
                Persistence.Add(t);
            }
        }
        public void Modify<T>(T t)
        {
            Persistence.Modify(t);
        }
        public void Delete<T>(T t)
        {
            Persistence.Delete(t);
        }
        public void Delete<T>(params T[] arr)
        {
            foreach (var t in arr)
            {
                Persistence.Delete(t);
            }
        }
        public T GetById<T>(object key)
            where T : new()
        {
            return Persistence.GetByKey<T>(key);
        }
        public IList<T> GetList<T>(params SortInfo[] orderBy)
            where T : new()
        {
            return Persistence.GetAll<T>(orderBy);
        }
        public void Dispose()
        {
            Persistence = null;
        }
    }
}
