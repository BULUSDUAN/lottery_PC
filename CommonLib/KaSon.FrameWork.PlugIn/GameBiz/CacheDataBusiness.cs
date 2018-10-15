using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using KaSon.FrameWork.Common.Net;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.ORM.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace KaSon.FrameWork.PlugIn.GameBiz
{
    public class CacheDataBusiness :DBbase, ICreateTogether_AfterTranCommit   // 创建合买，更新超级发起人状态、最新动态
        , IBettingSport_AfterTranCommit     // 购彩投注，更新最新动态
        , IBettingLottery_AfterTranCommit   // 购彩投注，更新最新动态
        , IJoinTogether_AfterTranCommit     // 参与合买，更新最新动态
        , IUser_AfterLogin                  // 用户登录，记录登录历史
        , IOrderPrize_AfterTranCommit       // 订单派奖，记录奖金级别统计、最新动态
        , ITogetherFollow_AfterTranCommit   // 定制跟单，更新定制跟单数据
        , IExistTogetherFollow_AfterTranCommit   // 撤销定制跟单，更新定制跟单数据
        , IAttention_AfterTranCommit        // 用户关注，更新关注信息
        , ICancelAttention_AfterTranCommit        // 取消用户关注，更新关注信息
        , IChangeHideDisplayNameCount_AfterTranCommit   // 修改用户显示名称位数，更新用户缓存
    {

        public object ExecPlugin(string type, object inputParam)
        {
            UsefullHelper.TryDoAction(() =>
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "ICreateTogether_AfterTranCommit":
                        CreateTogether_AfterTranCommit((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3], (string)paraList[4], (decimal)paraList[5], (DateTime)paraList[6]);
                        break;
                    case "IBettingSport_AfterTranCommit":
                        BettingSport_AfterTranCommit((string)paraList[0], (Sports_BetingInfo)paraList[1], (string)paraList[2]);
                        break;
                    case "IBettingLottery_AfterTranCommit":
                        BettingLottery_AfterTranCommit((string)paraList[0], (LotteryBettingInfo)paraList[1], (string)paraList[2], (string)paraList[3]);
                        break;
                    case "IJoinTogether_AfterTranCommit":
                        JoinTogether_AfterTranCommit((string)paraList[0], (string)paraList[1], (int)paraList[2], (string)paraList[3], (string)paraList[4], (string)paraList[5], (decimal)paraList[6], (TogetherSchemeProgress)paraList[7]);
                        break;
                    case "IUser_AfterLogin":
                        User_AfterLogin((string)paraList[0], (string)paraList[1], (string)paraList[2], (DateTime)paraList[3]);
                        break;
                    case "IOrderPrize_AfterTranCommit":
                        OrderPrize_AfterTranCommit((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3], (string)paraList[4], (decimal)paraList[5], (bool)paraList[6], (decimal)paraList[7], (decimal)paraList[8], (bool)paraList[9], (DateTime)paraList[10]);
                        break;
                    case "ITogetherFollow_AfterTranCommit":
                        TogetherFollow_AfterTranCommit((TogetherFollowerRuleInfo)paraList[0]);
                        break;
                    case "IExistTogetherFollow_AfterTranCommit":
                        ExistTogetherFollow_AfterTranCommit((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3]);
                        break;
                    case "IAttention_AfterTranCommit":
                        Attention_AfterTranCommit((string)paraList[0], (string)paraList[1]);
                        break;
                    case "ICancelAttention_AfterTranCommit":
                        CancelAttention_AfterTranCommit((string)paraList[0], (string)paraList[1]);
                        break;
                    case "IChangeHideDisplayNameCount_AfterTranCommit":
                        ChangeHideDisplayNameCount_AfterTranCommit((string)paraList[0], (int)paraList[1]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
                }
            });
            return null;
        }

        public void User_AfterLogin(string userId, string loginFrom, string loginIp, DateTime loginTime)
        {
            new Thread(() =>
            {
                // 更新用户登录日志，忽略异常，异常只记录日志
                UsefullHelper.TryDoAction(() => UpdateUserLoginHistory(userId, loginFrom, loginIp, loginTime));
            }).Start();
        }

        public void UpdateUserLoginHistory(string userId, string loginFrom, string loginIp, DateTime loginTime)
        {
            #region "20180331 增加"
            List<UserLoginHistoryInfo> arr = new BlogManager().QueryBlog_UserLoginHistory(userId);//默认取最新的10条出来
            if (arr != null && arr.Count > 0)
            {
                foreach (var item in arr)
                {
                    //登录日志记录太多 每天同一个ip只记录一条即可 
                    if (loginTime.ToString("yyyy-MM-dd") == item.LoginTime.ToString("yyyy-MM-dd") && item.LoginIp == loginIp)
                    {
                        //更新最新一次登陆时间
                        new BlogManager().UpdateBlog_UserLoginHistory(new E_Blog_UserLoginHistory { LoginIp = loginIp, LoginTime = loginTime, LoginFrom = loginFrom, IpDisplayName = item.IpDisplayName, UserId = userId, Id = item.Id });
                        return;
                    }
                }
            }
            #endregion
            string IpDisplayName = "未知";
            try
            {
                IpDisplayName = IpManager.GetIpDisplayname_Sina(loginIp).ToString();
            }
            catch
            {
            }
            new BlogManager().AddBlog_UserLoginHistory(new E_Blog_UserLoginHistory { LoginIp = loginIp, LoginTime = loginTime, LoginFrom = loginFrom, IpDisplayName = IpDisplayName, UserId = userId });

            //List<UserLoginHistoryInfo> arr = new BlogManager().QueryBlog_UserLoginHistory(userId);
        }

        public void CreateTogether_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal totalMoney, DateTime stopTime)
        {
            // 更新超级发起人状态，忽略异常，异常只记录日志
            //UsefullHelper.TryDoAction(() => UpdateSupperTogetherScheme(userId, stopTime));
            // 更新最新动态，写入数据库
            UsefullHelper.TryDoAction(() => UpdateProfileDynamic(userId, schemeId, gameCode, issuseNumber, totalMoney, "发起合买"));
            // 更新统计数据
            UsefullHelper.TryDoAction(() => UpdateProfileDataReport(userId, 1, null, null, null));
            //普通用户推广
            UsefullHelper.TryDoAction(() => UpdateSporeadUserData(userId, gameCode, totalMoney));
            //分享推广活动 用户购彩了 送红包 只送一次
            //UsefullHelper.TryDoAction(() => FirstLotteryGiveRedBag(userId));
        }

        public void BettingSport_AfterTranCommit(string userId, Sports_BetingInfo bettingOrder, string schemeId)
        {
            // 更新最新动态，忽略异常，异常只记录日志
            var desc = string.Format("代购了 {0}第{1}期￥{2:N0} 投注方案", GetGameName(bettingOrder.GameCode, bettingOrder.GameType), bettingOrder.IssuseNumber, bettingOrder.TotalMoney);
            UsefullHelper.TryDoAction(() => UpdateProfileDynamic(userId, schemeId, bettingOrder.GameCode, bettingOrder.IssuseNumber, bettingOrder.TotalMoney, "代购"));
            // 更新统计数据
            UsefullHelper.TryDoAction(() => UpdateProfileDataReport(userId, 1, null, null, null));
            //普通用户推广
            UsefullHelper.TryDoAction(() => UpdateSporeadUserData(userId, bettingOrder.GameCode, bettingOrder.TotalMoney));
            //分享推广活动 用户购彩了 送红包 只送一次
            //UsefullHelper.TryDoAction(() => FirstLotteryGiveRedBag(userId));
        }

        public void BettingLottery_AfterTranCommit(string userId, LotteryBettingInfo bettingOrder, string schemeId, string keyLine)
        {
            // 更新最新动态，忽略异常，异常只记录日志
            if (bettingOrder.IssuseNumberList.Count > 1)
            {
                UsefullHelper.TryDoAction(() => UpdateProfileDynamic(userId, schemeId, bettingOrder.GameCode, bettingOrder.IssuseNumberList[0].IssuseNumber, bettingOrder.TotalMoney, "代购"));
            }
            else
            {
                UsefullHelper.TryDoAction(() => UpdateProfileDynamic(userId, schemeId, bettingOrder.GameCode, bettingOrder.IssuseNumberList[0].IssuseNumber, bettingOrder.TotalMoney, "代购"));
            }
            // 更新统计数据
            UsefullHelper.TryDoAction(() => UpdateProfileDataReport(userId, 1, null, null, null));
            //普通用户推广
            UsefullHelper.TryDoAction(() => UpdateSporeadUserData(userId, bettingOrder.GameCode, bettingOrder.TotalMoney));
            //分享推广活动 用户购彩了 送红包 只送一次
            //UsefullHelper.TryDoAction(() => FirstLotteryGiveRedBag(userId));
        }

        public void JoinTogether_AfterTranCommit(string userId, string schemeId, int buyCount, string gameCode, string gameType, string issuseNumber, decimal totalMoney, TogetherSchemeProgress progress)
        {
            // 更新最新动态，忽略异常，异常只记录日志
            var desc = string.Format("参与了 {0}第{1}期￥{2:N0}的合买方案 参与{3}元", GetGameName(gameCode, gameType), issuseNumber, totalMoney, buyCount);
            UsefullHelper.TryDoAction(() => UpdateProfileDynamic(userId, schemeId, gameCode, issuseNumber, buyCount, "参与合买"));
            // 更新统计数据
            UsefullHelper.TryDoAction(() => UpdateProfileDataReport(userId, null, 1, null, null));
            //普通用户推广
            UsefullHelper.TryDoAction(() => UpdateSporeadUserData(userId, gameCode, totalMoney));
            //分享推广活动 用户购彩了 送红包 只送一次
            //UsefullHelper.TryDoAction(() => FirstLotteryGiveRedBag(userId));
        }

        public void OrderPrize_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal orderMoney, bool isBonus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney, bool isVirtualOrder, DateTime prizeTime)
        {
            if (isBonus)
            {
                // 更新用户获奖级别统计，忽略异常，异常只记录日志
                UsefullHelper.TryDoAction(() => UpdateProfileBonusLevel(userId, afterTaxBonusMoney));
                // 更新最新动态，忽略异常，异常只记录日志
                UsefullHelper.TryDoAction(() => UpdateProfileDynamic(userId, schemeId, gameCode, issuseNumber, orderMoney, string.Format("用户订单：{0}中奖", schemeId)));
                // 更新最新中奖，忽略异常，异常只记录日志
                UsefullHelper.TryDoAction(() => AddProfileLastBonus(userId, schemeId, gameCode, gameType, issuseNumber, afterTaxBonusMoney, prizeTime));
                // 更新历史战绩，忽略异常，异常只记录日志
                UsefullHelper.TryDoAction(() => UpdateProfileBeedings(userId, gameCode, gameType, orderMoney, afterTaxBonusMoney, isVirtualOrder, prizeTime));
                // 更新统计数据
                UsefullHelper.TryDoAction(() => UpdateProfileDataReport(userId, null, null, 1, afterTaxBonusMoney));
            }
        }
        public void TogetherFollow_AfterTranCommit(TogetherFollowerRuleInfo info)
        {
            UsefullHelper.TryDoAction(() => UpdateProfileFollower_Active(info.CreaterUserId, info.FollowerUserId, info.GameCode, info.GameType, 1));
            UsefullHelper.TryDoAction(() => UpdateProfileFollower_Passive(info.CreaterUserId, info.FollowerUserId, info.GameCode, info.GameType, 1));
        }
        public void ExistTogetherFollow_AfterTranCommit(string creatorUserId, string followUserId, string gameCode, string gameType)
        {
            UsefullHelper.TryDoAction(() => UpdateProfileFollower_Active(creatorUserId, followUserId, gameCode, gameType, -1));
            UsefullHelper.TryDoAction(() => UpdateProfileFollower_Passive(creatorUserId, followUserId, gameCode, gameType, -1));
        }
        public void Attention_AfterTranCommit(string activeUserId, string passiveUserId)
        {
            UsefullHelper.TryDoAction(() => UpdateProfileAttention_Active(activeUserId, passiveUserId, 1));
            UsefullHelper.TryDoAction(() => UpdateProfileAttention_Passive(activeUserId, passiveUserId, 1));
            UsefullHelper.TryDoAction(() => UpdateProfileUserInfo(activeUserId, null, null, null, 1, null, null));
            UsefullHelper.TryDoAction(() => UpdateProfileUserInfo(passiveUserId, null, null, null, null, 1, null));
        }
        public void CancelAttention_AfterTranCommit(string activeUserId, string passiveUserId)
        {
            UsefullHelper.TryDoAction(() => UpdateProfileAttention_Active(activeUserId, passiveUserId, -1));
            UsefullHelper.TryDoAction(() => UpdateProfileAttention_Passive(activeUserId, passiveUserId, -1));
            UsefullHelper.TryDoAction(() => UpdateProfileUserInfo(activeUserId, null, null, null, -1, null, null));
            UsefullHelper.TryDoAction(() => UpdateProfileUserInfo(passiveUserId, null, null, null, null, -1, null));
        }
        public void ChangeHideDisplayNameCount_AfterTranCommit(string userId, int hideDisplayNameCount)
        {
            UsefullHelper.TryDoAction(() => UpdateProfileUserInfo(userId, null, hideDisplayNameCount, null, null, null, null));
        }
        public string GetGameName(string gamecode, string type = "")
        {
            if (string.IsNullOrEmpty(gamecode))
            {
                return "";
            }
            type = string.IsNullOrEmpty(type) ? gamecode : type;
            //根据彩种编号获取彩种名称
            switch (gamecode.ToLower())
            {
                case "cqssc": return "时时彩";
                case "jxssc": return "新时时彩";
                case "sd11x5": return "老11选5";
                case "gd11x5": return "新11选5";
                case "jx11x5": return "11选5";
                case "pl3": return "排列3";
                case "fc3d": return "福彩3D";
                case "ssq": return "双色球";
                case "qxc": return "七星彩";
                case "qlc": return "七乐彩";
                case "dlt": return "大乐透";
                case "sdqyh": return "群英会";
                case "gdklsf": return "快乐十分";
                case "gxklsf": return "广西快乐十分";
                case "jsks": return "江苏快3";
                case "jczq":
                    switch (type.ToLower())
                    {
                        case "spf": return "竞彩让球胜平负";
                        case "brqspf": return "竞彩胜平负";
                        case "bf": return "竞彩比分";
                        case "zjq": return "竞彩总进球数";
                        case "bqc": return "竞彩半全场";
                        default: return "竞彩足球";
                    }
                case "jclq":
                    switch (type.ToLower())
                    {
                        case "sf": return "篮球胜负";
                        case "rfsf": return "篮球让分胜负";
                        case "sfc": return "篮球胜分差";
                        case "dxf": return "篮球大小分";
                        default: return "竞彩篮球";
                    }
                case "ctzq":
                    switch (type.ToLower())
                    {
                        case "t14c": return "足彩14场胜负彩";
                        case "tr9": return "足彩胜负任九场";
                        case "t6bqc": return "足彩六场半全场";
                        case "t4cjq": return "足彩四场进球彩";
                        default: return "传统足球";
                    }
                case "bjdc":
                    switch (type.ToLower())
                    {
                        case "sxds": return "单场上下单双";
                        case "spf": return "单场胜平负";
                        case "zjq": return "单场总进球";
                        case "bf": return "单场比分";
                        case "bqc": return "单场半全场";
                        default: return "北京单场";
                    }
                default: return gamecode;
            }
        }
        #region 最新动态
        public void UpdateProfileDynamic(string userId, string schemeId, string gameCode, string issuseNumber, decimal totalMoney, string dynamicType)
        {

            try
            {
                DB.Begin();

                var man = new Sports_Manager();
                var ub = new UserBalanceManager();

                var together = man.QuerySports_Together(schemeId);
                var user2 = string.Empty;
                switch (dynamicType)
                {
                    case "参与合买":
                        user2 = together.CreateUserId;
                        break;
                }

                var user1Name = ub.QueryUserRegister(userId);
                var user2Name = new C_User_Register();
                if (!string.IsNullOrEmpty(user2))
                {
                    user2Name = ub.QueryUserRegister(user2);
                }

                var entity = new E_Blog_Dynamic()
                {
                    UserId = userId,
                    UserDisplayName = user1Name.DisplayName,
                    UserId2 = user2,
                    User2DisplayName = string.IsNullOrEmpty(user2) ? "" : user2Name.DisplayName,
                    GameCode = gameCode,
                    GameType = together == null ? "" : together.GameType,
                    IssuseNumber = issuseNumber,
                    DynamicType = dynamicType,
                    Guarantees = together == null ? 0 : together.Guarantees,
                    Price = together == null ? 0M : together.Price,
                    Progress = together == null ? 0M : together.Progress,
                    TotalMonery = together == null ? 0M : together.TotalMoney,
                    SchemeId = schemeId,
                    Subscription = together == null ? 0 : together.Subscription,
                    CreateTime = DateTime.Now,
                };
                var manager = new BlogManager();
                manager.AddBlog_Dynamic(entity);
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            
        }
        public ProfileDynamicCollection QueryProfileDynamicCollection(string userId, int pageIndex, int pageSize)
        {
            var result = new ProfileDynamicCollection();
            var totalCount = 0;
            result.List.AddRange(new BlogManager().QueryProfileDynamicList(userId, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }
        #endregion

        #region 数据统计
        public void UpdateProfileDataReport(string userId, int? createCount, int? joinCount, int? bonusCount, decimal? bonusMoney)
        {
            var manager = new BlogManager();
            var man = manager.QueryBlog_DataReport(userId);
            if (man == null)
            {
                var entity = new E_Blog_DataReport()
                {
                    UserId = userId,
                    CreateSchemeCount = 0,
                    JoinSchemeCount = 0,
                    TotalBonusCount = 0,
                    TotalBonusMoney = 0M,
                    UpdateTime = DateTime.Now,
                };
                manager.AddBlog_DataReport(entity);
            }
            else
            {
                man.UserId = userId;
                man.CreateSchemeCount = !createCount.HasValue ? man.CreateSchemeCount : (man.CreateSchemeCount + createCount.Value);
                man.JoinSchemeCount = !joinCount.HasValue ? man.JoinSchemeCount : (man.JoinSchemeCount + joinCount.Value);
                man.TotalBonusCount = !bonusCount.HasValue ? man.TotalBonusCount : (man.TotalBonusCount + bonusCount.Value);
                man.TotalBonusMoney = !bonusMoney.HasValue ? man.TotalBonusMoney : (man.TotalBonusMoney + bonusMoney.Value);
                man.UpdateTime = DateTime.Now;

                manager.UpdateBlog_DataReport(man);
            }
        }
      
        public void UpdateSporeadUserData(string userId, string gamecode, decimal totalmoney)
        {
            var manager = new BlogManager();
            var man = manager.QueryBlog_UserSpread(userId);
            if (man != null)
            {
                switch (gamecode.ToUpper())
                {
                    case "CTZQ":
                        man.CTZQ = man.CTZQ + totalmoney;
                        break;
                    case "BJDC":
                        man.BJDC = man.BJDC + totalmoney;
                        break;
                    case "JCZQ":
                        man.JCZQ = man.JCZQ + totalmoney;
                        break;
                    case "JCLQ":
                        man.JCLQ = man.JCLQ + totalmoney;
                        break;
                    case "SSQ":
                    case "FC3D":
                    case "DLT":
                    case "PL3":
                        man.SZC = man.SZC + totalmoney;
                        break;
                    case "CQSSC":
                    case "JXSSC":
                    case "SD11X5":
                    case "GD11X5":
                    case "JX11X5":
                        man.GPC = man.GPC + totalmoney;
                        break;

                }
                man.UpdateTime = DateTime.Now;
                manager.UpdateBlog_UserSpread(man);
            }
        }
        #endregion

        #region 获奖级别
        public void UpdateProfileBonusLevel(string userId, decimal bonusMoney)
        {
            int winBaiCount = 0;
            int winQianCount = 0;
            int winWanCount = 0;
            int winShiWanCount = 0;
            int winBaiWanCount = 0;
            int winQianWanCount = 0;
            int winYiCount = 0;

            #region 用户最大中奖记录
            var maxLevelName = string.Empty;
            var maxLevelValue = 0;

            if (bonusMoney < 100)
            {
                maxLevelName = "幸运彩民";
                maxLevelValue = 0;
            }
            else if (bonusMoney >= 100 && bonusMoney < 1000)
            {
                winBaiCount = 1;
                maxLevelName = "百元";
                maxLevelValue = 100;
            }
            else if (bonusMoney >= 1000 && bonusMoney < 10000)
            {
                winQianCount = 1;
                maxLevelName = "千元";
                maxLevelValue = 1000;
            }
            else if (bonusMoney >= 10000 && bonusMoney < 100000)
            {
                winWanCount = 1;
                maxLevelName = "万元";
                maxLevelValue = 10000;
            }
            else if (bonusMoney >= 100000 && bonusMoney < 1000000)
            {
                winShiWanCount = 1;
                maxLevelName = "十万";
                maxLevelValue = 100000;
            }
            else if (bonusMoney >= 1000000 && bonusMoney < 10000000)
            {
                winBaiWanCount = 1;
                maxLevelName = "百万";
                maxLevelValue = 1000000;
            }
            else if (bonusMoney >= 10000000 && bonusMoney < 100000000)
            {
                winQianWanCount = 1;
                maxLevelName = "千万";
                maxLevelValue = 10000000;
            }
            else if (bonusMoney >= 100000000)
            {
                maxLevelName = "亿元";
                maxLevelValue = 100000000;
                winYiCount = 1;
            }

            #endregion

            var manager = new BlogManager();
            var main = manager.QueryBlog_ProfileBonusLevel(userId);
            if (main == null)
            {
                var entity = new E_Blog_ProfileBonusLevel()
                {
                    UserId = userId,
                    MaxLevelName = maxLevelName,
                    MaxLevelValue = maxLevelValue,
                    WinOneHundredCount = winBaiCount,
                    WinOneThousandCount = winQianCount,
                    WinTenThousandCount = winWanCount,
                    WinOneHundredThousandCount = winShiWanCount,
                    WinOneMillionCount = winBaiWanCount,
                    WinTenMillionCount = winQianWanCount,
                    WinHundredMillionCount = winYiCount,
                    UpdateTime = DateTime.Now,
                    TotalBonusMoney = bonusMoney,
                };
                manager.AddBlog_ProfileBonusLevel(entity);
            }
            else
            {
                #region 计算称号 20150922 暂时屏蔽

                //bonusMoney = main.TotalBonusMoney + bonusMoney;
                //if (bonusMoney < 100)
                //{
                //    maxLevelName = "幸运彩民";
                //    maxLevelValue = 0;
                //}
                //else if (bonusMoney >= 100 && bonusMoney < 1000)
                //{
                //    winBaiCount = 1;
                //    maxLevelName = "百元";
                //    maxLevelValue = 100;
                //}
                //else if (bonusMoney >= 1000 && bonusMoney < 10000)
                //{
                //    winQianCount = 1;
                //    maxLevelName = "千元";
                //    maxLevelValue = 1000;
                //}
                //else if (bonusMoney >= 10000 && bonusMoney < 100000)
                //{
                //    winWanCount = 1;
                //    maxLevelName = "万元";
                //    maxLevelValue = 10000;
                //}
                //else if (bonusMoney >= 100000 && bonusMoney < 1000000)
                //{
                //    winShiWanCount = 1;
                //    maxLevelName = "十万";
                //    maxLevelValue = 100000;
                //}
                //else if (bonusMoney >= 1000000 && bonusMoney < 10000000)
                //{
                //    winBaiWanCount = 1;
                //    maxLevelName = "百万";
                //    maxLevelValue = 1000000;
                //}
                //else if (bonusMoney >= 10000000 && bonusMoney < 100000000)
                //{
                //    winQianWanCount = 1;
                //    maxLevelName = "千万";
                //    maxLevelValue = 10000000;
                //}
                //else if (bonusMoney >= 100000000)
                //{
                //    maxLevelName = "亿元";
                //    maxLevelValue = 100000000;
                //    winYiCount = 1;
                //}

                #endregion


                #region 计算称号 new

                var sportManager = new Sports_Manager();
                var maxBonusMoney = sportManager.GetUserMaxBonusMoney(userId);
                if (maxBonusMoney < 100)
                {
                    maxLevelName = "幸运彩民";
                    maxLevelValue = 0;
                }
                else if (maxBonusMoney >= 100 && maxBonusMoney < 1000)
                {
                    winBaiCount = 1;
                    maxLevelName = "百元";
                    maxLevelValue = 100;
                }
                else if (maxBonusMoney >= 1000 && maxBonusMoney < 10000)
                {
                    winQianCount = 1;
                    maxLevelName = "千元";
                    maxLevelValue = 1000;
                }
                else if (maxBonusMoney >= 10000 && maxBonusMoney < 100000)
                {
                    winWanCount = 1;
                    maxLevelName = "万元";
                    maxLevelValue = 10000;
                }
                else if (maxBonusMoney >= 100000 && maxBonusMoney < 1000000)
                {
                    winShiWanCount = 1;
                    maxLevelName = "十万";
                    maxLevelValue = 100000;
                }
                else if (maxBonusMoney >= 1000000 && maxBonusMoney < 10000000)
                {
                    winBaiWanCount = 1;
                    maxLevelName = "百万";
                    maxLevelValue = 1000000;
                }
                else if (maxBonusMoney >= 10000000 && maxBonusMoney < 100000000)
                {
                    winQianWanCount = 1;
                    maxLevelName = "千万";
                    maxLevelValue = 10000000;
                }
                else if (maxBonusMoney >= 100000000)
                {
                    maxLevelName = "亿元";
                    maxLevelValue = 100000000;
                    winYiCount = 1;
                }

                #endregion

                main.MaxLevelName = maxLevelName;
                main.MaxLevelValue = maxLevelValue;
                main.WinOneHundredCount += winBaiCount;
                main.WinOneThousandCount += winQianCount;
                main.WinTenThousandCount += winWanCount;
                main.WinOneHundredThousandCount += winShiWanCount;
                main.WinOneMillionCount += winBaiWanCount;
                main.WinTenMillionCount += winQianWanCount;
                main.WinHundredMillionCount += winYiCount;
                main.UpdateTime = DateTime.Now;
                main.TotalBonusMoney += bonusMoney;
                //main.TotalBonusMoney = bonusMoney;//20150922修改
                manager.UpdateBlog_ProfileBonusLevel(main);
            }
        }
        public string QueryProfileBonusLevelTitle(string userId)
        {
            var manager = new BlogManager();
            var pb = manager.QueryBlog_ProfileBonusLevel(userId);
            if (pb == null || pb.MaxLevelValue == 0)
            {
                return "0|幸运彩民";
            }
            if (pb.MaxLevelValue >= 10000)
            {
                return pb.MaxLevelValue + "|" + pb.MaxLevelName + "奖得主";
            }
            else
            {
                return pb.MaxLevelValue + "|" + pb.MaxLevelName + "大奖得主";
            }
        }

        public string QueryProfileBonusLevelTitle1(string userId)
        {
            var manager = new BlogManager();
            var pb = manager.QueryBlog_ProfileBonusLevel(userId);
            if (pb == null || pb.MaxLevelValue == 0)
            {
                return "幸运彩民";
            }
            else
            {
                return pb.MaxLevelName;
            }
        }
        public ProfileBonusLevelCollection QueryProfileBonusLevelCollection(string userId)
        {
            var result = new ProfileBonusLevelCollection();
            var bp = new BlogManager().QueryBlog_ProfileBonusLevel1(userId);
            result.List.AddRange(bp);
            return result;
        }

    

        #endregion

        #region 最新中奖
        public void AddProfileLastBonus(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal bonusMoney, DateTime bonusTime)
        {

            try
            {
                DB.Begin();
                var entity = new E_Blog_NewProfileLastBonus()
                {
                    UserId = userId,
                    GameCode = gameCode,
                    GameType = gameType,
                    SchemeId = schemeId,
                    IssuseNumber = issuseNumber,
                    BonusMoney = bonusMoney,
                    BonusTime = bonusTime,
                };
                var manager = new BlogManager();
                manager.AddBlog_NewProfileLastBonus(entity);
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex; 
            }
            
        }
    
        #endregion

        #region 历史战绩

        public void UpdateProfileBeedings(string userId, string gameCode, string gameType, decimal orderMoney, decimal bonusMoney, bool isVirtualOrder, DateTime bonusTime)
        {

        }
        public void UpdateProfileBeedingsByInput(string userId, string gameCode, string gameType, decimal successBettingMoney, decimal successAndBonusMoney, int successAndBonusCount,
            decimal failBettingMoney, decimal failAndBonusMoney, int failAndBonusCount)
        {

        }
        public ProfileBeedingsInfo QueryProfileBeedingsInfo(string userId, string gameCode, string gameType)
        {
            var result = new ProfileBeedingsInfo();

            return result;
        }
        public ProfileBeedingsCollection QueryProfileBeedingsCollection(string userId)
        {
            var collection = new ProfileBeedingsCollection();
            return collection;
        }
        #endregion

        #region 用户关注
        /// <summary>
        /// 关注用户
        /// </summary>
        public void UpdateProfileAttention_Active(string activeUserId, string passiveUserId, int count)
        {
            string guanZhu = string.Empty;
            if (count == 1)
                guanZhu = "关注";
            if (count == -1)
                guanZhu = "取消关注";

            //添加一条动态
            var ub = new UserBalanceManager();
            var user1Name = ub.QueryUserRegister(activeUserId);
            var user2Name = ub.QueryUserRegister(passiveUserId);
            var entity = new E_Blog_Dynamic()
            {
                UserId = activeUserId,
                UserDisplayName = user1Name.DisplayName,
                UserId2 = passiveUserId,
                User2DisplayName = user2Name.DisplayName,
                GameCode = "",
                GameType = "",
                IssuseNumber = "",
                DynamicType = guanZhu,
                Guarantees = 0,
                Price = 0M,
                Progress = 0M,
                TotalMonery = 0M,
                SchemeId = "",
                Subscription = 0,
                CreateTime = DateTime.Now,
            };
            var manager = new BlogManager();
            manager.AddBlog_Dynamic(entity);
        }

        /// <summary>
        /// 被关注
        /// </summary>
        public void UpdateProfileAttention_Passive(string activeUserId, string passiveUserId, int count)
        {
            string guanZhu = string.Empty;
            if (count == 1)
                guanZhu = "被关注";
            if (count == -1)
                guanZhu = "取消被关注";

            //添加一条动态
            var ub = new UserBalanceManager();
            var user1Name = ub.QueryUserRegister(passiveUserId);
            var user2Name = ub.QueryUserRegister(activeUserId);
            var entity = new E_Blog_Dynamic()
            {
                UserId = passiveUserId,
                UserDisplayName = user1Name.DisplayName,
                UserId2 = activeUserId,
                User2DisplayName = user2Name.DisplayName,
                GameCode = "",
                GameType = "",
                IssuseNumber = "",
                DynamicType = guanZhu,
                Guarantees = 0,
                Price = 0M,
                Progress = 0M,
                TotalMonery = 0M,
                SchemeId = "",
                Subscription = 0,
                CreateTime = DateTime.Now,
            };
            var manager = new BlogManager();
            manager.AddBlog_Dynamic(entity);
        }


        #endregion

        #region 定制跟单
        public void UpdateProfileFollower_Active(string creatorUserId, string followUserId, string gameCode, string gameType, int count)
        {
        }
        public void UpdateProfileFollower_Passive(string creatorUserId, string followUserId, string gameCode, string gameType, int count)
        {
        }


        #endregion

        #region 基础信息
        public void UpdateProfileUserInfo(string userId, string displayName, int? hideNameCode, string headImage, int? attentionCount, int? attentionedCount, DateTime? regTime)
        {
        }
     
        #endregion
    }
}
