using KaSon.FrameWork.ORM.Factory;
using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal static class GlobalCache
    {
        [ThreadStatic]
        private static IDictionary<string, IDbHelper> _dbHelperPool;

       // [ThreadStatic]
        private static IDictionary<string, IDbProvider> _dbProviderPool;
        private static readonly IDictionary<string, string> _deleteTextPool = new Dictionary<string, string>();
        private static readonly IDictionary<Type, EntityInfo> _entityInfoPool = new Dictionary<Type, EntityInfo>();
        private static readonly IDictionary<string, ConstructionFactory> _factoryInstances = new Dictionary<string, ConstructionFactory>();
        private static readonly IDictionary<string, string> _insertTextPool = new Dictionary<string, string>();
        private static readonly object _lockDelete = new object();
        private static readonly object _lockEntity = new object();
        private static readonly object _lockFactory = new object();
        private static readonly object _lockInsert = new object();
        private static readonly object _lockUpdate = new object();
        [ThreadStatic]
        private static QueryContext _QueryContext;
        private static readonly IDictionary<string, string> _updateTextPool = new Dictionary<string, string>();

        public static void AddDeleteText(string entityType, string text)
        {
            lock (_lockDelete)
            {
                if (!_deleteTextPool.ContainsKey(entityType))
                {
                    _deleteTextPool.Add(entityType, text);
                }
            }
        }
        internal static IDictionary<string, IDbHelper> DbHelperPool
        {
            get
            {
                return (_dbHelperPool ?? (_dbHelperPool = new Dictionary<string, IDbHelper>()));
            }
        }
        internal static IDictionary<string, IDbProvider> DbProviderPool
        {
            get
            {
                return (_dbProviderPool ?? (_dbProviderPool = new Dictionary<string, IDbProvider>()));
            }
        }
        public static void AddEntity(Type entityType, EntityInfo info)
        {
            lock (_lockEntity)
            {
                if (!_entityInfoPool.ContainsKey(entityType))
                {
                    _entityInfoPool.Add(entityType, info);
                }
            }
        }

        internal static void AddFactoryInstances(string dbKey, ConstructionFactory factory)
        {
            lock (_lockFactory)
            {
                if (!_factoryInstances.ContainsKey(dbKey))
                {
                    _factoryInstances.Add(dbKey, factory);
                }
            }
        }

        public static void AddInsertText(string entityType, string text)
        {
            lock (_lockInsert)
            {
                if (!_insertTextPool.ContainsKey(entityType))
                {
                    _insertTextPool.Add(entityType, text);
                }
            }
        }

        public static void AddUpdateText(string entityType, string text)
        {
            lock (_lockUpdate)
            {
                if (!_updateTextPool.ContainsKey(entityType))
                {
                    _updateTextPool.Add(entityType, text);
                }
            }
        }

        internal static void Clear()
        {
           _dbHelperPool.Clear();
            _entityInfoPool.Clear();
            _insertTextPool.Clear();
            _updateTextPool.Clear();
            _deleteTextPool.Clear();
            _factoryInstances.Clear();
            _QueryContext = null;
        }

     

        internal static IDictionary<string, string> DeleteTextPool
        {
            get
            {
                return _deleteTextPool;
            }
        }

        internal static IDictionary<Type, EntityInfo> EntityInfoPool
        {
            get
            {
                return _entityInfoPool;
            }
        }

        internal static IDictionary<string, ConstructionFactory> FactoryInstances
        {
            get
            {
                return _factoryInstances;
            }
        }

        internal static IDictionary<string, string> InsertTextPool
        {
            get
            {
                return _insertTextPool;
            }
        }

        internal static QueryContext QueryContext
        {
            get
            {
                return _QueryContext;
            }
            set
            {
                _QueryContext = value;
            }
        }

        internal static IDictionary<string, string> UpdateTextPool
        {
            get
            {
                return _updateTextPool;
            }
        }
    }
}
