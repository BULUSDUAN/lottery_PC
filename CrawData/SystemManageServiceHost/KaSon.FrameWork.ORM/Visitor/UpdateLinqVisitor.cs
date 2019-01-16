namespace KaSon.FrameWork.ORM.Visitor
{

    using KaSon.FrameWork.ORM.Dal;
    using KaSon.FrameWork.Services.Attribute;
    using System;
    using System.Data;
    using System.Linq.Expressions;
    using System.Text;


    internal class UpdateLinqVisitor : ExpressionVisitor
    {
        protected readonly StringBuilder _builde = new StringBuilder();
        protected LambdaContext Context;

        public void Explain(Expression exp, LambdaContext context)
        {
            this.Context = context;
            SimplifyExpressionVisitor visitor = new SimplifyExpressionVisitor();
            this.Init();
            this.Visit(visitor.Simplify(exp));
            this.Context.UpdateText = this._builde.ToString();
        }

        protected virtual string GetParamerName()
        {
            return ("@P" + this.Context.ParamIndex);
        }

        protected virtual void Init()
        {
            this._builde.Append("UPDATE ");
            this._builde.Append(this.Context.Entity.Entity.Name);
            this._builde.Append(" SET ");
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
                    str = ",";
                    break;

                case ExpressionType.AndAlso:
                    str = ",";
                    break;

                case ExpressionType.Divide:
                    str = "/";
                    break;

                case ExpressionType.Equal:
                    str = "=";
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
            this.Visit(node.Left);
            this._builde.Append(str);
            this.Visit(node.Right);
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value != null)
            {
                string paramerName = this.GetParamerName();
                this._builde.Append(paramerName);
                this.Context.Parameters.Insert(paramerName, node.Value, ParameterDirection.Input);
                return node;
            }
            this._builde.Append("null");
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
                this._builde.Append(((FieldAttribute)obj2).Name);
                return node;
            }
            return node;
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            this.Visit(node.NewExpression);
            int num = 0;
            foreach (MemberAssignment assignment in node.Bindings)
            {
                foreach (object obj2 in assignment.Member.GetCustomAttributes(typeof(FieldAttribute), true))
                {
                    if (num > 0)
                    {
                        this._builde.Append(',');
                    }
                    this._builde.Append(((FieldAttribute)obj2).Name);
                    this._builde.Append(" = ");
                    if (assignment.Member.GetCustomAttributes(typeof(EncryptAttribute), true).Length > 0)
                    {
                        this._builde.Append("sys.ENCRYPTBYKEY(");
                        this.Visit(assignment.Expression);
                        this._builde.Append(')');
                    }
                    else
                    {
                        this.Visit(assignment.Expression);
                    }
                    num++;
                }
            }
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            this.Visit(node.Operand);
            return node;
        }

        internal class SqlServerUpdateVisitor : ExpressionVisitor
        {
            private readonly StringBuilder _builde = new StringBuilder();
            protected LambdaContext Context;

            public void Explain(Expression exp, LambdaContext context)
            {
                this.Context = context;
                SimplifyExpressionVisitor visitor = new SimplifyExpressionVisitor();
                this.Init();
                this.Visit(visitor.Simplify(exp));
                this.Context.UpdateText = this._builde.ToString();
            }

            protected virtual string GetParamerName()
            {
                return ("@P" + this.Context.ParamIndex);
            }

            protected virtual void Init()
            {
                this._builde.Append("UPDATE ");
                this._builde.Append(this.Context.Entity.Entity.Name);
                this._builde.Append(" SET ");
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
                        str = ",";
                        break;

                    case ExpressionType.AndAlso:
                        str = ",";
                        break;

                    case ExpressionType.Divide:
                        str = "/";
                        break;

                    case ExpressionType.Equal:
                        str = "=";
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
                this.Visit(node.Left);
                this._builde.Append(str);
                this.Visit(node.Right);
                return node;
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                if (node.Value != null)
                {
                    string paramerName = this.GetParamerName();
                    this._builde.Append(paramerName);
                    this.Context.Parameters.Insert(paramerName, node.Value, ParameterDirection.Input);
                    return node;
                }
                this._builde.Append("null");
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
                    this._builde.Append(((FieldAttribute)obj2).Name);
                    return node;
                }
                return node;
            }

            protected override Expression VisitMemberInit(MemberInitExpression node)
            {
                this.Visit(node.NewExpression);
                int num = 0;
                foreach (MemberAssignment assignment in node.Bindings)
                {
                    foreach (object obj2 in assignment.Member.GetCustomAttributes(typeof(FieldAttribute), true))
                    {
                        if (num > 0)
                        {
                            this._builde.Append(',');
                        }
                        this._builde.Append('[');
                        this._builde.Append(((FieldAttribute)obj2).Name);
                        this._builde.Append("] = ");
                        this.Visit(assignment.Expression);
                        num++;
                    }
                }
                return node;
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                this.Visit(node.Operand);
                return node;
            }
        }
    }
}

