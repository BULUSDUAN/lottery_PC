using KaSon.FrameWork.Services.Attribute;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using System.Text;

namespace KaSon.FrameWork.ORM
{
    //public sealed class DynamicMethod : MethodInfo
    //{
    //    public ParameterBuilder DefineParameter(int position, ParameterAttributes attributes, string parameterName) {

    //        return null;
    //    }
    //}
    class Widget
    {
        public int Length;
    }
    delegate ref int WidgetMeasurer(Widget w);
    /// <summary>
    /// 实体映射
    /// </summary>
    internal class EntityHelper
    {
        private static readonly Type ObjectType = typeof(object);
        private static readonly IDictionary<Type, DbType> TpyeToDbType;
        private static readonly IDictionary<Type, OpCode> ValueTpyeOpCodes;

        static EntityHelper()
        {
            Dictionary<Type, OpCode> dictionary = new Dictionary<Type, OpCode>();
            dictionary.Add(typeof(sbyte), OpCodes.Ldind_I1);
            dictionary.Add(typeof(byte), OpCodes.Ldind_U1);
            dictionary.Add(typeof(char), OpCodes.Ldind_U2);
            dictionary.Add(typeof(short), OpCodes.Ldind_I2);
            dictionary.Add(typeof(ushort), OpCodes.Ldind_U2);
            dictionary.Add(typeof(int), OpCodes.Ldind_I4);
            dictionary.Add(typeof(uint), OpCodes.Ldind_U4);
            dictionary.Add(typeof(long), OpCodes.Ldind_I8);
            dictionary.Add(typeof(ulong), OpCodes.Ldind_I8);
            dictionary.Add(typeof(bool), OpCodes.Ldind_I1);
            dictionary.Add(typeof(double), OpCodes.Ldind_R8);
            dictionary.Add(typeof(float), OpCodes.Ldind_R4);
            ValueTpyeOpCodes = dictionary;
            Dictionary<Type, DbType> dictionary2 = new Dictionary<Type, DbType>();
            dictionary2.Add(typeof(string), DbType.String);
            dictionary2.Add(typeof(decimal), DbType.Decimal);
            dictionary2.Add(typeof(int), DbType.Int32);
            dictionary2.Add(typeof(long), DbType.Int64);
            dictionary2.Add(typeof(bool), DbType.Boolean);
            dictionary2.Add(typeof(DateTime), DbType.DateTime);
            dictionary2.Add(typeof(Guid), DbType.Guid);
            dictionary2.Add(typeof(byte), DbType.Byte);
            dictionary2.Add(typeof(byte[]), DbType.Binary);
            TpyeToDbType = dictionary2;
        }
        /// <summary>
        /// 匿名应射
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static Func<DbDataReader, IList<T>> AnonymAssign<T>(ConstructorInfo constructor)
        {
            return delegate(DbDataReader reader)
            {
                Type type = typeof(T);
                PropertyInfo[] properties = type.GetProperties();
                List<T> list = new List<T>();
                while (reader.Read())
                {
                    object[] objArray2;
                    Type propertyType;
                    object[] values = new object[reader.FieldCount];
                    int num = reader.GetValues(values);
                    if (type.IsValueType || (type == typeof(string)))
                    {
                        if (type == typeof(bool))
                        {
                            list.Add((T)OperateCommon.AutoConvert(values[0], typeof(bool)));
                        }
                        else if (values[0] != DBNull.Value)
                        {
                            list.Add((T)values[0]);
                        }
                        else
                        {
                            T item = default(T);
                            list.Add(item);
                        }
                        continue;
                    }
                    bool flag = false;
                    int ordinal = 0;
                    while (ordinal < reader.FieldCount)
                    {
                        if (reader.GetName(ordinal) == "ROWNUMBER__")
                        {
                            flag = true;
                            break;
                        }
                        ordinal++;
                    }
                    if (flag)
                    {
                        objArray2 = new object[values.Length - 1];
                        ordinal = 1;
                        while (ordinal < values.Length)
                        {
                            if (!(values[ordinal] is DBNull))
                            {
                                propertyType = properties[ordinal - 1].PropertyType;
                                if (((((propertyType == typeof(decimal)) || (propertyType == typeof(int))) || ((propertyType == typeof(bool)) || (propertyType == typeof(long)))) || ((propertyType == typeof(double)) || (propertyType == typeof(float)))) || (propertyType == typeof(byte)))
                                {
                                    objArray2[ordinal - 1] = OperateCommon.AutoConvert(values[ordinal], propertyType);
                                }
                                else
                                {
                                    objArray2[ordinal - 1] = values[ordinal];
                                }
                            }
                            ordinal++;
                        }
                    }
                    else
                    {
                        objArray2 = new object[values.Length];
                        for (ordinal = 0; ordinal < values.Length; ordinal++)
                        {
                            if (!(values[ordinal] is DBNull))
                            {
                                propertyType = properties[ordinal].PropertyType;
                                if (((((propertyType == typeof(decimal)) || (propertyType == typeof(int))) || ((propertyType == typeof(bool)) || (propertyType == typeof(long)))) || ((propertyType == typeof(double)) || (propertyType == typeof(float)))) || (propertyType == typeof(byte)))
                                {
                                    objArray2[ordinal] = OperateCommon.AutoConvert(values[ordinal], propertyType);
                                }
                                else
                                {
                                    objArray2[ordinal] = values[ordinal];
                                }
                            }
                        }
                    }
                    object obj2 = constructor.Invoke(objArray2);
                    list.Add((T)obj2);
                }
                return list;
            };
        }


        private static TypeBuilder GetTypeBuilder()
        {
            var typeSignature = "MyDynamicType";
            var an = new AssemblyName(typeSignature);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);
            return tb;
        }


        #region  Get Set 对象属性方法 
        /// <summary>
        /// 获取 Get 方法 
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private static Func<object, object> CreateGetFunction(MethodInfo methodInfo, Type entityType)
        {
            //  DynamicMethod
           // var tb = GetTypeBuilder();
            //var pt = entityType.GetProperty("Name");
            // MethodBuilder method = tb.DefineMethod("GetValue", MethodAttributes.Public , ObjectType, new Type[] { ObjectType });
            //            var method = typeof(k_user).GetProperty("Name",
            //BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            //.GetGetMethod(false);
            //            var func = (Func<entityType, object>)
            //            Delegate.CreateDelegate(typeof(Func<entityType, object>), method);

            //FieldInfo lengthField = typeof(Widget).GetField(nameof(Widget.Length));

            ////FieldInfo lengthField = typeof(Widget).GetField(nameof(Widget.Length));
            //var emitter = System.Reflection.Emit<WidgetMeasurer>.NewDynamicMethod()
            //    .LoadArgument(0)
            //    .LoadFieldAddress(lengthField)
            //    .Return();
            //return emitter.CreateDelegate();
            //  var loadedType = tb.ty;
            //MethodInfo mi = loadedType.GetMethod(method.Name);
            //myDelegate = (MyDelegate)Delegate.CreateDelegate(typeof(MyDelegate), methodInfo);

            DynamicMethod method = new DynamicMethod("GetValue", ObjectType, new Type[] { ObjectType });
            ILGenerator iLGenerator = method.GetILGenerator();
            iLGenerator.DeclareLocal(ObjectType);
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Castclass, entityType);
            iLGenerator.EmitCall(OpCodes.Call, methodInfo, null);
            if (methodInfo.ReturnType.IsValueType)
            {
                iLGenerator.Emit(OpCodes.Box, methodInfo.ReturnType);
            }
            iLGenerator.Emit(OpCodes.Stloc_0);
            iLGenerator.Emit(OpCodes.Ldloc_0);
            iLGenerator.Emit(OpCodes.Ret);
           // method.DefineParameter(1, ParameterAttributes.In, "value");
          //  var type = tb.CreateTypeInfo();
           // var mi = type.GetMethod(method.Name);
            // var mi = tb.GetMethod(method.Name);

          

            //var b= mi.CreateDelegate(typeof(String)); 

            return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
        }


        /// <summary>
        ///  构造 SET 方法
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        internal static Action<object, object> CreateSetAction(MethodInfo methodInfo, Type entityType)
        {

            var tb = GetTypeBuilder();

            //MethodBuilder method = tb.DefineMethod("SetValue", MethodAttributes.Public | MethodAttributes.Virtual, null, new Type[] { ObjectType, ObjectType });

           DynamicMethod method = new DynamicMethod("SetValue", null, new Type[] { ObjectType, ObjectType });
            ILGenerator iLGenerator = method.GetILGenerator();
            Type parameterType = methodInfo.GetParameters()[0].ParameterType;
            iLGenerator.DeclareLocal(parameterType);
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Castclass, entityType);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            if (parameterType.IsValueType)
            {
                iLGenerator.Emit(OpCodes.Unbox, parameterType);
                if (ValueTpyeOpCodes.ContainsKey(parameterType))
                {
                    OpCode opcode = ValueTpyeOpCodes[parameterType];
                    iLGenerator.Emit(opcode);
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Ldobj, parameterType);
                }
            }
            else
            {
                iLGenerator.Emit(OpCodes.Castclass, parameterType);
            }
            iLGenerator.EmitCall(OpCodes.Callvirt, methodInfo, null);
            iLGenerator.Emit(OpCodes.Ret);
            //method.DefineParameter(1, ParameterAttributes.In, "obj");
            //method.DefineParameter(2, ParameterAttributes.In, "value");
            return (Action<object, object>)method.CreateDelegate(typeof(Action<object, object>));
        }

        #endregion
        public static Func<DbDataReader, IList<T>> EntityAssign<T>()
        {
            return EntityAssign<T>(GetEntityInfo(typeof(T)));
        }

       
        /// <summary>
        /// 数据库数据映射到对象,
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns name="Func<DbDataReader, IList<T>>">返回委托  Func<DbDataReader, IList<T>></returns>
        public static Func<DbDataReader, IList<T>> EntityAssign<T>(EntityInfo info)
        {
            return delegate(DbDataReader reader)
            {
                List<T> list = new List<T>();
                while (reader.Read())
                {
                    object[] values = new object[reader.FieldCount];
                    reader.GetValues(values);
                    Type type = typeof(T);
                    if (type.IsValueType || (type == typeof(string)))
                    {
                        if ((type == typeof(bool)) || (type == typeof(decimal)))
                        {
                            list.Add((T)OperateCommon.AutoConvert(values[0], type));
                        }
                        else if (values[0] != DBNull.Value)
                        {
                            list.Add((T)values[0]);
                        }
                        else
                        {
                            T item = default(T);
                            list.Add(item);
                        }
                    }
                    else
                    {
                        int num;
                        string name;
                        object obj2 = Activator.CreateInstance(typeof(T));
                        if (info.IsEntity)
                        {
                            num = 0;
                            while (num < reader.FieldCount)
                            {
                                name = reader.GetName(num);
                                if (values[num] != DBNull.Value)
                                {
                                    if (info.Fields.ContainsKey(name))
                                    {
                                        info.Fields[name].Property.Set(obj2, values[num]);
                                    }
                                    else if (info.Propertys.ContainsKey(name))
                                    {
                                        info.Propertys[name].Set(obj2, values[num]);
                                    }
                                }
                                num++;
                            }
                        }
                        else
                        {
                            for (num = 0; num < reader.FieldCount; num++)
                            {
                                name = reader.GetName(num);
                                if (info.Propertys.ContainsKey(name) && (values[num] != DBNull.Value))
                                {
                                    info.Propertys[name].Set(obj2, values[num]);
                                }
                            }
                        }
                        list.Add((T)obj2);
                    }
                }
                return list;
            };
        }


        public static Func<DbDataReader, IList<T>> EntityAssign<T>(QueryProviderContext context)
        {
            return EntityAssignExtend<T>(context);
        }
        public static Func<DbDataReader, IList<T>> EntityAssignExtend<T>(QueryProviderContext context)
        {
            IDictionary<object, IGrouping> groups = new Dictionary<object, IGrouping>();
            Type type = typeof(T);
            var multEntityInfo = EntityHelper.GetMultEntityInfo(type);
           
            return delegate(DbDataReader reader)
            {

                IList<T> list = new List<T>();
              
                //值类型
                if (type.IsValueType || (type == typeof(string)))
                {
                    while (reader.Read())
                    {
                        object _obj = reader.GetValue(0);
                        if ((type == typeof(bool)) || (type == typeof(decimal)))
                        {
                            list.Add((T)OperateCommon.AutoConvert(_obj, type));
                        }
                        else if (_obj != DBNull.Value)
                        {
                            list.Add((T)_obj);
                        }
                        else
                        {
                            T item = default(T);
                            list.Add(item);
                        }
                    }
                }
                else
                {
                    ///实体类型
                    int num;
                    string name;
                    object obj2 = null;
                    object _objvalue = null;

                    string readerkey = "";
                    QueryParameter qparameter = null;
                   bool queryBol= context.QueryColletion.ContainsEx(type.FullName, out qparameter);
                    object[] _objArr = type.GetCustomAttributes(typeof(EntityAttribute), false);
                    //是否实体
                    if (type.Name == "IGrouping`2")
                    {
                        //kEY 类型
                        Type _keyType = context.GroupKeyType;

                        //分组类型实体
                        Type _returnType = context.GroupType;

                        //分组类型实体
                        EntityInfo info2 = GetEntityInfo(_returnType);
                        Type groupingType = typeof(Grouping<,>).MakeGenericType(_keyType, _returnType);
                        while (reader.Read())
                        {
                            num = 0;
                            object returnValue = Activator.CreateInstance(_returnType);
                            while (num < reader.FieldCount)
                            {

                                name = reader.GetName(num);
                                _objvalue = reader.GetValue(num);

                                if (_objvalue != DBNull.Value)
                                {
                                    if (info2.Fields.ContainsKey(name))
                                    {
                                        info2.Fields[name].Property.Set(returnValue, _objvalue);
                                    }
                                    else if (info2.Propertys.ContainsKey(name))
                                    {
                                        info2.Propertys[name].Set(returnValue, _objvalue);
                                    }
                                }
                                num++;
                            }

                            object key = info2.Propertys[context.GroupKeyName].Get(returnValue);
                            if (groups.ContainsKey(key))
                            {
                                groups[key].Add(returnValue);

                            }
                            else
                            {
                                IGrouping group = (IGrouping)Activator.CreateInstance(groupingType, key);
                                group.Add(returnValue);
                                groups[key] = group;

                            }
                        }

                        var reusltEnumerato = groups.Values.GetEnumerator();
                        using (IEnumerator<IGrouping> enumerator = reusltEnumerato)
                        {
                            IGrouping current = enumerator.Current;
                            while (enumerator.MoveNext())
                            {
                                T t = (T)enumerator.Current.GetMe();
                                list.Add(t);
                            }

                        }


                    }
                    else if (_objArr.Length > 0)
                    {
                        EntityInfo info = GetEntityInfo(type);
                        while (reader.Read())
                        {
                            obj2 = Activator.CreateInstance(type);
                            num = 0;
                            while (num < reader.FieldCount)
                            {
                                name = reader.GetName(num);
                                _objvalue = reader.GetValue(num);
                                if (_objvalue != DBNull.Value)
                                {
                                    if (info.Fields.ContainsKey(name))
                                    {
                                        info.Fields[name].Property.Set(obj2, _objvalue);
                                    }
                                    else if (info.Propertys.ContainsKey(name))
                                    {
                                        info.Propertys[name].Set(obj2, _objvalue);
                                    }
                                }
                                num++;
                            }
                            list.Add((T)obj2);
                        }
                    }
                    else if (qparameter.IsAnonym)//匿名对象
                    {

                        PropertyInfo[] properties = type.GetProperties();

                        while (reader.Read())//一行一行
                        {
                            num = -1;
                            object[] objArray2 = new object[properties.Length];
                            
                            foreach (var item in properties)
                            {
                                num++;
                                Type propertyType = item.PropertyType;
                                //属性是实体类情况处理
                                //.ContainsEx(b.Type.ToString(),out qparameter)
                                QueryParameter _qparameter = null;
                                if (context.QueryColletion.ContainsEx(propertyType.FullName,out _qparameter))
                                {
                                    if (_qparameter.IsEntity) {
                                        object returnValue = Activator.CreateInstance(propertyType);
                                        EntityInfo info = GetEntityInfo(propertyType);
                                        var qcolle = context.QueryColletion[propertyType.FullName];

                                        foreach (var col in qcolle.Columns)
                                        {
                                            readerkey = qcolle.Alias + "_" + col.FieldMap;
                                            _objvalue = reader[readerkey];
                                            info.Propertys[col.Name].Set(returnValue, _objvalue);

                                        }
                                        objArray2[num] = returnValue;
                                    }

                                   

                                }
                                else
                                {
                                    //其他类型
                                    readerkey = item.Name;
                                    _objvalue = reader[readerkey];

                                    if (!(_objvalue is DBNull))
                                    {
                                        //不是实体类
                                        if (((((propertyType == typeof(decimal)) || (propertyType == typeof(int))) || ((propertyType == typeof(bool)) || (propertyType == typeof(long)))) || ((propertyType == typeof(double)) || (propertyType == typeof(float)))) || (propertyType == typeof(byte)))
                                        {
                                            objArray2[num] = OperateCommon.AutoConvert(_objvalue, propertyType);
                                        }
                                        else
                                        {
                                            objArray2[num] = _objvalue;
                                        }

                                    }

                                }


                            }


                            obj2 = context.Constructor.Invoke(objArray2);
                            list.Add((T)obj2);


                        }



                    }
                    else if (!qparameter.IsAnonym && !qparameter.IsEntity)//新对象
                    {
                        PropertyInfo[] properties = type.GetProperties();
                         readerkey = "";

                        //EntityInfo info = GetEntityInfo(type);
                        //while (reader.Read())
                        //{
                        //    obj2 = Activator.CreateInstance(type);
                        //    num = 0;
                        //    while (num < reader.FieldCount)
                        //    {
                        //        name = reader.GetName(num);
                        //        _objvalue = reader.GetValue(num);
                        //        if (_objvalue != DBNull.Value)
                        //        {
                        //            if (info.Fields.ContainsKey(name))
                        //            {
                        //                info.Fields[name].Property.Set(obj2, _objvalue);
                        //            }
                        //            else if (info.Propertys.ContainsKey(name))
                        //            {
                        //                info.Propertys[name].Set(obj2, _objvalue);
                        //            }
                        //        }
                        //        num++;
                        //    }
                        //    list.Add((T)obj2);
                        //}
                        EntityInfo info = GetEntityInfo(type);
                        string FieldTypeName = "";
                        while (reader.Read())//一行一行
                        {
                            obj2 = Activator.CreateInstance(type);
                            num = -1;
                           // object[] objArray2 = new object[properties.Length];

                            foreach (var fieldName in context.SqlSelectFields)
                            {
                                _objvalue = reader[fieldName.MemberName];


                                try
                                {
                                    FieldTypeName = info.Propertys[fieldName.MemberName].Info.PropertyType.Name;
                                    //if (_objvalue is DBNull)
                                    //{

                                    //    // Type propertyType = properties[num].PropertyType;
                                    //    Console.Write("");



                                    //}
                                    info.Propertys[fieldName.MemberName].Set(obj2, _objvalue);
                                    //if (_objvalue == null)
                                    //{
                                    //    switch (FieldTypeName)
                                    //    {
                                    //        case "Boolean":
                                    //            info.Propertys[fieldName.MemberName].Set(obj2, false);
                                    //            break;
                                    //        case "int":
                                    //            info.Propertys[fieldName.MemberName].Set(obj2, 0);
                                    //            break;


                                    //    }
                                    //}
                                    //else
                                    //{

                                    //}
                                }
                                catch (Exception EX)
                                {

                                    throw EX;
                                }
                               
                                
                               
                                //  var p = item.Value.Info;
                                //  Type propertyType = p.PropertyType;
                               // for (int j = 0; j < reader.FieldCount; j++)
                               // {

                               //    // _objvalue = reader.GetValue(j);
                               //   //  readerkey = reader.GetName(j);
                               //     //if (context.SqlSelectFields.Contains(fieldName))
                               //     //{
                                      
                                       
                               //     //    object returnValue = Activator.CreateInstance(propertyType);
                               //     //    EntityInfo propertyInfo = GetEntityInfo(propertyType);
                               //     //    var qcolle = context.QueryColletion[propertyType.FullName];

                               //     //    foreach (var col in qcolle.Columns)
                               //     //    {
                               //     //        readerkey = qcolle.Alias + "_" + col.FieldMap;
                               //     //        _objvalue = reader[readerkey];
                               //     //        propertyInfo.Propertys[col.Name].Set(returnValue, _objvalue);

                               //     //    }
                               //     //    //  objArray2[num] = returnValue;
                               //     //   // info.Propertys[p.Name].Set(obj2, returnValue);
                               //     //    break;
                               //     //}
                               //     //else
                               //     //{
                               //     //    if (readerkey != p.Name) {
                               //     //        continue;
                               //     //    }
                               //     //    //_objvalue = reader[p.Name];
                               //     //    if (((((propertyType == typeof(decimal)) || (propertyType == typeof(int))) || ((propertyType == typeof(bool)) || (propertyType == typeof(long)))) || ((propertyType == typeof(double)) || (propertyType == typeof(float)))) || (propertyType == typeof(byte)))
                               //     //    {
                               //     //        //  objArray2[num] = OperateCommon.AutoConvert(_objvalue, propertyType);
                               //     //        info.Propertys[p.Name].Set(obj2, _objvalue);
                               //     //    }
                               //     //    else
                               //     //    {
                               //     //        info.Propertys[p.Name].Set(obj2, _objvalue);
                               //     //    }
                               //     //    break;
                               //     //}
                               //}
                            }

                       

                           // objArray2[1] = 0;
                           // obj2 = context.Constructor.Invoke(objArray2);
                            list.Add((T)obj2);
                        }
                    }
                    else
                    {

                        PropertyInfo[] properties = type.GetProperties();
                        while (reader.Read())
                        {
                            object[] objArray2 = new object[reader.FieldCount];
                            for (num = 0; num < reader.FieldCount; num++)
                            {
                                _objvalue = reader.GetValue(num);


                                if (!(_objvalue is DBNull))
                                {

                                    Type propertyType = properties[num].PropertyType;

                                    //不是实体类
                                    if (((((propertyType == typeof(decimal)) || (propertyType == typeof(int))) || ((propertyType == typeof(bool)) || (propertyType == typeof(long)))) || ((propertyType == typeof(double)) || (propertyType == typeof(float)))) || (propertyType == typeof(byte)))
                                    {
                                        objArray2[num] = OperateCommon.AutoConvert(_objvalue, propertyType);
                                    }
                                    else
                                    {
                                        objArray2[num] = _objvalue;
                                    }




                                }
                            }
                            obj2 = context.Constructor.Invoke(objArray2);
                            list.Add((T)obj2);
                        }
                    }
                   
                }


                return list;
            };
        }


        /// <summary>
        ///  获取实体信息 
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static EntityInfo GetEntityInfo(Type entityType)
        {
           // Console.WriteLine("连接AddEntity" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
            if (GlobalCache.EntityInfoPool.Keys.Contains(entityType))
            {
                return GlobalCache.EntityInfoPool[entityType];
            }
            EntityInfo info = new EntityInfo
            {
                EntityType = entityType
            };
            foreach (object obj2 in entityType.GetCustomAttributes(typeof(EntityAttribute), false))
            {
                info.IsEntity = true;
                info.Entity = obj2 as EntityAttribute;
            }
            Dictionary<string, PropertyMap> dictionary = new Dictionary<string, PropertyMap>();
            Dictionary<string, FieldMap> dictionary2 = new Dictionary<string, FieldMap>();
            foreach (PropertyInfo info2 in entityType.GetProperties())
            {
                MethodInfo method;
                if (dictionary.ContainsKey(info2.Name))
                {
                    throw new ApplicationException(string.Format("不能添加重复键:{0}", info2.Name));
                }
                PropertyMap map = new PropertyMap
                {
                    Info = info2
                };
                if (info2.CanRead)
                {
                   method = entityType.GetMethod(string.Format("get_{0}", info2.Name));
                    map.Get = CreateGetFunction(method, entityType);

                    // map.Get = EntifyRf.CreateGetter(info2);//
                }
                if (info2.CanWrite)
                {
                    method = entityType.GetMethod(string.Format("set_{0}", info2.Name));
                    map.Set = CreateSetAction(method, entityType);
                   // map.Set = EntifyRf.CreateSetter(info2);//
                }
                foreach (object obj2 in info2.GetCustomAttributes(typeof(AutoConvertAttribute), true))
                {
                    AutoConvertAttribute attribute = obj2 as AutoConvertAttribute;
                    if (attribute != null)
                    {
                        map.AutoConvert = true;
                        break;
                    }
                }
                dictionary.Add(info2.Name, map);
                if (info.IsEntity && (info2.GetCustomAttributes(typeof(FieldAttribute), true).Length > 0))
                {
                    FieldAttribute attribute2 = info2.GetCustomAttributes(typeof(FieldAttribute), true)[0] as FieldAttribute;
                    FieldMap map2 = new FieldMap
                    {
                        Property = map
                    };
                    map.FieldMap = map2;
                    foreach (object obj2 in info2.GetCustomAttributes(true))
                    {
                        FieldAttribute attribute3 = obj2 as FieldAttribute;
                        if (attribute3 != null)
                        {
                            map2.Field = attribute3;
                            if (attribute3.IsPrimaryKey)
                            {
                                info.PrimaryKeys.Add(attribute3.Name);
                            }

                            if (attribute3.IsIdenty)
                            {
                                 // map2.Identity = attribute3;
                                 info.Autorecode = info2.Name;
                            }

                        }
                        else
                        {
                            EncryptAttribute attribute4 = obj2 as EncryptAttribute;
                            if (attribute4 != null)
                            {
                                map2.Encrypt = attribute4;
                                info.IsEncrypt = true;
                            }
                            else
                            {
                                IdentityAttribute attribute5 = obj2 as IdentityAttribute;
                                if (attribute5 != null)
                                {
                                    if (!attribute2.IsPrimaryKey)
                                    {
                                        throw new ApplicationException("自增特性只能挂在主键列上!");
                                    }
                                   // map2.Identity = attribute5;
                                    info.Autorecode = info2.Name;
                                }
                                else
                                {
                                    SequenceAttribute attribute6 = obj2 as SequenceAttribute;
                                    if (attribute6 != null)
                                    {
                                        if (!attribute2.IsPrimaryKey)
                                        {
                                            throw new ApplicationException("自增特性只能挂在主键列上!");
                                        }
                                        map2.Sequence = attribute6;
                                        info.Autorecode = info2.Name;
                                    }
                                    else
                                    {
                                        OracleTypeAttribute attribute7 = obj2 as OracleTypeAttribute;
                                        if (attribute7 != null)
                                        {
                                            map2.OracleType = attribute7;
                                        }
                                        else
                                        {
                                            SqlServerTypeAttribute attribute8 = obj2 as SqlServerTypeAttribute;
                                            if (attribute8 != null)
                                            {
                                                map2.SqlType = attribute8;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    dictionary2.Add(map2.Field.Name, map2);
                }
            }
            info.Propertys = dictionary;
            info.Fields = dictionary2;
            GlobalCache.AddEntity(entityType, info);
          //  Console.WriteLine("连接时间AddEntity" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
            return info;
        }

        /// <summary>
        /// 获取多实体信息
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static MultEntityInfo GetMultEntityInfo(Type entityType)
        {
            Dictionary<string, MultFieldInfo> dictionary = new Dictionary<string, MultFieldInfo>();
            foreach (PropertyInfo info in entityType.GetProperties())
            {
                foreach (object obj2 in info.GetCustomAttributes(true))
                {
                    MultFieldAttribute attribute = obj2 as MultFieldAttribute;
                    if (attribute != null)
                    {
                        MultFieldInfo info2 = new MultFieldInfo();
                        EntityInfo entityInfo = GetEntityInfo(attribute.EntityType);
                        if (!entityInfo.IsEntity)
                        {
                            throw new ArgumentException(info.Name + "属性联合类型不是实体类型");
                        }
                        info2.TableName = entityInfo.Entity.Name;
                        if (!entityInfo.Propertys.Keys.Contains(attribute.Name))
                        {
                            throw new ApplicationException(string.Format("找不到名称为{0}的属性", attribute.Name));
                        }
                        info2.FieldName = entityInfo.Propertys[attribute.Name].FieldMap.Field.Name;
                        info2.Info = info;
                        dictionary.Add(info.Name, info2);
                    }
                }
            }
            return new MultEntityInfo { Propertys = dictionary };
        }

        public static bool IsEntity(Type type)
        {
            return type.GetCustomAttributes(typeof(EntityAttribute), true).Any<object>();
        }
    }
}
