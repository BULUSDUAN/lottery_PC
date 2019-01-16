using System.Linq.Expressions;
namespace KaSon.FrameWork.ORM.Visitor.SQLServer
{

    internal class SkipVisitor : SkipLinqVisitor
    {

        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                base.Result = result;
                this.Visit(expression.Arguments[0]);
                base.Result.IsSkip = true;
                base.Result.Skip = new int?((int)((ConstantExpression)expression.Arguments[1]).Value);
            }
            return exp;
        }
    }
}

