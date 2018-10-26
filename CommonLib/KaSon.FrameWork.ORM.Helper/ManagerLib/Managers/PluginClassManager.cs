using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// kason
    /// </summary>
   public class PluginClassManager : DBbase
    {
        public List<C_JCLQ_MatchResult> QueryJCLQMatchResultByDay(int day)
        {
           // Session.Clear();
            var query = from r in DB.CreateQuery<C_JCLQ_MatchResult>()
                        where r.CreateTime > DateTime.Now.AddDays(day)
                        && r.MatchState == "2"
                        //&& (r.SF_SP != 1M && r.RFSF_SP != 1M && r.SFC_SP != 1M && r.DXF_SP != 1M )
                        select r;
            return query.ToList();
        }
        public List<C_Activity_PluginClass> QueryPluginClass(bool isEnable)
        {
          //  this.Session.Clear();
            return this.DB.CreateQuery<C_Activity_PluginClass>().Where(p => p.IsEnable == isEnable).OrderBy(p => p.OrderIndex).ToList();
        }

        /// <summary>
        /// 按接口名查询插件列表  modify-这个方法要改进
        /// </summary>
        public List<C_Activity_PluginClass> QueryPluginClassByInterfaceName(string interfaceName, int pageIndex, int pageSize, out int totalCount)
        {
           // Session.Clear();
            var query = from a in DB.CreateQuery<C_Activity_PluginClass>()
                        where (a.InterfaceName == interfaceName)
                        select a;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public void AddPluginClass(C_Activity_PluginClass pluginClass)
        {
            DB.GetDal<C_Activity_PluginClass>().Add(pluginClass);
        }

        public C_Activity_PluginClass QueryPluginClassById(int id)
        {
            return DB.CreateQuery<C_Activity_PluginClass>().Where(p => p.Id == id).FirstOrDefault();
        }

        public void UpdatePluginClass(C_Activity_PluginClass pluginClass)
        {
            DB.GetDal<C_Activity_PluginClass>().Update(pluginClass);
        }

        public void DeletePluginClass(C_Activity_PluginClass pluginClass)
        {
            DB.GetDal<C_Activity_PluginClass>().Delete(pluginClass);
        }

        /// <summary>
        /// 查询插件列表
        /// </summary>
        public List<C_Activity_PluginClass> QueryPluginClassList(int pageIndex, int pageSize, out int totalCount)
        {
            var query = DB.CreateQuery<C_Activity_PluginClass>();
            totalCount = query.ToList().Count;
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
