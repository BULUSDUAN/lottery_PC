using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using GameBiz.Domain.Managers;
using GameBiz.Domain.Entities;

namespace GameBiz.Business
{

    /// <summary>
    /// 插件类
    /// </summary>
    public class PluginClassBusiness
    {
        /// <summary>
        /// 添加插件类信息
        /// </summary>
        public void AddPluginClass(PluginClassInfo pluginClass)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new PluginClassManager();
                var entity = new PluginClass()
                {
                    AssemblyFileName = pluginClass.AssemblyFileName,
                    ClassName = pluginClass.ClassName,
                    EndTime = pluginClass.EndTime,
                    InterfaceName = pluginClass.InterfaceName,
                    IsEnable = pluginClass.IsEnable,
                    OrderIndex = pluginClass.OrderIndex,
                    StartTime = pluginClass.StartTime,
                };
                manager.AddPluginClass(entity);

                biz.CommitTran();
            }

            BusinessHelper.ClearPlugin();
        }

        /// <summary>
        /// 更新插件信息
        /// </summary>
        public void UpdatePluginClass(PluginClassInfo pluginClass, int id)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new PluginClassManager();
                var entity = manager.QueryPluginClassById(id);

                if (entity == null)
                {
                    throw new Exception("插件信息未被查询到");
                }
                entity.AssemblyFileName = pluginClass.AssemblyFileName;
                entity.ClassName = pluginClass.ClassName;
                entity.EndTime = pluginClass.EndTime;
                entity.InterfaceName = pluginClass.InterfaceName;
                entity.IsEnable = pluginClass.IsEnable;
                entity.OrderIndex = pluginClass.OrderIndex;
                entity.StartTime = pluginClass.StartTime;

                manager.UpdatePluginClass(entity);

                biz.CommitTran();
            }
            BusinessHelper.ClearPlugin();
        }

        /// <summary>
        /// 删除插件信息
        /// </summary>
        public void DeletePluginClass(int id)
        {
            var manager = new PluginClassManager();
            var entity = manager.QueryPluginClassById(id);
            manager.DeletePluginClass(entity);
            BusinessHelper.ClearPlugin();
        }

        /// <summary>
        /// 查询某个插件信息
        /// </summary>
        public PluginClassInfo PluginClassInfoById(int id)
        {
            var entity = new PluginClassManager().QueryPluginClassById(id);
            if (entity == null)
                throw new Exception("插件信息未被查询到");
            return new PluginClassInfo()
            {
                AssemblyFileName = entity.AssemblyFileName,
                ClassName = entity.ClassName,
                EndTime = entity.EndTime,
                InterfaceName = entity.InterfaceName,
                IsEnable = entity.IsEnable,
                OrderIndex = entity.OrderIndex,
                StartTime = entity.StartTime,
                Id = entity.Id,
            };
        }

        /// <summary>
        /// 查询插件列表
        /// </summary>
        public PluginClassInfo_Collection QueryPluginClassCollection(string interfaceName, int pageIndex, int pageSize)
        {
            using (var manage = new PluginClassManager())
            {
                PluginClassInfo_Collection colleciton = new PluginClassInfo_Collection();
                int totalCount;
                colleciton.PluginClassList = manage.QueryPluginClassByInterfaceName(interfaceName, pageIndex, pageSize, out totalCount);
                colleciton.TotalCount = totalCount;
                return colleciton;
            }
        }

        /// <summary>
        /// 查询插件列表
        /// </summary>
        public PluginClassInfo_Collection QueryPluginClassList(int pageIndex, int pageSize)
        {
            using (var manage = new PluginClassManager())
            {
                PluginClassInfo_Collection colleciton = new PluginClassInfo_Collection();
                int totalCount;
                colleciton.PluginClassList = manage.QueryPluginClassList(pageIndex, pageSize, out totalCount);
                colleciton.TotalCount = totalCount;
                return colleciton;
            }
        }
    }
}
