using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.CoreModel
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class CoreConfigInfo
    {
        public int Id { get; set; }
        public string ConfigKey { get; set; }
        public string ConfigName { get; set; }
        public string ConfigValue { get; set; }
        public DateTime CreateTime { get; set; }
    }



    public class CoreConfigInfoCollection : List<CoreConfigInfo>
    {
    }

    /// <summary>
    /// 竞彩足球禁赛配置
    /// </summary>
    public class DisableMatchConfigInfo
    {
        /// <summary>
        /// 比赛唯一编号
        /// </summary>
        public string MatchId { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 奖期
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public DateTime MatchStartTime { get; set; }
        /// <summary>
        /// 用英文输入法的:【逗号】如’,’分开。
        /// 竞彩足球：1:胜平负单关 2:比分单关 3:进球数单关 4:半全场单关 5:胜平负过关 6:比分过关 7:进球数过关 8:半全场过关9：不让球胜平负单关 10：不让球胜平负过关
        /// 竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
        /// </summary>
        public string PrivilegesType { get; set; }
    }


    public class DisableMatchConfigInfoCollection : List<DisableMatchConfigInfo>
    {
    }
}
