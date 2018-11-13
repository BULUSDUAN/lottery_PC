namespace KaSon.FrameWork.ORM.Visitor
{
    using System;
    using System.Linq.Expressions;

    internal class SelectManyLinqVisitor : LinqVisitor
    {
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                base.Result = result;
                this.Visit(expression.Arguments[0]);
                this.Write(", ");
                this.Visit(expression.Arguments[1]);
                base.Result.QueryColletion[exp.Type.GetGenericArguments()[0].FullName].HasAs = true;
            }
            return exp;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            this.VisitTable(node);
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            this.VisitTable(node);
            return node;
        }

        protected virtual void VisitTable(Expression exp)
        {
            if (exp.Type.IsGenericType)
            {
                QueryParameter parameter = base.Result.QueryColletion[exp.Type.GetGenericArguments()[0].FullName];
                if (parameter.IsEntity)
                {
                    this.Write(EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name);
                    this.Write(" AS ");
                }
                this.Write(parameter.Alias);
            }
        }
    }
}

