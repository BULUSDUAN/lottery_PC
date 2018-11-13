using KaSon.FrameWork.Services.Attribute;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace KaSon.FrameWork.ORM.Visitor.SQLServer
{

    internal class ContainsVisitor : Visitor.ContainsLinqVisitor
    {
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

            ///实例化成员值
            if (node.Expression is ConstantExpression)
            {
                object value = null;
                if (node.Member.MemberType == MemberTypes.Field)
                {
                    value = GetFieldValue(node);
                }
                else if (node.Member.MemberType == MemberTypes.Property)
                {
                    value = base.GetPropertyValue(node);
                }


               /// 是否数据
                if (value is Array || value.GetType().Name.ToString().Contains("List"))
                {

                    //  var arr = (Array)value;
                    StringBuilder sb = new StringBuilder();
                 //   string temp = "";
                    foreach (var item in value as IEnumerable)
                    {
                        if (item is Int32 || item is Int64)
                        {
                           // temp += temp + item.ToString() + ",";
                            sb.Append(item.ToString() + ",");
                        }
                        else
                        {
                            sb.Append( "'" + item + "',");
                          //  temp += temp + "'" + item + "',";
                        }
                    }

                    if (sb.ToString().Contains(","))
                    {
                        value = sb.ToString().Remove(sb.ToString().LastIndexOf(','));
                    }
                  
                    //string paramerName = base.GetParamerName();
                    this.Write(value+"");
                   // base.Result.Parameters.Insert(paramerName, value + "", ParameterDirection.Input);
                }
                else
                {
                    string paramerName = base.GetParamerName();
                    this.Write(paramerName);
                    base.Result.Parameters.Insert(paramerName, value + "", ParameterDirection.Input);
                }

             

       
                return node;
                // this.Visit(node.Expression);
            }
          

            if ((type != null) && base.Result.QueryColletion.Contains(type.FullName))
            {
                QueryParameter parameter = base.Result.QueryColletion[type.FullName];
                this.Write(parameter.Alias);
                this.Write(".");
                this.Write("[");
                this.Write(parameter.GetColumn(node.Member.Name).FieldMap);
                this.Write("]");
            }
            return node;
        }
     
    }
}
