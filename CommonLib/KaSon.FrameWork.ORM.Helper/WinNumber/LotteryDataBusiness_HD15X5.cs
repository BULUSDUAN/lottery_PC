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
    public class LotteryDataBusiness_HD15X5 : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get
            {
                return "HD15X5";
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

                this.ClearGameChartCache("QueryHD15X5_HZ");
                this.ClearGameChartCache("QueryHD15X5_JBZS");
                this.ClearGameChartCache("QueryHD15X5_CH");
                this.ClearGameChartCache("QueryHD15X5_DX");
                this.ClearGameChartCache("QueryHD15X5_JO");
                this.ClearGameChartCache("QueryHD15X5_JBZS");
                this.ClearGameChartCache("QueryHD15X5_LH");
                this.ClearNewWinNumberCache("QueryHD15X5_GameWinNumber");


                Import_HZ(issuseNumber, winNumber);
                Import_JBZS(issuseNumber, winNumber);
                AddHD15X5_CH(issuseNumber, winNumber);
                AddHD15X5_LH(issuseNumber, winNumber);
                Import_DX(issuseNumber, winNumber);
                Import_JO(issuseNumber, winNumber);
                Import_ZH(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }

        #region 前台查询数据

        /// <summary>
        ///查询和值走势列表按时间倒叙 
        /// </summary>
        public HD15X5_HZ_InfoCollection QueryHD15X5_HZ(int length)
        {
            HD15X5_HZ_InfoCollection Collection = new HD15X5_HZ_InfoCollection();
            var list = this.QueryGameChart<HD15X5_HZ_Info>(string.Format("QueryHD15X5_HZ_{0}", length), () =>
           {
               var infoList = new List<HD15X5_HZ_Info>();
               var entityList = new HD15X5_Manager().QueryHD15X5_HZ(length);

              ObjectConvert.ConvertEntityListToInfoList<List<HD15X5_HZ>, HD15X5_HZ, List<HD15X5_HZ_Info>, HD15X5_HZ_Info>(entityList, ref infoList,
                   () => { return new HD15X5_HZ_Info(); });
               return infoList;
           });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询基本走势列表按时间倒叙 
        /// </summary>
        public HD15X5_JBZS_InfoCollection QueryHD15X5_JBZS(int length)
        {
            HD15X5_JBZS_InfoCollection Collection = new HD15X5_JBZS_InfoCollection();
            var list = this.QueryGameChart<HD15X5_JBZS_Info>(string.Format("QueryHD15X5_JBZS_{0}", length), () =>
            {
                var infoList = new List<HD15X5_JBZS_Info>();
                var entityList = new HD15X5_Manager().QueryHD15X5_JBZS(length);

               ObjectConvert.ConvertEntityListToInfoList<List<HD15X5_JBZS>, HD15X5_JBZS, List<HD15X5_JBZS_Info>, HD15X5_JBZS_Info>(entityList, ref infoList,
                    () => { return new HD15X5_JBZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        public HD15X5_CH_InfoCollection QueryHD15X5_CH(int index)
        {
            HD15X5_CH_InfoCollection Collection = new HD15X5_CH_InfoCollection();
            var list = this.QueryGameChart<HD15X5_CH_Info>(string.Format("QueryHD15X5_CH_{0}", index), () =>
            {
                var infoList = new List<HD15X5_CH_Info>();
                var entityList = new HD15X5_Manager().QueryHD15X5_CH(index);

               ObjectConvert.ConvertEntityListToInfoList<List<HD15X5_CH>, HD15X5_CH, List<HD15X5_CH_Info>, HD15X5_CH_Info>(entityList, ref infoList,
                    () => { return new HD15X5_CH_Info(); },
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
        ///查询大小走势列表按时间倒叙 
        /// </summary>
        public HD15X5_DX_InfoCollection QueryHD15X5_DX(int length)
        {
            HD15X5_DX_InfoCollection Collection = new HD15X5_DX_InfoCollection();
            var list = this.QueryGameChart<HD15X5_DX_Info>(string.Format("QueryHD15X5_DX_{0}", length), () =>
            {
                var infoList = new List<HD15X5_DX_Info>();
                var entityList = new HD15X5_Manager().QueryHD15X5_DX(length);

               ObjectConvert.ConvertEntityListToInfoList<List<HD15X5_DX>, HD15X5_DX, List<HD15X5_DX_Info>, HD15X5_DX_Info>(entityList, ref infoList,
                    () => { return new HD15X5_DX_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询奇偶走势列表按时间倒叙 
        /// </summary>
        public HD15X5_JO_InfoCollection QueryHD15X5_JO(int length)
        {
            HD15X5_JO_InfoCollection Collection = new HD15X5_JO_InfoCollection();
            var list = this.QueryGameChart<HD15X5_JO_Info>(string.Format("QueryHD15X5_JO_{0}", length), () =>
            {
                var infoList = new List<HD15X5_JO_Info>();
                var entityList = new HD15X5_Manager().QueryHD15X5_JO(length);

               ObjectConvert.ConvertEntityListToInfoList<List<HD15X5_JO>, HD15X5_JO, List<HD15X5_JO_Info>, HD15X5_JO_Info>(entityList, ref infoList,
                    () => { return new HD15X5_JO_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询质和走势列表按时间倒叙 
        /// </summary>
        public HD15X5_ZH_InfoCollection QueryHD15X5_ZH(int length)
        {
            HD15X5_ZH_InfoCollection Collection = new HD15X5_ZH_InfoCollection();
            var list = this.QueryGameChart<HD15X5_ZH_Info>(string.Format("QueryHD15X5_ZH_{0}", length), () =>
            {
                var infoList = new List<HD15X5_ZH_Info>();
                var entityList = new HD15X5_Manager().QueryHD15X5_ZH(length);

               ObjectConvert.ConvertEntityListToInfoList<List<HD15X5_ZH>, HD15X5_ZH, List<HD15X5_ZH_Info>, HD15X5_ZH_Info>(entityList, ref infoList,
                    () => { return new HD15X5_ZH_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }
        public HD15X5_LH_InfoCollection QueryHD15X5_LH(int index)
        {
            HD15X5_LH_InfoCollection Collection = new HD15X5_LH_InfoCollection();
            var list = this.QueryGameChart<HD15X5_LH_Info>(string.Format("QueryHD15X5_LH_{0}", index), () =>
            {
                var infoList = new List<HD15X5_LH_Info>();
                var entityList = new HD15X5_Manager().QueryHD15X5_LH(index);

               ObjectConvert.ConvertEntityListToInfoList<List<HD15X5_LH>, HD15X5_LH, List<HD15X5_LH_Info>, HD15X5_LH_Info>(entityList, ref infoList,
                    () => { return new HD15X5_LH_Info(); },
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

        #region 生成走势数据

        public void Add_GameWinNumber(string issuseNumber, string winNumber)
        {
            new KJGameIssuseBusiness().IssusePrize(this.CurrentGameCode, issuseNumber, winNumber);
            var manager = new HD15X5_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddHD15X5_GameWinNumber(new HD15X5_GameWinNumber
            {
                GameCode = this.CurrentGameCode,
                IssuseNumber = issuseNumber,
                WinNumber = winNumber,
                CreateTime = DateTime.Now,
            });
        }

        /// <summary>
        /// 和值走势
        /// </summary>
        private void Import_HZ(string issuseNumber, string winNumber)
        {
            var manager = new HD15X5_Manager();
            var issuse = manager.QueryHD15X5_HZIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var winnumber = new int[] { int.Parse(winArray[0]), int.Parse(winArray[1]), int.Parse(winArray[2]), int.Parse(winArray[3]), int.Parse(winArray[4]) };
            var hz = winnumber[0] + winnumber[1] + winnumber[2] + winnumber[3] + winnumber[4];
            var hw = hz % 10;

            var last = manager.QueryHD15X5_HZ();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<HD15X5_HZ>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("HeZhi"))
                {
                    lastValue = hz;
                }
                if (p.Name.StartsWith("HeWei"))
                {
                    lastValue = hw;
                }
                if (p.Name.StartsWith("HZ_"))
                {
                    var order = p.Name.Replace("HZ_", string.Empty);
                    var hzfb = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = hz >= int.Parse(hzfb[0]) && hz <= int.Parse(hzfb[1]) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddHD15X5_HZ(entity);
        }

        /// <summary>
        /// 基本走势
        /// </summary>
        private void Import_JBZS(string issuseNumber, string winNumber)
        {
            var manager = new HD15X5_Manager();
            var issuse = manager.QueryHD15X5_JBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var winnumber = new int[] { int.Parse(winArray[0]), int.Parse(winArray[1]), int.Parse(winArray[2]), int.Parse(winArray[3]), int.Parse(winArray[4]) };
            var hz = winnumber[0] + winnumber[1] + winnumber[2] + winnumber[3] + winnumber[4];
            var hw = hz % 10;

            #region  小大个数
            int dCount = 0;
            int xCount = 0;
            foreach (var item in winnumber)
            {
                if (item <= 7)
                    xCount++;
                else
                    dCount++;
            }
            #endregion

            #region  奇偶排位
            int jCount = 0;
            int oCount = 0;
            foreach (var item in winnumber)
            {
                if (item % 2 == 0)
                    oCount++;
                else
                    jCount++;
            }
            #endregion

            #region  质和排位
            int zCount = 0;
            int hCount = 0;
            var zhilist = new int[] { 1, 2, 3, 5, 7, 11, 13 };

            foreach (var item in winnumber)
            {
                if (zhilist.Contains(item))
                    zCount++;
                else
                    hCount++;
            }

            #endregion

            var last = manager.QueryHD15X5_JBZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("DaXiaobi", string.Format("{0}:{1}", dCount, xCount));
            dic.Add("Jobi", string.Format("{0}:{1}", jCount, oCount));
            dic.Add("ZHbi", string.Format("{0}:{1}", zCount, hCount));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<HD15X5_JBZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red"))
                {
                    var order = p.Name.Replace("Red", string.Empty);
                    lastValue = winnumber.Contains(int.Parse(order)) ? 0 : lastValue;
                }
                if (p.Name == "HeZhi")
                {
                    lastValue = hz;
                }
                if (p.Name == "HW")
                {
                    lastValue = hw;
                }
                return lastValue;
            });

            manager.AddHD15X5_JBZS(entity);
        }

        /// <summary>
        /// 添加重号走势
        /// </summary>
        private void AddHD15X5_CH(string issuseNumber, string winNumber)
        {
            var manager = new HD15X5_Manager();
            var issuse = manager.QueryHD15X5_CHIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var HeZhi = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]) + int.Parse(winRed[3]) + int.Parse(winRed[4]);
            var last = manager.QueryLastHD15X5_CH();
            int chonghao = 0;

            for (int i = 0; i < (last == null ? 0 : last.WinNumber.Split(',').Length); i++)
            {
                if (last.WinNumber.Contains(winRed[i]))
                {
                    chonghao++;
                }
            }

            string DX_Proportion = string.Empty;
            string JO_Proportion = string.Empty;
            int d = 0;
            int x = 0;
            int j = 0;
            int o = 0;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) > 7)
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
            dic.Add("Duplicate", chonghao);
            dic.Add("HeZhi", HeZhi);
            dic.Add("JO_Proportion", JO_Proportion);
            dic.Add("DX_Proportion", DX_Proportion);
            var entity = this.CreateNewEntity<HD15X5_CH>(dic, (p) =>
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

            manager.AddHD15X5_CH(entity);
        }
        /// <summary>
        /// 添加连号走势
        /// </summary>
        private void AddHD15X5_LH(string issuseNumber, string winNumber)
        {
            var manager = new HD15X5_Manager();
            var issuse = manager.QueryHD15X5_LHIssuseNumber(issuseNumber);
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
                XLH += " 06 07";
            }
            if (winRed.Contains("07") && winRed.Contains("08"))
            {
                GHao++;
                XLH += " 07 08";
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
            if (winRed.Contains("11") && winRed.Contains("12"))
            {
                GHao++;
                DLH += " 11 12";
            }
            if (winRed.Contains("12") && winRed.Contains("13"))
            {
                GHao++;
                DLH += " 12 13";
            }
            if (winRed.Contains("13") && winRed.Contains("14"))
            {
                GHao++;
                DLH += " 13 14";
            }
            if (winRed.Contains("14") && winRed.Contains("15"))
            {
                GHao++;
                DLH += " 14 15";
            }
            var last = manager.QueryLastHD15X5_LH();
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
            dic.Add("GHao", GHao);
            dic.Add("DLH", DLH);
            dic.Add("XLH", XLH);
            var entity = this.CreateNewEntity<HD15X5_LH>(dic, (p) =>
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

            manager.AddHD15X5_LH(entity);
        }
        /// <summary>
        ///  大小走势
        /// </summary>
        private void Import_DX(string issuseNumber, string winNumber)
        {
            var manager = new HD15X5_Manager();
            var issuse = manager.QueryHD15X5_DXIssuseNumber(issuseNumber);
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
            if (array[0] <= 7)
                winxiao1 = "X";
            else
                winxiao1 = "D";

            if (array[1] <= 7)
                winxiao2 = "X";
            else
                winxiao2 = "D";

            if (array[2] <= 7)
                winxiao3 = "X";
            else
                winxiao3 = "D";

            if (array[3] <= 7)
                winxiao4 = "X";
            else
                winxiao4 = "D";

            if (array[4] <= 7)
                winxiao5 = "X";
            else
                winxiao5 = "D";

            var arrayWinXiao = new string[] { winxiao1, winxiao2, winxiao3, winxiao4, winxiao5 };
            var XiaoType = string.Join("", arrayWinXiao);
            int DaCount = XiaoType.Count(p => p == 'D');
            #endregion

            var last = manager.QueryHD15X5_DX();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("DXqualifying", string.Join("", arrayWinXiao));
            dic.Add("DaoXiaoBi", string.Format("{0}:{1}", DaCount, 5 - DaCount));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<HD15X5_DX>(dic, (p) =>
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

            manager.AddHD15X5_DX(entity);
        }

        /// <summary>
        ///  奇偶走势
        /// </summary>
        private void Import_JO(string issuseNumber, string winNumber)
        {
            var manager = new HD15X5_Manager();
            var issuse = manager.QueryHD15X5_JOIssuseNumber(issuseNumber);
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

            var last = manager.QueryHD15X5_JO();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("JOqualifying", string.Join("", arraywinji));
            dic.Add("JiOuBi", string.Format("{0}:{1}", JiCount, 5 - JiCount));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<HD15X5_JO>(dic, (p) =>
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

            manager.AddHD15X5_JO(entity);

        }

        /// <summary>
        ///  质和走势
        /// </summary>
        private void Import_ZH(string issuseNumber, string winNumber)
        {
            var manager = new HD15X5_Manager();
            var issuse = manager.QueryHD15X5_ZHIssuseNumber(issuseNumber);
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

            var last = manager.QueryHD15X5_ZH();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("ZHqualifying", string.Join("", arraywinzhi));
            dic.Add("ZhiHeBi", string.Format("{0}:{1}", ZhiCount, 5 - ZhiCount));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<HD15X5_ZH>(dic, (p) =>
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

            manager.AddHD15X5_ZH(entity);

        }

        #endregion

        /// <summary>
        /// 查询开奖数据
        /// </summary>
        public GameWinNumber_InfoCollection QueryHD15X5_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new HD15X5_GameWinNumberManager().QueryHD15X5_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<HD15X5_GameWinNumber>, HD15X5_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryHD15X5_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new HD15X5_GameWinNumberManager().QueryHD15X5_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<HD15X5_GameWinNumber>, HD15X5_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
            var manager = new HD15X5_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<HD15X5_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }
    }
}
