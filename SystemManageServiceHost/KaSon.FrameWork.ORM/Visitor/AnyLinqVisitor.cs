
namespace KaSon.FrameWork.ORM.Visitor
{

    using System;
    using System.Linq.Expressions;

    internal class AnyLinqVisitor : LinqVisitor
    {
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                base.Result = result;
                if (base.Result.ReturnType == typeof(bool))
                {
                    this.Visit(expression.Arguments[0]);
                    if (expression.Arguments.Count == 2)
                    {
                    }
                    return exp;
                }
                this.Write(" Exists(");
                QueryProviderContext result2 = new QueryProviderContext(result.DbProvider, result)
                {
                    ReturnType = expression.Arguments[0].Type.GetGenericArguments()[0]
                };
                result2.DbProvider.Factory.CreateDbBuilder().GetLinqText(expression.Arguments[0], result2);
                this.Write(result2.GetQueryText());
                this.Write(") ");
                if (expression.Arguments.Count == 2)
                {
                }
            }
            return exp;
        }
    }
}

