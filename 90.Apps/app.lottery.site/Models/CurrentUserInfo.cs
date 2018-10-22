using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Common.Net;
using System.Text;
using System.Configuration;
using Common.Utilities;
using System.IO;
using Common.Log;
using System.Collections;
using EntityModel;
using External.Core.Authentication;
using EntityModel.CoreModel.AuthEntities;
using EntityModel.CoreModel;

namespace app.lottery.site.Models
{
    /// <summary>
    /// 当前登录用户对象
    /// </summary>
    public class CurrentUserInfo
    {
        /// <summary>
        /// 用户登录信息
        /// </summary>
        public LoginInfo LoginInfo { get; set; }
        /// <summary>
        /// 用户是否进行实名认证
        /// </summary>
        public bool IsAuthenticationRealName { get; set; }
        /// <summary>
        /// 用户是否进行手机认证
        /// </summary>
        public bool IsAuthenticationMobile { get; set; }
        /// <summary>
        /// 用户是否进行邮箱认证
        /// </summary>
        public bool IsAuthenticationEmail { get; set; }
        /// <summary>
        /// 是否已绑定银行卡
        /// </summary>
        public bool IsBindBank { get; set; }
        /// <summary>
        /// 用户余额
        /// </summary>
        //public UserBalanceInfo UserBalance { get; set; }
        /// <summary>
        /// 用户实名认证信息
        /// </summary>
        public EntityModel.CoreModel.UserRealNameInfo RealNameInfo { get; set; }
        /// <summary>
        /// 用户邮箱认证信息
        /// </summary>
        public EntityModel.CoreModel.UserEmailInfo EmailInfo { get; set; }
        /// <summary>
        /// 用户手机认证信息
        /// </summary>
        public EntityModel.CoreModel.UserMobileInfo MobileInfo { get; set; }
        /// <summary>
        /// 绑定银行卡信息
        /// </summary>
        public BankCardInfo BankCardInfo { get; set; }
        /// <summary>
        /// 用户的完善资料里的QQ号码
        /// </summary>
        public string QQNumber { get; set; }
        /// <summary>
        /// 支付宝用户独立信息
        /// </summary>
        public string AlipayInfo { get; set; }
        /// <summary>
        /// 最后一次登录信息
        /// </summary>
        public UserLoginHistoryInfo LastLoginInfo { get; set; }
        /// <summary>
        /// 推广员信息
        /// </summary>
        //public PromoterInfo PromoterInfo { get; set; }
        /// <summary>
        /// 用户是否为推广员
        /// </summary>
        public bool IsPromoter { get; set; }
        /// <summary>
        /// 用户头像地址
        /// </summary>
        public string Figureurl { get; set; }
        /// <summary>
        /// 用户头像地址 1
        /// </summary>
        public string Figureurl_1 { get; set; }
        /// <summary>
        /// 用户头像地址 2
        /// </summary>
        public string Figureurl_2 { get; set; }
        /// <summary>
        /// 用户性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 用户QQ VIP 等级
        /// </summary>
        public string QQVipLevel { get; set; }
    }

    public class ThendLinkInfo
    {
        public string LinkName { get; set; }
        public string LinkUrl { get; set; }
    }

    public class HotThendCollection
    {
        public HotThendCollection()
        {
            LinkList = new List<ThendLinkInfo>();
        }

        public string MoreLinkName { get; set; }
        public string MoreLinkUrl { get; set; }
        public string GameCode { get; set; }
        public string GameName { get; set; }

        public List<ThendLinkInfo> LinkList { get; set; }
    }

    /// <summary>
    /// 走势图彩种类
    /// </summary>
    public class ThendLinkCollection
    {
        public ThendLinkCollection()
        {
            LinkList = new List<ThendLinkInfo>();
        }
        public string GDThend { get; set; }
        public string GameCode { get; set; }
        public string GameName { get; set; }
        public List<ThendLinkInfo> LinkList { get; set; }
    }

    /// <summary>
    /// 首页快速购买
    /// </summary>
    public class LotteryInfo
    {
        public string issue { get; set; }
        public string betTime { get; set; }
        public string awardTime { get; set; }
        public string pre { get; set; }
        public string win { get; set; }
        public int sale { get; set; }
    }

    #region 奖金优化
    public class Optimization
    {
        public string id { get; set; }         //id
        public bool dan { get; set; }          //设胆
        public string week { get; set; }       //场次
        public string host { get; set; }       //主队
        public string visit { get; set; }      //客队
        public int lose { get; set; }          //让球
        public string stoptime { get; set; }   //截止时间
        public string items { get; set; }      //投注选项
    }

    public class GameMatch
    {
        public string id { get; set; }
        public bool dan { get; set; }
        public string host { get; set; }
        public string[] matches { get; set; }
        public string matchcc { get; set; }
    }

    public class GameCombination
    {
        public double money { get; set; }
        public string combination { get; set; }
    }
    #endregion
}