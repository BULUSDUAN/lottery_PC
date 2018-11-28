using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using NHibernate.Linq;
using GameBiz.Domain.Entities;
using GameBiz.Core;

namespace GameBiz.Domain.Managers
{
    public class ExperterCommentsManager : GameBizEntityManagement
    {
        /// <summary>
        /// 添加专家吐槽
        /// </summary>
        public void AddExperterComments(ExperterComments entity)
        {
            this.Add<ExperterComments>(entity);
        }

        /// <summary>
        /// 删除专家吐槽
        /// </summary>
        public void DeleteExperterComments(ExperterComments entity)
        {
            this.Delete<ExperterComments>(entity);
        }

        /// <summary>
        /// 更新专家吐槽
        /// </summary>
        public void UpdateExperterComments(ExperterComments entity)
        {
            this.Update<ExperterComments>(entity);
        }

        /// <summary>
        /// 按ID专家吐槽信息
        /// </summary>
        public ExperterComments QueryExperterCommentsById(int id)
        {
            this.Session.Clear();
            return this.Session.Query<ExperterComments>().FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// 查询专家吐槽列表
        /// </summary>
        public List<ExperterCommentsInfo> QueryExperterCommentsList(string experterId, int length)
        {
            Session.Clear();
            var query = from r in this.Session.Query<ExperterComments>()
                        join u in this.Session.Query<UserRegister>() on r.SendUserId equals u.UserId
                        where (r.UserId == experterId)
                        orderby r.CreateTime descending
                        select new ExperterCommentsInfo
                        {
                            Id = r.Id,
                            CommentsTpye = r.CommentsTpye,
                            Content = r.Content,
                            CurrentTime = r.CurrentTime,
                            DisposeOpinion = r.DisposeOpinion,
                            AnalyzeSchemeId = r.AnalyzeSchemeId,
                            DealWithType = r.DealWithType,
                            RecommendSchemeId = r.RecommendSchemeId,
                            CreateTime = r.CreateTime,
                            SendUserId = u.UserId,
                            DisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                        };
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 已经查询吐槽内容列表
        /// </summary>
        public List<ExperterCommentsInfo> QueryExperterCommentsByTime(string currentTime, CommentsTpye commentsTpye, string id, int length)
        {
            Session.Clear();
            var query = from r in this.Session.Query<ExperterComments>()
                        join u in this.Session.Query<UserRegister>() on r.SendUserId equals u.UserId
                        //where (r.CurrentTime == currentTime && r.CommentsTpye == commentsTpye && r.DealWithType == DealWithType.HasDealWith)
                        orderby r.CreateTime descending
                        select new ExperterCommentsInfo
                        {
                            Id = r.Id,
                            ExperterId = r.UserId,
                            AnalyzeSchemeId = r.AnalyzeSchemeId,
                            DealWithType = r.DealWithType,
                            RecommendSchemeId = r.RecommendSchemeId,
                            CommentsTpye = r.CommentsTpye,
                            Content = r.Content,
                            CurrentTime = r.CurrentTime,
                            DisposeOpinion = r.DisposeOpinion,
                            CreateTime = r.CreateTime,
                            SendUserId = u.UserId,
                            DisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                        };
            switch (commentsTpye)
            {
                case CommentsTpye.HomeIndx:
                    query = query.Where(p => p.CurrentTime == currentTime && p.CommentsTpye == commentsTpye && p.DealWithType == DealWithType.HasDealWith);
                    break;
                case CommentsTpye.Experter:
                    query = query.Where(p => p.CurrentTime == currentTime && p.CommentsTpye == commentsTpye && p.DealWithType == DealWithType.HasDealWith && p.ExperterId == id);
                    break;
                case CommentsTpye.AllExperter:
                    query = query.Where(p => p.CurrentTime == currentTime && p.CommentsTpye == commentsTpye && p.DealWithType == DealWithType.HasDealWith);
                    break;
                case CommentsTpye.RecommendScheme:
                    query = query.Where(p => p.CurrentTime == currentTime && p.CommentsTpye == commentsTpye && p.DealWithType == DealWithType.HasDealWith && p.RecommendSchemeId == id);
                    break;
                case CommentsTpye.AnalyzeScheme:
                    query = query.Where(p => p.CurrentTime == currentTime && p.CommentsTpye == commentsTpye && p.DealWithType == DealWithType.HasDealWith && p.AnalyzeSchemeId == id);
                    break;
                default:
                    break;
            }

            return query.Take(length).ToList();


        }

        /// <summary>
        /// 未查询吐槽内容列表
        /// </summary>
        public List<ExperterCommentsInfo> QueryExperterNoneComments(CommentsTpye? commentsTpye, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var query = from r in this.Session.Query<ExperterComments>()
                        join u in this.Session.Query<UserRegister>() on r.SendUserId equals u.UserId
                        where (commentsTpye == null || r.CommentsTpye == commentsTpye)
                        && (r.DealWithType == DealWithType.NoneDealWith)
                        orderby r.CreateTime descending
                        select new ExperterCommentsInfo
                        {
                            Id = r.Id,
                            RecommendSchemeId = r.RecommendSchemeId,
                            AnalyzeSchemeId = r.AnalyzeSchemeId,
                            DealWithType = r.DealWithType,
                            CommentsTpye = r.CommentsTpye,
                            Content = r.Content,
                            CurrentTime = r.CurrentTime,
                            DisposeOpinion = r.DisposeOpinion,
                            CreateTime = r.CreateTime,
                            SendUserId = u.UserId,
                            DisplayName = u.DisplayName,
                            HideDisplayNameCount = u.HideDisplayNameCount,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
