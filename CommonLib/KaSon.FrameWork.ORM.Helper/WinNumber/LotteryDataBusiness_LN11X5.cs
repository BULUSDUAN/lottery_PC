using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common;

using KaSon.FrameWork.ORM.Helper.WinNumber.Manage;
using EntityModel;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;
using KaSon.FrameWork.Common.Utilities;

namespace KaSon.FrameWork.ORM.Helper.WinNumber
{
    public class LotteryDataBusiness_LN11X5 : LotteryDataBusiness, ILotteryDataBusiness
    {

        public string CurrentGameCode
        {
            get
            {
                return "LN11X5";
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

                this.ClearGameChartCache("QueryLN11X5_2LZS");
                this.ClearGameChartCache("QueryLN11X5_CHZS");
                this.ClearGameChartCache("QueryLN11X5_DLZS");
                this.ClearGameChartCache("QueryLN11X5_DXZS");
                this.ClearGameChartCache("QueryLN11X5_GHZS");
                this.ClearGameChartCache("QueryLN11X5_HZZS");
                this.ClearGameChartCache("QueryLN11X5_JBZS");
                this.ClearGameChartCache("QueryLN11X5_JOZS");
                this.ClearGameChartCache("QueryLN11X5_Q1ZS");
                this.ClearGameChartCache("QueryLN11X5_Q2ZS");
                this.ClearGameChartCache("QueryLN11X5_Q3ZS");
                this.ClearNewWinNumberCache("QueryLN11X5_GameWinNumber");


                AddLN11X5_JBZS(issuseNumber, winNumber);
                AddLN11X5_DXZS(issuseNumber, winNumber);
                AddLN11X5_JOZS(issuseNumber, winNumber);
                AddLN11X5_HZZS(issuseNumber, winNumber);
                AddLN11X5_CHZS(issuseNumber, winNumber);
                AddLN11X5_GHZS(issuseNumber, winNumber);
                AddLN11X5_2LZS(issuseNumber, winNumber);
                AddLN11X5_DLZS(issuseNumber, winNumber);
                AddLN11X5_Q1ZS(issuseNumber, winNumber);
                AddLN11X5_Q2ZS(issuseNumber, winNumber);
                AddLN11X5_Q3ZS(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }
        #region 生产走势数据

        public void Add_GameWinNumber(string issuseNumber, string winNumber)
        {
            new KJGameIssuseBusiness().IssusePrize(this.CurrentGameCode, issuseNumber, winNumber);
            var manager = new LN11X5_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddLN11X5_GameWinNumber(new LN11X5_GameWinNumber
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
        private void AddLN11X5_JBZS(string issuseNumber, string winNumber)
        {
            var manager = new LN11X5_JBZSManager();
            var issuse = manager.QueryLN11X5_JBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hezhi = Convert.ToInt32(winRed[0]) + Convert.ToInt32(winRed[1]) + Convert.ToInt32(winRed[2]) + Convert.ToInt32(winRed[3]) + Convert.ToInt32(winRed[4]);

            var last = manager.QueryLastLN11X5_JBZS();
            int chonghao = 0;
            for (int i = 0; i < (last == null ? 0 : last.WinNumber.Split(',').Length); i++)
            {
                if (last.WinNumber.Contains(winRed[i]))
                    chonghao++;
            }
            int lianhao = 0;
            for (int i = 0; i < winRed.Length; i++)
            {
                if (i != 4)
                    if ((Convert.ToInt32(winRed[i]) + 1) == Convert.ToInt32(winRed[i + 1]))
                        lianhao++;
            }
            var Max = winRed.Max();
            var Min = winRed.Min();
            string ZH_Proportion = string.Empty;
            string JO_Proportion = string.Empty;
            int z = 0;
            int h = 0;
            int j = 0;
            int o = 0;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) == 1 || Convert.ToInt32(item) == 2 || Convert.ToInt32(item) == 3 || Convert.ToInt32(item) == 5 || Convert.ToInt32(item) == 7 || Convert.ToInt32(item) == 11)
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
            dic.Add("KuaDu", Convert.ToInt32(Max) - Convert.ToInt32(Min));
            dic.Add("Duplicate", chonghao);
            dic.Add("ContinuousNumber", lianhao);
            var entity = this.CreateNewEntity<LN11X5_JBZS>(dic, (p) =>
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

            manager.AddLN11X5_JBZS(entity);
        }

        /// <summary>
        /// 添加大小走势
        /// </summary>
        private void AddLN11X5_DXZS(string issuseNumber, string winNumber)
        {
            var manager = new LN11X5_DXZSManager();
            var issuse = manager.QueryLN11X5_DXZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var redball = new string[]{
                Convert.ToInt32(winRed[0]) > 5 ? "大" : "小",
                Convert.ToInt32(winRed[1]) > 5 ? "大" : "小",
                 Convert.ToInt32(winRed[2]) > 5 ? "大" : "小",
                 Convert.ToInt32(winRed[3]) > 5 ? "大" : "小",
                 Convert.ToInt32(winRed[4]) > 5 ? "大" : "小"
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
            var last = manager.QueryLastLN11X5_DXZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("DX_Proportion", DX_Proportion);

            var entity = this.CreateNewEntity<LN11X5_DXZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (p.Name.StartsWith("DX_"))
                {
                    var order = p.Name.Replace("DX_", string.Empty);
                    if ((order == "Q_D" && DX_Proportion == "5:0") || (order == "1D4X" && DX_Proportion == "1:4") || (order == "2D3X" && DX_Proportion == "2:3") || (order == "3D2X" && DX_Proportion == "3:2") || (order == "4D1X" && DX_Proportion == "4:1") || (order == "Q_X" && DX_Proportion == "0:5"))
                    {
                        lastValue = 0;
                    }
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

            manager.AddLN11X5_DXZS(entity);
        }
        /// <summary>
        /// 添加奇偶走势
        /// </summary>
        private void AddLN11X5_JOZS(string issuseNumber, string winNumber)
        {
            var manager = new LN11X5_JOZSManager();
            var issuse = manager.QueryLN11X5_JOZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var redball = new string[]{
                Convert.ToInt32(winRed[0]) % 2==0 ? "偶" : "奇",
                Convert.ToInt32(winRed[1]) % 2==0 ? "偶" : "奇",
                Convert.ToInt32(winRed[2]) % 2==0 ? "偶" : "奇",
                Convert.ToInt32(winRed[3]) % 2==0 ? "偶" : "奇",
                Convert.ToInt32(winRed[4]) % 2==0 ? "偶" : "奇"
            };
            string JO_Proportion = string.Empty;
            int j = 0;
            int o = 0;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) % 2 == 0)
                {
                    o++;
                }
                else
                {
                    j++;
                }
            }
            JO_Proportion = string.Format("{0}:{1}", j, o);
            var last = manager.QueryLastLN11X5_JOZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("JO_Proportion", JO_Proportion);

            var entity = this.CreateNewEntity<LN11X5_JOZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (p.Name.StartsWith("JO_"))
                {
                    var order = p.Name.Replace("JO_", string.Empty);
                    if ((order == "Q_J" && JO_Proportion == "5:0") || (order == "1J4O" && JO_Proportion == "1:4") || (order == "2J3O" && JO_Proportion == "2:3") || (order == "3J2O" && JO_Proportion == "3:2") || (order == "4J1O" && JO_Proportion == "4:1") || (order == "Q_O" && JO_Proportion == "0:5"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("J_"))
                {
                    var value = Convert.ToInt32(p.Name.Replace("J_Red", string.Empty));
                    lastValue = (redball[value - 1] == "奇") ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_"))
                {
                    var value = Convert.ToInt32(p.Name.Replace("O_Red", string.Empty));
                    lastValue = (redball[value - 1] == "偶") ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddLN11X5_JOZS(entity);
        }
        /// <summary>
        /// 和值走势
        /// </summary>
        public void AddLN11X5_HZZS(string issuseNumber, string winNumber)
        {
            var manager = new LN11X5_HZZSManager();
            var issuse = manager.QueryLN11X5_HZZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]) + int.Parse(winRed[3]) + int.Parse(winRed[4]);
            var last = manager.QueryLastLN11X5_HZZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("HeZhi", hz);
            var entity = this.CreateNewEntity<LN11X5_HZZS>(dic, (p) =>
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

            manager.AddLN11X5_HZZS(entity);
        }
        /// <summary>
        /// 添加重号走势
        /// </summary>
        private void AddLN11X5_CHZS(string issuseNumber, string winNumber)
        {
            var manager = new LN11X5_CHZSManager();
            var issuse = manager.QueryLN11X5_CHZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var last = manager.QueryLastLN11X5_CHZS();
            int chonghao = 0;
            for (int i = 0; i < (last == null ? 0 : last.WinNumber.Split(',').Length); i++)
            {
                if (last.WinNumber.Contains(winRed[i]))
                    chonghao++;
            }
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("Duplicate", chonghao);
            var entity = this.CreateNewEntity<LN11X5_CHZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_"))
                {
                    var order = p.Name.Replace("Red_", string.Empty);
                    lastValue = winRed.Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("D_"))
                {
                    var order = p.Name.Replace("D_", string.Empty);
                    lastValue = chonghao == Convert.ToInt32(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddLN11X5_CHZS(entity);
        }
        /// <summary>
        /// 添加隔号走势
        /// </summary>
        private void AddLN11X5_GHZS(string issuseNumber, string winNumber)
        {
            var manager = new LN11X5_GHZSManager();
            var issuse = manager.QueryLN11X5_GHZSIssuseNumber(issuseNumber);
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
            var last = manager.QueryLastLN11X5_GHZS();
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
            dic.Add("GHao", GHao);
            var entity = this.CreateNewEntity<LN11X5_GHZS>(dic, (p) =>
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

            manager.AddLN11X5_GHZS(entity);
        }
        /// <summary>
        /// 添加2连走势
        /// </summary>
        private void AddLN11X5_2LZS(string issuseNumber, string winNumber)
        {
            var manager = new LN11X5_2LZSManager();
            var issuse = manager.QueryLN11X5_2LZSIssuseNumber(issuseNumber);
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
            var last = manager.QueryLastLN11X5_2LZS();
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
            dic.Add("GHao", GHao);
            var entity = this.CreateNewEntity<LN11X5_2LZS>(dic, (p) =>
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

            manager.AddLN11X5_2LZS(entity);
        }
        /// <summary>
        /// 添加多连走势
        /// </summary>
        private void AddLN11X5_DLZS(string issuseNumber, string winNumber)
        {
            var manager = new LN11X5_DLZSManager();
            var issuse = manager.QueryLN11X5_DLZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var last = manager.QueryLastLN11X5_DLZS();
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

            dic.Add("Red_1234", (winRed.Contains("01") && winRed.Contains("02") && winRed.Contains("03") && winRed.Contains("04")) ? 0 : (last == null ? 1 : last.Red_1234 + 1));
            dic.Add("Red_2345", (winRed.Contains("02") && winRed.Contains("03") && winRed.Contains("04") && winRed.Contains("05")) ? 0 : (last == null ? 1 : last.Red_2345 + 1));
            dic.Add("Red_3456", (winRed.Contains("03") && winRed.Contains("04") && winRed.Contains("05") && winRed.Contains("06")) ? 0 : (last == null ? 1 : last.Red_3456 + 1));
            dic.Add("Red_4567", (winRed.Contains("04") && winRed.Contains("05") && winRed.Contains("06") && winRed.Contains("07")) ? 0 : (last == null ? 1 : last.Red_4567 + 1));
            dic.Add("Red_5678", (winRed.Contains("05") && winRed.Contains("06") && winRed.Contains("07") && winRed.Contains("08")) ? 0 : (last == null ? 1 : last.Red_5678 + 1));
            dic.Add("Red_6789", (winRed.Contains("06") && winRed.Contains("07") && winRed.Contains("08") && winRed.Contains("09")) ? 0 : (last == null ? 1 : last.Red_6789 + 1));
            dic.Add("Red_78910", (winRed.Contains("07") && winRed.Contains("08") && winRed.Contains("09") && winRed.Contains("10")) ? 0 : (last == null ? 1 : last.Red_78910 + 1));
            dic.Add("Red_891011", (winRed.Contains("08") && winRed.Contains("09") && winRed.Contains("10") && winRed.Contains("11")) ? 0 : (last == null ? 1 : last.Red_891011 + 1));

            dic.Add("Red_12345", (winRed.Contains("01") && winRed.Contains("02") && winRed.Contains("03") && winRed.Contains("04") && winRed.Contains("05")) ? 0 : (last == null ? 1 : last.Red_12345 + 1));
            dic.Add("Red_23456", (winRed.Contains("02") && winRed.Contains("03") && winRed.Contains("04") && winRed.Contains("05") && winRed.Contains("06")) ? 0 : (last == null ? 1 : last.Red_23456 + 1));
            dic.Add("Red_34567", (winRed.Contains("03") && winRed.Contains("04") && winRed.Contains("05") && winRed.Contains("06") && winRed.Contains("07")) ? 0 : (last == null ? 1 : last.Red_34567 + 1));
            dic.Add("Red_45678", (winRed.Contains("04") && winRed.Contains("05") && winRed.Contains("06") && winRed.Contains("07") && winRed.Contains("08")) ? 0 : (last == null ? 1 : last.Red_45678 + 1));
            dic.Add("Red_56789", (winRed.Contains("05") && winRed.Contains("06") && winRed.Contains("07") && winRed.Contains("08") && winRed.Contains("09")) ? 0 : (last == null ? 1 : last.Red_56789 + 1));
            dic.Add("Red_678910", (winRed.Contains("06") && winRed.Contains("07") && winRed.Contains("08") && winRed.Contains("09") && winRed.Contains("10")) ? 0 : (last == null ? 1 : last.Red_678911 + 1));
            dic.Add("Red_7891011", (winRed.Contains("07") && winRed.Contains("08") && winRed.Contains("09") && winRed.Contains("10") && winRed.Contains("11")) ? 0 : (last == null ? 1 : last.Red_7891011 + 1));
            var entity = this.CreateNewEntity<LN11X5_DLZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                return lastValue;
            });

            manager.AddLN11X5_DLZS(entity);
        }

        /// <summary>
        /// 添加前1走势
        /// </summary>
        private void AddLN11X5_Q1ZS(string issuseNumber, string winNumber)
        {
            var manager = new LN11X5_Q1ZSManager();
            var issuse = manager.QueryLN11X5_Q1ZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var DaXiao = Convert.ToInt32(winRed[0]) > 5 ? "大" : "小";
            var DanS = Convert.ToInt32(winRed[0]) % 2 == 0 ? "双" : "单";
            var C3Y = Convert.ToInt32(winRed[0]) % 3;

            var DXDS = DaXiao + DanS;

            var last = manager.QueryLastLN11X5_Q1ZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("DDan", (DXDS == "大单") ? 0 : (last == null ? 1 : last.DDan + 1));
            dic.Add("DS", (DXDS == "大双") ? 0 : (last == null ? 1 : last.DS + 1));
            dic.Add("XDan", (DXDS == "小单") ? 0 : (last == null ? 1 : last.XDan + 1));
            dic.Add("XS", (DXDS == "小双") ? 0 : (last == null ? 1 : last.XS + 1));
            var entity = this.CreateNewEntity<LN11X5_Q1ZS>(dic, (p) =>
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

            manager.AddLN11X5_Q1ZS(entity);
        }

        /// <summary>
        /// 添加前2走势
        /// </summary>
        private void AddLN11X5_Q2ZS(string issuseNumber, string winNumber)
        {
            var manager = new LN11X5_Q2ZSManager();
            var issuse = manager.QueryLN11X5_Q2ZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var DaXiao = Convert.ToInt32(winRed[0]) > 5 ? "大" : "小";
            var DanS = Convert.ToInt32(winRed[0]) % 2 == 0 ? "双" : "单";
            var C3Y = Convert.ToInt32(winRed[0]) % 3;

            var DXDS = DaXiao + DanS;

            var DaXiao1 = Convert.ToInt32(winRed[1]) > 5 ? "大" : "小";
            var DanS1 = Convert.ToInt32(winRed[1]) % 2 == 0 ? "双" : "单";
            var C3Y1 = Convert.ToInt32(winRed[1]) % 3;

            var DXDS1 = DaXiao1 + DanS1;

            var last = manager.QueryLastLN11X5_Q2ZS();
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
            var entity = this.CreateNewEntity<LN11X5_Q2ZS>(dic, (p) =>
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
                return lastValue;
            });

            manager.AddLN11X5_Q2ZS(entity);
        }
        /// <summary>
        /// 添加前3走势
        /// </summary>
        private void AddLN11X5_Q3ZS(string issuseNumber, string winNumber)
        {
            var manager = new LN11X5_Q3ZSManager();
            var issuse = manager.QueryLN11X5_Q3ZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var DaXiao = Convert.ToInt32(winRed[0]) > 5 ? "大" : "小";
            var DanS = Convert.ToInt32(winRed[0]) % 2 == 0 ? "双" : "单";
            var C3Y = Convert.ToInt32(winRed[0]) % 3;

            var DXDS = DaXiao + DanS;

            var DaXiao1 = Convert.ToInt32(winRed[1]) > 5 ? "大" : "小";
            var DanS1 = Convert.ToInt32(winRed[1]) % 2 == 0 ? "双" : "单";
            var C3Y1 = Convert.ToInt32(winRed[1]) % 3;

            var DXDS1 = DaXiao1 + DanS1;

            var DaXiao2 = Convert.ToInt32(winRed[2]) > 5 ? "大" : "小";
            var DanS2 = Convert.ToInt32(winRed[2]) % 2 == 0 ? "双" : "单";
            var C3Y2 = Convert.ToInt32(winRed[2]) % 3;

            var DXDS2 = DaXiao2 + DanS2;

            var last = manager.QueryLastLN11X5_Q3ZS();
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
            var entity = this.CreateNewEntity<LN11X5_Q3ZS>(dic, (p) =>
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

            manager.AddLN11X5_Q3ZS(entity);
        }
        #endregion

        #region 查询数据
        public LN11X5_2LZS_InfoCollection QueryLN11X5_2LZS(int index)
        {
            LN11X5_2LZS_InfoCollection Collection = new LN11X5_2LZS_InfoCollection();
            var list = this.QueryGameChart<LN11X5_2LZS_Info>(string.Format("QueryLN11X5_2LZS_{0}", index), () =>
            {
                var infoList = new List<LN11X5_2LZS_Info>();
                var entityList = new LN11X5_2LZSManager().QueryLN11X5_2LZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<LN11X5_2LZS>, LN11X5_2LZS, List<LN11X5_2LZS_Info>, LN11X5_2LZS_Info>(entityList, ref infoList,
                    () => { return new LN11X5_2LZS_Info(); },
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
        public LN11X5_CHZS_InfoCollection QueryLN11X5_CHZS(int index)
        {
            LN11X5_CHZS_InfoCollection Collection = new LN11X5_CHZS_InfoCollection();
            var list = this.QueryGameChart<LN11X5_CHZS_Info>(string.Format("QueryLN11X5_CHZS_{0}", index), () =>
            {
                var infoList = new List<LN11X5_CHZS_Info>();
                var entityList = new LN11X5_CHZSManager().QueryLN11X5_CHZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<LN11X5_CHZS>, LN11X5_CHZS, List<LN11X5_CHZS_Info>, LN11X5_CHZS_Info>(entityList, ref infoList,
                    () => { return new LN11X5_CHZS_Info(); },
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
        public LN11X5_DLZS_InfoCollection QueryLN11X5_DLZS(int index)
        {
            LN11X5_DLZS_InfoCollection Collection = new LN11X5_DLZS_InfoCollection();
            var list = this.QueryGameChart<LN11X5_DLZS_Info>(string.Format("QueryLN11X5_DLZS_{0}", index), () =>
            {
                var infoList = new List<LN11X5_DLZS_Info>();
                var entityList = new LN11X5_DLZSManager().QueryLN11X5_DLZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<LN11X5_DLZS>, LN11X5_DLZS, List<LN11X5_DLZS_Info>, LN11X5_DLZS_Info>(entityList, ref infoList,
                    () => { return new LN11X5_DLZS_Info(); },
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
        public LN11X5_DXZS_InfoCollection QueryLN11X5_DXZS(int index)
        {
            LN11X5_DXZS_InfoCollection Collection = new LN11X5_DXZS_InfoCollection();
            var list = this.QueryGameChart<LN11X5_DXZS_Info>(string.Format("QueryLN11X5_DXZS_{0}", index), () =>
            {
                var infoList = new List<LN11X5_DXZS_Info>();
                var entityList = new LN11X5_DXZSManager().QueryLN11X5_DXZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<LN11X5_DXZS>, LN11X5_DXZS, List<LN11X5_DXZS_Info>, LN11X5_DXZS_Info>(entityList, ref infoList,
                    () => { return new LN11X5_DXZS_Info(); },
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
        public LN11X5_GHZS_InfoCollection QueryLN11X5_GHZS(int index)
        {
            LN11X5_GHZS_InfoCollection Collection = new LN11X5_GHZS_InfoCollection();
            var list = this.QueryGameChart<LN11X5_GHZS_Info>(string.Format("QueryLN11X5_GHZS_{0}", index), () =>
            {
                var infoList = new List<LN11X5_GHZS_Info>();
                var entityList = new LN11X5_GHZSManager().QueryLN11X5_GHZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<LN11X5_GHZS>, LN11X5_GHZS, List<LN11X5_GHZS_Info>, LN11X5_GHZS_Info>(entityList, ref infoList,
                    () => { return new LN11X5_GHZS_Info(); },
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
        public LN11X5_HZZS_InfoCollection QueryLN11X5_HZZS(int index)
        {
            LN11X5_HZZS_InfoCollection Collection = new LN11X5_HZZS_InfoCollection();
            var list = this.QueryGameChart<LN11X5_HZZS_Info>(string.Format("QueryLN11X5_HZZS_{0}", index), () =>
            {
                var infoList = new List<LN11X5_HZZS_Info>();
                var entityList = new LN11X5_HZZSManager().QueryLN11X5_HZZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<LN11X5_HZZS>, LN11X5_HZZS, List<LN11X5_HZZS_Info>, LN11X5_HZZS_Info>(entityList, ref infoList,
                    () => { return new LN11X5_HZZS_Info(); },
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
        public LN11X5_JBZS_InfoCollection QueryLN11X5_JBZS(int index)
        {
            LN11X5_JBZS_InfoCollection Collection = new LN11X5_JBZS_InfoCollection();
            var list = this.QueryGameChart<LN11X5_JBZS_Info>(string.Format("QueryLN11X5_JBZS_{0}", index), () =>
            {
                var infoList = new List<LN11X5_JBZS_Info>();
                var entityList = new LN11X5_JBZSManager().QueryLN11X5_JBZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<LN11X5_JBZS>, LN11X5_JBZS, List<LN11X5_JBZS_Info>, LN11X5_JBZS_Info>(entityList, ref infoList,
                    () => { return new LN11X5_JBZS_Info(); },
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
        public LN11X5_JOZS_InfoCollection QueryLN11X5_JOZS(int index)
        {
            LN11X5_JOZS_InfoCollection Collection = new LN11X5_JOZS_InfoCollection();
            var list = this.QueryGameChart<LN11X5_JOZS_Info>(string.Format("QueryLN11X5_JOZS_{0}", index), () =>
            {
                var infoList = new List<LN11X5_JOZS_Info>();
                var entityList = new LN11X5_JOZSManager().QueryLN11X5_JOZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<LN11X5_JOZS>, LN11X5_JOZS, List<LN11X5_JOZS_Info>, LN11X5_JOZS_Info>(entityList, ref infoList,
                    () => { return new LN11X5_JOZS_Info(); },
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
        public LN11X5_Q1ZS_InfoCollection QueryLN11X5_Q1ZS(int index)
        {
            LN11X5_Q1ZS_InfoCollection Collection = new LN11X5_Q1ZS_InfoCollection();
            var list = this.QueryGameChart<LN11X5_Q1ZS_Info>(string.Format("QueryLN11X5_Q1ZS_{0}", index), () =>
            {
                var infoList = new List<LN11X5_Q1ZS_Info>();
                var entityList = new LN11X5_Q1ZSManager().QueryLN11X5_Q1ZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<LN11X5_Q1ZS>, LN11X5_Q1ZS, List<LN11X5_Q1ZS_Info>, LN11X5_Q1ZS_Info>(entityList, ref infoList,
                    () => { return new LN11X5_Q1ZS_Info(); },
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
        public LN11X5_Q2ZS_InfoCollection QueryLN11X5_Q2ZS(int index)
        {
            LN11X5_Q2ZS_InfoCollection Collection = new LN11X5_Q2ZS_InfoCollection();
            var list = this.QueryGameChart<LN11X5_Q2ZS_Info>(string.Format("QueryLN11X5_Q2ZS_{0}", index), () =>
            {
                var infoList = new List<LN11X5_Q2ZS_Info>();
                var entityList = new LN11X5_Q2ZSManager().QueryLN11X5_Q2ZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<LN11X5_Q2ZS>, LN11X5_Q2ZS, List<LN11X5_Q2ZS_Info>, LN11X5_Q2ZS_Info>(entityList, ref infoList,
                    () => { return new LN11X5_Q2ZS_Info(); },
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
        public LN11X5_Q3ZS_InfoCollection QueryLN11X5_Q3ZS(int index)
        {
            LN11X5_Q3ZS_InfoCollection Collection = new LN11X5_Q3ZS_InfoCollection();
            var list = this.QueryGameChart<LN11X5_Q3ZS_Info>(string.Format("QueryLN11X5_Q3ZS_{0}", index), () =>
            {
                var infoList = new List<LN11X5_Q3ZS_Info>();
                var entityList = new LN11X5_Q3ZSManager().QueryLN11X5_Q3ZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<LN11X5_Q3ZS>, LN11X5_Q3ZS, List<LN11X5_Q3ZS_Info>, LN11X5_Q3ZS_Info>(entityList, ref infoList,
                    () => { return new LN11X5_Q3ZS_Info(); },
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
        public GameWinNumber_InfoCollection QueryLN11X5_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new LN11X5_GameWinNumberManager().QueryLN11X5_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<LN11X5_GameWinNumber>, LN11X5_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryLN11X5_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new LN11X5_GameWinNumberManager().QueryLN11X5_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<LN11X5_GameWinNumber>, LN11X5_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
            var manager = new LN11X5_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<LN11X5_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }
    }
}
