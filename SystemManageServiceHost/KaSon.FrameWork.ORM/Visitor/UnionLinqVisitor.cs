namespace KaSon.FrameWork.ORM.Visitor
{
    using System.Linq.Expressions;

    internal class UnionLinqVisitor : LinqVisitor
    {
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                base.Result = result;
                this.Visit(expression.Arguments[0]);
                this.Write(" Union ");
                this.Visit(expression.Arguments[1]);
            }
            return exp;
        }
    }
}

