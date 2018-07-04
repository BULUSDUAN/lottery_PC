using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace EntityModel.CoreModel
{
    public class LotteryServiceResponse
    {
        /// <summary>
        /// 消息序号（与传入时一样）
        /// </summary>
        public string MsgId { get; set; }
        /// <summary>
        /// 结果码
        /// </summary>
        public ResponseCode Code { get; set; }
        /// <summary>
        /// 处理提示消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 处理结果
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 加密串（（msgId+code + message + value 并MD5加密））
        /// </summary>
        public string Sign
        {
            get
            {
                //var valList=new List<string>();
                string strJson = string.Empty;
                if (Value != null && !string.IsNullOrEmpty(Value.ToString()))
                    strJson = JsonConvert.SerializeObject(Value);
                   // strJson = Common.JSON.JsonHelper.Serialize(Value);
               // json

                string sourceString = "caipiao" + MsgId + (int)Code + Message + strJson;
                return MD5(sourceString, Encoding.UTF8);
            }
        }
        /// <summary>
        /// 用指定编码得到哈希密码
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string MD5(string sourceString, Encoding encoding)
        {
            byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(encoding.GetBytes(sourceString));
            StringBuilder builder = new StringBuilder(0x20);
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append(buffer[i].ToString("x").PadLeft(2, '0'));
            }
            return builder.ToString();
        }
    }
}
