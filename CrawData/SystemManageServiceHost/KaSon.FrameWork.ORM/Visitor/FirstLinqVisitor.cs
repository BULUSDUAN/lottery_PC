namespace KaSon.FrameWork.ORM.Visitor
{

    using System.Linq.Expressions;

   
    internal class FirstLinqVisitor : LinqVisitor
    {
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                base.Result = result;
                result.Take = 1;
                this.Visit(expression.Arguments[0]);
            }
            return exp;
        }
    }
}

