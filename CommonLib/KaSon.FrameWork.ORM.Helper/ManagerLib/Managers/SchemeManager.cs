using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// kason
    /// </summary>
   public class SchemeManager : DBbase
    {
        public void AddOrderDetail(C_OrderDetail entity)
        {
            DB.GetDal<C_OrderDetail>().Add(entity);
        }

        public C_OrderDetail QueryOrderDetail(string schemeId)
        {
           // Session.Clear();
            return DB.CreateQuery<C_OrderDetail>().Where(o => o.SchemeId == schemeId).FirstOrDefault();
        }

        //public string QueryMobileNumberBySchemeId(string schemeId)
        //{
        //    string sql = string.Format(@"select m.Mobile
        //                from E_Authentication_Mobile as m
        //                left join C_OrderDetail as o on m.UserId = o.UserId
        //                where o.SchemeId='{0}' ", schemeId);
        //    return DB.CreateSQLQuery(sql).UniqueResult<string>();
        //}

        public void UpdateOrderDetail(C_OrderDetail entity)
        {
            DB.GetDal<C_OrderDetail>().Update(entity);
        }

        public List<C_OrderDetail> QueryOrderDetailList(DateTime startTime, DateTime endTime)
        {
           
            var query = from d in DB.CreateQuery<C_OrderDetail>()
                        where d.CreateTime >= startTime && d.CreateTime < endTime
                        select d;

            return query.ToList();
        }

        public List<C_OrderDetail> QueryOrderDetailListBySchemeId(string[] schemeIdArray)
        {
           
            var query = from d in DB.CreateQuery<C_OrderDetail>()
                        where schemeIdArray.Contains(d.SchemeId)
                        select d;

            return query.ToList();
        }

        /// <summary>
        /// 查询中奖次数
        /// </summary>
        public int QueryUserWinCount(string userId, string playType)
        {
            
            var query = from d in DB.CreateQuery<C_OrderDetail>()
                        where d.UserId == userId
                        && (playType == string.Empty || (d.PlayType == playType && (d.GameCode == "JCZQ" || d.GameCode == "JCLQ")))
                        && d.BonusStatus == (int)BonusStatus.Win
                        select d;
            return query.Count();
        }

        /// <summary>
        /// 查询用户中奖总金额
        /// </summary>
        public decimal QueryUserWinMoney(string userId)
        {
          
            var query = from d in DB.CreateQuery<C_OrderDetail>()
                        where d.UserId == userId
                        && d.BonusStatus == (int)BonusStatus.Win
                        select d;
            return query.Sum(d => d.AfterTaxBonusMoney);
        }

        //public System.Data.DataSet QueryBettingAnteCode(DateTime startTime, DateTime endTime)
        //{
           
        //    var sql = string.Format(@"SELECT c.[SchemeId]
	       //                         ,c.[GameCode]
	       //                         ,c.[GameType]
	       //                         ,c.[PlayType]
	       //                         ,c.[IssuseNumber]
	       //                         ,c.[MatchId]
	       //                         ,c.[AnteCode]
	       //                         ,c.[IsDan]
	       //                         ,c.[Odds]
	       //                         ,c.[CreateTime]
	       //                         ,o.Amount
	       //                         ,o.TotalMoney
	       //                         FROM [C_Sports_AnteCode] c
	       //                         left join C_OrderDetail o on c.SchemeId = o.SchemeId
	       //                         where c.CreateTime>'{0}' and c.CreateTime<'{1}'", startTime.ToString("yyyy/MM/dd"), endTime.AddDays(1).ToString("yyyy/MM/dd"));
        //    var query = CreateOutputQuery(Session.CreateSQLQuery(sql));
        //    return query.GetDataSet();
        //}

        /// <summary>
        /// 查询用户投注金额
        /// </summary>
        public decimal QueryUserBetTotalMoney(string userId)
        {
           
            var totalMoney = 0M;

            var daigouTotalMoney = DB.CreateQuery<C_OrderDetail>().Where(p => p.SchemeType != (int)SchemeType.TogetherBetting
                && p.TicketStatus == (int)TicketStatus.Ticketed && p.IsVirtualOrder == false && p.UserId == userId);
            if (daigouTotalMoney.Count() > 0)
                totalMoney += daigouTotalMoney.Sum(p => p.CurrentBettingMoney);

            var togetherTotalMoney = DB.CreateQuery<C_Sports_TogetherJoin>().Where(p => p.JoinUserId == userId && p.JoinSucess == true);
            if (togetherTotalMoney.Count() > 0)
                totalMoney += togetherTotalMoney.Sum(p => p.Price * p.RealBuyCount);

            return totalMoney;
        }
        public int GetCurrDayGiveGrowth(string userId)
        {
            var query = from g in DB.CreateQuery<C_Fund_UserGrowthDetail>()
                        where g.Category == BusinessHelper.FundCategory_BuyGrowthValue && g.UserId == userId && (g.CreateTime >= DateTime.Now.Date && g.CreateTime < DateTime.Now.AddDays(1).Date)
                        select g.PayMoney;
            if (query != null && query.Count() > 0)
            {
                return query.Sum();
            }
            return 0;
        }
    }
}
