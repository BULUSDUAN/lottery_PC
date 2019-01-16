using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Enum
{
    public enum ResponseCode
    {
        成功 = 101,
        失败 = 201,
    }
    public enum SchemeSource
    {
        Web = 0,
        Iphone = 101,
        Android = 102,
        Wap = 103,
        Touch = 104,
        YQS = 105,
        YQS_Advertising = 106,
        NS_Bet = 107,
        YQS_Bet = 108,
        Publisher_0321 = 109,
        WX_GiveLottery = 110,
        Web_GiveLottery = 111,
        LuckyDraw = 112
    }    
}
