using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Smartunicom.Security.Cryptography.X509;
using Org.BouncyCastle.Crypto;

namespace ChinaRailway.Common
{
    public class RS256
    {
#if NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        protected readonly RSA provider;
#else
        protected readonly RSACryptoServiceProvider provider;
#endif


        public RS256()
        {
#if NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
            provider = RSA.Create();
#else
            provider = new RSACryptoServiceProvider();
#endif
        }

        public void SetPublicKey(string publickey)
        {
            if (publickey.StartsWith("-"))
            {
                SetPublicKey(PEMHelper.ReadPublicKey(publickey));
                return;
            }

            SetPublicKey(Convert.FromBase64String(publickey));
        }

        public void SetPublicKey(byte[] publickey)
        {
            SetPublicKey(PublicKeyFactory.CreateKey(publickey));
        }

        public void SetPublicKey(AsymmetricKeyParameter parameter)
        {
            var rsa_para = new RSAParameters();
            {
                var para = (RsaKeyParameters)parameter;
                {
                    rsa_para.Modulus = para.Modulus.ToByteArrayUnsigned();
                    rsa_para.Exponent = para.Exponent.ToByteArrayUnsigned();
                }
            }

            SetPublicKey(rsa_para);
        }

        public void SetPublicKey(RSAParameters parameters)
        {
            provider.ImportParameters(parameters);
        }

        public void SetPrivateKey(string privatekey)
        {
            if (privatekey.StartsWith("-"))
            {
                SetPrivateKey(PEMHelper.ReadPrivateKey(privatekey));
                return;
            }

            SetPrivateKey(Convert.FromBase64String(privatekey));
        }

        public void SetPrivateKey(byte[] privatekey)
        {
            SetPrivateKey(PrivateKeyFactory.CreateKey(privatekey));
        }

        public void SetPrivateKey(AsymmetricKeyParameter parameter)
        {
            var rsa_para = new RSAParameters();
            {
                var para = (RsaPrivateCrtKeyParameters)parameter;
                {
                    rsa_para.Modulus = para.Modulus.ToByteArrayUnsigned();
                    rsa_para.Exponent = para.PublicExponent.ToByteArrayUnsigned();
                    rsa_para.D = para.Exponent.ToByteArrayUnsigned();
                    rsa_para.P = para.P.ToByteArrayUnsigned();
                    rsa_para.Q = para.Q.ToByteArrayUnsigned();
                    rsa_para.DP = para.DP.ToByteArrayUnsigned();
                    rsa_para.DQ = para.DQ.ToByteArrayUnsigned();
                    rsa_para.InverseQ = para.QInv.ToByteArrayUnsigned();
                }
            }

            SetPrivateKey(rsa_para);
        }

        public void SetPrivateKey(RSAParameters parameters)
        {
            provider.ImportParameters(parameters);
        }

        public string SignData(byte[] data)
        {
#if NET46 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
            return Convert.ToBase64String(provider.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
#else
            return Convert.ToBase64String(provider.SignData(data, "SHA256"));
#endif
        }

        public string SignData(string data)
        {
            return SignData(Encoding.UTF8.GetBytes(data));
        }

        public bool VerifyData(byte[] data, string sign)
        {
#if NET46 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
            return provider.VerifyData(data, Convert.FromBase64String(sign), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
#else
            return provider.VerifyData(data, "SHA256", Convert.FromBase64String(sign));
#endif
        }

        public bool VerifyData(string data, string sign)
        {
            return VerifyData(Encoding.UTF8.GetBytes(data), sign);
        }
    }
}
