using EntityModel.BonusPool;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.CrawGetters.BonusPoolGetter
{
    /// <summary>
    /// 开奖数据获取器
    /// </summary>
    public abstract class OpenDataGetter
    {
        public abstract OpenDataInfo GetOpenData(string gameCode, string issuseNumber);

        public static OpenDataGetter CreateInstance(string interfaceType)
        {
            switch (interfaceType.ToLower())
            {
                //case "500wan":
                //    return new OpenDataGetter_500wan();
                case "lehecai":
                    return new OpenDataGetter_Lehecai();
                case "aicaipiao":
                    return new OpenDataGetter_AiCai();
                case "cailele":
                    return new OpenDataGetter_CaiLeLe();
                default:
                    throw new ArgumentOutOfRangeException("不支持的接口类型 - " + interfaceType);
            }
        }
    }
}
