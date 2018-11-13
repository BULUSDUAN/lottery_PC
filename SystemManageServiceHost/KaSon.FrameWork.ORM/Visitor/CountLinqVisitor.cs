namespace KaSon.FrameWork.ORM.Visitor
{
  
    using System.Linq.Expressions;

    internal class CountLinqVisitor : LinqVisitor
    {
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                base.Result = result;
                QueryParameter parameter = base.Result.QueryColletion.Contains(expression.Arguments[0].Type.FullName) ? base.Result.QueryColletion[expression.Arguments[0].Type.FullName] : base.Result.QueryColletion[expression.Arguments[0].Type.GetGenericArguments()[0].FullName];
                if (parameter.GroupBy == null)
                {
                    string method;
                    if (expression != null)
                    {
                        method = expression.Method.Name;
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
                        if (!(!parameter.IsEntity || parameter.HasAs))
                        {
                            this.Write(EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name);
                            this.Write(" AS ");
                            parameter.HasAs = true;
                        }
                        this.Write(parameter.Alias);
                    }
                    return exp;
                }
                this.Write(" Count(*) ");
            }
            return exp;
        }
    }
}

