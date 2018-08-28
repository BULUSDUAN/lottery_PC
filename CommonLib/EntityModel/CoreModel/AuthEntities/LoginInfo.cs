using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    [ProtoContract]
    [Serializable]
    public class LoginInfo
    {
        /// <summary>
        /// 是否登录成功
        /// </summary>
        [ProtoMember(1)]
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        [ProtoMember(2)]
        public string Message { get; set; }
        /// <summary>
        /// 登录号
        /// </summary>
        [ProtoMember(3)]
        public string UserId { get; set; }
        /// <summary>
        /// VIP 级别
        /// </summary>
        [ProtoMember(4)]
        public int VipLevel { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        [ProtoMember(5)]
        public string LoginName { get; set; }
        /// <summary>
        /// 用户显示名称
        /// </summary>
        [ProtoMember(6)]
        public string DisplayName { get; set; }
        /// <summary>
        /// 登录方式
        /// </summary>
        [ProtoMember(7)]
        public string LoginFrom { get; set; }
        /// <summary>
        /// 用户口令
        /// </summary>
        [ProtoMember(8)]
        public string UserToken { get; set; }
        [ProtoMember(9)]
        public string Referrer { get; set; }
        [ProtoMember(10)]
        public string RegType { get; set; }
        [ProtoMember(11)]
        public string AgentId { get; set; }
        [ProtoMember(12)]
        public bool IsAgent { get; set; }
        [ProtoMember(13)]
        public int HideDisplayNameCount { get; set; }
        [ProtoMember(14)]
        public DateTime CreateTime { get; set; }
        [ProtoMember(15)]
        public List<string> FunctionList { get; set; }
        [ProtoMember(16)]
        public bool IsAdmin { get; set; }
        [ProtoMember(17)]
        public decimal CommissionBalance { get; set; }
        [ProtoMember(18)]
        public decimal FreezeBalance { get; set; }
        [ProtoMember(19)]
        public bool IsGeneralUser { get; set; }
        [ProtoMember(20)]
        public string MaxLevelName { get; set; }
        [ProtoMember(21)]
        public bool IsRebate { get; set; }
        /// <summary>
        /// 是否内部员工 0:网站普通用户；1：内部员工用户
        /// </summary>
        [ProtoMember(22)]
        public bool IsUserType { get; set; }
    }

    /// <summary>
    /// 用户绑定的数据
    /// </summary>
    [ProtoContract]
    public class UserBindInfos
    {
        [ProtoMember(1)]
        public string UserId { get; set; }
        [ProtoMember(2)]
        public int VipLevel { get; set; }
        [ProtoMember(3)]
        public string DisplayName { get; set; }
        [ProtoMember(4)]
        public string ComeFrom { get; set; }
        [ProtoMember(5)]
        public bool IsFillMoney { get; set; }
        [ProtoMember(6)]
        public bool IsEnable { get; set; }
        [ProtoMember(7)]
        public bool IsAgent { get; set; }
        [ProtoMember(8)]
        public int HideDisplayNameCount { get; set; }
        [ProtoMember(9)]
        public string BankCardRealName { get; set; }
         [ProtoMember(10)]
        public string ProvinceName { get; set; }
        [ProtoMember(11)]
        public string CityName { get; set; }
        [ProtoMember(12)]
        public string BankName { get; set; }
        [ProtoMember(13)]
        public string BankSubName { get; set; }
        [ProtoMember(14)]
        public string BankCardNumber { get; set; }
        [ProtoMember(15)]
        public string Email { get; set; }
        [ProtoMember(16)]
        public bool IsSettedMobile { get; set; }
        [ProtoMember(17)]
        public string Mobile { get; set; }
        [ProtoMember(18)]
        public string RealName { get; set; }
        [ProtoMember(19)]
        public string CardType { get; set; }
        [ProtoMember(20)]
        public string IdCardNumber { get; set; }
        [ProtoMember(21)]
        public string QQ { get; set; }
        [ProtoMember(22)]
        public string AlipayAccount { get; set; }
        [ProtoMember(23)]
        public int RebateCount { get; set; }
        [ProtoMember(24)]
        public int MaxLevelValue { get; set; }
        [ProtoMember(25)]
        public string MaxLevelName { get; set; }
        [ProtoMember(26)]
        public int WinOneHundredCount { get; set; }
        [ProtoMember(27)]
        public int WinOneThousandCount { get; set; }
        [ProtoMember(28)]
        public int WinTenThousandCount { get; set; }
        [ProtoMember(29)]
        public int WinOneHundredThousandCount { get; set; }
        [ProtoMember(30)]
        public int WinOneMillionCount { get; set; }
        [ProtoMember(31)]
        public int WinTenMillionCount { get; set; }
        [ProtoMember(32)]
        public int WinHundredMillionCount { get; set; }
        [ProtoMember(33)]
        public decimal TotalBonusMoney { get; set; }
        [ProtoMember(34)]
        public DateTime LoadDateTime { get; set; }
        /// <summary>
        /// 是否内部用户
        /// </summary>
        [ProtoMember(35)]
        public int IsUserType { get; set; }
        /// <summary>
        /// 佣金账户，为代理商计算佣金时，转到此账户
        /// </summary
        [ProtoMember(36)]
        public decimal CommissionBalance { get; set; }
        /// <summary>
        /// 名家余额
        /// </summary>
        [ProtoMember(37)]
        public decimal ExpertsBalance { get; set; }
        /// <summary>
        /// 奖金账户，中奖后返到此账户，可提现
        /// </summary>
        [ProtoMember(38)]
        public decimal BonusBalance { get; set; }
        /// <summary>
        /// 冻结账户，提现、追号、异常手工冻结
        /// </summary>
        [ProtoMember(39)]
        public decimal FreezeBalance { get; set; }
        /// <summary>
        /// 充值账户余额，充值充到此账户
        /// </summary>
        [ProtoMember(40)]
        public decimal FillMoneyBalance { get; set; }
        /// <summary>
        /// 红包余额
        /// </summary>
        [ProtoMember(41)]
        public decimal RedBagBalance { get; set; }
        /// <summary>
        /// 成长值
        /// </summary>
        [ProtoMember(42)]
        public int UserGrowth { get; set; }
        [ProtoMember(43)]
        public bool IsSetPwd { get; set; }
        /// <summary>
        /// 需要输入资金密码的地方
        /// </summary>
        [ProtoMember(44)]
        public string NeedPwdPlace { get; set; }
    }
}

