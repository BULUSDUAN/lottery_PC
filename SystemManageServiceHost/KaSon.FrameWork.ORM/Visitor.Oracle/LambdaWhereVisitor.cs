namespace KaSon.FrameWork.ORM.Visitor.Oracle
{

    using KaSon.FrameWork.Services.Attribute;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

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
              //  this.Out("[");
                this.Out(((FieldAttribute)obj2).Name);
               // this.Out("]");
                return node;
            }
            return node;
        }
    }
}

