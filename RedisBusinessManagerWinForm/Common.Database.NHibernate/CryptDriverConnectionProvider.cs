using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Connection;
using Common.Cryptography;

namespace Common.Database.NHibernate
{
    public class CryptDriverConnectionProvider : DriverConnectionProvider, ICryptConnection
    {
        private const string _dict = "connection.connection_string";

        public override void Configure(IDictionary<string, string> settings)
        {
            var conn = settings[_dict];
            settings[_dict] = DecryptConnString(conn);

            base.Configure(settings);
        }
        protected override string GetNamedConnectionString(IDictionary<string, string> settings)
        {
            var conn = settings[_dict];
            settings[_dict] = DecryptConnString(conn);

            return base.GetNamedConnectionString(settings);
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
