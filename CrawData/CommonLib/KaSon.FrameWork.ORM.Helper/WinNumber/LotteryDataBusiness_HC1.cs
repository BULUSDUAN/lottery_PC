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
    public class LotteryDataBusiness_HC1 : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get
            {
                return "HC1";
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
                this.ClearGameChartCache("QueryHC1_JBZS");
                this.ClearGameChartCache("QueryHC1_SXJJFWZS");
                this.ClearNewWinNumberCache("QueryHC1_GameWinNumber");

                AddHC1_SXJJFWZS(issuseNumber, winNumber);
                AddHC1_JBZS(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }

        #region 生成走势数据

        public void Add_GameWinNumber(string issuseNumber, string winNumber)
        {
            new KJGameIssuseBusiness().IssusePrize(this.CurrentGameCode, issuseNumber, winNumber);
            var manager = new HC1_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddHC1_GameWinNumber(new HC1_GameWinNumber
            {
                GameCode = "HC1",
                IssuseNumber = issuseNumber,
                WinNumber = winNumber,
                CreateTime = DateTime.Now,
            });
        }

        /// <summary>
        /// 添加基本走势
        /// </summary>
        private void AddHC1_JBZS(string issuseNumber, string winNumber)
        {
            var manager = new HC1_JBZSManager();
            var issuse = manager.QueryHC1_JBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var last = manager.QueryLastHC1_JBZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<HC1_JBZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_"))
                {
                    var order = p.Name.Replace("Red_", string.Empty);
                    lastValue = winNumber == order ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddHC1_JBZS(entity);
        }
        /// <summary>
        /// 添加生肖季节方位走势
        /// </summary>
        private void AddHC1_SXJJFWZS(string issuseNumber, string winNumber)
        {
            var manager = new HC1_SXJJFWZSManager();
            var issuse = manager.QueryHC1_SXJJFWZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var shengx = string.Empty;
            var SX = string.Empty;
            var jijie = string.Empty;
            var fangwei = string.Empty;
            var jj = string.Empty;
            var fw = string.Empty;
            if (winNumber == "01" || winNumber == "13" || winNumber == "25")
            {
                shengx = "鼠";
                SX = "shu";
            }
            if (winNumber == "02" || winNumber == "14" || winNumber == "26")
            {
                shengx = "牛";
                SX = "niu";
            }
            if (winNumber == "03" || winNumber == "15" || winNumber == "27")
            {
                shengx = "虎";
                SX = "hu";
            }
            if (winNumber == "04" || winNumber == "16" || winNumber == "28")
            {
                shengx = "兔";
                SX = "tu";
            }
            if (winNumber == "05" || winNumber == "17" || winNumber == "29")
            {
                shengx = "龙";
                SX = "long";
            }
            if (winNumber == "06" || winNumber == "18" || winNumber == "30")
            {
                shengx = "蛇";
                SX = "she";
            }
            if (winNumber == "07" || winNumber == "19" || winNumber == "31")
            {
                shengx = "马";
                SX = "ma";
            }
            if (winNumber == "08" || winNumber == "20" || winNumber == "32")
            {
                shengx = "羊";
                SX = "yang";
            }
            if (winNumber == "09" || winNumber == "21" || winNumber == "33")
            {
                shengx = "猴";
                SX = "hou";
            }
            if (winNumber == "10" || winNumber == "22" || winNumber == "34")
            {
                shengx = "鸡";
                SX = "ji";
            }
            if (winNumber == "11" || winNumber == "23" || winNumber == "35")
            {
                shengx = "狗";
                SX = "gou";
            }
            if (winNumber == "12" || winNumber == "24" || winNumber == "36")
            {
                shengx = "猪";
                SX = "zhu";
            }

            if (winNumber == "01" || winNumber == "02" || winNumber == "03" || winNumber == "04" || winNumber == "05" || winNumber == "06" || winNumber == "07" || winNumber == "08" || winNumber == "09")
            {
                jijie = "春";
                jj = "chun";
            }
            if (winNumber == "10" || winNumber == "11" || winNumber == "12" || winNumber == "13" || winNumber == "14" || winNumber == "15" || winNumber == "16" || winNumber == "17" || winNumber == "18")
            {
                jijie = "夏";
                jj = "xia";
            }
            if (winNumber == "19" || winNumber == "20" || winNumber == "21" || winNumber == "22" || winNumber == "23" || winNumber == "24" || winNumber == "25" || winNumber == "26" || winNumber == "27")
            {
                jijie = "秋";
                jj = "qiu";
            }
            if (winNumber == "28" || winNumber == "29" || winNumber == "30" || winNumber == "31" || winNumber == "32" || winNumber == "33" || winNumber == "34" || winNumber == "35" || winNumber == "36")
            {
                jijie = "冬";
                jj = "dong";
            }

            if (winNumber == "01" || winNumber == "03" || winNumber == "05" || winNumber == "07" || winNumber == "09" || winNumber == "11" || winNumber == "13" || winNumber == "15" || winNumber == "17")
            {
                fangwei = "东";
                fw = "dong";
            }
            if (winNumber == "02" || winNumber == "04" || winNumber == "06" || winNumber == "08" || winNumber == "10" || winNumber == "12" || winNumber == "14" || winNumber == "16" || winNumber == "18")
            {
                fangwei = "南";
                fw = "nan";
            }
            if (winNumber == "19" || winNumber == "21" || winNumber == "23" || winNumber == "25" || winNumber == "27" || winNumber == "29" || winNumber == "31" || winNumber == "33" || winNumber == "35")
            {
                fangwei = "西";
                fw = "xi";
            }
            if (winNumber == "20" || winNumber == "22" || winNumber == "24" || winNumber == "26" || winNumber == "28" || winNumber == "30" || winNumber == "32" || winNumber == "34" || winNumber == "36")
            {
                fangwei = "北";
                fw = "bei";
            }
            var last = manager.QueryLastHC1_SXJJFWZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);

            dic.Add("ShengX", shengx);
            dic.Add("JiJie", jijie);
            dic.Add("FangWei", fangwei);
            var entity = this.CreateNewEntity<HC1_SXJJFWZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("SX_"))
                {
                    var order = p.Name.Replace("SX_", string.Empty);
                    lastValue = SX == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("JJ_"))
                {
                    var order = p.Name.Replace("JJ_", string.Empty);
                    lastValue = jj == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("FW_"))
                {
                    var order = p.Name.Replace("FW_", string.Empty);
                    lastValue = fw == order ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddHC1_SXJJFWZS(entity);
        }
        #endregion

        #region 查询数据
        public HC1_JBZS_InfoCollection QueryHC1_JBZS(int index)
        {
            HC1_JBZS_InfoCollection Collection = new HC1_JBZS_InfoCollection();
            var list = this.QueryGameChart<HC1_JBZS_Info>(string.Format("QueryHC1_JBZS_{0}", index), () =>
            {
                var infoList = new List<HC1_JBZS_Info>();
                var entityList = new HC1_JBZSManager().QueryHC1_JBZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<HC1_JBZS>, HC1_JBZS, List<HC1_JBZS_Info>, HC1_JBZS_Info>(entityList, ref infoList,
                    () => { return new HC1_JBZS_Info(); },
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
        public HC1_SXJJFWZS_InfoCollection QueryHC1_SXJJFWZS(int index)
        {
            HC1_SXJJFWZS_InfoCollection Collection = new HC1_SXJJFWZS_InfoCollection();
            var list = this.QueryGameChart<HC1_SXJJFWZS_Info>(string.Format("QueryHC1_SXJJFWZS_{0}", index), () =>
            {
                var infoList = new List<HC1_SXJJFWZS_Info>();
                var entityList = new HC1_SXJJFWZSManager().QueryHC1_SXJJFWZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<HC1_SXJJFWZS>, HC1_SXJJFWZS, List<HC1_SXJJFWZS_Info>, HC1_SXJJFWZS_Info>(entityList, ref infoList,
                    () => { return new HC1_SXJJFWZS_Info(); },
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
        public GameWinNumber_InfoCollection QueryHC1_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new HC1_GameWinNumberManager().QueryHC1_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<HC1_GameWinNumber>, HC1_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryHC1_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new HC1_GameWinNumberManager().QueryHC1_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<HC1_GameWinNumber>, HC1_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
            var manager = new HC1_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<HC1_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }
    }
}
