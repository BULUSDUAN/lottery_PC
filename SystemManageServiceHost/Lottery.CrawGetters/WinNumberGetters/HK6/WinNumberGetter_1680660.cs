using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;
using System.Linq;
using System.Net;
using EntityModel;

namespace Lottery.CrawGetters
{
    public class WinNumberGetter_1680660 : WinNumberGetter
    {
        private static readonly Dictionary<string, string> UrlMapping;

        public static readonly string Url = "https://1680660.com/smallSix/queryLotteryDate.do?ym={0}";
        public static readonly string HostoryUrl = "https://1680660.com/smallSix/findSmallSixHistory.do";
        static WinNumberGetter_1680660()
        {
            //http://kaijiang.500.com/ssq.shtml
            UrlMapping = new Dictionary<string, string>();
            UrlMapping.Add("HK6", "https://1680660.com/smallSix/queryLotteryDate.do?ym={0}"); //2019-04重庆时时彩

            //UrlMapping.Add("GD11X5", "24");
        }

        private static List<string> GetRequestDate()
        {
            List<string> list = new List<string>();
           // ym = year+"";

            DateTime currentYesr = DateTime.Now;
            string temp = "";
            for (int i = 1; i <= 12; i++)
            {
                temp = (currentYesr.Year ) + "-0" + i;
                if (i >= 10)
                {
                    temp = (currentYesr.Year ) + "-" + i;
                }

                list.Add(temp);
            }
            //下一年的数据
            for (int i = 1; i <= 12; i++)
            {
                 temp = (currentYesr.Year+1) + "-0" + i;
                if (i >= 10)
                {
                    temp = (currentYesr.Year + 1) + "-" +i;
                }

                list.Add(temp);
            }

            return list;
        }
        private static IList<string> IssuseNum(string date)
        {
            List<string> list = new List<string>();
            try
            {
                
                string nurl = string.Format(Url, date);
                var json = PostManagerWithProxy.Get(nurl, Encoding.Default);

                var djson = JsonHelper.Decode(json);
                foreach (var item in djson)
                {

                    try
                    {
                        if (item.Name== "result")
                        {
                            Console.WriteLine("");
                            var data = item.Value.data.kjDate;
                            foreach (var item1 in data)
                            {

                                //"[\r\n  0,\r\n  0\r\n]"
                                string value = (item1 + "").Replace("\r\n", "")
                                    .Replace(" ", "").Replace("[", "").Replace("]", "");
                                var arr = value.Split(',');
                                if (arr.Length>1)
                                {
                                    if (arr[1]=="1")
                                    {
                                        //开奖日期
                                        int day = int.Parse(arr[0]);
                                        string sday = date +"-0" + day;
                                        if (day>=10)
                                        {
                                            sday = date + "-" + day; 
                                        }
                                        list.Add(sday.Replace("-", "/"));

                                    }

                                }

                                Console.WriteLine("");
                            }
                        }

                    }
                    catch (Exception)
                    {

                        continue;
                    }

                }
              
            }
            catch (Exception)
            {

                throw;
            }


            Console.WriteLine(string.Join(Environment.NewLine, list));


            return list;
        }

        public static List<blast_data> winNum()
        {
           string jsonstr = GetHostoryNum();
            List<blast_data> list = new List<blast_data>();
            try
            {

              

                var djson = JsonHelper.Decode(jsonstr);
                foreach (var item in djson)
                {

                    try
                    {
                        if (item.Name == "result")
                        {
                            Console.WriteLine("");
                            var data = item.Value.data.bodyList;
                            foreach (var item1 in data)
                            {

                                blast_data bdata = new blast_data() {
                                    createTime = DateTime.Now,
                                    updateTime = DateTime.Now,
                                    issueNo = (int)item1.issue.Value,
                                      kjtime = (item1.preDrawDate.Value + "").Replace("-","/"),
                                     kjdata = item1.preDrawCode.Value


                                };
                                list.Add(bdata);


                                //"[\r\n  0,\r\n  0\r\n]"
                                //                                issue: 134
                                //nanairo: 0
                                //preDrawCode: "25,34,3,30,13,8,32"
                                //preDrawDate: "2018-11-24"
                                //seventhBigSmall: 0
                                //seventhCompositeBig: 1
                                //seventhCompositeDouble: 0
                                //seventhMantissaBig: 1
                                //seventhSingleDouble: 1
                                //sumTotal: 145
                                //totalBigSmall: 1
                                //totalSingleDouble: 0



                                Console.WriteLine("");
                            }
                        }

                    }
                    catch (Exception)
                    {

                        continue;
                    }

                }

            }
            catch (Exception)
            {

                throw;
            }


            Console.WriteLine(string.Join(Environment.NewLine, list));


            return list;
        }

        public static List<string> GetIssuseNum() {
            List<string> ListDate = new List<string>();
            var list = GetRequestDate();
            foreach (var item in list)
            {
                var mouthList = IssuseNum(item);
                Thread.Sleep(5 * 1000);
                if (mouthList.Count > 0)
                {



                }
                else {
                     mouthList = IssuseNum(item);

                    if (mouthList.Count<=0)
                    {
                        return ListDate;
                    }
                }
                ListDate= ListDate.Concat(mouthList).ToList();

               // WriteLog("成功同步到数据库");
            }

            foreach (var item in ListDate)
            {
                //录入数据库

            }

            return ListDate;
        }

        private static string GetHostoryNum()
        {
            //
         //   string nurl = string.Format(Url, date);
            WebHeaderCollection Head = new WebHeaderCollection();
            //System.Text.Encoding.GetEncoding("GB2312")

            int year = DateTime.Now.Year;

            Head["Origin"] = "https://6hch.com";
            var json = PostManagerWithProxy.Post_Head(HostoryUrl, "year="+ year + "&type=1", System.Text.Encoding.UTF8,
                "application/x-www-form-urlencoded", "https://6hch.com/html/kaihistory.html", Head);




            return json;
        }

       

        /// <summary>
        ///     500万采集号码
        /// </summary>
        /// <param name="gameName">彩种</param>
        /// <param name="lastIssuseCount">int值：10，20，30，50，100</param>
        /// <returns></returns>
        public override Dictionary<string, string> GetWinNumber(string gameName, int lastIssuseCount,
            string issuseNumber)
        {
            var xmlUrl = "";// GetUrl(gameName, lastIssuseCount);
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlUrl);
            var result = new Dictionary<string, string>();

            switch (gameName)
            {
                case "SDQYH":
                case "GDKLSF":
                case "CQSSC":
                case "GD11X5":
                case "SHSSL":
                    result = GetWinNumber(xmlDoc, lastIssuseCount, gameName);
                    break;
                default:
                    break;
            }
            switch (gameName)
            {
                case "SSQ":
                case "QXC":
                case "QLC":
                case "DLT":
                case "PL3":
                case "PL5":
                case "JX11X5":
                case "FC3D":
                    result = GetWinNumber1(xmlDoc, lastIssuseCount, gameName);
                    break;
                default:
                    break;
            }
            return result;
        }

        private Dictionary<string, string> GetWinNumber(XmlDocument xmlDoc, int lastIssuseCount, string gameCode)
        {
            var result = new Dictionary<string, string>();
            var root = xmlDoc.GetElementsByTagName("xml")[0];
            var start = root.ChildNodes.Count - 1;

            for (; start >= 0; start--)
            {
                var ele = root.ChildNodes[start] as XmlElement;
                var issuse = ele.Attributes["expect"].Value;
                var winCode = ele.Attributes["opencode"].Value;
                switch (gameCode)
                {
                    case "SDQYH":
                        issuse = string.Format("20{0}-{1}", issuse.Substring(0, 6), issuse.Substring(8, 2));
                        break;
                    case "GDKLSF":
                        issuse = string.Format("{0}-{1}", issuse.Substring(0, 8), issuse.Substring(8, 2));
                        break;
                    case "CQSSC":
                        issuse = string.Format("{0}-{1}", issuse.Substring(0, 8), issuse.Substring(8, 3));
                        break;
                    case "GD11X5":
                        issuse = string.Format("20{0}-{1}", issuse.Substring(0, 6), issuse.Substring(6, 2));
                        break;
                    case "SHSSL":
                        issuse = string.Format("{0}-{1}", issuse.Substring(0, 8), issuse.Substring(8, 3));
                        //winCode = winCode.Split(',');//没有逗号分隔
                        break;
                    default:
                        break;
                }
                result.Add(issuse, winCode);

                if (result.Count >= lastIssuseCount)
                    break;
            }
            return result;
        }


        private Dictionary<string, string> GetWinNumber1(XmlDocument xmlDoc, int lastIssuseCount, string gameCode)
        {
            var result = new Dictionary<string, string>();
            var root = xmlDoc.GetElementsByTagName("xml")[0];
            //int start = root.ChildNodes.Count - 1;

            var name = xmlDoc.DocumentElement.Name;
            foreach (XmlElement item in xmlDoc[name].ChildNodes)
            {
                var winCode = item.Attributes["opencode"].Value;
                var issuse = item.Attributes["expect"].Value;
                switch (gameCode)
                {
                    case "SSQ":
                    case "QXC":
                    case "QLC":
                    case "DLT":
                        issuse = string.Format("{0}-{1}", DateTime.Now.Year, issuse.Substring(2, issuse.Length - 2));
                        break;
                    case "PL3":
                    case "PL5":
                        issuse = string.Format("{0}-{1}", DateTime.Now.Year, issuse.Substring(2, issuse.Length - 2));
                        break;
                    case "FC3D":
                        issuse = issuse.Insert(4, "-");
                        break;
                    case "JX11X5":
                        issuse = string.Format("20{0}-{1}", issuse.Substring(0, 6), issuse.Substring(6));
                        break;
                    default:
                        break;
                }
                result.Add(issuse, winCode);
            }
            return result;
        }
    }
}