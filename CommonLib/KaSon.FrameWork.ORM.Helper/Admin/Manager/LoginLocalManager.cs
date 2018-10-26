using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    public class LoginLocalManager : DBbase
    {
        public List<UserSysData> QueryUserList(DateTime regFrom, DateTime regTo, string keyType, string keyValue, bool? isEnable, bool? isFillMoney, bool? isUserType, bool? isAgent
            , string commonBlance, string bonusBlance, string freezeBlance, string vipRange, string comeFrom, string agentId, int pageIndex, int pageSize,
            out int totalCount, out decimal totalFillMoneyBalance, out decimal totalBonusBalance, out decimal totalCommissionBalance,
            out decimal totalExpertsBalance, out decimal totalFreezeBalance, out decimal totalRedBagBalance, out int totalDouDou, out decimal totalCPSBalance, string strOrderBy = "", int UserCreditType = -1)
        {
            #region 构造查询语句

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var sqlCondition = new List<string>();

            #region 条件 - 注册时间
            sqlCondition.Add(string.Format("  where R.CreateTime >= N'{0:yyyy-MM-dd}' AND R.CreateTime <= N'{1:yyyy-MM-dd}'", regFrom, regTo.AddDays(1)));
            #endregion

            #region "条件 是否内部员工 2018-01-21增加"
            if (isUserType.HasValue)
            {
                sqlCondition.Add(string.Format(" and R.UserType = {0} ", isUserType.Value ? 1 : 0));
            }
            else
            {
                sqlCondition.Add(" and R.UserType in (0,1) ");
            }
            if (UserCreditType != -1)
            {
                sqlCondition.Add(" and R.UserCreditType = " + UserCreditType + " ");
            }
            #endregion

            #region 条件 - 关键字
            if (!string.IsNullOrEmpty(keyValue))
            {
                if (string.IsNullOrEmpty(keyType) || keyType.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND (R.UserId = N'{0}' OR [R].[DisplayName] LIKE N'{0}%' OR M.Mobile LIKE N'{0}%' OR N.RealName LIKE N'{0}%' OR N.IdCardNumber LIKE N'{0}%' OR E.Email LIKE N'{0}%' )", keyValue));
                }
                else if (keyType.Equals("UserId", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND R.UserId = N'{0}'", keyValue));
                }
                else if (keyType.Equals("DisplayName", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND R.DisplayName LIKE N'{0}%'", keyValue));
                }
                else if (keyType.Equals("Mobile", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND M.Mobile LIKE N'{0}%'", keyValue));
                }
                else if (keyType.Equals("RealName", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND N.RealName LIKE N'{0}%'", keyValue));
                }
                else if (keyType.Equals("IdCard", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND N.IdCardNumber LIKE N'{0}%'", keyValue));
                }
                else if (keyType.Equals("Email", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND E.Email LIKE N'{0}%'", keyValue));
                }
                else if (keyType.Equals("RegisterIp", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND [R].RegisterIp LIKE N'{0}%'", keyValue));
                }
            }
            #endregion

            #region 条件 - 禁用状态、充值状态、经销商状态
            if (isEnable.HasValue)
            {
                sqlCondition.Add(string.Format("AND R.IsEnable = {0} ", isEnable.Value ? 1 : 0));
            }
            if (isFillMoney.HasValue)
            {
                sqlCondition.Add(string.Format("AND R.IsFillMoney = {0} ", isFillMoney.Value ? 1 : 0));
            }
            if (isAgent.HasValue)
            {
                sqlCondition.Add(string.Format("AND R.IsAgent = {0} ", isAgent.Value ? 1 : 0));
            }
            #endregion

            #region 条件 - 账户余额

            var spliter = '-';
            if (!string.IsNullOrEmpty(commonBlance))
            {
                var from = commonBlance.Split(spliter)[0];
                if (!string.IsNullOrEmpty(from)) { sqlCondition.Add(string.Format("AND B.FillMoneyBalance >= {0} ", decimal.Parse(from))); }
                var to = commonBlance.Split(spliter)[1];
                if (!string.IsNullOrEmpty(to)) { sqlCondition.Add(string.Format("AND B.FillMoneyBalance <= {0} ", decimal.Parse(to))); }
            }
            if (!string.IsNullOrEmpty(bonusBlance))
            {
                var from = bonusBlance.Split(spliter)[0];
                if (!string.IsNullOrEmpty(from)) { sqlCondition.Add(string.Format("AND B.BonusBalance >= {0} ", decimal.Parse(from))); }
                var to = bonusBlance.Split(spliter)[1];
                if (!string.IsNullOrEmpty(to)) { sqlCondition.Add(string.Format("AND B.BonusBalance <= {0} ", decimal.Parse(to))); }
            }
            if (!string.IsNullOrEmpty(freezeBlance))
            {
                var from = freezeBlance.Split(spliter)[0];
                if (!string.IsNullOrEmpty(from)) { sqlCondition.Add(string.Format("AND B.FreezeBalance >= {0} ", decimal.Parse(from))); }
                var to = freezeBlance.Split(spliter)[1];
                if (!string.IsNullOrEmpty(to)) { sqlCondition.Add(string.Format("AND B.FreezeBalance <= {0} ", decimal.Parse(to))); }
            }

            #endregion

            #region VIP等级限制

            if (!string.IsNullOrEmpty(vipRange))
            {
                var from = vipRange.Split(spliter)[0];
                if (!string.IsNullOrEmpty(from)) { sqlCondition.Add(string.Format("AND [R].[VipLevel] >= {0} ", decimal.Parse(from))); }
                var to = vipRange.Split(spliter)[1];
                if (!string.IsNullOrEmpty(to)) { sqlCondition.Add(string.Format("AND [R].[VipLevel] <= {0} ", decimal.Parse(to))); }
            }

            #endregion

            #region  条件 - 注册来源、经销商
            if (!string.IsNullOrEmpty(comeFrom))
            {
                sqlCondition.Add(string.Format("AND [R].[ComeFrom] = N'{0}' ", comeFrom));
            }
            if (!string.IsNullOrEmpty(agentId))
            {
                sqlCondition.Add(string.Format("AND R.AgentId = N'{0}' ", agentId));
            }
            #endregion

            var orderBy = " tab.CreateTime desc ";
            if (!string.IsNullOrEmpty(strOrderBy))
                orderBy = strOrderBy;

            #endregion

            var sqlBuilder_count = new StringBuilder();
            sqlBuilder_count.AppendLine("--moebius_onlyread");
            sqlBuilder_count.AppendLine("SELECT COUNT(1) AS TotalCount,SUM(FillMoneyBalance) as FillMoneyBalance,SUM(BonusBalance) as BonusBalance,Sum(CommissionBalance) as CommissionBalance,SUM(ExpertsBalance)as ExpertsBalance,SUM(FreezeBalance)as FreezeBalance,SUM(RedBagBalance)as RedBagBalance,SUM(CurrentDouDou) as DouDou,SUM(CPSBalance) as CPSBalance from(");
            sqlBuilder_count.AppendLine("SELECT [UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId");
            sqlBuilder_count.AppendLine("     ,FillMoneyBalance,BonusBalance,FreezeBalance,CommissionBalance,RedBagBalance,ExpertsBalance,CurrentDouDou,CPSBalance");
            sqlBuilder_count.AppendLine("FROM (");
            sqlBuilder_count.AppendLine("     SELECT ROW_NUMBER() OVER(ORDER BY [R].[CreateTime] DESC) AS [RowNumber],[R].[UserId],[R].[VipLevel],[R].[DisplayName],[R].[ComeFrom],[R].[RegisterIp],[R].[CreateTime],R.IsEnable,R.IsAgent,R.IsFillMoney,R.AgentId");
            sqlBuilder_count.AppendLine("         ,[B].FillMoneyBalance,B.BonusBalance,B.FreezeBalance,B.CommissionBalance,B.RedBagBalance,B.ExpertsBalance,CurrentDouDou,B.CPSBalance");
            sqlBuilder_count.AppendLine("     FROM [C_User_Register] AS [R] with(nolock)");
            sqlBuilder_count.AppendLine("     INNER JOIN [C_User_Balance] AS [B] with(nolock) ON [R].[UserId] = [B].[UserId]");

            if (!string.IsNullOrEmpty(keyType))
            {
                sqlBuilder_count.AppendLine("     LEFT OUTER JOIN [E_Authentication_Email] AS [E] with (nolock)  ON [E].[UserId] = [R].[UserId]");
                sqlBuilder_count.AppendLine("     LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock)  ON [M].[UserId] = [R].[UserId]");
                sqlBuilder_count.AppendLine("     LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock)  ON [N].[UserId] = [R].[UserId]");

            }

            sqlBuilder_count.AppendLine(string.Join(" ", sqlCondition.ToArray()));
            sqlBuilder_count.AppendLine(string.Format(") AS T"));
            sqlBuilder_count.AppendLine(") tab ");

            var sqlBuilder_query = new StringBuilder();
            sqlBuilder_query.AppendLine("--moebius_onlyread");
            sqlBuilder_query.AppendLine("select tab.[UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId,FillMoneyBalance,BonusBalance,FreezeBalance,CommissionBalance,RedBagBalance,ExpertsBalance,IsSettedMobile,Mobile,IsSettedRealName,RealName,CardType,IdCardNumber,IsSettedEmail,Email,VipLevel,UserType,AlipayAccount,QQ,OCAgentCategory,CPSBalance,CPSMode,UserCreditType,UpdateBy from(");
            sqlBuilder_query.AppendLine("SELECT [UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId");
            sqlBuilder_query.AppendLine("     ,FillMoneyBalance,BonusBalance,FreezeBalance,CommissionBalance,RedBagBalance,ExpertsBalance");
            sqlBuilder_query.AppendLine("     ,IsSettedMobile,Mobile,IsSettedRealName,RealName,CardType,IdCardNumber,IsSettedEmail,Email,VipLevel,UserType,AlipayAccount,QQ,OCAgentCategory,CPSBalance,CPSMode,UserCreditType,UpdateBy");
            sqlBuilder_query.AppendLine("FROM (");
            sqlBuilder_query.AppendLine("     SELECT ROW_NUMBER() OVER(ORDER BY " + (string.IsNullOrEmpty(strOrderBy) ? "[R].CreateTime desc" : strOrderBy) + ") AS [RowNumber],[R].[UserId],[R].[VipLevel],[R].[DisplayName],[R].[ComeFrom],[R].[RegisterIp],[R].[CreateTime],R.IsEnable,R.IsAgent,R.IsFillMoney,R.AgentId");
            sqlBuilder_query.AppendLine("         ,[B].FillMoneyBalance,B.BonusBalance,B.FreezeBalance,B.CommissionBalance,B.RedBagBalance,B.ExpertsBalance");
            sqlBuilder_query.AppendLine("         ,M.IsSettedMobile,M.Mobile,N.IsSettedRealName,N.RealName,N.CardType,N.IdCardNumber,E.IsSettedEmail,E.Email,R.UserType,a.AlipayAccount,Q.QQ,O.OCAgentCategory,B.CPSBalance,O.CPSMode,R.UserCreditType,N.UpdateBy");
            sqlBuilder_query.AppendLine("     FROM [C_User_Register] AS [R] with(nolock)");

            sqlBuilder_query.AppendLine("     INNER JOIN [C_User_Balance] AS [B] with(nolock) ON [R].[UserId] = [B].[UserId]");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Email] AS [E] with (nolock)  ON [E].[UserId] = [R].[UserId]");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock)  ON [M].[UserId] = [R].[UserId]");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock)  ON [N].[UserId] = [R].[UserId]");


            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Alipay] AS [A] with (nolock) ON [A].UserId=[R].UserId");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_QQ] AS [Q] with (nolock) ON [Q].UserId=[R].UserId");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [P_OCAgent] AS [O] with (nolock) on [O].UserId= r.UserId");

            sqlBuilder_query.AppendLine(string.Join(" ", sqlCondition.ToArray()));
            sqlBuilder_query.AppendLine(string.Format(") AS T WHERE [RowNumber] > {0} AND [RowNumber] <= {1}", pageIndex * pageSize, (pageIndex + 1) * pageSize));
            sqlBuilder_query.AppendLine(") tab order by " + orderBy + "");

            var totalModel = DB.CreateSQLQuery(sqlBuilder_count.ToString()) as UserSysCount;
            totalCount = 0;
            totalFillMoneyBalance = 0M;
            totalBonusBalance = 0M;
            totalCommissionBalance = 0M;
            totalExpertsBalance = 0M;
            totalFreezeBalance = 0M;
            totalRedBagBalance = 0M;
            totalDouDou = 0;
            totalCPSBalance = 0M;
            if (totalModel != null)
            {
                totalCount = totalModel.TotalCount;
                totalFillMoneyBalance = totalModel.FillMoneyBalance;
                totalBonusBalance = totalModel.BonusBalance;
                totalCommissionBalance = totalModel.CommissionBalance;
                totalExpertsBalance = totalModel.ExpertsBalance;
                totalFreezeBalance = totalModel.FreezeBalance;
                totalRedBagBalance = totalModel.RedBagBalance;
                totalDouDou = totalModel.DouDou;
                totalCPSBalance = totalModel.CPSBalance;
            }
            return DB.CreateSQLQuery(sqlBuilder_query.ToString()).List<UserSysData>().ToList();
        }
        public NotOnlineUserCollection QueryNotOnlineRecentlyList(int days, int pageIndex, int pageSize)
        {
            var result = new NotOnlineUserCollection();
            string countSql = @"select Count(*) from (select UserId from C_User_Register where IsFillMoney=1 and IsEnable=1
  except
SELECT distinct UserId from E_Blog_UserLoginHistory where DATEDIFF(DAY,LoginTime,CURRENT_TIMESTAMP)<=@days) as a";
            result.TotalCount = DB.CreateSQLQuery(countSql).SetInt("@days", days).First<int>();

            string querySql = @"with filluser AS(
 select UserId from C_User_Register where IsFillMoney=1 and IsEnable=1
  except
SELECT distinct UserId from E_Blog_UserLoginHistory where DATEDIFF(DAY,LoginTime,CURRENT_TIMESTAMP)<=@days)
,
 getmoney as (
  select UserId, SUM(RequestMoney)as chongzhi from dbo.C_FillMoney
where Status=1
group by UserId
  ),
	wmoney as (
select UserId, SUM(RequestMoney)as tikuan from C_Withdraw
where Status=3
group by UserId
)
select * from(
select  ROW_NUMBER() OVER(ORDER BY f.UserId) AS RowNumber, f.UserId,e.Mobile,a.RealName,fi.chongzhi as TotalFillMoney,
eb.TotalBonusMoney,isnull(wm.tikuan,0) as TotalWithdraw,cu.FillMoneyBalance,(eb.TotalBonusMoney-fi.chongzhi+cu.FillMoneyBalance) as Earnings from filluser as f
left join E_Authentication_Mobile as e on f.UserId=e.UserId
left join E_Authentication_RealName as a on f.UserId=a.UserId
left join getmoney as fi on f.UserId=fi.UserId
left join E_Blog_DataReport eb on f.userId=eb.UserId
left join C_User_Balance cu on f.userId=cu.UserId
left join wmoney wm on f.userId=wm.UserId)
AS T WHERE RowNumber > @skipsize AND RowNumber <= @maxsize";
            var array = DB.CreateSQLQuery(querySql)
                 .SetInt("@days", days)
                 .SetInt("@skipsize", pageIndex * pageSize)
                 .SetInt("@maxsize", (pageIndex + 1) * pageSize).List<NotOnlineUser>();
            if (array == null)
                return result;
            result.UserList = array.ToList();
            return result;
        }
        public string GiveMoneyToStayUser(string UserId, string operatorId)
        {
            CoreConfigInfo config = null;

            var configList = new UserIntegralManager().QueryAllCoreConfig();
            config = configList.FirstOrDefault(p => p.ConfigKey == "NotOnlineUserGift");

            if (config == null || string.IsNullOrEmpty(config.ConfigValue))
            {
                throw new LogicException("配置文件获取失败");
            }
            var json = Common.JSON.JsonHelper.Deserialize<NotOnlineUserGiftConfig>(config.ConfigValue);
            if (!json.IsEnable)
            {
                throw new LogicException("当前赠送活动配置设置为关闭");
            }
            var sql = @"select (eb.TotalBonusMoney-fi.chongzhi+cu.FillMoneyBalance) as Earnings  from C_User_Register c
left join  C_User_Balance cu on c.userId = cu.UserId
left join(
  select UserId, SUM(RequestMoney)as chongzhi from C_FillMoney
where Status = 1 and UserId = :useId
group by UserId
  ) as fi on c.UserId = fi.UserId
left join E_Blog_DataReport eb on c.userId = eb.UserId
 where c.IsFillMoney = 1 and c.IsEnable = 1 and c.UserId = @useId";
            var Earnings = DB.CreateSQLQuery(sql).SetString("@useId", UserId).First<decimal>();
            NotOnlineUserGiftItem theItem;
            if (Earnings >= 0)
            {
                var list = json.GiftList.Where(p => p.IsProfit == true).OrderByDescending(p => p.ProfitMoney).ToList();
                theItem = list.FirstOrDefault(p => Earnings >= p.ProfitMoney);
            }
            else
            {
                var AbsEarnings = Math.Abs(Earnings);
                var list = json.GiftList.Where(p => p.IsProfit == false).OrderByDescending(p => p.ProfitMoney).ToList();
                theItem = list.FirstOrDefault(p => AbsEarnings >= p.ProfitMoney);
            }
            if (theItem == null)
            {
                throw new LogicException("配置文件不完整");
            }
            var str = "赠送给用户";
            if (theItem.GiveBonus > 0)
            {
                BusinessHelper.Payin_To_Balance(AccountType.Bonus, BusinessHelper.FundCategory_Activity, UserId, Guid.NewGuid().ToString("N"), theItem.GiveBonus,
                string.Format("赠送回归现金{0:N2}元", theItem.GiveBonus), RedBagCategory.Activity, operatorId);
                str += string.Format("现金{0:N2}元", theItem.GiveBonus);
            }
            if (theItem.GiveRedPackets > 0)
            {
                BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, UserId, Guid.NewGuid().ToString("N"), theItem.GiveRedPackets,
               string.Format("赠送回归红包{0:N2}元", theItem.GiveRedPackets), RedBagCategory.Activity, operatorId);
                str += string.Format("红包{0:N2}元", theItem.GiveRedPackets);
            }
            return str;
        }
        public NotOnlineUserCollection QueryNotOnlineRecentlyList(int days, int pageIndex, int pageSize, string theEarnings)
        {
            var result = new NotOnlineUserCollection();
            var strs = theEarnings.Split('|');
            var endsql = "";
            if (strs.Count() == 1)
            {
                var num1 = Convert.ToInt32(strs[0]);
                if (num1 > 0)
                {
                    endsql = " and (isnull(wm.tikuan,0)+cu.FillMoneyBalance+cu.bonusbalance-fi.chongzhi) > " + num1;
                }
                if (num1 < 0)
                {
                    endsql = " and (isnull(wm.tikuan,0)+cu.FillMoneyBalance+cu.bonusbalance-fi.chongzhi) < " + num1;
                }
            }
            else
            {
                var num1 = Convert.ToInt32(strs[0]);
                var num2 = Convert.ToInt32(strs[1]);
                if (num2 > 0)
                {
                    endsql = " and (isnull(wm.tikuan,0)+cu.FillMoneyBalance+cu.bonusbalance-fi.chongzhi) >= " + num1 + " and (isnull(wm.tikuan,0)+cu.FillMoneyBalance+cu.bonusbalance-fi.chongzhi) <" + num2;
                }
                if (num2 < 0)
                {
                    endsql = " and (isnull(wm.tikuan,0)+cu.FillMoneyBalance+cu.bonusbalance-fi.chongzhi) <= " + num1 + " and (isnull(wm.tikuan,0)+cu.FillMoneyBalance+cu.bonusbalance-fi.chongzhi) >" + num2;
                }
            }
            string countSql = @"with filluser AS(
 select UserId from C_User_Register where IsFillMoney=1 and IsEnable=1
  except
SELECT distinct UserId from E_Blog_UserLoginHistory where DATEDIFF(DAY,LoginTime,CURRENT_TIMESTAMP)<=@days)
,
 getmoney as (
  select UserId, SUM(RequestMoney)as chongzhi from dbo.C_FillMoney
where Status=1
group by UserId
  )
,
	wmoney as (
select UserId, SUM(RequestMoney)as tikuan from C_Withdraw
where Status=3
group by UserId
)
select Count(UserId) from(
 select f.UserId,(isnull(wm.tikuan,0)+cu.FillMoneyBalance+cu.bonusbalance-fi.chongzhi) as Earnings from 
filluser as f 
left join wmoney wm on f.userId=wm.UserId
left join getmoney as fi on f.UserId=fi.UserId 
left join C_User_Balance cu on f.userId=cu.UserId 
 where 1=1 {0}
) as ts";
            result.TotalCount = DB.CreateSQLQuery(string.Format(countSql, endsql)).SetInt("@days", days).First<int>();

            string querySql = @"with filluser AS(
 select UserId from C_User_Register where IsFillMoney=1 and IsEnable=1
  except
SELECT distinct UserId from E_Blog_UserLoginHistory where DATEDIFF(DAY,LoginTime,CURRENT_TIMESTAMP)<=@days)
,
 getmoney as (
  select UserId, SUM(RequestMoney)as chongzhi from dbo.C_FillMoney
where Status=1
group by UserId
  ),
	wmoney as (
select UserId, SUM(RequestMoney)as tikuan from C_Withdraw
where Status=3
group by UserId
)
select * from(
select  ROW_NUMBER() OVER(ORDER BY f.UserId) AS RowNumber, f.UserId,e.Mobile,a.RealName,fi.chongzhi as TotalFillMoney,
eb.TotalBonusMoney,isnull(wm.tikuan,0) as TotalWithdraw,cu.FillMoneyBalance,(isnull(wm.tikuan,0)+cu.FillMoneyBalance+cu.bonusbalance-fi.chongzhi) as Earnings
,cr.DisplayName,cu.BonusBalance 
 from filluser as f
left join E_Authentication_Mobile as e on f.UserId=e.UserId
left join E_Authentication_RealName as a on f.UserId=a.UserId
left join getmoney as fi on f.UserId=fi.UserId
left join E_Blog_DataReport eb on f.userId=eb.UserId
left join C_User_Balance cu on f.userId=cu.UserId
left join wmoney wm on f.userId=wm.UserId
left Join C_User_Register cr on f.userId=cr.UserId
where 1=1 {0}
)
AS T WHERE RowNumber > @skipsize AND RowNumber <= @maxsize ";
            querySql = string.Format(querySql, endsql);
            var array = DB.CreateSQLQuery(querySql)
                 .SetInt("@days", days)
                 .SetInt("@skipsize", pageIndex * pageSize)
                 .SetInt("@maxsize", (pageIndex + 1) * pageSize).List<NotOnlineUser>();
            if (array == null)
                return result;
            result.UserList = array.ToList();
            return result;
        }
        public NotOnlineUserCollection QueryExcelNotOnlineRecentlyList(int days, string theEarnings)
        {
            var strs = theEarnings.Split('|');
            var endsql = "";
            if (strs.Count() == 1)
            {
                var num1 = Convert.ToInt32(strs[0]);
                if (num1 > 0)
                {
                    endsql = " and (isnull(wm.tikuan,0)+cu.FillMoneyBalance+cu.bonusbalance-fi.chongzhi) > " + num1;
                }
                if (num1 < 0)
                {
                    endsql = " and (isnull(wm.tikuan,0)+cu.FillMoneyBalance+cu.bonusbalance-fi.chongzhi) < " + num1;
                }
            }
            else
            {
                var num1 = Convert.ToInt32(strs[0]);
                var num2 = Convert.ToInt32(strs[1]);
                if (num2 > 0)
                {
                    endsql = " and (isnull(wm.tikuan,0)+cu.FillMoneyBalance+cu.bonusbalance-fi.chongzhi) >= " + num1 + " and (isnull(wm.tikuan,0)+cu.FillMoneyBalance+cu.bonusbalance-fi.chongzhi) <" + num2;
                }
                if (num2 < 0)
                {
                    endsql = " and (isnull(wm.tikuan,0)+cu.FillMoneyBalance+cu.bonusbalance-fi.chongzhi) <= " + num1 + " and (isnull(wm.tikuan,0)+cu.FillMoneyBalance+cu.bonusbalance-fi.chongzhi) >" + num2;
                }
            }
            string querySql = @"with filluser AS(
 select UserId from C_User_Register where IsFillMoney=1 and IsEnable=1
  except
SELECT distinct UserId from E_Blog_UserLoginHistory where DATEDIFF(DAY,LoginTime,CURRENT_TIMESTAMP)<=@days)
,
 getmoney as (
  select UserId, SUM(RequestMoney)as chongzhi from dbo.C_FillMoney
where Status=1
group by UserId
  ),
	wmoney as (
select UserId, SUM(RequestMoney)as tikuan from C_Withdraw
where Status=3
group by UserId
)
select  ROW_NUMBER() OVER(ORDER BY f.UserId) AS RowNumber, f.UserId,e.Mobile,a.RealName,fi.chongzhi as TotalFillMoney,
eb.TotalBonusMoney,isnull(wm.tikuan,0) as TotalWithdraw,cu.FillMoneyBalance,(isnull(wm.tikuan,0)+cu.FillMoneyBalance+cu.bonusbalance-fi.chongzhi) as Earnings
,cr.DisplayName,cu.BonusBalance 
 from filluser as f
left join E_Authentication_Mobile as e on f.UserId=e.UserId
left join E_Authentication_RealName as a on f.UserId=a.UserId
left join getmoney as fi on f.UserId=fi.UserId
left join E_Blog_DataReport eb on f.userId=eb.UserId
left join C_User_Balance cu on f.userId=cu.UserId
left join wmoney wm on f.userId=wm.UserId
left Join C_User_Register cr on f.userId=cr.UserId 
where 1=1 {0} ";
            querySql = string.Format(querySql, endsql);
            var array = DB.CreateSQLQuery(querySql)
                 .SetInt("@days", days).List<NotOnlineUser>();
            var result = new NotOnlineUserCollection();
            if (array == null)
                return result;
            result.UserList = array.ToList();
            return result;
        }
        public E_Login_Local GetLoginByUserId(string userId)
        {
            return DB.CreateQuery<E_Login_Local>().Where(x=>x.UserId==userId).FirstOrDefault();
        }
        public void UpdateLogin(E_Login_Local login)
        {
            DB.GetDal<E_Login_Local>().Update(login);
        }
    }
}
