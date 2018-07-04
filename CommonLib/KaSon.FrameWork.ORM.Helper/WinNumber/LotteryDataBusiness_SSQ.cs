using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common;

using KaSon.FrameWork.ORM.Helper.WinNumber.Manage;
using EntityModel;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;


namespace KaSon.FrameWork.ORM.Helper.WinNumber
{
    /// <summary>
    /// 时时彩相关
    /// </summary>
    public class LotteryDataBusiness_SSQ : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get
            {
                return "SSQ";
            }
        }

        public void ImportWinNumber(string issuseNumber, string winNumber)
        {
            if (string.IsNullOrEmpty(issuseNumber)) return;
            if (string.IsNullOrEmpty(winNumber)) return;

            var msg = string.Empty;
            AnalyzerFactory.AnalyzerFactory.GetWinNumberAnalyzer(this.CurrentGameCode).CheckWinNumber(winNumber, out msg);
            if (!string.IsNullOrEmpty(msg))
                throw new Exception(msg);

            //开启事务
            using (LottertDataDB)
            {
                LottertDataDB.Begin();

                this.ClearGameChartCache("QuerySSQ_JiBenZouSi");
                this.ClearGameChartCache("QuerySSQ_DX");
                this.ClearGameChartCache("QuerySSQ_C3");
                this.ClearGameChartCache("QuerySSQ_HeZhi");
                this.ClearGameChartCache("QuerySSQ_JiOu");
                this.ClearGameChartCache("QuerySSQ_KuaDu_1_6");
                this.ClearGameChartCache("QuerySSQ_KuaDu_SW");
                this.ClearGameChartCache("QuerySSQ_ZhiHe");
                this.ClearNewWinNumberCache("QuerySSQ_GameWinNumber");

                AddSSQ_KuaDu_1_6(issuseNumber, winNumber);
                AddSSQ_KuaDu_SW(issuseNumber, winNumber);
                AddSSQ_HeZhi(issuseNumber, winNumber);
                AddSSQ_C3(issuseNumber, winNumber);
                AddSSQ_ZhiHe(issuseNumber, winNumber);
                AddSSQ_JiBenZouSi(issuseNumber, winNumber);
                AddSSQ_DX(issuseNumber, winNumber);
                AddSSQ_JiOu(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }

        #region 生成走势数据

        public void Add_GameWinNumber(string issuseNumber, string winNumber)
        {
            new KJGameIssuseBusiness().IssusePrize(this.CurrentGameCode, issuseNumber, winNumber);
            var manager = new SSQ_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddSSQ_GameWinNumber(new SSQ_GameWinNumber
            {
                GameCode = this.CurrentGameCode,
                IssuseNumber = issuseNumber,
                WinNumber = winNumber,
                CreateTime = DateTime.Now,
            });

        }

        /// <summary>
        /// 跨度走势12,23,34,45,56
        /// </summary>
        public void AddSSQ_KuaDu_1_6(string issuseNumber, string winNumber)
        {
            var manager = new SSQ_KuaDu_1_6Manager();
            var issuse = manager.QuerySSQ_KuaDu_1_6IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var winRed = winArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var KD_12 = int.Parse(winRed[1]) - int.Parse(winRed[0]);
            var KD_W_12 = KD_12 % 10;

            var KD_23 = int.Parse(winRed[2]) - int.Parse(winRed[1]);
            var KD_W_23 = KD_23 % 10;

            var KD_34 = int.Parse(winRed[3]) - int.Parse(winRed[2]);
            var KD_W_34 = KD_34 % 10;

            var KD_45 = int.Parse(winRed[4]) - int.Parse(winRed[3]);
            var KD_W_45 = KD_45 % 10;

            var KD_56 = int.Parse(winRed[5]) - int.Parse(winRed[4]);
            var KD_W_56 = KD_56 % 10;

            var last = manager.QueryLastSSQ_KuaDu_1_6();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedLotteryNumber", winArray[0]);
            dic.Add("KuaDu_12", KD_12);
            dic.Add("KuaDu_W_12", KD_W_12);
            dic.Add("KuaDu_23", KD_23);
            dic.Add("KuaDu_W_23", KD_W_23);
            dic.Add("KuaDu_34", KD_34);
            dic.Add("KuaDu_W_34", KD_W_34);
            dic.Add("KuaDu_45", KD_45);
            dic.Add("KuaDu_W_45", KD_W_45);
            dic.Add("KuaDu_56", KD_56);
            dic.Add("KuaDu_W_56", KD_W_56);
            var entity = this.CreateNewEntity<SSQ_KuaDu_1_6>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("KD_12_"))
                {
                    var order = p.Name.Replace("KD_12_", string.Empty);
                    lastValue = int.Parse(order) == KD_12 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("KDW_12_"))
                {
                    var order = p.Name.Replace("KDW_12_", string.Empty);
                    lastValue = int.Parse(order) == KD_W_12 ? 0 : lastValue;
                }

                if (p.Name.StartsWith("KD_23_"))
                {
                    var order = p.Name.Replace("KD_23_", string.Empty);
                    lastValue = int.Parse(order) == KD_23 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("KDW_23_"))
                {
                    var order = p.Name.Replace("KDW_23_", string.Empty);
                    lastValue = int.Parse(order) == KD_W_23 ? 0 : lastValue;
                }

                if (p.Name.StartsWith("KD_34_"))
                {
                    var order = p.Name.Replace("KD_34_", string.Empty);
                    lastValue = int.Parse(order) == KD_34 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("KDW_34_"))
                {
                    var order = p.Name.Replace("KDW_34_", string.Empty);
                    lastValue = int.Parse(order) == KD_W_34 ? 0 : lastValue;
                }

                if (p.Name.StartsWith("KD_45_"))
                {
                    var order = p.Name.Replace("KD_45_", string.Empty);
                    lastValue = int.Parse(order) == KD_45 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("KDW_45_"))
                {
                    var order = p.Name.Replace("KDW_45_", string.Empty);
                    lastValue = int.Parse(order) == KD_W_45 ? 0 : lastValue;
                }

                if (p.Name.StartsWith("KD_56_"))
                {
                    var order = p.Name.Replace("KD_56_", string.Empty);
                    lastValue = int.Parse(order) == KD_56 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("KDW_56_"))
                {
                    var order = p.Name.Replace("KDW_56_", string.Empty);
                    lastValue = int.Parse(order) == KD_W_56 ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddSSQ_KuaDu_1_6(entity);
        }

        /// <summary>
        /// 跨度走势首尾
        /// </summary>
        public void AddSSQ_KuaDu_SW(string issuseNumber, string winNumber)
        {
            var manager = new SSQ_KuaDu_SWManager();
            var issuse = manager.QuerySSQ_KuaDu_SWIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var winRed = winArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var KD = int.Parse(winRed[5]) - int.Parse(winRed[0]);
            var last = manager.QueryLastSSQ_KuaDu_SW();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedLotteryNumber", winArray[0]);
            dic.Add("KuaDu", KD);
            var entity = this.CreateNewEntity<SSQ_KuaDu_SW>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("KD_"))
                {
                    var order = p.Name.Replace("KD_", string.Empty);
                    lastValue = int.Parse(order) == KD ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddSSQ_KuaDu_SW(entity);
        }

        /// <summary>
        /// 和值走势
        /// </summary>
        public void AddSSQ_HeZhi(string issuseNumber, string winNumber)
        {
            var manager = new SSQ_HeZhiManager();
            var issuse = manager.QuerySSQ_HeZhiIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var winRed = winArray[0].Split(',');
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]) + int.Parse(winRed[3]) + int.Parse(winRed[4]) + int.Parse(winRed[5]);
            var hw = hz % 10;

            var last = manager.QueryLastSSQ_HeZhi();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedLotteryNumber", winArray[0]);
            dic.Add("HeZhi", hz);
            dic.Add("HeWei", hw);
            dic.Add("Red1", winRed[0]);
            dic.Add("Red2", winRed[1]);
            dic.Add("Red3", winRed[2]);
            dic.Add("Red4", winRed[3]);
            dic.Add("Red5", winRed[4]);
            dic.Add("Red6", winRed[5]);

            var entity = this.CreateNewEntity<SSQ_HeZhi>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("HZ_"))
                {
                    var order = p.Name.Replace("HZ_", string.Empty);
                    var hzfb = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = hz >= int.Parse(hzfb[0]) && hz <= int.Parse(hzfb[1]) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("HW_"))
                {
                    var order = p.Name.Replace("HW_", string.Empty);
                    lastValue = int.Parse(order) == hw ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddSSQ_HeZhi(entity);
        }

        /// <summary>
        /// 除3余数
        /// </summary>
        private void AddSSQ_C3(string issuseNumber, string winNumber)
        {
            var manager = new SSQ_C3Manager();
            var issuse = manager.QuerySSQ_C3IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            if (winNumber.IndexOf("|") == -1)
                throw new Exception("开奖号码不正确！");
            //取红球
            var redBall = winNumber.Split('|')[0];
            var ball = redBall.Split(',');
            var redball = new int[]{
                Convert.ToInt32(ball[0])%3,
                Convert.ToInt32(ball[1])%3,
                Convert.ToInt32(ball[2])%3,
                Convert.ToInt32(ball[3])%3,
                Convert.ToInt32(ball[4])%3,
                Convert.ToInt32(ball[5])%3
            };
            var Y0_Number = redball.Where(p => p == 0).Count();
            var Y1_Number = redball.Where(p => p == 1).Count();
            var Y2_Number = redball.Where(p => p == 2).Count();

            var last = manager.QueryLastSSQ_C3();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedLotteryNumber", redBall);
            dic.Add("Red1", ball[0]);
            dic.Add("Red2", ball[1]);
            dic.Add("Red3", ball[2]);
            dic.Add("Red4", ball[3]);
            dic.Add("Red5", ball[4]);
            dic.Add("Red6", ball[5]);
            dic.Add("Y0_Number", Y0_Number);
            dic.Add("Y1_Number", Y1_Number);
            dic.Add("Y2_Number", Y2_Number);
            dic.Add("O_Red1_0", redball[0] == 0 ? 0 : (last == null ? 1 : last.O_Red1_0 + 1));
            dic.Add("O_Red1_1", redball[0] == 1 ? 0 : (last == null ? 1 : last.O_Red1_1 + 1));
            dic.Add("O_Red1_2", redball[0] == 2 ? 0 : (last == null ? 1 : last.O_Red1_2 + 1));
            dic.Add("O_Red2_0", redball[1] == 0 ? 0 : (last == null ? 1 : last.O_Red2_0 + 1));
            dic.Add("O_Red2_1", redball[1] == 1 ? 0 : (last == null ? 1 : last.O_Red2_1 + 1));
            dic.Add("O_Red2_2", redball[1] == 2 ? 0 : (last == null ? 1 : last.O_Red2_2 + 1));
            dic.Add("O_Red3_0", redball[2] == 0 ? 0 : (last == null ? 1 : last.O_Red3_0 + 1));
            dic.Add("O_Red3_1", redball[2] == 1 ? 0 : (last == null ? 1 : last.O_Red3_1 + 1));
            dic.Add("O_Red3_2", redball[2] == 2 ? 0 : (last == null ? 1 : last.O_Red3_2 + 1));
            dic.Add("O_Red4_0", redball[3] == 0 ? 0 : (last == null ? 1 : last.O_Red4_0 + 1));
            dic.Add("O_Red4_1", redball[3] == 1 ? 0 : (last == null ? 1 : last.O_Red4_1 + 1));
            dic.Add("O_Red4_2", redball[3] == 2 ? 0 : (last == null ? 1 : last.O_Red4_2 + 1));
            dic.Add("O_Red5_0", redball[4] == 0 ? 0 : (last == null ? 1 : last.O_Red5_0 + 1));
            dic.Add("O_Red5_1", redball[4] == 1 ? 0 : (last == null ? 1 : last.O_Red5_1 + 1));
            dic.Add("O_Red5_2", redball[4] == 2 ? 0 : (last == null ? 1 : last.O_Red5_2 + 1));
            dic.Add("O_Red6_0", redball[5] == 0 ? 0 : (last == null ? 1 : last.O_Red6_0 + 1));
            dic.Add("O_Red6_1", redball[5] == 1 ? 0 : (last == null ? 1 : last.O_Red6_1 + 1));
            dic.Add("O_Red6_2", redball[5] == 2 ? 0 : (last == null ? 1 : last.O_Red6_2 + 1));
            dic.Add("YS_Proportion", string.Format("{0}:{1}:{2}", Y0_Number, Y1_Number, Y2_Number));
            dic.Add("YS_Qualifying", string.Format("{0}{1}{2}{3}{4}{5}", redball[0], redball[1], redball[2], redball[3], redball[4], redball[5]));
            var entity = this.CreateNewEntity<SSQ_C3>(dic, (p) =>
            {
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (p.Name.StartsWith("Y0_Number"))
                {
                    var value = int.Parse(p.Name.Replace("Y0_Number", string.Empty));
                    lastValue = Y0_Number == value ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Y1_Number"))
                {
                    var value = int.Parse(p.Name.Replace("Y1_Number", string.Empty));
                    lastValue = Y1_Number == value ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Y2_Number"))
                {
                    var value = int.Parse(p.Name.Replace("Y2_Number", string.Empty));
                    lastValue = Y2_Number == value ? 0 : lastValue;
                }
                return lastValue;
            });
            //entity.YS_Proportion = string.Format("{0}:{1}:{2}",Y0_Number,Y1_Number,Y2_Number);
            //entity.YS_Qualifying = string.Format("{0}{1}{2}{3}{4}{5}", redball[0], redball[1], redball[2], redball[3], redball[4], redball[5]);
            manager.AddSSQ_C3(entity);

        }

        /// <summary>
        /// 添加质合走势
        /// </summary>
        private void AddSSQ_ZhiHe(string issuseNumber, string winNumber)
        {
            var manager = new SSQ_ZhiHeManager();
            var issuse = manager.QuerySSQ_ZhiHeIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            if (winNumber.IndexOf("|") == -1)
                throw new Exception("开奖号码不正确！");
            //取红球
            var redBall = winNumber.Split('|')[0];
            var ball = redBall.Split(',');
            var redball = new string[]{
                (Convert.ToInt32(ball[0])==1 ||Convert.ToInt32(ball[0])==2 ||Convert.ToInt32(ball[0])==3 ||Convert.ToInt32(ball[0])==5||Convert.ToInt32(ball[0])==7||Convert.ToInt32(ball[0])==11||Convert.ToInt32(ball[0])==13||Convert.ToInt32(ball[0])==17||Convert.ToInt32(ball[0])==19||Convert.ToInt32(ball[0])==23||Convert.ToInt32(ball[0])==29||Convert.ToInt32(ball[0])==31) ? "质" : "合",
                (Convert.ToInt32(ball[0])==1 ||Convert.ToInt32(ball[1])==2 ||Convert.ToInt32(ball[1])==3 ||Convert.ToInt32(ball[1])==5||Convert.ToInt32(ball[1])==7||Convert.ToInt32(ball[1])==11||Convert.ToInt32(ball[1])==13||Convert.ToInt32(ball[1])==17||Convert.ToInt32(ball[1])==19||Convert.ToInt32(ball[1])==23||Convert.ToInt32(ball[1])==29||Convert.ToInt32(ball[1])==31) ? "质" : "合",
                (Convert.ToInt32(ball[0])==1 ||Convert.ToInt32(ball[2])==2 ||Convert.ToInt32(ball[2])==3 ||Convert.ToInt32(ball[2])==5||Convert.ToInt32(ball[2])==7||Convert.ToInt32(ball[2])==11||Convert.ToInt32(ball[2])==13||Convert.ToInt32(ball[2])==17||Convert.ToInt32(ball[2])==19||Convert.ToInt32(ball[2])==23||Convert.ToInt32(ball[2])==29||Convert.ToInt32(ball[2])==31) ? "质" : "合",
                (Convert.ToInt32(ball[0])==1 ||Convert.ToInt32(ball[3])==2 ||Convert.ToInt32(ball[3])==3 ||Convert.ToInt32(ball[3])==5||Convert.ToInt32(ball[3])==7||Convert.ToInt32(ball[3])==11||Convert.ToInt32(ball[3])==13||Convert.ToInt32(ball[3])==17||Convert.ToInt32(ball[3])==19||Convert.ToInt32(ball[3])==23||Convert.ToInt32(ball[3])==29||Convert.ToInt32(ball[3])==31) ? "质" : "合",
                (Convert.ToInt32(ball[0])==1 ||Convert.ToInt32(ball[4])==2 ||Convert.ToInt32(ball[4])==3 ||Convert.ToInt32(ball[4])==5||Convert.ToInt32(ball[4])==7||Convert.ToInt32(ball[4])==11||Convert.ToInt32(ball[4])==13||Convert.ToInt32(ball[4])==17||Convert.ToInt32(ball[4])==19||Convert.ToInt32(ball[4])==23||Convert.ToInt32(ball[4])==29||Convert.ToInt32(ball[4])==31) ? "质" : "合",
                (Convert.ToInt32(ball[0])==1 ||Convert.ToInt32(ball[5])==2 ||Convert.ToInt32(ball[5])==3 ||Convert.ToInt32(ball[5])==5||Convert.ToInt32(ball[5])==7||Convert.ToInt32(ball[5])==11||Convert.ToInt32(ball[5])==13||Convert.ToInt32(ball[5])==17||Convert.ToInt32(ball[5])==19||Convert.ToInt32(ball[5])==23||Convert.ToInt32(ball[5])==29||Convert.ToInt32(ball[5])==31) ? "质" : "合"
            };
            //奇偶比例
            var ZH_Proportion = string.Empty;
            //奇偶排列
            var ZH_Qualifying = string.Empty;
            int z = 0;
            int h = 0;
            foreach (var item in ball)
            {
                if (Convert.ToInt32(item) == 1 || Convert.ToInt32(item) == 2 || Convert.ToInt32(item) == 3 || Convert.ToInt32(item) == 5 || Convert.ToInt32(item) == 7 || Convert.ToInt32(item) == 11 || Convert.ToInt32(item) == 13 || Convert.ToInt32(item) == 17 || Convert.ToInt32(item) == 19 || Convert.ToInt32(item) == 23 || Convert.ToInt32(item) == 29 || Convert.ToInt32(item) == 31)
                {
                    if (z == 0 && h == 0)
                    {
                        ZH_Qualifying += "质";
                        z++;
                    }
                    else
                    {
                        ZH_Qualifying += ",质";
                        z++;
                    }
                }
                else
                {
                    if (z == 0 && h == 0)
                    {
                        ZH_Qualifying += "合";
                        h++;
                    }
                    else
                    {
                        ZH_Qualifying += ",合";
                        h++;
                    }
                }
            }
            ZH_Proportion = z.ToString() + ":" + h.ToString();

            var last = manager.QueryLastSSQ_ZhiHe();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", redball[0]);
            dic.Add("RedBall2", redball[1]);
            dic.Add("RedBall3", redball[2]);
            dic.Add("RedBall4", redball[3]);
            dic.Add("RedBall5", redball[4]);
            dic.Add("RedBall6", redball[5]);
            dic.Add("ZH_Qualifying", ZH_Qualifying);
            dic.Add("ZH_Proportion", ZH_Proportion);
            dic.Add("RedLotteryNumber", redBall);
            var entity = this.CreateNewEntity<SSQ_ZhiHe>(dic, (p) =>
            {
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (p.Name.EndsWith("_Z"))
                {
                    var value = p.Name.Replace("_Z", string.Empty).Replace("O_", string.Empty);
                    var num = int.Parse(value.Replace("RedBall", string.Empty).ToString());
                    if (last != null)
                    {
                        lastValue = (redball[num - 1] == "合") ? lastValue : 0;
                    }
                    else
                    {
                        lastValue = redball[num - 1] != "合" ? 0 : lastValue;
                    }
                }
                if (p.Name.EndsWith("_H"))
                {
                    var value = p.Name.Replace("_H", string.Empty).Replace("O_", string.Empty);
                    var num = int.Parse(value.Replace("RedBall", string.Empty).ToString());
                    if (last != null)
                    {
                        lastValue = (redball[num - 1] == "质") ? lastValue : 0;
                    }
                    else
                    {
                        lastValue = redball[num - 1] != "质" ? 0 : lastValue;
                    }
                }
                if (p.Name.StartsWith("O_ZH_"))
                {
                    var value = p.Name.Replace("O_ZH_Proportion", string.Empty);
                    if (last != null)
                    {
                        lastValue = (ZH_Proportion.Replace(":", string.Empty) != value) ? lastValue : 0;
                    }
                    else
                    {
                        lastValue = (ZH_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                    }
                }
                return lastValue;
            });

            manager.AddSSQ_ZhiHe(entity);

        }

        /// <summary>
        /// 添加基本走势
        /// </summary>
        private void AddSSQ_JiBenZouSi(string issuseNumber, string winNumber)
        {
            var manager = new SSQ_JiBenZouSiManager();
            var issuse = manager.QuerySSQ_JiBenZouSiIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var winRed = winArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var winBlue = winArray[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var last = manager.QueryLastSSQ_JiBenZouSi();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<SSQ_JiBenZouSi>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red"))
                {
                    var order = p.Name.Replace("Red", string.Empty);
                    lastValue = winRed.Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Blue"))
                {
                    var order = p.Name.Replace("Blue", string.Empty);
                    lastValue = winBlue.Contains(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddSSQ_JiBenZouSi(entity);
        }

        /// <summary>
        /// 添加大小走势
        /// </summary>
        private void AddSSQ_DX(string issuseNumber, string winNumber)
        {
            var manager = new SSQ_DXManager();
            var issuse = manager.QuerySSQ_DXIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            if (winNumber.IndexOf("|") == -1)
                throw new Exception("开奖号码不正确！");
            //取红球
            var redBall = winNumber.Split('|')[0];
            var ball = redBall.Split(',');
            var redball = new string[]{
                Convert.ToInt32(ball[0]) > 16 ? "大" : "小",
                Convert.ToInt32(ball[1]) > 16 ? "大" : "小",
                Convert.ToInt32(ball[2]) > 16 ? "大" : "小",
                Convert.ToInt32(ball[3]) > 16 ? "大" : "小",
                Convert.ToInt32(ball[4]) > 16 ? "大" : "小",
                Convert.ToInt32(ball[5]) > 16 ? "大" : "小"
            };
            //大小比例
            var DX_Proportion = string.Empty;
            //大小排列
            var DX_Qualifying = string.Empty;
            if (redball[0] == "大")
            {
                DX_Proportion = "6:0";
                DX_Qualifying = "大,大,大,大,大,大";
            }
            else if (redball[1] == "大" && redball[0] == "小")
            {
                DX_Proportion = "5:1";
                DX_Qualifying = "小,大,大,大,大,大";
            }
            else if (redball[2] == "大" && redball[1] == "小")
            {
                DX_Proportion = "4:2";
                DX_Qualifying = "小,小,大,大,大,大";
            }
            else if (redball[3] == "大" && redball[2] == "小")
            {
                DX_Proportion = "3:3";
                DX_Qualifying = "小,小,小,大,大,大";
            }
            else if (redball[4] == "大" && redball[3] == "小")
            {
                DX_Proportion = "2:4";
                DX_Qualifying = "小,小,小,小,大,大";
            }
            else if (redball[5] == "大" && redball[4] == "小")
            {
                DX_Proportion = "1:5";
                DX_Qualifying = "小,小,小,小,小,大";
            }
            else if (redball[5] == "小")
            {
                DX_Proportion = "0:6";
                DX_Qualifying = "小,小,小,小,小,小";
            }
            var last = manager.QueryLastSSQ_DX();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", ball[0]);
            dic.Add("RedBall2", ball[1]);
            dic.Add("RedBall3", ball[2]);
            dic.Add("RedBall4", ball[3]);
            dic.Add("RedBall5", ball[4]);
            dic.Add("RedBall6", ball[5]);
            dic.Add("DX_Qualifying", DX_Qualifying);
            dic.Add("DX_Proportion", DX_Proportion);
            dic.Add("RedLotteryNumber", redBall);
            var entity = this.CreateNewEntity<SSQ_DX>(dic, (p) =>
            {
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (p.Name.EndsWith("_D"))
                {
                    var value = p.Name.Replace("_D", string.Empty).Replace("O_", string.Empty);
                    var num = int.Parse(value.Replace("RedBall", string.Empty).ToString());
                    if (last != null)
                    {
                        //var zz = last.GetType().GetProperty(value).GetValue(last, null).ToString();
                        //lastValue = (redball[num - 1] == "小" && redball[num - 1] == zz) ? lastValue : ((redball[num - 1] != zz) ? 1 : 0);
                        lastValue = (redball[num - 1] == "小") ? lastValue : 0;
                    }
                    else
                    {
                        lastValue = redball[num - 1] != "小" ? 0 : lastValue;
                    }
                }
                if (p.Name.EndsWith("_X"))
                {
                    var value = p.Name.Replace("_X", string.Empty).Replace("O_", string.Empty);
                    var num = int.Parse(value.Replace("RedBall", string.Empty).ToString());
                    if (last != null)
                    {
                        //var zz = last.GetType().GetProperty(value).GetValue(last, null).ToString();
                        //lastValue = (redball[num - 1] == "大" && redball[num - 1] == zz) ? lastValue : ((redball[num - 1] != zz) ? 1 : 0);
                        lastValue = (redball[num - 1] == "大") ? lastValue : 0;
                    }
                    else
                    {
                        lastValue = redball[num - 1] != "大" ? 0 : lastValue;
                    }
                }
                if (p.Name.StartsWith("O_DX_"))
                {
                    var value = p.Name.Replace("O_DX_Proportion", string.Empty);
                    if (last != null)
                    {
                        lastValue = (DX_Proportion.Replace(":", string.Empty) != value) ? lastValue : 0;
                    }
                    else
                    {
                        lastValue = (DX_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                    }
                }
                return lastValue;
            });

            manager.AddSSQ_DX(entity);
        }

        /// <summary>
        /// 添加奇偶走势
        /// </summary>
        private void AddSSQ_JiOu(string issuseNumber, string winNumber)
        {
            var manager = new SSQ_JiOuManager();
            var issuse = manager.QuerySSQ_JiOuIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            if (winNumber.IndexOf("|") == -1)
                throw new Exception("开奖号码不正确！");
            //取红球
            var redBall = winNumber.Split('|')[0];
            var ball = redBall.Split(',');
            var redball = new string[]{
                Convert.ToInt32(ball[0])% 2 == 0 ? "偶" : "奇",
                Convert.ToInt32(ball[1])% 2 == 0 ? "偶" : "奇",
                Convert.ToInt32(ball[2])% 2 == 0 ? "偶" : "奇",
                Convert.ToInt32(ball[3])% 2 == 0 ? "偶" : "奇",
                Convert.ToInt32(ball[4])% 2 == 0 ? "偶" : "奇",
                Convert.ToInt32(ball[5])% 2 == 0 ? "偶" : "奇"
            };
            //奇偶比例
            var JO_Proportion = string.Empty;
            //奇偶排列
            var JO_Qualifying = string.Empty;
            int j = 0;
            int o = 0;
            foreach (var item in ball)
            {
                if (Convert.ToInt32(item) % 2 == 0)
                {
                    if (j == 0 && o == 0)
                    {
                        JO_Qualifying += "偶";
                        o++;
                    }
                    else
                    {
                        JO_Qualifying += ",偶";
                        o++;
                    }
                }
                else
                {
                    if (j == 0 && o == 0)
                    {
                        JO_Qualifying += "奇";
                        j++;
                    }
                    else
                    {
                        JO_Qualifying += ",奇";
                        j++;
                    }
                }
            }
            JO_Proportion = j.ToString() + ":" + o.ToString();

            var last = manager.QueryLastSSQ_JiOu();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", ball[0]);
            dic.Add("RedBall2", ball[1]);
            dic.Add("RedBall3", ball[2]);
            dic.Add("RedBall4", ball[3]);
            dic.Add("RedBall5", ball[4]);
            dic.Add("RedBall6", ball[5]);
            dic.Add("JO_Qualifying", JO_Qualifying);
            dic.Add("JO_Proportion", JO_Proportion);
            dic.Add("RedLotteryNumber", redBall);
            var entity = this.CreateNewEntity<SSQ_JiOu>(dic, (p) =>
            {
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (p.Name.EndsWith("_J"))
                {
                    var value = p.Name.Replace("_J", string.Empty).Replace("O_", string.Empty);
                    var num = int.Parse(value.Replace("RedBall", string.Empty).ToString());
                    if (last != null)
                    {
                        //var zz = last.GetType().GetProperty(value).GetValue(last, null).ToString();
                        //lastValue = (redball[num - 1] == "小" && redball[num - 1] == zz) ? lastValue : ((redball[num - 1] != zz) ? 1 : 0);
                        lastValue = (redball[num - 1] == "偶") ? lastValue : 0;
                    }
                    else
                    {
                        lastValue = redball[num - 1] != "偶" ? 0 : lastValue;
                    }
                }
                if (p.Name.EndsWith("_O"))
                {
                    var value = p.Name.Replace("_O", string.Empty).Replace("O_", string.Empty);
                    var num = int.Parse(value.Replace("RedBall", string.Empty).ToString());
                    if (last != null)
                    {
                        //var zz = last.GetType().GetProperty(value).GetValue(last, null).ToString();
                        //lastValue = (redball[num - 1] == "大" && redball[num - 1] == zz) ? lastValue : ((redball[num - 1] != zz) ? 1 : 0);
                        lastValue = (redball[num - 1] == "奇") ? lastValue : 0;
                    }
                    else
                    {
                        lastValue = redball[num - 1] != "奇" ? 0 : lastValue;
                    }
                }
                if (p.Name.StartsWith("O_JO_"))
                {
                    var value = p.Name.Replace("O_JO_Proportion", string.Empty);
                    if (last != null)
                    {
                        lastValue = (JO_Proportion.Replace(":", string.Empty) != value) ? lastValue : 0;
                    }
                    else
                    {
                        lastValue = (JO_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                    }
                }
                return lastValue;
            });
            manager.AddSSQ_JiOu(entity);

        }

        #endregion

        #region 前台查询函数

        /// <summary>
        /// 基本走势查询
        /// </summary>
        public SSQ_JiBenZouSi_InfoCollection QuerySSQ_JiBenZouSi(int index)
        {
            SSQ_JiBenZouSi_InfoCollection Collection = new SSQ_JiBenZouSi_InfoCollection();

            var list = this.QueryGameChart<SSQ_JiBenZouSi_Info>(string.Format("QuerySSQ_JiBenZouSi_{0}", index), () =>
            {
                var infoList = new List<SSQ_JiBenZouSi_Info>();
                var entityList = new SSQ_JiBenZouSiManager().QuerySSQ_JiBenZouSi(index);

               ObjectConvert.ConvertEntityListToInfoList<List<SSQ_JiBenZouSi>, SSQ_JiBenZouSi, List<SSQ_JiBenZouSi_Info>, SSQ_JiBenZouSi_Info>(entityList, ref infoList,
                    () => { return new SSQ_JiBenZouSi_Info(); },
                    (entity, info) =>
                    {
                        //处理info里面有，页entity里面没有的属性
                        //info.WinNumber = entity.WinNumber;
                    });

                return infoList;
            });



            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        /// 大小走势查询
        /// </summary>
        public SSQ_DX_InfoCollection QuerySSQ_DX(int index)
        {
            SSQ_DX_InfoCollection Collection = new SSQ_DX_InfoCollection();
            var list = this.QueryGameChart<SSQ_DX_Info>(string.Format("QuerySSQ_DX_{0}", index), () =>
            {
                var infoList = new List<SSQ_DX_Info>();
                var entityList = new SSQ_DXManager().QuerySSQ_DX(index);

               ObjectConvert.ConvertEntityListToInfoList<List<SSQ_DX>, SSQ_DX, List<SSQ_DX_Info>, SSQ_DX_Info>(entityList, ref infoList,
                    () => { return new SSQ_DX_Info(); },
                    (entity, info) =>
                    {
                        //处理info里面有，页entity里面没有的属性
                        //info.WinNumber = entity.WinNumber;
                    });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        /// C3走势查询
        /// </summary>
        public SSQ_C3_InfoCollection QuerySSQ_C3(int index)
        {
            SSQ_C3_InfoCollection Collection = new SSQ_C3_InfoCollection();
            var list = this.QueryGameChart<SSQ_C3_Info>(string.Format("QuerySSQ_C3_{0}", index), () =>
            {
                var infoList = new List<SSQ_C3_Info>();
                var entityList = new SSQ_C3Manager().QuerySSQ_C3(index);

               ObjectConvert.ConvertEntityListToInfoList<List<SSQ_C3>, SSQ_C3, List<SSQ_C3_Info>, SSQ_C3_Info>(entityList, ref infoList,
                    () => { return new SSQ_C3_Info(); },
                    (entity, info) =>
                    {
                        //处理info里面有，页entity里面没有的属性
                        //info.WinNumber = entity.WinNumber;
                    });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        /// 和值走势查询
        /// </summary>
        public SSQ_HeZhi_InfoCollection QuerySSQ_HeZhi(int index)
        {
            SSQ_HeZhi_InfoCollection Collection = new SSQ_HeZhi_InfoCollection();
            var list = this.QueryGameChart<SSQ_HeZhi_Info>(string.Format("QuerySSQ_HeZhi_{0}", index), () =>
            {
                var infoList = new List<SSQ_HeZhi_Info>();
                var entityList = new SSQ_HeZhiManager().QuerySSQ_HeZhi(index);

               ObjectConvert.ConvertEntityListToInfoList<List<SSQ_HeZhi>, SSQ_HeZhi, List<SSQ_HeZhi_Info>, SSQ_HeZhi_Info>(entityList, ref infoList,
                    () => { return new SSQ_HeZhi_Info(); },
                    (entity, info) =>
                    {
                        //处理info里面有，页entity里面没有的属性
                        //info.WinNumber = entity.WinNumber;
                    });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        /// 奇偶走势查询
        /// </summary>
        public SSQ_JiOu_InfoCollection QuerySSQ_JiOu(int index)
        {
            SSQ_JiOu_InfoCollection Collection = new SSQ_JiOu_InfoCollection();
            var list = this.QueryGameChart<SSQ_JiOu_Info>(string.Format("QuerySSQ_JiOu_{0}", index), () =>
            {
                var infoList = new List<SSQ_JiOu_Info>();
                var entityList = new SSQ_JiOuManager().QuerySSQ_JiOu(index);

               ObjectConvert.ConvertEntityListToInfoList<List<SSQ_JiOu>, SSQ_JiOu, List<SSQ_JiOu_Info>, SSQ_JiOu_Info>(entityList, ref infoList,
                    () => { return new SSQ_JiOu_Info(); },
                    (entity, info) =>
                    {
                        //处理info里面有，页entity里面没有的属性
                        //info.WinNumber = entity.WinNumber;
                    });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        /// 跨度1_6走势查询
        /// </summary>
        public SSQ_KuaDu_1_6_InfoCollection QuerySSQ_KuaDu_1_6(int index)
        {
            SSQ_KuaDu_1_6_InfoCollection Collection = new SSQ_KuaDu_1_6_InfoCollection();
            var list = this.QueryGameChart<SSQ_KuaDu_1_6_Info>(string.Format("QuerySSQ_KuaDu_1_6_{0}", index), () =>
            {
                var infoList = new List<SSQ_KuaDu_1_6_Info>();
                var entityList = new SSQ_KuaDu_1_6Manager().QuerySSQ_KuaDu_1_6(index);

               ObjectConvert.ConvertEntityListToInfoList<List<SSQ_KuaDu_1_6>, SSQ_KuaDu_1_6, List<SSQ_KuaDu_1_6_Info>, SSQ_KuaDu_1_6_Info>(entityList, ref infoList,
                    () => { return new SSQ_KuaDu_1_6_Info(); },
                    (entity, info) =>
                    {
                        //处理info里面有，页entity里面没有的属性
                        //info.WinNumber = entity.WinNumber;
                    });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        /// 跨度SW走势查询
        /// </summary>
        public SSQ_KuaDu_SW_InfoCollection QuerySSQ_KuaDu_SW(int index)
        {
            SSQ_KuaDu_SW_InfoCollection Collection = new SSQ_KuaDu_SW_InfoCollection();
            var list = this.QueryGameChart<SSQ_KuaDu_SW_Info>(string.Format("QuerySSQ_KuaDu_SW_{0}", index), () =>
            {
                var infoList = new List<SSQ_KuaDu_SW_Info>();
                var entityList = new SSQ_KuaDu_SWManager().QuerySSQ_KuaDu_SW(index);

               ObjectConvert.ConvertEntityListToInfoList<List<SSQ_KuaDu_SW>, SSQ_KuaDu_SW, List<SSQ_KuaDu_SW_Info>, SSQ_KuaDu_SW_Info>(entityList, ref infoList,
                    () => { return new SSQ_KuaDu_SW_Info(); },
                    (entity, info) =>
                    {
                        //处理info里面有，页entity里面没有的属性
                        //info.WinNumber = entity.WinNumber;
                    });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        /// 质合走势查询
        /// </summary>
        public SSQ_ZhiHe_InfoCollection QuerySSQ_ZhiHe(int index)
        {
            SSQ_ZhiHe_InfoCollection Collection = new SSQ_ZhiHe_InfoCollection();
            var list = this.QueryGameChart<SSQ_ZhiHe_Info>(string.Format("QuerySSQ_ZhiHe_{0}", index), () =>
            {
                var infoList = new List<SSQ_ZhiHe_Info>();
                var entityList = new SSQ_ZhiHeManager().QuerySSQ_ZhiHe(index);

               ObjectConvert.ConvertEntityListToInfoList<List<SSQ_ZhiHe>, SSQ_ZhiHe, List<SSQ_ZhiHe_Info>, SSQ_ZhiHe_Info>(entityList, ref infoList,
                    () => { return new SSQ_ZhiHe_Info(); },
                    (entity, info) =>
                    {
                        //处理info里面有，页entity里面没有的属性
                        //info.WinNumber = entity.WinNumber;
                    });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        #endregion

        /// <summary>
        /// 查询开奖数据
        /// </summary>
        public GameWinNumber_InfoCollection QuerySSQ_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new SSQ_GameWinNumberManager().QuerySSQ_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<SSQ_GameWinNumber>, SSQ_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QuerySSQ_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new SSQ_GameWinNumberManager().QuerySSQ_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<SSQ_GameWinNumber>, SSQ_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
            //        () => { return new GameWinNumber_Info(); },
            //        (entity, info) =>
            //        {
            //            //处理info里面有，页entity里面没有的属性
            //            //info.WinNumber = entity.WinNumber;
            //        });
            //    collection.TotalCount = totalCount;
            //    collection.List.AddRange(infoList);
            //    return collection;
            //});
        }
        public GameWinNumber_InfoCollection QuerySSQ_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new SSQ_GameWinNumberManager().QuerySSQ_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<SSQ_GameWinNumber>, SSQ_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QuerySSQ_GameWinNumber_{0}_{1}_{2}_{3}", pageIndex, pageSize, startTime.ToString("yyyyMMdd"), endTime.ToString("yyyyMMdd"));
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new SSQ_GameWinNumberManager().QuerySSQ_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<SSQ_GameWinNumber>, SSQ_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
            //        () => { return new GameWinNumber_Info(); },
            //        (entity, info) =>
            //        {
            //            //处理info里面有，页entity里面没有的属性
            //            //info.WinNumber = entity.WinNumber;
            //        });
            //    collection.TotalCount = totalCount;
            //    collection.List.AddRange(infoList);
            //    return collection;
            //});
        }

        public GameWinNumber_Info QueryWinNumber(string issuseNumber)
        {
            var manager = new SSQ_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<SSQ_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }

    }
}
