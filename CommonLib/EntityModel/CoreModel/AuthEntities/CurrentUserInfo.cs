using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{

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
        public UserRealNameInfo RealNameInfo { get; set; }
        /// <summary>
        /// 用户邮箱认证信息
        /// </summary>
        //public UserEmailInfo EmailInfo { get; set; }
        ///// <summary>
        ///// 用户手机认证信息
        ///// </summary>
        //public UserMobileInfo MobileInfo { get; set; }
        ///// <summary>
        ///// 绑定银行卡信息
        ///// </summary>
        //public BankCardInfo BankCardInfo { get; set; }
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

}
