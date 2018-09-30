using EntityModel;
using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using EntityModel.LotteryJsonInfo;
using EntityModel.Redis;
using Kason.Sg.Core.CPlatform.Ioc;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Redis;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.ORM.Helper;
using KaSon.FrameWork.ORM.Helper.WinNumber;
using Lottery.Service.IModuleServices;
using Lottery.Service.ModuleBaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lottery.Service.ModuleServices
{
    [ModuleName("Data")]
    public class DataService : KgBaseService, IDataService
    {

        private readonly DataRepository _repository;
        IKgLog log = null;
        public DataService(DataRepository repository) : base()
        {
            this._repository = repository;
            log = new Log4Log();
        }

        #region 获取当前奖期信息
        /// <summary>
        /// 获取当前奖期信息
        /// </summary>
        public Task<Issuse_QueryInfo> QueryCurrentIssuseInfo(string gameCode)
        {
            try
            {
                return Task.FromResult(GameCacheBusiness.GetCurrentIssuserInfo(gameCode));
            }
            catch (Exception ex)
            {
                throw new Exception("查询当前奖期出错", ex);
            }
        }
        #endregion

        #region 从Redis中查询当前奖期
        /// <summary>
        /// 从Redis中查询当前奖期
        /// ByLocalStopTime
        /// </summary>
        public Task<LotteryIssuse_QueryInfo> QueryNextIssuseListByLocalStopTime(string gameCode)
        {
            var list = WebRedisHelper.QueryNextIssuseListByLocalStopTime(gameCode);
            if (list == null || list.Count <= 0)
                return null;
            return Task.FromResult(list.Where(p => p.LocalStopTime > DateTime.Now).OrderBy(p => p.OfficialStopTime).FirstOrDefault());
        }
        #endregion

        #region 前台查询广告信息
        /// <summary>
        /// 前台查询广告信息
        /// </summary>
        /// <param name="bannerType"></param>
        /// <param name="returnRecord"></param>
        /// <returns></returns>
        public Task<SiteMessageBannerInfo_Collection> QuerySitemessageBanngerList_Web(int bannerType, int returnRecord = 10)
        {
            try
            {
                return Task.FromResult(new DataQuery().QuerySitemessageBanngerList_Web(bannerType, returnRecord));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 查询文章列表
        /// <summary>
        /// 查询文章列表
        /// todo:后台权限
        /// </summary>
        public Task<ArticleInfo_QueryCollection> QueryArticleList(string key, string gameCode, string category, int pageIndex, int pageSize)
        {
            // 验证用户身份及权限
            try
            {
                //var userId = new UserAuthentication().ValidateUserAuthentication(userToken);
                var model = new DataQuery().QueryArticleList(key, gameCode, category, pageIndex, pageSize);
                return Task.FromResult(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 查询文章明细
        /// <summary>
        /// 根据编号查询文章信息_后台
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<ArticleInfo_Query> QueryArticleById_Web(string articleId)
        {
            try
            {
                var query = new DataQuery();
                var entity = query.GetArticleById(articleId);
                if (entity == null)
                {
                    throw new LogicException("指定编号的文章不存在");
                }
                entity.ReadCount++;
                query.UpdateArticle(entity);
                var info = new ArticleInfo_Query();
                ObjectConvert.ConverEntityToInfo<E_SiteMessage_Article_List, ArticleInfo_Query>(entity, ref info);
                return Task.FromResult(info);
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 查询公告
        /// <summary>
        /// 查询公告
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<BulletinInfo_Collection> QueryDisplayBulletinCollection(int agent, int pageIndex, int pageSize)
        {
            try
            {
                // 验证用户身份及权限
                //var userId = new UserAuthentication().ValidateUserAuthentication(userToken);
                var key = $"{RedisKeys.BulletinList}_{agent}_{pageIndex}_{pageSize}";
                var obj = RedisHelperEx.DB_Other.GetObj<BulletinInfo_Collection>(key);
                if (obj != null)
                {
                    return Task.FromResult(obj);
                }
                var result = new DataQuery().QueryDisplayBulletins(agent, pageIndex, pageSize);
                RedisHelperEx.DB_Other.SetObj(key, result, TimeSpan.FromMinutes(5));
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 提交建议
        /// <summary>
        /// 提交用户意见
        /// </summary>
        /// <param name="userIdea"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<CommonActionResult> SubmitUserIdea(UserIdeaInfo_Add userIdea)
        {
            var query = new DataQuery();
            query.SubmitUserIdea(userIdea);
            return Task.FromResult(new CommonActionResult(true, "提交建议成功"));
        }
        #endregion

        #region 活动列表
        /// <summary>
        /// 查询活动列表查询
        /// </summary>
        public Task<ActivityListInfoCollection> QueryActivInfoList(int pageIndex, int pageSize)
        {
            try
            {
                return Task.FromResult(new DataQuery().QueryActivInfoList(pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 查询最新期号
        /// <summary>
        /// 查询最新期号
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public Task<Issuse_QueryInfo> QueryCurretNewIssuseInfo(string gameCode, string gameType)
        {
            try
            {
                return Task.FromResult(GameServiceCache.QueryCurretNewIssuseInfo(gameCode, gameType));
            }
            catch (Exception ex)
            {
                throw new Exception("查询当前奖期出错", ex);
            }
        }
        public Task<Issuse_QueryInfoEX> QueryCurretNewIssuseInfoList()
        {
            try
            {
                return Task.FromResult(GameServiceCache.QueryCurretNewIssuseInfoByList());
            }
            catch (Exception ex)
            {
                throw new Exception("查询当前奖期出错", ex);
            }
        }
        //QueryCurretNewIssuseInfoByList

        /// <summary>
        /// 查询北京单场最新期号
        /// </summary>
        /// <returns></returns>
        public Task<BJDCIssuseInfo> QueryBJDCCurrentIssuseInfo()
        {
            try
            {
                return Task.FromResult(new DataQuery().QueryBJDCCurrentIssuseInfo());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 根据公告编号查询公告
        /// <summary>
        /// 根据公告编号查询公告
        /// </summary>
        /// <param name="bulletinId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<BulletinInfo_Query> QueryDisplayBulletinDetailById(long bulletinId)
        {
            try
            {
                var info = new DataQuery().QueryBulletinDetailById(bulletinId);
                if (info != null)
                {
                    if (info.Status != EnableStatus.Enable)
                    {
                        throw new LogicException("指定公告已经禁用");
                    }
                    if (info.EffectiveFrom != null && info.EffectiveFrom > DateTime.Now)
                    {
                        throw new LogicException("指定公告尚未发布");
                    }
                    if (info.EffectiveTo != null && info.EffectiveTo.Value.AddDays(1) < DateTime.Now)
                    {
                        throw new LogicException(string.Format("指定公告已经于{0:yyyy-MM-dd}过期", info.EffectiveTo));
                    }
                }
                return Task.FromResult(info);
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 查找配置表中的配置信息
        /// <summary>
        /// 查找C_Core_Config表中的配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<C_Core_Config> QueryCoreConfigByKey(string key)
        {
            try
            {
                return Task.FromResult(new CacheDataBusiness().QueryCoreConfigByKey(key));
            }
            catch (Exception ex)
            {
                throw new Exception("查询系统配置出错", ex);
            }
        }
        #endregion

        #region 根据代理商编码查询APP配置
        /// <summary>
        /// 根据代理商编码查询APP配置
        /// </summary>
        /// <returns></returns>
        public Task<APPConfigInfo> QueryAppConfigByAgentId(string appAgentId)
        {
            try
            {
                return Task.FromResult(new CacheDataBusiness().QueryAppConfigByAgentId(appAgentId));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 根据UrlType查询所有APP嵌套配置
        /// <summary>
        /// 根据UrlType查询所有APP嵌套配置
        /// </summary>
        /// <returns></returns>
        public Task<NestedUrlConfig_Collection> QueryNestedUrlConfigListByUrlType(int urlType)
        {
            try
            {
                return Task.FromResult(new CacheDataBusiness().QueryNestedUrlConfigListByUrlType(urlType));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 查询我的站内信
        ///// <summary>
        ///// 查询我的站内信
        ///// </summary>
        public Task<SiteMessageInnerMailListNew_Collection> QueryMyInnerMailList(int pageIndex, int pageSize, string userId)
        {
            // 验证用户身份及权限
            try
            {
                //var userId = new UserAuthentication().ValidateUserAuthentication(userToken);
                return Task.FromResult(new DataQuery().QueryInnerMailListByReceiver(userId, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询已读和未读站内信
        /// </summary>
        public Task<SiteMessageInnerMailListNew_Collection> QueryUnReadInnerMailListByReceiver(string userId, int pageIndex, int pageSize, int handleType)
        {
            try
            {
                return Task.FromResult(new DataQuery().QueryUnReadInnerMailList_ByReceiverId(userId, pageIndex, pageSize, handleType));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 阅读站内信
        /// <summary>
        /// 阅读站内信
        /// </summary>
        public Task<InnerMailInfo_Query> ReadInnerMail(string innerMailId, string userId)
        {
            // 验证用户身份及权限
            try
            {
                //var userId = new UserAuthentication().ValidateUserAuthentication(userToken);
                var dataQuery = new DataQuery();
                if (!dataQuery.IsMyInnerMail(innerMailId, userId))
                {
                    throw new LogicException(string.Format("此站内信不属于指定用户。站内信：{0}；用户：{1}。", innerMailId, userId));
                }
                var info = dataQuery.QueryInnerMailDetailByIdAndRead(innerMailId, userId);
                return Task.FromResult(info);
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            //using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            //{
            //    biz.BeginTran();

            //    var siteBiz = new SiteMessageControllBusiness();
            //    if (!siteBiz.IsMyInnerMail(innerMailId, userId))
            //    {
            //        throw new SiteMessageException(string.Format("此站内信不属于指定用户。站内信：{0}；用户：{1}。", innerMailId, userId));
            //    }
            //    siteBiz.ReadInnerMail(innerMailId, userId);
            //    var info = siteBiz.QueryInnerMailDetailById(innerMailId);

            //    biz.CommitTran();

            //    return info;
            //}
        }
        #endregion

        #region 查询红包使用规则
        /// <summary>
        /// 查询红包使用规则
        /// </summary>
        public Task<string> QueryRedBagUseConfig()
        {
            try
            {
                return Task.FromResult(new DataQuery().QueryRedBagUseConfig());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 查询文章列表_优化
        //private static Dictionary<string, ArticleInfo_QueryCollection> _articleCollection = new Dictionary<string, ArticleInfo_QueryCollection>();
        /// <summary>
        /// 查询文章列表
        /// todo:后台权限
        /// </summary>
        public Task<ArticleInfo_QueryCollection> QueryArticleList_YouHua(string category, string gameCode, int pageIndex, int pageSize)
        {
            try
            {
                //string cacheKey = string.Format("{0}_{1}_{2}_{3}", category, gameCode, pageIndex, pageSize);
                var result = new ArticleInfo_QueryCollection();
                //if (_articleCollection != null && _articleCollection.Count > 0 && _articleCollection.ContainsKey(cacheKey))
                //{
                //    result = _articleCollection.FirstOrDefault(s => s.Key == cacheKey).Value;
                //}
                //else
                //{
                if (string.IsNullOrEmpty(category))
                    throw new LogicException("未查询到文章类别");
                var Key = RedisKeys.ArticleListKey;
                string cacheKey = string.Format("{0}_{1}_{2}_{3}_{4}", Key, category, gameCode, pageIndex, pageSize);
                var obj = RedisHelperEx.DB_CoreCacheData.GetObj<ArticleInfo_QueryCollection>(cacheKey);
                if (obj != null)
                {
                    return Task.FromResult(obj);
                }
                var array = category.Split('|');
                var gameCodeArray = gameCode.Split('|');
                result = new DataQuery().QueryArticleList_YouHua(array, gameCodeArray, pageIndex, pageSize);
                //if (!_articleCollection.ContainsKey(cacheKey))
                //    _articleCollection.Add(cacheKey, result);
                //}
                if (result != null)
                {
                    RedisHelperEx.DB_CoreCacheData.SetObj(cacheKey, result, TimeSpan.FromMinutes(5));
                }
                return Task.FromResult(result);
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 查询fxid活动下所有邀请
        /// <summary>
        /// 查询fxid活动下所有邀请
        /// </summary>
        public Task<ShareSpreadCollection> QueryShareSpreadUsers(string agentId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                int userTotalCount = 0;
                decimal RedBagMoneyTotal = 0M;
                var shareList = new ShareSpreadCollection();
                var result = new DataQuery().QueryBlog_UserShareSpreadList(agentId, pageIndex, pageSize, startTime, endTime, out userTotalCount, out RedBagMoneyTotal);
                shareList.UserTotal = userTotalCount;
                shareList.RedBagMoneyTotal = RedBagMoneyTotal;
                if (result != null && result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        shareList.ShareSpreadList.Add(new BlogUserShareSpread()
                        {
                            Id = item.Id,
                            UserId = item.UserId,
                            UserName = item.userName,
                            AgentId = item.AgentId,
                            isGiveLotteryRedBag = item.isGiveLotteryRedBag,
                            isGiveRegisterRedBag = item.isGiveRegisterRedBag,
                            giveRedBagMoney = item.giveRedBagMoney,
                            CreateTime = item.CreateTime,
                            UpdateTime = item.UpdateTime
                        });
                    }
                }
                return Task.FromResult(shareList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 查询当前遗漏

        /// <summary>
        /// 查询一星单选遗漏
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Task<CQSSC_1X_ZS> QueryCQSSCCurrNumberOmission_1XDX(string key, int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSCCurrNumberOmission_1XDX(key, index));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询重庆时时彩当前遗漏_二星直选
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Task<CQSSC_2X_ZXZS> QueryCQSSCCurrNumberOmission_2XZX(string key, int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSCCurrNumberOmission_2XZX(key, index));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询重庆时时彩当前遗漏_三星直选
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Task<CQSSC_3X_ZXZS> QueryCQSSCCurrNumberOmission_3XZX(string key, int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSCCurrNumberOmission_3XZX(key, index));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询重庆时时彩当前遗漏_二星组选
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Task<CQSSC_2X_ZuXZS> QueryCQSSCCurrNumberOmission_2XZuX(string key, int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSCCurrNumberOmission_2XZuX(key, index));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询重庆时时彩当前遗漏_组三，组六
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Task<CQSSC_3X_ZuXZS> QueryCQSSCCurrNumberOmission_ZX3_ZX6(string key, int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSCCurrNumberOmission_ZX3_ZX6(key, index));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询重庆时时彩当前遗漏_大小单双
        /// </summary>
        public Task<CQSSC_DXDS> QueryCQSSCCurrNumberOmission_DXDS(string key, int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSCCurrNumberOmission_DXDS(key, index));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询重庆时时彩当前遗漏_五星基本走势
        /// </summary>
        public Task<CQSSC_5X_JBZS> QueryCQSSCCurrNumberOmission_5XJBZS(string key, int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSCCurrNumberOmission_5XJBZS(key, index));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion


        #region 获取游客token
        public Task<CommonActionResult> GetGuestToken()
        {
            var authBiz = new UserAuthentication();
            var token = authBiz.GetGuestToken();
            return Task.FromResult(new CommonActionResult(true, "获取匿名用户口令成功") { ReturnValue = token });
        }


        public Task<List<C_Bank_Info>> GetBankList()
        {
            try
            {
                return Task.FromResult(new DataQuery().GetBankList());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 根据GameCode与期号获取当年/当天最大期号
        public Task<List<string>> GetMaxIssueByGameCode(string gameCode, string currIssueNumber, int issueCount)
        {
            try
            {
                return Task.FromResult(new DataQuery().GetMaxIssueByGameCode(gameCode, currIssueNumber, issueCount));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        public Task<string> ReadSqlTimeLog(string FileName)
        {
            return Task.FromResult(KaSon.FrameWork.Common.Utilities.FileHelper.GetLogInfo("Log_Log\\SQLInfo", "LogTime_"));
        }

        public Task<List<CtzqIssuesWeb>> GetCTZQIssuseList_ByRedis(string Key)
        {
            try
            {
                return Task.FromResult(RedisHelperEx.DB_Match.GetObjs<CtzqIssuesWeb>(Key));
            }
            catch (Exception ex)
            {
                throw new Exception("获取出错", ex);
            }
        }

        public Task<List<CTZQ_MatchInfo_WEB>> GetCTZQMatchOddsList_ByRedis(string Key)
        {
            try
            {
                return Task.FromResult(RedisHelperEx.DB_Match.GetObjs<CTZQ_MatchInfo_WEB>(Key));
            }
            catch (Exception ex)
            {
                throw new Exception("获取出错", ex);
            }
        }

        public Task<List<BJDC_MatchInfo_WEB>> GetBJDCMatchOddsLis_ByRedis(string Key)
        {
            try
            {
                return Task.FromResult(RedisHelperEx.DB_Match.GetObjs<BJDC_MatchInfo_WEB>(Key));
            }
            catch (Exception ex)
            {
                throw new Exception("获取出错", ex);
            }
        }

        public Task<List<JCZQ_MatchInfo_WEB>> GetJCZQMatchOddsList_ByRedis(string Key)
        {
            try
            {
                return Task.FromResult(RedisHelperEx.DB_Match.GetObjs<JCZQ_MatchInfo_WEB>(Key));
            }
            catch (Exception ex)
            {
                throw new Exception("获取出错", ex);
            }
        }

        public Task<List<JCLQ_MatchInfo_WEB>> GetJCLQMatchOddsList_ByRedis(string Key)
        {
            try
            {
                return Task.FromResult(RedisHelperEx.DB_Match.GetObjs<JCLQ_MatchInfo_WEB>(Key));
            }
            catch (Exception ex)
            {
                throw new Exception("获取出错", ex);
            }
        }

        public Task<List<KaiJiang>> GetKaiJiangList_ByRedis()
        {
            try
            {
                string redisKey = EntityModel.Redis.RedisKeys.KaiJiang_Key;
                return Task.FromResult(RedisHelperEx.DB_Match.GetObjs<KaiJiang>(redisKey));
            }
            catch (Exception ex)
            {
                throw new Exception("获取出错", ex);
            }
        }

        public Task<List<APP_Advertising>> GetGameInfoIndex()
        {
            try
            {
                var Key = EntityModel.Redis.RedisKeys.APP_Advertising_V2;
                var RedisValue = RedisHelperEx.DB_Other.GetObjs<APP_Advertising>(Key);
                if (RedisValue == null)
                {
                    var Business = new CacheDataBusiness();
                    var Config = Business.QueryCoreConfigByKey(Key);
                    Record_AppAd AppAd = null;
                    List<APP_Advertising> AdList = null;
                    if (Config != null && !string.IsNullOrEmpty(Config.ConfigValue))
                    {
                        //.Replace("\r","").Replace("\n","").Replace("\t","")
                        AppAd = JsonHelper.Deserialize<Record_AppAd>(Config.ConfigValue);
                    }
                    if (AppAd == null || AppAd.records == null) AdList = new List<APP_Advertising>();
                    else AdList = AppAd.records;
                    var AllGame = Business.QueryLotteryAllGame();
                    var RetuenAdList = new List<APP_Advertising>();
                    foreach (var item in AllGame)
                    {
                        if (item.GameCode.ToLower() == "ctzq")
                        {
                            var ctzqgametype = new List<string>() { "tr9", "t14c", "t4cjq", "t6bqc" };
                            foreach (var gametype in ctzqgametype)
                            {
                                var tempitem = AdList.FirstOrDefault(p => p.name == gametype);
                                if (tempitem == null)
                                {
                                    RetuenAdList.Add(new APP_Advertising() { name = gametype, desc = item.EnableStatus == 0 ? "欢迎购彩" : "暂未开售", flag = item.EnableStatus == 0 ? "1" : "0" });
                                }
                                else
                                {
                                    RetuenAdList.Add(new APP_Advertising() { name = gametype, desc = item.EnableStatus == 0 ? tempitem.desc : "暂未开售", flag = item.EnableStatus == 0 ? "1" : "0" });
                                }
                            }
                        }
                        else if (item.GameCode.ToLower() == "sjb")
                        {
                            var gyjitem = AdList.FirstOrDefault(p => p.name.ToLower() == "gyj");
                            if (gyjitem == null)
                            {
                                RetuenAdList.Add(new APP_Advertising() { name = "gyj", desc = item.EnableStatus == 0 ? "欢迎购彩" : "暂未开售", flag = item.EnableStatus == 0 ? "1" : "0" });
                            }
                            var gjitem = AdList.FirstOrDefault(p => p.name == "gj");
                            if (gjitem == null)
                            {
                                RetuenAdList.Add(new APP_Advertising() { name = "gj", desc = item.EnableStatus == 0 ? "欢迎购彩" : "暂未开售", flag = item.EnableStatus == 0 ? "1" : "0" });
                            }
                        }
                        else
                        {
                            var templist = AdList.Where(p => p.name.ToLower().StartsWith(item.GameCode.ToLower())).ToList();
                            if (templist != null && templist.Count > 0)
                            {
                                var list = templist.Select(p => new APP_Advertising()
                                {
                                    name = p.name,
                                    desc = item.EnableStatus == 0 ? p.desc : "暂未开售",
                                    flag = item.EnableStatus == 0 ? "1" : "0"
                                });
                                RetuenAdList.AddRange(list);
                            }
                            else
                            {
                                RetuenAdList.Add(new APP_Advertising() { name = item.GameCode.ToLower(), desc = item.EnableStatus == 0 ? "欢迎购彩" : "暂未开售", flag = item.EnableStatus == 0 ? "1" : "0" });
                            }
                        }
                    }
                    RedisHelperEx.DB_Other.SetObj(Key, RetuenAdList, TimeSpan.FromMinutes(5));
                    return Task.FromResult(RetuenAdList);
                }
                else
                {
                    return Task.FromResult(RedisValue);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("获取出错", ex);
            }
        }

        public Task<WinNumber_QueryInfo> GetWinNumber(string gameCode, string gameType, string issuseNumber)
        {
            try
            {
                var entity = new IssuseBusiness().QueryWinNumberByIssuseNumber(gameCode, gameType, issuseNumber);
                return Task.FromResult(new WinNumber_QueryInfo
                {
                    GameCode = gameCode,
                    IssuseNumber = issuseNumber,
                    WinNumber = entity == null ? string.Empty : entity.WinNumber,
                });
            }
            catch (Exception ex)
            {
                throw new Exception("获取出错", ex);
            }
        }

        /// <summary>
        /// 判断余额是否足够充值
        /// 冻结需要充值的金额，并生成一条游戏充值数据存入游戏交易表中，返回订单号
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public Task<CommonActionResult> FreezeGameRecharge(string userId, decimal money, string userDisplayName)
        {
            try
            {
                var loginBiz = new LocalLoginBusiness();
                var entity = loginBiz.QueryUserBalance(userId);
                var totalMoney = entity.GetTotalCashMoney();
                if (totalMoney < money) //余额不足
                {
                    return Task.FromResult(new CommonActionResult()
                    {
                        IsSuccess = false,
                        Message = "余额不足"
                    });
                }
                //存入游戏交易表中
                return Task.FromResult(new DataQuery().FreezeGameRecharge(userId, money, userDisplayName));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("执行出错", ex);
            }
        }

        /// <summary>
        /// 充值完成或失败，扣除冻结金额或返还冻结金额
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="IsSuccess"></param>
        /// <returns></returns>
        public Task<CommonActionResult> EndFreezeGameRecharge(string orderId, bool isSuccess, string providerSerialNo)
        {
            try
            {
                return Task.FromResult(new DataQuery().EndFreezeGameRecharge(orderId, isSuccess, providerSerialNo));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("执行出错", ex);
            }
        }



        /// <summary>
        /// 增加游戏交易信息到交易表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public Task<CommonActionResult> AddGameWithdraw(string userId, decimal money, string userDisplayName, string orderId, string providerSerialNo)
        {
            try
            {
                return Task.FromResult(new DataQuery().AddGameWithdraw(userId, money, userDisplayName, orderId, providerSerialNo));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("执行出错", ex);
            }
        }

        public Task<CommonActionResult> EndAddGameWithdraw(string OrderId, bool IsSuccess)
        {
            try
            {
                return Task.FromResult(new DataQuery().EndAddGameWithdraw(OrderId, IsSuccess));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("执行出错", ex);
            }
        }
        #region PC端相关接口
        /// <summary>
        /// 获取快速投注数据，后期可能修改redis
        /// </summary>
        /// <param name="GameCodeList"></param>
        /// <returns></returns>
        public Task<QuickBuyModel> GetQuickBuy_PC(List<string> GameCodeList)
        {
            try
            {
                var db = RedisHelperEx.DB_CoreCacheData;
                var redisKey = RedisKeys.PC_Index_QuickBuy;
                var redisModel = db.GetObj<QuickBuyModel>(redisKey);
                var ResultModel = new QuickBuyModel();
                ResultModel.SZCList = new List<Issuse_QueryInfo>();
                ResultModel.JCZQList = new List<JCZQ_MatchInfo_WEB>();
                var now = DateTime.Now;
                if (redisModel == null)//如果redis中没有则需要一条一条找
                {
                    foreach (var item in GameCodeList)
                    {
                        var currentGame = QueryCurrentIssuseInfo(item).Result;
                        if (currentGame != null)
                        {
                            ResultModel.SZCList.Add(currentGame);
                        }
                    }
                    var key = EntityModel.Redis.RedisKeys.Key_JCZQ_Match_Odds_List;
                    string reidskey = $"{key}_HHDG1";
                    //查找竞彩足球数据
                    var JczqList = GetJCZQMatchOddsList_ByRedis(reidskey).Result;
                    if (JczqList != null)
                    {
                        ResultModel.JCZQList = JczqList.Where(p => Convert.ToDateTime(p.FSStopBettingTime) > now).ToList();
                    }
                }
                else//如果redis中存在，则把符合条件的数据返回，不符合的再从数据库中查找
                {
                    foreach (var item in GameCodeList)
                    {
                        var oldmodel = redisModel.SZCList.FirstOrDefault(p => p.Game.GameCode.IndexOf(item, StringComparison.OrdinalIgnoreCase) > -1);
                        if (oldmodel != null)
                        {
                            if (oldmodel.LocalStopTime > now)
                            {
                                ResultModel.SZCList.Add(oldmodel);
                                continue;
                            }

                        }
                        var currentGame = QueryCurrentIssuseInfo(item).Result;
                        if (currentGame != null)
                        {
                            ResultModel.SZCList.Add(currentGame);
                        }
                    }
                    if (redisModel.JCZQList != null && redisModel.JCZQList.Count > 0)
                    {
                        ResultModel.JCZQList = redisModel.JCZQList.Where(p => Convert.ToDateTime(p.FSStopBettingTime) > now).ToList();
                    }
                }
                return Task.FromResult(ResultModel);
            }
            catch (Exception ex)
            {
                throw new Exception("获取出错", ex);
            }
        }

        /// <summary>
        /// 大奖排行榜
        /// </summary>
        /// <param name="QueryBase"></param>
        /// <returns></returns>
        public Task<RankReportCollection_BettingProfit_Sport> QueryRankReport_BigBonus_Sport(QueryBonusBase QueryBase)
        {
            try
            {
                return Task.FromResult(new DataQuery().QueryRankInfoList_BigBonus_Sport(QueryBase));
            }
            catch (Exception ex)
            {
                throw new Exception("大奖排行榜查询出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 发单盈利排行榜 - 竞彩类
        /// </summary>
        /// <param name="QueryBase"></param>
        /// <returns></returns>
        public Task<RankReportCollection_BettingProfit_Sport> QueryRankReport_BettingProfit_Sport(QueryBonusBase QueryBase)
        {
            try
            {
                return Task.FromResult(new DataQuery().QueryRankReport_BettingProfit_Sport(QueryBase));
            }
            catch (Exception ex)
            {
                throw new Exception("发单盈利排行榜查询出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 跟单盈利排行榜 - 竞彩类
        /// </summary>
        /// <param name="QueryBase"></param>
        /// <returns></returns>
        public Task<RankReportCollection_BettingProfit_Sport> QueryRankReport_JoinProfit_Sport(QueryBonusBase QueryBase)
        {
            try
            {
                return Task.FromResult(new DataQuery().QueryRankReport_JoinProfit_Sport(QueryBase));
            }
            catch (Exception ex)
            {
                throw new Exception("跟单盈利排行榜查询出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 合买人气排行
        /// </summary>
        /// <param name="QueryBase"></param>
        /// <returns></returns>
        public Task<RankReportCollection_RankInfo_HotTogether> QueryRankInfoList_HotTogether(QueryBonusBase QueryBase)
        {
            try
            {
                return Task.FromResult(new DataQuery().QueryRankInfoList_HotTogether(QueryBase));
            }
            catch (Exception ex)
            {
                throw new Exception("合买人气排行查询出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 成功的战绩排行_竞彩类
        /// </summary>
        /// <param name="QueryBase"></param>
        /// <returns></returns>
        public Task<RankReportCollection_BettingProfit_Sport> QueryRankInfoList_SuccessOrder_Sport(QueryBonusBase QueryBase)
        {
            try
            {
                return Task.FromResult(new DataQuery().QueryRankInfoList_SuccessOrder_Sport(QueryBase));
            }
            catch (Exception ex)
            {
                throw new Exception("成功的战绩排行查询出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 跟单排行榜
        /// </summary>
        /// <param name="QueryBase"></param>
        /// <returns></returns>
        public Task<RankReportCollection_RankInfo_BeFollower> QueryRankInfoList_BeFollowerCount(QueryBonusBase QueryBase)
        {
            try
            {
                return Task.FromResult(new DataQuery().QueryRankInfoList_BeFollowerCount(QueryBase));
            }
            catch (Exception ex)
            {
                throw new Exception("成功的战绩排行查询出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 累积中奖排行榜 - 竞彩类
        /// </summary>
        /// <param name="QueryBase"></param>
        /// <returns></returns>
        public Task<RankReportCollection_TotalBonus_Sport> QueryRankReport_TotalBonus_Sport(QueryBonusBase QueryBase)
        {
            try
            {
                return Task.FromResult(new DataQuery().QueryRankReport_TotalBonus_Sport(QueryBase));
            }
            catch (Exception ex)
            {
                throw new Exception("累积中奖排行榜查询出错 - " + ex.Message, ex);
            }
        }


        /// <summary>
        /// 查询成功派奖列表
        /// </summary>
        public Task<string> QueryPrizedIssuseList(string gameCode, string gameType, int length)
        {
            try
            {
                return Task.FromResult(new DataQuery().QueryPrizedIssuseList(gameCode, gameType, length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询成功派奖列表出错", ex);
            }
        }

        /// <summary>
        /// 中奖查询，公共数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <param name="issuseNumber"></param>
        /// <param name="completeData"></param>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Task<BonusOrderInfoCollection> QueryBonusInfoList(string userId, string gameCode, string gameType, string issuseNumber, string completeData, string key, int pageIndex, int pageSize)
        {
            try
            {
                return Task.FromResult(new DataQuery().QueryBonusInfoList(userId, gameCode, gameType, issuseNumber, completeData, key, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                throw new Exception("查询成功派奖列表出错", ex);
            }
        }


        /// <summary>
        /// 中奖排行榜_按彩种查
        /// </summary>
        public Task<RankReportCollection_TotalBonus_Sport> QueryRankReport_BonusByGameCode_All(DateTime fromDate, DateTime toDate, int topCount, string gameCode)
        {
            try
            {
                return Task.FromResult(new DataQuery().QueryRankReport_BonusByGameCode_All(fromDate, toDate, topCount, gameCode));
            }
            catch (Exception ex)
            {
                throw new Exception("中奖查询出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询开奖历史(存到redis，三分钟)
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Task<WinNumber_QueryInfoCollection> QueryWinNumberHistory(string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                var RedisKey = RedisKeys.PC_QueryWinNumberHistory;
                var Key = $"{RedisKey}_{gameCode}_{startTime.ToString("yyyyMMdd")}_{endTime.ToString("yyyyMMdd")}_{pageIndex}_{pageSize}";
                var db = RedisHelperEx.DB_Other;
                var OldRedisList = db.GetObj<WinNumber_QueryInfoCollection>(RedisKey);
                if (OldRedisList != null)
                {
                    return Task.FromResult(OldRedisList);
                }
                var Model = new IssuseBusiness().QueryWinNumber(gameCode, startTime, endTime, pageIndex, pageSize);
                db.SetObj(Key, Model, TimeSpan.FromMinutes(3));
                return Task.FromResult(Model);
            }
            catch (Exception ex)
            {
                throw new Exception("查询开奖历史出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询开奖历史(存到redis，三分钟)
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Task<WinNumber_QueryInfoCollection> QueryWinNumberHistoryByCount(string gameCode, int count)
        {
            try
            {
                var RedisKey = RedisKeys.PC_QueryWinNumberHistoryByCount;
                var Key = $"{RedisKey}_{gameCode}_{count}";
                var db = RedisHelperEx.DB_Other;
                var OldRedisList = db.GetObj<WinNumber_QueryInfoCollection>(RedisKey);
                if (OldRedisList != null)
                {
                    return Task.FromResult(OldRedisList);
                }
                var Model = new IssuseBusiness().QueryWinNumber(gameCode, count);
                db.SetObj(Key, Model, TimeSpan.FromMinutes(3));
                return Task.FromResult(Model);
            }
            catch (Exception ex)
            {
                throw new Exception("查询开奖历史出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询开奖号码
        /// </summary>
        /// <param name="gameCode"></param>
        /// <returns></returns>
        public Task<GameWinNumber_Info> QueryNewWinNumber(string gameCode)
        {
            var list =new OrderQuery().QueryGameWinNumber(gameCode, 0, 1);
            if (list.List.Count == 0)
            return Task.FromResult(new GameWinNumber_Info());
            return Task.FromResult(list.List[0]);
        }

        /// <summary>
        /// 查询开奖号码
        /// </summary>
        /// <param name="gameType">仅传统足球需要传玩法</param>
        /// <returns></returns>
        public Task<GameWinNumber_InfoCollection> QueryGameWinNumber(string gameCode, int pageIndex, int pageSize)
        {
            try
            {
                switch (gameCode.ToUpper())
                {
                    case "CQ11X5":
                        return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_GameWinNumber(pageIndex, pageSize));
                    case "CQKLSF":
                        return Task.FromResult(new LotteryDataBusiness_CQKLSF().QueryCQKLSF_GameWinNumber(pageIndex, pageSize));
                    case "CQSSC":
                        return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_GameWinNumber(pageIndex, pageSize));
                    case "DF6J1":
                        return Task.FromResult(new LotteryDataBusiness_DF6_1().QueryDF6_1_GameWinNumber(pageIndex, pageSize));
                    case "DLT":
                        return Task.FromResult(new LotteryDataBusiness_DLT().QueryDLT_GameWinNumber(pageIndex, pageSize));
                    case "FC3D":
                        return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_GameWinNumber(pageIndex, pageSize));
                    case "GD11X5":
                        return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_GameWinNumber(pageIndex, pageSize));
                    case "GDKLSF":
                        return Task.FromResult(new LotteryDataBusiness_GDKLSF().QueryGDKLSF_GameWinNumber(pageIndex, pageSize));
                    case "HBK3":
                        return Task.FromResult(new LotteryDataBusiness_HBK3().QueryHBK3_GameWinNumber(pageIndex, pageSize));
                    case "HC1":
                        return Task.FromResult(new LotteryDataBusiness_HC1().QueryHC1_GameWinNumber(pageIndex, pageSize));
                    case "HD15X5":
                        return Task.FromResult(new LotteryDataBusiness_HD15X5().QueryHD15X5_GameWinNumber(pageIndex, pageSize));
                    case "HNKLSF":
                        return Task.FromResult(new LotteryDataBusiness_HNKLSF().QueryHNKLSF_GameWinNumber(pageIndex, pageSize));
                    case "JLK3":
                        return Task.FromResult(new LotteryDataBusiness_JLK3().QueryJLK3_GameWinNumber(pageIndex, pageSize));
                    case "JSKS":
                        return Task.FromResult(new LotteryDataBusiness_JSK3().QueryJSK3_GameWinNumber(pageIndex, pageSize));
                    case "JX11X5":
                        return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_GameWinNumber(pageIndex, pageSize));
                    case "JXSSC":
                        return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_GameWinNumber(pageIndex, pageSize));
                    case "LN11X5":
                        return Task.FromResult(new LotteryDataBusiness_LN11X5().QueryLN11X5_GameWinNumber(pageIndex, pageSize));
                    case "PL3":
                        return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_GameWinNumber(pageIndex, pageSize));
                    case "PL5":
                        return Task.FromResult(new LotteryDataBusiness_PL5().QueryPL5_GameWinNumber(pageIndex, pageSize));
                    case "QLC":
                        return Task.FromResult(new LotteryDataBusiness_QLC().QueryQLC_GameWinNumber(pageIndex, pageSize));
                    case "QXC":
                        return Task.FromResult(new LotteryDataBusiness_QXC().QueryQXC_GameWinNumber(pageIndex, pageSize));
                    case "SDQYH":
                        return Task.FromResult(new LotteryDataBusiness_SDQYH().QuerySDQYH_GameWinNumber(pageIndex, pageSize));
                    case "SSQ":
                        return Task.FromResult(new LotteryDataBusiness_SSQ().QuerySSQ_GameWinNumber(pageIndex, pageSize));
                    case "SD11X5":
                        return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_GameWinNumber(pageIndex, pageSize));
                    case "SDKLPK3":
                        return Task.FromResult(new LotteryDataBusiness_SDKLPK3().QuerySDKLPK3_GameWinNumber(pageIndex, pageSize));
                    case "CTZQ_T14C":
                    case "CTZQ_TR9":
                    case "CTZQ_T6BQC":
                    case "CTZQ_T4CJQ":
                        return Task.FromResult(new LotteryDataBusiness_CTZQ(gameCode).QueryCTZQ_GameWinNumber(pageIndex, pageSize));
                    default:
                        break;
                }
                throw new Exception("没有匹配的彩种: " + gameCode);
            }
            catch (Exception ex)
            {
                throw new Exception("查询号码数据 - " + ex.Message, ex);
            }
        }


        #endregion


    }


}
