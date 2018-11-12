using EntityModel;
using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Analyzer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Analyzer.Hk6Model
{
    public class Hk6_BaseAnalyzer
    {
       

        public static CommonActionResult BetingOrderCheck(HK6Sports_BetingInfo info,IList<blast_played> playedList)
        {
            CommonActionResult result = new CommonActionResult();
            result.IsSuccess = true;
            //校验投注订单合法信息，包括金额 玩法，号码，
            foreach (var item in info.orderList)
            {

                var played = playedList.Where(b => b.id == item.playedId).FirstOrDefault();
                if (played != null)
                {
                    //每一种玩法检查最小金额
                    //每一注最小金额是否合法
                    if (item.unitPrice < played.minCharge)
                    {
                        result.Message = "每一注最小金额错误";
                        result.IsSuccess = false;
                        return result;
                    }
                    //每一种玩法 号码检查是否合法
                    var tempbol = false;
                    switch (played.Tag)
                    {
                        case "ZT"://特码
                            tempbol = HK6_特码.BetingCodeCheck(item);
                            
                            break;
                        default:
                            break;
                    }
                    if (!tempbol)
                    {
                        result.Message = "号码格式错误";
                        result.IsSuccess = false;
                        return result;
                    }

                }
                else
                {
                    result.Message = "投注玩法不支持";
                    result.IsSuccess = false;
                    return result;
                }

            }

            //校验追号是否合法，加倍是否合法



            return result;
        }
    }
}
