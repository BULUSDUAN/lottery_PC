using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.ORM.Helper.WinNumber.Manage;
using EntityModel;
using KaSon.FrameWork.Common.Utilities;

namespace KaSon.FrameWork.ORM.Helper.WinNumber
{
    public class LotteryDataBusiness_CTZQ : LotteryDataBusiness, ILotteryDataBusiness
    {
        public LotteryDataBusiness_CTZQ(string gameCode)
        {
            var gameArray = gameCode.ToUpper().Split('_');
            if (gameArray.Length != 2)
                throw new Exception("此:{0}彩种传入错误" + gameCode);
            this.CurrentGameCode = gameArray[0];
            this.CurrentGameType = gameArray[1];
        }

        public string CurrentGameCode { get; set; }
        public string CurrentGameType { get; set; }

        /// <summary>
        /// 添加开奖号
        /// </summary>
        public void ImportWinNumber(string issuseNumber, string winNumber)
        {
            this.ClearNewWinNumberCache("QueryCTZQ_GameWinNumberT14CTR9");
            this.ClearNewWinNumberCache("QueryCTZQ_GameWinNumberT6BQC");
            this.ClearNewWinNumberCache("QueryCTZQ_GameWinNumberT4CJQ");

            if (string.IsNullOrEmpty(issuseNumber)) return;
            if (string.IsNullOrEmpty(winNumber)) return;

            if (CurrentGameType == "TR9" || CurrentGameType == "T14C")
            {
                var GameWinNumberManager = new CTZQ_T14C_GameWinNumberManager();
                var result = GameWinNumberManager.QueryWinNumber(issuseNumber);
                if (result == null)
                {
                    GameWinNumberManager.AddCTZQ_T14C_GameWinNumber(new CTZQ_T14C_GameWinNumber
                    {
                        GameCode = "CTZQ",
                        IssuseNumber = issuseNumber,
                        WinNumber = winNumber,
                        GameType = CurrentGameType,
                        CreateTime = DateTime.Now.Date,
                    });
                }
                else
                {
                    UpdateGameWinNumber(issuseNumber, winNumber);
                }
            }
            if (CurrentGameType == "T6BQC")
            {
                var GameManager = new CTZQ_T6BQC_GameWinNumberManager();
                var result = GameManager.QueryWinNumber(issuseNumber);
                if (result == null)
                {
                    GameManager.AddCTZQ_T6BQC_GameWinNumber(new CTZQ_T6BQC_GameWinNumber
                    {
                        GameCode = "CTZQ",
                        GameType = CurrentGameType,
                        IssuseNumber = issuseNumber,
                        WinNumber = winNumber,
                        CreateTime = DateTime.Now.Date,
                    });
                }
                else
                {
                    UpdateGameWinNumber(issuseNumber, winNumber);
                }
            }
            if (CurrentGameType == "T4CJQ")
            {
                var GameWinNumberManager = new CTZQ_T4CJQ_GameWinNumberManager();
                var result = GameWinNumberManager.QueryWinNumber(issuseNumber);
                if (result == null)
                {
                    GameWinNumberManager.AddCTZQ_T4CJQ_GameWinNumber(new CTZQ_T4CJQ_GameWinNumber
                    {
                        GameCode = "CTZQ",
                        GameType = CurrentGameType,
                        IssuseNumber = issuseNumber,
                        WinNumber = winNumber,
                        CreateTime = DateTime.Now.Date,
                    });
                }
                else
                {
                    UpdateGameWinNumber(issuseNumber, winNumber);
                }
            }
        }

        /// <summary>
        /// 更新开奖号
        /// </summary>
        public void UpdateGameWinNumber(string issuseNumber, string winNumber)
        {
            this.ClearNewWinNumberCache("QueryCTZQ_GameWinNumberT14CTR9");
            this.ClearNewWinNumberCache("QueryCTZQ_GameWinNumberT6BQC");
            this.ClearNewWinNumberCache("QueryCTZQ_GameWinNumberT4CJQ");

            if (string.IsNullOrEmpty(issuseNumber)) return;
            if (string.IsNullOrEmpty(winNumber)) return;
            if (CurrentGameType == "TR9" || CurrentGameType == "T14C")
            {
                var GameWinNumberManager = new CTZQ_T14C_GameWinNumberManager();
                var result = GameWinNumberManager.QueryWinNumber(issuseNumber);
                if (result == null)
                    throw new Exception("此期号未找到：" + issuseNumber);
                result.WinNumber = winNumber;
                result.CreateTime = DateTime.Now.Date;
                GameWinNumberManager.UpdateCTZQ_T14C_GameWinNumber(result);
            }
            if (CurrentGameType == "T6BQC")
            {
                var GameWinNumberManager = new CTZQ_T6BQC_GameWinNumberManager();
                var result = GameWinNumberManager.QueryWinNumber(issuseNumber);
                if (result == null)
                    throw new Exception("此期号未找到：" + issuseNumber);
                var tempListWinNumber = result.WinNumber.Split(',');
                result.WinNumber = winNumber;
                result.CreateTime = DateTime.Now.Date;
                GameWinNumberManager.UpdateCTZQ_T6BQC_GameWinNumber(result);
            }
            if (CurrentGameType == "T4CJQ")
            {
                var GameWinNumberManager = new CTZQ_T4CJQ_GameWinNumberManager();
                var result = GameWinNumberManager.QueryWinNumber(issuseNumber);
                if (result == null)
                    throw new Exception("此期号未找到：" + issuseNumber);
                result.WinNumber = winNumber;
                result.CreateTime = DateTime.Now.Date;
                GameWinNumberManager.UpdateCTZQ_T4CJQ_GameWinNumber(result);
            }
        }

        /// <summary>
        /// 查询开奖数据
        /// </summary>
        public GameWinNumber_InfoCollection QueryCTZQ_GameWinNumber(int pageIndex, int pageSize)
        {
            switch (CurrentGameType)
            {
                case "TR9":
                    return QueryCTZQ_GameWinNumberTR9(pageIndex, pageSize);
                case "T14C":
                    return QueryCTZQ_GameWinNumberT14CTR9(pageIndex, pageSize);
                case "T6BQC":
                    return QueryCTZQ_GameWinNumberT6BQC(pageIndex, pageSize);
                case "T4CJQ":
                    return QueryCTZQ_GameWinNumberT4CJQ(pageIndex, pageSize);
            }
            return QueryCTZQ_GameWinNumberT14CTR9(pageIndex, pageSize);
        }
        public GameWinNumber_InfoCollection QueryCTZQ_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            switch (CurrentGameType)
            {
                case "T14C":
                case "TR9":
                    return QueryCTZQ_GameWinNumberT14CTR9(startTime, endTime, pageIndex, pageSize);
                case "T6BQC":
                    return QueryCTZQ_GameWinNumberT6BQC(startTime, endTime, pageIndex, pageSize);
                case "T4CJQ":
                    return QueryCTZQ_GameWinNumberT4CJQ(startTime, endTime, pageIndex, pageSize);
            }
            return QueryCTZQ_GameWinNumberT14CTR9(startTime, endTime, pageIndex, pageSize);
        }

        /// <summary>
        /// 查询开奖数据14场 9场
        /// </summary>
        public GameWinNumber_InfoCollection QueryCTZQ_GameWinNumberT14CTR9(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new CTZQ_T14C_GameWinNumberManager().QueryCTZQ_T14C_TR9_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<CTZQ_T14C_GameWinNumber>, CTZQ_T14C_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryCTZQ_GameWinNumberT14CTR9_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new CTZQ_T14C_GameWinNumberManager().QueryCTZQ_T14C_TR9_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<CTZQ_T14C_GameWinNumber>, CTZQ_T14C_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
        /// <summary>
        /// 查询开奖数据9场
        /// </summary>
        public GameWinNumber_InfoCollection QueryCTZQ_GameWinNumberTR9(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new CTZQ_T14C_GameWinNumberManager().QueryCTZQ_TR9_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<CTZQ_T14C_GameWinNumber>, CTZQ_T14C_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryCTZQ_GameWinNumberT14CTR9_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new CTZQ_T14C_GameWinNumberManager().QueryCTZQ_T14C_TR9_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<CTZQ_T14C_GameWinNumber>, CTZQ_T14C_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
        public GameWinNumber_InfoCollection QueryCTZQ_GameWinNumberT14CTR9(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new CTZQ_T14C_GameWinNumberManager().QueryCTZQ_T14C_TR9_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<CTZQ_T14C_GameWinNumber>, CTZQ_T14C_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryCTZQ_GameWinNumberT14CTR9_{0}_{1}_{2}_{3}", pageIndex, pageSize, startTime.ToString("yyyyMMdd"), endTime.ToString("yyyyMMdd"));
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new CTZQ_T14C_GameWinNumberManager().QueryCTZQ_T14C_TR9_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<CTZQ_T14C_GameWinNumber>, CTZQ_T14C_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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

        /// <summary>
        /// 查询开奖数据 6场
        /// </summary>
        public GameWinNumber_InfoCollection QueryCTZQ_GameWinNumberT6BQC(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new CTZQ_T6BQC_GameWinNumberManager().QueryCTZQ_T6BQC_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<CTZQ_T6BQC_GameWinNumber>, CTZQ_T6BQC_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryCTZQ_GameWinNumberT6BQC_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new CTZQ_T6BQC_GameWinNumberManager().QueryCTZQ_T6BQC_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<CTZQ_T6BQC_GameWinNumber>, CTZQ_T6BQC_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
        public GameWinNumber_InfoCollection QueryCTZQ_GameWinNumberT6BQC(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new CTZQ_T6BQC_GameWinNumberManager().QueryCTZQ_T6BQC_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<CTZQ_T6BQC_GameWinNumber>, CTZQ_T6BQC_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryCTZQ_GameWinNumberT6BQC_{0}_{1}_{2}_{3}", pageIndex, pageSize, startTime.ToString("yyyyMMdd"), endTime.ToString("yyyyMMdd"));
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new CTZQ_T6BQC_GameWinNumberManager().QueryCTZQ_T6BQC_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<CTZQ_T6BQC_GameWinNumber>, CTZQ_T6BQC_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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

        /// <summary>
        /// 查询开奖数据 4场
        /// </summary>
        public GameWinNumber_InfoCollection QueryCTZQ_GameWinNumberT4CJQ(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new CTZQ_T4CJQ_GameWinNumberManager().QueryCTZQ_T4CJQ_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<CTZQ_T4CJQ_GameWinNumber>, CTZQ_T4CJQ_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryCTZQ_GameWinNumberT4CJQ_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new CTZQ_T4CJQ_GameWinNumberManager().QueryCTZQ_T4CJQ_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<CTZQ_T4CJQ_GameWinNumber>, CTZQ_T4CJQ_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
        public GameWinNumber_InfoCollection QueryCTZQ_GameWinNumberT4CJQ(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new CTZQ_T4CJQ_GameWinNumberManager().QueryCTZQ_T4CJQ_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<CTZQ_T4CJQ_GameWinNumber>, CTZQ_T4CJQ_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryCTZQ_GameWinNumberT4CJQ_{0}_{1}_{2}_{3}", pageIndex, pageSize, startTime.ToString("yyyyMMdd"), endTime.ToString("yyyyMMdd"));
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new CTZQ_T4CJQ_GameWinNumberManager().QueryCTZQ_T4CJQ_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<CTZQ_T4CJQ_GameWinNumber>, CTZQ_T4CJQ_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
            switch (CurrentGameType)
            {
                case "TR9":
                case "T14C":
                    return QueryWinNumberT14C(issuseNumber);
                case "T6BQC":
                    return QueryWinNumberT6BQC(issuseNumber);
                case "T4CJQ":
                    return QueryWinNumberT4CJQ(issuseNumber);
                default:
                    break;
            }
            return QueryWinNumberT14C(issuseNumber);
        }


        public GameWinNumber_Info QueryWinNumberT4CJQ(string issuseNumber)
        {
            var manager = new CTZQ_T4CJQ_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<CTZQ_T4CJQ_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            info.GameType = entity.GameType;
            return info;
        }


        public GameWinNumber_Info QueryWinNumberT14C(string issuseNumber)
        {
            var manager = new CTZQ_T14C_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<CTZQ_T14C_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            info.GameType = entity.GameType;
            return info;
        }


        public GameWinNumber_Info QueryWinNumberT6BQC(string issuseNumber)
        {
            var manager = new CTZQ_T6BQC_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<CTZQ_T6BQC_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            info.GameType = entity.GameType;
            return info;
        }
    }
}
