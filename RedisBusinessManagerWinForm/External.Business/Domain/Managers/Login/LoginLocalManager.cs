using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using External.Domain.Entities.Login;
using GameBiz.Auth.Domain.Entities;
using GameBiz.Business;
using GameBiz.Domain.Entities;
using NHibernate.Criterion;
using External.Core.Login;
using NHibernate.Linq;
using System.Linq;
using External.Domain.Entities.Authentication;
using External.Core.Authentication;
using Common.Utilities;

namespace External.Domain.Managers.Login
{
    public class LoginLocalManager : GameBiz.Business.GameBizEntityManagement
    {
        public LoginLocal Login(string loginName, string password)
        {
            Session.Clear();
            return this.Session.Query<LoginLocal>().Where(p => (p.LoginName == loginName || p.mobile == loginName) && p.Password == password).FirstOrDefault();

            //return Session.CreateCriteria<LoginLocal>()
            //    .Add(Restrictions.Eq("LoginName", loginName))
            //    .Add(Restrictions.Eq("Password", password))
            //    .UniqueResult<LoginLocal>();
        }
        public LoginLocal GetLoginByName(string loginName)
        {
            Session.Clear();
            //return Session.CreateCriteria<LoginLocal>()
            //    .Add(Restrictions.Eq("LoginName", loginName))
            //    .UniqueResult<LoginLocal>();
            return this.Session.Query<LoginLocal>().Where(p => (p.LoginName == loginName || p.mobile == loginName)).FirstOrDefault();
        }

        public LoginLocal GetLoginByUserId(string userId)
        {
            return GetByKey<LoginLocal>(userId);
        }
        public int GetTodayRegisterCount(DateTime date, string ip)
        {
            //Session.Clear();
            //var hql = "SELECT COUNT(*) FROM LoginLocal WHERE CreateTime >= :StartDate AND CreateTime < :EndTime AND Register.RegisterIp = :Ip";
            //var result = Session.CreateQuery(hql)
            //    .SetDateTime("StartDate", date.Date)
            //    .SetDateTime("EndTime", date.Date.AddDays(1))
            //    .SetString("Ip", ip)
            //    .UniqueResult<long>();
            //return (int)result;
            Session.Clear();
            var hql = "SELECT COUNT(*) FROM LoginLocal WHERE CreateTime >= :CreateTime AND Register.RegisterIp = :Ip";
            var result = Session.CreateQuery(hql)
                .SetDateTime("CreateTime", date)
                .SetString("Ip", ip)
                .UniqueResult<long>();
            return (int)result;
        }
        public void UpdateLogin(LoginLocal login)
        {
            Update<LoginLocal>(login);
        }
        public void Register(LoginLocal login)
        {
            login.CreateTime = DateTime.Now;
            Add<LoginLocal>(login);
        }
        public SystemUser LoadUser(string userId)
        {
            return LoadByKey<SystemUser>(userId);
        }
        public LoginLocal GetLocalLoginByUserId(string userId)
        {
            Session.Clear();
            return Session.CreateCriteria<LoginLocal>()
                .Add(Restrictions.Eq("UserId", userId))
                .UniqueResult<LoginLocal>();
        }

        public UserRegister LoadRegister(string userId)
        {
            return LoadByKey<UserRegister>(userId);
        }

        public int GetRgeisterTimeCount(DateTime date, string ip)
        {
            Session.Clear();
            var hql = "SELECT COUNT(userid) FROM C_User_Register WHERE CreateTime >= :StartDate AND RegisterIp = :Ip";
            var result = Session.CreateQuery(hql)
                .SetDateTime("CreateTime", date)
                .SetString("Ip", ip)
                .UniqueResult<long>();
            return (int)result;
        }


        public UserRegister GetRegisterByKey(string key)
        {
            Session.Clear();
            return Session.CreateCriteria<UserRegister>()
                .Add(Restrictions.Eq("UserKey", key))
                .UniqueResult<UserRegister>();
        }
        public List<string> QueryFunctionByRole(string[] arrayRole)
        {
            Session.Clear();
            string strSql = "select FunctionId from C_Auth_RoleFunction where RoleId in (:roleList)";
            var result = Session.CreateSQLQuery(strSql)
                   .SetParameterList("roleList", arrayRole).List();
            List<string> _fun = new List<string>();
            if (result != null)
            {
                foreach (var item in result)
                {
                    _fun.Add(item.ToString());
                }
            }
            return _fun;
        }

        //public UserQueryInfo GetUserByKey(string userId)
        //{
        //    Session.Clear();

        //    var query = from u in this.Session.Query<UserRegister>()
        //                join r in this.Session.Query<UserRealName>() on u.UserId equals r.UserId
        //                join m in this.Session.Query<UserMobile>() on u.UserId equals m.UserId
        //                where u.UserId == userId
        //                select new UserQueryInfo
        //                {
        //                    DisplayName = u.DisplayName,
        //                    RealName = r.RealName,
        //                    IdCardNumber = r.IdCardNumber,
        //                    Mobile = m.Mobile,

        //                };

        //    return query.FirstOrDefault();
        //}

        //public IList GetUserByKey(string key)
        //{
        //    var sqlBuilder_query = new StringBuilder();
        //    sqlBuilder_query.AppendLine("SELECT [R].[UserId],[R].[DisplayName],[R].[ComeFrom],[R].[RegisterIp],[R].[CreateTime],R.IsEnable,R.IsAgent,R.IsFillMoney,R.AgentId");
        //    sqlBuilder_query.AppendLine("    ,[B].CommonBalance,B.BonusBalance,B.FreezeBalance,B.CommissionBalance,B.ActivityBalance");
        //    sqlBuilder_query.AppendLine("    ,M.IsSettedMobile,M.Mobile,N.IsSettedRealName,N.RealName,N.CardType,N.IdCardNumber,E.IsSettedEmail,E.Email,[R].[VipLevel]");
        //    sqlBuilder_query.AppendLine("FROM [C_User_Register] AS [R] with(nolock)");
        //    sqlBuilder_query.AppendLine("INNER JOIN [C_User_Balance] AS [B] with(nolock) ON [R].[UserId] = [B].[UserId]");
        //    sqlBuilder_query.AppendLine("LEFT OUTER JOIN [E_Authentication_Email] AS [E] with (nolock)  ON [E].[UserId] = [R].[UserId]");
        //    sqlBuilder_query.AppendLine("LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock)  ON [M].[UserId] = [R].[UserId]");
        //    sqlBuilder_query.AppendLine("LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock)  ON [N].[UserId] = [R].[UserId]");
        //    sqlBuilder_query.AppendLine("WHERE [R].[UserId]=:UserId");

        //    return Session.CreateSQLQuery(sqlBuilder_query.ToString())
        //        .SetString("UserId", key)
        //        .UniqueResult() as IList;
        //}
        public IList QueryUserList(DateTime regFrom, DateTime regTo, string keyType, string keyValue, bool? isEnable, bool? isFillMoney, bool? isUserType, bool? isAgent
            , string commonBlance, string bonusBlance, string freezeBlance, string vipRange, string comeFrom, string agentId, int pageIndex, int pageSize,
            out int totalCount, out  decimal totalFillMoneyBalance, out  decimal totalBonusBalance, out  decimal totalCommissionBalance,
            out  decimal totalExpertsBalance, out  decimal totalFreezeBalance, out decimal totalRedBagBalance, out int totalDouDou, out  decimal totalCPSBalance, string strOrderBy = "")
        {
            #region 构造查询语句

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var sqlCondition = new List<string>();

            #region 条件 - 注册时间
            //sqlCondition.Add(string.Format("AND R.CreateTime >= N'{0:yyyy-MM-dd}' AND R.CreateTime <= N'{1:yyyy-MM-dd}'", regFrom, regTo.AddDays(1)));
            sqlCondition.Add(string.Format("  where R.CreateTime >= N'{0:yyyy-MM-dd}' AND R.CreateTime <= N'{1:yyyy-MM-dd}'", regFrom, regTo.AddDays(1)));
            //if (!string.IsNullOrEmpty(keyValue))
            //{
            //sqlCondition.Add(" and R.UserType in (0,1) ");
            //}
            //else
            //{
            //    sqlCondition.Add(" and R.UserType=0 ");//用户类别:0:网站普通用户；1：内部员工用户；
            //}
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
                    sqlCondition.Add(string.Format("AND [R].RegisterIp='{0}'", keyValue));
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

            #region 20151222
            //var sqlBuilder_count = new StringBuilder();
            //sqlBuilder_count.AppendLine("SELECT COUNT(1) AS TotalCount,SUM(FillMoneyBalance) as FillMoneyBalance,SUM(BonusBalance) as BonusBalance,Sum(CommissionBalance) as CommissionBalance,SUM(ExpertsBalance)as ExpertsBalance,SUM(wf.PayMoney)as FreezeBalance,SUM(RedBagBalance)as RedBagBalance,SUM(CurrentDouDou) as DouDou from(");
            //sqlBuilder_count.AppendLine("SELECT [UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId");
            //sqlBuilder_count.AppendLine("     ,FillMoneyBalance,BonusBalance,FreezeBalance,CommissionBalance,RedBagBalance,ExpertsBalance");
            //sqlBuilder_count.AppendLine("     ,IsSettedMobile,Mobile,IsSettedRealName,RealName,CardType,IdCardNumber,IsSettedEmail,Email,VipLevel,CurrentDouDou");
            //sqlBuilder_count.AppendLine("FROM (");
            //sqlBuilder_count.AppendLine("     SELECT ROW_NUMBER() OVER(ORDER BY [R].[CreateTime] DESC) AS [RowNumber],[R].[UserId],[R].[VipLevel],[R].[DisplayName],[R].[ComeFrom],[R].[RegisterIp],[R].[CreateTime],R.IsEnable,R.IsAgent,R.IsFillMoney,R.AgentId");
            //sqlBuilder_count.AppendLine("         ,[B].FillMoneyBalance,B.BonusBalance,B.FreezeBalance,B.CommissionBalance,B.RedBagBalance,B.ExpertsBalance");
            //sqlBuilder_count.AppendLine("         ,M.IsSettedMobile,M.Mobile,N.IsSettedRealName,N.RealName,N.CardType,N.IdCardNumber,E.IsSettedEmail,E.Email,CurrentDouDou");
            //sqlBuilder_count.AppendLine("     FROM [C_User_Register] AS [R] with(nolock)");
            //sqlBuilder_count.AppendLine("     INNER JOIN [C_User_Balance] AS [B] with(nolock) ON [R].[UserId] = [B].[UserId]");
            //sqlBuilder_count.AppendLine("     LEFT OUTER JOIN [E_Authentication_Email] AS [E] with (nolock)  ON [E].[UserId] = [R].[UserId]");
            //sqlBuilder_count.AppendLine("     LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock)  ON [M].[UserId] = [R].[UserId]");
            //sqlBuilder_count.AppendLine("     LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock)  ON [N].[UserId] = [R].[UserId]");
            //sqlBuilder_count.AppendLine("     WHERE 1=1 ");
            //sqlBuilder_count.AppendLine(string.Join(" ", sqlCondition.ToArray()));
            //sqlBuilder_count.AppendLine(string.Format(") AS T"));
            //sqlBuilder_count.AppendLine(") tab left join (select sum(f.PayMoney) PayMoney,f.UserId from C_Withdraw w inner join C_Fund_Detail f on w.OrderId=f.OrderId where w.Status=1 and f.AccountType=20 and f.Category!='" + BusinessHelper.FundCategory_IntegralRequestWithdraw + "' group by f.UserId) wf on tab.UserId=wf.UserId "); 

            //var sqlBuilder_query = new StringBuilder();
            //sqlBuilder_query.AppendLine("select tab.[UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId,FillMoneyBalance,BonusBalance,wf.PayMoney,CommissionBalance,RedBagBalance,ExpertsBalance,IsSettedMobile,Mobile,IsSettedRealName,RealName,CardType,IdCardNumber,IsSettedEmail,Email,VipLevel,UserType,AlipayAccount,QQ from(");
            //sqlBuilder_query.AppendLine("SELECT [UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId");
            //sqlBuilder_query.AppendLine("     ,FillMoneyBalance,BonusBalance,FreezeBalance,CommissionBalance,RedBagBalance,ExpertsBalance");
            //sqlBuilder_query.AppendLine("     ,IsSettedMobile,Mobile,IsSettedRealName,RealName,CardType,IdCardNumber,IsSettedEmail,Email,VipLevel,UserType,AlipayAccount,QQ");
            //sqlBuilder_query.AppendLine("FROM (");
            //sqlBuilder_query.AppendLine("     SELECT ROW_NUMBER() OVER(ORDER BY [R].[CreateTime] DESC) AS [RowNumber],[R].[UserId],[R].[VipLevel],[R].[DisplayName],[R].[ComeFrom],[R].[RegisterIp],[R].[CreateTime],R.IsEnable,R.IsAgent,R.IsFillMoney,R.AgentId");
            //sqlBuilder_query.AppendLine("         ,[B].FillMoneyBalance,B.BonusBalance,B.FreezeBalance,B.CommissionBalance,B.RedBagBalance,B.ExpertsBalance");
            //sqlBuilder_query.AppendLine("         ,M.IsSettedMobile,M.Mobile,N.IsSettedRealName,N.RealName,N.CardType,N.IdCardNumber,E.IsSettedEmail,E.Email,R.UserType,a.AlipayAccount,Q.QQ");
            //sqlBuilder_query.AppendLine("     FROM [C_User_Register] AS [R] with(nolock)");
            //sqlBuilder_query.AppendLine("     INNER JOIN [C_User_Balance] AS [B] with(nolock) ON [R].[UserId] = [B].[UserId]");
            //sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Email] AS [E] with (nolock)  ON [E].[UserId] = [R].[UserId]");
            //sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock)  ON [M].[UserId] = [R].[UserId]");
            //sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock)  ON [N].[UserId] = [R].[UserId]");
            //sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Alipay] AS [A] with (nolock) ON [A].UserId=[R].UserId");
            //sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_QQ] AS [Q] with (nolock) ON [Q].UserId=[R].UserId");
            //sqlBuilder_query.AppendLine("     WHERE 1=1 ");
            //sqlBuilder_query.AppendLine(string.Join(" ", sqlCondition.ToArray()));
            //sqlBuilder_query.AppendLine(string.Format(") AS T WHERE [RowNumber] > {0} AND [RowNumber] <= {1}", pageIndex * pageSize, (pageIndex + 1) * pageSize));
            //sqlBuilder_query.AppendLine(") tab left join (select sum(f.PayMoney) PayMoney,f.UserId from C_Withdraw w inner join C_Fund_Detail f on w.OrderId=f.OrderId where w.Status=1 and f.AccountType=20 and f.Category!='" + BusinessHelper.FundCategory_IntegralRequestWithdraw + "' group by f.UserId) wf on tab.UserId=wf.UserId order by "+strOrderBy+"");

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

            //sqlBuilder_count.AppendLine("     WHERE 1=1 ");
            sqlBuilder_count.AppendLine(string.Join(" ", sqlCondition.ToArray()));
            sqlBuilder_count.AppendLine(string.Format(") AS T"));
            sqlBuilder_count.AppendLine(") tab ");

            var sqlBuilder_query = new StringBuilder();
            sqlBuilder_query.AppendLine("--moebius_onlyread");
            sqlBuilder_query.AppendLine("select tab.[UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId,FillMoneyBalance,BonusBalance,FreezeBalance,CommissionBalance,RedBagBalance,ExpertsBalance,IsSettedMobile,Mobile,IsSettedRealName,RealName,CardType,IdCardNumber,IsSettedEmail,Email,VipLevel,UserType,AlipayAccount,QQ,OCAgentCategory,CPSBalance,CPSMode from(");
            sqlBuilder_query.AppendLine("SELECT [UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId");
            sqlBuilder_query.AppendLine("     ,FillMoneyBalance,BonusBalance,FreezeBalance,CommissionBalance,RedBagBalance,ExpertsBalance");
            sqlBuilder_query.AppendLine("     ,IsSettedMobile,Mobile,IsSettedRealName,RealName,CardType,IdCardNumber,IsSettedEmail,Email,VipLevel,UserType,AlipayAccount,QQ,OCAgentCategory,CPSBalance,CPSMode");
            sqlBuilder_query.AppendLine("FROM (");
            sqlBuilder_query.AppendLine("     SELECT ROW_NUMBER() OVER(ORDER BY " + (string.IsNullOrEmpty(strOrderBy) ? "[R].CreateTime desc" : strOrderBy) + ") AS [RowNumber],[R].[UserId],[R].[VipLevel],[R].[DisplayName],[R].[ComeFrom],[R].[RegisterIp],[R].[CreateTime],R.IsEnable,R.IsAgent,R.IsFillMoney,R.AgentId");
            sqlBuilder_query.AppendLine("         ,[B].FillMoneyBalance,B.BonusBalance,B.FreezeBalance,B.CommissionBalance,B.RedBagBalance,B.ExpertsBalance");
            sqlBuilder_query.AppendLine("         ,M.IsSettedMobile,M.Mobile,N.IsSettedRealName,N.RealName,N.CardType,N.IdCardNumber,E.IsSettedEmail,E.Email,R.UserType,a.AlipayAccount,Q.QQ,O.OCAgentCategory,B.CPSBalance,O.CPSMode");
            sqlBuilder_query.AppendLine("     FROM [C_User_Register] AS [R] with(nolock)");

            sqlBuilder_query.AppendLine("     INNER JOIN [C_User_Balance] AS [B] with(nolock) ON [R].[UserId] = [B].[UserId]");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Email] AS [E] with (nolock)  ON [E].[UserId] = [R].[UserId]");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock)  ON [M].[UserId] = [R].[UserId]");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock)  ON [N].[UserId] = [R].[UserId]");


            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Alipay] AS [A] with (nolock) ON [A].UserId=[R].UserId");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_QQ] AS [Q] with (nolock) ON [Q].UserId=[R].UserId");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [P_OCAgent] AS [O] with (nolock) on [O].UserId= r.UserId");

            //sqlBuilder_query.AppendLine("     WHERE 1=1 ");
            sqlBuilder_query.AppendLine(string.Join(" ", sqlCondition.ToArray()));
            sqlBuilder_query.AppendLine(string.Format(") AS T WHERE [RowNumber] > {0} AND [RowNumber] <= {1}", pageIndex * pageSize, (pageIndex + 1) * pageSize));
            sqlBuilder_query.AppendLine(") tab order by " + orderBy + "");



            #endregion

            var totalList = Session.CreateSQLQuery(sqlBuilder_count.ToString()).List();
            totalCount = 0;
            totalFillMoneyBalance = 0M;
            totalBonusBalance = 0M;
            totalCommissionBalance = 0M;
            totalExpertsBalance = 0M;
            totalFreezeBalance = 0M;
            totalRedBagBalance = 0M;
            totalDouDou = 0;
            totalCPSBalance = 0M;
            if (totalList.Count == 1)
            {
                var array = totalList[0] as object[];
                if (array.Length == 9 && Convert.ToInt32(array[0]) > 0)
                {
                    totalCount = int.Parse(array[0].ToString());
                    totalFillMoneyBalance = array[1] == null ? 0M : decimal.Parse(array[1].ToString());
                    totalBonusBalance = array[2] == null ? 0M : decimal.Parse(array[2].ToString());
                    totalCommissionBalance = array[3] == null ? 0M : decimal.Parse(array[3].ToString());
                    totalExpertsBalance = array[4] == null ? 0M : decimal.Parse(array[4].ToString());
                    totalFreezeBalance = array[5] == null ? 0M : decimal.Parse(array[5].ToString());
                    totalRedBagBalance = array[6] == null ? 0M : decimal.Parse(array[6].ToString());
                    totalDouDou = array[7] == null ? 0 : Convert.ToInt32(array[7]);
                    totalCPSBalance = array[8] == null ? 0M : decimal.Parse(array[8].ToString());


                    //totalCount = int.Parse(array[0].ToString());
                    //totalFillMoneyBalance = decimal.Parse(array[1].ToString());
                    //totalBonusBalance = decimal.Parse(array[2].ToString());
                    //totalCommissionBalance = 0M;
                    //totalExpertsBalance = decimal.Parse(array[4].ToString());
                    //totalFreezeBalance = decimal.Parse(array[5].ToString());
                    //totalRedBagBalance = decimal.Parse(array[6].ToString());
                }
            }
            return Session.CreateSQLQuery(sqlBuilder_query.ToString()).List();
        }

        public IList QueryUserList_AdminCPS(DateTime regFrom, DateTime regTo, string keyType, string keyValue, bool? isEnable, bool? isFillMoney, bool? isAgent
           , string commonBlance, string bonusBlance, string freezeBlance, string vipRange, string comeFrom, string agentId, int ocAgentCategory, int pageIndex, int pageSize,
           out int totalCount, out  decimal totalFillMoneyBalance, out  decimal totalBonusBalance, out  decimal totalCommissionBalance,
           out  decimal totalExpertsBalance, out  decimal totalFreezeBalance, out decimal totalRedBagBalance, out int totalDouDou, out  decimal totalCPSBalance, string strOrderBy = "")
        {
            #region 构造查询语句

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var sqlCondition = new List<string>();

            #region 条件 - 注册时间
            sqlCondition.Add(string.Format("AND R.CreateTime >= N'{0:yyyy-MM-dd}' AND R.CreateTime <= N'{1:yyyy-MM-dd}'", regFrom, regTo.AddDays(1)));
            if (!string.IsNullOrEmpty(keyValue))
            {
                sqlCondition.Add(" and R.UserType in (0,1) ");
            }
            else
            {
                sqlCondition.Add(" and R.UserType=0 ");//用户类别:0:网站普通用户；1：内部员工用户；
            }
            #endregion

            #region 条件 - 关键字
            if (!string.IsNullOrEmpty(keyValue))
            {
                if (string.IsNullOrEmpty(keyType) || keyType.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND (R.UserId = N'{0}' OR [R].[DisplayName] LIKE N'{0}%' OR M.Mobile LIKE N'{0}%' OR N.RealName LIKE N'{0}%' OR N.IdCardNumber LIKE N'{0}%' OR E.Email LIKE N'{0}%')", keyValue));
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

            #region 20151222
            //var sqlBuilder_count = new StringBuilder();
            //sqlBuilder_count.AppendLine("SELECT COUNT(1) AS TotalCount,SUM(FillMoneyBalance) as FillMoneyBalance,SUM(BonusBalance) as BonusBalance,Sum(CommissionBalance) as CommissionBalance,SUM(ExpertsBalance)as ExpertsBalance,SUM(wf.PayMoney)as FreezeBalance,SUM(RedBagBalance)as RedBagBalance,SUM(CurrentDouDou) as DouDou from(");
            //sqlBuilder_count.AppendLine("SELECT [UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId");
            //sqlBuilder_count.AppendLine("     ,FillMoneyBalance,BonusBalance,FreezeBalance,CommissionBalance,RedBagBalance,ExpertsBalance");
            //sqlBuilder_count.AppendLine("     ,IsSettedMobile,Mobile,IsSettedRealName,RealName,CardType,IdCardNumber,IsSettedEmail,Email,VipLevel,CurrentDouDou");
            //sqlBuilder_count.AppendLine("FROM (");
            //sqlBuilder_count.AppendLine("     SELECT ROW_NUMBER() OVER(ORDER BY [R].[CreateTime] DESC) AS [RowNumber],[R].[UserId],[R].[VipLevel],[R].[DisplayName],[R].[ComeFrom],[R].[RegisterIp],[R].[CreateTime],R.IsEnable,R.IsAgent,R.IsFillMoney,R.AgentId");
            //sqlBuilder_count.AppendLine("         ,[B].FillMoneyBalance,B.BonusBalance,B.FreezeBalance,B.CommissionBalance,B.RedBagBalance,B.ExpertsBalance");
            //sqlBuilder_count.AppendLine("         ,M.IsSettedMobile,M.Mobile,N.IsSettedRealName,N.RealName,N.CardType,N.IdCardNumber,E.IsSettedEmail,E.Email,CurrentDouDou");
            //sqlBuilder_count.AppendLine("     FROM [C_User_Register] AS [R] with(nolock)");
            //sqlBuilder_count.AppendLine("     INNER JOIN [C_User_Balance] AS [B] with(nolock) ON [R].[UserId] = [B].[UserId]");
            //sqlBuilder_count.AppendLine("     LEFT OUTER JOIN [E_Authentication_Email] AS [E] with (nolock)  ON [E].[UserId] = [R].[UserId]");
            //sqlBuilder_count.AppendLine("     LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock)  ON [M].[UserId] = [R].[UserId]");
            //sqlBuilder_count.AppendLine("     LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock)  ON [N].[UserId] = [R].[UserId]");
            //sqlBuilder_count.AppendLine("     WHERE 1=1 ");
            //sqlBuilder_count.AppendLine(string.Join(" ", sqlCondition.ToArray()));
            //sqlBuilder_count.AppendLine(string.Format(") AS T"));
            //sqlBuilder_count.AppendLine(") tab left join (select sum(f.PayMoney) PayMoney,f.UserId from C_Withdraw w inner join C_Fund_Detail f on w.OrderId=f.OrderId where w.Status=1 and f.AccountType=20 and f.Category!='" + BusinessHelper.FundCategory_IntegralRequestWithdraw + "' group by f.UserId) wf on tab.UserId=wf.UserId "); 

            //var sqlBuilder_query = new StringBuilder();
            //sqlBuilder_query.AppendLine("select tab.[UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId,FillMoneyBalance,BonusBalance,wf.PayMoney,CommissionBalance,RedBagBalance,ExpertsBalance,IsSettedMobile,Mobile,IsSettedRealName,RealName,CardType,IdCardNumber,IsSettedEmail,Email,VipLevel,UserType,AlipayAccount,QQ from(");
            //sqlBuilder_query.AppendLine("SELECT [UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId");
            //sqlBuilder_query.AppendLine("     ,FillMoneyBalance,BonusBalance,FreezeBalance,CommissionBalance,RedBagBalance,ExpertsBalance");
            //sqlBuilder_query.AppendLine("     ,IsSettedMobile,Mobile,IsSettedRealName,RealName,CardType,IdCardNumber,IsSettedEmail,Email,VipLevel,UserType,AlipayAccount,QQ");
            //sqlBuilder_query.AppendLine("FROM (");
            //sqlBuilder_query.AppendLine("     SELECT ROW_NUMBER() OVER(ORDER BY [R].[CreateTime] DESC) AS [RowNumber],[R].[UserId],[R].[VipLevel],[R].[DisplayName],[R].[ComeFrom],[R].[RegisterIp],[R].[CreateTime],R.IsEnable,R.IsAgent,R.IsFillMoney,R.AgentId");
            //sqlBuilder_query.AppendLine("         ,[B].FillMoneyBalance,B.BonusBalance,B.FreezeBalance,B.CommissionBalance,B.RedBagBalance,B.ExpertsBalance");
            //sqlBuilder_query.AppendLine("         ,M.IsSettedMobile,M.Mobile,N.IsSettedRealName,N.RealName,N.CardType,N.IdCardNumber,E.IsSettedEmail,E.Email,R.UserType,a.AlipayAccount,Q.QQ");
            //sqlBuilder_query.AppendLine("     FROM [C_User_Register] AS [R] with(nolock)");
            //sqlBuilder_query.AppendLine("     INNER JOIN [C_User_Balance] AS [B] with(nolock) ON [R].[UserId] = [B].[UserId]");
            //sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Email] AS [E] with (nolock)  ON [E].[UserId] = [R].[UserId]");
            //sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock)  ON [M].[UserId] = [R].[UserId]");
            //sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock)  ON [N].[UserId] = [R].[UserId]");
            //sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Alipay] AS [A] with (nolock) ON [A].UserId=[R].UserId");
            //sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_QQ] AS [Q] with (nolock) ON [Q].UserId=[R].UserId");
            //sqlBuilder_query.AppendLine("     WHERE 1=1 ");
            //sqlBuilder_query.AppendLine(string.Join(" ", sqlCondition.ToArray()));
            //sqlBuilder_query.AppendLine(string.Format(") AS T WHERE [RowNumber] > {0} AND [RowNumber] <= {1}", pageIndex * pageSize, (pageIndex + 1) * pageSize));
            //sqlBuilder_query.AppendLine(") tab left join (select sum(f.PayMoney) PayMoney,f.UserId from C_Withdraw w inner join C_Fund_Detail f on w.OrderId=f.OrderId where w.Status=1 and f.AccountType=20 and f.Category!='" + BusinessHelper.FundCategory_IntegralRequestWithdraw + "' group by f.UserId) wf on tab.UserId=wf.UserId order by "+strOrderBy+"");

            #endregion

            var sqlBuilder_count = new StringBuilder();
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

            sqlBuilder_count.AppendLine("     WHERE 1=1 ");
            sqlBuilder_count.AppendLine(string.Join(" ", sqlCondition.ToArray()));
            sqlBuilder_count.AppendLine(string.Format(") AS T"));
            sqlBuilder_count.AppendLine(") tab ");

            var sqlBuilder_query = new StringBuilder();
            sqlBuilder_query.AppendLine("select tab.[UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId,FillMoneyBalance,BonusBalance,FreezeBalance,CommissionBalance,RedBagBalance,ExpertsBalance,IsSettedMobile,Mobile,IsSettedRealName,RealName,CardType,IdCardNumber,IsSettedEmail,Email,VipLevel,UserType,AlipayAccount,QQ,OCAgentCategory,CPSBalance,CPSMode,ChannelName from(");
            sqlBuilder_query.AppendLine("SELECT [UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId");
            sqlBuilder_query.AppendLine("     ,FillMoneyBalance,BonusBalance,FreezeBalance,CommissionBalance,RedBagBalance,ExpertsBalance");
            sqlBuilder_query.AppendLine("     ,IsSettedMobile,Mobile,IsSettedRealName,RealName,CardType,IdCardNumber,IsSettedEmail,Email,VipLevel,UserType,AlipayAccount,QQ,OCAgentCategory,CPSBalance,CPSMode,ChannelName");
            sqlBuilder_query.AppendLine("FROM (");
            sqlBuilder_query.AppendLine("     SELECT ROW_NUMBER() OVER(ORDER BY " + (string.IsNullOrEmpty(strOrderBy) ? "[R].CreateTime desc" : strOrderBy) + ") AS [RowNumber],[R].[UserId],[R].[VipLevel],[R].[DisplayName],[R].[ComeFrom],[R].[RegisterIp],[R].[CreateTime],R.IsEnable,R.IsAgent,R.IsFillMoney,R.AgentId");
            sqlBuilder_query.AppendLine("         ,[B].FillMoneyBalance,B.BonusBalance,B.FreezeBalance,B.CommissionBalance,B.RedBagBalance,B.ExpertsBalance");
            sqlBuilder_query.AppendLine("         ,M.IsSettedMobile,M.Mobile,N.IsSettedRealName,N.RealName,N.CardType,N.IdCardNumber,E.IsSettedEmail,E.Email,R.UserType,a.AlipayAccount,Q.QQ,O.OCAgentCategory,B.CPSBalance,O.CPSMode,O.ChannelName");
            sqlBuilder_query.AppendLine("     FROM [C_User_Register] AS [R] with(nolock)");

            sqlBuilder_query.AppendLine("     INNER JOIN [C_User_Balance] AS [B] with(nolock) ON [R].[UserId] = [B].[UserId]");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Email] AS [E] with (nolock)  ON [E].[UserId] = [R].[UserId]");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock)  ON [M].[UserId] = [R].[UserId]");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock)  ON [N].[UserId] = [R].[UserId]");

            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Alipay] AS [A] with (nolock) ON [A].UserId=[R].UserId");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_QQ] AS [Q] with (nolock) ON [Q].UserId=[R].UserId");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [P_OCAgent] AS [O] with (nolock) on [O].UserId= r.UserId");
            sqlBuilder_query.AppendLine("     WHERE 1=1 ");
            sqlBuilder_query.AppendLine(string.Join(" ", sqlCondition.ToArray()));
            if (ocAgentCategory != -1)
            {
                sqlBuilder_query.AppendLine(string.Format("AND O.OCAgentCategory = {0} ", ocAgentCategory));
            }
            sqlBuilder_query.AppendLine(string.Format(") AS T WHERE [RowNumber] > {0} AND [RowNumber] <= {1}", pageIndex * pageSize, (pageIndex + 1) * pageSize));
            sqlBuilder_query.AppendLine(") tab order by " + orderBy + "");



            #endregion

            var totalList = Session.CreateSQLQuery(sqlBuilder_count.ToString()).List();
            totalCount = 0;
            totalFillMoneyBalance = 0M;
            totalBonusBalance = 0M;
            totalCommissionBalance = 0M;
            totalExpertsBalance = 0M;
            totalFreezeBalance = 0M;
            totalRedBagBalance = 0M;
            totalDouDou = 0;
            totalCPSBalance = 0M;
            if (totalList.Count == 1)
            {
                var array = totalList[0] as object[];
                if (array.Length == 9 && Convert.ToInt32(array[0]) > 0)
                {
                    totalCount = int.Parse(array[0].ToString());
                    totalFillMoneyBalance = array[1] == null ? 0M : decimal.Parse(array[1].ToString());
                    totalBonusBalance = array[2] == null ? 0M : decimal.Parse(array[2].ToString());
                    totalCommissionBalance = array[3] == null ? 0M : decimal.Parse(array[3].ToString());
                    totalExpertsBalance = array[4] == null ? 0M : decimal.Parse(array[4].ToString());
                    totalFreezeBalance = array[5] == null ? 0M : decimal.Parse(array[5].ToString());
                    totalRedBagBalance = array[6] == null ? 0M : decimal.Parse(array[6].ToString());
                    totalDouDou = array[7] == null ? 0 : Convert.ToInt32(array[7]);
                    totalCPSBalance = array[8] == null ? 0M : decimal.Parse(array[8].ToString());


                    //totalCount = int.Parse(array[0].ToString());
                    //totalFillMoneyBalance = decimal.Parse(array[1].ToString());
                    //totalBonusBalance = decimal.Parse(array[2].ToString());
                    //totalCommissionBalance = 0M;
                    //totalExpertsBalance = decimal.Parse(array[4].ToString());
                    //totalFreezeBalance = decimal.Parse(array[5].ToString());
                    //totalRedBagBalance = decimal.Parse(array[6].ToString());
                }
            }
            return Session.CreateSQLQuery(sqlBuilder_query.ToString()).List();
        }


        public IList QueryUserList_old(string mobile, string realName, string displayName, string email, string idCardNumber,
            string comeFrom, int isVip, DateTime regFrom, DateTime regTo, int status, int isFillMoney, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var sql = @"SELECT [R].[UserId],[R].[UserKey],[R].[DisplayName],[R].[ComeFrom],[R].[RegisterIp],[A].[Status],[A].[CreateTime],[B].EnabledBalance,B.DisabledBalance,[VC].[EnableBalance] + [VC].[DisableBalance] AS [VcBalance],[V].[VipLevel] 
                    FROM [C_User_Register] AS [R] with (nolock) INNER JOIN [C_Auth_Users] AS [A] with (nolock)  ON [R].[UserId] = [A].[UserId] INNER JOIN [C_User_Balance] AS [B] with (nolock)  ON [R].[UserId] = [B].[UserId] INNER JOIN [E_VirtualCoin_Account_List] AS [VC] with (nolock)  ON [VC].[UserId] = [B].[UserId] LEFT OUTER JOIN [E_Vip_Simple_Validity] AS [V] with (nolock)  ON [R].[UserId] = [V].[UserId] AND GETDATE() BETWEEN [V].[ValidityFrom] AND DATEADD(DAY, 1, [V].[ValidityTo]) LEFT OUTER JOIN [E_Authentication_Email] AS [E] with (nolock)  ON [E].[UserId] = [A].[UserId]  LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock)  ON [M].[UserId] = [A].[UserId]  LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock)  ON [N].[UserId] = [A].[UserId] WHERE ";
            var sqlCondition = new List<string>();
            sqlCondition.Add(string.Format("([A].[CreateTime] BETWEEN '{0}' AND DATEADD(DAY, 1, '{1}'))", regFrom, regTo));

            if (!string.IsNullOrEmpty(mobile))
                sqlCondition.Add(string.Format("[M].[Mobile] LIKE '{0}%'", mobile));
            if (!string.IsNullOrEmpty(realName))
                sqlCondition.Add(string.Format("[N].[RealName] LIKE '{0}%'", realName));
            if (!string.IsNullOrEmpty(displayName))
                sqlCondition.Add(string.Format("[R].[DisplayName] LIKE '{0}%'", displayName));
            if (!string.IsNullOrEmpty(email))
                sqlCondition.Add(string.Format("[E].[Email] LIKE '{0}%'", email));
            if (!string.IsNullOrEmpty(idCardNumber))
                sqlCondition.Add(string.Format("[N].[IdCardNumber] LIKE '{0}%'", idCardNumber));
            if (!string.IsNullOrEmpty(comeFrom))
                sqlCondition.Add(string.Format("[R].[ComeFrom] = '{0}'", comeFrom));
            if (isVip != 0)
                sqlCondition.Add(string.Format("({0} = 1 AND [V].[VipLevel] IS NOT NULL) OR ({0} = -1 AND [V].[VipLevel] IS NULL)", isVip));
            if (status != 0)
                sqlCondition.Add(string.Format("[A].[Status] = {0}", status));
            if (isFillMoney != 0)
                sqlCondition.Add(string.Format("({0}=1 AND R.IsFillMoney=1) OR ({0}=-1 AND R.IsFillMoney=0)", isFillMoney));
            sql += string.Join(" AND ", sqlCondition.ToArray());

            sql += " ORDER BY A.CreateTime DESC";

            var query1 = CreateOutputQuery(Session.GetNamedQuery("P_Core_Pager"));
            query1 = query1.AddInParameter("sqlStr", sql);
            query1 = query1.AddInParameter("currentPageIndex", pageIndex);
            query1 = query1.AddInParameter("pageSize", pageSize);
            var result = query1.ToListByPaging(out totalCount);

            return result;


            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_QueryUserList"));
            query = query.AddInParameter("Mobile", mobile);
            query = query.AddInParameter("RealName", realName);
            query = query.AddInParameter("DisplayName", displayName);
            query = query.AddInParameter("Email", email);
            query = query.AddInParameter("IdCardNumber", idCardNumber);
            query = query.AddInParameter("ComeFrom", comeFrom);
            query = query.AddInParameter("IsVip", isVip);
            query = query.AddInParameter("RegFrom", regFrom);
            query = query.AddInParameter("RegTo", regTo);
            query = query.AddInParameter("Status", status);
            query = query.AddInParameter("IsFillMoney", isFillMoney);
            query = query.AddInParameter("PageIndex", pageIndex);
            query = query.AddInParameter("PageSize", pageSize);
            query = query.AddOutParameter("TotalCount", "Int32");
            var list = query.List(out outputs);
            totalCount = (int)outputs["TotalCount"];
            return list;
        }
        public IList QueryBackgroundManagerList(string key, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_QueryBackgroundManagerList"));
            query = query.AddInParameter("Key", key);
            query = query.AddInParameter("PageIndex", pageIndex);
            query = query.AddInParameter("PageSize", pageSize);
            query = query.AddOutParameter("TotalCount", "Int32");
            var list = query.List(out outputs);
            totalCount = (int)outputs["TotalCount"];
            return list;
        }

        /// <summary>
        /// 红人列表
        /// </summary>
        /// <returns></returns>
        public IList QueryTogetherHotUserList(DateTime createFrom, DateTime createTo, string keyType, string keyValue, int pageIndex, int pageSize, out int totalCount)
        {
            #region 构造查询语句

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var sqlCondition = new List<string>();
            #region 条件 - 开通时间
            sqlCondition.Add(string.Format("AND T.CreateTime >=N'{0:yyyy-MM-dd}' AND T.CreateTime <=N'{1:yyyy-MM-dd}'", createFrom, createTo.AddDays(1)));
            #endregion

            #region 条件 - 关键字
            if (!string.IsNullOrEmpty(keyValue))
            {
                if (string.IsNullOrEmpty(keyType) || keyType.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND (R.UserId = N'{0}' OR [R].[DisplayName] LIKE N'{0}%' OR M.Mobile LIKE N'{0}%' OR N.RealName LIKE N'{0}%' OR N.IdCardNumber LIKE N'{0}%' OR E.Email LIKE N'{0}%')", keyValue));
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
            }
            #endregion

            var sqlBuilder_count = new StringBuilder();
            sqlBuilder_count.AppendLine("SELECT  COUNT(1) AS TotalCount,SUM(b.FillMoneyBalance) as FillMoneyBalance,SUM(b.BonusBalance) as BonusBalance,Sum(b.CommissionBalance) as CommissionBalance,SUM(ExpertsBalance)as ExpertsBalance,SUM(FreezeBalance)as FreezeBalance,SUM(RedBagBalance)as RedBagBalance FROM [C_TogetherHotUser] AS [T] with (nolock)");
            sqlBuilder_count.AppendLine("INNER JOIN [C_User_Balance] AS [B] with(nolock) ON [B].[UserId] = [T].[UserId]");
            sqlBuilder_count.AppendLine("LEFT OUTER JOIN [C_User_Register] AS [R] with (nolock) ON [R].[UserId] = [T].[UserId] ");
            sqlBuilder_count.AppendLine("LEFT OUTER JOIN [E_Authentication_Email] AS [E] with (nolock) ON [E].[UserId] = [T].[UserId] ");
            sqlBuilder_count.AppendLine("LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock) ON [M].[UserId] = [T].[UserId] ");
            sqlBuilder_count.AppendLine("LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock) ON [N].[UserId] = [T].[UserId] ");
            sqlBuilder_count.AppendLine("WHERE 1 = 1");
            sqlBuilder_count.AppendLine(string.Join(" ", sqlCondition.ToArray()));


            var sqlBuilder_query = new StringBuilder();
            sqlBuilder_query.AppendLine("SELECT t.[UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],[VipLevel],IsSettedMobile,Mobile,");
            sqlBuilder_query.AppendLine("IsSettedRealName,RealName,CardType,IdCardNumber,AgentId,HotRegTime,IsAgent,IsEnable");
            sqlBuilder_query.AppendLine("FROM (SELECT ROW_NUMBER() OVER(ORDER BY [R].[CreateTime] DESC) AS [RowNumber],[R].[UserId],[R].[DisplayName],[R].[ComeFrom],[R].[RegisterIp],[R].[CreateTime],[R].[VipLevel],[M].IsSettedMobile,[M].Mobile,N.IsSettedRealName,N.RealName,N.CardType,N.IdCardNumber,R.AgentId,T.CreateTime as'HotRegTime',R.IsAgent,R.IsEnable");
            sqlBuilder_query.AppendLine("FROM [C_TogetherHotUser]  AS [T] with (nolock)");
            sqlBuilder_query.AppendLine("INNER JOIN[C_User_Register]as [R] ON [R].[UserId] = [T].[UserId]");
            sqlBuilder_query.AppendLine("LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock)  ON [M].[UserId] = [T].[UserId]");
            sqlBuilder_query.AppendLine("LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock)  ON [N].[UserId] = [T].[UserId]");
            sqlBuilder_query.AppendLine("WHERE 1 = 1");
            sqlBuilder_query.AppendLine(string.Join(" ", sqlCondition.ToArray()));
            sqlBuilder_query.AppendLine(")AS T ");
            sqlBuilder_query.AppendLine(string.Format(" WHERE [RowNumber] > {0} AND [RowNumber] <= {1}", pageIndex * pageSize, (pageIndex + 1) * pageSize));

            #endregion

            var totalList = Session.CreateSQLQuery(sqlBuilder_count.ToString())
               .List();
            totalCount = 0;

            if (totalList.Count == 1)
            {
                var array = totalList[0] as object[];
                if (array.Length == 7 && Convert.ToInt32(array[0]) > 0)
                {
                    totalCount = int.Parse(array[0].ToString());
                }
            }
            return Session.CreateSQLQuery(sqlBuilder_query.ToString()).List();
        }
        public bool IsFillMoney(string userId, DateTime time)
        {
            var query = Session.Query<UserRegister>().FirstOrDefault(s => s.UserId == userId && s.CreateTime >= time);
            if (query != null && !string.IsNullOrEmpty(query.UserId))
                return false;
            return true;
        }
        //根据手机号查询用户编号
        public UserMobile_Collection QueryUserIDByMobile(string arrayMobile)
        {
            Session.Clear();
            var arrMobil = new string[] { };
            if (!string.IsNullOrEmpty(arrayMobile))
                arrMobil = arrayMobile.Split(',');
            string strSql = "select UserId,Mobile from E_Authentication_Mobile where Mobile in (:mobileList)";
            var result = Session.CreateSQLQuery(strSql).SetParameterList("mobileList", arrMobil).List();
            UserMobile_Collection moblieList = new UserMobile_Collection();
            if (result != null)
            {
                foreach (var item in result)
                {
                    UserMobileInfo info = new UserMobileInfo();
                    var array = item as object[];
                    info.UserId = array[0] == null ? string.Empty : array[0].ToString();
                    info.Mobile = array[1] == null ? string.Empty : array[1].ToString();
                    moblieList.MobileList.Add(info);
                }
            }
            return moblieList;
        }

        public UserBindInfos QueryUserBindInfos(string userId)
        {
            Session.Clear();
            var sql = string.Format(@"select r.UserId,r.VipLevel,r.DisplayName,r.ComeFrom,r.IsFillMoney,r.IsEnable,r.IsAgent,r.HideDisplayNameCount,
                                            b.RealName BankCardRealName,b.ProvinceName,b.CityName,b.BankName,b.BankSubName,b.BankCardNumber,
                                            e.Email,m.Mobile,n.RealName,n.CardType,n.IdCardNumber,
                                            --h.LoginFrom LastLoginFrom,h.LoginIp LastLoginIp,h.IpDisplayName LastLoginIpName,h.LoginTime LastLoginTime,
                                            a.c RebateCount,
                                            p.MaxLevelValue,p.MaxLevelName,p.WinOneHundredCount,p.WinOneThousandCount,p.WinTenThousandCount,p.WinOneHundredThousandCount,p.WinOneMillionCount,p.WinTenMillionCount,p.WinHundredMillionCount,p.TotalBonusMoney,q.QQ,al.AlipayAccount
                                            ,m.IsSettedMobile,r.userType

                                            from C_User_Register r
                                            left join C_BankCard b on r.userid=b.userid
                                            left join E_Authentication_Email e on r.userid=e.userid
                                            left join E_Authentication_Mobile m on r.userid=m.userid
                                            left join E_Authentication_RealName n on r.userid=n.userid
                                            left join E_Authentication_QQ q on r.userid=q.userid
                                            left join E_Authentication_Alipay al on r.userid=al.userid
                                           
                                            --left join (SELECT top 1 UserId,LoginFrom,LoginIp,IpDisplayName,LoginTime
                                   --                FROM [E_Blog_UserLoginHistory]
                                   --                where userid='{0}'
                                   --                order by LoginTime desc) as h on r.userid=h.userid

                                            left join (SELECT UserId, count(1) c
                                                   FROM  [P_OCAgent_Rebate]
                                                where UserId='{0}'
                                                   group by UserId)as a on r.userid=a.userid
                                            left join [E_Blog_ProfileBonusLevel] p on r.userid=p.userid
                                            where r.userid='{0}' ", userId);

            var array = this.Session.CreateSQLQuery(sql).List();

            var info = new UserBindInfos();
            if (array == null)
                return info;
            foreach (var item in array)
            {
                var row = item as object[];
                if (row.Length == 34)
                {
                    info = new UserBindInfos
                    {
                        UserId = UsefullHelper.GetDbValue<string>(row[0]),
                        VipLevel = UsefullHelper.GetDbValue<int>(row[1]),
                        DisplayName = UsefullHelper.GetDbValue<string>(row[2]),
                        ComeFrom = UsefullHelper.GetDbValue<string>(row[3]),
                        IsFillMoney = UsefullHelper.GetDbValue<bool>(row[4]),
                        IsEnable = UsefullHelper.GetDbValue<bool>(row[5]),
                        IsAgent = UsefullHelper.GetDbValue<bool>(row[6]),
                        HideDisplayNameCount = UsefullHelper.GetDbValue<int>(row[7]),
                        BankCardRealName = UsefullHelper.GetDbValue<string>(row[8]),
                        ProvinceName = UsefullHelper.GetDbValue<string>(row[9]),
                        CityName = UsefullHelper.GetDbValue<string>(row[10]),
                        BankName = UsefullHelper.GetDbValue<string>(row[11]),
                        BankSubName = UsefullHelper.GetDbValue<string>(row[12]),
                        BankCardNumber = UsefullHelper.GetDbValue<string>(row[13]),
                        Email = UsefullHelper.GetDbValue<string>(row[14]),
                        Mobile = UsefullHelper.GetDbValue<string>(row[15]),
                        RealName = UsefullHelper.GetDbValue<string>(row[16]),
                        CardType = UsefullHelper.GetDbValue<string>(row[17]),
                        IdCardNumber = UsefullHelper.GetDbValue<string>(row[18]),

                        //LastLoginFrom = UsefullHelper.GetDbValue<string>(row[19]),
                        //LastLoginIp = UsefullHelper.GetDbValue<string>(row[20]),
                        //LastLoginIpName = UsefullHelper.GetDbValue<string>(row[21]),
                        //LastLoginTime = UsefullHelper.GetDbValue<DateTime>(row[22]).ToString("yyyy-MM-dd HH:mm:ss"),

                        RebateCount = UsefullHelper.GetDbValue<int>(row[19]),
                        MaxLevelValue = UsefullHelper.GetDbValue<int>(row[20]),
                        MaxLevelName = UsefullHelper.GetDbValue<string>(row[21]),
                        WinOneHundredCount = UsefullHelper.GetDbValue<int>(row[22]),
                        WinOneThousandCount = UsefullHelper.GetDbValue<int>(row[23]),
                        WinTenThousandCount = UsefullHelper.GetDbValue<int>(row[24]),
                        WinOneHundredThousandCount = UsefullHelper.GetDbValue<int>(row[25]),
                        WinOneMillionCount = UsefullHelper.GetDbValue<int>(row[26]),
                        WinTenMillionCount = UsefullHelper.GetDbValue<int>(row[27]),
                        WinHundredMillionCount = UsefullHelper.GetDbValue<int>(row[28]),
                        TotalBonusMoney = UsefullHelper.GetDbValue<decimal>(row[29]),
                        QQ = UsefullHelper.GetDbValue<string>(row[30]),
                        AlipayAccount = UsefullHelper.GetDbValue<string>(row[31]),
                        IsSettedMobile = UsefullHelper.GetDbValue<bool>(row[32]),
                        IsUserType = UsefullHelper.GetDbValue<int>(row[33]),
                        LoadDateTime = DateTime.Now,
                    };
                }
                //没有去解决一个用户 由于不明原因绑定了多个卡 此处根据前台后台当前逻辑 都是默认读取第一条数据 所以读取完了第一条直接break 
                // 这样redis中就会保存用户和后台显示一样的卡号
                break;
            }

            return info;
        }

    }
}
