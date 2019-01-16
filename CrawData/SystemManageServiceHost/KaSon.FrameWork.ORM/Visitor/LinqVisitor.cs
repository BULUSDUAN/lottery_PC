namespace KaSon.FrameWork.ORM.Visitor
{
    using KaSon.FrameWork.ORM.Mdel;
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class LinqVisitor : ExpressionVisitor
    {
        protected QueryProviderContext Result;

        protected virtual string GetParamerName()
        {
            return ("@P" +Result.ParamIndex);
        }

        /// <summary>
        /// 获取成员值
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual object GetFieldValue(MemberExpression node)
        {
            var fieldInfo = (FieldInfo)node.Member;

            var instance = (node.Expression == null) ? null : TryEvaluate(node.Expression).Value;

            return fieldInfo.GetValue(instance);
        }

        /// <summary>
        ///  获取Lamdba 右边成员表达式值
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public virtual object GetValue(MemberExpression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));

            var getterLambda = Expression.Lambda<Func<object>>(objectMember);

            var getter = getterLambda.Compile();

            return getter();
        }
        // base.Result.SqlBuilder.Append(s);
        /// <summary>
        /// 构建sql 实体
        /// </summary>
        public virtual string BuildEntitySelectSql(QueryParameter parameter)
        {

            string buildstr = "";
            for (int j = 0; j < parameter.Columns.Count; j++)
            {
                if (j > 0)
                {
                    buildstr= buildstr+(", ");
                }
                buildstr = buildstr + (parameter.Alias);
                buildstr = buildstr + (".");
                buildstr = buildstr + (parameter.Columns[j].FieldMap);
                buildstr = buildstr + (" AS ");
                buildstr = buildstr + (parameter.Alias);
                buildstr = buildstr + ("_");
                buildstr = buildstr + (parameter.Columns[j].FieldMap);
                // this.SelectBuilder.Append(", ");
                //T1.id as T1_id
            }
            return buildstr;
        }
        /// <summary>
        /// 是否添加逗号
        /// </summary>
        public virtual void IsAddDouChar()
        {
            if (this.Result.SqlBuilder.ToString().ToLower().Contains("select") && this.Result.SqlBuilder.ToString().ToLower().Contains(",")) {

                
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public virtual object GetArrayValue(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.ArrayIndex:

                    BinaryExpression be = (BinaryExpression)expr;
                    Array arr = (Array)GetArrayValue(be.Left);
                    int index = (int)GetArrayValue(be.Right);
                   // Console.WriteLine("Index is: " + index);
                    return arr.GetValue(index);
                case ExpressionType.MemberAccess:
                    MemberExpression me = (MemberExpression)expr;
                    switch (me.Member.MemberType)
                    {
                        case MemberTypes.Property:
                            return ((PropertyInfo)me.Member).GetValue(GetArrayValue(me.Expression), null);
                        case MemberTypes.Field:
                            return ((FieldInfo)me.Member).GetValue(GetArrayValue(me.Expression));
                        default:
                            throw new NotSupportedException();
                    }
                case ExpressionType.Constant:
                    return ((ConstantExpression)expr).Value;
                default:
                    throw new NotSupportedException();

            }
        }

        public virtual object GetMemberValue(MemberExpression node)
        {
            object val=null;
          
            if (node.Member is FieldInfo)
            {
                var fildInfo = node.Member as FieldInfo;
                val = fildInfo.GetValue((node.Expression as ConstantExpression).Value);

                return val;
            }
            else if (node.Member is PropertyInfo)
            {


                var propertyInfo = node.Member as PropertyInfo;
                var mex = node.Expression as MemberExpression;
                var objExp = mex.Expression as ConstantExpression;
                var fld = mex.Member as FieldInfo;
                var value = fld.GetValue(objExp.Value);
                val = propertyInfo.GetValue(value, null);
                return val;
            }
            return "";

        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual object GetPropertyValue(MemberExpression node)
        {
            var propertyInfo = (PropertyInfo)node.Member;

             object instance=null;

            if (node.Expression != null)
            {
                if (node.NodeType == ExpressionType.Constant)
                {
                    instance = TryEvaluate(node.Expression).Value;
                }
                else if (node.NodeType ==ExpressionType.MemberAccess)
                {
                  
                   
                  return  instance= this.GetValue(node);
                }
            }
           



            return propertyInfo.GetValue(instance, null);
        }

        public virtual ConstantExpression TryEvaluate(Expression expression)
        {

            if (expression.NodeType == ExpressionType.Constant)
            {
                return (ConstantExpression)expression;
            }
            throw new NotSupportedException();

        }
        public virtual Expression Visit(Expression exp, QueryProviderContext result)
        {
            this.Result = result;
            return this.Visit(exp);
        }

        /// <summary>
        ///  Lambba 类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            this.Visit(node.Body);
            return node;
        }

        /// <summary>
        /// linq 方法类型 Visit
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            string method;
            if (node != null)
            {
                method = node.Method.Name;
             //   Console.WriteLine(method);
                switch (method)
                {
                    case "Where":
                    case "WhereSequenceMethod":
                    case "WhereOrdinal":
                        return this.Result.VisitorBuilder.Where().Visit(node, this.Result);

                    case "OfType":
                    case "Cast":
                 
                    case "Concat":
                    case "Intersect":
                    case "IntersectComparer":
                    case "Except":
                    case "ExceptComparer":
                    case "Last":
                    case "LastPredicate":
                    case "LastOrDefault":
                    case "LastOrDefaultPredicate":
                    case "ElementAt":
                    case "ElementAtOrDefault":
                    case "DefaultIfEmpty":
                    case "DefaultIfEmptyValue":
                    case "Reverse":
                    case "Empty":
                    case "SequenceEqual":
                    case "SequenceEqualComparer":
                    case "All":
                    case "Aggregate":
                    case "AggregateSeed":
                    case "AggregateSeedSelector":
                    case "AsQueryable":
                    case "AsQueryableGeneric":
                    case "AsEnumerable":
                    case "Zip":
                    case "NotSupported":
                        return node;
                    case "Skip":
                    case "SkipWhile":
                    case "SkipWhileOrdinal":
                        return this.Result.VisitorBuilder.Skip().Visit(node, this.Result);

                    case "Select":
                    case "SelectOrdinal":
                        return this.Result.VisitorBuilder.Select().Visit(node, this.Result);

                    case "SelectMany":
                    case "SelectManyOrdinal":
                    case "SelectManyResultSelector":
                    case "SelectManyOrdinalResultSelector":
                        return this.Result.VisitorBuilder.SelectMany().Visit(node, this.Result);

                    case "Join":
                    case "JoinComparer":
                       return this.Result.VisitorBuilder.Join().Visit(node, this.Result);

                    case "GroupJoin":
                    case "GroupJoinComparer":
                    case "GroupBy":
                    case "GroupByComparer":
                    case "GroupByElementSelector":
                    case "GroupByElementSelectorComparer":
                    case "GroupByResultSelector":
                    case "GroupByResultSelectorComparer":
                    case "GroupByElementSelectorResultSelector":
                    case "GroupByElementSelectorResultSelectorComparer":
                        return this.Result.VisitorBuilder.GroupBy().Visit(node, this.Result);

                    case "OrderBy":
                    case "OrderByComparer":
                    case "OrderByDescending":
                    case "OrderByDescendingComparer":
                    case "ThenBy":
                    case "ThenByComparer":
                    case "ThenByDescending":
                    case "ThenByDescendingComparer":
                        return this.Result.VisitorBuilder.OrderBy().Visit(node, this.Result);

                    case "Take":
                    case "TakeWhile":
                    case "TakeWhileOrdinal":
                       return this.Result.VisitorBuilder.Take().Visit(node, this.Result);

                    case "Distinct":
                    case "DistinctComparer":
                       return this.Result.VisitorBuilder.Distinct().Visit(node, this.Result);

                    case "Union":
                    case "UnionComparer":
                       return this.Result.VisitorBuilder.Union().Visit(node, this.Result);

                    case "First":
                    case "FirstPredicate":
                    case "FirstOrDefault":
                    case "FirstOrDefaultPredicate":
                       
                    case "Single":
                    case "SinglePredicate":
                    case "SingleOrDefault":
                    case "SingleOrDefaultPredicate":
                       return this.Result.VisitorBuilder.First().Visit(node, this.Result);

                    case "Contains":
                    case "ContainsComparer":
                        return this.Result.VisitorBuilder.Contains().Visit(node, this.Result);

                    case "Any":
                    case "AnyPredicate":
                       return this.Result.VisitorBuilder.Any().Visit(node, this.Result);

                    case "Count":
                    case "CountPredicate":
                    case "LongCount":
                    case "LongCountPredicate":
                        return this.Result.VisitorBuilder.Count().Visit(node, this.Result);

                    case "Min":
                    case "MinSelector":
                    case "MinInt":
                    case "MinNullableInt":
                    case "MinLong":
                    case "MinNullableLong":
                    case "MinDouble":
                    case "MinNullableDouble":
                    case "MinDecimal":
                    case "MinNullableDecimal":
                    case "MinSingle":
                    case "MinNullableSingle":
                    case "MinIntSelector":
                    case "MinNullableIntSelector":
                    case "MinLongSelector":
                    case "MinNullableLongSelector":
                    case "MinDoubleSelector":
                    case "MinNullableDoubleSelector":
                    case "MinDecimalSelector":
                    case "MinNullableDecimalSelector":
                    case "MinSingleSelector":
                    case "MinNullableSingleSelector":
                        return this.Result.VisitorBuilder.Min().Visit(node, this.Result);

                    case "Max":
                    case "MaxSelector":
                    case "MaxInt":
                    case "MaxNullableInt":
                    case "MaxLong":
                    case "MaxNullableLong":
                    case "MaxDouble":
                    case "MaxNullableDouble":
                    case "MaxDecimal":
                    case "MaxNullableDecimal":
                    case "MaxSingle":
                    case "MaxNullableSingle":
                    case "MaxIntSelector":
                    case "MaxNullableIntSelector":
                    case "MaxLongSelector":
                    case "MaxNullableLongSelector":
                    case "MaxDoubleSelector":
                    case "MaxNullableDoubleSelector":
                    case "MaxDecimalSelector":
                    case "MaxNullableDecimalSelector":
                    case "MaxSingleSelector":
                    case "MaxNullableSingleSelector":
                        return this.Result.VisitorBuilder.Max().Visit(node, this.Result);
                    case "Sum":
                    case "SumInt":
                    case "SumNullableInt":
                    case "SumLong":
                    case "SumNullableLong":
                    case "SumDouble":
                    case "SumNullableDouble":
                    case "SumDecimal":
                    case "SumNullableDecimal":
                    case "SumSingle":
                    case "SumNullableSingle":
                    case "SumIntSelector":
                    case "SumNullableIntSelector":
                    case "SumLongSelector":
                    case "SumNullableLongSelector":
                    case "SumDoubleSelector":
                    case "SumNullableDoubleSelector":
                    case "SumDecimalSelector":
                    case "SumNullableDecimalSelector":
                    case "SumSingleSelector":
                    case "SumNullableSingleSelector":
                        return this.Result.VisitorBuilder.Sum().Visit(node, this.Result);

                    case "Average":
                    case "AverageInt":
                    case "AverageNullableInt":
                    case "AverageLong":
                    case "AverageNullableLong":
                    case "AverageDouble":
                    case "AverageNullableDouble":
                    case "AverageDecimal":
                    case "AverageNullableDecimal":
                    case "AverageSingle":
                    case "AverageNullableSingle":
                    case "AverageIntSelector":
                    case "AverageNullableIntSelector":
                    case "AverageLongSelector":
                    case "AverageNullableLongSelector":
                    case "AverageDoubleSelector":
                    case "AverageNullableDoubleSelector":
                    case "AverageDecimalSelector":
                    case "AverageNullableDecimalSelector":
                    case "AverageSingleSelector":
                    case "AverageNullableSingleSelector":
                       return this.Result.VisitorBuilder.Average().Visit(node, this.Result);

                    case "Page":
                        return this.Result.VisitorBuilder.Page().Visit(node, this.Result);
                   

                    case "SetJoinMode":
                        return this.Result.VisitorBuilder.SetJoinMode().Visit(node, this.Result);

                    case "HasCount":
                        return node;
                     //  return this.Result.VisitorBuilder.HasCount().Visit(node, this.Result);
                      //  break;

                     
                }
            }
            return node;
        }

        /// <summary>
        /// linq 表达式类型 Visit
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            ExpressionType nodeType = node.NodeType;
            if (nodeType != ExpressionType.Convert)
            {
                if (nodeType != ExpressionType.Not)
                {
                    if (nodeType != ExpressionType.Quote)
                    {
                        this.Write(node.NodeType.ToString());
                        this.Write("(");

                        this.WriteWhere(node.NodeType.ToString());
                        this.WriteWhere("(");
                    }
                }
                else
                {
                    this.Write("Not(");

                    this.WriteWhere("Not(");
                }
            }
            this.Visit(node.Operand);
            switch (node.NodeType)
            {
                case ExpressionType.Convert:
                    return node;

                case ExpressionType.Quote:
                    return node;
            }
            this.Write(")");
            this.WriteWhere(")");
        
            return node;
        }

        protected virtual void Write(string s)
        {
            this.Result.SqlBuilder.Append(s);
        }
        protected virtual void AddSqlSelectFields(FieldMdel s)
        {
            this.Result.SqlSelectFields.Add(s);
        }
        protected virtual void WriteWhere(string s) {


           // this.Result.WhereText.Append(s);
        }
    }
}

