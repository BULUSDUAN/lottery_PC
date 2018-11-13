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
    public class SqlServerExtension : ICommonQuery
    {
        private readonly DbProvider _provider;

        public SqlServerExtension(DbProvider provider)
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


        private static string GetMultWhereSql(QueryArgs args, MultEntityInfo info, KaSon.FrameWork.Services.ORM.DbParameterCollection parameters)
        {
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
                str = str3 + ((i > 0) ? "," : "") + "[" + args.SortFields[i].Field + "]" + (args.SortFields[i].IsASC ? " ASC" : " DESC");
            }
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new Exception("没有设置OrderBy字段");
            }
            return str;
        }
        private static string GetMultOrderBySql(QueryArgs args, string Alias,QueryProviderContext result)
        {
            string str = string.Empty;
            for (int i = 0; i < args.SortFields.Count; i++)
            {
                string str3 = str;
                str = str3 + ((i > 0) ? "," : "") + Alias + "[" + args.SortFields[i].Field + "]" + (args.SortFields[i].IsASC ? " ASC" : " DESC");
            }
            if (string.IsNullOrWhiteSpace(str))
            {
                //throw new Exception("没有设置OrderBy字段");
                //默认Order by 为返回类型的第一个字段

                var entity = GlobalCache.EntityInfoPool[result.ReturnType];
                var first = entity.Fields.First().Key;
                ///获取实体别名
                string ModelAlis = Alias;
                str = ModelAlis + first;


            }
            return str;
        }

        private static string GetMultWhereSql(QueryArgs args, Type type, QueryProviderContext result,string sql="")
        {

            KaSon.FrameWork.Services.ORM.DbParameterCollection parameters = result.Parameters;


            if ((args.WhereFields == null) || (args.WhereFields.Count == 0))
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();

            //加上字段实体
            //在输出字段查找实体


            string TableModel = "";


            string selectFieldText = GetSelectAfterStr(sql);


           


            for (int i = 0; i < args.WhereFields.Count; i++)
            {
                WhereField field = args.WhereFields[i];
                string name = "@" + field.Field + i;
                bool flag = true;
                if (field.Value != null)
                {
                    //加上字段实体
                    //在输出字段查找实体
                    if (selectFieldText.ToLower().Contains(field.Field.ToLower()))
                    {
                        int t = selectFieldText.ToLower().IndexOf("as " + field.Field.Trim().ToLower());

                       string temp= selectFieldText.Substring(0,t);
                       var arr= temp.Split('.');
                       temp = arr[arr.Length - 2];

                       if (temp.IndexOf(',') > 0)
                       {
                           TableModel = temp.Split(',')[1].ToUpper() + ".";
                       }
                       else
                       {
                           TableModel = temp.Split(' ')[1].ToUpper() + ".";
                       }

                      

                    }

                    // Select T5
                    //"[headimg] as headimg,t1"
                    //"select t5.[id] as deviceid,t1.[id] as id,t1.[wxaccountid] as wxaccountid,t1.[wxcategoryid] as wxcategoryid,t1.[wxnumber] as wxnumber,t1.[name] as name,t1.[telphone] as telphone,t1.[mark] as mark,t1.[headimg] as headimg,t1.[createtime] as createtime,t5.[name] as devicename,t2.[wxnumber] as wxlogin,t3.[name] as wxcategoryname "

                 //   string Alias = result.QueryColletion[type.FullName].Alias + ".";
                    string str5;
                    string str = field.Value;
                    string str3 = "";
                    str3 = (builder.Length > 0) ? " AND " : "";
                    switch (field.WhereType)
                    {
                        case WhereType.UnEqual:
                            str3 = str3 + TableModel + field.Field + "<>" + name;
                            break;

                        case WhereType.Less:
                            str3 = str3 + TableModel + field.Field + "<" + name;
                            break;

                        case WhereType.LessOrEqual:
                            str3 = str3 + TableModel + field.Field + "<=" + name;
                            break;

                        case WhereType.Than:
                            str3 = str3 + TableModel + field.Field + ">" + name;
                            break;

                        case WhereType.ThanOrEqual:
                            str3 = str3 + TableModel + field.Field + ">=" + name;
                            break;

                        case WhereType.LeftLike:
                            flag = false;
                            str = SqlServerExtension.ReplaceLikeStr(str);
                            str5 = str3;
                            str3 = str5 + TableModel + field.Field + " Like '" + str + "%'";
                            break;

                        case WhereType.RightLike:
                            flag = false;
                            str = SqlServerExtension.ReplaceLikeStr(str);
                            str5 = str3;
                            str3 = str5 + TableModel + field.Field + " Like '%" + str + "'";
                            break;

                        case WhereType.Like:
                            flag = false;
                            str = SqlServerExtension.ReplaceLikeStr(str);
                            str5 = str3;
                            str3 = str5 + TableModel + field.Field + " Like '%" + str + "%'";
                            break;

                        case WhereType.In:
                            flag = false;
                            str = SqlServerExtension.ReplaceInStr(str);
                            str5 = str3;
                            str3 = str5 + TableModel + field.Field + " IN (" + str + ")";
                            break;

                        case WhereType.NotIn:
                            flag = false;
                            str = SqlServerExtension.ReplaceInStr(str);
                            str5 = str3;
                            str3 = str5 + TableModel + field.Field + " NOT IN (" + str + ")";
                            break;

                        default:
                            str3 = str3 + TableModel + field.Field + "=" + name;
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
                //    sql = " SELECT Count(*) FROM( " + sql + ") T";


                  //  sql = "SELECT Count(*) " + GetFromAfterStr(sql);

                    sql = "SELECT Count(*) " + GetFromAfterStr(sql);

                    result2.RowCount = Convert.ToInt32(this._provider.GetDbHelper().ExecuteScalar(sql, result.Parameters));
                }


           

                Page page = new Page
                {
                    Index = args.PageIndex,
                    Row = args.PageSize,
                    OrderBy = " Order by " + ((multEntityInfo == null) ? GetMultOrderBySql(args, Alias, result) : GetMultOrderBySql(args, multEntityInfo)),
                    AppendOrderBy = " Order by " + ((multEntityInfo == null) ? GetMultOrderBySql(args, "T.", result) : GetMultOrderBySql(args, multEntityInfo))
                
                };
                result.Page = page;
                sql = result.GetQueryText();
            }
            else
            {
                sql = result.GetQueryText();

             

                string str2 = (multEntityInfo == null) ? GetMultWhereSql(args, entityType, result, sql) : GetMultWhereSql(args, multEntityInfo, result.Parameters);
                if (result2.RowCount == 0)
                {
                   // string str3 = "SELECT Count(*) FROM (" + sql + ") T";
                    string str3 = sql;
                    if (!string.IsNullOrWhiteSpace(str2))
                    {
                        if (str3.ToLower().Contains("where"))
                        {
                            string[] arr = GetWhereSub(sql);
                            sql = arr[0] + " WHERE   " + str2 + " AND " + arr[1];
                        }
                        else
                        {
                            str3 = str3 + " WHERE " + str2;
                        }
                    }

                    str3 = "SELECT Count(*) " +GetFromAfterStr( str3) ;


                    result2.RowCount = Convert.ToInt32(this._provider.GetDbHelper().ExecuteScalar(str3, result.Parameters));
                }


                if (args.PageIndex > 0)
                {

                    string temp = "SELECT row_number() over ( order by " + ((multEntityInfo == null) ? GetMultOrderBySql(args, Alias, result) : GetMultOrderBySql(args, multEntityInfo)) + ")as __rownumber,";


                    sql = sql.Replace("SELECT", temp);
                   
                    if (!string.IsNullOrWhiteSpace(str2))
                    {
                        sql = sql + " WHERE " + str2;
                    }
                 //   sql = string.Concat(new object[] { "SELECT * FROM (", sql, ") T WHERE  __rownumber  <=", args.PageIndex * args.PageSize, " AND __rownumber >", (args.PageIndex - 1) * args.PageSize, " order by " + ((multEntityInfo == null) ? GetMultOrderBySql(args, "T.", result) : GetMultOrderBySql(args, multEntityInfo)) });
                    sql = string.Concat(new object[] { "SELECT * FROM (", sql, ") T WHERE  __rownumber  <=", args.PageIndex * args.PageSize, " AND __rownumber >", (args.PageIndex - 1) * args.PageSize, " order by " + ((multEntityInfo == null) ? GetMultOrderBySql(args, "T.", result) : GetMultOrderBySql(args, multEntityInfo)) });

                }
                else
                {
               
                    if (!string.IsNullOrWhiteSpace(str2))
                    {
                        if (sql.ToLower().Contains("where"))
                        {
                            string[] arr = GetWhereSub(sql);
                            sql = arr[0] + " WHERE   " + str2+" AND "+arr[1];
                        }
                        else
                        {
                            sql = sql + " WHERE " + str2;
                        }
                    }
                }
            }

            Func<DbDataReader, IList<TResult>> func;


            func = EntityHelper.EntityAssign<TResult>();

           // Console.WriteLine(sql);
          
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

        private static string GetFromAfterStr(string sql)
        {

            sql = sql.ToLower();

            sql = sql.Substring(sql.IndexOf("from"));

            return sql;
        }
        private static string[] GetWhereSub(string sql)
        {

            sql = sql.ToLower();

            string[] arr = { sql.Substring(0, sql.IndexOf("where")), sql.Substring(sql.IndexOf("where")+5) };

            return arr;
        }
        private static string GetSelectAfterStr(string sql)
        {

            sql = sql.ToLower();

            sql = sql.Substring(0,sql.IndexOf("from"));

            return sql;
        }
      
    }
}
