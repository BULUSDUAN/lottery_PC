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
    public class LotteryDataBusiness_PL5 : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get
            {
                return "PL5";
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

                this.ClearGameChartCache("QueryPL5_JBZS");
                this.ClearGameChartCache("QueryPL5_DX");
                this.ClearGameChartCache("QueryPL5_JO");
                this.ClearGameChartCache("QueryPL5_ZH");
                this.ClearGameChartCache("QueryPL5_Chu3");
                this.ClearGameChartCache("QueryPL5_HZ");
                this.ClearNewWinNumberCache("QueryPL5_GameWinNumber");

                Import_JBZS(issuseNumber, winNumber);
                Import_DX(issuseNumber, winNumber);
                Import_JO(issuseNumber, winNumber);
                Import_ZH(issuseNumber, winNumber);
                Import_Chu3(issuseNumber, winNumber);
                Import_HZ(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }

        #region 前台查询数据

        /// <summary>
        ///查询基本走势列表按时间倒叙 
        /// </summary>
        public PL5_JBZS_InfoCollection QueryPL5_JBZS(int length)
        {
            PL5_JBZS_InfoCollection Collection = new PL5_JBZS_InfoCollection();
            var list = this.QueryGameChart<PL5_JBZS_Info>(string.Format("QueryPL5_JBZS_{0}", length), () =>
            {
                var infoList = new List<PL5_JBZS_Info>();
                var entityList = new PL5_Manager().QueryPL5_JBZS(length);

               ObjectConvert.ConvertEntityListToInfoList<List<PL5_JBZS>, PL5_JBZS, List<PL5_JBZS_Info>, PL5_JBZS_Info>(entityList, ref infoList,
                    () => { return new PL5_JBZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询大小走势列表按时间倒叙 
        /// </summary>
        public PL5_DX_InfoCollection QueryPL5_DX(int length)
        {
            PL5_DX_InfoCollection Collection = new PL5_DX_InfoCollection();
            var list = this.QueryGameChart<PL5_DX_Info>(string.Format("QueryPL5_DX_{0}", length), () =>
            {
                var infoList = new List<PL5_DX_Info>();
                var entityList = new PL5_Manager().QueryPL5_DX(length);

               ObjectConvert.ConvertEntityListToInfoList<List<PL5_DX>, PL5_DX, List<PL5_DX_Info>, PL5_DX_Info>(entityList, ref infoList,
                    () => { return new PL5_DX_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询奇偶走势列表按时间倒叙 
        /// </summary>
        public PL5_JO_InfoCollection QueryPL5_JO(int length)
        {
            PL5_JO_InfoCollection Collection = new PL5_JO_InfoCollection();
            var list = this.QueryGameChart<PL5_JO_Info>(string.Format("QueryPL5_JO_{0}", length), () =>
            {
                var infoList = new List<PL5_JO_Info>();
                var entityList = new PL5_Manager().QueryPL5_JO(length);

               ObjectConvert.ConvertEntityListToInfoList<List<PL5_JO>, PL5_JO, List<PL5_JO_Info>, PL5_JO_Info>(entityList, ref infoList,
                    () => { return new PL5_JO_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询质和走势列表按时间倒叙 
        /// </summary>
        public PL5_ZH_InfoCollection QueryPL5_ZH(int length)
        {
            PL5_ZH_InfoCollection Collection = new PL5_ZH_InfoCollection();
            var list = this.QueryGameChart<PL5_ZH_Info>(string.Format("QueryPL5_ZH_{0}", length), () =>
            {
                var infoList = new List<PL5_ZH_Info>();
                var entityList = new PL5_Manager().QueryPL5_ZH(length);

               ObjectConvert.ConvertEntityListToInfoList<List<PL5_ZH>, PL5_ZH, List<PL5_ZH_Info>, PL5_ZH_Info>(entityList, ref infoList,
                    () => { return new PL5_ZH_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询除3走势列表按时间倒叙 
        /// </summary>
        public PL5_Chu3_InfoCollection QueryPL5_Chu3(int length)
        {
            PL5_Chu3_InfoCollection Collection = new PL5_Chu3_InfoCollection();
            var list = this.QueryGameChart<PL5_Chu3_Info>(string.Format("QueryPL5_Chu3_{0}", length), () =>
            {
                var infoList = new List<PL5_Chu3_Info>();
                var entityList = new PL5_Manager().QueryPL5_Chu3(length);

               ObjectConvert.ConvertEntityListToInfoList<List<PL5_Chu3>, PL5_Chu3, List<PL5_Chu3_Info>, PL5_Chu3_Info>(entityList, ref infoList,
                    () => { return new PL5_Chu3_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询和值走势列表按时间倒叙 
        /// </summary>
        public PL5_HZ_InfoCollection QueryPL5_HZ(int length)
        {
            PL5_HZ_InfoCollection Collection = new PL5_HZ_InfoCollection();
            var list = this.QueryGameChart<PL5_HZ_Info>(string.Format("QueryPL5_HZ_{0}", length), () =>
            {
                var infoList = new List<PL5_HZ_Info>();
                var entityList = new PL5_Manager().QueryPL5_HZ(length);

               ObjectConvert.ConvertEntityListToInfoList<List<PL5_HZ>, PL5_HZ, List<PL5_HZ_Info>, PL5_HZ_Info>(entityList, ref infoList,
                    () => { return new PL5_HZ_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        #endregion

        #region 生成走势图数据

        public void Add_GameWinNumber(string issuseNumber, string winNumber)
        {
            new KJGameIssuseBusiness().IssusePrize(this.CurrentGameCode, issuseNumber, winNumber);
            var manager = new PL5_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddPL5_GameWinNumber(new PL5_GameWinNumber
            {
                GameCode = this.CurrentGameCode,
                IssuseNumber = issuseNumber,
                WinNumber = winNumber,
                CreateTime = DateTime.Now,
            });
        }

        /// <summary>
        /// 基本走势
        /// </summary>
        private void Import_JBZS(string issuseNumber, string winNumber)
        {
            var manager = new PL5_Manager();
            var issuse = manager.QueryPL5_JBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var last = manager.QueryPL5_JBZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<PL5_JBZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("W_"))
                {
                    var order = p.Name.Replace("W_", string.Empty);
                    lastValue = winRed[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Q_"))
                {
                    var order = p.Name.Replace("Q_", string.Empty);
                    lastValue = winRed[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("B_"))
                {
                    var order = p.Name.Replace("B_", string.Empty);
                    lastValue = winRed[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("S_"))
                {
                    var order = p.Name.Replace("S_", string.Empty);
                    lastValue = winRed[3] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("G_"))
                {
                    var order = p.Name.Replace("G_", string.Empty);
                    lastValue = winRed[4] == order ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddPL5_JBZS(entity);
        }

        /// <summary>
        /// 任选大小走势
        /// </summary>
        private void Import_DX(string issuseNumber, string winNumber)
        {
            var manager = new PL5_Manager();
            var issuse = manager.QueryPL5_DXIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]) };

            #region 大小个数

            var winxiao1 = string.Empty;
            var winxiao2 = string.Empty;
            var winxiao3 = string.Empty;
            var winxiao4 = string.Empty;
            var winxiao5 = string.Empty;
            if (array[0] <= 4)
                winxiao1 = "X";
            else
                winxiao1 = "D";

            if (array[1] <= 4)
                winxiao2 = "X";
            else
                winxiao2 = "D";

            if (array[2] <= 4)
                winxiao3 = "X";
            else
                winxiao3 = "D";

            if (array[3] <= 4)
                winxiao4 = "X";
            else
                winxiao4 = "D";

            if (array[4] <= 4)
                winxiao5 = "X";
            else
                winxiao5 = "D";

            var arrayWinXiao = new string[] { winxiao1, winxiao2, winxiao3, winxiao4, winxiao5 };
            var XiaoType = string.Join("", arrayWinXiao);
            int DaCount = XiaoType.Count(p => p == 'D');
            #endregion

            var last = manager.QueryPL5_DX();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("DXqualifying", string.Join("", arrayWinXiao));
            dic.Add("DaoXiaoBi", string.Format("{0}:{1}", DaCount, 5 - DaCount));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<PL5_DX>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1_"))
                {
                    var order = p.Name.Replace("NO1_", string.Empty);
                    lastValue = arrayWinXiao[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2_"))
                {
                    var order = p.Name.Replace("NO2_", string.Empty);
                    lastValue = arrayWinXiao[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3_"))
                {
                    var order = p.Name.Replace("NO3_", string.Empty);
                    lastValue = arrayWinXiao[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO4_"))
                {
                    var order = p.Name.Replace("NO4_", string.Empty);
                    lastValue = arrayWinXiao[3] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO5_"))
                {
                    var order = p.Name.Replace("NO5_", string.Empty);
                    lastValue = arrayWinXiao[4] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var bi = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = DaCount == int.Parse(bi[0]) && 5 - DaCount == int.Parse(bi[1]) ? 0 : lastValue;
                }

                return lastValue;
            });

            manager.AddPL5_DX(entity);
        }

        /// <summary>
        /// 任选奇偶走势
        /// </summary>
        private void Import_JO(string issuseNumber, string winNumber)
        {
            var manager = new PL5_Manager();
            var issuse = manager.QueryPL5_JOIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]) };

            #region 奇偶个数

            var winji1 = string.Empty;
            var winji2 = string.Empty;
            var winji3 = string.Empty;
            var winji4 = string.Empty;
            var winji5 = string.Empty;
            if (array[0] % 2 == 1)
                winji1 = "J";
            else
                winji1 = "O";

            if (array[1] % 2 == 1)
                winji2 = "J";
            else
                winji2 = "O";

            if (array[2] % 2 == 1)
                winji3 = "J";
            else
                winji3 = "O";

            if (array[3] % 2 == 1)
                winji4 = "J";
            else
                winji4 = "O";

            if (array[4] % 2 == 1)
                winji5 = "J";
            else
                winji5 = "O";

            var arraywinji = new string[] { winji1, winji2, winji3, winji4, winji5 };
            var JiType = string.Join("", arraywinji);
            int JiCount = JiType.Count(p => p == 'J');
            #endregion

            var last = manager.QueryPL5_JO();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("JOqualifying", string.Join("", arraywinji));
            dic.Add("JiOuBi", string.Format("{0}:{1}", JiCount, 5 - JiCount));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<PL5_JO>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1_"))
                {
                    var order = p.Name.Replace("NO1_", string.Empty);
                    lastValue = arraywinji[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2_"))
                {
                    var order = p.Name.Replace("NO2_", string.Empty);
                    lastValue = arraywinji[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3_"))
                {
                    var order = p.Name.Replace("NO3_", string.Empty);
                    lastValue = arraywinji[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO4_"))
                {
                    var order = p.Name.Replace("NO4_", string.Empty);
                    lastValue = arraywinji[3] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO5_"))
                {
                    var order = p.Name.Replace("NO5_", string.Empty);
                    lastValue = arraywinji[4] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var bi = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = JiCount == int.Parse(bi[0]) && 5 - JiCount == int.Parse(bi[1]) ? 0 : lastValue;
                }

                return lastValue;
            });

            manager.AddPL5_JO(entity);

        }

        /// <summary>
        /// 任选质和走势
        /// </summary>
        private void Import_ZH(string issuseNumber, string winNumber)
        {
            var manager = new PL5_Manager();
            var issuse = manager.QueryPL5_ZHIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]) };

            #region 质和个数

            var zhilist = new int[] { 1, 2, 3, 5, 7 };
            var winzhi1 = string.Empty;
            var winzhi2 = string.Empty;
            var winzhi3 = string.Empty;
            var winzhi4 = string.Empty;
            var winzhi5 = string.Empty;
            if (zhilist.Contains(array[0]))
                winzhi1 = "Z";
            else
                winzhi1 = "H";

            if (zhilist.Contains(array[1]))
                winzhi2 = "Z";
            else
                winzhi2 = "H";

            if (zhilist.Contains(array[2]))
                winzhi3 = "Z";
            else
                winzhi3 = "H";

            if (zhilist.Contains(array[3]))
                winzhi4 = "Z";
            else
                winzhi4 = "H";

            if (zhilist.Contains(array[4]))
                winzhi5 = "Z";
            else
                winzhi5 = "H";

            var arraywinzhi = new string[] { winzhi1, winzhi2, winzhi3, winzhi4, winzhi5 };
            var zhiType = string.Join("", arraywinzhi);
            int ZhiCount = zhiType.Count(p => p == 'Z');
            #endregion

            var last = manager.QueryPL5_ZH();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("ZHqualifying", string.Join("", arraywinzhi));
            dic.Add("ZhiHeBi", string.Format("{0}:{1}", ZhiCount, 5 - ZhiCount));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<PL5_ZH>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1_"))
                {
                    var order = p.Name.Replace("NO1_", string.Empty);
                    lastValue = arraywinzhi[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2_"))
                {
                    var order = p.Name.Replace("NO2_", string.Empty);
                    lastValue = arraywinzhi[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3_"))
                {
                    var order = p.Name.Replace("NO3_", string.Empty);
                    lastValue = arraywinzhi[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO4_"))
                {
                    var order = p.Name.Replace("NO4_", string.Empty);
                    lastValue = arraywinzhi[3] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO5_"))
                {
                    var order = p.Name.Replace("NO5_", string.Empty);
                    lastValue = arraywinzhi[4] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var bi = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = ZhiCount == int.Parse(bi[0]) && 5 - ZhiCount == int.Parse(bi[1]) ? 0 : lastValue;
                }

                return lastValue;
            });

            manager.AddPL5_ZH(entity);

        }

        /// <summary>
        /// 除3走势
        /// </summary>
        private void Import_Chu3(string issuseNumber, string winNumber)
        {
            var manager = new PL5_Manager();
            var issuse = manager.QueryPL5_Chu3IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var chu3Arrary = new int[] { int.Parse(winRed[0]) % 3, int.Parse(winRed[1]) % 3, int.Parse(winRed[2]) % 3, int.Parse(winRed[3]) % 3, int.Parse(winRed[4]) % 3 };
            var count_0 = chu3Arrary.Count(p => p == 0);
            var count_1 = chu3Arrary.Count(p => p == 1);
            var count_2 = chu3Arrary.Count(p => p == 2);

            var last = manager.QueryPL5_Chu3();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("Chu3Bi", string.Format("{0}:{1}:{2}", count_0, count_1, count_2));

            var entity = this.CreateNewEntity<PL5_Chu3>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1_"))
                {
                    var order = p.Name.Replace("NO1_", string.Empty);
                    lastValue = chu3Arrary[0] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2_"))
                {
                    var order = p.Name.Replace("NO2_", string.Empty);
                    lastValue = chu3Arrary[1] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3_"))
                {
                    var order = p.Name.Replace("NO3_", string.Empty);
                    lastValue = chu3Arrary[2] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO4_"))
                {
                    var order = p.Name.Replace("NO4_", string.Empty);
                    lastValue = chu3Arrary[3] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO5_"))
                {
                    var order = p.Name.Replace("NO5_", string.Empty);
                    lastValue = chu3Arrary[4] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu0_"))
                {
                    var order = p.Name.Replace("Yu0_", string.Empty);
                    lastValue = int.Parse(order) == count_0 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu1_"))
                {
                    var order = p.Name.Replace("Yu1_", string.Empty);
                    lastValue = int.Parse(order) == count_1 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu2_"))
                {
                    var order = p.Name.Replace("Yu2_", string.Empty);
                    lastValue = int.Parse(order) == count_2 ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddPL5_Chu3(entity);
        }

        /// <summary>
        /// 和值走势
        /// </summary>
        private void Import_HZ(string issuseNumber, string winNumber)
        {
            var manager = new PL5_Manager();
            var issuse = manager.QueryPL5_HZIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]) + int.Parse(winRed[3]) + int.Parse(winRed[4]);
            var last = manager.QueryPL5_HZ();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<PL5_HZ>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("HZ_"))
                {
                    var order = p.Name.Replace("HZ_", string.Empty);
                    lastValue = hz == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddPL5_HZ(entity);
        }
        #endregion

        /// <summary>
        /// 查询开奖数据
        /// </summary>
        public GameWinNumber_InfoCollection QueryPL5_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new PL5_GameWinNumberManager().QueryPL5_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<PL5_GameWinNumber>, PL5_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryPL5_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new PL5_GameWinNumberManager().QueryPL5_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<PL5_GameWinNumber>, PL5_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
            var manager = new PL5_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<PL5_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }
    }
}
