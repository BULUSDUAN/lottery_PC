using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using System.Linq.Expressions;
using System.Dynamic;
using System.Data.Common;
using System.Data;
using System.Collections;

namespace Common.Database.NHibernate
{
    /// <summary>
    /// 实体类查询
    /// GetXXX查询单个对象，如果找不到返回null
    /// SingeXXX查询单个对象，如果找不到抛出异常
    /// LoadXXX延迟加载单个对象
    /// QueryXXX查询对象集合
    /// </summary>
    public abstract class EntityManagement<BizType> : IDisposable where BizType : BusinessManagement, new()
    {
        private bool _isCreateSessionBySelf = false;
        private BusinessManagement biz = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        public EntityManagement()
        {
            if (BusinessManagement.Current == null || BusinessManagement.Current.IsDisposed)
            {
                biz = BusinessManagement.CreateInstance<BizType>();
                _isCreateSessionBySelf = true;
            }
        }
        /// <summary>
        /// 事务控制对象
        /// </summary>
        public BusinessManagement Business
        {
            get { return biz; }
        }
        /// <summary>
        /// 会话
        /// </summary>
        public ISession Session
        {
            get { return BusinessManagement.Current.Session; }
        }
        /// <summary>
        /// 当前上下文是否在事务中
        /// </summary>
        public bool IsInTransaction
        {
            get { return BusinessManagement.Current.IsBeginTran; }
        }

        private bool _isDispose = false;
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_isDispose)
            {
                if (_isCreateSessionBySelf)
                {
                    biz.Dispose();
                }
                _isDispose = true;
            }
        }

        #region 增删改

        protected void Add<T>(params T[] entity)
        {
            foreach (var e in entity)
            {
                Session.Save(e);
            }
            Session.Flush();
        }
        protected void AddOrIgnore<T>(params T[] entity)
        {
            foreach (var e in entity)
            {
                Session.Persist(e);
                var id = Session.GetIdentifier(e);
                if (Session.Get<T>(id) == null)
                {
                    Session.Save(e);
                }
            }
            Session.Flush();
        }
        protected void Update<T>(params T[] entity)
        {
            Session.Clear();
            foreach (var e in entity)
            {
                Session.Update(e);
            }
            Session.Flush();
        }
        protected void SaveOrUpdate<T>(params T[] entity)
        {
            Session.Clear();
            foreach (var e in entity)
            {
                Session.SaveOrUpdate(e);
            }
            Session.Flush();
        }
        protected void Delete<T>(params T[] entity)
        {
            foreach (var e in entity)
            {
                Session.Delete(e);
            }
            Session.Flush();
        }

        #endregion

        protected OutputQuery CreateOutputQuery(IQuery query)
        {
            return new OutputQuery(Session, query);
        }

        #region 查询单个对象

        protected T GetByKey<T>(object key)
        {
            Session.Clear();
            return Session.Get<T>(key, LockMode.Read);
        }
        protected T LoadByKey<T>(object key)
        {
            Session.Clear();
            return Session.Load<T>(key, LockMode.Read);
        }
        protected T SingleByKey<T>(object key)
        {
            Session.Clear();
            var entity = Session.Get<T>(key, LockMode.Read);
            if (entity == null)
            {
                throw new ObjectNotFoundException(key, typeof(T));
            }
            return entity;
        }

        #endregion

        #region 查询对象集合

        private IList<T> QueryByProperty<T>(string propertyName, object value, int pageIndex, int pageSize, bool isCacheable = false) where T : class
        {
            Session.Clear();
            return Session.CreateCriteria<T>()
                .Add(Restrictions.Eq(propertyName, value))
                .SetFirstResult(pageIndex * pageSize)
                .SetMaxResults(pageSize)
                .SetCacheable(isCacheable)
                .List<T>();
        }

        private IList<T> QueryByProperty<T>(string propertyName, object value, bool isCacheable = false) where T : class
        {
            Session.Clear();
            return QueryByProperty<T>(propertyName, value, 0, int.MaxValue, isCacheable);
        }

        private IList<T> QueryAll<T>(int pageIndex, int pageSize, bool isCacheable = false) where T : class
        {
            Session.Clear();
            return Session.CreateCriteria<T>()
                .SetFirstResult(pageIndex * pageSize)
                .SetMaxResults(pageSize)
                .SetCacheable(isCacheable)
                .List<T>();
        }

        private IList<T> QueryAll<T>(bool isCacheable = false) where T : class
        {
            Session.Clear();
            return QueryAll<T>(0, int.MaxValue, isCacheable);
        }

        private IList<T> QueryByCriteria<T>(Func<ICriteria, ICriteria> criteriaFunc) where T : class
        {
            Session.Clear();
            return criteriaFunc(Session.CreateCriteria<T>())
                .List<T>();
        }

        private IList<T> QueryByCriteria<T>(Func<ICriteria, ICriteria> criteriaFunc, int pageIndex, int pageSize, bool isCacheable = false) where T : class
        {
            Session.Clear();
            return criteriaFunc(Session.CreateCriteria<T>())
                .SetFirstResult(pageIndex * pageSize)
                .SetMaxResults(pageSize)
                .SetCacheable(isCacheable)
                .List<T>();
        }

        private IList<T> QueryByCriteria<T>(ICriterion expression, Order order, int pageIndex, int pageSize, bool isCacheable = false) where T : class
        {
            //crit.Add(Restrictions.Eq("", ""));
            //Eq：这是一个相等判断的表达式；
            //Like：这是一个like判断的表达式；
            //Gt：这是一个大于判断的表达式；
            //And：这是两个表达式And操作后的表达式；
            //Or：这是两个表达式Or操作后的表达式；
            //Between：这是一个范围筛选的条件表达式，在两个数之间的范围。
            //In：这也是一个范围筛选的条件表达式，在多个离散的值中进行筛选。
            Session.Clear();
            return Session.CreateCriteria<T>()
                .Add(expression: expression)
                .AddOrder(order: order)
                .SetFirstResult(pageIndex * pageSize)
                .SetMaxResults(pageSize)
                .SetCacheable(isCacheable)
                .List<T>();
        }

        private IList<T> QueryByCriteria<T>(ICriterion expression, Order order, bool isCacheable = false) where T : class
        {
            Session.Clear();
            return QueryByCriteria<T>(expression, order, 0, int.MaxValue, isCacheable);
        }

        private IList<T> QueryByHQL<T>(string hql, int pageIndex, int pageSize, bool isCacheable = false)
        {
            Session.Clear();
            return Session.CreateQuery(hql)
                .SetFirstResult(pageIndex * pageSize)
                .SetMaxResults(pageSize)
                .SetCacheable(isCacheable)
                .List<T>();
        }

        private IList<T> QueryByHQL<T>(string hql, bool isCacheable = false)
        {
            return QueryByHQL<T>(hql, 0, int.MaxValue, isCacheable);
        }

        private IList<T> QueryBySQL<T>(string hql, int pageIndex, int pageSize, bool isCacheable = false)
        {
            Session.Clear();
            return Session.CreateSQLQuery(hql)
                .SetFirstResult(pageIndex * pageSize)
                .SetMaxResults(pageSize)
                .SetCacheable(isCacheable)
                .List<T>();
        }

        private IList<T> QueryBySQL<T>(string sql, bool isCacheable = false)
        {
            return QueryBySQL<T>(sql, 0, int.MaxValue, isCacheable);
        }

        private IList<T> QueryByExpression<T>(Expression<Func<T, bool>> alias, int pageIndex, int pageSize) where T : class
        {
            Session.Clear();
            return Session.QueryOver<T>().Where(alias).Skip(pageIndex * pageSize).Take(pageSize).Cacheable().List<T>();
        }

        private IList<T> QueryByExpression<T>(Expression<Func<T, bool>> alias) where T : class
        {
            Session.Clear();
            return QueryByExpression<T>(alias, 0, int.MaxValue);
        }

        #endregion
    }
}
