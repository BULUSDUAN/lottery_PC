
using KaSon.FrameWork.Common;
using KaSon.FrameWork.ORM.Helper.WinNumber.Manage;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.CoreModel;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber
{
    public class LotteryDataBusiness_CQ11X5 : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get
            {
                return "CQ11X5";
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

                this.ClearGameChartCache("QueryCQ11X5_012DWZS");
                this.ClearGameChartCache("QueryCQ11X5_012LZZS");
                this.ClearGameChartCache("QueryCQ11X5_2LZS");
                this.ClearGameChartCache("QueryCQ11X5_CHZS");
                this.ClearGameChartCache("QueryCQ11X5_DLZS");
                this.ClearGameChartCache("QueryCQ11X5_GHZS");
                this.ClearGameChartCache("QueryCQ11X5_HZZS");
                this.ClearGameChartCache("QueryCQ11X5_JBZS");
                this.ClearGameChartCache("QueryCQ11X5_KDZS");
                this.ClearGameChartCache("QueryCQ11X5_Q1JBZS");
                this.ClearGameChartCache("QueryCQ11X5_Q1XTZS");
                this.ClearGameChartCache("QueryCQ11X5_Q2JBZS");
                this.ClearGameChartCache("QueryCQ11X5_Q2XTZS");
                this.ClearGameChartCache("QueryCQ11X5_Q3JBZS");
                this.ClearGameChartCache("QueryCQ11X5_Q3XTZS");
                this.ClearGameChartCache("QueryCQ11X5_XTZS");

                this.ClearNewWinNumberCache("QueryCQ11X5_GameWinNumber");

                AddCQ11X5_JBZS(issuseNumber, winNumber);
                AddCQ11X5_012DWZS(issuseNumber, winNumber);
                AddCQ11X5_012LZZS(issuseNumber, winNumber);
                AddCQ11X5_HZZS(issuseNumber, winNumber);
                AddCQ11X5_CHZS(issuseNumber, winNumber);
                AddCQ11X5_XTZS(issuseNumber, winNumber);
                AddCQ11X5_GHZS(issuseNumber, winNumber);
                AddCQ11X5_2LZS(issuseNumber, winNumber);
                AddCQ11X5_DLZS(issuseNumber, winNumber);
                AddCQ11X5_Q1JBZS(issuseNumber, winNumber);
                AddCQ11X5_Q1XTZS(issuseNumber, winNumber);
                AddCQ11X5_Q2JBZS(issuseNumber, winNumber);
                AddCQ11X5_Q2XTZS(issuseNumber, winNumber);
                AddCQ11X5_Q3JBZS(issuseNumber, winNumber);
                AddCQ11X5_Q3XTZS(issuseNumber, winNumber);
                AddCQ11X5_KDZS(issuseNumber, winNumber);

                Add_GameWinNumber(issuseNumber, winNumber);
                LottertDataDB.Commit();
            }
        }

        #region 生成走势数据

        public void Add_GameWinNumber(string issuseNumber, string winNumber)
        {
            new KJGameIssuseBusiness().IssusePrize(this.CurrentGameCode, issuseNumber, winNumber);
            var manager = new CQ11X5_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddCQ11X5_GameWinNumber(new EntityModel.CQ11X5_GameWinNumber
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
        private void AddCQ11X5_JBZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_JBZSManager();
            var issuse = manager.QueryCQ11X5_JBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;
            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hezhi = Convert.ToInt32(winRed[0]) + Convert.ToInt32(winRed[1]) + Convert.ToInt32(winRed[2]) + Convert.ToInt32(winRed[3]) + Convert.ToInt32(winRed[4]);
            var last = manager.QueryLastCQ11X5_JBZS();
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
            string DX_Proportion = string.Empty;
            string JO_Proportion = string.Empty;
            int d = 0;
            int x = 0;
            int j = 0;
            int o = 0;
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
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("HeZhi", hezhi);
            dic.Add("JO_Proportion", JO_Proportion);
            dic.Add("DX_Proportion", DX_Proportion);
            dic.Add("Duplicate", chonghao);
            dic.Add("ContinuousNumber", lianhao);
            var entity = CreateNewEntity<CQ11X5_JBZS>(dic, (p) =>
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

            manager.AddCQ11X5_JBZS(entity);
        }

        /// <summary>
        /// 添加012定位走势
        /// </summary>
        private void AddCQ11X5_012DWZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_012DWZSManager();
            var issuse = manager.QueryCQ11X5_012DWZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var redball = new int[]{
                Convert.ToInt32(winRed[0]) %3,
                Convert.ToInt32(winRed[1]) %3,
                 Convert.ToInt32(winRed[2])%3,
                 Convert.ToInt32(winRed[3]) %3,
                 Convert.ToInt32(winRed[4]) %3
            };
            string C012BL = string.Format("{0}:{1}:{2}", redball.Where(p => p == 0).Count(), redball.Where(p => p == 1).Count(), redball.Where(p => p == 2).Count());
            string C012XT = string.Empty;
            foreach (var item in winRed)
            {
                C012XT += " " + (Convert.ToInt32(item) % 3).ToString();
            }
            var last = manager.QueryLastCQ11X5_012DWZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("C012XT", C012XT);
            dic.Add("C012BL", C012BL);

            var entity = this.CreateNewEntity<CQ11X5_012DWZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (p.Name.StartsWith("C31_"))
                {
                    var order = p.Name.Replace("C31_", string.Empty);
                    lastValue = redball[0] == Convert.ToInt32(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("C32_"))
                {
                    var order = p.Name.Replace("C32_", string.Empty);
                    lastValue = redball[1] == Convert.ToInt32(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("C33_"))
                {
                    var order = p.Name.Replace("C33_", string.Empty);
                    lastValue = redball[2] == Convert.ToInt32(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("C34_"))
                {
                    var order = p.Name.Replace("C34_", string.Empty);
                    lastValue = redball[3] == Convert.ToInt32(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("C35_"))
                {
                    var order = p.Name.Replace("C35_", string.Empty);
                    lastValue = redball[4] == Convert.ToInt32(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQ11X5_012DWZS(entity);
        }
        /// <summary>
        /// 添加012定位走势
        /// </summary>
        private void AddCQ11X5_012LZZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_012LZZSManager();
            var issuse = manager.QueryCQ11X5_012LZZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var redball = new int[]{
                Convert.ToInt32(winRed[0]) %3,
                Convert.ToInt32(winRed[1]) %3,
                Convert.ToInt32(winRed[2]) %3,
                Convert.ToInt32(winRed[3]) %3,
                Convert.ToInt32(winRed[4]) %3
            };
            string C012BL = string.Format("{0}:{1}:{2}", redball.Where(p => p == 0).Count(), redball.Where(p => p == 1).Count(), redball.Where(p => p == 2).Count());
            var last = manager.QueryLastCQ11X5_012LZZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<CQ11X5_012LZZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (p.Name.StartsWith("C_"))
                {
                    var order = p.Name.Replace("C_", string.Empty);
                    lastValue = C012BL.Replace(":", "") == order ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQ11X5_012LZZS(entity);
        }
        /// <summary>
        /// 和值走势
        /// </summary>
        public void AddCQ11X5_HZZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_HZZSManager();
            var issuse = manager.QueryCQ11X5_HZZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]) + int.Parse(winRed[3]) + int.Parse(winRed[4]);
            var last = manager.QueryLastCQ11X5_HZZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("HeZhi", hz);
            var entity = this.CreateNewEntity<CQ11X5_HZZS>(dic, (p) =>
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

            manager.AddCQ11X5_HZZS(entity);
        }
        /// <summary>
        /// 添加重号走势
        /// </summary>
        private void AddCQ11X5_CHZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_CHZSManager();
            var issuse = manager.QueryCQ11X5_CHZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var last = manager.QueryLastCQ11X5_CHZS();
            int chonghao = 0;
            int D1_01_04 = 0;
            int D2_05_08 = 0;
            int D3_09_11 = 0;
            for (int i = 0; i < (last == null ? 0 : last.WinNumber.Split(',').Length); i++)
            {
                if (last.WinNumber.Contains(winRed[i]))
                {
                    chonghao++;
                }
            }
            for (int i = 0; i < winRed.Length; i++)
            {
                if (Convert.ToInt32(winRed[i]) < 5)
                    D1_01_04++;
                if (Convert.ToInt32(winRed[i]) < 9 && Convert.ToInt32(winRed[i]) > 4)
                    D2_05_08++;
                if (Convert.ToInt32(winRed[i]) > 8)
                    D3_09_11++;
            }

            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("Duplicate", chonghao);
            dic.Add("D1_01_04", D1_01_04);
            dic.Add("D2_05_08", D2_05_08);
            dic.Add("D3_09_11", D3_09_11);
            var entity = this.CreateNewEntity<CQ11X5_CHZS>(dic, (p) =>
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

            manager.AddCQ11X5_CHZS(entity);
        }
        /// <summary>
        /// 添加形态走势
        /// </summary>
        private void AddCQ11X5_XTZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_XTZSManager();
            var issuse = manager.QueryCQ11X5_XTZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

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


            string ZH_Proportion = string.Empty;
            int z = 0;
            int h = 0;
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
            }
            ZH_Proportion = string.Format("{0}:{1}", z, h);
            var last = manager.QueryLastCQ11X5_XTZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<CQ11X5_XTZS>(dic, (p) =>
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

                if (p.Name.StartsWith("JO_"))
                {
                    var order = p.Name.Replace("JO_", string.Empty);
                    if ((order == "Q_J" && JO_Proportion == "5:0") || (order == "1J4O" && JO_Proportion == "1:4") || (order == "2J3O" && JO_Proportion == "2:3") || (order == "3J2O" && JO_Proportion == "3:2") || (order == "4J1O" && JO_Proportion == "4:1") || (order == "Q_O" && JO_Proportion == "0:5"))
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("ZH_"))
                {
                    var order = p.Name.Replace("ZH_", string.Empty);
                    if ((order == "Q_Z" && ZH_Proportion == "5:0") || (order == "1Z4H" && ZH_Proportion == "1:4") || (order == "2Z3H" && ZH_Proportion == "2:3") || (order == "3Z2H" && ZH_Proportion == "3:2") || (order == "4Z1H" && ZH_Proportion == "4:1") || (order == "Q_H" && ZH_Proportion == "0:5"))
                    {
                        lastValue = 0;
                    }
                }
                return lastValue;
            });

            manager.AddCQ11X5_XTZS(entity);
        }
        /// <summary>
        /// 添加隔号走势
        /// </summary>
        private void AddCQ11X5_GHZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_GHZSManager();
            var issuse = manager.QueryCQ11X5_GHZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var GHao = 0;
            var J_GHao = 0;
            var G_GHao = 0;
            if (winRed.Contains("01") && winRed.Contains("03"))
            {
                GHao++;
                J_GHao++;
            }
            if (winRed.Contains("02") && winRed.Contains("04"))
            {
                GHao++;
                G_GHao++;
            }
            if (winRed.Contains("03") && winRed.Contains("05"))
            {
                GHao++;
                J_GHao++;
            }
            if (winRed.Contains("04") && winRed.Contains("06"))
            {
                GHao++;
                G_GHao++;
            }
            if (winRed.Contains("05") && winRed.Contains("07"))
            {
                GHao++;
                J_GHao++;
            }
            if (winRed.Contains("06") && winRed.Contains("08"))
            {
                GHao++;
                G_GHao++;
            }
            if (winRed.Contains("07") && winRed.Contains("09"))
            {
                GHao++;
                J_GHao++;
            }
            if (winRed.Contains("08") && winRed.Contains("10"))
            {
                GHao++;
                G_GHao++;
            }
            if (winRed.Contains("09") && winRed.Contains("11"))
            {
                GHao++;
                J_GHao++;
            }
            var last = manager.QueryLastCQ11X5_GHZS();
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
            dic.Add("J_GHao", J_GHao);
            dic.Add("O_GHao", G_GHao);
            var entity = this.CreateNewEntity<CQ11X5_GHZS>(dic, (p) =>
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

            manager.AddCQ11X5_GHZS(entity);
        }
        /// <summary>
        /// 添加2连走势
        /// </summary>
        private void AddCQ11X5_2LZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_2LZSManager();
            var issuse = manager.QueryCQ11X5_2LZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var GHao = 0;
            var DLH = string.Empty;
            var XLH = string.Empty;
            if (winRed.Contains("01") && winRed.Contains("02"))
            {
                GHao++;
                XLH += " 01 02";
            }
            if (winRed.Contains("02") && winRed.Contains("03"))
            {
                GHao++;
                XLH += " 02 03";
            }
            if (winRed.Contains("03") && winRed.Contains("04"))
            {
                GHao++;
                XLH += " 03 04";
            }
            if (winRed.Contains("04") && winRed.Contains("05"))
            {
                GHao++;
                XLH += " 04 05";
            }
            if (winRed.Contains("05") && winRed.Contains("06"))
            {
                GHao++;
                XLH += " 05 06";
            }
            if (winRed.Contains("06") && winRed.Contains("07"))
            {
                GHao++;
                DLH += " 06 07";
            }
            if (winRed.Contains("07") && winRed.Contains("08"))
            {
                GHao++;
                DLH += " 07 08";
            }
            if (winRed.Contains("08") && winRed.Contains("09"))
            {
                GHao++;
                DLH += " 08 09";
            }
            if (winRed.Contains("09") && winRed.Contains("10"))
            {
                GHao++;
                DLH += " 09 10";
            }
            if (winRed.Contains("10") && winRed.Contains("11"))
            {
                GHao++;
                DLH += " 10 11";
            }
            var last = manager.QueryLastCQ11X5_2LZS();
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
            dic.Add("DLH", DLH);
            dic.Add("XLH", XLH);
            var entity = this.CreateNewEntity<CQ11X5_2LZS>(dic, (p) =>
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

            manager.AddCQ11X5_2LZS(entity);
        }
        /// <summary>
        /// 添加多连走势
        /// </summary>
        private void AddCQ11X5_DLZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_DLZSManager();
            var issuse = manager.QueryCQ11X5_DLZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var last = manager.QueryLastCQ11X5_DLZS();
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
            var entity = this.CreateNewEntity<CQ11X5_DLZS>(dic, (p) =>
            {
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                return lastValue;
            });

            manager.AddCQ11X5_DLZS(entity);
        }
        /// <summary>
        /// 添加前1基本走势
        /// </summary>
        private void AddCQ11X5_Q1JBZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_Q1JBZSManager();
            var issuse = manager.QueryCQ11X5_Q1JBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var DaXiao = Convert.ToInt32(winRed[0]) > 5 ? "大" : "小";
            var JO = Convert.ToInt32(winRed[0]) % 2 == 0 ? "偶" : "奇";
            var ZH = (Convert.ToInt32(winRed[0]) == 1 || Convert.ToInt32(winRed[0]) == 2 || Convert.ToInt32(winRed[0]) == 3 || Convert.ToInt32(winRed[0]) == 5 || Convert.ToInt32(winRed[0]) == 7 || Convert.ToInt32(winRed[0]) == 11) ? "质" : "合";
            var C3Y = Convert.ToInt32(winRed[0]) % 3;
            var hezhi = Convert.ToInt32(winRed[0]) + Convert.ToInt32(winRed[1]) + Convert.ToInt32(winRed[2]) + Convert.ToInt32(winRed[3]) + Convert.ToInt32(winRed[4]);

            var last = manager.QueryLastCQ11X5_Q1JBZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("HeZhi", hezhi);
            var entity = this.CreateNewEntity<CQ11X5_Q1JBZS>(dic, (p) =>
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
                if (p.Name == "JO_J")
                {
                    lastValue = JO == "奇" ? 0 : lastValue;
                }
                if (p.Name == "JO_O")
                {
                    lastValue = JO == "偶" ? 0 : lastValue;
                }
                if (p.Name == "DX_D")
                {
                    lastValue = DaXiao == "大" ? 0 : lastValue;
                }
                if (p.Name == "DX_X")
                {
                    lastValue = DaXiao == "小" ? 0 : lastValue;
                }
                if (p.Name == "ZH_Z")
                {
                    lastValue = ZH == "质" ? 0 : lastValue;
                }
                if (p.Name == "ZH_H")
                {
                    lastValue = ZH == "合" ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQ11X5_Q1JBZS(entity);
        }

        /// <summary>
        /// 添加前1形态走势
        /// </summary>
        private void AddCQ11X5_Q1XTZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_Q1XTZSManager();
            var issuse = manager.QueryCQ11X5_Q1XTZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var DaXiao = Convert.ToInt32(winRed[0]) > 5 ? "大" : "小";
            var DanS = Convert.ToInt32(winRed[0]) % 2 == 0 ? "双" : "单";
            var JO = Convert.ToInt32(winRed[0]) % 2 == 0 ? "偶" : "奇";
            var ZH = (Convert.ToInt32(winRed[0]) == 1 || Convert.ToInt32(winRed[0]) == 2 || Convert.ToInt32(winRed[0]) == 3 || Convert.ToInt32(winRed[0]) == 5 || Convert.ToInt32(winRed[0]) == 7 || Convert.ToInt32(winRed[0]) == 11) ? "质" : "合";
            var C3Y = Convert.ToInt32(winRed[0]) % 3;
            var DXDS = DaXiao + DanS;

            var last = manager.QueryLastCQ11X5_Q1XTZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("DDan", (DXDS == "大单") ? 0 : (last == null ? 1 : last.DDan + 1));
            dic.Add("DS", (DXDS == "大双") ? 0 : (last == null ? 1 : last.DS + 1));
            dic.Add("XDan", (DXDS == "小单") ? 0 : (last == null ? 1 : last.XDan + 1));
            dic.Add("XS", (DXDS == "小双") ? 0 : (last == null ? 1 : last.XS + 1));
            var entity = this.CreateNewEntity<CQ11X5_Q1XTZS>(dic, (p) =>
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
                if (p.Name == "DS_D")
                {
                    lastValue = DanS == "单" ? 0 : lastValue;
                }
                if (p.Name == "DS_S")
                {
                    lastValue = DanS == "双" ? 0 : lastValue;
                }
                if (p.Name == "JO_J")
                {
                    lastValue = JO == "奇" ? 0 : lastValue;
                }
                if (p.Name == "JO_O")
                {
                    lastValue = JO == "偶" ? 0 : lastValue;
                }
                if (p.Name == "DX_D")
                {
                    lastValue = DaXiao == "大" ? 0 : lastValue;
                }
                if (p.Name == "DX_X")
                {
                    lastValue = DaXiao == "小" ? 0 : lastValue;
                }
                if (p.Name == "ZH_Z")
                {
                    lastValue = ZH == "质" ? 0 : lastValue;
                }
                if (p.Name == "ZH_H")
                {
                    lastValue = ZH == "合" ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQ11X5_Q1XTZS(entity);
        }
        /// <summary>
        /// 添加前2基本走势
        /// </summary>
        private void AddCQ11X5_Q2JBZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_Q2JBZSManager();
            var issuse = manager.QueryCQ11X5_Q2JBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var DaXiao = Convert.ToInt32(winRed[0]) > 5 ? "大" : "小";
            var JO = Convert.ToInt32(winRed[0]) % 2 == 0 ? "偶" : "奇";
            var ZH = (Convert.ToInt32(winRed[0]) == 1 || Convert.ToInt32(winRed[0]) == 2 || Convert.ToInt32(winRed[0]) == 3 || Convert.ToInt32(winRed[0]) == 5 || Convert.ToInt32(winRed[0]) == 7 || Convert.ToInt32(winRed[0]) == 11) ? "质" : "合";

            var DaXiao1 = Convert.ToInt32(winRed[1]) > 5 ? "大" : "小";
            var JO1 = Convert.ToInt32(winRed[1]) % 2 == 0 ? "偶" : "奇";
            var ZH1 = (Convert.ToInt32(winRed[1]) == 1 || Convert.ToInt32(winRed[1]) == 2 || Convert.ToInt32(winRed[1]) == 3 || Convert.ToInt32(winRed[1]) == 5 || Convert.ToInt32(winRed[1]) == 7 || Convert.ToInt32(winRed[1]) == 11) ? "质" : "合";


            var last = manager.QueryLastCQ11X5_Q2JBZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<CQ11X5_Q2JBZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_"))
                {
                    var order = p.Name.Replace("Red_", string.Empty);
                    lastValue = winRed[0] == order ? 0 : lastValue;
                }
                if (p.Name == "JO_J")
                {
                    lastValue = JO == "奇" ? 0 : lastValue;
                }
                if (p.Name == "JO_O")
                {
                    lastValue = JO == "偶" ? 0 : lastValue;
                }
                if (p.Name == "DX_D")
                {
                    lastValue = DaXiao == "大" ? 0 : lastValue;
                }
                if (p.Name == "DX_X")
                {
                    lastValue = DaXiao == "小" ? 0 : lastValue;
                }
                if (p.Name == "ZH_Z")
                {
                    lastValue = ZH == "质" ? 0 : lastValue;
                }
                if (p.Name == "ZH_H")
                {
                    lastValue = ZH == "合" ? 0 : lastValue;
                }

                if (p.Name.StartsWith("Red1_"))
                {
                    var order = p.Name.Replace("Red1_", string.Empty);
                    lastValue = winRed[1] == order ? 0 : lastValue;
                }
                if (p.Name == "JO1_J")
                {
                    lastValue = JO1 == "奇" ? 0 : lastValue;
                }
                if (p.Name == "JO1_O")
                {
                    lastValue = JO1 == "偶" ? 0 : lastValue;
                }
                if (p.Name == "DX1_D")
                {
                    lastValue = DaXiao1 == "大" ? 0 : lastValue;
                }
                if (p.Name == "DX1_X")
                {
                    lastValue = DaXiao1 == "小" ? 0 : lastValue;
                }
                if (p.Name == "ZH1_Z")
                {
                    lastValue = ZH1 == "质" ? 0 : lastValue;
                }
                if (p.Name == "ZH1_H")
                {
                    lastValue = ZH1 == "合" ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQ11X5_Q2JBZS(entity);
        }

        /// <summary>
        /// 添加前2形态走势
        /// </summary>
        private void AddCQ11X5_Q2XTZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_Q2XTZSManager();
            var issuse = manager.QueryCQ11X5_Q2XTZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;
            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var DaXiao = Convert.ToInt32(winRed[0]) > 5 ? "大" : "小";
            var DanS = Convert.ToInt32(winRed[0]) % 2 == 0 ? "双" : "单";
            var C3Y = Convert.ToInt32(winRed[0]) % 3;

            var DaXiao1 = Convert.ToInt32(winRed[1]) > 5 ? "大" : "小";
            var DanS1 = Convert.ToInt32(winRed[1]) % 2 == 0 ? "双" : "单";
            var C3Y1 = Convert.ToInt32(winRed[1]) % 3;

            var DXDS = DaXiao + DanS;
            var DXDS1 = DaXiao1 + DanS1;

            var DX = DaXiao + DaXiao1;
            var DS = DanS + DanS1;

            var last = manager.QueryLastCQ11X5_Q2XTZS();
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

            dic.Add("DX_DD", (DX == "大大") ? 0 : (last == null ? 1 : last.DX_DD + 1));
            dic.Add("DX_DX", (DX == "大小") ? 0 : (last == null ? 1 : last.DX_DX + 1));
            dic.Add("DX_XD", (DX == "小大") ? 0 : (last == null ? 1 : last.DX_XD + 1));
            dic.Add("DX_XX", (DX == "小小") ? 0 : (last == null ? 1 : last.DX_XX + 1));

            dic.Add("DS_DD", (DS == "单单") ? 0 : (last == null ? 1 : last.DS_DD + 1));
            dic.Add("DS_DS", (DS == "单双") ? 0 : (last == null ? 1 : last.DS_DS + 1));
            dic.Add("DS_SD", (DS == "双单") ? 0 : (last == null ? 1 : last.DS_SD + 1));
            dic.Add("DS_SS", (DS == "双双") ? 0 : (last == null ? 1 : last.DS_SS + 1));
            var entity = this.CreateNewEntity<CQ11X5_Q2XTZS>(dic, (p) =>
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

            manager.AddCQ11X5_Q2XTZS(entity);
        }

        /// <summary>
        /// 添加前3基本走势
        /// </summary>
        private void AddCQ11X5_Q3JBZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_Q3JBZSManager();
            var issuse = manager.QueryCQ11X5_Q3JBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;
            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var last = manager.QueryLastCQ11X5_Q3JBZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<CQ11X5_Q3JBZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_"))
                {
                    var order = p.Name.Replace("Red_", string.Empty);
                    lastValue = winRed[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Red1_"))
                {
                    var order = p.Name.Replace("Red1_", string.Empty);
                    lastValue = winRed[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Red2_"))
                {
                    var order = p.Name.Replace("Red2_", string.Empty);
                    lastValue = winRed[2] == order ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQ11X5_Q3JBZS(entity);
        }
        /// <summary>
        /// 添加前3形态走势
        /// </summary>
        private void AddCQ11X5_Q3XTZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_Q3XTZSManager();
            var issuse = manager.QueryCQ11X5_Q3XTZSIssuseNumber(issuseNumber);
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
            var last = manager.QueryLastCQ11X5_Q3XTZS();
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
            var entity = this.CreateNewEntity<CQ11X5_Q3XTZS>(dic, (p) =>
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

            manager.AddCQ11X5_Q3XTZS(entity);
        }

        /// <summary>
        /// 添加跨度走势
        /// </summary>
        private void AddCQ11X5_KDZS(string issuseNumber, string winNumber)
        {
            var manager = new CQ11X5_KDZSManager();
            var issuse = manager.QueryCQ11X5_KDZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var kuadu = Convert.ToInt32(winRed.Max()) - Convert.ToInt32(winRed.Min());
            var JO = Convert.ToInt32(kuadu) % 2 == 0 ? "偶" : "奇";
            var ZH = (Convert.ToInt32(kuadu) == 1 || Convert.ToInt32(kuadu) == 2 || Convert.ToInt32(kuadu) == 3 || Convert.ToInt32(kuadu) == 5 || Convert.ToInt32(kuadu) == 7 || Convert.ToInt32(kuadu) == 11) ? "质" : "合";
            var C3Y = Convert.ToInt32(kuadu) % 3;
            var last = manager.QueryLastCQ11X5_KDZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("KuaDu", kuadu);
            var entity = this.CreateNewEntity<CQ11X5_KDZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("KD_"))
                {
                    var order = p.Name.Replace("KD_", string.Empty);
                    lastValue = kuadu == Convert.ToInt32(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("C3_"))
                {
                    var order = p.Name.Replace("C3_", string.Empty);
                    lastValue = C3Y == Convert.ToInt32(order) ? 0 : lastValue;
                }
                if (p.Name == "JO_J")
                {
                    lastValue = JO == "奇" ? 0 : lastValue;
                }
                if (p.Name == "JO_O")
                {
                    lastValue = JO == "偶" ? 0 : lastValue;
                }
                if (p.Name == "ZH_Z")
                {
                    lastValue = ZH == "质" ? 0 : lastValue;
                }
                if (p.Name == "ZH_H")
                {
                    lastValue = ZH == "合" ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddCQ11X5_KDZS(entity);
        }
        #endregion

        #region 查询数据

        public CQ11X5_012DWZS_InfoCollection QueryCQ11X5_012DWZS(int index)
        {
            CQ11X5_012DWZS_InfoCollection Collection = new CQ11X5_012DWZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_012DWZS_Info>(string.Format("QueryCQ11X5_012DWZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_012DWZS_Info>();
                var entityList = new CQ11X5_012DWZSManager().QueryCQ11X5_012DWZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_012DWZS>, CQ11X5_012DWZS, List<CQ11X5_012DWZS_Info>, CQ11X5_012DWZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_012DWZS_Info(); },
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
        public CQ11X5_012LZZS_InfoCollection QueryCQ11X5_012LZZS(int index)
        {
            CQ11X5_012LZZS_InfoCollection Collection = new CQ11X5_012LZZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_012LZZS_Info>(string.Format("QueryCQ11X5_012LZZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_012LZZS_Info>();
                var entityList = new CQ11X5_012LZZSManager().QueryCQ11X5_012LZZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_012LZZS>, CQ11X5_012LZZS, List<CQ11X5_012LZZS_Info>, CQ11X5_012LZZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_012LZZS_Info(); },
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
        public CQ11X5_2LZS_InfoCollection QueryCQ11X5_2LZS(int index)
        {
            CQ11X5_2LZS_InfoCollection Collection = new CQ11X5_2LZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_2LZS_Info>(string.Format("QueryCQ11X5_2LZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_2LZS_Info>();
                var entityList = new CQ11X5_2LZSManager().QueryCQ11X5_2LZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_2LZS>, CQ11X5_2LZS, List<CQ11X5_2LZS_Info>, CQ11X5_2LZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_2LZS_Info(); },
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
        public CQ11X5_CHZS_InfoCollection QueryCQ11X5_CHZS(int index)
        {
            CQ11X5_CHZS_InfoCollection Collection = new CQ11X5_CHZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_CHZS_Info>(string.Format("QueryCQ11X5_CHZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_CHZS_Info>();
                var entityList = new CQ11X5_CHZSManager().QueryCQ11X5_CHZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_CHZS>, CQ11X5_CHZS, List<CQ11X5_CHZS_Info>, CQ11X5_CHZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_CHZS_Info(); },
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
        public CQ11X5_DLZS_InfoCollection QueryCQ11X5_DLZS(int index)
        {
            CQ11X5_DLZS_InfoCollection Collection = new CQ11X5_DLZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_DLZS_Info>(string.Format("QueryCQ11X5_DLZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_DLZS_Info>();
                var entityList = new CQ11X5_DLZSManager().QueryCQ11X5_DLZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_DLZS>, CQ11X5_DLZS, List<CQ11X5_DLZS_Info>, CQ11X5_DLZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_DLZS_Info(); },
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
        public CQ11X5_GHZS_InfoCollection QueryCQ11X5_GHZS(int index)
        {
            CQ11X5_GHZS_InfoCollection Collection = new CQ11X5_GHZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_GHZS_Info>(string.Format("QueryCQ11X5_GHZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_GHZS_Info>();
                var entityList = new CQ11X5_GHZSManager().QueryCQ11X5_GHZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_GHZS>, CQ11X5_GHZS, List<CQ11X5_GHZS_Info>, CQ11X5_GHZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_GHZS_Info(); },
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
        public CQ11X5_HZZS_InfoCollection QueryCQ11X5_HZZS(int index)
        {
            CQ11X5_HZZS_InfoCollection Collection = new CQ11X5_HZZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_HZZS_Info>(string.Format("QueryCQ11X5_HZZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_HZZS_Info>();
                var entityList = new CQ11X5_HZZSManager().QueryCQ11X5_HZZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_HZZS>, CQ11X5_HZZS, List<CQ11X5_HZZS_Info>, CQ11X5_HZZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_HZZS_Info(); },
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
        public CQ11X5_JBZS_InfoCollection QueryCQ11X5_JBZS(int index)
        {
            CQ11X5_JBZS_InfoCollection Collection = new CQ11X5_JBZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_JBZS_Info>(string.Format("QueryCQ11X5_JBZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_JBZS_Info>();
                var entityList = new CQ11X5_JBZSManager().QueryCQ11X5_JBZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_JBZS>, CQ11X5_JBZS, List<CQ11X5_JBZS_Info>, CQ11X5_JBZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_JBZS_Info(); },
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
        public CQ11X5_KDZS_InfoCollection QueryCQ11X5_KDZS(int index)
        {
            CQ11X5_KDZS_InfoCollection Collection = new CQ11X5_KDZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_KDZS_Info>(string.Format("QueryCQ11X5_KDZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_KDZS_Info>();
                var entityList = new CQ11X5_KDZSManager().QueryCQ11X5_KDZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_KDZS>, CQ11X5_KDZS, List<CQ11X5_KDZS_Info>, CQ11X5_KDZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_KDZS_Info(); },
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
        public CQ11X5_Q1JBZS_InfoCollection QueryCQ11X5_Q1JBZS(int index)
        {
            CQ11X5_Q1JBZS_InfoCollection Collection = new CQ11X5_Q1JBZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_Q1JBZS_Info>(string.Format("QueryCQ11X5_Q1JBZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_Q1JBZS_Info>();
                var entityList = new CQ11X5_Q1JBZSManager().QueryCQ11X5_Q1JBZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_Q1JBZS>, CQ11X5_Q1JBZS, List<CQ11X5_Q1JBZS_Info>, CQ11X5_Q1JBZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_Q1JBZS_Info(); },
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
        public CQ11X5_Q1XTZS_InfoCollection QueryCQ11X5_Q1XTZS(int index)
        {
            CQ11X5_Q1XTZS_InfoCollection Collection = new CQ11X5_Q1XTZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_Q1XTZS_Info>(string.Format("QueryCQ11X5_Q1XTZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_Q1XTZS_Info>();
                var entityList = new CQ11X5_Q1XTZSManager().QueryCQ11X5_Q1XTZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_Q1XTZS>, CQ11X5_Q1XTZS, List<CQ11X5_Q1XTZS_Info>, CQ11X5_Q1XTZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_Q1XTZS_Info(); },
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
        public CQ11X5_Q2JBZS_InfoCollection QueryCQ11X5_Q2JBZS(int index)
        {
            CQ11X5_Q2JBZS_InfoCollection Collection = new CQ11X5_Q2JBZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_Q2JBZS_Info>(string.Format("QueryCQ11X5_Q2JBZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_Q2JBZS_Info>();
                var entityList = new CQ11X5_Q2JBZSManager().QueryCQ11X5_Q2JBZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_Q2JBZS>, CQ11X5_Q2JBZS, List<CQ11X5_Q2JBZS_Info>, CQ11X5_Q2JBZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_Q2JBZS_Info(); },
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
        public CQ11X5_Q2XTZS_InfoCollection QueryCQ11X5_Q2XTZS(int index)
        {
            CQ11X5_Q2XTZS_InfoCollection Collection = new CQ11X5_Q2XTZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_Q2XTZS_Info>(string.Format("QueryCQ11X5_Q2XTZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_Q2XTZS_Info>();
                var entityList = new CQ11X5_Q2XTZSManager().QueryCQ11X5_Q2XTZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_Q2XTZS>, CQ11X5_Q2XTZS, List<CQ11X5_Q2XTZS_Info>, CQ11X5_Q2XTZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_Q2XTZS_Info(); },
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
        public CQ11X5_Q3JBZS_InfoCollection QueryCQ11X5_Q3JBZS(int index)
        {
            CQ11X5_Q3JBZS_InfoCollection Collection = new CQ11X5_Q3JBZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_Q3JBZS_Info>(string.Format("QueryCQ11X5_Q3JBZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_Q3JBZS_Info>();
                var entityList = new CQ11X5_Q3JBZSManager().QueryCQ11X5_Q3JBZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_Q3JBZS>, CQ11X5_Q3JBZS, List<CQ11X5_Q3JBZS_Info>, CQ11X5_Q3JBZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_Q3JBZS_Info(); },
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
        public CQ11X5_Q3XTZS_InfoCollection QueryCQ11X5_Q3XTZS(int index)
        {
            CQ11X5_Q3XTZS_InfoCollection Collection = new CQ11X5_Q3XTZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_Q3XTZS_Info>(string.Format("QueryCQ11X5_Q3XTZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_Q3XTZS_Info>();
                var entityList = new CQ11X5_Q3XTZSManager().QueryCQ11X5_Q3XTZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_Q3XTZS>, CQ11X5_Q3XTZS, List<CQ11X5_Q3XTZS_Info>, CQ11X5_Q3XTZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_Q3XTZS_Info(); },
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
        public CQ11X5_XTZS_InfoCollection QueryCQ11X5_XTZS(int index)
        {
            CQ11X5_XTZS_InfoCollection Collection = new CQ11X5_XTZS_InfoCollection();
            var list = this.QueryGameChart<CQ11X5_XTZS_Info>(string.Format("QueryCQ11X5_XTZS_{0}", index), () =>
            {
                var infoList = new List<CQ11X5_XTZS_Info>();
                var entityList = new CQ11X5_XTZSManager().QueryCQ11X5_XTZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_XTZS>, CQ11X5_XTZS, List<CQ11X5_XTZS_Info>, CQ11X5_XTZS_Info>(entityList, ref infoList,
                    () => { return new CQ11X5_XTZS_Info(); },
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
        public GameWinNumber_InfoCollection QueryCQ11X5_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new CQ11X5_GameWinNumberManager().QueryCQ11X5_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_GameWinNumber>, CQ11X5_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryCQ11X5_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new CQ11X5_GameWinNumberManager().QueryCQ11X5_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<CQ11X5_GameWinNumber>, CQ11X5_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
            var manager = new CQ11X5_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<CQ11X5_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }


    }
}
