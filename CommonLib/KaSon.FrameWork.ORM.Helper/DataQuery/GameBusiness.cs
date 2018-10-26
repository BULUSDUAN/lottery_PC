using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using GameBiz.Domain.Entities;
using KaSon.FrameWork.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    public class GameBusiness
    {

        ///// <summary>
        ///// 所有彩种状态
        ///// </summary>
        //public LotteryGameInfoCollection LotteryGame()
        //{
        //    var result = new LotteryGameInfoCollection();
        //    result.AddRange(new DataQuery().QueryLotteryGameList());
        //    return result;
        //}
        ///// <summary>
        ///// 更新彩种状态
        ///// </summary>
        //public void UpdateLotteryGame(string gameCode, int enableStatus)
        //{
        //    var manager = new LotteryGameManager();
        //    var entity = manager.QueryLotteryGame(gameCode);
        //    if (entity == null)
        //        throw new Exception("没有查到该彩种！");
        //    entity.EnableStatus = (EnableStatus)enableStatus;
        //    if (_gameList == null)
        //        _gameList = new GameInfoCollection();
        //    _gameList.Clear();
        //    manager.UpdateLotteryGame(entity);

        //    //清空缓存
        //    BusinessHelper.ReloadLotteryGame();
        //}
        /// <summary>
        /// 返回彩种状态
        /// </summary>
        public EnableStatus LotteryGameToStatus(string gameCode)
        {
            var manager = new LotteryGameManager();
            var entity = manager.QueryLotteryGame(gameCode);
            if (entity == null)
                throw new Exception("没有查到该彩种！");
            return (EnableStatus)entity.EnableStatus;
        }
        //public void AddGame(GameInfo info)
        //{
        //    var manager = new LotteryGameManager();
        //    manager.AddLotteryGame(new LotteryGame
        //    {
        //        DisplayName = info.DisplayName,
        //        GameCode = info.GameCode,
        //        EnableStatus = EnableStatus.Enable,
        //    });
        //}

        //public void AddGameType(GameTypeInfo info)
        //{
        //    var manager = new LotteryGameManager();
        //    manager.AddLotteryGameType(new LotteryGameType
        //    {
        //        DisplayName = info.DisplayName,
        //        EnableStatus = EnableStatus.Enable,
        //        Game = new LotteryGameManager().LoadGame(info.Game.GameCode),
        //        GameType = info.GameType,
        //        GameTypeId = string.Format("{0}|{1}", info.Game.GameCode, info.GameType),
        //    });
        //}

        //public void AddIssuseInfo(Issuse_AddInfo info)
        //{
        //    var manager = new LotteryGameManager();
        //    if (manager.GetGameIssuse(string.Format("{0}|{1}", info.Game.GameCode, info.IssuseNumber)) == null)
        //    {
        //        manager.AddGameIssuse(new GameIssuse
        //        {
        //            CreateTime = DateTime.Now,
        //            GameCode = info.Game.GameCode,
        //            GameCode_IssuseNumber = string.Format("{0}|{1}", info.Game.GameCode, info.IssuseNumber),
        //            GatewayStopTime = info.GatewayStopTime,
        //            IssuseNumber = info.IssuseNumber,
        //            LocalStopTime = info.LocalStopTime,
        //            OfficialStopTime = info.OfficialStopTime,
        //            StartTime = info.StartTime,
        //            Status = info.Status,
        //        });
        //    }
        //}

        //public GameTypeInfoCollection QueryGameTypeList(string gameCode)
        //{
        //    GameTypeInfoCollection result = new GameTypeInfoCollection();
        //    result.AddRange(new LotteryGameManager().QueryEnableGameType(gameCode));
        //    return result;
        //}

        public GameInfoCollection QueryGameInfoCollection()
        {
            var result = new GameInfoCollection();
            result.AddRange(new LotteryGameManager().QueryEnableGame());
            return result;
        }

        //public Issuse_QueryInfo QueryIssuseInfo(string gameCode, string gameType, string issuseNumber)
        //{
        //    var issuse = new LotteryGameManager().QueryGameIssuseByKey(gameCode, gameType, issuseNumber);
        //    if (issuse == null) return new Issuse_QueryInfo { Status = IssuseStatus.OnSale };
        //    return new Issuse_QueryInfo
        //    {
        //        CreateTime = issuse.CreateTime,
        //        GameCode_IssuseNumber = issuse.GameCode_IssuseNumber,
        //        Game = new GameInfo
        //        {
        //            //DisplayName = issuse.Game.DisplayName,
        //            GameCode = issuse.GameCode
        //        },
        //        GatewayStopTime = issuse.GatewayStopTime,
        //        IssuseNumber = issuse.IssuseNumber,
        //        LocalStopTime = issuse.LocalStopTime,
        //        OfficialStopTime = issuse.OfficialStopTime,
        //        StartTime = issuse.StartTime,
        //        Status = issuse.Status,
        //        WinNumber = issuse.WinNumber,
        //    };
        //}

        public Issuse_QueryInfo QueryCurrentIssuseInfo(string gameCode)
        {
            var entity = new DataQuery().QueryCurrentIssuse(gameCode);
            //if (entity == null) return null;
            return entity;
        }

        public IList<C_BJDC_Issuse> QueryCurrentBJDCIssuseInfo()
        {
            return new DataQuery().CurrentBJDCIssuseInfo();
          //  return entity;
        }
        public Issuse_QueryInfo QueryCurrentNewIssuseInfo(string gameCode, string gameType)
        {

            var entity = new DataQuery().QueryCurrentNewIssuseInfo(gameCode, gameType);
            if (entity == null) return null;
            var info = new Issuse_QueryInfo { Status = IssuseStatus.OnSale };
            ObjectConvert.ConverEntityToInfo<GameIssuse, Issuse_QueryInfo>(entity, ref info);
            var gameInfo = new GameInfo();
            gameInfo.GameCode = entity.GameCode;
            //ObjectConvert.ConverEntityToInfo<LotteryGame, GameInfo>(entity.Game, ref gameInfo);
            info.Game = gameInfo;
            return info;
        }
        //public Issuse_QueryInfo QueryCurrentIssuseInfoWithOffical(string gameCode)
        //{
        //    var entity = new LotteryGameManager().QueryCurrentIssuseWithOffical(gameCode);
        //    if (entity == null) return null;
        //    var info = new Issuse_QueryInfo { Status = IssuseStatus.OnSale };
        //    ObjectConvert.ConverEntityToInfo<GameIssuse, Issuse_QueryInfo>(entity, ref info);
        //    var gameInfo = new GameInfo();
        //    gameInfo.GameCode = entity.GameCode;
        //    //ObjectConvert.ConverEntityToInfo<LotteryGame, GameInfo>(entity.Game, ref gameInfo);
        //    info.Game = gameInfo;
        //    return info;
        //}

        //public IssuseStatus QueryIssuseStatus(string gameCode, string issuseNumber)
        //{
        //    return new LotteryGameManager().QueryIssuseStatus(string.Format("{0}|{1}", gameCode, issuseNumber));
        //}

        //public SiteSummaryInfo QuerySiteSummary()
        //{
        //    var balanceManager = new UserBalanceManager();
        //    var fundManager = new FundManager();
        //    var m = balanceManager.QueryCommonAndBonusMoney();
        //    return new SiteSummaryInfo
        //    {
        //        TodayRegisterUserCount = balanceManager.QueryRegisterUserCount(),
        //        MonthTotalFillMoney = fundManager.QueryMonthFillMoney(),
        //        MonthTotalWithdraw = fundManager.QueryMonthWithdraw(),
        //        TotalCommonMoney = m.TotalCommonMoney,
        //        TotalBonusMoney = m.TotalBonusMoney,
        //    };
        //}
        public void UpdateLotteryGame(string gameCode, int enableStatus)
        {
            var manager = new LotteryGameManager();
            var entity = manager.QueryLotteryGame(gameCode);
            if (entity == null)
                throw new Exception("没有查到该彩种！");
            entity.EnableStatus = enableStatus;
            manager.UpdateLotteryGame(entity);
        }
    }
}
