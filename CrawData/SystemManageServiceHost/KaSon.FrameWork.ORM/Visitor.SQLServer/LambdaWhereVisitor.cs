namespace KaSon.FrameWork.ORM.Visitor.SQLServer
{

    using KaSon.FrameWork.Services.Attribute;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class LambdaWhereVisitor : LambdaLinqWhereVisitor
    {
        protected override Expression VisitMember(MemberExpression node)
        {
            this.Visit(node.Expression);
            object[] customAttributes = node.Member.GetCustomAttributes(typeof(FieldAttribute), true);
            int index = 0;
            while (index < customAttributes.Length)
            {
                object obj2 = customAttributes[index];
                this.Out("[");
                this.Out(((FieldAttribute)obj2).Name);
                this.Out("]");
                return node;
            }

            if (node.Expression is ConstantExpression)
            {
                //  var fieldInfo = (FieldInfo)node.Member;

                //  var instance = (node.Expression == null) ? null : TryEvaluate(node.Expression).Value;

                //   var obj = fieldInfo.GetValue(instance);
                //object value = null;
                //if (node.Member.MemberType == MemberTypes.Field)
                //{
                //    value = base.GetFieldValue(node);
                //}
                //else if (node.Member.MemberType == MemberTypes.Property)
                //{
                //    value = base.GetPropertyValue(node);
                //}

                //string paramerName = this.GetParamerName();
                //this.Write(paramerName);
                //base.Result.Parameters.Insert(paramerName, value, ParameterDirection.Input);
                //return node;
                // this.Visit(node.Expression);
            }


            return node;
        }
    }
}

