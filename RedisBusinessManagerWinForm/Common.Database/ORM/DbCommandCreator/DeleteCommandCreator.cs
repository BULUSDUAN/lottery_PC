using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Common.Database.DbAccess;

namespace Common.Database.ORM
{
    /// <summary>
    /// �õ�ɾ�����ݿ�����DbCommand
    /// </summary>
    internal class DeleteCommandCreator : DbCommandCreator
    {
        /// <summary>
        /// ��Ҫ�޸ĵ�ʵ����󣬲���Ϊ��
        /// </summary>
        private object _entity;
        public object Entity
        {
            get
            {
                return _entity;
            }
            set
            {
                _entity = value;
                EntityType = _entity.GetType();
            }
        }
        public DeleteType deleteType { get; set; }
        public override DbCommand GetDbCommand()
        {
            DbCommand dbCommand = null;
            switch (deleteType)
            {
                case DeleteType.Delete:
                    dbCommand = this.GetDeleteByEntityDbCommand();
                    break;
                case DeleteType.DeleteByCriteria:
                    dbCommand = this.GetDbCommand(this.criteria);
                    break;
            }
            return dbCommand;
        }
        public Type EntityType { get; set; }

        public Criteria criteria { get; set; }
        /// <summary>
        /// ��ʼ��DeleteCommandCreator��dbAccess����Ϊ��
        /// </summary>
        /// <exception cref="RException(DatabaseErrorCode.ConnectDbServerError - 0x00010001)">
        /// �����dbAccessΪ�գ��������쳣
        /// </exception>
        /// <param name="dbAccess">���ݿ���ʽӿ�</param>
        public DeleteCommandCreator(IDbAccess dbAccess)
        {
            DbAccess = dbAccess;
        }
        /// <summary>
        /// ɾ�����ݿ���ĳһ�����DbCommand��deleteCommandCreator������Entity����Ϊ��
        /// </summary>
        /// <exception cref="RException(DatabaseErrorCode.CommandTextIsEmpty - 0x00010102)">
        /// ����EntityΪnullʱ���������쳣
        /// </exception>
        /// <returns>ɾ�����ݿ���ĳһ�����DbCommand��DbCommand�Ĳ���Ϊ�������Entity������</returns>
        private DbCommand GetDeleteByEntityDbCommand()
        {
            TypeSchema entityInfo = ORMSchemaCache.GetTypeSchema(EntityType);
            List<PropertyInfo> fieldPropertyList = null;
            string sqlDelete = string.Format("DELETE FROM {0} {1}"
                , GetQuotedName(entityInfo.MappingTableAttribute.TableName)
                , GetDeleteStatement(entityInfo, out fieldPropertyList));
            DbCommand dbCommand = GetDbCommandByEntity(fieldPropertyList, Entity);
            dbCommand.CommandText = sqlDelete;
            return dbCommand;
        }
        private DbCommand GetDbCommand(Criteria cri)
        {
            TypeSchema entityInfo = ORMSchemaCache.GetTypeSchema(EntityType);
            List<PropertyInfo> fieldPropertyList = new List<PropertyInfo>();
            DbCommand dbCommand = GetDbCommandByEntity(fieldPropertyList, null);

            List<DbParameter> parameters;
            string sqlDelete = string.Format("DELETE FROM {0} WHERE 1=1 {1}"
                , GetQuotedName(entityInfo.MappingTableAttribute.TableName)
                , cri.GenerateExpression(EntityType, out parameters, DbAccess));
            dbCommand.Parameters.AddRange(parameters.ToArray());
            dbCommand.CommandText = sqlDelete;
            return dbCommand;
        }
        private string GetDeleteStatement(TypeSchema entityInfo, out List<PropertyInfo> fieldPropertyList)
        {
            string query = "";
            fieldPropertyList = new List<PropertyInfo>();
            foreach (SchemaItem mfi in entityInfo.GetKeyFieldInfos())
            {
                if (query != "") query += " AND ";
                query += GetQuotedName(mfi.MappingFieldAttribute.FieldName) + "=@" + mfi.ProInfo.Name;
                fieldPropertyList.Add(mfi.ProInfo);
            }
            return string.Format("WHERE {0}", query);
        }
    }
}