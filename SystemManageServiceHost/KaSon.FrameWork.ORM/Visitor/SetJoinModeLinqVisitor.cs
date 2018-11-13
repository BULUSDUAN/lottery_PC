
namespace KaSon.FrameWork.ORM.Visitor
{
    using KaSon.FrameWork.ORM;
    using KaSon.FrameWork.Services.Enum;
    using KaSon.FrameWork.Services.ORM;
    using System;
    using System.Linq.Expressions;

    internal class SetJoinModeLinqVisitor : LinqVisitor
    {
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                base.Result = result;
                this.Visit(expression.Arguments[0]);
                base.Result.JoinModes = ((ConstantExpression) expression.Arguments[1]).Value as JoinMode[];
            }
            return exp;
        }
    }
}

