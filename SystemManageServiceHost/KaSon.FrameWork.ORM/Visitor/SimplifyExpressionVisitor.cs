namespace KaSon.FrameWork.ORM.Visitor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class SimplifyExpressionVisitor
    {
        private readonly string[] _specialMethod = new string[] { "StartsWith", "Contains", "EndsWith", "All", "Any", "Parse" };

        internal SimplifyExpressionVisitor()
        {
        }

        private bool CheckArguments(IEnumerable<Expression> arguments)
        {
            return arguments.All<Expression>(exp => (exp is ConstantExpression));
        }

        private object[] GetArgumentsValue(IEnumerable<Expression> arguments)
        {
            return (from exp in arguments.OfType<ConstantExpression>() select exp.Value).ToList<object>().ToArray();
        }

        private bool IsBinaryTure(Expression exp)
        {
            return (((exp is ConstantExpression) && (exp as ConstantExpression).Type.Equals(typeof(bool))) && Convert.ToBoolean((exp as ConstantExpression).Value));
        }

        internal bool IsSpecialMethod(string name)
        {
            return this._specialMethod.Contains<string>(name);
        }

        internal virtual Expression Simplify(Expression exp)
        {
            return this.Visit(exp);
        }

        internal virtual Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return exp;
            }
            switch (exp.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return this.VisitBinary((BinaryExpression)exp);

                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression)exp);

                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);

                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);

                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);

                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)exp);

                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);

                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);

                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);

                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);

                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);

                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);

                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression)exp);
            }
            throw new Exception("不明表达式类型：" + exp.NodeType);
        }

        internal virtual Expression VisitBinary(BinaryExpression b)
        {
            Expression exp = this.Visit(b.Left);
            Expression expression2 = this.Visit(b.Right);
            Expression expression3 = this.Visit(b.Conversion);
            if (((exp == b.Left) && (expression2 == b.Right)) && (expression3 == b.Conversion))
            {
                return b;
            }
            if ((b.NodeType == ExpressionType.And) || (b.NodeType == ExpressionType.AndAlso))
            {
                if (this.IsBinaryTure(exp))
                {
                    return expression2;
                }
                if (this.IsBinaryTure(expression2))
                {
                    return exp;
                }
            }
            if ((exp == null) && (expression2 == null))
            {
                if (b.NodeType == ExpressionType.Equal)
                {
                    return Expression.Constant(true);
                }
                if (b.NodeType == ExpressionType.NotEqual)
                {
                    return Expression.Constant(false);
                }
            }
            if ((exp is ConstantExpression) && (expression2 is ConstantExpression))
            {
                if (!exp.Type.IsValueType)
                {
                    string str = (exp as ConstantExpression).Value.ToString();
                    string str2 = (expression2 as ConstantExpression).Value.ToString();
                    switch (b.NodeType)
                    {
                        case ExpressionType.Equal:
                            return Expression.Constant(str.Equals(str2));

                        case ExpressionType.NotEqual:
                            return Expression.Constant(!str.Equals(str2));
                    }
                }
                else
                {
                    decimal num = Convert.ToDecimal((exp as ConstantExpression).Value);
                    decimal num2 = Convert.ToDecimal((expression2 as ConstantExpression).Value);
                    switch (b.NodeType)
                    {
                        case ExpressionType.Equal:
                            return Expression.Constant(num.Equals(num2));

                        case ExpressionType.GreaterThan:
                            return Expression.Constant(num > num2);

                        case ExpressionType.GreaterThanOrEqual:
                            return Expression.Constant(num >= num2);

                        case ExpressionType.LessThan:
                            return Expression.Constant(num < num2);

                        case ExpressionType.LessThanOrEqual:
                            return Expression.Constant(num <= num2);

                        case ExpressionType.NotEqual:
                            return Expression.Constant(!num.Equals(num2));
                    }
                }
            }
            if (b.NodeType == ExpressionType.Coalesce)
            {
                return Expression.Coalesce(exp, expression2, expression3 as LambdaExpression);
            }
            if (exp.Type != expression2.Type)
            {
                expression2 = Expression.Convert(expression2, exp.Type);
            }
            return Expression.MakeBinary(b.NodeType, exp, expression2, b.IsLiftedToNull, b.Method);
        }

        internal virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment)binding);

                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding)binding);

                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding((MemberListBinding)binding);
            }
            throw new Exception("不明表达式类型：" + binding.BindingType);
        }

        internal virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            int num = 0;
            int count = original.Count;
            while (num < count)
            {
                MemberBinding item = this.VisitBinding(original[num]);
                if (list != null)
                {
                    list.Add(item);
                }
                else if (item != original[num])
                {
                    list = new List<MemberBinding>(count);
                    for (int i = 0; i < num; i++)
                    {
                        list.Add(original[i]);
                    }
                    list.Add(item);
                }
                num++;
            }
            if (list != null)
            {
                return list;
            }
            return original;
        }

        internal virtual Expression VisitConditional(ConditionalExpression c)
        {
            Expression test = this.Visit(c.Test);
            Expression ifTrue = this.Visit(c.IfTrue);
            Expression ifFalse = this.Visit(c.IfFalse);
            if (test is ConstantExpression)
            {
                if (Convert.ToBoolean(((ConstantExpression)test).Value))
                {
                    return ifTrue;
                }
                return ifFalse;
            }
            if (((test == c.Test) && (ifTrue == c.IfTrue)) && (ifFalse == c.IfFalse))
            {
                return c;
            }
            return Expression.Condition(test, ifTrue, ifFalse);
        }

        internal virtual Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

        internal virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            IList<Expression> arguments = this.VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments)
            {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }
            return initializer;
        }

        internal virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            int num = 0;
            int count = original.Count;
            while (num < count)
            {
                ElementInit item = this.VisitElementInitializer(original[num]);
                if (list != null)
                {
                    list.Add(item);
                }
                else if (item != original[num])
                {
                    list = new List<ElementInit>(count);
                    for (int i = 0; i < num; i++)
                    {
                        list.Add(original[i]);
                    }
                    list.Add(item);
                }
                num++;
            }
            if (list != null)
            {
                return list;
            }
            return original;
        }

        internal virtual IList<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;
            int num = 0;
            int count = original.Count;
            while (num < count)
            {
                Expression item = this.Visit(original[num]);
                if (list != null)
                {
                    list.Add(item);
                }
                else if (item != original[num])
                {
                    list = new List<Expression>(count);
                    for (int i = 0; i < num; i++)
                    {
                        list.Add(original[i]);
                    }
                    list.Add(item);
                }
                num++;
            }
            if (list != null)
            {
                return list;
            }
            return original;
        }

        internal virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> arguments = this.VisitExpressionList(iv.Arguments);
            Expression expression = this.Visit(iv.Expression);
            if ((arguments == iv.Arguments) && (expression == iv.Expression))
            {
                return iv;
            }
            return Expression.Invoke(expression, arguments);
        }

        internal virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Expression body = this.Visit(lambda.Body);
            if (body != lambda.Body)
            {
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);
            }
            return lambda;
        }

        internal virtual Expression VisitListInit(ListInitExpression init)
        {
            NewExpression newExpression = this.VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
            if ((newExpression == init.NewExpression) && (initializers == init.Initializers))
            {
                return init;
            }
            return Expression.ListInit(newExpression, initializers);
        }

        internal virtual Expression VisitMemberAccess(MemberExpression m)
        {
            object obj2;
            Expression expression = this.Visit(m.Expression);
            if (m.Expression == null)
            {
                obj2 = null;
                if (m.Member.MemberType == MemberTypes.Property)
                {
                    obj2 = ((PropertyInfo)m.Member).GetValue(null, null);
                }
                if (m.Member.MemberType == MemberTypes.Field)
                {
                    obj2 = ((FieldInfo)m.Member).GetValue(null);
                }
                return Expression.Constant(obj2);
            }
            if (expression is ConstantExpression)
            {
                object obj3 = (expression as ConstantExpression).Value;
                obj2 = null;
                if (obj3 == null)
                {
                    if (m.Member.Name == "Value")
                    {
                        return m;
                    }
                    if (m.Member.Name == "HasValue")
                    {
                        return Expression.Constant(false);
                    }
                }
                if (m.Member.MemberType == MemberTypes.Property)
                {
                    obj2 = ((PropertyInfo)m.Member).GetValue(obj3, null);
                }
                if (m.Member.MemberType == MemberTypes.Field)
                {
                    obj2 = ((FieldInfo)m.Member).GetValue(obj3);
                }
                return Expression.Constant(obj2);
            }
            if (expression != m.Expression)
            {
                return Expression.MakeMemberAccess(expression, m.Member);
            }
            return m;
        }

        internal virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression expression = this.Visit(assignment.Expression);
            if (expression != assignment.Expression)
            {
                if (expression.Type.Equals(assignment.Expression.Type))
                {
                    return Expression.Bind(assignment.Member, expression);
                }
                return Expression.Bind(assignment.Member, Expression.Convert(expression, assignment.Expression.Type));
            }
            return assignment;
        }

        internal virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression newExpression = this.VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
            if ((newExpression == init.NewExpression) && (bindings == init.Bindings))
            {
                return init;
            }
            return Expression.MemberInit(newExpression, bindings);
        }

        internal virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
            if (initializers != binding.Initializers)
            {
                return Expression.ListBind(binding.Member, initializers);
            }
            return binding;
        }

        internal virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
            if (bindings != binding.Bindings)
            {
                return Expression.MemberBind(binding.Member, bindings);
            }
            return binding;
        }

        internal virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            Expression instance = this.Visit(m.Object);
            IEnumerable<Expression> arguments = this.VisitExpressionList(m.Arguments);
            if ((this.CheckArguments(arguments) && (m.Method.ReturnType != typeof(void))) && !this.IsSpecialMethod(m.Method.Name))
            {
                object obj2 = null;
                if (m.Object != null)
                {
                    obj2 = ((ConstantExpression)instance).Value;
                }
                return Expression.Constant(m.Method.Invoke(obj2, this.GetArgumentsValue(arguments)));
            }
            if ((instance == m.Object) && (arguments == m.Arguments))
            {
                return m;
            }
            return Expression.Call(instance, m.Method, arguments);
        }

        internal virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> arguments = this.VisitExpressionList(nex.Arguments);
            if (arguments == nex.Arguments)
            {
                return nex;
            }
            if (nex.Members != null)
            {
                return Expression.New(nex.Constructor, arguments, nex.Members);
            }
            return Expression.New(nex.Constructor, arguments);
        }

        internal virtual Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> initializers = this.VisitExpressionList(na.Expressions);
            if (initializers == na.Expressions)
            {
                return na;
            }
            if (na.NodeType == ExpressionType.NewArrayInit)
            {
                return Expression.NewArrayInit(na.Type.GetElementType(), initializers);
            }
            return Expression.NewArrayBounds(na.Type.GetElementType(), initializers);
        }

        internal virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        internal virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            Expression expression = this.Visit(b.Expression);
            if (expression != b.Expression)
            {
                return Expression.TypeIs(expression, b.TypeOperand);
            }
            return b;
        }

        internal virtual Expression VisitUnary(UnaryExpression u)
        {
            Expression operand = this.Visit(u.Operand);
            if (operand != u.Operand)
            {
                return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
            }
            return u;
        }
    }
}

