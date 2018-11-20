using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using Common.Communication;
using GameBiz.Domain.Managers;
using GameBiz.Business;

namespace External.Business
{
    public class SendInnerMsgBusiness : ITogetherFollow_AfterTranCommit, IAttention_AfterTranCommit
    {
        /// <summary>
        /// 定制跟单
        /// </summary>
        /// <param name="info"></param>
        public void TogetherFollow_AfterTranCommit(TogetherFollowerRuleInfo info)
        {
            if (info == null) return;
            else if (string.IsNullOrEmpty(info.CreaterUserId) || string.IsNullOrEmpty(info.FollowerUserId)) return;
            UserBalanceManager userBalanceManger = new UserBalanceManager();
            var createUserInfo = userBalanceManger.GetUserRegister(info.CreaterUserId);
            if (createUserInfo == null) return;
            var followUserInfo = userBalanceManger.GetUserRegister(info.FollowerUserId);
            if (followUserInfo == null) return;
            var gameCodeName = string.Empty;
            var gameTypeName = string.Empty;
            gameCodeName = BusinessHelper.FormatGameCode(info.GameCode);
            if (!string.IsNullOrEmpty(info.GameType))
                gameTypeName = BusinessHelper.FormatGameType_Each(info.GameCode, info.GameType);
            SiteMessageControllBusiness siteMsgBusiness = new SiteMessageControllBusiness();
            var array = new string[] { "[UserName]=" + followUserInfo.DisplayName + "", "[CreateUser]=" + createUserInfo.DisplayName + "", "[GameCodeName]=" + gameCodeName + "", "[GameTypeName]=" + gameTypeName + "" };
            siteMsgBusiness.DoSendSiteMessage(info.FollowerUserId, string.Empty, "ON_User_Scheme_Together_FollowerRule", array);
        }
        
        public object ExecPlugin(string type, object inputParam)
        {
            try
            {
                var paraList = inputParam as object[];
                switch (type)
                {
                    case "ITogetherFollow_AfterTranCommit":
                        TogetherFollow_AfterTranCommit((TogetherFollowerRuleInfo)paraList[0]);
                        break;
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
                var writer = Common.Log.LogWriterGetter.GetLogWriter();
                writer.Write("EXEC_Plugin_AddA20140902_Business购彩不花钱_Error_", type, ex);
            }

            return null;
        }
        
        /// <summary>
        /// 关注
        /// </summary>
        /// <param name="activeUserId"></param>
        /// <param name="passiveUserId"></param>
        public void Attention_AfterTranCommit(string activeUserId, string passiveUserId)
        {
            if (string.IsNullOrEmpty(activeUserId) || string.IsNullOrEmpty(passiveUserId)) return;
            UserBalanceManager userBalanceManger = new UserBalanceManager();
            var activeUserInfo = userBalanceManger.GetUserRegister(activeUserId);
            if (activeUserInfo == null) return;
            var paasiveUserInfo = userBalanceManger.GetUserRegister(passiveUserId);
            if (paasiveUserInfo == null) return;

            SiteMessageControllBusiness siteMsgBusiness = new SiteMessageControllBusiness();
            var array = new string[] { "[UserName]=" + activeUserInfo.DisplayName + "", "[AttentionUser]=" + paasiveUserInfo.DisplayName + "" };
            siteMsgBusiness.DoSendSiteMessage(activeUserInfo.UserId, string.Empty, "ON_User_Attention", array);
        }
    }
}
