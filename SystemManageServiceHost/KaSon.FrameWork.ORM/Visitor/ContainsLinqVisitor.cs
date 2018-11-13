namespace KaSon.FrameWork.ORM.Visitor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class ContainsLinqVisitor : LinqVisitor
    {
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                base.Result = result;
                if (expression.Arguments.Count == 1)
                {
                    if ((expression.Object is MemberExpression) && !(expression.Object.Type.Name.ToString().Contains("List")))
                    {
                        this.Visit(expression.Object);
                        this.Write(" LIKE '%");
                        this.Visit(expression.Arguments[0]);
                        this.Write("%'");
                        return exp;
                    }
                    this.Visit(expression.Arguments[0]);
                    this.Write(" IN (");
                    if (expression.Object == null)
                    {
                        throw new Exception("IN不能是空集合");
                    }
                    this.Visit(expression.Object);
                    this.Write(")");
                    return exp;
                }
                this.Visit(expression.Arguments[1]);
                this.Write(" IN (");
                this.Visit(expression.Arguments[0]);
                this.Write(")");
            }
            return exp;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value == null)
            {
                this.Write("null");
                return node;
            }
            if ((node.Value is IEnumerable) && !node.Type.Equals(typeof(string)))
            {
                int num = 0;
                foreach (object obj2 in (IEnumerable) node.Value)
                {
                    if (num > 0)
                    {
                        this.Write(", ");
                    }
                    if (obj2.GetType().Equals(typeof(string)))
                    {
                        this.Write("'");
                    }
                    this.Write(obj2.ToString());
                    if (obj2.GetType().Equals(typeof(string)))
                    {
                        this.Write("'");
                    }
                    num++;
                }
                return node;
            }
            this.Write(node.Value.ToString());
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

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            for (int i = 0; i < node.Expressions.Count; i++)
            {
                object memberValue = new ValueVisitor().GetMemberValue(node.Expressions[i]);
                if (i > 0)
                {
                    this.Write(",");
                }
                if (memberValue.GetType() == typeof(string))
                {
                    this.Write("'");
                    this.Write(memberValue.ToString());
                    this.Write("'");
                }
                else
                {
                    this.Write(memberValue.ToString());
                }
            }
            return node;
        }
    }
}

