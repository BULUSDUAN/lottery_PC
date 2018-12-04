using EntityModel.CoreModel;
using KaSon.FrameWork.ORM.Helper.WinNumber.Manage;
using EntityModel;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using KaSon.FrameWork.Analyzer.AnalyzerFactory;
using KaSon.FrameWork.Common.Utilities;

namespace KaSon.FrameWork.ORM.Helper.WinNumber
{
    public class LotteryDataBusiness_CQSSC : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get
            {
                return "CQSSC";
            }
        }

        public void ImportWinNumber(string issuseNumber, string winNumber)
        {
            if (string.IsNullOrEmpty(issuseNumber)) return;
            if (string.IsNullOrEmpty(winNumber)) return;


            var msg = string.Empty;
            AnalyzerFactory.GetWinNumberAnalyzer(this.CurrentGameCode).CheckWinNumber(winNumber, out msg);
            if (!string.IsNullOrEmpty(msg))
                throw new Exception(msg);

            //开启事务
            using (LottertDataDB)
            {
                LottertDataDB.Begin();
                this.ClearGameChartCache("QueryCQSSC_1X_ZS");
                this.ClearGameChartCache("QueryCQSSC_2X_HZZS");
                this.ClearGameChartCache("QueryCQSSC_2X_ZuXZS");
                this.ClearGameChartCache("QueryCQSSC_2X_ZXZS");
                this.ClearGameChartCache("QueryCQSSC_3X_C3YS");
                this.ClearGameChartCache("QueryCQSSC_3X_DXZS");
                this.ClearGameChartCache("QueryCQSSC_3X_HZZS");
                this.ClearGameChartCache("QueryCQSSC_3X_JOZS");
                this.ClearGameChartCache("QueryCQSSC_3X_KD");
                this.ClearGameChartCache("QueryCQSSC_3X_ZHZS");
                this.ClearGameChartCache("QueryCQSSC_3X_ZuXZS");
                this.ClearGameChartCache("QueryCQSSC_3X_ZXZS");
                this.ClearGameChartCache("QueryCQSSC_5X_HZZS");
                this.ClearGameChartCache("QueryCQSSC_5X_JBZS");
                this.ClearGameChartCache("QueryCQSSC_DXDS");
                this.ClearNewWinNumberCache("QueryCQSSC_GameWinNumber");

                AddCQSSC_DXDS(issuseNumber, winNumber);
                AddCQSSC_1X_ZS(issuseNumber, winNumber);
                AddCQSSC_5X_HZZS(issuseNumber, winNumber);
                AddCQSSC_5X_JBZS(issuseNumber, winNumber);
                AddCQSSC_2X_HZZS(issuseNumber, winNumber);
                AddCQSSC_2X_ZXZS(issuseNumber, winNumber);
                AddCQSSC_2X_ZuXZS(issuseNumber, winNumber);
                AddCQSSC_3X_ZXZS(issuseNumber, winNumber);
                AddCQSSC_3X_ZuXZS(issuseNumber, winNumber);
                AddCQSSC_3X_HZZS(issuseNumber, winNumber);
                AddCQSSC_3X_DXZS(issuseNumber, winNumber);
                AddCQSSC_3X_JOZS(issuseNumber, winNumber);
                AddCQSSC_3X_ZHZS(issuseNumber, winNumber);
                AddCQSSC_3X_C3YS(issuseNumber, winNumber);
                AddCQSSC_3X_KD(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }

        #region 生成走势数据

        public void Add_GameWinNumber(string issuseNumber, string winNumber)
        {
            new KJGameIssuseBusiness().IssusePrize(this.CurrentGameCode, issuseNumber, winNumber);
            var manager = new CQSSC_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddCQSSC_GameWinNumber(new CQSSC_GameWinNumber
            {
                GameCode = this.CurrentGameCode,
                IssuseNumber = issuseNumber,
                WinNumber = winNumber,
                CreateTime = DateTime.Now,
            });
        }

        /// <summary>
        /// 1星走势
        /// </summary>
        public void AddCQSSC_DXDS(string issuseNumber, string winNumber)
        {
            var manager = new CQSSC_DXDSManager();
            var issuse = manager.QueryCQSSC_DXDSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            string num = winNumber.Substring(6);
            var winRed = num.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var DaXiao_S = Convert.ToInt32(winRed[0]) > 4 ? "大" : "小";
            var DanS_S = Convert.ToInt32(winRed[0]) % 2 == 0 ? "双" : "单";
            var DaXiao_G = Convert.ToInt32(winRed[1]) > 4 ? "大" : "小";
            var DanS_G = Convert.ToInt32(winRed[1]) % 2 == 0 ? "双" : "单";

            var DXDS1 = DaXiao_S + DaXiao_G;
            var DXDS2 = DaXiao_S + DanS_G;
            var DXDS3 = DanS_S + DaXiao_G;
            var DXDS4 = DanS_S + DanS_G;

            var last = manager.QueryLastCQSSC_DXDS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("DD", DXDS1 == "大大" ? 0 : (last == null ? 1 : last.DD + 1));
            dic.Add("DX", DXDS1 == "大小" ? 0 : (last == null ? 1 : last.DX + 1));
            dic.Add("DDan", DXDS2 == "大单" ? 0 : (last == null ? 1 : last.DDan + 1));
            dic.Add("DS", DXDS2 == "大双" ? 0 : (last == null ? 1 : last.DS + 1));
            dic.Add("XD", DXDS1 == "小大" ? 0 : (last == null ? 1 : last.XD + 1));
            dic.Add("XX", DXDS1 == "小小" ? 0 : (last == null ? 1 : last.XX + 1));
            dic.Add("XDan", DXDS2 == "小单" ? 0 : (last == null ? 1 : last.XDan + 1));
            dic.Add("XS", DXDS2 == "小双" ? 0 : (last == null ? 1 : last.XS + 1));
            dic.Add("DanD", DXDS3 == "单大" ? 0 : (last == null ? 1 : last.DanD + 1));
            dic.Add("DanX", DXDS3 == "单小" ? 0 : (last == null ? 1 : last.DanX + 1));
            dic.Add("DanDan", DXDS4 == "单单" ? 0 : (last == null ? 1 : last.DanDan + 1));
            dic.Add("DanS", DXDS4 == "单双" ? 0 : (last == null ? 1 : last.DanS + 1));
            dic.Add("SD", DXDS3 == "双大" ? 0 : (last == null ? 1 : last.SD + 1));
            dic.Add("SX", DXDS3 == "双小" ? 0 : (last == null ? 1 : last.SX + 1));
            dic.Add("SDan", DXDS4 == "双单" ? 0 : (last == null ? 1 : last.SDan + 1));
            dic.Add("SS", DXDS4 == "双双" ? 0 : (last == null ? 1 : last.SS + 1));
            var entity = this.CreateNewEntity<CQSSC_DXDS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("D_Red_S"))
                {
                    lastValue = DaXiao_S == "大" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("X_Red_S"))
                {
                    lastValue = DaXiao_S == "小" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Dan_Red_S"))
                {
                    lastValue = DanS_S == "单" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("S_Red_S"))
                {
                    lastValue = DanS_S == "双" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("D_Red_G"))
                {
                    lastValue = DaXiao_G == "大" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("X_Red_G"))
                {
                    lastValue = DaXiao_G == "小" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Dan_Red_G"))
                {
                    lastValue = DanS_G == "单" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("S_Red_G"))
                {
                    lastValue = DanS_G == "双" ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQSSC_DXDS(entity);
        }
        /// <summary>
        /// 1星走势
        /// </summary>
        public void AddCQSSC_1X_ZS(string issuseNumber, string winNumber)
        {
            var manager = new CQSSC_1X_ZSManager();
            var issuse = manager.QueryCQSSC_1X_ZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Substring(8);
            var DaXiao = Convert.ToInt32(winRed) > 4 ? "大" : "小";
            var JiOu = Convert.ToInt32(winRed) % 2 == 0 ? "偶" : "奇";
            var ZhiHe = (Convert.ToInt32(winRed) == 1 || Convert.ToInt32(winRed) == 2 || Convert.ToInt32(winRed) == 3 || Convert.ToInt32(winRed) == 5 || Convert.ToInt32(winRed) == 7) ? "质" : "合";

            var C3 = Convert.ToInt32(winRed) % 3;

            var last = manager.QueryLastCQSSC_1X_ZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed);
            dic.Add("O_Red1_0", C3 == 0 ? 0 : (last == null ? 1 : last.O_Red1_0 + 1));
            dic.Add("O_Red1_1", C3 == 1 ? 0 : (last == null ? 1 : last.O_Red1_1 + 1));
            dic.Add("O_Red1_2", C3 == 2 ? 0 : (last == null ? 1 : last.O_Red1_2 + 1));
            var entity = this.CreateNewEntity<CQSSC_1X_ZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_G"))
                {
                    var order = p.Name.Replace("Red_G", string.Empty);
                    lastValue = winRed == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("D_Red"))
                {
                    lastValue = DaXiao == "大" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("X_Red"))
                {
                    lastValue = DaXiao == "小" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("J_Red"))
                {
                    lastValue = JiOu == "奇" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_Red"))
                {
                    lastValue = JiOu == "偶" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Z_Red"))
                {
                    lastValue = ZhiHe == "质" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("H_Red"))
                {
                    lastValue = ZhiHe == "合" ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQSSC_1X_ZS(entity);
        }
        /// <summary>
        /// 5星和值走势
        /// </summary>
        public void AddCQSSC_5X_HZZS(string issuseNumber, string winNumber)
        {
            var manager = new CQSSC_5X_HZZSManager();
            var issuse = manager.QueryCQSSC_5X_HZZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]) + int.Parse(winRed[3]) + int.Parse(winRed[4]);
            var last = manager.QueryLastCQSSC_5X_HZZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("RedBall4", winRed[3]);
            dic.Add("RedBall5", winRed[4]);
            dic.Add("HeZhi", hz);
            var entity = this.CreateNewEntity<CQSSC_5X_HZZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("HZ_"))
                {
                    var order = p.Name.Replace("HZ_", string.Empty);
                    lastValue = hz == Convert.ToInt32(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQSSC_5X_HZZS(entity);
        }

        /// <summary>
        /// 5星基本走势
        /// </summary>
        private void AddCQSSC_5X_JBZS(string issuseNumber, string winNumber)
        {
            var manager = new CQSSC_5X_JBZSManager();
            var issuse = manager.QueryCQSSC_5X_JBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string DX_Proportion = string.Empty;
            string JO_Proportion = string.Empty;

            int d = 0;
            int x = 0;
            int j = 0;
            int o = 0;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) > 4)
                {
                    d++;
                }
                else
                {
                    x++;
                }

                if (Convert.ToInt32(item) % 2 == 0)
                {
                    o++;
                }
                else
                {
                    j++;
                }
            }
            DX_Proportion = string.Format("{0}:{1}", d, x);
            JO_Proportion = string.Format("{0}:{1}", j, o);

            var last = manager.QueryLastCQSSC_5X_JBZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("RedBall4", winRed[3]);
            dic.Add("RedBall5", winRed[4]);
            dic.Add("DX_Proportion", DX_Proportion);
            dic.Add("JO_Proportion", JO_Proportion);

            var entity = this.CreateNewEntity<CQSSC_5X_JBZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_W"))
                {
                    var order = p.Name.Replace("Red_W", string.Empty);
                    lastValue = winRed[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Red_Q"))
                {
                    var order = p.Name.Replace("Red_Q", string.Empty);
                    lastValue = winRed[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Red_B"))
                {
                    var order = p.Name.Replace("Red_B", string.Empty);
                    lastValue = winRed[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Red_S"))
                {
                    var order = p.Name.Replace("Red_S", string.Empty);
                    lastValue = winRed[3] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Red_G"))
                {
                    var order = p.Name.Replace("Red_G", string.Empty);
                    lastValue = winRed[4] == order ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQSSC_5X_JBZS(entity);
        }

        /// <summary>
        /// 2星和值走势
        /// </summary>
        public void AddCQSSC_2X_HZZS(string issuseNumber, string winNumber)
        {
            var manager = new CQSSC_2X_HZZSManager();
            var issuse = manager.QueryCQSSC_2X_HZZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            string num = winNumber.Substring(6);
            var winRed = num.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]);
            var hw = hz % 10;

            var last = manager.QueryLastCQSSC_2X_HZZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("HeZhi", hz);
            dic.Add("HeWei", hw);
            var entity = this.CreateNewEntity<CQSSC_2X_HZZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("HZ_"))
                {
                    var order = p.Name.Replace("HZ_", string.Empty);
                    lastValue = hz == Convert.ToInt32(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("HW_"))
                {
                    var order = p.Name.Replace("HW_", string.Empty);
                    lastValue = int.Parse(order) == hw ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQSSC_2X_HZZS(entity);
        }

        /// <summary>
        /// 2星直选走势
        /// </summary>
        private void AddCQSSC_2X_ZXZS(string issuseNumber, string winNumber)
        {
            var manager = new CQSSC_2X_ZXZSManager();
            var issuse = manager.QueryCQSSC_2X_ZXZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            string num = winNumber.Substring(6);
            var winRed = num.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string type = string.Empty;
            if (winRed[0] == winRed[1])
            {
                type = "对子";
            }
            string DX_Proportion = string.Empty;
            string JO_Proportion = string.Empty;
            string ZH_Proportion = string.Empty;

            int z = 0;
            int h = 0;
            int d = 0;
            int x = 0;
            int j = 0;
            int o = 0;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) == 1 || Convert.ToInt32(item) == 2 || Convert.ToInt32(item) == 3 || Convert.ToInt32(item) == 5 || Convert.ToInt32(item) == 7)
                {
                    z++;
                }
                else
                {
                    h++;
                }

                if (Convert.ToInt32(item) > 4)
                {
                    d++;
                }
                else
                {
                    x++;
                }

                if (Convert.ToInt32(item) % 2 == 0)
                {
                    o++;
                }
                else
                {
                    j++;
                }
            }
            ZH_Proportion = string.Format("{0}:{1}", z, h);
            DX_Proportion = string.Format("{0}:{1}", d, x);
            JO_Proportion = string.Format("{0}:{1}", j, o);
            var last = manager.QueryLastCQSSC_2X_ZXZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("Type", type);
            dic.Add("ZH_Proportion", ZH_Proportion);
            dic.Add("DX_Proportion", DX_Proportion);
            dic.Add("JO_Proportion", JO_Proportion);
            dic.Add("Type_DZ", type == "对子" ? 0 : (last == null ? 1 : last.Type_DZ + 1));

            var entity = this.CreateNewEntity<CQSSC_2X_ZXZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_S"))
                {
                    var order = p.Name.Replace("Red_S", string.Empty);
                    lastValue = winRed[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Red_G"))
                {
                    var order = p.Name.Replace("Red_G", string.Empty);
                    lastValue = winRed[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_JO_"))
                {
                    var value = p.Name.Replace("O_JO_Proportion", string.Empty);
                    lastValue = (JO_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_DX_"))
                {
                    var value = p.Name.Replace("O_DX_Proportion", string.Empty);
                    lastValue = (DX_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_ZH_"))
                {
                    var value = p.Name.Replace("O_ZH_Proportion", string.Empty);
                    lastValue = (ZH_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQSSC_2X_ZXZS(entity);
        }

        /// <summary>
        /// 2星组选走势
        /// </summary>
        private void AddCQSSC_2X_ZuXZS(string issuseNumber, string winNumber)
        {
            var manager = new CQSSC_2X_ZuXZSManager();
            var issuse = manager.QueryCQSSC_2X_ZuXZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            string num = winNumber.Substring(6);
            var winRed = num.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            string type = string.Empty;
            if (winRed[0] == winRed[1])
            {
                type = "对子";
            }
            string DX_Proportion = string.Empty;
            string JO_Proportion = string.Empty;
            string ZH_Proportion = string.Empty;

            int z = 0;
            int h = 0;
            int d = 0;
            int x = 0;
            int j = 0;
            int o = 0;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) == 1 || Convert.ToInt32(item) == 2 || Convert.ToInt32(item) == 3 || Convert.ToInt32(item) == 5 || Convert.ToInt32(item) == 7)
                {
                    z++;
                }
                else
                {
                    h++;
                }

                if (Convert.ToInt32(item) > 4)
                {
                    d++;
                }
                else
                {
                    x++;
                }

                if (Convert.ToInt32(item) % 2 == 0)
                {
                    o++;
                }
                else
                {
                    j++;
                }
            }
            ZH_Proportion = string.Format("{0}:{1}", z, h);
            DX_Proportion = string.Format("{0}:{1}", d, x);
            JO_Proportion = string.Format("{0}:{1}", j, o);

            var last = manager.QueryLastCQSSC_2X_ZuXZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("Type", type);
            dic.Add("Type_DZ", type == "对子" ? 0 : (last == null ? 1 : last.Type_DZ + 1));
            dic.Add("ZH_Proportion", ZH_Proportion);
            dic.Add("DX_Proportion", DX_Proportion);
            dic.Add("JO_Proportion", JO_Proportion);

            var entity = this.CreateNewEntity<CQSSC_2X_ZuXZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_"))
                {
                    var order = p.Name.Replace("Red_", string.Empty);
                    lastValue = winRed.Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("RedCan_"))
                {
                    var order = p.Name.Replace("RedCan_", string.Empty);
                    if (type == "对子")
                    {
                        lastValue = winRed[0] == order ? 2 : 1;
                    }
                    else
                    {
                        lastValue = 1;
                    }
                }
                if (p.Name.StartsWith("O_JO_"))
                {
                    var value = p.Name.Replace("O_JO_Proportion", string.Empty);
                    lastValue = (JO_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_DX_"))
                {
                    var value = p.Name.Replace("O_DX_Proportion", string.Empty);
                    lastValue = (DX_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_ZH_"))
                {
                    var value = p.Name.Replace("O_ZH_Proportion", string.Empty);
                    lastValue = (ZH_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQSSC_2X_ZuXZS(entity);
        }

        /// <summary>
        /// 3星直选走势
        /// </summary>
        private void AddCQSSC_3X_ZXZS(string issuseNumber, string winNumber)
        {
            var manager = new CQSSC_3X_ZXZSManager();
            var issuse = manager.QueryCQSSC_3X_ZXZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            string num = winNumber.Substring(4);
            var winRed = num.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var temp = winRed.Distinct().ToArray();
            string type = string.Empty;
            if (temp.Count() == 2)
            {
                type = "组三";
            }
            else if (temp.Count() == 3)
            {
                type = "组六";
            }
            else
            {
                type = "豹子";
            }

            var last = manager.QueryLastCQSSC_3X_ZXZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("Type", type);

            var entity = this.CreateNewEntity<CQSSC_3X_ZXZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_B"))
                {
                    var order = p.Name.Replace("Red_B", string.Empty);
                    lastValue = winRed[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Red_S"))
                {
                    var order = p.Name.Replace("Red_S", string.Empty);
                    lastValue = winRed[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Red_G"))
                {
                    var order = p.Name.Replace("Red_G", string.Empty);
                    lastValue = winRed[2] == order ? 0 : lastValue;
                }
                if (p.Name.Equals("Type_Z3"))
                {
                    lastValue = (type == "组三") ? 0 : lastValue;
                }
                if (p.Name.Equals("Type_Z6"))
                {
                    lastValue = (type == "组六") ? 0 : lastValue;
                }
                if (p.Name.Equals("Type_BZ"))
                {
                    lastValue = (type == "豹子") ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQSSC_3X_ZXZS(entity);
        }

        /// <summary>
        /// 3星组选走势
        /// </summary>
        private void AddCQSSC_3X_ZuXZS(string issuseNumber, string winNumber)
        {
            var manager = new CQSSC_3X_ZuXZSManager();
            var issuse = manager.QueryCQSSC_3X_ZuXZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            string num = winNumber.Substring(4);
            var winRed = num.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var temp = winRed.Distinct().ToArray();
            var t = string.Empty;
            string type = string.Empty;
            if (temp.Count() == 2)
            {
                type = "组三";
                foreach (var item in winRed)
                {
                    if (winRed.Count(p => p == item) == 2)
                    {
                        t = item;
                        break;
                    }
                }
            }
            else if (temp.Count() == 3)
            {
                type = "组六";
            }
            else
            {
                type = "豹子";
            }
            string DX_Proportion = string.Empty;
            string JO_Proportion = string.Empty;
            string ZH_Proportion = string.Empty;

            int z = 0;
            int h = 0;
            int d = 0;
            int x = 0;
            int j = 0;
            int o = 0;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) == 1 || Convert.ToInt32(item) == 2 || Convert.ToInt32(item) == 3 || Convert.ToInt32(item) == 5 || Convert.ToInt32(item) == 7)
                {
                    z++;
                }
                else
                {
                    h++;
                }

                if (Convert.ToInt32(item) > 4)
                {
                    d++;
                }
                else
                {
                    x++;
                }

                if (Convert.ToInt32(item) % 2 == 0)
                {
                    o++;
                }
                else
                {
                    j++;
                }
            }
            ZH_Proportion = string.Format("{0}:{1}", z, h);
            DX_Proportion = string.Format("{0}:{1}", d, x);
            JO_Proportion = string.Format("{0}:{1}", j, o);

            var last = manager.QueryLastCQSSC_3X_ZuXZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("Type", type);
            dic.Add("ZH_Proportion", ZH_Proportion);
            dic.Add("DX_Proportion", DX_Proportion);
            dic.Add("JO_Proportion", JO_Proportion);

            var entity = this.CreateNewEntity<CQSSC_3X_ZuXZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_"))
                {
                    var order = p.Name.Replace("Red_", string.Empty);
                    lastValue = winRed.Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("RedCan_"))
                {
                    var order = p.Name.Replace("RedCan_", string.Empty);
                    if (type == "组三")
                    {
                        lastValue = t == order ? 2 : 1;
                    }
                    else if (type == "组六")
                    {
                        lastValue = 1;
                    }
                    else if (type == "豹子")
                    {
                        lastValue = winRed[0] == order ? 3 : 1;
                    }
                }
                if (p.Name.Equals("Type_Z3"))
                {
                    lastValue = (type == "组三") ? 0 : lastValue;
                }
                if (p.Name.Equals("Type_Z6"))
                {
                    lastValue = (type == "组六") ? 0 : lastValue;
                }
                if (p.Name.Equals("Type_BZ"))
                {
                    lastValue = (type == "豹子") ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_JO_"))
                {
                    var value = p.Name.Replace("O_JO_Proportion", string.Empty);
                    lastValue = (JO_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_DX_"))
                {
                    var value = p.Name.Replace("O_DX_Proportion", string.Empty);
                    lastValue = (DX_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_ZH_"))
                {
                    var value = p.Name.Replace("O_ZH_Proportion", string.Empty);
                    lastValue = (ZH_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQSSC_3X_ZuXZS(entity);
        }

        /// <summary>
        /// 和值走势
        /// </summary>
        public void AddCQSSC_3X_HZZS(string issuseNumber, string winNumber)
        {
            var manager = new CQSSC_3X_HZZSManager();
            var issuse = manager.QueryCQSSC_3X_HZZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            string num = winNumber.Substring(4);
            var winRed = num.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]);
            var hw = hz % 10;

            var last = manager.QueryLastCQSSC_3X_HZZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);

            var entity = this.CreateNewEntity<CQSSC_3X_HZZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("HZ_"))
                {
                    var order = p.Name.Replace("HZ_", string.Empty);
                    lastValue = hz == Convert.ToInt32(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("HW_"))
                {
                    var order = p.Name.Replace("HW_", string.Empty);
                    lastValue = int.Parse(order) == hw ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQSSC_3X_HZZS(entity);
        }

        /// <summary>
        /// 大小走势
        /// </summary>
        private void AddCQSSC_3X_DXZS(string issuseNumber, string winNumber)
        {
            var manager = new CQSSC_3X_DXZSManager();
            var issuse = manager.QueryCQSSC_3X_DXZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            string num = winNumber.Substring(4);
            var winRed = num.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var redball = new string[]{
                Convert.ToInt32(winRed[0]) > 4 ? "大" : "小",
                Convert.ToInt32(winRed[1]) > 4 ? "大" : "小",
                Convert.ToInt32(winRed[2]) > 4 ? "大" : "小"
            };
            string DX_Proportion = string.Empty;
            int d = 0;
            int x = 0;
            string Dxxt = string.Empty;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) > 4)
                {
                    d++;
                    Dxxt += "大";
                }
                else
                {
                    x++;
                    Dxxt += "小";
                }
            }
            DX_Proportion = string.Format("{0}:{1}", d, x);

            var last = manager.QueryLastCQSSC_3X_DXZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("DX_Proportion", DX_Proportion);
            dic.Add("Dxxt", Dxxt);

            var entity = this.CreateNewEntity<CQSSC_3X_DXZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("DX_"))
                {
                    var order = p.Name.Replace("DX_", string.Empty);
                    if ((order == "Q_D" && Dxxt == "大大大") || (order == "DDX" && Dxxt == "大大小") || (order == "DXD" && Dxxt == "大小大") || (order == "XDD" && Dxxt == "小大大") || (order == "DXX" && Dxxt == "大小小") || (order == "XDX" && Dxxt == "小大小") || (order == "XXD" && Dxxt == "小小大") || (order == "Q_X" && Dxxt == "小小小"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("O_DX_"))
                {
                    var value = p.Name.Replace("O_DX_Proportion", string.Empty);
                    lastValue = (DX_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("D_"))
                {
                    var value = Convert.ToInt32(p.Name.Replace("D_Red", string.Empty));
                    lastValue = (redball[value - 1] == "大") ? 0 : lastValue;
                }
                if (p.Name.StartsWith("X_"))
                {
                    var value = Convert.ToInt32(p.Name.Replace("X_Red", string.Empty));
                    lastValue = (redball[value - 1] == "小") ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQSSC_3X_DXZS(entity);
        }

        /// <summary>
        /// 奇偶走势
        /// </summary>
        private void AddCQSSC_3X_JOZS(string issuseNumber, string winNumber)
        {
            var manager = new CQSSC_3X_JOZSManager();
            var issuse = manager.QueryCQSSC_3X_JOZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            string num = winNumber.Substring(4);
            var winRed = num.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var redball = new string[]{
                Convert.ToInt32(winRed[0]) % 2==0 ? "偶" : "奇",
                Convert.ToInt32(winRed[1]) % 2==0 ? "偶" : "奇",
                Convert.ToInt32(winRed[2]) % 2==0 ? "偶" : "奇"
            };
            string JO_Proportion = string.Empty;
            int j = 0;
            int o = 0;
            string Joxt = string.Empty;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) % 2 == 0)
                {
                    o++;
                    Joxt += "偶";
                }
                else
                {
                    j++;
                    Joxt += "奇";
                }
            }
            JO_Proportion = string.Format("{0}:{1}", j, o);

            var last = manager.QueryLastCQSSC_3X_JOZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("JO_Proportion", JO_Proportion);
            dic.Add("Joxt", Joxt);

            var entity = this.CreateNewEntity<CQSSC_3X_JOZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("JO_"))
                {
                    var order = p.Name.Replace("JO_", string.Empty);
                    if ((order == "Q_J" && Joxt == "奇奇奇") || (order == "JJO" && Joxt == "奇奇偶") || (order == "JOJ" && Joxt == "奇偶奇") || (order == "OJJ" && Joxt == "偶奇奇") || (order == "JOO" && Joxt == "奇偶偶") || (order == "OJO" && Joxt == "偶奇偶") || (order == "OOJ" && Joxt == "偶偶奇") || (order == "Q_O" && Joxt == "偶偶偶"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("O_JO_"))
                {
                    var value = p.Name.Replace("O_JO_Proportion", string.Empty);
                    lastValue = (JO_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("J_Red"))
                {
                    var value = Convert.ToInt32(p.Name.Replace("J_Red", string.Empty));
                    lastValue = (redball[value - 1] == "奇") ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_Red"))
                {
                    var value = Convert.ToInt32(p.Name.Replace("O_Red", string.Empty));
                    lastValue = (redball[value - 1] == "偶") ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQSSC_3X_JOZS(entity);
        }

        /// <summary>
        /// 质合走势
        /// </summary>
        private void AddCQSSC_3X_ZHZS(string issuseNumber, string winNumber)
        {
            var manager = new CQSSC_3X_ZHZSManager();
            var issuse = manager.QueryCQSSC_3X_ZHZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            string num = winNumber.Substring(4);
            var winRed = num.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var redball = new string[]{
                (Convert.ToInt32(winRed[0])==1 ||Convert.ToInt32(winRed[0])==2 ||Convert.ToInt32(winRed[0])==3 ||Convert.ToInt32(winRed[0])==5||Convert.ToInt32(winRed[0])==7) ? "质" : "合",
                (Convert.ToInt32(winRed[1])==1 ||Convert.ToInt32(winRed[1])==2 ||Convert.ToInt32(winRed[1])==3 ||Convert.ToInt32(winRed[1])==5||Convert.ToInt32(winRed[1])==7) ? "质" : "合",
                (Convert.ToInt32(winRed[2])==1 ||Convert.ToInt32(winRed[2])==2 ||Convert.ToInt32(winRed[2])==3 ||Convert.ToInt32(winRed[2])==5||Convert.ToInt32(winRed[2])==7) ? "质" : "合"
            };
            string ZH_Proportion = string.Empty;
            int z = 0;
            int h = 0;
            string Zhxt = string.Empty;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) == 1 || Convert.ToInt32(item) == 2 || Convert.ToInt32(item) == 3 || Convert.ToInt32(item) == 5 || Convert.ToInt32(item) == 7)
                {
                    z++;
                    Zhxt += "质";
                }
                else
                {
                    h++;
                    Zhxt += "合";
                }
            }
            ZH_Proportion = string.Format("{0}:{1}", z, h);

            var last = manager.QueryLastCQSSC_3X_ZHZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("ZH_Proportion", ZH_Proportion);
            dic.Add("Zhxt", Zhxt);

            var entity = this.CreateNewEntity<CQSSC_3X_ZHZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("ZH_"))
                {
                    var order = p.Name.Replace("ZH_", string.Empty);
                    if ((order == "Q_Z" && Zhxt == "质质质") || (order == "ZZH" && Zhxt == "质质合") || (order == "ZHZ" && Zhxt == "质合质") || (order == "HZZ" && Zhxt == "合质质") || (order == "ZHH" && Zhxt == "质合合") || (order == "HZH" && Zhxt == "合质合") || (order == "HHZ" && Zhxt == "合合质") || (order == "Q_H" && Zhxt == "合合合"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("O_ZH_"))
                {
                    var value = p.Name.Replace("O_ZH_Proportion", string.Empty);
                    lastValue = (ZH_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Z_Red"))
                {
                    var value = Convert.ToInt32(p.Name.Replace("Z_Red", string.Empty));
                    lastValue = (redball[value - 1] == "质") ? 0 : lastValue;
                }
                if (p.Name.StartsWith("H_Red"))
                {
                    var value = Convert.ToInt32(p.Name.Replace("H_Red", string.Empty));
                    lastValue = (redball[value - 1] == "合") ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQSSC_3X_ZHZS(entity);
        }

        /// <summary>
        /// 除3余数
        /// </summary>
        private void AddCQSSC_3X_C3YS(string issuseNumber, string winNumber)
        {
            var manager = new CQSSC_3X_C3YSManager();
            var issuse = manager.QueryCQSSC_3X_C3YSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            string num = winNumber.Substring(4);
            var ball = num.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var redball = new int[]{
                Convert.ToInt32(ball[0])%3,
                Convert.ToInt32(ball[1])%3,
                Convert.ToInt32(ball[2])%3
            };
            var Y0_Number = redball.Where(p => p == 0).Count();
            var Y1_Number = redball.Where(p => p == 1).Count();
            var Y2_Number = redball.Where(p => p == 2).Count();

            var last = manager.QueryLastCQSSC_3X_C3YS();

            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", ball[0]);
            dic.Add("RedBall2", ball[1]);
            dic.Add("RedBall3", ball[2]);
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
            dic.Add("YS_Proportion", string.Format("{0}:{1}:{2}", redball[0], redball[1], redball[2]));
            var entity = this.CreateNewEntity<CQSSC_3X_C3YS>(dic, (p) =>
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
            manager.AddCQSSC_3X_C3YS(entity);

        }

        /// <summary>
        /// 跨度
        /// </summary>
        public void AddCQSSC_3X_KD(string issuseNumber, string winNumber)
        {
            var manager = new CQSSC_3X_KDManager();
            var issuse = manager.QueryCQSSC_3X_KDIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            string num = winNumber.Substring(4);
            var winRed = num.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var max = Convert.ToInt32(winRed.Max());
            var min = Convert.ToInt32(winRed.Min());
            var kd = max - min;

            var last = manager.QueryLastCQSSC_3X_KD();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("KuaDu", kd);
            var entity = this.CreateNewEntity<CQSSC_3X_KD>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("KD_"))
                {
                    var value = Convert.ToInt32(p.Name.Replace("KD_", string.Empty));
                    lastValue = kd == value ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQSSC_3X_KD(entity);
        }
        #endregion

        #region 查询数据

        /// <summary>
        /// 1星走势
        /// </summary>
        public List<CQSSC_1X_ZS_Info> QueryCQSSC_1X_ZS(int index)
        {
            List<CQSSC_1X_ZS_Info> Collection = new List<CQSSC_1X_ZS_Info>();
            var list = this.QueryGameChart<CQSSC_1X_ZS_Info>(string.Format("QueryCQSSC_1X_ZS_{0}", index), () =>
            {
                var infoList = new List<CQSSC_1X_ZS_Info>();
                var entityList = new CQSSC_1X_ZSManager().QueryCQSSC_1X_ZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_1X_ZS>, CQSSC_1X_ZS, List<CQSSC_1X_ZS_Info>, CQSSC_1X_ZS_Info>(entityList, ref infoList,
                    () => { return new CQSSC_1X_ZS_Info(); },
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
        /// 重庆2星和值走势
        /// </summary>
        public List<CQSSC_2X_HZZS_Info> QueryCQSSC_2X_HZZS(int index)
        {
            List<CQSSC_2X_HZZS_Info> Collection = new List<CQSSC_2X_HZZS_Info>();
            var list = this.QueryGameChart<CQSSC_2X_HZZS_Info>(string.Format("QueryCQSSC_2X_HZZS_{0}", index), () =>
            {
                var infoList = new List<CQSSC_2X_HZZS_Info>();
                var entityList = new CQSSC_2X_HZZSManager().QueryCQSSC_2X_HZZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_2X_HZZS>, CQSSC_2X_HZZS, List<CQSSC_2X_HZZS_Info>, CQSSC_2X_HZZS_Info>(entityList, ref infoList,
                    () => { return new CQSSC_2X_HZZS_Info(); },
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
        /// 重庆2星组选走势
        /// </summary>
        public List<CQSSC_2X_ZuXZS_Info> QueryCQSSC_2X_ZuXZS(int index)
        {
            List<CQSSC_2X_ZuXZS_Info> Collection = new List<CQSSC_2X_ZuXZS_Info>();
            var list = this.QueryGameChart<CQSSC_2X_ZuXZS_Info>(string.Format("QueryCQSSC_2X_ZuXZS_{0}", index), () =>
            {
                var infoList = new List<CQSSC_2X_ZuXZS_Info>();
                var entityList = new CQSSC_2X_ZuXZSManager().QueryCQSSC_2X_ZuXZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_2X_ZuXZS>, CQSSC_2X_ZuXZS, List<CQSSC_2X_ZuXZS_Info>, CQSSC_2X_ZuXZS_Info>(entityList, ref infoList,
                    () => { return new CQSSC_2X_ZuXZS_Info(); },
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
        /// 重庆2星直选走势
        /// </summary>
        public List<CQSSC_2X_ZXZS_Info> QueryCQSSC_2X_ZXZS(int index)
        {
            List<CQSSC_2X_ZXZS_Info> Collection = new List<CQSSC_2X_ZXZS_Info>();
            var list = this.QueryGameChart<CQSSC_2X_ZXZS_Info>(string.Format("QueryCQSSC_2X_ZXZS_{0}", index), () =>
            {
                var infoList = new List<CQSSC_2X_ZXZS_Info>();
                var entityList = new CQSSC_2X_ZXZSManager().QueryCQSSC_2X_ZXZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_2X_ZXZS>, CQSSC_2X_ZXZS, List<CQSSC_2X_ZXZS_Info>, CQSSC_2X_ZXZS_Info>(entityList, ref infoList,
                    () => { return new CQSSC_2X_ZXZS_Info(); },
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
        /// 除3
        /// </summary>
        public List<CQSSC_3X_C3YS_Info> QueryCQSSC_3X_C3YS(int index)
        {
            List<CQSSC_3X_C3YS_Info> Collection = new List<CQSSC_3X_C3YS_Info>();
            var list = this.QueryGameChart<CQSSC_3X_C3YS_Info>(string.Format("QueryCQSSC_3X_C3YS_{0}", index), () =>
            {
                var infoList = new List<CQSSC_3X_C3YS_Info>();
                var entityList = new CQSSC_3X_C3YSManager().QueryCQSSC_3X_C3YS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_3X_C3YS>, CQSSC_3X_C3YS, List<CQSSC_3X_C3YS_Info>, CQSSC_3X_C3YS_Info>(entityList, ref infoList,
                    () => { return new CQSSC_3X_C3YS_Info(); },
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
        /// 大小走势
        /// </summary>
        public List<CQSSC_3X_DXZS_Info> QueryCQSSC_3X_DXZS(int index)
        {
            List<CQSSC_3X_DXZS_Info> Collection = new List<CQSSC_3X_DXZS_Info>();
            var list = this.QueryGameChart<CQSSC_3X_DXZS_Info>(string.Format("QueryCQSSC_3X_DXZS_{0}", index), () =>
            {
                var infoList = new List<CQSSC_3X_DXZS_Info>();
                var entityList = new CQSSC_3X_DXZSManager().QueryCQSSC_3X_DXZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_3X_DXZS>, CQSSC_3X_DXZS, List<CQSSC_3X_DXZS_Info>, CQSSC_3X_DXZS_Info>(entityList, ref infoList,
                    () => { return new CQSSC_3X_DXZS_Info(); },
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
        /// 和值走势
        /// </summary>
        public List<CQSSC_3X_HZZS_Info> QueryCQSSC_3X_HZZS(int index)
        {
            List<CQSSC_3X_HZZS_Info> Collection = new List<CQSSC_3X_HZZS_Info>();
            var list = this.QueryGameChart<CQSSC_3X_HZZS_Info>(string.Format("QueryCQSSC_3X_HZZS_{0}", index), () =>
            {
                var infoList = new List<CQSSC_3X_HZZS_Info>();
                var entityList = new CQSSC_3X_HZZSManager().QueryCQSSC_3X_HZZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_3X_HZZS>, CQSSC_3X_HZZS, List<CQSSC_3X_HZZS_Info>, CQSSC_3X_HZZS_Info>(entityList, ref infoList,
                    () => { return new CQSSC_3X_HZZS_Info(); },
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
        /// 奇偶走势
        /// </summary>
        public List<CQSSC_3X_JOZS_Info> QueryCQSSC_3X_JOZS(int index)
        {
            List<CQSSC_3X_JOZS_Info> Collection = new List<CQSSC_3X_JOZS_Info>();
            var list = this.QueryGameChart<CQSSC_3X_JOZS_Info>(string.Format("QueryCQSSC_3X_JOZS_{0}", index), () =>
            {
                var infoList = new List<CQSSC_3X_JOZS_Info>();
                var entityList = new CQSSC_3X_JOZSManager().QueryCQSSC_3X_JOZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_3X_JOZS>, CQSSC_3X_JOZS, List<CQSSC_3X_JOZS_Info>, CQSSC_3X_JOZS_Info>(entityList, ref infoList,
                    () => { return new CQSSC_3X_JOZS_Info(); },
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
        /// 跨度
        /// </summary>
        public List<CQSSC_3X_KD_Info> QueryCQSSC_3X_KD(int index)
        {
            List<CQSSC_3X_KD_Info> Collection = new List<CQSSC_3X_KD_Info>();
            var list = this.QueryGameChart<CQSSC_3X_KD_Info>(string.Format("QueryCQSSC_3X_KD_{0}", index), () =>
            {
                var infoList = new List<CQSSC_3X_KD_Info>();
                var entityList = new CQSSC_3X_KDManager().QueryCQSSC_3X_KD(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_3X_KD>, CQSSC_3X_KD, List<CQSSC_3X_KD_Info>, CQSSC_3X_KD_Info>(entityList, ref infoList,
                    () => { return new CQSSC_3X_KD_Info(); },
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
        /// 质合走势
        /// </summary>
        public List<CQSSC_3X_ZHZS_Info> QueryCQSSC_3X_ZHZS(int index)
        {
            List<CQSSC_3X_ZHZS_Info> Collection = new List<CQSSC_3X_ZHZS_Info>();
            var list = this.QueryGameChart<CQSSC_3X_ZHZS_Info>(string.Format("QueryCQSSC_3X_ZHZS_{0}", index), () =>
            {
                var infoList = new List<CQSSC_3X_ZHZS_Info>();
                var entityList = new CQSSC_3X_ZHZSManager().QueryCQSSC_3X_ZHZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_3X_ZHZS>, CQSSC_3X_ZHZS, List<CQSSC_3X_ZHZS_Info>, CQSSC_3X_ZHZS_Info>(entityList, ref infoList,
                    () => { return new CQSSC_3X_ZHZS_Info(); },
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
        /// 重庆3星组选走势
        /// </summary>
        public List<CQSSC_3X_ZuXZS_Info> QueryCQSSC_3X_ZuXZS(int index)
        {
            List<CQSSC_3X_ZuXZS_Info> Collection = new List<CQSSC_3X_ZuXZS_Info>();
            var list = this.QueryGameChart<CQSSC_3X_ZuXZS_Info>(string.Format("QueryCQSSC_3X_ZuXZS_{0}", index), () =>
            {
                var infoList = new List<CQSSC_3X_ZuXZS_Info>();
                var entityList = new CQSSC_3X_ZuXZSManager().QueryCQSSC_3X_ZuXZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_3X_ZuXZS>, CQSSC_3X_ZuXZS, List<CQSSC_3X_ZuXZS_Info>, CQSSC_3X_ZuXZS_Info>(entityList, ref infoList,
                    () => { return new CQSSC_3X_ZuXZS_Info(); },
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
        /// 重庆3星直选走势
        /// </summary>
        public List<CQSSC_3X_ZXZS_Info> QueryCQSSC_3X_ZXZS(int index)
        {
            List<CQSSC_3X_ZXZS_Info> Collection = new List<CQSSC_3X_ZXZS_Info>();
            var list = this.QueryGameChart<CQSSC_3X_ZXZS_Info>(string.Format("QueryCQSSC_3X_ZXZS_{0}", index), () =>
            {
                var infoList = new List<CQSSC_3X_ZXZS_Info>();
                var entityList = new CQSSC_3X_ZXZSManager().QueryCQSSC_3X_ZXZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_3X_ZXZS>, CQSSC_3X_ZXZS, List<CQSSC_3X_ZXZS_Info>, CQSSC_3X_ZXZS_Info>(entityList, ref infoList,
                    () => { return new CQSSC_3X_ZXZS_Info(); },
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
        /// 重庆3星直选走势(new)
        /// </summary>
        public CQSSC_3X_ZXZS_InfoCollection QueryCQSSC_3X_ZXZS_New(int index, string phaseOrder)
        {
            CQSSC_3X_ZXZS_InfoCollection Collection = new CQSSC_3X_ZXZS_InfoCollection();
            var list = this.QueryGameChart<CQSSC_3X_ZXZS_Info>(string.Format("QueryCQSSC_3X_ZXZS_{0}_{1}", index,phaseOrder), () =>
            {
                var infoList = new List<CQSSC_3X_ZXZS_Info>();
                var entityList = new CQSSC_3X_ZXZSManager().QueryCQSSC_3X_ZXZS_New(index, phaseOrder);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_3X_ZXZS>, CQSSC_3X_ZXZS, List<CQSSC_3X_ZXZS_Info>, CQSSC_3X_ZXZS_Info>(entityList, ref infoList,
                    () => { return new CQSSC_3X_ZXZS_Info(); },
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
        /// 和值走势
        /// </summary>
        public List<CQSSC_5X_HZZS_Info> QueryCQSSC_5X_HZZS(int index)
        {
            List<CQSSC_5X_HZZS_Info> Collection = new List<CQSSC_5X_HZZS_Info>();
            var list = this.QueryGameChart<CQSSC_5X_HZZS_Info>(string.Format("QueryCQSSC_5X_HZZS_{0}", index), () =>
            {
                var infoList = new List<CQSSC_5X_HZZS_Info>();
                var entityList = new CQSSC_5X_HZZSManager().QueryCQSSC_5X_HZZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_5X_HZZS>, CQSSC_5X_HZZS, List<CQSSC_5X_HZZS_Info>, CQSSC_5X_HZZS_Info>(entityList, ref infoList,
                    () => { return new CQSSC_5X_HZZS_Info(); },
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
        /// 重庆5星基本走势
        /// </summary>
        public List<CQSSC_5X_JBZS_Info> QueryCQSSC_5X_JBZS(int index)
        {
            List<CQSSC_5X_JBZS_Info> Collection = new List<CQSSC_5X_JBZS_Info>();
            var list = this.QueryGameChart<CQSSC_5X_JBZS_Info>(string.Format("QueryCQSSC_5X_JBZS_{0}", index), () =>
            {
                var infoList = new List<CQSSC_5X_JBZS_Info>();
                var entityList = new CQSSC_5X_JBZSManager().QueryCQSSC_5X_JBZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_5X_JBZS>, CQSSC_5X_JBZS, List<CQSSC_5X_JBZS_Info>, CQSSC_5X_JBZS_Info>(entityList, ref infoList,
                    () => { return new CQSSC_5X_JBZS_Info(); },
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
        /// 大小单双
        /// </summary>
        public List<CQSSC_DXDS_Info> QueryCQSSC_DXDS(int index)
        {
            List<CQSSC_DXDS_Info> Collection = new List<CQSSC_DXDS_Info>();
            var list = this.QueryGameChart<CQSSC_DXDS_Info>(string.Format("QueryCQSSC_DXDS_{0}", index), () =>
            {
                var infoList = new List<CQSSC_DXDS_Info>();
                var entityList = new CQSSC_DXDSManager().QueryCQSSC_DXDS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_DXDS>, CQSSC_DXDS, List<CQSSC_DXDS_Info>, CQSSC_DXDS_Info>(entityList, ref infoList,
                    () => { return new CQSSC_DXDS_Info(); },
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
        public GameWinNumber_InfoCollection QueryCQSSC_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new CQSSC_GameWinNumberManager().QueryCQSSC_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_GameWinNumber>, CQSSC_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryCQSSC_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new CQSSC_GameWinNumberManager().QueryCQSSC_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_GameWinNumber>, CQSSC_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
        public GameWinNumber_InfoCollection QueryCQSSC_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new CQSSC_GameWinNumberManager().QueryCQSSC_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_GameWinNumber>, CQSSC_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryCQSSC_GameWinNumber_{0}_{1}_{2}_{3}", pageIndex, pageSize, startTime.ToString("yyyyMMdd"), endTime.ToString("yyyyMMdd"));
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new CQSSC_GameWinNumberManager().QueryCQSSC_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_GameWinNumber>, CQSSC_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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


        public GameWinNumber_InfoCollection QueryCQSSC_GameWinNumberDesc(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new CQSSC_GameWinNumberManager().QueryCQSSC_GameWinNumberDesc(startTime, endTime, pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<CQSSC_GameWinNumber>, CQSSC_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;
        }

        public GameWinNumber_Info QueryWinNumber(string issuseNumber)
        {
            var manager = new CQSSC_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<CQSSC_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }

        #region 获取当前遗漏

        /// <summary>
        /// 查询重庆时时彩当前遗漏_一星单选
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CQSSC_1X_ZS QueryCQSSCCurrNumberOmission_1XDX(string key, int index)
        {
            var info = QueryOmissionData(key, () =>
            {
                var list = new CQSSC_1X_ZSManager().QueryCQSSC_1X_ZS(index);
                CQSSC_1X_ZS_Info jbzsInfo = new CQSSC_1X_ZS_Info();
                if (list != null && list.Count > 0)
                {
                    //ObjectConvert.ConverEntityToInfo(list[0], ref jbzsInfo);
                    // return jbzsInfo;
                    return list[0];
                }
                return null;
            });
            //var result = info == null ? null : info;
            var result = info;
            if (result != null && !string.IsNullOrEmpty(result.IssuseNumber))
                return result;
            return new CQSSC_1X_ZS();
        }

        /// <summary>
        /// 查询重庆时时彩当前遗漏_二星直选
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public CQSSC_2X_ZXZS QueryCQSSCCurrNumberOmission_2XZX(string key, int index)
        {
            var info = QueryOmissionData(key, () =>
            {
                var list = new CQSSC_2X_ZXZSManager().QueryCQSSC_2X_ZXZS(index);
                CQSSC_2X_ZXZS jbzsInfo = new CQSSC_2X_ZXZS();
                if (list != null && list.Count > 0)
                {
                    //ObjectConvert.ConverEntityToInfo(list[0], ref jbzsInfo);
                    // return jbzsInfo;
                    return list[0];
                }
                return null;
            });
            //var result = info == null ? null : info;
            var result = info;
            if (result != null && !string.IsNullOrEmpty(result.IssuseNumber))
                return result;
            return new CQSSC_2X_ZXZS();
        }
        /// <summary>
        /// 查询重庆时时彩当前遗漏_三星直选
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public CQSSC_3X_ZXZS QueryCQSSCCurrNumberOmission_3XZX(string key, int index)
        {
            var info = QueryOmissionData(key, () =>
            {
                var list = new CQSSC_3X_ZXZSManager().QueryCQSSC_3X_ZXZS(index);
                CQSSC_3X_ZXZS jbzsInfo = new CQSSC_3X_ZXZS();
                if (list != null && list.Count > 0)
                {
                    //ObjectConvert.ConverEntityToInfo(list[0], ref jbzsInfo);
                    // return jbzsInfo;
                    return list[0];
                }
                return null;
            });
            var result = info;
            if (result != null && !string.IsNullOrEmpty(result.IssuseNumber))
                return result;
            return new CQSSC_3X_ZXZS();
        }
        /// <summary>
        /// 查询重庆时时彩当前遗漏_二星组选
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public CQSSC_2X_ZuXZS QueryCQSSCCurrNumberOmission_2XZuX(string key, int index)
        {
            var info = QueryOmissionData(key, () =>
            {
                var list = new CQSSC_2X_ZuXZSManager().QueryCQSSC_2X_ZuXZS(index);
                CQSSC_2X_ZuXZS jbzsInfo = new CQSSC_2X_ZuXZS();
                if (list != null && list.Count > 0)
                {
                    //ObjectConvert.ConverEntityToInfo(list[0], ref jbzsInfo);
                    // return jbzsInfo;
                    return list[0];
                }
                return null;
            });
            var result = info;
            if (result != null && !string.IsNullOrEmpty(result.IssuseNumber))
                return result;
            return new CQSSC_2X_ZuXZS();
        }
        /// <summary>
        /// 查询重庆时时彩当前遗漏_组三，组六
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public CQSSC_3X_ZuXZS QueryCQSSCCurrNumberOmission_ZX3_ZX6(string key, int index)
        {
            var info = QueryOmissionData(key, () =>
            {
                var list = new CQSSC_3X_ZuXZSManager().QueryCQSSC_3X_ZuXZS(index);
                CQSSC_3X_ZuXZS jbzsInfo = new CQSSC_3X_ZuXZS();
                if (list != null && list.Count > 0)
                {
                    //ObjectConvert.ConverEntityToInfo(list[0], ref jbzsInfo);
                    // return jbzsInfo;
                    return list[0];
                }
                return null;
            });
            var result = info;
            if (result != null && !string.IsNullOrEmpty(result.IssuseNumber))
                return result;
            return new CQSSC_3X_ZuXZS();
        }

        /// <summary>
        /// 查询重庆时时彩当前遗漏_大小单双
        /// </summary>
        public CQSSC_DXDS QueryCQSSCCurrNumberOmission_DXDS(string key, int index)
        {
            var info = QueryOmissionData(key, () =>
            {
                var list = new CQSSC_DXDSManager().QueryCQSSC_DXDS(index);
                CQSSC_DXDS jbzsInfo = new CQSSC_DXDS();
                if (list != null && list.Count > 0)
                {
                    //ObjectConvert.ConverEntityToInfo(list[0], ref jbzsInfo);
                    // return jbzsInfo;
                    return list[0];
                }
                return null;
            });
            var result = info;
            if (result != null && !string.IsNullOrEmpty(result.IssuseNumber))
                return result;
            return new CQSSC_DXDS();
        }
        /// <summary>
        /// 查询重庆时时彩当前遗漏_五星基本走势
        /// </summary>
        public CQSSC_5X_JBZS QueryCQSSCCurrNumberOmission_5XJBZS(string key, int index)
        {
            var info = QueryOmissionData(key, () =>
            {
                var list = new CQSSC_5X_JBZSManager().QueryCQSSC_5X_JBZS(index);
                CQSSC_5X_JBZS jbzsInfo = new CQSSC_5X_JBZS();
                if (list != null && list.Count > 0)
                {
                    //ObjectConvert.ConverEntityToInfo(list[0], ref jbzsInfo);
                    // return jbzsInfo;
                    return list[0];
                }
                return null;
            });
            var result = info ;
            if (result != null && !string.IsNullOrEmpty(result.IssuseNumber))
                return result;
            return new CQSSC_5X_JBZS();
        }

        #endregion
    }
}
