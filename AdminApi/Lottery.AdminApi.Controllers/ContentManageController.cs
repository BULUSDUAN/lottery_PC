using EntityModel.Enum;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.ORM.Helper;
using Lottery.AdminApi.Model.HelpModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using KaSon.FrameWork.Common.ExceptionEx;
using KaSon.FrameWork.Common.Utilities;
using EntityModel;
using EntityModel.Communication;
using KaSon.FrameWork.Common.Net;
using Lottery.AdminApi.Controllers.CommonFilterActtribute;

namespace Lottery.AdminApi.Controllers
{
    [Area("api")]
    [ReusltFilter]
    [CheckLogin]
    public class ContentManageController : BaseController
    {
        #region 公告管理
        /// <summary>
        /// 公告管理
        /// </summary>
        public IActionResult NoticeManage(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("N101"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                //bool fbgg = false;
                //bool xggg = false;
                //bool jyqy = false;
                //if (CheckRights("FBGG100"))
                //    fbgg = true;
                //if (CheckRights("XGGG110"))
                //    xggg = true;
                //if (CheckRights("JYGG120"))
                //    jyqy = true;
                //ViewBag.Fbgg = fbgg;
                //ViewBag.Xggg = xggg;
                //ViewBag.JyQyqy = jyqy;
                string key = p.key;
                string statusStr = p.status;
                string priorityStr = p.priority;
                string isPutTopStr = p.isPutTop;
                string pageIndexStr = p.pageIndex;
                string pageSizeStr = p.pageSize;
                var Status = (EnableStatus)Convert.ToInt32(string.IsNullOrEmpty(statusStr) ? "9" : statusStr);
                var Priority = Convert.ToInt32(string.IsNullOrEmpty(priorityStr) ? "-1" : priorityStr);
                var IsPutTop = Convert.ToInt32(string.IsNullOrEmpty(isPutTopStr) ? "-1" : isPutTopStr);
                var PageIndex = string.IsNullOrEmpty(pageIndexStr) ? base.PageIndex : Convert.ToInt32(pageIndexStr);
                var PageSize = string.IsNullOrEmpty(pageSizeStr) ? base.PageSize : Convert.ToInt32(pageSizeStr);
                var service = new AdminService();
                var NoticeResult = service.QueryManagementBulletinCollection("%" + key + "%", Status, Priority,
                    IsPutTop, PageIndex, PageSize);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = NoticeResult
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        /// <summary>
        /// 禁用/启用
        /// </summary>
        public IActionResult DisnableOrEnableBullein(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("JYGG120"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string bulletinId = p.bulletinId;
                string enableStatus = p.enableStatus;
                var id = Convert.ToInt64(PreconditionAssert.IsNotEmptyString(bulletinId, "修改指定参数ID丢失"));
                var status = Convert.ToInt32(PreconditionAssert.IsNotEmptyString(enableStatus, "启用或禁用类型丢失"));
                var service = new AdminService();
                //if (status == (int)EnableStatus.Enable)
                //{
                //    var result = service.ChangeBulleinStatus(id, EnableStatus.Enable);
                //}
                //else
                //{
                //    var result = service.DisnableBullein(id);
                //}
                var result = service.ChangeBulleinStatus(id, (EnableStatus)status,CurrentUser.UserId);
                //new SiteSettingsController().BulletinInner();
                return Json(new LotteryServiceResponse {  Code = result.IsSuccess? AdminResponseCode.成功: AdminResponseCode.失败, Message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        ///// <summary>
        ///// 启用
        ///// </summary>
        //public JsonResult EnableBullein()
        //{
        //    try
        //    {
        //        if (!CheckRights("JYGG120"))
        //            throw new Exception("对不起，您的权限不足！");
        //        var bulletinId = Convert.ToInt64(PreconditionAssert.IsNotEmptyString(Request["bulletinId"], "修改指定参数ID丢失"));
        //        var result = base.ExternalClient.EnableBullein(bulletinId, CurrentUser.UserToken);
        //        try
        //        {
        //            new SiteSettingsController().BulletinInner();
        //        }
        //        catch
        //        {
        //        }
        //        return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Msg = ex.Message });
        //    }
        //}

        /// <summary>
        /// 查询公告（根据Id查询）
        /// </summary>
        public IActionResult NoticeInfo(LotteryServiceRequest entity)
        {
            try
            {
                //if (!CheckRights("FBGG100"))
                //    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string bulletinIdStr = p.bulletinId;
                if (string.IsNullOrEmpty(bulletinIdStr))
                {
                    throw new Exception("指定参数ID丢失");
                }
                int bulletinId = Convert.ToInt32(bulletinIdStr);
                var service = new AdminService();
                var Bulletin = service.QueryManagementBulletinDetailById(bulletinId);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = Bulletin
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        /// <summary>
        /// 发布公告
        /// </summary>
        //[ValidateInput(false)]
        public IActionResult PublishNotice(LotteryServiceRequest entity)
        {
            if (!CheckRights("FBGG100"))
                throw new Exception("对不起，您的权限不足！");
            var bulletin = new E_SiteMessage_Bulletin_List();
            var service = new AdminService();
            var p = JsonHelper.Decode(entity.Param);
            string title = p.title;
            string content = p.content;
            string effectiveFrom = p.effectiveFrom;
            string effectiveTo = p.effectiveTo;
            string priority = p.priority;
            string isPutTop = p.isPutTop;
            string status = p.status;
            string bulletinAgent = p.bulletinAgent;
            string chkBuildStatic = p.chkBuildStatic;
            string publishId = p.publishId;
            try
            {
                bulletin.Title = PreconditionAssert.IsNotEmptyString(title, "公告标题不能为空");
                bulletin.Content = PreconditionAssert.IsNotEmptyString(content, "公告内容不能为空");
                PreconditionAssert.IsNotEmptyString(effectiveFrom, "有效时间不能为空");
                PreconditionAssert.IsNotEmptyString(effectiveTo, "有效时间不能为空");
                if (!string.IsNullOrEmpty(effectiveFrom))
                {
                    bulletin.EffectiveFrom = Convert.ToDateTime(effectiveFrom);
                }
                if (!string.IsNullOrEmpty(effectiveTo))
                {
                    bulletin.EffectiveTo = Convert.ToDateTime(effectiveTo);
                }
                bulletin.Priority = int.Parse(PreconditionAssert.IsNotEmptyString(priority, "优先级参数不正确"));
                bulletin.IsPutTop = int.Parse(PreconditionAssert.IsNotEmptyString(isPutTop, "置顶参数不正确"));
                bulletin.Status = Convert.ToInt32(PreconditionAssert.IsNotEmptyString(status, "状态参数不正确"));
                bulletin.BulletinAgent = Convert.ToInt32(PreconditionAssert.IsNotEmptyString(bulletinAgent, "参数不正确"));
                try
                {
                    //同步生成静态页
                    if (chkBuildStatic != null)
                    {
                        if (Convert.ToBoolean(chkBuildStatic))
                        {
                            var arrPageType = new string[] { "10", "70" };
                            foreach (var item in arrPageType)
                            {
                                SendBuildStaticDataNotice(item, string.Empty);
                            }
                        }
                    }
                }
                catch
                {
                }
                if (string.IsNullOrEmpty(publishId))
                {
                    var noticeResult = service.PublishBulletin(bulletin,CurrentUser.UserId);
                    try
                    {
                        new SiteSettingsController().BulletinInner();
                    }
                    catch
                    {
                    }
                    return Json(new { IsSuccess = noticeResult.IsSuccess, Msg = noticeResult.Message });
                }
                else
                {
                    int bulletinId = Convert.ToInt32(publishId);
                    bulletin.Id = bulletinId;
                    var noticeResult = service.UpdateBulletin(bulletin,CurrentUser.UserId);
                    try
                    {
                        new SiteSettingsController().BulletinInner();
                    }
                    catch
                    {
                    }
                    return Json(new LotteryServiceResponse { Code = noticeResult.IsSuccess? AdminResponseCode.成功 : AdminResponseCode.失败, Message = noticeResult.Message });
                }
                
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        #endregion

        #region 文章管理
        /// <summary>
        /// 文章管理
        /// </summary>
        public IActionResult ArticleManage(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("N103"))
                    throw new Exception("对不起，您的权限不足！");
                //bool fbwz = false;
                //bool scwz = false;
                //bool xgwz = false;
                //bool gxxlh = false;
                //if (CheckRights("FBWZ100"))
                //    fbwz = true;
                //if (CheckRights("SCWZ120"))
                //    scwz = true;
                //if (CheckRights("XGWZ110"))
                //    xgwz = true;
                //if (CheckRights("GXXLH130"))
                //    gxxlh = true;
                //ViewBag.Fbwz = fbwz;
                //ViewBag.Scwz = scwz;
                //ViewBag.Xgwz = xgwz;
                //ViewBag.Gxxlh = gxxlh;
                //ViewBag.GameList = this.GameIssuseClient.QueryGameList(CurrentUser.UserToken);
                var p = JsonHelper.Decode(entity.Param);
                string key = p.key;
                string category = p.category;
                string gameCode = p.gameCode;
                string pageIndexStr = p.gameCode;
                string pageSizeStr = p.gameCode;
                key = string.IsNullOrEmpty(key) ? "" : key;
                category = string.IsNullOrEmpty(category) ? "" : category;
                gameCode = string.IsNullOrEmpty(gameCode) ? "" : gameCode;
                var service = new AdminService();
                var pageIndex = string.IsNullOrEmpty(pageIndexStr) ? base.PageIndex : Convert.ToInt32(pageIndexStr);
                var pageSize = string.IsNullOrEmpty(pageSizeStr) ? base.PageSize : Convert.ToInt32(pageSizeStr);
                var ArticleResult = service.QueryArticleList("%" + key + "%", gameCode,category, pageIndex, pageSize);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = ArticleResult
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        /// <summary>
        /// 查询选择
        /// </summary>
        /// <returns></returns>
        public IActionResult ArticleManageQueryGameList()
        {
            try
            {
                var service = new AdminService();
                var GameList = service.QueryGameList();
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = GameList
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        /// <summary>
        /// 查询文章（根据Id）
        /// </summary>
        public IActionResult ArticleInfo(LotteryServiceRequest entity)
        {
            try
            {
                //ViewBag.GameList = this.GameIssuseClient.QueryGameList(CurrentUser.UserToken);
                var service = new AdminService();
                var p = JsonHelper.Decode(entity.Param);
                string articleId = p.articleId;
                if (string.IsNullOrEmpty(articleId))
                {
                    throw new Exception("请先选择需要修改的文章");
                }
                var Article = service.QueryArticleById_Admin(articleId);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = Article
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        /// <summary>
        /// 发布文章
        /// </summary>
        public IActionResult ArticleOperate(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("FBWZ100"))
                    throw new Exception("对不起，您的权限不足！");
                //var article = new ArticleInfo_Add();
                var article = new E_SiteMessage_Article_List();
                var p = JsonHelper.Decode(entity.Param);
                string category = p.category;
                string isRedTitle = p.isRedTitle;
                string description = p.description;
                string title = p.title;
                string keyWords = p.keyWords;
                string descContent = p.descContent;
                string gameCode = p.gameCode;
                string articleId = p.articleId;
                article.Category = PreconditionAssert.IsNotEmptyString(category, "分类不能为空");
                article.IsRedTitle = Convert.ToBoolean(string.IsNullOrEmpty(isRedTitle)) ? false : Convert.ToBoolean(isRedTitle);
                article.CreateUserDisplayName = CurrentUser.DisplayName;
                article.CreateUserKey = CurrentUser.UserId;
                article.Description = PreconditionAssert.IsNotEmptyString(description, "内容不能为空");
                article.Title = PreconditionAssert.IsNotEmptyString(title, "标题不能为空");
                article.KeyWords = keyWords;
                article.DescContent = descContent;
                article.GameCode = string.IsNullOrEmpty(gameCode) ? "" : gameCode;
                var service = new AdminService();
                var result = new CommonActionResult();
                if (!string.IsNullOrEmpty(articleId))
                {
                    article.Id = articleId;
                    result = service.UpdateArticle(article);
                }
                else
                {
                    result = service.SubmitArticle(article);
                }
                try
                {
                    var originCategory = p.originCategory;
                    List<string> gameCodeList = new List<string>();
                    if (!string.IsNullOrEmpty(gameCode))
                    {
                        gameCodeList.Add(gameCode);
                    }
                    if (!string.IsNullOrEmpty(originCategory) && !gameCodeList.Contains(originCategory))
                    {
                        gameCodeList.Add(originCategory);
                    }
                    new SiteSettingsController().ArticleInner(gameCodeList.ToArray());
                }
                catch
                {
                }
                try
                {
                    string chkBuildStatic = p.chkBuildStatic;
                    //同步生成静态页
                    if (chkBuildStatic != null)
                    {
                        if (Convert.ToBoolean(chkBuildStatic))
                        {
                            var arrPageType = new string[] { "10", "50", "60" };
                            foreach (var item in arrPageType)
                            {
                                SendBuildStaticDataNotice(item, string.Empty);
                            }
                        }
                    }
                }
                catch
                {
                }
                return Json(new LotteryServiceResponse
                {
                    Code = result.IsSuccess?AdminResponseCode.成功:AdminResponseCode.失败,
                    Message = result.Message
                });

            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        public IActionResult DeleteArticle(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("SCWZ120"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string articleId = p.articleId;
                string gameCode = p.gameCode;
                articleId = PreconditionAssert.IsNotEmptyString(articleId, "删除编号不能为空");
                var service = new AdminService();
                var result = service.DeleteArticle(articleId);
                gameCode = string.IsNullOrEmpty(gameCode) ? "" : gameCode;
                try
                {
                    List<string> gameCodeList = new List<string>();
                    gameCodeList.Add(gameCode);
                    new SiteSettingsController().ArticleInner(gameCodeList.ToArray());
                }
                catch
                {
                }
                return Json(new LotteryServiceResponse
                {
                    Code = result.IsSuccess ? AdminResponseCode.成功 : AdminResponseCode.失败,
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }


        public IActionResult UpdateArticleIndex(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("GXXLH130"))
                    throw new Exception("对不起，您的权限不足！");
                //     序号描述。ID1,1|ID2,2
                var p = JsonHelper.Decode(entity.Param);
                string indexDescription= p.indexDescription;
                string gameCode = p.gameCode;
                var service = new AdminService();
                indexDescription = PreconditionAssert.IsNotEmptyString(indexDescription, "序号修改ID组异常");
                var result = service.UpdateArticleIndex(indexDescription);
                gameCode = string.IsNullOrEmpty(gameCode) ? "" : gameCode;
                try
                {
                    List<string> gameCodeList = new List<string>();
                    gameCodeList.Add(gameCode);
                    new SiteSettingsController().ArticleInner(gameCodeList.ToArray());
                }
                catch
                {
                }
                return Json(new LotteryServiceResponse
                {
                    Code = result.IsSuccess ? AdminResponseCode.成功 : AdminResponseCode.失败,
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        #endregion

        #region 广告图

        public ActionResult BannerManage(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("N104"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string queryType = p.queryType;
                //bool tjgg = false;
                //bool bcgg = false;
                //bool scgg = false;
                //if (CheckRights("TJGG100"))
                //    tjgg = true;
                //if (CheckRights("SCGG120"))
                //    scgg = true;
                //if (CheckRights("BCGG110"))
                //    bcgg = true;
                //ViewBag.Tjgg = tjgg;
                //ViewBag.Scgg = scgg;
                //ViewBag.Bcgg = bcgg;
                var service = new AdminService();
                queryType = string.IsNullOrEmpty(queryType) ? "10" : queryType;
                //var bannerType = (BannerType)int.Parse(queryType);
                var Lhlist = service.QuerySitemessageBanngerList_Web(int.Parse(queryType));
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = Lhlist
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public IActionResult RequestBanner(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("BCGG110"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string oldIndex = p.oldIndex;
                string title = p.title;
                string ggtype = p.ggType;
                string targetUrl = p.targetUrl;
                string imageUrl = p.ImageUrl;
                E_Sitemessage_Banner siteInfo = new E_Sitemessage_Banner();
                siteInfo.BannerId = int.Parse(oldIndex);
                siteInfo.BannerTitle = title;
                siteInfo.BannerType = int.Parse(ggtype);
                //var loadfile = Request.Files["loadfile"];
                siteInfo.JumpUrl = Request.Form["targeturl"];
                siteInfo.ImageUrl = imageUrl;
                siteInfo.IsEnable = true;
                var service = new AdminService();
                service.UpdateBannerInfo(siteInfo);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "更新成功",
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        public IActionResult RequestBannerByAdd(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("TJGG100"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string hd_Index = p.hd_Index;
                string ggtype = p.ggtype;
                string title = p.title;
                string targeturl = p.targeturl;
                string imageUrl = p.ImageUrl;
                E_Sitemessage_Banner siteInfo = new E_Sitemessage_Banner();
                if (hd_Index == null)
                    throw new Exception("序号不能为空！");
                siteInfo.BannerIndex = Convert.ToInt32(hd_Index) + 1;
                siteInfo.BannerTitle = title;
                siteInfo.BannerType = int.Parse(ggtype);
                siteInfo.JumpUrl = targeturl;
                siteInfo.ImageUrl = imageUrl;
                siteInfo.IsEnable = true;
                var service = new AdminService();
                service.AddBannerInfo(siteInfo);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "添加成功",
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        public JsonResult RequestBannerByDelete(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("SCGG120"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string oldindexStr = p.oldindex;
                var oldindex = int.Parse(oldindexStr);
                var service = new AdminService();
                service.DeleteBanner(oldindex);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "删除成功",
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        #endregion

        #region 静态文件
        public JsonResult SendBuildStaticPageNotice(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var key = p.key;
                var pageType = p.pageType;
                var code = Encipherment.MD5(string.Format("Home_BuildSpecificPage_{0}", pageType), Encoding.UTF8);

                var urlArray = (ConfigHelper.AllConfigInfo["SendUrl_Web"]==null?"": ConfigHelper.AllConfigInfo["SendUrl_Web"].ToString()).Split('|');
                var logList = new List<string>();
                foreach (var domain in urlArray)
                {
                    if (string.IsNullOrEmpty(domain)) continue;

                    try
                    {
                        var webSiteUrl = string.Format("{0}/{1}?pageType={2}&code={3}&key={4}", domain, "StaticHtml/BuildSpecificPage", pageType, code, key);
                        var str = PostManager.Get(webSiteUrl, Encoding.UTF8, timeoutSeconds: 60);
                        logList.Add(string.Format("域名{0}生成结果{1}", domain, str));
                    }
                    catch (Exception ex)
                    {
                        logList.Add(string.Format("域名{0}生成结果{1}", domain, ex.Message));
                    }
                }
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = string.Join(Environment.NewLine, logList),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        public JsonResult SendBuildStaticPageNotice_Wap(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var key = p.key;
                var pageType = p.pageType;
                var code = Encipherment.MD5(string.Format("Home_BuildSpecificPage_{0}", pageType), Encoding.UTF8);
                var urlArray = (ConfigHelper.AllConfigInfo["SendUrl_Wap"] == null ? "" : ConfigHelper.AllConfigInfo["SendUrl_Wap"].ToString()).Split('|');
                var logList = new List<string>();
                foreach (var domain in urlArray)
                {
                    if (string.IsNullOrEmpty(domain)) continue;
                    try
                    {
                        var webSiteUrl = string.Format("{0}/{1}?pageType={2}&code={3}&key={4}", domain, "StaticHtml/BuildSpecificPage", pageType, code, key);
                        var result = PostManager.Get(webSiteUrl, Encoding.UTF8, timeoutSeconds: 60);
                        logList.Add(string.Format("域名{0}生成结果{1}", domain, result));
                    }
                    catch (Exception ex)
                    {
                        logList.Add(string.Format("域名{0}生成结果{1}", domain, ex.Message));
                    }
                }
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = string.Join(Environment.NewLine, logList),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        public JsonResult SendBuildStaticPageNotice_App(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var key = p.key;
                var pageType = p.pageType;
                //var key = Request["key"];
                //var pageType = Request["pageType"];
                var code = Encipherment.MD5(string.Format("Home_BuildSpecificPage_{0}", pageType), Encoding.UTF8);
                var urlArray = (ConfigHelper.AllConfigInfo["SendUrl_App"] == null ? "" : ConfigHelper.AllConfigInfo["SendUrl_App"].ToString()).Split('|');
                var logList = new List<string>();
                foreach (var domain in urlArray)
                {
                    if (string.IsNullOrEmpty(domain)) continue;
                    try
                    {
                        var webSiteUrl = string.Format("{0}/{1}?pageType={2}&code={3}&key={4}", domain, "StaticHtml/BuildSpecificPage", pageType, code, key);
                        var result = PostManager.Get(webSiteUrl, Encoding.UTF8, timeoutSeconds: 60);
                        logList.Add(string.Format("域名{0}生成结果{1}", domain, result));
                    }
                    catch (Exception ex)
                    {
                        logList.Add(string.Format("域名{0}生成结果{1}", domain, ex.Message));
                    }
                }
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = string.Join(Environment.NewLine, logList),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        #endregion
    }
}
