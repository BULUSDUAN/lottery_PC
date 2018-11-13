namespace KaSon.FrameWork.ORM.Visitor
{
    using System;
    using System.Linq.Expressions;

    internal class GroupByLinqVisitor : LinqVisitor
    {
        public virtual Expression Visit(MethodCallExpression node, QueryProviderContext result)
        {
            base.Result = result;
            if (node.Arguments[0] is ConstantExpression)
            {
                EntityInfo entityInfo = EntityHelper.GetEntityInfo(node.Arguments[0].Type.GetGenericArguments()[0]);
                if (entityInfo.IsEntity)
                {
                    this.Write(entityInfo.Entity.Name);
                    this.Write(" AS ");
                }
                this.Write(base.Result.QueryColletion[node.Arguments[0].Type.GetGenericArguments()[0].FullName].Alias);
            }
            this.Visit(node.Arguments[0]);
            this.Write(" Group by ");
            this.Visit(node.Arguments[1]);
            base.Result.QueryColletion[node.Type.GetGenericArguments()[0].FullName].HasAs = true;
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            Type type = null;
            if (node.Expression is ParameterExpression)
            {
                type = (node.Expression as ParameterExpression).Type;
            }
            if (node.Expression is MemberExpression)
            {
                type = (node.Expression as MemberExpression).Type;
            }
            if ((type != null) && base.Result.QueryColletion.Contains(type.FullName))
            {
                QueryParameter parameter = base.Result.QueryColletion[type.FullName];
                this.Write(parameter.Alias);
                this.Write(".");
                this.Write(parameter.GetColumn(node.Member.Name).FieldMap);
            }
            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                if (i > 0)
                {
                    this.Write(",");
                }
                this.Visit(node.Arguments[i]);
            }
            return node;
        }
    }
}

