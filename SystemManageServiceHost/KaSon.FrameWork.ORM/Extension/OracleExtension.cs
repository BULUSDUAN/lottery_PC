using KaSon.FrameWork.ORM.Provider;
using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using KaSon.FrameWork.Services.ORM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Extension
{
    public class OracleExtension : ICommonQuery
    {
        private readonly DbProvider _provider;

        public OracleExtension(DbProvider provider)
        {
            this._provider = provider;
        }
        private static string GetMultOrderBySql(QueryArgs args, MultEntityInfo info)
        {
            string str = string.Empty;
            for (int i = 0; i < args.SortFields.Count; i++)
            {
                string str3 = str;
                str = str3 + ((i > 0) ? "," : "") + info.Propertys[args.SortFields[i].Field].TableName + "." + info.Propertys[args.SortFields[i].Field].FieldName + (args.SortFields[i].IsASC ? " ASC" : " DESC");
            }
            if (string.IsNullOrWhiteSpace(str))
            {
                str = info.Propertys.Values.ToList<MultFieldInfo>()[0].TableName + "." + info.Propertys.Values.ToList<MultFieldInfo>()[0].FieldName;
            }
            return str;
        }


        private static string GetMultWhereSql(QueryArgs args, MultEntityInfo info,QueryProviderContext result)
        {
            KaSon.FrameWork.Services.ORM.DbParameterCollection parameters = result.Parameters;
            if ((args.WhereFields == null) || (args.WhereFields.Count == 0))
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < args.WhereFields.Count; i++)
            {
                WhereField field = args.WhereFields[i];
                if (field.Value != null)
                {
                 
                    string str5;
                    string name = "@" + field.Field + i;
                    bool flag = true;
                    string str = field.Value;
                    string str3 = "";
                    str3 = (builder.Length > 0) ? " AND " : "";
                    if (!info.Propertys.ContainsKey(field.Field))
                    {
                        throw new ArgumentException("条件字段:(" + field.Field + ")不在联合实体属性中！");
                    }
                    switch (field.WhereType)
                    {
                        case WhereType.UnEqual:
                            str5 = str3;
                            str3 = str5 + info.Propertys[field.Field].TableName + "." + info.Propertys[field.Field].FieldName + "<>" + name;
                            break;

                        case WhereType.Less:
                            str5 = str3;
                            str3 = str5 + info.Propertys[field.Field].TableName + "." + info.Propertys[field.Field].FieldName + "<" + name;
                            break;

                        case WhereType.LessOrEqual:
                            str5 = str3;
                            str3 = str5 + info.Propertys[field.Field].TableName + "." + info.Propertys[field.Field].FieldName + "<=" + name;
                            break;

                        case WhereType.Than:
                            str5 = str3;
                            str3 = str5 + info.Propertys[field.Field].TableName + "." + info.Propertys[field.Field].FieldName + ">" + name;
                            break;

                        case WhereType.ThanOrEqual:
                            str5 = str3;
                            str3 = str5 + info.Propertys[field.Field].TableName + "." + info.Propertys[field.Field].FieldName + ">=" + name;
                            break;

                        case WhereType.LeftLike:
                            flag = false;
                            str = ReplaceLikeStr(str);
                            str5 = str3;
                            str3 = str5 + info.Propertys[field.Field].TableName + "." + info.Propertys[field.Field].FieldName + " Like '" + str + "%'";
                            break;

                        case WhereType.RightLike:
                            flag = false;
                            str = ReplaceLikeStr(str);
                            str5 = str3;
                            str3 = str5 + info.Propertys[field.Field].TableName + "." + info.Propertys[field.Field].FieldName + " Like '%" + str + "'";
                            break;

                        case WhereType.Like:
                            flag = false;
                            str = ReplaceLikeStr(str);
                            str5 = str3;
                            str3 = str5 + info.Propertys[field.Field].TableName + "." + info.Propertys[field.Field].FieldName + " Like '%" + str + "%'";
                            break;

                        case WhereType.In:
                            flag = false;
                            str = ReplaceInStr(str);
                            str5 = str3;
                            str3 = str5 + info.Propertys[field.Field].TableName + "." + info.Propertys[field.Field].FieldName + " IN (" + str + ")";
                            break;

                        case WhereType.NotIn:
                            flag = false;
                            str = ReplaceInStr(str);
                            str5 = str3;
                            str3 = str5 + info.Propertys[field.Field].TableName + "." + info.Propertys[field.Field].FieldName + " NOT IN (" + str + ")";
                            break;

                        default:
                            str5 = str3;
                            str3 = str5 + info.Propertys[field.Field].TableName + "." + info.Propertys[field.Field].FieldName + "=" + name;
                            break;
                    }
                    if (flag)
                    {
                        parameters.Insert(name, str, ParameterDirection.Input);
                    }
                    builder.Append(str3);
                }
            }
            return builder.ToString();
        }
        private static string GetMultOrderBySql(QueryArgs args)
        {
            string str = string.Empty;
            for (int i = 0; i < args.SortFields.Count; i++)
            {
                string str3 = str;
                str = str3 + ((i > 0) ? "," : "") + args.SortFields[i].Field  + (args.SortFields[i].IsASC ? " ASC" : " DESC");
            }
            if (string.IsNullOrWhiteSpace(str))
            {
                //throw new Exception("没有设置OrderBy字段");
            }
            return str;
        }
        private static string GetMultOrderBySql(QueryArgs args, string Alias)
        {
            string str = string.Empty;
            for (int i = 0; i < args.SortFields.Count; i++)
            {
                string str3 = str;
                str = str3 + ((i > 0) ? "," : "") + Alias + args.SortFields[i].Field + (args.SortFields[i].IsASC ? " ASC" : " DESC");
            }
            if (string.IsNullOrWhiteSpace(str))
            {
                //throw new Exception("没有设置OrderBy字段");
            }
            return str;
        }
     
        /// <summary>
        /// 通用查询  Where 过滤
        /// </summary>
        /// <param name="args"></param>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static string GetMultWhereSql(QueryArgs args, Type type,QueryProviderContext result)
        {

            KaSon.FrameWork.Services.ORM.DbParameterCollection parameters = result.Parameters;


            if ((args.WhereFields == null) || (args.WhereFields.Count == 0))
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < args.WhereFields.Count; i++)
            {
                WhereField field = args.WhereFields[i];
                string name = "@" + field.Field + i;
                bool flag = true;
                if (field.Value != null)
                {

                    string Alias = result.QueryColletion[type.FullName].Alias + ".";
                    string str5;
                    string str = field.Value;
                    string str3 = "";
                    str3 = (builder.Length > 0) ? " AND " : "";
                    switch (field.WhereType)
                    {
                        case WhereType.UnEqual:
                            str3 = str3 + Alias + field.Field + "<>" + name;
                            break;

                        case WhereType.Less:
                            str3 = str3 + Alias + field.Field + "<" + name;
                            break;

                        case WhereType.LessOrEqual:
                            str3 = str3 + Alias + field.Field + "<=" + name;
                            break;

                        case WhereType.Than:
                            str3 = str3 + Alias + field.Field + ">" + name;
                            break;

                        case WhereType.ThanOrEqual:
                            str3 = str3 + Alias + field.Field + ">=" + name;
                            break;

                        case WhereType.LeftLike:
                            flag = false;
                            str = OracleExtension.ReplaceLikeStr(str);
                            str5 = str3;
                            str3 = str5 + Alias + field.Field + " Like '" + str + "%'";
                            break;

                        case WhereType.RightLike:
                            flag = false;
                            str = OracleExtension.ReplaceLikeStr(str);
                            str5 = str3;
                            str3 = str5 + Alias + field.Field + " Like '%" + str + "'";
                            break;

                        case WhereType.Like:
                            flag = false;
                            str = OracleExtension.ReplaceLikeStr(str);
                            str5 = str3;
                            str3 = str5 + Alias + field.Field + " Like '%" + str + "%'";
                            break;

                        case WhereType.In:
                            flag = false;
                            str = OracleExtension.ReplaceInStr(str);
                            str5 = str3;
                            str3 = str5 + Alias + field.Field + " IN (" + str + ")";
                            break;

                        case WhereType.NotIn:
                            flag = false;
                            str = OracleExtension.ReplaceInStr(str);
                            str5 = str3;
                            str3 = str5 + Alias + field.Field + " NOT IN (" + str + ")";
                            break;

                        default:
                            str3 = str3 + Alias + field.Field + "=" + name;
                            break;
                    }
                    if (flag)
                    {
                        if (type.GetProperty(field.Field).PropertyType.Equals(typeof(DateTime)) || type.GetProperty(field.Field).PropertyType.Equals(typeof(DateTime?)))
                        {
                            parameters.Insert(name, Convert.ToDateTime(str), ParameterDirection.Input);
                        }
                        else
                        {
                            parameters.Insert(name, str, ParameterDirection.Input);
                        }
                    }
                    builder.Append(str3);
                }
            }
            return builder.ToString();
        }

   

        public  QueryResult Query<TResult>(IQueryable<TResult> query, QueryArgs args)
        {
            Type entityType = typeof(TResult);
            MultEntityInfo multEntityInfo = null;
           
            if (entityType.GetCustomAttributes(typeof(MultEntityAttribute), true).Count<object>() > 0)
            {
                multEntityInfo = EntityHelper.GetMultEntityInfo(entityType);
            }
            QueryProviderContext result = new QueryProviderContext(this._provider);
            string sql = "";
            result.ReturnType = typeof(TResult);
            ///获取别名
            string Alias = result.QueryColletion[typeof(TResult).FullName].Alias+".";

            this._provider.Factory.CreateDbBuilder().GetLinqText(query.Expression, result);
            QueryResult result2 = new QueryResult();
            if ((args.WhereFields.Count == 0) && (args.PageIndex > 0))
            {
                if (result2.RowCount == 0)
                {
                    sql = result.GetQueryText();
                  //  sql = " SELECT Count(*) FROM( " + sql + ") T";

                    sql = "SELECT Count(*) " + GetFromAfterStr(sql);


                    result2.RowCount = Convert.ToInt32(this._provider.GetDbHelper().ExecuteScalar(sql, result.Parameters));
                }
                Page page = new Page
                {
                    Index = args.PageIndex,
                    Row = args.PageSize,
                    OrderBy = " Order by " + ((multEntityInfo == null) ? GetMultOrderBySql(args, Alias) : GetMultOrderBySql(args, multEntityInfo))
                };
                result.Page = page;
                sql = result.GetQueryText();
            }
            else
            {
                sql = result.GetQueryText();
                string str2 = (multEntityInfo == null) ? GetMultWhereSql(args, entityType, result) : GetMultWhereSql(args, multEntityInfo, result);
                if (result2.RowCount == 0)
                {
                    string str3 =sql;
                  

                    if (!string.IsNullOrWhiteSpace(str2))
                    {
                     
                        if (str3.ToLower().Contains("where"))
                        {
                            str3 = str3 + " AND " + str2;
                        }
                        else
                        {
                            str3 = str3 + " WHERE " + str2;
                        }

                       
                    }

                    str3 = "SELECT Count(*) " + GetFromAfterStr(str3);
                    result2.RowCount = Convert.ToInt32(this._provider.GetDbHelper().ExecuteScalar(str3, result.Parameters));
                }
                if (args.PageIndex > 0)
                {
                   // Alias = "T.";

                 //   string test = GetMultOrderBySql(args, Alias);
                   // sql = "SELECT *,row_number() over ( order by " + ((multEntityInfo == null) ? GetMultOrderBySql(args, Alias) : GetMultOrderBySql(args, multEntityInfo)) + ")as __rownumber FROM (" + sql + ") T ";
                  //  sql = "SELECT * FROM (" + sql + ") T ";




                    if (!string.IsNullOrWhiteSpace(str2))
                    {


                        if (sql.ToLower().Contains("where"))
                        {
                            sql = sql + " AND " + str2;
                        }
                        else
                        {
                            sql = sql + " WHERE " + str2;
                        }
                       
                    }
                    if (args.SortFields !=null)
                    {
                          sql = sql +" order by " +GetMultOrderBySql(args, Alias);
                    }


                    // sql = string.Concat(new object[] { "SELECT * FROM (", sql, ") temp WHERE  __rownumber  <=", args.PageIndex * args.PageSize, " AND __rownumber >", (args.PageIndex - 1) * args.PageSize });
                    sql = sql + " limit " + (args.PageIndex - 1) * args.PageSize + "," + args.PageIndex * args.PageSize  ;
                }
                else
                {
                   // sql = "SELECT * FROM (" + sql + ") T ";
                    if (!string.IsNullOrWhiteSpace(str2))
                    {
                        sql = sql + " WHERE " + str2;
                    }
                }
            }

            Func<DbDataReader, IList<TResult>> func;


            func = EntityHelper.EntityAssign<TResult>();

          //  Console.WriteLine(sql);

          //  DataTable table = this._provider.GetDbHelper().ExcuteQuery(sql, result.Parameters).Tables[0];
            result2.Data = this._provider.Factory.CreateDbHelper().ExecuteReader(sql, func, result.Parameters);
            return result2;
        }

        protected static string ReplaceInStr(string str)
        {
            string str2 = str.ToLower();
            if (((str2.Contains("insert") || str2.Contains("update")) || str2.Contains("delete")) || str2.Contains("select"))
            {
                return str.Replace('(', (char)0xff08).Replace(')', (char)0xff09);
            }
            return str;
        }

        protected static string ReplaceLikeStr(string str)
        {
            string str2 = str.ToLower();
            if (((str2.Contains("insert") || str2.Contains("update")) || str2.Contains("delete")) || str2.Contains("select"))
            {
                return str.Replace("'", "''").Replace('(', (char)0xff08).Replace(')', (char)0xff09);
            }
            return str;
        }

        //获取from 后面的数据

        private static string GetFromAfterStr(string sql) {

            sql = sql.ToLower();

            sql = sql.Substring(sql.IndexOf("from"));

            return sql;
        }
    }
}
