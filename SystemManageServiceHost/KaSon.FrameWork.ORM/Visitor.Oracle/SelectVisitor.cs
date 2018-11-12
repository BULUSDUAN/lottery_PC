using KaSon.FrameWork.Services.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KaSon.FrameWork.ORM.Visitor.Oracle
{

    internal class SelectVisitor : Visitor.SelectLinqVisitor
    {
        /// <summary>
        /// 成员参数
        /// </summary>
        private bool IsMemberAssignment = false;
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            base.Result = result;
            if (expression != null )
            {
            
                if (!(base.Result.ReturnType.Equals(exp.Type.GetGenericArguments()[0]) || base.Result.IsExecute))
                {
                    this.Write(" (");
                }
                QueryParameter parameter = base.Result.QueryColletion[expression.Arguments[0].Type.GetGenericArguments()[0].FullName];
                if (result.IsCount)
                {
                    this.Write(" Select Count(*) From ");
                    result.IsCount = false;
                    result.HasSelect = true;
                }
                else if (result.IsSum)
                {
                   // this.Write(" Select Sum( ");
                  //  this.Visit(expression.Arguments[1]);

                  //  MemberAssignmentSub(this.Result.SqlBuilder);

                  //  this.Write(") From ");
                    result.HasSelect = true;
                    result.IsSum = false;
                }
                else if (result.IsMax)
                {
               //     this.Write(" Select Max( ");
                //    this.Visit(expression.Arguments[1]);

                //    MemberAssignmentSub(this.Result.SqlBuilder);

                //    this.Write(") From ");
                    result.HasSelect = true;
                    result.IsMax = false;
                }
                else if (result.IsMin)
                {
                  //  this.Write(" Select Min( ");
                   // this.Visit(expression.Arguments[1]);

                  //  MemberAssignmentSub(this.Result.SqlBuilder);

                 //   this.Write(") From ");
                    result.HasSelect = true;
                    result.IsMin = false;
                }
                else if (result.IsAverage)
                {
                    this.Write(" Select Average( ");
                    this.Visit(expression.Arguments[1]);
                    MemberAssignmentSub(this.Result.SqlBuilder);

                    this.Write(") From ");
                    result.HasSelect = true;
                    result.IsAverage = false;
                }
                else
                {

                    

                    this.Write(" Select ");
                    this.Visit(expression.Arguments[1]);
                    MemberAssignmentSub(this.Result.SqlBuilder);
                    
                    this.Write(" From ");
                }


                if (expression.Arguments[0] is ConstantExpression)
                {
                    if (!(!parameter.IsEntity || parameter.HasAs))
                    {
                       // this.Write("[");
                        this.Write(EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name);
                        //this.Write("]");
                       this.Write(" ");
                    }
                    this.Write(parameter.Alias);
                }
                else
                {
                    this.Visit(expression.Arguments[0]);
                }
                if (!(base.Result.ReturnType.Equals(exp.Type.GetGenericArguments()[0]) || base.Result.IsExecute))
                {
                    this.Write(")  ");
                   // this.Write("[");
                    this.Write(base.Result.QueryColletion[exp.Type.GetGenericArguments()[0].FullName].Alias);
                 
                    return exp;
                }
                base.Result.HasSelect = true;
            }
            else if (expression.Arguments[0].Type.GetGenericArguments()[0].FullName.Contains("IGrouping`2"))
            {
                Type group = expression.Arguments[0].Type.GetGenericArguments()[0];

                if (group.IsGenericType)
                {
                    result.IsGroup = true;
                    Type[] typeArr = group.GetGenericArguments();
                    base.Result.GroupKeyType = typeArr[0];
                    base.Result.GroupType = typeArr[1];

                }
                this.Write(" Select ");
                this.Visit(expression.Arguments[1]);
                this.Write(" From ");

                result.HasSelect = true;
            }
            return exp;
        }

        private void MemberAssignmentSub(StringBuilder sb) {

            if (IsMemberAssignment)
            {
                //成员参数去掉 逗号
                string sql = sb.ToString();
                sql.Substring(sql.Length - 1);
                if (sql.Contains(","))
                {
                    base.Result.SqlBuilder.Clear();
                    this.Write(sql.Remove(sql.Length - 1));

                }

            }
          
        }

        /// <summary>
        /// 参数标的是 如{b}
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
          //  Type tp = node.Type;
            if (base.Result.QueryColletion.Contains(node.Type.FullName))
            {

                //string name =node.Type.FullName;
                //bool isgroup = false;
                //if (node.Type.Name.Substring(0, 6).Equals("IGroup"))
                //{
                //   // this.Result.IsGroup = true;
                //    isgroup = true;
                //    name=node.Type.GetGenericArguments()[1].Name;
                //}

                QueryParameter parameter = base.Result.QueryColletion[node.Type.FullName];
                string _temp = "";
                for (int i = 0; i < parameter.Columns.Count; i++)
                {
                    if (i > 0)
                    {
                        this.Write(", ");

                        _temp += ", ";
                    }
                    this.Write(parameter.Alias);
                    this.Write(".");

                    //if (isgroup)
                    //{
                    //    this.Write("[");


                    //    this.Write("]");
                    //    this.Write(" AS ");

                    //}

                  //  this.Write("[");
                    this.Write(parameter.Columns[i].FieldMap);
                  //  this.Write("]");

                 

                    _temp = _temp + parameter.Alias
                    + "." + parameter.Columns[i].FieldMap + " ";

                }
                this.Result.SelectText = _temp;
               
            }
            //else if (base.Result.QueryColletion.Contains(node.Type.FullName) && node.Type.FullName.Contains("IGrouping`2"))
            //{
                   
            //       // QueryParameter parameter = base.Result.QueryColletion[node.Type.FullName];
            //        this.Write("Gp");
            //        this.Write(".");
            //        this.Write("[");
            //        this.Write("_GroupKey");
            //        this.Write("]");

              
            //}
            return node;
        }

        /// <summary>
        /// New 表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitNew(NewExpression node)
        {
            string _temp = "";
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                if (i > 0)
                {
                    this.Write(",");
                    _temp += ", ";
                }
                this.Visit(node.Arguments[i]);
                this.Write(" AS ");
              //  this.Write("[");
                this.Write(node.Members[i].Name);
              //  this.Write("]");

                _temp = _temp + node.Arguments[i] + " AS " + node.Members[i].Name
                 + " ";
            }
            this.Result.SelectText = _temp;
            if (base.Result.ReturnType.Equals(node.Type))
            {
                base.Result.Constructor = node.Constructor;
            }
            return node;
        }

        /// <summary>
        /// 节点描述:Lambda b => b"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return this.Visit(node.Body);
        }
        /// <summary>
        /// 来表达所有的一元运算，形式为op(operand)。如!a，b++ b => b等node.NodeType Quote  node.ToString()  "b => b"
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
           // Console.WriteLine("节点描述:"+node.Operand.NodeType +" "+node.ToString());
            this.Visit(node.Operand);

            return node;
        }


        protected override Expression VisitMember(MemberExpression node)
        {
            int num;
            QueryParameter parameter2;
            if (base.Result.QueryColletion.Contains(node.Type.FullName))
            {
                if (node.Expression.Type.Name.Equals("IGrouping`2"))
                {
                    QueryParameter parameter = base.Result.QueryColletion[node.Expression.Type.FullName];
                    num = 0;
                    foreach (KeyValuePair<string, GroupByParamKey> pair in parameter.GroupBy.KeyList)
                    {
                        if (num > 0)
                        {
                            this.Write(", ");
                        }
                        this.Write(base.Result.QueryColletion[pair.Value.FullName].Alias);
                        this.Write(".");
                        this.Write(base.Result.QueryColletion[pair.Value.FullName].GetColumn(pair.Value.MeberName).FieldMap);
                        num++;
                    }
                    return node;
                }
                parameter2 = base.Result.QueryColletion[node.Type.FullName];
                for (int i = 0; i < parameter2.Columns.Count; i++)
                {
                    if (i > 0)
                    {
                        this.Write(", ");
                    }
                    this.Write(parameter2.Alias);
                    this.Write(".");
                   // this.Write("[");
                    this.Write(parameter2.Columns[i].FieldMap);
                   // this.Write("]");
                }
                return node;
            }
            Type type = null;
            bool flag = false;
            if (node.Expression is ParameterExpression)
            {
                type = (node.Expression as ParameterExpression).Type;
            }
            if (node.Expression is MemberExpression)
            {
                if ((node.Expression as MemberExpression).Expression.Type.Name.Equals("IGrouping`2"))
                {
                    type = (node.Expression as MemberExpression).Expression.Type;
                    flag = true;
                }
                else
                {
                    type = (node.Expression as MemberExpression).Type;
                }
            }
            if ((type != null) && base.Result.QueryColletion.Contains(type.FullName))
            {
                parameter2 = base.Result.QueryColletion[type.FullName];
                if (parameter2.GroupBy == null)
                {
                    if (parameter2.IsGroupKey)
                    {
                        this.Write(parameter2.GetColumn(node.Member.Name).FieldMap);
                    }
                    else
                    {
                        this.Write(parameter2.Alias);
                        this.Write(".");
                      //  this.Write("[");
                        this.Write(parameter2.GetColumn(node.Member.Name).FieldMap);
                       // this.Write("]");
                    }
                    return node;
                }
                if ((node.Member.Name == "Key") && !flag)
                {
                    num = 0;
                    foreach (KeyValuePair<string, GroupByParamKey> pair in parameter2.GroupBy.KeyList)
                    {
                        if (num > 0)
                        {
                            this.Write(", ");
                        }
                        this.Write(base.Result.QueryColletion[pair.Value.FullName].Alias);
                        this.Write(".");
                       // this.Write("[");
                        this.Write(base.Result.QueryColletion[pair.Value.FullName].GetColumn(pair.Value.MeberName).FieldMap);
                       // this.Write("]");
                        num++;
                    }
                    return node;
                }
                this.Write(base.Result.QueryColletion[parameter2.GroupBy.KeyList[node.Member.Name].FullName].Alias);
                this.Write(".");
               // this.Write("[");
                this.Write(base.Result.QueryColletion[parameter2.GroupBy.KeyList[node.Member.Name].FullName].GetColumn(parameter2.GroupBy.KeyList[node.Member.Name].MeberName).FieldMap);
              //  this.Write("]");
            }
            return node;
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            IsMemberAssignment = true;
            this.Visit(node.Expression);
            this.Write(" AS ");
           // this.Write("[");
            this.Write(node.Member.Name);
            this.Write(" ,");
            return node;
        }

        protected virtual void Out(string s)
        {
            base.Result.SqlBuilder.Append(s);
        }

    }
}
