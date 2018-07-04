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
    /// <summary>
    /// 福彩3D
    /// </summary>
    public class LotteryDataBusiness_FC3D : LotteryDataBusiness, ILotteryDataBusiness
    {

        public string CurrentGameCode
        {
            get
            {
                return "FC3D";
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
                this.ClearGameChartCache("QueryFC3D_KuaDu_12");
                this.ClearGameChartCache("QueryFC3D_KuaDu_23");
                this.ClearGameChartCache("QueryFC3D_KuaDu_13");
                this.ClearGameChartCache("QueryFC3D_KuaDu_Z");
                this.ClearGameChartCache("QueryFC3D_HZZS");
                this.ClearGameChartCache("QueryFC3D_HZTZ");
                this.ClearGameChartCache("QueryFC3D_HZFB");
                this.ClearGameChartCache("QueryFC3D_Chu33");
                this.ClearGameChartCache("QueryFC3D_Chu32");
                this.ClearGameChartCache("QueryFC3D_Chu31");
                this.ClearGameChartCache("QueryFC3D_ZHHM");
                this.ClearGameChartCache("QueryFC3D_ZHXT");
                this.ClearGameChartCache("QueryFC3D_JOHM");
                this.ClearGameChartCache("QueryFC3D_JOXT");
                this.ClearGameChartCache("QueryFC3D_DXHM");
                this.ClearGameChartCache("QueryFC3D_DXXT");
                this.ClearGameChartCache("QueryFC3D_ZuXuanZouSi");
                this.ClearGameChartCache("QueryFC3D_ZhiXuanZouSi");
                this.ClearNewWinNumberCache("QueryFC3D_GameWinNumber");

                AddFC3D_KuaDu_12(issuseNumber, winNumber);
                AddFC3D_KuaDu_23(issuseNumber, winNumber);
                AddFC3D_KuaDu_13(issuseNumber, winNumber);
                AddFC3D_HZZS(issuseNumber, winNumber);
                AddFC3D_HZTZ(issuseNumber, winNumber);
                AddFC3D_HZFB(issuseNumber, winNumber);
                AddFC3D_Chu33(issuseNumber, winNumber);
                AddFC3D_Chu32(issuseNumber, winNumber);
                AddFC3D_Chu31(issuseNumber, winNumber);
                AddFC3D_ZHHM(issuseNumber, winNumber);
                AddFC3D_ZHXT(issuseNumber, winNumber);
                AddFC3D_JOHM(issuseNumber, winNumber);
                AddFC3D_JOXT(issuseNumber, winNumber);
                AddFC3D_DXHM(issuseNumber, winNumber);
                AddFC3D_DXXT(issuseNumber, winNumber);
                AddFC3D_ZuXuanZouSi(issuseNumber, winNumber);
                AddFC3D_ZhiXuanZouSi(issuseNumber, winNumber);
                AddFC3D_KuaDu_Z(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }

        #region 生成走势数据

        public void Add_GameWinNumber(string issuseNumber, string winNumber)
        {
            new KJGameIssuseBusiness().IssusePrize(this.CurrentGameCode, issuseNumber, winNumber);
            var manager = new FC3D_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddFC3D_GameWinNumber(new FC3D_GameWinNumber
            {
                GameCode = this.CurrentGameCode,
                IssuseNumber = issuseNumber,
                WinNumber = winNumber,
                CreateTime = DateTime.Now,
            });
        }

        /// <summary>
        /// 总跨度
        /// </summary>
        private void AddFC3D_KuaDu_Z(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_KuaDu_ZManager();
            var issuse = manager.QueryFC3D_KuaDu_ZIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var max = Convert.ToInt32(winRed.Max());
            var min = Convert.ToInt32(winRed.Min());
            var kd = max - min;

            string DX_HeZhi = "小";
            string JO_HeZhi = "奇";
            string ZH_HeZhi = "合";
            int C3_Y = kd % 3;
            int C4_Y = kd % 4;
            int C5_Y = kd % 5;
            if (kd > 4)
                DX_HeZhi = "大";
            if (kd % 2 == 0)
                JO_HeZhi = "偶";
            if (kd == 1 || kd == 2 || kd == 3 || kd == 5 || kd == 7)
                ZH_HeZhi = "质";

            var last = manager.QueryLastFC3D_KuaDu_Z();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("KuaDu", kd);
            var entity = this.CreateNewEntity<FC3D_KuaDu_Z>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("KD_"))
                {
                    var value = Convert.ToInt32(p.Name.Replace("KD_", string.Empty));
                    lastValue = kd == value ? 0 : lastValue;
                }
                if (p.Name.EndsWith("HeZhi_D"))
                {
                    lastValue = DX_HeZhi == "大" ? 0 : lastValue;
                }
                if (p.Name.EndsWith("HeZhi_X"))
                {
                    lastValue = DX_HeZhi == "小" ? 0 : lastValue;
                }
                if (p.Name.EndsWith("HeZhi_J"))
                {
                    lastValue = JO_HeZhi == "奇" ? 0 : lastValue;
                }
                if (p.Name.EndsWith("HeZhi_O"))
                {
                    lastValue = JO_HeZhi == "偶" ? 0 : lastValue;
                }
                if (p.Name.EndsWith("HeZhi_Z"))
                {
                    lastValue = ZH_HeZhi == "质" ? 0 : lastValue;
                }
                if (p.Name.EndsWith("HeZhi_H"))
                {
                    lastValue = ZH_HeZhi == "合" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_C4_Y"))
                {
                    var order = p.Name.Replace("O_C4_Y", string.Empty);
                    lastValue = int.Parse(order) == C4_Y ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_C3_Y"))
                {
                    var order = p.Name.Replace("O_C3_Y", string.Empty);
                    lastValue = int.Parse(order) == C3_Y ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_C5_Y"))
                {
                    var order = p.Name.Replace("O_C5_Y", string.Empty);
                    lastValue = int.Parse(order) == C5_Y ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddFC3D_KuaDu_Z(entity);
        }

        /// <summary>
        /// 跨度百十位走势
        /// </summary>
        private void AddFC3D_KuaDu_12(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_KuaDu_12Manager();
            var issuse = manager.QueryFC3D_KuaDu_12IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var kd = Math.Abs(int.Parse(winRed[0]) - int.Parse(winRed[1]));
            var zhilist = new int[] { 1, 2, 3, 5, 7 };

            var last = manager.QueryLastFC3D_KuaDu_12();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<FC3D_KuaDu_12>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("KD_"))
                {
                    var order = p.Name.Replace("KD_", string.Empty);
                    lastValue = kd == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("KDDX_"))
                {
                    var order = p.Name.Replace("KDDX_", string.Empty);
                    if (kd >= 5 && order == "D")
                        lastValue = 0;
                    if (kd < 5 && order == "X")
                        lastValue = 0;
                }
                if (p.Name.StartsWith("KDJ0_"))
                {
                    var order = p.Name.Replace("KDJ0_", string.Empty);
                    if (kd % 2 == 1 && order == "J")
                        lastValue = 0;
                    if (kd % 2 == 0 && order == "O")
                        lastValue = 0;
                }
                if (p.Name.StartsWith("KDZH_"))
                {
                    var order = p.Name.Replace("KDZH_", string.Empty);
                    if (zhilist.Contains(kd) && order == "Z")
                        lastValue = 0;
                    if (!zhilist.Contains(kd) && order == "H")
                        lastValue = 0;
                }
                if (p.Name.StartsWith("KDChu3_"))
                {
                    var order = p.Name.Replace("KDChu3_", string.Empty);
                    if (kd % 3 == int.Parse(order))
                        lastValue = 0;
                }
                if (p.Name.StartsWith("KDChu4_"))
                {
                    var order = p.Name.Replace("KDChu4_", string.Empty);
                    if (kd % 4 == int.Parse(order))
                        lastValue = 0;
                }
                if (p.Name.StartsWith("KDChu5_"))
                {
                    var order = p.Name.Replace("KDChu5_", string.Empty);
                    if (kd % 5 == int.Parse(order))
                        lastValue = 0;
                }
                return lastValue;
            });

            manager.AddFC3D_KuaDu_12(entity);
        }

        /// <summary>
        /// 跨度十个位走势
        /// </summary>
        private void AddFC3D_KuaDu_23(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_KuaDu_23Manager();
            var issuse = manager.QueryFC3D_KuaDu_23IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var kd = Math.Abs(int.Parse(winRed[1]) - int.Parse(winRed[2]));
            var zhilist = new int[] { 1, 2, 3, 5, 7 };

            var last = manager.QueryLastFC3D_KuaDu_23();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<FC3D_KuaDu_23>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("KD_"))
                {
                    var order = p.Name.Replace("KD_", string.Empty);
                    lastValue = kd == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("KDDX_"))
                {
                    var order = p.Name.Replace("KDDX_", string.Empty);
                    if (kd >= 5 && order == "D")
                        lastValue = 0;
                    if (kd < 5 && order == "X")
                        lastValue = 0;
                }
                if (p.Name.StartsWith("KDJ0_"))
                {
                    var order = p.Name.Replace("KDJ0_", string.Empty);
                    if (kd % 2 == 1 && order == "J")
                        lastValue = 0;
                    if (kd % 2 == 0 && order == "O")
                        lastValue = 0;
                }
                if (p.Name.StartsWith("KDZH_"))
                {
                    var order = p.Name.Replace("KDZH_", string.Empty);
                    if (zhilist.Contains(kd) && order == "Z")
                        lastValue = 0;
                    if (!zhilist.Contains(kd) && order == "H")
                        lastValue = 0;
                }
                if (p.Name.StartsWith("KDChu3_"))
                {
                    var order = p.Name.Replace("KDChu3_", string.Empty);
                    if (kd % 3 == int.Parse(order))
                        lastValue = 0;
                }
                if (p.Name.StartsWith("KDChu4_"))
                {
                    var order = p.Name.Replace("KDChu4_", string.Empty);
                    if (kd % 4 == int.Parse(order))
                        lastValue = 0;
                }
                if (p.Name.StartsWith("KDChu5_"))
                {
                    var order = p.Name.Replace("KDChu5_", string.Empty);
                    if (kd % 5 == int.Parse(order))
                        lastValue = 0;
                }
                return lastValue;
            });

            manager.AddFC3D_KuaDu_23(entity);
        }

        /// <summary>
        /// 跨度百个位走势
        /// </summary>
        private void AddFC3D_KuaDu_13(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_KuaDu_13Manager();
            var issuse = manager.QueryFC3D_KuaDu_13IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var kd = Math.Abs(int.Parse(winRed[0]) - int.Parse(winRed[2]));
            var zhilist = new int[] { 1, 2, 3, 5, 7 };

            var last = manager.QueryLastFC3D_KuaDu_13();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<FC3D_KuaDu_13>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("KD_"))
                {
                    var order = p.Name.Replace("KD_", string.Empty);
                    lastValue = kd == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("KDDX_"))
                {
                    var order = p.Name.Replace("KDDX_", string.Empty);
                    if (kd >= 5 && order == "D")
                        lastValue = 0;
                    if (kd < 5 && order == "X")
                        lastValue = 0;
                }
                if (p.Name.StartsWith("KDJ0_"))
                {
                    var order = p.Name.Replace("KDJ0_", string.Empty);
                    if (kd % 2 == 1 && order == "J")
                        lastValue = 0;
                    if (kd % 2 == 0 && order == "O")
                        lastValue = 0;
                }
                if (p.Name.StartsWith("KDZH_"))
                {
                    var order = p.Name.Replace("KDZH_", string.Empty);
                    if (zhilist.Contains(kd) && order == "Z")
                        lastValue = 0;
                    if (!zhilist.Contains(kd) && order == "H")
                        lastValue = 0;
                }
                if (p.Name.StartsWith("KDChu3_"))
                {
                    var order = p.Name.Replace("KDChu3_", string.Empty);
                    if (kd % 3 == int.Parse(order))
                        lastValue = 0;
                }
                if (p.Name.StartsWith("KDChu4_"))
                {
                    var order = p.Name.Replace("KDChu4_", string.Empty);
                    if (kd % 4 == int.Parse(order))
                        lastValue = 0;
                }
                if (p.Name.StartsWith("KDChu5_"))
                {
                    var order = p.Name.Replace("KDChu5_", string.Empty);
                    if (kd % 5 == int.Parse(order))
                        lastValue = 0;
                }
                return lastValue;
            });

            manager.AddFC3D_KuaDu_13(entity);
        }

        /// <summary>
        /// 和值走势
        /// </summary>
        public void AddFC3D_HZZS(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_HZZSManager();
            var issuse = manager.QueryFC3D_HZZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]);
            var avg = (hz / 3).ToString("N0");
            var hw = hz % 10;
            var last = manager.QueryLastFC3D_HZZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("HeZhi", hz);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);

            var entity = this.CreateNewEntity<FC3D_HZZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("AVG_"))
                {
                    var order = p.Name.Replace("AVG_", string.Empty);
                    lastValue = int.Parse(order) == Convert.ToInt32(avg) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("HW_"))
                {
                    var order = p.Name.Replace("HW_", string.Empty);
                    lastValue = int.Parse(order) == hw ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddFC3D_HZZS(entity);
        }

        /// <summary>
        /// 和值特征
        /// </summary>
        public void AddFC3D_HZTZ(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_HZTZManager();
            var issuse = manager.QueryFC3D_HZTZIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]);
            var last = manager.QueryLastFC3D_HZTZ();
            string DX_HeZhi = "小";
            string JO_HeZhi = "奇";
            string ZH_HeZhi = "合";
            string QU_HeZhi = "升";
            int C3_Y = hz % 3;
            int C4_Y = hz % 4;
            int C5_Y = hz % 5;
            if (hz > 13)
                DX_HeZhi = "大";
            if (hz % 2 == 0)
                JO_HeZhi = "偶";
            if (hz == 1 || hz == 2 || hz == 3 || hz == 5 || hz == 7 || hz == 11 || hz == 13 || hz == 17 || hz == 19 || hz == 23)
                ZH_HeZhi = "质";
            if (hz > (last == null ? 0 : last.HeZhi))
            {
                QU_HeZhi = "升";
            }
            else if (hz < (last == null ? 0 : last.HeZhi))
            {
                QU_HeZhi = "降";
            }
            else
            {
                QU_HeZhi = "平";
            }
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("HeZhi", hz);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);

            var entity = this.CreateNewEntity<FC3D_HZTZ>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.EndsWith("HeZhi_D"))
                {
                    lastValue = DX_HeZhi == "大" ? 0 : lastValue;
                }
                if (p.Name.EndsWith("HeZhi_X"))
                {
                    lastValue = DX_HeZhi == "小" ? 0 : lastValue;
                }
                if (p.Name.EndsWith("HeZhi_J"))
                {
                    lastValue = JO_HeZhi == "奇" ? 0 : lastValue;
                }
                if (p.Name.EndsWith("HeZhi_O"))
                {
                    lastValue = JO_HeZhi == "偶" ? 0 : lastValue;
                }
                if (p.Name.EndsWith("HeZhi_Z"))
                {
                    lastValue = ZH_HeZhi == "质" ? 0 : lastValue;
                }
                if (p.Name.EndsWith("HeZhi_H"))
                {
                    lastValue = ZH_HeZhi == "合" ? 0 : lastValue;
                }
                if (p.Name.EndsWith("HeZhi_Sheng"))
                {
                    lastValue = QU_HeZhi == "升" ? 0 : lastValue;
                }
                if (p.Name.EndsWith("HeZhi_Ping"))
                {
                    lastValue = QU_HeZhi == "平" ? 0 : lastValue;
                }
                if (p.Name.EndsWith("HeZhi_Jiang"))
                {
                    lastValue = QU_HeZhi == "降" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_C4_Y"))
                {
                    var order = p.Name.Replace("O_C4_Y", string.Empty);
                    lastValue = int.Parse(order) == C4_Y ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_C3_Y"))
                {
                    var order = p.Name.Replace("O_C3_Y", string.Empty);
                    lastValue = int.Parse(order) == C3_Y ? 0 : lastValue;
                }
                if (p.Name.StartsWith("O_C5_Y"))
                {
                    var order = p.Name.Replace("O_C5_Y", string.Empty);
                    lastValue = int.Parse(order) == C5_Y ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddFC3D_HZTZ(entity);
        }

        /// <summary>
        /// 和值分布
        /// </summary>
        public void AddFC3D_HZFB(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_HZFBManager();
            var issuse = manager.QueryFC3D_HZFBIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]);
            var hw = hz % 10;

            var last = manager.QueryLastFC3D_HZFB();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("HeZhi", hz);
            dic.Add("HeWei", hw);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);

            var entity = this.CreateNewEntity<FC3D_HZFB>(dic, (p) =>
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

            manager.AddFC3D_HZFB(entity);
        }

        /// <summary>
        /// 除3_3走势
        /// </summary>
        private void AddFC3D_Chu33(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_Chu33Manager();
            var issuse = manager.QueryFC3D_Chu33IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var redball = new int[]{
                Convert.ToInt32(winRed[0])%3,
                Convert.ToInt32(winRed[1])%3,
                Convert.ToInt32(winRed[2])%3
            };

            var last = manager.QueryLastFC3D_Chu33();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("y3xt", string.Format("{0}{1}{2}", redball[0], redball[1], redball[2]));

            var entity = this.CreateNewEntity<FC3D_Chu33>(dic, (p) =>
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

                if (p.Name.StartsWith("Y0_Number"))
                {
                    var value = int.Parse(p.Name.Replace("Y0_Number", string.Empty));
                    lastValue = redball[0] == value ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Y1_Number"))
                {
                    var value = int.Parse(p.Name.Replace("Y1_Number", string.Empty));
                    lastValue = redball[1] == value ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Y2_Number"))
                {
                    var value = int.Parse(p.Name.Replace("Y2_Number", string.Empty));
                    lastValue = redball[2] == value ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddFC3D_Chu33(entity);
        }

        /// <summary>
        /// 除3_2走势
        /// </summary>
        private void AddFC3D_Chu32(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_Chu32Manager();
            var issuse = manager.QueryFC3D_Chu32IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string P012_Proportion = string.Empty;
            var redball = new int[]{
                Convert.ToInt32(winRed[0])%3,
                Convert.ToInt32(winRed[1])%3,
                Convert.ToInt32(winRed[2])%3
            };

            P012_Proportion = string.Format("{0}:{1}:{2}", redball[0], redball[1], redball[2]);

            var last = manager.QueryLastFC3D_Chu32();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);

            var entity = this.CreateNewEntity<FC3D_Chu32>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("C_AAA_"))
                {
                    var value = p.Name.Replace("C_AAA_", string.Empty);
                    lastValue = (P012_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("C_AAB_"))
                {
                    var value = p.Name.Replace("C_AAB_", string.Empty);
                    lastValue = (P012_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("C_ABC_"))
                {
                    var value = p.Name.Replace("C_ABC_", string.Empty);
                    lastValue = (P012_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddFC3D_Chu32(entity);
        }

        /// <summary>
        /// 除3_1走势
        /// </summary>
        private void AddFC3D_Chu31(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_Chu31Manager();
            var issuse = manager.QueryFC3D_Chu31IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

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
            string P012_Proportion = string.Empty;
            var redball = new int[]{
                Convert.ToInt32(winRed[0])%3,
                Convert.ToInt32(winRed[1])%3,
                Convert.ToInt32(winRed[2])%3
            };

            var Y0_Number = redball.Where(p => p == 0).Count();
            var Y1_Number = redball.Where(p => p == 1).Count();
            var Y2_Number = redball.Where(p => p == 2).Count();

            P012_Proportion = string.Format("{0}:{1}:{2}", Y0_Number, Y1_Number, Y2_Number);

            var last = manager.QueryLastFC3D_Chu31();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("P012_Proportion", P012_Proportion);

            var entity = this.CreateNewEntity<FC3D_Chu31>(dic, (p) =>
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
                if (p.Name.StartsWith("O_P012_"))
                {
                    var value = p.Name.Replace("O_P012_Proportion", string.Empty);
                    lastValue = (P012_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }

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

            manager.AddFC3D_Chu31(entity);
        }

        /// <summary>
        /// 质合号码
        /// </summary>
        private void AddFC3D_ZHHM(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_ZHHMManager();
            var issuse = manager.QueryFC3D_ZHHMIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string ZH_Proportion = string.Empty;
            int z = 0;
            int h = 0;
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
            }
            ZH_Proportion = string.Format("{0}:{1}", z, h);
            var last = manager.QueryLastFC3D_ZHHM();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("ZH_Proportion", ZH_Proportion);

            var entity = this.CreateNewEntity<FC3D_ZHHM>(dic, (p) =>
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
                if (p.Name.StartsWith("O_ZH_"))
                {
                    var value = p.Name.Replace("O_ZH_Proportion", string.Empty);
                    lastValue = (ZH_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddFC3D_ZHHM(entity);
        }

        /// <summary>
        /// 质合形态走势
        /// </summary>
        private void AddFC3D_ZHXT(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_ZHXTManager();
            var issuse = manager.QueryFC3D_ZHXTIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
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

            var last = manager.QueryLastFC3D_ZHXT();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("ZH_Proportion", ZH_Proportion);
            dic.Add("Zhxt", Zhxt);

            var entity = this.CreateNewEntity<FC3D_ZHXT>(dic, (p) =>
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
                return lastValue;
            });

            manager.AddFC3D_ZHXT(entity);
        }

        /// <summary>
        /// 奇偶号码
        /// </summary>
        private void AddFC3D_JOHM(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_JOHMManager();
            var issuse = manager.QueryFC3D_JOHMIssuseNumber(issuseNumber);
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
            var last = manager.QueryLastFC3D_JOHM();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("JO_Proportion", JO_Proportion);

            var entity = this.CreateNewEntity<FC3D_JOHM>(dic, (p) =>
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
                if (p.Name.StartsWith("O_JO_"))
                {
                    var value = p.Name.Replace("O_JO_Proportion", string.Empty);
                    lastValue = (JO_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddFC3D_JOHM(entity);
        }

        /// <summary>
        /// 奇偶形态走势
        /// </summary>
        private void AddFC3D_JOXT(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_JOXTManager();
            var issuse = manager.QueryFC3D_JOXTIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
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

            var last = manager.QueryLastFC3D_JOXT();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("JO_Proportion", JO_Proportion);
            dic.Add("Joxt", Joxt);

            var entity = this.CreateNewEntity<FC3D_JOXT>(dic, (p) =>
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
                return lastValue;
            });

            manager.AddFC3D_JOXT(entity);
        }

        /// <summary>
        /// 大小号码
        /// </summary>
        private void AddFC3D_DXHM(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_DXHMManager();
            var issuse = manager.QueryFC3D_DXHMIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string DX_Proportion = string.Empty;
            int d = 0;
            int x = 0;
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
            }
            DX_Proportion = string.Format("{0}:{1}", d, x);
            var last = manager.QueryLastFC3D_DXHM();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("DX_Proportion", DX_Proportion);

            var entity = this.CreateNewEntity<FC3D_DXHM>(dic, (p) =>
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
                if (p.Name.StartsWith("O_DX_"))
                {
                    var value = p.Name.Replace("O_DX_Proportion", string.Empty);
                    lastValue = (DX_Proportion.Replace(":", string.Empty) == value) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddFC3D_DXHM(entity);
        }

        /// <summary>
        /// 大小形态走势
        /// </summary>
        private void AddFC3D_DXXT(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_DXXTManager();
            var issuse = manager.QueryFC3D_DXXTIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
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

            var last = manager.QueryLastFC3D_DXXT();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("DX_Proportion", DX_Proportion);
            dic.Add("Dxxt", Dxxt);

            var entity = this.CreateNewEntity<FC3D_DXXT>(dic, (p) =>
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
                return lastValue;
            });

            manager.AddFC3D_DXXT(entity);
        }

        /// <summary>
        /// 组选走势
        /// </summary>
        private void AddFC3D_ZuXuanZouSi(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_ZuXuanZouSiManager();
            var issuse = manager.QueryFC3D_ZuXuanZouSiIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
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
                if (Convert.ToInt32(item) == 2 || Convert.ToInt32(item) == 3 || Convert.ToInt32(item) == 5 || Convert.ToInt32(item) == 7 || Convert.ToInt32(item) == 11 || Convert.ToInt32(item) == 13 || Convert.ToInt32(item) == 17 || Convert.ToInt32(item) == 19 || Convert.ToInt32(item) == 23 || Convert.ToInt32(item) == 29 || Convert.ToInt32(item) == 31)
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

            var last = manager.QueryLastFC3D_ZuXuanZouSi();
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

            var entity = this.CreateNewEntity<FC3D_ZuXuanZouSi>(dic, (p) =>
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

            manager.AddFC3D_ZuXuanZouSi(entity);
        }

        /// <summary>
        /// 直选走势
        /// </summary>
        private void AddFC3D_ZhiXuanZouSi(string issuseNumber, string winNumber)
        {
            var manager = new FC3D_ZhiXuanZouSiManager();
            var issuse = manager.QueryFC3D_ZhiXuanZouSiIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
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

            var last = manager.QueryLastZhiXuanZouSi();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("Type", type);

            var entity = this.CreateNewEntity<FC3D_ZhiXuanZouSi>(dic, (p) =>
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

            manager.AddZhiXuanZouSi(entity);
        }

        #endregion

        #region 查询走势

        /// <summary>
        /// 总跨度
        /// </summary>
        public FC3D_KuaDu_Z_InfoCollection QueryFC3D_KuaDu_Z(int index)
        {
            FC3D_KuaDu_Z_InfoCollection Collection = new FC3D_KuaDu_Z_InfoCollection();
            var list = this.QueryGameChart<FC3D_KuaDu_Z_Info>(string.Format("QueryFC3D_KuaDu_Z_{0}", index), () =>
            {
                var infoList = new List<FC3D_KuaDu_Z_Info>();
                var entityList = new FC3D_KuaDu_ZManager().QueryFC3D_KuaDu_Z(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_KuaDu_Z>, FC3D_KuaDu_Z, List<FC3D_KuaDu_Z_Info>, FC3D_KuaDu_Z_Info>(entityList, ref infoList,
                    () => { return new FC3D_KuaDu_Z_Info(); },
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
        /// 跨度百十位走势
        /// </summary>
        public FC3D_KuaDu_12_InfoCollection QueryFC3D_KuaDu_12(int index)
        {
            FC3D_KuaDu_12_InfoCollection Collection = new FC3D_KuaDu_12_InfoCollection();
            var list = this.QueryGameChart<FC3D_KuaDu_12_Info>(string.Format("QueryFC3D_KuaDu_12_{0}", index), () =>
            {
                var infoList = new List<FC3D_KuaDu_12_Info>();
                var entityList = new FC3D_KuaDu_12Manager().QueryFC3D_KuaDu_12(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_KuaDu_12>, FC3D_KuaDu_12, List<FC3D_KuaDu_12_Info>, FC3D_KuaDu_12_Info>(entityList, ref infoList,
                    () => { return new FC3D_KuaDu_12_Info(); },
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
        /// 跨度十个位走势
        /// </summary>
        public FC3D_KuaDu_23_InfoCollection QueryFC3D_KuaDu_23(int index)
        {
            FC3D_KuaDu_23_InfoCollection Collection = new FC3D_KuaDu_23_InfoCollection();
            var list = this.QueryGameChart<FC3D_KuaDu_23_Info>(string.Format("QueryFC3D_KuaDu_23_{0}", index), () =>
            {
                var infoList = new List<FC3D_KuaDu_23_Info>();
                var entityList = new FC3D_KuaDu_23Manager().QueryFC3D_KuaDu_23(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_KuaDu_23>, FC3D_KuaDu_23, List<FC3D_KuaDu_23_Info>, FC3D_KuaDu_23_Info>(entityList, ref infoList,
                    () => { return new FC3D_KuaDu_23_Info(); },
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
        /// 跨度百个位走势
        /// </summary>
        public FC3D_KuaDu_13_InfoCollection QueryFC3D_KuaDu_13(int index)
        {
            FC3D_KuaDu_13_InfoCollection Collection = new FC3D_KuaDu_13_InfoCollection();
            var list = this.QueryGameChart<FC3D_KuaDu_13_Info>(string.Format("QueryFC3D_KuaDu_13_{0}", index), () =>
            {
                var infoList = new List<FC3D_KuaDu_13_Info>();
                var entityList = new FC3D_KuaDu_13Manager().QueryFC3D_KuaDu_13(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_KuaDu_13>, FC3D_KuaDu_13, List<FC3D_KuaDu_13_Info>, FC3D_KuaDu_13_Info>(entityList, ref infoList,
                    () => { return new FC3D_KuaDu_13_Info(); },
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
        public FC3D_HZZS_InfoCollection QueryFC3D_HZZS(int index)
        {
            FC3D_HZZS_InfoCollection Collection = new FC3D_HZZS_InfoCollection();
            var list = this.QueryGameChart<FC3D_HZZS_Info>(string.Format("QueryFC3D_HZZS_{0}", index), () =>
            {
                var infoList = new List<FC3D_HZZS_Info>();
                var entityList = new FC3D_HZZSManager().QueryFC3D_HZZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_HZZS>, FC3D_HZZS, List<FC3D_HZZS_Info>, FC3D_HZZS_Info>(entityList, ref infoList,
                    () => { return new FC3D_HZZS_Info(); },
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
        /// 和值特征
        /// </summary>
        public FC3D_HZTZ_InfoCollection QueryFC3D_HZTZ(int index)
        {
            FC3D_HZTZ_InfoCollection Collection = new FC3D_HZTZ_InfoCollection();
            var list = this.QueryGameChart<FC3D_HZTZ_Info>(string.Format("QueryFC3D_HZTZ_{0}", index), () =>
            {
                var infoList = new List<FC3D_HZTZ_Info>();
                var entityList = new FC3D_HZTZManager().QueryFC3D_HZTZ(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_HZTZ>, FC3D_HZTZ, List<FC3D_HZTZ_Info>, FC3D_HZTZ_Info>(entityList, ref infoList,
                    () => { return new FC3D_HZTZ_Info(); },
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
        /// 和值分布
        /// </summary>
        public FC3D_HZFB_InfoCollection QueryFC3D_HZFB(int index)
        {
            FC3D_HZFB_InfoCollection Collection = new FC3D_HZFB_InfoCollection();
            var list = this.QueryGameChart<FC3D_HZFB_Info>(string.Format("QueryFC3D_HZFB_{0}", index), () =>
            {
                var infoList = new List<FC3D_HZFB_Info>();
                var entityList = new FC3D_HZFBManager().QueryFC3D_HZFB(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_HZFB>, FC3D_HZFB, List<FC3D_HZFB_Info>, FC3D_HZFB_Info>(entityList, ref infoList,
                    () => { return new FC3D_HZFB_Info(); },
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
        /// 除3_3走势
        /// </summary>
        public FC3D_Chu33_InfoCollection QueryFC3D_Chu33(int index)
        {
            FC3D_Chu33_InfoCollection Collection = new FC3D_Chu33_InfoCollection();
            var list = this.QueryGameChart<FC3D_Chu33_Info>(string.Format("QueryFC3D_Chu33_{0}", index), () =>
            {
                var infoList = new List<FC3D_Chu33_Info>();
                var entityList = new FC3D_Chu33Manager().QueryFC3D_Chu33(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_Chu33>, FC3D_Chu33, List<FC3D_Chu33_Info>, FC3D_Chu33_Info>(entityList, ref infoList,
                    () => { return new FC3D_Chu33_Info(); },
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
        /// 除3_2走势
        /// </summary>
        public FC3D_Chu32_InfoCollection QueryFC3D_Chu32(int index)
        {
            FC3D_Chu32_InfoCollection Collection = new FC3D_Chu32_InfoCollection();
            var list = this.QueryGameChart<FC3D_Chu32_Info>(string.Format("QueryFC3D_Chu32_{0}", index), () =>
            {
                var infoList = new List<FC3D_Chu32_Info>();
                var entityList = new FC3D_Chu32Manager().QueryFC3D_Chu32(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_Chu32>, FC3D_Chu32, List<FC3D_Chu32_Info>, FC3D_Chu32_Info>(entityList, ref infoList,
                    () => { return new FC3D_Chu32_Info(); },
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
        /// 除3_1走势
        /// </summary>
        public FC3D_Chu31_InfoCollection QueryFC3D_Chu31(int index)
        {
            FC3D_Chu31_InfoCollection Collection = new FC3D_Chu31_InfoCollection();
            var list = this.QueryGameChart<FC3D_Chu31_Info>(string.Format("QueryFC3D_Chu31_{0}", index), () =>
            {
                var infoList = new List<FC3D_Chu31_Info>();
                var entityList = new FC3D_Chu31Manager().QueryFC3D_Chu31(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_Chu31>, FC3D_Chu31, List<FC3D_Chu31_Info>, FC3D_Chu31_Info>(entityList, ref infoList,
                    () => { return new FC3D_Chu31_Info(); },
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
        /// 质合号码
        /// </summary>
        public FC3D_ZHHM_InfoCollection QueryFC3D_ZHHM(int index)
        {
            FC3D_ZHHM_InfoCollection Collection = new FC3D_ZHHM_InfoCollection();
            var list = this.QueryGameChart<FC3D_ZHHM_Info>(string.Format("QueryFC3D_ZHHM_{0}", index), () =>
            {
                var infoList = new List<FC3D_ZHHM_Info>();
                var entityList = new FC3D_ZHHMManager().QueryFC3D_ZHHM(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_ZHHM>, FC3D_ZHHM, List<FC3D_ZHHM_Info>, FC3D_ZHHM_Info>(entityList, ref infoList,
                    () => { return new FC3D_ZHHM_Info(); },
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
        /// 质合形态走势
        /// </summary>
        public FC3D_ZHXT_InfoCollection QueryFC3D_ZHXT(int index)
        {
            FC3D_ZHXT_InfoCollection Collection = new FC3D_ZHXT_InfoCollection();
            var list = this.QueryGameChart<FC3D_ZHXT_Info>(string.Format("QueryFC3D_ZHXT_{0}", index), () =>
            {
                var infoList = new List<FC3D_ZHXT_Info>();
                var entityList = new FC3D_ZHXTManager().QueryFC3D_ZHXT(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_ZHXT>, FC3D_ZHXT, List<FC3D_ZHXT_Info>, FC3D_ZHXT_Info>(entityList, ref infoList,
                    () => { return new FC3D_ZHXT_Info(); },
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
        /// 奇偶号码
        /// </summary>
        public FC3D_JOHM_InfoCollection QueryFC3D_JOHM(int index)
        {
            FC3D_JOHM_InfoCollection Collection = new FC3D_JOHM_InfoCollection();
            var list = this.QueryGameChart<FC3D_JOHM_Info>(string.Format("QueryFC3D_JOHM_{0}", index), () =>
            {
                var infoList = new List<FC3D_JOHM_Info>();
                var entityList = new FC3D_JOHMManager().QueryFC3D_JOHM(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_JOHM>, FC3D_JOHM, List<FC3D_JOHM_Info>, FC3D_JOHM_Info>(entityList, ref infoList,
                    () => { return new FC3D_JOHM_Info(); },
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
        /// 奇偶形态走势
        /// </summary>
        public FC3D_JOXT_InfoCollection QueryFC3D_JOXT(int index)
        {
            FC3D_JOXT_InfoCollection Collection = new FC3D_JOXT_InfoCollection();
            var list = this.QueryGameChart<FC3D_JOXT_Info>(string.Format("QueryFC3D_JOXT_{0}", index), () =>
            {
                var infoList = new List<FC3D_JOXT_Info>();
                var entityList = new FC3D_JOXTManager().QueryFC3D_JOXT(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_JOXT>, FC3D_JOXT, List<FC3D_JOXT_Info>, FC3D_JOXT_Info>(entityList, ref infoList,
                    () => { return new FC3D_JOXT_Info(); },
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
        /// 大小号码
        /// </summary>
        public FC3D_DXHM_InfoCollection QueryFC3D_DXHM(int index)
        {
            FC3D_DXHM_InfoCollection Collection = new FC3D_DXHM_InfoCollection();
            var list = this.QueryGameChart<FC3D_DXHM_Info>(string.Format("QueryFC3D_DXHM_{0}", index), () =>
            {
                var infoList = new List<FC3D_DXHM_Info>();
                var entityList = new FC3D_DXHMManager().QueryFC3D_DXHM(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_DXHM>, FC3D_DXHM, List<FC3D_DXHM_Info>, FC3D_DXHM_Info>(entityList, ref infoList,
                    () => { return new FC3D_DXHM_Info(); },
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
        /// 大小形态走势
        /// </summary>
        public FC3D_DXXT_InfoCollection QueryFC3D_DXXT(int index)
        {
            FC3D_DXXT_InfoCollection Collection = new FC3D_DXXT_InfoCollection();
            var list = this.QueryGameChart<FC3D_DXXT_Info>(string.Format("QueryFC3D_DXXT_{0}", index), () =>
            {
                var infoList = new List<FC3D_DXXT_Info>();
                var entityList = new FC3D_DXXTManager().QueryFC3D_DXXT(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_DXXT>, FC3D_DXXT, List<FC3D_DXXT_Info>, FC3D_DXXT_Info>(entityList, ref infoList,
                    () => { return new FC3D_DXXT_Info(); },
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
        /// 组选走势
        /// </summary>
        public FC3D_ZuXuanZouSi_InfoCollection QueryFC3D_ZuXuanZouSi(int index)
        {
            FC3D_ZuXuanZouSi_InfoCollection Collection = new FC3D_ZuXuanZouSi_InfoCollection();
            var list = this.QueryGameChart<FC3D_ZuXuanZouSi_Info>(string.Format("QueryFC3D_ZuXuanZouSi_{0}", index), () =>
            {
                var infoList = new List<FC3D_ZuXuanZouSi_Info>();
                var entityList = new FC3D_ZuXuanZouSiManager().QueryFC3D_ZuXuanZouSi(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_ZuXuanZouSi>, FC3D_ZuXuanZouSi, List<FC3D_ZuXuanZouSi_Info>, FC3D_ZuXuanZouSi_Info>(entityList, ref infoList,
                    () => { return new FC3D_ZuXuanZouSi_Info(); },
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
        /// 直选走势
        /// </summary>
        public FC3D_ZhiXuanZouSi_InfoCollection QueryFC3D_ZhiXuanZouSi(int index)
        {
            FC3D_ZhiXuanZouSi_InfoCollection Collection = new FC3D_ZhiXuanZouSi_InfoCollection();
            var list = this.QueryGameChart<FC3D_ZhiXuanZouSi_Info>(string.Format("QueryFC3D_ZhiXuanZouSi_{0}", index), () =>
            {
                var infoList = new List<FC3D_ZhiXuanZouSi_Info>();
                var entityList = new FC3D_ZhiXuanZouSiManager().QueryFC3D_ZhiXuanZouSi(index);

               ObjectConvert.ConvertEntityListToInfoList<List<FC3D_ZhiXuanZouSi>, FC3D_ZhiXuanZouSi, List<FC3D_ZhiXuanZouSi_Info>, FC3D_ZhiXuanZouSi_Info>(entityList, ref infoList,
                    () => { return new FC3D_ZhiXuanZouSi_Info(); },
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
        public GameWinNumber_InfoCollection QueryFC3D_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new FC3D_GameWinNumberManager().QueryFC3D_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<FC3D_GameWinNumber>, FC3D_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryFC3D_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new FC3D_GameWinNumberManager().QueryFC3D_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<FC3D_GameWinNumber>, FC3D_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
        public GameWinNumber_InfoCollection QueryFC3D_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new FC3D_GameWinNumberManager().QueryFC3D_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<FC3D_GameWinNumber>, FC3D_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryFC3D_GameWinNumber_{0}_{1}_{2}_{3}", pageIndex, pageSize, startTime.ToString("yyyyMMdd"), endTime.ToString("yyyyMMdd"));
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new FC3D_GameWinNumberManager().QueryFC3D_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<FC3D_GameWinNumber>, FC3D_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
            var manager = new FC3D_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<FC3D_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }
    }
}
