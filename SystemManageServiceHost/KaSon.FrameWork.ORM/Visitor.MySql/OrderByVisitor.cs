namespace KaSon.FrameWork.ORM.Visitor.MySql
{
    
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class OrderByVisitor : OrderByLinqVisitor
    {
        private OrderKeyMap _keymap = new OrderKeyMap();
        public override Expression Visit(Expression exp, QueryProviderContext result)
        {
            MethodCallExpression expression = exp as MethodCallExpression;
            if (expression != null)
            {
                
                string method = expression.Method.Name;
                base.Result = result;

                base.Result.IsOrder = true;

                    if (expression.Arguments[0] is ConstantExpression)
                    {
                        QueryParameter parameter = base.Result.QueryColletion[expression.Arguments[0].Type.GetGenericArguments()[0].FullName];
                        if (parameter.IsEntity)
                        {
                           // this.Write("[");
                            this.Write(EntityHelper.GetEntityInfo(parameter.ParameterType).Entity.Name);
                          //  this.Write("]");
                            this.Write(" AS ");
                        }
                        this.Write(parameter.Alias);
                    }
                    else
                    {
                        this.Visit(expression.Arguments[0]);
                    }
                    //switch (method)
                    //{
                    //    case "OrderBy":
                    //    case "OrderByComparer":
                    //    case "OrderByDescending":
                    //    case "OrderByDescendingComparer":
                    //      //  this.WritePage(" ORDER BY ");

                    //        break;

                    //    case "ThenBy":
                    //    case "ThenByComparer":
                    //    case "ThenByDescending":
                    //    case "ThenByDescendingComparer":
                    //      //  this.WritePage(" , ");
                    //        break;

                    //    default:
                    //        throw new Exception("不是ORDER BY语句！");
                    //}
                    this.Visit(expression.Arguments[1]);
                    switch (method)
                    {
                        case "OrderBy":
                        case "OrderByComparer":
                        case "ThenBy":
                        case "ThenByComparer":
                            //  this.WritePage(" ASC ");
                            _keymap.isAsc = true;
                            base.Result.IsOrderAsc = true;
                            base.Result.OrderKeyList.Add(_keymap);
                            break;

                        case "OrderByDescending":
                        case "OrderByDescendingComparer":
                        case "ThenByDescending":
                        case "ThenByDescendingComparer":
                            //  this.WritePage(" DESC ");
                            _keymap.isAsc = false;
                            base.Result.OrderKeyList.Add(_keymap);
                            base.Result.IsOrderAsc = false;
                            break;

                        default:
                            throw new Exception("不是ORDER BY语句！");
                    }
                


            
            }
            base.Result.HasSelect = false;



    //   string name=((MethodCallExpression)((LambdaExpression)((UnaryExpression)expression.Arguments[1]).Operand).Body).Method.Name;



     //   var p= ((UnaryExpression)expression.Arguments[1]).Operand);

            //随机排序
            var nexp=expression.Arguments[1];
            if (expression.Arguments[1] is UnaryExpression)
            {
            

               var body =((UnaryExpression)expression.Arguments[1]).Operand;


               if (body is LambdaExpression)
               {

                   var lab =((LambdaExpression)((UnaryExpression)expression.Arguments[1]).Operand).Body;

                   if (lab is MethodCallExpression)
                   {
                       string name = ((MethodCallExpression)((LambdaExpression)((UnaryExpression)expression.Arguments[1]).Operand).Body).Method.Name;

                       switch (name)
                       {
                           case "NewGuid":
                               _keymap.Key = "RAND() ";

                               break;

                           default:
                               break;
                       }

                   }
               }

               //if (nexp.Operand is LambdaExpression)
               //{ 
               
               
               
               //}

            }
           



            return exp;
        }


    
        //}

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
                base.Result.OrderModelAlas = parameter.Alias;
                _keymap.ModelAlis = parameter.Alias;
                _keymap.ModelType = parameter.ParameterType;
                _keymap.Key = parameter.GetColumn(node.Member.Name).FieldMap;
                //if (parameter.GroupBy == null)
                //{
                //    this.WritePage(parameter.Alias);
                //    this.WritePage(".");
                //    this.WritePage("[");
                //    this.WritePage(parameter.GetColumn(node.Member.Name).FieldMap);
                //    this.WritePage("]");
                //    return node;
                //}
                //if (node.Member.Name == "Key")
                //{
                //    int num = 0;
                //    foreach (KeyValuePair<string, GroupByParamKey> pair in parameter.GroupBy.KeyList)
                //    {
                //        if (num > 0)
                //        {
                //            this.Write(", ");
                //        }
                //        this.WritePage(base.Result.QueryColletion[pair.Value.FullName].Alias);
                //        this.WritePage(".");
                //        this.WritePage("[");
                //        this.WritePage(base.Result.QueryColletion[pair.Value.FullName].GetColumn(pair.Value.MeberName).FieldMap);
                //        this.WritePage("]");
                //        num++;
                //    }
                //    return node;
                //}
                //this.WritePage(base.Result.QueryColletion[parameter.GroupBy.KeyList[node.Member.Name].FullName].Alias);
                //this.WritePage(".[");
                //this.WritePage(base.Result.QueryColletion[parameter.GroupBy.KeyList[node.Member.Name].FullName].GetColumn(parameter.GroupBy.KeyList[node.Member.Name].MeberName).FieldMap);
                //this.WritePage("]");


            }
            return node;
        }
    }
}

