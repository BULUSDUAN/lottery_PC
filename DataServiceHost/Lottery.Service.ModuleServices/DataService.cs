using EntityModel;
using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.Enum;
using Kason.Sg.Core.CPlatform.Ioc;
using KaSon.FrameWork.Helper;
using Lottery.Kg.ORM.Helper;
using System;
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
        #endregion
    }


}
