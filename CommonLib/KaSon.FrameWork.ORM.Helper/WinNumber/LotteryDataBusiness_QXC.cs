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
    public class LotteryDataBusiness_QXC : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get
            {
                return "QXC";
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

                this.ClearGameChartCache("QueryQXC_JBZS");
                this.ClearGameChartCache("QueryQXC_DX");
                this.ClearGameChartCache("QueryQXC_DX");
                this.ClearGameChartCache("QueryQXC_ZH");
                this.ClearGameChartCache("QueryQXC_Chu3");
                this.ClearNewWinNumberCache("QueryQXC_GameWinNumber");

                Import_JBZS(issuseNumber, winNumber);
                Import_DX(issuseNumber, winNumber);
                Import_JO(issuseNumber, winNumber);
                Import_ZH(issuseNumber, winNumber);
                Import_Chu3(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }

        #region 前台查询数据

        /// <summary>
        ///查询基本走势列表按时间倒叙 
        /// </summary>
        public QXC_JBZS_InfoCollection QueryQXC_JBZS(int length)
        {
            QXC_JBZS_InfoCollection Collection = new QXC_JBZS_InfoCollection();
            var list = this.QueryGameChart<QXC_JBZS_Info>(string.Format("QueryQXC_JBZS_{0}", length), () =>
            {
                var infoList = new List<QXC_JBZS_Info>();
                var entityList = new QXC_Manager().QueryQXC_JBZS(length);

               ObjectConvert.ConvertEntityListToInfoList<List<QXC_JBZS>, QXC_JBZS, List<QXC_JBZS_Info>, QXC_JBZS_Info>(entityList, ref infoList,
                    () => { return new QXC_JBZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询大小走势列表按时间倒叙 
        /// </summary>
        public QXC_DX_InfoCollection QueryQXC_DX(int length)
        {
            QXC_DX_InfoCollection Collection = new QXC_DX_InfoCollection();
            var list = this.QueryGameChart<QXC_DX_Info>(string.Format("QueryQXC_DX_{0}", length), () =>
            {
                var infoList = new List<QXC_DX_Info>();
                var entityList = new QXC_Manager().QueryQXC_DX(length);

               ObjectConvert.ConvertEntityListToInfoList<List<QXC_DX>, QXC_DX, List<QXC_DX_Info>, QXC_DX_Info>(entityList, ref infoList,
                    () => { return new QXC_DX_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询奇偶走势列表按时间倒叙 
        /// </summary>
        public QXC_JO_InfoCollection QueryQXC_JO(int length)
        {
            QXC_JO_InfoCollection Collection = new QXC_JO_InfoCollection();
            var list = this.QueryGameChart<QXC_JO_Info>(string.Format("QueryQXC_JO_{0}", length), () =>
            {
                var infoList = new List<QXC_JO_Info>();
                var entityList = new QXC_Manager().QueryQXC_JO(length);

               ObjectConvert.ConvertEntityListToInfoList<List<QXC_JO>, QXC_JO, List<QXC_JO_Info>, QXC_JO_Info>(entityList, ref infoList,
                    () => { return new QXC_JO_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询质和走势列表按时间倒叙 
        /// </summary>
        public QXC_ZH_InfoCollection QueryQXC_ZH(int length)
        {
            QXC_ZH_InfoCollection Collection = new QXC_ZH_InfoCollection();
            var list = this.QueryGameChart<QXC_ZH_Info>(string.Format("QueryQXC_ZH_{0}", length), () =>
            {
                var infoList = new List<QXC_ZH_Info>();
                var entityList = new QXC_Manager().QueryQXC_ZH(length);

               ObjectConvert.ConvertEntityListToInfoList<List<QXC_ZH>, QXC_ZH, List<QXC_ZH_Info>, QXC_ZH_Info>(entityList, ref infoList,
                    () => { return new QXC_ZH_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询除3走势列表按时间倒叙 
        /// </summary>
        public QXC_Chu3_InfoCollection QueryQXC_Chu3(int length)
        {
            QXC_Chu3_InfoCollection Collection = new QXC_Chu3_InfoCollection();
            var list = this.QueryGameChart<QXC_Chu3_Info>(string.Format("QueryQXC_Chu3_{0}", length), () =>
            {
                var infoList = new List<QXC_Chu3_Info>();
                var entityList = new QXC_Manager().QueryQXC_Chu3(length);

               ObjectConvert.ConvertEntityListToInfoList<List<QXC_Chu3>, QXC_Chu3, List<QXC_Chu3_Info>, QXC_Chu3_Info>(entityList, ref infoList,
                    () => { return new QXC_Chu3_Info(); });
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
            var manager = new QXC_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddQXC_GameWinNumber(new QXC_GameWinNumber
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
            var manager = new QXC_Manager();
            var issuse = manager.QueryQXC_JBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var last = manager.QueryQXC_JBZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<QXC_JBZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1_"))
                {
                    var order = p.Name.Replace("NO1_", string.Empty);
                    lastValue = winRed[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2_"))
                {
                    var order = p.Name.Replace("NO2_", string.Empty);
                    lastValue = winRed[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3_"))
                {
                    var order = p.Name.Replace("NO3_", string.Empty);
                    lastValue = winRed[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO4_"))
                {
                    var order = p.Name.Replace("NO4_", string.Empty);
                    lastValue = winRed[3] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO5_"))
                {
                    var order = p.Name.Replace("NO5_", string.Empty);
                    lastValue = winRed[4] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO6_"))
                {
                    var order = p.Name.Replace("NO6_", string.Empty);
                    lastValue = winRed[5] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO7_"))
                {
                    var order = p.Name.Replace("NO7_", string.Empty);
                    lastValue = winRed[6] == order ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddQXC_JBZS(entity);
        }

        /// <summary>
        /// 大小走势
        /// </summary>
        private void Import_DX(string issuseNumber, string winNumber)
        {
            var manager = new QXC_Manager();
            var issuse = manager.QuerQXC_DXIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]), int.Parse(winRed[5]), int.Parse(winRed[6]) };

            #region  小大排位
            int dCount = 0;
            int xCount = 0;
            foreach (var item in array)
            {
                if (item <= 4)
                    xCount++;
                else
                    dCount++;
            }
            #endregion

            #region 大小个数

            string winxiao1, winxiao2, winxiao3, winxiao4, winxiao5, winxiao6, winxiao7 = string.Empty;

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

            if (array[5] <= 4)
                winxiao6 = "X";
            else
                winxiao6 = "D";

            if (array[6] <= 4)
                winxiao7 = "X";
            else
                winxiao7 = "D";

            var arrayWinXiao = new string[] { winxiao1, winxiao2, winxiao3, winxiao4, winxiao5, winxiao6, winxiao7 };
            #endregion

            var last = manager.QueryQXC_DX();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("DaoXiaoBi", string.Format("{0}:{1}", dCount, xCount));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<QXC_DX>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1"))
                {
                    var order = p.Name.Replace("NO1", string.Empty);
                    lastValue = arrayWinXiao[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2"))
                {
                    var order = p.Name.Replace("NO2", string.Empty);
                    lastValue = arrayWinXiao[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3"))
                {
                    var order = p.Name.Replace("NO3", string.Empty);
                    lastValue = arrayWinXiao[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO4"))
                {
                    var order = p.Name.Replace("NO4", string.Empty);
                    lastValue = arrayWinXiao[3] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO5"))
                {
                    var order = p.Name.Replace("NO5", string.Empty);
                    lastValue = arrayWinXiao[4] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO6"))
                {
                    var order = p.Name.Replace("NO6", string.Empty);
                    lastValue = arrayWinXiao[5] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO7"))
                {
                    var order = p.Name.Replace("NO7", string.Empty);
                    lastValue = arrayWinXiao[6] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var bi = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(bi[0]) == dCount && int.Parse(bi[1]) == xCount ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddQXC_DX(entity);
        }

        /// <summary>
        /// 奇偶走势
        /// </summary>
        private void Import_JO(string issuseNumber, string winNumber)
        {
            var manager = new QXC_Manager();
            var issuse = manager.QueryQXC_JOIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]), int.Parse(winRed[5]), int.Parse(winRed[6]) };

            #region  奇偶排位
            int jCount = 0;
            int oCount = 0;
            foreach (var item in array)
            {
                if (item % 2 == 0)
                    oCount++;
                else
                    jCount++;
            }

            #endregion

            #region 奇偶个数

            string winxji1, winji2, winji3, winji4, winji5, winji6, winji7 = string.Empty;

            if (array[0] % 2 == 0)
                winxji1 = "O";
            else
                winxji1 = "J";

            if (array[1] % 2 == 0)
                winji2 = "O";
            else
                winji2 = "J";

            if (array[2] % 2 == 0)
                winji3 = "O";
            else
                winji3 = "J";

            if (array[3] % 2 == 0)
                winji4 = "O";
            else
                winji4 = "J";

            if (array[4] % 2 == 0)
                winji5 = "O";
            else
                winji5 = "J";

            if (array[5] % 2 == 0)
                winji6 = "O";
            else
                winji6 = "J";

            if (array[6] % 2 == 0)
                winji7 = "O";
            else
                winji7 = "J";

            var arrayWinJi = new string[] { winxji1, winji2, winji3, winji4, winji5, winji6, winji7 };
            #endregion

            var last = manager.QueryQXC_JO();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("JiOuBi", string.Format("{0}:{1}", jCount, oCount));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<QXC_JO>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1"))
                {
                    var order = p.Name.Replace("NO1", string.Empty);
                    lastValue = arrayWinJi[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2"))
                {
                    var order = p.Name.Replace("NO2", string.Empty);
                    lastValue = arrayWinJi[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3"))
                {
                    var order = p.Name.Replace("NO3", string.Empty);
                    lastValue = arrayWinJi[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO4"))
                {
                    var order = p.Name.Replace("NO4", string.Empty);
                    lastValue = arrayWinJi[3] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO5"))
                {
                    var order = p.Name.Replace("NO5", string.Empty);
                    lastValue = arrayWinJi[4] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO6"))
                {
                    var order = p.Name.Replace("NO6", string.Empty);
                    lastValue = arrayWinJi[5] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO7"))
                {
                    var order = p.Name.Replace("NO7", string.Empty);
                    lastValue = arrayWinJi[6] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var bi = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(bi[0]) == jCount && int.Parse(bi[1]) == oCount ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddQXC_JO(entity);
        }

        /// <summary>
        /// 质和走势
        /// </summary>
        private void Import_ZH(string issuseNumber, string winNumber)
        {
            var manager = new QXC_Manager();
            var issuse = manager.QueryQXC_ZHIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var zhilist = new int[] { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]), int.Parse(winRed[5]), int.Parse(winRed[6]) };

            #region 质和个数

            string winxzhi1, winzhii2, winzhi3, winzhi4, winzhi5, winzhi6, winzhi7 = string.Empty;

            if (zhilist.Contains(array[0]))
                winxzhi1 = "Z";
            else
                winxzhi1 = "H";

            if (zhilist.Contains(array[1]))
                winzhii2 = "Z";
            else
                winzhii2 = "H";

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

            if (zhilist.Contains(array[5]))
                winzhi6 = "Z";
            else
                winzhi6 = "H";

            if (zhilist.Contains(array[6]))
                winzhi7 = "Z";
            else
                winzhi7 = "H";

            var arrayWinZhi = new string[] { winxzhi1, winzhii2, winzhi3, winzhi4, winzhi5, winzhi6, winzhi7 };
            #endregion

            #region  质和排位
            int zCount = 0;
            int hCount = 0;
            foreach (var item in array)
            {
                if (zhilist.Contains(item))
                    zCount++;
                else
                    hCount++;
            }

            #endregion

            var last = manager.QueryQXC_ZH();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("ZhiHeBi", string.Format("{0}:{1}", zCount, hCount));
            dic.Add("CreateTime", DateTime.Now);
            var zhiHe_paiwei = new List<string>();

            var entity = this.CreateNewEntity<QXC_ZH>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1"))
                {
                    var order = p.Name.Replace("NO1", string.Empty);
                    lastValue = arrayWinZhi[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2"))
                {
                    var order = p.Name.Replace("NO2", string.Empty);
                    lastValue = arrayWinZhi[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3"))
                {
                    var order = p.Name.Replace("NO3", string.Empty);
                    lastValue = arrayWinZhi[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO4"))
                {
                    var order = p.Name.Replace("NO4", string.Empty);
                    lastValue = arrayWinZhi[3] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO5"))
                {
                    var order = p.Name.Replace("NO5", string.Empty);
                    lastValue = arrayWinZhi[4] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO6"))
                {
                    var order = p.Name.Replace("NO6", string.Empty);
                    lastValue = arrayWinZhi[5] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO7"))
                {
                    var order = p.Name.Replace("NO7", string.Empty);
                    lastValue = arrayWinZhi[6] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var bi = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(bi[0]) == zCount && int.Parse(bi[1]) == hCount ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddQXC_ZH(entity);
        }

        /// <summary>
        /// 除3走势
        /// </summary>
        private void Import_Chu3(string issuseNumber, string winNumber)
        {
            var manager = new QXC_Manager();
            var issuse = manager.QueryQXC_Chu3IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]) % 3, int.Parse(winRed[1]) % 3, int.Parse(winRed[2]) % 3, int.Parse(winRed[3]) % 3, int.Parse(winRed[4]) % 3, int.Parse(winRed[5]) % 3, int.Parse(winRed[6]) % 3 };
            var array_0 = array.Count(p => p == 0);
            var array_1 = array.Count(p => p == 1);
            var array_2 = array.Count(p => p == 2);

            var last = manager.QueryQXC_Chu3();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("Chu3Bi", string.Format("{0}:{1}:{2}", array_0, array_1, array_2));
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<QXC_Chu3>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1_"))
                {
                    var order = p.Name.Replace("NO1_", string.Empty);
                    lastValue = array[0] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2_"))
                {
                    var order = p.Name.Replace("NO2_", string.Empty);
                    lastValue = array[1] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3_"))
                {
                    var order = p.Name.Replace("NO3_", string.Empty);
                    lastValue = array[2] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO4_"))
                {
                    var order = p.Name.Replace("NO4_", string.Empty);
                    lastValue = array[3] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO5_"))
                {
                    var order = p.Name.Replace("NO5_", string.Empty);
                    lastValue = array[4] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO6_"))
                {
                    var order = p.Name.Replace("NO6_", string.Empty);
                    lastValue = array[5] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO7_"))
                {
                    var order = p.Name.Replace("NO7_", string.Empty);
                    lastValue = array[6] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu0_"))
                {
                    var order = p.Name.Replace("Yu0_", string.Empty);
                    lastValue = array_0 == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu1_"))
                {
                    var order = p.Name.Replace("Yu1_", string.Empty);
                    lastValue = array_1 == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu2_"))
                {
                    var order = p.Name.Replace("Yu2_", string.Empty);
                    lastValue = array_2 == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });
            manager.AddQXC_Chu3(entity);
        }

        #endregion

        /// <summary>
        /// 查询开奖数据
        /// </summary>
        public GameWinNumber_InfoCollection QueryQXC_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new QXC_GameWinNumberManager().QueryQXC_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<QXC_GameWinNumber>, QXC_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryQXC_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new QXC_GameWinNumberManager().QueryQXC_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<QXC_GameWinNumber>, QXC_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
            var manager = new QXC_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<QXC_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }
    }
}
