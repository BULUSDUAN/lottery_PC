using EntityModel;
using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.Enum;
using Kason.Sg.Core.CPlatform.Ioc;
using KaSon.FrameWork.Common.Redis;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.ORM.Helper;
using KaSon.FrameWork.ORM.Helper.WinNumber;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lottery.Service.ModuleServices
{
    [ModuleName("Data")]
    public class DataService
    {
        #region 获取当前奖期信息
        /// <summary>
        /// 获取当前奖期信息
        /// </summary>
        public Issuse_QueryInfo QueryCurrentIssuseInfo(string gameCode)
        {
            try
            {
                return GameCacheBusiness.GetCurrentIssuserInfo(gameCode);
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
        public LotteryIssuse_QueryInfo QueryCurrentIssuseByLocalStopTime(string gameCode)
        {
            var list = WebRedisHelper.QueryNextIssuseListByLocalStopTime(gameCode);
            if (list == null || list.Count <= 0)
                return null;
            return list.Where(p => p.LocalStopTime > DateTime.Now).OrderBy(p => p.OfficialStopTime).FirstOrDefault();
        }
        #endregion

        #region 前台查询广告信息
        /// <summary>
        /// 前台查询广告信息
        /// </summary>
        /// <param name="bannerType"></param>
        /// <param name="returnRecord"></param>
        /// <returns></returns>
        public SiteMessageBannerInfo_Collection QuerySitemessageBanngerList_Web(BannerType bannerType, int returnRecord = 10)
        {
            try
            {
                return new DataQuery().QuerySitemessageBanngerList_Web(bannerType, returnRecord);
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
        public ArticleInfo_QueryCollection QueryArticleList(string key, string gameCode, string category, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            try
            {
                var userId = new UserAuthentication().ValidateUserAuthentication(userToken);
                var model = new DataQuery().QueryArticleList(key, gameCode, category, pageIndex, pageSize);
                return model;
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
        public ArticleInfo_Query QueryArticleById_Web(string articleId)
        {
            try
            {
                var query = new DataQuery();
                var entity = query.GetArticleById(articleId);
                if (entity == null)
                {
                    throw new ArgumentException("指定编号的文章不存在");
                }
                entity.ReadCount++;
                query.UpdateArticle(entity);
                var info = new ArticleInfo_Query();
                ObjectConvert.ConverEntityToInfo<E_SiteMessage_Article_List, ArticleInfo_Query>(entity, ref info);
                return info;
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
        public BulletinInfo_Collection QueryDisplayBulletinCollection(BulletinAgent agent, int pageIndex, int pageSize, string userToken)
        {
            try
            {
                // 验证用户身份及权限
                var userId = new UserAuthentication().ValidateUserAuthentication(userToken);
                return new DataQuery().QueryDisplayBulletins(agent, pageIndex, pageSize);
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
        public CommonActionResult SubmitUserIdea(UserIdeaInfo_Add userIdea)
        {
            var query = new DataQuery();
            query.SubmitUserIdea(userIdea);
            return new CommonActionResult(true, "提交建议成功");
        }
        #endregion

        #region 活动列表
        /// <summary>
        /// 查询活动列表查询
        /// </summary>
        public ActivityListInfoCollection QueryActivInfoList(int pageIndex, int pageSize)
        {
            try
            {
                return new DataQuery().QueryActivInfoList(pageIndex, pageSize);
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
        public Issuse_QueryInfo QueryCurretNewIssuseInfo(string gameCode, string gameType)
        {
            try
            {
                return GameServiceCache.QueryCurretNewIssuseInfo(gameCode, gameType);
            }
            catch (Exception ex)
            {
                throw new Exception("查询当前奖期出错", ex);
            }
        }

        /// <summary>
        /// 查询北京单场最新期号
        /// </summary>
        /// <returns></returns>
        public BJDCIssuseInfo QueryBJDCCurrentIssuseInfo()
        {
            try
            {
                return new DataQuery().QueryBJDCCurrentIssuseInfo();
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
        public BulletinInfo_Query QueryDisplayBulletinDetailById(long bulletinId)
        {
            var info = new DataQuery().QueryBulletinDetailById(bulletinId);
            if (info != null)
            {
                if (info.Status != EnableStatus.Enable)
                {
                    throw new Exception("指定公告已经禁用");
                }
                if (info.EffectiveFrom != null && info.EffectiveFrom > DateTime.Now)
                {
                    throw new Exception("指定公告尚未发布");
                }
                if (info.EffectiveTo != null && info.EffectiveTo.Value.AddDays(1) < DateTime.Now)
                {
                    throw new Exception(string.Format("指定公告已经于{0:yyyy-MM-dd}过期", info.EffectiveTo));
                }
            }
            return info;
        }
        #endregion

        #region 查找配置表中的配置信息
        /// <summary>
        /// 查找C_Core_Config表中的配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public C_Core_Config QueryCoreConfigByKey(string key)
        {
            try
            {
                return new CacheDataBusiness().QueryCoreConfigByKey(key);
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
        public APPConfigInfo QueryAppConfigByAgentId(string appAgentId)
        {
            try
            {
                return new CacheDataBusiness().QueryAppConfigByAgentId(appAgentId);
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
        public NestedUrlConfig_Collection QueryNestedUrlConfigListByUrlType(int urlType)
        {
            try
            {
                return new CacheDataBusiness().QueryNestedUrlConfigListByUrlType(urlType);
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
        public SiteMessageInnerMailListNew_Collection QueryMyInnerMailList(int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            try
            {
                var userId = new UserAuthentication().ValidateUserAuthentication(userToken);
                return new DataQuery().QueryInnerMailListByReceiver(userId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询已读和未读站内信
        /// </summary>
        public SiteMessageInnerMailListNew_Collection QueryUnReadInnerMailListByReceiver(string userId, int pageIndex, int pageSize, int handleType)
        {
            try
            {
                return new DataQuery().QueryUnReadInnerMailList_ByReceiverId(userId, pageIndex, pageSize, handleType);
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
        public InnerMailInfo_Query ReadInnerMail(string innerMailId, string userToken)
        {
            // 验证用户身份及权限
            try
            {
                var userId = new UserAuthentication().ValidateUserAuthentication(userToken);
                var dataQuery = new DataQuery();
                if (!dataQuery.IsMyInnerMail(innerMailId, userId))
                {
                    throw new SiteMessageException(string.Format("此站内信不属于指定用户。站内信：{0}；用户：{1}。", innerMailId, userId));
                }
                var info = dataQuery.QueryInnerMailDetailByIdAndRead(innerMailId, userId);
                return info;
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
        public string QueryRedBagUseConfig()
        {
            try
            {
                return new DataQuery().QueryRedBagUseConfig();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 查询文章列表_优化
        private static Dictionary<string, ArticleInfo_QueryCollection> _articleCollection = new Dictionary<string, ArticleInfo_QueryCollection>();
        /// <summary>
        /// 查询文章列表
        /// todo:后台权限
        /// </summary>
        public ArticleInfo_QueryCollection QueryArticleList_YouHua(string category, string gameCode, int pageIndex, int pageSize)
        {
            try
            {
                string cacheKey = string.Format("{0}_{1}_{2}_{3}", category, gameCode, pageIndex, pageSize);
                var result = new ArticleInfo_QueryCollection();
                if (_articleCollection != null && _articleCollection.Count > 0 && _articleCollection.ContainsKey(cacheKey))
                {
                    result = _articleCollection.FirstOrDefault(s => s.Key == cacheKey).Value;
                }
                else
                {
                    if (string.IsNullOrEmpty(category))
                        throw new Exception("未查询到文章类别");
                    var array = category.Split('|');
                    var gameCodeArray = gameCode.Split('|');
                    result = new DataQuery().QueryArticleList_YouHua(array, gameCodeArray, pageIndex, pageSize);
                    if (!_articleCollection.ContainsKey(cacheKey))
                        _articleCollection.Add(cacheKey, result);
                }
                return result;
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
        public ShareSpreadCollection QueryShareSpreadUsers(string agentId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
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
                return shareList;
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
        public CQSSC_1X_ZS QueryCQSSCCurrNumberOmission_1XDX(string key, int index)
        {
            try
            {
                return new LotteryDataBusiness_CQSSC().QueryCQSSCCurrNumberOmission_1XDX(key, index);
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
        public CQSSC_2X_ZXZS QueryCQSSCCurrNumberOmission_2XZX(string key, int index)
        {
            try
            {
                return new LotteryDataBusiness_CQSSC().QueryCQSSCCurrNumberOmission_2XZX(key, index);
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
        public CQSSC_3X_ZXZS QueryCQSSCCurrNumberOmission_3XZX(string key, int index)
        {
            try
            {
                return new LotteryDataBusiness_CQSSC().QueryCQSSCCurrNumberOmission_3XZX(key, index);
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
        public CQSSC_2X_ZuXZS QueryCQSSCCurrNumberOmission_2XZuX(string key, int index)
        {
            try
            {
                return new LotteryDataBusiness_CQSSC().QueryCQSSCCurrNumberOmission_2XZuX(key, index);
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
        public CQSSC_3X_ZuXZS QueryCQSSCCurrNumberOmission_ZX3_ZX6(string key, int index)
        {
            try
            {
                return new LotteryDataBusiness_CQSSC().QueryCQSSCCurrNumberOmission_ZX3_ZX6(key, index);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询重庆时时彩当前遗漏_大小单双
        /// </summary>
        public CQSSC_DXDS QueryCQSSCCurrNumberOmission_DXDS(string key, int index)
        {
            try
            {
                return new LotteryDataBusiness_CQSSC().QueryCQSSCCurrNumberOmission_DXDS(key, index);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询重庆时时彩当前遗漏_五星基本走势
        /// </summary>
        public CQSSC_5X_JBZS QueryCQSSCCurrNumberOmission_5XJBZS(string key, int index)
        {
            try
            {
                return new LotteryDataBusiness_CQSSC().QueryCQSSCCurrNumberOmission_5XJBZS(key, index);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

    }


}
