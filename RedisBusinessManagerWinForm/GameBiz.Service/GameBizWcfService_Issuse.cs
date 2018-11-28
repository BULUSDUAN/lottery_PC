using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using GameBiz.Business;
using GameBiz.Core;
using System.Configuration;
using GameBiz.Auth.Business;
using System.Threading;
using GameBiz.Domain.Entities;

namespace GameBiz.Service
{
    public class GameBizWcfService_Issuse : WcfService
    {
        public GameBizWcfService_Issuse()
        {
            KnownTypeRegister.RegisterKnownTypes(CommunicationObjectGetter.GetCommunicationObjectTypes());
        }
        /// <summary>
        /// 查询彩种
        /// </summary>
        public GameInfoCollection QueryGameList(string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return GameBusiness.GameList;
            }
            catch (Exception ex)
            {
                throw new Exception("查询彩种出错", ex);
            }
        }
        /// <summary>
        /// 查询彩种状态
        /// </summary>
        public LotteryGameInfoCollection QueryLotteryGameList(string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new GameBusiness().LotteryGame();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新彩种状态
        /// </summary>
        public CommonActionResult UpdateLotteryGame(string userToken, string gameCode, int enableStatus)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new GameBusiness().UpdateLotteryGame(gameCode, enableStatus);

                return new CommonActionResult(true, string.Format("更新彩种状态成功"));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询彩种玩法
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public GameTypeInfoCollection QueryGameTypeList(string gameCode, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return GameBizWcfServiceCache.GetGameType(gameCode);
            }
            catch (Exception ex)
            {
                throw new Exception("查询彩种玩法出错", ex);
            }
        }
        /// <summary>
        /// 获取当前奖期信息
        /// </summary>
        public Issuse_QueryInfo QueryCurrentIssuseInfo(string gameCode)
        {
            try
            {
                return GameBizWcfServiceCache.GetCurrentIssuserInfo(gameCode);
            }
            catch (Exception ex)
            {
                throw new Exception("查询当前奖期出错", ex);
            }
        }
        /// <summary>
        /// 以官方结束时间为准,获取当前奖期信息
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Issuse_QueryInfo QueryCurrentIssuseInfoWithOffical(string gameCode)
        {
            try
            {
                return GameBizWcfServiceCache.GetCurrentIssuseInfoWithOffical(gameCode);
            }
            catch (Exception ex)
            {
                throw new Exception("查询当前奖期出错", ex);
            }
        }

        /// <summary>
        /// 查询所有彩种当前期号
        /// </summary>
        public LotteryIssuse_QueryInfoCollection QueryAllGameCurrentIssuse(bool byOfficial)
        {
            return GameBizWcfServiceCache.QueryAllGameCurrentIssuse(byOfficial);
        }

        /// <summary>
        /// 查询最新期号
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public Issuse_QueryInfo QueryCurretNewIssuseInfo(string gameCode, string gameType)
        {
            try
            {
                return GameBizWcfServiceCache.QueryCurretNewIssuseInfo(gameCode, gameType);
            }
            catch (Exception ex)
            {
                throw new Exception("查询当前奖期出错", ex);
            }
        }
        /// <summary>
        /// 查询奖期
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <param name="issuseNumber"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Issuse_QueryInfo QueryIssuseInfo(string gameCode, string gameType, string issuseNumber)
        {
            try
            {
                return new GameBusiness().QueryIssuseInfo(gameCode, gameType, issuseNumber);
            }
            catch (Exception ex)
            {
                throw new Exception("查询奖期出错", ex);
            }
        }
        /// <summary>
        /// 添加本地奖期数据
        /// </summary>
        public CommonActionResult AddLocalIssuseList(LocalIssuse_AddInfoCollection list, int localAdvanceSeconds)
        {
            try
            {
                new IssuseBusiness().AddLocalIssuseList(list, localAdvanceSeconds);
                return new CommonActionResult(true, string.Format("添加本地奖期数据完成"));
            }
            catch (Exception ex)
            {
                throw new Exception("添加奖期信息出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询开奖历史
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public WinNumber_QueryInfoCollection QueryWinNumberHistory(string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                return GameBizWcfServiceCache.QueryWinNumberHistory(gameCode, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询开奖历史出错 -- " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据期数查询开奖历史
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="count"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public WinNumber_QueryInfoCollection QueryWinNumberHistoryByCount(string gameCode, int count)
        {
            try
            {
                return GameBizWcfServiceCache.QueryWinNumberHistoryByCount(gameCode, count);
            }
            catch (Exception ex)
            {
                throw new Exception("查询开奖历史出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询开奖大厅
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public WinNumber_QueryInfoCollection QueryWinNumberAll()
        {
            try
            {
                return GameBizWcfServiceCache.GetAllNewWinNumbers();
            }
            catch (Exception ex)
            {
                throw new Exception("查询开奖大厅出错 --" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询彩种最新开奖号码
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public WinNumber_QueryInfo QueryNewWinNumber(string gameCode)
        {
            try
            {
                return GameBizWcfServiceCache.GetNewWinNumber(gameCode);
            }
            catch (Exception ex)
            {
                throw new Exception("查询彩种最新开奖号码出错 -- " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询彩种最新开奖号码-及时不带缓存
        /// </summary>
        public WinNumber_QueryInfo GetNewWinNumber(string gameCode, string gameType)
        {
            try
            {
                return new GameBizWcfServiceCache().GetNewWinNumberFirst(gameCode, gameType);
            }
            catch (Exception ex)
            {
                throw new Exception("查询彩种最新开奖号码出错 -- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询彩种开奖号码
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="issuseNumber"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public WinNumber_QueryInfo QueryWinNumber(string gameCode, string issuseNumber)
        {
            try
            {
                return GameBizWcfServiceCache.GetWinNumber(gameCode, issuseNumber);
            }
            catch (Exception ex)
            {
                throw new Exception("查询彩种开奖号码出错 -- " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 设置最新开奖号码
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="issuseNumber"></param>
        /// <param name="winNumber"></param>
        /// <param name="awardTime"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public CommonActionResult SetNewWinNumber(string gameCode, string issuseNumber, string winNumber, DateTime awardTime)
        {
            try
            {
                GameBizWcfServiceCache.SetNewWinNumber(gameCode, issuseNumber, winNumber, awardTime);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("设置最新开奖号码失败 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 验证用户权限
        /// </summary>
        public bool CheckUserFunction(string funId, string userToken)
        {
            try
            {
                // 验证用户身份及权限
                string userId;
                GameBizAuthBusiness.ValidateAuthentication(userToken, "R", funId, out userId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 查询用户注册信息
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public UserRegInfo QueryUserRegister(string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {

                return new IssuseBusiness().QueryUserRegister(userId);
            }
            catch (Exception ex)
            {
                throw new Exception("查询用户信息出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询站点描述信息
        /// </summary>
        public SiteSummaryInfo QuerySiteSummary(string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new GameBusiness().QuerySiteSummary();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #region 北京单场数据

        /// <summary>
        /// 添加北京单场队伍信息
        /// </summary>
        public CommonActionResult Add_BJDC_MatchList(string issuseNumber, string matchIdList)
        {
            try
            {
                var matchIdArray = matchIdList.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                new IssuseBusiness().Update_BJDC_MatchList(issuseNumber, matchIdArray);
                //new IssuseBusiness().Add_BJDC_MatchList(issuseNumber);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("添加北京单场队伍信息出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新北京单场队伍信息
        /// </summary>
        public CommonActionResult Update_BJDC_MatchList(string issuseNumber, string matchIdList, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var matchIdArray = matchIdList.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                new IssuseBusiness().Update_BJDC_MatchList(issuseNumber, matchIdArray);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("更新北京单场队伍信息出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工更新北京单场队伍信息
        /// </summary>
        public CommonActionResult ManualUpdate_BJDC_MatchList(string issuseNumber, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                //new IssuseBusiness().ManualUpdate_BJDC_MatchResultList(issuseNumber);
                new IssuseBusiness().ManualUpdate_BJDC_MatchList(issuseNumber);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工更新北京单场比赛结果
        /// </summary>
        public CommonActionResult ManualUpdate_BJDC_MatchResultList(string issuseNumber, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().ManualUpdate_BJDC_MatchResultList(issuseNumber);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 添加北京单场比赛结果
        /// </summary>
        public CommonActionResult Add_BJDC_MatchResultList(string issuseNumber, string matchResultIdList)
        {
            try
            {
                var matchResultIdArray = matchResultIdList.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                var biz = new IssuseBusiness();
                biz.Update_BJDC_MatchResultList(issuseNumber, matchResultIdArray);
                //new IssuseBusiness().Add_BJDC_MatchResultList(issuseNumber);
                new Thread(() =>
                {
                    Common.Utilities.UsefullHelper.TryDoAction(() =>
                    {
                        biz.Update_BJDC_HitCount(issuseNumber, matchResultIdArray);
                    });
                }).Start();
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("添加北京单场比赛结果出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新北京单场比赛结果
        /// </summary>
        public CommonActionResult Update_BJDC_MatchResultList(string issuseNumber, string matchResultIdList)
        {
            try
            {
                var matchResultIdArray = matchResultIdList.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                var biz = new IssuseBusiness();
                biz.Update_BJDC_MatchResultList(issuseNumber, matchResultIdArray);
                new Thread(() =>
                {
                    Common.Utilities.UsefullHelper.TryDoAction(() =>
                    {
                        biz.Update_BJDC_HitCount(issuseNumber, matchResultIdArray);
                    });
                }).Start();
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("更新北京单场比赛结果出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据订单号更新单个订单的过关统
        /// </summary>
        public CommonActionResult Update_BJDC_HitCountBySchemeId(string schemeId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().Update_BJDC_HitCountBySchemeId(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        /// <summary>
        /// 查询当前北京单场比赛信息
        /// </summary>
        /// <returns></returns>
        public CoreBJDCMatchInfoCollection QueryCurrentBJDCMatchInfo(string userToken)
        {
            try
            {
                return new IssuseBusiness().QueryCurrentBJDCMatchInfo();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }
        /// <summary>
        /// 更新北京单场玩法
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="privilegesType"></param>
        /// <returns></returns>
        public CommonActionResult UpdateBJDCMatchInfo(string Id, string privilegesType)
        {
            try
            {
                new IssuseBusiness().UpdateBJDCMatchInfo(Id, privilegesType);
                return new CommonActionResult(true, "更新成功！");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public string QueryBJDCCurrentIssuse()
        {
            try
            {
                return new IssuseBusiness().QueryBJDCCurrentIssuse();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public BJDCIssuseInfo QueryBJDCCurrentIssuseInfo()
        {
            try
            {
                return new IssuseBusiness().QueryBJDCCurrentIssuseInfo();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 获取北京单场最新开奖期号
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public BJDCMatchResultInfo_Collection GetBJDCIssuse(int count)
        {
            try
            {
                return new IssuseBusiness().GetBJDCIssuse(count);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public string QueryBJDCLastIssuseNumber(int count)
        {
            return new IssuseBusiness().QueryBJDCLastIssuseNumber(count);
        }

        #endregion

        #region 竞彩足球

        /// <summary>
        /// 添加竞彩足球队伍数据
        /// </summary>
        public CommonActionResult Add_JCZQ_MatchList(string matchIdArray)
        {
            try
            {
                var array = matchIdArray.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                new IssuseBusiness().Update_JCZQ_MatchList(array);
                //new IssuseBusiness().Add_JCZQ_MatchList(array);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("添加竞彩足球队伍数据异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新竞彩足球队伍数据
        /// </summary>
        public CommonActionResult Update_JCZQ_MatchList(string matchIdArray)
        {
            try
            {
                var array = matchIdArray.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                new IssuseBusiness().Update_JCZQ_MatchList(array);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("更新竞彩足球队伍数据异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工更新竞彩足球队伍数据
        /// </summary>
        public CommonActionResult ManualUpdate_JCZQ_MatchList(string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().ManualUpdate_JCZQ_MatchList();
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 添加竞彩足球比赛结果数据
        /// </summary>
        public CommonActionResult Add_JCZQ_MatchResultList(string matchIdArray)
        {
            try
            {
                var array = matchIdArray.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                var biz = new IssuseBusiness();
                biz.Update_JCZQ_MatchResultList(array);
                new Thread(() =>
                {
                    Common.Utilities.UsefullHelper.TryDoAction(() =>
                    {
                        biz.Update_JCZQ_HitCount(array);
                    });
                }).Start();
                //new IssuseBusiness().Add_JCZQ_MatchResultList(array);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("添加竞彩足球比赛结果数据异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新竞彩足球比赛结果数据
        /// </summary>
        public CommonActionResult Update_JCZQ_MatchResultList(string matchIdArray)
        {
            try
            {
                var array = matchIdArray.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                var biz = new IssuseBusiness();
                biz.Update_JCZQ_MatchResultList(array);
                new Thread(() =>
                {
                    Common.Utilities.UsefullHelper.TryDoAction(() =>
                    {
                        biz.Update_JCZQ_HitCount(array);
                    });
                }).Start();
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("更新竞彩足球比赛结果数据异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据订单号，更新单个订单的过关统计
        /// </summary>
        public CommonActionResult Update_JCZQ_HitCountBySchemeId(string schemeId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().Update_JCZQ_HitCountBySchemeId(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询竞彩足球比赛结果
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public JCZQMatchResult_Collection QueryJCZQMatchResult(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                return new IssuseBusiness().QueryJCZQMatchResult(startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public JCZQMatchResult_Collection QueryJCZQMatchResultByTime(DateTime time)
        {
            try
            {
                return new IssuseBusiness().QueryJCZQMatchResult(time);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询当前竞彩足球比赛结果
        /// </summary>
        public CoreJCZQMatchInfoCollection QueryCurrentJCZQMatchInfo(string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new IssuseBusiness().QueryCurrentJCZQMatchInfo();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新竞彩足球比赛信息
        /// </summary>
        public CommonActionResult UpdateJCZQMatchInfo(string matchId, string privilegesType, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().UpdateJCZQMatchInfo(matchId, privilegesType);
                return new CommonActionResult(true, "更新成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region 竞彩篮球

        /// <summary>
        /// 添加竞彩篮球队伍数据.
        /// </summary>
        public CommonActionResult Add_JCLQ_MatchList(string matchIdArray)
        {
            try
            {
                var array = matchIdArray.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                new IssuseBusiness().Update_JCLQ_MatchList(array);
                //new IssuseBusiness().Add_JCLQ_MatchList(array);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("添加竞彩篮球队伍数据异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新竞彩篮球队伍数据
        /// </summary>
        public CommonActionResult Update_JCLQ_MatchList(string matchIdArray)
        {
            try
            {
                var array = matchIdArray.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                new IssuseBusiness().Update_JCLQ_MatchList(array);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("更新竞彩篮球队伍数据异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工更新竞彩篮球队伍数据
        /// </summary>
        public CommonActionResult ManualUpdate_JCLQ_MatchList(string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().ManualUpdate_JCLQ_MatchList();
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 添加竞彩篮球比赛结果
        /// </summary>
        public CommonActionResult Add_JCLQ_MatchResultList(string matchIdArray)
        {
            try
            {
                var array = matchIdArray.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                var biz = new IssuseBusiness();
                biz.Update_JCLQ_MatchResultList(array);
                new Thread(() =>
                {
                    Common.Utilities.UsefullHelper.TryDoAction(() =>
                    {
                        biz.Update_JCLQ_HitCount(array);
                    });
                }).Start();
                //new IssuseBusiness().Add_JCLQ_MatchResultList(array);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("添加竞彩篮球比赛结果数据异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新竞彩篮球比赛结果数据
        /// </summary>
        public CommonActionResult Update_JCLQ_MatchResultList(string matchIdArray)
        {
            try
            {
                var array = matchIdArray.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                var biz = new IssuseBusiness();
                biz.Update_JCLQ_MatchResultList(array);
                new Thread(() =>
                {
                    Common.Utilities.UsefullHelper.TryDoAction(() =>
                    {
                        biz.Update_JCLQ_HitCount(array);
                    });
                }).Start();
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("更新竞彩篮球比赛结果数据异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据方案号更新单个订单的过关统计
        /// </summary>
        public CommonActionResult Update_JCLQ_HitCountBySchemeId(string schemeId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().Update_JCLQ_HitCountBySchemeId(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询竞彩篮球比赛结果
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public JCLQMatchResult_Collection QueryJCLQMatchResult(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                return new IssuseBusiness().QueryJCLQMatchResult(startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public JCLQMatchResult_Collection QueryJCLQMatchResultByTime(DateTime time)
        {
            try
            {
                return new IssuseBusiness().QueryJCLQMatchResult(time);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询当前竞彩篮球比赛结果
        /// </summary>
        public CoreJCLQMatchInfoCollection QueryCurrentJCLQMatchInfo(string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new IssuseBusiness().QueryCurrentJCLQMatchInfo();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新竞彩篮球比赛信息
        /// </summary>
        public CommonActionResult UpdateJCLQMatchInfo(string matchId, string privilegesType, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().UpdateJCLQMatchInfo(matchId, privilegesType);
                return new CommonActionResult(true, "更新成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region 传统足球

        /// <summary>
        /// 添加传统足球奖期信息
        /// </summary>
        public CommonActionResult Add_CTZQ_GameIssuse(string array)
        {
            try
            {
                //var param = gameCode + "&" + string.Join("_", (from l in addList select l.Id).ToArray());
                var p = array.Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries);
                if (p.Length != 2)
                    throw new Exception("传入参数不正确：" + array);

                var gameCode = p[0];
                var issuseIdArray = p[1].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);

                new IssuseBusiness().Update_CTZQ_GameIssuse(gameCode, issuseIdArray);
                //new IssuseBusiness().Add_CTZQ_GameIssuse(gameCode, issuseIdArray);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("添加传统足球奖期信息异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新传统足球奖期信息
        /// </summary>
        public CommonActionResult Update_CTZQ_GameIssuse(string array)
        {
            try
            {
                //var param = gameCode + "&" + string.Join("_", (from l in addList select l.Id).ToArray());
                var p = array.Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries);
                if (p.Length != 2)
                    throw new Exception("传入参数不正确：" + array);

                var gameCode = p[0];
                var issuseIdArray = p[1].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                new IssuseBusiness().Update_CTZQ_GameIssuse(gameCode, issuseIdArray);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("更新传统足球奖期信息异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 添加传统足球队伍信息
        /// </summary>
        public CommonActionResult Add_CTZQ_MatchList(string array)
        {
            try
            {
                //var param = gameCode + "&" + issuseNumber + "&" + string.Join("_", (from l in addList select l.Id).ToArray());
                var p = array.Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries);
                if (p.Length != 3)
                    throw new Exception("传入参数不正确：" + array);

                var gameCode = p[0];
                var issuseNumber = p[1];
                var issuseIdArray = p[2].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                new IssuseBusiness().Update_CTZQ_MatchList(gameCode, issuseNumber, issuseIdArray);
                //new IssuseBusiness().Add_CTZQ_MatchList(gameCode, issuseNumber, issuseIdArray);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("添加传统足球队伍信息异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新传统足球队伍信息
        /// </summary>
        public CommonActionResult Update_CTZQ_MatchList(string array)
        {
            try
            {
                //var param = gameCode + "&" + issuseNumber + "&" + string.Join("_", (from l in addList select l.Id).ToArray());
                var p = array.Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries);
                if (p.Length != 3)
                    throw new Exception("传入参数不正确：" + array);

                var gameCode = p[0];
                var issuseNumber = p[1];
                var issuseIdArray = p[2].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                new IssuseBusiness().Update_CTZQ_MatchList(gameCode, issuseNumber, issuseIdArray);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("更新传统足球队伍信息异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 手工更新传统足球队伍信息
        /// </summary>
        public CommonActionResult ManualUpdate_CTZQ_MatchList(string gameCode, string issuseNumber, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().ManualUpdate_CTZQ_MatchList(gameCode, issuseNumber);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 添加传统足球奖级信息
        /// </summary>
        public CommonActionResult Add_CTZQ_MatchPoolList(string array)
        {
            try
            {
                //var param = gameCode + "&" + issuseNumber + "&" + string.Join("_", (from l in addList select l.Id).ToArray());
                var p = array.Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries);
                if (p.Length != 3)
                    throw new Exception("传入参数不正确：" + array);

                var gameCode = p[0];
                var issuseNumber = p[1];
                var issuseIdArray = p[2].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                var biz = new IssuseBusiness();
                biz.Update_CTZQ_MatchPoolList(gameCode, issuseNumber, issuseIdArray);
                //new IssuseBusiness().Add_CTZQ_MatchPoolList(gameCode, issuseNumber, issuseIdArray);
                new Thread(() =>
                {
                    Common.Utilities.UsefullHelper.TryDoAction(() =>
                    {
                        biz.Update_CTZQ_HitCount(gameCode, issuseNumber);
                    });
                }).Start();
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("添加传统足球奖级信息异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新传统足球奖级信息
        /// </summary>
        public CommonActionResult Update_CTZQ_MatchPoolList(string array)
        {
            try
            {
                //var param = gameCode + "&" + issuseNumber + "&" + string.Join("_", (from l in addList select l.Id).ToArray());
                var p = array.Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries);
                if (p.Length != 3)
                    throw new Exception("传入参数不正确：" + array);

                var gameCode = p[0];
                var issuseNumber = p[1];
                var issuseIdArray = p[2].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                var biz = new IssuseBusiness();
                biz.Update_CTZQ_MatchPoolList(gameCode, issuseNumber, issuseIdArray);
                new Thread(() =>
                {
                    Common.Utilities.UsefullHelper.TryDoAction(() =>
                    {
                        biz.Update_CTZQ_HitCount(gameCode, issuseNumber);
                    });
                }).Start();
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("更新传统足球奖级信息异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据方案号更新单个订单的过关统
        /// </summary>
        public CommonActionResult Update_CTZQ_HitCountBySchemeId(string schemeId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new IssuseBusiness().Update_CTZQ_HitCountBySchemeId(schemeId);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询传统足球开奖结果
        /// </summary>
        public CTZQMatchInfo_Collection QueryCTZQMatchListByIssuseNumber(string gameType, string issuseNumber, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new Sports_Business().QueryCTZQMatchListByIssuseNumber(gameType, issuseNumber);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询传统足球当前奖期的期号
        /// </summary>
        public string QueryCTZQCurrentIssuse()
        {
            try
            {
                return new IssuseBusiness().QueryCTZQCurrentIssuse();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public Issuse_QueryInfo QueryCTZQCurrentIssuseInfo(string gameType)
        {
            return new IssuseBusiness().QueryCTZQCurrentIssuse(gameType);
        }

        #endregion

        /// <summary>
        /// 北京单场查询开奖结果
        /// </summary>
        /// <param name="issuseNumber"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public BJDCMatchResultInfo_Collection QueryBJDC_MatchResultList(string issuseNumber)
        {
            try
            {
                return new IssuseBusiness().QueryBJDC_MatchResultList(issuseNumber);
            }
            catch (Exception ex)
            {
                throw new Exception("北京单场查询开奖结果异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 北京单场查询开奖结果
        /// </summary>
        public BJDCMatchResultInfo_Collection QueryBJDC_MatchResultCollection(string issuseNumber, int pageIndex, int pageSize)
        {
            try
            {
                return new IssuseBusiness().QueryBJDC_MatchResultList(issuseNumber, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("北京单场查询开奖结果异常 - " + ex.Message, ex);
            }
        }

        #region 胜负过关相关操作

        /// <summary>
        /// 后台手工添加或更新胜负过关比赛数据
        /// </summary>
        public CommonActionResult ManualUpdate_SFGG_MatchList(string issuseNumber)
        {
            try
            {
                new IssuseBusiness().ManualUpdate_SFGG_MatchList(issuseNumber);
                return new CommonActionResult(true, "更新比赛数据成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 后台手工添加或更新胜负过关比赛结果
        /// </summary>
        public CommonActionResult ManualUpdate_SFGG_MatchResultList(string issuseNumber)
        {
            try
            {
                new IssuseBusiness().ManualUpdate_SFGG_MatchResultList(issuseNumber);
                return new CommonActionResult(true, "更新比赛结果成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        /// <summary>
        /// 查询状态为撤单和部分出票失败并且短信没有发送成功
        /// </summary>
        /// <returns></returns>
        public SendMsgHistoryRecord_Collection QueryFailMsgList()
        {
            try
            {
                return new SqlQueryBusiness().QueryFailMsgList();
            }
            catch
            {
                return new SendMsgHistoryRecord_Collection();
            }
        }

    }
}
