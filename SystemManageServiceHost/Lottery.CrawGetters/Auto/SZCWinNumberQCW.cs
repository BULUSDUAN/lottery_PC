using EntityModel.CoreModel;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.CrawGetters.Auto
{
    public class SZCWinNumberQCW : ISZCWinNumberCrawler
    {
        public Dictionary<string, string> Process(string gameCode)
        {


            string url = InitConfigInfo.SZC_OPEN_MIRROR_URL;
            string json = PostManager.Get(string.Format(url, gameCode, DateTime.Now.Ticks), Encoding.UTF8);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var item in JsonHelper.Deserialize<GameWinNumber_Info[]>(json))
            {
                dict[item.IssuseNumber] = item.WinNumber;
            }
            return dict;


        }
    }
}
