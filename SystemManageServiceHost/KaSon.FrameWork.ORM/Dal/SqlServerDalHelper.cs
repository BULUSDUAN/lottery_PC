namespace KaSon.FrameWork.ORM.Dal
{
    using KaSon.FrameWork.ORM.Provider;
    using KaSon.FrameWork.Services.ORM;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq.Expressions;
    using System.Text;

    internal class SqlServerDalHelper : DalHelper
    {
        private readonly DbProvider _dbProvider;

        public SqlServerDalHelper(DbProvider dbProvider)
        {
            this._dbProvider = dbProvider;
        }

        private void BuilderInsertParameter(KaSon.FrameWork.Services.ORM.DbParameterCollection paramerters, object entity, EntityInfo info, int count)
        {
            foreach (KeyValuePair<string, FieldMap> pair in info.Fields)
            {
                if (pair.Value.Identity == null)
                {
                    if (pair.Value.Version != null)
                    {
                        string name = pair.Key + count;
                        paramerters.Insert(name, pair.Value.Version.InitialValue, ParameterDirection.Input);
                    }
                    else
                    {
                        string introduced4 = pair.Key + count;
                        paramerters.Insert(introduced4, pair.Value.Property.Get(entity) ?? DBNull.Value, ParameterDirection.Input);
                    }
                }
            }
        }

        public override object ExcuteAddWithGetId(string sql,KaSon.FrameWork.Services.ORM.DbParameterCollection paramerters)
        {
            return this._dbProvider.GetDbHelper().ExecuteScalar(sql, paramerters);
        }
        /// <summary>
        /// ÅúÁ¿Â¼Èë
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        public override void ExcuteBulkAdd<T>(IList<T> entitys)
        {
            Type entityType = typeof(T);
            EntityInfo entityInfo = EntityHelper.GetEntityInfo(entityType);
            long num = 0L;
            DataTable dataSource = new DataTable();
            foreach (KeyValuePair<string, FieldMap> pair in entityInfo.Fields)
            {
                try
                {
                    if (pair.Value.Property.Info.PropertyType.FullName.Contains("System.DateTime"))
                    {
                        dataSource.Columns.Add(pair.Key, typeof(DateTime));
                    }
                    else {
                        dataSource.Columns.Add(pair.Key, pair.Value.Property.Info.PropertyType);
                    }

                   // "System.Nullable`1[[System.DateTime, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]"
                  
                }
                catch (Exception)
                {

                    throw;
                }
             
            }
            foreach (T local in entitys)
            {
                DataRow row = dataSource.NewRow();
                foreach (KeyValuePair<string, FieldMap> pair in entityInfo.Fields)
                {
                    if (pair.Value.Identity != null)
                    {
                        row[pair.Key] = num;
                    }
                    else
                    {
                        row[pair.Key] = pair.Value.Property.Get(local) ?? DBNull.Value;
                    }
                }
                dataSource.Rows.Add(row);
                num += 1L;
            }
            IDbHelper dbHelper = this._dbProvider.GetDbHelper();
            if (dbHelper is ISqlServerSpecial)
            {
                BulkAddParam param = new BulkAddParam {
                    TableName = entityInfo.Entity.Name
                };
                foreach (KeyValuePair<string, FieldMap> pair in entityInfo.Fields)
                {
                    ColumnMapping item = new ColumnMapping {
                        DataTableColumnName = pair.Key,
                        DBColumnName = pair.Key
                    };
                    param.ColumnMappings.Add(item);
                }
                (dbHelper as ISqlServerSpecial).BulkAdd(dataSource, param);
            }
        }

        private string GetInsertSql(EntityInfo info)
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, FieldMap> pair in info.Fields)
            {
                if (pair.Value.Identity != null)
                {
                    string str = "@" + pair.Key + "{0}";
                    builder.AppendFormat("DECLARE {0} bigint; ", str);
                    builder.AppendFormat("EXEC USP_XT_GetTableId '{0}',1,{1} OUTPUT; ", info.Entity.Name, str);
                    break;
                }
            }
            builder.Append(" INSERT INTO [");
            builder.Append(info.Entity.Name);
            builder.Append("] (");
            int num = 0;
            foreach (KeyValuePair<string, FieldMap> pair in info.Fields)
            {
                if (num > 0)
                {
                    builder.Append(',');
                }
                builder.Append('[');
                builder.Append(pair.Key);
                builder.Append(']');
                num++;
            }
            builder.Append(") VALUES(");
            num = 0;
            foreach (KeyValuePair<string, FieldMap> pair in info.Fields)
            {
                if (num > 0)
                {
                    builder.Append(',');
                }
                builder.Append('@');
                builder.Append(pair.Key + "{0}");
                num++;
            }
            builder.Append(");");
            return builder.ToString();
        }

        public override IList<T> GetRandomList<T>(LambdaContext context, Expression where, int count)
        {
           // this._dbProvider.Factory.CreateDbBuilder().SelectText(context, where, count, false);
            string sql = context.SelectText + context.WhereText + " order by NEWID()";
            Func<DbDataReader, IList<T>> func = EntityHelper.EntityAssign<T>(context.Entity);
            return this._dbProvider.GetDbHelper().ExecuteReader<IList<T>>(sql, func, context.Parameters);
        }
    }
}

