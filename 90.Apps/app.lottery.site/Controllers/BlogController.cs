using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using app.lottery.site.Controllers;
using System.Web.Mvc;
using GameBiz.Core;
using Common.Net;
using System.IO;
using System.Text;
using app.lottery.site.cbbao.Models;
using System.Configuration;

namespace app.lottery.site.iqucai.Controllers
{
    public class BlogController : BaseController
    {
        /// <summary>
        /// 会员中心
        /// </summary>
        public ActionResult Index(string id)
        {

            ViewBag.UserInfo = WCFClients.GameClient.QueryProfileUserInfo(id);
            ViewBag.UserId = id;
            try
            {
                //更新记录
                WCFClients.GameClient.UpdateProfileVisitHistory(id, IpManager.IPAddress, UserToken);
                //查询记录
                ViewBag.VisitList = WCFClients.GameClient.QueryProfileVisitHistoryCollection(id, 10);

                ViewBag.pageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? PageIndex : int.Parse(Request["pageIndex"]);
                ViewBag.pageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 5 : 5;

                ViewBag.DynamicList = WCFClients.GameClient.QueryProfileDynamicCollection(id, ViewBag.pageIndex, ViewBag.pageSize);

                ViewBag.StandingList = WCFClients.GameClient.QueryBonusUserBeedingList(ViewBag.UserId);
                ViewBag.Attention = WCFClients.GameClient.QueryUserAttentionList(ViewBag.UserId, 0, 3, UserToken);
                //ViewBag.Attention = WCFClients.GameClient.QueryProfileAttentionCollection(ViewBag.UserId, 10);
                ViewBag.CreateTog = WCFClients.GameQueryClient.QueryCreateTogetherOrderListByUserId(ViewBag.UserId, null, "", DateTime.Today.AddDays(-7), DateTime.Today.AddDays(1), 0, 10);

                ViewBag.JoinTog = WCFClients.GameQueryClient.QueryJoinTogetherOrderListByUserId(ViewBag.UserId, null, "", DateTime.Today.AddDays(-7), DateTime.Today.AddDays(1), 0, 10);

                //ViewBag.UserComment = WCFClients.ExternalClient.QueryUserComment(id, 5);
                // ViewBag.IsShowData = this.IsShowData();

            }
            catch
            {

            }
            return View();
        }

        /// <summary>
        /// 会员中心左侧菜单
        /// </summary>
        public PartialViewResult left(string id, string name)
        {
            ViewBag.UserId = name;
            ViewBag.User = CurrentUser;
            ViewBag.Id = id;
            ViewBag.GZ = false;
            try
            {
                ViewBag.BonusLevel = WCFClients.GameClient.QueryProfileBonusLevelInfo(id);
                ViewBag.BonusList = WCFClients.GameClient.QueryProfileLastBonusCollection(id);

                ViewBag.UserInfo = WCFClients.GameClient.QueryProfileUserInfo(id);

                ViewBag.DataReport = WCFClients.GameClient.QueryProfileDataReport(id);
                ViewBag.Count = WCFClients.GameQueryClient.QueryTogetherFollowerCount(id);
                if (CurrentUser != null)
                {
                    ViewBag.GZ = WCFClients.GameClient.QueryIsAttention(CurrentUser.LoginInfo.UserId, id);
                }

            }
            catch
            {

            }
            return PartialView();
        }



        /// <summary>
        /// 定制跟单
        /// </summary>
        public ActionResult custombill(string id)
        {
            ViewBag.User = CurrentUser;
            var blog = QueryBlogEntityCustombill(id);
            //new BlogEntity
            //{
            //    BonusOrderInfo = new BonusOrderInfoCollection(),
            //    CreateTime = DateTime.Now,
            //    FollowerCount = 0,
            //    ProfileBonusLevel = new ProfileBonusLevelInfo(),
            //    ProfileDataReport = new ProfileDataReport(),
            //    ProfileLastBonus = new ProfileLastBonusCollection(),
            //    ProfileUserInfo = new ProfileUserInfo(),
            //    UserBeedingListInfo = new UserBeedingListInfoCollection(),
            //    UserCurrentOrderInfo = new UserCurrentOrderInfoCollection(),
            //};// QueryBlogEntityCustombill(id);// LoadBlogEntityCustombill(id);
            ViewBag.UserId = id;
            ViewBag.UserInfo = blog.ProfileUserInfo;
            ViewBag.BonusLevel = blog.ProfileBonusLevel;
            ViewBag.BonusListZj = blog.ProfileLastBonus;
            ViewBag.DataReport = blog.ProfileDataReport;
            ViewBag.Count = blog.FollowerCount;
            ViewBag.BillList = blog.UserBeedingListInfo;
            return View();
        }
        /// <summary>
        /// 定制跟单 读取博客数据
        /// </summary>
        private BlogEntity LoadBlogEntityCustombill(string userId)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "blog", userId);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var dataFileName = Path.Combine(path, string.Format("custombill_{0}.json", userId));
            if (!System.IO.File.Exists(dataFileName))
            {
                //文件不存在（第一次访问）
                return QueryBlogEntityAndSaveCustombillJson(dataFileName, userId);
            }
            else
            {
                //从文件中读取
                var json = System.IO.File.ReadAllText(dataFileName, Encoding.UTF8);
                var blog = Common.JSON.JsonSerializer.Deserialize<BlogEntity>(json);
                var validMinute = int.Parse(ConfigurationManager.AppSettings["BlogValidMinute"]);
                if ((DateTime.Now - blog.CreateTime).TotalMinutes > validMinute)
                {
                    //重新生成文件
                    return QueryBlogEntityAndSaveCustombillJson(dataFileName, userId);
                }
                else
                {
                    //使用缓存数据
                    return blog;
                }
            }
        }
        /// <summary>
        /// 定制跟单 查询博客数据并且保存在json中
        /// </summary>
        private BlogEntity QueryBlogEntityAndSaveCustombillJson(string dataFileName, string userId)
        {
            if (System.IO.File.Exists(dataFileName))
                System.IO.File.Delete(dataFileName);

            var blog = QueryBlogEntityCustombill(userId);
            var contents = Common.JSON.JsonSerializer.Serialize(blog);
            System.IO.File.WriteAllText(dataFileName, contents, Encoding.UTF8);
            return blog;
        }
        /// <summary>
        /// 定制跟单 从数据库中查询数据
        /// </summary>
        private BlogEntity QueryBlogEntityCustombill(string userId)
        {
            try
            {
                var blog = new BlogEntity();
                blog.ProfileUserInfo = WebRedisHelper.QueryProfileUserInfo(userId); //  WCFClients.GameClient.QueryProfileUserInfo(userId);
                blog.ProfileBonusLevel = WebRedisHelper.QueryProfileBonusLevelInfo(userId); // WCFClients.GameClient.QueryProfileBonusLevelInfo(userId);
                blog.ProfileLastBonus = WebRedisHelper.QueryProfileLastBonusCollection(userId);// WCFClients.GameClient.QueryProfileLastBonusCollection(userId);
                blog.ProfileDataReport = WebRedisHelper.QueryProfileDataReport(userId);// WCFClients.GameClient.QueryProfileDataReport(userId);
                blog.FollowerCount = WebRedisHelper.QueryTogetherFollowerCount(userId);// WCFClients.GameQueryClient.QueryTogetherFollowerCount(userId);
                blog.UserBeedingListInfo = WebRedisHelper.QueryUserBeedingListInfoCollection(userId);// WCFClients.GameClient.QueryUserBeedingList("", "", userId, string.Empty, 0, 100, QueryUserBeedingListOrderByProperty.TotalBonusMoney, OrderByCategory.DESC, UserToken);
                blog.CreateTime = DateTime.Now;
                return blog;
            }
            catch (Exception)
            {
                return new BlogEntity();
            }
        }
        /// <summary>
        /// 当前投注
        /// </summary>
        public ActionResult current(string id)
        {
            ViewBag.UserId = id;
            ViewBag.UserInfo = WCFClients.GameClient.QueryProfileUserInfo(id);
            try
            {
                ViewBag.PageIndex = string.IsNullOrEmpty(Request.QueryString["pageIndex"]) ? 0 : int.Parse(Request.QueryString["pageIndex"]);
                ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? 10 : int.Parse(Request.QueryString["pageSize"]);
                ViewBag.PageIndexc = string.IsNullOrEmpty(Request.QueryString["pageIndexc"]) ? 0 : int.Parse(Request.QueryString["pageIndexc"]);
                ViewBag.PageSizec = string.IsNullOrEmpty(Request.QueryString["pageSizec"]) ? 10 : int.Parse(Request.QueryString["pageSizec"]);
                ViewBag.CreateGame = string.IsNullOrEmpty(Request["creategame"]) ? "" : Request["creategame"].Trim();
                ViewBag.JoinGame = string.IsNullOrEmpty(Request["joingame"]) ? "" : Request["joingame"].Trim();
                ViewBag.IsCreateUser = string.IsNullOrEmpty(Request["IsCreateUser"]) ? false : bool.Parse(Request["IsCreateUser"].Trim());
                ViewBag.CurrentOrder = WCFClients.GameClient.QueryUserCurrentOrderList(id, ViewBag.CreateGame, UserToken, ViewBag.PageIndexc, ViewBag.PageSizec);
                ViewBag.Together = WCFClients.GameClient.QueryUserCurrentTogetherOrderList(id, ViewBag.JoinGame, ViewBag.PageIndex, ViewBag.PageSize);
            }
            catch
            {

            }
            return View();
        }

        /// <summary>
        /// 他的粉丝
        /// </summary>
        public ActionResult fans(string id)
        {
            ViewBag.UserId = id;
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.gz = null;
            if (CurrentUser != null)
            {
                ViewBag.gz = WCFClients.GameClient.QueryUserAttentionList(CurrentUser.LoginInfo.UserId, 0, 100, UserToken);
            }
            ViewBag.UserInfo = WCFClients.GameClient.QueryProfileUserInfo(id);
            ViewBag.pageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 10 : int.Parse(Request["pageSize"]);
            var attentionType = string.IsNullOrEmpty(Request["attentionType"]) ? "attentioned" : Request["attentionType"];

            switch (attentionType)
            {
                case "attention":
                    ViewBag.AttentionList = WCFClients.GameClient.QueryUserAttentionList(ViewBag.UserId, ViewBag.pageIndex, ViewBag.PageSize, UserToken);
                    break;
                case "attentioned":
                    ViewBag.AttentionList = WCFClients.GameClient.QueryAttentionUserList(ViewBag.UserId, ViewBag.pageIndex, ViewBag.PageSize, UserToken);
                    break;
                case "attentionrank":
                    ViewBag.AttentionList = WCFClients.GameClient.QueryUserAttentionSummaryRank(ViewBag.pageIndex, ViewBag.PageSize, UserToken);
                    break;
                default:
                    ViewBag.AttentionList = WCFClients.GameClient.QueryUserAttentionList(ViewBag.UserId, ViewBag.pageIndex, ViewBag.PageSize, UserToken);
                    break;
            }
            ViewBag.AttenttionType = attentionType;
            var cc = ViewBag.AttentionList;
            return View();
        }

        /// <summary>
        /// 他关注的人
        /// </summary>
        public ActionResult attention(string id)
        {
            ViewBag.UserId = id;
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.gz = null;
            if (CurrentUser != null)
            {
                ViewBag.gz = WCFClients.GameClient.QueryUserAttentionList(CurrentUser.LoginInfo.UserId, 0, 100, UserToken);
            }
            ViewBag.UserInfo = WCFClients.GameClient.QueryProfileUserInfo(id);
            ViewBag.pageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 10 : int.Parse(Request["pageSize"]);
            var attentionType = string.IsNullOrEmpty(Request["attentionType"]) ? "attention" : Request["attentionType"];

            switch (attentionType)
            {
                case "attention":
                    ViewBag.AttentionList = WCFClients.GameClient.QueryUserAttentionList(ViewBag.UserId, ViewBag.pageIndex, ViewBag.PageSize, UserToken);
                    break;
                case "attentioned":
                    ViewBag.AttentionList = WCFClients.GameClient.QueryAttentionUserList(ViewBag.UserId, ViewBag.pageIndex, ViewBag.PageSize, UserToken);
                    break;
                case "attentionrank":
                    ViewBag.AttentionList = WCFClients.GameClient.QueryUserAttentionSummaryRank(ViewBag.pageIndex, ViewBag.PageSize, UserToken);
                    break;
                default:
                    ViewBag.AttentionList = WCFClients.GameClient.QueryUserAttentionList(ViewBag.UserId, ViewBag.pageIndex, ViewBag.PageSize, UserToken);
                    break;
            }
            ViewBag.AttenttionType = attentionType;
            return View();
        }
        /// <summary>
        /// 历史战绩 读取博客数据
        /// </summary>
        private BlogEntity LoadBlogEntityStandings(string userId, string gameCode, string gameType, int pageIndex, int pageSize)
        {
            //return new BlogEntity();

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "blog", userId);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var dataFileName = Path.Combine(path, string.Format("index_{0}_{1}.json", gameCode, gameType));
            if (gameCode == "SZC" && gameType == "SSQ")
            {
                dataFileName = Path.Combine(path, string.Format("index_{0}.json", userId));
            }
            if (!System.IO.File.Exists(dataFileName))
            {
                //文件不存在（第一次访问）
                return QueryBlogEntityAndSaveStandingsJson(dataFileName, userId, gameCode, gameType, pageIndex, pageSize);
            }
            else
            {
                //从文件中读取
                var json = System.IO.File.ReadAllText(dataFileName, Encoding.UTF8);
                var blog = Common.JSON.JsonSerializer.Deserialize<BlogEntity>(json);
                var validMinute = int.Parse(ConfigurationManager.AppSettings["BlogValidMinute"]);
                if ((DateTime.Now - blog.CreateTime).TotalMinutes > validMinute)
                {
                    //重新生成文件
                    return QueryBlogEntityAndSaveStandingsJson(dataFileName, userId, gameCode, gameType, pageIndex, pageSize);
                }
                else
                {
                    //使用缓存数据
                    return blog;
                }
            }
        }

        /// <summary>
        ///历史战绩  查询博客数据并保存json
        /// </summary>
        private BlogEntity QueryBlogEntityAndSaveStandingsJson(string dataFileName, string userId, string gameCode, string gameType, int pageIndex, int pageSize)
        {
            if (System.IO.File.Exists(dataFileName))
                System.IO.File.Delete(dataFileName);

            var blog = QueryBlogEntityStandings(userId, gameCode, gameType, pageIndex, pageSize);
            var contents = Common.JSON.JsonSerializer.Serialize(blog);
            System.IO.File.WriteAllText(dataFileName, contents, Encoding.UTF8);
            return blog;
        }



        /// <summary>
        /// 历史战绩 从数据库中查询博客数据
        /// </summary>
        private BlogEntity QueryBlogEntityStandings(string userId, string gameCode, string gameType, int pageIndex, int pageSize)
        {
            try
            {
                var blog = new BlogEntity();
                blog.ProfileUserInfo = WebRedisHelper.QueryProfileUserInfo(userId); // WCFClients.GameClient.QueryProfileUserInfo(userId);
                blog.ProfileBonusLevel = WebRedisHelper.QueryProfileBonusLevelInfo(userId); // WCFClients.GameClient.QueryProfileBonusLevelInfo(userId);
                blog.ProfileLastBonus = WebRedisHelper.QueryProfileLastBonusCollection(userId);// WCFClients.GameClient.QueryProfileLastBonusCollection(userId);
                blog.ProfileDataReport = WebRedisHelper.QueryProfileDataReport(userId);// WCFClients.GameClient.QueryProfileDataReport(userId);
                blog.FollowerCount = WebRedisHelper.QueryTogetherFollowerCount(userId);// WCFClients.GameQueryClient.QueryTogetherFollowerCount(userId);
                blog.UserCurrentOrderInfo = WebRedisHelper.QueryUserCurrentOrderInfoCollection(userId, (gameCode == "SZC" ? gameType : gameCode));// WCFClients.GameClient.QueryUserCurrentOrderList(userId, (gameCode == "SZC" ? gameType : gameCode), UserToken, pageIndex, pageSize);
                blog.BonusOrderInfo = WebRedisHelper.QueryBonusOrderInfoCollection(userId, gameCode, gameType);// WCFClients.GameQueryClient.QueryBonusInfoList(userId, (gameCode == "SZC" ? gameType : gameCode), (gameCode == "SZC" ? "" : gameType), "", "", "", pageIndex, pageSize, UserToken);
                blog.CreateTime = DateTime.Now;
                return blog;
            }
            catch (Exception)
            {
                return new BlogEntity();
            }
        }
        /// <summary>
        /// 历史战绩
        /// </summary>
        public ActionResult standings(string id)
        {
            //return Redirect("/upgrade/closeFunc.html");

            var pageIndex = 0;
            var pageSize = 30;
            var gameCode = string.IsNullOrEmpty(Request["gameCode"]) ? "SZC" : Request["gameCode"].ToUpper();
            var gameType = string.IsNullOrEmpty(Request["gameType"]) ? "SSQ" : Request["gameType"].ToUpper();
            if (string.IsNullOrEmpty(gameType))
            {
                switch (gameCode)
                {
                    case "JCZQ":
                        gameType = "BRQSPF";
                        break;
                    case "BJDC":
                        gameType = "SPF";
                        break;
                    case "JCLQ":
                        gameType = "SF";
                        break;
                    case "CTZQ":
                        gameType = "T14C";
                        break;
                    case "SZC":
                        gameType = "SSQ";
                        break;
                    default:
                        break;
                }
            }
            //var blog = LoadBlogEntityStandings(id, gameCode, gameType, pageIndex, pageSize);
            var blog = QueryBlogEntityStandings(id, gameCode, gameType, pageIndex, pageSize); 
            //new BlogEntity
            //{
            //    BonusOrderInfo = new BonusOrderInfoCollection(),
            //    CreateTime = DateTime.Now,
            //    FollowerCount = 0,
            //    ProfileBonusLevel = new ProfileBonusLevelInfo(),
            //    ProfileDataReport = new ProfileDataReport(),
            //    ProfileLastBonus = new ProfileLastBonusCollection(),
            //    ProfileUserInfo = new ProfileUserInfo(),
            //    UserBeedingListInfo = new UserBeedingListInfoCollection(),
            //    UserCurrentOrderInfo = new UserCurrentOrderInfoCollection(),
            //};// QueryBlogEntityStandings(id, gameCode, gameType, pageIndex, pageSize);
            ViewBag.User = CurrentUser;
            ViewBag.UserId = id;
            ViewBag.UserInfo = blog.ProfileUserInfo;
            ViewBag.BonusLevel = blog.ProfileBonusLevel;
            ViewBag.BonusListZj = blog.ProfileLastBonus;
            ViewBag.DataReport = blog.ProfileDataReport;
            ViewBag.BonusList = blog.BonusOrderInfo;
            ViewBag.Count = blog.FollowerCount;
            ViewBag.CurrentOrder = blog.UserCurrentOrderInfo;
            ViewBag.GameCode = gameCode;
            ViewBag.GameType = gameType;
            return View();
        }

    }
}