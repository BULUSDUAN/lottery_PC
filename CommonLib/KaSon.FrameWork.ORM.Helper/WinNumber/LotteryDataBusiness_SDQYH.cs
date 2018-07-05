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
<<<<<<< HEAD
using KaSon.FrameWork.Analyzer.AnalyzerFactory;
=======
>>>>>>> a7171008b4bea1dab11582695738b3dd1fe77dcf

namespace KaSon.FrameWork.ORM.Helper.WinNumber
{
    public class LotteryDataBusiness_SDQYH : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get
            {
                return "SDQYH";
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

                this.ClearGameChartCache("QuerySDQYH_RXJO_Info");
                this.ClearGameChartCache("QuerySDQYH_RXZH_Info");
                this.ClearGameChartCache("QuerySDQYH_RXDX_Info");
                this.ClearGameChartCache("QuerySDQYH_Chu3_Info");
                this.ClearGameChartCache("QuerySDQYH_SX1_Info");
                this.ClearGameChartCache("QuerySDQYH_SX2_Info");
                this.ClearGameChartCache("QuerySDQYH_SX3_Info");
                this.ClearNewWinNumberCache("QuerySDQYH_GameWinNumber");

                Import_RXJO(issuseNumber, winNumber);
                Import_RXZH(issuseNumber, winNumber);
                Import_RXDX(issuseNumber, winNumber);
                Import_RXChu3(issuseNumber, winNumber);
                Import_SX1(issuseNumber, winNumber);
                Import_SX2(issuseNumber, winNumber);
                Import_SX3(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }

        #region 前台查询数据


        /// <summary>
        ///查询任选奇偶列表按时间倒叙 
        /// </summary>
        public SDQYH_RXJO_InfoCollection QuerySDQYH_RXJO_Info(int length)
        {
            SDQYH_RXJO_InfoCollection Collection = new SDQYH_RXJO_InfoCollection();
            var list = this.QueryGameChart<SDQYH_RXJO_Info>(string.Format("QuerySDQYH_RXJO_Info_{0}", length), () =>
            {
                var infoList = new List<SDQYH_RXJO_Info>();
                var entityList = new SDQYH_Manager().QuerySDQYH_RXJO_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDQYH_RXJO>, SDQYH_RXJO, List<SDQYH_RXJO_Info>, SDQYH_RXJO_Info>(entityList, ref infoList,
                    () => { return new SDQYH_RXJO_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询任选质和列表按时间倒叙 
        /// </summary>
        public SDQYH_RXZH_InfoCollection QuerySDQYH_RXZH_Info(int length)
        {
            SDQYH_RXZH_InfoCollection Collection = new SDQYH_RXZH_InfoCollection();
            var list = this.QueryGameChart<SDQYH_RXZH_Info>(string.Format("QuerySDQYH_RXZH_Info_{0}", length), () =>
            {
                var infoList = new List<SDQYH_RXZH_Info>();
                var entityList = new SDQYH_Manager().QuerySDQYH_RXZH_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDQYH_RXZH>, SDQYH_RXZH, List<SDQYH_RXZH_Info>, SDQYH_RXZH_Info>(entityList, ref infoList,
                    () => { return new SDQYH_RXZH_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询任选大小列表按时间倒叙 
        /// </summary>
        public SDQYH_RXDX_InfoCollection QuerySDQYH_RXDX_Info(int length)
        {
            SDQYH_RXDX_InfoCollection Collection = new SDQYH_RXDX_InfoCollection();
            var list = this.QueryGameChart<SDQYH_RXDX_Info>(string.Format("QuerySDQYH_RXDX_Info_{0}", length), () =>
            {
                var infoList = new List<SDQYH_RXDX_Info>();
                var entityList = new SDQYH_Manager().QuerySDQYH_RXDX_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDQYH_RXDX>, SDQYH_RXDX, List<SDQYH_RXDX_Info>, SDQYH_RXDX_Info>(entityList, ref infoList,
                    () => { return new SDQYH_RXDX_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询任选除3列表按时间倒叙 
        /// </summary>
        public SDQYH_Chu3_InfoCollection QuerySDQYH_Chu3_Info(int length)
        {
            SDQYH_Chu3_InfoCollection Collection = new SDQYH_Chu3_InfoCollection();
            var list = this.QueryGameChart<SDQYH_Chu3_Info>(string.Format("QuerySDQYH_Chu3_Info_{0}", length), () =>
            {
                var infoList = new List<SDQYH_Chu3_Info>();
                var entityList = new SDQYH_Manager().QuerySDQYH_Chu3_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDQYH_Chu3>, SDQYH_Chu3, List<SDQYH_Chu3_Info>, SDQYH_Chu3_Info>(entityList, ref infoList,
                    () => { return new SDQYH_Chu3_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询顺一列表按时间倒叙 
        /// </summary>
        public SDQYH_SX1_InfoCollection QuerySDQYH_SX1_Info(int length)
        {
            SDQYH_SX1_InfoCollection Collection = new SDQYH_SX1_InfoCollection();
            var list = this.QueryGameChart<SDQYH_SX1_Info>(string.Format("QuerySDQYH_SX1_Info_{0}", length), () =>
            {
                var infoList = new List<SDQYH_SX1_Info>();
                var entityList = new SDQYH_Manager().QuerySDQYH_SX1_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDQYH_SX1>, SDQYH_SX1, List<SDQYH_SX1_Info>, SDQYH_SX1_Info>(entityList, ref infoList,
                    () => { return new SDQYH_SX1_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询顺二列表按时间倒叙 
        /// </summary>
        public SDQYH_SX2_InfoCollection QuerySDQYH_SX2_Info(int length)
        {
            SDQYH_SX2_InfoCollection Collection = new SDQYH_SX2_InfoCollection();
            var list = this.QueryGameChart<SDQYH_SX2_Info>(string.Format("QuerySDQYH_SX2_Info_{0}", length), () =>
            {
                var infoList = new List<SDQYH_SX2_Info>();
                var entityList = new SDQYH_Manager().QuerySDQYH_SX2_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDQYH_SX2>, SDQYH_SX2, List<SDQYH_SX2_Info>, SDQYH_SX2_Info>(entityList, ref infoList,
                    () => { return new SDQYH_SX2_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询顺三列表按时间倒叙 
        /// </summary>
        public SDQYH_SX3_InfoCollection QuerySDQYH_SX3_Info(int length)
        {
            SDQYH_SX3_InfoCollection Collection = new SDQYH_SX3_InfoCollection();
            var list = this.QueryGameChart<SDQYH_SX3_Info>(string.Format("QuerySDQYH_SX3_Info_{0}", length), () =>
            {
                var infoList = new List<SDQYH_SX3_Info>();
                var entityList = new SDQYH_Manager().QuerySDQYH_SX3_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDQYH_SX3>, SDQYH_SX3, List<SDQYH_SX3_Info>, SDQYH_SX3_Info>(entityList, ref infoList,
                    () => { return new SDQYH_SX3_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        #endregion

        #region 生成走势数据

        public void Add_GameWinNumber(string issuseNumber, string winNumber)
        {
            new KJGameIssuseBusiness().IssusePrize(this.CurrentGameCode, issuseNumber, winNumber);
            var manager = new SDQYH_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddSDQYH_GameWinNumber(new SDQYH_GameWinNumber
            {
                GameCode = this.CurrentGameCode,
                IssuseNumber = issuseNumber,
                WinNumber = winNumber,
                CreateTime = DateTime.Now,
            });

        }

        /// <summary>
        /// 任选奇偶走势
        /// </summary>
        private void Import_RXJO(string issuseNumber, string winNumber)
        {
            var manager = new SDQYH_Manager();
            var issuse = manager.QuerySDQYH_RXJOIssuseNumber(issuseNumber);
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

            var last = manager.QuerySDQYH_RXJO();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("JOqualifying", string.Join("", arraywinji));
            dic.Add("JiOuBi", string.Format("{0}:{1}", JiCount, 5 - JiCount));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<SDQYH_RXJO>(dic, (p) =>
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

            manager.AddSDQYH_RXJO(entity);

        }

        /// <summary>
        /// 任选质和走势
        /// </summary>
        private void Import_RXZH(string issuseNumber, string winNumber)
        {
            var manager = new SDQYH_Manager();
            var issuse = manager.QuerySDQYH_RXZHIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]) };

            #region 质和个数

            var zhilist = new int[] { 1, 2, 3, 5, 7, 11, 13, 19, 23 };
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

            var last = manager.QuerySDQYH_RXZH();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("ZHqualifying", string.Join("", arraywinzhi));
            dic.Add("ZhiHeBi", string.Format("{0}:{1}", ZhiCount, 5 - ZhiCount));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<SDQYH_RXZH>(dic, (p) =>
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
            manager.AddSDQYH_RXZH(entity);

        }

        /// <summary>
        /// 任选大小走势
        /// </summary>
        private void Import_RXDX(string issuseNumber, string winNumber)
        {
            var manager = new SDQYH_Manager();
            var issuse = manager.QuerySDQYH_RXDXIssuseNumber(issuseNumber);
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
            if (array[0] <= 12)
                winxiao1 = "X";
            else
                winxiao1 = "D";

            if (array[1] <= 12)
                winxiao2 = "X";
            else
                winxiao2 = "D";

            if (array[2] <= 12)
                winxiao3 = "X";
            else
                winxiao3 = "D";

            if (array[3] <= 12)
                winxiao4 = "X";
            else
                winxiao4 = "D";

            if (array[4] <= 12)
                winxiao5 = "X";
            else
                winxiao5 = "D";

            var arrayWinXiao = new string[] { winxiao1, winxiao2, winxiao3, winxiao4, winxiao5 };
            var XiaoType = string.Join("", arrayWinXiao);
            int DaCount = XiaoType.Count(p => p == 'D');
            #endregion

            var last = manager.QuerySDQYH_RXDX();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("DXqualifying", string.Join("", arrayWinXiao));
            dic.Add("DaoXiaoBi", string.Format("{0}:{1}", DaCount, 5 - DaCount));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<SDQYH_RXDX>(dic, (p) =>
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


            manager.AddSDQYH_RXDX(entity);
        }

        /// <summary>
        /// 除3走势
        /// </summary>
        private void Import_RXChu3(string issuseNumber, string winNumber)
        {
            var manager = new SDQYH_Manager();
            var issuse = manager.QuerySDQYH_Chu3IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var chu3Arrary = new int[] { int.Parse(winRed[0]) % 3, int.Parse(winRed[1]) % 3, int.Parse(winRed[2]) % 3, int.Parse(winRed[3]) % 3, int.Parse(winRed[4]) % 3 };
            var count_0 = chu3Arrary.Count(p => p == 0);
            var count_1 = chu3Arrary.Count(p => p == 1);
            var count_2 = chu3Arrary.Count(p => p == 2);

            var last = manager.QuerySDQYH_Chu3();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("Chu3Bi", "");

            var entity = this.CreateNewEntity<SDQYH_Chu3>(dic, (p) =>
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

            entity.Chu3Bi = string.Format("{0}:{1}:{2}:{3}:{4}", chu3Arrary[0], chu3Arrary[1], chu3Arrary[2], chu3Arrary[3], chu3Arrary[4]);
            manager.AddSDQYH_Chu3(entity);
        }

        /// <summary>
        /// 顺选1走势
        /// </summary>
        private void Import_SX1(string issuseNumber, string winNumber)
        {
            var manager = new SDQYH_Manager();
            var issuse = manager.QuerySDQYH_SX1IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var winxiao = string.Empty;
            if (int.Parse(winRed[0]) <= 10)
                winxiao = "X";
            else
                winxiao = "D";

            var winji = string.Empty;
            if (int.Parse(winRed[0]) % 2 == 1)
                winji = "J";
            else
                winji = "O";

            var zhilist = new int[] { 1, 2, 3, 5, 7, 11, 13, 19, 23 };
            var winzhi = string.Empty;
            if (zhilist.Contains(int.Parse(winRed[0])))
                winzhi = "Z";
            else
                winzhi = "H";

            var last = manager.QuerySDQYH_SX1();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<SDQYH_SX1>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO_"))
                {
                    var order = p.Name.Replace("NO_", string.Empty);
                    lastValue = winRed[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("DX_"))
                {
                    var order = p.Name.Replace("DX_", string.Empty);
                    lastValue = winxiao == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("JO_"))
                {
                    var order = p.Name.Replace("JO_", string.Empty);
                    lastValue = winji == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("ZH_"))
                {
                    var order = p.Name.Replace("ZH_", string.Empty);
                    lastValue = winzhi == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("YU_"))
                {
                    var order = p.Name.Replace("YU_", string.Empty);
                    lastValue = int.Parse(winRed[0]) % 3 == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddSDQYH_SX1(entity);
        }

        /// <summary>
        /// 顺选1走势
        /// </summary>
        private void Import_SX2(string issuseNumber, string winNumber)
        {
            var manager = new SDQYH_Manager();
            var issuse = manager.QuerySDQYH_SX2IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var winxiao = string.Empty;
            if (int.Parse(winRed[1]) <= 10)
                winxiao = "X";
            else
                winxiao = "D";

            var winji = string.Empty;
            if (int.Parse(winRed[1]) % 2 == 1)
                winji = "J";
            else
                winji = "O";

            var zhilist = new int[] { 1, 2, 3, 5, 7, 11, 13, 19, 23 };
            var winzhi = string.Empty;
            if (zhilist.Contains(int.Parse(winRed[1])))
                winzhi = "Z";
            else
                winzhi = "H";

            var last = manager.QuerySDQYH_SX2();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<SDQYH_SX2>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO_"))
                {
                    var order = p.Name.Replace("NO_", string.Empty);
                    lastValue = winRed[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("DX_"))
                {
                    var order = p.Name.Replace("DX_", string.Empty);
                    lastValue = winxiao == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("JO_"))
                {
                    var order = p.Name.Replace("JO_", string.Empty);
                    lastValue = winji == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("ZH_"))
                {
                    var order = p.Name.Replace("ZH_", string.Empty);
                    lastValue = winzhi == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("YU_"))
                {
                    var order = p.Name.Replace("YU_", string.Empty);
                    lastValue = int.Parse(winRed[1]) % 3 == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddSDQYH_SX2(entity);
        }

        /// <summary>
        /// 顺选1走势
        /// </summary>
        private void Import_SX3(string issuseNumber, string winNumber)
        {
            var manager = new SDQYH_Manager();
            var issuse = manager.QuerySDQYH_SX3IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var winxiao = string.Empty;
            if (int.Parse(winRed[2]) <= 10)
                winxiao = "X";
            else
                winxiao = "D";

            var winji = string.Empty;
            if (int.Parse(winRed[2]) % 2 == 1)
                winji = "J";
            else
                winji = "O";

            var zhilist = new int[] { 1, 2, 3, 5, 7, 11, 13, 19, 23 };
            var winzhi = string.Empty;
            if (zhilist.Contains(int.Parse(winRed[2])))
                winzhi = "Z";
            else
                winzhi = "H";

            var last = manager.QuerySDQYH_SX3();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<SDQYH_SX3>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO_"))
                {
                    var order = p.Name.Replace("NO_", string.Empty);
                    lastValue = winRed[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("DX_"))
                {
                    var order = p.Name.Replace("DX_", string.Empty);
                    lastValue = winxiao == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("JO_"))
                {
                    var order = p.Name.Replace("JO_", string.Empty);
                    lastValue = winji == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("ZH_"))
                {
                    var order = p.Name.Replace("ZH_", string.Empty);
                    lastValue = winzhi == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("YU_"))
                {
                    var order = p.Name.Replace("YU_", string.Empty);
                    lastValue = int.Parse(winRed[2]) % 3 == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddSDQYH_SX3(entity);
        }

        #endregion

        /// <summary>
        /// 查询开奖数据
        /// </summary>
        public GameWinNumber_InfoCollection QuerySDQYH_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new SDQYH_GameWinNumberManager().QuerySDQYH_GameWinNumber(pageIndex, pageSize, out totalCount);
           ObjectConvert.ConvertEntityListToInfoList<List<SDQYH_GameWinNumber>, SDQYH_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QuerySDQYH_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new SDQYH_GameWinNumberManager().QuerySDQYH_GameWinNumber(pageIndex, pageSize, out totalCount);
            //   ObjectConvert.ConvertEntityListToInfoList<List<SDQYH_GameWinNumber>, SDQYH_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
            var manager = new SDQYH_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<SDQYH_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }
    }
}
