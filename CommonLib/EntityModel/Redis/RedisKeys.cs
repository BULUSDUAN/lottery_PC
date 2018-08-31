using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.Redis
{
    /// <summary>
    /// 所有Key值
    /// </summary>
    public class RedisKeys
    {
        /// <summary>
        /// 可投注的比赛
        /// </summary>
        public const string Key_Running_Match_List = "Running_Match_List";
        /// <summary>
        /// 比赛结果
        /// </summary>
        public const string Key_MatchResult_List = "MatchResult_List";
        /// <summary>
        /// 数字彩奖池
        /// </summary>
        public const string Key_SZC_BonusPool = "SZC_BonusPool";
        /// <summary>
        /// 当前奖期信息
        /// </summary>
        public const string Key_CurrentIssuse = "CurrentIssuse";
        /// <summary>
        /// 未来奖期列表
        /// </summary>
        public const string Key_NextIssuse_List = "NextIssuse_List";
        /// <summary>
        /// 用户信息绑定
        /// </summary>
        public const string Key_UserBind = "UserBind";
        /// <summary>
        /// 红包使用配置
        /// </summary>
        public const string Key_RedBagUseConfig = "RedBagUseConfig";
        /// <summary>
        /// 系统配置
        /// </summary>
        public const string Key_CoreConfig = "CoreConfig";
        /// <summary>
        /// 所有彩种开启禁用状态
        /// </summary>
        public const string Key_AllGameCode = "AllGameCode";
        /// <summary>
        /// 合买大厅 订单数据
        /// </summary>
        public const string Key_Core_Togegher_OrderList = "Core_Togegher_OrderList";
        /// <summary>
        /// 合买大厅 红人（超级发起人）
        /// </summary>
        public const string Key_Core_Togegher_SupperUser = "Core_Togegher_SupperUser";
        /// <summary>
        /// 过关统计
        /// </summary>
        public const string Key_Core_GGTJ_List = "Core_GGTJ_List";

        public const string KaiJiang_Key = "KaiJiang_RedisKey";

        /// <summary>
        /// 彩种未结算的订单（已出票的订单）
        /// </summary>
        public const string Key_Running_Order_List = "Running_Order_List";

        /// <summary>
        /// 等待出票的订单(投注后未拆票的订单)
        /// </summary>
        public const string Key_Waiting_Order_List = "Waiting_Order_List";

        /// <summary>
        /// 等待出票的追号订单
        /// </summary>
        public const string Key_Waiting_Chase_Order_List = "Waiting_Chase_Order_List";
        /// <summary>
        /// 进行中的追号订单
        /// </summary>
        public const string Key_Running_Chase_Order_List = "Running_Chase_Order_List";

        /// <summary>
        /// 暂时不能出票的订单（半夜投注的订单）
        /// </summary>
        public const string Key_CanNotTicket_Order_List = "CanNotTicket_Order_List";


        /// <summary>
        /// 数字彩普通订单投注消息队列
        /// </summary>
        public const string Key_Lottery_OrderBet_List = "Lottery_OrderBet_List";
        /// <summary>
        /// 足彩普通订单投注消息队列
        /// </summary>
        public const string Key_Sports_OrderBet_List = "Sports_OrderBet_List";
        /// <summary>
        /// 单式上传订单投注消息队列
        /// </summary>
        public const string Key_SingleScheme_OrderBet_List = "SingleScheme_OrderBet_List";
        /// <summary>
        /// 优化订单投注消息队列
        /// </summary>
        public const string Key_YouHua_OrderBet_List = "YouHua_OrderBet_List";
        /// <summary>
        /// 足彩合买订单投注消息队列
        /// </summary>
        public const string Key_Together_Sports_OrderBet_List = "Together_Sports_OrderBet_List";
        /// <summary>
        /// 优化合买订单投注消息队列
        /// </summary>
        public const string Key_Together_YouHua_OrderBet_List = "Together_YouHua_OrderBet_List";
        /// <summary>
        /// 单式合买订单投注消息队列
        /// </summary>
        public const string Key_Together_SingleScheme_OrderBet_List = "Together_SingleScheme_OrderBet_List";
        /// <summary>
        /// 竞猜足球
        /// </summary>
        public const string Key_JCZQ_Match_Odds_List = "JCZQ_Match_Odds_List";
        /// <summary>
        /// 传统足球期号
        /// </summary>
        public const string Key_CTZQ_Issuse_List = "CTZQ_Issuse_List";
        /// <summary>
        /// 传统足球
        /// </summary>
        public const string Key_CTZQ_Match_Odds_List = "CTZQ_Match_Odds_List";
        /// <summary>
        /// 北京单场
        /// </summary>
        public const string Key_BJDC_Match_Odds_List = "BJDC_Match_Odds_List";
        /// <summary>
        /// 竞猜篮球
        /// </summary>
        public const string Key_JCLQ_Match_Odds_List = "JCLQ_Match_Odds_List";

        public const string ArticleListKey = "APP_ArticleList"; 

    }
}
