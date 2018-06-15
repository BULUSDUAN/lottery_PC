using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.Helper.分析器工厂;
namespace KaSon.FrameWork.Helper
{
    /// <summary>
    /// 对象转换器。用于领域层与服务层之间对象的转换
    /// </summary>
    public static class ObjectConvert
    {
        /// <summary>
        /// 将服务层的Info对象转换为Entity实体对象
        /// </summary>
        /// <typeparam name="TInfo">服务层的Info类型</typeparam>
        /// <typeparam name="TEntity">Entity实体类型</typeparam>
        /// <param name="info">服务层的Info对象，不能为null</param>
        /// <param name="entity">Entity实体对象，不能为null</param>
        public static void ConverInfoToEntity<TInfo, TEntity>(TInfo info, ref TEntity entity)
            where TInfo : class
            where TEntity : class
        {
            PreconditionAssert.IsNotNull(info, "转换的源Info对象不能为null");
            PreconditionAssert.IsNotNull(entity, "转换的目标Entity对象不能为null");

            var typeInfo = typeof(TInfo);
            var typeEntity = typeof(TEntity);
            foreach (var prop in typeInfo.GetProperties())
            {
                // 如果定义了映射标签
                if (prop.IsDefined(typeof(ConvertDeeplyMappingAttribute), false))
                {
                    var attr = prop.GetCustomAttributes(typeof(ConvertDeeplyMappingAttribute), false)[0] as ConvertDeeplyMappingAttribute;
                    var mappings = attr.MappingName.Split('.');
                    if (mappings.Length == 1)
                    {
                        var entityProp = typeEntity.GetProperty(mappings[0]);
                        if (entityProp == null)
                        {
                            throw new Exception("未找到映射的属性" + mappings[0]);
                        }
                        var value = prop.GetValue(info, null);
                        entityProp.SetValue(entity, value, null);
                    }
                }
                else
                {
                    var entityProp = typeEntity.GetProperty(prop.Name);
                    if (entityProp != null)
                    {
                        if (prop.PropertyType == entityProp.PropertyType)
                        {
                            var value = prop.GetValue(info, null);
                            entityProp.SetValue(entity, value, null);
                        }
                        else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            if (Nullable.GetUnderlyingType(prop.PropertyType) == entityProp.PropertyType)
                            {
                                var value = prop.GetValue(info, null);
                                if (value != null)
                                {
                                    entityProp.SetValue(entity, value, null);
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 将Entity实体对象转换为服务层的Info对象
        /// </summary>
        /// <typeparam name="TEntity">Entity实体类型</typeparam>
        /// <typeparam name="TInfo">服务层的Info类型</typeparam>
        /// <param name="entity">Entity实体对象，不能为null</param>
        /// <param name="info">服务层的Info对象，不能为null</param>
        public static void ConverEntityToInfo<TEntity, TInfo>(TEntity entity, ref TInfo info)
            where TEntity : class
            where TInfo : class
        {
            PreconditionAssert.IsNotNull(entity, "转换的源Entity对象不能为null");
            PreconditionAssert.IsNotNull(info, "转换的目标Info对象不能为null");

            var typeInfo = typeof(TInfo);
            var typeEntity = typeof(TEntity);
            foreach (var prop in typeInfo.GetProperties())
            {
                // 如果定义了映射标签
                if (prop.IsDefined(typeof(ConvertDeeplyMappingAttribute), false))
                {
                    var attr = prop.GetCustomAttributes(typeof(ConvertDeeplyMappingAttribute), false)[0] as ConvertDeeplyMappingAttribute;
                    var mappings = attr.MappingName.Split('.');
                    if (mappings.Length < 1)
                    {
                        throw new Exception("深度映射定义错误");
                    }
                    var value = GetDeeplyPropertyValue(typeEntity, entity, mappings);
                    prop.SetValue(info, value, null);
                }
                else
                {
                    var entityProp = typeEntity.GetProperty(prop.Name);
                    if (entityProp != null)
                    {
                        if (prop.PropertyType == entityProp.PropertyType)
                        {
                            var value = entityProp.GetValue(entity, null);
                            prop.SetValue(info, value, null);
                        }
                        else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            if (Nullable.GetUnderlyingType(prop.PropertyType) == entityProp.PropertyType)
                            {
                                var value = entityProp.GetValue(entity, null);
                                prop.SetValue(info, value, null);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 将服务层的Info对象列表转换为Entity实体对象
        /// </summary>
        /// <typeparam name="TInfoList">Info列表类型</typeparam>
        /// <typeparam name="TInfo">服务层的Info类型</typeparam>
        /// <typeparam name="TEntityList">Entity列表类型</typeparam>
        /// <typeparam name="TEntity">Entity实体类型</typeparam>
        /// <param name="infoList">Info对象列表</param>
        /// <param name="entityList">Entity实体列表</param>
        /// <param name="createEntityHandler">创建Entity对象的句柄</param>
        /// <param name="manualHandler">手工处理转换的句柄</param>
        public static void ConvertInfoListToEntityList<TInfoList, TInfo, TEntityList, TEntity>(TInfoList infoList, ref TEntityList entityList, Func<TEntity> createEntityHandler, Action<TInfo, TEntity> manualHandler = null)
            where TInfoList : IList<TInfo>
            where TInfo : class
            where TEntityList : IList<TEntity>
            where TEntity : class
        {
            foreach (var info in infoList)
            {
                var entity = createEntityHandler();
                ConverInfoToEntity<TInfo, TEntity>(info, ref entity);
                if (manualHandler != null)
                {
                    manualHandler(info, entity);
                }
                entityList.Add(entity);
            }
        }
        /// <summary>
        /// 将Entity实体对象列表转换为服务层的Info对象
        /// </summary>
        /// <typeparam name="TEntityList">Entity列表类型</typeparam>
        /// <typeparam name="TEntity">Entity实体类型</typeparam>
        /// <typeparam name="TInfoList">Info列表类型</typeparam>
        /// <typeparam name="TInfo">服务层的Info类型</typeparam>
        /// <param name="entityList">Entity实体列表</param>
        /// <param name="infoList">Info对象列表</param>
        /// <param name="createInfoHandler">创建Info对象的句柄</param>
        /// <param name="manualHandler">手工处理转换的句柄</param>
        public static void ConvertEntityListToInfoList<TEntityList, TEntity, TInfoList, TInfo>(TEntityList entityList, ref TInfoList infoList, Func<TInfo> createInfoHandler, Action<TEntity, TInfo> manualHandler = null)
            where TEntityList : IList<TEntity>
            where TEntity : class
            where TInfoList : IList<TInfo>
            where TInfo : class
        {
            foreach (var entity in entityList)
            {
                var info = createInfoHandler();
                ConverEntityToInfo<TEntity, TInfo>(entity, ref info);
                if (manualHandler != null)
                {
                    manualHandler(entity, info);
                }
                infoList.Add(info);
            }
        }
        private static object GetDeeplyPropertyValue(Type entityType, object entity, string[] mappings)
        {
            var mapping = mappings[0];
            var prop = entityType.GetProperty(mapping);
            if (prop == null)
            {
                throw new Exception(string.Format("对象{0}中不存在名称为{1}的属性。路径：{2}", entityType.FullName, mapping, string.Join(".", mappings)));
            }
            if (entity == null)
            {
                return null;
            }
            var value = prop.GetValue(entity, null);
            var tmp = mappings.Skip(1).ToArray();
            if (tmp.Length == 0)
            {
                return value;
            }
            return GetDeeplyPropertyValue(prop.PropertyType, value, tmp);
        }
    }
}