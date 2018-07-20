using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.ORM.Helper;
using KaSon.FrameWork.ORM.Helper.UserHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.PlugIn.External
{

    /// <summary>
    /// 首次购买彩票 ,赠送成长值 ,出票完成接口
    /// </summary>
    public class FirstBuyLotteryBusiness : IComplateTicket
    {
        /// <summary> 
        /// 首次购买彩票,赠送成长值 ,出票完成接口
        /// </summary>
        public void ComplateTicket(string userId, string schemeId, decimal totalMoney, decimal totalErrorMoney)
        {
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null) return;
            if (order.IsVirtualOrder) return;

            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(userId, TaskCategory.FistBuyLottery);
            if (old == null)
            {
                //查询vip等级
                var balanceManager = new UserBalanceManager();
                var user = balanceManager.QueryUserRegister(userId);
                gv.AddUserTaskRecord(new E_UserTaskRecord
                {
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.FistBuyLottery,
                    TaskName = "首次购买彩票",
                    CreateTime = DateTime.Now,
                    UserId = userId,
                    OrderId = schemeId,
                });
                if (user.VipLevel >= 2)
                {
                    //增加成长值 
                    BusinessHelper.Payin_UserGrowth("首次购买彩票", schemeId, userId, 50, "首次成功购彩可获得50点成长值");
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = userId,
                        OrderId = schemeId,
                        Content = "首次成功购彩可获得50点成长值",
                        ValueGrowth = 50,
                        VipLevel = 2,
                        IsGive = true,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = (int)TaskCategory.FistBuyLottery,
                        TaskName = "首次购买彩票",
                        CreateTime = DateTime.Now,
                    });
                }
                else
                {
                    //储存成长值
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = userId,
                        OrderId = schemeId,
                        Content = "首次成功购彩可获得50点成长值",
                        ValueGrowth = 50,
                        IsGive = false,
                        VipLevel = 2,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = (int)TaskCategory.FistBuyLottery,
                        TaskName = "首次购买彩票",
                        CreateTime = DateTime.Now,
                    });
                }
            }

        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IComplateTicket":
                        ComplateTicket((string)paraList[0], (string)paraList[1], (decimal)paraList[2], (decimal)paraList[3]);
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
                //writer.Write("EXEC_Plugin_AddFirstBuyLotteryBusiness_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 完成充值接口，首次充值赠送300点成长值
    /// </summary>
    public class FillMoneyBusiness : ICompleteFillMoney_AfterTranCommit
    {
        /// <summary>
        /// 完成充值接口，首次充值赠送300点成长值
        /// </summary>
        public void CompleteFillMoney_AfterTranCommit(string orderId, FillMoneyStatus status, FillMoneyAgentType agentType, decimal fillMoney, string userId, int vipLevel)
        {
            if (status != FillMoneyStatus.Success) return;

            //查询vip等级
            var balanceManager = new UserBalanceManager();
            var gv = new TaskListManager();

            var fundManager = new FundManager();
            var count = fundManager.QueryFillMoneyCount(userId);
            if (count == 1)
            {
                gv.AddUserTaskRecord(new E_UserTaskRecord
                {
                    UserId = userId,
                    OrderId = orderId,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.FistTopUp,
                    TaskName = "首次充值",
                    CreateTime = DateTime.Now,
                });
                var user = balanceManager.QueryUserRegister(userId);
                if (user.VipLevel >= 1)
                {
                    //增加成长值 
                    BusinessHelper.Payin_UserGrowth("首次充值", orderId, userId, 300, "首次充值成功可获得300点成长值");
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = userId,
                        OrderId = orderId,
                        Content = "首次充值成功可获得300点成长值",
                        ValueGrowth = 300,
                        IsGive = true,
                        VipLevel = 1,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = (int)TaskCategory.FistTopUp,
                        TaskName = "首次充值",
                        CreateTime = DateTime.Now,
                    });
                }
                else
                {
                    //储存成长值
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = userId,
                        OrderId = orderId,
                        Content = "首次充值成功可获得300点成长值",
                        ValueGrowth = 300,
                        IsGive = false,
                        VipLevel = 1,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = (int)TaskCategory.FistTopUp,
                        TaskName = "首次充值",
                        CreateTime = DateTime.Now,
                    });
                }
            }
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "ICompleteFillMoney_AfterTranCommit":
                        CompleteFillMoney_AfterTranCommit((string)paraList[0], (FillMoneyStatus)paraList[1], (FillMoneyAgentType)paraList[2], (decimal)paraList[3], (string)paraList[4], (int)paraList[5]);
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
                //writer.Write("EXEC_AddPlugin_CompleteFillMoney_AfterTranCommit_Error_", type, ex);
            }

            return null;
        }
    }


    /// <summary>
    /// 设置资金密码接口,赠送成长值
    /// </summary>
    public class BalancePasswordBusiness : IBalancePassword
    {
        /// <summary>
        /// 设置资金密码接口
        /// </summary>
        public void AddBalancePassword(string userId, string oldPassword, bool isSetPwd, string newPassword)
        {
            //查询vip等级
            var balanceManager = new UserBalanceManager();
            var user = balanceManager.QueryUserRegister(userId);
            if (user == null)
                return;
            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(userId, TaskCategory.SetBalancePassword);
            if (old == null)
            {
                var order = Guid.NewGuid().ToString("N");
                //增加成长值 
                BusinessHelper.Payin_UserGrowth("设置资金密码", order, userId, 50, "完成首次资金密码设置可获得50点成长值");
                gv.AddUserTaskRecord(new E_UserTaskRecord
                {
                    UserId = userId,
                    OrderId = order,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.SetBalancePassword,
                    TaskName = "设置资金密码",
                    CreateTime = DateTime.Now,
                });
                gv.AddTaskList(new E_TaskList
                {
                    UserId = userId,
                    OrderId = order,
                    Content = "完成首次资金密码设置可获得50点成长值",
                    ValueGrowth = 50,
                    VipLevel = 0,
                    IsGive = true,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.SetBalancePassword,
                    TaskName = "设置资金密码",
                    CreateTime = DateTime.Now,
                });
            }

        }

        public object ExecPlugin(string type, object inputParam)
        {

            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IBalancePassword":
                        AddBalancePassword((string)paraList[0], (string)paraList[1], (bool)paraList[2], (string)paraList[3]);
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
                //writer.Write("EXEC_Plugin_AddBalancePassword_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 绑定银行卡接口，赠送成长值
    /// </summary>
    public class BankCardBusiness : IAddBankCard
    {
        /// <summary>
        /// 绑定银行卡接口
        /// </summary>
        public void AddBankCard(string userId, string bankCardNumber, string bankCode, string bankName, string bankSubName, string cityName, string provinceName, string realName)
        {
            //查询vip等级
            var balanceManager = new UserBalanceManager();
            var user = balanceManager.QueryUserRegister(userId);
            if (user == null)
                return;
            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(userId, TaskCategory.BankCar);
            if (old == null)
            {
                var order = Guid.NewGuid().ToString("N");
                //增加成长值 
                BusinessHelper.Payin_UserGrowth("绑定银行卡", order, userId, 100, "首次添加银行卡成功可获得100点成长值");
                gv.AddUserTaskRecord(new E_UserTaskRecord
                {
                    UserId = userId,
                    OrderId = order,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.BankCar,
                    TaskName = "绑定银行卡",
                    CreateTime = DateTime.Now,
                });
                gv.AddTaskList(new E_TaskList
                {
                    UserId = userId,
                    OrderId = order,
                    Content = "首次添加银行卡成功可获得100点成长值",
                    ValueGrowth = 100,
                    VipLevel = 0,
                    IsGive = true,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.BankCar,
                    TaskName = "绑定银行卡",
                    CreateTime = DateTime.Now,
                });
            }

            //清理UserBindInfo缓存
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheData", "UserBindInfo", DateTime.Today.ToString("yyyyMMdd"));
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                var filePath = System.IO.Path.Combine(path, string.Format("{0}.json", userId));
                if (!System.IO.File.Exists(filePath))
                    return;

                System.IO.File.Delete(filePath);
            }
            catch (Exception)
            {
            }
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IAddBankCard":
                        AddBankCard((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3], (string)paraList[4], (string)paraList[5], (string)paraList[6], (string)paraList[7]);
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
                //writer.Write("EXEC_Plugin_AddBankCard_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 首次购买竞彩二串一（竞彩篮球、竞彩足球）,出票完成接口
    /// </summary>
    public class FistJingcaiP2_1Business : IComplateTicket
    {
        /// <summary> 
        /// 首次购买竞彩二串一（竞彩篮球、竞彩足球）,出票完成接口
        /// </summary>
        public void ComplateTicket(string userId, string schemeId, decimal totalMoney, decimal totalErrorMoney)
        {
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null) return;
            if (order.IsVirtualOrder) return;

            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(userId, TaskCategory.FistJingcaiP2_1);
            if (old != null)
                return;

            if (old == null && SchemeType.GeneralBetting == (SchemeType)order.SchemeType && (order.GameCode == "JCLQ" || order.GameCode == "JCZQ") && order.GameType != "HH" && order.PlayType == "2_1")
            {
                //查询vip等级
                var balanceManager = new UserBalanceManager();
                var user = balanceManager.QueryUserRegister(userId);
                gv.AddUserTaskRecord(new E_UserTaskRecord
                {
                    UserId = userId,
                    OrderId = schemeId,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.FistJingcaiP2_1,
                    TaskName = "购买竞彩2串1",
                    CreateTime = DateTime.Now,
                });
                if (user.VipLevel >= 3)
                {
                    //增加成长值 
                    BusinessHelper.Payin_UserGrowth("购买竞彩2串1", schemeId, userId, 100, "首次购买竞足或竞篮两场比赛2串1玩法可获得100点成长值");
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = userId,
                        OrderId = schemeId,
                        Content = "首次购买竞足或竞篮两场比赛2串1玩法可获得100点成长值",
                        ValueGrowth = 100,
                        VipLevel = 3,
                        IsGive = true,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = (int)TaskCategory.FistJingcaiP2_1,
                        TaskName = "购买竞彩2串1",
                        CreateTime = DateTime.Now,
                    });
                }
                else
                {
                    //储存成长值
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = userId,
                        OrderId = schemeId,
                        Content = "首次购买竞足或竞篮两场比赛2串1玩法可获得100点成长值",
                        ValueGrowth = 100,
                        VipLevel = 3,
                        IsGive = false,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = (int)TaskCategory.FistJingcaiP2_1,
                        TaskName = "购买竞彩2串1",
                        CreateTime = DateTime.Now,
                    });
                }
            }

        }

        public object ExecPlugin(string type, object inputParam)
        {

            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IComplateTicket":
                        ComplateTicket((string)paraList[0], (string)paraList[1], (decimal)paraList[2], (decimal)paraList[3]);
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
                //writer.Write("EXEC_Plugin_AddFistJingcaiP2_1Business_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 首次购买竞彩混投2串1（竞彩篮球、竞彩足球）,出票完成接口
    /// </summary>
    public class FistHHP2_1Business : IComplateTicket
    {
        /// <summary> 
        /// 首次购买竞彩混投二串一（竞彩篮球、竞彩足球）,出票完成接口
        /// </summary>
        public void ComplateTicket(string userId, string schemeId, decimal totalMoney, decimal totalErrorMoney)
        {
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null) return;
            if (order.IsVirtualOrder) return;

            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(userId, TaskCategory.FistHHP2_1);
            if (old != null)
                return;

            if (old == null && SchemeType.GeneralBetting == (SchemeType)order.SchemeType && (order.GameCode == "JCLQ" || order.GameCode == "JCZQ") && order.GameType == "HH" && order.PlayType == "2_1")
            {
                //查询vip等级
                var balanceManager = new UserBalanceManager();
                var user = balanceManager.QueryUserRegister(userId);
                gv.AddUserTaskRecord(new E_UserTaskRecord
                {
                    UserId = userId,
                    OrderId = schemeId,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.FistHHP2_1,
                    TaskName = "购买竞彩混投2串1",
                    CreateTime = DateTime.Now,
                });
                if (user.VipLevel >= 3)
                {
                    //增加成长值 
                    BusinessHelper.Payin_UserGrowth("购买混投2串1", schemeId, userId, 100, "首次购买竞足或竞篮两场比赛混投2串1玩法可获得100点成长值");
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = userId,
                        OrderId = schemeId,
                        Content = "首次购买竞足或竞篮两场比赛混投2串1玩法可获得100点成长值",
                        VipLevel = 3,
                        ValueGrowth = 100,
                        IsGive = true,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = (int)TaskCategory.FistHHP2_1,
                        TaskName = "购买竞彩混投2串1",
                        CreateTime = DateTime.Now,
                    });
                }
                else
                {
                    //储存成长值
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = userId,
                        OrderId = schemeId,
                        Content = "首次购买竞足或竞篮两场比赛混投2串1玩法可获得100点成长值",
                        ValueGrowth = 100,
                        IsGive = false,
                        VipLevel = 3,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = (int)TaskCategory.FistHHP2_1,
                        TaskName = "购买竞彩混投2串1",
                        CreateTime = DateTime.Now,
                    });
                }
            }

        }

        public object ExecPlugin(string type, object inputParam)
        {

            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IComplateTicket":
                        ComplateTicket((string)paraList[0], (string)paraList[1], (decimal)paraList[2], (decimal)paraList[3]);
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
                //writer.Write("EXEC_Plugin_AddFistHHP2_1Business_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 竞彩首次中奖,派奖接口
    /// </summary>
    public class JingcaiFistWinBusiness : IOrderPrize_AfterTranCommit
    {
        /// <summary> 
        /// 竞彩首次中奖,派奖接口
        /// </summary>
        public void OrderPrize_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal orderMoney, bool isBonus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney, bool isVirtualOrder, DateTime prizeTime)
        {
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Complate(schemeId);
            if (order == null) return;
            if (order.IsVirtualOrder) return;

            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(userId, TaskCategory.JingcaiFistWin);
            if (old != null)
                return;

            #region 竞彩首次中奖

            if (old == null && SchemeType.GeneralBetting == (SchemeType)order.SchemeType && (order.GameCode == "JCLQ" || order.GameCode == "JCZQ"))
            {
                //查询vip等级
                var balanceManager = new UserBalanceManager();
                var user = balanceManager.QueryUserRegister(userId);
                gv.AddUserTaskRecord(new E_UserTaskRecord
                {
                    UserId = userId,
                    OrderId = schemeId,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.JingcaiFistWin,
                    TaskName = "竞彩首次中奖",
                    CreateTime = DateTime.Now,
                });
                if (user.VipLevel >= 3)
                {
                    //增加成长值 
                    BusinessHelper.Payin_UserGrowth("竞彩首次中奖", schemeId, userId, 100, "认购的竞彩方案首次中奖可获得100点成长值");
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = userId,
                        OrderId = schemeId,
                        Content = "认购的竞彩方案首次中奖可获得100点成长值",
                        ValueGrowth = 100,
                        VipLevel = 3,
                        IsGive = true,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = (int)TaskCategory.JingcaiFistWin,
                        TaskName = "竞彩首次中奖",
                        CreateTime = DateTime.Now,
                    });
                }
                else
                {
                    //储存成长值
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = userId,
                        OrderId = schemeId,
                        Content = "认购的竞彩方案首次中奖可获得100点成长值",
                        VipLevel = 3,
                        ValueGrowth = 100,
                        IsGive = false,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = (int)TaskCategory.JingcaiFistWin,
                        TaskName = "竞彩首次中奖",
                        CreateTime = DateTime.Now,
                    });
                }
            }
            #endregion
        }
        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IOrderPrize_AfterTranCommit":
                        OrderPrize_AfterTranCommit((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3], (string)paraList[4], (decimal)paraList[5], (bool)paraList[6], (decimal)paraList[7], (decimal)paraList[8], (bool)paraList[9], (DateTime)paraList[10]);
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
                //writer.Write("EXEC_Plugin_AddFistHHP2_1Business_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 首次追号投注,出票完成接口
    /// </summary>
    public class FistZhuihaoBuyBusiness : IComplateTicket
    {
        /// <summary> 
        ///  首次追号投注,出票完成接口
        /// </summary>
        public void ComplateTicket(string userId, string schemeId, decimal totalMoney, decimal totalErrorMoney)
        {
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null) return;
            if (order.IsVirtualOrder) return;
            if (SchemeType.ChaseBetting != (SchemeType)order.SchemeType)
                return;

            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(userId, TaskCategory.FistZhuihaoBuy);
            if (old != null)
                return;

            //查询vip等级
            var balanceManager = new UserBalanceManager();
            var user = balanceManager.QueryUserRegister(userId);
            if (user == null)
                return;
            gv.AddUserTaskRecord(new E_UserTaskRecord
            {
                UserId = userId,
                OrderId = schemeId,
                CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                TaskCategory = (int)TaskCategory.FistZhuihaoBuy,
                TaskName = "首次追号投注",
                CreateTime = DateTime.Now,
            });
            if (user.VipLevel >= 3)
            {
                //增加成长值 
                BusinessHelper.Payin_UserGrowth("首次追号投注", schemeId, userId, 100, "首次完成追号投注可获得100点成长值");
                gv.AddTaskList(new E_TaskList
                {
                    UserId = userId,
                    OrderId = schemeId,
                    Content = "首次完成追号投注可获得100点成长值",
                    VipLevel = 3,
                    ValueGrowth = 100,
                    IsGive = true,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.FistZhuihaoBuy,
                    TaskName = "首次追号投注",
                    CreateTime = DateTime.Now,
                });
            }
            else
            {
                //储存成长值
                gv.AddTaskList(new E_TaskList
                {
                    UserId = userId,
                    OrderId = schemeId,
                    Content = "首次完成追号投注可获得100点成长值",
                    ValueGrowth = 100,
                    VipLevel = 3,
                    IsGive = false,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.FistZhuihaoBuy,
                    TaskName = "首次追号投注",
                    CreateTime = DateTime.Now,
                });
            }
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IComplateTicket":
                        ComplateTicket((string)paraList[0], (string)paraList[1], (decimal)paraList[2], (decimal)paraList[3]);
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
                //writer.Write("EXEC_Plugin_AddFistZhuihaoBuyBusiness_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 首次参与合买,出票完成接口
    /// </summary>
    public class FistHeMaiBusiness : IComplateTicket
    {
        /// <summary> 
        ///  首次参与合买,出票完成接口
        /// </summary>
        public void ComplateTicket(string userId, string schemeId, decimal totalMoney, decimal totalErrorMoney)
        {
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null) return;
            if (order.IsVirtualOrder) return;
            if (order.SchemeType != (int)SchemeType.TogetherBetting) return;

            var manager = new Sports_Manager();
            var joinList = manager.QuerySports_TogetherSucessJoin(schemeId);
            if (joinList.Count == 0)
                return;

            var gv = new TaskListManager();

            foreach (var item in joinList)
            {
                var old = gv.QueryTaskListByCategory(item.JoinUserId, TaskCategory.FistHeMai);
                if (old != null)
                    continue;

                //查询vip等级
                var balanceManager = new UserBalanceManager();
                var user = balanceManager.QueryUserRegister(item.JoinUserId);
                if (user == null)
                    continue;

                gv.AddUserTaskRecord(new E_UserTaskRecord
                {
                    UserId = item.JoinUserId,
                    OrderId = schemeId,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.FistHeMai,
                    TaskName = "首次参与合买",
                    CreateTime = DateTime.Now,
                });
                if (user.VipLevel >= 3)
                {
                    //增加成长值 
                    BusinessHelper.Payin_UserGrowth("首次参与合买", schemeId, item.JoinUserId, 50, "首次参与合买可获得50点成长值");
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = item.JoinUserId,
                        OrderId = schemeId,
                        Content = "首次参与合买可获得50点成长值",
                        ValueGrowth = 50,
                        VipLevel = 3,
                        IsGive = true,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = (int)TaskCategory.FistHeMai,
                        TaskName = "首次参与合买",
                        CreateTime = DateTime.Now,
                    });
                }
                else
                {
                    //储存成长值
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = item.JoinUserId,
                        OrderId = schemeId,
                        Content = "首次参与合买可获得50点成长值",
                        ValueGrowth = 50,
                        VipLevel = 3,
                        IsGive = false,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = (int)TaskCategory.FistHeMai,
                        TaskName = "首次参与合买",
                        CreateTime = DateTime.Now,
                    });
                }
            }

            #region old code
            //foreach (var item in joinList)
            //{
            //    var old = gv.QueryTaskListByCategory(item.JoinUserId, TaskCategory.FistHeMai);
            //    if (old != null)
            //        continue;

            //    //查询vip等级
            //    var balanceManager = new UserBalanceManager();
            //    var user = balanceManager.QueryUserRegister(userId);
            //    if (user == null)
            //        continue;

            //    gv.AddUserTaskRecord(new UserTaskRecord
            //    {
            //        UserId = userId,
            //        OrderId = schemeId,
            //        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
            //        TaskCategory = TaskCategory.FistHeMai,
            //        TaskName = "首次参与合买",
            //        CreateTime = DateTime.Now,
            //    });
            //    if (user.VipLevel >= 3)
            //    {
            //        //增加成长值 
            //        BusinessHelper.Payin_UserGrowth("首次参与合买", schemeId, userId, 50, "首次参与合买可获得50点成长值");
            //        gv.AddTaskList(new TaskList
            //        {
            //            UserId = userId,
            //            OrderId = schemeId,
            //            Content = "首次参与合买可获得50点成长值",
            //            ValueGrowth = 50,
            //            VipLevel = 3,
            //            IsGive = true,
            //            CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
            //            TaskCategory = TaskCategory.FistHeMai,
            //            TaskName = "首次参与合买",
            //            CreateTime = DateTime.Now,
            //        });
            //    }
            //    else
            //    {
            //        //储存成长值
            //        gv.AddTaskList(new TaskList
            //        {
            //            UserId = userId,
            //            OrderId = schemeId,
            //            Content = "首次参与合买可获得50点成长值",
            //            ValueGrowth = 50,
            //            VipLevel = 3,
            //            IsGive = false,
            //            CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
            //            TaskCategory = TaskCategory.FistHeMai,
            //            TaskName = "首次参与合买",
            //            CreateTime = DateTime.Now,
            //        });
            //    }
            //} 
            #endregion
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IComplateTicket":
                        ComplateTicket((string)paraList[0], (string)paraList[1], (decimal)paraList[2], (decimal)paraList[3]);
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
                //writer.Write("EXEC_Plugin_AddFistHeMaiBusiness_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 首次关注彩友,关注好友接口
    /// </summary>
    public class FistFocusOnFriendBusiness : IAttention_AfterTranCommit
    {
        /// <summary> 
        ///  首次关注彩友,关注好友接口
        /// </summary>
        public void Attention_AfterTranCommit(string activeUserId, string passiveUserId)
        {
            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(activeUserId, TaskCategory.FistFocusOnFriend);
            var order = Guid.NewGuid().ToString("N");
            if (old == null)
            {
                //查询vip等级
                var balanceManager = new UserBalanceManager();
                var user = balanceManager.QueryUserRegister(activeUserId);
                gv.AddUserTaskRecord(new E_UserTaskRecord
                {
                    UserId = activeUserId,
                    OrderId = Guid.NewGuid().ToString("N"),
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.FistFocusOnFriend,
                    TaskName = "关注彩友",
                    CreateTime = DateTime.Now,
                });
                if (user.VipLevel >= 3)
                {
                    //增加成长值 
                    BusinessHelper.Payin_UserGrowth("关注彩友", order, activeUserId, 50, "首次成功关注任一彩友可获得50点成长值");
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = activeUserId,
                        OrderId = Guid.NewGuid().ToString("N"),
                        Content = "首次成功关注任一彩友可获得50点成长值",
                        ValueGrowth = 50,
                        VipLevel = 3,
                        IsGive = true,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = (int)TaskCategory.FistFocusOnFriend,
                        TaskName = "关注彩友",
                        CreateTime = DateTime.Now,
                    });
                }
                else
                {
                    //储存成长值
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = activeUserId,
                        OrderId = order,
                        Content = "首次成功关注任一彩友可获得50点成长值",
                        ValueGrowth = 50,
                        VipLevel = 3,
                        IsGive = false,
                        CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                        TaskCategory = (int)TaskCategory.FistFocusOnFriend,
                        TaskName = "关注彩友",
                        CreateTime = DateTime.Now,
                    });
                }
            }

        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IAttention_AfterTranCommit":
                        Attention_AfterTranCommit((string)paraList[0], (string)paraList[1]);
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
                //writer.Write("EXEC_Plugin_AddFistFocusOnFriendBusiness_Error_", type, ex);
            }

            return null;
        }
    }
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
            BusinessHelper.Payin_UserGrowth("每日登录", orderId, userId, giveGrowth, string.Format("今天是您登陆第:{0}天可获得{1}点成长值", loginList.Count, giveGrowth));
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

    /// <summary>
    /// 首次累计消费10元,出票完成接口
    /// </summary>
    public class CumulativeTenYuanBusiness : IComplateTicket
    {
        /// <summary> 
        ///  首次累计消费10元,出票完成接口
        /// </summary>
        public void ComplateTicket(string userId, string schemeId, decimal totalMoney, decimal totalErrorMoney)
        {
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null) return;
            if (order.IsVirtualOrder) return;

            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(userId, TaskCategory.FistConsumptionTenYuan);
            if (old != null)
                return;

            var manager = new FundManager();
            var betMoney = manager.QueryBetMoney(userId);
            if (betMoney < 10M)
                return;

            //查询vip等级
            var balanceManager = new UserBalanceManager();
            var user = balanceManager.QueryUserRegister(userId);
            if (user == null)
                return;

            gv.AddUserTaskRecord(new E_UserTaskRecord
            {
                UserId = userId,
                OrderId = schemeId,
                CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                TaskCategory = (int)TaskCategory.FistConsumptionTenYuan,
                TaskName = "累计购彩满10元",
                CreateTime = DateTime.Now,
            });
            if (user.VipLevel >= 2)
            {
                //增加成长值 
                BusinessHelper.Payin_UserGrowth("累计购彩满10元", schemeId, userId, 50, "成功购彩满10元可获得50点成长值");
                gv.AddTaskList(new E_TaskList
                {
                    UserId = userId,
                    OrderId = schemeId,
                    Content = "成功购彩满10元可获得50点成长值",
                    ValueGrowth = 50,
                    IsGive = true,
                    VipLevel = 2,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.FistConsumptionTenYuan,
                    TaskName = "累计购彩满10元",
                    CreateTime = DateTime.Now,
                });
            }
            else
            {
                //储存成长值
                gv.AddTaskList(new E_TaskList
                {
                    UserId = userId,
                    OrderId = schemeId,
                    Content = "成功购彩满10元可获得50点成长值",
                    ValueGrowth = 50,
                    IsGive = false,
                    VipLevel = 2,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.FistConsumptionTenYuan,
                    TaskName = "累计购彩满10元",
                    CreateTime = DateTime.Now,
                });
            }

        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IComplateTicket":
                        ComplateTicket((string)paraList[0], (string)paraList[1], (decimal)paraList[2], (decimal)paraList[3]);
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
                //writer.Write("EXEC_Plugin_AddCumulativeTenYuanBusiness_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 每日购彩≥50元,出票完成接口
    /// </summary>
    public class EverDayBuyLotteryBusiness : IComplateTicket
    {
        /// <summary> 
        ///  每日购彩≥50元,出票完成接口
        /// </summary>
        public void ComplateTicket(string userId, string schemeId, decimal totalMoney, decimal totalErrorMoney)
        {
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null) return;
            if (order.IsVirtualOrder) return;

            var today = DateTime.Now.ToString("yyyyMMdd");
            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(userId, TaskCategory.EverDayBuyLottery, today);
            if (old != null)
                return;

            gv.AddUserTaskRecord(new E_UserTaskRecord
            {
                OrderMoney = totalMoney - totalErrorMoney,
                UserId = userId,
                OrderId = schemeId,
                CurrentTime = today,
                TaskCategory = (int)TaskCategory.EverDayBuyLottery,
                TaskName = "每日购彩≥50元",
                CreateTime = DateTime.Now,
            });

            var todayRecordList = gv.QueryUserTaskRecordByCategory(userId, TaskCategory.EverDayBuyLottery, today);
            var todayTotalMoney = todayRecordList.Count == 0 ? 0M : todayRecordList.Sum(p => p.OrderMoney);
            if (todayTotalMoney >= 50M)
            {
                //查询vip等级
                var balanceManager = new UserBalanceManager();
                var user = balanceManager.QueryUserRegister(userId);
                if (user.VipLevel >= 1)
                {
                    //增加成长值 
                    BusinessHelper.Payin_UserGrowth("每日购彩≥50元", schemeId, userId, 50, "每日购彩≥50元可获得50点成长值");
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = userId,
                        OrderId = schemeId,
                        Content = "每日购彩≥50元可获得50点成长值",
                        ValueGrowth = 50,
                        IsGive = true,
                        VipLevel = 1,
                        CurrentTime = today,
                        TaskCategory = (int)TaskCategory.EverDayBuyLottery,
                        TaskName = "每日购彩≥50元",
                        CreateTime = DateTime.Now,
                    });
                }
                else
                {
                    //储存成长值
                    gv.AddTaskList(new E_TaskList
                    {
                        UserId = userId,
                        OrderId = schemeId,
                        Content = "每日购彩≥50元可获得50点成长值",
                        ValueGrowth = 50,
                        IsGive = false,
                        VipLevel = 1,
                        CurrentTime = today,
                        TaskCategory = (int)TaskCategory.EverDayBuyLottery,
                        TaskName = "每日购彩≥50元",
                        CreateTime = DateTime.Now,
                    });
                }
            }
        }
        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IComplateTicket":
                        ComplateTicket((string)paraList[0], (string)paraList[1], (decimal)paraList[2], (decimal)paraList[3]);
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
                //writer.Write("EXEC_Plugin_AddEverDayBuyLotteryBusiness_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 购买竞彩2串1满5次（竞彩篮球、竞彩足球）,出票完成接口
    /// </summary>
    public class JingcaiP2_1Totle5Business : IComplateTicket
    {
        /// <summary> 
        /// 购买竞彩2串1满5次（竞彩篮球、竞彩足球）,出票完成接口
        /// </summary>
        public void ComplateTicket(string userId, string schemeId, decimal totalMoney, decimal totalErrorMoney)
        {
            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Running(schemeId);
            if (order == null) return;
            if (order.IsVirtualOrder) return;
            if (!new string[] { "JCZQ", "JCLQ" }.Contains(order.GameCode)) return;
            if (order.PlayType != "2_1") return;

            var today = DateTime.Now.ToString("yyyyMMdd");
            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(userId, TaskCategory.JingcaiP2_1Totle5, today);
            if (old != null)
                return;

            gv.AddUserTaskRecord(new E_UserTaskRecord
            {
                OrderMoney = totalMoney - totalErrorMoney,
                UserId = userId,
                OrderId = schemeId,
                CurrentTime = today,
                TaskCategory = (int)TaskCategory.JingcaiP2_1Totle5,
                TaskName = "购买竞彩2串1满5次",
                CreateTime = DateTime.Now,
            });

            var recordList = gv.QueryUserTaskRecordByCategory(userId, TaskCategory.JingcaiP2_1Totle5, today);
            if (recordList.Count < 5)
                return;

            var orderId = Guid.NewGuid().ToString("N");
            //查询vip等级
            var balanceManager = new UserBalanceManager();
            var user = balanceManager.QueryUserRegister(userId);
            if (user.VipLevel >= 3)
            {
                //增加成长值 
                BusinessHelper.Payin_UserGrowth("购买竞彩2串1满5次", orderId, userId, 100, "成功购买竞彩2串1满5次可获得100点成长值");
                gv.AddTaskList(new E_TaskList
                {
                    UserId = userId,
                    OrderId = orderId,
                    Content = "成功购买竞彩2串1满5次可获得100点成长值",
                    ValueGrowth = 100,
                    IsGive = true,
                    VipLevel = 3,
                    CurrentTime = today,
                    TaskCategory = (int)TaskCategory.JingcaiP2_1Totle5,
                    TaskName = "购买竞彩2串1满5次",
                    CreateTime = DateTime.Now,
                });
            }
            else
            {
                //储存成长值
                gv.AddTaskList(new E_TaskList
                {
                    UserId = userId,
                    OrderId = orderId,
                    Content = "成功购买竞彩2串1满5次可获得100点成长值",
                    ValueGrowth = 100,
                    IsGive = false,
                    VipLevel = 3,
                    CurrentTime = today,
                    TaskCategory = (int)TaskCategory.JingcaiP2_1Totle5,
                    TaskName = "购买竞彩2串1满5次",
                    CreateTime = DateTime.Now,
                });
            }
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IComplateTicket":
                        ComplateTicket((string)paraList[0], (string)paraList[1], (decimal)paraList[2], (decimal)paraList[3]);
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
                //writer.Write("EXEC_Plugin_AddJingcaiP2_1Totle5Business_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 中奖100次,派奖接口
    /// </summary>
    public class Win100CountBusiness : IOrderPrize_AfterTranCommit
    {
        /// <summary> 
        /// 中奖100次,派奖接口
        /// </summary>
        public void OrderPrize_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal orderMoney,
            bool isBonus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney, bool isVirtualOrder, DateTime prizeTime)
        {
            if (afterTaxBonusMoney <= 0M) return;
            if (isVirtualOrder) return;

            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(userId, TaskCategory.Win100Count);
            if (old != null) return;

            gv.AddUserTaskRecord(new E_UserTaskRecord
            {
                OrderMoney = afterTaxBonusMoney,
                UserId = userId,
                OrderId = schemeId,
                CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                TaskCategory = (int)TaskCategory.Win100Count,
                TaskName = "中奖100次",
                CreateTime = DateTime.Now,
            });

            var recordList = gv.QueryUserTaskRecordByCategory(userId, TaskCategory.Win100Count);
            if (recordList.Count < 100)
                return;

            //增加成长值 
            BusinessHelper.Payin_UserGrowth("中奖100次", schemeId, userId, 100, "中奖次数达到100次可获得100点成长值");
            gv.AddTaskList(new E_TaskList
            {
                UserId = userId,
                OrderId = schemeId,
                Content = "中奖次数达到100次可获得100点成长值",
                ValueGrowth = 100,
                IsGive = true,
                VipLevel = 0,
                CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                TaskCategory = (int)TaskCategory.Win100Count,
                TaskName = "中奖100次",
                CreateTime = DateTime.Now,
            });
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IOrderPrize_AfterTranCommit":
                        OrderPrize_AfterTranCommit((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3], (string)paraList[4], (decimal)paraList[5], (bool)paraList[6], (decimal)paraList[7], (decimal)paraList[8], (bool)paraList[9], (DateTime)paraList[10]);
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
                //writer.Write("EXEC_Plugin_AddWin100CountBusiness_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 竞彩 2_1 中奖100次,派奖接口
    /// </summary>
    public class JCWin100CountBusiness : IOrderPrize_AfterTranCommit
    {
        /// <summary> 
        /// 中奖100次,派奖接口
        /// </summary>
        public void OrderPrize_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal orderMoney,
            bool isBonus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney, bool isVirtualOrder, DateTime prizeTime)
        {
            if (afterTaxBonusMoney <= 0M) return;
            if (isVirtualOrder) return;
            if (!new string[] { "JCZQ", "JCLQ" }.Contains(gameCode))
                return;
            var manager = new Sports_Manager();
            var order = manager.QuerySports_Order_Complate(schemeId);
            if (order == null)
                return;
            if (order.PlayType != "2_1")
                return;

            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(userId, TaskCategory.JCWin100Count);
            if (old != null)
                return;

            gv.AddUserTaskRecord(new E_UserTaskRecord
            {
                OrderMoney = afterTaxBonusMoney,
                UserId = userId,
                OrderId = schemeId,
                CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                TaskCategory = (int)TaskCategory.JCWin100Count,
                TaskName = "竞彩中奖100次",
                CreateTime = DateTime.Now,
            });

            var recordList = gv.QueryUserTaskRecordByCategory(userId, TaskCategory.JCWin100Count);
            if (recordList.Count < 100)
                return;

            //增加成长值 
            BusinessHelper.Payin_UserGrowth("竞彩中奖100次", schemeId, userId, 100, "竞彩中奖次数达到100次可获得100点成长值");
            gv.AddTaskList(new E_TaskList
            {
                UserId = userId,
                OrderId = schemeId,
                Content = "竞彩中奖次数达到100次可获得100点成长值",
                ValueGrowth = 100,
                IsGive = true,
                VipLevel = 0,
                CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                TaskCategory = (int)TaskCategory.JCWin100Count,
                TaskName = "竞彩中奖100次",
                CreateTime = DateTime.Now,
            });
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IOrderPrize_AfterTranCommit":
                        OrderPrize_AfterTranCommit((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3], (string)paraList[4], (decimal)paraList[5], (bool)paraList[6], (decimal)paraList[7], (decimal)paraList[8], (bool)paraList[9], (DateTime)paraList[10]);
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
                //writer.Write("EXEC_Plugin_AddJCWin100CountBusiness_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 用户总中奖金额1000以上,派奖接口
    /// </summary>
    public class Win1000YuanBusiness : IOrderPrize_AfterTranCommit
    {
        /// <summary> 
        /// 用户总中奖金额1000以上,派奖接口
        /// </summary>
        public void OrderPrize_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal orderMoney,
            bool isBonus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney, bool isVirtualOrder, DateTime prizeTime)
        {
            if (afterTaxBonusMoney <= 0M) return;
            if (isVirtualOrder) return;

            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(userId, TaskCategory.Win1000Yuan);
            if (old != null)
                return;

            gv.AddUserTaskRecord(new E_UserTaskRecord
            {
                OrderMoney = afterTaxBonusMoney,
                UserId = userId,
                OrderId = schemeId,
                CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                TaskCategory = (int)TaskCategory.Win1000Yuan,
                TaskName = "累计中奖金额1000元",
                CreateTime = DateTime.Now,
            });

            var recordList = gv.QueryUserTaskRecordByCategory(userId, TaskCategory.Win1000Yuan);
            var totalWinMoey = recordList.Count == 0 ? 0M : recordList.Sum(p => p.OrderMoney);
            if (totalWinMoey < 1000M)
                return;

            //增加成长值 
            BusinessHelper.Payin_UserGrowth("累计中奖金额1000元", schemeId, userId, 500, "累计中奖金额1000元可获得500点成长值");
            gv.AddTaskList(new E_TaskList
            {
                UserId = userId,
                OrderId = schemeId,
                Content = "累计中奖金额1000元可获得500点成长值",
                ValueGrowth = 500,//20150531 dj 修改：原值100，修改为500
                IsGive = true,
                VipLevel = 0,
                CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                TaskCategory = (int)TaskCategory.Win1000Yuan,
                TaskName = "累计中奖金额1000元",
                CreateTime = DateTime.Now,
            });
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IOrderPrize_AfterTranCommit":
                        OrderPrize_AfterTranCommit((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3], (string)paraList[4], (decimal)paraList[5], (bool)paraList[6], (decimal)paraList[7], (decimal)paraList[8], (bool)paraList[9], (DateTime)paraList[10]);
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
                //writer.Write("EXEC_Plugin_AddWin1000YuanBusiness_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 首次使用,奖金优化投注
    /// </summary>
    public class BonusOptimize : IBonusOptimize
    {
        public void AddBonusOptimize(string userId, string schemeId)
        {
            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(userId, TaskCategory.BonusOptimize);
            if (old != null)
                return;

            //查询vip等级
            var balanceManager = new UserBalanceManager();
            var user = balanceManager.QueryUserRegister(userId);
            if (user == null)
                return;

            gv.AddUserTaskRecord(new E_UserTaskRecord
            {
                OrderMoney = 0M,
                UserId = userId,
                OrderId = schemeId,
                CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                TaskCategory = (int)TaskCategory.BonusOptimize,
                TaskName = "首次使用奖金优化",
                CreateTime = DateTime.Now,
            });
            if (user.VipLevel >= 4)
            {
                //增加成长值 
                BusinessHelper.Payin_UserGrowth("首次使用奖金优化", schemeId, userId, 200, "首次使用奖金优化购彩可获得200点成长值");
                gv.AddTaskList(new E_TaskList
                {
                    UserId = userId,
                    OrderId = schemeId,
                    Content = "首次使用奖金优化购彩可获得200点成长值",
                    ValueGrowth = 200,
                    VipLevel = 4,
                    IsGive = true,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.BonusOptimize,
                    TaskName = "首次使用奖金优化",
                    CreateTime = DateTime.Now,
                });
            }
            else
            {
                //储存成长值
                gv.AddTaskList(new E_TaskList
                {
                    UserId = userId,
                    OrderId = schemeId,
                    Content = "首次使用奖金优化购彩可获得200点成长值",
                    ValueGrowth = 200,
                    VipLevel = 4,
                    IsGive = false,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.BonusOptimize,
                    TaskName = "首次使用奖金优化",
                    CreateTime = DateTime.Now,
                });
            }
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IBonusOptimize":
                        AddBonusOptimize((string)paraList[0], (string)paraList[1]);
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
                //writer.Write("EXEC_Plugin_AddBonusOptimize_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 奖金优化投注满5次
    /// </summary>
    public class BonusOptimizeTotal5 : IBonusOptimize
    {
        public void AddBonusOptimize(string userId, string schemeId)
        {
            var gv = new TaskListManager();
            var tadayTime = DateTime.Now.ToString("yyyyMMdd");
            var old = gv.QueryToDayGiveTask(userId, TaskCategory.BonusBuyLotteryTotle5, tadayTime);
            if (old != null)
                return;

            gv.AddUserTaskRecord(new E_UserTaskRecord
            {
                OrderMoney = 0M,
                UserId = userId,
                OrderId = schemeId,
                CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                TaskCategory = (int)TaskCategory.BonusBuyLotteryTotle5,
                TaskName = "奖金优化投注满5次",
                CreateTime = DateTime.Now,
            });

            var recordList = gv.QueryUserTaskRecordByCategory(userId, TaskCategory.BonusBuyLotteryTotle5);
            if (recordList.Count < 5)
                return;
            //查询vip等级
            var balanceManager = new UserBalanceManager();
            var user = balanceManager.QueryUserRegister(userId);
            if (user == null)
                return;

            if (user.VipLevel >= 3)
            {
                //增加成长值 
                BusinessHelper.Payin_UserGrowth("奖金优化投注满5次", schemeId, userId, 100, "成功使用奖金优化投注满5次领取100点成长值");
                gv.AddTaskList(new E_TaskList
                {
                    UserId = userId,
                    OrderId = schemeId,
                    Content = "成功使用奖金优化投注满5次领取100点成长值",
                    ValueGrowth = 100,
                    IsGive = true,
                    VipLevel = 3,
                    CurrentTime = tadayTime,
                    TaskCategory = (int)TaskCategory.BonusBuyLotteryTotle5,
                    TaskName = "奖金优化投注满5次",
                    CreateTime = DateTime.Now,
                });
            }
            else
            {
                //储存成长值
                gv.AddTaskList(new E_TaskList
                {
                    UserId = userId,
                    OrderId = schemeId,
                    Content = "成功使用奖金优化投注满5次领取100点成长值",
                    ValueGrowth = 100,
                    IsGive = false,
                    VipLevel = 3,
                    CurrentTime = tadayTime,
                    TaskCategory = (int)TaskCategory.BonusBuyLotteryTotle5,
                    TaskName = "奖金优化投注满5次",
                    CreateTime = DateTime.Now,
                });
            }
        }

        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IBonusOptimize":
                        AddBonusOptimize((string)paraList[0], (string)paraList[1]);
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
                //writer.Write("EXEC_Plugin_BonusOptimizeTotal5_Error_", type, ex);
            }

            return null;
        }
    }

    /// <summary>
    /// 首次特别中奖奖金优化,派奖接口
    /// </summary>
    public class SpecialWinReward : IOrderPrize_AfterTranCommit
    {
        /// <summary> 
        /// 首次特别中奖奖金优化,派奖接口
        /// </summary>
        public void OrderPrize_AfterTranCommit(string userId, string schemeId, string gameCode, string gameType, string issuseNumber, decimal orderMoney,
            bool isBonus, decimal preTaxBonusMoney, decimal afterTaxBonusMoney, bool isVirtualOrder, DateTime prizeTime)
        {
            if (afterTaxBonusMoney < 0M) return;
            if (isVirtualOrder) return;

            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Complate(schemeId);
            if (order == null)
                return;
            if (order.SchemeBettingCategory != (int)SchemeBettingCategory.YouHua)
                return;

            var gv = new TaskListManager();
            var old = gv.QueryTaskListByCategory(userId, TaskCategory.SpecialWinReward);
            if (old != null)
                return;

            #region 赠送成长值

            //查询vip等级
            var balanceManager = new UserBalanceManager();
            var user = balanceManager.QueryUserRegister(userId);
            if (user == null)
                return;

            gv.AddUserTaskRecord(new E_UserTaskRecord
            {
                UserId = userId,
                OrderId = schemeId,
                CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                TaskCategory = (int)TaskCategory.SpecialWinReward,
                TaskName = "特别中奖奖励",
                CreateTime = DateTime.Now,
            });
            if (user.VipLevel >= 4)
            {
                //增加成长值 
                BusinessHelper.Payin_UserGrowth("特别中奖奖励", schemeId, userId, 100, "使用奖金优化首次中奖可获得特别奖励可获得100点成长值");
                gv.AddTaskList(new E_TaskList
                {
                    UserId = userId,
                    OrderId = schemeId,
                    Content = "使用奖金优化首次中奖可获得特别奖励可获得100点成长值",
                    ValueGrowth = 100,
                    VipLevel = 4,
                    IsGive = true,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.SpecialWinReward,
                    TaskName = "特别中奖奖励",
                    CreateTime = DateTime.Now,
                });
            }
            else
            {
                //储存成长值
                gv.AddTaskList(new E_TaskList
                {
                    UserId = userId,
                    OrderId = schemeId,
                    Content = "使用奖金优化首次中奖可获得特别奖励可获得100点成长值",
                    ValueGrowth = 100,
                    VipLevel = 4,
                    IsGive = false,
                    CurrentTime = DateTime.Now.ToString("yyyyMMdd"),
                    TaskCategory = (int)TaskCategory.SpecialWinReward,
                    TaskName = "特别中奖奖励",
                    CreateTime = DateTime.Now,
                });
            }

            #endregion

        }
        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "IOrderPrize_AfterTranCommit":
                        OrderPrize_AfterTranCommit((string)paraList[0], (string)paraList[1], (string)paraList[2], (string)paraList[3], (string)paraList[4], (decimal)paraList[5], (bool)paraList[6], (decimal)paraList[7], (decimal)paraList[8], (bool)paraList[9], (DateTime)paraList[10]);
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
                //writer.Write("EXEC_Plugin_AddSpecialWinReward_Error_", type, ex);
            }

            return null;
        }
    }

    #region 任务相关查询

    /// <summary>
    /// 前台查询
    /// </summary>
    public class TaskBusiness
    {
        /// <summary>
        ///查询某成长值的赠送记录
        /// </summary>
        public TaskListInfoCollection QueryCompleteTaskList(string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var result = new TaskListInfoCollection();
            var totalCount = 0;
            result.List.AddRange(new TaskListManager().QueryCompleteTaskList(userId, starTime, endTime, pageIndex, pageSize, out totalCount));
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        ///查询某成长值的赠送记录
        /// </summary>
        public TaskListInfoCollection QueryCompleteTaskToUserList(string userId)
        {
            var result = new TaskListInfoCollection();
            result.List.AddRange(new TaskListManager().QueryCompleteTaskToUserList(userId));
            return result;
        }

        /// <summary>
        ///查询某人成长值的累计型完成进度
        /// </summary>
        public TaskHotCumulativeInfoCollection QueryTaskListProgress(string userId)
        {
            var result = new TaskHotCumulativeInfoCollection();
            result.List.AddRange(new TaskListManager().QueryTaskListProgress(userId));
            return result;
        }

        /// <summary>
        ///查询该任务是否今天完成
        /// </summary>
        public bool QueryTaskUserToday(string userId, TaskCategory taskCategory)
        {
            var today = new TaskListManager().QueryTaskUserToday(userId, taskCategory);
            if (today == 1)
                return true;
            return false;
        }

        /// <summary>
        ///最新会员得到成长值动态
        /// </summary>
        public TaskHotTodayInfooCollection QueryTaskHotTodayInfoList(int lenth)
        {
            var result = new TaskHotTodayInfooCollection();
            result.List.AddRange(new TaskListManager().QueryTaskHotTodayInfoList(lenth));
            return result;
        }

        /// <summary>
        /// 自动完成未赠送的 用户任务 成长值
        /// </summary>
        public void AutoComplateTask()
        {
            var balanceManager = new UserBalanceManager();
            //查询所有任务
            var taskManager = new TaskListManager();
            var taskList = taskManager.QueryWaitComplateTask();
            var g = taskList.GroupBy(p => p.UserId);
            foreach (var item in g)
            {
                var user = balanceManager.QueryUserRegister(item.Key);
                var userVipLevel = user.VipLevel;
                var userTaskList = taskList.Where(p => p.UserId == user.UserId).OrderBy(p => p.CreateTime).ToList();
                foreach (var task in userTaskList)
                {
                    if (userVipLevel >= task.VipLevel)
                    {
                        //执行赠送成长值
                        if (task.ValueGrowth > 0M)
                            userVipLevel = BusinessHelper.Payin_UserGrowth(task.TaskName, task.OrderId, user.UserId, (int)task.ValueGrowth, task.Content);
                    }
                    task.IsGive = true;
                    taskManager.UpdateTaskList(task);
                }
            }
        }
        public int QueryLoginDayByUserId(string userId)
        {
            var gv = new TaskListManager();
            var historyList = new List<string>();
            for (int i = 0; i < 7; i++)
            {
                historyList.Add(DateTime.Today.AddDays(-i).ToString("yyyyMMdd"));
            }
            var loginList = gv.QueryUserTaskRecordByCategory(userId, TaskCategory.EveryDayLogin, historyList.ToArray());
            var count = 1;
            //if (loginList.Count >= 7)
            //{
            //    count = 5;
            //}
            count = new LoginHistoryBusiness().GetUserTaskRecordSort(loginList);

            return count;
        }
    }

    #endregion
}
