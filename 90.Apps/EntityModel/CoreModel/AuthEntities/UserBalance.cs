using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    /// <summary>
    /// 用户余额
    /// </summary>
    public class UserBalance
    {
        public UserBalance()
        {
            Version = 1;
        }
        public virtual string UserId { get; set; }
        public virtual int Version { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public virtual SystemUser User { get; set; }
        /// <summary>
        /// 充值账户余额
        /// </summary>
        public virtual decimal FillMoneyBalance { get; set; }
        /// <summary>
        /// 奖金账户，中奖后返到此账户，可提现
        /// </summary>
        public virtual decimal BonusBalance { get; set; }
        /// <summary>
        /// 佣金账户，为代理商计算佣金时，转到此账户
        /// </summary>
        public virtual decimal CommissionBalance { get; set; }
        /// <summary>
        /// 名家余额
        /// </summary>
        public virtual decimal ExpertsBalance { get; set; }
        /// <summary>
        /// 冻结账户，提现、追号、异常手工冻结
        /// </summary>
        public virtual decimal FreezeBalance { get; set; }
        /// <summary>
        /// 红包余额
        /// </summary>
        public virtual decimal RedBagBalance { get; set; }
        /// <summary>
        /// CPS余额
        /// </summary>
        public virtual decimal CPSBalance { get; set; }


        /// <summary>
        /// 成长值
        /// </summary>
        public virtual int UserGrowth { get; set; }
        /// <summary>
        /// 当前豆豆值
        /// </summary>
        public virtual int CurrentDouDou { get; set; }


        public virtual bool IsSetPwd { get; set; }
        /// <summary>
        /// 需要输入资金密码的地方
        /// </summary>
        public virtual string NeedPwdPlace { get; set; }
        /// <summary>
        /// 资金密码
        /// </summary>
        public virtual string Password { get; set; }
        /// <summary>
        /// 代理商
        /// </summary>
        public virtual string AgentId { get; set; }

        public virtual decimal GetTotalEnableMoney()
        {
            //return this.FillMoneyBalance + this.BonusBalance + this.CommissionBalance + this.ExpertsBalance + this.RedBagBalance;
            //return this.FillMoneyBalance + this.BonusBalance + this.ExpertsBalance + this.RedBagBalance;
            return this.FillMoneyBalance + this.BonusBalance + this.CommissionBalance;
        }
    }
    /// <summary>
    /// 用户资金冻结明细
    /// </summary>
    //public class UserBalanceFreeze
    //{
    //    public virtual long Id { get; set; }
    //    public virtual string UserId { get; set; }
    //    public virtual string OrderId { get; set; }
    //    public virtual decimal FreezeMoney { get; set; }
    //    public virtual FrozenCategory Category { get; set; }
    //    public virtual string Description { get; set; }
    //    public virtual DateTime CreateTime { get; set; }
    //}

    /// <summary>
    /// 注册信息
    /// </summary>
    public class UserRegister
    {
        public virtual string UserId { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public virtual SystemUser User { get; set; }
        /// <summary>
        /// VIP等级
        /// </summary>
        public virtual int VipLevel { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public virtual string DisplayName { get; set; }
        /// <summary>
        /// 用户名称来源
        /// </summary>
        public virtual string ComeFrom { get; set; }
        /// <summary>
        /// 注册IP
        /// </summary>
        public virtual string RegisterIp { get; set; }
        /// <summary>
        /// 注册引用页面
        /// </summary>
        public virtual string ReferrerUrl { get; set; }
        /// 注册引用
        /// </summary>
        public virtual string Referrer { get; set; }
        /// <summary>
        /// 注册类型
        /// </summary>
        public virtual string RegType { get; set; }
        public virtual bool IsEnable { get; set; }
        public virtual bool IsAgent { get; set; }
        /// <summary>
        /// 是否是名家
        /// </summary>
        public virtual bool IsExperter { get; set; }
        /// <summary>
        /// 是否充值
        /// </summary>
        public virtual bool IsFillMoney { get; set; }
        /// <summary>
        /// 隐藏用户名数
        /// </summary>
        public virtual int HideDisplayNameCount { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 代理商
        /// </summary>
        public virtual string AgentId { get; set; }
        public virtual bool IsIgnoreReport { get; set; }
        /// <summary>
        /// 父级路径
        /// </summary>
        public virtual string ParentPath { get; set; }
        /// <summary>
        /// 用户类别:0:网站普通用户；1：内部员工用户；
        /// </summary>
        public virtual int UserType { get; set; }
    }

    /// <summary>
    /// 用户余额历史
    /// </summary>
    public class UserBalanceHistory
    {
        public virtual long Id { get; set; }
        public virtual string SaveDateTime { get; set; }
        public virtual string UserId { get; set; }
        /// <summary>
        /// 充值账户余额
        /// </summary>
        public virtual decimal FillMoneyBalance { get; set; }
        /// <summary>
        /// 奖金账户，中奖后返到此账户，可提现
        /// </summary>
        public virtual decimal BonusBalance { get; set; }
        /// <summary>
        /// 佣金账户，为代理商计算佣金时，转到此账户
        /// </summary>
        public virtual decimal CommissionBalance { get; set; }
        /// <summary>
        /// 名家余额
        /// </summary>
        public virtual decimal ExpertsBalance { get; set; }
        /// <summary>
        /// 冻结账户，提现、追号、异常手工冻结
        /// </summary>
        public virtual decimal FreezeBalance { get; set; }
        /// <summary>
        /// 红包余额
        /// </summary>
        public virtual decimal RedBagBalance { get; set; }
        /// <summary>
        /// 成长值
        /// </summary>
        public virtual int UserGrowth { get; set; }
        /// <summary>
        /// 当前豆豆值
        /// </summary>
        public virtual int CurrentDouDou { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 用户余额报表
    /// </summary>
    public class UserBalanceReport
    {
        public virtual long Id { get; set; }
        public virtual string SaveDateTime { get; set; }
        /// <summary>
        /// 充值账户余额
        /// </summary>
        public virtual decimal TotalFillMoneyBalance { get; set; }
        /// <summary>
        /// 奖金账户，中奖后返到此账户，可提现
        /// </summary>
        public virtual decimal TotalBonusBalance { get; set; }
        /// <summary>
        /// 佣金账户，为代理商计算佣金时，转到此账户
        /// </summary>
        public virtual decimal TotalCommissionBalance { get; set; }
        /// <summary>
        /// 名家余额
        /// </summary>
        public virtual decimal TotalExpertsBalance { get; set; }
        /// <summary>
        /// 冻结账户，提现、追号、异常手工冻结
        /// </summary>
        public virtual decimal TotalFreezeBalance { get; set; }
        /// <summary>
        /// 红包余额
        /// </summary>
        public virtual decimal TotalRedBagBalance { get; set; }
        /// <summary>
        /// 成长值
        /// </summary>
        public virtual int TotalUserGrowth { get; set; }
        /// <summary>
        /// 当前豆豆值
        /// </summary>
        public virtual int TotalDouDou { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
