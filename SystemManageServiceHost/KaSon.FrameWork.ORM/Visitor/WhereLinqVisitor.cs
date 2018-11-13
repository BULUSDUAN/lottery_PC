namespace KaSon.FrameWork.ORM.Visitor
{
    using System;
    using System.Data;
    using System.Linq.Expressions;

    internal class WhereLinqVisitor : LinqVisitor
    {
        internal static bool CheckIsNull(Expression exp)
        {
            if (exp.NodeType == ExpressionType.Constant)
            {
                return (((ConstantExpression) exp).Value == null);
            }
            return (((exp.NodeType == ExpressionType.Convert) && (((UnaryExpression) exp).Operand.NodeType == ExpressionType.Constant)) && (((ConstantExpression) ((UnaryExpression) exp).Operand).Value == null));
        }

        protected virtual string GetParamerName()
        {
            return ("@P" + base.Result.ParamIndex);
        }

        internal static bool IsNullWhere(Expression exp)
        {
            return (((exp.NodeType == ExpressionType.Quote) && (((UnaryExpression) exp).Operand.NodeType == ExpressionType.Lambda)) && ((((LambdaExpression) ((UnaryExpression) exp).Operand).Body.NodeType == ExpressionType.Constant) && (((LambdaExpression) ((UnaryExpression) exp).Operand).Body.Type == typeof(bool))));
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
            this.Write("(");
            this.WriteWhere("(");

            this.Visit(node.Left);

            this.Write(" ");
            this.WriteWhere(" ");

          

            if (CheckIsNull(node.Right))
            {
                string _temp = (str == "=") ? " IS NULL" : " IS NOT NULL";
                this.Write(_temp);
                this.WriteWhere(_temp);
            }
            else
            {
                this.Write(str);
                this.Write(" ");

                this.WriteWhere(str);
                this.WriteWhere(" ");


                this.Visit(node.Right);
            }
            this.Write(")");

            this.WriteWhere(")");


            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value == null)
            {
                this.Write("null");

                this.WriteWhere("null");
                return node;
            }
            string paramerName = this.GetParamerName();
            this.Write(paramerName);
            base.Result.Parameters.Insert(paramerName, node.Value, ParameterDirection.Input);
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

                this.WriteWhere(parameter.Alias);
                this.WriteWhere(".");

                this.Write(parameter.GetColumn(node.Member.Name).FieldMap);
            }
            return node;
        }

        //protected override Expression VisitMethodCall(MethodCallExpression node)
        //{
        //    string method;
        //    Expression expression = node;
        //    Expression expression2 = node;
        //    if ((expression.NodeType == ExpressionType.Lambda))
        //    {
        //        expression = ((LambdaExpression)expression).Body;
        //    }
        //    if (expression.NodeType == ExpressionType.Call)
        //    {
        //         expression2 = (MethodCallExpression)expression;
        //         base.VisitMethodCall(node);
        //         return node;
        //    }

        //    method = node.Method.Name;
        //    switch (method)
        //    {
        //        case "ContainsString":
        //        case "ContainsSingle":
        //        case "ContainsDecimal":
        //        case "ContainsDouble":
        //        case "ContainsInt32":
        //        case "ContainsInt64":
        //            return base.Result.VisitorBuilder.Contains().Visit(node, base.Result);

        //        case "StartsWith":
        //            this.Visit(node.Object);
        //            this.Write(" Like '");
        //            this.VisitValue(node.Arguments[0]);
        //            this.Write("%' ");
        //            return node;

        //        case "EndsWith":
        //            this.Visit(node.Object);
        //            this.Write(" Like '%");
        //            this.VisitValue(node.Arguments[0]);
        //            this.Write("' ");
        //            return node;

        //        case "ConvertToDecimal":
        //        case "ConvertToInt64":
        //        case "ConvertToInt32":
        //        case "ConvertToDouble":
        //        case "ConvertToSingle":
        //            this.Write(" to_number( ");
        //            this.Visit(node.Arguments[0]);
        //            this.Write(") ");
        //            return node;

        //        case "ConvertToDatetime":
        //            this.Write(" to_date( ");
        //            this.Visit(node.Arguments[0]);
        //            this.Write(") ");
        //            return node;
        //    }
        //    foreach (Expression expression1 in node.Arguments)
        //    {
        //        this.Visit(expression1);
        //    }
        //    this.Visit(node.Object);
        //    return node;
        //}

        protected virtual void VisitValue(Expression node)
        {
            ValueVisitor visitor = new ValueVisitor();
            string _temp = visitor.GetMemberValue(node).ToString();
            this.Write(_temp);

            this.WriteWhere(_temp);

        }
    }
}

