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
using KaSon.FrameWork.Analyzer.AnalyzerFactory;

namespace KaSon.FrameWork.ORM.Helper.WinNumber
{
    public class LotteryDataBusiness_DLT : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get
            {
                return "DLT";
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

                this.ClearGameChartCache("QueryDLT_JiBenZouSi_Info");
                this.ClearGameChartCache("QueryDLT_DX_Info");
                this.ClearGameChartCache("QueryDLT_JiOu_Info");
                this.ClearGameChartCache("QueryDLT_ZhiHe_Info");
                this.ClearGameChartCache("QueryDLT_HeZhi_Info");
                this.ClearGameChartCache("QueryDLT_Chu3_Info");
                this.ClearGameChartCache("QueryDLT_KuaDu_SW_Info");
                this.ClearGameChartCache("QueryDLT_KuaDu_12_Info");
                this.ClearGameChartCache("QueryDLT_KuaDu_23_Info");
                this.ClearGameChartCache("QueryDLT_KuaDu_34_Info");
                this.ClearGameChartCache("QueryDLT_KuaDu_45_Info");
                this.ClearNewWinNumberCache("QueryDLT_GameWinNumber");

                Import_JBZS(issuseNumber, winNumber);
                Import_DX(issuseNumber, winNumber);
                Import_JiOu(issuseNumber, winNumber);
                Import_ZhiHe(issuseNumber, winNumber);
                Import_DLT_HeZhi(issuseNumber, winNumber);
                Import_DLT_Chu3(issuseNumber, winNumber);
                Import_DLT_KuaDu_SW(issuseNumber, winNumber);
                Import_DLT_KuaDu_12(issuseNumber, winNumber);
                Import_DLT_KuaDu_23(issuseNumber, winNumber);
                Import_DLT_KuaDu_34(issuseNumber, winNumber);
                Import_DLT_KuaDu_45(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }

        #region 前台查询数据

        /// <summary>
        ///查询基本走势列表按时间倒叙 
        /// </summary>
        public List<DLT_JiBenZouSi_Info> QueryDLT_JiBenZouSi_Info(int length)
        {
            List<DLT_JiBenZouSi_Info> Collection = new List<DLT_JiBenZouSi_Info>();
            var list = this.QueryGameChart<DLT_JiBenZouSi_Info>(string.Format("QueryDLT_JiBenZouSi_Info_{0}", length), () =>
           {
               var infoList = new List<DLT_JiBenZouSi_Info>();
               var entityList = new DLT_Manager().QueryDLT_JiBenZouSi(length);

              ObjectConvert.ConvertEntityListToInfoList<List<DLT_JiBenZouSi>, DLT_JiBenZouSi, List<DLT_JiBenZouSi_Info>, DLT_JiBenZouSi_Info>(entityList, ref infoList,
                   () => { return new DLT_JiBenZouSi_Info(); });
               return infoList;
           });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询大小走势列表按时间倒叙 
        /// </summary>
        public List<DLT_DX_Info> QueryDLT_DX_Info(int length)
        {
            List<DLT_DX_Info> Collection = new List<DLT_DX_Info>();
            var list = this.QueryGameChart<DLT_DX_Info>(string.Format("QueryDLT_DX_Info_{0}", length), () =>
                {
                    var infoList = new List<DLT_DX_Info>();
                    var entityList = new DLT_Manager().QueryDLT_DX(length);

                   ObjectConvert.ConvertEntityListToInfoList<List<DLT_DX>, DLT_DX, List<DLT_DX_Info>, DLT_DX_Info>(entityList, ref infoList,
                        () => { return new DLT_DX_Info(); });
                    return infoList;
                });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询奇偶走势列表按时间倒叙 
        /// </summary>
        public List<DLT_JiOu_Info> QueryDLT_JiOu_Info(int length)
        {
            List<DLT_JiOu_Info> Collection = new List<DLT_JiOu_Info>();
            var list = this.QueryGameChart<DLT_JiOu_Info>(string.Format("QueryDLT_JiOu_Info_{0}", length), () =>
             {
                 var infoList = new List<DLT_JiOu_Info>();
                 var entityList = new DLT_Manager().QueryDLT_JiOu(length);

                ObjectConvert.ConvertEntityListToInfoList<List<DLT_JiOu>, DLT_JiOu, List<DLT_JiOu_Info>, DLT_JiOu_Info>(entityList, ref infoList,
                     () => { return new DLT_JiOu_Info(); });
                 return infoList;
             });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询质和走势列表按时间倒叙 
        /// </summary>
        public List<DLT_ZhiHe_Info> QueryDLT_ZhiHe_Info(int length)
        {
            List<DLT_ZhiHe_Info> Collection = new List<DLT_ZhiHe_Info>();
            var list = this.QueryGameChart<DLT_ZhiHe_Info>(string.Format("QueryDLT_ZhiHe_Info_{0}", length), () =>
                {
                    var infoList = new List<DLT_ZhiHe_Info>();
                    var entityList = new DLT_Manager().QueryDLT_ZhiHe(length);

                   ObjectConvert.ConvertEntityListToInfoList<List<DLT_ZhiHe>, DLT_ZhiHe, List<DLT_ZhiHe_Info>, DLT_ZhiHe_Info>(entityList, ref infoList,
                        () => { return new DLT_ZhiHe_Info(); });
                    return infoList;
                });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询和值走势列表按时间倒叙 
        /// </summary>
        public List<DLT_HeZhi_Info> QueryDLT_HeZhi_Info(int length)
        {
            List<DLT_HeZhi_Info> Collection = new List<DLT_HeZhi_Info>();
            var list = this.QueryGameChart<DLT_HeZhi_Info>(string.Format("QueryDLT_HeZhi_Info_{0}", length), () =>
               {
                   var infoList = new List<DLT_HeZhi_Info>();
                   var entityList = new DLT_Manager().QueryDLT_HeZhi(length);

                  ObjectConvert.ConvertEntityListToInfoList<List<DLT_HeZhi>, DLT_HeZhi, List<DLT_HeZhi_Info>, DLT_HeZhi_Info>(entityList, ref infoList,
                       () => { return new DLT_HeZhi_Info(); });
                   return infoList;
               });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询除3走势列表按时间倒叙 
        /// </summary>
        public List<DLT_Chu3_Info> QueryDLT_Chu3_Info(int length)
        {
            List<DLT_Chu3_Info> Collection = new List<DLT_Chu3_Info>();
            var list = this.QueryGameChart<DLT_Chu3_Info>(string.Format("QueryDLT_Chu3_Info_{0}", length), () =>
                {
                    var infoList = new List<DLT_Chu3_Info>();
                    var entityList = new DLT_Manager().QueryDLT_Chu3(length);

                   ObjectConvert.ConvertEntityListToInfoList<List<DLT_Chu3>, DLT_Chu3, List<DLT_Chu3_Info>, DLT_Chu3_Info>(entityList, ref infoList,
                        () => { return new DLT_Chu3_Info(); });
                    return infoList;
                });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询首尾走势列表按时间倒叙 
        /// </summary>
        public List<DLT_KuaDu_SW_Info> QueryDLT_KuaDu_SW_Info(int length)
        {
            List<DLT_KuaDu_SW_Info> Collection = new List<DLT_KuaDu_SW_Info>();
            var list = this.QueryGameChart<DLT_KuaDu_SW_Info>(string.Format("QueryDLT_KuaDu_SW_Info_{0}", length), () =>
                {
                    var infoList = new List<DLT_KuaDu_SW_Info>();
                    var entityList = new DLT_Manager().QueryDLT_KuaDu_SW(length);

                   ObjectConvert.ConvertEntityListToInfoList<List<DLT_KuaDu_SW>, DLT_KuaDu_SW, List<DLT_KuaDu_SW_Info>, DLT_KuaDu_SW_Info>(entityList, ref infoList,
                        () => { return new DLT_KuaDu_SW_Info(); });
                    return infoList;
                });
            Collection.AddRange(list);
            return Collection;
        }


        /// <summary>
        ///查询12走势列表按时间倒叙 
        /// </summary>
        public List<DLT_KuaDu_12_Info> QueryDLT_KuaDu_12_Info(int length)
        {
            List<DLT_KuaDu_12_Info> Collection = new List<DLT_KuaDu_12_Info>();
            var list = this.QueryGameChart<DLT_KuaDu_12_Info>(string.Format("QueryDLT_KuaDu_12_Info_{0}", length), () =>
            {
                var infoList = new List<DLT_KuaDu_12_Info>();
                var entityList = new DLT_Manager().QueryDLT_KuaDu_12(length);

               ObjectConvert.ConvertEntityListToInfoList<List<DLT_KuaDu_12>, DLT_KuaDu_12, List<DLT_KuaDu_12_Info>, DLT_KuaDu_12_Info>(entityList, ref infoList,
                () => { return new DLT_KuaDu_12_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询23走势列表按时间倒叙 
        /// </summary>
        public List<DLT_KuaDu_23_Info> QueryDLT_KuaDu_23_Info(int length)
        {
            List<DLT_KuaDu_23_Info> Collection = new List<DLT_KuaDu_23_Info>();
            var list = this.QueryGameChart<DLT_KuaDu_23_Info>(string.Format("QueryDLT_KuaDu_23_Info_{0}", length), () =>
                {
                    var infoList = new List<DLT_KuaDu_23_Info>();
                    var entityList = new DLT_Manager().QueryDLT_KuaDu_23(length);

                   ObjectConvert.ConvertEntityListToInfoList<List<DLT_KuaDu_23>, DLT_KuaDu_23, List<DLT_KuaDu_23_Info>, DLT_KuaDu_23_Info>(entityList, ref infoList,
                    () => { return new DLT_KuaDu_23_Info(); });
                    return infoList;
                });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询34走势列表按时间倒叙 
        /// </summary>
        public List<DLT_KuaDu_34_Info> QueryDLT_KuaDu_34_Info(int length)
        {
            List<DLT_KuaDu_34_Info> Collection = new List<DLT_KuaDu_34_Info>();
            var list = this.QueryGameChart<DLT_KuaDu_34_Info>(string.Format("QueryDLT_KuaDu_34_Info_{0}", length), () =>
                {
                    var infoList = new List<DLT_KuaDu_34_Info>();
                    var entityList = new DLT_Manager().QueryDLT_KuaDu_34(length);

                   ObjectConvert.ConvertEntityListToInfoList<List<DLT_KuaDu_34>, DLT_KuaDu_34, List<DLT_KuaDu_34_Info>, DLT_KuaDu_34_Info>(entityList, ref infoList,
                    () => { return new DLT_KuaDu_34_Info(); });
                    return infoList;
                });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询45走势列表按时间倒叙 
        /// </summary>
        public List<DLT_KuaDu_45_Info> QueryDLT_KuaDu_45_Info(int length)
        {
            List<DLT_KuaDu_45_Info> Collection = new List<DLT_KuaDu_45_Info>();
            var list = this.QueryGameChart<DLT_KuaDu_45_Info>(string.Format("QueryDLT_KuaDu_45_Info_{0}", length), () =>
                {
                    var infoList = new List<DLT_KuaDu_45_Info>();
                    var entityList = new DLT_Manager().QueryDLT_KuaDu_45(length);

                   ObjectConvert.ConvertEntityListToInfoList<List<DLT_KuaDu_45>, DLT_KuaDu_45, List<DLT_KuaDu_45_Info>, DLT_KuaDu_45_Info>(entityList, ref infoList,
                    () => { return new DLT_KuaDu_45_Info(); });
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
            var manager = new DLT_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddDLT_GameWinNumber(new DLT_GameWinNumber
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
            var manager = new DLT_Manager();
            var issuse = manager.QueryDLT_JiBenZouSiIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var winRed = winArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var winBlue = winArray[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var last = manager.QueryLastDLT_JiBenZouSi();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<DLT_JiBenZouSi>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red"))
                {
                    var order = p.Name.Replace("Red", string.Empty);
                    lastValue = winRed.Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Blue"))
                {
                    var order = p.Name.Replace("Blue", string.Empty);
                    lastValue = winBlue.Contains(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddDLT_JiBenZouSi(entity);
        }

        /// <summary>
        /// 大乐透大小走势
        /// </summary>
        public void Import_DX(string issuseNumber, string winNumber)
        {
            var manager = new DLT_Manager();
            var issuse = manager.QueryDLT_DXIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var winRed = winArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]) };

            #region  小大排位
            int dCount = 0;
            int xCount = 0;
            foreach (var item in array)
            {
                if (item <= 15)
                    xCount++;
                else
                    dCount++;
            }

            var da = "大";
            var strda = string.Empty;
            for (int i = 0; i < dCount; i++)
            {
                strda += da;
            }

            var xiao = "小";
            var strxiao = string.Empty;
            for (int i = 0; i < xCount; i++)
            {
                strxiao += xiao;
            }

            #endregion

            #region 大小个数

            string winxiao1, winxiao2, winxiao3, winxiao4, winxiao5 = string.Empty;

            if (array[0] <= 17)
                winxiao1 = "X";
            else
                winxiao1 = "D";

            if (array[1] <= 17)
                winxiao2 = "X";
            else
                winxiao2 = "D";

            if (array[2] <= 17)
                winxiao3 = "X";
            else
                winxiao3 = "D";

            if (array[3] <= 17)
                winxiao4 = "X";
            else
                winxiao4 = "D";

            if (array[4] <= 17)
                winxiao5 = "X";
            else
                winxiao5 = "D";


            var arrayWinXiao = new string[] { winxiao1, winxiao2, winxiao3, winxiao4, winxiao5 };
            #endregion

            var last = manager.QueryDLT_DX();
            var dic = new Dictionary<string, object>();
            dic.Add("QianQu", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("DaoXiaoQualifying", string.Format("{0}{1}", strxiao, strda));
            dic.Add("DaoXiaoBi", string.Format("{0}:{1}", dCount, xCount));
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("IssuseNumber", issuseNumber);
            var entity = this.CreateNewEntity<DLT_DX>(dic, (p) =>
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
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var bi = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(bi[0]) == dCount && int.Parse(bi[1]) == xCount ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddDLT_DX(entity);
        }

        /// <summary>
        /// 大乐透奇偶走势
        /// </summary>
        public void Import_JiOu(string issuseNumber, string winNumber)
        {
            var manager = new DLT_Manager();
            var issuse = manager.QueryDLT_JiOuIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var winRed = winArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]) };

            #region  奇偶排位

            var paiwei = string.Empty;
            var ji = "奇";
            var ou = "偶";
            for (int k = 0; k < array.Length; k++)
            {
                if (array[k] % 2 == 0)
                {
                    paiwei += ji;
                }
                else
                {
                    paiwei += ou;
                }
            }

            int jCount = 0;
            foreach (var item in array)
            {
                if (item % 2 == 1)
                    jCount++;
            }

            #endregion

            #region 奇偶个数

            string jo1, jo2, jo3, jo4, jo5 = string.Empty;

            if (array[0] % 2 == 1)
                jo1 = "J";
            else
                jo1 = "O";

            if (array[1] % 2 == 1)
                jo2 = "J";
            else
                jo2 = "O";

            if (array[2] % 2 == 1)
                jo3 = "J";
            else
                jo3 = "O";

            if (array[3] % 2 == 1)
                jo4 = "J";
            else
                jo4 = "O";

            if (array[4] % 2 == 1)
                jo5 = "J";
            else
                jo5 = "O";

            var arrayjo = new string[] { jo1, jo2, jo3, jo4, jo5 };
            #endregion
            var last = manager.QueryDLT_JiOu();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("QianQu", winArray[0]);
            dic.Add("JiOuQualifying", paiwei);
            dic.Add("JiOuBi", string.Format("{0}:{1}", jCount, 5 - jCount));
            var jiou_paiwei = new List<string>();
            var entity = this.CreateNewEntity<DLT_JiOu>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1"))
                {
                    var order = p.Name.Replace("NO1", string.Empty);
                    lastValue = arrayjo[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2"))
                {
                    var order = p.Name.Replace("NO2", string.Empty);
                    lastValue = arrayjo[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3"))
                {
                    var order = p.Name.Replace("NO3", string.Empty);
                    lastValue = arrayjo[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO4"))
                {
                    var order = p.Name.Replace("NO4", string.Empty);
                    lastValue = arrayjo[3] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO5"))
                {
                    var order = p.Name.Replace("NO5", string.Empty);
                    lastValue = arrayjo[4] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var bi = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(bi[0]) == jCount && int.Parse(bi[1]) == 5 - jCount ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddDLT_JiOu(entity);
        }

        /// <summary>
        /// 大乐透质和走势
        /// </summary>
        public void Import_ZhiHe(string issuseNumber, string winNumber)
        {
            var manager = new DLT_Manager();
            var issuse = manager.QueryDLT_ZhiHeIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var winRed = winArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var zhilist = new int[] { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]) };


            #region 质和个数

            string winxzhi1, winzhii2, winzhi3, winzhi4, winzhi5 = string.Empty;

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


            var arrayWinZhi = new string[] { winxzhi1, winzhii2, winzhi3, winzhi4, winzhi5 };
            #endregion

            #region  质和排位

            var paiwei = string.Empty;
            var zhi = "质";

            var he = "合";
            for (int k = 0; k < array.Length; k++)
            {
                if (zhilist.Contains(array[k]))
                {
                    paiwei += zhi;
                }
                else
                {
                    paiwei += he;
                }
            }

            #endregion

            int zhiCount = paiwei.ToArray().Count(p => p == '质');

            var last = manager.QueryDLT_ZhiHe();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("QianQu", winArray[0]);
            dic.Add("ZhiHeQualifying", paiwei);
            dic.Add("ZhiHeBi", string.Format("{0}:{1}", zhiCount, 5 - zhiCount));

            var entity = this.CreateNewEntity<DLT_ZhiHe>(dic, (p) =>
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
                    lastValue = int.Parse(bi[0]) == zhiCount && int.Parse(bi[1]) == 5 - zhiCount ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddDLT_ZhiHe(entity);

        }

        /// <summary>
        /// 大乐透和值走势
        /// </summary>
        public void Import_DLT_HeZhi(string issuseNumber, string winNumber)
        {
            var manager = new DLT_Manager();
            var issuse = manager.QueryDLT_HeZhiIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var winRed = winArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]) + int.Parse(winRed[3]) + int.Parse(winRed[4]);
            var hw = hz % 10;

            var last = manager.QueryDLT_HeZhi();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("QianQu", winArray[0]);
            dic.Add("HeZhi", hz);
            dic.Add("HeWei", hw);

            var entity = this.CreateNewEntity<DLT_HeZhi>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("HZ_"))
                {
                    var order = p.Name.Replace("HZ_", string.Empty);
                    var hzfb = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = hz >= int.Parse(hzfb[0]) && hz <= int.Parse(hzfb[1]) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("HW_"))
                {
                    var order = p.Name.Replace("HW_", string.Empty);
                    lastValue = int.Parse(order) == hw ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddDLT_HeZhi(entity);
        }

        /// <summary>
        /// 大乐透除3走势
        /// </summary>
        public void Import_DLT_Chu3(string issuseNumber, string winNumber)
        {
            var manager = new DLT_Manager();
            var issuse = manager.QueryDLT_Chu3IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var winRed = winArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var chu3Arrary = new int[] { int.Parse(winRed[0]) % 3, int.Parse(winRed[1]) % 3, int.Parse(winRed[2]) % 3, int.Parse(winRed[3]) % 3, int.Parse(winRed[4]) % 3 };
            var count_0 = chu3Arrary.Count(p => p == 0);
            var count_1 = chu3Arrary.Count(p => p == 1);
            var count_2 = chu3Arrary.Count(p => p == 2);

            var last = manager.QueryDLT_Chu3();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("QianQu", winArray[0]);
            dic.Add("Chu3Qualifying", string.Format("{0}{1}{2}{3}{4}", chu3Arrary[0], chu3Arrary[1], chu3Arrary[2], chu3Arrary[3], chu3Arrary[4]));
            dic.Add("Chu3Bi", string.Format("{0}:{1}:{2}", count_0, count_1, count_2));

            var entity = this.CreateNewEntity<DLT_Chu3>(dic, (p) =>
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

            manager.AddDLT_Chu3(entity);
        }

        /// <summary>
        /// 大乐透首尾跨度走势
        /// </summary>
        public void Import_DLT_KuaDu_SW(string issuseNumber, string winNumber)
        {
            var manager = new DLT_Manager();
            var issuse = manager.QueryDLT_KuaDu_SWIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var winRed = winArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var kd = int.Parse(winRed[4]) - int.Parse(winRed[0]);

            var last = manager.QueryDLT_KuaDu_SW();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("QianQu", winArray[0]);

            var entity = this.CreateNewEntity<DLT_KuaDu_SW>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("KD_"))
                {
                    var order = p.Name.Replace("KD_", string.Empty);
                    lastValue = kd == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddDLT_KuaDu_SW(entity);
        }

        /// <summary>
        /// 大乐透12跨度走势
        /// </summary>
        public void Import_DLT_KuaDu_12(string issuseNumber, string winNumber)
        {
            var manager = new DLT_Manager();
            var issuse = manager.QueryDLT_KuaDu_12IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var winRed = winArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var kd = int.Parse(winRed[1]) - int.Parse(winRed[0]);
            var last = manager.QueryDLT_KuaDu_12();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("QianQu", winArray[0]);

            var entity = this.CreateNewEntity<DLT_KuaDu_12>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("KD_"))
                {
                    var order = p.Name.Replace("KD_", string.Empty);
                    lastValue = kd == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddDLT_KuaDu_12(entity);
        }

        /// <summary>
        /// 大乐透23跨度走势
        /// </summary>
        public void Import_DLT_KuaDu_23(string issuseNumber, string winNumber)
        {
            var manager = new DLT_Manager();
            var issuse = manager.QueryDLT_KuaDu_23IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var winRed = winArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var kd = int.Parse(winRed[2]) - int.Parse(winRed[1]);

            var last = manager.QueryDLT_KuaDu_23();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("QianQu", winArray[0]);

            var entity = this.CreateNewEntity<DLT_KuaDu_23>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("KD_"))
                {
                    var order = p.Name.Replace("KD_", string.Empty);
                    lastValue = kd == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddDLT_KuaDu_23(entity);
        }

        /// <summary>
        /// 大乐透34跨度走势
        /// </summary>
        public void Import_DLT_KuaDu_34(string issuseNumber, string winNumber)
        {
            var manager = new DLT_Manager();
            var issuse = manager.QueryDLT_KuaDu_34IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var winRed = winArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var kd = int.Parse(winRed[3]) - int.Parse(winRed[2]);
            var last = manager.QueryDLT_KuaDu_34();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("QianQu", winArray[0]);

            var entity = this.CreateNewEntity<DLT_KuaDu_34>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("KD_"))
                {
                    var order = p.Name.Replace("KD_", string.Empty);
                    lastValue = kd == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddDLT_KuaDu_34(entity);
        }

        /// <summary>
        /// 大乐透45跨度走势
        /// </summary>
        public void Import_DLT_KuaDu_45(string issuseNumber, string winNumber)
        {
            var manager = new DLT_Manager();
            var issuse = manager.QueryDLT_KuaDu_45IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winArray = winNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var winRed = winArray[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var kd = int.Parse(winRed[4]) - int.Parse(winRed[3]);

            var last = manager.QueryDLT_KuaDu_45();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("QianQu", winArray[0]);

            var entity = this.CreateNewEntity<DLT_KuaDu_45>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("KD_"))
                {
                    var order = p.Name.Replace("KD_", string.Empty);
                    lastValue = kd == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddDLT_KuaDu_45(entity);
        }
        #endregion

        /// <summary>
        /// 查询开奖数据
        /// </summary>
        public GameWinNumber_InfoCollection QueryDLT_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new DLT_GameWinNumberManager().QueryDLT_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<DLT_GameWinNumber>, DLT_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryDLT_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new DLT_GameWinNumberManager().QueryDLT_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<DLT_GameWinNumber>, DLT_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
        public GameWinNumber_InfoCollection QueryDLT_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new DLT_GameWinNumberManager().QueryDLT_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<DLT_GameWinNumber>, DLT_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryDLT_GameWinNumber_{0}_{1}_{2}_{3}", pageIndex, pageSize, startTime.ToString("yyyyMMdd"), endTime.ToString("yyyyMMdd"));
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new DLT_GameWinNumberManager().QueryDLT_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<DLT_GameWinNumber>, DLT_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
            var manager = new DLT_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<DLT_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }
    }
}
