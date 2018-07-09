using EntityModel.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.CoreModel
{
    /// <summary>
    /// 用户注册信息
    /// </summary>
    public class UserRegInfo
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 注册来源
        /// </summary>
        public string ComeFrom { get; set; }
        /// <summary>
        /// 注册IP
        /// </summary>
        public string RegisterIp { get; set; }
        /// <summary>
        /// 注册引用页面
        /// </summary>
        public string ReferrerUrl { get; set; }
        /// <summary>
        /// 注册引用
        /// </summary>
        public string Referrer { get; set; }
        /// <summary>
        /// 注册类型
        /// </summary>
        public string RegType { get; set; }
        /// <summary>
        /// 代理商
        /// </summary>
        public string AgentId { get; set; }
    }
    /// <summary>
    /// 用户余额
    /// </summary>
    [ProtoContract]
    [Serializable]
    public class UserBalanceInfo
    {
        [ProtoMember(1)]
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        [ProtoMember(2)]
        /// <summary>
        /// 充值账户余额，充值充到此账户
        /// </summary>
        public decimal FillMoneyBalance { get; set; }
        /// <summary>
        /// 奖金账户，中奖后返到此账户，可提现
        /// </summary>
         [ProtoMember(3)]
        public decimal BonusBalance { get; set; }
        /// <summary>
        /// 冻结账户，提现、追号、异常手工冻结
        /// </summary>
        [ProtoMember(4)]
        public decimal FreezeBalance { get; set; }
        /// <summary>
        /// 佣金账户，为代理商计算佣金时，转到此账户
        /// </summary
        [ProtoMember(5)]
        public decimal CommissionBalance { get; set; }
        /// <summary>
        /// 名家余额
        /// </summary>
        [ProtoMember(6)]
        public decimal ExpertsBalance { get; set; }
        /// <summary>
        /// 红包余额
        /// </summary>
        [ProtoMember(7)]
        public decimal RedBagBalance { get; set; }
        /// <summary>
        /// CPS余额
        /// </summary
        [ProtoMember(8)]
        public decimal CPSBalance { get; set; }


        /// <summary>
        /// 成长值
        /// </summary>
        [ProtoMember(9)]
        public int UserGrowth { get; set; }
        /// <summary>
        /// 当前豆豆值
        /// </summary>
        [ProtoMember(10)]
        public int CurrentDouDou { get; set; }

        [ProtoMember(11)]
        public bool IsSetPwd { get; set; }
        /// <summary>
        /// 需要输入资金密码的地方
        /// </summary>
        [ProtoMember(12)]
        public string NeedPwdPlace { get; set; }
        [ProtoMember(13)]
        public string BalancePwd { get; set; }

        /// <summary>
        /// 获取现金金额
        /// </summary>
        public decimal GetTotalCashMoney()
        {
            return this.FillMoneyBalance + this.BonusBalance + this.CommissionBalance + this.ExpertsBalance;
            //return this.FillMoneyBalance + this.BonusBalance + this.ExpertsBalance;
        }

        private static string[] _allPlaceList = new string[] {
                "Bet"               // 投注
                , "Withdraw"        // 提现
                , "Transfer"        // 转账
                , "Red"             // 送红包
                , "CancelWithdraw"  // 取消提现
                , "CancelChase"     // 取消追号
                ,"BuyExperter"      //购买名家分析方案
                ,"ExchangeDouDou"   //兑换豆豆
            };
        public bool CheckIsNeedPassword(string placeName)
        {
            if (!_allPlaceList.Contains(placeName))
            {
                throw new ArgumentException("不支持资金密码的设置 - " + placeName);
            }
            if (IsSetPwd)
            {
                if (string.IsNullOrEmpty(NeedPwdPlace))
                {
                    return false;
                }
                if (NeedPwdPlace == "ALL" || NeedPwdPlace.Split('|', ',').Contains(placeName))
                {
                    return true;
                }
            }
            return false;
        }
    }
    /// <summary>
    /// 用户资金冻结明细
    /// </summary>
    public class UserBalanceFreezeInfo
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public FrozenCategory Category { get; set; }
        public decimal FreezeMoney { get; set; }
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
    }
    public class UserBalanceFreezeCollection
    {
        public int TotalCount { get; set; }
        public decimal TotalMoney { get; set; }
        public IList<UserBalanceFreezeInfo> FreezeList { get; set; }
    }

    /// <summary>
    /// 用户余额历史对象
    /// </summary>
    public class UserBalanceHistoryInfo
    {
        public long Id { get; set; }
        public string SaveDateTime { get; set; }
        public string UserId { get; set; }
        /// <summary>
        /// 充值账户余额
        /// </summary>
        public decimal FillMoneyBalance { get; set; }
        /// <summary>
        /// 奖金账户，中奖后返到此账户，可提现
        /// </summary>
        public decimal BonusBalance { get; set; }
        /// <summary>
        /// 佣金账户，为代理商计算佣金时，转到此账户
        /// </summary>
        public decimal CommissionBalance { get; set; }
        /// <summary>
        /// 名家余额
        /// </summary>
        public decimal ExpertsBalance { get; set; }
        /// <summary>
        /// 冻结账户，提现、追号、异常手工冻结
        /// </summary>
        public decimal FreezeBalance { get; set; }
        /// <summary>
        /// 红包余额
        /// </summary>
        public decimal RedBagBalance { get; set; }
        /// <summary>
        /// 成长值
        /// </summary>
        public int UserGrowth { get; set; }
        /// <summary>
        /// 当前豆豆值
        /// </summary>
        public int CurrentDouDou { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    public class UserBalanceHistoryInfoCollection
    {
        public UserBalanceHistoryInfoCollection()
        {
            InfoList = new List<UserBalanceHistoryInfo>();
        }
        public int TotalCount { get; set; }
        /// <summary>
        /// 总的充值金额
        /// </summary>
        public decimal TotalFillMoneyBalance { get; set; }
        /// <summary>
        /// 总的奖金金额
        /// </summary>
        public decimal TotalBonusBalance { get; set; }
        /// <summary>
        /// 总的返点金额
        /// </summary>
        public decimal TotalCommissionBalance { get; set; }
        /// <summary>
        /// 总的名家金额
        /// </summary>
        public decimal TotalExpertsBalance { get; set; }
        /// <summary>
        /// 总的冻结金额
        /// </summary>
        public decimal TotalFreezeBalance { get; set; }
        /// <summary>
        /// 总的红包金额
        /// </summary>
        public decimal TotalRedBagBalance { get; set; }
        /// <summary>
        /// 总的成长值
        /// </summary>
        public int TotalUserGrowth { get; set; }
        /// <summary>
        /// 总的豆豆
        /// </summary>
        public int TotalDouDou { get; set; }
        public List<UserBalanceHistoryInfo> InfoList { get; set; }
    }

    /// <summary>
    /// 用户余额报表对象
    /// </summary>
    public class UserBalanceReportInfo
    {
        public long Id { get; set; }
        public string SaveDateTime { get; set; }
        /// <summary>
        /// 充值账户余额
        /// </summary>
        public decimal TotalFillMoneyBalance { get; set; }
        /// <summary>
        /// 奖金账户，中奖后返到此账户，可提现
        /// </summary>
        public decimal TotalBonusBalance { get; set; }
        /// <summary>
        /// 佣金账户，为代理商计算佣金时，转到此账户
        /// </summary>
        public decimal TotalCommissionBalance { get; set; }
        /// <summary>
        /// 名家余额
        /// </summary>
        public decimal TotalExpertsBalance { get; set; }
        /// <summary>
        /// 冻结账户，提现、追号、异常手工冻结
        /// </summary>
        public decimal TotalFreezeBalance { get; set; }
        /// <summary>
        /// 红包余额
        /// </summary>
        public decimal TotalRedBagBalance { get; set; }
        /// <summary>
        /// 成长值
        /// </summary>
        public int TotalUserGrowth { get; set; }
        /// <summary>
        /// 当前豆豆值
        /// </summary>
        public int TotalDouDou { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    public class UserBalanceReportInfoCollection
    {
        public UserBalanceReportInfoCollection()
        {
            InfoList = new List<UserBalanceReportInfo>();
        }

        public int TotalCount { get; set; }
        /// <summary>
        /// 总的充值金额
        /// </summary>
        public decimal SumFillMoneyBalance { get; set; }
        /// <summary>
        /// 总的奖金金额
        /// </summary>
        public decimal SumBonusBalance { get; set; }
        /// <summary>
        /// 总的返点金额
        /// </summary>
        public decimal SumCommissionBalance { get; set; }
        /// <summary>
        /// 总的名家金额
        /// </summary>
        public decimal SumExpertsBalance { get; set; }
        /// <summary>
        /// 总的冻结金额
        /// </summary>
        public decimal SumFreezeBalance { get; set; }
        /// <summary>
        /// 总的红包金额
        /// </summary>
        public decimal SumRedBagBalance { get; set; }
        /// <summary>
        /// 总的成长值
        /// </summary>
        public int SumUserGrowth { get; set; }
        /// <summary>
        /// 总的豆豆
        /// </summary>
        public int SumDouDou { get; set; }
        public List<UserBalanceReportInfo> InfoList { get; set; }
    }


    public class NewKPIDetailInfoCollection
    {
        public NewKPIDetailInfoCollection()
        {
            InfoList = new List<NewKPIDetailInfo>();
        }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 总登录户数
        /// </summary>
        public int SumLoginCount { get; set; }
        /// <summary>
        /// 总开户数
        /// </summary>
        public int SumOpenCoutn { get; set; }
        /// <summary>
        /// 总总下注户数
        /// </summary>
        public int SumPalyGameCount { get; set; }
        /// <summary>
        /// 总总提现户数
        /// </summary>
        public int SumWinthdrawCount { get; set; }
        /// <summary>
        /// 总总充值户数5
        /// </summary>
        public int SumRechargeCount { get; set; }
        /// <summary>
        /// 总总活跃户数6
        /// </summary>
        public int SumActiveCount { get; set; }
        /// <summary>
        /// 总首投户数7
        /// </summary>
        public int SumFristPalyGameCount { get; set; }
        /// <summary>
        /// 总首提现户数8
        /// </summary>
        public int SumFristWinthdrawCount { get; set; }
        /// <summary>
        /// 总首充值户数9
        /// </summary>
        public int SumFristRechargeCount { get; set; }

        public IList<NewKPIDetailInfo> InfoList { get; set; }
    }

    /// <summary>
    /// 用户余额报表对象
    /// </summary>
    public class NewKPIDetailInfo
    {
        public long Id { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 登录户数
        /// </summary>
        public int LoginCount { get; set; }
        /// <summary>
        /// 开户数
        /// </summary>
        public int OpenCoutn { get; set; }
        /// <summary>
        /// 总下注户数
        /// </summary>
        public int PalyGameCount { get; set; }
        /// <summary>
        /// 总提现户数
        /// </summary>
        public int WinthdrawCount { get; set; }
        /// <summary>
        /// 总充值户数5
        /// </summary>
        public int RechargeCount { get; set; }
        /// <summary>
        /// 总活跃户数6
        /// </summary>
        public int ActiveCount { get; set; }
        /// <summary>
        /// 首投户数7
        /// </summary>
        public int FristPalyGameCount { get; set; }
        /// <summary>
        /// 首提现户数8
        /// </summary>
        public int FristWinthdrawCount { get; set; }
        /// <summary>
        /// 首充值户数
        /// </summary>
        public int FristRechargeCount { get; set; }

    }



    public class NewSummaryReportInfoCollection
    {
        public NewSummaryReportInfoCollection()
        {
            InfoList = new List<NewSummaryReportInfo>();
        }


        public IList<NewSummaryReportInfo> InfoList { get; set; }
    }

    /// <summary>
    /// 资金汇总报表对象
    /// </summary>
    public class NewSummaryReportInfo
    {
        public long Id { get; set; }
        /// <summary>
        /// 帐变类型
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 账户类型
        /// </summary>
        public int accounttype { get; set; }
        /// <summary>
        /// 加减操作
        /// </summary>
        public int paytype { get; set; }
        /// <summary>
        /// 金额汇总
        /// </summary>
        public decimal PayMoney { get; set; }


    }
}
