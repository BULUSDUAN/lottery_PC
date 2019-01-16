
namespace KaSon.FrameWork.ORM.Visitor.MySql
{
    using System;
    using System.Linq.Expressions;

    internal class SelectManyVisitor : SelectManyLinqVisitor
    {
        protected override void VisitTable(Expression exp)
        {
            if (exp.Type.IsGenericType)
            {
                QueryParameter parameter = base.Result.QueryColletion[exp.Type.GetGenericArguments()[0].FullName];
                if (parameter.IsEntity)
                {
                  //  this.Write("[");
                    this.Write(EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name);
                   // this.Write("]");
                    this.Write(" AS ");
                }
                this.Write(parameter.Alias);
            }
        }
    }
}

