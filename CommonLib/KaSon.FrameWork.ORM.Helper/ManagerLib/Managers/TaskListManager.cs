using EntityModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.UserHelper
{
   public class TaskListManager:DBbase
    {
        /// <summary>
        /// 添加一条未赠送记录
        /// </summary>
        public void AddTaskList(E_TaskList entity)
        {
            DB.GetDal<E_TaskList>().Add(entity);
        }

        /// <summary>
        /// 查询某个赠送记录
        /// </summary>
        public E_TaskList QueryTaskListByCategory(string userId, TaskCategory taskCategory)
        {
           
            return DB.CreateQuery<E_TaskList>().Where(p => p.UserId == userId && p.TaskCategory == (int)taskCategory).FirstOrDefault();
        }

        public void AddUserTaskRecord(E_UserTaskRecord entity)
        {
            DB.GetDal<E_UserTaskRecord>().Add(entity);
        }
            /// <summary>
            /// 查询
            /// </summary>
            public E_TaskList QueryTaskList(int id)
            {
              
                return DB.CreateQuery<E_TaskList>().Where(p => p.Id == id).FirstOrDefault();
            }

            public List<E_UserTaskRecord> QueryUserTaskRecordByCategory(string userId, TaskCategory taskCategory)
            {
               
                return DB.CreateQuery<E_UserTaskRecord>().Where(p => p.UserId == userId && p.TaskCategory == (int)taskCategory).ToList();
            }
            public List<E_UserTaskRecord> QueryUserTaskRecordByCategory(string userId, TaskCategory taskCategory, string currentTime)
            {
               
                return DB.CreateQuery<E_UserTaskRecord>().Where(p => p.UserId == userId && p.TaskCategory == (int)taskCategory && p.CurrentTime == currentTime).ToList();
            }
            public List<E_UserTaskRecord> QueryUserTaskRecordByCategory(string userId, TaskCategory taskCategory, string[] currentTime)
            {
               
                return DB.CreateQuery<E_UserTaskRecord>().Where(p => p.UserId == userId && p.TaskCategory == (int)taskCategory && currentTime.Contains(p.CurrentTime)).ToList();
            }

         
            public E_TaskList QueryTaskListByCategory(string userId, TaskCategory taskCategory, string currentTime)
            {
               
                return DB.CreateQuery<E_TaskList>().Where(p => p.UserId == userId && p.TaskCategory == (int)taskCategory && p.CurrentTime == currentTime).FirstOrDefault();
            }

            /// <summary>
            /// 查询某人，已领取记录
            /// </summary>
            public E_TaskList QueryIsGiveTask(string userId, TaskCategory taskCategory)
            {
               
                return DB.CreateQuery<E_TaskList>().Where(p => p.UserId == userId && p.TaskCategory == (int)taskCategory && p.IsGive == true).FirstOrDefault();
            }

            /// <summary>
            /// 查询某人，今日已领取记录
            /// </summary>
            public E_TaskList QueryToDayGiveTask(string userId, TaskCategory taskCategory, string currentTime)
            {
               
                return DB.CreateQuery<E_TaskList>().Where(p => p.UserId == userId && p.TaskCategory == (int)taskCategory && p.CurrentTime == currentTime && p.IsGive == true).FirstOrDefault();
            }

            /// <summary>
            /// 查询某个赠送记录条数
            /// </summary>
            public int QueryTaskCount(string userId, TaskCategory taskCategory)
            {
              
                //return this.Session.Query<TaskList>().Where(p => p.UserId == userId && p.TaskCategory == taskCategory).Count();
                return DB.CreateQuery<E_TaskList>().Count(p => p.UserId == userId && p.TaskCategory == (int)taskCategory);
            }

            /// <summary>
            /// 查询今日奖金优化投注条数
            /// </summary>
            public int QueryToDayTaskCount(string userId, TaskCategory taskCategory, string currentTime)
            {
                
                return DB.CreateQuery<E_TaskList>().Where(p => p.UserId == userId && p.TaskCategory == (int)taskCategory && p.CurrentTime == currentTime).Count();
            }

            /// <summary>
            /// 查询某个中奖金额
            /// </summary>
            public decimal QueryTaskMonery(string userId, TaskCategory taskCategory)
            {
               
                var query = from f in DB.CreateQuery<E_TaskList>()
                            where f.UserId == userId && f.TaskCategory == (int)taskCategory
                            select f;
                if (query.Count() == 0) return 0M;
                return query.Sum(p => p.ValueGrowth);
            }

            /// <summary>
            /// 查询未赠送成长值
            /// </summary>
            public List<E_TaskList> QueryTaskList(string userId, int vipLevel)
            {
                
                var query = from r in DB.CreateQuery<E_TaskList>()
                            where (r.UserId == userId && r.IsGive == false)
                            && r.TaskCategory != (int)TaskCategory.EverDayBuyLottery
                            && r.TaskCategory != (int)TaskCategory.JingcaiP2_1Totle5
                            && r.TaskCategory != (int)TaskCategory.BonusBuyLotteryTotle5
                            && r.TaskCategory != (int)TaskCategory.Win100Count
                            && r.TaskCategory != (int)TaskCategory.JCWin100Count
                            && r.TaskCategory != (int)TaskCategory.Win1000Yuan
                            && r.VipLevel <= vipLevel

                            orderby r.CreateTime descending
                            select r;
                return query.ToList();
            }

            /// <summary>
            /// 查询某成长值的赠送记录
            /// </summary>
            //public List<TaskListInfo> QueryCompleteTaskList(string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
            //{
            //    Session.Clear();
            //    var query = from r in this.Session.Query<TaskList>()
            //                where (r.UserId == userId && r.IsGive == true)
            //                && (r.CreateTime >= starTime && r.CreateTime < endTime)
            //                orderby r.CreateTime descending
            //                select new TaskListInfo
            //                {
            //                    UserId = r.UserId,
            //                    OrderId = r.OrderId,
            //                    TaskName = r.TaskName,
            //                    Content = r.Content,
            //                    TaskCategory = r.TaskCategory,
            //                    ValueGrowth = r.ValueGrowth,
            //                    CreateTime = r.CreateTime,
            //                };
            //    totalCount = query.Count();
            //    return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //}

            /// <summary>
            /// 查询某成长值的赠送记录
            /// </summary>
            //public List<TaskListInfo> QueryCompleteTaskToUserList(string userId)
            //{
            //    Session.Clear();
            //    var query = from r in this.Session.Query<TaskList>()
            //                where (r.UserId == userId && r.IsGive == true)
            //                && r.TaskCategory != TaskCategory.BonusBuyLotteryTotle5
            //                && r.TaskCategory != TaskCategory.Win100Count
            //                && r.TaskCategory != TaskCategory.JCWin100Count
            //                && r.TaskCategory != TaskCategory.Win1000Yuan
            //                && r.TaskCategory != TaskCategory.EverDayBuyLottery
            //                && r.TaskCategory != TaskCategory.JingcaiP2_1Totle5
            //                orderby r.CreateTime descending
            //                select new TaskListInfo
            //                {
            //                    UserId = r.UserId,
            //                    OrderId = r.OrderId,
            //                    TaskName = r.TaskName,
            //                    Content = r.Content,
            //                    TaskCategory = r.TaskCategory,
            //                    ValueGrowth = r.ValueGrowth,
            //                    CreateTime = r.CreateTime,
            //                };
            //    return query.ToList();
            //}

            /// <summary>
            /// 查询某人成长值的累计型完成进度
            /// </summary>
            //public List<TaskHotCumulativeInfo> QueryTaskListProgress(string userId)
            //{
            //    Session.Clear();
            //    var query = from r in this.Session.Query<TaskList>()
            //                where (r.UserId == userId)
            //                && (r.TaskCategory == TaskCategory.Win100Count || r.TaskCategory == TaskCategory.JCWin100Count || r.TaskCategory == TaskCategory.Win1000Yuan)
            //                group r by new { TaskCategory = r.TaskCategory, TaskName = r.TaskName, WinMonery = r.ValueGrowth } into _r
            //                select new TaskHotCumulativeInfo
            //                {
            //                    TaskCategory = _r.Key.TaskCategory,
            //                    Count = _r.Count(),
            //                    TaskName = _r.Key.TaskName,
            //                    WinMonery = _r.Key.WinMonery,
            //                };
            //    return query.ToList();
            //}

            /// <summary>
            /// 查询该任务是否今天完成
            /// </summary>
            public int QueryTaskUserToday(string userId, TaskCategory taskCategory)
            {
                
                var query = from f in DB.CreateQuery<E_TaskList>()
                            where f.UserId == userId && f.TaskCategory == (int)taskCategory
                            && f.CreateTime >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)
                            select f;
                return query.Count();
            }

            /// <summary>
            /// 最新会员得到成长值动态
            /// </summary>
            //public List<TaskHotTodayInfo> QueryTaskHotTodayInfoList(int lenth)
            //{
            //    Session.Clear();
            //    var query = from r in this.Session.Query<TaskList>()
            //                join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
            //                where r.IsGive == true
            //                orderby r.CreateTime descending
            //                select new TaskHotTodayInfo
            //                {
            //                    DisplayName = u.DisplayName,
            //                    UserId = r.UserId,
            //                    TaskName = r.TaskName,
            //                    ValueGrowth = r.ValueGrowth,
            //                };
            //    return query.Take(lenth).ToList();
            //}

            /// <summary>
            /// 更新状态
            /// </summary>
            public void UpdateTaskList(E_TaskList entity)
            {
            DB.GetDal<E_TaskList>().Update(entity);
        }

            public List<E_TaskList> QueryWaitComplateTask()
            {
              
                return DB.CreateQuery<E_TaskList>().Where(p => p.IsGive == false).OrderBy(p => p.CreateTime).ToList();
            }

        
    }
}
