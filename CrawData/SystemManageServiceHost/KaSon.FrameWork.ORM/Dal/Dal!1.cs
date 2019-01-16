namespace KaSon.FrameWork.ORM.Dal
{
    using KaSon.FrameWork.ORM.Factory;
    using KaSon.FrameWork.ORM.IBuilder;
    using KaSon.FrameWork.ORM.Provider;
    using KaSon.FrameWork.Services.Enum;
    using KaSon.FrameWork.Services.ORM;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class Dal<T> : IDal<T> where T: class, new()
    {
      
        private readonly IDbBuilder _dbBuilder;
        private readonly IDbParamterBuilder _dbParamterBuilder;
        private readonly DbProvider _dbProvider;
        private bool _IsLLink = false;
       
        public Dal(DbProvider dbProvider, bool isLLink)
        {
            _IsLLink = isLLink;
            this._dbProvider = dbProvider;
            this._dbParamterBuilder = dbProvider.Factory.CreateDbParamterBuilder();
            this._dbBuilder = this._dbProvider.Factory.CreateDbBuilder();
        }

        /// <summary>
        /// 录入实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="getId"></param>
        public void Add(T entity, bool getId = false)
        {
            string insertText;
            KaSon.FrameWork.Services.ORM.DbParameterCollection insertParamters;
            Type entityType = entity.GetType();
      
            string key = this._dbProvider.ConnInfo.FactoryType + "-" + entityType.FullName;
            EntityInfo entityInfo = EntityHelper.GetEntityInfo(entityType);
            if (GlobalCache.InsertTextPool.ContainsKey(key))
            {
                insertText = GlobalCache.InsertTextPool[key];
            }
            else
            {
              
                insertText = this._dbBuilder.GetInsertText(entityInfo);
                GlobalCache.AddInsertText(key, insertText);
            }
            //if (this._dbProvider.Factory.ConfigInfo.Provider == ProviderInfo.SqlServer)
            //{

            //    ///自动递增键
            //   // if (entityInfo.Autorecode != null)
            //  //  {
            //      //  StringBuilder builder = new StringBuilder();
            //      //  builder.AppendFormat("DECLARE @ID bigint; ", new object[0]);
            //       // builder.AppendFormat("EXEC USP_XT_GetTableId '{0}',1, @ID OUTPUT;  SELECT @ID ;", entityInfo.Entity.Name);
            //       // object obj2 = this._dbProvider.CreateDbHelper().ExecuteScalar(builder.ToString(), null);
            //      //  entityInfo.Propertys[entityInfo.Autorecode].Set(entity, obj2);
            //   // }
            //    insertParamters = this._dbParamterBuilder.GetInsertParamters(entity, entityInfo);
            //    if (getId)
            //    {
            //        object obj3 = this._dbProvider.Factory.CreateDalHelper(this._dbProvider).ExcuteAddWithGetId(insertText, insertParamters);
            //        entityInfo.Propertys[entityInfo.Autorecode].Set(entity, obj3);
            //    }
            //    else
            //    {
            //        this._dbProvider.GetDbHelper().ExcuteNonQuery(insertText, insertParamters);
            //    }
            //}
            //else
            //{
            //    insertParamters = this._dbParamterBuilder.GetInsertParamters(entity, entityInfo);
            //    if (getId)
            //    {
            //        object obj3 = this._dbProvider.Factory.CreateDalHelper(this._dbProvider).ExcuteAddWithGetId(insertText, insertParamters);
            //        entityInfo.Propertys[entityInfo.Autorecode].Set(entity, obj3);
            //    }
            //    else
            //    {
            //        this._dbProvider.GetDbHelper().ExcuteNonQuery(insertText, insertParamters);
            //    }
            //}

            //获取录入参数
            insertParamters = this._dbParamterBuilder.GetInsertParamters(entity, entityInfo);
            if (getId)
            {
                //调入Set 方法 更新自增编号值
                object obj3 = this._dbProvider.Factory.CreateDalHelper(this._dbProvider).ExcuteAddWithGetId(insertText, insertParamters);
                entityInfo.Propertys[entityInfo.Autorecode].Set(entity, obj3);
            }
            else
            {
                this._dbProvider.GetDbHelper().ExcuteNonQuery(insertText, insertParamters);
            }
        }

        public void Add(IList<T> entitylist, bool getId = false)
        {
            if (!this._dbProvider.IsTran)
            {
                foreach (T local in entitylist)
                {
                    this.Add(local, getId);
                }
            }
            else
            {
                try
                {
                    this._dbProvider.Begin();
                    foreach (T local in entitylist)
                    {
                        this.Add(local, getId);
                    }
                    this._dbProvider.Commit();
                }
                catch
                {
                    this._dbProvider.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 更新改变的实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int UpdateChange(T entity) 
        {

            string updateText;
            Type entityType = entity.GetType();
            EntityInfo entityInfo = EntityHelper.GetEntityInfo(entityType);
            string key = this._dbProvider.ConnInfo.FactoryType + "-Change" + entityType.FullName;
            if (GlobalCache.UpdateTextPool.ContainsKey(key))
            {
                updateText = GlobalCache.UpdateTextPool[key];
            }
            else
            {
                updateText = this._dbBuilder.GetUpdateText(entityInfo);
                GlobalCache.AddUpdateText(key, updateText);
            }



            KaSon.FrameWork.Services.ORM.DbParameterCollection updateParamters = this._dbParamterBuilder.GetUpdateParamters(entity, entityInfo);
            return this._dbProvider.GetDbHelper().ExcuteNonQuery(updateText, updateParamters);
           
        }
      
        public void AddEx(object entity, bool getId = false)
        {
            this.Add((T) entity, getId);
        }

      

        public int Count(Expression<Func<T, bool>> exp, bool noLock = false)
        {
            LambdaContext context = this.GetContext();
            this._dbBuilder.CountText(context, exp, noLock);
            string sql = context.CountText + context.WhereText;
            return int.Parse(this._dbProvider.GetDbHelper().ExecuteScalar(sql, context.Parameters).ToString());
        }

        public int Delete(T entity)
        {
            string deleteText;
            Type entityType = entity.GetType();
            EntityInfo entityInfo = EntityHelper.GetEntityInfo(entityType);
            string key = this._dbProvider.ConnInfo.FactoryType + "-" + entityType.FullName;
            if (GlobalCache.DeleteTextPool.ContainsKey(key))
            {
                deleteText = GlobalCache.DeleteTextPool[key];
            }
            else
            {
                deleteText = this._dbBuilder.GetDeleteText(entityInfo);
                GlobalCache.AddDeleteText(key, deleteText);
            }
           KaSon.FrameWork.Services.ORM.DbParameterCollection deleteParamters = this._dbParamterBuilder.GetDeleteParamters(entity, entityInfo);
            return this._dbProvider.GetDbHelper().ExcuteNonQuery(deleteText, deleteParamters);
        }

        public int Delete(IList<T> entitylist)
        {
            Func<T, int> selector = null;
            Func<T, int> func2 = null;
            int num = 0;
            if (this._dbProvider.IsTran)
            {
                if (selector == null)
                {
                    selector = entity => Delete(entity);
                }
                return entitylist.Sum<T>(selector);
            }
            try
            {
                this._dbProvider.Begin();
                if (func2 == null)
                {
                    func2 = entity => Delete(entity);
                }
                num = entitylist.Sum<T>(func2);
                this._dbProvider.Commit();
            }
            catch
            {
                this._dbProvider.Rollback();
                throw;
            }
            return num;
        }

        public int Delete(Expression<Func<T, bool>> exp)
        {
            LambdaContext context = this.GetContext();
            this._dbBuilder.DeleteText(context, exp);
            string sql = context.DeleteText + context.WhereText;
            return this._dbProvider.GetDbHelper().ExcuteNonQuery(sql, context.Parameters);
        }

        public T Get(Expression<Func<T, bool>> exp, bool noLock = false)
        {
            LambdaContext context = this.GetContext();
            this._dbBuilder.SelectText(context, exp, 1, noLock);
            string sql = context.SelectText + context.WhereText;
            Func<DbDataReader, IList<T>> func = EntityHelper.EntityAssign<T>(context.Entity);
            IList<T> source = this._dbProvider.GetDbHelper().ExecuteReader<IList<T>>(sql, func, context.Parameters);
            T local = default(T);
            if (source.Any<T>())
            {
                local = source[0];
            }
            return local;
        }

        public object Get(Expression<Func<T, object>> select, Expression<Func<T, bool>> where, bool noLock = false)
        {
            LambdaContext context = this.GetContext();
            this._dbBuilder.SelectText(context, select, where, noLock);
            string sql = context.SelectText + context.WhereText;
            return this._dbProvider.GetDbHelper().ExecuteScalar(sql, context.Parameters);
        }

        private LambdaContext GetContext()
        {
            Type entityType = typeof(T);
            EntityInfo entityInfo = EntityHelper.GetEntityInfo(entityType);
            return new LambdaContext(this) { Entity = entityInfo };
        }

        public IList<T> GetList(Expression<Func<T, bool>> exp, bool noLock = false)
        {
            return this.GetList<T>(exp, noLock);
        }

        public IList<TResult> GetList<TResult>(Expression<Func<T, bool>> exp, bool noLock = false) where TResult: new()
        {
            LambdaContext context = this.GetContext();
            this._dbBuilder.SelectText(context, exp, noLock);
            string sql = context.SelectText + context.WhereText;
            Func<DbDataReader, IList<TResult>> func = EntityHelper.EntityAssign<TResult>(context.Entity);
            return this._dbProvider.GetDbHelper().ExecuteReader<IList<TResult>>(sql, func, context.Parameters);
        }

        public T GetRandom(Expression<Func<T, bool>> exp)
        {
            return this.GetRandomList(exp, 1).FirstOrDefault<T>();
        }

        public IList<T> GetRandomList(Expression<Func<T, bool>> exp, int count)
        {
            LambdaContext context = this.GetContext();
            return this._dbProvider.Factory.CreateDalHelper(this._dbProvider).GetRandomList<T>(context, exp, count);
        }

        public object Sum<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> where, bool noLock = false)
        {
            LambdaContext context = this.GetContext();
            this._dbBuilder.SumText(context, select, where, noLock);
            string sql = context.SumText + context.WhereText;
            return this._dbProvider.GetDbHelper().ExecuteScalar(sql, context.Parameters);
        }

        public int Update(T entity)
        {
            string updateText;
            Type entityType = entity.GetType();
            EntityInfo entityInfo = EntityHelper.GetEntityInfo(entityType);
            string key = this._dbProvider.ConnInfo.FactoryType + "-" + entityType.FullName;
            if (GlobalCache.UpdateTextPool.ContainsKey(key))
            {
                updateText = GlobalCache.UpdateTextPool[key];
            }
            else
            {
                updateText = this._dbBuilder.GetUpdateText(entityInfo);
                GlobalCache.AddUpdateText(key, updateText);
            }
            KaSon.FrameWork.Services.ORM.DbParameterCollection updateParamters = this._dbParamterBuilder.GetUpdateParamters(entity, entityInfo);
            return this._dbProvider.GetDbHelper().ExcuteNonQuery(updateText, updateParamters);
        }

        public int Update(IList<T> entitylist)
        {
            Func<T, int> selector = null;
            Func<T, int> func2 = null;
            int num = 0;
            if (this._dbProvider.IsTran)
            {
                if (selector == null)
                {
                    selector = entity => Update(entity);
                }
                return entitylist.Sum<T>(selector);
            }
            try
            {
                this._dbProvider.Begin();
                if (func2 == null)
                {
                    func2 = entity => Update(entity);
                }
                num = entitylist.Sum<T>(func2);
                this._dbProvider.Commit();
            }
            catch
            {
                this._dbProvider.Rollback();
                throw;
            }
            return num;
        }

        public int Update<TResult>(Expression<Func<T, TResult>> update, Expression<Func<T, bool>> where) where TResult: class
        {
            LambdaContext context = this.GetContext();
            this._dbBuilder.UpdateText(context, update, where);
            string sql = context.UpdateText + context.WhereText;
            return this._dbProvider.GetDbHelper().ExcuteNonQuery(sql, context.Parameters);
        }

        /// <summary>
        /// 批量录入
        /// </summary>
        /// <param name="entitylist"></param>
        /// <param name="getId"></param>
        public void BulkAdd(IList<T> entitylist)
        {
           DalHelper dalHelper= this._dbProvider.Factory.CreateDalHelper(this._dbProvider);

            // dalHelper.ExcuteBulkAdd(entitylist);
            if (this._dbProvider.IsTran)
            {
                dalHelper.ExcuteBulkAdd(entitylist);
            }
            else
            {
                try
                {
                    this._dbProvider.Begin();
                    dalHelper.ExcuteBulkAdd(entitylist);
                    this._dbProvider.Commit();
                }
                catch(Exception ex)
                {
                    this._dbProvider.Rollback();
                    throw ex;
                }
            }

        }
        private static string SetUpdateTextByWhere(string whereStr, string field)
        {
            if (string.IsNullOrWhiteSpace(whereStr))
            {
                whereStr = "[" + field + "] = @" + field;
                return whereStr;
            }
            string str2 = whereStr;
            whereStr = str2 + " AND [" + field + "] = @" + field;
            return whereStr;
        }

        /// <summary>
        /// 空的内容不更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int EmptyNotUpdate(T entity)
        {
            string updateText;
            Type entityType = entity.GetType();
            EntityInfo entityInfo = EntityHelper.GetEntityInfo(entityType);

            StringBuilder builder = new StringBuilder();
            builder.Append("UPDATE [");
            builder.Append(entityInfo.Entity.Name);
            builder.Append("] SET ");
            int num = 0;
            string whereStr = null;
            KaSon.FrameWork.Services.ORM.DbParameterCollection parameters = new KaSon.FrameWork.Services.ORM.DbParameterCollection();
            foreach (KeyValuePair<string, FieldMap> pair in entityInfo.Fields)
            {
                if (pair.Value.Field.IsPrimaryKey)
                {
                    whereStr = SetUpdateTextByWhere(whereStr, pair.Key);
                }
                else if (pair.Value.Identity == null)
                {
                   
                 object value= pair.Value.Property.Get(entity);
                 if (!String.IsNullOrWhiteSpace(value.ToString()))
                 {

                     if (pair.Value.Version != null)
                     {
                         if (num > 0)
                         {
                             builder.Append(',');
                         }
                         builder.Append('[');
                         builder.Append(pair.Key);
                         builder.Append("] = @");
                         builder.Append(pair.Key);
                         builder.Append(" + ");
                         builder.Append(pair.Value.Version.Interval);
                         whereStr = SetUpdateTextByWhere(whereStr, pair.Key);
                         num++;
                     }
                     else
                     {
                         if (num > 0)
                         {
                             builder.Append(',');
                         }
                         builder.Append('[');
                         builder.Append(pair.Key);
                         builder.Append("] = @");
                         builder.Append(pair.Key);
                         num++;
                     }

                     parameters.Insert(pair.Key, pair.Value.Property.Get(entity) ?? DBNull.Value, System.Data.ParameterDirection.Input);
                 }


                }
            }
            builder.Append(" where ");
            builder.Append(whereStr);
           string sql= builder.ToString();


           return this._dbProvider.GetDbHelper().ExcuteNonQuery(sql, parameters);

        }
    }
}

