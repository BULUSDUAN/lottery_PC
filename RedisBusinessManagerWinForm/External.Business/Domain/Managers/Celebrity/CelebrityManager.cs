using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Common.Communication;
using External.Domain.Entities.Celebritys;
using External.Core;
using External.Core.Celebritys;
using GameBiz.Business;
using GameBiz.Core;
using NHibernate.Linq;
using GameBiz.Domain.Entities;
using GameBiz.Domain.Managers;
using System.Linq.Expressions;
using System.Reflection;
using External.Domain.Entities;
using Common.Utilities;
using Common.Database.ORM;
using System.Data;

namespace External.Business.Domain.Managers.Celebritys
{
    public class CelebrityManager : GameBizEntityManagement
    {
        #region 名家相关

        /// <summary>
        /// 添加名家
        /// </summary>
        public void AddCelebrity(Celebrity entity)
        {
            this.Add<Celebrity>(entity);
        }
        /// <summary>
        /// 修改名家
        /// </summary>
        public void UpdateCelebrity(Celebrity entity)
        {
            this.Update<Celebrity>(entity);
        }
        /// <summary>
        /// 删除名家
        /// </summary>
        public void DeleteCelebrity(Celebrity entity)
        {
            this.Delete<Celebrity>(entity);
        }
        public Celebrity QueryCelebrityById(string userId)
        {
            Session.Clear();
            return this.Session.Query<Celebrity>().FirstOrDefault(p => p.UserId == userId);
        }
        /// <summary>
        /// 根据用户编号 查询单个名家
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CelebritysInfo QueryCelebrityInto(string userId)
        {
            var query = from c in this.Session.Query<Celebrity>()
                        join r in this.Session.Query<UserAttentionSummary>() on c.UserId equals r.UserId
                        where c.UserId == userId
                        select new CelebritysInfo
                        {
                            UserId = c.UserId,
                            CreateTime = c.CreateTime,
                            Attention = r.BeAttentionUserCount,
                            Fans = r.FollowerUserCount,
                            CelebrityType = c.CelebrityType,
                            DealWithType = c.DealWithType,
                            IsEnable = c.IsEnable,
                            Description = c.Description,
                            Picurl = c.Picurl,
                            WinnerUrl = c.WinnerUrl,
                        };
            return query.FirstOrDefault();
        }
        /// <summary>
        /// 名家审核列表
        /// </summary>
        /// <returns></returns>
        public List<CelebritysInfo> QueryCelebrityAuditList(int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from c in this.Session.Query<Celebrity>()
                        join r in this.Session.Query<UserAttentionSummary>() on c.UserId equals r.UserId
                        where c.DealWithType == DealWithType.NoneDealWith
                        select new CelebritysInfo
                        {
                            UserId = c.UserId,
                            CreateTime = c.CreateTime,
                            Attention = r.BeAttentionUserCount,
                            Fans = r.FollowerUserCount,
                            CelebrityType = c.CelebrityType,
                            DealWithType = c.DealWithType,
                            IsEnable = c.IsEnable,
                            Description = c.Description,
                            Picurl = c.Picurl,
                            WinnerUrl = c.WinnerUrl,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        /// <summary>
        /// 查询名家列表
        /// </summary>
        public List<CelebritysInfo> QueryCelebrityList(CelebrityType? celebrityType, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from c in Session.Query<Celebrity>()
                        join r in this.Session.Query<UserAttentionSummary>() on c.UserId equals r.UserId
                        where c.DealWithType == DealWithType.HasDealWith
                        && (!celebrityType.HasValue || c.CelebrityType == celebrityType)
                        select new CelebritysInfo
                        {
                            UserId = c.UserId,

                            CreateTime = c.CreateTime,
                            Attention = r.BeAttentionUserCount,
                            Fans = r.FollowerUserCount,
                            CelebrityType = c.CelebrityType,
                            DealWithType = c.DealWithType,
                            IsEnable = c.IsEnable,
                            Description = c.Description,
                            Picurl = c.Picurl,
                            WinnerUrl = c.WinnerUrl,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        #endregion

        #region 名家返点相关

        /// <summary>
        /// 添加名家返点
        /// </summary>
        public void AddCelebrityRebate(CelebrityRebate entity)
        {
            this.Add<CelebrityRebate>(entity);
        }
        /// <summary>
        /// 修改名家返点
        /// </summary>
        public void UpdateCelebrityRebate(CelebrityRebate entity)
        {
            this.Update<CelebrityRebate>(entity);
        }
        /// <summary>
        /// 删除名剑返点
        /// </summary>
        public void DeleteCelebrityRebate(CelebrityRebate entity)
        {
            this.Delete<CelebrityRebate>(entity);
        }
        public CelebrityRebate QueryCelebrityRebateById(int Id)
        {
            Session.Clear();
            return this.Session.Query<CelebrityRebate>().FirstOrDefault(p => p.Id == Id);
        }
        /// <summary>
        /// 根据用户编号 查询单个名家返点
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CelebrityRebateInfo QueryCelebrityRebateInto(int Id)
        {
            var query = from c in this.Session.Query<CelebrityRebate>()
                        where c.Id == Id
                        select new CelebrityRebateInfo
                        {
                            Id = c.Id,
                            GameCode = c.GameCode,
                            GameType = c.GameType,
                            Rebate = c.Rebate,
                            CreateTime = c.CreateTime,

                        };
            return query.FirstOrDefault();
        }
        /// <summary>
        /// 查询名家返点列表
        /// </summary>
        public List<CelebrityRebateInfo> QueryCelebrityRebateList(int pageIndex, int pageSize, out int totalCount)
        {
            var query = from r in this.Session.Query<CelebrityRebate>()
                        select new CelebrityRebateInfo
                        {
                            Id = r.Id,
                            GameCode = r.GameCode,
                            GameType = r.GameType,
                            Rebate = r.Rebate,
                            CreateTime = r.CreateTime,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        #endregion

        #region 名家吐槽

        /// <summary>
        /// 添加吐槽
        /// </summary>
        public void AddCelebrityTsukkomi(CelebrityTsukkomi entity)
        {
            this.Add<CelebrityTsukkomi>(entity);
        }
        /// <summary>
        /// 删除名家吐槽
        /// </summary>
        public void DeleteCelebrityTsukkomi(CelebrityTsukkomi entity)
        {
            this.Delete<CelebrityTsukkomi>(entity);
        }
        /// <summary>
        /// 修改名家吐槽
        /// </summary>
        public void UpdateCelebrityTsukkomi(CelebrityTsukkomi entity)
        {
            this.Update<CelebrityTsukkomi>(entity);
        }

        public CelebrityTsukkomi QueryTsukkomiById(int id)
        {
            return this.Session.Query<CelebrityTsukkomi>().FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// 查询名家审核吐槽列表
        /// </summary>
        public List<CelebrityTsukkomiInfo> QueryCelebrityTsukkomiList(int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from t in this.Session.Query<CelebrityTsukkomi>()
                        join u in this.Session.Query<UserRegister>() on t.SendUserId equals u.UserId
                        where t.DealWithType == DealWithType.NoneDealWith
                        select new CelebrityTsukkomiInfo
                        {
                            Id = t.Id,
                            Content = t.Content,
                            CreateTime = t.CreateTime,
                            UserId = t.UserId,
                            SendUserId = t.SendUserId,
                            DealWithType = t.DealWithType,
                            DisplayName = u.DisplayName,
                            DisposeOpinion = t.DisposeOpinion,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 查询名家吐槽列表
        /// </summary>
        public List<CelebrityTsukkomiInfo> QueryCelebrityTsukkomiInfosByUserId(string userId, int length,out int totalCount)
        {
            var query = from t in this.Session.Query<CelebrityTsukkomi>()
                        join u in this.Session.Query<UserRegister>() on t.SendUserId equals u.UserId
                        where t.UserId == userId
                        select new CelebrityTsukkomiInfo
                        {
                            Id = t.Id,
                            Content = t.Content,
                            CreateTime = t.CreateTime,
                            UserId = t.UserId,
                            SendUserId = t.SendUserId,
                            DealWithType = t.DealWithType,
                            DisplayName = u.DisplayName,
                            DisposeOpinion = t.DisposeOpinion,
                        };
            totalCount = query.Count();
            return query.Take(length).ToList();
        }


        #endregion

        #region 新赢家平台 20150209 dj

        public void AddWinnerModel(WinnerModel entity)
        {
            this.Add<WinnerModel>(entity);
        }
        public void AddWinnerModelCycle(WinnerModelCycle entity)
        {
            this.Add<WinnerModelCycle>(entity);
        }
        public int AddWinnerModelCycleGetId(WinnerModelCycle entity)
        {
            this.Add<WinnerModelCycle>(entity);
            return entity.ModelCycleId;
        }
        public void UpdateWinnerModelCycle(WinnerModelCycle entity)
        {
            this.Update<WinnerModelCycle>(entity);
        }
        public void EditWinnerModel(WinnerModel entity)
        {
            this.Update<WinnerModel>(entity);
        }
        public WinnerModel QueryWinnerModelByName(string modelName)
        {
            return Session.Query<WinnerModel>().FirstOrDefault(s => s.ModelName == modelName);
        }
        public WinnerModel QueryWinnerModel(string modelId, string userId)
        {
            return Session.Query<WinnerModel>().FirstOrDefault(s => s.ModelId == modelId && s.UserId == userId);
        }
        public WinnerModel QueryWinnerModelByModelId(string modelId)
        {
            return Session.Query<WinnerModel>().FirstOrDefault(s => s.ModelId == modelId && s.IsDelete == false);
        }
        public WinnerModelCycle QueryWinnerCycleByModelId(string modelId)
        {
            return Session.Query<WinnerModelCycle>().FirstOrDefault(s => s.ModelId == modelId);
        }
        public void AddWinnerModelCollection(WinnerModelCollection entity)
        {
            this.Add<WinnerModelCollection>(entity);
        }
        public void DeleteWinnerModelCollection(WinnerModelCollection entity)
        {
            this.Delete<WinnerModelCollection>(entity);
        }
        public WinnerModelCollection QueryWinnerModelShouCang(string modelId, string userId)
        {
            Session.Clear();
            return Session.Query<WinnerModelCollection>().FirstOrDefault(s => s.ModelId == modelId && s.UserId == userId);
        }
        public void AddWinnerModelScheme(WinnerModelScheme entity)
        {
            this.Add<WinnerModelScheme>(entity);
        }
        public int AddWinnerModelSchemeDetail(WinnerModelSchemeDetail entity)
        {
            this.Add<WinnerModelSchemeDetail>(entity);
            return entity.ModelSchemeDetailId;
        }
        public void AddModelBidding(WinnerModelBidding entity)
        {
            this.Add<WinnerModelBidding>(entity);
        }
        public void UpdateModelBidding(WinnerModelBidding entity)
        {
            this.Update<WinnerModelBidding>(entity);
        }
        public WinnerModelScheme QueryWinnerModelSchemeById(int modelSchemeId)
        {
            return this.GetByKey<WinnerModelScheme>(modelSchemeId);
        }
        public WinnerModelScheme QueryWinnerModelSchemeByKeyLine(string keyLine)
        {
            Session.Clear();
            return Session.Query<WinnerModelScheme>().FirstOrDefault(s => s.ModelKeyLine == keyLine);
        }
        public List<WinnerModelScheme> QueryWinnerModelSchemeListByModelId(string modelId)
        {
            Session.Clear();
            return Session.Query<WinnerModelScheme>().Where(s => s.ModelId == modelId).ToList();
        }
        public List<WinnerModelScheme> QueryNotStopWinnerModelSchemeListByModelId(string modelId)
        {
            Session.Clear();
            return Session.Query<WinnerModelScheme>().Where(s => s.ModelId == modelId && s.IsStop == false).ToList();
        }
        public void UpdateWinnerModelScheme(WinnerModelScheme entity)
        {
            this.Update<WinnerModelScheme>(entity);
        }
        public WinnerModelCycle QueryWinnerModelCycleById(int modelCycleId)
        {
            //Session.Clear();
            return this.GetByKey<WinnerModelCycle>(modelCycleId);
        }
        public void UpdateWinnerModelSchemeDetail(params WinnerModelSchemeDetail[] entityList)
        {
            this.Update<WinnerModelSchemeDetail>(entityList);
        }
        public void EditWinnerModelCycle(WinnerModelCycle entity)
        {
            this.Update<WinnerModelCycle>(entity);
        }
        public List<WinnerModelSchemeDetail> QueryNoPayModelSchemeDetailByKeyLine(string keyLine)
        {
            return Session.Query<WinnerModelSchemeDetail>().Where(s => s.ModelKeyLine == keyLine && s.PayStatus == PayStatus.WaitingPay && s.IsComplete == false).ToList();
        }
        public List<WinnerModelSchemeDetail> QueryWinnerModelSchemeDetailByKeyLine(string keyLine)
        {
            Session.Clear();
            return Session.Query<WinnerModelSchemeDetail>().Where(s => s.ModelKeyLine == keyLine).ToList();
        }
        public WinnerModelSchemeDetail QueryWinnerModelSchemeDetailBySchemeId(string schemeId, string keyLine)
        {
            return Session.Query<WinnerModelSchemeDetail>().FirstOrDefault(s => s.SchemeId == schemeId && s.ModelKeyLine == keyLine);
        }
        public WinnerModelSchemeDetail QueryWinnerModelSchemeDetailBySchemeId(string schemeId)
        {
            return Session.Query<WinnerModelSchemeDetail>().FirstOrDefault(s => s.SchemeId == schemeId);
        }
        public List<WinnerModelCycle> QueryWinnerModelCycleBySchemeId(string schemeId)
        {
            return Session.Query<WinnerModelCycle>().Where(s => s.SchemeId == schemeId && s.IsComplete == false).ToList();
        }
        public WinnerModelCycle QueryCurrWinnerModelCycle(string modelId)
        {
            return Session.Query<WinnerModelCycle>().OrderByDescending(s=>s.CreateTime).FirstOrDefault(s =>s.ModelId == modelId && s.IsComplete == false);
        }

        public List<WinnerModelCycle> QueryWinnerModelCycleByModelId(string modelId)
        {
            return Session.Query<WinnerModelCycle>().Where(s=>s.ModelId==modelId).ToList();
        }
        public WinnerModelBidding QueryWinnerModelBiddingByModelId(string userId, string modelId)
        {
            return Session.Query<WinnerModelBidding>().FirstOrDefault(s => s.UserId == userId && s.ModelId == modelId);
        }
        public List<OCDouDouDetail> QueryFundDouDouList(string modelId)
        {
            return Session.Query<OCDouDouDetail>().Where(s => s.OrderId == modelId && s.Category == "豆豆模型竞价排行" && s.CreateTime.Date == DateTime.Now.Date).ToList();
        }
        public OCDouDouDetailInfoCollection QueryMyFundDouDouList(string modelId, string userId, int pageIndex, int pageSize)
        {
            OCDouDouDetailInfoCollection collection = new OCDouDouDetailInfoCollection();
            collection.TotalCount = 0;
            var query = from f in Session.Query<OCDouDouDetail>()
                        where f.OrderId == modelId && f.UserId == userId && f.Category == "豆豆模型竞价排行"
                        select new OCDouDouDetailInfo
                            {
                                OrderId = f.OrderId,
                                AfterBalance = f.AfterBalance,
                                CreateTime = f.CreateTime,
                            };
            if (query != null && query.Count() > 0)
            {
                collection.TotalCount = query.Count();
                collection.List = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;
        }
        public WinnerModelBidding_Collection QueryMyModelBiddingListByModelId(string userId, int pageIndex, int pageSize)
        {
            Session.Clear();
            string sql = "select m.ModelId,b.BidDouDou,b.ClickNumber,b.CurrBidDouDou,m.ModelName,b.ModifyTime,b.CreateTime,m.UserId,m.UserDisplayName from C_Winner_Model m left join C_Winner_ModelBidding b on b.ModelId=m.ModelId where m.UserId=:userId";
            var result = Session.CreateSQLQuery(sql)
                              .SetString("userId", userId).List();
            WinnerModelBidding_Collection collection = new WinnerModelBidding_Collection();
            if (result != null)
            {
                foreach (var item in result)
                {
                    var array = item as object[];
                    WinnerModelBiddingInfo info = new WinnerModelBiddingInfo();
                    info.ModelId = array[0].ToString();
                    info.BidDouDou = array[1] == null ? 0M : Convert.ToDecimal(array[1]);
                    info.ClickNumber = array[2] == null ? 0 : Convert.ToInt32(array[2]);
                    info.CurrBidDouDou = array[3] == null ? 0M : Convert.ToDecimal(array[3]);
                    info.ModelName = array[4] == null ? string.Empty : array[4].ToString();
                    info.ModifyTime = array[5] == null ? Convert.ToDateTime(null) : Convert.ToDateTime(array[5]);
                    info.CreateTime = array[6] == null ? Convert.ToDateTime(null) : Convert.ToDateTime(array[6]);
                    info.UserId = array[7] == null ? string.Empty : array[7].ToString();
                    info.UserDisplayName = array[8] == null ? string.Empty : array[8].ToString();
                    collection.ModelBidList.Add(info);
                }
            }
            collection.TotalCount = result.Count;
            if (collection != null && collection.ModelBidList.Count > 0)
            {
                if (pageSize == -1)
                    return collection;
                else
                    collection.ModelBidList = collection.ModelBidList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;
        }
        public WinnerModelBidding_Collection QueryBiddingRank(int pageIndex, int pageSize)
        {
            Session.Clear();
            WinnerModelBidding_Collection collection = new WinnerModelBidding_Collection();
            var query = from b in Session.Query<WinnerModelBidding>()
                        join m in Session.Query<WinnerModel>() on b.ModelId equals m.ModelId
                        orderby b.CurrBidDouDou descending, b.ModifyTime ascending
                        select new WinnerModelBiddingInfo
                        {
                            ModelId = m.ModelId,
                            UserDisplayName = m.UserDisplayName,
                            ClickNumber = b.ClickNumber,
                            CurrBidDouDou = b.CurrBidDouDou,
                            ModelName = m.ModelName,
                        };
            collection.TotalCount = 0;
            if (query.Count() > 0)
            {
                collection.TotalCount = query.Count();
                collection.ModelBidList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;
        }
        public WinnerModelBidding_Collection QueryBiddingRankByModelId(string modelId, string userId, int pageIndex, int pageSize)
        {
            Session.Clear();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("where 1=1");
            if (!string.IsNullOrEmpty(userId))
                strSql.Append(" and m.UserId='" + userId + "'");
            if (!string.IsNullOrEmpty(modelId))
                strSql.Append(" and m.ModelId='" + modelId + "'");
            string sql = "select m.ModelId,b.BidDouDou,b.ClickNumber,b.CurrBidDouDou,m.ModelName,b.ModifyTime,b.CreateTime,m.UserId,m.UserDisplayName from C_Winner_Model m left join C_Winner_ModelBidding b on b.ModelId=m.ModelId " + strSql + " order by b.CurrBidDouDou desc";
            var result = Session.CreateSQLQuery(sql).List();
            WinnerModelBidding_Collection collection = new WinnerModelBidding_Collection();
            if (result != null)
            {
                foreach (var item in result)
                {
                    var array = item as object[];
                    WinnerModelBiddingInfo info = new WinnerModelBiddingInfo();
                    info.ModelId = array[0].ToString();
                    info.BidDouDou = array[1] == null ? 0M : Convert.ToDecimal(array[1]);
                    info.ClickNumber = array[2] == null ? 0 : Convert.ToInt32(array[2]);
                    info.CurrBidDouDou = array[3] == null ? 0M : Convert.ToDecimal(array[3]);
                    info.ModelName = array[4] == null ? string.Empty : array[4].ToString();
                    info.ModifyTime = array[5] == null ? Convert.ToDateTime(null) : Convert.ToDateTime(array[5]);
                    info.CreateTime = array[6] == null ? Convert.ToDateTime(null) : Convert.ToDateTime(array[6]);
                    info.UserId = array[7] == null ? string.Empty : array[7].ToString();
                    info.UserDisplayName = array[8] == null ? string.Empty : array[8].ToString();
                    collection.ModelBidList.Add(info);
                }
            }
            collection.TotalCount = result.Count;
            if (collection != null && collection.ModelBidList.Count > 0)
                collection.ModelBidList = collection.ModelBidList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return collection;
        }
        public WinnerModelInfo_Collection QueryWinnerModelCollection(string modelName, bool isBuy, bool isExperter, bool isFirstPayment, string orderBy, int pageIndex, int pageSize)
        {
            #region

            Session.Clear();
            if (string.IsNullOrEmpty(orderBy))
                orderBy = "TotalReportRatio";
            WinnerModelInfo_Collection collection = new WinnerModelInfo_Collection();
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Winner_QueryModelList"))
                .AddInParameter("ModelName", modelName)
                .AddInParameter("IsBuy", isBuy)
                .AddInParameter("IsExperter", isExperter)
                .AddInParameter("IsFirstPayment", isFirstPayment)
                .AddInParameter("StrOrderBy", orderBy)
                .AddInParameter("PageIndex", pageIndex)
                .AddInParameter("PageSize", pageSize)
                .AddOutParameter("TotalCount", "Int32");

            var dt = query.GetDataTable(out outputs);
            collection.TotalCount = UsefullHelper.GetDbValue<int>(outputs["TotalCount"]);
            collection.ModelListInfo.AddRange(ORMHelper.DataTableToInfoList<WinnerModelInfo>(dt));
            var modelCycleList = Session.Query<WinnerModelCycle>().Where(s=>s.IsComplete==true).ToList();
            if (modelCycleList != null && modelCycleList.Count > 0)
            {
                foreach (var item in collection.ModelListInfo)
                {
                    var result = modelCycleList.Where(s => s.ModelId == item.ModelId).ToList();
                    if (result != null && result.Count > 0)
                    {
                        var list = new List<WinnerModelCycleInfo>();
                        foreach (var l in result)
                        {
                            WinnerModelCycleInfo info = new WinnerModelCycleInfo();
                            ObjectConvert.ConverEntityToInfo(l, ref info);
                            info.CompleteTime = l.CompleteTime.HasValue ? l.CompleteTime.Value : Convert.ToDateTime(null);
                            info.CreateTime = l.CreateTime.HasValue ? l.CreateTime.Value : Convert.ToDateTime(null);
                            info.StopBettingTime = l.StopBettingTime.HasValue ? l.StopBettingTime.Value : Convert.ToDateTime(null);
                            list.Add(info);
                        }
                        item.ModelCycleInfoList = list;
                    }
                }
            }

            return collection;

            #endregion
        }
        public object GetPropertyValue(object obj, string property)
        {
            PropertyInfo p = obj.GetType().GetProperty(property);
            return p.GetValue(obj, null);
        }
        public List<WinnerModelCycleInfo> GetModelCycleList(string modelId)
        {
            var manager = new CelebrityManager();
            var list = manager.QueryWinnerModelCycleList(modelId, 0, -1);
            return list.ModelCycleList;
        }

        public WinnerModelInfo_Collection QueryWinnerModelListByUserId(string userId, int pageIndex, int pageSize)
        {
            #region

            Session.Clear();
            var query = from w in Session.Query<WinnerModel>()
                        where w.UserId == userId &&w.IsDelete==false
                        select w;
            WinnerModelInfo_Collection collection = new WinnerModelInfo_Collection();
            foreach (var w in query)
            {
                WinnerModelInfo info = new WinnerModelInfo();
                info.ModelId = w.ModelId;
                info.ModelType = (ModelType)w.ModelType;
                info.ModelName = w.ModelName;
                info.ModelDescription = w.ModelDescription;
                info.SchemeId = w.SchemeId;
                info.GameCode = w.GameCode;
                info.GameType = w.GameType;
                info.PlayType = w.PlayType;
                info.UserId = w.UserId;
                info.TotalBettingIssuseCount = w.TotalBettingIssuseCount;
                info.TotalBonusIssuseCount = w.TotalBonusIssuseCount;
                info.SchemeMoney = w.SchemeMoney;
                info.TotalBuyCount = w.TotalBuyCount;
                info.TotalBonusMoney = w.TotalBonusMoney;
                info.TotalProfitMoney = w.TotalProfitMoney;
                info.UserDisplayName = w.UserDisplayName;
                info.TotalSaleMoney = w.TotalSaleMoney;
                info.ActulProfitRatio = w.ActulProfitRatio;
                info.TotalReportRatio = w.TotalReportRatio;
                info.BonusFrequency = w.BonusFrequency;
                info.IsDelete = w.IsDelete;
                info.DeleteTime = w.DeleteTime.HasValue ? w.DeleteTime.Value : Convert.ToDateTime(null);
                info.CreateTime = w.CreateTime.HasValue ? w.CreateTime.Value : Convert.ToDateTime(null);
                //info.IsExperter = w.IsExperter;
                info.IsShare = w.IsShare;
                info.ModelSecurity = (ModelSecurity)w.ModelSecurity;
                info.IsFirstPayment = w.IsFirstPayment;
                info.ProfitIssuseCount = w.ProfitIssuseCount;
                info.RiskType = (RiskType)w.RiskType;
                info.LossRatio = w.LossRatio;
                info.CommissionRitio = w.CommissionRitio;
                //info.IsProfit = w.IsProfit;
                info.TotalLoseMoney = w.TotalLoseMoney;
                info.TotalHaveMoney = w.TotalHaveMoney;
                info.TotalShareBonusMoney = w.TotalShareBonusMoney;
                info.TotalCommissionMoney = w.TotalCommissionMoney;
                info.CurrSchemeStopTime = w.CurrSchemeStopTime;
                //info.ModelCycleInfoList = GetModelCycleList(w.ModelId);
                var cycleList = new List<WinnerModelCycleInfo>();
                foreach (var item in w.ModelCycleList.Where(s=>s.IsComplete==true))
                {
                    WinnerModelCycleInfo cycleInfo=new WinnerModelCycleInfo ();
                    ObjectConvert.ConverEntityToInfo(item,ref cycleInfo);
                    cycleInfo.CompleteTime = item.CompleteTime.HasValue ? item.CompleteTime.Value : Convert.ToDateTime(null);
                    cycleInfo.CreateTime = item.CreateTime.HasValue ? item.CreateTime.Value : Convert.ToDateTime(null);
                    cycleInfo.StopBettingTime = item.StopBettingTime.HasValue ? item.StopBettingTime.Value : Convert.ToDateTime(null);
                    cycleList.Add(cycleInfo);
                }
                info.ModelCycleInfoList = cycleList;
                info.TotalModelCollection = w.TotalModelCollection;
                collection.ModelListInfo.Add(info);
            }
            if (collection != null && collection.ModelListInfo.Count > 0)
            {
                collection.TotalCount = collection.ModelListInfo.Count;
                collection.ModelListInfo = collection.ModelListInfo.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;

            #endregion

        }
        public WinnerModelCycleInfo_Collection QueryWinnerModelCycleCollection(string modelId, int pageIndex, int pageSize)
        {
            #region

            Session.Clear();
            WinnerModelCycleInfo_Collection collection = new WinnerModelCycleInfo_Collection();
            collection.TotalCount = 0;
            //查询模型
            var queryModel = from w in Session.Query<WinnerModel>()
                             where w.ModelId == modelId &&w.IsDelete==false
                             select new WinnerModelInfo
                             {
                                 ModelId = w.ModelId,
                                 ModelType = (ModelType)w.ModelType,
                                 ModelName = w.ModelName,
                                 ModelDescription = w.ModelDescription,
                                 SchemeId = w.SchemeId,
                                 GameCode = w.GameCode,
                                 GameType = w.GameType,
                                 PlayType = w.PlayType,
                                 UserId = w.UserId,
                                 TotalBettingIssuseCount = w.TotalBettingIssuseCount,
                                 TotalBonusIssuseCount = w.TotalBonusIssuseCount,
                                 SchemeMoney = w.SchemeMoney,
                                 TotalBuyCount = w.TotalBuyCount,
                                 TotalBonusMoney = w.TotalBonusMoney,
                                 TotalProfitMoney = w.TotalProfitMoney,
                                 UserDisplayName = w.UserDisplayName,
                                 TotalSaleMoney = w.TotalSaleMoney,
                                 ActulProfitRatio = w.ActulProfitRatio,
                                 TotalReportRatio = w.TotalReportRatio,
                                 BonusFrequency = w.BonusFrequency,
                                 IsDelete = w.IsDelete,
                                 DeleteTime = w.DeleteTime.HasValue ? w.DeleteTime.Value : Convert.ToDateTime(null),
                                 CreateTime = w.CreateTime.Value,
                                 //IsExperter = w.IsExperter,
                                 IsShare = w.IsShare,
                                 ModelSecurity = (ModelSecurity)w.ModelSecurity,
                                 IsFirstPayment = w.IsFirstPayment,
                                 ProfitIssuseCount = w.ProfitIssuseCount,
                                 RiskType = (RiskType)w.RiskType,
                                 LossRatio = w.LossRatio,
                                 CommissionRitio = w.CommissionRitio,
                                 //IsProfit = w.IsProfit,
                                 TotalLoseMoney = w.TotalLoseMoney,
                                 TotalHaveMoney = w.TotalHaveMoney,
                                 TotalShareBonusMoney = w.TotalShareBonusMoney,
                                 TotalCommissionMoney = w.TotalCommissionMoney,
                                 IsEnableFirstPayment = w.IsEnableFirstPayment,
                                 CurrSchemeStopTime=w.CurrSchemeStopTime,
                                 TotalModelCollection=w.TotalModelCollection,
                             };

            if (queryModel != null && queryModel.Count() > 0)
            {
                collection.ModelList.AddRange(queryModel.ToList());
            }
            collection.ModelCycleList = QueryAllWinnerModelCycleList(modelId, pageIndex, pageSize).ModelCycleList;

            return collection;

            #endregion

        }
        public WinnerModelCycleInfo_Collection QueryWinnerModelCycleList(string modelId, int pageIndex, int pageSize)
        {
            #region

            Session.Clear();
            WinnerModelCycleInfo_Collection collection = new WinnerModelCycleInfo_Collection();
            collection.TotalCount = 0;

            //查询模型每期方案
            var queryModelCycle = from c in Session.Query<WinnerModelCycle>()
                                  where c.ModelId == modelId && c.IsComplete == true
                                  select new WinnerModelCycleInfo
                                  {
                                      ModelCycleId = c.ModelCycleId,
                                      CurrModelIssuse = c.CurrModelIssuse,
                                      CurrBettingMoney = c.CurrBettingMoney,
                                      CurrBonusMoney = c.CurrBonusMoney,
                                      ModelProgressStatus = (ModelProgressStatus)c.ModelProgressStatus,
                                      SchemeId = c.SchemeId,
                                      CreateTime = c.CreateTime.HasValue ? c.CreateTime.Value : Convert.ToDateTime(null),
                                      StopBettingTime = c.StopBettingTime.Value,
                                  };
            if (queryModelCycle != null && queryModelCycle.Count() > 0)
            {
                collection.TotalCount = queryModelCycle.Count();
                if (pageSize == -1)
                    collection.ModelCycleList = queryModelCycle.ToList();
                else
                    collection.ModelCycleList = queryModelCycle.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;

            #endregion

        }
        public WinnerModelCycleInfo_Collection QueryAllWinnerModelCycleList(string modelId, int pageIndex, int pageSize)
        {
            #region

            Session.Clear();
            WinnerModelCycleInfo_Collection collection = new WinnerModelCycleInfo_Collection();
            collection.TotalCount = 0;

            //查询模型每期方案
            var queryModelCycle = from c in Session.Query<WinnerModelCycle>()
                                  where c.ModelId == modelId
                                  select new WinnerModelCycleInfo
                                  {
                                      ModelCycleId = c.ModelCycleId,
                                      CurrModelIssuse = c.CurrModelIssuse,
                                      CurrBettingMoney = c.CurrBettingMoney,
                                      CurrBonusMoney = c.CurrBonusMoney,
                                      ModelProgressStatus = (ModelProgressStatus)c.ModelProgressStatus,
                                      SchemeId = c.SchemeId,
                                      CreateTime = c.CreateTime.HasValue ? c.CreateTime.Value : Convert.ToDateTime(null),
                                      StopBettingTime = c.StopBettingTime.Value,
                                      ModelId=c.ModelId,
                                  };
            if (queryModelCycle != null && queryModelCycle.Count() > 0)
            {
                collection.TotalCount = queryModelCycle.Count();
                if (pageSize == -1)
                    collection.ModelCycleList = queryModelCycle.ToList();
                else
                    collection.ModelCycleList = queryModelCycle.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;

            #endregion

        }

        public Sports_SchemeQueryInfoCollection QuerySaveOrderToModel(string userId, int pageIndex, int pageSize)
        {
            #region

            Session.Clear();
            Sports_SchemeQueryInfoCollection collection = new Sports_SchemeQueryInfoCollection();
            collection.TotalCount = 0;
            var query = from o in Session.Query<Sports_Order_Running>()
                        where o.SchemeType == SchemeType.SaveScheme && o.StopTime > DateTime.Now && o.UserId == userId
                        select new Sports_SchemeQueryInfo
                        {
                            SchemeId = o.SchemeId,
                            GameCode = o.GameCode,
                            GameType = o.GameType,
                            TotalMoney = o.TotalMoney,
                            CreateTime = o.CreateTime,
                            SchemeType = o.SchemeType,
                            SchemeBettingCategory = o.SchemeBettingCategory,
                            BonusStatus = o.BonusStatus,
                            ProgressStatus = o.ProgressStatus,
                            TicketStatus = o.TicketStatus,
                        };
            if (query != null && query.Count() > 0)
            {
                collection.TotalCount = query.Count();
                collection.List = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;

            #endregion

        }
        public WinnerModelInfo_Collection QueryWinnerModelShouCangCollection(string userId, int pageIndex, int pageSize)
        {
            #region

            Session.Clear();
            WinnerModelInfo_Collection collection = new WinnerModelInfo_Collection();
            collection.TotalCount = 0;

            #region 暂时屏蔽

            //var query = from c in Session.Query<WinnerModelCollection>()
            //            join w in Session.Query<WinnerModel>() on c.ModelId equals w.ModelId
            //            where c.UserId == userId
            //            select new WinnerModelInfo
            //            {
            //                ModelId = w.ModelId,
            //                ModelType = (ModelType)w.ModelType,
            //                ModelName = w.ModelName,
            //                ModelDescription = w.ModelDescription,
            //                SchemeId = w.SchemeId,
            //                GameCode = w.GameCode,
            //                GameType = w.GameType,
            //                PlayType = w.PlayType,
            //                UserId = w.UserId,
            //                TotalBettingIssuseCount = w.TotalBettingIssuseCount,
            //                TotalBonusIssuseCount = w.TotalBonusIssuseCount,
            //                SchemeMoney = w.SchemeMoney,
            //                TotalBuyCount = w.TotalBuyCount,
            //                TotalBonusMoney = w.TotalBonusMoney,
            //                TotalProfitMoney = w.TotalProfitMoney,
            //                UserDisplayName = w.UserDisplayName,
            //                TotalSaleMoney = w.TotalSaleMoney,
            //                ActulProfitRatio = w.ActulProfitRatio,
            //                TotalReportRatio = w.TotalReportRatio,
            //                BonusFrequency = w.BonusFrequency,
            //                IsDelete = w.IsDelete,
            //                DeleteTime = w.DeleteTime.HasValue ? w.DeleteTime.Value : Convert.ToDateTime(null),
            //                CreateTime = w.CreateTime.Value,
            //                //IsExperter = w.IsExperter,
            //                IsShare = w.IsShare,
            //                ModelSecurity = (ModelSecurity)w.ModelSecurity,
            //                IsFirstPayment = w.IsFirstPayment,
            //                ProfitIssuseCount = w.ProfitIssuseCount,
            //                RiskType = (RiskType)w.RiskType,
            //                LossRatio = w.LossRatio,
            //                CommissionRitio = w.CommissionRitio,
            //                //IsProfit = w.IsProfit,
            //                TotalLoseMoney = w.TotalLoseMoney,
            //                TotalHaveMoney = w.TotalHaveMoney,
            //                TotalShareBonusMoney = w.TotalShareBonusMoney,
            //                TotalCommissionMoney = w.TotalCommissionMoney,
            //            };

            #endregion

            var query = from c in Session.Query<WinnerModelCollection>()
                        join w in Session.Query<WinnerModel>() on c.ModelId equals w.ModelId
                        where c.UserId == userId
                        select w;

            if (query != null && query.Count() > 0)
            {
                foreach (var item in query.ToList())
                {
                    WinnerModelInfo info = new WinnerModelInfo();
                    info.ModelId = item.ModelId;
                    info.ModelType = (ModelType)item.ModelType;
                    info.ModelName = item.ModelName;
                    info.ModelDescription = item.ModelDescription;
                    info.SchemeId = item.SchemeId;
                    info.GameCode = item.GameCode;
                    info.GameType = item.GameType;
                    info.PlayType = item.PlayType;
                    info.UserId = item.UserId;
                    info.TotalBettingIssuseCount = item.TotalBettingIssuseCount;
                    info.TotalBonusIssuseCount = item.TotalBonusIssuseCount;
                    info.SchemeMoney = item.SchemeMoney;
                    info.TotalBuyCount = item.TotalBuyCount;
                    info.TotalBonusMoney = item.TotalBonusMoney;
                    info.TotalProfitMoney = item.TotalProfitMoney;
                    info.UserDisplayName = item.UserDisplayName;
                    info.TotalSaleMoney = item.TotalSaleMoney;
                    info.ActulProfitRatio = item.ActulProfitRatio;
                    info.TotalReportRatio = item.TotalReportRatio;
                    info.BonusFrequency = item.BonusFrequency;
                    info.IsDelete = item.IsDelete;
                    info.DeleteTime = Convert.ToDateTime(item.DeleteTime);
                    info.CreateTime = Convert.ToDateTime(item.CreateTime);
                    info.IsShare = item.IsShare;
                    info.ModelSecurity = (ModelSecurity)item.ModelSecurity;
                    info.IsFirstPayment = item.IsFirstPayment;
                    info.ProfitIssuseCount = item.ProfitIssuseCount;
                    info.RiskType = (RiskType)item.RiskType;
                    info.LossRatio = item.LossRatio;
                    info.CommissionRitio = item.CommissionRitio;
                    info.TotalLoseMoney = item.TotalLoseMoney;
                    info.TotalHaveMoney = item.TotalHaveMoney;
                    info.TotalShareBonusMoney = item.TotalShareBonusMoney;
                    info.TotalCommissionMoney = item.TotalCommissionMoney;
                    var cycleList = new List<WinnerModelCycleInfo>();
                    foreach (var i in item.ModelCycleList)
                    {
                        WinnerModelCycleInfo cycleInfo = new WinnerModelCycleInfo();
                        ObjectConvert.ConverEntityToInfo(i, ref cycleInfo);
                        cycleInfo.CompleteTime = i.CompleteTime.HasValue ? i.CompleteTime.Value : Convert.ToDateTime(null);
                        cycleInfo.CreateTime = i.CreateTime.HasValue ? i.CreateTime.Value : Convert.ToDateTime(null);
                        cycleInfo.StopBettingTime = i.StopBettingTime.HasValue ? i.StopBettingTime.Value : Convert.ToDateTime(null);
                        cycleList.Add(cycleInfo);
                    }
                    info.ModelCycleInfoList = cycleList;
                    collection.ModelListInfo.Add(info);
                }
                collection.TotalCount = collection.ModelListInfo.Count;
                if (pageSize == -1)
                    return collection;
                else
                    collection.ModelListInfo = collection.ModelListInfo.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;

            #endregion

        }
        public List<WinnerModelCycleInfo> ConvertEntityToInfo(IList<WinnerModelCycle> list)
        {
            var result = new List<WinnerModelCycleInfo>();
            foreach (var item in list)
            {
                WinnerModelCycleInfo t = new WinnerModelCycleInfo();
                ObjectConvert.ConverEntityToInfo(item, ref t);
                t.CompleteTime = item.CompleteTime.HasValue ? item.CompleteTime.Value : Convert.ToDateTime(null);
                t.CreateTime = item.CreateTime.HasValue ? item.CreateTime.Value : Convert.ToDateTime(null);
                t.StopBettingTime = item.StopBettingTime.HasValue ? item.StopBettingTime.Value : Convert.ToDateTime(null);
                result.Add(t);
            }
            return result;
        }
        public WinnerModelSchemeInfo_Collection QueryMyBuyModelSchemeList(string userId, DateTime date, int pageIndex, int pageSize)
        {
            Session.Clear();
            WinnerModelSchemeInfo_Collection collection = new WinnerModelSchemeInfo_Collection();
            collection.TotalCount = 0;
            var startTime = new DateTime(date.Year, date.Month, 1);
            var endTime = startTime.AddMonths(1);
            var query = from s in Session.Query<WinnerModelScheme>()
                        join w in Session.Query<WinnerModel>() on s.ModelId equals w.ModelId
                        where s.UserId == userId && (s.CreateTime.Value >= startTime && s.CreateTime.Value < endTime) && s.IsStop == false
                        select new WinnerModelSchemeInfo
                        {
                            ModelId = s.ModelId,
                            ModelSchemeId = s.ModelSchemeId,
                            ModelKeyLine = s.ModelKeyLine,
                            CreateTime = s.CreateTime.Value,
                            ModelName = s.ModelName,
                            TotalMoney = s.TotalMoney,
                            UserId = s.UserId,
                            SchemeProgressStatus = s.SchemeProgressStatus,
                            TotalChaseIssuseCount = s.TotalChaseIssuseCount,
                            CompleteIssuseCount = s.CompleteIssuseCount,
                            CompleteIssuseMoney=s.CompleteIssuseMoney,
                            TotalBonusMoney = s.TotalBonusMoney,
                            UserDisplayName = w.UserDisplayName,
                            BettingType=s.BettingType,
                            BuyType=s.BuyType,
                        };
            if (query != null && query.Count() > 0)
            {
                collection.TotalCount = query.Count();
                if (pageSize == -1)
                    collection.ModelSchemeList.AddRange(query.ToList());
                else
                    collection.ModelSchemeList.AddRange(query.Skip(pageIndex * pageSize).Take(pageSize).ToList());
            }
            return collection;
        }
        public WinnerModelRank_Collection QueryModelRanlList(int queryDay, bool isExperter, int pageIndex, int pageSize)
        {
            Session.Clear();
            WinnerModelRank_Collection collection = new WinnerModelRank_Collection();
            collection.TotalCount = 0;
            var StartTime = DateTime.Now.AddDays(-queryDay);

            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Winner_QueryModelRanlList"))
                .AddInParameter("StartTime", StartTime)
                .AddInParameter("EndTime", DateTime.Now.AddDays(1).Date)
                .AddInParameter("IsExperter", isExperter)
                .AddInParameter("PageIndex", pageIndex)
                .AddInParameter("PageSize", pageSize)
                .AddOutParameter("TotalCount", "Int32");

            var dt = query.GetDataTable(out outputs);
            collection.TotalCount = UsefullHelper.GetDbValue<int>(outputs["TotalCount"]);
            collection.ModelRankList.AddRange(ORMHelper.DataTableToInfoList<WinnerModelRankInfo>(dt));
            return collection;
        }
        public WinnerModelRank_Collection QueryMyAttentionList(string userId, int pageIndex, int pageSize)
        {
            Session.Clear();
            WinnerModelRank_Collection collection = new WinnerModelRank_Collection();
            collection.TotalCount = 0;
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Winner_QueryMyAttention"))
                .AddInParameter("UserId", userId)
                .AddInParameter("PageIndex", pageIndex)
                .AddInParameter("PageSize", pageSize)
                .AddOutParameter("TotalCount", "Int32");

            var dt = query.GetDataTable(out outputs);
            collection.TotalCount = UsefullHelper.GetDbValue<int>(outputs["TotalCount"]);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    WinnerModelRankInfo info = new WinnerModelRankInfo();
                    info.UserDisplayName = row["UserDisplayName"] == null ? string.Empty : row["UserDisplayName"].ToString();
                    info.TotalModelCount = Convert.ToInt32(row["TotalModelCount"]);
                    info.TotalAttentionCount = Convert.ToInt32(row["BeAttentionUserCount"]);
                    info.TotalFollowerCount = Convert.ToInt32(row["FollowerUserCount"]);
                    info.TotalRateReturn = Convert.ToDecimal(row["TotalRateReturn"]);
                    collection.ModelRankList.Add(info);
                }
            }

            return collection;

            #region
            //var query = from w in Session.Query<WinnerModel>()
            //            join a in Session.Query<UserAttention>() on w.UserId equals a.BeAttentionUserId
            //            where a.FollowerUserId == userId
            //            select new WinnerModelRankInfo
            //            {
            //                UserDisplayName = w.UserDisplayName,
            //                TotalBuyCount = w.TotalBuyCount == null ? 0 : w.TotalBuyCount,
            //                TotalBuyMoney = w.TotalModelBettingMoney == null ? 0M : w.TotalModelBettingMoney,
            //                TotalBonusMoney = w.TotalModelBonusMoney == null ? 0M : w.TotalModelBonusMoney,
            //                UserId = w.UserId,
            //            };





            //var query = from w in Session.Query<WinnerModel>()
            //            join a in Session.Query<UserAttention>() on w.UserId equals a.BeAttentionUserId
            //            where a.FollowerUserId == userId
            //            group w by new { w.UserId, w.UserDisplayName } into g

            //            select new WinnerModelRankInfo
            //            {
            //                UserDisplayName = g.Key.UserDisplayName,
            //                TotalBuyCount = g.Sum(s => s.TotalBuyCount),
            //                TotalBuyMoney = g.Sum(s => s.TotalModelBettingMoney),
            //                TotalBonusMoney = g.Sum(s => s.TotalModelBonusMoney),
            //                UserId = g.Key.UserId,
            //                TotalModelCount = g.Count(),
            //            };
            //if (query != null && query.ToList().Count> 0)
            //{
            //    var result = from q in query.ToList()
            //                 join u in Session.Query<UserAttentionSummary>() on q.UserId equals u.UserId into r
            //                 from a in r.DefaultIfEmpty()
            //                 select new WinnerModelRankInfo
            //                 {
            //                     UserDisplayName = q.UserDisplayName,
            //                     TotalModelCount = q.TotalModelCount,
            //                     TotalAttentionCount = a == null ? 0 : a.BeAttentionUserCount,
            //                     TotalFollowerCount = a == null ? 0 : a.FollowerUserCount,
            //                     TotalRateReturn = q.TotalBuyMoney == 0 ? 0 : q.TotalBonusMoney / q.TotalBuyMoney,
            //                 };
            //    if (result != null && result.Count() > 0)
            //    {
            //        collection.TotalCount = result.Count();
            //        collection.ModelRankList = result.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //    }


                //var newQuery = query.GroupBy(s => new { s.UserId, s.UserDisplayName, s.TotalBuyCount, s.TotalBuyMoney, s.TotalBonusMoney }).Select(s => new
                //{
                //    UserDisplayName = s.Key.UserDisplayName,
                //    TotalBuyCount = s.Sum(q => s.Key.TotalBuyCount),
                //    TotalBuyMoney = s.Sum(q => s.Key.TotalBuyMoney),
                //    TotalBonusMoney = s.Sum(q => s.Key.TotalBonusMoney),
                //    UserId = s.Key.UserId,
                //    TotalModelCount = s.Count(),
                //});


                //var newQuery = query.GroupBy(s => new { s.UserId, s.UserDisplayName}).Select(s => new
                //{
                //    UserDisplayName = s.Key.UserDisplayName,
                //    TotalBuyCount = s.Sum(q => q.TotalBuyCount),
                //    //TotalBuyMoney = s.Sum(q => q.TotalBuyMoney),
                //    //TotalBonusMoney = s.Sum(q => q.TotalBonusMoney),
                //    UserId = s.Key.UserId,
                //    TotalModelCount = s.Count(),
                //});

                //if (newQuery != null && newQuery.Count() > 0)
                //{
                //    var result = from q in newQuery.ToList()
                //                 join u in Session.Query<UserAttentionSummary>() on q.UserId equals u.UserId into r
                //                 from a in r.DefaultIfEmpty()
                //                 select new WinnerModelRankInfo
                //                 {
                //                     UserDisplayName = q.UserDisplayName,
                //                     TotalModelCount = q.TotalModelCount,
                //                     TotalAttentionCount = a == null ? 0 : a.BeAttentionUserCount,
                //                     TotalFollowerCount = a == null ? 0 : a.FollowerUserCount,
                //                     TotalRateReturn = q.TotalBuyMoney == 0 ? 0 : q.TotalBonusMoney / q.TotalBuyMoney,
                //                 };
                //    if (result != null && result.Count() > 0)
                //    {
                //        collection.TotalCount = result.Count();
                //        collection.ModelRankList = result.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                //    }
                //}
            //}
            #endregion

        }
        public WinnerModelCenterInfo QueryWinnerModelCenter(string clickUserId)
        {
            Session.Clear();
            WinnerModelCenterInfo info = new WinnerModelCenterInfo();

            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Winner_QueryCelebrityCenter"))
                .AddInParameter("ClickUserId", clickUserId)
                .AddOutParameter("TotalModelCount", "Int32")
                .AddOutParameter("TotalBuyCount", "Int32")
                .AddOutParameter("TotalBonusMoney", "Decimal")
                .AddOutParameter("CreateTime", "DateTime")
                .AddOutParameter("ProfiteModelCount", "Int32")
                .AddOutParameter("TotalRateReturn", "Decimal")
                .AddOutParameter("TotalBonusFrequency", "Decimal");

            var dt = query.GetDataTable(out outputs);
            info.TotalModelCount = UsefullHelper.GetDbValue<int>(outputs["TotalModelCount"]);
            info.TotalBuyCount = UsefullHelper.GetDbValue<int>(outputs["TotalBuyCount"]);
            info.TotalBonusMoney = UsefullHelper.GetDbValue<decimal>(outputs["TotalBonusMoney"]);
            info.CreateTime = UsefullHelper.GetDbValue<DateTime>(outputs["CreateTime"]);
            info.ProfiteModelCount = UsefullHelper.GetDbValue<int>(outputs["ProfiteModelCount"]);
            info.TotalRateReturn = UsefullHelper.GetDbValue<decimal>(outputs["TotalRateReturn"]);
            info.TotalBonusFrequency = UsefullHelper.GetDbValue<decimal>(outputs["TotalBonusFrequency"]);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    info.UserId = UsefullHelper.GetDbValue<string>(row["UserId"]);
                    info.UserDisplayName = UsefullHelper.GetDbValue<string>(row["UserDisplayName"]);
                    info.BeAttentionUserCount = UsefullHelper.GetDbValue<int>(row["BeAttentionUserCount"]);
                    info.FollowerUserCount = UsefullHelper.GetDbValue<int>(row["FollowerUserCount"]);
                    info.Description = UsefullHelper.GetDbValue<string>(row["Description"]);
                    if (Convert.ToString(row["DealWithType"]) == string.Empty)
                        info.DealWithType = null;
                    else
                        info.DealWithType = UsefullHelper.GetDbValue<DealWithType>(row["DealWithType"]);
                }
            }
            return info;
        }

        #region 测试函数

        public List<Sports_Order_Running> QuerySaveOrderList()
        {
            return Session.Query<Sports_Order_Running>().OrderBy(s => s.CreateTime).Where(s => s.SchemeBettingCategory == SchemeBettingCategory.WinnerModel).ToList();
        }

        #endregion

        #endregion
    }
}
