namespace KaSon.FrameWork.ORM.Visitor
{
    using System;
    using System.Linq.Expressions;

    internal class SkipLinqVisitor : LinqVisitor
    {
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                base.Result = result;
                this.Visit(expression.Arguments[0]);
                base.Result.IsSkip = true;
                base.Result.Skip = new int?((int) ((ConstantExpression) expression.Arguments[1]).Value);
            }
            return exp;
        }
    }
}

