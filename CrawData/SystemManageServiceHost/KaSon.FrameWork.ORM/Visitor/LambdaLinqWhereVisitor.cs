namespace KaSon.FrameWork.ORM.Visitor
{
    using KaSon.FrameWork.ORM.Dal;
    using KaSon.FrameWork.Services.Attribute;
    using System;
    using System.Collections;
    using System.Data;
    using System.Linq.Expressions;
    using System.Text;

    internal class LambdaLinqWhereVisitor : ExpressionVisitor
    {
        private readonly StringBuilder _builde = new StringBuilder();
        protected LambdaContext Context;

        private static bool CheckIsNull(Expression exp)
        {
            if (exp.NodeType == ExpressionType.Constant)
            {
                return (((ConstantExpression)exp).Value == null);
            }
            return (((exp.NodeType == ExpressionType.Convert) && (((UnaryExpression)exp).Operand.NodeType == ExpressionType.Constant)) && (((ConstantExpression)((UnaryExpression)exp).Operand).Value == null));
        }

        public void Explain(Expression exp, LambdaContext context)
        {
            this.Context = context;
            Expression node = new SimplifyExpressionVisitor().Simplify(exp);
            this.Visit(node);
            this.Context.WhereText = this._builde.ToString();
        }

        protected virtual string GetParamerName()
        {
            return ("@P" + this.Context.ParamIndex);
        }

        protected virtual void Out(string s)
        {
            this._builde.Append(s);
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

                case ExpressionType.And:
                    str = "And";
                    break;

                case ExpressionType.AndAlso:
                    str = "And";
                    break;

                case ExpressionType.Divide:
                    str = "/";
                    break;

                case ExpressionType.Equal:
                    str = "=";
                    break;

                case ExpressionType.GreaterThan:
                    str = ">";
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    str = ">=";
                    break;

                case ExpressionType.LessThan:
                    str = "<";
                    break;

                case ExpressionType.LessThanOrEqual:
                    str = "<=";
                    break;

                case ExpressionType.Modulo:
                    str = "%";
                    break;

                case ExpressionType.MultiplyChecked:
                    str = "*";
                    break;

                case ExpressionType.NotEqual:
                    str = "<>";
                    break;

                case ExpressionType.Or:
                    str = "Or";
                    break;

                case ExpressionType.OrElse:
                    str = "Or";
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
            this.Out("(");
            this.Visit(node.Left);
            this.Out(" ");
            if (CheckIsNull(node.Right))
            {
                this.Out((str == "=") ? " IS NULL" : " IS NOT NULL");
            }
            else
            {
                this.Out(str);
                this.Out(" ");
                this.Visit(node.Right);
            }
            this.Out(")");
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value != null)
            {
                if ((node.Value is IEnumerable) && !node.Type.Equals(typeof(string)))
                {
                    int num = 0;
                    foreach (object obj2 in (IEnumerable)node.Value)
                    {
                        if (num > 0)
                        {
                            this.Out(", ");
                        }
                        bool flag = (obj2.GetType() == typeof(string)) || (obj2.GetType() == typeof(Guid));
                        if (flag)
                        {
                            this.Out("'");
                        }
                        this.Out(obj2.ToString());
                        if (flag)
                        {
                            this.Out("'");
                        }
                        num++;
                    }
                    return node;
                }
                string paramerName = this.GetParamerName();
                this.Out(paramerName);
                this.Context.Parameters.Insert(paramerName, node.Value, ParameterDirection.Input);
                return node;
            }
            this.Out("null");
            return node;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            this.Visit(node.Body);
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            this.Visit(node.Expression);
            object[] customAttributes = node.Member.GetCustomAttributes(typeof(FieldAttribute), true);
            int index = 0;
            while (index < customAttributes.Length)
            {
                object obj2 = customAttributes[index];
                this.Out(((FieldAttribute)obj2).Name);
                return node;
            }
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            string name = node.Method.Name;
            if (name != null)
            {
                if (!(name == "StartsWith"))
                {
                    if (name != "Contains")
                    {
                        if (name == "EndsWith")
                        {
                            this.Visit(node.Object);
                            this.Out(" Like ");
                            this.Out("'%");
                            this.VisitProperty(node);
                            this.Out("'");
                            return node;
                        }
                        if ((name == "All") || (name == "Any"))
                        {
                        }
                        return node;
                    }
                }
                else
                {
                    this.Visit(node.Object);
                    this.Out(" Like ");
                    this.Out("'");
                    this.VisitProperty(node);
                    this.Out("%'");
                    return node;
                }
                if (node.Object == null)
                {
                    this.Visit(node.Arguments[1]);
                    this.Out(" in (");
                    this.Visit(node.Arguments[0]);
                    this.Out(" ) ");
                    return node;
                }
                if (node.Object.Type.IsGenericType)
                {
                    this.Visit(node.Arguments[0]);
                    this.Out(" in (");
                    this.Visit(node.Object);
                    this.Out(" ) ");
                    return node;
                }
                this.Visit(node.Object);
                this.Out(" Like ");
                this.Out("'%");
                this.VisitProperty(node);
                this.Out("%'");
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
                    this.Out(",");
                }
                if (memberValue.GetType() == typeof(string))
                {
                    this.Out("'");
                    this.Out(memberValue.ToString());
                    this.Out("'");
                }
                else
                {
                    this.Out(memberValue.ToString());
                }
            }
            return node;
        }

        private void VisitProperty(MethodCallExpression node)
        {
            ValueVisitor visitor = new ValueVisitor();
            this.Out(visitor.GetMemberValue(node.Arguments[0]).ToString());
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            ExpressionType nodeType = node.NodeType;
            if (nodeType != ExpressionType.Convert)
            {
                if (nodeType != ExpressionType.Not)
                {
                    if (nodeType != ExpressionType.Quote)
                    {
                        this.Out(node.NodeType.ToString());
                        this.Out("(");
                    }
                }
                else
                {
                    this.Out("Not(");
                }
            }
            this.Visit(node.Operand);
            switch (node.NodeType)
            {
                case ExpressionType.Convert:
                    return node;

                case ExpressionType.Quote:
                    return node;
            }
            this.Out(")");
            return node;
        }
    }
}

