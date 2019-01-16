using KaSon.FrameWork.Services.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace KaSon.FrameWork.ORM.Visitor.SQLServer
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
        private Expression _bnode = null;
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            _bnode = exp;
        string method="";
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
                    this.Write("[");
                    this.Write(EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name);
                    this.Write("]");
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
        /// 
      
        protected override Expression VisitBinary(BinaryExpression node)
        {
            string str;
            _bnode = node;
            ///是否连接运算符
            bool m_isLinkBinary=false;

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
                    m_isLinkBinary=true;
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
                     m_isLinkBinary=true;
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


            string tempsql = "";
            if (CheckIsNull(node.Right))
            {
                tempsql = (str == "=") ? " IS NULL" : " IS NOT NULL";
                this.Write(tempsql);
            }
            else if (node.Right.Type.Name.Contains("Nullable") )
            {
                MemberExpression me = node.Right as MemberExpression;
                if (node.Right is UnaryExpression) {
                    var un = node.Right as UnaryExpression;
                    me = un.Operand as MemberExpression;
                }
               
               // memberExpr = un.Operand as MemberExpression

                var val = base.GetMemberValue(me);
                if (val == null)
                {
                    tempsql = (str == "=") ? " IS NULL" : " IS NOT NULL";
                    this.Write(tempsql);
                }
                else {
                    this.Write(str);
                    this.Write(" ");
                    this.Visit(node.Right);
                }
              
            }
            else
            {
                this.Write(str);
                this.Write(" ");

                //标识是右边的 成员过来的
                _memberIsRight = !m_isLinkBinary;



                //数组赋值时候 获取数组值
                if (node.Right.NodeType == ExpressionType.ArrayIndex)
                {
                    var value = this.GetArrayValue(node.Right as Expression);

                    string paramerName = this.GetParamerName();
                    this.Write(paramerName);

                    base.Result.Parameters.Insert(paramerName, value, ParameterDirection.Input);

                }
                else if (node.Right.NodeType == ExpressionType.ListInit)
                {
                    var value = this.GetArrayValue(node.Right as Expression);

                    string paramerName = this.GetParamerName();
                    this.Write(paramerName);

                    base.Result.Parameters.Insert(paramerName, value, ParameterDirection.Input);

                }
                else if (node.Right.NodeType == ExpressionType.Call)
                {

                    this.MethodCallExtend(node.Right as MethodCallExpression);

                }
                else
                {
                    this.Visit(node.Right);
                }

                
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

            Type type = node.Type;
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
                    type = node.Member.ReflectedType;
                    var ex = node.Expression;
                    string temp = ex==null? "":ex.ToString();
                    if (type != null && base.Result.QueryColletion.Contains(type.FullName) && temp.Contains("h__TransparentIdentifier"))
                    {
                        QueryParameter parameter = base.Result.QueryColletion[type.FullName];
                        this.Write(parameter.Alias);
                        this.Write(".");
                        this.Write("[");
                        this.Write(parameter.GetColumn(node.Member.Name).FieldMap);
                        this.Write("]");
                        return node;
                    }
                    else {

                        value = base.GetPropertyValue(node);
                      
                    }
                    
                }
                string paramerName = this.GetParamerName();
                this.Write(paramerName);
                base.Result.Parameters.Insert(paramerName, value, ParameterDirection.Input);



                // this.Visit(node.Expression);
                return node;
            }


            
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
                this.Write("[");
                this.Write(parameter.GetColumn(node.Member.Name).FieldMap);
                this.Write("]");
            }
            else {

               // Console.WriteLine(node.Type);
                var temptype = node.Type;

                object val;
                string paramerName = "";
                string n = node.Type.Name;
                if (node.Type.Name.Contains("Nullable")) {
                    n = "Nullable";
                }
                switch (n)
                {
                    case "Nullable":
                    case "String":
                    case "Datetime":
                    case "Decimal":
                    case "Double":
                    case "Int32":
                    case "Int":
                    case "Int64":

                        //var me =(MemberExpression) node.Expression;
                        //var constExpr = (ConstantExpression)me.Expression;
                        //var objReference = constExpr.Value;

                        val=  base.GetMemberValue(node);
                        if (val != null)
                        {
                            paramerName = this.GetParamerName();
                            this.Write(paramerName);
                            base.Result.Parameters.Insert(paramerName, val, ParameterDirection.Input);

                        }
                        else
                        {
                            this.Write("null");
                        }





                        // string name = mex.Member.Name;
                        //// mex.Member.Name.Dump();
                        // var fieldOnClosureExpression = mex.Expression as MemberExpression;


                        // var closureClassInstance = closureClassExpression.Value;

                        // // Find the field value, in this case it's a reference to the "s" variable
                        // var closureFieldInfo = fieldOnClosureExpression.Member as FieldInfo;
                        // var closureFieldValue = closureFieldInfo.GetValue(closureClassInstance);


                        // val = propertyInfo.GetValue(closureFieldValue, null);

                        // var cex = mex.Expression as ConstantExpression;
                        // var fld = mex.Member as FieldInfo;

                        //    var value = fld.GetValue(cex.Value);

                        //val = fld.GetValue(cex.Value);

                        //var propertyInfo = mex.Member as PropertyInfo;
                        //var propertyValue = propertyInfo.GetValue(val, null);

                        // text.DataBindings.Add("Text", x, name);
                        //var fieldInfo = ce.Value.GetType().GetField(me.Member.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        //var value = fieldInfo.GetValue(ce.Value);

                        //val = (node.Member as FieldInfo).GetValue((node.Expression as ConstantExpression).Value);

                        ////var mnode = node.Member. as ConstantExpression;
                        //if (node.Value == null)
                        //{
                        //    this.Write("null");
                        //    return node;
                        //}
                        ////  Type t = node.Value.GetType();



                        //  return node;

                        //扩展方法
                        break;

                    default:
                        if (temptype.IsValueType) {
                           val = (node.Member as FieldInfo).GetValue((node.Expression as ConstantExpression).Value);
                            if (val != null)
                            {
                                paramerName = this.GetParamerName();
                                this.Write(paramerName);
                                base.Result.Parameters.Insert(paramerName, val, ParameterDirection.Input);

                            }
                            else {
                                this.Write("null");
                            }


                        }
                        break;



                }

            }

            //不是字段的情况

          

          
            return node;
        }

        /// <summary>
        /// 处理方法,后缀方法处理
        /// </summary>
        /// <param name="node"></param>
        protected void MethodCallExtend(MethodCallExpression node)
        {
            string method = node.Method.Name;
            switch (method)
            {
                case "Trim":
                case "ToString":
                    Expression strValue = Expression.Call(node, typeof(string).GetMethod("Trim", Type.EmptyTypes));

                    if (strValue == null)
                    {
                        strValue = Expression.Call(node.Object, typeof(object).GetMethod("ToString"));
                    }

                    var result1 = Expression.Lambda(strValue).Compile().DynamicInvoke();

                    string paramerName = this.GetParamerName();
                    this.Write(paramerName);
                    base.Result.Parameters.Insert(paramerName, result1, ParameterDirection.Input);
                    break;
                default:
                    this.VisitMethodCall(node);
                    break;
            
            }
        
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

                    //扩展方法
              
     
                default:
                  return  base.VisitMethodCall(node);

                  

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
