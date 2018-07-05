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
<<<<<<< HEAD
using KaSon.FrameWork.Analyzer.AnalyzerFactory;
=======
using KaSon.FrameWork.Common.Utilities;
>>>>>>> a7171008b4bea1dab11582695738b3dd1fe77dcf

namespace KaSon.FrameWork.ORM.Helper.WinNumber
{
    /// <summary>
    /// 山东快乐扑克3
    /// </summary>
    public class LotteryDataBusiness_SDKLPK3 : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get { return "SDKLPK3"; }
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

                this.ClearGameChartCache("QuerySDKLPK3_JBZS_Info");
                this.ClearGameChartCache("QuerySDKLPK3_ZHXZS_Info");
                this.ClearGameChartCache("QuerySDKLPK3_HSZS_Info");
                this.ClearGameChartCache("QuerySDKLPK3_DXZS_Info");
                this.ClearGameChartCache("QuerySDKLPK3_JOZS_Info");
                this.ClearGameChartCache("QuerySDKLPK3_ZHZS_Info");
                this.ClearGameChartCache("QuerySDKLPK3_C3YZS_Info");
                this.ClearGameChartCache("QuerySDKLPK3_HZZS_Info");
                this.ClearGameChartCache("QuerySDKLPK3_LXZS_Info");
                this.ClearNewWinNumberCache("QuerySDKLPK3_GameWinNumber");

                Import_JBZS(issuseNumber, winNumber);
                Import_ZHXZS(issuseNumber, winNumber);
                Import_HSZS(issuseNumber, winNumber);
                Import_DXZS(issuseNumber, winNumber);
                Import_JOZS(issuseNumber, winNumber);
                Import_ZHZS(issuseNumber, winNumber);
                Import_C3YZS(issuseNumber, winNumber);
                Import_HZZS(issuseNumber, winNumber);
                Import_LXZS(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }

        #region 前台查询数据

        /// <summary>
        /// 查询基本走势列表按时间倒序
        /// </summary>
        public SDKLPK3_JBZS_InfoCollection QuerySDKLPK3_JBZS_Info(int length)
        {
            SDKLPK3_JBZS_InfoCollection Collection = new SDKLPK3_JBZS_InfoCollection();
            var list = this.QueryGameChart<SDKLPK3_JBZS_Info>(string.Format("QuerySDKLPK3_JBZS_Info_{0}", length), () =>
            {
                var infoList = new List<SDKLPK3_JBZS_Info>();
                var entityList = new SDKLPK3_Manager().QuerySDKLPK3_JBZS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDKLPK3_JBZS>, SDKLPK3_JBZS, List<SDKLPK3_JBZS_Info>, SDKLPK3_JBZS_Info>(entityList, ref infoList,
                    () => { return new SDKLPK3_JBZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        /// 查询组选走势列表按时间倒序
        /// </summary>
        public SDKLPK3_ZHXZS_InfoCollection QuerySDKLPK3_ZHXZS_Info(int length)
        {
            SDKLPK3_ZHXZS_InfoCollection Collection = new SDKLPK3_ZHXZS_InfoCollection();
            var list = this.QueryGameChart<SDKLPK3_ZHXZS_Info>(string.Format("QuerySDKLPK3_ZHXZS_Info_{0}", length), () =>
            {
                var infoList = new List<SDKLPK3_ZHXZS_Info>();
                var entityList = new SDKLPK3_Manager().QuerySDKLPK3_ZHXZS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDKLPK3_ZHXZS>, SDKLPK3_ZHXZS, List<SDKLPK3_ZHXZS_Info>, SDKLPK3_ZHXZS_Info>(entityList, ref infoList,
                    () => { return new SDKLPK3_ZHXZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        /// 查询花色走势列表按时间倒序
        /// </summary>
        public SDKLPK3_HSZS_InfoCollection QuerySDKLPK3_HSZS_Info(int length)
        {
            SDKLPK3_HSZS_InfoCollection Collection = new SDKLPK3_HSZS_InfoCollection();
            var list = this.QueryGameChart<SDKLPK3_HSZS_Info>(string.Format("QuerySDKLPK3_HSZS_Info_{0}", length), () =>
            {
                var infoList = new List<SDKLPK3_HSZS_Info>();
                var entityList = new SDKLPK3_Manager().QuerySDKLPK3_HSZS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDKLPK3_HSZS>, SDKLPK3_HSZS, List<SDKLPK3_HSZS_Info>, SDKLPK3_HSZS_Info>(entityList, ref infoList,
                    () => { return new SDKLPK3_HSZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        /// 查询大小走势列表按时间倒序
        /// </summary>
        public SDKLPK3_DXZS_InfoCollection QuerySDKLPK3_DXZS_Info(int length)
        {
            SDKLPK3_DXZS_InfoCollection Collection = new SDKLPK3_DXZS_InfoCollection();
            var list = this.QueryGameChart<SDKLPK3_DXZS_Info>(string.Format("QuerySDKLPK3_DXZS_Info_{0}", length), () =>
            {
                var infoList = new List<SDKLPK3_DXZS_Info>();
                var entityList = new SDKLPK3_Manager().QuerySDKLPK3_DXZS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDKLPK3_DXZS>, SDKLPK3_DXZS, List<SDKLPK3_DXZS_Info>, SDKLPK3_DXZS_Info>(entityList, ref infoList,
                    () => { return new SDKLPK3_DXZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        /// 查询奇偶走势列表按时间倒序
        /// </summary>
        public SDKLPK3_JOZS_InfoCollection QuerySDKLPK3_JOZS_Info(int length)
        {
            SDKLPK3_JOZS_InfoCollection Collection = new SDKLPK3_JOZS_InfoCollection();
            var list = this.QueryGameChart<SDKLPK3_JOZS_Info>(string.Format("QuerySDKLPK3_JOZS_Info_{0}", length), () =>
            {
                var infoList = new List<SDKLPK3_JOZS_Info>();
                var entityList = new SDKLPK3_Manager().QuerySDKLPK3_JOZS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDKLPK3_JOZS>, SDKLPK3_JOZS, List<SDKLPK3_JOZS_Info>, SDKLPK3_JOZS_Info>(entityList, ref infoList,
                    () => { return new SDKLPK3_JOZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        /// 查询质合走势列表按时间倒序
        /// </summary>
        public SDKLPK3_ZHZS_InfoCollection QuerySDKLPK3_ZHZS_Info(int length)
        {
            SDKLPK3_ZHZS_InfoCollection Collection = new SDKLPK3_ZHZS_InfoCollection();
            var list = this.QueryGameChart<SDKLPK3_ZHZS_Info>(string.Format("QuerySDKLPK3_ZHZS_Info_{0}", length), () =>
            {
                var infoList = new List<SDKLPK3_ZHZS_Info>();
                var entityList = new SDKLPK3_Manager().QuerySDKLPK3_ZHZS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDKLPK3_ZHZS>, SDKLPK3_ZHZS, List<SDKLPK3_ZHZS_Info>, SDKLPK3_ZHZS_Info>(entityList, ref infoList,
                    () => { return new SDKLPK3_ZHZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        /// 查询除3余走势列表按时间倒序
        /// </summary>
        public SDKLPK3_C3YZS_InfoCollection QuerySDKLPK3_C3YZS_Info(int length)
        {
            SDKLPK3_C3YZS_InfoCollection Collection = new SDKLPK3_C3YZS_InfoCollection();
            var list = this.QueryGameChart<SDKLPK3_C3YZS_Info>(string.Format("QuerySDKLPK3_C3YZS_Info_{0}", length), () =>
            {
                var infoList = new List<SDKLPK3_C3YZS_Info>();
                var entityList = new SDKLPK3_Manager().QuerySDKLPK3_C3YZS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDKLPK3_C3YZS>, SDKLPK3_C3YZS, List<SDKLPK3_C3YZS_Info>, SDKLPK3_C3YZS_Info>(entityList, ref infoList,
                    () => { return new SDKLPK3_C3YZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        /// 查询和值走势列表按时间倒序
        /// </summary>
        public SDKLPK3_HZZS_InfoCollection QuerySDKLPK3_HZZS_Info(int length)
        {
            SDKLPK3_HZZS_InfoCollection Collection = new SDKLPK3_HZZS_InfoCollection();
            var list = this.QueryGameChart<SDKLPK3_HZZS_Info>(string.Format("QuerySDKLPK3_HZZS_Info_{0}", length), () =>
            {
                var infoList = new List<SDKLPK3_HZZS_Info>();
                var entityList = new SDKLPK3_Manager().QuerySDKLPK3_HZZS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDKLPK3_HZZS>, SDKLPK3_HZZS, List<SDKLPK3_HZZS_Info>, SDKLPK3_HZZS_Info>(entityList, ref infoList,
                    () => { return new SDKLPK3_HZZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        /// 查询类型走势列表按时间倒序
        /// </summary>
        public SDKLPK3_LXZS_InfoCollection QuerySDKLPK3_LXZS_Info(int length)
        {
            SDKLPK3_LXZS_InfoCollection Collection = new SDKLPK3_LXZS_InfoCollection();
            var list = this.QueryGameChart<SDKLPK3_LXZS_Info>(string.Format("QuerySDKLPK3_LXZS_Info_{0}", length), () =>
            {
                var infoList = new List<SDKLPK3_LXZS_Info>();
                var entityList = new SDKLPK3_Manager().QuerySDKLPK3_LXZS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<SDKLPK3_LXZS>, SDKLPK3_LXZS, List<SDKLPK3_LXZS_Info>, SDKLPK3_LXZS_Info>(entityList, ref infoList,
                    () => { return new SDKLPK3_LXZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        #endregion

        #region 生成走势图

        /// <summary>
        /// 基本走势
        /// </summary>
        private void Import_JBZS(string issuseNumber, string winNumber)
        {
            var manager = new SDKLPK3_Manager();
            var issuse = manager.QuerySDKLPK3_JBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var numberArray = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var bonusList = new List<string>();
            for (int i = 0; i < numberArray.Length; i++)
            {
                var num = numberArray[i];
                bonusList.Add(string.Format("D{0}_{1}", i + 1, int.Parse(num.Substring(1))));
            }

            var last = manager.QuerySDKLPK3_JBZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<SDKLPK3_JBZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                if (bonusList.Contains(p.Name))
                {
                    //开出号码
                    return 0;
                }
                return lastValue;
            });
            manager.AddSDKLPK3_JBZS(entity);
        }

        /// <summary>
        /// 组选走势
        /// </summary>
        private void Import_ZHXZS(string issuseNumber, string winNumber)
        {
            var manager = new SDKLPK3_Manager();
            var issuse = manager.QuerySDKLPK3_ZHXZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var numberArray = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var shu = new List<int>();
            var bonusList = new List<string>();
            for (int i = 0; i < numberArray.Length; i++)
            {
                var num = numberArray[i];
                var n = int.Parse(num.Substring(1));
                shu.Add(n);
            }
            //数字
            var shuStrList = shu.Select(p => string.Format("S_{0}", p)).ToList();
            //大小
            var da = shu.Count(p => p >= 7);
            var xiao = shu.Count(p => p < 7);
            bonusList.Add(string.Format("DX_{0}{1}", da, xiao));

            //奇偶
            var ou = shu.Count(p => p % 2 == 0);
            var ji = 3 - ou;
            bonusList.Add(string.Format("JO_{0}{1}", ji, ou));

            //质合
            var zhiArray = new int[] { 1, 2, 3, 5, 7, 11, 13 };
            var heArray = new int[] { 4, 6, 8, 9, 10, 12 };
            var zhi = shu.Count(p => zhiArray.Contains(p));
            var he = shu.Count(p => heArray.Contains(p));
            bonusList.Add(string.Format("ZH_{0}{1}", zhi, he));

            var last = manager.QuerySDKLPK3_ZHXZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<SDKLPK3_ZHXZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);

                //数字判断
                if (shuStrList.Contains(p.Name))
                    return 0;

                //状态判断
                if (bonusList.Contains(p.Name))
                    return 0;

                return lastValue;
            });
            manager.AddSDKLPK3_ZHXZS(entity);
        }

        /// <summary>
        /// 花色
        /// </summary>
        private void Import_HSZS(string issuseNumber, string winNumber)
        {
            var manager = new SDKLPK3_Manager();
            var issuse = manager.QuerySDKLPK3_HSZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var numberArray = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var shu = new List<int>();
            var hua = new List<int>();
            var bonusList = new List<string>();
            for (int i = 0; i < numberArray.Length; i++)
            {
                var num = numberArray[i];
                //花色
                var h = int.Parse(num.Substring(0, 1));
                switch (h)
                {
                    case 1:
                        hua.Add(1);
                        bonusList.Add(string.Format("D{0}_1", i + 1));
                        break;
                    case 2:
                        hua.Add(2);
                        bonusList.Add(string.Format("D{0}_2", i + 1));
                        break;
                    case 3:
                        hua.Add(3);
                        bonusList.Add(string.Format("D{0}_3", i + 1));
                        break;
                    case 4:
                        hua.Add(4);
                        bonusList.Add(string.Format("D{0}_4", i + 1));
                        break;
                    default:
                        break;
                }

                var n = int.Parse(num.Substring(1));
                shu.Add(n);
            }

            //判断是不是同花
            var huaCount = hua.Distinct().Count();
            if (huaCount == 1)
            {
                bonusList.Add("TH");
            }

            //判断是不是顺子
            var shuArray = shu.OrderBy(p => p).ToArray();
            if ((shuArray[0] == 1 && shuArray[1] == 12 && shuArray[2] == 13)
                || (shuArray[0] + 1 == shuArray[1] && shuArray[1] + 1 == shuArray[2]))
            {
                bonusList.Add("SZ");
            }

            //同花顺
            if (bonusList.Contains("TH") && bonusList.Contains("SZ"))
            {
                bonusList.Remove("TH");
                bonusList.Remove("SZ");
                bonusList.Add("THS");
            }

            var last = manager.QuerySDKLPK3_HSZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<SDKLPK3_HSZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);

                //状态判断
                if (bonusList.Contains(p.Name))
                    return 0;

                return lastValue;
            });
            manager.AddSDKLPK3_HSZS(entity);
        }

        /// <summary>
        /// 大小
        /// </summary>
        private void Import_DXZS(string issuseNumber, string winNumber)
        {
            var manager = new SDKLPK3_Manager();
            var issuse = manager.QuerySDKLPK3_DXZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var numberArray = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var bonusList = new List<string>();
            var dxList = new List<string>();
            for (int i = 0; i < numberArray.Length; i++)
            {
                var num = numberArray[i];
                var shu = int.Parse(num.Substring(1));
                var dx = shu >= 7 ? "D" : "X";
                dxList.Add(dx);
                bonusList.Add(string.Format("D{0}_{1}", i + 1, dx));
            }
            //大小比
            if (dxList.Count(p => p == "D") == 3)
                bonusList.Add("DXB_30");
            if (dxList.Count(p => p == "D") == 2)
                bonusList.Add("DXB_21");
            if (dxList.Count(p => p == "D") == 1)
                bonusList.Add("DXB_12");
            if (dxList.Count(p => p == "D") == 0)
                bonusList.Add("DXB_03");

            //大小状态
            var dxStr = string.Join("", dxList.ToArray());
            bonusList.Add(dxStr);

            var last = manager.QuerySDKLPK3_DXZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<SDKLPK3_DXZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);

                //状态判断
                if (bonusList.Contains(p.Name))
                    return 0;

                return lastValue;
            });
            manager.AddSDKLPK3_DXZS(entity);
        }

        /// <summary>
        /// 奇偶
        /// </summary>
        private void Import_JOZS(string issuseNumber, string winNumber)
        {
            var manager = new SDKLPK3_Manager();
            var issuse = manager.QuerySDKLPK3_JOZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var numberArray = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var bonusList = new List<string>();
            var joList = new List<string>();
            for (int i = 0; i < numberArray.Length; i++)
            {
                var num = numberArray[i];
                var shu = int.Parse(num.Substring(1));
                var jo = shu % 2 == 0 ? "O" : "J";
                joList.Add(jo);
                bonusList.Add(string.Format("D{0}_{1}", i + 1, jo));
            }

            //奇偶比
            if (joList.Count(p => p == "J") == 3)
                bonusList.Add("JOB_30");
            if (joList.Count(p => p == "J") == 2)
                bonusList.Add("JOB_21");
            if (joList.Count(p => p == "J") == 1)
                bonusList.Add("JOB_12");
            if (joList.Count(p => p == "J") == 0)
                bonusList.Add("JOB_03");

            //奇偶状态
            var dxStr = string.Join("", joList.ToArray());
            bonusList.Add(dxStr);

            var last = manager.QuerySDKLPK3_JOZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<SDKLPK3_JOZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);

                //状态判断
                if (bonusList.Contains(p.Name))
                    return 0;

                return lastValue;
            });
            manager.AddSDKLPK3_JOZS(entity);
        }

        /// <summary>
        /// 质合
        /// </summary>
        private void Import_ZHZS(string issuseNumber, string winNumber)
        {
            var manager = new SDKLPK3_Manager();
            var issuse = manager.QuerySDKLPK3_ZHZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var numberArray = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var bonusList = new List<string>();
            var zhList = new List<string>();
            var zhiArray = new int[] { 1, 2, 3, 5, 7, 11, 13 };
            //var heArray = new int[] { 4, 6, 8, 9, 10, 12 };
            for (int i = 0; i < numberArray.Length; i++)
            {
                var num = numberArray[i];
                var shu = int.Parse(num.Substring(1));
                var zh = zhiArray.Contains(shu) ? "Z" : "H";
                zhList.Add(zh);
                bonusList.Add(string.Format("D{0}_{1}", i + 1, zh));
            }

            //质合比
            if (zhList.Count(p => p == "Z") == 3)
                bonusList.Add("ZHB_30");
            if (zhList.Count(p => p == "Z") == 2)
                bonusList.Add("ZHB_21");
            if (zhList.Count(p => p == "Z") == 1)
                bonusList.Add("ZHB_12");
            if (zhList.Count(p => p == "Z") == 0)
                bonusList.Add("ZHB_03");

            //质合状态
            var dxStr = string.Join("", zhList.ToArray());
            bonusList.Add(dxStr);

            var last = manager.QuerySDKLPK3_ZHZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<SDKLPK3_ZHZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);

                //状态判断
                if (bonusList.Contains(p.Name))
                    return 0;

                return lastValue;
            });
            manager.AddSDKLPK3_ZHZS(entity);
        }

        /// <summary>
        /// 除3余
        /// </summary>
        private void Import_C3YZS(string issuseNumber, string winNumber)
        {
            var manager = new SDKLPK3_Manager();
            var issuse = manager.QuerySDKLPK3_C3YZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var numberArray = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var bonusList = new List<string>();
            var yList = new List<int>();
            for (int i = 0; i < numberArray.Length; i++)
            {
                var num = numberArray[i];
                var shu = int.Parse(num.Substring(1));
                var y = shu % 3;
                yList.Add(y);
                bonusList.Add(string.Format("D{0}_{1}", i + 1, y));
            }

            //余0 1 2个数
            bonusList.Add(string.Format("Y0_{0}", yList.Count(p => p == 0)));
            bonusList.Add(string.Format("Y1_{0}", yList.Count(p => p == 1)));
            bonusList.Add(string.Format("Y2_{0}", yList.Count(p => p == 2)));

            var last = manager.QuerySDKLPK3_C3YZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("C3YB", string.Join(":", yList.ToArray()));

            var entity = this.CreateNewEntity<SDKLPK3_C3YZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);

                //状态判断
                if (bonusList.Contains(p.Name))
                    return 0;

                return lastValue;
            });
            manager.AddSDKLPK3_C3YZS(entity);
        }

        /// <summary>
        /// 和值
        /// </summary>
        private void Import_HZZS(string issuseNumber, string winNumber)
        {
            var manager = new SDKLPK3_Manager();
            var issuse = manager.QuerySDKLPK3_HZZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var numberArray = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var bonusList = new List<string>();
            var sum = 0;
            for (int i = 0; i < numberArray.Length; i++)
            {
                var num = numberArray[i];
                var shu = int.Parse(num.Substring(1));
                sum += shu;
            }
            //和值
            bonusList.Add(string.Format("HZ_{0}", sum));

            var last = manager.QuerySDKLPK3_HZZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("HZ", sum);

            var entity = this.CreateNewEntity<SDKLPK3_HZZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);

                //状态判断
                if (bonusList.Contains(p.Name))
                    return 0;

                return lastValue;
            });
            manager.AddSDKLPK3_HZZS(entity);
        }

        /// <summary>
        /// 类型
        /// </summary>
        private void Import_LXZS(string issuseNumber, string winNumber)
        {
            var manager = new SDKLPK3_Manager();
            var issuse = manager.QuerySDKLPK3_LXZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var numberArray = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var shu = new List<int>();
            var hua = new List<int>();
            var bonusList = new List<string>();
            for (int i = 0; i < numberArray.Length; i++)
            {
                var num = numberArray[i];
                var n = int.Parse(num.Substring(1));
                shu.Add(n);
                bonusList.Add(string.Format("S_{0}", n));
                //花色
                var h = int.Parse(num.Substring(0, 1));
                switch (h)
                {
                    case 1:
                        hua.Add(1);
                        break;
                    case 2:
                        hua.Add(2);
                        break;
                    case 3:
                        hua.Add(3);
                        break;
                    case 4:
                        hua.Add(4);
                        break;
                    default:
                        break;
                }
            }
            //判断是不是同花
            if (hua.Distinct().Count() == 1)
            {
                bonusList.Add("TH");
            }

            //判断是不是顺子
            var shuArray = shu.OrderBy(p => p).ToArray();
            if ((shuArray[0] == 1 && shuArray[1] == 12 && shuArray[2] == 13)
                || (shuArray[0] + 1 == shuArray[1] && shuArray[1] + 1 == shuArray[2]))
            {
                bonusList.Add("SZ");
            }
            //判断是不是同花顺
            if (bonusList.Contains("TH") && bonusList.Contains("SZ"))
            {
                bonusList.Remove("TH");
                bonusList.Remove("SZ");
                bonusList.Add("THS");
            }
            //判断是不是对子
            if (shu.Distinct().Count() == 2)
            {
                bonusList.Add("DZ");
            }
            //判断是不是豹子
            if (shu.Distinct().Count() == 1)
            {
                bonusList.Add("BZ");
            }

            var last = manager.QuerySDKLPK3_LXZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<SDKLPK3_LXZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);

                //状态判断
                if (bonusList.Contains(p.Name))
                    return 0;

                return lastValue;
            });
            manager.AddSDKLPK3_LXZS(entity);
        }

        #endregion


        public void Add_GameWinNumber(string issuseNumber, string winNumber)
        {
            new KJGameIssuseBusiness().IssusePrize(this.CurrentGameCode, issuseNumber, winNumber);
            var manager = new SDKLPK3_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddSDKLPK3_GameWinNumber(new SDKLPK3_GameWinNumber
            {
                GameCode = this.CurrentGameCode,
                IssuseNumber = issuseNumber,
                WinNumber = winNumber,
                CreateTime = DateTime.Now,
            });

        }

        /// <summary>
        /// 查询开奖数据
        /// </summary>
        public GameWinNumber_InfoCollection QuerySDKLPK3_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new SDKLPK3_GameWinNumberManager().QuerySDKLPK3_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<SDKLPK3_GameWinNumber>, SDKLPK3_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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

        public GameWinNumber_InfoCollection QuerySDKLPK3_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new SDKLPK3_GameWinNumberManager().QuerySDKLPK3_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<SDKLPK3_GameWinNumber>, SDKLPK3_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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

        public GameWinNumber_InfoCollection QuerySDKLPK3_GameWinNumberDesc(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new SDKLPK3_GameWinNumberManager().QuerySDKLPK3_GameWinNumberDesc(startTime, endTime, pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<SDKLPK3_GameWinNumber>, SDKLPK3_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
            var manager = new SDKLPK3_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<SDKLPK3_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }
    }
}
