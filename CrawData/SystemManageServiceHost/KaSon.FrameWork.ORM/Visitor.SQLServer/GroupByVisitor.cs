namespace KaSon.FrameWork.ORM.Visitor.SQLServer
{
    using System;
    using System.Linq.Expressions;

    internal class GroupByVisitor : GroupByLinqVisitor
    {
        public override Expression Visit(MethodCallExpression node, QueryProviderContext result)
        {
            base.Result = result;
            base.Result.IsGroup = true;
            if (node.Arguments[0] is ConstantExpression)
            {
                EntityInfo entityInfo = EntityHelper.GetEntityInfo(node.Arguments[0].Type.GetGenericArguments()[0]);
                if (entityInfo.IsEntity)
                {
                    this.Write("[");
                    this.Write(entityInfo.Entity.Name);
                    this.Write("]");
                    this.Write(" AS ");
                }
                this.Write(base.Result.QueryColletion[node.Arguments[0].Type.GetGenericArguments()[0].FullName].Alias);
            }
            this.Visit(node.Arguments[0]);
            this.Write(" Group by ");

            this.Visit(node.Arguments[1]);
          //  result.GroupModelAlas

            base.Result.QueryColletion[node.Type.GetGenericArguments()[0].FullName].HasAs = true;
            return node;
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
                this.Write(parameter.Alias);
                this.Write(".");
                this.Write("[");
                this.Write(parameter.GetColumn(node.Member.Name).FieldMap);
                ///Ìæ»»_GroupKey
                ///
                base.Result.GroupKeyName = parameter.GetColumn(node.Member.Name).FieldMap;
              //  string temp=this.Result.SqlBuilder.ToString();
                this.Result.GroupType = type;
                this.Result.GroupKeyType = node.Type;
                this.Result.GroupModelAlas = parameter.Alias;

               // temp = temp.Replace("_GroupKey", parameter.GetColumn(node.Member.Name).FieldMap).Replace("Gp", parameter.Alias);

                // this.Result.SqlBuilder.Clear();

                // this.Write(temp);

                this.Write("]");
            }
            return node;
        }
    }
}

