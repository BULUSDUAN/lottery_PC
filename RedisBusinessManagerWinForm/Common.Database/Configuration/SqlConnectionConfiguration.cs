using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using Common.Cryptography;
using Common.Database.DbAccess;

namespace Common.Database.Configuration
{
    /// <summary>
    /// MSSql数据库连接配置
    /// </summary>
    public class SqlConnectionConfiguration : ConnectionConfiguration, ICryptConnection
    {
        /// <summary>
        /// 连接字符串部分，不包含敏感数据
        /// </summary>
        [ConfigurationProperty("PartialConnectionString", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string PartialConnectionString
        {
            get { return (string)base["PartialConnectionString"]; }
            set { base["PartialConnectionString"] = value; }
        }
        /// <summary>
        /// 用户名（敏感数据）。如果为null，则忽略此项。
        /// </summary>
        [ConfigurationProperty("User", DefaultValue = null, IsKey = false, IsRequired = false)]
        public string User
        {
            get { return (string)base["User"]; }
            set { base["User"] = value; }
        }
        /// <summary>
        /// 密码（敏感数据）。如果为null，则忽略此项。
        /// </summary>
        [ConfigurationProperty("Password", DefaultValue = null, IsKey = false, IsRequired = false)]
        public string Password
        {
            get { return (string)base["Password"]; }
            set { base["Password"] = value; }
        }
        /// <summary>
        /// 敏感数据是否已加密
        /// </summary>
        [ConfigurationProperty("UseEncryption", DefaultValue = false, IsKey = false, IsRequired = false)]
        public bool UseEncryption
        {
            get { return (bool)base["UseEncryption"]; }
            set { base["UseEncryption"] = value; }
        }
        /// <summary>
        /// 获取数据库链接
        /// </summary>
        public override DbConnection GetDbConnection()
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            return conn;
        }
        /// <summary>
        /// 获取数据库类型
        /// </summary>
        public override RDatabaseType DatabaseType { get { return RDatabaseType.MSSQL; } }
        /// <summary>
        /// 获取完整数据库连接字符串
        /// </summary>
        public override string ConnectionString
        {
            get
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(PartialConnectionString);
                if (!builder.IntegratedSecurity)
                {
                    if (User != null)
                    {
                        string userId = User;
                        if (UseEncryption) { userId = DecryptConnString(userId); }
                        builder.UserID = userId;
                    }
                    if (Password != null)
                    {
                        string password = Password;
                        if (UseEncryption) { password = DecryptConnString(password); }
                        builder.Password = password;
                    }
                }
                return builder.ToString();
            }
        }
        public string DecryptConnString(string connStr)
        {
            var crypt = new SymmetricCrypt(Consts.GET_CONN_CRYPT_TYPE());
            return crypt.Decrypt(connStr, Consts.GET_CONN_CRYPT_KEY(), Consts.GET_CONN_CRYPT_IV());
        }
        public string EncryptConnString(string connStr)
        {
            var crypt = new SymmetricCrypt(Common.Database.Consts.GET_CONN_CRYPT_TYPE());
            return crypt.Encrypt(connStr, Consts.GET_CONN_CRYPT_KEY(), Consts.GET_CONN_CRYPT_IV());
        }
    }
}
