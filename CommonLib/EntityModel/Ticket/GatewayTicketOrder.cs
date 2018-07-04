using EntityModel.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EntityModel.Ticket
{
    /// <summary>
    /// 出票定单
    /// </summary>
    
    public class GatewayTicketOrder
    {
        public GatewayTicketOrder()
        {
            Amount = 1;
            AnteCodeList = new GatewayAnteCodeCollection();
        }
        /// <summary>
        /// 代理商生成的订单号，该号码可用于票查询，不能重复。
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 彩票期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 投注总金额 （包含多倍多期的总金额）
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 投注号码
        /// </summary>
        public GatewayAnteCodeCollection AnteCodeList { get; set; }
        /// <summary>
        /// 投注倍数
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 附加信息 （不能包含特殊符号）
        /// </summary>
        public string Attach { get; set; }
        /// <summary>
        /// 单注投注单价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 是否虚拟订单（不用出票到外部）
        /// </summary>
        public bool IsVirtualOrder { get; set; }
        /// <summary>
        /// 是否追加(大乐透)
        /// </summary>
        public bool IsAppend { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 是否生成投注中的票
        /// </summary>
        public bool IsRunningTicket { get; set; }

        public override string ToString()
        {
            return    JsonConvert.SerializeObject(this);// Json Common.JSON.JsonSerializer.Serialize(this);
        }
    }

    /// <summary>
    /// 出票定单 - 北京单场
    /// </summary>
  //  [CommunicationObject]
    public class GatewayTicketOrder_Sport
    {
        public GatewayTicketOrder_Sport()
        {
            Amount = 1;
            AnteCodeList = new GatewayAnteCodeCollection_Sport();
        }
        /// <summary>
        /// 代理商生成的订单号，该号码可用于票查询，不能重复。
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 彩票期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 串关方式。5_1|6_1
        /// </summary>
        public string PlayType { get; set; }
        /// <summary>
        /// 投注总金额 （包含多倍多期的总金额）
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 投注号码
        /// </summary>
        public GatewayAnteCodeCollection_Sport AnteCodeList { get; set; }
        /// <summary>
        /// 投注倍数
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 附加信息 （不能包含特殊符号）
        /// </summary>
        public string Attach { get; set; }
        /// <summary>
        /// 单注投注单价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 是否虚拟订单（不用出票到外部）
        /// </summary>
        public bool IsVirtualOrder { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 是否生成投注中的票
        /// </summary>
        public bool IsRunningTicket { get; set; }

        public bool CheckIsDanGuan()
        {
            return PlayType.Equals("1_1");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("出票定单 - 北京单场");
            return sb.ToString();
        }
    }


    /// <summary>
    /// 投注号码 - 用于出票
    /// </summary>
   // [CommunicationObject]
    public class GatewayAnteCode_Sport : ISportAnteCode
    {
        public string GameType { get; set; }
        public string MatchId { get; set; }
        public string AnteCode { get; set; }
        public string Odds { get { return ""; } }
        public bool IsDan { get; set; }
        public int Length { get { return AnteCode.Split(',').Length; } }

        public string GetMatchResult(string gameCode, string gameType, string score)
        {
            return "";
        }
    }

    /// <summary>
    /// 投注号码集合 - 用于出票
    /// </summary>
    //[CommunicationObject]
    public class GatewayAnteCodeCollection_Sport : List<GatewayAnteCode_Sport>
    {
    }

    /// <summary>
    /// 出票订单 单式上传
    /// </summary>
   // [CommunicationObject]
    public class GatewayTicketOrder_SingleScheme
    {
        public string OrderId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        /// <summary>
        /// 串关方式。5_1|6_1
        /// </summary>
        public string PlayType { get; set; }
        public string IssuseNumber { get; set; }
        public string SelectMatchId { get; set; }
        /// <summary>
        /// 允许投注的号，如胜平负 只能投 3 1 0
        /// </summary>
        public string AllowCodes { get; set; }
        /// <summary>
        /// 是否包括场次编号
        /// </summary>
        public bool ContainsMatchId { get; set; }
        /// <summary>
        /// 是否虚拟订单（不用出票到外部）
        /// </summary>
        public bool IsVirtualOrder { get; set; }
        public int Amount { get; set; }
        public decimal TotalMoney { get; set; }
        public byte[] FileBuffer { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 是否生成投注中的票
        /// </summary>
        public bool IsRunningTicket { get; set; }

    }

    /// <summary>
    /// 投注号码 - 用于出票
    /// </summary>
   // [CommunicationObject]
    public class GatewayAnteCode
    {
        /// <summary>
        /// 游戏类型
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 号码
        /// </summary>
        public string AnteNumber { get; set; }
    }
    /// <summary>
    /// 投注号码集合 - 用于出票
    /// </summary>
   // [CommunicationObject]
    public class GatewayAnteCodeCollection : List<GatewayAnteCode>
    {
        /// <summary>
        /// 添加一个号码
        /// </summary>
        public void AddAnteCode(string gameType, string anteCode)
        {
            this.Add(new GatewayAnteCode()
            {
                GameType = gameType,
                AnteNumber = anteCode
            });
        }
    }
}
