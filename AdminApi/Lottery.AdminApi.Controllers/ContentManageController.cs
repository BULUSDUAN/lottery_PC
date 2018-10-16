//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Lottery.AdminApi.Controllers
//{
//    public class ContentManageController:BaseController
//    {
//        #region 公告管理
//        /// <summary>
//        /// 公告管理
//        /// </summary>
//        public IActionResult NoticeManage()
//        {
//            try
//            {
//                if (!CheckRights("N101"))
//                    throw new Exception("对不起，您的权限不足！");
//                bool fbgg = false;
//                bool xggg = false;
//                bool jyqy = false;
//                if (CheckRights("FBGG100"))
//                    fbgg = true;
//                if (CheckRights("XGGG110"))
//                    xggg = true;
//                if (CheckRights("JYGG120"))
//                    jyqy = true;
//                ViewBag.Fbgg = fbgg;
//                ViewBag.Xggg = xggg;
//                ViewBag.JyQyqy = jyqy;
//                ViewBag.Key = string.IsNullOrWhiteSpace(Request["key"]) ? "" : Request["key"];
//                ViewBag.Status = (EnableStatus)Convert.ToInt32(string.IsNullOrWhiteSpace(Request["status"]) ? "9" : Request["status"]);
//                ViewBag.Priority = Convert.ToInt32(string.IsNullOrWhiteSpace(Request["priority"]) ? "-1" : Request["priority"]);
//                ViewBag.IsPutTop = Convert.ToInt32(string.IsNullOrWhiteSpace(Request["isPutTop"]) ? "-1" : Request["isPutTop"]);
//                ViewBag.PageIndex = string.IsNullOrWhiteSpace(Request["pageIndex"]) ? base.PageIndex : Convert.ToInt32(Request["pageIndex"]);
//                ViewBag.PageSize = string.IsNullOrWhiteSpace(Request["pageSize"]) ? base.PageSize : Convert.ToInt32(Request["pageSize"]);
//                ViewBag.NoticeResult = base.ExternalClient.QueryManagementBulletinCollection("%" + ViewBag.Key + "%", ViewBag.Status, ViewBag.Priority,
//                    ViewBag.IsPutTop, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//                return View();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        /// <summary>
//        /// 禁用
//        /// </summary>
//        public JsonResult DisnableBullein()
//        {
//            try
//            {
//                var bulletinId = Convert.ToInt64(PreconditionAssert.IsNotEmptyString(Request["bulletinId"], "修改指定参数ID丢失"));
//                var result = base.ExternalClient.DisnableBullein(bulletinId, CurrentUser.UserToken);
//                try
//                {
//                    new SiteSettingsController().BulletinInner();
//                }
//                catch
//                {
//                }
//                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }
//        /// <summary>
//        /// 启用
//        /// </summary>
//        public JsonResult EnableBullein()
//        {
//            try
//            {
//                var bulletinId = Convert.ToInt64(PreconditionAssert.IsNotEmptyString(Request["bulletinId"], "修改指定参数ID丢失"));
//                var result = base.ExternalClient.EnableBullein(bulletinId, CurrentUser.UserToken);
//                try
//                {
//                    new SiteSettingsController().BulletinInner();
//                }
//                catch
//                {
//                }
//                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }

//        /// <summary>
//        /// 发布公告
//        /// </summary>
//        public ActionResult NoticeInfo()
//        {
//            if (!string.IsNullOrWhiteSpace(Request["bulletinId"]))
//            {
//                long bulletinId = Convert.ToInt64(Request["bulletinId"]);
//                ViewBag.Bulletin = base.ExternalClient.QueryManagementBulletinDetailById(bulletinId, CurrentUser.UserToken);
//            }
//            return View();
//        }
//        /// <summary>
//        /// 发布公告
//        /// </summary>
//        [ValidateInput(false)]
//        public JsonResult PublishNotice(string content)
//        {
//            var bulletin = new BulletinInfo_Publish();
//            try
//            {
//                bulletin.Title = PreconditionAssert.IsNotEmptyString(Request.Form["title"], "公告标题不能为空");
//                bulletin.Content = PreconditionAssert.IsNotEmptyString(content, "公告内容不能为空");
//                if (!string.IsNullOrWhiteSpace(Request["effectiveFrom"]))
//                {
//                    bulletin.EffectiveFrom = Convert.ToDateTime(Request["effectiveFrom"]);
//                }
//                if (!string.IsNullOrWhiteSpace(Request["effectiveTo"]))
//                {
//                    bulletin.EffectiveTo = Convert.ToDateTime(Request["effectiveTo"]);
//                }
//                bulletin.Priority = int.Parse(PreconditionAssert.IsNotEmptyString(Request["priority"], "优先级参数不正确"));
//                bulletin.IsPutTop = int.Parse(PreconditionAssert.IsNotEmptyString(Request["isPutTop"], "置顶参数不正确"));
//                bulletin.Status = (EnableStatus)Convert.ToInt32(PreconditionAssert.IsNotEmptyString(Request["status"], "状态参数不正确"));
//                bulletin.BulletinAgent = (BulletinAgent)Convert.ToInt32(PreconditionAssert.IsNotEmptyString(Request["bulletinAgent"], "状态参数不正确"));

//                try
//                {
//                    //同步生成静态页
//                    if (Request["chkBuildStatic"] != null)
//                    {
//                        if (Convert.ToBoolean(Request["chkBuildStatic"]))
//                        {
//                            var arrPageType = new string[] { "10", "70" };
//                            foreach (var item in arrPageType)
//                            {
//                                SendBuildStaticDataNotice(item, string.Empty);
//                            }
//                        }
//                    }
//                }
//                catch
//                {
//                }
//                if (string.IsNullOrWhiteSpace(Request["publishId"]))
//                {
//                    var noticeResult = base.ExternalClient.PublishBulletin(bulletin, CurrentUser.UserToken);
//                    try
//                    {
//                        new SiteSettingsController().BulletinInner();
//                    }
//                    catch
//                    {
//                    }
//                    return Json(new { IsSuccess = noticeResult.IsSuccess, Msg = noticeResult.Message });
//                }
//                else
//                {
//                    long bulletinId = Convert.ToInt64(Request["publishId"]);
//                    var noticeResult = base.ExternalClient.UpdateBulletin(bulletinId, bulletin, CurrentUser.UserToken);
//                    try
//                    {
//                        new SiteSettingsController().BulletinInner();
//                    }
//                    catch
//                    {
//                    }
//                    return Json(new { IsSuccess = noticeResult.IsSuccess, Msg = noticeResult.Message });
//                }
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }
//        #endregion
//    }
//}
