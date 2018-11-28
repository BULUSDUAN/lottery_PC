using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Common.Database.DbAccess;
using Common.Utilities;
using Common.Database.Configuration;

namespace Common.Database.ORM
{
    public class ObjectPersistence
    {
        private readonly IDbAccess _dbAccess;

        public ObjectPersistence(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }
        /// <summary>
        /// 通过主键和指定类型从数据库获取对象
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="keyValue">主键值（通常为Guid），不能为空</param>
        /// <returns>从数据库中得到的对象,如果数据库中没找到对象会返回一个空对象</returns>
        /// 前置条件
        /// 1.keyValue不能为空
        /// 2.传入的对象T只能有一个主键
        public T GetByKey<T>(object keyValue)
            where T : new()
        {
            PreconditionAssert.IsNotNull(keyValue, ErrorMessages.NullReferenceException);
            ORMHelper.EntityIsMappingDatabase(typeof(T), ErrorMessages.EntityMappingError);

            SelectCommandCreator dbCommandCreator = new SelectCommandCreator(_dbAccess);
            dbCommandCreator.ObjectType = typeof(T);
            dbCommandCreator.KeyValue = keyValue;
            dbCommandCreator.SelectType = SelectType.GetOneByKeyValue;
            DbCommand dbCommand = dbCommandCreator.GetDbCommand();
            DataTable dt = _dbAccess.GetDataTableByCommand(dbCommand);

            if (dt.Rows.Count == 0)
            {
                return default(T);
            }
            else if (dt.Rows.Count == 1)
            {
                return ORMHelper.ConvertDataRowToEntity<T>(dt.Rows[0]);
            }
            else
            {
                //引发数据获取结果错误
                throw new ORMException(ErrorMessages.ResultNotUniqueMessage);
            }
        }
        /// <summary>
        /// 通过对象已有的主键值（可以多个主键）和指定类型从数据库获取对象
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="entity">外部构造的对象，不能为空</param>
        /// <returns>从数据库中得到的对象,如果数据库中没找到对象会返回一个空对象</returns>
        /// 前置条件
        /// 传入的entity对象不能为空
        public T GetByKeys<T>(T entity) where T : new()
        {
            PreconditionAssert.IsNotNull(entity, ErrorMessages.NullReferenceException);
            ORMHelper.EntityIsMappingDatabase(entity.GetType(), ErrorMessages.EntityMappingError);

            ORMHelper.CheckEntityKey(entity);
            SelectCommandCreator dbCommandCreator = new SelectCommandCreator(_dbAccess);
            dbCommandCreator.Entity = entity;
            dbCommandCreator.SelectType = SelectType.GetOneByEntityKey;
            DbCommand dbCommand = dbCommandCreator.GetDbCommand();
            DataTable dt = _dbAccess.GetDataTableByCommand(dbCommand);
            if (dt.Rows.Count == 0)
            {
                return default(T);
            }
            else if (dt.Rows.Count == 1)
            {
                return ORMHelper.ConvertDataRowToEntity<T>(dt.Rows[0]);
            }
            else
            {
                throw new ORMException(ErrorMessages.ResultNotUniqueMessage);
            }
        }
        /// <summary>
        /// 新增一个实体对象到数据库中
        /// </summary>
        /// <param name="entity">需要保存的实体对象</param>
        /// 前置条件:
        /// 1.参数entity不允许为空
        public void Add(object entity)
        {
            //断言参入的参数为null或者空字符串（RErrorCode.NullReference - 0x00000001）
            PreconditionAssert.IsNotNull(entity, ErrorMessages.NullReferenceException);
            ORMHelper.EntityIsMappingDatabase(entity.GetType(), ErrorMessages.EntityMappingError);
            ORMHelper.CheckEntityIsNotReadOnly(entity.GetType(), ErrorMessages.EntityReadOnly);

            InsertCommandCreator icc = new InsertCommandCreator(_dbAccess);
            icc.Entity = entity;
            DbCommand dbCommand = icc.GetDbCommand();
            _dbAccess.ExecCommand(dbCommand);
        }
        /// <summary>
        /// 修改一个数据库中已有的对象，该对象的主键必须有值
        /// 该对象为空或主键为空或修改失败会抛出异常
        /// </summary>
        /// <param name="entity">需要修改的实体对象</param>
        /// 前置条件
        /// 参数entity不允许为空
        public void Modify(object entity)
        {
            //断言参入的参数为null或者空字符串（RErrorCode.NullReference - 0x00000001）
            PreconditionAssert.IsNotNull(entity, ErrorMessages.NullReferenceException);
            ORMHelper.EntityIsMappingDatabase(entity.GetType(), ErrorMessages.EntityMappingError);
            ORMHelper.CheckEntityIsNotReadOnly(entity.GetType(), ErrorMessages.EntityReadOnly);

            ModifyCommandCreator mcc = new ModifyCommandCreator(_dbAccess);
            mcc.Entity = entity;
            DbCommand dbCommand = mcc.GetDbCommand();
            _dbAccess.ExecCommand(dbCommand);
        }
        /// <summary>
        /// 删除一个数据库中已有的对象，该对象的主键必须有值
        /// 该对象为空或主键为空或删除失败会抛出异常
        /// </summary>
        /// <param name="entity">需要删除的实体</param>
        /// 前置条件
        /// 参数entity不允许为空
        public void Delete(object entity)
        {
            //断言参入的参数为null或者空字符串（RErrorCode.NullReference - 0x00000001）
            PreconditionAssert.IsNotNull(entity, ErrorMessages.NullReferenceException);
            ORMHelper.EntityIsMappingDatabase(entity.GetType(), ErrorMessages.EntityMappingError);
            ORMHelper.CheckEntityIsNotReadOnly(entity.GetType(), ErrorMessages.EntityReadOnly);

            DeleteCommandCreator dcc = new DeleteCommandCreator(_dbAccess);
            dcc.deleteType = DeleteType.Delete;
            dcc.Entity = entity;
            DbCommand dbCommand = dcc.GetDbCommand();
            _dbAccess.ExecCommand(dbCommand);
        }
        /// <summary>
        /// 根据表达式删除指定类型的数据
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="cri">表达式</param>
        /// 前置条件
        /// 参数cri不允许为空
        public void Delete<T>(Criteria cri)
        {
            PreconditionAssert.IsNotNull(cri, ErrorMessages.NullReferenceException);
            ORMHelper.CheckEntityIsNotReadOnly(typeof(T), ErrorMessages.EntityReadOnly);

            DeleteCommandCreator dcc = new DeleteCommandCreator(_dbAccess);
            dcc.EntityType = typeof(T);
            dcc.criteria = cri;
            dcc.deleteType = DeleteType.DeleteByCriteria;
            DbCommand dbCommand = dcc.GetDbCommand();
            _dbAccess.ExecCommand(dbCommand);
        }
        /// <summary>
        /// 获取数据表中的所有数据集合，并按指定排序方式进行排序
        /// </summary>
        /// <typeparam name="T">指定的类型</typeparam>
        /// <param name="orderBy">排序方式</param>
        /// <returns>数据表中的所有数据集合</returns>
        /// 前置条件
        /// 传入的orderBy对象可以为空，为空的时候则不进行排序
        public IList<T> GetAll<T>(params SortInfo[] orderBy) where T : new()
        {
            PreconditionAssert.IsNotNull(orderBy, ErrorMessages.NullReferenceException);
            ORMHelper.EntityIsMappingDatabase(typeof(T), ErrorMessages.EntityMappingError);

            SelectCommandCreator dbCommandCreator = new SelectCommandCreator(_dbAccess);
            dbCommandCreator.ObjectType = typeof(T);
            dbCommandCreator.OrderBy = orderBy;
            dbCommandCreator.SelectType = SelectType.GetAll;
            DbCommand dbCommand = dbCommandCreator.GetDbCommand();

            DataTable dt = _dbAccess.GetDataTableByCommand(dbCommand);
            return ORMHelper.DataTableToList<T>(dt);
        }
        /// <summary>
        /// 通过指定类型,过滤条件和排序方式从数据库中获取该指定类型的数据集合
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="filterProterties">字段名称，可包含多个</param>
        /// <param name="orderBy">排序方式</param>
        /// <returns>指定类型的数据集合</returns>
        /// 前置条件
        /// 传入的参数entity，filterProterties不允许为空
        /// orderBy可以为空，如果为空则不进行排序
        public IList<T> GetList<T>(T entity, string[] filterProterties, params SortInfo[] orderBy)
            where T : new()
        {
            //断言传入的entity对象为null或者为空字符串，出现空引用异常
            PreconditionAssert.IsNotNull(entity, ErrorMessages.NullReferenceException);
            PreconditionAssert.IsNotNull(filterProterties, ErrorMessages.NullReferenceException);
            ORMHelper.EntityIsMappingDatabase(entity.GetType(), ErrorMessages.EntityMappingError);

            SelectCommandCreator dbCommandCreator = new SelectCommandCreator(_dbAccess);
            dbCommandCreator.Entity = entity;
            dbCommandCreator.FilterProterties = filterProterties;
            dbCommandCreator.OrderBy = orderBy;
            dbCommandCreator.SelectType = SelectType.GetList;
            DbCommand dbCommand = dbCommandCreator.GetDbCommand();
            DataTable dt = _dbAccess.GetDataTableByCommand(dbCommand);
            return ORMHelper.DataTableToList<T>(dt);
        }
        public IList<T> GetList<T>(string sql, params object[] paramList)
            where T : new()
        {
            var dt = _dbAccess.GetDataTableBySQL(sql, paramList);
            return ORMHelper.DataTableToList<T>(dt);
        }
        public DataTable GetTable(string sql)
        {
            var dt = _dbAccess.GetDataTableBySQL(sql, new object[] { });
            return dt;
        }
        /// <summary>
        /// 根据表达式，查询对象集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="criteria">表达式</param>
        /// <param name="orderBy">排序</param>
        /// <returns>指定类型的数据集合</returns>
        public IList<T> GetList<T>(Criteria criteria, params SortInfo[] orderBy)
            where T : new()
        {
            PreconditionAssert.IsNotNull(criteria, ErrorMessages.NullReferenceException);

            SelectCommandCreator dbCommandCreator = new SelectCommandCreator(_dbAccess);
            dbCommandCreator.OrderBy = orderBy;
            dbCommandCreator.ObjectType = typeof(T);
            dbCommandCreator.Cri = criteria;
            dbCommandCreator.SelectType = SelectType.GetListByExpression;
            DbCommand dbCommand = dbCommandCreator.GetDbCommand();
            DataTable dt = _dbAccess.GetDataTableByCommand(dbCommand);
            return ORMHelper.DataTableToList<T>(dt);
        }
        /// <summary>
        /// 通过指定类型,过滤条件和排序方式从数据库中获取该指定类型的数据条数
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="filterProterties">过滤条件</param>
        /// <returns>指定类型的数据条数</returns>
        /// 前置条件
        /// 传入的参数entity不允许为空
        /// filterProterties,orderBy可以为空，如果为空则没有where条件查询和不进行排序
        public int GetCount(object entity, string[] filterProterties)
        {
            //断言传入的entity对象为null或者为空字符串，出现空引用异常
            PreconditionAssert.IsNotNull(entity, ErrorMessages.NullReferenceException);
            ORMHelper.EntityIsMappingDatabase(entity.GetType(), ErrorMessages.EntityMappingError);

            SelectCommandCreator dbCommandCreator = new SelectCommandCreator(_dbAccess);
            dbCommandCreator.Entity = entity;
            dbCommandCreator.FilterProterties = filterProterties;
            dbCommandCreator.SelectType = SelectType.GetCount;
            DbCommand dbCommand = dbCommandCreator.GetDbCommand();
            return (int)_dbAccess.GetRC1ByCommand(dbCommand);
        }
        /// <summary>
        /// 获取数据库中所有的数据总数
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <returns>数据总数</returns>
        public int GetAllCount<T>()
        {
            ORMHelper.EntityIsMappingDatabase(typeof(T), ErrorMessages.EntityMappingError);

            SelectCommandCreator dbCommandCreator = new SelectCommandCreator(_dbAccess);
            dbCommandCreator.ObjectType = typeof(T);
            dbCommandCreator.SelectType = SelectType.GetAllCount;
            DbCommand dbCommand = dbCommandCreator.GetDbCommand();
            return (int)_dbAccess.GetRC1ByCommand(dbCommand);
        }
    }
    /// <summary>
    /// 排序信息
    /// </summary>
    public class SortInfo
    {
        /// <summary>
        /// 通过传入的字段名确定要排序的列,初始化排序对象实例
        /// </summary>
        /// <param name="propertyName">指定的要排序的属性名称，不允许为null或空串</param>
        /// <exception cref="RException(RErrorCode.ArgmentesError - 0x00000014)">
        /// 以下几种情况会引发该异常：
        /// 1.传入的propertyName为null或空串时。
        /// </exception>
        public SortInfo(string propertyName)
            : this(propertyName, SortDirection.Asc)
        {
        }
        /// <summary>
        /// 通过传入的字段名称和排序方式确定要排序的列和排序的方式,初始化排序对象实例
        /// </summary>
        /// <param name="propertyName">要排序的属性名称，不允许为null或空串</param>
        /// <param name="direction">排序方向</param>
        /// <exception cref="RException(RErrorCode.ArgmentesError - 0x00000014)">
        /// 以下几种情况会引发该异常：
        /// 1.传入的propertyName为null或空串时。
        /// </exception>
        public SortInfo(string propertyName, SortDirection direction)
        {
            PropertyName = propertyName;
            Direction = direction;
        }
        /// <summary>
        /// 要排序的属性名称
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 排序方向
        /// </summary>
        public SortDirection Direction { get; set; }
    }
}