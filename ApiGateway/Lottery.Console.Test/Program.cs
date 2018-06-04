using System;
using System.Reflection;
using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Data;

namespace Lottery.Console.Test
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EncryptAttribute : System.Attribute
    {
    }
    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FieldAttribute : System.Attribute
    {
        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        public FieldAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 是否能空
        /// </summary>
        public bool CanNull { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否默认值
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsIdenty { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 精度，限数值，小数点后面位数
        /// </summary>
        public int Precision { get; set; }
    }

    /// <summary>
    /// SqlServer 自增ID
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IdentityAttribute : System.Attribute
    {
    }
    /// <summary>
    /// Oracle序列号
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SequenceAttribute : System.Attribute
    {
        public SequenceAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 函数名称
        /// </summary>
        public string Name { get; private set; }
    }
    public enum EntityType
    {
        Table,
        View
    }
    /// <summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EntityAttribute : System.Attribute
    {
        private EntityType _type = EntityType.Table;

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        public EntityAttribute(string name)
        {
            this.Name = name;
            this.EmptyIsUpdate = true;
        }
        public EntityAttribute(string name, bool emptyIsUpdate)
        {
            this.Name = name;
            this.EmptyIsUpdate = emptyIsUpdate;
        }
        /// <summary>
        /// 实体对应的表名/视图名
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        /// 整个实体更新时，是否空字段要更新
        /// </summary>
        public bool EmptyIsUpdate { get; set; }

        /// <summary>
        /// 实体类型
        /// </summary>
        public EntityType Type
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }
    }
    internal class FieldMap
    {
        public EncryptAttribute Encrypt { get; set; }

        public FieldAttribute Field { get; set; }


   

        public PropertyMap Property { get; set; }

        public SequenceAttribute Sequence { get; set; }

    

    
    }
    internal class PropertyMap
    {
        private Func<object, object> _get = null;
        private Action<object, object> _set = null;

        public bool AutoConvert { get; set; }

        public FieldMap FieldMap { get; set; }

        public Func<object, object> Get
        {
            get
            {
                return this._get;
            }
            set
            {
                this._get = delegate (object entity)
                {
                    object obj2;
                    try
                    {
                        obj2 = value(entity);
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("获取字段" + this.Info.Name + "的值时发生错误", exception);
                    }
                    return obj2;
                };
            }
        }

        public PropertyInfo Info { get; set; }
        public static object _AutoConvert(object value, Type type)
        {
            if (value == null)
            {
                return null;
            }
            Type type2 = value.GetType();
            if (type.IsEnum)
            {
                type = typeof(int);
            }
            if (type == type2)
            {
                return value;
            }
            string str = Convert.ToString(value);
            if (type.Name == "String")
            {
                return str;
            }
            if (type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }
            if (str.Length == 0)
            {
                if (type == typeof(bool))
                {
                    return false;
                }
                if (type == typeof(DateTime))
                {
                    return null;
                }
                if (type == typeof(int))
                {
                    return 0;
                }
                if (type == typeof(long))
                {
                    return 0L;
                }
                if (type == typeof(decimal))
                {
                    return 0M;
                }
                if (type == typeof(double))
                {
                    return 0.0;
                }
            }
            if (((type == typeof(int)) || (type == typeof(long))) || (type == typeof(byte)))
            {
                return type.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { string.Format("{0:0}", value) });
            }
            if ((type == typeof(bool)) && (((((type2 == typeof(byte)) || (type2 == typeof(int))) || ((type2 == typeof(decimal)) || (type2 == typeof(float)))) || (type2 == typeof(float))) || (type2 == typeof(long))))
            {
                return (Convert.ToInt64(string.Format("{0:0}", value)) != 0L);
            }
            if (type == typeof(bool))
            {
                return (str.ToUpper().Equals("FALSE") ? ((object)0) : ((object)!str.Equals("0")));
            }
            return type.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { str });
        }
        public Action<object, object> Set
        {
            get
            {
                return this._set;
            }
            set
            {
                this._set = delegate (object entity, object obj)
                {
                    try
                    {
                        value(entity, _AutoConvert(obj, this.Info.PropertyType));
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("设置字段" + this.Info.Name + "的值时发生错误", exception);
                    }
                };
            }
        }
    }
    internal class EntityInfo
    {
        private readonly IList<string> _primaryKeys = new List<string>();

        /// <summary>
        /// 自动递增键
        /// </summary>
        public string Autorecode { get; set; }
        
        public EntityAttribute Entity { get; set; }

        public Type EntityType { get; set; }

        public IDictionary<string, FieldMap> Fields { get; set; }

        public bool IsEncrypt { get; set; }

        public bool IsEntity { get; set; }

        public IList<string> PrimaryKeys
        {
            get
            {
                return this._primaryKeys;
            }
        }

        public IDictionary<string, PropertyMap> Propertys { get; set; }
    }
    class Program
    {
        private static readonly Type ObjectType = typeof(object);
        private static readonly IDictionary<Type, DbType> TpyeToDbType;
        private static readonly IDictionary<Type, OpCode> ValueTpyeOpCodes;
        #region  Get Set 对象属性方法 
        /// <summary>
        /// 获取 Get 方法 
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        static Program()
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

          //  var tb = GetTypeBuilder();

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

        private static bool IsParameterMatch(ParameterInfo[] x, ParameterInfo[] y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            if (x.Length != y.Length) return false;
            for (int i = 0; i < x.Length; i++)
                if (x[i].ParameterType != y[i].ParameterType) return false;
            return true;
        }
        MethodInfo GetPropertySetter(PropertyInfo propertyInfo, Type type)
        {
            if (propertyInfo.DeclaringType == type) return propertyInfo.GetSetMethod(true);

          var get=  propertyInfo.DeclaringType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Single(x => x.Name == propertyInfo.Name
                        && x.PropertyType == propertyInfo.PropertyType
                    //   && IsParameterMatch(x.GetIndexParameters(), propertyInfo.GetIndexParameters())
                        ).GetGetMethod();
            


            return propertyInfo.DeclaringType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Single(x => x.Name == propertyInfo.Name
                        && x.PropertyType == propertyInfo.PropertyType
                        && IsParameterMatch(x.GetIndexParameters(), propertyInfo.GetIndexParameters())
                        ).GetSetMethod(true);

        }

        public static EntityInfo GetEntityInfo(Type entityType)
        {
            // Console.WriteLine("连接AddEntity" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
        
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


                    map.Get = Program.CreateGetFunction(method, entityType);
                }
                if (info2.CanWrite)
                {
                    method = entityType.GetMethod(string.Format("set_{0}", info2.Name));
                    map.Set =  CreateSetAction(method, entityType);
                }
              
                dictionary.Add(info2.Name, map);
               
            }
            info.Propertys = dictionary;
            info.Fields = dictionary2;
           // GlobalCache.AddEntity(entityType, info);
            //  Console.WriteLine("连接时间AddEntity" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
            return info;
        }
        static void Main(string[] args)
        {
            TestModel test = new TestModel();

            //object obj2 = Activator.CreateInstance(typeof(T));

            EntityInfo ent =  GetEntityInfo(test.GetType());

            System.Console.WriteLine(ent.Propertys.Keys);

            var pro = ent.Propertys["name"];
            object obj = "123456123132";
            pro.Set(test, obj);
            System.Console.WriteLine(pro.Get(test));

            Type testType = typeof(TestModel);

            System.Console.ReadKey(true);

           System.Console.WriteLine("Hello World!");
        }
    }
}
