using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace KaSon.FrameWork.ORM.Visitor
{
  internal  class SelectLinqVisitor:LinqVisitor
    {
        protected MemberAssignment SelNodeExp = null;
        protected ConstructorInfo FieldConstructorInfo = null;
        protected bool SelRightIsClass = false;
        public override Expression Visit(Expression exp, QueryProviderContext result)
      {
         
          
          return exp;
      }
        protected override Expression VisitNew(NewExpression node)
        {
            string _temp = "";
            
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                var b = node.Arguments[i];
                if (i > 0)
                {
                    this.Write(",");
                    _temp += ", ";
                }
                //匿名类,属性是实体类
                QueryParameter qparameter = null;
                if (node.Type.Name.Contains("f__AnonymousType") && base.Result.QueryColletion.ContainsEx(b.Type.ToString(), out qparameter))
                {

                    if (qparameter.IsEntity)
                    {
                        ParameterExpression pnode = b as ParameterExpression;
                        QueryParameter parameter = base.Result.QueryColletion[b.Type.ToString()];
                        //  this.BuildEntitySelectSql(parameter);
                        this.Write(base.BuildEntitySelectSql(qparameter));
                        continue;
                    }

                }

                this.Visit(node.Arguments[i]);
                this.Write(" AS ");
               // this.Write("[");
                this.Write(node.Members[i].Name);
                //this.Write("]");
                //this.SqlSelectFields(node.Members[i].Name);
                _temp = _temp + node.Arguments[i] + " AS " + node.Members[i].Name;
              //   + "]";
            }
            this.Result.SelectText = _temp;

            if (this.SelRightIsClass) {
                this.FieldConstructorInfo = node.Constructor;
                return node;
            }

            if (base.Result.ReturnType.Equals(node.Type))
            {
                base.Result.Constructor = node.Constructor;
            }
            return node;
        }
    }
}
