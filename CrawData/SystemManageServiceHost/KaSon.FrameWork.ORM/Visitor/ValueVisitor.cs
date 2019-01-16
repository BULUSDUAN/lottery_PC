namespace KaSon.FrameWork.ORM.Visitor
{
    using System;
    using System.Linq.Expressions;

    public class ValueVisitor : ExpressionVisitor
    {
        private object _result = null;

        public object GetMemberValue(Expression exp)
        {
            this.Visit(exp);
            return this._result;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            this._result = node.Value;
            return node;
        }
    }
}

