namespace KaSon.FrameWork.ORM.Visitor
{

    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;

    internal class JoinLinqVisitor : LinqVisitor
    {
        protected bool IsLeft = true;
        protected bool IsSelect;
         
        protected readonly List<string> LeftCondition = new List<string>();
        protected readonly List<string> RightCondition = new List<string>();
        protected StringBuilder SelectBuilder = new StringBuilder();

        /// <summary>
        ///  linq 表达式树 Visit 
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression == null)
            {
                throw new ArgumentNullException("exp");
            }
            base.Result = result;
            this.Visit(expression.Arguments[0]);
            this.Write(" Join ");
            this.Visit(expression.Arguments[1]);
            this.Write(" on ");
            this.Visit(expression.Arguments[2]);
            this.IsLeft = false;
            this.Visit(expression.Arguments[3]);
            for (int i = 0; i < this.LeftCondition.Count; i++)
            {
                if (i > 0)
                {
                    this.Write(" and ");
                }
                this.Write(this.LeftCondition[i]);
                this.Write(" = ");
                this.Write(this.RightCondition[i]);
            }

           // this.Result.SqlBuilder.ToString();
           // this.

            this.IsSelect = true;
            if (base.Result.IsSum)
            {
                this.SelectBuilder.Append(" SELECT SUM(");
                this.Visit(expression.Arguments[4]);
                this.SelectBuilder.Append(") FROM ");
                result.IsSum = false;
            }
            if (base.Result.IsMax)
            {
                this.SelectBuilder.Append(" SELECT MAX(");
                this.Visit(expression.Arguments[4]);
                this.SelectBuilder.Append(") FROM ");
                result.IsMax = false;
            }
            if (base.Result.IsMin)
            {
                this.SelectBuilder.Append(" SELECT MIN(");
                this.Visit(expression.Arguments[4]);
                this.SelectBuilder.Append(") FROM ");
                result.IsMin = false;
            }


           
            if (base.Result.IsAverage)
            {
                this.SelectBuilder.Append(" SELECT Average(");
                this.Visit(expression.Arguments[4]);
                this.SelectBuilder.Append(") FROM ");
                result.IsAverage = false;
             
            }
            else
            {
                this.SelectBuilder.Append("SELECT ");
                this.Visit(expression.Arguments[4]);
                this.SelectBuilder.Append(" FROM ");
              

            }
            base.Result.JoinSelect = this.SelectBuilder.ToString();
            this.IsSelect = true;
            base.Result.QueryColletion[exp.Type.GetGenericArguments()[0].FullName].HasAs = true;
            base.Result.HasSelect = false;

            //组装语句
         // string temp= base.Result.SqlBuilder.ToString();
         // base.Result.SqlBuilder.Clear();
         // this.Write(this.SelectBuilder.ToString());
         // this.Write(temp);
         



            return exp;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (this.IsSelect)
            {
                string str;
                switch (node.NodeType)
                {
                    case ExpressionType.Add:
                        str = "+";
                        break;

                    case ExpressionType.AddChecked:
                        str = "+";
                        break;

                    case ExpressionType.Divide:
                        str = "/";
                        break;

                    case ExpressionType.Equal:
                        str = "=";
                        break;

                    case ExpressionType.Modulo:
                        str = "%";
                        break;

                    case ExpressionType.MultiplyChecked:
                        str = "*";
                        break;

                    case ExpressionType.Subtract:
                        str = "-";
                        break;

                    case ExpressionType.SubtractChecked:
                        str = "-";
                        break;

                    default:
                        throw new InvalidOperationException();
                }
                this.SelectBuilder.Append("(");
                this.Visit(node.Left);
                this.SelectBuilder.Append(str);
                this.Visit(node.Right);
                this.SelectBuilder.Append(")");
            }
            return node;
        }

        /// <summary>
        ///  常数表达式 for Sql；如类名 {value(Kad.FrameWork.ORM2.Linq.DbQuery`1[Kad.Store.Model.StockChkEntity])}
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (this.IsSelect)
            {
                if (node.Type.IsValueType && (node.Value != null))
                {
                    this.SelectBuilder.Append(node.Value.ToString());
                }
                if ((node.Type == typeof(string)) && (node.Value != null))
                {
                    this.SelectBuilder.Append("'");
                    this.SelectBuilder.Append(node.Value.ToString());
                    this.SelectBuilder.Append("'");
                }
                return node;
            }
            if (node.Type.IsGenericType)
            {
                QueryParameter parameter = base.Result.QueryColletion[node.Type.GetGenericArguments()[0].FullName];
                if (parameter.IsEntity)
                {
                    this.Write(EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name);
                    this.Write(" AS ");
                }
                this.Write(parameter.Alias);
            }
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
            if (type != null)
            {
                QueryParameter parameter;
                if (this.IsSelect)
                {
                    if (base.Result.QueryColletion.Contains(type.FullName))
                    {
                        parameter = base.Result.QueryColletion[type.FullName];
                        this.SelectBuilder.Append(parameter.Alias);
                        this.SelectBuilder.Append(".");
                        this.SelectBuilder.Append(parameter.GetColumn(node.Member.Name).FieldMap);
                    }
                    return node;
                }
                if (!base.Result.QueryColletion.Contains(type.FullName))
                {
                    return node;
                }
                parameter = base.Result.QueryColletion[type.FullName];
                string item = parameter.Alias + "." + parameter.GetColumn(node.Member.Name).FieldMap;
                if (this.IsLeft)
                {
                    this.LeftCondition.Add(item);
                    return node;
                }
                this.RightCondition.Add(item);
            }
            return node;
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            var p = node.Expression.Type;

            if (this.IsSelect)
            {
                QueryParameter qparameter = null;
                if (!p.Name.Contains("f__AnonymousType") && base.Result.QueryColletion.ContainsEx(p.FullName, out qparameter))
                {

                    if (qparameter.IsEntity)
                    {

                        this.SelectBuilder.Append(base.BuildEntitySelectSql(qparameter)); 
                        return node;

                    }
                }

                this.Visit(node.Expression);
                this.SelectBuilder.Append(" AS ");
                this.SelectBuilder.Append(node.Member.Name);
            }
            return node;
        }

        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            if (this.IsSelect)
            {
                switch (node.BindingType)
                {
                    case MemberBindingType.Assignment:
                        return this.VisitMemberAssignment((MemberAssignment) node);

                    case MemberBindingType.MemberBinding:
                        return this.VisitMemberMemberBinding((MemberMemberBinding) node);

                    case MemberBindingType.ListBinding:
                        return this.VisitMemberListBinding((MemberListBinding) node);
                }
            }
            return node;
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            if (this.IsSelect)
            {
                

                for (int i = 0; i < node.Bindings.Count; i++)
                {
                    if (i > 0)
                    {
                        this.SelectBuilder.Append(",");
                    }
                    this.VisitMemberBinding(node.Bindings[i]);
                }
                this.Visit(node.NewExpression);
            }
            return node;
        }
       
        protected override Expression VisitNew(NewExpression node)
        {
            if (this.IsSelect)
            {
                if (node.Arguments.Count == 0)
                {
                    //不是实体类,不是匿名类
                    if (!base.Result.QueryColletion.Contains(node.Type.ToString()) && !node.Type.Name.Contains("f__AnonymousType"))
                    {
                        //this.SelectBuilder.Append(" AS ");
                        //this.SelectBuilder.Append(node.Members[i].Name);
                    }

                }

                for (int i = 0; i < node.Arguments.Count; i++)
                {
                    var b = node.Arguments[i];
                    if (i > 0)
                    {
                        this.SelectBuilder.Append(",");
                    }


                    //匿名类,属性是实体类
                    QueryParameter qparameter = null;
                    if (node.Type.Name.Contains("f__AnonymousType") && base.Result.QueryColletion.ContainsEx(b.Type.ToString(),out qparameter))
                    {

                        if (qparameter.IsEntity) {
                            ParameterExpression pnode = b as ParameterExpression;
                            QueryParameter parameter = base.Result.QueryColletion[b.Type.ToString()];
                          //  this.BuildEntitySelectSql(parameter);
                            this.SelectBuilder.Append(base.BuildEntitySelectSql(qparameter));
                            continue;
                        }
                       
                    }
                   
                    this.Visit(b);

                    this.SelectBuilder.Append(" AS ");
                    this.SelectBuilder.Append(node.Members[i].Name);



                }
                if (base.Result.ReturnType == node.Type)
                {
                    base.Result.Constructor = node.Constructor;
                }
                return node;
            }
            foreach (Expression expression in node.Arguments)
            {
                this.Visit(expression);
            }
            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (this.IsSelect)
            {
                if (!base.Result.QueryColletion.Contains(node.Type.FullName))
                {
                    return node;
                }
                QueryParameter parameter = base.Result.QueryColletion[node.Type.FullName];
                for (int i = 0; i < parameter.Columns.Count; i++)
                {
                    if (i > 0)
                    {
                        this.SelectBuilder.Append(", ");
                    }
                    this.SelectBuilder.Append(parameter.Alias);
                    this.SelectBuilder.Append(".");
                    this.SelectBuilder.Append(parameter.Columns[i].FieldMap);
                }
            }
            return node;
        }
    }
}

