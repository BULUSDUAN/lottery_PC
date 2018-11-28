using System;
using GameBiz.Core;
using GameBiz.Domain.Managers;
using System.IO;
using System.Xml;
using Common.Log;
using System.Text;
using System.Linq;
using Common.JSON;
using System.Threading;
using Common.Net;
using Common.Utilities;
using System.Collections.Generic;
using GameBiz.Business.Domain.Managers;
using GameBiz.Domain.Entities;
using Common.Lottery.Redis;

namespace GameBiz.Business
{
    public class CacheDataBusiness
        : ICreateTogether_AfterTranCommit   // 创建合买，更新超级发起人状态、最新动态
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
        private static string _baseDir = "";
        private static int _maxLoginHistoryCount = 20;
        private static int _maxVisitorHistoryCount = 10;
        private static int _maxDynamicHistoryCount = 5;
        private static int _maxLastBonusHistoryCount = 10;
        private static int _maxAttentionCount = 50;
        public static void SetCacheDataBaseDir(string dir)
        {
            _baseDir = dir;
        }

        #region 超级发起人

        public SupperCreatorCollection QuerySupperCreatorCollection()
        {
            var fileName = Path.Combine(_baseDir, "supper_creator.xml");
            if (!File.Exists(fileName))
            {
                return new SupperCreatorCollection();
            }
            var doc = new XmlDocument();
            doc.Load(fileName);

            var collection = new SupperCreatorCollection();
            collection.AnalyzeXmlNode(doc.SelectSingleNode("supper"));
            return collection;
        }
        public void UpdateSupperTogetherScheme(string userId, DateTime stopTime)
        {
            var fileName = Path.Combine(_baseDir, "supper_creator.xml");
            var doc = new XmlDocument();
            doc.Load(fileName);

            foreach (XmlNode node in doc.SelectNodes("supper/item"))
            {
                var user = new SupperCreatorInfo();
                user.AnalyzeXmlNode(node);
                if (user.UserId.Equals(userId))
                {
                    var currentStopTime = DateTime.Parse(node.Attributes["lastStopTime"].Value);
                    if (stopTime > currentStopTime)
                    {
                        node.Attributes["hasTogetherScheme"].Value = "true";
                        node.Attributes["lastStopTime"].Value = stopTime.ToString("yyyy-MM-dd HH:mm:ss");
                        doc.Save(fileName);
                        break;
                    }
                }
            }
        }

        #endregion

        #region 用户登录日志

        public UserLoginHistoryInfo QueryUserLastLoginInfo(string userId)
        {
            return new BlogManager().QueryLastLoginInfo(userId);
        }
        public UserLoginHistoryCollection QueryUserLoginHistoryCollection(string userId)
        {
            var result = new UserLoginHistoryCollection();
            result.AddRange(new BlogManager().QueryBlog_UserLoginHistory(userId));
            return result;
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
                        new BlogManager().UpdateBlog_UserLoginHistory(new Blog_UserLoginHistory { LoginIp = loginIp, LoginTime = loginTime, LoginFrom = loginFrom, IpDisplayName = item.IpDisplayName, UserId = userId, Id = item.Id });
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
            new BlogManager().AddBlog_UserLoginHistory(new Blog_UserLoginHistory { LoginIp = loginIp, LoginTime = loginTime, LoginFrom = loginFrom, IpDisplayName = IpDisplayName, UserId = userId });

            //List<UserLoginHistoryInfo> arr = new BlogManager().QueryBlog_UserLoginHistory(userId);
        }

        #endregion

        #region 投注方案

        public void DeleteSchemeInfoXml(string schemeId)
        {
            if (string.IsNullOrEmpty(schemeId)) return;
            var fileName = GetSchemeFileFullName(schemeId);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
        public void SaveSchemeInfoToXml(Sports_SchemeQueryInfo schemeInfo)
        {
            var fileName = GetSchemeFileFullName(schemeInfo.SchemeId);
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }
            var doc = new XmlDocument();
            XmlNode root;
            if (!File.Exists(fileName))
            {
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
                var t = doc.CreateElement("scheme");
                t.Attributes.Append(doc.CreateAttribute("schemeId")).Value = schemeInfo.SchemeId;
                doc.AppendChild(t);
                root = t.AppendChild(doc.CreateElement("info"));
            }
            else
            {
                doc.Load(fileName);
                root = doc.SelectSingleNode("scheme/info");
                if (root == null)
                {
                    var t = doc.SelectSingleNode("scheme");
                    if (t == null)
                    {
                        throw new Exception("方案文件格式错误 - " + schemeInfo.SchemeId);
                    }
                    root = t.AppendChild(doc.CreateElement("info"));
                }
            }
            var xml = schemeInfo.ToInnerXmlString("info");
            root.InnerXml = xml;
            doc.Save(fileName);
        }
        public void SaveSchemeAnteCodeCollectionToXml(string schemeId, Sports_AnteCodeQueryInfoCollection anteCodeCollection)
        {
            var fileName = GetSchemeFileFullName(schemeId);
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }
            var doc = new XmlDocument();
            XmlNode root;
            if (!File.Exists(fileName))
            {
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
                var t = doc.CreateElement("scheme");
                t.Attributes.Append(doc.CreateAttribute("schemeId")).Value = schemeId;
                doc.AppendChild(t);
                root = t.AppendChild(doc.CreateElement("antecode_list"));
            }
            else
            {
                doc.Load(fileName);
                root = doc.SelectSingleNode("scheme/antecode_list");
                if (root == null)
                {
                    var t = doc.SelectSingleNode("scheme");
                    if (t == null)
                    {
                        throw new Exception("方案文件格式错误 - " + schemeId);
                    }
                    root = t.AppendChild(doc.CreateElement("antecode_list"));
                }
            }
            var xml = anteCodeCollection.ToInnerXmlString("antecode");
            root.InnerXml = xml;
            doc.Save(fileName);
        }
        public Sports_SchemeQueryInfo QuerySports_SchemeQueryInfo(string schemeId)
        {
            var fileName = GetSchemeFileFullName(schemeId);
            if (!File.Exists(fileName))
            {
                return null;
            }
            var doc = new XmlDocument();
            doc.Load(fileName);
            var root = doc.SelectSingleNode("scheme/info");
            if (root == null)
            {
                return null;
            }
            var info = new Sports_SchemeQueryInfo();
            info.AnalyzeXmlNode(root);
            return info;
        }
        public Sports_AnteCodeQueryInfoCollection QuerySports_AnteCodeQueryInfoCollection(string schemeId)
        {
            var fileName = GetSchemeFileFullName(schemeId);
            if (!File.Exists(fileName))
            {
                return null;
            }
            var doc = new XmlDocument();
            doc.Load(fileName);
            var root = doc.SelectSingleNode("scheme/antecode_list");
            if (root == null)
            {
                return null;
            }
            var collection = new Sports_AnteCodeQueryInfoCollection();
            collection.AnalyzeXmlNode(root);
            return collection;
        }

        #endregion

        #region 个人主页

        #region 基础信息
        public void UpdateProfileUserInfo(string userId, string displayName, int? hideNameCode, string headImage, int? attentionCount, int? attentionedCount, DateTime? regTime)
        {
        }
        public ProfileUserInfo QueryProfileUserInfo(string userId)
        {
            var manager = new Sports_Manager();
            var info = manager.QueryProfileUserInfo(userId);
            //var pb = new BlogManager().QueryBlog_ProfileBonusLevel(userId);
            //if (pb == null || pb.MaxLevelValue == 0)
            //{
            //    info.MaxLevelName = "幸运彩民";
            //}
            //else
            //{
            //    info.MaxLevelName = pb.MaxLevelName;
            //}
            return info;
        }
        #endregion

        #region 数据统计
        public void UpdateProfileDataReport(string userId, int? createCount, int? joinCount, int? bonusCount, decimal? bonusMoney)
        {
            var manager = new BlogManager();
            var man = manager.QueryBlog_DataReport(userId);
            if (man == null)
            {
                var entity = new Blog_DataReport()
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
        public ProfileDataReport QueryProfileDataReport(string userId)
        {
            var manager = new BlogManager();
            var dataReport = manager.QueryBlog_DataReport(userId);
            if (dataReport == null)
                return new ProfileDataReport();
            return new ProfileDataReport
            {
                UserId = dataReport.UserId,
                CreateSchemeCount = dataReport.CreateSchemeCount,
                JoinSchemeCount = dataReport.JoinSchemeCount,
                TotalBonusCount = dataReport.TotalBonusCount,
                TotalBonusMoney = dataReport.TotalBonusMoney,
                UpdateTime = dataReport.UpdateTime,
            };

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

        #region 访客历史记录
        public void UpdateProfileVisitHistory(string userId, string visitorUserId, string visitorIp, DateTime visitTime)
        {
            if (userId == visitorUserId)
                return;

            var userManager = new UserBalanceManager();
            var visitor = userManager.QueryUserRegister(visitorUserId);
            if (visitor == null) return;

            var manager = new BlogManager();
            var entity = manager.QueryBlog_UserVisitHistory(userId, visitorUserId);
            if (entity == null)
            {
                manager.AddBlog_UserVisitHistory(new Blog_UserVisitHistory()
                {
                    UserId = userId,
                    VisitUserId = visitorUserId,
                    VisitorHideNameCount = visitor.HideDisplayNameCount.ToString(),
                    VisitorUserDisplayName = visitor.DisplayName,
                    VisitorIp = visitorIp,
                    IpDisplayName = IpManager.GetIpDisplayname_Sina(visitorIp).ToString(),
                    CreateTime = DateTime.Now,
                });
            }
            else
            {
                if ((DateTime.Now - entity.CreateTime).TotalMinutes > 10)
                {
                    entity.CreateTime = DateTime.Now;
                    manager.UpdateBlog_UserVisitHistory(entity);
                }
            }
        }
        public ProfileVisitHistoryCollection QueryProfileVisitHistoryCollection(string userId)
        {
            var result = new ProfileVisitHistoryCollection();
            result.AddRange(new BlogManager().QueryBlog_UserVisitHistory(userId));
            return result;
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
                var entity = new Blog_ProfileBonusLevel()
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

        public ProfileBonusLevelInfo QueryProfileBonusLevelInfo(string userId)
        {
            var result = new ProfileBonusLevelInfo();
            var pb = new BlogManager().QueryBlog_ProfileBonusLevel(userId);
            if (pb == null)
                return new ProfileBonusLevelInfo();
            return new ProfileBonusLevelInfo
            {
                UserId = pb.UserId,
                MaxLevelName = pb.MaxLevelName,
                MaxLevelValue = pb.MaxLevelValue,
                WinHundredMillionCount = pb.WinHundredMillionCount,
                WinOneHundredCount = pb.WinOneHundredCount,
                WinOneHundredThousandCount = pb.WinOneHundredThousandCount,
                WinOneMillionCount = pb.WinOneMillionCount,
                WinOneThousandCount = pb.WinOneThousandCount,
                WinTenMillionCount = pb.WinTenMillionCount,
                WinTenThousandCount = pb.WinTenThousandCount,
            };
        }

        #endregion

        #region 最新动态
        public void UpdateProfileDynamic(string userId, string schemeId, string gameCode, string issuseNumber, decimal totalMoney, string dynamicType)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

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
                var user2Name = new UserRegister();
                if (!string.IsNullOrEmpty(user2))
                {
                    user2Name = ub.QueryUserRegister(user2);
                }

                var entity = new Blog_Dynamic()
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
                biz.CommitTran();
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

        #region 最新中奖
        public void AddProfileLastBonus(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal bonusMoney, DateTime bonusTime)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();
                var entity = new Blog_NewProfileLastBonus()
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
                biz.CommitTran();
            }
        }
        public ProfileLastBonusCollection QueryProfileLastBonusCollection(string userId)
        {
            var result = new ProfileLastBonusCollection();
            var totalCount = 0;
            result.List.AddRange(new BlogManager().QueryProfileLastBonusList(userId, out totalCount));
            result.TotalCount = totalCount;
            return result;
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

        #region 定制跟单
        public void UpdateProfileFollower_Active(string creatorUserId, string followUserId, string gameCode, string gameType, int count)
        {
        }
        public void UpdateProfileFollower_Passive(string creatorUserId, string followUserId, string gameCode, string gameType, int count)
        {
        }

        public int QueryProfileFollowedCount(string userId, string gameCode, string gameType)
        {
            var manager = new Sports_Manager();
            return manager.QueryTogetherFollowerRecord(userId, gameCode, gameType);
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
            var entity = new Blog_Dynamic()
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
            var entity = new Blog_Dynamic()
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
        public int QueryProfileAttentionCount(string userId)
        {
            var manager = new Sports_Manager();
            var user = manager.QueryUserAttentionSummary(userId);
            return user.BeAttentionUserCount;
        }
        public int QueryProfileAttentionedCount(string userId)
        {
            var manager = new Sports_Manager();
            var user = manager.QueryUserAttentionSummary(userId);
            return user.FollowerUserCount;
        }
        public ProfileAttentionCollection QueryProfileAttentionCollection(string userId, int pageIndex, int pageSize)
        {
            var result = new ProfileAttentionCollection();
            int totalCount;
            var bp = new Sports_Manager().QueryProfileAttentionInfoList(userId, pageIndex, pageSize, out  totalCount);
            result.TotalCount = totalCount;
            result.List.AddRange(bp);
            return result;
        }

        #endregion

        #region 分享推广
        ///// <summary>
        ///// 分享推广 购彩 送红包
        ///// </summary>
        ///// <param name="userId"></param>
        //public void FirstLotteryGiveRedBag(string userId)
        //{
        //    using (var biz = new GameBizBusinessManagement())
        //    {
        //        biz.BeginTran();
        //        //分享推广 购彩 送红包
        //        //购彩了 且是通过分享注册的用户 没有送红包 就执行分享推广活动
        //        var entityBankCard = new BankCardManager().BankCardById(userId);
        //        var entityShareSpread = new BlogManager().QueryBlog_UserShareSpread(userId);
        //        if (entityBankCard != null && entityShareSpread != null && !entityShareSpread.isGiveLotteryRedBag)
        //        {
        //            //购彩了 没有给分享者送活动红包 就执行送红包 只送一次
        //            var giveFillMoney = decimal.Parse(Activity.Business.ActivityCache.QueryActivityConfig("ActivityConfig.FirstLotteryGiveRedBagTofxid").ConfigValue);
        //            if (giveFillMoney > 0)
        //            {
        //                BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, entityShareSpread.AgentId, Guid.NewGuid().ToString("N"), giveFillMoney
        //                                  , string.Format("{1}用户购彩了赠送红包给分享推广用户{0}元", giveFillMoney, userId), RedBagCategory.FxidRegister);
                        
        //                entityShareSpread.isGiveLotteryRedBag = true;
        //                entityShareSpread.UpdateTime = DateTime.Now;
        //                entityShareSpread.giveRedBagMoney = entityShareSpread.giveRedBagMoney + giveFillMoney;
        //                new BlogManager().UpdateBlog_UserShareSpread(entityShareSpread);
        //            }
        //        }
        //        biz.CommitTran();
        //    }
        //}

        /// <summary>
        /// 分享推广 购彩 送红包(满x元送z元红包)
        /// </summary>
        /// <param name="userId"></param>
        //public void FirstLotteryGiveRedBag(string userId,decimal totalMoney)
        //{
        //    using (var biz = new GameBizBusinessManagement())
        //    {
        //        biz.BeginTran();
        //        //分享推广 购彩 送红包
        //        //购彩了 且是通过分享注册的用户 没有送红包 就执行分享推广活动
        //        //var entityBankCard = new BankCardManager().BankCardById(userId);
        //        /*entityBankCard != null */
        //        var entityShareSpread = new BlogManager().QueryBlog_UserShareSpread(userId);
        //        var satisfyFillMoney = decimal.Parse(Activity.Business.ActivityCache.QueryActivityConfig("ActivityConfig.SatisfyLotteryGiveRedBagTofxid").ConfigValue);
        //        if (entityShareSpread != null && !entityShareSpread.isGiveLotteryRedBag&& totalMoney>= satisfyFillMoney)
        //        {
        //            //购彩了 没有给分享者送活动红包 就执行送红包 只送一次
        //            var giveFillMoney = decimal.Parse(Activity.Business.ActivityCache.QueryActivityConfig("ActivityConfig.FirstLotteryGiveRedBagTofxid").ConfigValue);
        //            if (giveFillMoney > 0)
        //            {
        //                BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, entityShareSpread.AgentId, Guid.NewGuid().ToString("N"), giveFillMoney
        //                                  , string.Format("{1}用户购彩超过{2}元，赠送红包给分享推广用户{0}元", giveFillMoney, userId, satisfyFillMoney), RedBagCategory.FxidRegister);

        //                entityShareSpread.isGiveLotteryRedBag = true;
        //                entityShareSpread.UpdateTime = DateTime.Now;
        //                entityShareSpread.giveRedBagMoney = entityShareSpread.giveRedBagMoney + giveFillMoney;
        //                new BlogManager().UpdateBlog_UserShareSpread(entityShareSpread);
        //            }
        //        }
        //        biz.CommitTran();
        //    }
        //}
        #endregion


        //#region 分享中奖订单推广注册
        //public void FirstOrderShareRegisterRedBag(string schemeId)
        //{
        //    using (var biz = new GameBizBusinessManagement())
        //    {
        //        biz.BeginTran();
        //        var schemeInfo = new Sports_Business().QuerySportsSchemeInfo(schemeId);
        //        if (schemeInfo!=null&&schemeInfo.BonusStatus != BonusStatus.Win || schemeInfo.PreTaxBonusMoney==0) return;
        //        //分享推广 购彩 送红包
        //        //购彩了 且是通过分享注册的用户 没有送红包 就执行分享推广活动
        //        //var entityBankCard = new BankCardManager().BankCardById(userId);
        //        //var entityShareSpread = new BlogManager().QueryBlog_UserShareSpread(userId);
        //        //if (entityBankCard != null && entityShareSpread != null && !entityShareSpread.isGiveLotteryRedBag)
        //        //{
        //            //购彩了 没有给分享者送活动红包 就执行送红包 只送一次
        //        var shareGiveRedBagPre = decimal.Parse(Activity.Business.ActivityCache.QueryActivityConfig("ActivityConfig.WinningShareGiveRedBag").ConfigValue);
        //        var business = new BlogManager();
        //        var oldmodel = business.QueryBlog_OrderShareRegisterRedBag(schemeId, schemeInfo.UserId);
        //        if (oldmodel != null) 
        //        {
        //            if (!oldmodel.IsGiveRegisterRedBag&&shareGiveRedBagPre > 0) 
        //            {
        //                var giveFillMoney = schemeInfo.PreTaxBonusMoney * shareGiveRedBagPre / 100;
        //                BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, schemeInfo.UserId, Guid.NewGuid().ToString("N"), giveFillMoney
        //                                      , string.Format("分享中奖订单{0}，加奖红包：{1}元", schemeId, giveFillMoney.ToString("f2")), RedBagCategory.OrderRegister);
        //                oldmodel.IsGiveRegisterRedBag = true;
        //            }
        //            //更新条数
        //            oldmodel.RegisterCount +=1;
        //            oldmodel.UpdateTime = DateTime.Now;
        //            business.UpdateBlog_OrderShareRegisterRedBag(oldmodel);
        //        }
        //        else
        //        {
        //            var flag=false;
        //            var giveFillMoney = 0m;
        //             if (shareGiveRedBagPre > 0)
        //            {
        //            //发奖
        //                giveFillMoney = schemeInfo.PreTaxBonusMoney * shareGiveRedBagPre / 100;
        //            BusinessHelper.Payin_To_Balance(AccountType.RedBag, BusinessHelper.FundCategory_Activity, schemeInfo.UserId, Guid.NewGuid().ToString("N"), giveFillMoney
        //                                  , string.Format("分享中奖订单{0}，加奖红包：{1}元", schemeId, giveFillMoney), RedBagCategory.OrderRegister);
        //            flag=true;
        //             }
        //            //插入数据库
        //             var newmodel = new BlogOrderShareRegisterRedBag()
        //             {
        //                 CreateTime = DateTime.Now,
        //                 IsGiveRegisterRedBag = flag,
        //                 RedBagMoney = giveFillMoney,
        //                 RedBagPre = shareGiveRedBagPre,
        //                 UpdateTime = DateTime.Now,
        //                 RegisterCount = 1,
        //                 SchemeId = schemeId,
        //                 UserId = schemeInfo.UserId
        //             };
        //             business.Add_OrderShareRegisterRedBag(newmodel);
        //        }
        //        biz.CommitTran();
        //    }
        //}
        //#endregion

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
          //  UsefullHelper.TryDoAction(() => FirstLotteryGiveRedBag(userId, totalMoney));
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
            //UsefullHelper.TryDoAction(() => FirstLotteryGiveRedBag(userId, totalMoney));
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
           // UsefullHelper.TryDoAction(() => FirstLotteryGiveRedBag(userId, bettingOrder.TotalMoney));
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
          //  UsefullHelper.TryDoAction(() => FirstLotteryGiveRedBag(userId, bettingOrder.TotalMoney));
            
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
        public void User_AfterLogin(string userId, string loginFrom, string loginIp, DateTime loginTime)
        {
            new Thread(() =>
            {
                // 更新用户登录日志，忽略异常，异常只记录日志
                UsefullHelper.TryDoAction(() => UpdateUserLoginHistory(userId, loginFrom, loginIp, loginTime));
            }).Start();
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

        #endregion

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
        public string GetGameType(string gamecode, string type)
        {
            switch (gamecode.ToLower())
            {
                case "jczq":
                case "jclq":
                case "ctzq":
                case "bjdc":
                    return type;
                default: return "";
            }
        }
        private string GetUserFileFullName(string userId, string fileName)
        {
            var fullName = Path.Combine(_baseDir, "Users", userId.Substring(0, 3), userId, fileName);
            return fullName;
        }
        private string GetSchemeFileFullName(string schemeId)
        {
            schemeId = schemeId.Trim();
            var prev = GetSchemeIdPrev(schemeId);
            var fullName = Path.Combine(_baseDir, "Schemes", prev, schemeId.Substring(0, prev.Length + 3), schemeId + ".xml");
            return fullName;
        }
        private string GetSchemeIdPrev(string schemeId)
        {
            if (string.IsNullOrEmpty(schemeId)) return string.Empty;

            if (schemeId.StartsWith("TSM", StringComparison.OrdinalIgnoreCase)) { return "TSM"; }
            else if (schemeId.StartsWith("JCZQ", StringComparison.OrdinalIgnoreCase)) { return "JCZQ"; }
            else if (schemeId.StartsWith("JCLQ", StringComparison.OrdinalIgnoreCase)) { return "JCLQ"; }
            else if (schemeId.StartsWith("BJDC", StringComparison.OrdinalIgnoreCase)) { return "BJDC"; }
            else if (schemeId.StartsWith("CTZQ", StringComparison.OrdinalIgnoreCase)) { return "CTZQ"; }
            else if (schemeId.StartsWith("DLT", StringComparison.OrdinalIgnoreCase)) { return "DLT"; }
            else if (schemeId.StartsWith("PL3", StringComparison.OrdinalIgnoreCase)) { return "PL3"; }
            else if (schemeId.StartsWith("SSQ", StringComparison.OrdinalIgnoreCase)) { return "SSQ"; }
            else if (schemeId.StartsWith("FC3D", StringComparison.OrdinalIgnoreCase)) { return "FC3D"; }
            else if (schemeId.StartsWith("CQSSC", StringComparison.OrdinalIgnoreCase)) { return "CQSSC"; }
            else if (schemeId.StartsWith("JXSSC", StringComparison.OrdinalIgnoreCase)) { return "JXSSC"; }
            else if (schemeId.StartsWith("SD11X5", StringComparison.OrdinalIgnoreCase)) { return "SD11X5"; }
            else if (schemeId.StartsWith("GD11X5", StringComparison.OrdinalIgnoreCase)) { return "GD11X5"; }
            else if (schemeId.StartsWith("JX11X5", StringComparison.OrdinalIgnoreCase)) { return "JX11X5"; }
            else if (schemeId.StartsWith("GDKLSF", StringComparison.OrdinalIgnoreCase)) { return "GDKLSF"; }
            else if (schemeId.StartsWith("JSKS", StringComparison.OrdinalIgnoreCase)) { return "JSKS"; }
            else if (schemeId.StartsWith("SDKLPK3", StringComparison.OrdinalIgnoreCase)) { return "SDKLPK3"; }

            else { return schemeId.Substring(0, 5); }
        }

        #region 系统配置相关

        private static List<CoreConfigInfo> _coreConfigList = new List<CoreConfigInfo>();
        /// <summary>
        /// 获取系统配置
        /// </summary>
        public CoreConfigInfoCollection QueryAllCoreConfig()
        {
            //var collection = new CoreConfigInfoCollection();
            //var list = new UserIntegralManager().QueryAllCoreConfig();
            //collection.AddRange(list);
            //return collection;

            if (_coreConfigList.Count == 0)
                _coreConfigList = new UserIntegralManager().QueryAllCoreConfig();

            var collection = new CoreConfigInfoCollection();
            collection.AddRange(_coreConfigList);
            return collection;
        }

        public CoreConfigInfo QueryCoreConfigByKey(string key)
        {
            //var _coreConfigList = new UserIntegralManager().QueryAllCoreConfig();
            //var config = _coreConfigList.FirstOrDefault(p => p.ConfigKey == key);
            //if (config == null)
            //    throw new Exception(string.Format("找不到配置项：{0}", key));
            //return config;

            if (_coreConfigList.Count == 0)
                _coreConfigList = new UserIntegralManager().QueryAllCoreConfig();
            var config = _coreConfigList.FirstOrDefault(p => p.ConfigKey == key);
            if (config == null)
                throw new Exception(string.Format("找不到配置项：{0}", key));
            return config;
        }

        /// <summary>
        /// 从Redis中查询系统配置
        /// </summary>
        public string QueryCoreConfigFromRedis(string key)
        {
            if (_coreConfigList.Count == 0)
                _coreConfigList = new UserIntegralManager().QueryAllCoreConfig();
            var config = _coreConfigList.FirstOrDefault(p => p.ConfigKey == key);
            if (config == null)
                throw new Exception(string.Format("找不到配置项：{0}", key));
            return config.ConfigValue;


            //var db = RedisHelper.DB_CoreCacheData;
            //var redisKey = RedisKeys.Key_CoreConfig;
            //foreach (var item in db.ListRangeAsync(redisKey).Result)
            //{
            //    if (!item.HasValue) continue;
            //    var array = item.ToString().Split('^');
            //    if (array.Length != 3)
            //        continue;
            //    if (array[0] == key)
            //        return array[2];
            //}
            //return string.Empty;
        }


        /// <summary>
        /// 更新系统配置
        /// </summary>
        public void UpdateCoreConfigInfo(CoreConfigInfo info)
        {
            var manager = new UserIntegralManager();
            var entity = manager.QueryCoreConfig(info.Id);
            if (entity == null) return;
            entity.ConfigName = info.ConfigName;
            entity.ConfigValue = info.ConfigValue;
            entity.CreateTime = DateTime.Now;
            manager.UpdateCoreConfig(entity);

            this.ClearCoreConfig();
            //LoadCoreConfigToRedis();
        }

        /// <summary>
        /// 清空系统配置
        /// </summary>
        public void ClearCoreConfig()
        {
            if (_coreConfigList != null)
                _coreConfigList.Clear();
        }

        /// <summary>
        /// 加载配置项到Redis
        /// </summary>
        public void LoadCoreConfigToRedis()
        {
            var db = RedisHelper.DB_CoreCacheData;
            var key = RedisKeys.Key_CoreConfig;
            db.KeyDeleteAsync(key);
            var coreConfigList = new UserIntegralManager().QueryAllCoreConfig();
            foreach (var item in coreConfigList)
            {
                var v = string.Format("{0}^{1}^{2}", item.ConfigKey, item.ConfigName, item.ConfigValue);
                db.ListRightPushAsync(key, v);
            }

            //禁用的彩种
            var gameList = new LotteryGameManager().QueryAllGame();
            var gameKey = RedisKeys.Key_AllGameCode;
            db.KeyDeleteAsync(gameKey);
            foreach (var g in gameList)
            {
                var v = string.Format("{0}^{1}", g.GameCode.ToUpper(), (int)g.EnableStatus);
                db.ListRightPushAsync(gameKey, v);
            }
        }

        #region APP升级相关配置

        private static List<APPConfigInfo> _AppConfigList = new List<APPConfigInfo>();

        public APPConfig_Collection QueryAppConfigList()
        {
            if (_AppConfigList.Count == 0)
                _AppConfigList = new UserIntegralManager().QueryAppConfigList();

            var collection = new APPConfig_Collection();
            collection.AppConfigList.AddRange(_AppConfigList);
            return collection;
        }

        public APPConfigInfo QueryAppConfigByAgentId(string appAgentId)
        {
            if (string.IsNullOrEmpty(appAgentId))
                appAgentId = "100000";
            if (_AppConfigList.Count == 0)
                _AppConfigList = new UserIntegralManager().QueryAppConfigList();
            var config = _AppConfigList.FirstOrDefault(p => p.AppAgentId == appAgentId);
            if (config == null)
            {
                var entity = new UserIntegralManager().QueryAppConfigByAgentId(appAgentId);
                if (entity == null)
                    throw new Exception("未查询到下载地址");
                config = new APPConfigInfo();
                ObjectConvert.ConverEntityToInfo(entity, ref config);
                _AppConfigList.Add(config);
            }
            return config;
        }
        public void UpdateAppConfig(APPConfigInfo info)
        {
            var manager = new UserIntegralManager();
            var entity = manager.QueryAppConfigByAgentId(info.AppAgentId);
            if (entity == null)
                throw new Exception("未查询到当前配置");
            entity.ConfigCode = info.ConfigCode;
            entity.ConfigDownloadUrl = info.ConfigDownloadUrl;
            entity.ConfigExtended = info.ConfigExtended;
            entity.ConfigName = info.ConfigName;
            entity.ConfigUpdateContent = info.ConfigUpdateContent;
            entity.ConfigVersion = info.ConfigVersion;
            entity.IsForcedUpgrade = info.IsForcedUpgrade;
            entity.AgentName = info.AgentName;
            manager.UpdateAppConfig(entity);
            ClearAPPConfig();
        }
        /// <summary>
        /// 清空系统配置
        /// </summary>
        public void ClearAPPConfig()
        {
            if (_AppConfigList != null)
                _AppConfigList.Clear();
        }


        #endregion

        #region APP端嵌套页面配置

        private static List<NestedUrlConfig> _NestedUrlConfigList = new List<NestedUrlConfig>();
        private static List<NestedUrlConfig> _AllNestedUrlConfigList = new List<NestedUrlConfig>();

        public NestedUrlConfigInfo QueryNestedUrlConfigByKey(string configKey)
        {
            if (string.IsNullOrEmpty(configKey))
                throw new Exception("未查询到配置信息");
            if (_NestedUrlConfigList == null || _NestedUrlConfigList.Count <= 0)
                _NestedUrlConfigList = new UserIntegralManager().QueryNestedUrlList();
            var config = _NestedUrlConfigList.FirstOrDefault(p => p.ConfigKey == configKey);
            NestedUrlConfigInfo info = new NestedUrlConfigInfo();
            if (config == null)
            {
                var entity = new UserIntegralManager().QueryNestedUrlByKey(configKey);
                if (entity == null)
                    throw new Exception("未查询到配置信息");
                ObjectConvert.ConverEntityToInfo(entity, ref info);
                _NestedUrlConfigList.Add(config);
            }
            else
                ObjectConvert.ConverEntityToInfo(config, ref info);
            return info;
        }

        /// <summary>
        /// 根据UrlType查询所有APP嵌套配置
        /// </summary>
        /// <returns></returns>
        public NestedUrlConfig_Collection QueryNestedUrlConfigListByUrlType(int urlType)
        {
            using (var manager = new UserIntegralManager())
            {
                try
                {
                    NestedUrlConfig_Collection collection = new NestedUrlConfig_Collection();
                    if (_AllNestedUrlConfigList == null || _AllNestedUrlConfigList.Count <= 0)
                    {
                        var nestedConfigList = manager.QueryNestedUrlList();
                        _AllNestedUrlConfigList.AddRange(nestedConfigList);
                    }
                    var list = _AllNestedUrlConfigList.Where(s => s.UrlType == (UrlType)urlType || s.UrlType == UrlType.All).ToList();
                    if (list == null || list.Count <= 0)
                        _AllNestedUrlConfigList.AddRange(list);
                    foreach (var item in list)
                    {
                        NestedUrlConfigInfo info = new NestedUrlConfigInfo();
                        info.ConfigKey = item.ConfigKey;
                        info.CreateTime = item.CreateTime;
                        info.Id = item.Id;
                        info.IsEnable = item.IsEnable;
                        info.Remarks = item.Remarks;
                        info.Url = item.Url;
                        info.UrlType = item.UrlType;
                        collection.NestedUrlList.Add(info);
                    }
                    return collection;
                }
                catch
                {
                    ClearNestedUrlConfig();
                    return new NestedUrlConfig_Collection();
                }
            }
        }
        /// <summary>
        /// 清空系统配置
        /// </summary>
        public void ClearNestedUrlConfig()
        {
            if (_NestedUrlConfigList != null || _NestedUrlConfigList.Count <= 0)
                _NestedUrlConfigList.Clear();
            if (_AllNestedUrlConfigList != null || _AllNestedUrlConfigList.Count <= 0)
                _AllNestedUrlConfigList.Clear();
        }


        #endregion


        #endregion

        /// <summary>
        /// 清理用户绑定数据缓存
        /// </summary>
        public void ClearUserBindInfoCache(string userId)
        {
            try
            {
                var fullKey = string.Format("{0}_{1}", RedisKeys.Key_UserBind, userId);
                var db = RedisHelper.DB_UserBindData;
                db.KeyDeleteAsync(fullKey);
            }
            catch (Exception)
            {
            }
            //try
            //{
            //    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "UserBindInfo", DateTime.Today.ToString("yyyyMMdd"));
            //    if (!System.IO.Directory.Exists(path))
            //        System.IO.Directory.CreateDirectory(path);
            //    var filePath = System.IO.Path.Combine(path, string.Format("{0}.json", userId));
            //    if (!System.IO.File.Exists(filePath))
            //        return;

            //    System.IO.File.Delete(filePath);

            //    //var cache = _cacheUserBindInfo.FirstOrDefault(p => p.UserId == userId);
            //    //if (cache != null)
            //    //    _cacheUserBindInfo.Remove(cache);
            //}
            //catch (Exception)
            //{
            //}
        }
       
    }
}
