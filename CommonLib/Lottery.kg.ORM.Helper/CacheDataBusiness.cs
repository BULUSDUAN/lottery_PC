using EntityModel;
using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper
{
   public class CacheDataBusiness:DBbase
    {
       


        public C_Core_Config QueryCoreConfigByKey(string key)
        {       
            var config = DB.CreateQuery<C_Core_Config>().FirstOrDefault(p => p.ConfigKey == key);
            if (config == null)
                throw new Exception(string.Format("找不到配置项：{0}", key));
            return config;
        }

        /// <summary>
        /// 从Redis中查询系统配置
        /// </summary>
        public string QueryCoreConfigFromRedis(string key)
        {
            var config = DB.CreateQuery<C_Core_Config>().FirstOrDefault(p => p.ConfigKey == key);
            if (config == null)
                throw new Exception(string.Format("找不到配置项：{0}", key));
            return config.ConfigValue;
        }
    }
}
