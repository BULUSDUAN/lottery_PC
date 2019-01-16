namespace KaSon.FrameWork.ORM.Visitor
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class OrderByLinqVisitor : LinqVisitor
    {
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                base.Result = result;
                if (expression.Arguments[0] is ConstantExpression)
                {
                    QueryParameter parameter = base.Result.QueryColletion[expression.Arguments[0].Type.GetGenericArguments()[0].FullName];
                    if (parameter.IsEntity)
                    {
                        this.Write(EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name);
                        this.Write(" AS ");
                    }
                    this.Write(parameter.Alias);
                }
                else
                {
                    this.Visit(expression.Arguments[0]);
                }
                string method = expression.Method.Name;
                switch (method)
                {
                    case "OrderBy":
                    case "OrderByComparer":
                    case "OrderByDescending":
                    case "OrderByDescendingComparer":
                        this.WritePage(" ORDER BY ");
                        break;

                    case "ThenBy":
                    case "ThenByComparer":
                    case "ThenByDescending":
                    case "ThenByDescendingComparer":
                        this.WritePage(" , ");
                        break;

                    default:
                        throw new Exception("不是ORDER BY语句！");
                }
                this.Visit(expression.Arguments[1]);
                switch (method)
                {
                    case "OrderBy":
                    case "OrderByComparer":
                    case "ThenBy":
                    case "ThenByComparer":
                        this.WritePage(" ASC ");
                        break;

                    case "OrderByDescending":
                    case "OrderByDescendingComparer":
                    case "ThenByDescending":
                    case "ThenByDescendingComparer":
                        this.WritePage(" DESC ");
                        break;

                    default:
                        throw new Exception("不是ORDER BY语句！");
                }
            }
            base.Result.HasSelect = false;
            return exp;
        }

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

             

                if (parameter.GroupBy == null)
                {
                    this.WritePage(parameter.Alias);
                    this.WritePage(".");
                    this.WritePage(parameter.GetColumn(node.Member.Name).FieldMap);
                    return node;
                }
                if (node.Member.Name == "Key")
                {
                    int num = 0;
                    foreach (KeyValuePair<string, GroupByParamKey> pair in parameter.GroupBy.KeyList)
                    {
                        if (num > 0)
                        {
                            this.Write(", ");
                        }
                        this.WritePage(base.Result.QueryColletion[pair.Value.FullName].Alias);
                        this.WritePage(".");
                        this.WritePage(base.Result.QueryColletion[pair.Value.FullName].GetColumn(pair.Value.MeberName).FieldMap);
                        num++;
                    }
                    return node;
                }
                this.WritePage(base.Result.QueryColletion[parameter.GroupBy.KeyList[node.Member.Name].FullName].Alias);
                this.WritePage(".");
                this.WritePage(base.Result.QueryColletion[parameter.GroupBy.KeyList[node.Member.Name].FullName].GetColumn(parameter.GroupBy.KeyList[node.Member.Name].MeberName).FieldMap);
           
            
            }
            return node;
        }

        protected virtual void WritePage(string s)
        {
            if (!base.Result.IsGroup)
            {
               
                base.Write(s);
            }
           
           
               
            
            
        }
       
    }
}

