namespace KaSon.FrameWork.ORM.Visitor.SQLServer
{
    using KaSon.FrameWork.ORM.Visitor;
   
    using System;
    using System.Linq.Expressions;

    internal class JoinVisitor : JoinLinqVisitor
    {
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (base.IsSelect)
            {
                if (node.Type.IsValueType && (node.Value != null))
                {
                    base.Result.SqlBuilder.Append(node.Value.ToString());
                }
                if ((node.Type == typeof(string)) && (node.Value != null))
                {
                    base.SelectBuilder.Append("'");
                    base.SelectBuilder.Append(node.Value.ToString());
                    base.SelectBuilder.Append("'");
                }
                return node;
            }
            if (node.Type.IsGenericType)
            {
                QueryParameter parameter = base.Result.QueryColletion[node.Type.GetGenericArguments()[0].FullName];
                if (parameter.IsEntity)
                {
                    this.Write("[");
                    this.Write(EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name);
                    this.Write("]");
                    this.Write(" AS ");
                }
                this.Write(parameter.Alias);
            }
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
            if (type != null)
            {
                QueryParameter parameter;
                if (base.IsSelect)
                {
                    if (base.Result.QueryColletion.Contains(type.FullName))
                    {
                        parameter = base.Result.QueryColletion[type.FullName];
                        base.SelectBuilder.Append(parameter.Alias);
                        base.SelectBuilder.Append(".");
                        base.SelectBuilder.Append("[");
                        base.SelectBuilder.Append(parameter.GetColumn(node.Member.Name).FieldMap);
                        base.SelectBuilder.Append("]");
                    }
                    return node;
                }
                if (!base.Result.QueryColletion.Contains(type.FullName))
                {
                    return node;
                }
                parameter = base.Result.QueryColletion[type.FullName];
                string item = parameter.Alias + ".[" + parameter.GetColumn(node.Member.Name).FieldMap + "]";
                if (base.IsLeft)
                {
                    base.LeftCondition.Add(item);
                    return node;
                }
                base.RightCondition.Add(item);
            }
            return node;
        }
    }
}

