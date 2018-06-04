using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
using KaSon.FrameWork.Helper;
namespace EntityModel.HelpModel
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
                    strJson = JsonHelper.Serialize(Value);

                string sourceString = "caipiao" + MsgId + (int)Code + Message + strJson;
                return Encipherment.MD5(sourceString,Encoding.UTF8);
            }
        }

    }
}
