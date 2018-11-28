using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;

namespace Smartunicom.Security.Cryptography.X509
{
    public static class PEMHelper
    {
        public static AsymmetricKeyParameter ReadPublicKey(string pem)
        {
            using (var reader = new StringReader(pem))
            {
                var pem_reader = new PemReader(reader);
                {
                    return (AsymmetricKeyParameter)pem_reader.ReadObject();
                }
            }
        }

        public static AsymmetricKeyParameter ReadPrivateKey(string pem)
        {
            using (var reader = new StringReader(pem))
            {
                var pem_reader = new PemReader(reader);
                {
                    var keypair = (AsymmetricCipherKeyPair)pem_reader.ReadObject();
                    {
                        return keypair.Private;
                    }
                }
            }
        }

        public static string ConvertKey(AsymmetricKeyParameter parameter)
        {
            using (var writer = new StringWriter())
            {
                var pem_writer = new PemWriter(writer);
                {
                    pem_writer.WriteObject(parameter);
                    pem_writer.Writer.Flush();
                }

                return writer.ToString();
            }
        }

        public static string ConvertPublicKey(AsymmetricKeyParameter parameter)
        {
            return ConvertKey(parameter);
        }

        public static string ConvertPrivateKey(AsymmetricKeyParameter parameter)
        {
            return ConvertKey(parameter);
        }
    }
}
