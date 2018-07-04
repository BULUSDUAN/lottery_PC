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
    public class LotteryDataBusiness_PL3 : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get
            {
                return "PL3";
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

                this.ClearGameChartCache("QueryPL3_JiBenZouSi_Info");
                this.ClearGameChartCache("QueryPL3_ZuXuanZouSi_info");
                this.ClearGameChartCache("QueryPL3_DX_info");
                this.ClearGameChartCache("QueryPL3_DXHM_info");
                this.ClearGameChartCache("QueryPL3_JIOU_info");
                this.ClearGameChartCache("QueryPL3_JOHM_info");
                this.ClearGameChartCache("QueryPL3_ZhiHe_info");
                this.ClearGameChartCache("QueryPL3_ZHHM_info");
                this.ClearGameChartCache("QueryPL3_HeiZhi_Info");
                this.ClearGameChartCache("QueryPL3_KuaDu_12_Info");
                this.ClearGameChartCache("QueryPL3_KuaDu_13_Info");
                this.ClearGameChartCache("QueryPL3_KuaDu_23_Info");
                this.ClearGameChartCache("QueryPL3_PL3_Chu31_Info");
                this.ClearGameChartCache("QueryPL3_PL3_Chu32_Info");
                this.ClearGameChartCache("QueryPL3_PL3_Chu33_Info");
                this.ClearGameChartCache("QueryPL3_PL3_HZTZ_Info");
                this.ClearGameChartCache("QueryPL3_PL3_HZHW_Info");
                this.ClearNewWinNumberCache("QueryPL3_GameWinNumber");

                Import_JBZS(issuseNumber, winNumber);
                Import_ZXZS(issuseNumber, winNumber);
                Import_DX(issuseNumber, winNumber);
                Import_DXHM(issuseNumber, winNumber);
                Import_JIOU(issuseNumber, winNumber);
                Import_JOHM(issuseNumber, winNumber);
                Import_ZhiHe(issuseNumber, winNumber);
                Import_ZHHM(issuseNumber, winNumber);
                Import_HeZhi(issuseNumber, winNumber);
                Import_HZTZ(issuseNumber, winNumber);
                Import_HZHW(issuseNumber, winNumber);

                Import_KuaDu_12(issuseNumber, winNumber);
                Import_KuaDu_23(issuseNumber, winNumber);
                Import_KuaDu_13(issuseNumber, winNumber);
                Import_Chu31(issuseNumber, winNumber);
                Import_Chu32(issuseNumber, winNumber);
                Import_Chu33(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }

        #region 前台查询数据

        /// <summary>
        ///查询基本走势列表按时间倒叙 
        /// </summary>
        public PL3_JiBenZouSi_InfoCollection QueryPL3_JiBenZouSi_Info(int length)
        {
            PL3_JiBenZouSi_InfoCollection Collection = new PL3_JiBenZouSi_InfoCollection();
            var list = this.QueryGameChart<PL3_JiBenZouSi_Info>(string.Format("QueryPL3_JiBenZouSi_Info_{0}", length), () =>
            {
                var infoList = new List<PL3_JiBenZouSi_Info>();
                var entityList = new PL3_Manager().QueryPL3_JiBenZouSi(length);

               ObjectConvert.ConvertEntityListToInfoList<List<PL3_JiBenZouSi>, PL3_JiBenZouSi, List<PL3_JiBenZouSi_Info>, PL3_JiBenZouSi_Info>(entityList, ref infoList,
                    () => { return new PL3_JiBenZouSi_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询组选走势列表按时间倒叙 
        /// </summary>
        public PL3_ZuXuanZouSi_InfoCollection QueryPL3_ZuXuanZouSi_info(int length)
        {
            PL3_ZuXuanZouSi_InfoCollection Collection = new PL3_ZuXuanZouSi_InfoCollection();
            var list = this.QueryGameChart<PL3_ZuXuanZouSi_Info>(string.Format("QueryPL3_ZuXuanZouSi_info_{0}", length), () =>
            {
                var infoList = new List<PL3_ZuXuanZouSi_Info>();
                var entityList = new PL3_Manager().QueryPL3_ZuXuanZouSi_info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<PL3_ZuXuanZouSi>, PL3_ZuXuanZouSi, List<PL3_ZuXuanZouSi_Info>, PL3_ZuXuanZouSi_Info>(entityList, ref infoList,
                    () => { return new PL3_ZuXuanZouSi_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询大小走势列表按时间倒叙 
        /// </summary>
        public PL3_DX_InfoCollection QueryPL3_DX_info(int length)
        {
            PL3_DX_InfoCollection Collection = new PL3_DX_InfoCollection();
            var list = this.QueryGameChart<PL3_DX_Info>(string.Format("QueryPL3_DX_info_{0}", length), () =>
            {
                var infoList = new List<PL3_DX_Info>();
                var entityList = new PL3_Manager().QueryPL3_DX_info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<PL3_DX>, PL3_DX, List<PL3_DX_Info>, PL3_DX_Info>(entityList, ref infoList,
                    () => { return new PL3_DX_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询大小号码走势列表按时间倒叙 
        /// </summary>
        public PL3_DXHM_InfoCollection QueryPL3_DXHM_info(int length)
        {
            PL3_DXHM_InfoCollection Collection = new PL3_DXHM_InfoCollection();

            var list = this.QueryGameChart<PL3_DXHM_Info>(string.Format("QueryPL3_DXHM_info_{0}", length), () =>
            {
                var infoList = new List<PL3_DXHM_Info>();
                var entityList = new PL3_Manager().QueryPL3_DXHM_info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<PL3_DXHM>, PL3_DXHM, List<PL3_DXHM_Info>, PL3_DXHM_Info>(entityList, ref infoList,
                    () => { return new PL3_DXHM_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询奇偶走势列表按时间倒叙 
        /// </summary>
        public PL3_JIOU_InfoCollection QueryPL3_JIOU_info(int length)
        {
            PL3_JIOU_InfoCollection Collection = new PL3_JIOU_InfoCollection();

            var list = this.QueryGameChart<PL3_JIOU_Info>(string.Format("QueryPL3_JIOU_info_{0}", length), () =>
            {
                var infoList = new List<PL3_JIOU_Info>();
                var entityList = new PL3_Manager().QueryPL3_JIOU_info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<PL3_JIOU>, PL3_JIOU, List<PL3_JIOU_Info>, PL3_JIOU_Info>(entityList, ref infoList,
                    () => { return new PL3_JIOU_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询奇偶号码走势列表按时间倒叙 
        /// </summary>
        public PL3_JOHM_InfoCollection QueryPL3_JOHM_info(int length)
        {
            PL3_JOHM_InfoCollection Collection = new PL3_JOHM_InfoCollection();
            var list = this.QueryGameChart<PL3_JOHM_Info>(string.Format("QueryPL3_JOHM_info_{0}", length), () =>
                {
                    var infoList = new List<PL3_JOHM_Info>();
                    var entityList = new PL3_Manager().QueryPL3_JOHM_info(length);

                   ObjectConvert.ConvertEntityListToInfoList<List<PL3_JOHM>, PL3_JOHM, List<PL3_JOHM_Info>, PL3_JOHM_Info>(entityList, ref infoList,
                        () => { return new PL3_JOHM_Info(); });
                    return infoList;
                });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询质和走势列表按时间倒叙 
        /// </summary>
        public PL3_ZhiHe_InfoCollection QueryPL3_ZhiHe_info(int length)
        {
            PL3_ZhiHe_InfoCollection Collection = new PL3_ZhiHe_InfoCollection();
            var list = this.QueryGameChart<PL3_ZhiHe_Info>(string.Format("QueryPL3_ZhiHe_info_{0}", length), () =>
               {
                   var infoList = new List<PL3_ZhiHe_Info>();
                   var entityList = new PL3_Manager().QueryPL3_ZhiHe_info(length);

                  ObjectConvert.ConvertEntityListToInfoList<List<PL3_ZhiHe>, PL3_ZhiHe, List<PL3_ZhiHe_Info>, PL3_ZhiHe_Info>(entityList, ref infoList,
                       () => { return new PL3_ZhiHe_Info(); });
                   return infoList;
               });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询质和号码走势列表按时间倒叙 
        /// </summary>
        public PL3_ZHHM_InfoCollection QueryPL3_ZHHM_info(int length)
        {
            PL3_ZHHM_InfoCollection Collection = new PL3_ZHHM_InfoCollection();
            var list = this.QueryGameChart<PL3_ZHHM_Info>(string.Format("QueryPL3_ZHHM_info_{0}", length), () =>
               {
                   var infoList = new List<PL3_ZHHM_Info>();
                   var entityList = new PL3_Manager().QueryPL3_ZHHM_info(length);

                  ObjectConvert.ConvertEntityListToInfoList<List<PL3_ZHHM>, PL3_ZHHM, List<PL3_ZHHM_Info>, PL3_ZHHM_Info>(entityList, ref infoList,
                       () => { return new PL3_ZHHM_Info(); });
                   return infoList;
               });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询和值走势列表按时间倒叙 
        /// </summary>
        public PL3_HeiZhi_InfoCollection QueryPL3_HeiZhi_Info(int length)
        {
            PL3_HeiZhi_InfoCollection Collection = new PL3_HeiZhi_InfoCollection();

            var list = this.QueryGameChart<PL3_HeiZhi_Info>(string.Format("QueryPL3_HeiZhi_Info_{0}", length), () =>
               {
                   var infoList = new List<PL3_HeiZhi_Info>();
                   var entityList = new PL3_Manager().QueryPL3_HeiZhi_Info(length);

                  ObjectConvert.ConvertEntityListToInfoList<List<PL3_HeiZhi>, PL3_HeiZhi, List<PL3_HeiZhi_Info>, PL3_HeiZhi_Info>(entityList, ref infoList,
                       () => { return new PL3_HeiZhi_Info(); });
                   return infoList;
               });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询跨度百位、十位列表按时间倒叙 
        /// </summary>
        public PL3_KuaDu_12_InfoCollection QueryPL3_KuaDu_12_Info(int length)
        {
            PL3_KuaDu_12_InfoCollection Collection = new PL3_KuaDu_12_InfoCollection();
            var list = this.QueryGameChart<PL3_KuaDu_12_Info>(string.Format("QueryPL3_KuaDu_12_Info_{0}", length), () =>
              {
                  var infoList = new List<PL3_KuaDu_12_Info>();
                  var entityList = new PL3_Manager().QueryPL3_KuaDu_12_Info(length);

                 ObjectConvert.ConvertEntityListToInfoList<List<PL3_KuaDu_12>, PL3_KuaDu_12, List<PL3_KuaDu_12_Info>, PL3_KuaDu_12_Info>(entityList, ref infoList,
                      () => { return new PL3_KuaDu_12_Info(); });
                  return infoList;
              });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询跨度百位、个位列表按时间倒叙 
        /// </summary>
        public PL3_KuaDu_13_InfoCollection QueryPL3_KuaDu_13_Info(int length)
        {
            PL3_KuaDu_13_InfoCollection Collection = new PL3_KuaDu_13_InfoCollection();

            var list = this.QueryGameChart<PL3_KuaDu_13_Info>(string.Format("QueryPL3_KuaDu_13_Info_{0}", length), () =>
              {
                  var infoList = new List<PL3_KuaDu_13_Info>();
                  var entityList = new PL3_Manager().QueryPL3_KuaDu_13_Info(length);

                 ObjectConvert.ConvertEntityListToInfoList<List<PL3_KuaDu_13>, PL3_KuaDu_13, List<PL3_KuaDu_13_Info>, PL3_KuaDu_13_Info>(entityList, ref infoList,
                      () => { return new PL3_KuaDu_13_Info(); });
                  return infoList;
              });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询跨度十位、个位列表按时间倒叙 
        /// </summary>
        public PL3_KuaDu_23_InfoCollection QueryPL3_KuaDu_23_Info(int length)
        {
            PL3_KuaDu_23_InfoCollection Collection = new PL3_KuaDu_23_InfoCollection();
            var list = this.QueryGameChart<PL3_KuaDu_23_Info>(string.Format("QueryPL3_KuaDu_23_Info_{0}", length), () =>
             {
                 var infoList = new List<PL3_KuaDu_23_Info>();
                 var entityList = new PL3_Manager().QueryPL3_KuaDu_23_Info(length);

                ObjectConvert.ConvertEntityListToInfoList<List<PL3_KuaDu_23>, PL3_KuaDu_23, List<PL3_KuaDu_23_Info>, PL3_KuaDu_23_Info>(entityList, ref infoList,
                     () => { return new PL3_KuaDu_23_Info(); });
                 return infoList;
             });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询除3 1走势按时间倒叙 
        /// </summary>
        public PL3_Chu31_InfoCollection QueryPL3_PL3_Chu31_Info(int length)
        {
            PL3_Chu31_InfoCollection Collection = new PL3_Chu31_InfoCollection();

            var list = this.QueryGameChart<PL3_Chu31_Info>(string.Format("QueryPL3_PL3_Chu31_Info_{0}", length), () =>
             {
                 var infoList = new List<PL3_Chu31_Info>();
                 var entityList = new PL3_Manager().QueryPL3_PL3_Chu31_Info(length);

                ObjectConvert.ConvertEntityListToInfoList<List<PL3_Chu31>, PL3_Chu31, List<PL3_Chu31_Info>, PL3_Chu31_Info>(entityList, ref infoList,
                     () => { return new PL3_Chu31_Info(); });
                 return infoList;
             });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询除3 2走势按时间倒叙 
        /// </summary>
        public PL3_Chu32_InfoCollection QueryPL3_PL3_Chu32_Info(int length)
        {
            PL3_Chu32_InfoCollection Collection = new PL3_Chu32_InfoCollection();

            var list = this.QueryGameChart<PL3_Chu32_Info>(string.Format("QueryPL3_PL3_Chu32_Info_{0}", length), () =>
             {
                 var infoList = new List<PL3_Chu32_Info>();
                 var entityList = new PL3_Manager().QueryPL3_PL3_Chu32_Info(length);

                ObjectConvert.ConvertEntityListToInfoList<List<PL3_Chu32>, PL3_Chu32, List<PL3_Chu32_Info>, PL3_Chu32_Info>(entityList, ref infoList,
                     () => { return new PL3_Chu32_Info(); });
                 return infoList;
             });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询除3 3走势按时间倒叙 
        /// </summary>
        public PL3_Chu33_InfoCollection QueryPL3_PL3_Chu33_Info(int length)
        {
            PL3_Chu33_InfoCollection Collection = new PL3_Chu33_InfoCollection();

            var list = this.QueryGameChart<PL3_Chu33_Info>(string.Format("QueryPL3_PL3_Chu33_Info_{0}", length), () =>
            {
                var infoList = new List<PL3_Chu33_Info>();
                var entityList = new PL3_Manager().QueryPL3_PL3_Chu33_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<PL3_Chu33>, PL3_Chu33, List<PL3_Chu33_Info>, PL3_Chu33_Info>(entityList, ref infoList,
                    () => { return new PL3_Chu33_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///添加排列3 和值特征走势数据
        /// </summary>
        public PL3_HZTZ_InfoCollection QueryPL3_PL3_HZTZ_Info(int length)
        {
            PL3_HZTZ_InfoCollection Collection = new PL3_HZTZ_InfoCollection();

            var list = this.QueryGameChart<PL3_HZTZ_Info>(string.Format("QueryPL3_PL3_HZTZ_Info_{0}", length), () =>
            {
                var infoList = new List<PL3_HZTZ_Info>();
                var entityList = new PL3_Manager().QueryPL3_PL3_HZTZ_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<PL3_HZTZ>, PL3_HZTZ, List<PL3_HZTZ_Info>, PL3_HZTZ_Info>(entityList, ref infoList,
                    () => { return new PL3_HZTZ_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///添加排列3 和值合尾走势数据
        /// </summary>
        public PL3_HZHW_InfoCollection QueryPL3_PL3_HZHW_Info(int length)
        {
            PL3_HZHW_InfoCollection Collection = new PL3_HZHW_InfoCollection();

            var list = this.QueryGameChart<PL3_HZHW_Info>(string.Format("QueryPL3_PL3_HZHW_Info_{0}", length), () =>
            {
                var infoList = new List<PL3_HZHW_Info>();
                var entityList = new PL3_Manager().QueryPL3_PL3_HZHW_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<PL3_HZHW>, PL3_HZHW, List<PL3_HZHW_Info>, PL3_HZHW_Info>(entityList, ref infoList,
                    () => { return new PL3_HZHW_Info(); });
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
            var manager = new PL3_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddPL3_GameWinNumber(new PL3_GameWinNumber
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
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_JiBenZouSiIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]) };
            var temp = array.Distinct().ToArray();

            var last = manager.QueryPL3_JiBenZouSi();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<PL3_JiBenZouSi>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("BW_"))
                {
                    var order = p.Name.Replace("BW_", string.Empty);
                    lastValue = winRed[0].Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("SW_"))
                {
                    var order = p.Name.Replace("SW_", string.Empty);
                    lastValue = winRed[1].Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("GW_"))
                {
                    var order = p.Name.Replace("GW_", string.Empty);
                    lastValue = winRed[2].Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("ZH_"))
                {
                    var order = p.Name.Replace("ZH_", string.Empty);
                    if (temp.Count() == 2 && order == "3")
                        lastValue = 0;
                    if (temp.Count() == 3 && order == "6")
                        lastValue = 0;
                    if (temp.Count() == 1 && order == "Baozi")
                        lastValue = 0;
                }
                return lastValue;
            });

            manager.AddPL3_JiBenZouSi(entity);
        }

        /// <summary>
        /// 大小走势
        /// </summary>
        private void Import_DX(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_DXIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winArray[0]), int.Parse(winArray[1]), int.Parse(winArray[2]) };
            var win1 = string.Empty;
            var win2 = string.Empty;
            var win3 = string.Empty;

            if (array[0] >= 5)
                win1 = "D";
            else
                win1 = "X";

            if (array[1] >= 5)
                win2 = "D";
            else
                win2 = "X";

            if (array[2] >= 5)
                win3 = "D";
            else
                win3 = "X";

            var arrayWin3 = new string[] { win1, win2, win3 };
            var dxType = string.Join("", arrayWin3);
            int daCount = dxType.Count(p => p == 'D');

            var last = manager.QueryPL3_DX();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("DXType", dxType);
            dic.Add("DaoXiaoBi", string.Format("{0}:{1}", daCount, 3 - daCount));

            var entity = this.CreateNewEntity<PL3_DX>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("DX_"))
                {
                    var order = p.Name.Replace("DX_", string.Empty);
                    if (order == "DDD" && order == dxType)
                        lastValue = 0;
                    if (order == "DDX" && order == dxType)
                        lastValue = 0;
                    if (order == "DXD" && order == dxType)
                        lastValue = 0;
                    if (order == "XDD" && order == dxType)
                        lastValue = 0;
                    if (order == "DXX" && order == dxType)
                        lastValue = 0;
                    if (order == "XDX" && order == dxType)
                        lastValue = 0;
                    if (order == "XXD" && order == dxType)
                        lastValue = 0;
                    if (order == "XXX" && order == dxType)
                        lastValue = 0;
                }
                return lastValue;
            });

            entity.Bi0_3 = last == null ? 1 : (daCount == 0 ? 0 : last.Bi0_3 + 1);
            entity.Bi1_2 = last == null ? 1 : (daCount == 1 ? 0 : last.Bi1_2 + 1);
            entity.Bi2_1 = last == null ? 1 : (daCount == 2 ? 0 : last.Bi2_1 + 1);
            entity.Bi3_0 = last == null ? 1 : (daCount == 3 ? 0 : last.Bi3_0 + 1);

            manager.AddPL3_DX(entity);
        }

        /// <summary>
        /// 组选走势
        /// </summary>
        private void Import_ZXZS(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_ZuXuanZouSiIssuseNumber(issuseNumber);
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

            var last = manager.QueryPL3_ZuXuanZouSi();
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

            var entity = this.CreateNewEntity<PL3_ZuXuanZouSi>(dic, (p) =>
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

            manager.AddPL3_ZuXuanZouSi(entity);
        }

        /// <summary>
        /// 大小号码分布
        /// </summary>
        private void Import_DXHM(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_DXHMIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]) };
            var win1 = string.Empty;
            var win2 = string.Empty;
            var win3 = string.Empty;

            if (array[0] >= 5)
                win1 = "D";
            else
                win1 = "X";

            if (array[1] >= 5)
                win2 = "D";
            else
                win2 = "X";

            if (array[2] >= 5)
                win3 = "D";
            else
                win3 = "X";

            var arrayWin3 = new string[] { win1, win2, win3 };
            var dxType = string.Join("", arrayWin3);
            int daCount = dxType.Count(p => p == 'D');

            var last = manager.QueryPL3_DXHM();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("DaoXiaoBi", string.Format("{0}:{1}", daCount, 3 - daCount));

            var entity = this.CreateNewEntity<PL3_DXHM>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("BW_"))
                {
                    var order = p.Name.Replace("BW_", string.Empty);
                    lastValue = winRed[0].Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("SW_"))
                {
                    var order = p.Name.Replace("SW_", string.Empty);
                    lastValue = winRed[1].Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("GW_"))
                {
                    var order = p.Name.Replace("GW_", string.Empty);
                    lastValue = winRed[2].Contains(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            entity.Bi0_3 = daCount == 0 ? 0 : (last == null ? 1 : last.Bi0_3 + 1);
            entity.Bi1_2 = daCount == 1 ? 0 : (last == null ? 1 : last.Bi1_2 + 1);
            entity.Bi2_1 = daCount == 2 ? 0 : (last == null ? 1 : last.Bi2_1 + 1);
            entity.Bi3_0 = daCount == 3 ? 0 : (last == null ? 1 : last.Bi3_0 + 1);
            manager.AddPL3_DXHM(entity);
        }

        /// <summary>
        /// 排列3奇偶走势
        /// </summary>
        private void Import_JIOU(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_JIOUIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winArray[0]), int.Parse(winArray[1]), int.Parse(winArray[2]) };
            var win1 = string.Empty;
            var win2 = string.Empty;
            var win3 = string.Empty;

            if (array[0] % 2 == 0)
                win1 = "O";
            else
                win1 = "J";

            if (array[1] % 2 == 0)
                win2 = "O";
            else
                win2 = "J";

            if (array[2] % 2 == 0)
                win3 = "O";
            else
                win3 = "J";

            var arrayWin3 = new string[] { win1, win2, win3 };
            var joType = string.Join("", arrayWin3);
            int jiCount = joType.Count(p => p == 'J');

            var last = manager.QueryPL3_JIOU();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("JOType", joType);
            dic.Add("JOBi", string.Format("{0}:{1}", jiCount, 3 - jiCount));

            var entity = this.CreateNewEntity<PL3_JIOU>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("JO_"))
                {
                    var order = p.Name.Replace("JO_", string.Empty);
                    if (order == "JJJ" && order == joType)
                        lastValue = 0;
                    if (order == "JJO" && order == joType)
                        lastValue = 0;
                    if (order == "JOJ" && order == joType)
                        lastValue = 0;
                    if (order == "OJJ" && order == joType)
                        lastValue = 0;
                    if (order == "JOO" && order == joType)
                        lastValue = 0;
                    if (order == "OJO" && order == joType)
                        lastValue = 0;
                    if (order == "OOJ" && order == joType)
                        lastValue = 0;
                    if (order == "OOO" && order == joType)
                        lastValue = 0;
                }
                return lastValue;
            });

            entity.Bi0_3 = last == null ? 1 : (jiCount == 0 ? 0 : last.Bi0_3 + 1);
            entity.Bi1_2 = last == null ? 1 : (jiCount == 1 ? 0 : last.Bi1_2 + 1);
            entity.Bi2_1 = last == null ? 1 : (jiCount == 2 ? 0 : last.Bi2_1 + 1);
            entity.Bi3_0 = last == null ? 1 : (jiCount == 3 ? 0 : last.Bi3_0 + 1);

            manager.AddPL3_JIOU(entity);

        }

        /// <summary>
        /// 奇偶号码分布
        /// </summary>
        private void Import_JOHM(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_JOHMIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]) };
            var win1 = string.Empty;
            var win2 = string.Empty;
            var win3 = string.Empty;

            if (array[0] % 2 == 0)
                win1 = "O";
            else
                win1 = "J";

            if (array[1] % 2 == 0)
                win2 = "O";
            else
                win2 = "J";

            if (array[2] % 2 == 0)
                win3 = "O";
            else
                win3 = "J";

            var arrayWin3 = new string[] { win1, win2, win3 };
            var joType = string.Join("", arrayWin3);
            int jiCount = joType.Count(p => p == 'J');

            var last = manager.QueryPL3_JOHM();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("JOBi", string.Format("{0}:{1}", jiCount, 3 - jiCount));

            var entity = this.CreateNewEntity<PL3_JOHM>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("BW_"))
                {
                    var order = p.Name.Replace("BW_", string.Empty);
                    lastValue = winRed[0].Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("SW_"))
                {
                    var order = p.Name.Replace("SW_", string.Empty);
                    lastValue = winRed[1].Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("GW_"))
                {
                    var order = p.Name.Replace("GW_", string.Empty);
                    lastValue = winRed[2].Contains(order) ? 0 : lastValue;
                }
                return lastValue;
            });
            entity.Bi0_3 = jiCount == 0 ? 0 : (last == null ? 1 : last.Bi0_3 + 1);
            entity.Bi1_2 = jiCount == 1 ? 0 : (last == null ? 1 : last.Bi1_2 + 1);
            entity.Bi2_1 = jiCount == 2 ? 0 : (last == null ? 1 : last.Bi2_1 + 1);
            entity.Bi3_0 = jiCount == 3 ? 0 : (last == null ? 1 : last.Bi3_0 + 1);

            manager.AddPL3_JOHM(entity);
        }

        /// <summary>
        /// 排列3质和走势
        /// </summary>
        private void Import_ZhiHe(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_ZhiHeIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winArray[0]), int.Parse(winArray[1]), int.Parse(winArray[2]) };
            var win1 = string.Empty;
            var win2 = string.Empty;
            var win3 = string.Empty;

            var zhilist = new int[] { 1, 2, 3, 5, 7 };

            if (zhilist.Contains(array[0]))
                win1 = "Z";
            else
                win1 = "H";
            if (zhilist.Contains(array[1]))
                win2 = "Z";
            else
                win2 = "H";
            if (zhilist.Contains(array[2]))
                win3 = "Z";
            else
                win3 = "H";

            var arrayWin3 = new string[] { win1, win2, win3 };
            var zhType = string.Join("", arrayWin3);
            int zhiCount = zhType.Count(p => p == 'Z');

            var last = manager.QueryPL3_ZhiHe();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("ZHType", zhType);
            dic.Add("ZHBi", string.Format("{0}:{1}", zhiCount, 3 - zhiCount));

            var entity = this.CreateNewEntity<PL3_ZhiHe>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("ZH_"))
                {
                    var order = p.Name.Replace("ZH_", string.Empty);
                    if (order == "ZZZ" && order == zhType)
                        lastValue = 0;
                    if (order == "ZZH" && order == zhType)
                        lastValue = 0;
                    if (order == "ZHZ" && order == zhType)
                        lastValue = 0;
                    if (order == "HZZ" && order == zhType)
                        lastValue = 0;
                    if (order == "ZHH" && order == zhType)
                        lastValue = 0;
                    if (order == "HZH" && order == zhType)
                        lastValue = 0;
                    if (order == "HHZ" && order == zhType)
                        lastValue = 0;
                    if (order == "HHH" && order == zhType)
                        lastValue = 0;
                }
                return lastValue;
            });

            entity.Bi0_3 = last == null ? 1 : (zhiCount == 0 ? 0 : last.Bi0_3 + 1);
            entity.Bi1_2 = last == null ? 1 : (zhiCount == 1 ? 0 : last.Bi1_2 + 1);
            entity.Bi2_1 = last == null ? 1 : (zhiCount == 2 ? 0 : last.Bi2_1 + 1);
            entity.Bi3_0 = last == null ? 1 : (zhiCount == 3 ? 0 : last.Bi3_0 + 1);

            manager.AddPL3_ZhiHe(entity);
        }

        /// <summary>
        /// 质和号码分布
        /// </summary>
        private void Import_ZHHM(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_ZHHMIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]) };
            var win1 = string.Empty;
            var win2 = string.Empty;
            var win3 = string.Empty;

            var zhilist = new int[] { 1, 2, 3, 5, 7 };

            if (zhilist.Contains(array[0]))
                win1 = "Z";
            else
                win1 = "H";
            if (zhilist.Contains(array[1]))
                win2 = "Z";
            else
                win2 = "H";
            if (zhilist.Contains(array[2]))
                win3 = "Z";
            else
                win3 = "H";

            var arrayWin3 = new string[] { win1, win2, win3 };
            var zhiType = string.Join("", arrayWin3);
            int zhiCount = zhiType.Count(p => p == 'Z');

            var last = manager.QueryPL3_ZHHM();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("ZHBi", string.Format("{0}:{1}", zhiCount, 3 - zhiCount));

            var entity = this.CreateNewEntity<PL3_ZHHM>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("BW_"))
                {
                    var order = p.Name.Replace("BW_", string.Empty);
                    lastValue = winRed[0].Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("SW_"))
                {
                    var order = p.Name.Replace("SW_", string.Empty);
                    lastValue = winRed[1].Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("GW_"))
                {
                    var order = p.Name.Replace("GW_", string.Empty);
                    lastValue = winRed[2].Contains(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            entity.Bi0_3 = zhiCount == 0 ? 0 : (last == null ? 1 : last.Bi0_3 + 1);
            entity.Bi1_2 = zhiCount == 1 ? 0 : (last == null ? 1 : last.Bi1_2 + 1);
            entity.Bi2_1 = zhiCount == 2 ? 0 : (last == null ? 1 : last.Bi2_1 + 1);
            entity.Bi3_0 = zhiCount == 3 ? 0 : (last == null ? 1 : last.Bi3_0 + 1);

            manager.AddPL3_ZHHM(entity);
        }

        /// <summary>
        /// 和值走势
        /// </summary>
        private void Import_HeZhi(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_HeiZhiIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winArray[0]) + int.Parse(winArray[1]) + int.Parse(winArray[2]);
            var hw = hz % 10;

            var last = manager.QueryPL3_HeiZhi();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("HZ", hz);

            var entity = this.CreateNewEntity<PL3_HeiZhi>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号 
                if (p.Name.StartsWith("HZ_"))
                {
                    var order = p.Name.Replace("HZ_", string.Empty);
                    lastValue = hz == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("HW_"))
                {
                    var order = p.Name.Replace("HW_", string.Empty);
                    lastValue = hw == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddPL3_HeiZhi(entity);
        }

        /// <summary>
        /// 和值特征走势
        /// </summary>
        private void Import_HZTZ(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_HZTZIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]);
            var last = manager.QueryPL3_HZTZ();
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

            var entity = this.CreateNewEntity<PL3_HZTZ>(dic, (p) =>
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

            manager.AddPL3_HZTZ(entity);
        }

        /// <summary>
        /// 和值合尾走势
        /// </summary>
        private void Import_HZHW(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_HZHWIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]);
            var avg = (hz / 3).ToString("N0");
            var hw = hz % 10;
            var last = manager.QueryPL3_HZHW();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("HeZhi", hz);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);

            var entity = this.CreateNewEntity<PL3_HZHW>(dic, (p) =>
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

            manager.AddPL3_HZHW(entity);
        }


        /// <summary>
        /// 跨度百十位走势
        /// </summary>
        private void Import_KuaDu_12(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_KuaDu_12IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var kd = Math.Abs(int.Parse(winRed[0]) - int.Parse(winRed[1]));
            var zhilist = new int[] { 1, 2, 3, 5, 7 };

            var last = manager.QueryPL3_KuaDu_12();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<PL3_KuaDu_12>(dic, (p) =>
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

            manager.AddPL3_KuaDu_12(entity);
        }

        /// <summary>
        /// 跨度百十位走势
        /// </summary>
        private void Import_KuaDu_23(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_KuaDu_23IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var kd = Math.Abs(int.Parse(winRed[1]) - int.Parse(winRed[2]));
            var zhilist = new int[] { 1, 2, 3, 5, 7 };

            var last = manager.QueryPL3_KuaDu_23();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<PL3_KuaDu_23>(dic, (p) =>
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

            manager.AddPL3_KuaDu_23(entity);
        }

        /// <summary>
        /// 跨度百个位走势
        /// </summary>
        private void Import_KuaDu_13(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_KuaDu_13IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var kd = Math.Abs(int.Parse(winRed[0]) - int.Parse(winRed[2]));
            var zhilist = new int[] { 1, 2, 3, 5, 7 };

            var last = manager.QueryPL3_KuaDu_13();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<PL3_KuaDu_13>(dic, (p) =>
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

            manager.AddPL3_KuaDu_13(entity);
        }

        /// <summary>
        /// 除3 走势1
        /// </summary>
        private void Import_Chu31(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_Chu31IssuseNumber(issuseNumber);
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

            var last = manager.QueryPL3_Chu31();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("P012_Proportion", P012_Proportion);

            var entity = this.CreateNewEntity<PL3_Chu31>(dic, (p) =>
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

            manager.AddPL3_Chu31(entity);
        }

        /// <summary>
        /// 除3 走势2
        /// </summary>
        private void Import_Chu32(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_Chu32IssuseNumber(issuseNumber);
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

            var last = manager.QueryPL3_Chu32();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);

            var entity = this.CreateNewEntity<PL3_Chu32>(dic, (p) =>
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

            manager.AddPL3_Chu32(entity);
        }

        /// <summary>
        /// 除3 走势3
        /// </summary>
        private void Import_Chu33(string issuseNumber, string winNumber)
        {
            var manager = new PL3_Manager();
            var issuse = manager.QueryPL3_Chu33IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var redball = new int[]{
                Convert.ToInt32(winRed[0])%3,
                Convert.ToInt32(winRed[1])%3,
                Convert.ToInt32(winRed[2])%3
            };

            var last = manager.QueryPL3_Chu33();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("RedBall1", winRed[0]);
            dic.Add("RedBall2", winRed[1]);
            dic.Add("RedBall3", winRed[2]);
            dic.Add("y3xt", string.Format("{0}{1}{2}", redball[0], redball[1], redball[2]));

            var entity = this.CreateNewEntity<PL3_Chu33>(dic, (p) =>
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

            manager.AddPL3_Chu33(entity);
        }

        #endregion

        /// <summary>
        /// 查询开奖数据
        /// </summary>
        public GameWinNumber_InfoCollection QueryPL3_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new PL3_GameWinNumberManager().QueryPL3_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<PL3_GameWinNumber>, PL3_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryPL3_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new PL3_GameWinNumberManager().QueryPL3_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<PL3_GameWinNumber>, PL3_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
        public GameWinNumber_InfoCollection QueryPL3_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new PL3_GameWinNumberManager().QueryPL3_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<PL3_GameWinNumber>, PL3_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryPL3_GameWinNumber_{0}_{1}_{2}_{3}", pageIndex, pageSize, startTime.ToString("yyyyMMdd"), endTime.ToString("yyyyMMdd"));
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new PL3_GameWinNumberManager().QueryPL3_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<PL3_GameWinNumber>, PL3_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
            var manager = new PL3_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<PL3_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }
    }
}
