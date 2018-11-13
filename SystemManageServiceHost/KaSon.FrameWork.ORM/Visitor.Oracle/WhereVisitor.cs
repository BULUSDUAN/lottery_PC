﻿using KaSon.FrameWork.Services.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace KaSon.FrameWork.ORM.Visitor.Oracle
{

    internal class WhereVisitor : Visitor.WhereLinqVisitor
    {

        protected override string GetParamerName()
        {
            return (":P" + base.Result.ParamIndex);
        }
         public override Expression Visit(Expression exp, QueryProviderContext result)
        {
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
                  //  this.Write("[");
                    string name = EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name;
                    this.Write(name);
                    this.Write(" ");
                    this.WriteWhere(name);

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
               // this.Write("(");
            }
        Label_0137:
            this.Visit(expression.Arguments[0]);
            if (!parameter.HasAs)
            {
               // this.Write(")");
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
              //  this.Write(" AS ");
                this.Write(parameter.Alias);

                //部件where

             //   this.WriteWhere(" AS ");
                this.WriteWhere(parameter.Alias);


                parameter.HasAs = true;
            }
            QueryParameter parameter2 = base.Result.QueryColletion[expression.Type.GetGenericArguments()[0].FullName];
            if (parameter.GroupBy == null)
            {
              //  base.Result. = true;


                string _temp = parameter2.HasWhere ? " And " : " WHERE ";

                this.Write(_temp);
              
            }
            else
            {
                base.Result.IsGroup = true;

                string _temp = parameter2.HasWhere ? " And " : " Having ";

                this.Write(_temp);
                this.WriteWhere(_temp);
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
           // this.Write("(");
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
           // this.Write(")");
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


                this.WriteWhere("null");

                return node;
            }
          //  Type t = node.Value.GetType();
            string paramerName = this.GetParamerName();
            this.Write(paramerName);

            this.WriteWhere(paramerName);

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
            if (node.Expression is ParameterExpression)
            {
                type = (node.Expression as ParameterExpression).Type;
            }
            if (node.Expression is MemberExpression)
            {
                type = (node.Expression as MemberExpression).Type;
            }
            if (node.Expression is ConstantExpression)
            {
              //  var fieldInfo = (FieldInfo)node.Member;

              //  var instance = (node.Expression == null) ? null : TryEvaluate(node.Expression).Value;

             //   var obj = fieldInfo.GetValue(instance);
                object value = null;
                if (node.Member.MemberType == MemberTypes.Field)
                {
                    value = GetFieldValue(node);
                }
                else if (node.Member.MemberType == MemberTypes.Property)
                {
                    value = GetPropertyValue(node);
                }

                string paramerName = this.GetParamerName();


                this.Write(paramerName);
                this.WriteWhere(paramerName);


                base.Result.Parameters.Insert(paramerName, value, ParameterDirection.Input);
                return node;
               // this.Visit(node.Expression);
            }
            if ((type != null) && base.Result.QueryColletion.Contains(type.FullName))
            {
                QueryParameter parameter = base.Result.QueryColletion[type.FullName];
                this.Write(parameter.Alias);
                this.Write(".");

                this.WriteWhere(parameter.Alias);
                this.WriteWhere(".");
              

              //  this.Write("[");
                this.Write(parameter.GetColumn(node.Member.Name).FieldMap);
               // this.Write("]");
            }
            return node;
        }
        private static object GetFieldValue(MemberExpression node)
        {
            var fieldInfo = (FieldInfo)node.Member;

            var instance = (node.Expression == null) ? null : TryEvaluate(node.Expression).Value;

            return fieldInfo.GetValue(instance);
        }

        private static object GetPropertyValue(MemberExpression node)
        {
            var propertyInfo = (PropertyInfo)node.Member;

            var instance = (node.Expression == null) ? null : TryEvaluate(node.Expression).Value;

            return propertyInfo.GetValue(instance, null);
        }

        private static ConstantExpression TryEvaluate(Expression expression)
        {

            if (expression.NodeType == ExpressionType.Constant)
            {
                return (ConstantExpression)expression;
            }
            throw new NotSupportedException();

        }

    
   
    }
}
