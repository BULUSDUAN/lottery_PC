using KaSon.FrameWork.Services.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KaSon.FrameWork.ORM.Visitor.Oracle
{

    internal class ContainsVisitor : Visitor.ContainsLinqVisitor
    {
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
              //  this.Write("[");
                this.Write(parameter.GetColumn(node.Member.Name).FieldMap);
               // this.Write("]");
            }
            return node;
        }
    }
}
