using EntityModel;
using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MongoDB.Driver;
using EntityModel.Enum;
using EntityModel.LotteryJsonInfo;
using KaSon.FrameWork.Analyzer.AnalyzerFactory;
using KaSon.FrameWork.Common.Algorithms;

namespace KaSon.FrameWork.ORM.Helper
{
    public class IssuseBusiness:DBbase
    {
        private IMongoDatabase mDB;
        public IssuseBusiness(IMongoDatabase _mDB)
        {
            mDB = _mDB;
        }
        public IssuseBusiness()
        {
           
        }
        #region 北京单场私有方法
        private List<BJDCIssuseInfo> LoadBJDCIssuseLIst()
        {
            //var fileName = string.Format(@"{0}\{1}\Match_IssuseNumber_List.json", _baseDir, "BJDC");
            //var json = ReadFileString(fileName);
            //if (string.IsNullOrEmpty(json))
            //    return new List<MatchBiz.Core.BJDC_IssuseInfo>();
         //   return JsonSerializer.Deserialize<List<MatchBiz.Core.BJDC_IssuseInfo>>(json);

            var coll = mDB.GetCollection<BJDCIssuseInfo>("BJDCIssuseInfo");
            var builder = Builders<BJDCIssuseInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
          //  DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

            var filter = builder.Ne("IssuseNumber", ""); //& builder.Lte("GameCode", gameType) & builder.Lte("IssuseNumber", issuseNumber);
            var documents = coll.Find<BJDCIssuseInfo>(filter).ToList();

            return documents;
        }
        private List<BJDC_MatchInfo> LoadBJDCMatchList(string issuseNumber)
        {
          //  var fileName = string.Format(@"{2}\{0}\{1}\Match_List.json", "BJDC", issuseNumber, _baseDir);
            //if (!File.Exists(fileName))
            //    throw new ArgumentException("未找到数据文件" + fileName);

            //var json = ReadFileString(fileName);
            //if (string.IsNullOrEmpty(json))
            //    return new List<MatchBiz.Core.BJDC_MatchInfo>();
            //return JsonSerializer.Deserialize<List<MatchBiz.Core.BJDC_MatchInfo>>(json);

            var coll = mDB.GetCollection<BJDC_MatchInfo>("BJDC_MatchInfo");
            var builder = Builders<BJDC_MatchInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
            //  DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

            var filter = builder.Eq("IssuseNumber", issuseNumber); //& builder.Lte("GameCode", gameType) & builder.Lte("IssuseNumber", issuseNumber);
            var documents = coll.Find<BJDC_MatchInfo>(filter).ToList();

            return documents;
        }

        private List<BJDC_MatchResultInfo> LoadBJDCMatchResultList(string issuseNumber)
        {
            //var fileName = Path.Combine(_baseDir, string.Format(@"{0}\{1}\MatchResult_List.json", "BJDC", issuseNumber));
            //var fileName = string.Format(@"{2}\{0}\{1}\MatchResult_List.json", "BJDC", issuseNumber, _baseDir);
            ////if (!File.Exists(fileName))
            ////    throw new ArgumentException("未找到数据文件" + fileName);

            //var json = ReadFileString(fileName);
            //if (string.IsNullOrEmpty(json))
            //    return new List<BJDC_MatchResultInfo>();
            var coll = mDB.GetCollection<BJDC_MatchResultInfo>("BJDC_MatchResultInfo");
            var builder = Builders<BJDC_MatchResultInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
            //  DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

            var filter = builder.Eq("IssuseNumber", issuseNumber); //& builder.Lte("GameCode", gameType) & builder.Lte("IssuseNumber", issuseNumber);
            var documents = coll.Find<BJDC_MatchResultInfo>(filter).ToList();
            return documents;
        }
        private List<JCZQ_OZBMatchInfo> LoadJCZQ_OZB_GJ_MatchList()
        {
            //var fileName = string.Format(@"{1}\{0}\OZB_GJ.json", "OZB", _baseDir);
            //var json = ReadFileString(fileName);
            //if (string.IsNullOrEmpty(json))
            //    return new List<JCZQ_OZBMatchInfo>();
            //return JsonSerializer.Deserialize<List<JCZQ_OZBMatchInfo>>(json);


            var coll = mDB.GetCollection<JCZQ_OZBMatchInfo>("JCZQ_OZBMatchInfo");
            var builder = Builders<JCZQ_OZBMatchInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
            //  DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

            //var filter = builder.Eq("IssuseNumber", issuseNumber); //& builder.Lte("GameCode", gameType) & builder.Lte("IssuseNumber", issuseNumber);
            var documents = coll.Find<JCZQ_OZBMatchInfo>(null).ToList();
            return documents;
        }
        private List<JCLQ_MatchInfo> LoadJCLQMatchList()
        {
            //var fileName = Path.Combine(_baseDir, string.Format(@"{0}\Match_List.json", "JCLQ"));
            //var fileName = string.Format(@"{1}\{0}\Match_List.json", "JCLQ", _baseDir);
            ////if (!File.Exists(fileName))
            ////    throw new ArgumentException("未找到数据文件" + fileName);

            //var json = ReadFileString(fileName);
            //if (string.IsNullOrEmpty(json))
            //    return new List<MatchBiz.Core.JCLQ_MatchInfo>();
            //return JsonSerializer.Deserialize<List<MatchBiz.Core.JCLQ_MatchInfo>>(json);


            var coll = mDB.GetCollection<JCLQ_MatchInfo>("JCLQ_MatchInfo");
            var builder = Builders<JCLQ_MatchInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
            //  DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

            //var filter = builder.Eq("IssuseNumber", issuseNumber); //& builder.Lte("GameCode", gameType) & builder.Lte("IssuseNumber", issuseNumber);
            var documents = coll.Find<JCLQ_MatchInfo>(null).ToList();
            return documents;
        }

        private List<JCLQ_MatchResultInfo> LoadJCLQMatchResultList()
        {
            //var fileName = Path.Combine(_baseDir, string.Format(@"{0}\Match_Result_List.json", "JCLQ"));

            var coll = mDB.GetCollection<JCLQ_MatchResultInfo>("JCLQ_MatchResultInfo");
            var builder = Builders<JCLQ_MatchResultInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
            //  DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

            //var filter = builder.Eq("IssuseNumber", issuseNumber); //& builder.Lte("GameCode", gameType) & builder.Lte("IssuseNumber", issuseNumber);
            var documents = coll.Find<JCLQ_MatchResultInfo>(null).ToList();
            return documents;
        }
        private List<SJB_MatchInfo> LoadSJB_GJMatchList()
        {
            //var fileName = Path.Combine(_baseDir, string.Format(@"{0}\Match_List.json", "JCZQ"));
            var coll = mDB.GetCollection<SJB_MatchInfo>("SJB_MatchInfo");
            var builder = Builders<SJB_MatchInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
            //  DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

            //var filter = builder.Eq("IssuseNumber", issuseNumber); //& builder.Lte("GameCode", gameType) & builder.Lte("IssuseNumber", issuseNumber);
            var documents = coll.Find<SJB_MatchInfo>(null).ToList();
            return documents;
        }
        /// <summary>
        /// 冠亚军比赛
        /// </summary>
        private List<SJB_MatchInfo> LoadSJB_GYJMatchList()
        {
            //var fileName = Path.Combine(_baseDir, string.Format(@"{0}\Match_List.json", "JCZQ"));
            var coll = mDB.GetCollection<SJB_MatchInfo>("SJB_MatchInfo_GYJ");
            var builder = Builders<SJB_MatchInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
            //  DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

            //var filter = builder.Eq("IssuseNumber", issuseNumber); //& builder.Lte("GameCode", gameType) & builder.Lte("IssuseNumber", issuseNumber);
            var documents = coll.Find<SJB_MatchInfo>(null).ToList();
            return documents;
        }
        #endregion
        #region 北京单场


        public void Update_BJDC_IssuseList()
        {
            var existIssuseList = LoadBJDCIssuseLIst();
            Update_BJDC_IssuseList(existIssuseList);

            //重新加载比赛到缓存
            RedisMatchBusiness.ReloadCurrentBJDCMatch();
        }

        private void Update_BJDC_IssuseList(List<BJDCIssuseInfo> issuseInfoList)
        {
            //开启事务
            using (DB)
            {
                DB.Begin();
                try
                {
                    var manager = new BJDCMatchManager();
                    foreach (var item in issuseInfoList)
                    {
                        var issuse = manager.QueryBJDC_Issuse(item.IssuseNumber);
                        if (issuse == null)
                        {
                            manager.AddBJDC_Issuse(new C_BJDC_Issuse
                            {
                                IssuseNumber = item.IssuseNumber,
                                MinLocalStopTime = DateTime.Parse(item.MinLocalStopTime),
                                MinMatchStartTime = DateTime.Parse(item.MinMatchStartTime)
                            });
                        }
                        else
                        {
                            issuse.MinLocalStopTime = DateTime.Parse(item.MinLocalStopTime);
                            issuse.MinMatchStartTime = DateTime.Parse(item.MinMatchStartTime);
                            manager.UpdateBJDC_Issuse(issuse);
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
        }

        public void Update_BJDC_MatchList(string issuseNumber, string[] matchIdList)
        {
            var matchInfoList = LoadBJDCMatchList(issuseNumber);
            UpdateBJDCMatch(issuseNumber, matchIdList, matchInfoList);

            //重新加载比赛到缓存
            RedisMatchBusiness.ReloadCurrentBJDCMatch();
        }

        private void UpdateBJDCMatch(string issuseNumber, string[] matchIdList, List<BJDC_MatchInfo> matchInfoList)
        {
            //开启事务
            var manager = new BJDCMatchManager();
            using (manager.DB)
            {
               

               
                var oldList = manager.QueryBJDC_MatchList(issuseNumber);
                var oldResultList = manager.QueryBJDC_MatchResult_PrizeList(issuseNumber);

                manager.DB.Begin();
                try
                {
                    foreach (var matchId in matchIdList)
                    {
                        string id = string.Format("{0}|{1}", issuseNumber, matchId);
                        var old = oldList.FirstOrDefault(p => p.Id == id);
                        var oldResult = oldResultList.FirstOrDefault(p => p.Id == id);
                        var current = matchInfoList.FirstOrDefault(p => p.Id == id);
                        if (current == null)
                            continue;
                        //重新添加
                        if (old == null)
                        {
                            manager.AddBJDC_Match(new C_BJDC_Match
                            {
                                CreateTime = DateTime.Parse(current.CreateTime),
                                FlatOdds = current.FlatOdds,
                                GuestTeamName = current.GuestTeamName,
                                GuestTeamSort = current.GuestTeamSort,
                                HomeTeamName = current.HomeTeamName,
                                HomeTeamSort = current.HomeTeamSort,
                                Id = current.Id,
                                IssuseNumber = current.IssuseNumber,
                                LetBall = current.LetBall,
                                LocalStopTime = DateTime.Parse(current.LocalStopTime),
                                LoseOdds = current.LoseOdds,
                                MatchColor = current.MatchColor,
                                MatchName = current.MatchName,
                                MatchOrderId = current.MatchOrderId,
                                MatchStartTime = DateTime.Parse(current.MatchStartTime),
                                MatchState = (int)current.MatchState,
                                WinOdds = current.WinOdds,
                                MatchId = current.MatchId,
                                HomeTeamId = current.HomeTeamId,
                                GuestTeamId = current.GuestTeamId,
                                Mid = current.Mid,
                            });
                        }
                        else
                        {
                            //更新
                            old.FlatOdds = current.FlatOdds;
                            old.GuestTeamName = current.GuestTeamName;
                            old.GuestTeamSort = current.GuestTeamSort;
                            old.HomeTeamName = current.HomeTeamName;
                            old.HomeTeamSort = current.HomeTeamSort;
                            old.LetBall = current.LetBall;
                            old.LocalStopTime = DateTime.Parse(current.LocalStopTime);
                            old.LoseOdds = current.LoseOdds;
                            old.MatchColor = current.MatchColor;
                            old.MatchName = current.MatchName;
                            old.MatchStartTime = DateTime.Parse(current.MatchStartTime);
                            old.MatchState = (int)current.MatchState;
                            old.WinOdds = current.WinOdds;
                            manager.UpdateBJDC_Match(old);
                        }

                        if (oldResult == null)
                        {
                            manager.AddBJDC_MatchResult_Prize(new C_BJDC_MatchResult_Prize
                            {
                                CreateTime = DateTime.Parse(current.CreateTime),
                                Id = current.Id,
                                IssuseNumber = current.IssuseNumber,
                                MatchState = "0",
                                MatchOrderId = current.MatchOrderId,
                                HomeFull_Result = "-",
                                HomeHalf_Result = "-",
                                GuestFull_Result = "-",
                                GuestHalf_Result = "-",
                                BQC_Result = "-",
                                BF_Result = "-",
                                SPF_Result = "-",
                                SXDS_Result = "-",
                                ZJQ_Result = "-",
                                BF_SP = 0M,
                                SXDS_SP = 0M,
                                BQC_SP = 0M,
                                SPF_SP = 0M,
                                ZJQ_SP = 0M,
                            });
                        }

                    }
                    manager.DB.Commit();
                }
                catch (Exception)
                {
                    manager.DB.Rollback();
                    throw;
                }
           

              
            }
        }

        public void ManualUpdate_BJDC_MatchList(string issuseNumber)
        {
            var matchInfoList = LoadBJDCMatchList(issuseNumber);
            var matchIdArray = matchInfoList.Select(p => p.MatchOrderId.ToString()).ToArray();
            UpdateBJDCMatch(issuseNumber, matchIdArray, matchInfoList);

            //重新加载比赛到缓存
            RedisMatchBusiness.ReloadCurrentBJDCMatch();
        }

        public void Update_BJDC_MatchResultList(string issuseNumber, string[] matchResultIdArray)
        {
            var matchResultList = LoadBJDCMatchResultList(issuseNumber);
            var manager = new BJDCMatchManager();
            //开启事务
            using (manager.DB)
            {
               

               
                var oldList = manager.QueryBJDC_MatchResultList(issuseNumber);
                var oldResultList = manager.QueryBJDC_MatchResult_PrizeList(issuseNumber);

                manager.DB.Begin();
                try
                {

               
                foreach (var matchResultId in matchResultIdArray)
                {
                    string id = string.Format("{0}|{1}", issuseNumber, matchResultId);
                    var old = oldList.FirstOrDefault(p => p.Id == id);
                    var oldResult = oldResultList.FirstOrDefault(p => p.Id == id);
                    var current = matchResultList.FirstOrDefault(p => p.Id == id);
                    if (current == null)
                        continue;

                    var state = "1";
                    if (current.SPF_SP > 1M && current.SXDS_SP > 1M && current.ZJQ_SP > 1M && current.BF_SP > 1M && current.BQC_SP > 1M && current.MatchState == "Finish")
                        state = "2";
                    //重新添加
                    if (old == null)
                    {
                        manager.AddBJDC_MatchResult(new C_BJDC_MatchResult
                        {
                            CreateTime = DateTime.Parse(current.CreateTime),
                            Id = current.Id,
                            IssuseNumber = current.IssuseNumber,
                            BF_Result = current.BF_Result,
                            BF_SP = current.BF_SP,
                            BQC_Result = current.BQC_Result,
                            BQC_SP = current.BQC_SP,
                            SPF_Result = current.SPF_Result,
                            SPF_SP = current.SPF_SP,
                            SXDS_Result = current.SXDS_Result,
                            SXDS_SP = current.SXDS_SP,
                            ZJQ_Result = current.ZJQ_Result,
                            ZJQ_SP = current.ZJQ_SP,
                            GuestFull_Result = current.GuestFull_Result,
                            GuestHalf_Result = current.GuestHalf_Result,
                            HomeFull_Result = current.HomeFull_Result,
                            HomeHalf_Result = current.HomeHalf_Result,
                            MatchOrderId = current.MatchOrderId,
                            MatchState = state,
                        });
                    }
                    else
                    {
                        if (old.MatchState != "2")
                        {
                            //更新
                            old.BF_Result = current.BF_Result;
                            old.BF_SP = current.BF_SP;
                            old.BQC_Result = current.BQC_Result;
                            old.BQC_SP = current.BQC_SP;
                            old.GuestFull_Result = current.GuestFull_Result;
                            old.GuestHalf_Result = current.GuestHalf_Result;
                            old.HomeFull_Result = current.HomeFull_Result;
                            old.HomeHalf_Result = current.HomeHalf_Result;
                            old.MatchState = state;
                            old.SPF_Result = current.SPF_Result;
                            old.SPF_SP = current.SPF_SP;
                            old.SXDS_Result = current.SXDS_Result;
                            old.SXDS_SP = current.SXDS_SP;
                            old.ZJQ_Result = current.ZJQ_Result;
                            old.ZJQ_SP = current.ZJQ_SP;
                            manager.UpdateBJDC_MatchResult(old);
                        }
                    }


                    if (oldResult == null)
                    {
                        manager.AddBJDC_MatchResult_Prize(new C_BJDC_MatchResult_Prize
                        {
                            CreateTime = DateTime.Parse(current.CreateTime),
                            Id = current.Id,
                            IssuseNumber = current.IssuseNumber,
                            BF_Result = current.BF_Result,
                            BF_SP = current.BF_SP,
                            BQC_Result = current.BQC_Result,
                            BQC_SP = current.BQC_SP,
                            SPF_Result = current.SPF_Result,
                            SPF_SP = current.SPF_SP,
                            SXDS_Result = current.SXDS_Result,
                            SXDS_SP = current.SXDS_SP,
                            ZJQ_Result = current.ZJQ_Result,
                            ZJQ_SP = current.ZJQ_SP,
                            GuestFull_Result = current.GuestFull_Result,
                            GuestHalf_Result = current.GuestHalf_Result,
                            HomeFull_Result = current.HomeFull_Result,
                            HomeHalf_Result = current.HomeHalf_Result,
                            MatchOrderId = current.MatchOrderId,
                            MatchState = state,
                        });
                    }
                    else
                    {
                        if (oldResult.MatchState != "2")
                        {
                            //更新
                            oldResult.BF_Result = current.BF_Result;
                            oldResult.BF_SP = current.BF_SP;
                            oldResult.BQC_Result = current.BQC_Result;
                            oldResult.BQC_SP = current.BQC_SP;
                            oldResult.GuestFull_Result = current.GuestFull_Result;
                            oldResult.GuestHalf_Result = current.GuestHalf_Result;
                            oldResult.HomeFull_Result = current.HomeFull_Result;
                            oldResult.HomeHalf_Result = current.HomeHalf_Result;
                            oldResult.MatchState = state;
                            oldResult.SPF_Result = current.SPF_Result;
                            oldResult.SPF_SP = current.SPF_SP;
                            oldResult.SXDS_Result = current.SXDS_Result;
                            oldResult.SXDS_SP = current.SXDS_SP;
                            oldResult.ZJQ_Result = current.ZJQ_Result;
                            oldResult.ZJQ_SP = current.ZJQ_SP;
                            manager.UpdateBJDC_MatchResult_Prize(oldResult);
                        }
                    }

                }

                manager.DB.Commit();
                }
                catch (Exception)
                {
                    manager.DB.Rollback();
                    throw;
                }
            }
        }

        public void ManualUpdate_BJDC_MatchResultList(string issuseNumber)
        {
            var matchResultList = LoadBJDCMatchResultList(issuseNumber);
            var matchIdArray = matchResultList.Select(p => p.MatchOrderId.ToString()).ToArray();
            Update_BJDC_MatchResultList(issuseNumber, matchIdArray);
        }

        #region 胜负过关相关操作
        private List<T> LoadGameMatchList<T>(string gameCode, string issuseNumber, string fileName)
        {
            //fileName = string.Format(@"{3}\{0}\{1}\{2}.json", gameCode, issuseNumber, fileName, _baseDir);
            ////if (!File.Exists(fileName))
            ////    throw new ArgumentException("未找到数据文件" + fileName);

            //var json = ReadFileString(fileName);
            //if (string.IsNullOrEmpty(json))
            //    return new List<T>();
            string name = typeof(T).Name;
            var coll = mDB.GetCollection<T>(name);
            var builder = Builders<T>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
            //  DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

            var filter = builder.Eq("gameCode", gameCode) & builder.Eq("issuseNumber", issuseNumber);// & builder.Lte("IssuseNumber", issuseNumber);
            var documents = coll.Find<T>(filter).ToList();
            return documents;
        }
        private void UpdateOrder_RunningHitMatchCount(string schemeId, int hitCount, int rightCount, int error1Count, int error2Count, int bonusCount)
        {
            //开启事务
            var sportsManager = new Sports_Manager();
            using (sportsManager.DB)
            {
                sportsManager.DB.Begin();
                try
                {

               

                  var sql = string.Format(@"UPDATE C_Sports_Order_Running
                                            SET HitMatchCount='{0}',RightCount='{1}',Error1Count='{2}',Error2Count='{3}',BonusCount='{4}'
                                            WHERE SchemeId='{5}'", hitCount, rightCount, error1Count, error2Count, bonusCount, schemeId);
                sql += Environment.NewLine;
                sql += string.Format(@" UPDATE C_Sports_Order_Complate
                                            SET HitMatchCount='{0}',RightCount='{1}',Error1Count='{2}',Error2Count='{3}',BonusCount='{4}'
                                            WHERE SchemeId='{5}'", hitCount, rightCount, error1Count, error2Count, bonusCount, schemeId);
              
                sportsManager.ExecSql(sql);
                    sportsManager.DB.Commit();
                }
                catch (Exception)
                {
                    sportsManager.DB.Rollback();
                    throw;
                }
               
            }
        }

        /// <summary>
        /// 后台手工添加或更新胜负过关比赛数据
        /// </summary>
        public void ManualUpdate_SFGG_MatchList(string issuseNumber)
        {
            var matchResultList = LoadGameMatchList<SFGG_MatchInfo>("BJDC", issuseNumber, "Match_SFGG_List");
            var matchIdArray = matchResultList.Select(p => p.MatchOrderId.ToString()).ToArray();
            UpdateSFGGMatch(issuseNumber, matchIdArray, matchResultList);
        }

        /// <summary>
        /// 后台手工添加或更新胜负过关比赛结果
        /// </summary>
        public void ManualUpdate_SFGG_MatchResultList(string issuseNumber)
        {
            var matchResultList = LoadGameMatchList<SFGG_MatchInfo>("BJDC", issuseNumber, "MatchResult_SFGG_List");
            var matchIdArray = matchResultList.Select(p => p.MatchOrderId.ToString()).ToArray();
            Update_SFGG_MatchResultList(issuseNumber, matchIdArray);
        }
        /// <summary>
        /// 添加或更新胜负过关比赛数据
        /// </summary>
        public void Update_SFGG_MatchList(string issuseNumber, string[] matchIdList)
        {
            var matchInfoList = LoadGameMatchList<SFGG_MatchInfo>("BJDC", issuseNumber, "Match_SFGG_List");
            UpdateSFGGMatch(issuseNumber, matchIdList, matchInfoList);
        }
        private void UpdateSFGGMatch(string issuseNumber, string[] matchIdList, List<SFGG_MatchInfo> matchInfoList)
        {
            //开启事务
            var manager = new SFGGMatchManager();
            using (manager.DB)
            {
            

               
                var oldList = manager.QuerySFGGMatchList(issuseNumber);
                manager.DB.Begin();
                try
                {

               
                foreach (var item in matchIdList)
                {
                    string id = string.Format("{0}|{1}", issuseNumber, item);
                    var old = oldList.FirstOrDefault(p => p.MatchId == id);
                    var current = matchInfoList.FirstOrDefault(p => p.Id == id);
                    if (current == null)
                        continue;
                    //重新添加
                    if (old == null)
                    {
                        C_SFGG_Match info = new C_SFGG_Match();
                        info.MatchId = current.Id;
                        info.MatchOrderId = current.MatchOrderId;
                        info.MatchState = current.MatchState;
                        info.Category = current.Category;
                        info.IssuseNumber = current.IssuseNumber;
                        info.MatchName = current.MatchName;
                        info.HomeTeamName = current.HomeTeamName;
                        info.GuestTeamName = current.GuestTeamName;
                        info.LetBall = current.LetBall;
                        info.LoseOdds = current.LoseOdds;
                        info.WinOdds = current.WinOdds;
                        if (current.MatchStartTime != null)
                            info.MatchStartTime = DateTime.Parse(current.MatchStartTime);
                        if (current.BetStopTime != null)
                            info.BetStopTime = DateTime.Parse(current.BetStopTime);
                        if (current.CreateTime != null)
                            info.CreateTime = DateTime.Parse(current.CreateTime);
                        manager.AddSFGGMatch(info);
                        continue;
                    }
                    old.MatchState = current.MatchState;
                    old.MatchName = current.MatchName;
                    old.HomeTeamName = current.HomeTeamName;
                    old.GuestTeamName = current.GuestTeamName;
                    old.LetBall = current.LetBall;
                    old.LoseOdds = current.LoseOdds;
                    old.WinOdds = current.WinOdds;
                    if (current.MatchStartTime != null)
                        old.MatchStartTime = Convert.ToDateTime(current.MatchStartTime);
                    if (current.BetStopTime != null)
                        old.BetStopTime = Convert.ToDateTime(current.BetStopTime);
                    manager.UpdateSFGGMatch(old);
                }
                manager.DB.Commit();
                }
                catch (Exception)
                {
                    manager.DB.Rollback();
                    throw;
                }
            }
        }
        public void Update_SFGG_MatchResultList(string issuseNumber, string[] matchResultIdArray)
        {
            var matchResultList = LoadGameMatchList<SFGG_MatchResultInfo>("BJDC", issuseNumber, "MatchResult_SFGG_List");
            var manager = new SFGGMatchManager();
            //开启事务
            using (manager.DB)
            {
               

              
                var oldList = manager.QuerySFGGMatchList(issuseNumber);
                manager.DB.Begin();
                try
                {
                    foreach (var matchResultId in matchResultIdArray)
                {
                    string id = string.Format("{0}|{1}", issuseNumber, matchResultId);
                    var old = oldList.FirstOrDefault(p => p.MatchId == id);
                    var current = matchResultList.FirstOrDefault(p => p.Id == id);
                    if (current == null)
                        continue;
                    if (old.MatchResultState == "2")
                        continue;
                    var state = "1";
                    if (current.SF_SP > 1M && current.MatchState == "Finish")
                        state = "2";
                    //更新
                    old.HomeFull_Result = current.HomeFull_Result;
                    old.GuestFull_Result = current.GuestFull_Result;
                    if (current.MatchResultTime != null)
                        old.MatchResultTime = Convert.ToDateTime(current.MatchResultTime);
                    old.MatchResultState = state;
                    old.GuestFull_Result = current.GuestFull_Result;
                    old.SF_Result = current.SF_Result;
                    old.SF_SP = current.SF_SP;
                    manager.UpdateSFGGMatch(old);
                }

                manager.DB.Commit();
            }
                catch (Exception)
            {
                manager.DB.Rollback();
                throw;
            }
        }
        }

        #endregion

        /// <summary>
        /// 更新北京单场命中场数
        /// </summary>
        public void Update_BJDC_HitCount(string issuseNumber, string[] matchResultIdArray)
        {
            //暂停业务
            return;

            //1 普通投注
            //var sportsManager = new Sports_Manager();
            //var dealSchemeList = new List<string>();
            //foreach (var item in sportsManager.QueryBJDCSportsAnteCodeByIsuseNumber(issuseNumber, matchResultIdArray))
            //{
            //    if (dealSchemeList.Contains(item.SchemeId)) continue;

            //    Update_BJDC_General_HitCount(issuseNumber, item.GameType, item.SchemeId);

            //    dealSchemeList.Add(item.SchemeId);
            //}

            ////2 单式上传
            //sportsManager = new Sports_Manager();
            //dealSchemeList = new List<string>();
            //foreach (var item in sportsManager.QueryBJDCSingleCodeByIsuseNumber(issuseNumber, matchResultIdArray))
            //{
            //    if (string.IsNullOrEmpty(item)) continue;
            //    var array = item.Split('|');//[SchemeId]+'|'+[GameType]
            //    if (array.Length != 2) continue;
            //    if (dealSchemeList.Contains(array[0])) continue;

            //    Update_BJDC_Single_HitCount(issuseNumber, array[1], array[0]);

            //    dealSchemeList.Add(array[0]);
            //}
        }

        /// <summary>
        /// 更新单个订单的过关统计
        /// </summary>
        public void Update_BJDC_HitCountBySchemeId(string schemeId)
        {
            //暂停业务
            return;

            //var sportsManager = new Sports_Manager();
            //var order = sportsManager.QuerySports_Order_Running(schemeId);
            //if (order == null)
            //    throw new Exception(string.Format("没有找到{0}的Order_Running数据", schemeId));
            ////普通投注
            //if (order.SchemeBettingCategory == SchemeBettingCategory.GeneralBetting)
            //{
            //    Update_BJDC_General_HitCount(order.IssuseNumber, order.GameType, schemeId);
            //    return;
            //}
            ////单式上传 和 过滤投注
            //Update_BJDC_Single_HitCount(order.IssuseNumber, order.GameType, schemeId);
        }

        private void Update_BJDC_Single_HitCount(string issuseNumber, string gameType, string schemeId)
        {
            var sportsManager = new Sports_Manager();
            var bjdcManager = new BJDCMatchManager();
            var codeInfo = sportsManager.QuerySingleScheme_AnteCode(schemeId);
            if (codeInfo == null)
                throw new Exception(string.Format("查不到方案{0}的投注号码信息", schemeId));

            var selectMatchIdArray = codeInfo.SelectMatchId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var allowCodeArray = codeInfo.AllowCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //分析投注号码
            //var codeText = File.ReadAllText(codeInfo.AnteCodeFullFileName, Encoding.UTF8);
            var codeText = Encoding.UTF8.GetString(codeInfo.FileBuffer);
            var matchIdList = new List<string>();
            var codeList = AnalyzerFactory.CheckSingleSchemeAnteCode(codeText, codeInfo.PlayType, codeInfo.ContainsMatchId, selectMatchIdArray, allowCodeArray, out matchIdList);

            var hitCount = 0;
            var rightCount = 0;
            var error1Count = 0;
            var error2Count = 0;
            var bonusCount = 0;

            var matchResultList = bjdcManager.QueryBJDC_MatchResultList(issuseNumber, matchIdList.ToArray());
            var totalResultList = new List<C_BJDC_MatchResult>();
            foreach (var item in codeList)
            {
                var playType = string.Empty;
                var anteCodeList = new List<C_Sports_AnteCode>();
                foreach (var t in item.Split('#'))
                {
                    var array = t.Split('|');
                    if (array.Length != 3) continue;
                    if (array[1] == "*") continue;
                    if (string.IsNullOrEmpty(playType))
                        playType = array[2];
                    anteCodeList.Add(new C_Sports_AnteCode
                    {
                        MatchId = array[0],
                        AnteCode = array[1],
                        PlayType = array[2],
                        IsDan = false,
                    });
                }

                #region 计算命中场数

                foreach (var code in anteCodeList)
                {
                    var query = from r in matchResultList
                                where r.IssuseNumber == issuseNumber && r.MatchOrderId.ToString() == code.MatchId
                                select r;
                    var first = query.FirstOrDefault();
                    if (totalResultList.FirstOrDefault(p => p.MatchOrderId == int.Parse(code.MatchId)) == null)
                    {
                        totalResultList.Add(first == null ? new C_BJDC_MatchResult
                        {
                            MatchOrderId = int.Parse(code.MatchId),
                            SPF_Result = "-1",
                            ZJQ_Result = "-1",
                            SXDS_Result = "-1",
                            BF_Result = "-1",
                            BQC_Result = "-1",
                        } : first);
                    }
                    switch (gameType)
                    {
                        case "SPF":
                            query = query.Where(p => code.AnteCode.Contains(p.SPF_Result));
                            break;
                        case "ZJQ":
                            query = query.Where(p => code.AnteCode.Contains(p.ZJQ_Result));
                            break;
                        case "SXDS":
                            query = query.Where(p => code.AnteCode.Contains(p.SXDS_Result));
                            break;
                        case "BF":
                            query = query.Where(p => code.AnteCode.Contains(p.BF_Result));
                            break;
                        case "BQC":
                            query = query.Where(p => code.AnteCode.Contains(p.BQC_Result));
                            break;
                    }
                    if (query.FirstOrDefault() != null)
                        hitCount++;
                }

                #endregion

                #region 计算中奖注数
                //C_Sports_AnteCode
                var playTypeArray = playType.Split('_');
                if (playTypeArray.Length != 2) continue;
                var baseCount = int.Parse(playTypeArray[0]);
                var analyzer = AnalyzerFactory.GetSportAnalyzer("BJDC", gameType, baseCount);
                var bonusResult = analyzer.CaculateBonus(anteCodeList.ToArray(), totalResultList.ToArray());
                bonusCount += bonusResult.BonusCount;

                #endregion
            }

            UpdateOrder_RunningHitMatchCount(schemeId, hitCount, rightCount, error1Count, error2Count, bonusCount);
        }

        public void Update_BJDC_General_HitCount(string issuseNumber, string gameType, string schemeId)
        {
            var sportsManager = new Sports_Manager();
            var bjdcManager = new BJDCMatchManager();

            #region 计算命中场数

            //计算命中场数
            gameType = gameType.ToUpper();
            var hitCount = 0;
            var playType = string.Empty;
            var codeList = sportsManager.QuerySportsAnteCodeBySchemeId(schemeId);
            var matchResultList = bjdcManager.QueryBJDC_MatchResultList(issuseNumber, (from c in codeList select c.MatchId).ToArray());
            var totalResultList = new List<C_BJDC_MatchResult>();
            foreach (var code in codeList)
            {
                if (string.IsNullOrEmpty(playType))
                    playType = code.PlayType;
                var query = from r in matchResultList
                            where r.IssuseNumber == code.IssuseNumber && r.MatchOrderId.ToString() == code.MatchId
                            select r;
                var first = query.FirstOrDefault();
                if (totalResultList.FirstOrDefault(p => p.MatchOrderId == int.Parse(code.MatchId)) == null)
                {
                    totalResultList.Add(first == null ? new C_BJDC_MatchResult
                    {
                        MatchOrderId = int.Parse(code.MatchId),
                        SPF_Result = "-1",
                        ZJQ_Result = "-1",
                        SXDS_Result = "-1",
                        BF_Result = "-1",
                        BQC_Result = "-1",
                    } : first);
                }
                switch (gameType)
                {
                    case "SPF":
                        query = query.Where(p => code.AnteCode.Contains(p.SPF_Result));
                        break;
                    case "ZJQ":
                        query = query.Where(p => code.AnteCode.Contains(p.ZJQ_Result));
                        break;
                    case "SXDS":
                        query = query.Where(p => code.AnteCode.Contains(p.SXDS_Result));
                        break;
                    case "BF":
                        query = query.Where(p => code.AnteCode.Contains(p.BF_Result));
                        break;
                    case "BQC":
                        query = query.Where(p => code.AnteCode.Contains(p.BQC_Result));
                        break;
                    default:
                        continue;
                }
                if (query.FirstOrDefault() != null)
                    hitCount++;
            }

            #endregion

            #region 计算全中、错1、错2场注数

            //计算全中、错1、错2场注数
            var rightCount = 0;
            var error1Count = 0;
            var error2Count = 0;
            var ticketList = sportsManager.QueryTicketList(schemeId);
            foreach (var ticket in ticketList)
            {
                if (string.IsNullOrEmpty(ticket.BetContent)) continue;
                //156_1/157_3/158_1或29_33/30_33/32_33或119_2,3/120_3,4/121_4
                var betArray = ticket.BetContent.Split('/');
                var ticket_rightCount = 0;
                var ticket_errorCount = 0;
                foreach (var match in betArray)
                {
                    //119_2,3
                    var matchArray = match.Split('_');
                    if (matchArray.Length != 2) continue;

                    var mr = matchResultList.Where(p => p.MatchOrderId == int.Parse(matchArray[0])).FirstOrDefault();
                    if (mr == null) continue;

                    switch (ticket.GameType.ToUpper())
                    {
                        case "SPF":
                            if (matchArray[1].Split(',').Contains(mr.SPF_Result))
                                ticket_rightCount++;
                            else
                                ticket_errorCount++;
                            break;
                        case "ZJQ":
                            if (matchArray[1].Split(',').Contains(mr.ZJQ_Result))
                                ticket_rightCount++;
                            else
                                ticket_errorCount++;
                            break;
                        case "SXDS":
                            if (matchArray[1].Split(',').Contains(mr.SXDS_Result))
                                ticket_rightCount++;
                            else
                                ticket_errorCount++;
                            break;
                        case "BF":
                            if (matchArray[1].Split(',').Contains(mr.BF_Result))
                                ticket_rightCount++;
                            else
                                ticket_errorCount++;
                            break;
                        case "BQC":
                            if (matchArray[1].Split(',').Contains(mr.BQC_Result))
                                ticket_rightCount++;
                            else
                                ticket_errorCount++;
                            break;
                        default:
                            continue;
                    }
                }
                //单张票的正确场数 等于投注票数
                if (ticket_rightCount == betArray.Length)
                {
                    rightCount++;
                }
                else if (ticket_errorCount == 1)
                {
                    error1Count++;
                }
                else if (ticket_errorCount == 2)
                {
                    error2Count++;
                }
            }

            #endregion

            #region 计算中奖注数

            var bonusCount = 0;
            foreach (var item in playType.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var t = item.Split('_');
                if (t.Length != 2) return;
                var baseCount = int.Parse(t[0]);
                var analyzer = AnalyzerFactory.GetSportAnalyzer("BJDC", gameType, baseCount);
                var bonusResult = analyzer.CaculateBonus(codeList.ToArray(), totalResultList.ToArray());
                bonusCount += bonusResult.BonusCount;
            }

            #endregion

            UpdateOrder_RunningHitMatchCount(schemeId, hitCount, rightCount, error1Count, error2Count, bonusCount);
        }

        public string QueryBJDCCurrentIssuse()
        {
            var current = new BJDCMatchManager().QueryBJDCCurrentIssuse();
            if (current == null)
                return string.Empty;
            return current.IssuseNumber;
        }

        public C_BJDC_Issuse QueryBJDCCurrentIssuseInfo()
        {
            var current = new BJDCMatchManager().QueryBJDCCurrentIssuseInfo();
            if (current == null)
                return null;
            return current;
        }

        public BJDCMatchResultInfo_Collection GetBJDCIssuse(int count)
        {
            using (var manager = new BJDCMatchManager())
            {
                BJDCMatchResultInfo_Collection collection = new BJDCMatchResultInfo_Collection();
                collection.ListInfo.AddRange(manager.GetBJDCIssuse(count));
                return collection;
            }
        }

        public string QueryBJDCLastIssuseNumber(int count)
        {
            var manager = new BJDCMatchManager();
            var array = manager.QueryBJDCLastIssuseNumber(count);
            return string.Join("|", array);
        }

        #endregion

        #region 传统足球
        private List<CTZQ_BonusLevelInfo> LoadCTZQBonusLevelList(string issuseNumber, string gameType)
        {
            //var fileName = Path.Combine(_baseDir, string.Format(@"{0}\{1}\CTZQ_{2}_BonusLevel.json", "CTZQ", issuseNumber, gameType));
            //var fileName = string.Format(@"{3}\{0}\{1}\CTZQ_{2}_BonusLevel.json", "CTZQ", issuseNumber, gameType, _baseDir);
            //if (!File.Exists(fileName))
            //    throw new ArgumentException("未找到数据文件" + fileName);

            //var json = ReadFileString(fileName);
            //if (string.IsNullOrEmpty(json))
            //    return new List<MatchBiz.Core.CTZQ_BonusLevelInfo>();
            //return JsonSerializer.Deserialize<List<MatchBiz.Core.CTZQ_BonusLevelInfo>>(json);

            var coll = mDB.GetCollection<CTZQ_BonusLevelInfo>("CTZQ_BonusLevelInfo");
            var builder = Builders<CTZQ_BonusLevelInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);

            var filter = builder.Eq("issuseNumber", gameType) & builder.Eq("GameType", gameType);// & builder.Eq("IssuseNumber", issuseNumber);
            var documents = coll.Find<CTZQ_BonusLevelInfo>(filter).ToList();

            return documents;
        }
        private IList<CTZQ_BonusPoolInfo> GetBonusPoolList_CTZQ(string gameCode, string gameType, string issuseNumber)
        {
            //var fileName = string.Format(@"{3}\{0}\{2}\{0}_{1}_BonusLevel.json", gameCode, gameType, issuseNumber, _baseDir);
            ////if (!File.Exists(fileName))
            ////{
            ////    throw new ArgumentException("奖池不存在或尚未开奖");
            ////}
            //var json = ReadFileString(fileName);
            //var resultList = JsonSerializer.Deserialize<List<CTZQ_BonusPoolInfo>>(json);


            var coll = mDB.GetCollection<CTZQ_BonusPoolInfo>("CTZQ_BonusPoolInfo");
            var builder = Builders<CTZQ_BonusPoolInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
            DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

            var filter = builder.Eq("GameCode", gameCode) & builder.Eq("GameCode", gameType) & builder.Eq("IssuseNumber", issuseNumber);
            var documents = coll.Find<CTZQ_BonusPoolInfo>(filter).ToList();

            return documents;
        }
        //获取期号
        private List<CTZQ_IssuseInfo> LoadCTZQGameIssuse(string gameType)
        {
            //var fileName = Path.Combine(_baseDir, string.Format(@"{0}\Match_{1}_Issuse_List.json", "CTZQ", gameType));
          //  var fileName = string.Format(@"{2}\{0}\Match_{1}_Issuse_List.json", "CTZQ", gameType, _baseDir);
            //if (!File.Exists(fileName))
            //    throw new ArgumentException("未找到数据文件" + fileName);
            var coll = mDB.GetCollection<CTZQ_IssuseInfo>("CTZQ_IssuseInfo");
            var builder = Builders<CTZQ_IssuseInfo>.Filter;
           // DateTime startTime = DateTime.Now.AddMonths(1);
            DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month-1, 1, 12, 0, 0, DateTimeKind.Utc);

            var filter = builder.Eq("GameType", gameType);// & builder.Lte("CreateTime", endTime);
            var documents = coll.Find<CTZQ_IssuseInfo>(filter).ToList();

            return documents;
           // var json = ReadFileString(fileName);
            //if (string.IsNullOrEmpty(json))
            //    return new List<MatchBiz.Core.CTZQ_IssuseInfo>();
            //return JsonSerializer.Deserialize<List<MatchBiz.Core.CTZQ_IssuseInfo>>(json);
        }
        private List<CTZQ_MatchInfo> LoadCTZQMatchList(string issuseNumber, string gameType)
        {
            //var fileName = Path.Combine(_baseDir, string.Format(@"{0}\{1}\Match_{2}_List.json", "CTZQ", issuseNumber, gameType));
          //  var fileName = string.Format(@"{3}\{0}\{1}\Match_{2}_List.json", "CTZQ", issuseNumber, gameType, _baseDir);
            //if (!File.Exists(fileName))
            //    throw new ArgumentException("未找到数据文件" + fileName);
            var coll = mDB.GetCollection<CTZQ_MatchInfo>("CTZQ_MatchInfo");
            var builder = Builders<CTZQ_MatchInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
            DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

            var filter = builder.Eq("IssuseNumber", issuseNumber) & builder.Eq("GameType", gameType);
            var documents = coll.Find<CTZQ_MatchInfo>(filter).ToList();

            return documents;
            //var json = ReadFileString(fileName);
            //if (string.IsNullOrEmpty(json))
            //    return new List<MatchBiz.Core.CTZQ_MatchInfo>();
            //return JsonSerializer.Deserialize<List<MatchBiz.Core.CTZQ_MatchInfo>>(json);
        }
        public bool CanPrize_CTZQ(string gameType, int totalBonusCount, string winNumber)
        {
            if (gameType.Equals("T14C") || gameType.Equals("TR9"))
            {
                return totalBonusCount > 0;
            }
            else
            {
                return !winNumber.Contains("*");
            }
        }
        public string UpdateBonusPool_CTZQ(string gameCode, string gameType, string issuseNumber, out int totalBonusCount)
        {
            var winNumber = "";
            var bonusPoolList = GetBonusPoolList_CTZQ(gameCode, gameType, issuseNumber);
            totalBonusCount = bonusPoolList.Sum(b => b.BonusCount);
            using (DB)
            {
                try
                {

               
                DB.Begin();
                //var bonusManager = new Ticket_BonusManager();
                foreach (var info in bonusPoolList)
                {
                    //string errMsg;
                    //var analyzer = AnalyzerFactory.GetWinNumberAnalyzer(gameCode, gameType);
                    //if (!analyzer.CheckWinNumber(info.MatchResult, out errMsg))
                    //{
                    //    throw new FormatException(errMsg);
                    //}
                    winNumber = info.MatchResult;
                    var entity = DB.CreateQuery<T_Ticket_BonusPool>().Where(p => p.GameCode == gameCode && p.GameType == gameType && p.IssuseNumber == issuseNumber && p.BonusLevel == info.BonusLevel).FirstOrDefault();
                    //var entity = bonusManager.GetBonusPool(gameCode, gameType, issuseNumber, info.BonusLevel);
                    if (entity == null)
                    {
                        entity = new T_Ticket_BonusPool
                        {
                            Id = info.Id,
                            GameCode = gameCode,
                            GameType = gameType,
                            IssuseNumber = info.IssuseNumber,
                            BonusLevel = info.BonusLevel,
                            BonusCount = info.BonusCount,
                            BonusLevelDisplayName = info.BonusLevelDisplayName,
                            BonusMoney = info.BonusMoney,
                            WinNumber = info.MatchResult,
                            CreateTime = DateTime.Parse(info.CreateTime),
                        };
                        DB.GetDal<T_Ticket_BonusPool>().Add(entity);
                    }
                    else
                    {
                        entity.BonusCount = info.BonusCount;
                        entity.BonusLevelDisplayName = info.BonusLevelDisplayName;
                        entity.BonusMoney = info.BonusMoney;
                        entity.WinNumber = info.MatchResult;
                        entity.CreateTime = DateTime.Parse(info.CreateTime);
                       // bonusManager.UpdateBonusPool(entity);
                        DB.GetDal<T_Ticket_BonusPool>().Update(entity);
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
            return winNumber;
        }

        public void Update_CTZQ_GameIssuse(string gameCode, string[] array)
        {
            var list = LoadCTZQGameIssuse(gameCode);
          
            //开启事务
            using (DB)
            {
                

                //  var lotteryManager = new LotteryGameManager();
                //var manager = new CTZQMatchManager();
                // var oldList = manager.QueryCTZQGameIssuseListById(array);

                // var oldIssuseList = lotteryManager.QueryGameIssuseByKey(array);
                var oldList = DB.CreateQuery<C_CTZQ_GameIssuse>().Where(p => array.Contains(p.Id)).ToList();
                var oldIssuseList = DB.CreateQuery<C_Game_Issuse>().Where(p => array.Contains(p.GameCode_IssuseNumber));
                try
                {

                DB.Begin();
                foreach (var item in array)
                {
                    var old = oldList.FirstOrDefault(p => p.Id == item);
                    var current = list.FirstOrDefault(p => p.Id == item);
                    if (current == null)
                        continue;
                    if (old == null)
                    {
                        //重新添加
                        //var entity = new CTZQ_GameIssuse();
                        //ObjectConvert.ConverInfoToEntity<MatchBiz.Core.CTZQ_IssuseInfo, CTZQ_GameIssuse>(current, ref entity);
                        DB.GetDal<C_CTZQ_GameIssuse>().Add(new C_CTZQ_GameIssuse
                        {
                            CreateTime = DateTime.Parse(current.CreateTime),
                            GameCode = current.GameCode,
                            GameType = current.GameType,
                            Id = current.Id,
                            IssuseNumber = current.IssuseNumber,
                            StopBettingTime = DateTime.Parse(current.StopBettingTime),
                            WinNumber = current.WinNumber,
                        });
                    }
                    else
                    {
                        old.StopBettingTime = DateTime.Parse(current.StopBettingTime);
                        old.WinNumber = current.WinNumber;
                        DB.GetDal<C_CTZQ_GameIssuse>().Update(old);
                    }
                    var oldIssuse = oldIssuseList.FirstOrDefault(p => p.GameCode_IssuseNumber == item);
                    if (oldIssuse == null)
                    {
                        DB.GetDal<C_Game_Issuse>().Add(new C_Game_Issuse
                        {
                            AwardTime =DateTime.Parse("1910-01-01"),//默认为空时间
                            CreateTime = DateTime.Parse(current.CreateTime),
                            GameCode = current.GameCode,
                            GameType = current.GameType,
                            GatewayStopTime = DateTime.Parse(current.StopBettingTime),
                            LocalStopTime = DateTime.Parse(current.StopBettingTime),
                            OfficialStopTime = DateTime.Parse(current.StopBettingTime),
                            IssuseNumber = current.IssuseNumber,
                            StartTime = DateTime.Parse(current.CreateTime),
                            Status =(int) IssuseStatus.OnSale,
                            WinNumber = string.Empty,
                            GameCode_IssuseNumber = string.Format("{0}|{1}|{2}", current.GameCode, current.GameType, current.IssuseNumber),
                        });
                    }
                    else
                    {
                        if (oldIssuse.LocalStopTime.ToString("yyyy-MM-dd HH:mm:ss") != current.StopBettingTime)
                        {
                            //有新的数据变化，执行更新
                            oldIssuse.GatewayStopTime = DateTime.Parse(current.StopBettingTime);
                            oldIssuse.LocalStopTime = DateTime.Parse(current.StopBettingTime);
                            oldIssuse.OfficialStopTime = DateTime.Parse(current.StopBettingTime);
                            //oldIssuse.WinNumber = current.WinNumber;
                            DB.GetDal<C_Game_Issuse>().Update(oldIssuse);
                        }
                    }
                }

                DB.Commit();

                }
                catch
                {
                    //更新 传统足球期号错误
                    DB.Rollback();
                    
                }
            }
        }

        public void Update_CTZQ_MatchList(string gameCode, string issuseNumber, string[] array)
        {
            var list = LoadCTZQMatchList(issuseNumber, gameCode);
            UpdateCTZQMatchList(array, list);
        }

        private  void UpdateCTZQMatchList(string[] array, List<CTZQ_MatchInfo> list)
        {
            //开启事务
            using (DB)
            {
                //  biz.BeginTran();

                // var manager = new CTZQMatchManager();
                var oldList = DB.CreateQuery<C_CTZQ_Match>().Where(p => array.Contains(p.Id)).ToList();
                //  var oldList = manager.QueryCTZQMatchListById(array);
                try
                {

                DB.Begin();
                foreach (var item in array)
                {
                    var old = oldList.FirstOrDefault(p => p.Id == item);
                    var current = list.FirstOrDefault(p => p.Id == item);
                    if (current == null)
                        continue;
                    if (old == null)
                    {
                        //重新添加
                        //var entity = new CTZQ_Match();
                        //ObjectConvert.ConverInfoToEntity<MatchBiz.Core.CTZQ_MatchInfo, CTZQ_Match>(current, ref entity);
                        DB.GetDal<C_CTZQ_Match>().Add(new C_CTZQ_Match
                        {
                            Color = current.Color,
                            GuestTeamHalfScore = current.GuestTeamHalfScore,
                            GuestTeamScore = current.GuestTeamScore,
                            GuestTeamStanding = int.Parse(current.GuestTeamStanding),
                            GuestTeamId = current.GuestTeamId,
                            GuestTeamName = current.GuestTeamName,
                            HomeTeamHalfScore = current.HomeTeamHalfScore,
                            HomeTeamId = current.HomeTeamId,
                            HomeTeamName = current.HomeTeamName,
                            HomeTeamScore = current.HomeTeamScore,
                            HomeTeamStanding =int.Parse( current.HomeTeamStanding),
                            MatchResult = current.MatchResult,
                            MatchStartTime = DateTime.Parse(current.MatchStartTime),
                            MatchState =(int) current.MatchState,
                            UpdateTime = DateTime.Parse(current.UpdateTime),
                            GameCode = current.GameCode,
                            GameType = current.GameType,
                            Id = current.Id,
                            IssuseNumber = current.IssuseNumber,
                            MatchId = current.MatchId,
                            MatchName = current.MatchName,
                            Mid = current.Mid,
                            OrderNumber = current.OrderNumber,
                        });
                        continue;
                    }
                    old.Color = current.Color;
                    old.GuestTeamHalfScore = current.GuestTeamHalfScore;
                    old.GuestTeamScore = current.GuestTeamScore;
                    old.GuestTeamStanding = int.Parse(current.GuestTeamStanding);
                    old.GuestTeamId = current.GuestTeamId;
                    old.GuestTeamName = current.GuestTeamName;
                    old.HomeTeamHalfScore = current.HomeTeamHalfScore;
                    old.HomeTeamId = current.HomeTeamId;
                    old.HomeTeamName = current.HomeTeamName;
                    old.HomeTeamScore = current.HomeTeamScore;
                    old.HomeTeamStanding = int.Parse(current.HomeTeamStanding);
                    old.MatchResult = current.MatchResult;
                    old.MatchStartTime = DateTime.Parse(current.MatchStartTime);
                    old.MatchState = (int)current.MatchState;
                    old.UpdateTime = DateTime.Parse(current.UpdateTime);
                    DB.GetDal<C_CTZQ_Match>().Update(old);
                }

                DB.Commit();

                }
                catch (Exception)
                {
                    DB.Rollback();
                    throw;
                }
            }
        }

        public void ManualUpdate_CTZQ_MatchList(string gameCode, string issuseNumber)
        {
            var list = LoadCTZQMatchList(issuseNumber, gameCode);
            var matchIdArray = list.Select(p => p.Id).ToArray();
            UpdateCTZQMatchList(matchIdArray, list);
        }
       
        public void Update_CTZQ_MatchPoolList(string gameCode, string issuseNumber, string[] array)
        {
            var list = LoadCTZQBonusLevelList(issuseNumber, gameCode);
            var lotteryManager = new LotteryGameManager();
            var manager = new CTZQMatchManager();
            //开启事务
            using (DB)
            {
                

              
               
                var oldList = manager.QueryCTZQMatchPoolListById(array);
                DB.Begin();
                try
                {

            
                foreach (var item in array)
                {
                    var old = oldList.FirstOrDefault(p => p.Id == item);
                    var current = list.FirstOrDefault(p => p.Id == item);
                    if (current == null)
                        continue;
                    if (old == null)
                    {
                        //重新添加
                        //var entity = new CTZQ_MatchPool();
                        //ObjectConvert.ConverInfoToEntity<MatchBiz.Core.CTZQ_BonusLevelInfo, CTZQ_MatchPool>(current, ref entity);
                        manager.AddCTZQ_MatchPool(new C_CTZQ_MatchPool
                        {
                            BonusCount = current.BonusCount,
                            BonusLevel = current.BonusLevel,
                            BonusLevelDisplayName = current.BonusLevelDisplayName,
                            BonusMoney = current.BonusMoney,
                            MatchResult = current.MatchResult,
                            CreateTime = DateTime.Parse(current.CreateTime),
                            GameCode = current.GameCode,
                            GameType = current.GameType,
                            Id = current.Id,
                            IssuseNumber = current.IssuseNumber,
                        });
                        continue;
                    }
                    old.BonusCount = current.BonusCount;
                    old.BonusLevel = current.BonusLevel;
                    old.BonusLevelDisplayName = current.BonusLevelDisplayName;
                    old.BonusMoney = current.BonusMoney;
                    old.MatchResult = current.MatchResult;
                    manager.UpdateCTZQ_MatchPool(old);

                    //更新奖期数据
                    var issuseEntity = lotteryManager.QueryGameIssuseByKey(old.GameCode, old.GameType, old.IssuseNumber);
                    if (issuseEntity != null && issuseEntity.Status !=(int) IssuseStatus.Stopped && string.IsNullOrEmpty(issuseEntity.WinNumber))
                    {
                        issuseEntity.Status = (int)IssuseStatus.Stopped;
                        issuseEntity.AwardTime = DateTime.Now;
                        issuseEntity.WinNumber = current.MatchResult;
                        lotteryManager.UpdateGameIssuse(issuseEntity);
                    }

                    //更新队伍信息
                    UpdateCTZQMatchEntityResult(gameCode, issuseNumber, old.MatchResult);
                }

                DB.Commit();
                }
                catch (Exception)
                {
                    DB.Rollback();
                    throw;
                }
            }
        }

        private void UpdateCTZQMatchEntityResult(string gameType, string issuseNumber, string result)
        {
            var updateMatchSql = new List<string>();
            var resultArray = result.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            switch (gameType)
            {
                case "T6BQC":
                    //分上下半场 特殊处理
                    for (int i = 0; i < 6; i++)
                    {
                        var id = string.Format("{0}|{1}|{2}|{3}", "CTZQ", gameType, issuseNumber, i + 1);
                        updateMatchSql.Add(string.Format("update C_CTZQ_Match set MatchResult='{0}' where Id='{1}'", string.Join(",", resultArray[i * 2], resultArray[i * 2 + 1]), id));
                    }
                    break;
                case "T4CJQ":
                    //分上下半场 特殊处理
                    for (int i = 0; i < 4; i++)
                    {
                        var id = string.Format("{0}|{1}|{2}|{3}", "CTZQ", gameType, issuseNumber, i + 1);
                        updateMatchSql.Add(string.Format("update C_CTZQ_Match set MatchResult='{0}' where Id='{1}'", string.Join(",", resultArray[i * 2], resultArray[i * 2 + 1]), id));
                    }
                    break;
                default:
                    for (int i = 0; i < resultArray.Length; i++)
                    {
                        var id = string.Format("{0}|{1}|{2}|{3}", "CTZQ", gameType, issuseNumber, i + 1);
                        updateMatchSql.Add(string.Format("update C_CTZQ_Match set MatchResult='{0}' where Id='{1}'", resultArray[i], id));
                    }
                    break;
            }
            if (updateMatchSql.Count != 0)
            {
                try
                {
                    new CTZQMatchManager().ExecUpdateCTZQMatch(string.Join(Environment.NewLine, updateMatchSql.ToArray()));
                }
                catch (Exception ex)
                {
                    throw new Exception("更新队伍结果失败：" + ex.ToString(), ex);
                }
            }
        }

        /// <summary>
        /// 更新CTZQ命中场次
        /// </summary>
        public void Update_CTZQ_HitCount(string gameCode, string issuseNumber)
        {
            //暂停业务
            return;

            //1 普通投注
            //var pool = new CTZQMatchManager().QueryCTZQ_MatchPoolByKey(string.Format("{0}|{1}|{2}|{3}", "CTZQ", gameCode, issuseNumber, "1"));
            //if (pool == null) return;
            //if (string.IsNullOrEmpty(pool.MatchResult)) return;

            //var bonusCodeArray = pool.MatchResult.Split(',');
            //var sportsManager = new Sports_Manager();
            //var codeList = sportsManager.QueryCTZQSportsAnteCodeByIssuseNumber(gameCode, issuseNumber);
            //var dealSchemeList = new List<string>();
            //foreach (var item in codeList)
            //{
            //    if (dealSchemeList.Contains(item.SchemeId)) continue;

            //    Update_CTZQ_General_HitCount(item.GameType, item.SchemeId, item.AnteCode, bonusCodeArray);

            //    dealSchemeList.Add(item.SchemeId);
            //}
            ////2 单式上传
            //sportsManager = new Sports_Manager();
            //var singleCodeList = sportsManager.QuerySingleScheme_AnteCode("CTZQ", gameCode, issuseNumber);
            //dealSchemeList = new List<string>();
            //foreach (var item in singleCodeList)
            //{
            //    if (dealSchemeList.Contains(item.SchemeId)) continue;

            //    Update_CTZQ_Single_HitCount(item, bonusCodeArray);

            //    dealSchemeList.Add(item.SchemeId);
            //}
        }

        /// <summary>
        /// 更新单个订单的过关统计
        /// </summary>
        public void Update_CTZQ_HitCountBySchemeId(string schemeId)
        {
            //暂停业务
            return;

            //var sportsManager = new Sports_Manager();
            //var order = sportsManager.QuerySports_Order_Running(schemeId);
            //if (order == null)
            //    throw new Exception(string.Format("没有找到{0}的Order_Running数据", schemeId));

            //var poolKey = string.Format("{0}|{1}|{2}|{3}", "CTZQ", order.GameType, order.IssuseNumber, "1");
            //var pool = new CTZQMatchManager().QueryCTZQ_MatchPoolByKey(poolKey);
            //if (pool == null)
            //    throw new Exception(string.Format("没有找到Key为{0}的奖池数据", poolKey));
            //if (string.IsNullOrEmpty(pool.MatchResult))
            //    throw new Exception(string.Format("Key为{0}的开奖结果为空", poolKey));
            //var bonusCodeArray = pool.MatchResult.Split(',');

            ////普通投注
            //if (order.SchemeBettingCategory == SchemeBettingCategory.GeneralBetting)
            //{
            //    var code = sportsManager.QueryOneSportsAnteCodeBySchemeId(schemeId);
            //    Update_CTZQ_General_HitCount(order.GameType, order.SchemeId, code.AnteCode, bonusCodeArray);
            //    return;
            //}
            ////单式上传 和 过滤投注
            //var singleCode = sportsManager.QuerySingleScheme_AnteCode(schemeId);
            //Update_CTZQ_Single_HitCount(singleCode, bonusCodeArray);
        }

        public void Update_CTZQ_General_HitCount(string gameType, string schemeId, string anteCode, string[] bonusCodeArray)
        {
            var allCodeArray = anteCode.Split('|');
            var codeArray = allCodeArray[0].Split(',');
            if (codeArray.Length != bonusCodeArray.Length) return;
            var danCodeArray = new string[] { };
            if (allCodeArray.Length == 2)
                danCodeArray = allCodeArray[1].Split(',');

            #region 计算命中场数

            //计算命中场数
            var hitCount = 0;
            switch (gameType)
            {
                case "T14C":
                case "TR9":
                    for (int i = 0; i < codeArray.Length; i++)
                    {
                        if (bonusCodeArray[i] == "*") continue;
                        var code = codeArray[i];
                        if (code.Contains(bonusCodeArray[i]))
                            hitCount++;
                    }
                    break;
                case "T6BQC":
                case "T4CJQ":
                    for (int i = 0; i < codeArray.Length / 2; i++)
                    {
                        if (bonusCodeArray[i * 2] == "*" || bonusCodeArray[i * 2 + 1] == "*") continue;
                        var code1 = codeArray[i * 2];
                        var code2 = codeArray[i * 2 + 1];
                        if (code1.Contains(bonusCodeArray[i * 2]) && code2.Contains(bonusCodeArray[i * 2 + 1]))
                            hitCount++;
                    }
                    break;
            }

            #endregion

            #region 计算全中、错1、错2场注数

            //计算全中、错1、错2场注数
            var rightCount = 0;
            var error1Count = 0;
            var error2Count = 0;
            //构建二维数组
            var list = new List<string[]>();
            foreach (var item in codeArray)
            {
                var t = (from p in item.ToArray() select p.ToString()).ToArray();
                list.Add(t);
            }
            var arr = list.ToArray();
            var c = new ArrayCombination();
            switch (gameType)
            {
                case "T14C":
                case "T6BQC":
                case "T4CJQ":
                    c.Calculate(arr, (item) =>
                    {
                        var errorCount = 0;
                        for (int i = 0; i < item.Length; i++)
                        {
                            var r = bonusCodeArray[i];
                            if (r == "*") continue;
                            if (item[i] != r)
                                errorCount++;
                        }
                        if (errorCount == 0)
                            rightCount++;
                        if (errorCount == 1)
                            error1Count++;
                        if (errorCount == 2)
                            error2Count++;
                    });
                    break;
                case "TR9":
                    //todo 设胆的情况
                    //3,1,0,1,3,10,1,3,1,1,*,*,*,*|5,8,9
                    //有胆
                    var danAllRight = true;
                    foreach (var item in danCodeArray)
                    {
                        var danIndex = int.Parse(item);
                        if (danIndex > 13) continue;
                        if (bonusCodeArray[danIndex] == "*") continue;
                        if (codeArray[danIndex].Contains(bonusCodeArray[danIndex])) continue;
                        danAllRight = false;
                        break;
                    }
                    if (danAllRight)
                    {
                        var tr9Index = new List<int>();
                        for (int i = 0; i < codeArray.Length; i++)
                        {
                            if (danCodeArray.Contains(i.ToString())) continue;

                            if (codeArray[i] != "*")
                                tr9Index.Add(i);
                        }
                        new Combination().Calculate<int>(tr9Index.ToArray(), 9 - danCodeArray.Length, (indexItem) =>
                        {
                            var tmplist = new List<string[]>();
                            for (var i = 0; i < 14; i++)
                            {
                                if (indexItem.Contains(i))
                                {
                                    tmplist.Add(arr[i]);
                                }
                                else
                                {
                                    tmplist.Add(new string[] { "*" });
                                }
                            }
                            c.Calculate(tmplist.ToArray(), (item) =>
                            {
                                var errorCount = 0;
                                for (int i = 0; i < item.Length; i++)
                                {
                                    var current = item[i];
                                    if (current == "*") continue;
                                    var r = bonusCodeArray[i];
                                    if (r == "*") continue;
                                    if (current != r)
                                        errorCount++;
                                }
                                if (errorCount == 0)
                                    rightCount++;
                                if (errorCount == 1)
                                    error1Count++;
                                if (errorCount == 2)
                                    error2Count++;
                            });
                        });
                    }
                    else
                    {
                        rightCount = 0;
                        error1Count = 0;
                        error2Count = 0;
                    }
                    break;
            }

            #endregion

            UpdateOrder_RunningHitMatchCount(schemeId, hitCount, rightCount, error1Count, error2Count, 0);
        }

        public void Update_CTZQ_Single_HitCount(C_SingleScheme_AnteCode anteCode, string[] bonusCodeArray)
        {
            var allowCodeArray = anteCode.AllowCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //var codeText = File.ReadAllText(anteCode.AnteCodeFullFileName, Encoding.UTF8);
            var codeText = Encoding.UTF8.GetString(anteCode.FileBuffer);
            var ctzqMatchIdList = new List<string>();
            var ctzqCodeList = AnalyzerFactory.CheckCTZQSingleSchemeAnteCode(codeText, anteCode.GameType, allowCodeArray, out ctzqMatchIdList);

            var rightCount = 0;
            var error1Count = 0;
            var error2Count = 0;
            var hitCount = 0;

            foreach (var ctzqCode in ctzqCodeList)
            {
                var tempHitCount = 0;
                var codeArray = string.Join(",", ctzqCode.ToArray()).Split(',');
                #region 计算命中场数

                //计算命中场数
                switch (anteCode.GameType)
                {
                    case "T14C":
                    case "TR9":
                        for (int i = 0; i < codeArray.Length; i++)
                        {
                            if (bonusCodeArray[i] == "*") continue;
                            var code = codeArray[i];
                            if (code == "*") continue;
                            if (code.Contains(bonusCodeArray[i]))
                                tempHitCount++;
                        }
                        break;
                    case "T6BQC":
                    case "T4CJQ":
                        for (int i = 0; i < codeArray.Length / 2; i++)
                        {
                            if (bonusCodeArray[i * 2] == "*" || bonusCodeArray[i * 2 + 1] == "*") continue;
                            var code1 = codeArray[i * 2];
                            var code2 = codeArray[i * 2 + 1];
                            if (code1.Contains(bonusCodeArray[i * 2]) && code2.Contains(bonusCodeArray[i * 2 + 1]))
                                tempHitCount++;
                        }
                        break;
                }
                if (tempHitCount > hitCount)
                    hitCount = tempHitCount;

                #endregion

                #region 计算全中、错1、错2场注数

                //构建二维数组
                var list = new List<string[]>();
                foreach (var item in codeArray)
                {
                    var t = (from p in item.ToArray() select p.ToString()).ToArray();
                    list.Add(t);
                }
                var arr = list.ToArray();
                var c = new ArrayCombination();
                switch (anteCode.GameType)
                {
                    case "T14C":
                    case "T6BQC":
                    case "T4CJQ":
                        c.Calculate(arr, (item) =>
                        {
                            var errorCount = 0;
                            for (int i = 0; i < item.Length; i++)
                            {
                                var r = bonusCodeArray[i];
                                if (r == "*") continue;
                                if (item[i] != r)
                                    errorCount++;
                            }
                            if (errorCount == 0)
                                rightCount++;
                            if (errorCount == 1)
                                error1Count++;
                            if (errorCount == 2)
                                error2Count++;
                        });
                        break;
                    case "TR9":
                        var tr9Index = new List<int>();
                        for (int i = 0; i < codeArray.Length; i++)
                        {
                            if (codeArray[i] != "*")
                                tr9Index.Add(i);
                        }
                        new Combination().Calculate<int>(tr9Index.ToArray(), 9, (indexItem) =>
                        {
                            var tmplist = new List<string[]>();
                            for (var i = 0; i < 14; i++)
                            {
                                if (indexItem.Contains(i))
                                {
                                    tmplist.Add(arr[i]);
                                }
                                else
                                {
                                    tmplist.Add(new string[] { "*" });
                                }
                            }
                            c.Calculate(tmplist.ToArray(), (item) =>
                            {
                                var errorCount = 0;
                                for (int i = 0; i < item.Length; i++)
                                {
                                    var current = item[i];
                                    if (current == "*") continue;
                                    var r = bonusCodeArray[i];
                                    if (r == "*") continue;
                                    if (current != r)
                                        errorCount++;
                                }
                                if (errorCount == 0)
                                    rightCount++;
                                if (errorCount == 1)
                                    error1Count++;
                                if (errorCount == 2)
                                    error2Count++;
                            });
                        });
                        break;
                }

                #endregion
            }

            UpdateOrder_RunningHitMatchCount(anteCode.SchemeId, hitCount, rightCount, error1Count, error2Count, 0);
        }

        public string QueryCTZQCurrentIssuse()
        {
            var current = new CTZQMatchManager().QueryCTZQCurrentIssuse();
            if (current == null)
                return string.Empty;
            return current.IssuseNumber;
        }

        public Issuse_QueryInfo QueryCTZQCurrentIssuse(string gameType)
        {
            var current = new CTZQMatchManager().QueryCTZQCurrentIssuse(gameType);
            return current;
        }

        #endregion

        #region 竞彩足球
        private List<JCZQ_MatchResultInfo> LoadJCZQMatchResultList()
        {
            //var fileName = Path.Combine(_baseDir, string.Format(@"{0}\Match_Result_List.json", "JCZQ"));
            //var fileName = string.Format(@"{1}\{0}\Match_Result_List.json", "JCZQ", _baseDir);
            ////if (!File.Exists(fileName))
            ////    throw new ArgumentException("未找到数据文件" + fileName);

            //var json = ReadFileString(fileName);
            //if (string.IsNullOrEmpty(json))
            //    return new List<MatchBiz.Core.JCZQ_MatchResultInfo>(); ;
            //return JsonSerializer.Deserialize<List<MatchBiz.Core.JCZQ_MatchResultInfo>>(json);

            var coll = mDB.GetCollection<JCZQ_MatchResultInfo>("JCZQ_MatchResultInfo");
            var builder = Builders<JCZQ_MatchResultInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
            //  DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

            var filter = builder.Ne("IssuseNumber", ""); //& builder.Lte("GameCode", gameType) & builder.Lte("IssuseNumber", issuseNumber);
            var documents = coll.Find<JCZQ_MatchResultInfo>(null).ToList();
            return documents;
        }
        private List<JCZQ_MatchInfo> LoadJCZQMatchList()
        {
            //var fileName = Path.Combine(_baseDir, string.Format(@"{0}\Match_List.json", "JCZQ"));
            //var fileName = string.Format(@"{1}\{0}\Match_List_FB.json", "JCZQ", _baseDir);
            ////if (!File.Exists(fileName))
            ////    throw new ArgumentException("未找到数据文件" + fileName);

            //var json = ReadFileString(fileName);
            //if (string.IsNullOrEmpty(json))
            //    return new List<JCZQ_MatchInfo>();
            //return JsonSerializer.Deserialize<List<JCZQ_MatchInfo>>(json);


            var coll = mDB.GetCollection<JCZQ_MatchInfo>("JCZQ_MatchInfo");
            var builder = Builders<JCZQ_MatchInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
            //  DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

            var filter = builder.Ne("IssuseNumber", ""); //& builder.Lte("GameCode", gameType) & builder.Lte("IssuseNumber", issuseNumber);
            var documents = coll.Find<JCZQ_MatchInfo>(null).ToList();

            return documents;
        }

        private List<JCZQ_SJBMatchInfo> LoadJCZQ_SJB_GJ_MatchList()
        {
            //var fileName = string.Format(@"{1}\{0}\SJB_GJ.json", "SJB", _baseDir);
            //var json = ReadFileString(fileName);
            //if (string.IsNullOrEmpty(json))
            //    return new List<JCZQ_SJBMatchInfo>();
            //return JsonSerializer.Deserialize<List<JCZQ_SJBMatchInfo>>(json);

            var coll = mDB.GetCollection<JCZQ_SJBMatchInfo>("JCZQ_SJBMatchInfo");
            var builder = Builders<JCZQ_SJBMatchInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
            //  DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

           //var filter = builder.Ne("IssuseNumber", ""); //& builder.Lte("GameCode", gameType) & builder.Lte("IssuseNumber", issuseNumber);
            var documents = coll.Find<JCZQ_SJBMatchInfo>(null).ToList();
            return documents;
        }
        private List<JCZQ_SJBMatchInfo> LoadJCZQ_SJB_GYJ_MatchList()
        {
            //var fileName = string.Format(@"{1}\{0}\SJB_GJ.json", "SJB", _baseDir);
            //var json = ReadFileString(fileName);
            //if (string.IsNullOrEmpty(json))
            //    return new List<JCZQ_SJBMatchInfo>();
            //return JsonSerializer.Deserialize<List<JCZQ_SJBMatchInfo>>(json);

            var coll = mDB.GetCollection<JCZQ_SJBMatchInfo>("JCZQ_SJBMatchInfo");
            var builder = Builders<JCZQ_SJBMatchInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
            //  DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

            //var filter = builder.Ne("IssuseNumber", ""); //& builder.Lte("GameCode", gameType) & builder.Lte("IssuseNumber", issuseNumber);
            var documents = coll.Find<JCZQ_SJBMatchInfo>(null).ToList();
            return documents;
        }
        private List<JCZQ_OZBMatchInfo> LoadJCZQ_OZB_GYJ_MatchList()
        {

            var coll = mDB.GetCollection<JCZQ_OZBMatchInfo>("JCZQ_OZBMatchInfo");
            var builder = Builders<JCZQ_OZBMatchInfo>.Filter;
            // DateTime startTime = DateTime.Now.AddMonths(1);
            //  DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1, 12, 0, 0, DateTimeKind.Utc);

            //var filter = builder.Ne("IssuseNumber", ""); //& builder.Lte("GameCode", gameType) & builder.Lte("IssuseNumber", issuseNumber);
            var documents = coll.Find<JCZQ_OZBMatchInfo>(null).ToList();
            return documents;
        }
        public void Update_JCZQ_MatchList(string[] matchIdArray)
        {
            var matchInfoList = LoadJCZQMatchList();
            UpdateJCZQMatch(matchIdArray, matchInfoList);
            //try
            //{
            //    JCZQ_MatchInfo_Collection collection = new JCZQ_MatchInfo_Collection();
            //    var result = matchInfoList.Where(s => s.State == "NR" || s.State == "N").ToList();
            //    collection.MatchList.AddRange(result);
            //    BusinessHelper.ExecPlugin<IGetJCSingleMatchData>(new object[] { collection });
            //}
            //catch { }


            //重新加载比赛到缓存
            RedisMatchBusiness.ReloadCurrentJCZQMatch();
        }

        private void UpdateJCZQMatch(string[] matchIdArray, List<JCZQ_MatchInfo> matchInfoList)
        {
            //开启事务
            var manager = new JCZQMatchManager();
            using (manager.DB)
            {
            

              
                var oldList = manager.QueryJCZQ_MatchListByMatchId(matchIdArray);
                var oldResultList = manager.QueryJCZQ_MatchResult_PrizeListByMatchId(matchIdArray);
                manager.DB.Begin();
                try
                {

               
                foreach (var item in matchIdArray)
                {
                    var old = oldList.FirstOrDefault(p => p.MatchId == item);
                    var oldResult = oldResultList.FirstOrDefault(p => p.MatchId == item);
                    var current = matchInfoList.FirstOrDefault(p => p.MatchId == item);
                    if (current == null)
                        continue;

                    if (old == null)
                    {
                        //重新添加
                        //var entity = new JCZQ_Match();
                        //ObjectConvert.ConverInfoToEntity<MatchBiz.Core.JCZQ_MatchInfo, JCZQ_Match>(current, ref entity);
                        manager.AddJCZQ_Match(new C_JCZQ_Match
                        {
                            CreateTime = DateTime.Parse(current.CreateTime),
                            StartDateTime = DateTime.Parse(current.StartDateTime),
                            DSStopBettingTime = DateTime.Parse(current.DSStopBettingTime),
                            FSStopBettingTime = DateTime.Parse(current.FSStopBettingTime),
                            WinOdds = current.WinOdds,
                            FlatOdds = current.FlatOdds,
                            LoseOdds = current.LoseOdds,
                            LetBall = current.LetBall,
                            LeagueColor = current.LeagueColor,
                            LeagueId = current.LeagueId,
                            LeagueName = current.LeagueName,
                            HomeTeamId = current.HomeTeamId,
                            HomeTeamName = current.HomeTeamName,
                            GuestTeamId = current.GuestTeamId,
                            GuestTeamName = current.GuestTeamName,
                            PrivilegesType = current.PrivilegesType,
                            MatchData = current.MatchData,
                            MatchId = current.MatchId,
                            MatchIdName = current.MatchIdName,
                            MatchNumber = current.MatchNumber,
                            Mid = current.Mid,
                            MatchStopDesc = current.MatchStopDesc,
                        });
                    }
                    else
                    {
                        old.StartDateTime = DateTime.Parse(current.StartDateTime);
                        old.DSStopBettingTime = DateTime.Parse(current.DSStopBettingTime);
                        old.FSStopBettingTime = DateTime.Parse(current.FSStopBettingTime);
                        old.WinOdds = current.WinOdds;
                        old.FlatOdds = current.FlatOdds;
                        old.LoseOdds = current.LoseOdds;
                        old.LetBall = current.LetBall;
                        old.LeagueColor = current.LeagueColor;
                        old.LeagueId = current.LeagueId;
                        old.LeagueName = current.LeagueName;
                        old.HomeTeamId = current.HomeTeamId;
                        old.HomeTeamName = current.HomeTeamName;
                        old.GuestTeamId = current.GuestTeamId;
                        old.GuestTeamName = current.GuestTeamName;
                        //old.PrivilegesType = current.PrivilegesType;
                        old.MatchStopDesc = current.MatchStopDesc;
                        manager.UpdateJCZQ_Match(old);
                    }

                    if (oldResult == null)
                    {
                        manager.AddJCZQ_MatchResult_Prize(new C_JCZQ_MatchResult_Prize
                        {
                            CreateTime = DateTime.Parse(current.CreateTime),
                            MatchData = current.MatchData,
                            MatchId = current.MatchId,
                            MatchNumber = current.MatchNumber,
                            FullGuestTeamScore = 0,
                            FullHomeTeamScore = 0,
                            HalfHomeTeamScore = 0,
                            HalfGuestTeamScore = 0,
                            BF_Result = "-",
                            BQC_Result = "-",
                            BRQSPF_Result = "-",
                            SPF_Result = "-",
                            ZJQ_Result = "-",
                            BF_SP = 0M,
                            BQC_SP = 0M,
                            BRQSPF_SP = 0M,
                            SPF_SP = 0M,
                            ZJQ_SP = 0M,
                            MatchState = "0",
                        });
                    }
                }

                manager.DB.Commit();
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public void ManualUpdate_JCZQ_MatchList()
        {
            var matchInfoList = LoadJCZQMatchList();
            var matchIdArray = matchInfoList.Select(p => p.MatchId).ToArray();
            UpdateJCZQMatch(matchIdArray, matchInfoList);

            //重新加载比赛到缓存
            RedisMatchBusiness.ReloadCurrentJCZQMatch();
        }

        public void Update_JCZQ_MatchResultList(string[] matchIdArray)
        {
            var matchResultList = LoadJCZQMatchResultList();
            //开启事务
            var manager = new JCZQMatchManager();
            using (manager.DB)
            {
             

                //延迟分钟数
                var delayMinute = 10;
               
                var oldList = manager.QueryJCZQ_MatchResultListByMatchId(matchIdArray);//数据库
                var oldResultList = manager.QueryJCZQ_MatchResult_PrizeListByMatchId(matchIdArray);//采集
                manager.DB.Begin();
                try
                {

                foreach (var item in matchIdArray)
                {
                    var old = oldList.FirstOrDefault(p => p.MatchId == item);
                    var oldResult = oldResultList.FirstOrDefault(p => p.MatchId == item);
                    var current = matchResultList.FirstOrDefault(p => p.MatchId == item);
                    if (current == null)
                        continue;

                    var state = "1";
                    if (old == null)
                    {
                        //重新添加
                        old = new C_JCZQ_MatchResult
                        {
                            HalfGuestTeamScore = current.HalfGuestTeamScore,
                            HalfHomeTeamScore = current.HalfHomeTeamScore,
                            FullGuestTeamScore = current.FullGuestTeamScore,
                            FullHomeTeamScore = current.FullHomeTeamScore,
                            MatchState = state,
                            SPF_Result = current.SPF_Result,
                            SPF_SP = current.SPF_SP,
                            BRQSPF_Result = current.BRQSPF_Result,
                            BRQSPF_SP = current.BRQSPF_SP,
                            BF_Result = current.BF_Result,
                            BF_SP = current.BF_SP,
                            ZJQ_Result = current.ZJQ_Result,
                            ZJQ_SP = current.ZJQ_SP,
                            BQC_Result = current.BQC_Result,
                            BQC_SP = current.BQC_SP,
                            CreateTime = DateTime.Now,
                            MatchData = current.MatchData,
                            MatchId = current.MatchId,
                            MatchNumber = current.MatchNumber,
                        };
                        manager.AddJCZQ_MatchResult(old);
                        //manager.AddJCZQ_MatchResult(new JCZQ_MatchResult
                        //{
                        //    HalfGuestTeamScore = current.HalfGuestTeamScore,
                        //    HalfHomeTeamScore = current.HalfHomeTeamScore,
                        //    FullGuestTeamScore = current.FullGuestTeamScore,
                        //    FullHomeTeamScore = current.FullHomeTeamScore,
                        //    MatchState = state,
                        //    SPF_Result = current.SPF_Result,
                        //    SPF_SP = current.SPF_SP,
                        //    BRQSPF_Result = current.BRQSPF_Result,
                        //    BRQSPF_SP = current.BRQSPF_SP,
                        //    BF_Result = current.BF_Result,
                        //    BF_SP = current.BF_SP,
                        //    ZJQ_Result = current.ZJQ_Result,
                        //    ZJQ_SP = current.ZJQ_SP,
                        //    BQC_Result = current.BQC_Result,
                        //    BQC_SP = current.BQC_SP,
                        //    CreateTime = DateTime.Now,
                        //    MatchData = current.MatchData,
                        //    MatchId = current.MatchId,
                        //    MatchNumber = current.MatchNumber,
                        //});
                    }
                    else
                    {
                        if ((DateTime.Now - old.CreateTime).TotalMinutes > delayMinute && old.MatchState != "2")
                        {
                            if ((current.SPF_SP > 1M || current.SPF_SP == 0M)
                            && (current.BRQSPF_SP > 1M || current.BRQSPF_SP == 0M)
                            && (current.ZJQ_SP > 1M || current.ZJQ_SP == 0M)
                            && (current.BF_SP > 1M || current.BF_SP == 0M)
                            && (current.BQC_SP > 1M || current.BQC_SP == 0M)
                            && current.MatchState == "2")
                                state = "2";

                            old.HalfGuestTeamScore = current.HalfGuestTeamScore;
                            old.HalfHomeTeamScore = current.HalfHomeTeamScore;
                            old.FullGuestTeamScore = current.FullGuestTeamScore;
                            old.FullHomeTeamScore = current.FullHomeTeamScore;
                            old.MatchState = state;
                            old.SPF_Result = current.SPF_Result;
                            old.SPF_SP = current.SPF_SP;
                            old.BRQSPF_Result = current.BRQSPF_Result;
                            old.BRQSPF_SP = current.BRQSPF_SP;
                            old.BF_Result = current.BF_Result;
                            old.BF_SP = current.BF_SP;
                            old.ZJQ_Result = current.ZJQ_Result;
                            old.ZJQ_SP = current.ZJQ_SP;
                            old.BQC_Result = current.BQC_Result;
                            old.BQC_SP = current.BQC_SP;
                            manager.UpdateJCZQ_MatchResult(old);
                        }
                    }

                    if (oldResult == null)
                    {
                        //重新添加
                        manager.AddJCZQ_MatchResult_Prize(new C_JCZQ_MatchResult_Prize
                        {
                            HalfGuestTeamScore = current.HalfGuestTeamScore,
                            HalfHomeTeamScore = current.HalfHomeTeamScore,
                            FullGuestTeamScore = current.FullGuestTeamScore,
                            FullHomeTeamScore = current.FullHomeTeamScore,
                            MatchState = state,
                            SPF_Result = current.SPF_Result,
                            SPF_SP = current.SPF_SP,
                            BRQSPF_Result = current.BRQSPF_Result,
                            BRQSPF_SP = current.BRQSPF_SP,
                            BF_Result = current.BF_Result,
                            BF_SP = current.BF_SP,
                            ZJQ_Result = current.ZJQ_Result,
                            ZJQ_SP = current.ZJQ_SP,
                            BQC_Result = current.BQC_Result,
                            BQC_SP = current.BQC_SP,
                            CreateTime = DateTime.Now,
                            MatchData = current.MatchData,
                            MatchId = current.MatchId,
                            MatchNumber = current.MatchNumber,
                        });
                    }
                    else
                    {
                        if ((DateTime.Now - old.CreateTime).TotalMinutes > delayMinute && old.MatchState != "2")
                        {
                            if ((current.SPF_SP > 1M || current.SPF_SP == 0M)
                           && (current.BRQSPF_SP > 1M || current.BRQSPF_SP == 0M)
                           && (current.ZJQ_SP > 1M || current.ZJQ_SP == 0M)
                           && (current.BF_SP > 1M || current.BF_SP == 0M)
                           && (current.BQC_SP > 1M || current.BQC_SP == 0M)
                           && current.MatchState == "2")
                                state = "2";

                            oldResult.HalfGuestTeamScore = current.HalfGuestTeamScore;
                            oldResult.HalfHomeTeamScore = current.HalfHomeTeamScore;
                            oldResult.FullGuestTeamScore = current.FullGuestTeamScore;
                            oldResult.FullHomeTeamScore = current.FullHomeTeamScore;
                            oldResult.MatchState = state;
                            oldResult.SPF_Result = current.SPF_Result;
                            oldResult.SPF_SP = current.SPF_SP;
                            oldResult.BRQSPF_Result = current.BRQSPF_Result;
                            oldResult.BRQSPF_SP = current.BRQSPF_SP;
                            oldResult.BF_Result = current.BF_Result;
                            oldResult.BF_SP = current.BF_SP;
                            oldResult.ZJQ_Result = current.ZJQ_Result;
                            oldResult.ZJQ_SP = current.ZJQ_SP;
                            oldResult.BQC_Result = current.BQC_Result;
                            oldResult.BQC_SP = current.BQC_SP;
                            manager.UpdateJCZQ_MatchResult_Prize(oldResult);
                        }
                    }
                }

                manager.DB.Commit();

                }
                catch (Exception)
                {
                    manager.DB.Rollback();
                    throw;
                }
            }
        }

        public void Update_JCZQ_OZB_GJ(string[] matchIdArray)
        {
            var matchInfoList = LoadJCZQ_OZB_GJ_MatchList();
            UpdateJCZQ_OZB_GJ_Match("GJ", matchIdArray, matchInfoList);

            //重新加载比赛到缓存
            //RedisMatchBusiness.ReloadCurrentJCZQ_OZBMatch();
        }

        public void Update_JCZQ_OZB_GYJ(string[] matchIdArray)
        {
            var matchInfoList = LoadJCZQ_OZB_GYJ_MatchList();
            UpdateJCZQ_OZB_GJ_Match("GYJ", matchIdArray, matchInfoList);

            //重新加载比赛到缓存
            //RedisMatchBusiness.ReloadCurrentJCZQ_OZBMatch();
        }

        private void UpdateJCZQ_OZB_GJ_Match(string gameType, string[] matchIdArray, List<JCZQ_OZBMatchInfo> matchInfoList)
        {
            //开启事务
            var manager = new JCZQMatchManager();
            using (manager.DB)
            {
                manager.DB.Begin();

                try
                {

             
               
                var oldList = manager.QueryJCZQ_OZBMatchList(gameType, matchIdArray);
                foreach (var item in matchIdArray)
                {
                    var old = oldList.FirstOrDefault(p => p.MatchId == item);
                    var current = matchInfoList.FirstOrDefault(p => p.MatchId == item);
                    if (current == null)
                        continue;

                    if (old == null)
                    {
                        //重新添加
                        manager.AddJCZQ_OZBMatch(new C_JCZQ_OZBMatch
                        {
                            BetState = current.BetState,
                            BonusMoney = current.BonusMoney,
                            GameCode = current.GameCode,
                            GameType = current.GameType,
                            IssuseNumber = current.IssuseNumber,
                            MatchId = current.MatchId,
                            Probadbility = current.Probadbility,
                            SupportRate = current.SupportRate,
                            TeamName = current.Team,
                            UpdateDateTime = DateTime.Now,
                        });
                    }
                    else
                    {
                        old.UpdateDateTime = DateTime.Now;
                        old.BetState = current.BetState;
                        old.BonusMoney = current.BonusMoney;
                        old.Probadbility = current.Probadbility;
                        old.SupportRate = current.SupportRate;
                        manager.UpdateJCZQ_OZBMatch(old);
                    }
                }

                manager.DB.Commit();
                }
                catch (Exception)
                {
                    manager.DB.Rollback();
                    throw;
                }
            }
        }


        public void Update_JCZQ_SJB_GJ(string[] matchIdArray)
        {
            var matchInfoList = LoadJCZQ_SJB_GJ_MatchList();
            UpdateJCZQ_SJB_GJ_Match("GJ", matchIdArray, matchInfoList);

            //重新加载比赛到缓存
            //RedisMatchBusiness.ReloadCurrentJCZQ_OZBMatch();
        }

        public void Update_JCZQ_SJB_GYJ(string[] matchIdArray)
        {
            var matchInfoList = LoadJCZQ_SJB_GYJ_MatchList();
            UpdateJCZQ_SJB_GJ_Match("GYJ", matchIdArray, matchInfoList);

            //重新加载比赛到缓存
            //RedisMatchBusiness.ReloadCurrentJCZQ_OZBMatch();
        }

        private void UpdateJCZQ_SJB_GJ_Match(string gameType, string[] matchIdArray, List<JCZQ_SJBMatchInfo> matchInfoList)
        {
            //开启事务
            var manager = new JCZQMatchManager();
            using (manager.DB)
            {
                var oldList = manager.QueryJCZQ_SJBMatchList(gameType, matchIdArray);
                manager.DB.Begin();

                try
                {

                  
                    foreach (var item in matchIdArray)
                    {
                        var old = oldList.FirstOrDefault(p => p.MatchId == item);
                        var current = matchInfoList.FirstOrDefault(p => p.MatchId == item);
                        if (current == null)
                            continue;

                        if (old == null)
                        {
                            //重新添加
                            manager.AddJCZQ_SJBMatch(new C_JCZQ_SJBMatch
                            {
                                BetState = current.BetState,
                                BonusMoney = current.BonusMoney,
                                GameCode = current.GameCode,
                                GameType = current.GameType,
                                IssuseNumber = current.IssuseNumber,
                                MatchId = current.MatchId,
                                Probadbility = current.Probadbility,
                                SupportRate = current.SupportRate,
                                TeamName = current.Team,
                                UpdateDateTime = DateTime.Now,
                            });
                        }
                        else
                        {
                            old.UpdateDateTime = DateTime.Now;
                            old.BetState = current.BetState;
                            old.BonusMoney = current.BonusMoney;
                            old.Probadbility = current.Probadbility;
                            old.SupportRate = current.SupportRate;
                            manager.UpdateJCZQ_SJBMatch(old);

                            if (current.BetState == "胜出")
                            {
                                LotteryGameManager lotterymanager = new LotteryGameManager();
                                var issuse = lotterymanager.QueryGameIssuseByKey("SJB", current.GameType, current.IssuseNumber);
                                if (issuse.Status != (int)IssuseStatus.Stopped)
                                {
                                    issuse.WinNumber = current.MatchId.ToString();
                                    issuse.AwardTime = DateTime.Now;
                                    issuse.Status = (int)IssuseStatus.Stopped;
                                    lotterymanager.UpdateGameIssuse(issuse);
                                }
                            }
                        }
                    }
                    manager.DB.Commit();
                }
                catch (Exception)
                {
                    manager.DB.Rollback();

                    throw;
                }
               

               
            }
        }

        /// <summary>
        /// todo:缓存
        /// </summary>
        public JCZQMatchResult_Collection QueryJCZQMatchResult(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var result = new JCZQMatchResult_Collection();
            var totalCount = 0;
            var manager = new JCZQMatchManager();
            result.MatchResultList = manager.QueryJCZQMatchResult(startTime, endTime, pageIndex, pageSize, out totalCount);
            result.TotalCount = totalCount;
            return result;
        }

        public JCZQMatchResult_Collection QueryJCZQMatchResult(DateTime time)
        {
            var result = new JCZQMatchResult_Collection();
            var manager = new JCZQMatchManager();
            result.MatchResultList = manager.QueryJCZQMatchResult(time);
            return result;
        }

        public IList<C_JCZQ_Match> QueryCurrentJCZQMatchInfo()
        {
            var collection = new List<C_JCZQ_Match>();
            var manager = new JCZQMatchManager();
            collection.AddRange(manager.QueryCurrentJCZQMatchInfo());
            return collection;
        }

        public void UpdateJCZQMatchInfo(string matchId, string privilegesType)
        {
            var manager = new JCZQMatchManager();
            var entity = manager.GetJCZQMatch(matchId);
            if (entity == null)
                throw new Exception(string.Format("比赛{0}不存在", matchId));
            entity.PrivilegesType = privilegesType;
            manager.UpdateJCZQ_Match(entity);
        }

        /// <summary>
        /// 更新JCZQ命中场数
        /// </summary>
        public void Update_JCZQ_HitCount(string[] matchIdArray)
        {
            //暂停业务
            return;

            //1 普通投注
            //var sportsManager = new Sports_Manager();
            //var dealSchemeList = new List<string>();
            //foreach (var item in sportsManager.QueryJCZQSportsAnteCodeByIsuseNumber(matchIdArray))
            //{
            //    if (dealSchemeList.Contains(item.SchemeId)) continue;

            //    Update_JCZQ_General_HitCount(item.GameType, item.SchemeId, matchIdArray);

            //    dealSchemeList.Add(item.SchemeId);
            //}

            ////2 单式上传
            //sportsManager = new Sports_Manager();
            //dealSchemeList = new List<string>();
            //foreach (var item in sportsManager.QueryJCZQSingleCodeByIsuseNumber(matchIdArray))
            //{
            //    if (string.IsNullOrEmpty(item)) continue;
            //    var array = item.Split('|');//[SchemeId]+'|'+[GameType]
            //    if (array.Length != 2) continue;
            //    if (dealSchemeList.Contains(array[0])) continue;

            //    Update_JCZQ_Single_HitCount(array[1], array[0]);

            //    dealSchemeList.Add(array[0]);
            //}
        }
        /// <summary>
        /// 更新单个订单的过关统计
        /// </summary>
        public void Update_JCZQ_HitCountBySchemeId(string schemeId)
        {
            //暂停业务
            return;
            //var sportsManager = new Sports_Manager();
            //var order = sportsManager.QuerySports_Order_Running(schemeId);
            //if (order == null)
            //    throw new Exception(string.Format("没有找到{0}的Order_Running数据", schemeId));
            ////普通投注
            //if (order.SchemeBettingCategory == SchemeBettingCategory.GeneralBetting)
            //{
            //    var codeList = sportsManager.QuerySportsAnteCodeBySchemeId(schemeId);
            //    Update_JCZQ_General_HitCount(order.GameType, schemeId, (from c in codeList select c.MatchId).ToArray());
            //    return;
            //}
            ////单式上传 和 过滤投注
            //Update_JCZQ_Single_HitCount(order.GameType, schemeId);
        }

        public void Update_JCZQ_General_HitCount(string gameType, string schemeId, string[] matchIdArray)
        {
            #region 计算命中场数

            //计算命中场数
            gameType = gameType.ToUpper();
            var hitCount = 0;
            var playType = string.Empty;
            var sportsManager = new Sports_Manager();
            var codeList = sportsManager.QuerySportsAnteCodeBySchemeId(schemeId);
            var jczqManager = new JCZQMatchManager();
            var matchResultList = jczqManager.QueryJCZQ_MatchResultListByMatchId(matchIdArray);
            var totalResultList = new List<C_JCZQ_MatchResult>();
            foreach (var code in codeList)
            {
                if (string.IsNullOrEmpty(playType))
                    playType = code.PlayType;
                var query = from r in matchResultList
                            where r.MatchId == code.MatchId
                            select r;
                var first = query.FirstOrDefault();
                if (totalResultList.FirstOrDefault(p => p.MatchId == code.MatchId) == null)
                {
                    totalResultList.Add(first == null ? new C_JCZQ_MatchResult
                    {
                        MatchId = code.MatchId,
                        SPF_Result = "-1",
                        ZJQ_Result = "-1",
                        BF_Result = "-1",
                        BQC_Result = "-1",
                    } : first);
                }
                switch (gameType)
                {
                    case "SPF":
                        query = query.Where(p => code.AnteCode.Contains(p.SPF_Result));
                        break;
                    case "BRQSPF":
                        query = query.Where(p => code.AnteCode.Contains(p.BRQSPF_Result));
                        break;
                    case "BF":
                        query = query.Where(p => code.AnteCode.Contains(p.BF_Result));
                        break;
                    case "ZJQ":
                        query = query.Where(p => code.AnteCode.Contains(p.ZJQ_Result));
                        break;
                    case "BQC":
                        query = query.Where(p => code.AnteCode.Contains(p.BQC_Result));
                        break;
                    default:
                        continue;
                }

                if (query.FirstOrDefault() != null)
                    hitCount++;
            }

            #endregion

            #region 计算全中、错1、错2场注数

            //计算全中、错1、错2场注数
            var rightCount = 0;
            var error1Count = 0;
            var error2Count = 0;
            var ticketList = sportsManager.QueryTicketList(schemeId);
            foreach (var ticket in ticketList)
            {
                if (string.IsNullOrEmpty(ticket.BetContent)) continue;
                //140905008_3/140906001_3或BRQSPF_140906001_0/ZJQ_140906002_2
                var betArray = ticket.BetContent.Split('/');
                var ticket_rightCount = 0;
                var ticket_errorCount = 0;
                foreach (var match in betArray)
                {
                    //140905008_3 或 ZJQ_140906002_2
                    var matchArray = match.Split('_');
                    if (matchArray.Length != 2 && matchArray.Length != 3) continue;

                    var c_gameType = ticket.GameType.ToUpper();
                    var c_matchId = string.Empty;
                    var c_betContent = string.Empty;
                    if (matchArray.Length == 2)
                    {
                        //140905008_3 
                        c_matchId = matchArray[0];
                        c_betContent = matchArray[1];
                    }
                    if (matchArray.Length == 3)
                    {
                        //ZJQ_140906002_2
                        c_gameType = matchArray[0].ToUpper();
                        c_matchId = matchArray[1];
                        c_betContent = matchArray[2];
                    }

                    var mr = matchResultList.Where(p => p.MatchId == c_matchId).FirstOrDefault();
                    if (mr == null) continue;

                    switch (c_gameType)
                    {
                        case "SPF":
                            if (c_betContent.Split(',').Contains(mr.SPF_Result))
                                ticket_rightCount++;
                            else
                                ticket_errorCount++;
                            break;
                        case "BRQSPF":
                            if (c_betContent.Split(',').Contains(mr.BRQSPF_Result))
                                ticket_rightCount++;
                            else
                                ticket_errorCount++;
                            break;
                        case "BF":
                            if (c_betContent.Split(',').Contains(mr.BF_Result))
                                ticket_rightCount++;
                            else
                                ticket_errorCount++;
                            break;
                        case "ZJQ":
                            if (c_betContent.Split(',').Contains(mr.ZJQ_Result))
                                ticket_rightCount++;
                            else
                                ticket_errorCount++;
                            break;
                        case "BQC":
                            if (c_betContent.Split(',').Contains(mr.BQC_Result))
                                ticket_rightCount++;
                            else
                                ticket_errorCount++;
                            break;
                        default:
                            continue;
                    }
                }
                //单张票的正确场数 等于投注票数
                if (ticket_rightCount == betArray.Length)
                {
                    rightCount++;
                }
                else if (ticket_errorCount == 1)
                {
                    error1Count++;
                }
                else if (ticket_errorCount == 2)
                {
                    error2Count++;
                }
            }

            #endregion

            #region 计算中奖注数

            var bonusCount = 0;
            foreach (var item in playType.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var t = item.Split('_');
                if (t.Length != 2) return;
                var baseCount = int.Parse(t[0]);
                var analyzer = AnalyzerFactory.GetSportAnalyzer("JCZQ", gameType, baseCount);
                var bonusResult = analyzer.CaculateBonus(codeList.ToArray(), totalResultList.ToArray());
                bonusCount += bonusResult.BonusCount;
            }

            #endregion

            UpdateOrder_RunningHitMatchCount(schemeId, hitCount, rightCount, error1Count, error2Count, bonusCount);
        }

        public void Update_JCZQ_Single_HitCount(string gameType, string schemeId)
        {
            var sportsManager = new Sports_Manager();
            var bjdcManager = new JCZQMatchManager();
            var codeInfo = sportsManager.QuerySingleScheme_AnteCode(schemeId);
            if (codeInfo == null)
                throw new Exception(string.Format("查不到方案{0}的投注号码信息", schemeId));

            var selectMatchIdArray = codeInfo.SelectMatchId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var allowCodeArray = codeInfo.AllowCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //分析投注号码
            //var codeText = File.ReadAllText(codeInfo.AnteCodeFullFileName, Encoding.UTF8);
            var codeText = Encoding.UTF8.GetString(codeInfo.FileBuffer);
            var matchIdList = new List<string>();
            var codeList = AnalyzerFactory.CheckSingleSchemeAnteCode(codeText, codeInfo.PlayType, codeInfo.ContainsMatchId, selectMatchIdArray, allowCodeArray, out matchIdList);

            var hitCount = 0;
            var rightCount = 0;
            var error1Count = 0;
            var error2Count = 0;
            var bonusCount = 0;

            var matchResultList = bjdcManager.QueryJCZQ_MatchResultListByMatchId(matchIdList.ToArray());
            var totalResultList = new List<C_JCZQ_MatchResult>();
            foreach (var item in codeList)
            {
                var playType = string.Empty;
                var anteCodeList = new List<C_Sports_AnteCode>();
                foreach (var t in item.Split('#'))
                {
                    var array = t.Split('|');
                    if (array.Length != 3) continue;
                    if (array[1] == "*") continue;
                    if (string.IsNullOrEmpty(playType))
                        playType = array[2];
                    anteCodeList.Add(new C_Sports_AnteCode
                    {
                        MatchId = array[0],
                        AnteCode = array[1],
                        PlayType = array[2],
                        IsDan = false,
                    });
                }

                #region 计算命中场数

                foreach (var code in anteCodeList)
                {
                    if (string.IsNullOrEmpty(playType))
                        playType = code.PlayType;
                    var query = from r in matchResultList
                                where r.MatchId == code.MatchId
                                select r;
                    var first = query.FirstOrDefault();
                    if (totalResultList.FirstOrDefault(p => p.MatchId == code.MatchId) == null)
                    {
                        totalResultList.Add(first == null ? new C_JCZQ_MatchResult
                        {
                            MatchId = code.MatchId,
                            SPF_Result = "-1",
                            ZJQ_Result = "-1",
                            BF_Result = "-1",
                            BQC_Result = "-1",
                        } : first);
                    }
                    switch (gameType)
                    {
                        case "SPF":
                            query = query.Where(p => code.AnteCode.Contains(p.SPF_Result));
                            break;
                        case "BRQSPF":
                            query = query.Where(p => code.AnteCode.Contains(p.BRQSPF_Result));
                            break;
                        case "BF":
                            query = query.Where(p => code.AnteCode.Contains(p.BF_Result));
                            break;
                        case "ZJQ":
                            query = query.Where(p => code.AnteCode.Contains(p.ZJQ_Result));
                            break;
                        case "BQC":
                            query = query.Where(p => code.AnteCode.Contains(p.BQC_Result));
                            break;
                    }

                    if (query.FirstOrDefault() != null)
                        hitCount++;
                }

                #endregion

                #region 计算中奖注数

                var playTypeArray = playType.Split('_');
                if (playTypeArray.Length != 2) continue;
                var baseCount = int.Parse(playTypeArray[0]);
                var analyzer = AnalyzerFactory.GetSportAnalyzer("JCZQ", gameType, baseCount);
                var bonusResult = analyzer.CaculateBonus(anteCodeList.ToArray(), totalResultList.ToArray());
                bonusCount += bonusResult.BonusCount;

                #endregion

            }
            UpdateOrder_RunningHitMatchCount(schemeId, hitCount, rightCount, error1Count, error2Count, bonusCount);
        }

        #endregion

        #region 竞彩篮球

        public void Update_JCLQ_MatchList(string[] matchIdArray)
        {
            var matchInfoList = LoadJCLQMatchList();
            UpdateJCLQMatch(matchIdArray, matchInfoList);

            //重新加载比赛到缓存
            RedisMatchBusiness.ReloadCurrentJCLQMatch();
        }

        private void UpdateJCLQMatch(string[] matchIdArray, List<JCLQ_MatchInfo> matchInfoList)
        {
            //开启事务
            var manager = new JCLQMatchManager();
            using (manager.DB)
            {
                
                
                var oldList = manager.QueryJCLQ_MatchListByMatchId(matchIdArray);
                var oldResultList = manager.QueryJCLQ_MatchResult_PrizeListByMatchId(matchIdArray);

                manager.DB.Begin();

                try
                {

            
                foreach (var item in matchIdArray)
                {
                    var old = oldList.FirstOrDefault(p => p.MatchId == item);
                    var oldResult = oldResultList.FirstOrDefault(p => p.MatchId == item);
                    var current = matchInfoList.FirstOrDefault(p => p.MatchId == item);
                    if (current == null)
                        continue;
                    if (old == null)
                    {
                        //重新添加
                        //var entity = new JCLQ_Match();
                        //ObjectConvert.ConverInfoToEntity<MatchBiz.Core.JCLQ_MatchInfo, JCLQ_Match>(current, ref entity);
                        manager.AddJCLQ_Match(new C_JCLQ_Match
                        {
                            StartDateTime = DateTime.Parse(current.StartDateTime),
                            DSStopBettingTime = DateTime.Parse(current.DSStopBettingTime),
                            FSStopBettingTime = DateTime.Parse(current.FSStopBettingTime),
                            AverageLose = current.AverageLose,
                            AverageWin = current.AverageWin,
                            GuestTeamName = current.GuestTeamName,
                            HomeTeamName = current.HomeTeamName,
                            MatchState = current.MatchState,
                            PrivilegesType = current.PrivilegesType,
                            CreateTime = DateTime.Parse(current.CreateTime),
                            GuestTeamId = current.GuestTeamId,
                            HomeTeamId = current.HomeTeamId,
                            LeagueColor = current.LeagueColor,
                            LeagueId = current.LeagueId,
                            LeagueName = current.LeagueName,
                            MatchData = current.MatchData,
                            MatchId = current.MatchId,
                            MatchIdName = current.MatchIdName,
                            MatchNumber = current.MatchNumber,
                            Mid = current.Mid,
                        });
                    }
                    else
                    {
                        old.StartDateTime = DateTime.Parse(current.StartDateTime);
                        old.DSStopBettingTime = DateTime.Parse(current.DSStopBettingTime);
                        old.FSStopBettingTime = DateTime.Parse(current.FSStopBettingTime);
                        old.AverageLose = current.AverageLose;
                        old.AverageWin = current.AverageWin;
                        old.GuestTeamName = current.GuestTeamName;
                        old.HomeTeamName = current.HomeTeamName;
                        old.MatchState = current.MatchState;
                        //old.PrivilegesType = current.PrivilegesType;
                        manager.UpdateJCLQ_Match(old);
                    }

                    if (oldResult == null)
                    {
                        manager.AddJCLQ_MatchResult_Prize(new C_JCLQ_MatchResult_Prize
                        {
                            MatchState = "0",
                            CreateTime = DateTime.Parse(current.CreateTime),
                            MatchData = current.MatchData,
                            MatchId = current.MatchId,
                            MatchNumber = current.MatchNumber,
                            GuestScore = 0,
                            HomeScore = 0,
                            SFC_Result = "-",
                            DXF_Result = "-",
                            RFSF_Result = "-",
                            SF_Result = "-",
                            DXF_Trend = "-",
                            RFSF_Trend = "-",
                            DXF_SP = 0M,
                            RFSF_SP = 0M,
                            SF_SP = 0M,
                            SFC_SP = 0M,
                        });
                    }
                }

                manager.DB.Commit();
                }
                catch (Exception)
                {

                    throw;
                }
                // biz.CommitTran();
            }
        }

        public void ManualUpdate_JCLQ_MatchList()
        {
            var matchInfoList = LoadJCLQMatchList();
            var matchIdArray = matchInfoList.Select(p => p.MatchId).ToArray();
            UpdateJCLQMatch(matchIdArray, matchInfoList);

            //重新加载比赛到缓存
            RedisMatchBusiness.ReloadCurrentJCLQMatch();
        }

        public void Update_JCLQ_MatchResultList(string[] matchIdArray)
        {
            var matchResultList = LoadJCLQMatchResultList();
            var manager = new JCLQMatchManager();
            //开启事务
            using (manager.DB)
            {
               // biz.BeginTran();

              
                var oldList = manager.QueryJCLQ_MatchResultListByMatchId(matchIdArray);
                var oldResultList = manager.QueryJCLQ_MatchResult_PrizeListByMatchId(matchIdArray);

                manager.DB.Begin();
                try
                {

               
                foreach (var item in matchIdArray)
                {
                    var old = oldList.FirstOrDefault(p => p.MatchId == item);
                    var oldResult = oldResultList.FirstOrDefault(p => p.MatchId == item);
                    var current = matchResultList.FirstOrDefault(p => p.MatchId == item);
                    if (current == null)
                        continue;

                    var state = "1";
                    if ((current.SF_SP > 1M || current.SF_SP == 0M) && (current.RFSF_SP > 1M || current.RFSF_SP == 0M) && (current.SFC_SP > 1M || current.SFC_SP == 0M) && (current.DXF_SP > 1M || current.DXF_SP == 0M) && current.MatchState == "2")
                        state = "2";

                    if (old == null)
                    {
                        //重新添加
                        //var entity = new JCLQ_MatchResult();
                        //ObjectConvert.ConverInfoToEntity<MatchBiz.Core.JCLQ_MatchResultInfo, JCLQ_MatchResult>(current, ref entity);
                        manager.AddJCLQ_MatchResult(new C_JCLQ_MatchResult
                        {

                            HomeScore = current.HomeScore,
                            GuestScore = current.GuestScore,
                            MatchState = state,
                            RFSF_Trend = current.RFSF_Trend,
                            DXF_Trend = current.DXF_Trend,
                            SF_Result = current.SF_Result,
                            SF_SP = current.SF_SP,
                            RFSF_Result = current.RFSF_Result,
                            RFSF_SP = current.RFSF_SP,
                            SFC_Result = current.SFC_Result,
                            SFC_SP = current.SFC_SP,
                            DXF_Result = current.DXF_Result,
                            DXF_SP = current.DXF_SP,
                            CreateTime = DateTime.Parse(current.CreateTime),
                            MatchData = current.MatchData,
                            MatchId = current.MatchId,
                            MatchNumber = current.MatchNumber,
                        });
                    }
                    else
                    {
                        if (old.MatchState != "2")
                        {
                            old.HomeScore = current.HomeScore;
                            old.GuestScore = current.GuestScore;
                            old.MatchState = state;
                            old.RFSF_Trend = current.RFSF_Trend;
                            old.DXF_Trend = current.DXF_Trend;
                            old.SF_Result = current.SF_Result;
                            old.SF_SP = current.SF_SP;
                            old.RFSF_Result = current.RFSF_Result;
                            old.RFSF_SP = current.RFSF_SP;
                            old.SFC_Result = current.SFC_Result;
                            old.SFC_SP = current.SFC_SP;
                            old.DXF_Result = current.DXF_Result;
                            old.DXF_SP = current.DXF_SP;
                            manager.UpdateJCLQ_MatchResult(old);
                        }
                    }

                    if (oldResult == null)
                    {
                        manager.AddJCLQ_MatchResult_Prize(new C_JCLQ_MatchResult_Prize
                        {

                            HomeScore = current.HomeScore,
                            GuestScore = current.GuestScore,
                            MatchState = state,
                            RFSF_Trend = current.RFSF_Trend,
                            DXF_Trend = current.DXF_Trend,
                            SF_Result = current.SF_Result,
                            SF_SP = current.SF_SP,
                            RFSF_Result = current.RFSF_Result,
                            RFSF_SP = current.RFSF_SP,
                            SFC_Result = current.SFC_Result,
                            SFC_SP = current.SFC_SP,
                            DXF_Result = current.DXF_Result,
                            DXF_SP = current.DXF_SP,
                            CreateTime = DateTime.Parse(current.CreateTime),
                            MatchData = current.MatchData,
                            MatchId = current.MatchId,
                            MatchNumber = current.MatchNumber,
                        });
                    }
                    else
                    {
                        if (oldResult.MatchState != "2")
                        {
                            oldResult.HomeScore = current.HomeScore;
                            oldResult.GuestScore = current.GuestScore;
                            oldResult.MatchState = state;
                            oldResult.RFSF_Trend = current.RFSF_Trend;
                            oldResult.DXF_Trend = current.DXF_Trend;
                            oldResult.SF_Result = current.SF_Result;
                            oldResult.SF_SP = current.SF_SP;
                            oldResult.RFSF_Result = current.RFSF_Result;
                            oldResult.RFSF_SP = current.RFSF_SP;
                            oldResult.SFC_Result = current.SFC_Result;
                            oldResult.SFC_SP = current.SFC_SP;
                            oldResult.DXF_Result = current.DXF_Result;
                            oldResult.DXF_SP = current.DXF_SP;
                            manager.UpdateJCLQ_MatchResult_Prize(oldResult);
                        }
                    }

                    }
                    manager.DB.Commit();
                }
                catch (Exception)
                {
                    manager.DB.Rollback();

                    throw;
                }

              
            }
        }

        /// <summary>
        /// todo:缓存
        /// </summary>
        public JCLQMatchResult_Collection QueryJCLQMatchResult(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var result = new JCLQMatchResult_Collection();
            var totalCount = 0;
            var manager = new JCLQMatchManager();
            result.MatchResultList = manager.QueryJCLQMatchResult(startTime, endTime, pageIndex, pageSize, out totalCount);
            result.TotalCount = totalCount;
            return result;
        }
        public JCLQMatchResult_Collection QueryJCLQMatchResult(DateTime time)
        {
            var result = new JCLQMatchResult_Collection();
            var manager = new JCLQMatchManager();
            result.MatchResultList = manager.QueryJCLQMatchResult(time);
            return result;
        }
        public List<C_JCLQ_Match> QueryCurrentJCLQMatchInfo()
        {
            var collection = new List<C_JCLQ_Match>();
            var manager = new JCLQMatchManager();
            collection.AddRange(manager.QueryCurrentJCLQMatchInfo());
            return collection;
        }

        public void UpdateJCLQMatchInfo(string matchId, string privilegesType)
        {
            var manager = new JCLQMatchManager();
            var entity = manager.GetJCLQMatch(matchId);
            if (entity == null)
                throw new Exception(string.Format("比赛{0}不存在", matchId));
            entity.PrivilegesType = privilegesType;
            manager.UpdateJCLQ_Match(entity);
        }

        public CoreBJDCMatchInfoCollection QueryCurrentBJDCMatchInfo()
        {
            using (var manager = new BJDCMatchManager())
            {
                var collection = new CoreBJDCMatchInfoCollection();
                collection.AddRange(manager.QueryCurrentBJDCMatchInfo());
                return collection;
            }
        }


        public void UpdateBJDCMatchInfo(string Id, string privilegesType)
        {
            var manager = new BJDCMatchManager();
            var entity = manager.GetBJDCMatchById(Id);
            if (entity == null)
                throw new Exception(string.Format("比赛{0}不存在", Id));
            entity.PrivilegesType = privilegesType;
            manager.UpdateBJDC_Match(entity);
        }

        /// <summary>
        /// 更新JCLQ命中场数
        /// </summary>
        public void Update_JCLQ_HitCount(string[] matchIdArray)
        {
            //暂停业务
            return;

            //1 普通投注
            //var sportsManager = new Sports_Manager();
            //var dealSchemeList = new List<string>();
            //foreach (var item in sportsManager.QueryJCLQSportsAnteCodeByIsuseNumber(matchIdArray))
            //{
            //    if (dealSchemeList.Contains(item.SchemeId)) continue;

            //    Update_JCLQ_General_HitCount(item.GameType, item.SchemeId, matchIdArray);

            //    dealSchemeList.Add(item.SchemeId);
            //}

            //sportsManager = new Sports_Manager();
            //dealSchemeList = new List<string>();
            //foreach (var item in sportsManager.QueryJCLQSingleCodeByIsuseNumber(matchIdArray))
            //{
            //    if (string.IsNullOrEmpty(item)) continue;
            //    var array = item.Split('|');//[SchemeId]+'|'+[GameType]
            //    if (array.Length != 2) continue;
            //    if (dealSchemeList.Contains(array[0])) continue;

            //    Update_JCLQ_Single_HitCount(array[1], array[0]);

            //    dealSchemeList.Add(array[0]);
            //}
        }
        /// <summary>
        /// 更新单个订单的过关统计
        /// </summary>
        public void Update_JCLQ_HitCountBySchemeId(string schemeId)
        {
            //暂停业务
            return;
            //var sportsManager = new Sports_Manager();
            //var order = sportsManager.QuerySports_Order_Running(schemeId);
            //if (order == null)
            //    throw new Exception(string.Format("没有找到{0}的Order_Running数据", schemeId));
            ////普通投注
            //if (order.SchemeBettingCategory == SchemeBettingCategory.GeneralBetting)
            //{
            //    var codeList = sportsManager.QuerySportsAnteCodeBySchemeId(schemeId);
            //    Update_JCLQ_General_HitCount(order.GameType, schemeId, (from c in codeList select c.MatchId).ToArray());
            //    return;
            //}
            ////单式上传 和 过滤投注
            //Update_JCLQ_Single_HitCount(order.GameType, schemeId);
        }

        public void Update_JCLQ_General_HitCount(string gameType, string schemeId, string[] matchIdArray)
        {
            #region 计算命中场数

            //计算命中场数
            gameType = gameType.ToUpper();
            var hitCount = 0;
            var playType = string.Empty;
            var sportsManager = new Sports_Manager();
            var codeList = sportsManager.QuerySportsAnteCodeBySchemeId(schemeId);
            var jclqManager = new JCLQMatchManager();
            var matchResultList = jclqManager.QueryJCLQ_MatchResultListByMatchId(matchIdArray);
            var totalResultList = new List<C_JCLQ_MatchResult>();
            foreach (var code in codeList)
            {
                if (string.IsNullOrEmpty(playType))
                    playType = code.PlayType;
                var query = from r in matchResultList
                            where r.MatchId == code.MatchId
                            select r;
                var first = query.FirstOrDefault();
                if (totalResultList.FirstOrDefault(p => p.MatchId == code.MatchId) == null)
                {
                    totalResultList.Add(first == null ? new C_JCLQ_MatchResult
                    {
                        MatchId = code.MatchId,
                        SF_Result = "-1",
                        RFSF_Result = "-1",
                        SFC_Result = "-1",
                        DXF_Result = "-1",
                    } : first);
                }
                switch (gameType)
                {
                    case "SF":
                        query = query.Where(p => code.AnteCode.Contains(p.SF_Result));
                        break;
                    case "RFSF":
                        query = query.Where(p => code.AnteCode.Contains(p.RFSF_Result));
                        break;
                    case "SFC":
                        query = query.Where(p => code.AnteCode.Contains(p.SFC_Result));
                        break;
                    case "DXF":
                        query = query.Where(p => code.AnteCode.Contains(p.DXF_Result));
                        break;
                    default:
                        continue;
                }

                if (query.FirstOrDefault() != null)
                    hitCount++;
            }

            #endregion

            #region 计算全中、错1、错2场注数

            //计算全中、错1、错2场注数
            var rightCount = 0;
            var error1Count = 0;
            var error2Count = 0;
            var ticketList = sportsManager.QueryTicketList(schemeId);
            foreach (var ticket in ticketList)
            {
                if (string.IsNullOrEmpty(ticket.BetContent)) continue;
                //DXF_140821301_0/SFC_140821302_04 或 140821301_03,14/140821302_04
                var betArray = ticket.BetContent.Split('/');
                var ticket_rightCount = 0;
                var ticket_errorCount = 0;
                foreach (var match in betArray)
                {
                    //140821302_04 或 DXF_140821301_0
                    var matchArray = match.Split('_');
                    if (matchArray.Length != 2 && matchArray.Length != 3) continue;

                    var c_gameType = ticket.GameType.ToUpper();
                    var c_matchId = string.Empty;
                    var c_betContent = string.Empty;
                    if (matchArray.Length == 2)
                    {
                        //140905008_3 
                        c_matchId = matchArray[0];
                        c_betContent = matchArray[1];
                    }
                    if (matchArray.Length == 3)
                    {
                        //DXF_140821301_0
                        c_gameType = matchArray[0].ToUpper();
                        c_matchId = matchArray[1];
                        c_betContent = matchArray[2];
                    }

                    var mr = matchResultList.Where(p => p.MatchId == c_matchId).FirstOrDefault();
                    if (mr == null) continue;

                    switch (c_gameType)
                    {
                        case "SF":
                            if (c_betContent.Split(',').Contains(mr.SFC_Result))
                                ticket_rightCount++;
                            else
                                ticket_errorCount++;
                            break;
                        case "RFSF":
                            if (c_betContent.Split(',').Contains(mr.RFSF_Result))
                                ticket_rightCount++;
                            else
                                ticket_errorCount++;
                            break;
                        case "SFC":
                            if (c_betContent.Split(',').Contains(mr.SFC_Result))
                                ticket_rightCount++;
                            else
                                ticket_errorCount++;
                            break;
                        case "DXF":
                            if (c_betContent.Split(',').Contains(mr.DXF_Result))
                                ticket_rightCount++;
                            else
                                ticket_errorCount++;
                            break;
                        default:
                            continue;
                    }
                }
                //单张票的正确场数 等于投注票数
                if (ticket_rightCount == betArray.Length)
                {
                    rightCount++;
                }
                else if (ticket_errorCount == 1)
                {
                    error1Count++;
                }
                else if (ticket_errorCount == 2)
                {
                    error2Count++;
                }
            }

            #endregion


            #region 计算中奖注数

            var bonusCount = 0;
            foreach (var item in playType.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var t = item.Split('_');
                if (t.Length != 2) return;
                var baseCount = int.Parse(t[0]);
                var analyzer = AnalyzerFactory.GetSportAnalyzer("JCLQ", gameType, baseCount);
                var bonusResult = analyzer.CaculateBonus(codeList.ToArray(), totalResultList.ToArray());
                bonusCount += bonusResult.BonusCount;
            }

            #endregion

            UpdateOrder_RunningHitMatchCount(schemeId, hitCount, rightCount, error1Count, error2Count, bonusCount);
        }

        public void Update_JCLQ_Single_HitCount(string gameType, string schemeId)
        {
            var sportsManager = new Sports_Manager();
            var bjdcManager = new JCLQMatchManager();
            var codeInfo = sportsManager.QuerySingleScheme_AnteCode(schemeId);
            if (codeInfo == null)
                throw new Exception(string.Format("查不到方案{0}的投注号码信息", schemeId));

            var selectMatchIdArray = codeInfo.SelectMatchId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var allowCodeArray = codeInfo.AllowCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //分析投注号码
            //var codeText = File.ReadAllText(codeInfo.AnteCodeFullFileName, Encoding.UTF8);
            var codeText = Encoding.UTF8.GetString(codeInfo.FileBuffer);
            var matchIdList = new List<string>();
            var codeList = AnalyzerFactory.CheckSingleSchemeAnteCode(codeText, codeInfo.PlayType, codeInfo.ContainsMatchId, selectMatchIdArray, allowCodeArray, out matchIdList);

            var hitCount = 0;
            var rightCount = 0;
            var error1Count = 0;
            var error2Count = 0;
            var bonusCount = 0;

            var matchResultList = bjdcManager.QueryJCLQ_MatchResultListByMatchId(matchIdList.ToArray());
            var totalResultList = new List<C_JCLQ_MatchResult>();
            foreach (var item in codeList)
            {
                var playType = string.Empty;
                var anteCodeList = new List<C_Sports_AnteCode>();
                foreach (var t in item.Split('#'))
                {
                    var array = t.Split('|');
                    if (array.Length != 3) continue;
                    if (array[1] == "*") continue;
                    if (string.IsNullOrEmpty(playType))
                        playType = array[2];
                    anteCodeList.Add(new C_Sports_AnteCode
                    {
                        MatchId = array[0],
                        AnteCode = array[1],
                        PlayType = array[2],
                        IsDan = false,
                    });
                }

                #region 计算命中场数

                foreach (var code in anteCodeList)
                {
                    if (string.IsNullOrEmpty(playType))
                        playType = code.PlayType;
                    var query = from r in matchResultList
                                where r.MatchId == code.MatchId
                                select r;
                    var first = query.FirstOrDefault();
                    if (totalResultList.FirstOrDefault(p => p.MatchId == code.MatchId) == null)
                    {
                        totalResultList.Add(first == null ? new C_JCLQ_MatchResult
                        {
                            MatchId = code.MatchId,
                            SF_Result = "-1",
                            RFSF_Result = "-1",
                            SFC_Result = "-1",
                            DXF_Result = "-1",
                        } : first);
                    }
                    switch (gameType)
                    {
                        case "SF":
                            query = query.Where(p => code.AnteCode.Contains(p.SF_Result));
                            break;
                        case "RFSF":
                            query = query.Where(p => code.AnteCode.Contains(p.RFSF_Result));
                            break;
                        case "SFC":
                            query = query.Where(p => code.AnteCode.Contains(p.SFC_Result));
                            break;
                        case "DXF":
                            query = query.Where(p => code.AnteCode.Contains(p.DXF_Result));
                            break;
                    }

                    if (query.FirstOrDefault() != null)
                        hitCount++;
                }

                #endregion

                #region 计算中奖注数

                var playTypeArray = playType.Split('_');
                if (playTypeArray.Length != 2) continue;
                var baseCount = int.Parse(playTypeArray[0]);
                var analyzer = AnalyzerFactory.GetSportAnalyzer("JCLQ", gameType, baseCount);
                var bonusResult = analyzer.CaculateBonus(anteCodeList.ToArray(), totalResultList.ToArray());
                bonusCount += bonusResult.BonusCount;

                #endregion

            }
            UpdateOrder_RunningHitMatchCount(schemeId, hitCount, rightCount, error1Count, error2Count, bonusCount);
        }

        #endregion

        #region 世界杯
        /// <summary>
        /// 冠军比赛
        /// </summary>
        public void Update_SJB_GJ_MatchList(string[] matchIdArray)
        {
            var matchInfoList = LoadSJB_GJMatchList();
            UpdateSJB_GJMatch(matchIdArray, matchInfoList);
        }
        private void UpdateSJB_GJMatch(string[] matchIdArray, List<SJB_MatchInfo> matchInfoList)
        {
            var manager = new SJBMatchManager();
            //开启事务
            using (manager.DB)
            {

                var oldList = manager.QuerySJB_MatchListByMatchId(matchIdArray);
                manager.DB.Begin();

                try
                {

                foreach (var item in matchIdArray)
                {
                    var old = oldList.FirstOrDefault(p => p.MatchId == int.Parse(item) && p.GameType == "GJ");
                    var current = matchInfoList.FirstOrDefault(p => p.MatchId == int.Parse(item) && p.GameType == "GJ");
                    if (current == null)
                        continue;
                    if (old == null)
                    {
                        //重新添加
                        manager.AddSJB_Match(new C_SJB_Match
                        {
                            BetState = current.BetState,
                            BonusMoney = current.BonusMoney,
                            MatchId = current.MatchId,
                            Probadbility = current.Probadbility,
                            SupportRate = current.SupportRate,
                            Team = current.Team,
                            GameType = current.GameType,
                            IssuseNumber = current.IssuseNumber
                        });
                        continue;
                    }
                    old.BetState = current.BetState;
                    old.BonusMoney = current.BonusMoney;
                    old.Probadbility = current.Probadbility;
                    old.SupportRate = current.SupportRate;
                    old.Team = current.Team;
                    manager.UpdateSJB_Match(old);

                    if (current.BetState == "胜出")
                    {
                        LotteryGameManager lotterymanager = new LotteryGameManager();
                        var issuse = lotterymanager.QueryGameIssuseByKey("SJB", current.GameType, current.IssuseNumber);
                        if (issuse.Status !=(int) IssuseStatus.Stopped)
                        {
                            issuse.WinNumber = current.MatchId.ToString();
                            issuse.AwardTime = DateTime.Now;
                            issuse.Status = (int)IssuseStatus.Stopped;
                            lotterymanager.UpdateGameIssuse(issuse);
                        }
                    }
                }

                manager.DB.Commit();

                }
                catch (Exception)
                {
                    manager.DB.Rollback();

                    throw;
                }

            }
        }
        /// <summary>
        /// 冠亚军比赛
        /// </summary>
        public void Update_SJB_GYJ_MatchList(string[] matchIdArray)
        {
            var matchInfoList = LoadSJB_GYJMatchList();
            UpdateSJB_GYJMatch(matchIdArray, matchInfoList);
        }
        private void UpdateSJB_GYJMatch(string[] matchIdArray, List<SJB_MatchInfo> matchInfoList)
        {
            var manager = new SJBMatchManager();
            //开启事务
            using (manager.DB)
            {
                manager.DB.Begin();

                try
                {

                
            
                var oldList = manager.QuerySJB_MatchListByMatchId(matchIdArray);
                foreach (var item in matchIdArray)
                {
                    var old = oldList.FirstOrDefault(p => p.MatchId == int.Parse(item) && p.GameType == "GYJ");
                    var current = matchInfoList.FirstOrDefault(p => p.MatchId == int.Parse(item) && p.GameType == "GYJ");
                    if (current == null)
                        continue;
                    if (old == null)
                    {
                        //重新添加
                        manager.AddSJB_Match(new C_SJB_Match
                        {
                            BetState = current.BetState,
                            BonusMoney = current.BonusMoney,
                            MatchId = current.MatchId,
                            Probadbility = current.Probadbility,
                            SupportRate = current.SupportRate,
                            Team = current.Team,
                            GameType = current.GameType,
                            IssuseNumber = current.IssuseNumber
                        });
                        continue;
                    }
                    old.BetState = current.BetState;
                    old.BonusMoney = current.BonusMoney;
                    old.Probadbility = current.Probadbility;
                    old.SupportRate = current.SupportRate;
                    old.Team = current.Team;
                    manager.UpdateSJB_Match(old);

                    if (current.BetState == "胜出")
                    {
                        LotteryGameManager lotterymanager = new LotteryGameManager();
                        var issuse = lotterymanager.QueryGameIssuseByKey("SJB", current.GameType, current.IssuseNumber);
                        if (issuse.Status != (int)IssuseStatus.Stopped)
                        {
                            issuse.WinNumber = current.MatchId.ToString();
                            issuse.AwardTime = DateTime.Now;
                            issuse.Status = (int)IssuseStatus.Stopped;
                            lotterymanager.UpdateGameIssuse(issuse);
                        }
                    }
                }

                manager.DB.Commit();
                }
                catch (Exception)
                {
                    manager.DB.Rollback();
                    throw;
                }
            }
        }
        #endregion

        public C_Game_Issuse QueryWinNumberByIssuseNumber(string gameCode, string gameType, string issuseNumber)
        {
            var query = from b in DB.CreateQuery<C_Game_Issuse>()
                        where b.GameCode == gameCode
                        && (gameType==null||gameType=="" || b.GameType == gameType)
                        && b.IssuseNumber == issuseNumber
                        select b;
                        //select new WinNumber_QueryInfo
                        //{
                        //    AwardTime = b.AwardTime,
                        //    GameCode = b.GameCode,
                        //    GameType = b.GameType,
                        //    IssuseNumber = b.IssuseNumber,
                        //    WinNumber = b.WinNumber,
                        //};
            return query.FirstOrDefault();
        }
    }
}
