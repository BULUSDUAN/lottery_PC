namespace KaSon.FrameWork.ORM.Visitor
{
  
    using System.Linq.Expressions;

    internal class DistinctLinqVisitor : LinqVisitor
    {
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                base.Result = result;
                this.Visit(expression.Arguments[0]);
                base.Result.IsDistinct = true;
            }
            return exp;
        }
    }
}

