using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection
{
    /// <summary>
    /// 双色球基本走势
    /// </summary>
     
    public class SSQ_JiBenZouSi_Info : ImportInfoBase
    {
        public int Red01 { get; set; }
        public int Red02 { get; set; }
        public int Red03 { get; set; }
        public int Red04 { get; set; }
        public int Red05 { get; set; }
        public int Red06 { get; set; }
        public int Red07 { get; set; }
        public int Red08 { get; set; }
        public int Red09 { get; set; }
        public int Red10 { get; set; }
        public int Red11 { get; set; }
        public int Red12 { get; set; }
        public int Red13 { get; set; }
        public int Red14 { get; set; }
        public int Red15 { get; set; }
        public int Red16 { get; set; }
        public int Red17 { get; set; }
        public int Red18 { get; set; }
        public int Red19 { get; set; }
        public int Red20 { get; set; }
        public int Red21 { get; set; }
        public int Red22 { get; set; }
        public int Red23 { get; set; }
        public int Red24 { get; set; }
        public int Red25 { get; set; }
        public int Red26 { get; set; }
        public int Red27 { get; set; }
        public int Red28 { get; set; }
        public int Red29 { get; set; }
        public int Red30 { get; set; }
        public int Red31 { get; set; }
        public int Red32 { get; set; }
        public int Red33 { get; set; }

        public int Blue01 { get; set; }
        public int Blue02 { get; set; }
        public int Blue03 { get; set; }
        public int Blue04 { get; set; }
        public int Blue05 { get; set; }
        public int Blue06 { get; set; }
        public int Blue07 { get; set; }
        public int Blue08 { get; set; }
        public int Blue09 { get; set; }
        public int Blue10 { get; set; }
        public int Blue11 { get; set; }
        public int Blue12 { get; set; }
        public int Blue13 { get; set; }
        public int Blue14 { get; set; }
        public int Blue15 { get; set; }
        public int Blue16 { get; set; }
    }

     
    public class SSQ_JiBenZouSi_InfoCollection : List<SSQ_JiBenZouSi_Info>
    {
    }

    /// <summary>
    /// 大小走势
    /// </summary>
    
    public class SSQ_DX_Info : ImportInfoBase
    {
        /// <summary>
        /// 红球开奖号
        /// </summary>
        public string RedLotteryNumber { get; set; }

        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }
        public string RedBall4 { get; set; }
        public string RedBall5 { get; set; }
        public string RedBall6 { get; set; }


        public int O_RedBall1_D { get; set; }
        public int O_RedBall2_D { get; set; }
        public int O_RedBall3_D { get; set; }
        public int O_RedBall4_D { get; set; }
        public int O_RedBall5_D { get; set; }
        public int O_RedBall6_D { get; set; }

        public int O_RedBall1_X { get; set; }
        public int O_RedBall2_X { get; set; }
        public int O_RedBall3_X { get; set; }
        public int O_RedBall4_X { get; set; }
        public int O_RedBall5_X { get; set; }
        public int O_RedBall6_X { get; set; }
        /// <summary>
        /// 大小排位
        /// </summary>
        public string DX_Qualifying { get; set; }
        /// <summary>
        /// 大小比例
        /// </summary>
        public string DX_Proportion { get; set; }
        /// <summary>
        /// 大小比例遗漏
        /// </summary>
        public int O_DX_Proportion06 { get; set; }
        public int O_DX_Proportion15 { get; set; }
        public int O_DX_Proportion24 { get; set; }
        public int O_DX_Proportion33 { get; set; }
        public int O_DX_Proportion42 { get; set; }
        public int O_DX_Proportion51 { get; set; }
        public int O_DX_Proportion60 { get; set; }
    }

    
   public class SSQ_DX_InfoCollection : List<SSQ_DX_Info>
   {
   }

    /// <summary>
    /// 除3余数
    /// </summary>
    
   public class SSQ_C3_Info : ImportInfoBase
    {
        /// <summary>
        /// 红球开奖号
        /// </summary>
        public string RedLotteryNumber { get; set; }

        public string Red1 { get; set; }
        public string Red2 { get; set; }
        public string Red3 { get; set; }
        public string Red4 { get; set; }
        public string Red5 { get; set; }
        public string Red6 { get; set; }

        public int O_Red1_0 { get; set; }
        public int O_Red2_0 { get; set; }
        public int O_Red3_0 { get; set; }
        public int O_Red4_0 { get; set; }
        public int O_Red5_0 { get; set; }
        public int O_Red6_0 { get; set; }

        public int O_Red1_1 { get; set; }
        public int O_Red2_1 { get; set; }
        public int O_Red3_1 { get; set; }
        public int O_Red4_1 { get; set; }
        public int O_Red5_1 { get; set; }
        public int O_Red6_1 { get; set; }

        public int O_Red1_2 { get; set; }
        public int O_Red2_2 { get; set; }
        public int O_Red3_2 { get; set; }
        public int O_Red4_2 { get; set; }
        public int O_Red5_2 { get; set; }
        public int O_Red6_2 { get; set; }

        public int Y0_Number { get; set; }
        public int Y1_Number { get; set; }
        public int Y2_Number { get; set; }

        /// <summary>
        /// 余X个数
        /// </summary>
        public int Y0_Number0 { get; set; }
        public int Y0_Number1 { get; set; }
        public int Y0_Number2 { get; set; }
        public int Y0_Number3 { get; set; }
        public int Y0_Number4 { get; set; }
        public int Y0_Number5 { get; set; }
        public int Y0_Number6 { get; set; }

        public int Y1_Number0 { get; set; }
        public int Y1_Number1 { get; set; }
        public int Y1_Number2 { get; set; }
        public int Y1_Number3 { get; set; }
        public int Y1_Number4 { get; set; }
        public int Y1_Number5 { get; set; }
        public int Y1_Number6 { get; set; }

        public int Y2_Number0 { get; set; }
        public int Y2_Number1 { get; set; }
        public int Y2_Number2 { get; set; }
        public int Y2_Number3 { get; set; }
        public int Y2_Number4 { get; set; }
        public int Y2_Number5 { get; set; }
        public int Y2_Number6 { get; set; }
        /// <summary>
        /// 余数排位
        /// </summary>
        public string YS_Qualifying { get; set; }
        /// <summary>
        /// 余数比例
        /// </summary>
        public string YS_Proportion { get; set; }
    }

    
   public class SSQ_C3_InfoCollection : List<SSQ_C3_Info>
   {
   }

    /// <summary>
    /// 和值走势
    /// </summary>
    
   public class SSQ_HeZhi_Info : ImportInfoBase
    {
        /// <summary>
        /// 红球开奖号
        /// </summary>
        public string RedLotteryNumber { get; set; }
        /// <summary>
        /// 和值
        /// </summary>
        public int HeZhi { get; set; }
        public string Red1 { get; set; }
        public string Red2 { get; set; }
        public string Red3 { get; set; }
        public string Red4 { get; set; }
        public string Red5 { get; set; }
        public string Red6 { get; set; }
        /// <summary>
        /// 和尾
        /// </summary>
        public int HeWei { get; set; }
        /// <summary>
        /// 和值分布
        /// </summary>
        public int HZ_21_49 { get; set; }
        public int HZ_50_59 { get; set; }
        public int HZ_60_69 { get; set; }
        public int HZ_70_79 { get; set; }
        public int HZ_80_89 { get; set; }
        public int HZ_90_99 { get; set; }
        public int HZ_100_109 { get; set; }
        public int HZ_110_119 { get; set; }
        public int HZ_120_129 { get; set; }
        public int HZ_130_139 { get; set; }
        public int HZ_140_183 { get; set; }
        /// <summary>
        /// 和尾分布
        /// </summary> 
        public int HW_0 { get; set; }
        public int HW_1 { get; set; }
        public int HW_2 { get; set; }
        public int HW_3 { get; set; }
        public int HW_4 { get; set; }
        public int HW_5 { get; set; }
        public int HW_6 { get; set; }
        public int HW_7 { get; set; }
        public int HW_8 { get; set; }
        public int HW_9 { get; set; }
    }

    
   public class SSQ_HeZhi_InfoCollection : List<SSQ_HeZhi_Info>
   {
   }

    /// <summary>
    /// 奇偶走势
    /// </summary>
    
   public class SSQ_JiOu_Info : ImportInfoBase
    {
        /// <summary>
        /// 红球开奖号
        /// </summary>
        public string RedLotteryNumber { get; set; }

        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }
        public string RedBall4 { get; set; }
        public string RedBall5 { get; set; }
        public string RedBall6 { get; set; }


        public int O_RedBall1_J { get; set; }
        public int O_RedBall2_J { get; set; }
        public int O_RedBall3_J { get; set; }
        public int O_RedBall4_J { get; set; }
        public int O_RedBall5_J { get; set; }
        public int O_RedBall6_J { get; set; }

        public int O_RedBall1_O { get; set; }
        public int O_RedBall2_O { get; set; }
        public int O_RedBall3_O { get; set; }
        public int O_RedBall4_O { get; set; }
        public int O_RedBall5_O { get; set; }
        public int O_RedBall6_O { get; set; }
        /// <summary>
        /// 奇偶排位
        /// </summary>
        public string JO_Qualifying { get; set; }
        /// <summary>
        /// 奇偶比例
        /// </summary>
        public string JO_Proportion { get; set; }
        /// <summary>
        /// 奇偶比例遗漏
        /// </summary>
        public int O_JO_Proportion60 { get; set; }
        public int O_JO_Proportion51 { get; set; }
        public int O_JO_Proportion42 { get; set; }
        public int O_JO_Proportion33 { get; set; }
        public int O_JO_Proportion24 { get; set; }
        public int O_JO_Proportion15 { get; set; }
        public int O_JO_Proportion06 { get; set; }
    }

    
   public class SSQ_JiOu_InfoCollection : List<SSQ_JiOu_Info>
   {
   }

    /// <summary>
    /// 跨度1_6
    /// </summary>
    
   public class SSQ_KuaDu_1_6_Info : ImportInfoBase
    {
        /// <summary>
        /// 红球开奖号
        /// </summary>
        public string RedLotteryNumber { get; set; }
        /// <summary>
        /// 12跨度号
        /// </summary>
        public int KuaDu_12 { get; set; }
        /// <summary>
        /// 12跨度号尾
        /// </summary>
        public int KuaDu_W_12 { get; set; }
        /// <summary>
        /// 23跨度号
        /// </summary>
        public int KuaDu_23 { get; set; }
        /// <summary>
        /// 23跨度号尾
        /// </summary>
        public int KuaDu_W_23 { get; set; }
        /// <summary>
        /// 34跨度号
        /// </summary>
        public int KuaDu_34 { get; set; }
        /// <summary>
        /// 34跨度号尾
        /// </summary>
        public int KuaDu_W_34 { get; set; }
        /// <summary>
        /// 45跨度号
        /// </summary>
        public int KuaDu_45 { get; set; }
        /// <summary>
        /// 45跨度号尾
        /// </summary>
        public int KuaDu_W_45 { get; set; }
        /// <summary>
        /// 56跨度号
        /// </summary>
        public int KuaDu_56 { get; set; }
        /// <summary>
        /// 56跨度号尾
        /// </summary>
        public int KuaDu_W_56 { get; set; }
        /// <summary>
        /// 12跨号数
        /// </summary>
        public int KD_12_1 { get; set; }
        public int KD_12_2 { get; set; }
        public int KD_12_3 { get; set; }
        public int KD_12_4 { get; set; }
        public int KD_12_5 { get; set; }
        public int KD_12_6 { get; set; }
        public int KD_12_7 { get; set; }
        public int KD_12_8 { get; set; }
        public int KD_12_9 { get; set; }
        public int KD_12_10 { get; set; }
        public int KD_12_11 { get; set; }
        public int KD_12_12 { get; set; }
        public int KD_12_13 { get; set; }
        public int KD_12_14 { get; set; }
        public int KD_12_15 { get; set; }
        public int KD_12_16 { get; set; }
        public int KD_12_17 { get; set; }
        public int KD_12_18 { get; set; }
        public int KD_12_19 { get; set; }
        public int KD_12_20 { get; set; }
        public int KD_12_21 { get; set; }
        public int KD_12_22 { get; set; }
        public int KD_12_23 { get; set; }
        public int KD_12_24 { get; set; }
        public int KD_12_25 { get; set; }
        public int KD_12_26 { get; set; }
        public int KD_12_27 { get; set; }
        public int KD_12_28 { get; set; }

        /// <summary>
        /// 12跨度尾数
        /// </summary>
        public int KDW_12_0 { get; set; }
        public int KDW_12_1 { get; set; }
        public int KDW_12_2 { get; set; }
        public int KDW_12_3 { get; set; }
        public int KDW_12_4 { get; set; }
        public int KDW_12_5 { get; set; }
        public int KDW_12_6 { get; set; }
        public int KDW_12_7 { get; set; }
        public int KDW_12_8 { get; set; }
        public int KDW_12_9 { get; set; }


        /// <summary>
        /// 23跨号数
        /// </summary>
        public int KD_23_1 { get; set; }
        public int KD_23_2 { get; set; }
        public int KD_23_3 { get; set; }
        public int KD_23_4 { get; set; }
        public int KD_23_5 { get; set; }
        public int KD_23_6 { get; set; }
        public int KD_23_7 { get; set; }
        public int KD_23_8 { get; set; }
        public int KD_23_9 { get; set; }
        public int KD_23_10 { get; set; }
        public int KD_23_11 { get; set; }
        public int KD_23_12 { get; set; }
        public int KD_23_13 { get; set; }
        public int KD_23_14 { get; set; }
        public int KD_23_15 { get; set; }
        public int KD_23_16 { get; set; }
        public int KD_23_17 { get; set; }
        public int KD_23_18 { get; set; }
        public int KD_23_19 { get; set; }
        public int KD_23_20 { get; set; }
        public int KD_23_21 { get; set; }
        public int KD_23_22 { get; set; }
        public int KD_23_23 { get; set; }
        public int KD_23_24 { get; set; }
        public int KD_23_25 { get; set; }
        public int KD_23_26 { get; set; }
        public int KD_23_27 { get; set; }
        public int KD_23_28 { get; set; }

        /// <summary>
        /// 23跨度尾数
        /// </summary>
        public int KDW_23_0 { get; set; }
        public int KDW_23_1 { get; set; }
        public int KDW_23_2 { get; set; }
        public int KDW_23_3 { get; set; }
        public int KDW_23_4 { get; set; }
        public int KDW_23_5 { get; set; }
        public int KDW_23_6 { get; set; }
        public int KDW_23_7 { get; set; }
        public int KDW_23_8 { get; set; }
        public int KDW_23_9 { get; set; }

        /// <summary>
        /// 34跨号数
        /// </summary>
        public int KD_34_1 { get; set; }
        public int KD_34_2 { get; set; }
        public int KD_34_3 { get; set; }
        public int KD_34_4 { get; set; }
        public int KD_34_5 { get; set; }
        public int KD_34_6 { get; set; }
        public int KD_34_7 { get; set; }
        public int KD_34_8 { get; set; }
        public int KD_34_9 { get; set; }
        public int KD_34_10 { get; set; }
        public int KD_34_11 { get; set; }
        public int KD_34_12 { get; set; }
        public int KD_34_13 { get; set; }
        public int KD_34_14 { get; set; }
        public int KD_34_15 { get; set; }
        public int KD_34_16 { get; set; }
        public int KD_34_17 { get; set; }
        public int KD_34_18 { get; set; }
        public int KD_34_19 { get; set; }
        public int KD_34_20 { get; set; }
        public int KD_34_21 { get; set; }
        public int KD_34_22 { get; set; }
        public int KD_34_23 { get; set; }
        public int KD_34_24 { get; set; }
        public int KD_34_25 { get; set; }
        public int KD_34_26 { get; set; }
        public int KD_34_27 { get; set; }
        public int KD_34_28 { get; set; }

        /// <summary>
        /// 34跨度尾数
        /// </summary>
        public int KDW_34_0 { get; set; }
        public int KDW_34_1 { get; set; }
        public int KDW_34_2 { get; set; }
        public int KDW_34_3 { get; set; }
        public int KDW_34_4 { get; set; }
        public int KDW_34_5 { get; set; }
        public int KDW_34_6 { get; set; }
        public int KDW_34_7 { get; set; }
        public int KDW_34_8 { get; set; }
        public int KDW_34_9 { get; set; }

        /// <summary>
        /// 45跨号数
        /// </summary>
        public int KD_45_1 { get; set; }
        public int KD_45_2 { get; set; }
        public int KD_45_3 { get; set; }
        public int KD_45_4 { get; set; }
        public int KD_45_5 { get; set; }
        public int KD_45_6 { get; set; }
        public int KD_45_7 { get; set; }
        public int KD_45_8 { get; set; }
        public int KD_45_9 { get; set; }
        public int KD_45_10 { get; set; }
        public int KD_45_11 { get; set; }
        public int KD_45_12 { get; set; }
        public int KD_45_13 { get; set; }
        public int KD_45_14 { get; set; }
        public int KD_45_15 { get; set; }
        public int KD_45_16 { get; set; }
        public int KD_45_17 { get; set; }
        public int KD_45_18 { get; set; }
        public int KD_45_19 { get; set; }
        public int KD_45_20 { get; set; }
        public int KD_45_21 { get; set; }
        public int KD_45_22 { get; set; }
        public int KD_45_23 { get; set; }
        public int KD_45_24 { get; set; }
        public int KD_45_25 { get; set; }
        public int KD_45_26 { get; set; }
        public int KD_45_27 { get; set; }
        public int KD_45_28 { get; set; }

        /// <summary>
        /// 45跨度尾数
        /// </summary>
        public int KDW_45_0 { get; set; }
        public int KDW_45_1 { get; set; }
        public int KDW_45_2 { get; set; }
        public int KDW_45_3 { get; set; }
        public int KDW_45_4 { get; set; }
        public int KDW_45_5 { get; set; }
        public int KDW_45_6 { get; set; }
        public int KDW_45_7 { get; set; }
        public int KDW_45_8 { get; set; }
        public int KDW_45_9 { get; set; }

        /// <summary>
        /// 56跨号数
        /// </summary>
        public int KD_56_1 { get; set; }
        public int KD_56_2 { get; set; }
        public int KD_56_3 { get; set; }
        public int KD_56_4 { get; set; }
        public int KD_56_5 { get; set; }
        public int KD_56_6 { get; set; }
        public int KD_56_7 { get; set; }
        public int KD_56_8 { get; set; }
        public int KD_56_9 { get; set; }
        public int KD_56_10 { get; set; }
        public int KD_56_11 { get; set; }
        public int KD_56_12 { get; set; }
        public int KD_56_13 { get; set; }
        public int KD_56_14 { get; set; }
        public int KD_56_15 { get; set; }
        public int KD_56_16 { get; set; }
        public int KD_56_17 { get; set; }
        public int KD_56_18 { get; set; }
        public int KD_56_19 { get; set; }
        public int KD_56_20 { get; set; }
        public int KD_56_21 { get; set; }
        public int KD_56_22 { get; set; }
        public int KD_56_23 { get; set; }
        public int KD_56_24 { get; set; }
        public int KD_56_25 { get; set; }
        public int KD_56_26 { get; set; }
        public int KD_56_27 { get; set; }
        public int KD_56_28 { get; set; }

        /// <summary>
        /// 56跨度尾数
        /// </summary>
        public int KDW_56_0 { get; set; }
        public int KDW_56_1 { get; set; }
        public int KDW_56_2 { get; set; }
        public int KDW_56_3 { get; set; }
        public int KDW_56_4 { get; set; }
        public int KDW_56_5 { get; set; }
        public int KDW_56_6 { get; set; }
        public int KDW_56_7 { get; set; }
        public int KDW_56_8 { get; set; }
        public int KDW_56_9 { get; set; }


    }

    
   public class SSQ_KuaDu_1_6_InfoCollection : List<SSQ_KuaDu_1_6_Info>
   {
   }

    /// <summary>
    /// 跨度首尾
    /// </summary>
    
   public class SSQ_KuaDu_SW_Info : ImportInfoBase
    {
        /// <summary>
        /// 红球开奖号
        /// </summary>
        public string RedLotteryNumber { get; set; }

        public int KuaDu { get; set; }
        /// <summary>
        /// 跨号数
        /// </summary>
        public int KD_5 { get; set; }
        public int KD_6 { get; set; }
        public int KD_7 { get; set; }
        public int KD_8 { get; set; }
        public int KD_9 { get; set; }
        public int KD_10 { get; set; }
        public int KD_11 { get; set; }
        public int KD_12 { get; set; }
        public int KD_13 { get; set; }
        public int KD_14 { get; set; }
        public int KD_15 { get; set; }
        public int KD_16 { get; set; }
        public int KD_17 { get; set; }
        public int KD_18 { get; set; }
        public int KD_19 { get; set; }
        public int KD_20 { get; set; }
        public int KD_21 { get; set; }
        public int KD_22 { get; set; }
        public int KD_23 { get; set; }
        public int KD_24 { get; set; }
        public int KD_25 { get; set; }
        public int KD_26 { get; set; }
        public int KD_27 { get; set; }
        public int KD_28 { get; set; }
        public int KD_29 { get; set; }
        public int KD_30 { get; set; }
        public int KD_31 { get; set; }
        public int KD_32 { get; set; }
    }

    
   public class SSQ_KuaDu_SW_InfoCollection : List<SSQ_KuaDu_SW_Info>
   {
   }

    /// <summary>
    /// 质合走势
    /// </summary>
    
   public class SSQ_ZhiHe_Info : ImportInfoBase
    {
        /// <summary>
        /// 红球开奖号
        /// </summary>
        public string RedLotteryNumber { get; set; }

        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }
        public string RedBall4 { get; set; }
        public string RedBall5 { get; set; }
        public string RedBall6 { get; set; }


        public int O_RedBall1_Z { get; set; }
        public int O_RedBall2_Z { get; set; }
        public int O_RedBall3_Z { get; set; }
        public int O_RedBall4_Z { get; set; }
        public int O_RedBall5_Z { get; set; }
        public int O_RedBall6_Z { get; set; }

        public int O_RedBall1_H { get; set; }
        public int O_RedBall2_H { get; set; }
        public int O_RedBall3_H { get; set; }
        public int O_RedBall4_H { get; set; }
        public int O_RedBall5_H { get; set; }
        public int O_RedBall6_H { get; set; }
        /// <summary>
        /// 奇偶排位
        /// </summary>
        public string ZH_Qualifying { get; set; }
        /// <summary>
        /// 奇偶比例
        /// </summary>
        public string ZH_Proportion { get; set; }
        /// <summary>
        /// 奇偶比例遗漏
        /// </summary>
        public int O_ZH_Proportion60 { get; set; }
        public int O_ZH_Proportion51 { get; set; }
        public int O_ZH_Proportion42 { get; set; }
        public int O_ZH_Proportion33 { get; set; }
        public int O_ZH_Proportion24 { get; set; }
        public int O_ZH_Proportion15 { get; set; }
        public int O_ZH_Proportion06 { get; set; }
    }

    
   public class SSQ_ZhiHe_InfoCollection : List<SSQ_ZhiHe_Info>
   {
   }
}
