using KaSon.FrameWork.ORM.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KaSon.FrameWork.ORM.IBuilder
{
    internal interface IDbBuilder
    {
        void CountText(LambdaContext context, Expression where, bool noLock);
        void DeleteText(LambdaContext context, Expression where);
        string GetDeleteText(EntityInfo entityInfo);
        string GetInsertText(EntityInfo entityInfo);
        string GetLinqText(QueryProviderContext context);
        void GetLinqText(Expression exp, QueryProviderContext result);

        /// <summary>
        /// 获取更新SQL 语句
        /// </summary>
        /// <param name="entityInfo"></param>
        /// <returns></returns>
        string GetUpdateText(EntityInfo entityInfo);

        /// <summary>
        /// 获取实体字段改变SQL更新语句 语句
        /// </summary>
        /// <returns></returns>
        string GetEmpayNotUpdateText(EntityInfo entityInfo);

        void LinqGroupByText(MethodCallExpression exp, QueryProviderContext context);
        void LinqHavingText(MethodCallExpression exp, QueryProviderContext context);
        void LinqJoinModeText(MethodCallExpression exp, QueryProviderContext context);
      
        void LinqOrderByDescendingText(MethodCallExpression exp, QueryProviderContext context);
        void LinqOrderByText(MethodCallExpression exp, QueryProviderContext context);
        void LinqPageText(MethodCallExpression exp, QueryProviderContext context);
        void LinqSelectManyText(MethodCallExpression exp, QueryProviderContext context);
        void LinqSelectText(MethodCallExpression exp, QueryProviderContext context);
        void LinqTakeText(MethodCallExpression exp, QueryProviderContext context);
        void LinqThenByDescendingText(MethodCallExpression exp, QueryProviderContext context);
        void LinqThenByText(MethodCallExpression exp, QueryProviderContext context);
        void LinqWhereText(MethodCallExpression exp, QueryProviderContext context);
        void SelectText(LambdaContext context, Expression where, bool noLock);
        void SelectText(LambdaContext context, Expression where, int top, bool noLock);
        void SelectText(LambdaContext context, Expression select, Expression where, bool noLock);
        void SetGroupByKey(Expression expression, QueryContext queryContext, string fullName);
        void SumText(LambdaContext context, Expression select, Expression where, bool noLock);
        void UpdateText(LambdaContext context, Expression updateExp, Expression whereExp);
    }
}
