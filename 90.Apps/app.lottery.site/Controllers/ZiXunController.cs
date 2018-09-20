using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using app.lottery.site.Controllers;
using Common.Utilities;
using External.Core.SiteMessage;
using GameBiz.Core;
using app.lottery.site.cbbao.Models;
using System.IO;
using System.Text;

namespace app.lottery.site.iqucai.Controllers
{
    public class ZiXunController : BaseController
    {
        public PartialViewResult ZiXunRight(string id)
        {
            if (string.IsNullOrEmpty(id))
                id = "szczx";
            ArticleInfo_QueryCollection data = null;
            switch (id)
            {
                case "jdxw":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("FocusCMS", "", 0, 7);
                    break;
                case "ssdp":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Match_Comment", "", 0, 7);
                    break;
                case "szczx":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|FC3D|DLT|PL3|CQSSC|JX11X5", 0, 7);
                    break;
                case "gpczx":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "CQSSC|JX11X5", 0, 7);
                    break;
                case "jjczx":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCZQ|JCLQ", 0, 7);
                    break;
                case "ssq_dlt":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|DLT", 0, 7);
                    break;
                case "zc_siliu":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "T14C|TR9|T6BQC|T4CJQ", 0, 7);
                    break;
                case "zd_jl":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCLQ|BJDC", 0, 7);
                    break;
                case "fc3d_pl3_pl5":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "FC3D|PL3|PL5", 0, 7);
                    break;
                case "zjxw":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("BonusCMS", "", 0, 7);
                    break;
                case "cpbk":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_Know", "", 0, 7);
                    break;
                default:
                    break;
            }
            ViewBag.category = id;
            ViewBag.DataSource = data;
            return PartialView();
        }
        /// <summary>
        /// 资讯首页
        /// </summary>
        //public ActionResult HotCaiXun()
        //{
        //    ViewBag.jdxw = WCFClients.ExternalClient.QueryArticleList_YouHua("FocusCMS", "", 0, 6);
        //    ViewBag.ssdp = WCFClients.ExternalClient.QueryArticleList_YouHua("Match_Comment", "", 0, 7);
        //    ViewBag.szc = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|FC3D|DLT|PL3|CQSSC|JX11X5", 0, 7);
        //    ViewBag.gpc = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "CQSSC|JX11X5", 0, 7);
        //    ViewBag.jjc = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCZQ|JCLQ", 0, 7);
        //    ViewBag.s_d = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|DLT", 0, 7);
        //    ViewBag.z_s_l = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "T14C|TR9|T6BQC|T4CJQ", 0, 7);
        //    ViewBag.z_j = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCLQ|BJDC", 0, 7);
        //    ViewBag.f_p = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "FC3D|PL3|PL5", 0, 7);
        //    ViewBag.zjxw = WCFClients.ExternalClient.QueryArticleList_YouHua("BonusCMS", "", 0, 5);
        //    ViewBag.r_cpbk = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_Know", "", 0, 10);
        //    ViewBag.Ads = WCFClients.ExternalClient.QuerySitemessageBanngerList_Web(BannerType.Index_ZiXun);
        //    return View();
        //}

        public ActionResult More(string id)
        {
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            id = string.IsNullOrEmpty(id) ? "jdxw" : id;
            ArticleInfo_QueryCollection data = null;
            switch (id)
            {
                case "jdxw":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("FocusCMS", "", ViewBag.pageNo, 30);
                    break;
                case "ssdp":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Match_Comment", "", ViewBag.pageNo, 30);
                    break;
                case "szczx":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|FC3D|DLT|PL3|CQSSC|JX11X5", ViewBag.pageNo, 30);
                    break;
                case "gpczx":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "CQSSC|JX11X5", ViewBag.pageNo, 30);
                    break;
                case "jjczx":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCZQ|JCLQ", ViewBag.pageNo, 30);
                    break;
                case "ssq_dlt":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|DLT", ViewBag.pageNo, 30);
                    break;
                case "zc_siliu":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "T14C|TR9|T6BQC|T4CJQ", ViewBag.pageNo, 30);
                    break;
                case "zd_jl":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCLQ|BJDC", ViewBag.pageNo, 30);
                    break;
                case "fc3d_pl3_pl5":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "FC3D|PL3|PL5", ViewBag.pageNo, 30);
                    break;
                case "zjxw":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("BonusCMS", "", ViewBag.pageNo, 30);
                    break;
                case "cpbk":
                    data = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_Know", "", ViewBag.pageNo, 30);
                    break;
                default:
                    break;
            }
            ViewBag.DataSource = data;
            ViewBag.category = id;

            return View();
        }

        public ActionResult hotcaixun_cpzj()
        {
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.pageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 25 : int.Parse(Request["pageSize"]);
            var category = "CPZJ";
            ViewBag.category = category;
            ViewBag.hotDataSource = WCFClients.ExternalClient.QueryArticleList("", "", category, ViewBag.pageNo, ViewBag.PageSize, UserToken);
            return View();
        }
        public ActionResult hotcaixun_hot()
        {
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.pageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 25 : int.Parse(Request["pageSize"]);
            var category = "HOT";
            ViewBag.category = category;
            ViewBag.hotDataSource = WCFClients.ExternalClient.QueryArticleList("", "", category, ViewBag.pageNo, ViewBag.PageSize, UserToken);
            return View();
        }
        public ActionResult hotcaixun_cpbk()
        {
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.pageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 25 : int.Parse(Request["pageSize"]);
            var category = "CPBK";
            ViewBag.category = category;
            ViewBag.hotDataSource = WCFClients.ExternalClient.QueryArticleList("", "", category, ViewBag.pageNo, ViewBag.PageSize, UserToken);
            return View();
        }
        /// <summary>
        /// 赛事点评
        /// </summary>
        public ActionResult CompetitionDian()
        {
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.pageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 25 : int.Parse(Request["pageSize"]);
            var category = "LQDP|ZQDP";
            ViewBag.category = category;
            ViewBag.dpDataSource = WCFClients.ExternalClient.QueryArticleList("", "", category, ViewBag.pageNo, ViewBag.PageSize, UserToken);
            return View();
        }
        public ActionResult CompetitionDian_LQDP()
        {
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.pageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 25 : int.Parse(Request["pageSize"]);
            var category = "LQDP";
            ViewBag.category = category;
            ViewBag.dpDataSource = WCFClients.ExternalClient.QueryArticleList("", "", category, ViewBag.pageNo, ViewBag.PageSize, UserToken);
            return View();
        }
        public ActionResult CompetitionDian_ZQDP()
        {
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.pageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 25 : int.Parse(Request["pageSize"]);
            var category = "ZQDP";
            ViewBag.category = category;
            ViewBag.dpDataSource = WCFClients.ExternalClient.QueryArticleList("", "", category, ViewBag.pageNo, ViewBag.PageSize, UserToken);
            return View();
        }
        /// <summary>
        /// 赛事资讯
        /// </summary>
        public ActionResult competitionzixun()
        {
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.pageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 25 : int.Parse(Request["pageSize"]);
            var gameCode = "";
            ViewBag.gameCode = gameCode;
            ViewBag.ssDataSource = WCFClients.ExternalClient.QueryArticleList("", gameCode, "INFO", ViewBag.pageNo, ViewBag.PageSize, UserToken);
            return View();
        }
        public ActionResult competitionzixun_jczq()
        {
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.pageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 25 : int.Parse(Request["pageSize"]);
            var gameCode = "JCZQ";
            ViewBag.gameCode = gameCode;
            ViewBag.ssDataSource = WCFClients.ExternalClient.QueryArticleList("", gameCode, "INFO", ViewBag.pageNo, ViewBag.PageSize, UserToken);
            return View();
        }
        public ActionResult competitionzixun_jclq()
        {
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.pageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 25 : int.Parse(Request["pageSize"]);
            var gameCode = "JCLQ";
            ViewBag.gameCode = gameCode;
            ViewBag.ssDataSource = WCFClients.ExternalClient.QueryArticleList("", gameCode, "INFO", ViewBag.pageNo, ViewBag.PageSize, UserToken);
            return View();
        }
        public ActionResult competitionzixun_bjdc()
        {
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.pageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 25 : int.Parse(Request["pageSize"]);
            var gameCode = "BJDC";
            ViewBag.gameCode = gameCode;
            ViewBag.ssDataSource = WCFClients.ExternalClient.QueryArticleList("", gameCode, "INFO", ViewBag.pageNo, ViewBag.PageSize, UserToken);
            return View();
        }
        public ActionResult competitionzixun_ctzq()
        {
            ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
            ViewBag.pageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 25 : int.Parse(Request["pageSize"]);
            var gameCode = "CTZQ";
            ViewBag.gameCode = gameCode;
            ViewBag.ssDataSource = WCFClients.ExternalClient.QueryArticleList("", gameCode, "INFO", ViewBag.pageNo, ViewBag.PageSize, UserToken);
            return View();
        }

        public ActionResult TextPart(string id, string num)
        {
            try
            {
                PreconditionAssert.IsNotEmptyString(num, "文章不存在");
                ViewBag.Details = WCFClients.ExternalClient.QueryArticleById_Web(num);
                if (id == "jdxw")
                {
                    ViewBag.DataSource = WCFClients.ExternalClient.QueryArticleList_YouHua("FocusCMS", "", 0, 30);
                }
                if (id == "ssdp")
                {
                    ViewBag.DataSource = WCFClients.ExternalClient.QueryArticleList_YouHua("Match_Comment", "", 0, 30);
                }
                if (id == "szczx")
                {
                    ViewBag.DataSource = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|FC3D|DLT|PL3|CQSSC|JX11X5", 0, 30);
                }
                if (id == "gpczx")
                {
                    ViewBag.DataSource = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "CQSSC|JX11X5", 0, 30);

                }
                if (id == "jjczx")
                {
                    ViewBag.DataSource = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCZQ|JCLQ", 0, 30);

                }

                if (id == "ssq_dlt")
                {
                    ViewBag.DataSource = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|DLT", 0, 30);

                }
                if (id == "zc_siliu")
                {
                    ViewBag.DataSource = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "T14C|TR9|T6BQC|T4CJQ", 0, 30);

                }
                if (id == "zd_jl")
                {
                    ViewBag.DataSource = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCLQ|BJDC", 0, 30);

                }
                if (id == "fc3d_pl3_pl5")
                {
                    ViewBag.DataSource = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "FC3D|PL3|PL5", 0, 30);

                }
                if (id == "zjxw")
                {
                    ViewBag.DataSource = WCFClients.ExternalClient.QueryArticleList_YouHua("BonusCMS", "", 0, 30);

                }
                if (id == "cpbk")
                {
                    ViewBag.DataSource = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_Know", "", 0, 30);
                }
                ViewBag.category = id;
            }
            catch (Exception ex)
            {
                ViewBag.Details = new ArticleInfo_Query()
                {
                    GameCode = "",
                    Description = ex.Message,
                    CreateTime = DateTime.Now,
                    Category = "",
                    Title = ex.Message,
                };
            }
            return View();
        }

        [HttpPost]
        public JsonResult commit_comment(string textNum, string commentMain)
        {
            try
            {
                textNum = PreconditionAssert.IsNotEmptyString(textNum, "该文章不存在");
                commentMain = PreconditionAssert.IsNotEmptyString(commentMain, "评论内容不能为空");
                var pl = new ZixunComment();
                pl.UserDisplayName = CurrentUser.LoginInfo.LoginName;
                pl.TextNum = textNum;
                pl.Content = commentMain;
                pl.CreateTime = DateTime.Now.ToString();
                buildComment(pl);
                return Json(new { Succuss = true });
            }
            catch (Exception ex)
            {
                return Json(new { Succuss = false, Msg = ex.Message });
            }
        }

        private void buildComment(ZixunComment pl)
        {
            if (pl == null) return;
            if (string.IsNullOrEmpty(pl.TextNum)) return;
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "zixunComment");
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            var dataFileName = System.IO.Path.Combine(path, string.Format("textpart_{0}.json", pl.TextNum));

            var contentList = new List<ZixunComment>();
            if (System.IO.File.Exists(dataFileName))
            {
                var oldJson = System.IO.File.ReadAllText(dataFileName, Encoding.UTF8);
                contentList.AddRange(Common.JSON.JsonSerializer.Deserialize<List<ZixunComment>>(oldJson));
            }
            contentList.Add(pl);
            var json = Common.JSON.JsonSerializer.Serialize<List<ZixunComment>>(contentList);
            System.IO.File.WriteAllText(dataFileName, json, System.Text.Encoding.UTF8);
        }

        public PartialViewResult Comment(string id)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData", "zixunComment", string.Format("textpart_{0}.json", id));
            if (System.IO.File.Exists(path))
            {
                try
                {
                    var json = System.IO.File.ReadAllText(path, Encoding.UTF8);
                    var content = Common.JSON.JsonSerializer.Deserialize<List<ZixunComment>>(json);
                    ViewBag.content = content;
                }
                catch (Exception)
                {
                    ViewBag.content = new List<ZixunComment>();
                }
            }
            else
            {
                ViewBag.content = new List<ZixunComment>();
            }
            return PartialView();
        }

        /// <summary>
        /// 网站公告
        /// </summary>
        /// <returns></returns>
        public ActionResult notice()
        {
            var id = Request["Id"];
            ViewBag.NoticeList = WCFClients.ExternalClient.QueryDisplayBulletinCollection(BulletinAgent.Local, 0, 10, UserToken);
            ViewBag.Notice = WCFClients.ExternalClient.QueryDisplayBulletinDetailById(long.Parse(id));
            return View();
        }

        /// <summary>
        /// 公告列表
        /// </summary>
        /// <returns></returns>
        public ActionResult notecilist()
        {
            try
            {
                ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
                ViewBag.pageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 15 : int.Parse(Request["pageSize"]);
                ViewBag.NoticeList = WCFClients.ExternalClient.QueryDisplayBulletinCollection(BulletinAgent.Local, ViewBag.pageNo, ViewBag.pageSize, UserToken);
            }
            catch
            {
                ViewBag.NoticeList = null;
            }
            return View();
        }

        //文章列表页
        public ActionResult articlelist(string id)
        {
            try
            {
                id = string.IsNullOrEmpty(id) ? "" : id;
                ViewBag.GameCode = id.ToUpper();
                ViewBag.Category = string.IsNullOrEmpty(Request["category"]) ? "" : Request["category"];
                ViewBag.pageNo = string.IsNullOrEmpty(Request["pageNo"]) ? 0 : int.Parse(Request["pageNo"]);
                ViewBag.pageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 19 : int.Parse(Request["pageSize"]);
                ArticleInfo_QueryCollection list = WCFClients.ExternalClient.QueryArticleList("", id, ViewBag.Category, ViewBag.pageNo, ViewBag.pageSize, UserToken);
                list.ArticleList = list.ArticleList.OrderByDescending(a => a.CreateTime).ToList();
                ViewBag.ArticleList = list;

                ViewBag.NewWin = WCFClients.GameIssuseClient.QueryNewWinNumber(ViewBag.GameCode);
            }
            catch
            {
                ViewBag.ArticleList = null;
            }
            return View();
        }
        #region 子视图页面
        //专家分析
        public PartialViewResult expertana()
        {
            return PartialView();
        }

        //热点资讯
        public PartialViewResult hots()
        {
            return PartialView();
        }

        //文章列表页
        public PartialViewResult articlesider(string id)
        {
            try
            {
                id = string.IsNullOrEmpty(id) ? "" : id;

                ViewBag.NewWin = WCFClients.GameIssuseClient.QueryNewWinNumber(id);

                ViewBag.Bonus = WCFClients.GameQueryClient.QueryBonusInfoList("", id, "", "", "", "", 0, 10, UserToken);
            }
            catch
            {
            }
            return PartialView();
        }
        #endregion
    }
}
