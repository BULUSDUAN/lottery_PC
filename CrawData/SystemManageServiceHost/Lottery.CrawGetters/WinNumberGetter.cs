using System;
using System.Collections.Generic;
using Lottery.CrawGetters;

namespace Lottery.CrawGetters
{
    /// <summary>
    ///     中奖号码获取器
    /// </summary>
    public abstract class WinNumberGetter: IWinNumberGetter
    {
       // public WinNumberGetter(  ILogger<WinNumberGetter> logger) { }
        /// <summary>
        ///     获取中奖号码
        /// </summary>
        /// <param name="gameCode">彩种</param>
        /// <param name="lastIssuseCount">获取中奖号码的期数</param>
        /// <returns>返回的中奖号码列表。期号和中奖号码两个信息</returns>
        public abstract Dictionary<string, string> GetWinNumber(string gameCode, int lastIssuseCount,
            string issuseNumber = "");

        /// <summary>
        ///     创建实例
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns></returns>
        public  WinNumberGetter CreateInstance(string interfaceType)
        {
            switch (interfaceType.ToLower())
            {
                case "shishicai":
                    return new WinNumberGetter_Get_ShiShiCai();
                case "500wan":
                    return new WinNumberGetter_500Wan();
                case "cailele":
                    return new WinNumberGetter_CaiLeLe();
                case "wozhongla":
                    return new WinNumberGetter_WoZhongLa();
                case "baidulehecai":
                    return new WinNumberGetter_LeHeCai();
                case "gfw":
                    return new WinNumberGetter_GFW();
                case "gxfc":
                    return new WinNumberGetter_GXFC();
                case "aicaipiao":
                    return new WinNumberGetter_AiCaiPiao();
                case "lehecai":
                    return new WinNumberGetter_LeHeCai();
                case "shishicaipost":
                    return new WinNumberGetter_Post_ShiShiCai();
                case "kuaicaile":
                    return new WinNumberGetter_KuaiCaiLe();
                case "gdlottery":
                    return new WinNumberGetter_GDlottery();
                case "jiangsukuai3":
                    return new WinNumberGetter_JiangSuKuai3();
                case "aicaile":
                    return new WinNumberGetter_AiCaiLe();
                case "zhongmin":
                    return new WinNumberGetter_ZhongMin();
                case "liangcai":
                    return new WinNumberGetter_Liangcai();
                case "kaicaiwang":
                    return new WinNumberGetter_KaiCaiWang();
                default:
                    throw new ArgumentOutOfRangeException("不支持的接口类型 - " + interfaceType);
            }
        }
    }
}