using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Common.Database.DbAccess;

namespace Common.Database.ORM
{
    /// <summary>
    /// DbCommand生成器的抽象类，所有生成DbCommand的类。
    /// 此类提供对生成DbCommand的基础函数。
    /// </summary>
    internal abstract class DbCommandCreator
    {
        protected IDbAccess DbAccess { get; set; }

        public abstract DbCommand GetDbCommand();

        protected DbCommand GetDbCommandByKeyValue(TypeSchema entityInfo, object keyValue)
        {
            DbCommand dbCommand = DbAccess.CreateDbCommand();

            foreach (SchemaItem mfi in entityInfo.GetKeyFieldInfos())
            {
                Type type = mfi.ProInfo.PropertyType;
                type = Nullable.GetUnderlyingType(type) ?? type;
                DbType dbType = ORMHelper.GetDbTypeByName(type.Name);
                DbParameter parameter = DbAccess.CreateDbParameter();
                parameter.ParameterName = "@" + mfi.ProInfo.Name;
                parameter.DbType = dbType;

                //parameter.Value = Convert.ChangeType(keyValue, fieldProperty.PropertyType);
                parameter.Value = Convert.ChangeType(keyValue, type);
                dbCommand.Parameters.Add(parameter);
            }
            return dbCommand;
        }

        protected DbCommand GetDbCommandByEntity(List<PropertyInfo> fieldPropertyList, object entity)
        {
            DbCommand dbCommand = DbAccess.CreateDbCommand();

            foreach (PropertyInfo fieldProperty in fieldPropertyList)
            {
                DbParameter parameter = GetDbParameter(fieldProperty, entity);
                dbCommand.Parameters.Add(parameter);
            }
            return dbCommand;
        }

        protected DbParameter GetDbParameter(PropertyInfo fieldProperty, object entity)
        {
            DbParameter parameter = DbAccess.CreateDbParameter();
            Type tempType = fieldProperty.PropertyType;
            DbType dbType;
            if (tempType.IsEnum)
            {
                dbType = DbType.Int32;
            }
            else
            {
                if (Nullable.GetUnderlyingType(tempType) != null)//如果是 int? 类型则进行处理 
                {
                    tempType = Nullable.GetUnderlyingType(tempType);
                }
                dbType = ORMHelper.GetDbTypeByName(tempType.Name);
            }
            parameter.ParameterName = "@" + fieldProperty.Name;
            parameter.DbType = dbType;
            parameter.Value = fieldProperty.GetValue(entity, null) ?? DBNull.Value;

            return parameter;
        }

        protected string GetQuotedName(string objName)
        {
            // 例如：[TableName]
            return DbAccess.GetDbObjectName(objName);
        }
    }
}