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
        /// ͨ��������ָ�����ʹ����ݿ��ȡ����
        /// </summary>
        /// <typeparam name="T">ָ������</typeparam>
        /// <param name="keyValue">����ֵ��ͨ��ΪGuid��������Ϊ��</param>
        /// <returns>�����ݿ��еõ��Ķ���,������ݿ���û�ҵ�����᷵��һ���ն���</returns>
        /// ǰ������
        /// 1.keyValue����Ϊ��
        /// 2.����Ķ���Tֻ����һ������
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
                //�������ݻ�ȡ�������
                throw new ORMException(ErrorMessages.ResultNotUniqueMessage);
            }
        }
        /// <summary>
        /// ͨ���������е�����ֵ�����Զ����������ָ�����ʹ����ݿ��ȡ����
        /// </summary>
        /// <typeparam name="T">ָ������</typeparam>
        /// <param name="entity">�ⲿ����Ķ��󣬲���Ϊ��</param>
        /// <returns>�����ݿ��еõ��Ķ���,������ݿ���û�ҵ�����᷵��һ���ն���</returns>
        /// ǰ������
        /// �����entity������Ϊ��
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
        /// ����һ��ʵ��������ݿ���
        /// </summary>
        /// <param name="entity">��Ҫ�����ʵ�����</param>
        /// ǰ������:
        /// 1.����entity������Ϊ��
        public void Add(object entity)
        {
            //���Բ���Ĳ���Ϊnull���߿��ַ�����RErrorCode.NullReference - 0x00000001��
            PreconditionAssert.IsNotNull(entity, ErrorMessages.NullReferenceException);
            ORMHelper.EntityIsMappingDatabase(entity.GetType(), ErrorMessages.EntityMappingError);
            ORMHelper.CheckEntityIsNotReadOnly(entity.GetType(), ErrorMessages.EntityReadOnly);

            InsertCommandCreator icc = new InsertCommandCreator(_dbAccess);
            icc.Entity = entity;
            DbCommand dbCommand = icc.GetDbCommand();
            _dbAccess.ExecCommand(dbCommand);
        }
        /// <summary>
        /// �޸�һ�����ݿ������еĶ��󣬸ö��������������ֵ
        /// �ö���Ϊ�ջ�����Ϊ�ջ��޸�ʧ�ܻ��׳��쳣
        /// </summary>
        /// <param name="entity">��Ҫ�޸ĵ�ʵ�����</param>
        /// ǰ������
        /// ����entity������Ϊ��
        public void Modify(object entity)
        {
            //���Բ���Ĳ���Ϊnull���߿��ַ�����RErrorCode.NullReference - 0x00000001��
            PreconditionAssert.IsNotNull(entity, ErrorMessages.NullReferenceException);
            ORMHelper.EntityIsMappingDatabase(entity.GetType(), ErrorMessages.EntityMappingError);
            ORMHelper.CheckEntityIsNotReadOnly(entity.GetType(), ErrorMessages.EntityReadOnly);

            ModifyCommandCreator mcc = new ModifyCommandCreator(_dbAccess);
            mcc.Entity = entity;
            DbCommand dbCommand = mcc.GetDbCommand();
            _dbAccess.ExecCommand(dbCommand);
        }
        /// <summary>
        /// ɾ��һ�����ݿ������еĶ��󣬸ö��������������ֵ
        /// �ö���Ϊ�ջ�����Ϊ�ջ�ɾ��ʧ�ܻ��׳��쳣
        /// </summary>
        /// <param name="entity">��Ҫɾ����ʵ��</param>
        /// ǰ������
        /// ����entity������Ϊ��
        public void Delete(object entity)
        {
            //���Բ���Ĳ���Ϊnull���߿��ַ�����RErrorCode.NullReference - 0x00000001��
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
        /// ���ݱ��ʽɾ��ָ�����͵�����
        /// </summary>
        /// <typeparam name="T">ָ������</typeparam>
        /// <param name="cri">���ʽ</param>
        /// ǰ������
        /// ����cri������Ϊ��
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
        /// ��ȡ���ݱ��е��������ݼ��ϣ�����ָ������ʽ��������
        /// </summary>
        /// <typeparam name="T">ָ��������</typeparam>
        /// <param name="orderBy">����ʽ</param>
        /// <returns>���ݱ��е��������ݼ���</returns>
        /// ǰ������
        /// �����orderBy�������Ϊ�գ�Ϊ�յ�ʱ���򲻽�������
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
        /// ͨ��ָ������,��������������ʽ�����ݿ��л�ȡ��ָ�����͵����ݼ���
        /// </summary>
        /// <typeparam name="T">ָ������</typeparam>
        /// <param name="entity">ʵ�����</param>
        /// <param name="filterProterties">�ֶ����ƣ��ɰ������</param>
        /// <param name="orderBy">����ʽ</param>
        /// <returns>ָ�����͵����ݼ���</returns>
        /// ǰ������
        /// ����Ĳ���entity��filterProterties������Ϊ��
        /// orderBy����Ϊ�գ����Ϊ���򲻽�������
        public IList<T> GetList<T>(T entity, string[] filterProterties, params SortInfo[] orderBy)
            where T : new()
        {
            //���Դ����entity����Ϊnull����Ϊ���ַ��������ֿ������쳣
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
        /// ���ݱ��ʽ����ѯ���󼯺�
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="criteria">���ʽ</param>
        /// <param name="orderBy">����</param>
        /// <returns>ָ�����͵����ݼ���</returns>
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
        /// ͨ��ָ������,��������������ʽ�����ݿ��л�ȡ��ָ�����͵���������
        /// </summary>
        /// <param name="entity">ʵ�����</param>
        /// <param name="filterProterties">��������</param>
        /// <returns>ָ�����͵���������</returns>
        /// ǰ������
        /// ����Ĳ���entity������Ϊ��
        /// filterProterties,orderBy����Ϊ�գ����Ϊ����û��where������ѯ�Ͳ���������
        public int GetCount(object entity, string[] filterProterties)
        {
            //���Դ����entity����Ϊnull����Ϊ���ַ��������ֿ������쳣
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
        /// ��ȡ���ݿ������е���������
        /// </summary>
        /// <typeparam name="T">ָ������</typeparam>
        /// <returns>��������</returns>
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
    /// ������Ϣ
    /// </summary>
    public class SortInfo
    {
        /// <summary>
        /// ͨ��������ֶ���ȷ��Ҫ�������,��ʼ���������ʵ��
        /// </summary>
        /// <param name="propertyName">ָ����Ҫ������������ƣ�������Ϊnull��մ�</param>
        /// <exception cref="RException(RErrorCode.ArgmentesError - 0x00000014)">
        /// ���¼���������������쳣��
        /// 1.�����propertyNameΪnull��մ�ʱ��
        /// </exception>
        public SortInfo(string propertyName)
            : this(propertyName, SortDirection.Asc)
        {
        }
        /// <summary>
        /// ͨ��������ֶ����ƺ�����ʽȷ��Ҫ������к�����ķ�ʽ,��ʼ���������ʵ��
        /// </summary>
        /// <param name="propertyName">Ҫ������������ƣ�������Ϊnull��մ�</param>
        /// <param name="direction">������</param>
        /// <exception cref="RException(RErrorCode.ArgmentesError - 0x00000014)">
        /// ���¼���������������쳣��
        /// 1.�����propertyNameΪnull��մ�ʱ��
        /// </exception>
        public SortInfo(string propertyName, SortDirection direction)
        {
            PropertyName = propertyName;
            Direction = direction;
        }
        /// <summary>
        /// Ҫ�������������
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public SortDirection Direction { get; set; }
    }
}