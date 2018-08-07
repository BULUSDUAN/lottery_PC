using EntityModel.CoreModel;
using EntityModel.Enum;
using KaSon.FrameWork.ORM.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common;

namespace KaSon.FrameWork.PlugIn.External
{
    public class BDFXOrderBusiness :DBbase, IComplateTicket, IOrderPrize_AfterTranCommit
    {
        private static Log4Log writerLog = new Log4Log();
        /// <summary>
        /// 宝单分享
        /// </summary>
        public void ComplateTicket(string userId, string schemeId, decimal totalMoney, decimal totalErrorMoney)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(schemeId))
                    return;
                var orderManager = new Sports_Manager();
                var sd_Manager = new TotalSingleTreasureManager();
                var manager = new BDFXManager();
                var orderRunning = orderManager.QuerySports_Order_Running(schemeId);
                if (orderRunning == null)
                    return;
                if (orderRunning.SchemeType == (int)SchemeType.SingleTreasure)
                {
                    var saveOrderEntity = orderManager.QuerySaveOrder(schemeId);
                    if (saveOrderEntity == null) return;
                    var BDFXOrderEntity = sd_Manager.QueryTotalSingleTreasureBySchemeId(schemeId);
                    var anteCodeList = orderManager.QuerySportsAnteCodeBySchemeId(schemeId);
                    DateTime firstMatchStopTime = new DateTime();
                    DateTime lastMatchStopTime = new DateTime();
                    if (orderRunning.GameCode.ToUpper() == "JCZQ")
                    {
                        var matchIdArray = (from l in anteCodeList select l.MatchId).Distinct().ToArray();
                        var matchList = orderManager.QueryJCZQSaleMatchCount(matchIdArray);
                        firstMatchStopTime = matchList.Min(s => s.FSStopBettingTime);
                        lastMatchStopTime = matchList.Max(s => s.FSStopBettingTime);
                    }
                    if (BDFXOrderEntity == null || string.IsNullOrEmpty(BDFXOrderEntity.UserId))//晒单
                    {
                        var epectedBonusMoney = 0M;
                        var ticketList = orderManager.QueryTicketList(schemeId);
                        if (ticketList != null && ticketList.Count > 0)
                        {
                            foreach (var item in ticketList)
                            {
                                var minMoney = 0M;
                                var maxMoney = 0M;
                                var playCount = 0;
                                if (!string.IsNullOrEmpty(item.PlayType))
                                {
                                    var tempArray = item.PlayType.Replace("P", "").Split('_');
                                    if (tempArray.Length >= 2)
                                    {
                                        playCount = Convert.ToInt32(tempArray[1]);
                                    }
                                }

                                if (string.IsNullOrEmpty(item.PlayType) || playCount <= 1)
                                {
                                    var winMoneyList = string.IsNullOrEmpty(item.BetContent) || string.IsNullOrEmpty(item.LocOdds) ? null : Common.Utilities.UsefullHelper.GetTicketMinMoneyOrMaxMoney(item.BetContent, item.LocOdds, out minMoney, out maxMoney);
                                }
                                else
                                {
                                    Common.Utilities.UsefullHelper.GetTicketMinMoneyOrMaxMoney_MN(item.PlayType.Replace("P", ""), item.BetContent, item.LocOdds, out minMoney, out maxMoney);
                                }
                                epectedBonusMoney += maxMoney;
                            }
                        }
                        epectedBonusMoney = Convert.ToDecimal(epectedBonusMoney.ToString("N2"));
                        var expectedReturnRate = 0M;
                        if (orderRunning.TotalMoney > 0)
                        {
                            expectedReturnRate = epectedBonusMoney / orderRunning.TotalMoney;
                            expectedReturnRate = Math.Truncate(expectedReturnRate * 100) / 100M;
                        }
                        BDFXOrderEntity = new C_TotalSingleTreasure();
                        BDFXOrderEntity.Commission = saveOrderEntity.BDFXCommission;
                        BDFXOrderEntity.CreateTime = DateTime.Now;
                        BDFXOrderEntity.CurrentBetMoney = orderRunning.TotalMoney;
                        BDFXOrderEntity.ExpectedBonusMoney = epectedBonusMoney;
                        BDFXOrderEntity.ExpectedReturnRate = expectedReturnRate;
                        BDFXOrderEntity.FirstMatchStopTime = firstMatchStopTime;
                        BDFXOrderEntity.IsBonus = false;
                        BDFXOrderEntity.IsComplate = false;
                        BDFXOrderEntity.LastMatchStopTime = lastMatchStopTime.AddMinutes(120);
                        BDFXOrderEntity.TotalBuyCount = 0;
                        BDFXOrderEntity.TotalBuyMoney = 0;
                        BDFXOrderEntity.TotalBonusMoney = 0;
                        BDFXOrderEntity.ProfitRate = 0;
                        BDFXOrderEntity.SchemeId = orderRunning.SchemeId;
                        BDFXOrderEntity.TotalCommissionMoney = 0;
                        BDFXOrderEntity.UserId = orderRunning.UserId;
                        BDFXOrderEntity.SingleTreasureDeclaration = saveOrderEntity.SingleTreasureDeclaration;
                        BDFXOrderEntity.Security = orderRunning.Security;
                        sd_Manager.AddTotalSingleTreasure(BDFXOrderEntity);
                    }
                    var singleTreasureAttentionSummary = manager.QuerySingleTreasureAttentionSummaryByUserId(orderRunning.UserId);
                    if (singleTreasureAttentionSummary == null || string.IsNullOrEmpty(singleTreasureAttentionSummary.UserId))//初始化关注统计表
                    {
                        C_SingleTreasure_AttentionSummary entity = new C_SingleTreasure_AttentionSummary();
                        entity.UserId = orderRunning.UserId;
                        entity.BeConcernedUserCount = 0;
                        entity.ConcernedUserCount = 0;
                        entity.SingleTreasureCount += 1;
                        entity.UpdateTime = DateTime.Now;
                        manager.AddSingleTreasureAttentionSummary(entity);
                    }
                    else
                    {
                        singleTreasureAttentionSummary.SingleTreasureCount += 1;
                        manager.UpdateSingleTreasureAttentionSummary(singleTreasureAttentionSummary);
                    }
                }
                else if (orderRunning.SchemeType == (int)SchemeType.SingleCopy)//抄单
                {
                    var recordSingleCopyEntity = sd_Manager.QueryBDFXRecordSingleCopyBySchemeId(schemeId);
                    if (recordSingleCopyEntity != null)
                    {
                        var bdEntity = sd_Manager.QueryTotalSingleTreasureBySchemeId(recordSingleCopyEntity.BDXFSchemeId);
                        if (bdEntity != null)
                        {
                            bdEntity.TotalBuyCount += 1;
                            bdEntity.TotalBuyMoney += totalMoney;
                            sd_Manager.UpdateTotalSingleTreasure(bdEntity);
                        }
                    }
                }
            }
            catch
            {
            }
        }
        /// <summary>
        /// 派奖后计算宝单分享相关
        /// </summary>
        public void OrderPrize_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal orderMoney, bool isBonus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney, bool isVirtualOrder, DateTime prizeTime)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(schemeId))
                return;
            var orderManager = new Sports_Manager();
            var bdfxManager = new BDFXManager();
            var orderComplate = orderManager.QuerySports_Order_Complate(schemeId);

            if (orderComplate == null) return;
            if (orderComplate.SchemeType == (int)SchemeType.SingleTreasure)//宝单
            {
                var bdfxEntity = bdfxManager.QueryTotalSingleTreasureBySchemeId(schemeId);
                if (bdfxEntity != null)
                {
                    bdfxEntity.CurrBonusMoney = afterTaxBonusMoney;
                    bdfxEntity.IsBonus = true;
                    bdfxEntity.IsComplate = true;
                    if (bdfxEntity.CurrentBetMoney > 0)
                    {
                        var currProfitRate = Math.Truncate(((afterTaxBonusMoney - bdfxEntity.CurrentBetMoney) / bdfxEntity.CurrentBetMoney) * 100) / 100;
                        bdfxEntity.CurrProfitRate = currProfitRate;
                    }
                    else
                        bdfxEntity.CurrProfitRate = 0;
                    bdfxManager.UpdateTotalSingleTreasure(bdfxEntity);
                }
            }
            else if (orderComplate.SchemeType == (int)SchemeType.SingleCopy && afterTaxBonusMoney > 0)//抄单
            {
                var manager = new Sports_Manager();
                var order = manager.QuerySports_Order_Complate(schemeId);
                if (order == null)
                    throw new LogicException(string.Format("自动派钱，没有找到订单{0}", schemeId));
                var bdfxRecorSingleEntity = bdfxManager.QueryBDFXRecordSingleCopyBySchemeId(schemeId);
                if (bdfxRecorSingleEntity != null)
                {
                    var BDFXEntity = bdfxManager.QueryTotalSingleTreasureBySchemeId(bdfxRecorSingleEntity.BDXFSchemeId);
                    if (BDFXEntity != null)
                    {
                        BDFXEntity.TotalBonusMoney += afterTaxBonusMoney;
                        if (BDFXEntity.TotalBuyMoney > 0 && BDFXEntity.TotalBonusMoney != 0)
                        {
                            var profiteRate = (BDFXEntity.TotalBonusMoney - BDFXEntity.TotalBuyMoney) / BDFXEntity.TotalBuyMoney;
                            BDFXEntity.ProfitRate = Math.Truncate(profiteRate * 100) / 100M;
                        }
                        else
                            BDFXEntity.ProfitRate = 0;
                        //var commissionMoney = afterTaxBonusMoney * BDFXEntity.Commission / 100;
                        //BDFXEntity.TotalCommissionMoney += commissionMoney;
                        var commissionMoney = (order.AfterTaxBonusMoney - order.TotalMoney) * BDFXEntity.Commission / 100M;
                        commissionMoney = Math.Truncate(commissionMoney * 100) / 100M;
                        if (commissionMoney > 0)
                            BDFXEntity.TotalCommissionMoney += commissionMoney;
                        else
                            BDFXEntity.TotalCommissionMoney = 0;
                        bdfxManager.UpdateTotalSingleTreasure(BDFXEntity);
                    }
                }
            }

        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IComplateTicket":
                        ComplateTicket((string)paraList[0], (string)paraList[1], (decimal)paraList[2], (decimal)paraList[3]);
                        break;
                    case "IOrderPrize_AfterTranCommit":
                        OrderPrize_AfterTranCommit((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3], (string)paraList[4], (decimal)paraList[5], (bool)paraList[6], (decimal)paraList[7], (decimal)paraList[8], (bool)paraList[9], (DateTime)paraList[10]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
                }
                return null;
            }
            catch (LogicException)
            {
            }
            catch (Exception ex)
            {

                writerLog.ErrrorLog("EXEC_Plugin_AddA20140902_Business购彩不花钱_Error_", ex);
            }

            return null;
        }

        //public TotalSingleTreasure_Collection QueryTodayBDFXList(string userId, string userName, string gameCode, string strOrderBy, string currentUserId, DateTime startTime, DateTime endTime, string isMyBD, int pageIndex, int pageSize)
        //{
        //    var manager = new BDFXManager();
            
        //        string orderBy = "bdfxcreatetime";
        //        string desc = "desc";
        //        if (!string.IsNullOrEmpty(strOrderBy))
        //        {
        //            var array = strOrderBy.ToLower().Split('|');
        //            if (array != null && array.Length > 1)
        //            {
        //                orderBy = array[0].ToString();
        //                desc = array[1].ToString();
        //            }
        //        }
        //        startTime = startTime.Date;
        //        endTime = endTime.AddDays(1).Date;
        //        var bdfxList = manager.QueryTodayBDFXList(userId, userName, gameCode, orderBy, desc, startTime, endTime, isMyBD, pageIndex, pageSize);
        //        if (!string.IsNullOrEmpty(currentUserId) && bdfxList != null && bdfxList.TotalCount > 0)
        //        {
        //            var userIdList = manager.QueryBeConcernedUserIdList(currentUserId);
        //            var singleTraList = bdfxList.TotalSingleTreasureList.Where(s => userIdList.ToArray().Contains(s.UserId)).ToList();
        //            bdfxList.TotalSingleTreasureList = singleTraList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        //            bdfxList.TotalCount = singleTraList.Count;
        //            return bdfxList;
        //        }
        //        bdfxList.TotalSingleTreasureList = bdfxList.TotalSingleTreasureList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        //        return bdfxList;
            
        //}
        /// <summary>
        /// 查询关注(关注总数、被关注总数、晒单总数等)
        /// </summary>
        public ConcernedInfo QueryConcernedByUserId(string bdfxUserId, string currUserId, string startTime, string endTime)
        {
                var manager = new BDFXManager();            
                var sTime = new DateTime();
                var eTime = new DateTime();
                if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                {
                    startTime = startTime.Replace('.', '-');
                    endTime = endTime.Replace('.', '-');
                    sTime = Convert.ToDateTime(DateTime.Now.Year + "-" + startTime).Date;
                    eTime = Convert.ToDateTime(DateTime.Now.Year + "-" + endTime).AddDays(1).Date;
                }
                else if (string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                {
                    var currTime = DateTime.Now;
                    int day = Convert.ToInt32(currTime.DayOfWeek) - 1;
                    if (currTime.DayOfWeek != 0)
                        currTime = currTime.AddDays(-day);
                    else
                        currTime = currTime.AddDays(-6);
                    sTime = currTime.AddDays(-7).Date;
                    eTime = currTime.Date;
                }
                return manager.QueryConcernedByUserId(bdfxUserId, currUserId, sTime, eTime);
            
        }
        /// <summary>
        /// 关注
        /// </summary>
        public void BDFXAttention(string currUserId, string bgzUserId)
        {

            try
            {
                #region 关注
                DB.Begin();
                var manager = new BDFXManager();
                if (string.IsNullOrEmpty(currUserId))
                    throw new Exception("关注人编号不能为空");
                else if (string.IsNullOrEmpty(bgzUserId))
                    throw new Exception("被关注人编号不能为空");
                var singleTreasureAttention = manager.QuerySingleTreasureAttentionByUserId(bgzUserId, currUserId);
                if (singleTreasureAttention != null && !string.IsNullOrEmpty(singleTreasureAttention.ConcernedUserId))
                    throw new Exception("您已经关注了他");
                if (currUserId == bgzUserId)
                    throw new Exception("不能关注自己");
                singleTreasureAttention = new C_SingleTreasure_Attention();
                singleTreasureAttention.BeConcernedUserId = bgzUserId;
                singleTreasureAttention.ConcernedUserId = currUserId;
                singleTreasureAttention.CreateTime = DateTime.Now;
                manager.AddSingleTreasureAttention(singleTreasureAttention);
                //修改被关注者信息
                var BGZSummary = manager.QuerySingleTreasureAttentionSummaryByUserId(bgzUserId);
                if (BGZSummary != null && !string.IsNullOrEmpty(BGZSummary.UserId))
                {
                    BGZSummary.BeConcernedUserCount += 1;
                    BGZSummary.UpdateTime = DateTime.Now;
                    manager.UpdateSingleTreasureAttentionSummary(BGZSummary);
                }
                else
                {
                    BGZSummary = new C_SingleTreasure_AttentionSummary();
                    BGZSummary.BeConcernedUserCount = 1;
                    BGZSummary.ConcernedUserCount = 0;
                    BGZSummary.SingleTreasureCount = 0;
                    BGZSummary.UpdateTime = DateTime.Now;
                    BGZSummary.UserId = bgzUserId;
                    manager.AddSingleTreasureAttentionSummary(BGZSummary);
                }
                //修改关注者信息
                var GZSummary = manager.QuerySingleTreasureAttentionSummaryByUserId(currUserId);
                if (GZSummary != null && !string.IsNullOrEmpty(GZSummary.UserId))
                {
                    GZSummary.ConcernedUserCount += 1;
                    GZSummary.UpdateTime = DateTime.Now;
                    manager.UpdateSingleTreasureAttentionSummary(GZSummary);
                }
                else
                {
                    GZSummary = new C_SingleTreasure_AttentionSummary();
                    GZSummary.BeConcernedUserCount = 0;
                    GZSummary.ConcernedUserCount = 1;
                    GZSummary.SingleTreasureCount = 0;
                    GZSummary.UpdateTime = DateTime.Now;
                    GZSummary.UserId = currUserId;
                    manager.AddSingleTreasureAttentionSummary(GZSummary);
                }
                DB.Commit();
                #endregion
            }
            catch (Exception EX)
            {
                DB.Rollback();
                throw EX;
            }
            
        }
        /// <summary>
        /// 取消关注
        /// </summary>
        public void BDFXCancelAttention(string currUserId, string bgzUserId)
        {

            try
            {
                #region 取消关注
                DB.Begin();
                var manager = new BDFXManager();
                var singleTreasureAttention = manager.QuerySingleTreasureAttentionByUserId(bgzUserId, currUserId);
                if (singleTreasureAttention == null || string.IsNullOrEmpty(singleTreasureAttention.ConcernedUserId))
                    throw new Exception("您还未关注他");
                manager.DeleteSingleTreasureAttention(singleTreasureAttention);
                //修改被关注者信息
                var BGZSummary = manager.QuerySingleTreasureAttentionSummaryByUserId(bgzUserId);
                if (BGZSummary != null && !string.IsNullOrEmpty(BGZSummary.UserId))
                {
                    BGZSummary.BeConcernedUserCount -= 1;
                    BGZSummary.UpdateTime = DateTime.Now;
                    manager.UpdateSingleTreasureAttentionSummary(BGZSummary);
                }

                //修改关注者信息
                var GZSummary = manager.QuerySingleTreasureAttentionSummaryByUserId(currUserId);
                if (GZSummary != null && !string.IsNullOrEmpty(GZSummary.UserId))
                {
                    GZSummary.ConcernedUserCount -= 1;
                    GZSummary.UpdateTime = DateTime.Now;
                    manager.UpdateSingleTreasureAttentionSummary(GZSummary);
                }
                DB.Commit();
                #endregion
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            
        }
        /// <summary>
        /// 查询宝单详情
        /// </summary>
        public BDFXOrderDetailInfo QueryBDFXOrderDetailBySchemeId(string schemeId)
        {
            var BDFXManager = new BDFXManager();
            var sportsBusiness = new Sports_Business();

            var orderDetailInfo = BDFXManager.QueryBDFXOrderDetailBySchemeId(schemeId);
            if (orderDetailInfo != null && !string.IsNullOrEmpty(orderDetailInfo.SchemeId))
            {
                orderDetailInfo.AnteCodeCollection = new Sports_AnteCodeQueryInfoCollection();
                orderDetailInfo.AnteCodeList = new List<AnteCodeInfo>();
                orderDetailInfo.NearTimeProfitRateCollection = new NearTimeProfitRate_Collection();
                var anteCodeCollection = sportsBusiness.QuerySportsOrderAnteCodeList(schemeId);
                if (anteCodeCollection != null)
                    orderDetailInfo.AnteCodeCollection = anteCodeCollection;
                var nearTimeProfitInfo = BDFXManager.QueryNearTimeProfitRate(orderDetailInfo.UserId);
                if (nearTimeProfitInfo != null)
                    orderDetailInfo.NearTimeProfitRateCollection.NearTimeProfitRateList.AddRange(nearTimeProfitInfo);

                var currTime = DateTime.Now;
                int day = Convert.ToInt32(currTime.DayOfWeek) - 1;
                if (currTime.DayOfWeek != 0)
                    currTime = currTime.AddDays(-day);
                else
                    currTime = currTime.AddDays(-6);
                var startTime = currTime.AddDays(-7).Date;
                var endTime = currTime.Date;
                var rankNumber = BDFXManager.QueryRankNumber(orderDetailInfo.UserId);
                orderDetailInfo.RankNumber = rankNumber;
                var anteCodeList = BDFXManager.QueryAnteCodeListBySchemeId(schemeId);
                if (anteCodeList != null && anteCodeList.Count > 0)
                    orderDetailInfo.AnteCodeList = anteCodeList;
            }

            return orderDetailInfo;
        }
        /// <summary>
        /// 查询高手排行
        /// </summary>
        //public BDFXGSRank_Collection QueryGSRankList(string startTime, string endTime, string currUserId, string isMyGZ)
        //{
        //        var manager = new BDFXManager();            
        //        startTime = startTime.Replace('.', '-');
        //        endTime = endTime.Replace('.', '-');
        //        var sTime = Convert.ToDateTime(DateTime.Now.Year + "-" + startTime).Date;
        //        var eTime = Convert.ToDateTime(DateTime.Now.Year + "-" + endTime).AddDays(1).Date;
        //        if (!string.IsNullOrEmpty(isMyGZ))
        //            isMyGZ = isMyGZ.ToLower();
        //        return manager.QueryGSRankList(sTime, eTime, currUserId, isMyGZ);
            
        //}
        /// <summary>
        /// 查询我的主页
        /// </summary>
        //public TotalSingleTreasure_Collection QueryBDFXAutherHomePage(string userId, string strIsBonus, string currentTime, int pageIndex, int pageSize)
        //{
        //        var manager = new BDFXManager();
            
        //        return manager.QueryBDFXAutherHomePage(userId, strIsBonus, currentTime, pageIndex, pageSize);
            
        //}
        /// <summary>
        /// 查询宝单奖金提成信息
        /// </summary>
        public BDFXCommisionInfo QueryBDFXCommision(string schemeId)
        {
             var manager = new BDFXManager();
            
             return manager.QueryBDFXCommision(schemeId);
            
        }
        public string QueryYesterdayNR(DateTime startTime, DateTime endTime, int count)
        {
            var manager = new BDFXManager();
            
            return manager.QueryYesterdayNR(startTime, endTime, count);
            
        }
        public BDFXNRRankList_Collection QueryNRRankList(DateTime startTime, DateTime endTime, int count)
        {
            var manager = new BDFXManager();
            
            return manager.QueryNRRankList(startTime, endTime, count);
            
        }

        //public decimal QueryBetMoneyByDay(string userId, int days)
        //{
        //    var manager = new External.Business.Domain.Managers.RestrictionsBetMoney.RestrictionsBetMoneyManager()
            
        //        return manager.QueryBetMoneyByDay(userId, days);
            
        //}

        //public SingleTreasureAttention_Collection QueryBDFXAllAttentionIList()
        //{
        //    using (var manager = new BDFXManager())
        //    {
        //        var collection = new SingleTreasureAttention_Collection();
        //        collection.AddRange(manager.QueryBDFXAllAttentionIList());
        //        return collection;
        //    }
        //}

        //public WebUserSchemeShareExpert_Collection QueryWebUserSchemeShareExpertList(int queryCount, int expertType)
        //{
        //    if (queryCount <= 0) queryCount = 6;
        //    using (var manager = new BDFXManager())
        //    {
        //        var collction = manager.QueryWebUserSchemeShareExpertList(queryCount, expertType);
        //        //if (collction != null && collction.TotalCount > 0)
        //        //{
        //        //    var tempList = collction.UserSchemeShareExpertList;
        //        //    var _userIdList = tempList.Select(s => s.UserId).ToList();
        //        //    var bonusLeveList = new CacheDataBusiness().QueryBlog_ProfileBonusLevelByUserIds(string.Join(",", _userIdList));
        //        //    foreach (var item in tempList)
        //        //    {
        //        //        var tempImg = new UserBusiness().GetUserHeadPortrait(item.UserId);
        //        //        if (!string.IsNullOrEmpty(tempImg))//判断用户是否上传自定义头像
        //        //            item.UserCustomerImgUrl = tempImg;
        //        //        else
        //        //        {
        //        //            var tempInfo = bonusLeveList == null ? null : bonusLeveList.List.FirstOrDefault(s => s.UserId == item.UserId);
        //        //            if (tempInfo != null)
        //        //                item.MaxLevelName = tempInfo.MaxLevelName;
        //        //        }
        //        //    }
        //        //}
        //        return collction;
        //    }
        //}

        //public void AddUserSchemeShareExpert(string userId, int shortIndex, CopyOrderSource source)
        //{
        //    using (var manager = new BDFXManager())
        //    {
        //        var entity = manager.QueryUserSchemeShareExpertByUserId(userId, source);
        //        if (entity == null)
        //        {
        //            entity = new UserSchemeShareExpert();
        //            entity.ExpertType = source;
        //            entity.ShowSort = shortIndex;
        //            entity.IsEnable = true;
        //            entity.CreateTime = DateTime.Now;
        //            entity.UserId = userId;
        //            manager.AddUserSchemeShareExpert(entity);
        //        }
        //        else
        //        {
        //            entity.ExpertType = source;
        //            entity.ShowSort = shortIndex;
        //            entity.UserId = userId;
        //            manager.UpdateUserSchemeShareExpert(entity);
        //        }
        //    }
        //}
        //public UserSchemeShareExpert_Collection QueryUserSchemeShareExpertList(string userKey, int source, int pageIndex, int pageSize)
        //{
        //    using (var manager = new BDFXManager())
        //    {
        //        return manager.QueryUserSchemeShareExpertList(userKey, source, pageIndex, pageSize);
        //    }
        //}
        //public void DeleteUserSchemeShareExpert(string id)
        //{
        //    if (string.IsNullOrEmpty(id)) throw new Exception("用户编号不能为空");
        //    using (var manager = new BDFXManager())
        //    {
        //        manager.DeleteUserSchemeShareExpert(id);
        //    }
        //}
    }
}
