using KaSon.FrameWork.ORM.Dal;
using KaSon.FrameWork.ORM.IBuilder;
using KaSon.FrameWork.ORM.Visitor;
using KaSon.FrameWork.ORM.Visitor.SQLServer;
using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace KaSon.FrameWork.ORM.Builder
{



    internal class SQLServerBuilder : IDbBuilder
    {
        /// <summary>
        /// 获取查询语句
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="result"></param>
        public void GetLinqText(Expression exp, QueryProviderContext result)
        {
            ///指定Visitor 构造者
            result.VisitorBuilder = new SQLServerVisitorBuilder();
            new LinqVisitor().Visit(exp, result);
           // string sql = GetLinqText(result);

            result.SetTextFunc(new Func<QueryProviderContext, string>(SQLServerBuilder.GetLinqText));

          //  Console.WriteLine(sql);

        }
        public void CountText(LambdaContext context, Expression where, bool noLock)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT COUNT( ");
            if (context.Entity.PrimaryKeys.Count > 0)
            {
                builder.Append('[');
                builder.Append(context.Entity.PrimaryKeys[0]);
                builder.Append(']');
            }
            else
            {
                builder.Append('*');
            }
            builder.Append(')');
            builder.Append(" FROM ");
            builder.Append(context.Entity.Entity.Name);
            if (noLock)
            {
                builder.Append(" WITH(NOLOCK) ");
            }
            context.CountText = builder.ToString();
            new LambdaWhereVisitor().Explain(where, context);
            context.WhereText = " WHERE " + context.WhereText;
        }

        private static string CreateSelect(EntityInfo info)
        {
            StringBuilder builder = new StringBuilder();
            if (info.IsEntity)
            {
                int num = 0;
                foreach (KeyValuePair<string, FieldMap> pair in info.Fields)
                {
                    if (num > 0)
                    {
                        builder.Append(',');
                    }
                    builder.Append('[');
                    builder.Append(info.Entity.Name);
                    builder.Append("].[");
                    builder.Append(pair.Key);
                    builder.Append(']');
                    num++;
                }
            }
            return builder.ToString();
        }

        public void DeleteText(LambdaContext context, Expression where)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("DELETE FROM ");
            builder.Append('[');
            builder.Append(context.Entity.Entity.Name);
            builder.Append(']');
            context.DeleteText = builder.ToString();
            new LambdaWhereVisitor().Explain(where, context);
            context.WhereText = " WHERE " + context.WhereText;
        }

        public string GetDeleteText(EntityInfo entityInfo)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("DELETE FROM [");
            builder.Append(entityInfo.Entity.Name);
            builder.Append("] WHERE ");
            for (int i = 0; i < entityInfo.PrimaryKeys.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(" AND ");
                }
                builder.Append('[');
                builder.Append(entityInfo.PrimaryKeys[i]);
                builder.Append("] = @");
                builder.Append(entityInfo.PrimaryKeys[i]);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 插入 SQL 语句
        /// </summary>
        /// <param name="entityInfo"></param>
        /// <returns></returns>
        public virtual string GetInsertText(EntityInfo entityInfo)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("INSERT INTO [");
            builder.Append(entityInfo.Entity.Name);
            builder.Append("] (");
            int num = 0;
            foreach (KeyValuePair<string, FieldMap> pair in entityInfo.Fields)
            {
                if (!pair.Value.Field.IsIdenty && !pair.Value.Field.IsDefault)
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
            }
            builder.Append(") VALUES(");
            num = 0;
            foreach (KeyValuePair<string, FieldMap> pair in entityInfo.Fields)
            {
                if (!pair.Value.Field.IsIdenty && !pair.Value.Field.IsDefault)
                {
                    if (num > 0)
                    {
                        builder.Append(',');
                    }
                    builder.Append('@');
                    builder.Append(pair.Key);
                    num++;
                }
            }
            builder.Append(')');
            if (!string.IsNullOrWhiteSpace(entityInfo.Autorecode))
            {
                builder.Append(" SELECT @@IDENTITY AS ");
                builder.Append(entityInfo.Autorecode);
            }
            return builder.ToString();
        }

        private static string GetLinqText(QueryProviderContext result)
        {
            int? nullable;
            string sql = "";
            if (string.IsNullOrWhiteSpace(result.SqlBuilder.ToString()))
            {
                QueryParameter param = result.QueryColletion[result.ReturnType.FullName];
                sql = GetSelectText(param) + "[" + EntityHelper.GetEntityInfo(result.ReturnType).Entity.Name + "] AS " + param.Alias;
            }
            else if (!result.HasSelect )
            {
                if (string.IsNullOrWhiteSpace(result.JoinSelect))
                {
                    if (result.QueryColletion.Contains(result.ReturnType.FullName) )
                    {
                        sql = GetSelectText(result.QueryColletion[result.ReturnType.FullName]) + result.SqlBuilder.ToString();
                    }

                    if (result.IsExecute)
                    {
                       sql = result.SqlBuilder.ToString();
                    }
                }
                else
                {
                    //Json 方法构造sql
                    sql = result.JoinSelect + result.SqlBuilder;
                }
            }
            else
            {
                sql = result.SqlBuilder.ToString();
            }

            //排序
            if (result.IsOrder && !result.IsGroup  && result.Page==null && !result.IsSkip)
            {
                string lowSql = sql.ToLower();
                if (lowSql.Contains(" count"))
                {

                }
                else {
                    sql = sql + GetOrderText(result.OrderKeyList);
                }

               
            }

            if (result.JoinModes != null)
            {
                sql = SetJoinMode(sql, result.JoinModes);
            }
            // 唯一 
            if (result.IsDistinct)
            {
                sql = SetDistinct(sql);
            }

            //是否跳过前几条
            if (result.IsSkip)
            {
                sql = SetSkin(sql,result);
            }


             // Take  输入多少条
            if (result.Take.HasValue && (((nullable = result.Take).GetValueOrDefault() > 0) && nullable.HasValue))
            {
                return SetTake(sql, result.Take.Value);
            }
          

            //第一条数据
            //if (result.IsSingle)
            //{

            //    sql = GetSingleTest(sql);
            //}
           

           //分组
            if (result.IsGroup)
            {
              sql=  SetSetGroupBy(sql,result);
             // string orderTest = "";
              if (result.IsOrder)
              {
                 sql=sql+ GetOrderText(result.OrderKeyList);

              }
            }

            if (result.Page != null)
            {
                sql = SetPage(sql, result);
            }

           // if(result.iw)
          //  Console.WriteLine(sql);
           
            //if (result.Page != null)
            //{
            //    sql = SetPage(sql, result.Page);
            //}
            return sql;
        }

        /// <summary>
        /// 扩展程序第一次调用
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private static string SetPage(string sql,QueryProviderContext result)
        {
            Page page = result.Page;
            string orderby = page.OrderBy;
            if (page.OrderBy==null)
            {
                if (result.OrderKeyList.Count>0)
                {
                    orderby = GetOrderText(result.OrderKeyList);
                }
                else
                {
                    var entity = GlobalCache.EntityInfoPool[result.ReturnType];

                    if (entity==null)
                    {
                        throw new Exception("没有排序，又分页，返回类型也不是实体，error!");
                    }
                    //获取实体别名
                    string ModelAlis = result.QueryColletion[result.ReturnType.FullName].Alias;
                    //默认放回类型的第一个字段排序
                    var first = entity.Fields.First().Key;
                    //排序SQL
                    orderby = "order by " + ModelAlis+"."+ first;
                }

              
            }

            sql = sql.Replace("From", "FROM").Replace("from", "FROM");
            Regex regex = new Regex(@"\bFROM\b");
            string replacement = ",row_number() over (" + orderby + ")as __rownumber FROM";
            sql = regex.Replace(sql, replacement, 1);
            sql = sql + ") as T  where __rownumber between ";
            sql = sql + (((page.Index - 1) * page.Row) );
            sql = sql + " and ";
            sql = sql + (page.Index * page.Row + 1);



            sql = "SELECT * FROM (" + sql + page.AppendOrderBy;
            return sql;
        }


        /// <summary>
        /// 跳过前几条
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private static string SetSkin(string sql,QueryProviderContext Content)
        {

            //默认Order by 为返回类型的第一个字段
            string orderby = "";
            if (Content.IsOrder)
            {
                orderby = GetOrderText(Content.OrderKeyList);
            }
            else
            {
                EntityInfo entity = null;

                GlobalCache.EntityInfoPool.TryGetValue(Content.ReturnType,out entity);

                // var entity = GlobalCache.EntityInfoPool[Content.ReturnType];
                string first ="";
                if (entity != null)
                {
                     first = entity.Fields.First().Key;
                   // orderby = "order by " + first;
                    //if (Content.OrderKeyList.Count > 0)
                    //{
                    //    var p = Content.OrderKeyList.Where(b => b.ModelType == Content.ReturnType).FirstOrDefault();
                    //    if (p != null)
                    //    {
                    //        first = p.Key + " " + (p.isAsc ? "ASC" : "Desc");
                    //    }
                    //}

                    //  orderby = "order by " + first;
                }
                else {
                    GlobalCache.EntityInfoPool.TryGetValue(Content.SelectNodeType, out entity);

                    if (entity == null)
                    {
                        first = "Id";
                    }
                    else {
                        first = entity.Fields.First().Key;
                    }
                    
                   
                    
                }


                orderby = "order by  " + first;
            }


            sql = sql.Replace("From", "FROM").Replace("from", "FROM");
            Regex regex = new Regex(@"\bFROM\b");
            string replacement = ",row_number() over ( " + orderby + "  )as __rownumber FROM";
            sql = regex.Replace(sql, replacement, 1);
            sql = sql + ") as T  where __rownumber >= ";

            sql = sql + Content.Skip.Value;



            sql = "SELECT * FROM (" + sql;
            return sql;
        }

        /// <summary>
        ///  排序默认放到最后
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static string GetOrderText(IList<OrderKeyMap> list) {
            string   orderTest = " Order BY ";
            foreach (var b in list)
            {
                if (b.isAsc)
                {
                    orderTest = orderTest +b.ModelAlis+"."+ b.Key + " ASC ,";
                }
                else
                {
                    orderTest = orderTest + b.ModelAlis + "." + b.Key + " DESC ,";
                }

            }
            if (list.Count > 0)
            {
                orderTest = orderTest.Remove(orderTest.LastIndexOf(","));
            }

            return orderTest;
        }
        private static string GetSelectFields(EntityInfo info)
        {
            StringBuilder builder = new StringBuilder();
            int num = 0;
            foreach (KeyValuePair<string, FieldMap> pair in info.Fields)
            {
                if (num > 0)
                {
                    builder.Append(',');
                }
                builder.Append(" [");
                builder.Append(pair.Key);
                builder.Append(']');
                num++;
            }
            return builder.ToString();
        }

        private static string GetSelectText(QueryParameter param)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(" SELECT ");

            

            for (int i = 0; i < param.Columns.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(param.Alias);
                builder.Append(".[");
                builder.Append(param.Columns[i].FieldMap);
                builder.Append("]");

                builder.Append(" AS ");
                builder.Append(param.Columns[i].Name);
            }
            builder.Append(" FROM ");
            return builder.ToString();
        }

        private static string GetSingleTest (string sql){

            sql = sql.ToLower().Replace("select","SELECT TOP 1 ");

            return sql;
        }

        /// <summary>
        /// 获取更新SQL 语句
        /// </summary>
        /// <param name="entityInfo"></param>
        /// <returns></returns>
        public string GetUpdateText(EntityInfo entityInfo)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("UPDATE [");
            builder.Append(entityInfo.Entity.Name);
            builder.Append("] SET ");
            int num = 0;
            string whereStr = null;
            foreach (KeyValuePair<string, FieldMap> pair in entityInfo.Fields)
            {
                if (pair.Value.Field.IsPrimaryKey)
                {
                    whereStr = SetUpdateTextByWhere(whereStr, pair.Key);
                }
                else if (pair.Value.Identity == null)
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
                }
            }
            builder.Append(" where ");
            builder.Append(whereStr);
            return builder.ToString();
        }

        public void LinqGroupByText(MethodCallExpression exp, QueryProviderContext context)
        {
           // new GroupByVisitor().Explain(exp, context);
        }

        //public void LinqHavingText(MethodCallExpression exp, QueryProviderContext context)
        //{
        //    new HavingVisitor().Explain(exp, context);
        //}

        public void LinqJoinModeText(MethodCallExpression exp, QueryProviderContext context)
        {
            context.JoinModes = (JoinMode[])((ConstantExpression)exp.Arguments[1]).Value;
        }

      

        public void LinqOrderByDescendingText(MethodCallExpression exp, QueryProviderContext context)
        {
          //  new OrderByVisitor().Explain(exp, context);
            context.OrderByText = context.OrderByText + " DESC ";
        }

        public void LinqOrderByText(MethodCallExpression exp, QueryProviderContext context)
        {
           // new OrderByVisitor().Explain(exp, context);
            context.OrderByText = context.OrderByText + " ASC ";
        }

        public void LinqPageText(MethodCallExpression exp, QueryProviderContext context)
        {
            ValueVisitor visitor = new ValueVisitor();
         //   context.RowCount = (int)visitor.GetMemberValue(exp.Arguments[1]);
          //  context.PageIndex = (int)visitor.GetMemberValue(exp.Arguments[2]);
        }

        public void LinqSelectManyText(MethodCallExpression exp, QueryProviderContext context)
        {
            //new SelectManyVisitor().Explain(exp, context);
        }

        public void LinqSelectText(MethodCallExpression exp, QueryProviderContext context)
        {
           // new SelectVisitor().Explain(exp, context);
        }

        public void LinqTakeText(MethodCallExpression exp, QueryProviderContext context)
        {
          //  context.Top = (int)((ConstantExpression)exp.Arguments[1]).Value;
        }

        public void LinqThenByDescendingText(MethodCallExpression exp, QueryProviderContext context)
        {
            OrderByVisitor visitor = new OrderByVisitor();
            context.OrderByText = context.OrderByText + ",";
           // visitor.Explain(exp, context);
            context.OrderByText = context.OrderByText + " DESC ";
        }

        public void LinqThenByText(MethodCallExpression exp, QueryProviderContext context)
        {
            OrderByVisitor visitor = new OrderByVisitor();
            context.OrderByText = context.OrderByText + ",";
          //  visitor.Explain(exp, context);
            context.OrderByText = context.OrderByText + " ASC ";
        }

        public void LinqWhereText(MethodCallExpression exp, QueryProviderContext context)
        {
           // new WhereVisitor().Explain(exp, context);
        }

        public void SelectText(LambdaContext context, Expression where, bool noLock)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT ");
            builder.Append(GetSelectFields(context.Entity));
            builder.Append(" FROM ");
            builder.Append(context.Entity.Entity.Name);
            if (noLock)
            {
                builder.Append(" WITH(NOLOCK) ");
            }
            context.SelectText = builder.ToString();
            new LambdaWhereVisitor().Explain(where, context);
            context.WhereText = " WHERE " + context.WhereText;
        }

        public void SelectText(LambdaContext context, Expression where, int top, bool noLock)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT TOP ");
            builder.Append(top);
            builder.Append(" ");
            builder.Append(GetSelectFields(context.Entity));
            builder.Append(" FROM ");
            builder.Append(context.Entity.Entity.Name);
            if (noLock)
            {
                builder.Append(" WITH(NOLOCK) ");
            }
            context.SelectText = builder.ToString();
            new LambdaWhereVisitor().Explain(where, context);
            context.WhereText = " WHERE " + context.WhereText;
        }

        public void SelectText(LambdaContext context, Expression select, Expression where, bool noLock)
        {
          //  new LambdaSelectVisitor().Explain(select, context);
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT ");
            builder.Append(context.SelectText);
            builder.Append(" FROM ");
            builder.Append(context.Entity.Entity.Name);
            if (noLock)
            {
                builder.Append(" WITH(NOLOCK) ");
            }
            context.SelectText = builder.ToString();
            new LambdaWhereVisitor().Explain(where, context);
            context.WhereText = " WHERE " + context.WhereText;
        }

        private static string SetDistinct(string sql)
        {
            sql = sql.Replace("Select", "SELECT").Replace("select", "SELECT");
            sql = new Regex(@"\bSELECT\b").Replace(sql, "SELECT Distinct ", 1);
            return sql;
        }

        public void SetGroupByKey(Expression expression, QueryContext queryContext, string fullName)
        {
            new GroupByKeyVistor().Visit(expression, queryContext, fullName);
        }
        /// <summary>
        /// 分组实现 GetSQL 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static string SetSetGroupBy(string sql,QueryProviderContext result)
        {

            if (sql.ToLower().IndexOf("select") <= 0)
            {
                string temp = sql.Remove(sql.ToLower().IndexOf("group"));

                sql = "SELECT Group2.*  FROM ( Select " + result.GroupModelAlas + ".[" + result.GroupKeyName + "] From " + sql
                    + " ) AS Group1";
                sql += "  LEFT OUTER JOIN ( SELECT * From " + temp + ") AS Group2 ON Group1.[" + result.GroupKeyName + "]=Group2.[" + result.GroupKeyName + "]";
        

            }
            else
            {
                string temp = sql.Remove(sql.ToLower().IndexOf("group by"));

                temp = temp.Substring(temp.ToLower().IndexOf("from"));
               string  stemp = sql.Substring(sql.ToLower().IndexOf("from"));

               if (sql.ToLower().Contains("[key]"))
                {
                    //sql = sql.ToLower().Replace("[key]", "[ID] as [key]");
                    sql = sql.ToLower().Replace("t1.[key]", result.GroupModelAlas + ".[" + result.GroupKeyName + "]");
                    sql = sql.ToLower().Replace("t2.[key]", result.GroupModelAlas + ".[" + result.GroupKeyName + "]");
               }


                sql = "SELECT Group2.*  FROM (  " + sql
                    + " ) AS Group1";
                sql += "  LEFT OUTER JOIN ( SELECT *  " + temp + ") AS Group2 ON Group1.[" + result.GroupKeyName + "]=Group2.[" + result.GroupKeyName + "]";
        
            }

          
            return sql;
        }
        private static string SetJoinMode(string sql, IEnumerable<JoinMode> modes)
        {
            sql = sql.Replace("Join", "_join_");
            Regex regex = new Regex(@"\b_join_\b");
            using (IEnumerator<JoinMode> enumerator = modes.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    switch (enumerator.Current)
                    {
                        case JoinMode.Inner:
                            sql = regex.Replace(sql, "Join", 1);
                            break;

                        case JoinMode.Left:
                            sql = regex.Replace(sql, "left Join", 1);
                            break;

                        case JoinMode.Right:
                            sql = regex.Replace(sql, "right Join", 1);
                            break;

                        case JoinMode.Full:
                            sql = regex.Replace(sql, "full outer Join", 1);
                            break;
                    }
                }
            }
            sql = sql.Replace("_join_", "Join");
            return sql;
        }

        //private static string SetPage(string sql, Page page)
        //{
        //    sql = sql.Replace("From", "FROM").Replace("from", "FROM");
        //    Regex regex = new Regex(@"\bFROM\b");
        //    string replacement = ",row_number() over (" + page.OrderBy + ")as __rownumber FROM";
        //    sql = regex.Replace(sql, replacement, 1);
        //    sql = sql + ") as temp  where __rownumber between ";
        //    sql = sql + (((page.Index - 1) * page.Row) + 1);
        //    sql = sql + " and ";
        //    sql = sql + (page.Index * page.Row);
        //    sql = "SELECT * FROM (" + sql;
        //    return sql;
        //}

        private static string SetTake(string sql, int number)
        {
            sql = sql.Replace("Select", "SELECT").Replace("select", "SELECT");
            sql = new Regex(@"\bSELECT\b").Replace(sql, "SELECT TOP " + number, 1);
            return sql;
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

        public void SumText(LambdaContext context, Expression select, Expression where, bool noLock)
        {
          //  new LambdaSelectVisitor().Explain(select, context);
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT SUM( ");
            builder.Append(context.SelectText);
            builder.Append(')');
            builder.Append(" FROM ");
            builder.Append(context.Entity.Entity.Name);
            if (noLock)
            {
                builder.Append(" WITH(NOLOCK) ");
            }
            context.SumText = builder.ToString();
            new LambdaWhereVisitor().Explain(where, context);
            context.WhereText = " WHERE " + context.WhereText;
        }

        public void UpdateText(LambdaContext context, Expression updateExp, Expression whereExp)
        {
            new UpdateVisitor().Explain(updateExp, context);
            new LambdaWhereVisitor().Explain(whereExp, context);
            context.WhereText = " WHERE " + context.WhereText;
        }


        string IDbBuilder.GetLinqText(QueryProviderContext context)
        {
            throw new NotImplementedException();
        }

        public void LinqHavingText(MethodCallExpression exp, QueryProviderContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取更新实体字段的SQL 语句
        /// </summary>
        /// <param name="entityInfo"></param>
        /// <returns></returns>
        public string GetEmpayNotUpdateText(EntityInfo entityInfo)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("UPDATE [");
            builder.Append(entityInfo.Entity.Name);
            builder.Append("] SET ");
            int num = 0;
            string whereStr = null;
            foreach (KeyValuePair<string, FieldMap> pair in entityInfo.Fields)
            {
                if (pair.Value.Field.IsPrimaryKey)
                {
                    whereStr = SetUpdateTextByWhere(whereStr, pair.Key);
                }
                else if (pair.Value.Identity == null)
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
                }
            }
            builder.Append(" where ");
            builder.Append(whereStr);
            return builder.ToString();
        }
    }
}
