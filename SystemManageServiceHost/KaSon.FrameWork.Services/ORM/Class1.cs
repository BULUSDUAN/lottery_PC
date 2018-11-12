using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KaSon.FrameWork.Services.ORM
{
    public interface IDal<T> where T : class
    {
        /// <summary>
        /// 插入一个实体对象到数据库，并判断是否返回自增长Id
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="getId">是否返回自增长Id</param>
        void Add(T entity, bool getId = false);
        /// <summary>
        /// 插入一批实体对象到数据库，并判断是否返回自增长Id
        /// </summary>
        /// <param name="entitylist">实体对象列表</param>
        /// <param name="getId">是否返回自增长Id</param>
        void Add(IList<T> entitylist, bool getId = false);

        /// <summary>
        /// 高效批量录入
        /// </summary>
        /// <param name="entitylist"></param>
        /// <param name="getId"></param>
        void BulkAdd(IList<T> entitylist);

        /// <summary>
        /// 根据条件表达式获得指定记录的条数
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <param name="nolock">是否允许脏读</param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> exp, bool nolock = false);
        /// <summary>
        /// 根据实体的主键自动删除对应数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        int Delete(T entity);
        /// <summary>
        /// 根据一批实体的主键自动删除对应数据
        /// </summary>
        /// <param name="entitylist">实体对象列表</param>
        int Delete(IList<T> entitylist);
        /// <summary>
        /// 根据指定的条件表达式自动删除对应数据
        /// </summary>
        /// <param name="exp"></param>
        int Delete(Expression<Func<T, bool>> exp);
        /// <summary>
        /// 根据条件表达式获得实体对象
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <param name="nolock">是否允许脏读</param>
        /// <returns></returns>
        T Get(Expression<Func<T, bool>> exp, bool nolock = false);
        /// <summary>
        /// 根据条件表达式获得实体对象指定属性的值
        /// </summary>
        /// <param name="select">条件表达式</param>
        /// <param name="where">条件表达式</param>
        /// <param name="nolock">是否允许脏读</param>
        /// <returns></returns>
        object Get(Expression<Func<T, object>> select, Expression<Func<T, bool>> where, bool nolock = false);
        /// <summary>
        /// 根据条件表达式获得实体对象列表
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <param name="nolock">是否允许脏读</param>
        /// <returns></returns>
        IList<T> GetList(Expression<Func<T, bool>> exp, bool nolock = false);
        /// <summary>
        /// 根据条件表达式获得数据并映射到指定实体对象列表
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <param name="nolock">是否允许脏读</param>
        /// <returns></returns>
        IList<TResult> GetList<TResult>(Expression<Func<T, bool>> exp, bool nolock = false) where TResult : new();
        /// <summary>
        /// 根据条件表达式获取某列的和
        /// </summary>
        /// <param name="select"></param>
        /// <param name="where"></param>
        /// <param name="noLock"></param>
        /// <returns></returns>
        object Sum<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> where, bool noLock = false);

        /// <summary>
        /// 根据实体的主键自动更新对应数据,改变的数据才更新,还没实现
        /// </summary>
        /// <param name="entity"></param>
        int UpdateChange(T entity);

        /// <summary>
        /// 根据实体的主键自动更新对应数据,改变的数据才更新,还没实现
        /// </summary>
        /// <param name="entity"></param>
        int EmptyNotUpdate(T entity);

        /// <summary>
        /// 根据实体的主键自动更新对应数据
        /// </summary>
        /// <param name="entity"></param>
        int Update(T entity);
        /// <summary>
        /// 根据一批实体的主键自动更新对应数据
        /// </summary>
        /// <param name="entitylist"></param>
        int Update(IList<T> entitylist);
        /// <summary>
        /// 根据条件表达式将映射对象的数据进行局部更新
        /// </summary>
        /// <param name="update"></param>
        /// <param name="where"></param>
        int Update<TResult>(Expression<Func<T, TResult>> update, Expression<Func<T, bool>> where) where TResult : class;
    }
}
