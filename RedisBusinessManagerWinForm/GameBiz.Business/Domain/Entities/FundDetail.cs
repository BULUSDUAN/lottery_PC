using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using Common;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 资金明细
    /// </summary>
    public class FundDetail
    {
        /// <summary>
        /// 自增编号
        /// </summary>
        public virtual long Id { get; set; }
        /// <summary>
        /// 线索编号
        /// </summary>
        public virtual string KeyLine { get; set; }
        /// <summary>
        /// 内部订单号
        /// SchemeId|IssuseNumber 格式
        /// </summary>
        public virtual string OrderId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 收支类型
        /// </summary>
        public virtual PayType PayType { get; set; }
        /// <summary>
        /// 账户类型
        /// </summary>
        public virtual AccountType AccountType { get; set; }
        /// <summary>
        /// 分类编号
        /// </summary>
        public virtual string Category { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Summary { get; set; }
        /// <summary>
        /// 收入金额
        /// </summary>
        public virtual decimal PayMoney { get; set; }
        /// <summary>
        /// 交易前余额
        /// </summary>
        public virtual decimal BeforeBalance { get; set; }
        /// <summary>
        /// 交易后余额
        /// </summary>
        public virtual decimal AfterBalance { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 代理商
        /// </summary>
        public virtual string AgentId { get; set; }
        /// <summary>
        /// 操作员Id
        /// </summary>
        public virtual string OperatorId { get; set; }
    }

    /// <summary>
    /// 澳彩豆豆明细
    /// </summary>
    public class OCDouDouDetail
    {
        /// <summary>
        /// 自增编号
        /// </summary>
        public virtual long Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public virtual string OrderId { get; set; }
        /// <summary>
        /// 收支类型
        /// </summary>
        public virtual PayType PayType { get; set; }
        /// <summary>
        /// 分类编号
        /// </summary>
        public virtual string Category { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Summary { get; set; }
        /// <summary>
        /// 收入金额
        /// </summary>
        public virtual int PayMoney { get; set; }
        /// <summary>
        /// 交易前余额
        /// </summary>
        public virtual int BeforeBalance { get; set; }
        /// <summary>
        /// 交易后余额
        /// </summary>
        public virtual int AfterBalance { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public virtual DateTime CreateTime { get; set; }

    }

    /// <summary>
    /// 用户成长值明细
    /// </summary>
    public class UserGrowthDetail
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string OrderId { get; set; }
        /// <summary>
        /// 收支类型
        /// </summary>
        public virtual PayType PayType { get; set; }
        /// <summary>
        /// 分类编号
        /// </summary>
        public virtual string Category { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Summary { get; set; }
        /// <summary>
        /// 收入金额
        /// </summary>
        public virtual int PayMoney { get; set; }
        /// <summary>
        /// 交易前余额
        /// </summary>
        public virtual int BeforeBalance { get; set; }
        /// <summary>
        /// 交易后余额
        /// </summary>
        public virtual int AfterBalance { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
