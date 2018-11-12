using KaSon.FrameWork.Services.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace KaSon.FrameWork.ORM.Visitor.MySql
{

    internal class WhereVisitor : Visitor.WhereLinqVisitor
    {

        protected override string GetParamerName()
        {
            return ("@P" + base.Result.ParamIndex);
        }
        /// <summary>
        /// 是否是右边的成员
        /// </summary>
        private bool _memberIsRight = false;
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            string method = "";
            base.Result = result;
            Expression exp1 = exp;
            MethodCallExpression expression = exp as MethodCallExpression;
            method = expression.Method.Name;
            if (expression == null)
            {
                goto Label_023F;
            }

            QueryParameter parameter = base.Result.QueryColletion[expression.Arguments[0].Type.GetGenericArguments()[0].FullName];

            if (expression.Arguments[0] is ConstantExpression)
            {

                if (parameter.IsEntity)
                {
                   // this.Write("[");
                    this.Write(EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name);
                  //  this.Write("]");
                }
                goto Label_0163;
            }
            if ((exp1.NodeType == ExpressionType.Lambda) && true)
            {
                exp1 = ((LambdaExpression)exp1).Body;
            }
            if (exp1.NodeType == ExpressionType.Call)
            {
                MethodCallExpression expression2 = (MethodCallExpression)expression;
                method = expression2.Method.Name;
            }

            if (expression.NodeType == ExpressionType.Call)
            {

                switch (method)
                {
                    case "SelectMany":
                    case "SelectManyResultSelector":
                    case "Join":
                    case "Where":
                    case "GroupBy":
                    case "GroupByElementSelector":
                    case "GroupByResultSelector":
                        goto Label_0137;

                    //  goto Label_0138;

                }
                this.Write("(");
            }
        Label_0137:
            this.Visit(expression.Arguments[0]);
            if (!parameter.HasAs)
            {
                this.Write(")");
            }
        //Label_0138:
        //    this.Visit(expression.Arguments[0]);
        //    if (!parameter.HasAs)
        //    {
        //        this.Write(")");
        //    }
        Label_0163:
            if (WhereLinqVisitor.IsNullWhere(expression.Arguments[1]))
            {
                return exp;
            }
            if (!parameter.HasAs)
            {
                this.Write(" AS ");
                this.Write(parameter.Alias);
                parameter.HasAs = true;
            }
            QueryParameter parameter2 = base.Result.QueryColletion[expression.Type.GetGenericArguments()[0].FullName];
            if (parameter.GroupBy == null)
            {
                //  base.Result. = true;

                this.Write(parameter2.HasWhere ? " And " : " Where ");
            }
            else
            {
                base.Result.IsGroup = true;
                this.Write(parameter2.HasWhere ? " And " : " Having ");
            }
            parameter2.HasWhere = true;
            this.Visit(expression.Arguments[1]);

            return exp;
        Label_023F:
            base.Result.HasSelect = false;
            return exp;
        }

        /// <summary>
        /// 运算符
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            string str;

            ///是否连接运算符
            bool m_isLinkBinary = false;

            switch (node.NodeType)
            {
                case ExpressionType.Add:
                    str = "+";
                    break;

                case ExpressionType.AddChecked:
                    str = "+";
                    break;

                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    str = "And";
                    m_isLinkBinary = true;
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
                case ExpressionType.OrElse:
                    str = "Or";
                    m_isLinkBinary = true;
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
            this.Write(" ");
            if (CheckIsNull(node.Right))
            {
                this.Write((str == "=") ? " IS NULL" : " IS NOT NULL");
            }
            else
            {
                this.Write(str);
                this.Write(" ");


                //标识是右边的 成员过来的
                _memberIsRight = !m_isLinkBinary;


                this.Visit(node.Right);
            }
            this.Write(")");
            return node;
        }
        /// <summary>
        /// 常数表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value == null)
            {
                this.Write("null");
                return node;
            }
            //  Type t = node.Value.GetType();
            string paramerName = this.GetParamerName();
            this.Write(paramerName);
            base.Result.Parameters.Insert(paramerName, node.Value, ParameterDirection.Input);
            return node;
        }



        /// <summary>
        /// 成员表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {

            // dynamic


            ///是否是右边成员过来的
            if (_memberIsRight)
            {

                _memberIsRight = false;
                object value = null;
                if (node.Member.MemberType == MemberTypes.Field)
                {
                    value = base.GetFieldValue(node);
                }
                else if (node.Member.MemberType == MemberTypes.Property)
                {

                    value = base.GetPropertyValue(node);
                }

                string paramerName = this.GetParamerName();
                this.Write(paramerName);
                base.Result.Parameters.Insert(paramerName, value, ParameterDirection.Input);

                // this.Visit(node.Expression);


                return node;
            }


            Type type = node.Type;
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
               // this.Write("[");
                this.Write(parameter.GetColumn(node.Member.Name).FieldMap);
               // this.Write("]");
            }



            return node;
        }


        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            string method = node.Method.Name;
            switch (method)
            {
                case "ContainsString":
                case "ContainsSingle":
                case "ContainsDecimal":
                case "ContainsDouble":
                case "ContainsInt32":
                case "ContainsInt64":
                case "Contains":
                    return base.Result.VisitorBuilder.Contains().Visit(node, base.Result);

                case "StartsWith":
                    this.Visit(node.Object);
                    this.Write(" Like '");
                    this.VisitValue(node.Arguments[0]);
                    this.Write("%' ");
                    return node;

                case "EndsWith":
                    this.Visit(node.Object);
                    this.Write(" Like '%");
                    this.VisitValue(node.Arguments[0]);
                    this.Write("' ");
                    return node;

                case "ConvertToDecimal":
                    this.Write(" convert( decimal, ");
                    this.Visit(node.Arguments[0]);
                    this.Write(") ");
                    return node;

                case "ConvertToInt64":
                    this.Write(" convert( bitint, ");
                    this.Visit(node.Arguments[0]);
                    this.Write(") ");
                    return node;

                case "ConvertToInt32":
                    this.Write(" convert( int, ");
                    this.Visit(node.Arguments[0]);
                    this.Write(") ");
                    return node;

                case "ConvertToDouble":
                    this.Write(" convert( double, ");
                    this.Visit(node.Arguments[0]);
                    this.Write(") ");
                    return node;

                case "ConvertToSingle":
                    this.Write(" convert( float, ");
                    this.Visit(node.Arguments[0]);
                    this.Write(") ");
                    return node;

                case "ConvertToDatetime":
                    this.Write(" convert( datetime, ");
                    this.Visit(node.Arguments[0]);
                    this.Write(") ");
                    return node;
                default:
                    return base.VisitMethodCall(node);



            }
            //foreach (Expression expression in node.Arguments)
            //{
            //    this.Visit(expression);
            //}
            //this.Visit(node.Object);
            return node;
        }
    }
}
