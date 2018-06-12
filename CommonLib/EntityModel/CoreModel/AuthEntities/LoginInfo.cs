using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    [Serializable]
    public class LoginInfo
    {
        /// <summary>
        /// 是否登录成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 登录号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// VIP 级别
        /// </summary>
        public int VipLevel { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 用户显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 登录方式
        /// </summary>
        public string LoginFrom { get; set; }
        /// <summary>
        /// 用户口令
        /// </summary>
        public string UserToken { get; set; }

        public string Referrer { get; set; }
        public string RegType { get; set; }

        public string AgentId { get; set; }
        public bool IsAgent { get; set; }
        public int HideDisplayNameCount { get; set; }
        public DateTime CreateTime { get; set; }
        public List<string> FunctionList { get; set; }
        public bool IsAdmin { get; set; }
        public decimal CommissionBalance { get; set; }
        public decimal FreezeBalance { get; set; }
        public bool IsGeneralUser { get; set; }
        public string MaxLevelName { get; set; }
        public bool IsRebate { get; set; }
        /// <summary>
        /// 是否内部员工 0:网站普通用户；1：内部员工用户
        /// </summary>
        public bool IsUserType { get; set; }
    }

    /// <summary>
    /// 用户绑定的数据
    /// </summary>
    public class UserBindInfos
    {
        public string UserId { get; set; }
        public int VipLevel { get; set; }
        public string DisplayName { get; set; }
        public string ComeFrom { get; set; }
        public bool IsFillMoney { get; set; }
        public bool IsEnable { get; set; }
        public bool IsAgent { get; set; }
        public int HideDisplayNameCount { get; set; }

        public string BankCardRealName { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string BankName { get; set; }
        public string BankSubName { get; set; }
        public string BankCardNumber { get; set; }

        public string Email { get; set; }
        public bool IsSettedMobile { get; set; }
        public string Mobile { get; set; }
        public string RealName { get; set; }
        public string CardType { get; set; }
        public string IdCardNumber { get; set; }
        public string QQ { get; set; }
        public string AlipayAccount { get; set; }

        //public string LastLoginFrom { get; set; }
        //public string LastLoginIp { get; set; }
        //public string LastLoginIpName { get; set; }
        //public string LastLoginTime { get; set; }

        public int RebateCount { get; set; }

        public int MaxLevelValue { get; set; }
        public string MaxLevelName { get; set; }
        public int WinOneHundredCount { get; set; }
        public int WinOneThousandCount { get; set; }
        public int WinTenThousandCount { get; set; }
        public int WinOneHundredThousandCount { get; set; }
        public int WinOneMillionCount { get; set; }
        public int WinTenMillionCount { get; set; }
        public int WinHundredMillionCount { get; set; }
        public decimal TotalBonusMoney { get; set; }

        public DateTime LoadDateTime { get; set; }
        /// <summary>
        /// 是否内部用户
        /// </summary>
        public int IsUserType { get; set; }

    }
}

