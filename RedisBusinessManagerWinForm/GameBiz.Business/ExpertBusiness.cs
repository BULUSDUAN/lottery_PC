using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using GameBiz.Domain.Managers;
using GameBiz.Core;
using Common.JSON;
using GameBiz.Domain.Entities;
using System.IO;
using Common.Lottery;
using Common.Communication;
using Common.Utilities;

namespace GameBiz.Business
{
    public class ExpertBusiness
    {
        #region 名家相关

        /// <summary>
        /// 添加名家
        /// </summary>
        public void AddExperter(ExperterInfo experter)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new ExperterManager();
                var useerManager = new UserBalanceManager();
                var userEntity = useerManager.QueryUserRegister(experter.UserId);
                if (userEntity == null)
                    throw new LogicException(string.Format("用户Id：{0}没有找到该用户", experter.UserId));
                var entityUser = manager.QueryExperterById(experter.UserId);
                if (entityUser != null)
                    throw new LogicException(string.Format("该用户：{0}已经是名家", experter.UserId));
                var entity = new Experter()
                {
                    UserId = experter.UserId,
                    ExperterHeadImage = experter.ExperterHeadImage,
                    AdeptGameCode = "JCZQ",
                    ExperterSummary = experter.ExperterSummary,
                    RecentlyOrderCount = 0,
                    IsEnable = true,
                    ExperterType = experter.ExperterType,
                    CreateTime = DateTime.Now,
                    WeekShooting = 0M,
                    MonthRate = 0M,
                    TotalRate = 0M,
                    WeekRate = 0M,
                    DealWithType = DealWithType.HasDealWith,
                    DisposeOpinion = "",
                    MonthShooting = 0M,
                    TotalShooting = 0M,
                };
                manager.AddExperter(entity);
                biz.CommitTran();
            }
        }

        /// <summary>
        /// 查询某名家的信息
        /// </summary>
        public ExperterInfo QueryExperterById(string userId)
        {
            return new ExperterManager().QueryExperterInfo(userId);
        }

        /// <summary>
        /// 修改专家信息
        /// </summary>
        public void UpdateExperter(string userId, string image, string summary, string adeptGameCode)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new ExperterManager();
                var useerManager = new UserBalanceManager();
                var userEntity = useerManager.QueryUserRegister(userId);
                if (userEntity == null)
                    throw new LogicException(string.Format("用户Id：{0}没有找到该用户", userId));

                var experterId = manager.QueryExperterById(userId);
                if (experterId == null)
                    throw new LogicException("没有该专家！");

                var noneDealWith = manager.QueryExperterUpdateHitstroy(userId);
                if (noneDealWith != null)
                    throw new LogicException("您上次提交的信息正在审核中，请耐心等待！");

                var entity = new ExperterUpdateHitstroy()
                {
                    UserId = userId,
                    ExperterHeadImage = image,
                    AdeptGameCode = adeptGameCode,
                    ExperterSummary = summary,
                    CreateTime = DateTime.Now,
                    DealWithType = DealWithType.NoneDealWith,
                    DisposeOpinion = "",
                };
                manager.AddExperterUpdateHitstroy(entity);

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 启用，禁用名家
        /// </summary>
        public void UpdateExperterIsEnable(string userId, bool isEnable)
        {
            var manager = new ExperterManager();
            var entity = manager.QueryExperterById(userId);
            if (entity == null)
            {
                throw new Exception(string.Format("修改用户:{0}信息未被查询到！", userId));
            }
            entity.IsEnable = isEnable;
            manager.UpdateExperter(entity);
        }

        /// <summary>
        /// 名家审核
        /// </summary>
        public void UpdateExperterIsShow(string userId, DealWithType dealWithType, string disposeOpinion, string image, string summary, string adeptGameCode)
        {

            var manager = new ExperterManager();
            var entity = manager.QueryExperterById(userId);
            var experter = manager.QueryExperterUpdateHitstroy(userId);
            if (entity == null)
                throw new Exception(string.Format("修改用户:{0}信息不存在！", userId));
            if (experter == null)
                throw new Exception(string.Format("审核用户:{0}信息不存在！", userId));

            if (dealWithType == DealWithType.HasDealWith)
            {
                //1.通过审核修改名家资料,更新审核状态
                entity.DealWithType = dealWithType;
                entity.DisposeOpinion = disposeOpinion;
                entity.ExperterHeadImage = image;
                entity.ExperterSummary = summary;
                entity.AdeptGameCode = adeptGameCode;
                manager.UpdateExperter(entity);

                experter.DealWithType = dealWithType;
                experter.DisposeOpinion = disposeOpinion;
                manager.UpdateExperterUpdateHitstroy(experter);

            }
            if (dealWithType == DealWithType.NoneThrough)
            {
                //2.更新审核状态
                experter.DealWithType = dealWithType;
                experter.DisposeOpinion = disposeOpinion;
                manager.UpdateExperterUpdateHitstroy(experter);
            }
        }

        /// <summary>
        /// 删除专家
        /// </summary>
        public void DeleteExperter(string ExperterId)
        {
            var exper = new ExperterManager();
            var entity = exper.QueryExperterById(ExperterId);
            if (entity == null)
                throw new LogicException(string.Format("查不到{0}专家的信息", ExperterId));
            exper.DeleteExperter(entity);
        }

        /// <summary>
        /// 查询名家列表
        /// </summary>
        public ExperterInfoCollection QueryExperterList(ExperterType? experterType, int pageIndex, int pageSize)
        {
            var result = new ExperterInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new ExperterManager().QueryExperterList(experterType, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 专家命中率排行列表
        /// </summary>
        public ExperterShootingInfoCollection QueryExperterShootingList(ShootingType? shootingType, int pageIndex, int pageSize)
        {
            var result = new ExperterShootingInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new ExperterManager().QueryExperterShootingList(shootingType, pageIndex, pageSize, out  totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 查询名家发单记录
        /// </summary>
        public ExperterPublishedInfoCollection QueryExperterPublishedList(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var result = new ExperterPublishedInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new ExperterManager().QueryExperterPublishedList(userId, startTime, endTime, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 查询修改后待审核列表
        /// </summary>
        public ExperterAuditInfoCollection QueryExperterAuditList(int pageIndex, int pageSize)
        {
            var result = new ExperterAuditInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new ExperterManager().QueryExperterAuditList(pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 查询某名家的修改记录
        /// </summary>
        public ExperterUpdateInfoCollection QueryExperterUpdateList(string userId, int pageIndex, int pageSize)
        {
            var result = new ExperterUpdateInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new ExperterManager().QueryExperterUpdateList(userId, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        #endregion

        #region 名家方案相关

        /// <summary>
        /// 查询名家推荐方案列表
        /// </summary>
        public ExperterSchemeInfoCollection QueryExperterSchemeList(ExperterType experterType, string userId, string currentTime, int pageIndex, int pageSize)
        {
            var result = new ExperterSchemeInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new ExperterSchemeManager().QueryExperterSchemeList(experterType, userId, currentTime, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 查询名家争霸赛排行
        /// </summary>
        public ExperterRankingInfoCollection QueryExperterRankingList(DateTime starTime, DateTime endTime)
        {
            var result = new ExperterRankingInfoCollection();
            result.List.AddRange(new ExperterSchemeManager().QueryExperterRankingList(starTime, endTime));
            return result;
        }


        /// <summary>
        /// 查询名家推荐方案列表
        /// </summary>
        public ExperterHistorySchemeInfoCollection QueryExperterHistorySchemeList(string userId, int pageIndex, int pageSize)
        {
            var result = new ExperterHistorySchemeInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new ExperterSchemeManager().QueryExperterHistorySchemeList(userId, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 更新投票记录
        /// </summary>
        public void UpdateExperterVote(Vote vote, string schemeId, string userId)
        {
            var manager = new ExperterSchemeManager();
            var scheme = manager.QueryExperterSchemeId(schemeId);

            var voteCount = manager.QueryIsVoteScheme(schemeId, userId);
            if (voteCount != null)
                throw new LogicException("已参与本次订单投票，不能重复参与！");

            var currentVoteAgainst = string.Empty;
            var currentVoteSupport = string.Empty;
            if (Vote.Against == vote)
            {
                scheme.Against = scheme.Against + 1;
                currentVoteAgainst = userId;
            }
            else
            {
                scheme.Support = scheme.Support + 1;
                currentVoteSupport = userId;
            }
            manager.UpdateExperterScheme(scheme);

            var entity = new ExperterSchemeSupport()
            {
                AgainstUserId = currentVoteAgainst,
                SupportUserId = currentVoteSupport,
                SchemeId = schemeId,
                CreateTime = DateTime.Now,
            };
            manager.AddExperterSchemeSupport(entity);
        }

        /// <summary>
        /// 该场方案是否投票
        /// </summary>
        public IsVote QueryIsVote(string schemeId, string userId)
        {
            var manager = new ExperterSchemeManager();
            var voteSprt = manager.QueryIsVoteSupport(schemeId, userId);
            if (voteSprt != null)
                return IsVote.Support;

            var voteAgainst = manager.QueryIsVoteAgainst(schemeId, userId);
            if (voteAgainst != null)
                return IsVote.Against;

            if (voteSprt == null && voteAgainst == null)
                return IsVote.NoVote;
            return IsVote.NoVote;
        }


        #endregion

        #region 吐槽相关

        /// <summary>
        /// 添加专家吐槽
        /// </summary>
        public void AddExperterComments(string userId, string sendUserId, CommentsTpye commentsTpye, string analyzeSchemeId, string recommendSchemeId, string content, string currentTime)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new ExperterCommentsManager();
                var entity = new ExperterComments()
                {
                    UserId = userId,
                    CommentsTpye = commentsTpye,
                    AnalyzeSchemeId = analyzeSchemeId,
                    RecommendSchemeId = recommendSchemeId,
                    DealWithType = DealWithType.NoneDealWith,
                    Content = content,
                    SendUserId = sendUserId,
                    DisposeOpinion = "",
                    CurrentTime = currentTime,
                    CreateTime = DateTime.Now,
                };
                manager.AddExperterComments(entity);

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 修改专家吐槽信息
        /// </summary>
        public void UpdateExperterComments(UpdateCommentsInfo experter)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new ExperterCommentsManager();
                var entity = manager.QueryExperterCommentsById(experter.id);
                if (entity == null)
                    throw new Exception("修改信息未被查询到");

                entity.DisposeOpinion = experter.DisposeOpinion;
                entity.DealWithType = experter.DealWithType;

                manager.UpdateExperterComments(entity);
                biz.CommitTran();
            }
        }

        /// <summary>
        /// 删除专家吐槽
        /// </summary>
        public void DeleteExperterCommentsById(int id)
        {
            var exper = new ExperterCommentsManager();
            var entity = exper.QueryExperterCommentsById(id);
            if (entity == null)
                throw new LogicException(string.Format("查不到{0}该吐槽信息", id));
            exper.DeleteExperterComments(entity);
        }

        /// <summary>
        ///查询吐槽内容列表
        /// </summary>
        public ExperterCommentsCollection QueryExperterCommentsByTime(string currentTime, CommentsTpye commentsTpye, string id, int length)
        {
            ExperterCommentsCollection result = new ExperterCommentsCollection();
            result.List.AddRange(new ExperterCommentsManager().QueryExperterCommentsByTime(currentTime, commentsTpye, id, length));
            return result;
        }

        /// <summary>
        ///查询未处理吐槽内容列表
        /// </summary>
        public ExperterCommentsCollection QueryExperterNoneComments(CommentsTpye? commentsTpye, int pageIndex, int pageSize)
        {
            var result = new ExperterCommentsCollection();
            var totalCount = 0;
            result.List.AddRange(new ExperterCommentsManager().QueryExperterNoneComments(commentsTpye, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        #endregion

        #region 名家分析推荐相关

        /// <summary>
        /// 添加分析信息
        /// </summary>
        public void AddExperterAnalyzeScheme(ExperterAnalyzeSchemeInfo experter)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();
                var man1 = new ExperterManager();
                var zhuanjia = man1.QueryExperterById(experter.UserId);
                if (zhuanjia == null)
                    throw new Exception("该用户不是名家，申请名家请联系在线客服！");

                var exManager = new ExperterSchemeManager();
                var schemeCount = exManager.QueryExperterCurrentTimeScheme(experter.UserId, DateTime.Now.ToString("yyyy-MM-dd"));
                if (schemeCount <= 0)
                    throw new LogicException("每天至少发布一个推荐后才可以发布分析！");

                var manager = new ExperterAnalyzeSchemeeManager();
                var entity = new ExperterAnalyzeScheme()
                {
                    Title = experter.Title,
                    Content = experter.Content,
                    Price = experter.Price,
                    SellCount = 0,
                    DealWithType = DealWithType.NoneDealWith,
                    AnalyzeId = BusinessHelper.GetAnalysisId(),
                    UserId = experter.UserId,
                    CreateTime = DateTime.Now,
                    CurrentTime = DateTime.Now.ToString("yyyy-MM-dd"),
                };
                manager.AddExperterAnalyzeScheme(entity);
                biz.CommitTran();
            }
        }

        /// <summary>
        /// 按分析ID查询分析详细
        /// </summary>
        public ExperterAnalyzeSchemeInfo QueryExperterAnalyzeId(string analyzeId)
        {
            var entity = new ExperterAnalyzeSchemeeManager().QueryExperterAnalyzeId(analyzeId);
            if (entity == null)
                throw new LogicException(string.Format("查不到{0}分析信息", analyzeId));
            return new ExperterAnalyzeSchemeInfo()
            {
                AnalyzeId = entity.AnalyzeId,
                Content = entity.Content,
                DisposeOpinion = entity.DisposeOpinion,
                Title = entity.Title,
                UserId = entity.UserId,
                Source = entity.Source,
                Price = entity.Price,
                CurrentTime = entity.CurrentTime,
                DealWithType = entity.DealWithType,
                CreateTime = entity.CreateTime,
            };
        }

        /// <summary>
        /// 查询专家推荐分析列表
        /// </summary>
        public ExperterAnalyzeSchemeInfoCollection QueryExperterAnalyzeSchemeList(string userId, int pageIndex, int pageSize, string currentTime = "")
        {
            var result = new ExperterAnalyzeSchemeInfoCollection();
            var totalCount = 0;

            var manager = new ExperterAnalyzeSchemeeManager();
            var analyzeList = manager.QueryExperterAnalyzeSchemeList(pageIndex, pageSize, out totalCount, currentTime);
            var buyArray = manager.QueryUserBuyedAnalyze(userId, (from a in analyzeList select a.AnalyzeId).ToArray());
            foreach (var item in analyzeList)
            {
                if (buyArray.Contains(item.AnalyzeId))
                {
                    item.IsBuy = true;
                }
            }
            result.List.AddRange(analyzeList);
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 查询某专家推荐分析列表
        /// </summary>
        public ExperterAnalyzeInfoCollection QueryUserAnalyzeList(string userId, string currentDate, int pageIndex, int pageSize)
        {
            var result = new ExperterAnalyzeInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new ExperterAnalyzeSchemeeManager().QueryUserAnalyzeList(userId, currentDate, pageIndex, pageSize, out  totalCount));

            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 后台查询专家推荐分析列表
        /// </summary>
        public ExperterAnalyzeSchemeInfoCollection QueryBackgroundExperterAnalyzeList(int pageIndex, int pageSize, string currentTime = "")
        {
            var result = new ExperterAnalyzeSchemeInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new ExperterAnalyzeSchemeeManager().QueryBackgroundExperterAnalyzeList(pageIndex, pageSize, out totalCount, currentTime));

            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 购买分析信息
        /// </summary>
        public void BuyExperterAnalyzeScheme(ExperterAnalyzeTransactionInfo experter, string password)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new ExperterAnalyzeSchemeeManager();
                var old = manager.QueryExperterAnalyzeTransaction(experter.AnalyzeId, experter.UserId);
                if (old != null)
                    throw new Exception("您已购买该分析内容！");

                //用户扣款
                var user = string.Format("用户:{0}购买专家:{1}分析方案{2}，单价为:{3}", experter.UserId, experter.ExperterId, experter.AnalyzeId, experter.Price);
                BusinessHelper.Payout_To_End(BusinessHelper.FundCategory_BuyExpScheme, experter.UserId, Guid.NewGuid().ToString("N"), experter.Price, user, "BuyExperter", password);
                //专家收款
                var expt = string.Format("专家:{0}收取用户:{1}分析方案{2}，单价为:{3}", experter.ExperterId, experter.UserId, experter.AnalyzeId, experter.Price);
                BusinessHelper.Payin_To_Balance(AccountType.Experts, BusinessHelper.FundCategory_BuyExpScheme, experter.ExperterId, Guid.NewGuid().ToString("N"), experter.Price, expt);

                var entity = new ExperterAnalyzeTransaction()
                {
                    UserId = experter.UserId,
                    AnalyzeId = experter.AnalyzeId,
                    ExperterId = experter.ExperterId,
                    Price = experter.Price,
                    CurrentTime = experter.CurrentTime,
                    CreateTime = DateTime.Now,
                };
                manager.AddExperterAnalyzeTransaction(entity);

                //增加购买次数
                var anId = manager.QueryExperterAnalyzeId(experter.AnalyzeId);
                if (anId == null)
                {
                    throw new Exception("修改信息未被查询到");
                }
                anId.SellCount++;
                manager.UpdateExperterAnalyzeScheme(anId);

                biz.CommitTran();
            }
        }

        /// <summary>
        /// 是否购买该分析方案
        /// </summary>
        public bool QueryIsBuyAnalyzeScheme(string analyzeId, string userId)
        {
            var manager = new ExperterAnalyzeSchemeeManager();
            var old = manager.QueryExperterAnalyzeTransaction(analyzeId, userId);
            if (old != null)
                return true;
            return false;
        }

        /// <summary>
        /// 修改买分析信息
        /// </summary>
        public void UpdateExperterAnalyzeScheme(ExperterAnalyzeUpdateInfo experter)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new ExperterAnalyzeSchemeeManager();
                var entity = manager.QueryExperterAnalyzeId(experter.AnalyzeId);
                if (entity == null)
                {
                    throw new Exception("修改信息未被查询到");
                }
                entity.DisposeOpinion = experter.DisposeOpinion;
                entity.DealWithType = experter.DealWithType;
                manager.UpdateExperterAnalyzeScheme(entity);

                biz.CommitTran();
            }
        }

        #endregion
    }
}
