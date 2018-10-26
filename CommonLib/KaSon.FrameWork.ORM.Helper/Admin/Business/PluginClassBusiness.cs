using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    public class PluginClassBusiness
    {
        /// <summary>
        /// 添加插件类信息
        /// </summary>
        public void AddPluginClass(C_Activity_PluginClass pluginClass)
        {
            //using (var biz = new GameBizBusinessManagement())
            //{
            //    biz.BeginTran();

            var manager = new PluginClassManager();
            //var entity = new C_Activity_PluginClass()
            //{
            //    AssemblyFileName = pluginClass.AssemblyFileName,
            //    ClassName = pluginClass.ClassName,
            //    EndTime = pluginClass.EndTime,
            //    InterfaceName = pluginClass.InterfaceName,
            //    IsEnable = pluginClass.IsEnable,
            //    OrderIndex = pluginClass.OrderIndex,
            //    StartTime = pluginClass.StartTime
            //};
            manager.AddPluginClass(pluginClass);

            //    biz.CommitTran();
            //}

            //BusinessHelper.ClearPlugin();
        }

        /// <summary>
        /// 更新插件信息
        /// </summary>
        public void UpdatePluginClass(C_Activity_PluginClass pluginClass)
        {
            //using (var biz = new GameBizBusinessManagement())
            //{
            //    biz.BeginTran();

            var manager = new PluginClassManager();
            var entity = manager.QueryPluginClassById(pluginClass.Id);

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

            //    biz.CommitTran();
            //}
            //BusinessHelper.ClearPlugin();
        }

        /// <summary>
        /// 删除插件信息
        /// </summary>
        public void DeletePluginClass(int id)
        {
            var manager = new PluginClassManager();
            var entity = manager.QueryPluginClassById(id);
            manager.DeletePluginClass(entity);
            //BusinessHelper.ClearPlugin();
        }

        /// <summary>
        /// 查询某个插件信息
        /// </summary>
        public C_Activity_PluginClass PluginClassInfoById(int id)
        {
            var entity = new PluginClassManager().QueryPluginClassById(id);
            if (entity == null)
                throw new Exception("插件信息未被查询到");
            return entity;
        }

        /// <summary>
        /// 查询插件列表
        /// </summary>
        public PluginClassInfo_Collection QueryPluginClassCollection(string interfaceName, int pageIndex, int pageSize)
        {
            //using ()
            //{
            var manage = new PluginClassManager();
            PluginClassInfo_Collection colleciton = new PluginClassInfo_Collection();
            int totalCount;
            colleciton.PluginClassList = manage.QueryPluginClassByInterfaceName(interfaceName, pageIndex, pageSize, out totalCount);
            colleciton.TotalCount = totalCount;
            return colleciton;
            //}
        }

        /// <summary>
        /// 查询插件列表
        /// </summary>
        public PluginClassInfo_Collection QueryPluginClassList(int pageIndex, int pageSize)
        {
            //using (var manage = new PluginClassManager())
            //{
            var manage = new PluginClassManager();
            PluginClassInfo_Collection colleciton = new PluginClassInfo_Collection();
            int totalCount;
            colleciton.PluginClassList = manage.QueryPluginClassList(pageIndex, pageSize, out totalCount);
            colleciton.TotalCount = totalCount;
            return colleciton;
            //}
        }
    }
}
