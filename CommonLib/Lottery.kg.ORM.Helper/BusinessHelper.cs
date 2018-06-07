using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace Lottery.Kg.ORM.Helper
{
    public class BusinessHelper:DBbase
    {
       
        private static List<C_Activity_PluginClass> _enablePluginClass = new List<C_Activity_PluginClass>();


        public  void ExecPlugin<T>(object inputParam) where T : class, IPlugin
        {
            if (_enablePluginClass == null || _enablePluginClass.Count == 0)
                _enablePluginClass = QueryPluginClass(true);


            foreach (var plugin in _enablePluginClass)
            {
                try
                {
                    if (typeof(T).FullName != plugin.InterfaceName) continue;

                    if (plugin.StartTime > DateTime.Now) continue;//未开始
                    if (plugin.EndTime < DateTime.Now) continue;//已结束

                    var fullName = plugin.ClassName + "," + plugin.AssemblyFileName;
                    var type = Type.GetType(fullName);
                    if (type == null)
                    {
                        throw new ArgumentNullException("类型在当前域中不存在，或对应组件未加载：" + fullName);
                    }
                    var i = Type.GetType(fullName).GetConstructor(Type.EmptyTypes).Invoke(new object[0]) as T;
                    if (i == null)
                    {
                        throw new ArgumentNullException("无法实例化对象：" + fullName);
                    }
                    //new Thread(() =>
                    //{
                    try
                    {
                        i.ExecPlugin(typeof(T).Name, inputParam);
                    }
                    catch (AggregateException ex)
                    {
                        throw new AggregateException(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                        //writer.Write("ERROR_ExecPlugin", "_ExecPlugin", Common.Log.LogType.Error, string.Format("执行插件{0}出错", plugin.ClassName), ex.ToString());
                    }
                    //}).Start();
                }
                catch (AggregateException ex)
                {
                    throw new AggregateException(ex.Message);
                }
                catch (Exception ex)
                {
                    //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                    //writer.Write("ERROR_ExecPlugin", "_ExecPlugin", Common.Log.LogType.Error, string.Format("执行插件{0}出错", plugin.ClassName), ex.ToString());
                }
            }

        }
        public interface IPlugin
        {
            object ExecPlugin(string type, object inputParam);
        }


        public List<C_Activity_PluginClass> QueryPluginClass(bool isEnable)
        {

            return DB.CreateQuery<C_Activity_PluginClass>().Where(p => p.IsEnable == isEnable).OrderBy(p => p.OrderIndex).ToList();
        }
    }
}
