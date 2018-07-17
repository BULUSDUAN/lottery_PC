using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.ORM.Helper;
using KaSon.FrameWork.ORM.Helper.UserHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.PlugIn.External
{
       /// <summary>
        /// 每日登录,登录接口
        /// </summary>
        public class LoginHistoryBusiness : IUser_AfterLogin
        {
            /// <summary> 
            /// 每日登录,登录接口
            /// </summary>
            public void User_AfterLogin(string userId, string loginFrom, string loginIp, DateTime loginTime)
            {
                var gv = new TaskListManager();
                var toDayLogin = gv.QueryUserTaskRecordByCategory(userId, TaskCategory.EveryDayLogin, DateTime.Now.ToString("yyyyMMdd"));
                if (toDayLogin.Count > 0)
                    return;

                var orderId = Guid.NewGuid().ToString("N");
                gv.AddUserTaskRecord(new E_UserTaskRecord
                {
                    TaskCategory = (int)TaskCategory.EveryDayLogin,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    CreateTime = DateTime.Now,
                    TaskName = "每日登录",
                    UserId = userId,
                    OrderId = orderId,
                });

                var historyList = new List<string>();
                for (int i = 0; i < 7; i++)
                {
                    historyList.Add(DateTime.Today.AddDays(-i).ToString("yyyyMMdd"));
                }
                var loginList = gv.QueryUserTaskRecordByCategory(userId, TaskCategory.EveryDayLogin, historyList.ToArray());
                var giveGrowth = 1;
                if (loginList.Count >= 7)
                {
                    giveGrowth = 5;
                }
                //if (loginList.Count >= 3 && loginList.Count <= 6)
                //{
                //    giveGrowth = 3;
                //}
                var count = GetUserTaskRecordSort(loginList);
                if (count >= 3 && count <= 6)
                {
                    giveGrowth = 3;
                }
                gv.AddTaskList(new E_TaskList
                {
                    TaskCategory = (int)TaskCategory.EveryDayLogin,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    CreateTime = DateTime.Now,
                    TaskName = "每日登录",
                    UserId = userId,
                    OrderId = orderId,
                    Content = string.Format("今天是您登陆第:{0}天可获得{1}点成长值", loginList.Count, giveGrowth),
                    ValueGrowth = giveGrowth,
                    IsGive = true,
                    VipLevel = 0,
                });

            //增加成长值 
            BusinessHelper businessHelper = new BusinessHelper();
            businessHelper.Payin_UserGrowth("每日登录", orderId, userId, giveGrowth, string.Format("今天是您登陆第:{0}天可获得{1}点成长值", loginList.Count, giveGrowth));
            }
            public int GetUserTaskRecordSort(List<E_UserTaskRecord> taskList)
            {
                var giveCount = 1;
                if (taskList != null && taskList.Count > 0)
                {
                    taskList = taskList.OrderByDescending(s => s.CreateTime).ToList();
                    for (int i = 0; i < taskList.Count; i++)
                    {
                        if (i + 1 < taskList.Count)
                        {
                            var timeSpan = (Convert.ToDateTime(taskList[i].CreateTime.ToString("yyy-MM-dd")) - Convert.ToDateTime(taskList[i + 1].CreateTime.ToString("yyy-MM-dd")));
                            if (timeSpan.TotalDays == 1)
                            {
                                giveCount++;
                            }
                            else break;
                        }
                    }
                }
                return giveCount;
            }

            public object ExecPlugin(string type, object inputParam)
            {
                try
                {
                    var paraList = inputParam as object[];
                    switch (type)
                    {
                        case "IUser_AfterLogin":
                            User_AfterLogin((string)paraList[0], (string)paraList[1], (string)paraList[2], (DateTime)paraList[3]);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
                    }
                    return null;
                }
                catch (LogicException)
                {
                }
                catch (Exception ex)
                {
                    //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                    //writer.Write("EXEC_Plugin_AddLoginHistoryBusiness_Error_", type, ex);
                }

                return null;
            }
        }
}
