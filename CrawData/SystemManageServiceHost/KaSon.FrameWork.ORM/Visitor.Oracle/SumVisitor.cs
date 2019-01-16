namespace KaSon.FrameWork.ORM.Visitor.Oracle
{
    using KaSon.FrameWork.ORM;
    using KaSon.FrameWork.ORM.Visitor;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    internal class SumVisitor : SumLinqVisitor
    {
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                base.Result = result;
                result.IsSum = true;
                QueryParameter parameter = base.Result.QueryColletion.Contains(expression.Arguments[0].Type.FullName) ? base.Result.QueryColletion[expression.Arguments[0].Type.FullName] : base.Result.QueryColletion[expression.Arguments[0].Type.GetGenericArguments()[0].FullName];
                if (parameter.GroupBy == null)
                {
                    string method;
                    if (expression.Arguments[0]!=null)
                    {method=expression.Method.Name;
                        if (expression.Arguments.Count > 1)
                        {
                            if (method != "Select")
                            {
                           
                                this.Write("SELECT SUM(");
                                this.Visit(expression.Arguments[1]);
                                this.Write(") FROM ");
                                base.Result.HasSelect = true;
                            }
                          
                        }
                        result.IsSum = true;
                        this.Visit(expression.Arguments[0]);
                        return exp;
                    }
                    if (expression.Arguments[0] is ConstantExpression)
                    {
                        if (!(base.Result.HasSelect || (expression.Arguments.Count <= 1)))
                        {
                            this.Write("SELECT SUM(");
                            this.Visit(expression.Arguments[1]);
                            this.Write(") FROM ");
                            base.Result.HasSelect = true;
                        }
                        if (!(!parameter.IsEntity || parameter.HasAs))
                        {
                           // this.Write("[");
                            this.Write(EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name);
                           // this.Write("]");
                            this.Write(" ");
                            parameter.HasAs = true;
                        }
                        this.Write(parameter.Alias);
                    }
                    return exp;
                }
                this.Write(" SUM(");
                this.Visit(expression.Arguments[1]);
                this.Write(") ");
            }
            return exp;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            QueryParameter parameter;
            if (base.Result.QueryColletion.Contains(node.Type.FullName))
            {
                parameter = base.Result.QueryColletion[node.Type.FullName];
                for (int i = 0; i < parameter.Columns.Count; i++)
                {
                    if (i > 0)
                    {
                        this.Write(", ");
                    }
                    this.Write(parameter.Alias);
                    this.Write(".");
                   // this.Write("[");
                    this.Write(parameter.Columns[i].FieldMap);
                  //  this.Write("]");
                }
                return node;
            }
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
                parameter = base.Result.QueryColletion[type.FullName];
                this.Write(parameter.Alias);
                this.Write(".");
              //  this.Write("[");
                this.Write(parameter.GetColumn(node.Member.Name).FieldMap);
              //  this.Write("]");
            }
            return node;
        }


        protected override Expression VisitConstant(ConstantExpression node)
        {
            var value = node.Value;

            var bol = value is IQueryable;
            if (bol)
            {
                Type type = node.Type.GetGenericArguments()[0];
                //  value.e

                var parameter = base.Result.QueryColletion[type.FullName];

                //   parameter = base.Result.QueryColletion[type.FullName];
              //  this.Write("[");
                //   this.Write(parameter.m);
                this.Write(EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name);
              //  this.Write("]");
                this.Write(" ");
                this.Write(parameter.Alias);



            }
            return node;
        }
    }
}

