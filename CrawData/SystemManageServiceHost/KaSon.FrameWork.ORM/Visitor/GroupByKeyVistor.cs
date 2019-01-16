namespace KaSon.FrameWork.ORM.Visitor
{
    using System;
    using System.Linq.Expressions;

    internal class GroupByKeyVistor : ExpressionVisitor
    {
        protected GroupByParam GroupBy = new GroupByParam();
        protected QueryContext QueryContext;

        public Expression Visit(Expression exp,QueryContext queryContext, string fullName)
        {
            this.QueryContext = queryContext;
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                this.Visit(expression.Arguments[1]);
            }
            queryContext.Parameters[fullName].GroupBy = this.GroupBy;

          

            return exp;
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
            if ((type != null) && this.QueryContext.Parameters.Contains(type.FullName))
            {
                GroupByParamKey key = new GroupByParamKey {
                    FullName = type.FullName,
                    MeberName = node.Member.Name
                };
                this.GroupBy.KeyList.Add(node.Member.Name, key);
            }
            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                if (node.Arguments[i] is MemberExpression)
                {
                    this.VisitNewKey(node.Arguments[i] as MemberExpression, node.Members[i].Name);
                }
                else
                {
                    this.Visit(node.Arguments[i]);
                }
            }
            this.QueryContext.SafeAddQueryParameter(node.Type);
            this.QueryContext.Parameters[node.Type.FullName].IsGroupKey = true;
            return node;
        }

        protected virtual Expression VisitNewKey(MemberExpression node, string key)
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
            if ((type != null) && this.QueryContext.Parameters.Contains(type.FullName))
            {
                QueryParameter parameter = this.QueryContext.Parameters[type.FullName];
                GroupByParamKey key2 = new GroupByParamKey {
                    FullName = type.FullName,
                    MeberName = node.Member.Name
                };
                this.GroupBy.KeyList.Add(key, key2);
            }
            return node;
        }
    }
}

