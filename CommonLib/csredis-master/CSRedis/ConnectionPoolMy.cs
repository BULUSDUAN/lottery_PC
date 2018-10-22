using System;
using System.Collections.Generic;
using System.Text;

namespace CSRedis
{
    /// <summary>
    /// Connection链接池
    /// </summary>
    public partial class ConnectionPool
    {/// <summary>
     /// kason 创建Redis 连接
     /// </summary>
     /// <param name="ex_ip"></param>
     /// <param name="ex_port"></param>
     /// <param name="ex_password"></param>
     /// <param name="ex_defaultdatabase"></param>
     /// <param name="ex_writebuffer"></param>
     /// <param name="ex_poolsize"></param>
     /// <param name="ex_ssl"></param>
     /// <param name="ex_Prefix"></param>
        public void ConnectionStringEx(string ex_ip = "127.0.0.1", int ex_port = 6379, string ex_password = "",
            int ex_defaultdatabase = 0, int ex_writebuffer = 10240,
            int ex_poolsize = 20, bool ex_ssl = false, string ex_Prefix = "")
        {
            _ip = ex_ip;
            _port = ex_port;
            _password = ex_password;
            _database = ex_defaultdatabase;
            _writebuffer = ex_writebuffer;
            _poolsize = ex_poolsize;
            _ssl = ex_ssl;
            Prefix = ex_Prefix;

            if (_poolsize <= 0) _poolsize = 50;
            var initConns = new RedisConnection2[_poolsize];
            for (var a = 0; a < _poolsize; a++) initConns[a] = GetFreeConnection();
            foreach (var conn in initConns) ReleaseConnection(conn);
        }
    }
}
