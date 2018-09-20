using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using app.lottery.site.Controllers;
using GameBiz.Core;

namespace app.lottery.site.iqucai.Controllers
{

    public class ExpertController : BaseController
    {

        #region 部分视图
        /// <summary>
        /// 登录状态
        /// </summary>
        public PartialViewResult State()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserBalance = CurrentUserBalance;
            return PartialView();
        }
        /// <summary>
        /// 底部
        /// </summary>
        public PartialViewResult Footer()
        {
            //赛事点评
            ViewBag.DPDataSource = WCFClients.ExternalClient.QueryArticleList("", "", "LQDP|ZQDP", 0, 6, UserToken);
            //赛事资讯
            ViewBag.SSDataSource = WCFClients.ExternalClient.QueryArticleList("", "", "INFO", 0, 6, UserToken);
            //合买名人
            string userId = string.Empty;
            if (CurrentUser != null)
                userId = CurrentUser.LoginInfo.UserId;
            ViewBag.SuperList = WCFClients.GameClient.QueryHotUserTogetherOrderList(userId);
            return PartialView();
        }

        #endregion

        /// <summary>
        /// 专家首页
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.Time = string.IsNullOrEmpty(Request["time"]) ? DateTime.Now.ToString("yyyy-MM-dd") : Request["time"];
            if (CurrentUser != null)
            {
                ViewBag.User = CurrentUser;
                ViewBag.UserToken = CurrentUser.LoginInfo.UserToken;
            }
            //推荐列表
            //ViewBag.Comment = WCFClients.ExperterClient.QueryExperterSchemeList(GameBiz.Core.ExperterType.iqucaiUser, "103757", ViewBag.Time, 0, 2);
            //名家列表
            ViewBag.ExperterList = WCFClients.ExperterClient.QueryExperterList(null, 0, 8);
            ViewBag.Tucao = WCFClients.ExperterClient.QueryExperterCommentsByTime(ViewBag.Time, GameBiz.Core.CommentsTpye.HomeIndx, "", 100);
            //命中榜
            ViewBag.ExpertWeek = WCFClients.ExperterClient.QueryExperterShootingList(ShootingType.Week, 0, 8);
            ViewBag.ExpertMonth = WCFClients.ExperterClient.QueryExperterShootingList(ShootingType.Month, 0, 8);
            ViewBag.ExpertTotal = WCFClients.ExperterClient.QueryExperterShootingList(ShootingType.Total, 0, 8);


            return View();
        }
        /// <summary>
        /// 推荐方案
        /// </summary>
        public ActionResult Recommend()
        {
            if (CurrentUser != null)
            {
                ViewBag.User = CurrentUser;
            }
            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 5 : int.Parse(Request["pageSize"]);
            ViewBag.PageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.PageByte = string.IsNullOrEmpty(Request["pageByte"]) ? 2 : int.Parse(Request["pageByte"]);
            ViewBag.Time = string.IsNullOrEmpty(Request["time"]) ? DateTime.Now.ToString("yyyy-MM-dd") : Request["time"];

            ViewBag.Tucao = WCFClients.ExperterClient.QueryExperterCommentsByTime(ViewBag.Time, GameBiz.Core.CommentsTpye.RecommendScheme, "", 100);
            //ViewBag.Comment = WCFClients.ExperterClient.QueryExperterSchemeList(GameBiz.Core.ExperterType.iqucaiUser, "", ViewBag.Time, ViewBag.PageNo, ViewBag.PageByte);
            ViewBag.Recommend = WCFClients.ExperterClient.QueryExperterAnalyzeSchemeList("", ViewBag.PageIndex, ViewBag.PageSize, ViewBag.Time);
            //命中榜
            ViewBag.ExpertWeek = WCFClients.ExperterClient.QueryExperterShootingList(ShootingType.Week, 0, 11);
            ViewBag.ExpertMonth = WCFClients.ExperterClient.QueryExperterShootingList(ShootingType.Month, 0, 11);
            ViewBag.ExpertTotal = WCFClients.ExperterClient.QueryExperterShootingList(ShootingType.Total, 0, 11);

            return View();
        }
        /// <summary>
        /// 所有专家
        /// </summary>
        public ActionResult AllExpert()
        {
            ViewBag.Time = string.IsNullOrEmpty(Request["time"]) ? DateTime.Now.ToString("yyyy-MM-dd") : Request["time"];

            ViewBag.AllExpert = WCFClients.ExperterClient.QueryExperterList(null, 0, 20);
            ViewBag.Tucao = WCFClients.ExperterClient.QueryExperterCommentsByTime(ViewBag.Time, GameBiz.Core.CommentsTpye.AllExperter, "", 100);
            //命中榜
            ViewBag.ExpertWeek = WCFClients.ExperterClient.QueryExperterShootingList(ShootingType.Week, 0, 8);
            ViewBag.ExpertMonth = WCFClients.ExperterClient.QueryExperterShootingList(ShootingType.Month, 0, 8);
            ViewBag.ExpertTotal = WCFClients.ExperterClient.QueryExperterShootingList(ShootingType.Total, 0, 8);

            return View();
        }
        /// <summary>
        /// 个人文章
        /// </summary>
        [CheckLogin]
        public ActionResult PersonArticle()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.Time = string.IsNullOrEmpty(Request["time"]) ? DateTime.Now.ToString("yyyy-MM-dd") : Request["time"];

            var experterid = Request["experterid"];
            var analyzeid = Request["AndanalyzeId"];
            ViewBag.experterid = experterid;
            ViewBag.analyzeid = analyzeid;

            ViewBag.ExperterList = WCFClients.ExperterClient.QueryExperterList(null, 0, 5);
            ViewBag.Tucao = WCFClients.ExperterClient.QueryExperterCommentsByTime(ViewBag.Time, GameBiz.Core.CommentsTpye.AnalyzeScheme, analyzeid, 100);

            ViewBag.Analyze = WCFClients.ExperterClient.QueryExperterAnalyzeId(analyzeid);
            ViewBag.User = WCFClients.ExperterClient.QueryExperterById(experterid);
            ViewBag.Correlation = WCFClients.ExperterClient.QueryExperterAnalyzeSchemeList(experterid, 0, 5, "");
            //命中榜
            ViewBag.ExpertWeek = WCFClients.ExperterClient.QueryExperterShootingList(ShootingType.Week, 0, 10);
            ViewBag.ExpertMonth = WCFClients.ExperterClient.QueryExperterShootingList(ShootingType.Month, 0, 10);
            ViewBag.ExpertTotal = WCFClients.ExperterClient.QueryExperterShootingList(ShootingType.Total, 0, 10);

            return View();
        }
        /// <summary>
        /// 个人专栏
        /// </summary>
        public ActionResult PersonSpacial()
        {
            ViewBag.CurrentUser = CurrentUser;

            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 5 : int.Parse(Request["pageSize"]);
            ViewBag.Time = string.IsNullOrEmpty(Request["time"]) ? DateTime.Now.ToString("yyyy-MM-dd") : Request["time"];
            var experterid = Request["UserId"];
            ViewBag.experterid = experterid;

            //ViewBag.Comment = WCFClients.ExperterClient.QueryExperterSchemeList(GameBiz.Core.ExperterType.iqucaiUser, experterid, ViewBag.Time, 0, 2);
            ViewBag.ExperterList = WCFClients.ExperterClient.QueryExperterList(null, 0, 10);
            ViewBag.Tucao = WCFClients.ExperterClient.QueryExperterCommentsByTime(ViewBag.Time, GameBiz.Core.CommentsTpye.Experter, experterid, 100);
            ViewBag.Recommend = WCFClients.ExperterClient.QueryUserAnalyzeList(experterid, ViewBag.Time, ViewBag.PageIndex, ViewBag.PageSize);

            ViewBag.User = WCFClients.ExperterClient.QueryExperterById(experterid);
            //命中榜
            ViewBag.ExpertWeek = WCFClients.ExperterClient.QueryExperterShootingList(ShootingType.Week, 0, 10);
            ViewBag.ExpertMonth = WCFClients.ExperterClient.QueryExperterShootingList(ShootingType.Month, 0, 10);
            ViewBag.ExpertTotal = WCFClients.ExperterClient.QueryExperterShootingList(ShootingType.Total, 0, 10);

            return View();
        }

        /// <summary>
        /// 吐槽
        /// </summary>
        public JsonResult AddTucao()
        {
            try
            {
                var content = Request["Content"];
                var commentstpye = Request["CommentsTpye"];
                var analyzeSchemeId = Request["AnalyzeId"];
                var recommendSchemeId = Request["RecommendId"];
                var userid = Request["UserId"];

                switch (commentstpye)
                {
                    case "10":
                        WCFClients.ExperterClient.AddExperterComments(userid, this.CurrentUser.LoginInfo.UserId, CommentsTpye.HomeIndx, analyzeSchemeId, recommendSchemeId, content, DateTime.Now.ToString("yyyy-MM-dd"));
                        break;
                    case "20":
                        WCFClients.ExperterClient.AddExperterComments(userid, this.CurrentUser.LoginInfo.UserId, CommentsTpye.Experter, analyzeSchemeId, recommendSchemeId, content, DateTime.Now.ToString("yyyy-MM-dd"));
                        break;
                    case "30":
                        WCFClients.ExperterClient.AddExperterComments(userid, this.CurrentUser.LoginInfo.UserId, CommentsTpye.AllExperter, analyzeSchemeId, recommendSchemeId, content, DateTime.Now.ToString("yyyy-MM-dd"));
                        break;
                    case "40":
                        WCFClients.ExperterClient.AddExperterComments(userid, this.CurrentUser.LoginInfo.UserId, CommentsTpye.RecommendScheme, analyzeSchemeId, recommendSchemeId, content, DateTime.Now.ToString("yyyy-MM-dd"));
                        break;
                    case "50":
                        WCFClients.ExperterClient.AddExperterComments(userid, this.CurrentUser.LoginInfo.UserId, CommentsTpye.AnalyzeScheme, analyzeSchemeId, recommendSchemeId, content, DateTime.Now.ToString("yyyy-MM-dd"));
                        break;
                }

                return Json(new { IsSucess = true, Msg = "吐槽成功，请等待审核结果！" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSucess = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 投票  （支持、反对）
        /// </summary>
        public JsonResult AddOpposeAndSupport()
        {
            try
            {
                var support = Request["Support"];
                var oppose = Request["Oppose"];
                var SchemeId = Request["SchemeId"];

                if (support == "1")
                {
                    WCFClients.ExperterClient.UpdateExperterVote(Vote.Support, SchemeId, this.CurrentUser.LoginInfo.UserToken);
                }
                if (oppose == "2")
                {
                    WCFClients.ExperterClient.UpdateExperterVote(Vote.Against, SchemeId, this.CurrentUser.LoginInfo.UserToken);
                }
                return Json(new { IsSucess = true, Msg = "投票成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSucess = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 判断购买方案详情
        /// </summary>
        public JsonResult BuyProgramDetails()
        {
            try
            {
                var experterid = Request["experterid"];
                var price = Request["price"];
                var analyzeid = Request["analyzeid"];
                var password = Request["password"];

                var analyze = WCFClients.ExperterClient.BuyExperterAnalyzeScheme(new ExperterAnalyzeTransactionInfo
               {
                   UserId = this.CurrentUser.LoginInfo.UserId,
                   Price = decimal.Parse(price),
                   AnalyzeId = analyzeid,
                   CreateTime = DateTime.Now,
                   ExperterId = experterid,
                   CurrentTime = DateTime.Now.ToString("yyyy-MM-dd"),
               }, password);
                return Json(new { IsSucess = true, Msg = "购买成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSucess = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 判断是否购买
        /// </summary>
        /// <returns></returns>
        public JsonResult IsBuyProgram()
        {
            try
            {
                var experterid = Request["experterid"];
                var analyzeid = Request["analyzeid"];
                var boo = WCFClients.ExperterClient.QueryIsBuyAnalyzeScheme(analyzeid, experterid);
                if (boo)
                {
                    return Json(new { IsSuccess = true, Msg = "已购买！" });//已购买
                }
                else
                {
                    return Json(new { IsSuccess = false, Msg = "未购买！" });//未购买
                }

            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });//异常
            }
        }
        /// <summary>
        /// 添加关注
        /// </summary>
        public JsonResult AddAttention()
        {
            try
            {
                var userid = Request["userId"];
                var r = WCFClients.GameClient.AttentionUser(userid, this.CurrentUser.LoginInfo.UserToken);

                return Json(new { IsSucess = r.IsSuccess, Msg = r.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSucess = false, Msg = ex.Message });
            }
        }
    }
}
