namespace KaSon.FrameWork.ORM.Visitor
{

    using System;
    using System.Linq.Expressions;

    internal class PageLinqVisitor : LinqVisitor
    {
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                base.Result = result;
                int num = (int)((ConstantExpression)expression.Arguments[1]).Value;
                int num2 = (int)((ConstantExpression)expression.Arguments[2]).Value;
                Page page = new Page
                {
                    Row = num,
                    Index = num2
                };
                base.Result.Page = page;
                this.Visit(expression.Arguments[0]);
            }
            return exp;
        }
    }
}

