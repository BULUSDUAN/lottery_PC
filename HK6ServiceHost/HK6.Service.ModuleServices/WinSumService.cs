using Kason.Sg.Core.CPlatform.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using EntityModel.Enum;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common;

using EntityModel.Communication;
using KaSon.FrameWork.ORM.Helper;
using KaSon.FrameWork.Common.Sport;
using System.Threading.Tasks;

using System.IO;
using System.Text;
using EntityModel.ExceptionExtend;


using Kason.Sg.Core.ProxyGenerator;
using Microsoft.Extensions.Logging;
using KaSon.FrameWork.ORM;
using EntityModel;
using KaSon.FrameWork.Analyzer.Hk6Model;
using KaSon.FrameWork.Common.Hk6;

namespace HK6.ModuleBaseServices
{
    /// <summary>
    /// 管理系统服务
    /// </summary>
    [ModuleName("WinSum")]
    public class WinSumService : KgBaseService, IWinSumService
    {

        //IKgLog log = null;
        //public BettingService()
        //{
        //    log = new Log4Log();
        //}
        ILogger<BettingService> _Log;
        private readonly Repository _rep;
        private IDbProvider DB = null;
        private IDbProvider LettoryDB = null;
        public WinSumService(Repository repository, ILogger<BettingService> log)
        {
            _Log = log;
            this._rep = repository;
            DB = _rep.LDB.Init("MySql.Default", true);
            LettoryDB = _rep.DB.Init("SqlServer.Default", true);
        }
        /// <summary>
        /// 普通订单缓存数据
        /// </summary>
        private static Dictionary<string, HK6Sports_BetingInfo> _bettingListInfo = new Dictionary<string, HK6Sports_BetingInfo>();

        /// <summary>
        /// 开奖日期，开奖时间
        /// </summary>
        /// <param name="winDate"></param>
        /// <param name="winNum"></param>
        /// <returns></returns>
       public Task<CommonActionResult> Sum(string userId, string IssueNo, string winNum) {
            CommonActionResult result = new CommonActionResult();

            #region 校验token 权限校验
           // string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(tokens);
            //C_Auth_UserRole
            var UserRole = LettoryDB.CreateQuery<C_Auth_UserRole>().Where(b => b.UserId == userId).FirstOrDefault();
            if (UserRole==null)
            {
               // result.ReturnValue = ex.ToString();
                result.Message = "没有权限操作";
                result.IsSuccess = false;
                result.Code = 300;
                result.StatuCode = 300;
                return Task.FromResult(result);
            }
            string roleid = UserRole.RoleId;
            var Auth_Roles = LettoryDB.CreateQuery<C_Auth_Roles>().Where(b => b.RoleId == roleid).FirstOrDefault();
            if (Auth_Roles != null && Auth_Roles.IsAdmin)
            {

            }
            else {
                var Auth_RoleFunction = LettoryDB.CreateQuery<C_Auth_RoleFunction>().Where(b => b.RoleId == roleid).FirstOrDefault();
                if (Auth_RoleFunction==null || Auth_RoleFunction.FunctionId.Trim() != "GLHKJ100")
                {
                    result.Message = "没有权限操作";
                    result.IsSuccess = false;
                    result.Code = 300;
                    result.StatuCode = 300;
                    return Task.FromResult(result);

                }

            }
            #endregion
            //
            try
            {
                var playedlist = DB.CreateQuery<blast_played>().ToList();
              var list=  DB.CreateQuery<blast_bet_orderdetail>().Where(b => b.issueNo == IssueNo && b.BonusStatus==0).ToList<blast_bet_orderdetail>();

                DB.Begin();
             
               // BaseOrderHelper bh = new BaseOrderHelper();
                foreach (blast_bet_orderdetail item in list)
                {
                    //开始结算
                    
                    string tm = winNum.Split('|')[1];
                    string zm = winNum.Split('|')[0];
                    var p = playedlist.Where(b=>b.playId==item.playId).FirstOrDefault();

                    BaseOrderHelper winHelper = BaseOrderHelper.GetOrderHelper(item, DB);
                    winHelper.WinMoney(item, winNum);
                    
                }
                //添加记录
                int atcNo = int.Parse(IssueNo);
                DB.GetDal<blast_data_time>().Update(b => new blast_data_time()
                {
                    isOpen = true,
                    winNum = winNum,
                }, b => b.actionNo == IssueNo);
                DB.Commit();
                result.Message = "开奖成功";
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ReturnValue = ex.ToString();
                result.Message = "系统错误";
                result.IsSuccess = false;
                result.Code = 500;
                result.StatuCode = 500;
                //throw;
                DB.Rollback();
            }
            finally {
                LettoryDB.Dispose();
                DB.Dispose();
            }


           return Task.FromResult(result);
        }
    }
}
