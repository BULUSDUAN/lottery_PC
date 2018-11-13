namespace KaSon.FrameWork.ORM.Visitor.SQLServer
{

    using KaSon.FrameWork.ORM.Visitor;
    using KaSon.FrameWork.ORM;
   
    using System.Linq.Expressions;
    using System.Linq;
    using System;

    internal class CountVisitor : CountLinqVisitor
    {
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
         
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                string method = expression.Method.Name;
                base.Result = result;
                result.IsCount = true;
                QueryParameter parameter = null;
                if (base.Result.QueryColletion.Contains(expression.Arguments[0].Type.FullName))
                {
                    parameter = base.Result.QueryColletion[expression.Arguments[0].Type.FullName];
                }
                else if (base.Result.QueryColletion.Contains(expression.Arguments[0].Type.GetGenericArguments()[0].FullName))
                {
                    parameter = base.Result.QueryColletion[expression.Arguments[0].Type.GetGenericArguments()[0].FullName];
                }
                if ((parameter != null) && (parameter.GroupBy == null))
                {
                   
                    if (expression !=null)
                    {
                        if (method == "Select")
                        {
                            result.IsCount = true;
                        }
                        else
                        {
                            this.Write("SELECT COUNT(*) FROM ");
                            base.Result.HasSelect = true;
                        }

                       
                        this.Visit(expression.Arguments[0]);
                        return exp;
                    }
                    if (expression.Arguments[0] is ConstantExpression)
                    {
                        if (!base.Result.HasSelect)
                        {
                            this.Write("SELECT COUNT(*) FROM ");
                            base.Result.HasSelect = true;
                        }
                        if (!(!parameter.IsEntity || parameter.HasAs))
                        {
                            this.Write(EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name);
                            this.Write(" AS ");
                            parameter.HasAs = true;
                        }
                        this.Write("[");
                        this.Write(parameter.Alias);
                        this.Write("]");
                    }
                    return exp;
                }
                if (parameter == null)
                {
                    if (expression != null)
                    {
                        if (method == "Select")
                        {
                            result.IsCount = true;
                        }
                        else
                        {
                            this.Write("SELECT COUNT(*) FROM ");
                            base.Result.HasSelect = true;
                        }
                        this.Visit(expression.Arguments[0]);
                    }
                    return exp;
                }
                this.Write(" Count(*) ");
            }
            return exp;
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
                this.Write("[");
                //   this.Write(parameter.m);
                this.Write(EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name);
                this.Write("]");
                this.Write(" AS ");
                this.Write(parameter.Alias);



            }
            return node;
        }
    }
}

