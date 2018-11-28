using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection
{
     
    public class KJLocalIssuse_AddInfo
    {
        public string GameCode { get; set; }
        public string IssuseNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime BettingStopTime { get; set; }
        public DateTime OfficialStopTime { get; set; }
    }
     
    public class KJLocalIssuse_AddInfoCollection : List<KJLocalIssuse_AddInfo>
    {
    }

     
    public class KJGameInfo
    {
        public string GameCode { get; set; }
        public string DisplayName { get; set; }
    }
     
    public class KJGameInfoCollection : List<KJGameInfo>
    {
    }

     
    public class KJIssuse_QueryInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string GameCode_IssuseNumber { get; set; }
        /// <summary>
        /// 游戏名称
        /// </summary>
        public KJGameInfo Game { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 开启时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 本地截至投注时间
        /// </summary>
        public DateTime LocalStopTime { get; set; }
        /// <summary>
        /// 外部接口截至投注时间
        /// </summary>
        public DateTime GatewayStopTime { get; set; }
        /// <summary>
        /// 官方截至时间
        /// </summary>
        public DateTime OfficialStopTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public IssuseStatus Status { get; set; }
        /// <summary>
        /// 中奖号码
        /// </summary>
        public string WinNumber { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
