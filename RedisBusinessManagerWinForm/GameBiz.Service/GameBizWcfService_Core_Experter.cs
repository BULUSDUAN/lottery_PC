using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using GameBiz.Auth.Business;
using GameBiz.Core;
using GameBiz.Business;
using System.Configuration;
using Common.Expansion;

namespace GameBiz.Service
{
    public class GameBizWcfService_Core_Experter : WcfService
    {
        public GameBizWcfService_Core_Experter()
        {
            KnownTypeRegister.RegisterKnownTypes(CommunicationObjectGetter.GetCommunicationObjectTypes());
        }

        #region 吐槽相关

        /// <summary>
        /// 添加吐槽
        /// </summary>
        public CommonActionResult AddExperterComments(string userId, string sendUserId, CommentsTpye commentsTpye, string analyzeSchemeId, string recommendSchemeId, string content, string currentTime)
        {
            try
            {
                new ExpertBusiness().AddExperterComments(userId, sendUserId, commentsTpye, analyzeSchemeId, recommendSchemeId, content, currentTime);
                return new CommonActionResult(true, "操作成功");

            }
            catch (Exception ex)
            {
                throw new Exception("添加吐槽 - " + ex.Message);
            }
        }

        /// <summary>
        /// 修改专家吐槽信息
        /// </summary>
        public CommonActionResult UpdateExperterComments(UpdateCommentsInfo experter)
        {
            try
            {
                new ExpertBusiness().UpdateExperterComments(experter);
                return new CommonActionResult(true, "操作成功");

            }
            catch (Exception ex)
            {
                throw new Exception("添加吐槽 - " + ex.Message);
            }
        }

        /// <summary>
        /// 删除专家吐槽
        /// </summary>
        public CommonActionResult DeleteExperterCommentsById(int id)
        {
            try
            {
                new ExpertBusiness().DeleteExperterCommentsById(id);
                return new CommonActionResult(true, "操作成功");

            }
            catch (Exception ex)
            {
                throw new Exception("添加吐槽 - " + ex.Message);
            }
        }

        /// <summary>
        /// 查询吐槽内容列表
        /// </summary>
        public ExperterCommentsCollection QueryExperterCommentsByTime(string currentTime, CommentsTpye commentsTpye, string id, int length)
        {
            try
            {
                return new ExpertBusiness().QueryExperterCommentsByTime(currentTime, commentsTpye, id, length);
            }
            catch (Exception ex)
            {
                throw new Exception("查询吐槽内容列表 - " + ex.Message);
            }
        }

        /// <summary>
        /// 查询未处理吐槽内容列表
        /// </summary>
        public ExperterCommentsCollection QueryExperterNoneComments(CommentsTpye? commentsTpye, int pageIndex, int pageSize)
        {
            try
            {
                return new ExpertBusiness().QueryExperterNoneComments(commentsTpye, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询未处理吐槽内容列表 - " + ex.Message);
            }
        }


        #endregion

        #region 名家方案相关

        /// <summary>
        /// 查询名家推荐方案列表
        /// </summary>
        public ExperterSchemeInfoCollection QueryExperterSchemeList(ExperterType experterType, string userId, string currentTime, int pageIndex, int pageSize)
        {
            try
            {
                return new ExpertBusiness().QueryExperterSchemeList(experterType, userId, currentTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询名家推荐方案列表 - " + ex.Message);
            }
        }

        /// <summary>
        /// 查询名家争霸赛排行
        /// </summary>
        public ExperterRankingInfoCollection QueryExperterRankingList(DateTime starTime, DateTime endTime)
        {
            try
            {
                return new ExpertBusiness().QueryExperterRankingList(starTime, endTime);
            }
            catch (Exception ex)
            {
                throw new Exception("查询名家争霸赛排行 - " + ex.Message);
            }
        }

        /// <summary>
        /// 查询名家推荐方案列表
        /// </summary>
        public ExperterHistorySchemeInfoCollection QueryExperterHistorySchemeList(string userId, int pageIndex, int pageSize)
        {
            try
            {
                return new ExpertBusiness().QueryExperterHistorySchemeList(userId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询名家推荐方案列表 - " + ex.Message);
            }
        }

        /// <summary>
        /// 更新投票记录
        /// </summary>
        public void UpdateExperterVote(Vote vote, string schemeId, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new ExpertBusiness().UpdateExperterVote(vote, schemeId, myId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 该场方案是否投票
        /// </summary>
        public IsVote QueryIsVote(string schemeId, string userId)
        {
            try
            {
                return new ExpertBusiness().QueryIsVote(schemeId, userId);
            }
            catch (Exception ex)
            {
                throw new Exception("该场方案是否投票 - " + ex.Message);
            }
        }

        #endregion

        #region 名家分析推荐相关

        /// <summary>
        /// 按分析ID查询分析详细
        /// </summary>
        public ExperterAnalyzeSchemeInfo QueryExperterAnalyzeId(string analyzeId)
        {
            try
            {
                return new ExpertBusiness().QueryExperterAnalyzeId(analyzeId);
            }
            catch (Exception ex)
            {
                throw new Exception("按分析ID查询分析详细 - " + ex.Message);
            }
        }

        /// <summary>
        /// 添加分析信息
        /// </summary>
        public void AddExperterAnalyzeScheme(ExperterAnalyzeSchemeInfo experter)
        {
            try
            {
                new ExpertBusiness().AddExperterAnalyzeScheme(experter);
            }
            catch (Exception ex)
            {
                throw new Exception("添加分析信息 - " + ex.Message);
            }
        }

        /// <summary>
        /// 添加购买分析信息
        /// </summary>
        public CommonActionResult BuyExperterAnalyzeScheme(ExperterAnalyzeTransactionInfo experter, string password)
        {
            try
            {
                new ExpertBusiness().BuyExperterAnalyzeScheme(experter, password);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("添加购买分析信息 - " + ex.Message);
            }
        }

        /// <summary>
        /// 是否购买该分析方案
        /// </summary>
        public bool QueryIsBuyAnalyzeScheme(string analyzeId, string userId)
        {
            try
            {
                return new ExpertBusiness().QueryIsBuyAnalyzeScheme(analyzeId, userId);
            }
            catch (Exception ex)
            {
                throw new Exception("添加购买分析信息 - " + ex.Message);
            }
        }

        /// <summary>
        /// 修改买分析信息
        /// </summary>
        public CommonActionResult UpdateExperterAnalyzeScheme(ExperterAnalyzeUpdateInfo experter)
        {
            try
            {
                new ExpertBusiness().UpdateExperterAnalyzeScheme(experter);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("添加购买分析信息 - " + ex.Message);
            }
        }

        /// <summary>
        /// 查询专家推荐分析列表
        /// </summary>
        public ExperterAnalyzeSchemeInfoCollection QueryExperterAnalyzeSchemeList(string userId, int pageIndex, int pageSize, string currentTime = "")
        {
            try
            {
                return new ExpertBusiness().QueryExperterAnalyzeSchemeList(userId, pageIndex, pageSize, currentTime);
            }
            catch (Exception ex)
            {
                throw new Exception("查询专家推荐分析列表 - " + ex.Message);
            }
        }

        /// <summary>
        /// 查询某专家推荐分析列表
        /// </summary>
        public ExperterAnalyzeInfoCollection QueryUserAnalyzeList(string userId, string currentDate, int pageIndex, int pageSize)
        {
            try
            {
                return new ExpertBusiness().QueryUserAnalyzeList(userId, currentDate, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询某专家推荐分析列表 - " + ex.Message);
            }
        }

        /// <summary>
        /// 后台查询专家推荐分析列表
        /// </summary>
        public ExperterAnalyzeSchemeInfoCollection QueryBackgroundExperterAnalyzeList(int pageIndex, int pageSize, string userToken, string currentTime = "")
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new ExpertBusiness().QueryBackgroundExperterAnalyzeList(pageIndex, pageSize, currentTime);
            }
            catch (Exception ex)
            {
                throw new Exception("后台查询专家推荐分析列表 - " + ex.Message);
            }
        }

        #endregion

        #region 名家相关

        /// <summary>
        /// 添加名家
        /// </summary>
        public CommonActionResult AddExperter(ExperterInfo experter)
        {
            try
            {
                new ExpertBusiness().AddExperter(experter);
                return new CommonActionResult(true, "操作成功");

            }
            catch (Exception ex)
            {
                throw new Exception("添加名家 - " + ex.Message);
            }
        }

        /// <summary>
        /// 修改名家信息
        /// </summary>
        public CommonActionResult UpdateExperter(string userId, string image, string summary, string adeptGameCode)
        {
            try
            {
                new ExpertBusiness().UpdateExperter(userId, image, summary, adeptGameCode);
                return new CommonActionResult(true, "操作成功");

            }
            catch (Exception ex)
            {
                throw new Exception("修改名家信息 - " + ex.Message);
            }
        }

        /// <summary>
        /// 启用，禁用名家
        /// </summary>
        public CommonActionResult UpdateExperterIsEnable(string userId, bool isEnable)
        {
            try
            {
                new ExpertBusiness().UpdateExperterIsEnable(userId, isEnable);
                return new CommonActionResult(true, "操作成功");

            }
            catch (Exception ex)
            {
                throw new Exception("启用，禁用名家 - " + ex.Message);
            }
        }

        /// <summary>
        /// 名家审核
        /// </summary>
        public CommonActionResult UpdateExperterIsShow(string userId, DealWithType dealWithType, string disposeOpinion, string image, string summary, string adeptGameCode)
        {
            try
            {
                new ExpertBusiness().UpdateExperterIsShow(userId, dealWithType, disposeOpinion, image, summary, adeptGameCode);
                return new CommonActionResult(true, "操作成功");

            }
            catch (Exception ex)
            {
                throw new Exception("修改通过审核 - " + ex.Message);
            }
        }

        /// <summary>
        /// 删除专家
        /// </summary>
        public CommonActionResult DeleteExperter(string ExperterId)
        {
            try
            {
                new ExpertBusiness().DeleteExperter(ExperterId);
                return new CommonActionResult(true, "操作成功");

            }
            catch (Exception ex)
            {
                throw new Exception("删除专家成功！ - " + ex.Message);
            }
        }

        /// <summary>
        /// 查询名家列表
        /// </summary>
        public ExperterInfoCollection QueryExperterList(ExperterType? experterType, int pageIndex, int pageSize)
        {
            try
            {
                return new ExpertBusiness().QueryExperterList(experterType, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询名家列表列表 - " + ex.Message);
            }
        }

        /// <summary>
        /// 专家命中率排行列表
        /// </summary>
        public ExperterShootingInfoCollection QueryExperterShootingList(ShootingType? shootingType, int pageIndex, int pageSize)
        {
            try
            {
                return new ExpertBusiness().QueryExperterShootingList(shootingType, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("专家命中率排行列表 - " + ex.Message);
            }
        }

        /// <summary>
        /// 查询名家发单记录
        /// </summary>
        public ExperterPublishedInfoCollection QueryExperterPublishedList(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                return new ExpertBusiness().QueryExperterPublishedList(userId, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询名家发单记录 - " + ex.Message);
            }
        }

        /// <summary>
        /// 查询修改后待审核列表
        /// </summary>
        public ExperterAuditInfoCollection QueryExperterAuditList(int pageIndex, int pageSize)
        {
            try
            {
                return new ExpertBusiness().QueryExperterAuditList(pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询修改后待审核列表 - " + ex.Message);
            }
        }

        /// <summary>
        /// 查询某名家的修改记录
        /// </summary>
        public ExperterUpdateInfoCollection QueryExperterUpdateList(string userId, int pageIndex, int pageSize)
        {
            try
            {
                return new ExpertBusiness().QueryExperterUpdateList(userId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询某名家的修改记录列表 - " + ex.Message);
            }
        }

        /// <summary>
        /// 查询名家 
        /// </summary>
        public ExperterInfo QueryExperterById(string userId)
        {
            try
            {
                return new ExpertBusiness().QueryExperterById(userId);
            }
            catch (Exception ex)
            {
                throw new Exception("查询名家 - " + ex.Message);
            }
        }

        #endregion

    }
}
