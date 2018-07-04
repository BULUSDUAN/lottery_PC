using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;
using KaSon.FrameWork.Common.Algorithms;
using System.IO;

namespace KaSon.FrameWork.Common.Utilities
{
    /// <summary>
    /// 有用而无法明确归类的的函数
    /// </summary>
    public static class UsefullHelper
    {
        public static object moneyLocker = new object();
        public static object ticketLocker = new object();
        public static object orderLocker = new object();
        public static object order_TLocker = new object();
        public static object order_SingleScheme = new object();
        public static object vcMoneyLocker = new object();
        /// <summary>
        /// 是否当前系统配置为测试
        /// </summary>
         private static bool isTest=false;
        public static bool IsInTest
        {
            get
            {
                //if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["IsTest"]))
                //{
                //    return false;
                //}
                // isTest = ConfigurationManager.AppSettings["IsTest"];
                //if (isTest.Equals("true", StringComparison.OrdinalIgnoreCase) || isTest.Equals("1"))
                //{
                //    return true;
                //}
                return isTest;
            }
        }
        /// <summary>
        /// 尝试执行操作，失败记录日志，不会抛出异常
        /// </summary>
        /// <param name="action"></param>
        public static void TryDoAction(Action action)
        {
            try
            {
                if (action != null)
                {
                    action();
                }
            }
            catch (Exception ex)
            {
                try
                {
                    WLog(action.Method.Name.Replace("<", "").Replace(">", ""), ex,"");
                }
                catch
                {
                }
            }
        }
        private static bool _AppLogEnabled = true;
        private static object LockObj = new object();
        private static void WriteLog(string msg, string logfile = "")
        {
            lock (LockObj)
            {
                string str = "";
                if (!string.IsNullOrEmpty(logfile))
                {
                    str = @"Common_Log\" + logfile;
                }
                else
                {
                    str = @"Common_Log\" + DateTime.Now.ToString("yyyyMMddHH") + "_" + AppDomain.CurrentDomain.Id.ToString() + ".log";
                }
                str = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, str);
                string directoryName = Path.GetDirectoryName(str);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                string str3 = string.Format("{0}：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), msg);
                using (StreamWriter writer = new StreamWriter(str, true, Encoding.Default))
                {
                    writer.WriteLine(str3);
                }
            }
        }
               /// <summary>
        /// 记录框架的异常信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="logfile">日志文件</param>
        public static void WLog(string name,Exception ex, string logfile)
        {
            if (_AppLogEnabled && (ex != null))
            {
                StringBuilder builder = new StringBuilder();
                if (!string.IsNullOrEmpty(name)) {
                    builder.AppendFormat("错误标签:{0}\r\n", name);
                }
               
                builder.AppendFormat("发生［{0}］异常，异常相关信息如下：", ex.GetType().ToString());
                builder.AppendFormat("\r\n异常描述：{0}", ex.Message);
                builder.AppendFormat("\r\n异 常 源：{0}", ex.Source);
                builder.AppendFormat("\r\n堆栈跟踪：\r\n{0}", ex.StackTrace);
                if (ex.InnerException != null)
                {
                    builder.AppendFormat("\r\n内含异常：{0}", ex.InnerException.GetType().ToString());
                    builder.AppendFormat("\r\n异常描述：{0}", ex.InnerException.Message);
                    builder.AppendFormat("\r\n异 常 源：{0}", ex.InnerException.Source);
                    builder.AppendFormat("\r\n堆栈跟踪：\r\n{0}\r\n", ex.InnerException.StackTrace);
                }
                else
                {
                    builder.Append("\r\n内含异常：无\r\n");
                }
                WriteLog(builder.ToString(), logfile);
            }
        }
        public static T GetDbValue<T>(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return default(T);
            }
            return (T)value;
        }
        public static string RemoveStringEmpty(string src)
        {
            return src.Replace(" ", "")
                .Replace("　", "");
        }

        /// <summary>
        /// 替换html标签
        /// </summary>
        public static string ReplaceHtmlTag(string source, string htmlTag)
        {
            //htmlTag :script
            source = source.ToLower();
            var startHtmlTag = string.Format("<{0}>", htmlTag);
            while (source.IndexOf(startHtmlTag) > 0)
            {
                var startIndex = source.IndexOf(startHtmlTag);
                var endHtmlTag = string.Format("</{0}>", htmlTag);
                var endIndex = source.IndexOf(endHtmlTag);

                var tem = source.Substring(startIndex, endIndex - startIndex + endHtmlTag.Length);
                source = source.Replace(tem, string.Empty);
            }
            return source;
        }
        public static string ReplaceHtmlTag(string source, List<string> htmlTagList)
        {
            foreach (var tag in htmlTagList)
            {
                source = ReplaceHtmlTag(source, tag);
            }
            return source;
        }
        public static string UUID(int length = 10)
        {
            return DateTime.Now.ToString("MMddHHmmssmmm") + GetUUID(length);
            //var buffer = Guid.NewGuid().ToByteArray();
            //var code = System.Math.Abs(BitConverter.ToInt32(buffer, 0)).ToString();
            //if (code.Length < 8)
            //    return code.PadLeft(8, '0');
            //else
            //    return code.Substring(0, 8);

            //var str = Math.Abs(Guid.NewGuid().ToString().GetHashCode()).ToString();
            //if (str.Length < 8)
            //    return str.PadLeft(8, '0');
            //else
            //    return str.Substring(0, 8);
        }
        public static string GetUUID(int length)
        {
            var buffer = Guid.NewGuid().ToByteArray();
            var code = System.Math.Abs(BitConverter.ToInt32(buffer, 0)).ToString();
            if (code.Length >= length)
                return code.Substring(0, length);

            var diffLength = length - code.Length;
            var diffCode = GetUUID(diffLength);
            return string.Format("{0}{1}", diffCode, code);
        }

        /// <summary>
        /// 计算单张票中，最大最小中奖金额(M串N)
        /// 暂只计算了JCZQ JCLQ
        /// </summary>
        public static void GetTicketMinMoneyOrMaxMoney_MN(string playType, string betContent, string betOdds, out decimal minMoney, out decimal maxMoney)
        {
            minMoney = 0M;
            maxMoney = 0M;

            //M_N
            var chuanArray = playType.Split('_');
            if (chuanArray.Length != 2) return;
            var a = int.Parse(chuanArray[0]);
            var b = int.Parse(chuanArray[1]);
            if (b <= 1) return;
            //串关包括的M串1种类
            var countList = AnalyzeChuanJC(a, b);

            var c = new Combination();
            //var betContent = "SPF_140709001_3,1/SPF_140709052_1/BRQSPF_140709053_3,0/SPF_140709054_3";
            //var betOdds = "140709001_3|1.5200,1|3.7200,0|4.9500/140709052_3|1.8300,1|3.5000,0|3.3600/140709053_3|3.2000,1|3.4500,0|1.9200/140709054_3|2.8800,1|3.5500,0|2.0000";
            //拆分odds
            var oddstrArray = betOdds.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            //投注内容
            var betContentArray = betContent.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            //投注的比赛
            var betMatchIdList = new List<string>();
            #region 查找出投注的比赛
            foreach (var contentItem in betContentArray)
            {
                var matchId = string.Empty;
                var content = string.Empty;
                //140709051_2,3
                //SPF_140709001_3,1
                var array = contentItem.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                switch (array.Length)
                {
                    case 2:
                        matchId = array[0];
                        break;
                    case 3:
                        matchId = array[1];
                        break;
                    default:
                        break;
                }
                if (!betMatchIdList.Contains(matchId))
                    betMatchIdList.Add(matchId);
            }
            #endregion
            var tempMin = 0M;
            var tempMax = 0M;
            foreach (var m in countList)
            {
                c.Calculate(betMatchIdList.ToArray(), m, (arr) =>
                {
                    var tempBetContentList = new List<string>();
                    var tempBetOddsList = new List<string>();
                    foreach (var item in arr)
                    {
                        foreach (var bc in betContentArray)
                        {
                            if (bc.IndexOf(item) >= 0)
                                tempBetContentList.Add(bc);
                        }
                        foreach (var bo in oddstrArray)
                        {
                            if (bo.IndexOf(item) >= 0)
                                tempBetOddsList.Add(bo);
                        }
                    }

                    var min = 0M;
                    var max = 0M;
                    GetTicketMinMoneyOrMaxMoney(string.Join("/", tempBetContentList.ToArray()), string.Join("/", tempBetOddsList.ToArray()), out min, out max);
                    if (tempMin == 0M)
                        tempMin = min;
                    if (min < tempMin)
                        tempMin = min;
                    tempMax += max;
                });
            }
            minMoney = tempMin;
            maxMoney = tempMax;
        }

        private static List<int> AnalyzeChuanJC(int a, int b)
        {
            var list = new List<int>();
            if (b == 1)
            {
                list.Add(a);
                return list;
            }
            switch (a + "_" + b)
            {
                #region 3串
                case "3_3":
                    list.Add(2);
                    break;
                case "3_4":
                    list.Add(2);
                    list.Add(3);
                    break;
                #endregion

                #region 4串
                case "4_4":
                    list.Add(3);
                    break;
                case "4_5":
                    list.Add(3);
                    list.Add(4);
                    break;
                case "4_6":
                    list.Add(2);
                    break;
                case "4_11":
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    break;
                #endregion

                #region 5串
                case "5_5":
                    list.Add(4);
                    break;
                case "5_6":
                    list.Add(4);
                    list.Add(5);
                    break;
                case "5_10":
                    list.Add(2);
                    break;
                case "5_16":
                    list.Add(3);
                    list.Add(4);
                    list.Add(5);
                    break;
                case "5_20":
                    list.Add(2);
                    list.Add(3);
                    break;
                case "5_26":
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    list.Add(5);
                    break;
                #endregion

                #region 6串
                case "6_6":
                    list.Add(5);
                    break;
                case "6_7":
                    list.Add(5);
                    list.Add(6);
                    break;
                case "6_15":
                    list.Add(2);
                    break;
                case "6_20":
                    list.Add(3);
                    break;
                case "6_22":
                    list.Add(4);
                    list.Add(5);
                    list.Add(6);
                    break;
                case "6_35":
                    list.Add(2);
                    list.Add(3);
                    break;
                case "6_42":
                    list.Add(3);
                    list.Add(4);
                    list.Add(5);
                    list.Add(6);
                    break;
                case "6_50":
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    break;
                case "6_57":
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    list.Add(5);
                    list.Add(6);
                    break;
                #endregion

                #region 7串
                case "7_7":
                    list.Add(6);
                    break;
                case "7_8":
                    list.Add(6);
                    list.Add(7);
                    break;
                case "7_21":
                    list.Add(5);
                    break;
                case "7_35":
                    list.Add(4);
                    break;
                case "7_120":
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    list.Add(5);
                    list.Add(6);
                    list.Add(7);
                    break;
                #endregion

                #region 8串
                case "8_8":
                    list.Add(7);
                    break;
                case "8_9":
                    list.Add(7);
                    list.Add(8);
                    break;
                case "8_28":
                    list.Add(6);
                    break;
                case "8_56":
                    list.Add(5);
                    break;
                case "8_70":
                    list.Add(4);
                    break;
                case "8_247":
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    list.Add(5);
                    list.Add(6);
                    list.Add(7);
                    list.Add(8);
                    break;
                #endregion
            }
            return list;
        }

        /// <summary>
        /// 计算单张票中，最大最小中奖金额
        /// 暂只计算了JCZQ JCLQ
        /// </summary>
        public static List<decimal> GetTicketMinMoneyOrMaxMoney(string betContent, string betOdds, out decimal minMoney, out decimal maxMoney)
        {
            //var betContent = "SPF_140709001_3,1/SPF_140709052_1/BRQSPF_140709053_3,0/SPF_140709054_3";
            //var betOdds = "140709001_3|1.5200,1|3.7200,0|4.9500/140709052_3|1.8300,1|3.5000,0|3.3600/140709053_3|3.2000,1|3.4500,0|1.9200/140709054_3|2.8800,1|3.5500,0|2.0000";
            //拆分odds
            var oddstrArray = betOdds.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            //投注内容
            var winMoneyList = new List<decimal>();
            var betContent2Array = SplitBetContentTo2Array(betContent);
            new ArrayCombination().Calculate(betContent2Array, (array) =>
            {
                var currentSp = 2M;
                foreach (var item in array)
                {
                    var itemArray = item.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    if (itemArray.Length != 2)
                        continue;
                    var sp = GetMatchOdd(oddstrArray, itemArray[0], itemArray[1]);
                    currentSp *= sp;
                }
                winMoneyList.Add(currentSp);
            });
            minMoney = winMoneyList.Count > 0 ? winMoneyList.Min() : 0M;
            maxMoney = winMoneyList.Count > 0 ? winMoneyList.Max() : 0M;
            return winMoneyList;
        }


        /// <summary>
        /// 投注内容拆分为二维数组
        /// </summary>
        private static string[][] SplitBetContentTo2Array(string betContent)
        {
            var contentList = new List<string[]>();
            //投注内容
            var betContentArray = betContent.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var contentItem in betContentArray)
            {
                var matchId = string.Empty;
                var content = string.Empty;
                //140709051_2,3
                //SPF_140709001_3,1
                var array = contentItem.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                switch (array.Length)
                {
                    case 2:
                        matchId = array[0];
                        content = array[1];
                        break;
                    case 3:
                        matchId = array[1];
                        content = array[2];
                        break;
                    default:
                        break;
                }
                var betArray = content.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => string.Format("{0}_{1}", matchId, p)).ToArray();
                contentList.Add(betArray);
            }
            return contentList.ToArray();
        }

        /// <summary>
        /// 根据投注 odd数组，投注比赛，投注内容，获取投注内容对应的sp值
        /// </summary>
        private static decimal GetMatchOdd(string[] oddstrArray, string matchId, string betContent)
        {
            try
            {
                //oddstrArray : 140709052_3|1.8300,1|3.5000,0|3.3600   140709001_3|1.5200,1|3.7200,0|4.9500
                //matchId : 140709052
                //betContent : 3
                var oddstr = oddstrArray.FirstOrDefault(p => p.StartsWith(matchId));
                if (string.IsNullOrEmpty(oddstr))
                    return 1M;
                //oddstr :140709001_3|1.5200,1|3.7200,0|4.9500
                var oddArray = oddstr.Replace(string.Format("{0}_", matchId), string.Empty).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                //3|1.5200
                //1|3.7200
                //0|4.9500
                var spItem = oddArray.FirstOrDefault(p => p.StartsWith(betContent));
                if (string.IsNullOrEmpty(spItem))
                    return 1M;
                return decimal.Parse(spItem.Replace(string.Format("{0}|", betContent), string.Empty));
            }
            catch (Exception)
            {
                return 1M;
            }
        }
    }
}
