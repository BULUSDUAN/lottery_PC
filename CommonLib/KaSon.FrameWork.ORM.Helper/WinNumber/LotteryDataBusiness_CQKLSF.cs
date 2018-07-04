using EntityModel.CoreModel;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.ORM.Helper.WinNumber.Manage;
using EntityModel;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber
{
    public class LotteryDataBusiness_CQKLSF : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get
            {
                return "CQKLSF";
            }
        }

        public void ImportWinNumber(string issuseNumber, string winNumber)
        {
            if (string.IsNullOrEmpty(issuseNumber)) return;
            if (string.IsNullOrEmpty(winNumber)) return;

            //var msg = string.Empty;
            //AnalyzerFactory.GetWinNumberAnalyzer(this.CurrentGameCode).CheckWinNumber(winNumber, out msg);
            //if (!string.IsNullOrEmpty(msg))
            //    throw new Exception(msg);

            //开启事务
            using (LottertDataDB)
            {
                LottertDataDB.Begin();

                this.ClearGameChartCache("QueryCQKLSF_2LZS");
                this.ClearGameChartCache("QueryCQKLSF_3LZS");
                this.ClearGameChartCache("QueryCQKLSF_DXZS");
                this.ClearGameChartCache("QueryCQKLSF_GHZS");
                this.ClearGameChartCache("QueryCQKLSF_JBZS");
                this.ClearGameChartCache("QueryCQKLSF_JOZS");
                this.ClearGameChartCache("QueryCQKLSF_Q1ZS");
                this.ClearGameChartCache("QueryCQKLSF_Q3ZS");
                this.ClearGameChartCache("QueryCQKLSF_QJZS");
                this.ClearGameChartCache("QueryCQKLSF_TWZS");
                this.ClearNewWinNumberCache("QueryCQKLSF_GameWinNumber");

                AddCQKLSF_JBZS(issuseNumber, winNumber);
                AddCQKLSF_DXZS(issuseNumber, winNumber);
                AddCQKLSF_JOZS(issuseNumber, winNumber);
                AddCQKLSF_QJZS(issuseNumber, winNumber);
                AddCQKLSF_2LZS(issuseNumber, winNumber);
                AddCQKLSF_3LZS(issuseNumber, winNumber);
                AddCQKLSF_TWZS(issuseNumber, winNumber);
                AddCQKLSF_GHZS(issuseNumber, winNumber);
                AddCQKLSF_Q1ZS(issuseNumber, winNumber);
                AddCQKLSF_Q3ZS(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);
                LottertDataDB.Commit();
            }
        }

        #region 生成走势数据

        public void Add_GameWinNumber(string issuseNumber, string winNumber)
        {
            new KJGameIssuseBusiness().IssusePrize(this.CurrentGameCode, issuseNumber, winNumber);
            var manager = new CQKLSF_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddCQKLSF_GameWinNumber(new CQKLSF_GameWinNumber
            {
                GameCode = this.CurrentGameCode,
                IssuseNumber = issuseNumber,
                WinNumber = winNumber,
                CreateTime = DateTime.Now,
            });
        }

        /// <summary>
        /// 添加基本走势
        /// </summary>
        private void AddCQKLSF_JBZS(string issuseNumber, string winNumber)
        {
            var manager = new CQKLSF_JBZSManager();
            var issuse = manager.QueryCQKLSF_JBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;
            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var hezhi = Convert.ToInt32(winRed[0]) + Convert.ToInt32(winRed[1]) + Convert.ToInt32(winRed[2]) + Convert.ToInt32(winRed[3]) + Convert.ToInt32(winRed[4]) + Convert.ToInt32(winRed[5]) + Convert.ToInt32(winRed[6]) + Convert.ToInt32(winRed[7]);
            var KuaDu = Convert.ToInt32(winRed.Max()) - Convert.ToInt32(winRed.Min());
            var last = manager.QueryLastCQKLSF_JBZS();
            string ZH_Proportion = string.Empty;
            string JO_Proportion = string.Empty;
            int z = 0;
            int h = 0;
            int j = 0;
            int o = 0;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) == 1 || Convert.ToInt32(item) == 2 || Convert.ToInt32(item) == 3 || Convert.ToInt32(item) == 5 || Convert.ToInt32(item) == 7 || Convert.ToInt32(item) == 11 || Convert.ToInt32(item) == 13 || Convert.ToInt32(item) == 17 || Convert.ToInt32(item) == 19)
                {
                    z++;
                }
                else
                {
                    h++;
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
            JO_Proportion = string.Format("{0}:{1}", j, o);
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("HeZhi", hezhi);
            dic.Add("JO_Proportion", JO_Proportion);
            dic.Add("ZH_Proportion", ZH_Proportion);
            dic.Add("KuaDu", KuaDu);
            var entity = this.CreateNewEntity<CQKLSF_JBZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_"))
                {
                    var order = p.Name.Replace("Red_", string.Empty);
                    lastValue = winRed.Contains(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQKLSF_JBZS(entity);
        }
        /// <summary>
        /// 大小走势
        /// </summary>
        private void AddCQKLSF_DXZS(string issuseNumber, string winNumber)
        {
            var manager = new CQKLSF_DXZSManager();
            var issuse = manager.QueryCQKLSF_DXZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var redball = new string[]{
                Convert.ToInt32(winRed[0])>10?"大":"小",
                Convert.ToInt32(winRed[1]) >10?"大":"小",
                 Convert.ToInt32(winRed[2])>10?"大":"小",
                 Convert.ToInt32(winRed[3])>10?"大":"小",
                 Convert.ToInt32(winRed[4]) >10?"大":"小",
                 Convert.ToInt32(winRed[5])>10?"大":"小",
                 Convert.ToInt32(winRed[6]) >10?"大":"小",
                 Convert.ToInt32(winRed[7]) >10?"大":"小"
            };
            string DX_Proportion = string.Empty;
            int d = 0;
            int x = 0;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) > 5)
                {
                    d++;
                }
                else
                {
                    x++;
                }
            }
            DX_Proportion = string.Format("{0}:{1}", d, x);
            var last = manager.QueryLastCQKLSF_DXZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<CQKLSF_DXZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (p.Name.StartsWith("DX_"))
                {
                    var order = p.Name.Replace("DX_", string.Empty);
                    if ((order == "Q_D" && DX_Proportion == "8:0") || (order == "1D7X" && DX_Proportion == "1:7") || (order == "2D6X" && DX_Proportion == "2:6") || (order == "3D5X" && DX_Proportion == "3:5") || (order == "4D4X" && DX_Proportion == "4:4") || (order == "5D3X" && DX_Proportion == "5:3") || (order == "6D2X" && DX_Proportion == "6:2") || (order == "7D1X" && DX_Proportion == "7:1") || (order == "Q_X" && DX_Proportion == "0:8"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("DX1_"))
                {
                    var order = p.Name.Replace("DX1_", string.Empty);
                    if ((order == "D" && redball[0] == "大") || (order == "X" && redball[0] == "小"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("DX2_"))
                {
                    var order = p.Name.Replace("DX2_", string.Empty);
                    if ((order == "D" && redball[1] == "大") || (order == "X" && redball[1] == "小"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("DX3_"))
                {
                    var order = p.Name.Replace("DX3_", string.Empty);
                    if ((order == "D" && redball[2] == "大") || (order == "X" && redball[2] == "小"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("DX4_"))
                {
                    var order = p.Name.Replace("DX4_", string.Empty);
                    if ((order == "D" && redball[3] == "大") || (order == "X" && redball[3] == "小"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("DX5_"))
                {
                    var order = p.Name.Replace("DX5_", string.Empty);
                    if ((order == "D" && redball[4] == "大") || (order == "X" && redball[4] == "小"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("DX6_"))
                {
                    var order = p.Name.Replace("DX6_", string.Empty);
                    if ((order == "D" && redball[5] == "大") || (order == "X" && redball[5] == "小"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("DX7_"))
                {
                    var order = p.Name.Replace("DX7_", string.Empty);
                    if ((order == "D" && redball[6] == "大") || (order == "X" && redball[6] == "小"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("DX8_"))
                {
                    var order = p.Name.Replace("DX8_", string.Empty);
                    if ((order == "D" && redball[7] == "大") || (order == "X" && redball[7] == "小"))
                    {
                        lastValue = 0;
                    }
                }
                return lastValue;
            });

            manager.AddCQKLSF_DXZS(entity);
        }
        /// <summary>
        /// 奇偶走势
        /// </summary>
        private void AddCQKLSF_JOZS(string issuseNumber, string winNumber)
        {
            var manager = new CQKLSF_JOZSManager();
            var issuse = manager.QueryCQKLSF_JOZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var redball = new string[]{
                Convert.ToInt32(winRed[0])%2==0?"偶":"奇",
                Convert.ToInt32(winRed[1])%2==0?"偶":"奇",
                 Convert.ToInt32(winRed[2])%2==0?"偶":"奇",
                 Convert.ToInt32(winRed[3])%2==0?"偶":"奇",
                 Convert.ToInt32(winRed[4])%2==0?"偶":"奇",
                 Convert.ToInt32(winRed[5])%2==0?"偶":"奇",
                 Convert.ToInt32(winRed[6])%2==0?"偶":"奇",
                 Convert.ToInt32(winRed[7])%2==0?"偶":"奇"
            };
            string JO_Proportion = string.Empty;
            int J = 0;
            int O = 0;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) % 2 == 0)
                {
                    O++;
                }
                else
                {
                    J++;
                }
            }
            JO_Proportion = string.Format("{0}:{1}", J, O);
            var last = manager.QueryLastCQKLSF_JOZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<CQKLSF_JOZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (p.Name.StartsWith("JO_"))
                {
                    var order = p.Name.Replace("JO_", string.Empty);
                    if ((order == "Q_J" && JO_Proportion == "8:0") || (order == "1J7O" && JO_Proportion == "1:7") || (order == "2J6O" && JO_Proportion == "2:6") || (order == "3J5O" && JO_Proportion == "3:5") || (order == "4J4O" && JO_Proportion == "4:4") || (order == "5J3O" && JO_Proportion == "5:3") || (order == "6J2O" && JO_Proportion == "6:2") || (order == "7J1O" && JO_Proportion == "7:1") || (order == "Q_O" && JO_Proportion == "0:8"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("JO1_"))
                {
                    var order = p.Name.Replace("JO1_", string.Empty);
                    if ((order == "J" && redball[0] == "奇") || (order == "O" && redball[0] == "偶"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("JO2_"))
                {
                    var order = p.Name.Replace("JO2_", string.Empty);
                    if ((order == "J" && redball[1] == "奇") || (order == "O" && redball[1] == "偶"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("JO3_"))
                {
                    var order = p.Name.Replace("JO3_", string.Empty);
                    if ((order == "J" && redball[2] == "奇") || (order == "O" && redball[2] == "偶"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("JO4_"))
                {
                    var order = p.Name.Replace("JO4_", string.Empty);
                    if ((order == "J" && redball[3] == "奇") || (order == "O" && redball[3] == "偶"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("JO5_"))
                {
                    var order = p.Name.Replace("JO5_", string.Empty);
                    if ((order == "J" && redball[4] == "奇") || (order == "O" && redball[4] == "偶"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("JO6_"))
                {
                    var order = p.Name.Replace("JO6_", string.Empty);
                    if ((order == "J" && redball[5] == "奇") || (order == "O" && redball[5] == "偶"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("JO7_"))
                {
                    var order = p.Name.Replace("JO7_", string.Empty);
                    if ((order == "J" && redball[6] == "奇") || (order == "O" && redball[6] == "偶"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("JO8_"))
                {
                    var order = p.Name.Replace("JO8_", string.Empty);
                    if ((order == "J" && redball[7] == "奇") || (order == "O" && redball[7] == "偶"))
                    {
                        lastValue = 0;
                    }
                }
                return lastValue;
            });

            manager.AddCQKLSF_JOZS(entity);
        }
        /// <summary>
        /// 添加区间走势
        /// </summary>
        private void AddCQKLSF_QJZS(string issuseNumber, string winNumber)
        {
            var manager = new CQKLSF_QJZSManager();
            var issuse = manager.QueryCQKLSF_QJZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var QJ_15 = winRed.Where(p => int.Parse(p) >= 1 && int.Parse(p) <= 5).Count();
            var QJ_610 = winRed.Where(p => int.Parse(p) >= 6 && int.Parse(p) <= 10).Count();
            var QJ_1115 = winRed.Where(p => int.Parse(p) >= 11 && int.Parse(p) <= 15).Count();
            var QJ_1620 = winRed.Where(p => int.Parse(p) >= 16 && int.Parse(p) <= 20).Count();

            var last = manager.QueryLastCQKLSF_QJZS();
            var dic = new Dictionary<string, object>();

            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<CQKLSF_QJZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("QJ_1_5_"))
                {
                    var order = p.Name.Replace("QJ_1_5_", string.Empty);
                    lastValue = QJ_15 == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("QJ_6_10_"))
                {
                    var order = p.Name.Replace("QJ_6_10_", string.Empty);
                    lastValue = QJ_610 == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("QJ_11_15_"))
                {
                    var order = p.Name.Replace("QJ_11_15_", string.Empty);
                    lastValue = QJ_1115 == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("QJ_16_20_"))
                {
                    var order = p.Name.Replace("QJ_16_20_", string.Empty);
                    lastValue = QJ_1620 == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQKLSF_QJZS(entity);
        }
        /// <summary>
        /// 添加2连走势
        /// </summary>
        private void AddCQKLSF_2LZS(string issuseNumber, string winNumber)
        {
            var manager = new CQKLSF_2LZSManager();
            var issuse = manager.QueryCQKLSF_2LZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;
            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var GHao = 0;
            if (winRed.Contains("01") && winRed.Contains("02"))
                GHao++;
            if (winRed.Contains("02") && winRed.Contains("03"))
                GHao++;
            if (winRed.Contains("03") && winRed.Contains("04"))
                GHao++;
            if (winRed.Contains("04") && winRed.Contains("05"))
                GHao++;
            if (winRed.Contains("05") && winRed.Contains("06"))
                GHao++;
            if (winRed.Contains("06") && winRed.Contains("07"))
                GHao++;
            if (winRed.Contains("07") && winRed.Contains("08"))
                GHao++;
            if (winRed.Contains("08") && winRed.Contains("09"))
                GHao++;
            if (winRed.Contains("09") && winRed.Contains("10"))
                GHao++;
            if (winRed.Contains("10") && winRed.Contains("11"))
                GHao++;
            if (winRed.Contains("11") && winRed.Contains("12"))
                GHao++;
            if (winRed.Contains("12") && winRed.Contains("13"))
                GHao++;
            if (winRed.Contains("13") && winRed.Contains("14"))
                GHao++;
            if (winRed.Contains("14") && winRed.Contains("15"))
                GHao++;
            if (winRed.Contains("15") && winRed.Contains("16"))
                GHao++;
            if (winRed.Contains("16") && winRed.Contains("17"))
                GHao++;
            if (winRed.Contains("17") && winRed.Contains("18"))
                GHao++;
            if (winRed.Contains("18") && winRed.Contains("19"))
                GHao++;
            if (winRed.Contains("19") && winRed.Contains("20"))
                GHao++;
            var last = manager.QueryLastCQKLSF_2LZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("Red_12", (winRed.Contains("01") && winRed.Contains("02")) ? 0 : (last == null ? 1 : last.Red_12 + 1));
            dic.Add("Red_23", (winRed.Contains("02") && winRed.Contains("03")) ? 0 : (last == null ? 1 : last.Red_23 + 1));
            dic.Add("Red_34", (winRed.Contains("03") && winRed.Contains("04")) ? 0 : (last == null ? 1 : last.Red_34 + 1));
            dic.Add("Red_45", (winRed.Contains("04") && winRed.Contains("05")) ? 0 : (last == null ? 1 : last.Red_45 + 1));
            dic.Add("Red_56", (winRed.Contains("05") && winRed.Contains("06")) ? 0 : (last == null ? 1 : last.Red_56 + 1));
            dic.Add("Red_67", (winRed.Contains("06") && winRed.Contains("07")) ? 0 : (last == null ? 1 : last.Red_67 + 1));
            dic.Add("Red_78", (winRed.Contains("07") && winRed.Contains("08")) ? 0 : (last == null ? 1 : last.Red_78 + 1));
            dic.Add("Red_89", (winRed.Contains("08") && winRed.Contains("09")) ? 0 : (last == null ? 1 : last.Red_89 + 1));
            dic.Add("Red_910", (winRed.Contains("09") && winRed.Contains("10")) ? 0 : (last == null ? 1 : last.Red_910 + 1));
            dic.Add("Red_1011", (winRed.Contains("10") && winRed.Contains("11")) ? 0 : (last == null ? 1 : last.Red_1011 + 1));

            dic.Add("Red_1112", (winRed.Contains("11") && winRed.Contains("12")) ? 0 : (last == null ? 1 : last.Red_1112 + 1));
            dic.Add("Red_1213", (winRed.Contains("12") && winRed.Contains("13")) ? 0 : (last == null ? 1 : last.Red_1213 + 1));
            dic.Add("Red_1314", (winRed.Contains("13") && winRed.Contains("14")) ? 0 : (last == null ? 1 : last.Red_1314 + 1));
            dic.Add("Red_1415", (winRed.Contains("14") && winRed.Contains("15")) ? 0 : (last == null ? 1 : last.Red_1415 + 1));
            dic.Add("Red_1516", (winRed.Contains("15") && winRed.Contains("16")) ? 0 : (last == null ? 1 : last.Red_1516 + 1));
            dic.Add("Red_1617", (winRed.Contains("16") && winRed.Contains("17")) ? 0 : (last == null ? 1 : last.Red_1617 + 1));
            dic.Add("Red_1718", (winRed.Contains("17") && winRed.Contains("18")) ? 0 : (last == null ? 1 : last.Red_1718 + 1));
            dic.Add("Red_1819", (winRed.Contains("18") && winRed.Contains("19")) ? 0 : (last == null ? 1 : last.Red_1819 + 1));
            dic.Add("Red_1920", (winRed.Contains("19") && winRed.Contains("20")) ? 0 : (last == null ? 1 : last.Red_1920 + 1));
            var entity = this.CreateNewEntity<CQKLSF_2LZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (p.Name.StartsWith("GH_"))
                {
                    var order = p.Name.Replace("GH_", string.Empty);
                    lastValue = GHao == Convert.ToInt32(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQKLSF_2LZS(entity);
        }
        /// <summary>
        /// 添加3连走势
        /// </summary>
        private void AddCQKLSF_3LZS(string issuseNumber, string winNumber)
        {
            var manager = new CQKLSF_3LZSManager();
            var issuse = manager.QueryCQKLSF_3LZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var GHao = 0;
            if (winRed.Contains("01") && winRed.Contains("02") && winRed.Contains("03"))
                GHao++;
            if (winRed.Contains("02") && winRed.Contains("03") && winRed.Contains("04"))
                GHao++;
            if (winRed.Contains("03") && winRed.Contains("04") && winRed.Contains("05"))
                GHao++;
            if (winRed.Contains("04") && winRed.Contains("05") && winRed.Contains("06"))
                GHao++;
            if (winRed.Contains("05") && winRed.Contains("06") && winRed.Contains("07"))
                GHao++;
            if (winRed.Contains("06") && winRed.Contains("07") && winRed.Contains("08"))
                GHao++;
            if (winRed.Contains("07") && winRed.Contains("08") && winRed.Contains("09"))
                GHao++;
            if (winRed.Contains("08") && winRed.Contains("09") && winRed.Contains("10"))
                GHao++;
            if (winRed.Contains("09") && winRed.Contains("10") && winRed.Contains("11"))
                GHao++;
            if (winRed.Contains("10") && winRed.Contains("11") && winRed.Contains("12"))
                GHao++;
            if (winRed.Contains("11") && winRed.Contains("12") && winRed.Contains("13"))
                GHao++;
            if (winRed.Contains("12") && winRed.Contains("13") && winRed.Contains("14"))
                GHao++;
            if (winRed.Contains("13") && winRed.Contains("14") && winRed.Contains("15"))
                GHao++;
            if (winRed.Contains("14") && winRed.Contains("15") && winRed.Contains("16"))
                GHao++;
            if (winRed.Contains("15") && winRed.Contains("16") && winRed.Contains("17"))
                GHao++;
            if (winRed.Contains("16") && winRed.Contains("17") && winRed.Contains("18"))
                GHao++;
            if (winRed.Contains("17") && winRed.Contains("18") && winRed.Contains("19"))
                GHao++;
            if (winRed.Contains("18") && winRed.Contains("19") && winRed.Contains("20"))
                GHao++;
            var last = manager.QueryLastCQKLSF_3LZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("Red_123", (winRed.Contains("01") && winRed.Contains("02") && winRed.Contains("03")) ? 0 : (last == null ? 1 : last.Red_123 + 1));
            dic.Add("Red_234", (winRed.Contains("02") && winRed.Contains("03") && winRed.Contains("04")) ? 0 : (last == null ? 1 : last.Red_234 + 1));
            dic.Add("Red_345", (winRed.Contains("03") && winRed.Contains("04") && winRed.Contains("05")) ? 0 : (last == null ? 1 : last.Red_345 + 1));
            dic.Add("Red_456", (winRed.Contains("04") && winRed.Contains("05") && winRed.Contains("06")) ? 0 : (last == null ? 1 : last.Red_456 + 1));
            dic.Add("Red_567", (winRed.Contains("05") && winRed.Contains("06") && winRed.Contains("07")) ? 0 : (last == null ? 1 : last.Red_567 + 1));
            dic.Add("Red_678", (winRed.Contains("06") && winRed.Contains("07") && winRed.Contains("08")) ? 0 : (last == null ? 1 : last.Red_678 + 1));
            dic.Add("Red_789", (winRed.Contains("07") && winRed.Contains("08") && winRed.Contains("09")) ? 0 : (last == null ? 1 : last.Red_789 + 1));
            dic.Add("Red_8910", (winRed.Contains("08") && winRed.Contains("09") && winRed.Contains("10")) ? 0 : (last == null ? 1 : last.Red_8910 + 1));
            dic.Add("Red_91011", (winRed.Contains("09") && winRed.Contains("10") && winRed.Contains("11")) ? 0 : (last == null ? 1 : last.Red_91011 + 1));
            dic.Add("Red_101112", (winRed.Contains("10") && winRed.Contains("11") && winRed.Contains("12")) ? 0 : (last == null ? 1 : last.Red_101112 + 1));

            dic.Add("Red_111213", (winRed.Contains("11") && winRed.Contains("12") & winRed.Contains("13")) ? 0 : (last == null ? 1 : last.Red_111213 + 1));
            dic.Add("Red_121314", (winRed.Contains("12") && winRed.Contains("13") && winRed.Contains("14")) ? 0 : (last == null ? 1 : last.Red_121314 + 1));
            dic.Add("Red_131415", (winRed.Contains("13") && winRed.Contains("14") && winRed.Contains("15")) ? 0 : (last == null ? 1 : last.Red_131415 + 1));
            dic.Add("Red_141516", (winRed.Contains("14") && winRed.Contains("15") && winRed.Contains("16")) ? 0 : (last == null ? 1 : last.Red_141516 + 1));
            dic.Add("Red_151617", (winRed.Contains("15") && winRed.Contains("16") && winRed.Contains("17")) ? 0 : (last == null ? 1 : last.Red_151617 + 1));
            dic.Add("Red_161718", (winRed.Contains("16") && winRed.Contains("17") && winRed.Contains("18")) ? 0 : (last == null ? 1 : last.Red_161718 + 1));
            dic.Add("Red_171819", (winRed.Contains("17") && winRed.Contains("18") && winRed.Contains("19")) ? 0 : (last == null ? 1 : last.Red_171819 + 1));
            dic.Add("Red_181920", (winRed.Contains("18") && winRed.Contains("19") && winRed.Contains("20")) ? 0 : (last == null ? 1 : last.Red_181920 + 1));
            var entity = this.CreateNewEntity<CQKLSF_3LZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (p.Name.StartsWith("GH_"))
                {
                    var order = p.Name.Replace("GH_", string.Empty);
                    lastValue = GHao == Convert.ToInt32(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQKLSF_3LZS(entity);
        }
        /// <summary>
        /// 同尾走势
        /// </summary>
        private void AddCQKLSF_TWZS(string issuseNumber, string winNumber)
        {
            var manager = new CQKLSF_TWZSManager();
            var issuse = manager.QueryCQKLSF_TWZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var redball = new int[]{
                Convert.ToInt32(winRed[0])%10,
                Convert.ToInt32(winRed[1]) %10,
                 Convert.ToInt32(winRed[2])%10,
                 Convert.ToInt32(winRed[3])%10,
                 Convert.ToInt32(winRed[4])%10,
                 Convert.ToInt32(winRed[5])%10,
                 Convert.ToInt32(winRed[6])%10,
                 Convert.ToInt32(winRed[7])%10
            };
            var duiwei = 0;
            var TW1 = redball.Where(p => p == 1).Count();
            if (TW1 >= 2)
                duiwei++;
            var TW2 = redball.Where(p => p == 2).Count();
            if (TW2 >= 2)
                duiwei++;
            var TW3 = redball.Where(p => p == 3).Count();
            if (TW3 >= 2)
                duiwei++;
            var TW4 = redball.Where(p => p == 4).Count();
            if (TW4 >= 2)
                duiwei++;
            var TW5 = redball.Where(p => p == 5).Count();
            if (TW5 >= 2)
                duiwei++;
            var TW6 = redball.Where(p => p == 6).Count();
            if (TW6 >= 2)
                duiwei++;
            var TW7 = redball.Where(p => p == 7).Count();
            if (TW7 >= 2)
                duiwei++;
            var TW8 = redball.Where(p => p == 8).Count();
            if (TW8 >= 2)
                duiwei++;
            var TW9 = redball.Where(p => p == 9).Count();
            if (TW9 >= 2)
                duiwei++;
            var TW0 = redball.Where(p => p == 0).Count();
            if (TW0 >= 2)
                duiwei++;
            var last = manager.QueryLastCQKLSF_TWZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<CQKLSF_TWZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (p.Name.StartsWith("GH_"))
                {
                    var order = p.Name.Replace("GH_", string.Empty);
                    lastValue = duiwei == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name == "TW_1")
                    lastValue = TW1 < 1 ? 0 : lastValue;
                if (p.Name == "TW_2")
                    lastValue = TW2 < 1 ? 0 : lastValue;
                if (p.Name == "TW_3")
                    lastValue = TW3 < 1 ? 0 : lastValue;
                if (p.Name == "TW_4")
                    lastValue = TW4 < 1 ? 0 : lastValue;
                if (p.Name == "TW_5")
                    lastValue = TW5 < 1 ? 0 : lastValue;
                if (p.Name == "TW_6")
                    lastValue = TW6 < 1 ? 0 : lastValue;
                if (p.Name == "TW_7")
                    lastValue = TW7 < 1 ? 0 : lastValue;
                if (p.Name == "TW_8")
                    lastValue = TW8 < 1 ? 0 : lastValue;
                if (p.Name == "TW_9")
                    lastValue = TW9 < 1 ? 0 : lastValue;
                if (p.Name == "TW_0")
                    lastValue = TW0 < 1 ? 0 : lastValue;
                return lastValue;
            });

            manager.AddCQKLSF_TWZS(entity);
        }
        /// <summary>
        /// 添加隔号走势
        /// </summary>
        private void AddCQKLSF_GHZS(string issuseNumber, string winNumber)
        {
            var manager = new CQKLSF_GHZSManager();
            var issuse = manager.QueryCQKLSF_GHZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var GHao = 0;
            if (winRed.Contains("01") && winRed.Contains("03"))
                GHao++;
            if (winRed.Contains("02") && winRed.Contains("04"))
                GHao++;
            if (winRed.Contains("03") && winRed.Contains("05"))
                GHao++;
            if (winRed.Contains("04") && winRed.Contains("06"))
                GHao++;
            if (winRed.Contains("05") && winRed.Contains("07"))
                GHao++;
            if (winRed.Contains("06") && winRed.Contains("08"))
                GHao++;
            if (winRed.Contains("07") && winRed.Contains("09"))
                GHao++;
            if (winRed.Contains("08") && winRed.Contains("10"))
                GHao++;
            if (winRed.Contains("09") && winRed.Contains("11"))
                GHao++;
            if (winRed.Contains("10") && winRed.Contains("12"))
                GHao++;
            if (winRed.Contains("11") && winRed.Contains("13"))
                GHao++;
            if (winRed.Contains("12") && winRed.Contains("14"))
                GHao++;
            if (winRed.Contains("13") && winRed.Contains("15"))
                GHao++;
            if (winRed.Contains("14") && winRed.Contains("16"))
                GHao++;
            if (winRed.Contains("15") && winRed.Contains("17"))
                GHao++;
            if (winRed.Contains("16") && winRed.Contains("18"))
                GHao++;
            if (winRed.Contains("17") && winRed.Contains("19"))
                GHao++;
            if (winRed.Contains("18") && winRed.Contains("20"))
                GHao++;
            var last = manager.QueryLastCQKLSF_GHZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("Red_13", (winRed.Contains("01") && winRed.Contains("03")) ? 0 : (last == null ? 1 : last.Red_13 + 1));
            dic.Add("Red_24", (winRed.Contains("02") && winRed.Contains("04")) ? 0 : (last == null ? 1 : last.Red_24 + 1));
            dic.Add("Red_35", (winRed.Contains("03") && winRed.Contains("05")) ? 0 : (last == null ? 1 : last.Red_35 + 1));
            dic.Add("Red_46", (winRed.Contains("04") && winRed.Contains("06")) ? 0 : (last == null ? 1 : last.Red_46 + 1));
            dic.Add("Red_57", (winRed.Contains("05") && winRed.Contains("07")) ? 0 : (last == null ? 1 : last.Red_57 + 1));
            dic.Add("Red_68", (winRed.Contains("06") && winRed.Contains("08")) ? 0 : (last == null ? 1 : last.Red_68 + 1));
            dic.Add("Red_79", (winRed.Contains("07") && winRed.Contains("09")) ? 0 : (last == null ? 1 : last.Red_79 + 1));
            dic.Add("Red_810", (winRed.Contains("08") && winRed.Contains("10")) ? 0 : (last == null ? 1 : last.Red_810 + 1));
            dic.Add("Red_911", (winRed.Contains("09") && winRed.Contains("11")) ? 0 : (last == null ? 1 : last.Red_911 + 1));

            dic.Add("Red_1012", (winRed.Contains("10") && winRed.Contains("12")) ? 0 : (last == null ? 1 : last.Red_1012 + 1));
            dic.Add("Red_1113", (winRed.Contains("11") && winRed.Contains("13")) ? 0 : (last == null ? 1 : last.Red_1113 + 1));
            dic.Add("Red_1214", (winRed.Contains("12") && winRed.Contains("14")) ? 0 : (last == null ? 1 : last.Red_1214 + 1));
            dic.Add("Red_1315", (winRed.Contains("13") && winRed.Contains("15")) ? 0 : (last == null ? 1 : last.Red_1315 + 1));
            dic.Add("Red_1416", (winRed.Contains("14") && winRed.Contains("16")) ? 0 : (last == null ? 1 : last.Red_1416 + 1));
            dic.Add("Red_1517", (winRed.Contains("15") && winRed.Contains("17")) ? 0 : (last == null ? 1 : last.Red_1517 + 1));
            dic.Add("Red_1618", (winRed.Contains("16") && winRed.Contains("18")) ? 0 : (last == null ? 1 : last.Red_1618 + 1));
            dic.Add("Red_1719", (winRed.Contains("17") && winRed.Contains("19")) ? 0 : (last == null ? 1 : last.Red_1719 + 1));
            dic.Add("Red_1820", (winRed.Contains("18") && winRed.Contains("20")) ? 0 : (last == null ? 1 : last.Red_1820 + 1));
            var entity = this.CreateNewEntity<CQKLSF_GHZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (p.Name.StartsWith("GH_"))
                {
                    var order = p.Name.Replace("GH_", string.Empty);
                    lastValue = GHao == Convert.ToInt32(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQKLSF_GHZS(entity);
        }
        /// <summary>
        /// 添加前1走势
        /// </summary>
        private void AddCQKLSF_Q1ZS(string issuseNumber, string winNumber)
        {
            var manager = new CQKLSF_Q1ZSManager();
            var issuse = manager.QueryCQKLSF_Q1ZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var DaXiao = Convert.ToInt32(winRed[0]) > 5 ? "大" : "小";
            var DanS = Convert.ToInt32(winRed[0]) % 2 == 0 ? "双" : "单";
            var C3Y = Convert.ToInt32(winRed[0]) % 3;
            var DXDS = DaXiao + DanS;

            var last = manager.QueryLastCQKLSF_Q1ZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("DDan", (DXDS == "大单") ? 0 : (last == null ? 1 : last.DDan + 1));
            dic.Add("DS", (DXDS == "大双") ? 0 : (last == null ? 1 : last.DS + 1));
            dic.Add("XDan", (DXDS == "小单") ? 0 : (last == null ? 1 : last.XDan + 1));
            dic.Add("XS", (DXDS == "小双") ? 0 : (last == null ? 1 : last.XS + 1));
            var entity = this.CreateNewEntity<CQKLSF_Q1ZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_"))
                {
                    var order = p.Name.Replace("Red_", string.Empty);
                    lastValue = winRed[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("C3_"))
                {
                    var order = p.Name.Replace("C3_", string.Empty);
                    lastValue = C3Y == Convert.ToInt32(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQKLSF_Q1ZS(entity);
        }
        // <summary>
        /// 添加前3走势
        /// </summary>
        private void AddCQKLSF_Q3ZS(string issuseNumber, string winNumber)
        {
            var manager = new CQKLSF_Q3ZSManager();
            var issuse = manager.QueryCQKLSF_Q3ZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var DaXiao = Convert.ToInt32(winRed[0]) > 5 ? "大" : "小";
            var DanS = Convert.ToInt32(winRed[0]) % 2 == 0 ? "双" : "单";
            var C3Y = Convert.ToInt32(winRed[0]) % 3;

            var DaXiao1 = Convert.ToInt32(winRed[1]) > 5 ? "大" : "小";
            var DanS1 = Convert.ToInt32(winRed[1]) % 2 == 0 ? "双" : "单";
            var C3Y1 = Convert.ToInt32(winRed[1]) % 3;

            var DaXiao2 = Convert.ToInt32(winRed[2]) > 5 ? "大" : "小";
            var DanS2 = Convert.ToInt32(winRed[2]) % 2 == 0 ? "双" : "单";
            var C3Y2 = Convert.ToInt32(winRed[2]) % 3;

            var DXDS = DaXiao + DanS;
            var DXDS1 = DaXiao1 + DanS1;
            var DXDS2 = DaXiao2 + DanS2;
            var last = manager.QueryLastCQKLSF_Q3ZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("DDan", (DXDS == "大单") ? 0 : (last == null ? 1 : last.DDan + 1));
            dic.Add("DS", (DXDS == "大双") ? 0 : (last == null ? 1 : last.DS + 1));
            dic.Add("XDan", (DXDS == "小单") ? 0 : (last == null ? 1 : last.XDan + 1));
            dic.Add("XS", (DXDS == "小双") ? 0 : (last == null ? 1 : last.XS + 1));
            dic.Add("DDan1", (DXDS1 == "大单") ? 0 : (last == null ? 1 : last.DDan1 + 1));
            dic.Add("DS1", (DXDS1 == "大双") ? 0 : (last == null ? 1 : last.DS1 + 1));
            dic.Add("XDan1", (DXDS1 == "小单") ? 0 : (last == null ? 1 : last.XDan1 + 1));
            dic.Add("XS1", (DXDS1 == "小双") ? 0 : (last == null ? 1 : last.XS1 + 1));
            dic.Add("DDan2", (DXDS2 == "大单") ? 0 : (last == null ? 1 : last.DDan2 + 1));
            dic.Add("DS2", (DXDS2 == "大双") ? 0 : (last == null ? 1 : last.DS2 + 1));
            dic.Add("XDan2", (DXDS2 == "小单") ? 0 : (last == null ? 1 : last.XDan2 + 1));
            dic.Add("XS2", (DXDS2 == "小双") ? 0 : (last == null ? 1 : last.XS2 + 1));
            var entity = this.CreateNewEntity<CQKLSF_Q3ZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (p.Name.StartsWith("C3_"))
                {
                    var order = p.Name.Replace("C3_", string.Empty);
                    lastValue = C3Y == Convert.ToInt32(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("C31_"))
                {
                    var order = p.Name.Replace("C31_", string.Empty);
                    lastValue = C3Y1 == Convert.ToInt32(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("C32_"))
                {
                    var order = p.Name.Replace("C32_", string.Empty);
                    lastValue = C3Y2 == Convert.ToInt32(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQKLSF_Q3ZS(entity);
        }
        #endregion

        #region 查询数据
        public CQKLSF_2LZS_InfoCollection QueryCQKLSF_2LZS(int index)
        {
            CQKLSF_2LZS_InfoCollection Collection = new CQKLSF_2LZS_InfoCollection();
            var list = this.QueryGameChart<CQKLSF_2LZS_Info>(string.Format("QueryCQKLSF_2LZS_{0}", index), () =>
            {
                var infoList = new List<CQKLSF_2LZS_Info>();
                var entityList = new CQKLSF_2LZSManager().QueryCQKLSF_2LZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQKLSF_2LZS>, CQKLSF_2LZS, List<CQKLSF_2LZS_Info>, CQKLSF_2LZS_Info>(entityList, ref infoList,
                    () => { return new CQKLSF_2LZS_Info(); },
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
        public CQKLSF_3LZS_InfoCollection QueryCQKLSF_3LZS(int index)
        {
            CQKLSF_3LZS_InfoCollection Collection = new CQKLSF_3LZS_InfoCollection();
            var list = this.QueryGameChart<CQKLSF_3LZS_Info>(string.Format("QueryCQKLSF_3LZS_{0}", index), () =>
            {
                var infoList = new List<CQKLSF_3LZS_Info>();
                var entityList = new CQKLSF_3LZSManager().QueryCQKLSF_3LZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQKLSF_3LZS>, CQKLSF_3LZS, List<CQKLSF_3LZS_Info>, CQKLSF_3LZS_Info>(entityList, ref infoList,
                    () => { return new CQKLSF_3LZS_Info(); },
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
        public CQKLSF_DXZS_InfoCollection QueryCQKLSF_DXZS(int index)
        {
            CQKLSF_DXZS_InfoCollection Collection = new CQKLSF_DXZS_InfoCollection();
            var list = this.QueryGameChart<CQKLSF_DXZS_Info>(string.Format("QueryCQKLSF_DXZS_{0}", index), () =>
            {
                var infoList = new List<CQKLSF_DXZS_Info>();
                var entityList = new CQKLSF_DXZSManager().QueryCQKLSF_DXZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQKLSF_DXZS>, CQKLSF_DXZS, List<CQKLSF_DXZS_Info>, CQKLSF_DXZS_Info>(entityList, ref infoList,
                    () => { return new CQKLSF_DXZS_Info(); },
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
        public CQKLSF_GHZS_InfoCollection QueryCQKLSF_GHZS(int index)
        {
            CQKLSF_GHZS_InfoCollection Collection = new CQKLSF_GHZS_InfoCollection();
            var list = this.QueryGameChart<CQKLSF_GHZS_Info>(string.Format("QueryCQKLSF_GHZS_{0}", index), () =>
            {
                var infoList = new List<CQKLSF_GHZS_Info>();
                var entityList = new CQKLSF_GHZSManager().QueryCQKLSF_GHZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQKLSF_GHZS>, CQKLSF_GHZS, List<CQKLSF_GHZS_Info>, CQKLSF_GHZS_Info>(entityList, ref infoList,
                    () => { return new CQKLSF_GHZS_Info(); },
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
        public CQKLSF_JBZS_InfoCollection QueryCQKLSF_JBZS(int index)
        {
            CQKLSF_JBZS_InfoCollection Collection = new CQKLSF_JBZS_InfoCollection();
            var list = this.QueryGameChart<CQKLSF_JBZS_Info>(string.Format("QueryCQKLSF_JBZS_{0}", index), () =>
            {
                var infoList = new List<CQKLSF_JBZS_Info>();
                var entityList = new CQKLSF_JBZSManager().QueryCQKLSF_JBZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQKLSF_JBZS>, CQKLSF_JBZS, List<CQKLSF_JBZS_Info>, CQKLSF_JBZS_Info>(entityList, ref infoList,
                    () => { return new CQKLSF_JBZS_Info(); },
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
        public CQKLSF_JOZS_InfoCollection QueryCQKLSF_JOZS(int index)
        {
            CQKLSF_JOZS_InfoCollection Collection = new CQKLSF_JOZS_InfoCollection();
            var list = this.QueryGameChart<CQKLSF_JOZS_Info>(string.Format("QueryCQKLSF_JOZS_{0}", index), () =>
            {
                var infoList = new List<CQKLSF_JOZS_Info>();
                var entityList = new CQKLSF_JOZSManager().QueryCQKLSF_JOZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQKLSF_JOZS>, CQKLSF_JOZS, List<CQKLSF_JOZS_Info>, CQKLSF_JOZS_Info>(entityList, ref infoList,
                    () => { return new CQKLSF_JOZS_Info(); },
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
        public CQKLSF_Q1ZS_InfoCollection QueryCQKLSF_Q1ZS(int index)
        {
            CQKLSF_Q1ZS_InfoCollection Collection = new CQKLSF_Q1ZS_InfoCollection();
            var list = this.QueryGameChart<CQKLSF_Q1ZS_Info>(string.Format("QueryCQKLSF_Q1ZS_{0}", index), () =>
            {
                var infoList = new List<CQKLSF_Q1ZS_Info>();
                var entityList = new CQKLSF_Q1ZSManager().QueryCQKLSF_Q1ZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQKLSF_Q1ZS>, CQKLSF_Q1ZS, List<CQKLSF_Q1ZS_Info>, CQKLSF_Q1ZS_Info>(entityList, ref infoList,
                    () => { return new CQKLSF_Q1ZS_Info(); },
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
        public CQKLSF_Q3ZS_InfoCollection QueryCQKLSF_Q3ZS(int index)
        {
            CQKLSF_Q3ZS_InfoCollection Collection = new CQKLSF_Q3ZS_InfoCollection();
            var list = this.QueryGameChart<CQKLSF_Q3ZS_Info>(string.Format("QueryCQKLSF_Q3ZS_{0}", index), () =>
            {
                var infoList = new List<CQKLSF_Q3ZS_Info>();
                var entityList = new CQKLSF_Q3ZSManager().QueryCQKLSF_Q3ZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQKLSF_Q3ZS>, CQKLSF_Q3ZS, List<CQKLSF_Q3ZS_Info>, CQKLSF_Q3ZS_Info>(entityList, ref infoList,
                    () => { return new CQKLSF_Q3ZS_Info(); },
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
        public CQKLSF_QJZS_InfoCollection QueryCQKLSF_QJZS(int index)
        {
            CQKLSF_QJZS_InfoCollection Collection = new CQKLSF_QJZS_InfoCollection();
            var list = this.QueryGameChart<CQKLSF_QJZS_Info>(string.Format("QueryCQKLSF_QJZS_{0}", index), () =>
            {
                var infoList = new List<CQKLSF_QJZS_Info>();
                var entityList = new CQKLSF_QJZSManager().QueryCQKLSF_QJZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQKLSF_QJZS>, CQKLSF_QJZS, List<CQKLSF_QJZS_Info>, CQKLSF_QJZS_Info>(entityList, ref infoList,
                    () => { return new CQKLSF_QJZS_Info(); },
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
        public CQKLSF_TWZS_InfoCollection QueryCQKLSF_TWZS(int index)
        {
            CQKLSF_TWZS_InfoCollection Collection = new CQKLSF_TWZS_InfoCollection();
            var list = this.QueryGameChart<CQKLSF_TWZS_Info>(string.Format("QueryCQKLSF_TWZS_{0}", index), () =>
            {
                var infoList = new List<CQKLSF_TWZS_Info>();
                var entityList = new CQKLSF_TWZSManager().QueryCQKLSF_TWZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQKLSF_TWZS>, CQKLSF_TWZS, List<CQKLSF_TWZS_Info>, CQKLSF_TWZS_Info>(entityList, ref infoList,
                    () => { return new CQKLSF_TWZS_Info(); },
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
        public GameWinNumber_InfoCollection QueryCQKLSF_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new CQKLSF_GameWinNumberManager().QueryCQKLSF_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<CQKLSF_GameWinNumber>, CQKLSF_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryCQKLSF_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new CQKLSF_GameWinNumberManager().QueryCQKLSF_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<CQKLSF_GameWinNumber>, CQKLSF_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
            var manager = new CQKLSF_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<CQKLSF_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }

    }
}
