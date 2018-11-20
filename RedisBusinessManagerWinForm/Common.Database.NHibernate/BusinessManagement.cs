using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;


using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using NHibernate.Driver;
using System.Data;
using NHibernate.AdoNet;
using NHibernate.Transaction;
using NHibernate.Exceptions;
using System.Transactions;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Common.Database.NHibernate
{
    /// <summary>
    /// 事务管理器
    /// </summary>
    public abstract class BusinessManagement : IDisposable
    {
        [ThreadStatic]
        private static BusinessUtility _current;
        private static object lckObj = new object();
        /// <summary>
        /// 当前事务控制对象
        /// </summary>
        public static BusinessUtility Current
        {
            get
            {
                return _current;
            }
        }
        public void ClearCache()
        {
            BusinessUtility.ClearSession(NHibernateConfigFileName);
        }
        public void AddAssembly(string assemName)
        {
            BusinessUtility.AddAssembly(NHibernateConfigFileName, assemName);
        }
        public void AddAssembly(Assembly assem)
        {
            BusinessUtility.AddAssembly(NHibernateConfigFileName, assem);
        }
        /// <summary>
        /// 创建事务控制对象实例
        /// </summary>
        /// <typeparam name="T">事务控制对象类型，从BusinessManagement继承</typeparam>
        /// <returns>事务控制对象</returns>
        public static T CreateInstance<T>() where T : BusinessManagement, new()
        {
            return new T();
        }
        /// <summary>
        /// NHibernate配置文件名称
        /// </summary>
        public abstract string NHibernateConfigFileName { get; }
        private int originLevel = 0;
        /// <summary>
        /// 构造函数
        /// </summary>
        public BusinessManagement()
        {
            if (_current == null || _current.IsDisposed)
            {
                lock (lckObj)
                {
                    if (_current == null || _current.IsDisposed)
                    {
                        _current = new BusinessUtility(NHibernateConfigFileName);
                    }
                }
            }
            originLevel = _current.TranLevel;
        }
        /// <summary>
        /// 获取当前NHibernate会话
        /// </summary>
        public ISession Session { get { return _current.Session; } }
        /// <summary>
        /// 事务层级
        /// </summary>
        public int TranLevel { get { return _current.TranLevel; } }
        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTran()
        {
            if (TranLevel > originLevel)
            {
                throw new System.Transactions.TransactionException("多次开启事务");
            }
            _current.BeginTran();
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            if (TranLevel == 0)
            {
                throw new System.Transactions.TransactionException("没开启事务，或者事务已经提交");
            }
            if (TranLevel <= originLevel)
            {
                throw new System.Transactions.TransactionException("多次提交事务");
            }
            _current.CommitTran();
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTran()
        {
            if (TranLevel == 0)
            {
                throw new System.Transactions.TransactionException("没开启事务，或者事务已经提交");
            }
            if (TranLevel <= originLevel)
            {
                throw new System.Transactions.TransactionException("多次回滚事务");
            }
            _current.RollbackTran();
        }
        /// <summary>
        /// 释放资源。如果事务已开启，回滚事务。释放事务，释放会话。
        /// </summary>
        public void Dispose()
        {
            if (!_current.IsDisposed)
            {
                try
                {
                    if (TranLevel != originLevel)
                    {
                        RollbackTran();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("释放BusinessManagement资源异常。TranLevel:{0};originLevel:{1};IsBeginTran:{2};IsCommitTran:{3};IsDisposed:{4};IsRollbackTran:{5};StackTrace:{6}"
                        , TranLevel, originLevel, _current.IsBeginTran, _current.IsCommitTran, _current.IsDisposed, _current.IsRollbackTran, ex.StackTrace), ex);
                }
                finally
                {
                    if (TranLevel == 0)
                    {
                        _current.Dispose();
                    }
                }
            }
        }
    }
    /// <summary>
    /// 事务控制对象
    /// </summary>
    public class BusinessUtility : IDisposable
    {
        /// <summary>
        /// 会话对象
        /// </summary>
        public readonly ISession Session;
        private ITransaction _tran;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cfgFileName">NHibernate配置文件</param>
        public BusinessUtility(string cfgFileName)
        {
            Session = GetSessionFactory(cfgFileName).OpenSession();
        }
        /// <summary>
        /// 事务层级
        /// </summary>
        public int TranLevel { get; private set; }
        /// <summary>
        /// 是否已开启事务
        /// </summary>
        public bool IsBeginTran { get { return _tran != null && _tran.IsActive; } }
        /// <summary>
        /// 是否已提交事务
        /// </summary>
        public bool IsCommitTran { get { return _tran.WasCommitted; } }
        /// <summary>
        /// 是否已回滚事务
        /// </summary>
        public bool IsRollbackTran { get { return _tran.WasRolledBack; } }
        /// <summary>
        /// 是否已释放资源
        /// </summary>
        public bool IsDisposed { get; set; }
        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTran()
        {
            if (TranLevel == 0)
            {
                _tran = Session.BeginTransaction();
            }
            TranLevel++;
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            TranLevel--;
            if (TranLevel == 0)
            {
                _tran.Commit();
            }
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTran()
        {
            try
            {
                if (_tran.IsActive && !_tran.WasCommitted && !_tran.WasRolledBack)
                {
                    _tran.Rollback();
                }
            }
            finally
            {
                TranLevel = 0;
            }
        }
        /// <summary>
        /// 释放资源。如果事务已开启，回滚事务。释放事务，释放会话。
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                if (Transaction.Current == null)
                {
                    Session.Clear();
                    Session.Close();
                    Session.Dispose();
                }
                IsDisposed = true;
            }
        }

        internal static Dictionary<string, ISessionFactory> _factoryList = new Dictionary<string, ISessionFactory>();
        internal static Dictionary<string, Configuration> _cfgList = new Dictionary<string, Configuration>();
        internal static void ClearSession(string cfgFileName)
        {
            _cfgList.Remove(cfgFileName);
            _factoryList.Remove(cfgFileName);
        }
        internal static void AddAssembly(string cfgFileName, string assemName)
        {
            BuildConfiguration(cfgFileName);
            _cfgList[cfgFileName].AddAssembly(assemName);
        }
        internal static void AddAssembly(string cfgFileName, Assembly assem)
        {
            BuildConfiguration(cfgFileName);
            _cfgList[cfgFileName].AddAssembly(assem);
        }
        private static ISessionFactory GetSessionFactory(string cfgFileName)
        {
            if (!_factoryList.ContainsKey(cfgFileName) || _factoryList[cfgFileName] == null || _factoryList[cfgFileName].IsClosed)
            {
                BuildConfiguration(cfgFileName);
                _factoryList[cfgFileName] = _cfgList[cfgFileName].BuildSessionFactory();
            }
            return _factoryList[cfgFileName];
        }
        internal static void BuildConfiguration(string cfgFileName)
        {
            if (!_cfgList.ContainsKey(cfgFileName) || _cfgList[cfgFileName] == null)
            {
                _cfgList[cfgFileName] = new Configuration().Configure(cfgFileName);
            }
        }
    }
}
