using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace External.Domain.Entities.Agent
{
    //public class AgentUsers
    //{
    //    public virtual string AgentId { get; set; }
    //    public virtual int AgentLevel { get; set; }
    //    public virtual string DisplayName { get; set; }
    //    public virtual DateTime CreateTime { get; set; }
    //    public virtual int IsEnable { get; set; }
    //    public virtual string PAgentId { get; set; }
    //}

    public class AgentReturnPoint
    {
        public virtual int ID { get; set; }
        public virtual string AgentIdFrom { get; set; }
        public virtual string AgentIdTo { get; set; }
        public virtual int SetLevel { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual decimal ReturnPoint { get; set; }
    }

    public class AgentReturnPointReality
    {
        public virtual int ID { get; set; }
        public virtual string UserId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual decimal? MyPoint { get; set; }
        public virtual decimal? LowerPoint { get; set; }
        public virtual DateTime? LowerUpTime { get; set; }
        public virtual DateTime? MyUpTime { get; set; }
    }

    public class AgentCommissionApply
    {
        public virtual string ID { get; set; }
        public virtual DateTime RequestTime { get; set; }
        public virtual DateTime FromTime { get; set; }
        public virtual DateTime ToTime { get; set; }
        public virtual string RequestUserId { get; set; }
        public virtual decimal RequestCommission { get; set; }
        public virtual decimal? ResponseCommission { get; set; }
        public virtual string ResponseUserId { get; set; }
        public virtual decimal DealSale { get; set; }
        public virtual DateTime? ResponseTime { get; set; }
        public virtual int ApplyState { get; set; }
        public virtual string Remark { get; set; }
    }

    public class AgentCommissionDetail
    {
        public virtual int ID { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual int ApplyState { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string PAgentId { get; set; }
        public virtual string UserId { get; set; }
        public virtual int Category { get; set; }
        public virtual decimal Sale { get; set; }
        public virtual decimal? InitialPoint { get; set; }
        public virtual decimal? LowerPoint { get; set; }
        public virtual decimal? ActualPoint { get; set; }
        public virtual decimal? Deduction { get; set; }
        public virtual decimal BeforeCommission { get; set; }
        public virtual decimal ActualCommission { get; set; }
        public virtual string Remark { get; set; }
        public virtual string DetailKeyword { get; set; }
        public virtual DateTime ComplateDateTime { get; set; }
    }

    //public class AgentApplyClose
    //{
    //    public virtual int ID { get; set; }
    //    public virtual string ApplyId { get; set; }
    //    public virtual string GameCode { get; set; }
    //    public virtual string GameType { get; set; }
    //    public virtual decimal Sale { get; set; }
    //    public virtual decimal Commission { get; set; }
    //}

    public class RebateDetail
    {
        public virtual int ID { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual string UserId { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual SchemeType SchemeType { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual decimal Point { get; set; }
        public virtual decimal PayinMoney { get; set; }
        public virtual string Remark { get; set; }
    }

    public class AgentCaculateHistory
    {
        public virtual long ID { get; set; }
        public virtual DateTime CaculateTimeFrom { get; set; }
        public virtual DateTime CaculateTimeTo { get; set; }
        public virtual int TotalAgentCount { get; set; }
        public virtual int TotalOrderCount { get; set; }
        public virtual decimal TotalOrderMoney { get; set; }
        public virtual decimal TotalBuyMoney { get; set; }
        public virtual decimal TotalCommisionMoney { get; set; }
        public virtual int ErrorCount { get; set; }
        public virtual decimal ErrorOrderMoney { get; set; }
        public virtual decimal ErrorBuyMoney { get; set; }
        public virtual string ErrorSchemeIdList { get; set; }
        public virtual long MillisecondSpan { get; set; }
        public virtual string CreateBy { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

}
