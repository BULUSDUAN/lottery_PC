using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Common.Cryptography;
using Common.XmlAnalyzer;
using System.Security.Cryptography;

namespace Common.Lottery.Gateway.ZhongMin
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
          return  Encipherment.MD5(body, Encoding.UTF8);
        }
    }
    public class MessageRequestInfo : XmlMappingObject
    {
        public class MessageHead : XmlMappingObject
        {
            [XmlMapping("transcode", 0, MappingType = MappingType.Attribute)]
            public string Transcode { get; set; }
            [XmlMapping("partnerid", 1, MappingType = MappingType.Attribute)]
            public string Partnerid { get; set; }
            [XmlMapping("version", 2, MappingType = MappingType.Attribute)]
            public string Version { get; set; }
            [XmlMapping("time", 3, MappingType = MappingType.Attribute)]
            public string DateTime { get; set; }
        }
        [XmlMapping("head", 0)]
        public MessageHead Head { get; set; }
        [XmlMapping("body", 1)]
        public string Body { get; set; }
    }


    public class MessageResponseErrorInfo : XmlMappingObject
    {
        /// <summary>
        /// 返回码
        /// </summary>
        [XmlMapping("transcode", 1, MappingType = MappingType.Attribute)]
        public string ResultCode { get; set; }

        private string _description = null;
        public string ResultDescription
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    _description = ResultCodeAnalyzer.GetResultDescription(ResultCode);
                }
                return _description;
            }
        }
    }
}
