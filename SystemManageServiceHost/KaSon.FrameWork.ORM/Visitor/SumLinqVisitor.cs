namespace KaSon.FrameWork.ORM.Visitor
{
    using System;
    using System.Linq.Expressions;

    internal class SumLinqVisitor : LinqVisitor
    {
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                base.Result = result;
                QueryParameter parameter = base.Result.QueryColletion.Contains(expression.Arguments[0].Type.FullName) ? base.Result.QueryColletion[expression.Arguments[0].Type.FullName] : base.Result.QueryColletion[expression.Arguments[0].Type.GetGenericArguments()[0].FullName];
                if (parameter.GroupBy == null)
                {

                  

                    string method;
                    if (expression != null){
                        if (expression.Arguments.Count > 1)
                        {
                             method = expression.Method.Name;
                        if (method == "Select")
                        {
                                result.IsSum = true;
                            }
                            else
                            {
                                this.Write("SELECT SUM(");
                                this.Visit(expression.Arguments[1]);
                                this.Write(") FROM ");
                                base.Result.HasSelect = true;
                            }
                            this.Visit(expression.Arguments[0]);
                            return exp;
                        }
                        result.IsSum = true;
                        this.Visit(expression.Arguments[0]);
                        return exp;
                    }
                    if (expression.Arguments[0] is ConstantExpression)
                    {
                        if (!(!parameter.IsEntity || parameter.HasAs))
                        {
                            this.Write(EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name);
                            this.Write(" AS ");
                            parameter.HasAs = true;
                        }
                        this.Write(parameter.Alias);
                    }
                    return exp;
                }
                this.Write(" SUM(");
                this.Visit(expression.Arguments[1]);
                this.Write(") ");
            }
            return exp;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            string str;
            switch (node.NodeType)
            {
                case ExpressionType.Add:
                    str = "+";
                    break;

                case ExpressionType.AddChecked:
                    str = "+";
                    break;

                case ExpressionType.Divide:
                    str = "/";
                    break;

                case ExpressionType.Modulo:
                    str = "%";
                    break;

                case ExpressionType.MultiplyChecked:
                    str = "*";
                    break;

                case ExpressionType.Subtract:
                    str = "-";
                    break;

                case ExpressionType.SubtractChecked:
                    str = "-";
                    break;

                default:
                    throw new InvalidOperationException();
            }
            this.Write("(");
            this.Visit(node.Left);
            this.Write(str);
            this.Visit(node.Right);
            this.Write(")");
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            QueryParameter parameter;
            if (base.Result.QueryColletion.Contains(node.Type.FullName))
            {
                parameter = base.Result.QueryColletion[node.Type.FullName];
                for (int i = 0; i < parameter.Columns.Count; i++)
                {
                    if (i > 0)
                    {
                        this.Write(", ");
                    }
                    this.Write(parameter.Alias);
                    this.Write(".");
                    this.Write(parameter.Columns[i].FieldMap);
                }
                return node;
            }
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
                parameter = base.Result.QueryColletion[type.FullName];
                this.Write(parameter.Alias);
                this.Write(".");
                this.Write(parameter.GetColumn(node.Member.Name).FieldMap);
            }
            return node;
        }
    }
}

