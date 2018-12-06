using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Interface;
using KaSon.FrameWork.Analyzer.Hk6Model;
using KaSon.FrameWork.Common.Utilities;

namespace  KaSon.FrameWork.Analyzer.Model
{
    internal class HK6_特码 
    {
        public HK6_特码(string gameCode)
        {
        }
     

        /// <summary>
        /// 检查中奖号码格式是否正确
        /// </summary>
        /// <param name="winNumber">中奖号码</param>
        /// <param name="errMsg">错误消息</param>
        /// <returns>格式是否正确</returns>
        public bool CheckWinNumber(string winNumber, out string errMsg)
        {
            throw new Exception("还没实现");
        }


        /// <summary>
        /// 号码格式检查
        /// </summary>
        /// <param name="order"></param>
        /// <param name="played"></param>
        /// <returns></returns>
        public static  bool BetingCodeCheck(HK6Sports_OrderInfo order)
        {

            //if (string.IsNullOrEmpty(order.betingCode))
            //{
            //    return false;
            //}
            //继续完善
            return true;
        }

       
    }
}
