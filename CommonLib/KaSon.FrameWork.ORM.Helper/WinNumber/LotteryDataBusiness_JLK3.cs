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
    public class LotteryDataBusiness_JLK3 : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get
            {
                return "JLK3";
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

                this.ClearGameChartCache("QueryJLK3_JBZS_Info");
                this.ClearGameChartCache("QueryJLK3_HZ_Info");
                this.ClearGameChartCache("QueryJLK3_XT_Info");
                this.ClearGameChartCache("QueryJLK3_ZH_Info");
                this.ClearGameChartCache("QueryJLK3_ZHZS_Info");
                this.ClearNewWinNumberCache("QueryJLK3_GameWinNumber");

                Import_JBZS(issuseNumber, winNumber);
                Import_HZ(issuseNumber, winNumber);
                Import_XT(issuseNumber, winNumber);
                Import_ZH(issuseNumber, winNumber);
                Import_ZHZS(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }

        #region 前台查询数据

        /// <summary>
        ///查询基本走势列表按时间倒叙 
        /// </summary>
        public JLK3_JBZS_InfoCollection QueryJLK3_JBZS_Info(int length)
        {
            JLK3_JBZS_InfoCollection Collection = new JLK3_JBZS_InfoCollection();
            var list = this.QueryGameChart<JLK3_JBZS_Info>(string.Format("QueryJLK3_JBZS_Info_{0}", length), () =>
            {
                var infoList = new List<JLK3_JBZS_Info>();
                var entityList = new JLK3_Manager().QueryJLK3_JBZS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JLK3_JBZS>, JLK3_JBZS, List<JLK3_JBZS_Info>, JLK3_JBZS_Info>(entityList, ref infoList,
                    () => { return new JLK3_JBZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询和值走势列表按时间倒叙 
        /// </summary>
        public JLK3_HZ_InfoCollection QueryJLK3_HZ_Info(int length)
        {
            JLK3_HZ_InfoCollection Collection = new JLK3_HZ_InfoCollection();
            var list = this.QueryGameChart<JLK3_HZ_Info>(string.Format("QueryJLK3_HZ_Info_{0}", length), () =>
            {
                var infoList = new List<JLK3_HZ_Info>();
                var entityList = new JLK3_Manager().QueryJLK3_HZ_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JLK3_HZ>, JLK3_HZ, List<JLK3_HZ_Info>, JLK3_HZ_Info>(entityList, ref infoList,
                    () => { return new JLK3_HZ_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询形态走势列表按时间倒叙 
        /// </summary>
        public JLK3_XT_InfoCollection QueryJLK3_XT_Info(int length)
        {
            JLK3_XT_InfoCollection Collection = new JLK3_XT_InfoCollection();
            var list = this.QueryGameChart<JLK3_XT_Info>(string.Format("QueryJLK3_XT_Info_{0}", length), () =>
            {
                var infoList = new List<JLK3_XT_Info>();
                var entityList = new JLK3_Manager().QueryJLK3_XT_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JLK3_XT>, JLK3_XT, List<JLK3_XT_Info>, JLK3_XT_Info>(entityList, ref infoList,
                    () => { return new JLK3_XT_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询组合走势列表按时间倒叙 
        /// </summary>
        public JLK3_ZH_InfoCollection QueryJLK3_ZH_Info(int length)
        {
            JLK3_ZH_InfoCollection Collection = new JLK3_ZH_InfoCollection();
            var list = this.QueryGameChart<JLK3_ZH_Info>(string.Format("QueryJLK3_ZH_Info_{0}", length), () =>
            {
                var infoList = new List<JLK3_ZH_Info>();
                var entityList = new JLK3_Manager().QueryJLK3_ZH_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JLK3_ZH>, JLK3_ZH, List<JLK3_ZH_Info>, JLK3_ZH_Info>(entityList, ref infoList,
                    () => { return new JLK3_ZH_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询综合走势列表按时间倒叙 
        /// </summary>
        public JLK3_ZHZS_InfoCollection QueryJLK3_ZHZS_Info(int length)
        {
            JLK3_ZHZS_InfoCollection Collection = new JLK3_ZHZS_InfoCollection();
            var list = this.QueryGameChart<JLK3_ZHZS_Info>(string.Format("QueryJLK3_ZHZS_Info_{0}", length), () =>
            {
                var infoList = new List<JLK3_ZHZS_Info>();
                var entityList = new JLK3_Manager().QueryJLK3_ZHZS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JLK3_ZHZS>, JLK3_ZHZS, List<JLK3_ZHZS_Info>, JLK3_ZHZS_Info>(entityList, ref infoList,
                    () => { return new JLK3_ZHZS_Info(); });
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
            var manager = new JLK3_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddJLK3_GameWinNumber(new JLK3_GameWinNumber
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
            var manager = new JLK3_Manager();
            var issuse = manager.QueryJLK3_JBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]) };

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

            #region 奇偶 大小个数

            int jCount = 0;
            int oCount = 0;
            foreach (var item in array)
            {
                if (item % 2 == 1)
                    jCount++;
                else
                    oCount++;
            }


            int dCount = 0;
            int xCount = 0;
            foreach (var item in array)
            {
                if (item <= 3)
                    xCount++;
                else
                    dCount++;
            }
            #endregion

            var last = manager.QueryJLK3_JBZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<JLK3_JBZS>(dic, (p) =>
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
                if (p.Name.StartsWith("JiCount_"))
                {
                    var order = p.Name.Replace("JiCount_", string.Empty);
                    lastValue = jCount == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("XiaoCount_"))
                {
                    var order = p.Name.Replace("XiaoCount_", string.Empty);
                    lastValue = xCount == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("He_"))
                {
                    var order = p.Name.Replace("He_", string.Empty);
                    lastValue = array[0] + array[1] + array[2] == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJLK3_JBZS(entity);

        }

        /// <summary>
        /// 和值走势
        /// </summary>
        private void Import_HZ(string issuseNumber, string winNumber)
        {
            var manager = new JLK3_Manager();
            var issuse = manager.QueryJLK3_HZIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]);
            var hw = hz % 10;

            var last = manager.QueryJLK3_HZ();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<JLK3_HZ>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("He_"))
                {
                    var order = p.Name.Replace("He_", string.Empty);
                    lastValue = hz == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("HW_"))
                {
                    var order = p.Name.Replace("HW_", string.Empty);
                    lastValue = hw == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu_"))
                {
                    var order = p.Name.Replace("Yu_", string.Empty);
                    lastValue = hz % 3 == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJLK3_HZ(entity);

        }

        /// <summary>
        /// 形态走势
        /// </summary>
        public void Import_XT(string issuseNumber, string winNumber)
        {
            var manager = new JLK3_Manager();
            var issuse = manager.QueryJLK3_XTIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]);

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

            var last = manager.QueryJLK3_XT();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<JLK3_XT>(dic, (p) =>
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
                if (p.Name.StartsWith("XT_3T"))
                {
                    lastValue = type == "豹子" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("XT_3BT"))
                {
                    lastValue = type == "组六" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("XT_2T"))
                {
                    lastValue = type == "组三" ? 0 : lastValue;
                }
                if (p.Name.StartsWith("XT_2BT"))
                {
                    if (type == "组三" || type == "组六")
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("KD_"))
                {
                    var order = p.Name.Replace("KD_", string.Empty);
                    lastValue = int.Parse(winRed[2]) - int.Parse(winRed[0]) == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("He_"))
                {
                    var order = p.Name.Replace("He_", string.Empty);
                    lastValue = hz == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJLK3_XT(entity);

        }

        /// <summary>
        /// 组合走势
        /// </summary>
        public void Import_ZH(string issuseNumber, string winNumber)
        {
            var manager = new JLK3_Manager();
            var issuse = manager.QueryJLK3_ZHIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]);

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

            var last = manager.QueryJLK3_ZH();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<JLK3_ZH>(dic, (p) =>
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
                if (p.Name.StartsWith("AA"))
                {
                    var order = p.Name.Replace("AA", string.Empty);
                    if (type == "组三")
                    {
                        lastValue = t + t == order ? 0 : lastValue;
                    }
                    if (type == "豹子")
                    {
                        lastValue = winRed[0] + winRed[2] == order ? 0 : lastValue;
                    }
                }
                if (p.Name.StartsWith("AB"))
                {
                    var order = p.Name.Replace("AB", string.Empty);
                    if (winRed[0] + winRed[1] == order || winRed[0] + winRed[2] == order || winRed[1] + winRed[2] == order)
                    {
                        lastValue = 0;
                    }
                }
                if (p.Name.StartsWith("KD_"))
                {
                    var order = p.Name.Replace("KD_", string.Empty);
                    lastValue = int.Parse(winRed[2]) - int.Parse(winRed[0]) == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("HZ"))
                {
                    lastValue = hz;
                }
                return lastValue;
            });

            manager.AddJLK3_ZH(entity);

        }

        /// <summary>
        /// 综合走势
        /// </summary>
        public void Import_ZHZS(string issuseNumber, string winNumber)
        {
            var manager = new JLK3_Manager();
            var issuse = manager.QueryJLK3_ZHZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]);

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

            var last = manager.QueryJLK3_ZHZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<JLK3_ZHZS>(dic, (p) =>
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
                if (p.Name.StartsWith("BW_"))
                {
                    var order = p.Name.Replace("BW_", string.Empty);
                    lastValue = int.Parse(winRed[0]) == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("SW_"))
                {
                    var order = p.Name.Replace("SW_", string.Empty);
                    lastValue = int.Parse(winRed[1]) == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("GW_"))
                {
                    var order = p.Name.Replace("GW_", string.Empty);
                    lastValue = int.Parse(winRed[2]) == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJLK3_ZHZS(entity);

        }
        #endregion

        /// <summary>
        /// 查询开奖数据
        /// </summary>
        public GameWinNumber_InfoCollection QueryJLK3_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new JLK3_GameWinNumberManager().QueryJLK3_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<JLK3_GameWinNumber>, JLK3_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryJLK3_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new JLK3_GameWinNumberManager().QueryJLK3_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<JLK3_GameWinNumber>, JLK3_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
            var manager = new JLK3_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<JLK3_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }
    }
}
