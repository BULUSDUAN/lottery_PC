using EntityModel.Ticket;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.BonusPool
{
   public class BonusPoolManager : DBbase
    {
        private string jsons = "";
        public BonusPoolManager(string _jsons) {
            // _baseDir = Path;
            jsons = _jsons;
        }
        private string ReadFileString()
        {
            string strResult = this.jsons;

            if (!string.IsNullOrEmpty(strResult))
            {
                if (strResult.ToLower().StartsWith("var"))
                {
                    string[] strArray = strResult.Split('=');
                    if (strArray != null && strArray.Length == 2)
                    {
                        if (strArray[1].ToString().Trim().EndsWith(";"))
                        {
                            return strArray[1].ToString().Trim().TrimEnd(';');
                        }
                        return strArray[1].ToString().Trim();
                    }
                }
            }
            return strResult;
        }
        private SZC_BonusPoolInfo GetBonusPoolList_SZC(string gameCode, string issuseNumber)
        {
           // var fileName = string.Format(@"{2}\{0}\{0}_{1}.json", gameCode, issuseNumber, _baseDir);
            //if (!File.Exists(fileName))
            //{
            //    throw new ArgumentException("奖池不存在或尚未开奖");
            //}
            var json = ReadFileString();
            var resultList = JsonHelper.Deserialize<SZC_BonusPoolInfo>(json);
            return resultList;
        }
        public void UpdateBonusPool_SZC(string gameCode, string issuseNumber)
        {
            var bonusPoolList = GetBonusPoolList_SZC(gameCode, issuseNumber);
            using (DB)
            {
                DB.Begin();
                // tran.BeginTran();
                try
                {
                    var bonusManager = new Ticket_BonusManager();
                    foreach (var info in bonusPoolList.GradeList)
                    {
                        var entity = bonusManager.GetBonusPool(gameCode, "", issuseNumber, info.Grade);
                        if (entity == null)
                        {
                            entity = new EntityModel.T_Ticket_BonusPool
                            {
                                Id = string.Format("{0}|{1}|{2}", bonusPoolList.GameCode, bonusPoolList.IssuseNumber, info.Grade),
                                GameCode = gameCode,
                                GameType = "",
                                IssuseNumber = bonusPoolList.IssuseNumber,
                                BonusLevel = info.Grade,
                                BonusCount = info.BonusCount,
                                BonusLevelDisplayName = info.GradeName,
                                BonusMoney = info.BonusMoney,
                                WinNumber = bonusPoolList.WinNumber,
                                CreateTime = DateTime.Now,
                            };
                            bonusManager.AddBonusPool(entity);
                        }
                    }
                    DB.Commit();
                }
                catch (Exception)
                {
                    DB.Rollback();
                    throw;
                }
                

              
            }

            #region 对依赖奖池派奖的奖期派奖
            //var issuse = new Ticket_IssuseManager();
            //var iss_Entity = issuse.QueryIssuseListByIssuse(gameCode, issuseNumber);
            //if (iss_Entity != null && iss_Entity.Status == IssuseStatus.Awarding && !string.IsNullOrEmpty(iss_Entity.WinNumber))
            //{
            //    var noticeIdList = PrizeIssuse(gameCode, issuseNumber, bonusPoolList.WinNumber);
            //    try
            //    {
            //        new Thread(() =>
            //        {
            //            try
            //            {
            //                foreach (var noticeId in noticeIdList)
            //                {
            //                    // 开启线程发送通知
            //                    //new Thread(() => admin.SendNotification(noticeId)).Start();
            //                    SendNotification(noticeId);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Common.Log.LogWriterGetter.GetLogWriter().Write("TicketGateWayAdmin", "UpdateBonusPool_SZC", Common.Log.LogType.Information, "test", ex.ToString());
            //            }

            //        }).Start();
            //    }
            //    catch (Exception ex)
            //    {
            //        Common.Log.LogWriterGetter.GetLogWriter().Write("TicketGateWayAdmin2", "UpdateBonusPool_SZC", Common.Log.LogType.Information, "test", ex.ToString());
            //    }
            //    //foreach (var noticeId in noticeIdList)
            //    //{
            //    //    // 开启线程发送通知
            //    //    new Thread(() => SendNotification(noticeId)).Start();
            //    //}
            //}
            #endregion

        }
    }
}
