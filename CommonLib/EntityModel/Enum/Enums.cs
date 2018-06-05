using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Enum
{  
    /// <summary>
    /// 投注方案类别
    /// </summary>
    public enum SchemeType
    {
        /// <summary>
        /// 普通投注
        /// </summary>
        GeneralBetting = 1,
        /// <summary>
        /// 追号投注
        /// </summary>
        ChaseBetting = 2,
        /// <summary>
        /// 合买投注
        /// </summary>
        TogetherBetting = 3,
        /// <summary>
        /// 专家方案
        /// </summary>
        ExperterScheme = 4,
        /// <summary>
        /// 保存的订单
        /// </summary>
        SaveScheme = 5,
        /// <summary>
        /// 宝单
        /// </summary>
        SingleTreasure = 6,
        /// <summary>
        /// 抄单
        /// </summary>
        SingleCopy = 7,
    }
    /// <summary>
    /// 出票状态
    /// </summary>
    public enum TicketStatus
    {
        /// <summary>
        /// 等待投注
        /// </summary>
        Waitting = 0,
        /// <summary>
        /// 投注中
        /// </summary>
        Ticketing = 10,
        /// <summary>
        /// 已打票
        /// </summary>
        PrintTicket = 20,
        /// <summary>
        /// 被终止
        /// </summary>
        Abort = 50,
        /// <summary>
        /// 被跳过
        /// </summary>
        Skipped = 70,
        /// <summary>
        /// 已投注完成
        /// </summary>
        Ticketed = 90,
        /// <summary>
        /// 有错误发生
        /// </summary>
        Error = 99,
    }

    /// <summary>
    /// 进行状态
    /// </summary>
    public enum ProgressStatus
    {
        /// <summary>
        /// 等待中
        /// </summary>
        Waitting = 0,
        /// <summary>
        /// 进行中
        /// </summary>
        Running = 10,
        /// <summary>
        /// 自动停止
        /// </summary>
        AutoStop = 20,
        /// <summary>
        /// 被中断
        /// </summary>
        Aborted = 80,
        /// <summary>
        /// 已完成
        /// </summary>
        Complate = 90,
    }

    /// <summary>
    /// 中奖状态
    /// </summary>
    public enum BonusStatus
    {
        /// <summary>
        /// 未开奖
        /// </summary>
        Waitting = 0,
        /// <summary>
        /// 正在计算派奖
        /// </summary>
        Awarding = 10,
        /// <summary>
        /// 已中奖
        /// </summary>
        Win = 20,
        /// <summary>
        /// 未中奖
        /// </summary>
        Lose = 30,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 99,
    }
}
