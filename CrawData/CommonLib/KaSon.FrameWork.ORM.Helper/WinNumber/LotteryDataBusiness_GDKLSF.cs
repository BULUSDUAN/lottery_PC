using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.ORM.Helper.WinNumber.Manage;
using EntityModel;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;

namespace KaSon.FrameWork.ORM.Helper.WinNumber
{
    public class LotteryDataBusiness_GDKLSF : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get
            {
                return "GDKLSF";
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

                this.ClearGameChartCache("QueryGDKLSF_JBZS");
                this.ClearGameChartCache("QueryGDKLSF_DW1");
                this.ClearGameChartCache("QueryGDKLSF_DW2");
                this.ClearGameChartCache("QueryGDKLSF_DW3");
                this.ClearGameChartCache("QueryGDKLSF_DX");
                this.ClearGameChartCache("QueryGDKLSF_JO");
                this.ClearGameChartCache("QueryGDKLSF_ZH");
                this.ClearNewWinNumberCache("QueryGDKLSF_GameWinNumber");

                Import_JBZS(issuseNumber, winNumber);
                Import__DW1(issuseNumber, winNumber);
                Import__DW2(issuseNumber, winNumber);
                Import__DW3(issuseNumber, winNumber);
                Import__DX(issuseNumber, winNumber);
                Import__JO(issuseNumber, winNumber);
                Import__ZH(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }


        #region 前台查询数据

        /// <summary>
        ///查询基本走势列表按时间倒叙 
        /// </summary>
        public GDKLSF_JBZS_InfoCollection QueryGDKLSF_JBZS(int length)
        {
            GDKLSF_JBZS_InfoCollection Collection = new GDKLSF_JBZS_InfoCollection();
            var list = this.QueryGameChart<GDKLSF_JBZS_Info>(string.Format("QueryGDKLSF_JBZS_{0}", length), () =>
            {
                var infoList = new List<GDKLSF_JBZS_Info>();
                var entityList = new GDKLSF_Manager().QueryGDKLSF_JBZS(length);

               ObjectConvert.ConvertEntityListToInfoList<List<GDKLSF_JBZS>, GDKLSF_JBZS, List<GDKLSF_JBZS_Info>, GDKLSF_JBZS_Info>(entityList, ref infoList,
                    () => { return new GDKLSF_JBZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询定位第一位列表按时间倒叙 
        /// </summary>
        public GDKLSF_DW1_InfoCollection QueryGDKLSF_DW1(int length)
        {
            GDKLSF_DW1_InfoCollection Collection = new GDKLSF_DW1_InfoCollection();
            var list = this.QueryGameChart<GDKLSF_DW1_Info>(string.Format("QueryGDKLSF_DW1_{0}", length), () =>
            {
                var infoList = new List<GDKLSF_DW1_Info>();
                var entityList = new GDKLSF_Manager().QueryGDKLSF_DW1(length);

               ObjectConvert.ConvertEntityListToInfoList<List<GDKLSF_DW1>, GDKLSF_DW1, List<GDKLSF_DW1_Info>, GDKLSF_DW1_Info>(entityList, ref infoList,
                    () => { return new GDKLSF_DW1_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询定位第二位列表按时间倒叙 
        /// </summary>
        public GDKLSF_DW2_InfoCollection QueryGDKLSF_DW2(int length)
        {
            GDKLSF_DW2_InfoCollection Collection = new GDKLSF_DW2_InfoCollection();
            var list = this.QueryGameChart<GDKLSF_DW2_Info>(string.Format("QueryGDKLSF_DW2_{0}", length), () =>
            {
                var infoList = new List<GDKLSF_DW2_Info>();
                var entityList = new GDKLSF_Manager().QueryGDKLSF_DW2(length);

               ObjectConvert.ConvertEntityListToInfoList<List<GDKLSF_DW2>, GDKLSF_DW2, List<GDKLSF_DW2_Info>, GDKLSF_DW2_Info>(entityList, ref infoList,
                    () => { return new GDKLSF_DW2_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询定位第三位列表按时间倒叙 
        /// </summary>
        public GDKLSF_DW3_InfoCollection QueryGDKLSF_DW3(int length)
        {
            GDKLSF_DW3_InfoCollection Collection = new GDKLSF_DW3_InfoCollection();
            var list = this.QueryGameChart<GDKLSF_DW3_Info>(string.Format("QueryGDKLSF_DW3_{0}", length), () =>
            {
                var infoList = new List<GDKLSF_DW3_Info>();
                var entityList = new GDKLSF_Manager().QueryGDKLSF_DW3(length);

               ObjectConvert.ConvertEntityListToInfoList<List<GDKLSF_DW3>, GDKLSF_DW3, List<GDKLSF_DW3_Info>, GDKLSF_DW3_Info>(entityList, ref infoList,
                    () => { return new GDKLSF_DW3_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询大小列表按时间倒叙 
        /// </summary>
        public GDKLSF_DX_InfoCollection QueryGDKLSF_DX(int length)
        {
            GDKLSF_DX_InfoCollection Collection = new GDKLSF_DX_InfoCollection();
            var list = this.QueryGameChart<GDKLSF_DX_Info>(string.Format("QueryGDKLSF_DX_{0}", length), () =>
            {
                var infoList = new List<GDKLSF_DX_Info>();
                var entityList = new GDKLSF_Manager().QueryGDKLSF_DX(length);

               ObjectConvert.ConvertEntityListToInfoList<List<GDKLSF_DX>, GDKLSF_DX, List<GDKLSF_DX_Info>, GDKLSF_DX_Info>(entityList, ref infoList,
                    () => { return new GDKLSF_DX_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询奇偶列表按时间倒叙 
        /// </summary>
        public GDKLSF_JO_InfoCollection QueryGDKLSF_JO(int length)
        {
            GDKLSF_JO_InfoCollection Collection = new GDKLSF_JO_InfoCollection();
            var list = this.QueryGameChart<GDKLSF_JO_Info>(string.Format("QueryGDKLSF_JO_{0}", length), () =>
            {
                var infoList = new List<GDKLSF_JO_Info>();
                var entityList = new GDKLSF_Manager().QueryGDKLSF_JO(length);

               ObjectConvert.ConvertEntityListToInfoList<List<GDKLSF_JO>, GDKLSF_JO, List<GDKLSF_JO_Info>, GDKLSF_JO_Info>(entityList, ref infoList,
                    () => { return new GDKLSF_JO_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询质和列表按时间倒叙 
        /// </summary>
        public GDKLSF_ZH_InfoCollection QueryGDKLSF_ZH(int length)
        {
            GDKLSF_ZH_InfoCollection Collection = new GDKLSF_ZH_InfoCollection();
            var list = this.QueryGameChart<GDKLSF_ZH_Info>(string.Format("QueryGDKLSF_ZH_{0}", length), () =>
            {
                var infoList = new List<GDKLSF_ZH_Info>();
                var entityList = new GDKLSF_Manager().QueryGDKLSF_ZH(length);

               ObjectConvert.ConvertEntityListToInfoList<List<GDKLSF_ZH>, GDKLSF_ZH, List<GDKLSF_ZH_Info>, GDKLSF_ZH_Info>(entityList, ref infoList,
                    () => { return new GDKLSF_ZH_Info(); });
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
            var manager = new GDKLSF_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddGDKLSF_GameWinNumber(new GDKLSF_GameWinNumber
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
            var manager = new GDKLSF_Manager();
            var issuse = manager.QueryGDKLSF_JBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var winNum = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[3]), int.Parse(winRed[3]), int.Parse(winRed[5]), int.Parse(winRed[6]), int.Parse(winRed[7]) };
            var hz = winNum[0] + winNum[1] + winNum[2] + winNum[3] + winNum[4] + winNum[5] + winNum[6] + winNum[7];
            var hw = hz % 10;

            #region 大小个数
            int dCount = 0;
            int xCount = 0;
            foreach (var item in winNum)
            {
                if (item <= 10)
                    xCount++;
                else
                    dCount++;
            }
            #endregion

            #region 奇偶个数
            int jCount = 0;
            int oCount = 0;
            foreach (var item in winNum)
            {
                if (item % 2 == 1)
                    jCount++;
                else
                    oCount++;
            }
            #endregion

            #region  质和排位
            var zhilist = new int[] { 1, 2, 3, 5, 7, 11, 13, 17, 19 };
            int zCount = 0;
            foreach (var item in winNum)
            {
                if (zhilist.Contains(item))
                    zCount++;
            }
            #endregion

            var last = manager.QueryGDKLSF_JBZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("JO_Bi", string.Format("{0}:{1}", jCount, oCount));
            dic.Add("DX_Bi", string.Format("{0}:{1}", dCount, xCount));
            dic.Add("ZhiCount", zCount);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<GDKLSF_JBZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_"))
                {
                    var order = p.Name.Replace("Red_", string.Empty);
                    lastValue = winRed.Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("HeZhi"))
                {
                    lastValue = hz;
                }
                if (p.Name.StartsWith("HeWei"))
                {
                    lastValue = hw;
                }
                return lastValue;
            });

            manager.AddGDKLSF_JBZS(entity);
        }

        /// <summary>
        /// 定位第一位走势
        /// </summary>
        private void Import__DW1(string issuseNumber, string winNumber)
        {
            var manager = new GDKLSF_Manager();
            var issuse = manager.QueryGDKLSF_DW1IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var dw = int.Parse(winRed[0]);

            var dx = string.Empty;
            if (dw >= 10)
                dx = "D";
            else
                dx = "X";

            var jo = string.Empty;
            if (dw % 2 == 1)
                jo = "J";
            else
                jo = "O";

            var zhilist = new int[] { 1, 2, 3, 5, 7, 11, 13, 17, 19 };
            var zh = string.Empty;
            if (zhilist.Contains(dw))
                zh = "Z";
            else
                zh = "H";

            var last = manager.QueryGDKLSF_DW1();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<GDKLSF_DW1>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_"))
                {
                    var order = p.Name.Replace("Red_", string.Empty);
                    lastValue = winRed.Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NODX_"))
                {
                    var order = p.Name.Replace("NODX_", string.Empty);
                    lastValue = dx == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NOJO_"))
                {
                    var order = p.Name.Replace("NOJO_", string.Empty);
                    lastValue = jo == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NOZH_"))
                {
                    var order = p.Name.Replace("NOZH_", string.Empty);
                    lastValue = zh == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu_"))
                {
                    var order = p.Name.Replace("Yu_", string.Empty);
                    lastValue = dw % 3 == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddGDKLSF_DW1(entity);
        }

        /// <summary>
        /// 定位第二位走势
        /// </summary>
        private void Import__DW2(string issuseNumber, string winNumber)
        {
            var manager = new GDKLSF_Manager();
            var issuse = manager.QueryGDKLSF_DW2IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var dw = int.Parse(winRed[1]);

            var dx = string.Empty;
            if (dw >= 10)
                dx = "D";
            else
                dx = "X";

            var jo = string.Empty;
            if (dw % 2 == 1)
                jo = "J";
            else
                jo = "O";

            var zhilist = new int[] { 1, 2, 3, 5, 7, 11, 13, 17, 19 };
            var zh = string.Empty;
            if (zhilist.Contains(dw))
                zh = "Z";
            else
                zh = "H";

            var last = manager.QueryGDKLSF_DW2();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<GDKLSF_DW2>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_"))
                {
                    var order = p.Name.Replace("Red_", string.Empty);
                    lastValue = winRed.Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NODX_"))
                {
                    var order = p.Name.Replace("NODX_", string.Empty);
                    lastValue = dx == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NOJO_"))
                {
                    var order = p.Name.Replace("NOJO_", string.Empty);
                    lastValue = jo == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NOZH_"))
                {
                    var order = p.Name.Replace("NOZH_", string.Empty);
                    lastValue = zh == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu_"))
                {
                    var order = p.Name.Replace("Yu_", string.Empty);
                    lastValue = dw % 3 == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddGDKLSF_DW2(entity);
        }

        /// <summary>
        /// 定位第三位走势
        /// </summary>
        private void Import__DW3(string issuseNumber, string winNumber)
        {
            var manager = new GDKLSF_Manager();
            var issuse = manager.QueryGDKLSF_DW3IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var dw = int.Parse(winRed[2]);

            var dx = string.Empty;
            if (dw >= 10)
                dx = "D";
            else
                dx = "X";

            var jo = string.Empty;
            if (dw % 2 == 1)
                jo = "J";
            else
                jo = "O";

            var zhilist = new int[] { 1, 2, 3, 5, 7, 11, 13, 17, 19 };
            var zh = string.Empty;
            if (zhilist.Contains(dw))
                zh = "Z";
            else
                zh = "H";

            var last = manager.QueryGDKLSF_DW3();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<GDKLSF_DW3>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_"))
                {
                    var order = p.Name.Replace("Red_", string.Empty);
                    lastValue = winRed.Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NODX_"))
                {
                    var order = p.Name.Replace("NODX_", string.Empty);
                    lastValue = dx == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NOJO_"))
                {
                    var order = p.Name.Replace("NOJO_", string.Empty);
                    lastValue = jo == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NOZH_"))
                {
                    var order = p.Name.Replace("NOZH_", string.Empty);
                    lastValue = zh == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu_"))
                {
                    var order = p.Name.Replace("Yu_", string.Empty);
                    lastValue = dw % 3 == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddGDKLSF_DW3(entity);
        }

        /// <summary>
        /// 大小
        /// </summary>
        private void Import__DX(string issuseNumber, string winNumber)
        {
            var manager = new GDKLSF_Manager();
            var issuse = manager.QueryGDKLSF_DXIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]), int.Parse(winRed[5]), int.Parse(winRed[6]), int.Parse(winRed[7]) };

            #region  小大个数
            int dCount = 0;
            int xCount = 0;
            foreach (var item in array)
            {
                if (item <= 10)
                    xCount++;
                else
                    dCount++;
            }
            #endregion

            #region 大小排位

            string winxiao1, winxiao2, winxiao3, winxiao4, winxiao5, winxiao6, winxiao7, winxiao8 = string.Empty;

            if (array[0] <= 10)
                winxiao1 = "X";
            else
                winxiao1 = "D";

            if (array[1] <= 10)
                winxiao2 = "X";
            else
                winxiao2 = "D";

            if (array[2] <= 10)
                winxiao3 = "X";
            else
                winxiao3 = "D";

            if (array[3] <= 10)
                winxiao4 = "X";
            else
                winxiao4 = "D";

            if (array[4] <= 10)
                winxiao5 = "X";
            else
                winxiao5 = "D";

            if (array[5] <= 10)
                winxiao6 = "X";
            else
                winxiao6 = "D";

            if (array[6] <= 10)
                winxiao7 = "X";
            else
                winxiao7 = "D";

            if (array[7] <= 10)
                winxiao8 = "X";
            else
                winxiao8 = "D";

            var arrayWinXiao = new string[] { winxiao1, winxiao2, winxiao3, winxiao4, winxiao5, winxiao6, winxiao7, winxiao8 };
            #endregion

            var last = manager.QueryGDKLSF_DX();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<GDKLSF_DX>(dic, (p) =>
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
                if (p.Name.StartsWith("NO8"))
                {
                    var order = p.Name.Replace("NO8", string.Empty);
                    lastValue = arrayWinXiao[7] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var bi = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(bi[0]) == dCount && int.Parse(bi[1]) == xCount ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddGDKLSF_DX(entity);
        }

        /// <summary>
        /// 奇偶
        /// </summary>
        private void Import__JO(string issuseNumber, string winNumber)
        {
            var manager = new GDKLSF_Manager();
            var issuse = manager.QueryGDKLSF_JOIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]), int.Parse(winRed[5]), int.Parse(winRed[6]), int.Parse(winRed[7]) };

            #region  小大个数
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

            #region 奇偶排位

            string winxiao1, winxiao2, winxiao3, winxiao4, winxiao5, winxiao6, winxiao7, winxiao8 = string.Empty;

            if (array[0] % 2 == 0)
                winxiao1 = "O";
            else
                winxiao1 = "J";

            if (array[1] % 2 == 0)
                winxiao2 = "O";
            else
                winxiao2 = "J";

            if (array[2] % 2 == 0)
                winxiao3 = "O";
            else
                winxiao3 = "J";

            if (array[3] % 2 == 0)
                winxiao4 = "O";
            else
                winxiao4 = "J";

            if (array[4] % 2 == 0)
                winxiao5 = "O";
            else
                winxiao5 = "J";

            if (array[5] % 2 == 0)
                winxiao6 = "O";
            else
                winxiao6 = "J";

            if (array[6] % 2 == 0)
                winxiao7 = "O";
            else
                winxiao7 = "J";

            if (array[7] % 2 == 0)
                winxiao8 = "O";
            else
                winxiao8 = "J";

            var arrayWinJo = new string[] { winxiao1, winxiao2, winxiao3, winxiao4, winxiao5, winxiao6, winxiao7, winxiao8 };
            #endregion

            var last = manager.QueryGDKLSF_JO();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<GDKLSF_JO>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1"))
                {
                    var order = p.Name.Replace("NO1", string.Empty);
                    lastValue = arrayWinJo[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2"))
                {
                    var order = p.Name.Replace("NO2", string.Empty);
                    lastValue = arrayWinJo[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3"))
                {
                    var order = p.Name.Replace("NO3", string.Empty);
                    lastValue = arrayWinJo[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO4"))
                {
                    var order = p.Name.Replace("NO4", string.Empty);
                    lastValue = arrayWinJo[3] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO5"))
                {
                    var order = p.Name.Replace("NO5", string.Empty);
                    lastValue = arrayWinJo[4] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO6"))
                {
                    var order = p.Name.Replace("NO6", string.Empty);
                    lastValue = arrayWinJo[5] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO7"))
                {
                    var order = p.Name.Replace("NO7", string.Empty);
                    lastValue = arrayWinJo[6] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO8"))
                {
                    var order = p.Name.Replace("NO8", string.Empty);
                    lastValue = arrayWinJo[7] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var bi = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(bi[0]) == jCount && int.Parse(bi[1]) == oCount ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddGDKLSF_JO(entity);
        }

        /// <summary>
        /// 质和
        /// </summary>
        private void Import__ZH(string issuseNumber, string winNumber)
        {
            var manager = new GDKLSF_Manager();
            var issuse = manager.QueryGDKLSF_ZHIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]), int.Parse(winRed[5]), int.Parse(winRed[6]), int.Parse(winRed[7]) };
            var zhilist = new int[] { 1, 2, 3, 5, 7, 11, 13, 17, 19 };

            #region  质和个数
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

            #region 质和排位

            string winxiao1, winxiao2, winxiao3, winxiao4, winxiao5, winxiao6, winxiao7, winxiao8 = string.Empty;

            if (zhilist.Contains(array[0]))
                winxiao1 = "Z";
            else
                winxiao1 = "H";

            if (zhilist.Contains(array[1]))
                winxiao2 = "Z";
            else
                winxiao2 = "H";

            if (zhilist.Contains(array[2]))
                winxiao3 = "Z";
            else
                winxiao3 = "H";

            if (zhilist.Contains(array[3]))
                winxiao4 = "Z";
            else
                winxiao4 = "H";

            if (zhilist.Contains(array[4]))
                winxiao5 = "Z";
            else
                winxiao5 = "H";

            if (zhilist.Contains(array[5]))
                winxiao6 = "Z";
            else
                winxiao6 = "H";

            if (zhilist.Contains(array[6]))
                winxiao7 = "Z";
            else
                winxiao7 = "H";

            if (zhilist.Contains(array[7]))
                winxiao8 = "Z";
            else
                winxiao8 = "H";

            var arrayWinZhi = new string[] { winxiao1, winxiao2, winxiao3, winxiao4, winxiao5, winxiao6, winxiao7, winxiao8 };
            #endregion

            var last = manager.QueryGDKLSF_ZH();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<GDKLSF_ZH>(dic, (p) =>
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
                if (p.Name.StartsWith("NO8"))
                {
                    var order = p.Name.Replace("NO8", string.Empty);
                    lastValue = arrayWinZhi[7] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var bi = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(bi[0]) == zCount && int.Parse(bi[1]) == hCount ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddGDKLSF_ZH(entity);
        }
        #endregion

        /// <summary>
        /// 查询开奖数据
        /// </summary>
        public GameWinNumber_InfoCollection QueryGDKLSF_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new GDKLSF_GameWinNumberManager().QueryGDKLSF_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<GDKLSF_GameWinNumber>, GDKLSF_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryGDKLSF_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new GDKLSF_GameWinNumberManager().QueryGDKLSF_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<GDKLSF_GameWinNumber>, GDKLSF_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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


        public GameWinNumber_InfoCollection QueryGDKLSF_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new GDKLSF_GameWinNumberManager().QueryGDKLSF_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<GDKLSF_GameWinNumber>, GDKLSF_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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


        public GameWinNumber_InfoCollection QueryGDKLSF_GameWinNumberDesc(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new GDKLSF_GameWinNumberManager().QueryGDKLSF_GameWinNumberDesc(startTime, endTime, pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<GDKLSF_GameWinNumber>, GDKLSF_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
            var manager = new GDKLSF_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<GDKLSF_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }
    }
}
