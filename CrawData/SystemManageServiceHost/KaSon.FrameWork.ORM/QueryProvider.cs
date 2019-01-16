using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal class QueryProvider : IQueryProvider
    {
        private Provider.DbProvider _dbProvider;
        private QueryProviderContext _context;
        public QueryProvider(Provider.DbProvider dbProvider)
        {
            // TODO: Complete member initialization
            this._dbProvider = dbProvider;
            this._context = dbProvider.QueryProviderContext;
        }

        #region 提供者接口
        public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            Type type = typeof(TElement);
            if (!this._dbProvider.QueryContext.Parameters.Contains(type.FullName))
            {
                if (!(type.IsValueType || (type == typeof(string))))
                {
                    this._dbProvider.QueryContext.SafeAddQueryParameter(type);
                }
                if (type.Name.Equals("IGrouping`2"))
                {
                    this._dbProvider.Factory.CreateDbBuilder().SetGroupByKey(expression, this._dbProvider.QueryContext, type.FullName);
                }
            }
            return new Query<TElement>(this._dbProvider, expression);
        }

        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {

            string  method=(expression as MethodCallExpression).Method.Name;
            KaSon.FrameWork.ORM.QueryProviderContext<TResult> result = new KaSon.FrameWork.ORM.QueryProviderContext<TResult>(this._dbProvider)
            {
                ReturnType = typeof(TResult),
                IsExecute = true
            };
            Expression exp1 = expression;
            if ((exp1.NodeType == ExpressionType.Lambda) && true)
            {
                exp1 = ((LambdaExpression)exp1).Body;
            }
            if (exp1.NodeType == ExpressionType.Call)
            {
                MethodCallExpression expression2 = (MethodCallExpression)expression;
                method = expression2.Method.Name;
            }

           

                switch (method)
                {
                    case "First":
                    case "FirstPredicate":
                        return this.ExecuteQuery<TResult>(expression).First<TResult>();

                    case "FirstOrDefault":
                    case "FirstOrDefaultPredicate":
                        return this.ExecuteQuery<TResult>(expression).FirstOrDefault<TResult>();

                    case "Single":
                    case "SinglePredicate":
                        return this.ExecuteQuery<TResult>(expression).Single<TResult>();

                    case "SingleOrDefault":
                    case "SingleOrDefaultPredicate":
                        return this.ExecuteQuery<TResult>(expression).SingleOrDefault<TResult>();
                case "Sum":
                case "Any":
                    case "Count":
                    case "CountPredicate":
                    case "Min":
                    case "MinSelector":
                    case "Max":
                    case "MaxSelector":
                    case "MinInt":
                    case "MinNullableInt":
                    case "MinLong":
                    case "MinNullableLong":
                    case "MinDouble":
                    case "MinNullableDouble":
                    case "MinDecimal":
                    case "MinNullableDecimal":
                    case "MinSingle":
                    case "MinNullableSingle":
                    case "MinIntSelector":
                    case "MinNullableIntSelector":
                    case "MinLongSelector":
                    case "MinNullableLongSelector":
                    case "MinDoubleSelector":
                    case "MinNullableDoubleSelector":
                    case "MinDecimalSelector":
                    case "MinNullableDecimalSelector":
                    case "MinSingleSelector":
                    case "MinNullableSingleSelector":
                    case "MaxInt":
                    case "MaxNullableInt":
                    case "MaxLong":
                    case "MaxNullableLong":
                    case "MaxDouble":
                    case "MaxNullableDouble":
                    case "MaxDecimal":
                    case "MaxNullableDecimal":
                    case "MaxSingle":
                    case "MaxNullableSingle":
                    case "MaxIntSelector":
                    case "MaxNullableIntSelector":
                    case "MaxLongSelector":
                    case "MaxNullableLongSelector":
                    case "MaxDoubleSelector":
                    case "MaxNullableDoubleSelector":
                    case "MaxDecimalSelector":
                    case "MaxNullableDecimalSelector":
                    case "MaxSingleSelector":
                    case "MaxNullableSingleSelector":
                    case "SumInt":
                    case "SumNullableInt":
                    case "SumLong":
                    case "SumNullableLong":
                    case "SumDouble":
                    case "SumNullableDouble":
                    case "SumDecimal":
                    case "SumNullableDecimal":
                    case "SumSingle":
                    case "SumNullableSingle":
                    case "SumIntSelector":
                    case "SumNullableIntSelector":
                    case "SumLongSelector":
                    case "SumNullableLongSelector":
                    case "SumDoubleSelector":
                    case "SumNullableDoubleSelector":
                    case "SumDecimalSelector":
                    case "SumNullableDecimalSelector":
                    case "SumSingleSelector":
                    case "SumNullableSingleSelector":
                    case "AverageInt":
                    case "AverageNullableInt":
                    case "AverageLong":
                    case "AverageNullableLong":
                    case "AverageDouble":
                    case "AverageNullableDouble":
                    case "AverageDecimal":
                    case "AverageNullableDecimal":
                    case "AverageSingle":
                    case "AverageNullableSingle":
                    case "AverageIntSelector":
                    case "AverageNullableIntSelector":
                    case "AverageLongSelector":
                    case "AverageNullableLongSelector":
                    case "AverageDoubleSelector":
                    case "AverageNullableDoubleSelector":
                    case "AverageDecimalSelector":
                    case "AverageNullableDecimalSelector":
                    case "AverageSingleSelector":
                    case "AverageNullableSingleSelector":
                        result.SetExecuteFunc(new Func<QueryProviderContext, TResult>(QueryProvider.ExcuteSingle<TResult>));
                        break;
                }
            
            return this.Execute<TResult>(expression, result);

           // return default(TResult);
        }
      
        public object Execute(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }
        #endregion
       
        /// <summary>
        /// 一列执行 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        private static T ExcuteSingle<T>(QueryProviderContext result)
        {
            string queryText = result.GetQueryText();
           
            object obj2 = result.DbProvider.GetDbHelper().ExecuteScalar(queryText, result.Parameters);
            if (obj2 == null)
            {
                return default(T);
            }
            return (T)OperateCommon.AutoConvert(obj2, typeof(T));
        }
        /// <summary>
        /// 一行执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private T Execute<T>(Expression expression, QueryProviderContext<T> result)
        {
            this._dbProvider.Factory.CreateDbBuilder().GetLinqText(expression, result);
            this.Context = result;
            return result.Execute();
        }

        /// <summary>
        /// 执行查询(ToList 遍历是触发)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal IEnumerable<T> ExecuteQuery<T>(System.Linq.Expressions.Expression expression)
        {
            ///获取SQL 语句

            QueryProviderContext<IEnumerable<T>> context = new QueryProviderContext<IEnumerable<T>>(_dbProvider) { ReturnType = typeof(T) };
           // this._dbProvider.Factory.CreateDbBuilder().GetLinqText(expression, context);
            this.Context = context;
            context.SetExecuteFunc(new Func<QueryProviderContext, IEnumerable<T>>(QueryProvider.ExecuteMuilt<T>));
            return this.Execute<IEnumerable<T>>(expression, context);
        }

        /// <summary>
        ///  List 执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryResult"></param>
        /// <returns></returns>
        private static IEnumerable<T> ExecuteMuilt<T>(QueryProviderContext queryResult)
        {
            Func<DbDataReader, IList<T>> func;
            string queryText;
            if (queryResult.QueryColletion.Contains(queryResult.ReturnType.FullName))
            {
                if (queryResult.QueryColletion[queryResult.ReturnType.FullName].IsAnonym && (queryResult.Constructor != null))
                {
                    func = EntityHelper.EntityAssign<T>(queryResult);
                    queryText = queryResult.GetQueryText();
                    return queryResult.DbProvider.GetDbHelper().ExecuteReader<IList<T>>(queryText, func, queryResult.Parameters);
                }
                func = EntityHelper.EntityAssign<T>(queryResult);
                queryText = queryResult.GetQueryText();
                return queryResult.DbProvider.GetDbHelper().ExecuteReader<IList<T>>(queryText, func, queryResult.Parameters);
            }
            func = EntityHelper.EntityAssign<T>();
            queryText = queryResult.GetQueryText();
            return queryResult.DbProvider.GetDbHelper().ExecuteReader<IList<T>>(queryText, func, queryResult.Parameters);
        }
        internal QueryProviderContext Context
        {
            get
            {
                return this._context;
            }
            set {
                this._context = value;
              
            }
        }
    }
}
