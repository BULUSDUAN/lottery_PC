using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Common.Utilities;
using Common.Database.DbAccess;
using Common.Mappings;

namespace Common.Database.ORM
{
    public static class ORMHelper
    {
        public static string GetEntityMappingTableName(Type entityType)
        {
            TypeSchema entityInfo = ORMSchemaCache.GetTypeSchema(entityType);
            string tableName = entityInfo.MappingTableAttribute.TableName;
            return tableName;
        }
        public static DbType GetDbTypeByName(string typeName)
        {
            // 根据数据的数据类型，获取对应数据库字段类型
            switch (typeName)
            {
                case "Byte[]":
                    return DbType.Binary;
                default:
                    return (DbType)Enum.Parse(typeof(DbType), typeName, true);
            }
        }
        public static void CheckEntityKey(object entity)
        {
            TypeSchema entityInfo = ORMSchemaCache.GetTypeSchema(entity.GetType());

            foreach (SchemaItem mfi in entityInfo.GetKeyFieldInfos())
            {
                if (mfi.ProInfo.GetValue(entity, null) == null)//如果对象的属性为null，则把此参数设置为DBNull
                {
                    throw new ORMException(ErrorMessages.PrimaryKeyIsNull);
                }
            }
        }
        public static string GetEntityInfoMessage(object entity)
        {
            TypeSchema entityInfo = ORMSchemaCache.GetTypeSchema(entity.GetType());
            StringBuilder infoBuilder = new StringBuilder();
            infoBuilder.AppendLine("Entity Type: " + entity.GetType().FullName);
            foreach (SchemaItem mfi in entityInfo.GetKeyFieldInfos())
            {
                PropertyInfo property = mfi.ProInfo;
                infoBuilder.AppendLine("[" + property.Name + "]: " + property.GetValue(entity, null));
            }
            return infoBuilder.ToString();
        }
        public static string GetOrderInfoMessage(SortInfo[] orderBy)
        {
            StringBuilder infoBuilder = new StringBuilder();
            infoBuilder.AppendLine("Order Field Number: " + (orderBy == null ? "0" : orderBy.Length.ToString()));
            if (orderBy != null)
            {
                foreach (SortInfo sort in orderBy)
                {
                    if (sort == null)
                    {
                        infoBuilder.AppendLine("null");
                    }
                    else
                    {
                        infoBuilder.AppendLine(sort.PropertyName + "[" + sort.Direction.ToString() + "]");
                    }
                }
            }
            return infoBuilder.ToString();
        }
        public static string GetCriteriaMessage(Criteria criteria)
        {
            StringBuilder infoBuilder = new StringBuilder();
            foreach (Expression exp in criteria._expressionList)
            {
                if (exp == null)
                {
                    infoBuilder.AppendLine("null");
                }
                else
                {
                    infoBuilder.AppendLine(exp.GetDescription());
                }
            }
            return infoBuilder.ToString();
        }
        public static IList<T> DataTableToList<T>(DataTable dataTable) where T : new()
        {
            List<T> list = new List<T>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                T t = ConvertDataRowToEntity<T>(dataTable.Rows[i]);
                list.Add(t);
            }
            return list;
        }
        /// <summary>
        /// 将数据库中的行数据转换成一个实体对象
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="dataRow">数据库中的行数据</param>
        /// <returns>指定类型的实体对象</returns>
        public static T ConvertDataRowToEntity<T>(DataRow dataRow) where T : new()
        {
            //断言传入的datarow不能为null
            PreconditionAssert.IsNotNull(dataRow, ErrorMessages.NullReferenceException);

            T tempT = new T();
            Type entityType = typeof(T);
            string fieldName;
            TypeSchema entityInfo = ORMSchemaCache.GetTypeSchema(entityType);
            foreach (SchemaItem mfi in entityInfo.GetAllFieldInfos())
            {
                fieldName = mfi.MappingFieldAttribute.FieldName;
                if (string.IsNullOrEmpty(fieldName))
                    continue;

                if (dataRow.Table.Columns.Contains(fieldName))
                {
                    if (dataRow[fieldName].Equals(DBNull.Value))
                    {
                        mfi.ProInfo.SetValue(tempT, null, null);
                    }
                    else
                    {
                        Type type = Nullable.GetUnderlyingType(mfi.ProInfo.PropertyType) ?? mfi.ProInfo.PropertyType;
                        mfi.ProInfo.SetValue(tempT, dataRow[fieldName], null);
                    }
                }
            }
            return tempT;
        }
        public static void EntityIsMappingDatabase(Type type, string message)
        {
            TypeSchema entityInfo = ORMSchemaCache.GetTypeSchema(type);

            if (entityInfo.MappingTableAttribute == null)
            {
                throw new ORMException(message);
            }
            if (entityInfo.GetKeyFieldInfos() == null || entityInfo.GetKeyFieldInfos().Count == 0)
            {
                throw new ORMException(message);
            }
        }
        public static void CheckEntityIsNotReadOnly(Type type, string message)
        {
            object[] tableAttr = type.GetCustomAttributes(typeof(EntityMappingTableAttribute), true);
            EntityMappingTableAttribute attr = tableAttr[0] as EntityMappingTableAttribute;
            if (attr == null || attr.ReadOnly == true)
            {
                throw new ORMException(message);
            }
        }

        /// <summary>
        /// 表转换为info对象集合
        /// </summary>
        public static IList<T> DataTableToInfoList<T>(DataTable dataTable) where T : new()
        {
            List<T> list = new List<T>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                T t = ConvertDataRowToInfo<T>(dataTable.Rows[i]);
                list.Add(t);
            }
            return list;
        }

        public static T ConvertDataRowToInfo<T>(DataRow dataRow) where T : new()
        {
            //断言传入的datarow不能为null
            PreconditionAssert.IsNotNull(dataRow, ErrorMessages.NullReferenceException);

            Type infoType = typeof(T);
            T tempT = new T();
            //var tempT = Activator.CreateInstance(infoType);
            foreach (var item in infoType.GetProperties())
            {
                var propertyName = item.Name;
                if (!dataRow.Table.Columns.Contains(propertyName))
                    continue;

                if (dataRow[propertyName].Equals(DBNull.Value))
                {
                    item.SetValue(tempT, null, null);
                }
                else
                {
                    //Type type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType;
                    item.SetValue(tempT, dataRow[propertyName], null);
                }
            }
            return tempT;
        }

    }
}