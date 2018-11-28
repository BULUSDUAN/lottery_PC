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
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;
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
        public Task<CommonActionResult> FreezeGameRecharge(string userId, decimal money, string userDisplayName, int gameType)
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
                return Task.FromResult(new DataQuery().FreezeGameRecharge(userId, money, userDisplayName, gameType));
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

        ///// <summary>
        ///// 把外部订单号存入数据库
        ///// </summary>
        ///// <returns></returns>
        //public Task<CommonActionResult> GameRecharge_Step2(string orderId, string providerSerialNo)
        //{
        //    try
        //    {
        //        return Task.FromResult(new DataQuery().GameRecharge_Step2(orderId, providerSerialNo));
        //    }
        //    catch (LogicException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("执行出错", ex);
        //    }
        //}

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

        public Task<CommonActionResult> AddGameWithdraw_Step1(string userId, decimal money, string userDisplayName, int gameType)
        {
            try
            {
                return Task.FromResult(new DataQuery().AddGameWithdraw_Step1(userId, money, userDisplayName, gameType));
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

        //public Task<CommonActionResult> AddGameWithdraw_Step2(string orderId, string providerSerialNo)
        //{
        //    try
        //    {
        //        return Task.FromResult(new DataQuery().AddGameWithdraw_Step2(orderId, providerSerialNo));
        //    }
        //    catch (LogicException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("执行出错", ex);
        //    }
        //}

        public Task<CommonActionResult> EndAddGameWithdraw(string OrderId, bool IsSuccess, string providerSerialNo)
        {
            try
            {
                return Task.FromResult(new DataQuery().EndAddGameWithdraw(OrderId, IsSuccess, providerSerialNo));
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
        /// <summary>
        /// 基本走势
        /// </summary>
        public Task<SSQ_JiBenZouSi_InfoCollection> QueryCache_SSQ_JiBenZouSi_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SSQ().QuerySSQ_JiBenZouSi(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询双色球基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 大小走势
        /// </summary>
        public Task<SSQ_DX_InfoCollection> QueryCache_SSQ_DX_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SSQ().QuerySSQ_DX(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大小基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 除3走势
        /// </summary>
        public Task<SSQ_C3_InfoCollection> QueryCache_SSQ_C3_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SSQ().QuerySSQ_C3(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询除3基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 奇偶走势
        /// </summary>
        public Task<SSQ_JiOu_InfoCollection> QueryCache_SSQ_JiOu_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SSQ().QuerySSQ_JiOu(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询奇偶基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 跨度SW走势
        /// </summary>
        public Task<SSQ_KuaDu_SW_InfoCollection> QueryCache_SSQ_KuaDu_SW_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SSQ().QuerySSQ_KuaDu_SW(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询跨度SW基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 和值走势
        /// </summary>
        public Task<SSQ_HeZhi_InfoCollection> QueryCache_SSQ_HeZhi_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SSQ().QuerySSQ_HeZhi(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询和值基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 质合走势
        /// </summary>
        public Task<SSQ_ZhiHe_InfoCollection> QueryCache_SSQ_ZhiHe_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SSQ().QuerySSQ_ZhiHe(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询质合基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 跨度1_6走势
        /// </summary>
        public Task<SSQ_KuaDu_1_6_InfoCollection> QueryCache_SSQ_KuaDu_1_6_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SSQ().QuerySSQ_KuaDu_1_6(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询跨度1_6基本走势 - " + ex.Message, ex);
            }
        }

        #region 大乐透走势
       
        /// <summary>
        /// 查询除3走势
        /// </summary>
        public Task<DLT_Chu3_InfoCollection> QueryDLT_Chu3_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DLT().QueryDLT_Chu3_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大乐透除3走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询大小走势
        /// </summary>
        public Task<DLT_DX_InfoCollection> QueryDLT_DX_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DLT().QueryDLT_DX_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大乐透大小走势 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询和值走势
        /// </summary>
        public Task<DLT_HeZhi_InfoCollection> QueryDLT_HeZhi_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DLT().QueryDLT_HeZhi_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大乐透和值走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询基本走势
        /// </summary>
        public Task<DLT_JiBenZouSi_InfoCollection> QueryDLT_JiBenZouSi_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DLT().QueryDLT_JiBenZouSi_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大乐透基本走势 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询奇偶走势
        /// </summary>
        public Task<DLT_JiOu_InfoCollection> QueryDLT_JiOu_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DLT().QueryDLT_JiOu_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大乐透奇偶走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        ///查询12走势列表按时间倒叙 
        /// </summary>
        public Task<DLT_KuaDu_12_InfoCollection> QueryDLT_KuaDu_12_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DLT().QueryDLT_KuaDu_12_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大乐透12走势 - " + ex.Message, ex);
            }
        }
        /// <summary>
        ///查询23走势列表按时间倒叙 
        /// </summary>
        public Task<DLT_KuaDu_23_InfoCollection> QueryDLT_KuaDu_23_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DLT().QueryDLT_KuaDu_23_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大乐透23走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        ///查询34走势列表按时间倒叙 
        /// </summary>
        public Task<DLT_KuaDu_34_InfoCollection> QueryDLT_KuaDu_34_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DLT().QueryDLT_KuaDu_34_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大乐透34走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        ///查询45走势列表按时间倒叙 
        /// </summary>
        public Task<DLT_KuaDu_45_InfoCollection> QueryDLT_KuaDu_45_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DLT().QueryDLT_KuaDu_45_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大乐透45走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        ///查询首尾走势列表按时间倒叙 
        /// </summary>
        public Task<DLT_KuaDu_SW_InfoCollection> QueryDLT_KuaDu_SW_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DLT().QueryDLT_KuaDu_SW_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大乐透首尾走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询质和走势
        /// </summary>
        public Task<DLT_ZhiHe_InfoCollection> QueryDLT_ZhiHe_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DLT().QueryDLT_ZhiHe_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大乐透质和走势 - " + ex.Message, ex);
            }
        }
        #endregion

        #region  福彩3D走势
        /// <summary>
        /// 直选走势
        /// </summary>
        public Task<FC3D_ZhiXuanZouSi_InfoCollection> QueryCache_FC3D_ZhiXuanZouSi_Info(int index)
        {
            try
            {
                return  Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_ZhiXuanZouSi(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询直选走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 总跨度
        /// </summary>
        public Task<FC3D_KuaDu_Z_InfoCollection> QueryCache_FC3D_KuaDu_Z_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_KuaDu_Z(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询总跨度 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 和值走势
        /// </summary>
        public Task<FC3D_HZZS_InfoCollection> QueryCache_FC3D_HZZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_HZZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询和值走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 奇偶号码
        /// </summary>
        public Task<FC3D_JOHM_InfoCollection> QueryCache_FC3D_JOHM_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_JOHM(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询奇偶号码 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 除3_3
        /// </summary>
        public Task<FC3D_Chu33_InfoCollection> QueryCache_FC3D_Chu33_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_Chu33(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询除3_3 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 大小号码
        /// </summary>
        public Task<FC3D_DXHM_InfoCollection> QueryCache_FC3D_DXHM_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_DXHM(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大小号码 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 组选走势
        /// </summary>
        public Task<FC3D_ZuXuanZouSi_InfoCollection> QueryCache_FC3D_ZuXuanZouSi_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_ZuXuanZouSi(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询组选走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 除3_1
        /// </summary>
        public Task<FC3D_Chu31_InfoCollection> QueryCache_FC3D_Chu31_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_Chu31(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询除3_1 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 除3_2
        /// </summary>
        public Task<FC3D_Chu32_InfoCollection> QueryCache_FC3D_Chu32_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_Chu32(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询除3_2 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 跨度百位、十位
        /// </summary>
        public Task<FC3D_KuaDu_12_InfoCollection> QueryCache_FC3D_KuaDu_12_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_KuaDu_12(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询跨度百位、十位 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 跨度百位、个位
        /// </summary>
        public Task<FC3D_KuaDu_13_InfoCollection> QueryCache_FC3D_KuaDu_13_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_KuaDu_13(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询跨度百位、个位 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 跨度十位、个位
        /// </summary>
        public Task<FC3D_KuaDu_23_InfoCollection> QueryCache_FC3D_KuaDu_23_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_KuaDu_23(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询跨度十位、个位 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 大小形态走势
        /// </summary>
        public Task<FC3D_DXXT_InfoCollection> QueryCache_FC3D_DXXT_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_DXXT(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大小形态走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 和值分布
        /// </summary>
        public Task<FC3D_HZFB_InfoCollection> QueryCache_FC3D_HZFB_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_HZFB(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询和值分布 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 和值特征
        /// </summary>
        public Task<FC3D_HZTZ_InfoCollection> QueryCache_FC3D_HZTZ_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_HZTZ(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询和值特征 - " + ex.Message, ex);
            }
        }


        /// <summary>
        /// 奇偶形态
        /// </summary>
        public Task<FC3D_JOXT_InfoCollection> QueryCache_FC3D_JOXT_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_JOXT(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询奇偶形态 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 质合号码
        /// </summary>
        public Task<FC3D_ZHHM_InfoCollection> QueryCache_FC3D_ZHHM_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_ZHHM(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询质合号码 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 质合形态
        /// </summary>
        public Task<FC3D_ZHXT_InfoCollection> QueryCache_FC3D_ZHXT_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_FC3D().QueryFC3D_ZHXT(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询质合形态 - " + ex.Message, ex);
            }
        }

        #endregion

        #region 排列3
        /// <summary>
        /// 查询基本走势
        /// </summary>
        public Task<PL3_JiBenZouSi_InfoCollection> QueryPL3_JiBenZouSi_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_JiBenZouSi_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询组选走势
        /// </summary>
        public Task<PL3_ZuXuanZouSi_InfoCollection> QueryPL3_ZuXuanZouSi_info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_ZuXuanZouSi_info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3组选走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询大小走势
        /// </summary>
        public Task<PL3_DX_InfoCollection> QueryPL3_DX_info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_DX_info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3大小走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询大小号码走势
        /// </summary>
        public Task<PL3_DXHM_InfoCollection> QueryPL3_DXHM_info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_DXHM_info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3大小号码走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询奇偶走势
        /// </summary>
        public Task<PL3_JIOU_InfoCollection> QueryPL3_JIOU_info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_JIOU_info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3奇偶走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询奇偶号码走势
        /// </summary>
        public Task<PL3_JOHM_InfoCollection> QueryPL3_JOHM_info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_JOHM_info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3奇偶号码走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询质和走势
        /// </summary>
        public Task<PL3_ZhiHe_InfoCollection> QueryPL3_ZhiHe_info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_ZhiHe_info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3质和走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询质和号码走势
        /// </summary>
        public Task<PL3_ZHHM_InfoCollection> QueryPL3_ZHHM_info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_ZHHM_info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3质和号码走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询和值走势
        /// </summary>
        public Task<PL3_HeiZhi_InfoCollection> QueryPL3_HeiZhi_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_HeiZhi_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3质和走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询跨度百位、十位走势
        /// </summary>
        public Task<PL3_KuaDu_12_InfoCollection> QueryPL3_KuaDu_12_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_KuaDu_12_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3跨度百位、十位走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询跨度百位、个位走势
        /// </summary>
        public Task<PL3_KuaDu_13_InfoCollection> QueryPL3_KuaDu_13_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_KuaDu_13_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3跨度百位、个位走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询跨度十位、个位走势
        /// </summary>
        public Task<PL3_KuaDu_23_InfoCollection> QueryPL3_KuaDu_23_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_KuaDu_23_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3跨度十位、个位走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询除3 走势1走势
        /// </summary>
        public Task<PL3_Chu31_InfoCollection> QueryPL3_PL3_Chu31_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_PL3_Chu31_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3除31走势走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询除3 走势2走势
        /// </summary>
        public Task<PL3_Chu32_InfoCollection> QueryPL3_PL3_Chu32_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_PL3_Chu32_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3除32走势走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询除3 走势3走势
        /// </summary>
        public Task<PL3_Chu33_InfoCollection> QueryPL3_PL3_Chu33_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_PL3_Chu33_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3除33走势走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询和值特征 走势3走势
        /// </summary>
        public Task<PL3_HZTZ_InfoCollection> QueryPL3_PL3_HZTZ_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_PL3_HZTZ_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3和值特征走势走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询和值合尾 走势3走势
        /// </summary>
        public Task<PL3_HZHW_InfoCollection> QueryPL3_PL3_HZHW_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL3().QueryPL3_PL3_HZHW_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列3和值合尾走势走势 - " + ex.Message, ex);
            }
        }


        #endregion

        #region 重庆时时彩走势
        /// <summary>
        /// 大小单双     
        /// </summary>
        public Task<CQSSC_DXDS_InfoCollection> QueryCache_CQSSC_DXDS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_DXDS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大小单双- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 重庆1星走势
        /// </summary>
        public Task<CQSSC_1X_ZS_InfoCollection> QueryCache_CQSSC_1X_ZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_1X_ZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询重庆1星走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 重庆2星和值走势       
        /// </summary>
        public Task<CQSSC_2X_HZZS_InfoCollection> QueryCache_CQSSC_2X_HZZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_2X_HZZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询重庆2星和值走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 重庆2星组选走势       
        /// </summary>
        public Task<CQSSC_2X_ZuXZS_InfoCollection> QueryCache_CQSSC_2X_ZuXZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_2X_ZuXZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询重庆2星组选走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 重庆2星直选走势     
        /// </summary>
        public Task<CQSSC_2X_ZXZS_InfoCollection> QueryCache_CQSSC_2X_ZXZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_2X_ZXZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询重庆2星直选走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 除3  
        /// </summary>
        public Task<CQSSC_3X_C3YS_InfoCollection> QueryCache_CQSSC_3X_C3YS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_3X_C3YS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询除3- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 大小走势       
        /// </summary>
        public Task<CQSSC_3X_DXZS_InfoCollection> QueryCache_CQSSC_3X_DXZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_3X_DXZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大小走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 和值走势 
        /// </summary>
        public Task<CQSSC_3X_HZZS_InfoCollection> QueryCache_CQSSC_3X_HZZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_3X_HZZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询和值走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 奇偶走势    
        /// </summary>
        public Task<CQSSC_3X_JOZS_InfoCollection> QueryCache_CQSSC_3X_JOZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_3X_JOZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询奇偶走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 跨度   
        /// </summary>
        public Task<CQSSC_3X_KD_InfoCollection> QueryCache_CQSSC_3X_KD_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_3X_KD(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询跨度- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 质合走势  
        /// </summary>
        public Task<CQSSC_3X_ZHZS_InfoCollection> QueryCache_CQSSC_3X_ZHZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_3X_ZHZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询质合走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 重庆3星组选走势       
        /// </summary>
        public Task<CQSSC_3X_ZuXZS_InfoCollection> QueryCache_CQSSC_3X_ZuXZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_3X_ZuXZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询重庆3星组选走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 重庆3星直选走势    
        /// </summary>
        public Task<CQSSC_3X_ZXZS_InfoCollection> QueryCache_CQSSC_3X_ZXZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_3X_ZXZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询重庆3星直选走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 和值走势   
        /// </summary>
        public Task<CQSSC_5X_HZZS_InfoCollection> QueryCache_CQSSC_5X_HZZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_5X_HZZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询和值走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 重庆5星基本走势       
        /// </summary>
        public Task<CQSSC_5X_JBZS_InfoCollection> QueryCache_CQSSC_5X_JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQSSC().QueryCQSSC_5X_JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询重庆5星基本走势- " + ex.Message, ex);
            }
        }
        #endregion

        #region 江西11选5

        /// <summary>
        /// 查询基本走势
        /// </summary>
        public Task<JX11X5_RXJBZS_InfoCollection> QueryJX11X5_RXJBZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_RXJBZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5任选基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 任选大小
        /// </summary>
        public Task<JX11X5_RXDX_InfoCollection> QueryJX11X5_RXDX_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_RXDX_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5任选大小走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 任选奇偶
        /// </summary>
        public Task<JX11X5_RXJO_InfoCollection> QueryJX11X5_RXJO_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_RXJO_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5任选奇偶走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 任选质和
        /// </summary>
        public Task<JX11X5_RXZH_InfoCollection> QueryJX11X5_RXZH_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_RXZH_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5任选质和走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 任选和值
        /// </summary>
        public Task<JX11X5_RXHZ_InfoCollection> QueryJX11X5_RXHZ_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_RXHZ_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5任选和值走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 任选除3
        /// </summary>
        public Task<JX11X5_Chu3_InfoCollection> QueryJX11X5_Chu3_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_Chu3_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5任选除3走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 任选第一位
        /// </summary>
        public Task<JX11X5_RX1_InfoCollection> QueryJX11X5_RX1_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_RX1_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5任选第一位走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 任选第二位
        /// </summary>
        public Task<JX11X5_RX2_InfoCollection> QueryJX11X5_RX2_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_RX2_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5任选第二位走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 任选第三位
        /// </summary>
        public Task<JX11X5_RX3_InfoCollection> QueryJX11X5_RX3_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_RX3_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5任选第三位走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 任选第四位
        /// </summary>
        public Task<JX11X5_RX4_InfoCollection> QueryJX11X5_RX4_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_RX4_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5任选第四位走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 任选第五位
        /// </summary>
        public Task<JX11X5_RX5_InfoCollection> QueryJX11X5_RX5_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_RX5_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5任选第五位走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 前三直选
        /// </summary>
        public Task<JX11X5_Q3ZS_InfoCollection> QueryJX11X5_Q3ZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_Q3ZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5前三直选走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 前三组选
        /// </summary>
        public Task<JX11X5_Q3ZUS_InfoCollection> QueryJX11X5_Q3ZUS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_Q3ZUS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5前三组选走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 前三大小
        /// </summary>
        public Task<JX11X5_Q3DX_InfoCollection> QueryJX11X5_Q3DX_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_Q3DX_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5前三大小走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 前三基偶
        /// </summary>
        public Task<JX11X5_Q3JO_InfoCollection> QueryJX11X5_Q3JO_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_Q3JO_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5前三大小走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 前三质和
        /// </summary>
        public Task<JX11X5_Q3ZH_InfoCollection> QueryJX11X5_Q3ZH_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_Q3ZH_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5前三质和走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 前三除3
        /// </summary>
        public Task<JX11X5_Q3Chu3_InfoCollection> QueryJX11X5_Q3Chu3_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_Q3Chu3_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5前三除3走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 前三和值
        /// </summary>
        public Task<JX11X5_Q3HZ_InfoCollection> QueryJX11X5_Q3HZ_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_Q3HZ_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5前三和值走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 前2直选
        /// </summary>
        public Task<JX11X5_Q2ZS_InfoCollection> QueryJX11X5_Q2ZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_Q2ZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5前2直选走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 前2组选
        /// </summary>
        public Task<JX11X5_Q2ZUS_InfoCollection> QueryJX11X5_Q2ZUS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_Q2ZUS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5前2组选走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 前2和值
        /// </summary>
        public Task<JX11X5_Q2HZ_InfoCollection> QueryJX11X5_Q2HZ_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JX11X5().QueryJX11X5_Q2HZ_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西11选5前2和值走势 - " + ex.Message, ex);
            }
        }
        #endregion

        #region 山东11选5
        public Task<YDJ11_012DWZS_InfoCollection> QueryCache_YDJ11_012DWZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_012DWZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询012定位走势 - " + ex.Message, ex);
            }
        }
        public Task<YDJ11_012LZZS_InfoCollection> QueryCache_YDJ11_012LZZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_012LZZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询路比值走势 - " + ex.Message, ex);
            }
        }
        public Task<YDJ11_2LZS_InfoCollection> QueryCache_YDJ11_2LZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_2LZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询2连走势 - " + ex.Message, ex);
            }
        }
        public Task<YDJ11_CHZS_InfoCollection> QueryCache_YDJ11_CHZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_CHZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询重号走势 - " + ex.Message, ex);
            }
        }
        public Task<YDJ11_DLZS_InfoCollection> QueryCache_YDJ11_DLZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_DLZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询多连走势 - " + ex.Message, ex);
            }
        }
        public Task<YDJ11_GHZS_InfoCollection> QueryCache_YDJ11_GHZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_GHZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询隔号走势 - " + ex.Message, ex);
            }
        }
        public Task<YDJ11_HZZS_InfoCollection> QueryCache_YDJ11_HZZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_HZZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询和值走势 - " + ex.Message, ex);
            }
        }
        public Task<YDJ11_JBZS_InfoCollection> QueryCache_YDJ11_JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询基本走势 - " + ex.Message, ex);
            }
        }
        public Task<YDJ11_KDZS_InfoCollection> QueryCache_YDJ11_KDZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_KDZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询跨度走势 - " + ex.Message, ex);
            }
        }
        public Task<YDJ11_Q1JBZS_InfoCollection> QueryCache_YDJ11_Q1JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_Q1JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前1基本走势 - " + ex.Message, ex);
            }
        }
        public Task<YDJ11_Q1XTZS_InfoCollection> QueryCache_YDJ11_Q1XTZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_Q1XTZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前1形态走势 - " + ex.Message, ex);
            }
        }
        public Task<YDJ11_Q2JBZS_InfoCollection> QueryCache_YDJ11_Q2JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_Q2JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前2基本走势 - " + ex.Message, ex);
            }
        }
        public Task<YDJ11_Q2XTZS_InfoCollection> QueryCache_YDJ11_Q2XTZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_Q2XTZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前2形态走势 - " + ex.Message, ex);
            }
        }
        public Task<YDJ11_Q3JBZS_InfoCollection> QueryCache_YDJ11_Q3JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_Q3JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前3基本走势 - " + ex.Message, ex);
            }
        }
        public Task<YDJ11_Q3XTZS_InfoCollection> QueryCache_YDJ11_Q3XTZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_Q3XTZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前3形态走势 - " + ex.Message, ex);
            }
        }
        public Task<YDJ11_XTZS_InfoCollection> QueryCache_YDJ11_XTZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_YDJ11().QueryYDJ11_XTZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询形态走势 - " + ex.Message, ex);
            }
        }
        #endregion

        #region 广东11选5

        public Task<GD11X5_012DWZS_InfoCollection> QueryCache_GD11X5_012DWZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_012DWZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询012定位走势 - " + ex.Message, ex);
            }
        }
        public Task<GD11X5_012LZZS_InfoCollection> QueryCache_GD11X5_012LZZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_012LZZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询路比值走势 - " + ex.Message, ex);
            }
        }
        public Task<GD11X5_2LZS_InfoCollection> QueryCache_GD11X5_2LZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_2LZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询2连走势 - " + ex.Message, ex);
            }
        }
        public Task<GD11X5_CHZS_InfoCollection> QueryCache_GD11X5_CHZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_CHZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询重号走势 - " + ex.Message, ex);
            }
        }
        public Task<GD11X5_DLZS_InfoCollection> QueryCache_GD11X5_DLZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_DLZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询多连走势 - " + ex.Message, ex);
            }
        }
        public Task<GD11X5_GHZS_InfoCollection> QueryCache_GD11X5_GHZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_GHZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询隔号走势 - " + ex.Message, ex);
            }
        }
        public Task<GD11X5_HZZS_InfoCollection> QueryCache_GD11X5_HZZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_HZZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询和值走势 - " + ex.Message, ex);
            }
        }
        public Task<GD11X5_JBZS_InfoCollection> QueryCache_GD11X5_JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询基本走势 - " + ex.Message, ex);
            }
        }
        public Task<GD11X5_KDZS_InfoCollection> QueryCache_GD11X5_KDZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_KDZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询跨度走势 - " + ex.Message, ex);
            }
        }
        public Task<GD11X5_Q1JBZS_InfoCollection> QueryCache_GD11X5_Q1JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_Q1JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前1基本走势 - " + ex.Message, ex);
            }
        }
        public Task<GD11X5_Q1XTZS_InfoCollection> QueryCache_GD11X5_Q1XTZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_Q1XTZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前1形态走势 - " + ex.Message, ex);
            }
        }
        public Task<GD11X5_Q2JBZS_InfoCollection> QueryCache_GD11X5_Q2JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_Q2JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前2基本走势 - " + ex.Message, ex);
            }
        }
        public Task<GD11X5_Q2XTZS_InfoCollection> QueryCache_GD11X5_Q2XTZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_Q2XTZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前2形态走势 - " + ex.Message, ex);
            }
        }
        public Task<GD11X5_Q3JBZS_InfoCollection> QueryCache_GD11X5_Q3JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_Q3JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前3基本走势 - " + ex.Message, ex);
            }
        }
        public Task<GD11X5_Q3XTZS_InfoCollection> QueryCache_GD11X5_Q3XTZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_Q3XTZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前3形态走势 - " + ex.Message, ex);
            }
        }
        public Task<GD11X5_XTZS_InfoCollection> QueryCache_GD11X5_XTZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GD11X5().QueryGD11X5_XTZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询形态走势 - " + ex.Message, ex);
            }
        }

        #endregion

        #region 广东快乐10分
        /// <summary>
        /// 基本走势
        /// </summary>
        public Task<GDKLSF_JBZS_InfoCollection> QueryGDKLSF_JBZS(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GDKLSF().QueryGDKLSF_JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        ///  定位第一位走势
        /// </summary>
        public Task<GDKLSF_DW1_InfoCollection> QueryGDKLSF_DW1(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GDKLSF().QueryGDKLSF_DW1(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询定位第一位走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        ///  定位第二位走势
        /// </summary>
        public Task<GDKLSF_DW2_InfoCollection> QueryGDKLSF_DW2(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GDKLSF().QueryGDKLSF_DW2(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询定位第二位走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        ///  定位第三位走势
        /// </summary>
        public Task<GDKLSF_DW3_InfoCollection> QueryGDKLSF_DW3(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GDKLSF().QueryGDKLSF_DW3(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询定位第三位走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 大小走势
        /// </summary>
        public Task<GDKLSF_DX_InfoCollection> QueryGDKLSF_DX(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GDKLSF().QueryGDKLSF_DX(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大小走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 奇偶走势
        /// </summary>
        public Task<GDKLSF_JO_InfoCollection> QueryGDKLSF_JO(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GDKLSF().QueryGDKLSF_JO(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询奇偶走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 质和走势
        /// </summary>
        public Task<GDKLSF_ZH_InfoCollection> QueryGDKLSF_ZH(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_GDKLSF().QueryGDKLSF_ZH(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询质和走势 - " + ex.Message, ex);
            }
        }
        #endregion

        #region 江苏快3走势图
        /// <summary>
        /// 查询基本走势
        /// </summary>
        public Task<JSK3_JBZS_InfoCollection> QueryJSK3_JBZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JSK3().QueryJSK3_JBZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江苏快3基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询和值走势
        /// </summary>
        public Task<JSK3_HZ_InfoCollection> QueryJSK3_HZ_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JSK3().QueryJSK3_HZ_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江苏快3和值走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询形态走势
        /// </summary>
        public Task<JSK3_XT_InfoCollection> QueryJSK3_XT_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JSK3().QueryJSK3_XT_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江苏快3形态走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询组合走势
        /// </summary>
        public Task<JSK3_ZH_InfoCollection> QueryJSK3_ZH_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JSK3().QueryJSK3_ZH_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江苏快3组合走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询综合走势
        /// </summary>
        public Task<JSK3_ZHZS_InfoCollection> QueryJSK3_ZHZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JSK3().QueryJSK3_ZHZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江苏快3综合走势 - " + ex.Message, ex);
            }
        }

        #endregion

        #region 山东快乐扑克3走势图

        /// <summary>
        /// 查询基本走势
        /// </summary>
        public Task<SDKLPK3_JBZS_InfoCollection> QueryCache_SDKLPK3_JBZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDKLPK3().QuerySDKLPK3_JBZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询组选走势
        /// </summary>
        public Task<SDKLPK3_ZHXZS_InfoCollection> QueryCache_SDKLPK3_ZHXZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDKLPK3().QuerySDKLPK3_ZHXZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询组选走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询花色走势
        /// </summary>
        public Task<SDKLPK3_HSZS_InfoCollection> QueryCache_SDKLPK3_HSZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDKLPK3().QuerySDKLPK3_HSZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询花色走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询大小走势
        /// </summary>
        public Task<SDKLPK3_DXZS_InfoCollection> QueryCache_SDKLPK3_DXZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDKLPK3().QuerySDKLPK3_DXZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大小走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询奇偶走势
        /// </summary>
        public Task<SDKLPK3_JOZS_InfoCollection> QueryCache_SDKLPK3_JOZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDKLPK3().QuerySDKLPK3_JOZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询奇偶走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询质合走势
        /// </summary>
        public Task<SDKLPK3_ZHZS_InfoCollection> QueryCache_SDKLPK3_ZHZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDKLPK3().QuerySDKLPK3_ZHZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询质合走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询除3余走势
        /// </summary>
        public Task<SDKLPK3_C3YZS_InfoCollection> QueryCache_SDKLPK3_C3YZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDKLPK3().QuerySDKLPK3_C3YZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询除3余走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询和值走势
        /// </summary>
        public Task<SDKLPK3_HZZS_InfoCollection> QueryCache_SDKLPK3_HZZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDKLPK3().QuerySDKLPK3_HZZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询和值走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询类型走势
        /// </summary>
        public Task<SDKLPK3_LXZS_InfoCollection> QueryCache_SDKLPK3_LXZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDKLPK3().QuerySDKLPK3_LXZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询类型走势 - " + ex.Message, ex);
            }
        }
        #endregion

        #region 七乐彩走势图
        /// <summary>
        /// 查询基本走势
        /// </summary>
        public Task<QLC_JBZS_InfoCollection> QueryQLC_JBZS(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_QLC().QueryQLC_JBZS(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询七乐彩基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询大小走势
        /// </summary>
        public Task<QLC_DX_InfoCollection> QueryQLC_DX(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_QLC().QueryQLC_DX(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询七乐彩大小走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询奇偶走势
        /// </summary>
        public Task<QLC_JO_InfoCollection> QueryQLC_JO(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_QLC().QueryQLC_JO(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询七乐彩奇偶走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询质和走势
        /// </summary>
        public Task<QLC_ZH_InfoCollection> QueryQLC_ZH(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_QLC().QueryQLC_ZH(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询七乐彩质和走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询除3走势
        /// </summary>
        public Task<QLC_Chu3_InfoCollection> QueryQLC_Chu3(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_QLC().QueryQLC_Chu3(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询七乐彩除3走势 - " + ex.Message, ex);
            }
        }
        #endregion

        #region 江苏快3
        /// <summary>
        /// 查询基本走势
        /// </summary>
        public Task<JLK3_JBZS_InfoCollection> QueryJLK3_JBZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JLK3().QueryJLK3_JBZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江苏快3基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询和值走势
        /// </summary>
        public Task<JLK3_HZ_InfoCollection> QueryJLK3_HZ_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JLK3().QueryJLK3_HZ_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江苏快3和值走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询形态走势
        /// </summary>
        public Task<JLK3_XT_InfoCollection> QueryJLK3_XT_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JLK3().QueryJLK3_XT_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江苏快3形态走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询组合走势
        /// </summary>
        public Task<JLK3_ZH_InfoCollection> QueryJLK3_ZH_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JLK3().QueryJLK3_ZH_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江苏快3组合走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询综合走势
        /// </summary>
        public Task<JLK3_ZHZS_InfoCollection> QueryJLK3_ZHZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JLK3().QueryJLK3_ZHZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江苏快3综合走势 - " + ex.Message, ex);
            }
        }
        #endregion

        #region 湖北快3
        /// <summary>
        /// 查询基本走势
        /// </summary>
        public Task<HBK3_JBZS_InfoCollection> QueryHBK3_JBZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_HBK3().QueryHBK3_JBZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询湖北快3基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询和值走势
        /// </summary>
        public Task<HBK3_HZ_InfoCollection> QueryHBK3_HZ_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_HBK3().QueryHBK3_HZ_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询湖北快3和值走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询形态走势
        /// </summary>
        public Task<HBK3_XT_InfoCollection> QueryHBK3_XT_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_HBK3().QueryHBK3_XT_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询湖北快3形态走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询组合走势
        /// </summary>
        public Task<HBK3_ZH_InfoCollection> QueryHBK3_ZH_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_HBK3().QueryHBK3_ZH_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询湖北快3组合走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询综合走势
        /// </summary>
        public Task<HBK3_ZHZS_InfoCollection> QueryHBK3_ZHZS_Info(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_HBK3().QueryHBK3_ZHZS_Info(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询湖北快3综合走势 - " + ex.Message, ex);
            }
        }
        #endregion


        #region 山东群英会走势图

        /// <summary>
        /// 山东群英会奇偶走势
        /// </summary>
        public Task<SDQYH_RXJO_InfoCollection> QuerySDQYH_RXJO_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDQYH().QuerySDQYH_RXJO_Info(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询山东群英会奇偶走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 山东群英会质和走势
        /// </summary>
        public Task<SDQYH_RXZH_InfoCollection> QuerySDQYH_RXZH_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDQYH().QuerySDQYH_RXZH_Info(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询山东群英会质和走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 山东群英会大小走势
        /// </summary>
        public Task<SDQYH_RXDX_InfoCollection> QuerySDQYH_RXDX_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDQYH().QuerySDQYH_RXDX_Info(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询山东群英会大小走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 山东群英会除3走势
        /// </summary>
        public Task<SDQYH_Chu3_InfoCollection> QuerySDQYH_Chu3_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDQYH().QuerySDQYH_Chu3_Info(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询山东群英会除3走势 - " + ex.Message, ex);
            }
        }


        /// <summary>
        /// 山东群英会顺选1走势
        /// </summary>
        public Task<SDQYH_SX1_InfoCollection> QuerySDQYH_SX1_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDQYH().QuerySDQYH_SX1_Info(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询山东群英会顺选1走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 山东群英会顺选2走势
        /// </summary>
        public Task<SDQYH_SX2_InfoCollection> QuerySDQYH_SX2_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDQYH().QuerySDQYH_SX2_Info(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询山东群英会顺选2走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 山东群英会顺选3走势
        /// </summary>
        public Task<SDQYH_SX3_InfoCollection> QuerySDQYH_SX3_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_SDQYH().QuerySDQYH_SX3_Info(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询山东群英会顺选3走势 - " + ex.Message, ex);
            }
        }
        #endregion

        #region 好彩1走势图

        public Task<HC1_JBZS_InfoCollection> QueryCache_HC1_JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_HC1().QueryHC1_JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询基本走势 - " + ex.Message, ex);
            }
        }
        public Task<HC1_SXJJFWZS_InfoCollection> QueryCache_HC1_SXJJFWZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_HC1().QueryHC1_SXJJFWZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询生肖季节方位 - " + ex.Message, ex);
            }
        }
        #endregion

        #region 华东15选5走势图
        /// <summary>
        /// 查询和值走势
        /// </summary>
        public Task<HD15X5_HZ_InfoCollection> QueryHD15X5_HZ(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_HD15X5().QueryHD15X5_HZ(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询华东15选5和值走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询基本走势
        /// </summary>
        public Task<HD15X5_JBZS_InfoCollection> QueryHD15X5_JBZS(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_HD15X5().QueryHD15X5_JBZS(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询华东15选5基本走势 - " + ex.Message, ex);
            }
        }

        public Task<HD15X5_CH_InfoCollection> QueryCache_HD15X5_CH_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_HD15X5().QueryHD15X5_CH(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询华东15选5重号走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询大小走势
        /// </summary>
        public Task<HD15X5_DX_InfoCollection> QueryHD15X5_DX(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_HD15X5().QueryHD15X5_DX(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询华东15选5大小走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询奇偶走势
        /// </summary>
        public Task<HD15X5_JO_InfoCollection> QueryHD15X5_JO(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_HD15X5().QueryHD15X5_JO(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询华东15选5奇偶走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询质和走势
        /// </summary>
        public Task<HD15X5_ZH_InfoCollection> QueryHD15X5_ZH(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_HD15X5().QueryHD15X5_ZH(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询华东15选5质和走势 - " + ex.Message, ex);
            }
        }
        public Task<HD15X5_LH_InfoCollection> QueryCache_HD15X5_LH_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_HD15X5().QueryHD15X5_LH(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询华东15选5连号走势 - " + ex.Message, ex);
            }
        }
        #endregion

        #region 东方6+1走势图
        public Task<DF6_1_DXZS_InfoCollection> QueryCache_DF6_1_DXZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DF6_1().QueryDF6_1_DXZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大小走势 - " + ex.Message, ex);
            }
        }
        public Task<DF6_1_HZZS_InfoCollection> QueryCache_DF6_1_HZZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DF6_1().QueryDF6_1_HZZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询和值走势 - " + ex.Message, ex);
            }
        }
        public Task<DF6_1_JBZS_InfoCollection> QueryCache_DF6_1_JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DF6_1().QueryDF6_1_JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询基本走势 - " + ex.Message, ex);
            }
        }
        public Task<DF6_1_JOZS_InfoCollection> QueryCache_DF6_1_JOZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DF6_1().QueryDF6_1_JOZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询奇偶走势 - " + ex.Message, ex);
            }
        }
        public Task<DF6_1_KDZS_InfoCollection> QueryCache_DF6_1_KDZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DF6_1().QueryDF6_1_KDZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询跨度走势 - " + ex.Message, ex);
            }
        }
        public Task<DF6_1_ZHZS_InfoCollection> QueryCache_DF6_1_ZHZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_DF6_1().QueryDF6_1_ZHZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询质合走势 - " + ex.Message, ex);
            }
        }
        #endregion

        #region 排列五走势
        /// <summary>
        /// 查询基本走势
        /// </summary>
        public Task<PL5_JBZS_InfoCollection> QueryPL5_JBZS(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL5().QueryPL5_JBZS(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列5基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询大小走势
        /// </summary>
        public Task<PL5_DX_InfoCollection> QueryPL5_DX(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL5().QueryPL5_DX(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列5大小走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询奇偶走势
        /// </summary>
        public Task<PL5_JO_InfoCollection> QueryPL5_JO(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL5().QueryPL5_JO(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列5奇偶走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询质和走势
        /// </summary>
        public Task<PL5_ZH_InfoCollection> QueryPL5_ZH(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL5().QueryPL5_ZH(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列5质和走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询除3走势
        /// </summary>
        public Task<PL5_Chu3_InfoCollection> QueryPL5_Chu3(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL5().QueryPL5_Chu3(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列5除3走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询和值走势
        /// </summary>
        public Task<PL5_HZ_InfoCollection> QueryPL5_HZ(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_PL5().QueryPL5_HZ(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询排列5和值走势 - " + ex.Message, ex);
            }
        }
        #endregion

        #region 七星彩走势图
        /// <summary>
        /// 查询基本走势
        /// </summary>
        public Task<QXC_JBZS_InfoCollection> QueryQXC_JBZS(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_QXC().QueryQXC_JBZS(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询七星彩基本走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询大小走势
        /// </summary>
        public Task<QXC_DX_InfoCollection> QueryQXC_DX(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_QXC().QueryQXC_DX(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询七星彩大小走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询奇偶走势
        /// </summary>
        public Task<QXC_JO_InfoCollection> QueryQXC_JO(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_QXC().QueryQXC_JO(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询七星彩奇偶走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询质和走势
        /// </summary>
        public Task<QXC_ZH_InfoCollection> QueryQXC_ZH(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_QXC().QueryQXC_ZH(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询七星彩质和走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询除3走势
        /// </summary>
        public Task<QXC_Chu3_InfoCollection> QueryQXC_Chu3(int length)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_QXC().QueryQXC_Chu3(length));
            }
            catch (Exception ex)
            {
                throw new Exception("查询七星彩除3走势 - " + ex.Message, ex);
            }
        }
        #endregion

        #region  重庆11选5

        public Task<CQ11X5_012DWZS_InfoCollection> QueryCache_CQ11X5_012DWZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_012DWZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询012定位走势 - " + ex.Message, ex);
            }
        }
        public Task<CQ11X5_012LZZS_InfoCollection> QueryCache_CQ11X5_012LZZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_012LZZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询路比值走势 - " + ex.Message, ex);
            }
        }
        public Task<CQ11X5_2LZS_InfoCollection> QueryCache_CQ11X5_2LZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_2LZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询2连走势 - " + ex.Message, ex);
            }
        }
        public Task<CQ11X5_CHZS_InfoCollection> QueryCache_CQ11X5_CHZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_CHZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询重号走势 - " + ex.Message, ex);
            }
        }
        public Task<CQ11X5_DLZS_InfoCollection> QueryCache_CQ11X5_DLZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_DLZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询多连走势 - " + ex.Message, ex);
            }
        }
        public Task<CQ11X5_GHZS_InfoCollection> QueryCache_CQ11X5_GHZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_GHZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询隔号走势 - " + ex.Message, ex);
            }
        }
        public Task<CQ11X5_HZZS_InfoCollection> QueryCache_CQ11X5_HZZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_HZZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询和值走势 - " + ex.Message, ex);
            }
        }
        public Task<CQ11X5_JBZS_InfoCollection> QueryCache_CQ11X5_JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询基本走势 - " + ex.Message, ex);
            }
        }
        public Task<CQ11X5_KDZS_InfoCollection> QueryCache_CQ11X5_KDZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_KDZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询跨度走势 - " + ex.Message, ex);
            }
        }
        public Task<CQ11X5_Q1JBZS_InfoCollection> QueryCache_CQ11X5_Q1JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_Q1JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前1基本走势 - " + ex.Message, ex);
            }
        }
        public Task<CQ11X5_Q1XTZS_InfoCollection> QueryCache_CQ11X5_Q1XTZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_Q1XTZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前1形态走势 - " + ex.Message, ex);
            }
        }
        public Task<CQ11X5_Q2JBZS_InfoCollection> QueryCache_CQ11X5_Q2JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_Q2JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前2基本走势 - " + ex.Message, ex);
            }
        }
        public Task<CQ11X5_Q2XTZS_InfoCollection> QueryCache_CQ11X5_Q2XTZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_Q2XTZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前2形态走势 - " + ex.Message, ex);
            }
        }
        public Task<CQ11X5_Q3JBZS_InfoCollection> QueryCache_CQ11X5_Q3JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_Q3JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前3基本走势 - " + ex.Message, ex);
            }
        }
        public Task<CQ11X5_Q3XTZS_InfoCollection> QueryCache_CQ11X5_Q3XTZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_Q3XTZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前3形态走势 - " + ex.Message, ex);
            }
        }
        public Task<CQ11X5_XTZS_InfoCollection> QueryCache_CQ11X5_XTZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_CQ11X5().QueryCQ11X5_XTZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询形态走势 - " + ex.Message, ex);
            }
        }
        #endregion

        #region 辽宁11选5
        public Task<LN11X5_2LZS_InfoCollection> QueryCache_LN11X5_2LZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_LN11X5().QueryLN11X5_2LZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询2连走势 - " + ex.Message, ex);
            }
        }
        public Task<LN11X5_CHZS_InfoCollection> QueryCache_LN11X5_CHZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_LN11X5().QueryLN11X5_CHZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询重号走势 - " + ex.Message, ex);
            }
        }
        public Task<LN11X5_DLZS_InfoCollection> QueryCache_LN11X5_DLZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_LN11X5().QueryLN11X5_DLZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询多练走势 - " + ex.Message, ex);
            }
        }
        public Task<LN11X5_DXZS_InfoCollection> QueryCache_LN11X5_DXZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_LN11X5().QueryLN11X5_DXZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大小走势 - " + ex.Message, ex);
            }
        }
        public Task<LN11X5_GHZS_InfoCollection> QueryCache_LN11X5_GHZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_LN11X5().QueryLN11X5_GHZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询隔号走势 - " + ex.Message, ex);
            }
        }
        public Task<LN11X5_HZZS_InfoCollection> QueryCache_LN11X5_HZZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_LN11X5().QueryLN11X5_HZZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询和值走势 - " + ex.Message, ex);
            }
        }
        public Task<LN11X5_JBZS_InfoCollection> QueryCache_LN11X5_JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_LN11X5().QueryLN11X5_JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询基本走势 - " + ex.Message, ex);
            }
        }
        public Task<LN11X5_JOZS_InfoCollection> QueryCache_LN11X5_JOZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_LN11X5().QueryLN11X5_JOZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询奇偶走势 - " + ex.Message, ex);
            }
        }
        public Task<LN11X5_Q1ZS_InfoCollection> QueryCache_LN11X5_Q1ZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_LN11X5().QueryLN11X5_Q1ZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前1走势 - " + ex.Message, ex);
            }
        }
        public Task<LN11X5_Q2ZS_InfoCollection> QueryCache_LN11X5_Q2ZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_LN11X5().QueryLN11X5_Q2ZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前2走势 - " + ex.Message, ex);
            }
        }
        public Task<LN11X5_Q3ZS_InfoCollection> QueryCache_LN11X5_Q3ZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_LN11X5().QueryLN11X5_Q3ZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询前3走势 - " + ex.Message, ex);
            }
        }
        #endregion

        #region 江西时时彩走势图

        /// <summary>
        /// 江西1星走势
        /// </summary>
        public Task<JXSSC_1X_ZS_InfoCollection> QueryCache_JXSSC_1X_ZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_1X_ZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西1星走势 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 江西2星和值走势       
        /// </summary>
        public Task<JXSSC_2X_HZZS_InfoCollection> QueryCache_JXSSC_2X_HZZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_2X_HZZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西2星和值走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 江西2星组选走势       
        /// </summary>
        public Task<JXSSC_2X_ZuXZS_InfoCollection> QueryCache_JXSSC_2X_ZuXZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_2X_ZuXZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西2星组选走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 江西2星直选走势     
        /// </summary>
        public Task<JXSSC_2X_ZXZS_InfoCollection> QueryCache_JXSSC_2X_ZXZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_2X_ZXZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西2星直选走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 除3  
        /// </summary>
        public Task<JXSSC_3X_C3YS_InfoCollection> QueryCache_JXSSC_3X_C3YS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_3X_C3YS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询除3- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 大小走势       
        /// </summary>
        public Task<JXSSC_3X_DXZS_InfoCollection> QueryCache_JXSSC_3X_DXZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_3X_DXZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大小走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 和值走势 
        /// </summary>
        public Task<JXSSC_3X_HZZS_InfoCollection> QueryCache_JXSSC_3X_HZZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_3X_HZZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询和值走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 奇偶走势    
        /// </summary>
        public Task<JXSSC_3X_JOZS_InfoCollection> QueryCache_JXSSC_3X_JOZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_3X_JOZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询奇偶走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 跨度   
        /// </summary>
        public Task<JXSSC_3X_KD_InfoCollection> QueryCache_JXSSC_3X_KD_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_3X_KD(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询跨度- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 质合走势  
        /// </summary>
        public Task<JXSSC_3X_ZHZS_InfoCollection> QueryCache_JXSSC_3X_ZHZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_3X_ZHZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询质合走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 江西3星组选走势       
        /// </summary>
        public Task<JXSSC_3X_ZuXZS_InfoCollection> QueryCache_JXSSC_3X_ZuXZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_3X_ZuXZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西3星组选走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 江西3星直选走势    
        /// </summary>
        public Task<JXSSC_3X_ZXZS_InfoCollection> QueryCache_JXSSC_3X_ZXZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_3X_ZXZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西3星直选走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 和值走势   
        /// </summary>
        public Task<JXSSC_5X_HZZS_InfoCollection> QueryCache_JXSSC_5X_HZZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_5X_HZZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询和值走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 江西5星基本走势       
        /// </summary>
        public Task<JXSSC_5X_JBZS_InfoCollection> QueryCache_JXSSC_5X_JBZS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_5X_JBZS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询江西5星基本走势- " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 大小单双     
        /// </summary>
        public Task<JXSSC_DXDS_InfoCollection> QueryCache_JXSSC_DXDS_Info(int index)
        {
            try
            {
                return Task.FromResult(new LotteryDataBusiness_JXSSC().QueryJXSSC_DXDS(index));
            }
            catch (Exception ex)
            {
                throw new Exception("查询大小单双- " + ex.Message, ex);
            }
        }
        #endregion

        #region 快速购买数字彩
        public Task<LotteryIssuse_QueryInfo> QueryCurrentIssuseByOfficialStopTime(string gameCode)
        {
            try
            {
                var list = WebRedisHelper.QueryNextIssuseListByOfficialStopTime(gameCode);
                if (list == null || list.Count <= 0)
                    return null;
                return Task.FromResult(list.Where(p => p.OfficialStopTime > DateTime.Now).OrderBy(p => p.OfficialStopTime).FirstOrDefault());
            }
            catch (Exception ex)
            {
                throw new Exception("查询快速购买失败- " + ex.Message, ex);
            }
        }

        #endregion

        #region 合买大厅合买名人
        /// <summary>
        /// 从Redis中查询出合买红人数据
        /// </summary>
        public Task<List<TogetherHotUserInfo>> QueryHotTogetherUserListFromRedis()
        {
            try
            {
                var list = WebRedisHelper.QueryHotTogetherUserListFromRedis();
                return Task.FromResult(list);
            }
            catch (Exception ex)
            {
                throw new Exception("查询合买红人失败- " + ex.Message, ex);
            }
        }

        #endregion
    }


}
