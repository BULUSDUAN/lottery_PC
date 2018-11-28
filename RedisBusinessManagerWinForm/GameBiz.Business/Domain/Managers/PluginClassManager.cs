using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using GameBiz.Domain.Entities;
using NHibernate.Linq;
using GameBiz.Core;

namespace GameBiz.Domain.Managers
{
    public class PluginClassManager : GameBizEntityManagement
    {
        public void AddPluginClass(PluginClass entity)
        {
            this.Add(entity);
        }

        public void UpdatePluginClass(PluginClass entity)
        {
            this.Update(entity);
        }

        public void DeletePluginClass(PluginClass entity)
        {
            this.Delete(entity);
        }

        /// <summary>
        /// 按接口名查询插件列表
        /// </summary>
        public List<PluginClassInfo> QueryPluginClassByInterfaceName(string interfaceName, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            var query = from a in Session.QueryOver<PluginClass>().List()
                        where (a.InterfaceName == interfaceName)
                        select new PluginClassInfo
                        {
                            Id = a.Id,
                            AssemblyFileName = a.AssemblyFileName,
                            ClassName = a.ClassName,
                            EndTime = a.EndTime,
                            InterfaceName = a.InterfaceName,
                            IsEnable = a.IsEnable,
                            OrderIndex = a.OrderIndex,
                            StartTime = a.StartTime,
                        };
            totalCount = query.ToList().Count;
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 查询插件列表
        /// </summary>
        public List<PluginClassInfo> QueryPluginClassList(int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            var query = from a in Session.QueryOver<PluginClass>().List()
                        select new PluginClassInfo
                        {
                            Id = a.Id,
                            AssemblyFileName = a.AssemblyFileName,
                            ClassName = a.ClassName,
                            EndTime = a.EndTime,
                            InterfaceName = a.InterfaceName,
                            IsEnable = a.IsEnable,
                            OrderIndex = a.OrderIndex,
                            StartTime = a.StartTime,
                        };
            totalCount = query.ToList().Count;
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        ///// <summary>
        ///// 按是否启用查询插件列表
        ///// </summary>
        //public List<PluginClassInfo> QueryPluginClassByIsEnable(bool isEnable, int pageIndex, int pageSize, out int totalCount)
        //{
        //    Session.Clear();
        //    var query = from a in Session.QueryOver<PluginClass>().List()
        //                where (a.IsEnable == isEnable)
        //                select new PluginClassInfo
        //                {
        //                    AssemblyFileName = a.AssemblyFileName,
        //                    ClassName = a.ClassName,
        //                    EndTime = a.EndTime,
        //                    InterfaceName = a.InterfaceName,
        //                    IsEnable = a.IsEnable,
        //                    OrderIndex = a.OrderIndex,
        //                    StartTime = a.StartTime,
        //                };
        //    totalCount = query.ToList().Count;
        //    return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        //}

        public List<PluginClass> QueryPluginClass(bool isEnable)
        {
            this.Session.Clear();
            return this.Session.Query<PluginClass>().Where(p => p.IsEnable == isEnable).OrderBy(p => p.OrderIndex).ToList();
        }

        public PluginClass QueryPluginClassById(int id)
        {
            this.Session.Clear();
            return this.Session.Query<PluginClass>().FirstOrDefault(p => p.Id == id);
        }

    }
}
