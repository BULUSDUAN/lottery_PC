using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSRedis
{
    public class CSRedisConfig
    {
        public string C_IP { get; set; } = "127.0.0.1";
        public int C_Post { get; set; } = 6379;
        public string C_Password { get; set; } = "";
        public int C_Defaultdatabase { get; set; } = 0;
        public int C_PoolSize { get; set; } = 1000;
        public int c_Writebuffer { get; set; } = 20480;
        public string C_Prefix { get; set; } = "";
        public bool C_SSL { get; set; } = false;

    }

    public partial class CSRedisClient
    {
        /// <summary>
        /// kason 配置Redis
        /// </summary>
        /// <param name="list"></param>
        public CSRedisClient(IList<CSRedisConfig> list)
        {
            //ClusterRule = null;
            //if (ClusterRule == null) ClusterRule = key => {
            //    var idx = Math.Abs(string.Concat(key).GetHashCode()) % ClusterNodes.Count;
            //    return idx < 0 || idx >= ClusterKeys.Count ? ClusterKeys.First() : ClusterKeys[idx];
            //};

            ////if (connectionStrings == null || connectionStrings.Any() == false) throw new Exception("Redis ConnectionString 未设置");
            //foreach (var item in list)
            //{
            //    var pool = new ConnectionPool();
            //    pool.ConnectionStringEx(item.C_IP, item.C_Post, item.C_Password, item.C_Defaultdatabase, item.c_Writebuffer, item.C_PoolSize, item.C_SSL, item.C_Prefix);
            //    pool.Connected += (s, o) => {
            //        RedisClient rc = s as RedisClient;
            //    };
            //    if (ClusterNodes.ContainsKey(pool.ClusterKey)) throw new Exception($"ClusterName: {pool.ClusterKey} 重复，请检查");
            //    ClusterNodes.Add(pool.ClusterKey, pool);
            //}
            //ClusterKeys = ClusterNodes.Keys.ToList();
            //_clusterKeys = ClusterNodes.Keys.ToList();
            ////   if (connectionStrings == null || connectionStrings.Any() == false) throw new Exception("Redis ConnectionString 未设置");
            //var pool = new ConnectionPool();
            //pool.ConnectionStringEx(ex_ip, ex_port, ex_password, ex_defaultdatabase, ex_writebuffer, ex_poolsize, ex_ssl, ex_Prefix);
            //pool.Connected += (s, o) => {
            //    RedisClient rc = s as RedisClient;
            //};
            //if (ClusterNodes.ContainsKey(pool.ClusterKey)) throw new Exception($"ClusterName: {pool.ClusterKey} 重复，请检查");
            //ClusterNodes.Add(pool.ClusterKey, pool);
            //_clusterKeys = ClusterNodes.Keys.ToList();
        }


        public CSRedisClient(CSRedisConfig cSRedisConfig)
        {
            //ClusterRule = null;
            //if (ClusterRule == null) ClusterRule = key => {
            //    var idx = Math.Abs(string.Concat(key).GetHashCode()) % ClusterNodes.Count;
            //    return idx < 0 || idx >= ClusterKeys.Count ? ClusterKeys.First() : ClusterKeys[idx];
            //};
            //var pool = new ConnectionPool();
            //pool.ConnectionStringEx(cSRedisConfig.C_IP, cSRedisConfig.C_Post, cSRedisConfig.C_Password,
            //    cSRedisConfig.C_Defaultdatabase, cSRedisConfig.c_Writebuffer, cSRedisConfig.C_PoolSize,
            //    cSRedisConfig.C_SSL, cSRedisConfig.C_Prefix);
            //pool.Connected += (s, o) => {
            //    RedisClient rc = s as RedisClient;
            //};
            //if (ClusterNodes.ContainsKey(pool.ClusterKey)) throw new Exception($"ClusterName: {pool.ClusterKey} 重复，请检查");
            //ClusterNodes.Add(pool.ClusterKey, pool);
            //ClusterKeys = ClusterNodes.Keys.ToList();
        }

        /// <summary>
        /// 在列表中添加一个或多个值
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="value">一个或多个值</param>
        /// <returns></returns>
        //public long RPush(string key, params object[] value) => value == null || value.Any() == false ? 0 : ExecuteScalar(key, (c, k) => c.Client.RPush(k, value));
    }
}
