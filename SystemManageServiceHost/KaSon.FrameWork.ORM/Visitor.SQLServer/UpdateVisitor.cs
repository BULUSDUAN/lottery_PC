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

    internal class UpdateVisitor : UpdateLinqVisitor
    {
        protected override void Init()
        {
            base._builde.Append("UPDATE ");
            base._builde.Append('[');
            base._builde.Append(base.Context.Entity.Entity.Name);
            base._builde.Append(']');
            base._builde.Append(" SET ");
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            this.Visit(node.Expression);
            object[] customAttributes = node.Member.GetCustomAttributes(typeof(FieldAttribute), true);
            int index = 0;
            while (index < customAttributes.Length)
            {
                object obj2 = customAttributes[index];
                base._builde.Append('[');
                base._builde.Append(((FieldAttribute)obj2).Name);
                base._builde.Append(']');
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
                        base._builde.Append(',');
                    }
                    base._builde.Append('[');
                    base._builde.Append(((FieldAttribute)obj2).Name);
                    base._builde.Append("] = ");
                    this.Visit(assignment.Expression);
                    num++;
                }
            }
            return node;
        }
    }
}
