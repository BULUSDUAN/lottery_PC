using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activity.Domain.Entities
{
    /// <summary>
    /// 网站各类活动配置
    /// </summary>
    public class ActivityConfig
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 配置key
        /// </summary>
        public virtual string ConfigKey { get; set; }
        /// <summary>
        /// 配置名字
        /// </summary>
        public virtual string ConfigName { get; set; }
        /// <summary>
        /// 配置 值
        /// </summary>
        public virtual string ConfigValue { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    public class A20150919_注册送红包
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 赠送红包金额
        /// </summary>
        public virtual decimal GiveRedBagMoney { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    public class A20150919_已绑定身份和手机的用户登录送红包
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 登录日期
        /// </summary>
        public virtual string LoginDate { get; set; }
        /// <summary>
        /// 赠送红包金额
        /// </summary>
        public virtual decimal GiveRedBagMoney { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 注册绑定送红包
    /// </summary>
    public class A20150919_注册绑定送红包
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 是否已绑定银行卡
        /// </summary>
        public virtual bool IsBindBankCard { get; set; }
        /// <summary>
        /// 是否已实名认证
        /// </summary>
        public virtual bool IsBindRealName { get; set; }
        /// <summary>
        /// 是否已绑定手机号
        /// </summary>
        public virtual bool IsBindMobile { get; set; }
        /// <summary>
        /// 是否已赠送红包
        /// </summary>
        public virtual bool IsGiveRedBag { get; set; }
        /// <summary>
        /// 赠送红包金额
        /// </summary>
        public virtual decimal GiveRedBagMoney { get; set; }
        /// <summary>
        /// 是否赠送现金
        /// </summary>
        public virtual bool IsBonus { get; set; }
        /// <summary>
        /// 增送现金金额
        /// </summary>
        public virtual decimal GiveBonusMoney { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    public class A20150919_充值送红包配置
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 充值金额
        /// </summary>
        public virtual decimal FillMoney { get; set; }
        /// <summary>
        /// 赠送金额
        /// </summary>
        public virtual decimal GiveMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class A20150919_充值送红包记录
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 网银快捷充值送红包不设上限，用来区分每月上限红包
        /// PayType=1 充值赠送  PayType=2 充值类型赠送
        /// </summary>
        public virtual int PayType { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public virtual string OrderId { get; set; }
        /// <summary>
        /// 赠送月份
        /// </summary>
        public virtual string GiveMonth { get; set; }
        /// <summary>
        /// 充值金额
        /// </summary>
        public virtual decimal FillMoney { get; set; }
        /// <summary>
        /// 赠送金额
        /// </summary>
        public virtual decimal GiveMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class A20150919_加奖配置
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 排序索引
        /// </summary>
        public virtual int OrderIndex { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 玩法，如果全部则为 ALL
        /// </summary>
        public virtual string GameType { get; set; }
        /// <summary>
        /// 过关方式  全部：ALL,过关:M_1,单关:1_1
        /// </summary>
        public virtual string PlayType { get; set; }
        /// <summary>
        /// 加奖百分比
        /// </summary>
        public virtual decimal AddBonusMoneyPercent { get; set; }
        /// <summary>
        /// 最大加奖金额
        /// </summary>
        public virtual decimal MaxAddBonusMoney { get; set; }
        /// <summary>
        /// 加奖方式
        /// </summary>
        public virtual string AddMoneyWay { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class A20150919_加奖赠送记录
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public virtual string OrderId { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 玩法，如果全部则为 ALL
        /// </summary>
        public virtual string GameType { get; set; }
        /// <summary>
        /// 过关方式  全部：ALL,过关:PM_1,单关:P1_1
        /// </summary>
        public virtual string PlayType { get; set; }
        /// <summary>
        /// 中奖金额
        /// </summary>
        public virtual decimal BonusMoney { get; set; }
        /// <summary>
        /// 加奖金额
        /// </summary>
        public virtual decimal AddBonusMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class A20150919_红包使用配置
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 使用百分比
        /// </summary>
        public virtual decimal UsePercent { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class A20150919_列表用户不加奖
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public virtual string GameType { get; set; }
        /// <summary>
        /// 串关方式
        /// </summary>
        public virtual string PlayType { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
