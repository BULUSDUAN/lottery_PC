using EntityModel.Enum;
using EntityModel.Xml;
using System.Security.Cryptography;
using System.Text;


namespace KaSon.FrameWork.Common.Gateway
{
    public static class MessageHelper
    {
        public static string GetEncryptBody(string body, string key)
        {
            var iKey = Encipherment.HexStr2ByteArr(key);
            var result = Encipherment.EncryptDES(body, iKey, CipherMode.ECB);
            return result;
        }

        public static string GetDecryptBody(string body, string key)
        {
            var iKey = Encipherment.HexStr2ByteArr(key);
            var result = Encipherment.DecryptDES(body, iKey, CipherMode.ECB);
            return result;
        }

        public static string GetMd5Body(string body)
        {
            return Encipherment.MD5(body, Encoding.UTF8);
        }
    }

 

    public class MessageResponseErrorInfo : XmlMappingObject
    {
        private string _description;

        /// <summary>
        ///     返回码
        /// </summary>
        [XmlMapping("transcode", 1, MappingType = MappingType.Attribute)]
        public string ResultCode { get; set; }

        public string ResultDescription
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                    _description = ResultCodeAnalyzer.GetResultDescription(ResultCode);
                return _description;
            }
        }
    }
}