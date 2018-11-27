using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection
{
    /// <summary>
    /// 重庆1星走势
    /// </summary>
     
    public class CQSSC_1X_ZS_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }

        public int Red_G0 { get; set; }
        public int Red_G1 { get; set; }
        public int Red_G2 { get; set; }
        public int Red_G3 { get; set; }
        public int Red_G4 { get; set; }
        public int Red_G5 { get; set; }
        public int Red_G6 { get; set; }
        public int Red_G7 { get; set; }
        public int Red_G8 { get; set; }
        public int Red_G9 { get; set; }

        public int D_Red1 { get; set; }
        public int X_Red1 { get; set; }
        public int J_Red1 { get; set; }
        public int O_Red1 { get; set; }
        public int Z_Red1 { get; set; }
        public int H_Red1 { get; set; }

        public int O_Red1_0 { get; set; }
        public int O_Red1_1 { get; set; }
        public int O_Red1_2 { get; set; }
    }

     
    public class CQSSC_1X_ZS_InfoCollection : List<CQSSC_1X_ZS_Info>
    {
    }
    /// <summary>
    /// 重庆2星和值走势
    /// </summary>
     
    public class CQSSC_2X_HZZS_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }

        public int HeZhi { set; get; }

        public int HeWei { set; get; }

        public int HZ_0 { get; set; }
        public int HZ_1 { get; set; }
        public int HZ_2 { get; set; }
        public int HZ_3 { get; set; }
        public int HZ_4 { get; set; }
        public int HZ_5 { get; set; }
        public int HZ_6 { get; set; }
        public int HZ_7 { get; set; }
        public int HZ_8 { get; set; }
        public int HZ_9 { get; set; }
        public int HZ_10 { get; set; }
        public int HZ_11 { get; set; }
        public int HZ_12 { get; set; }
        public int HZ_13 { get; set; }
        public int HZ_14 { get; set; }
        public int HZ_15 { get; set; }
        public int HZ_16 { get; set; }
        public int HZ_17 { get; set; }
        public int HZ_18 { get; set; }

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

     
    public class CQSSC_2X_HZZS_InfoCollection : List<CQSSC_2X_HZZS_Info>
    {
    }
    /// <summary>
    /// 重庆2星组选走势
    /// </summary>
     
    public class CQSSC_2X_ZuXZS_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// 大小比例
        /// </summary>
        public string DX_Proportion { get; set; }

        /// <summary>
        /// 奇偶比例
        /// </summary>
        public string JO_Proportion { get; set; }

        /// <summary>
        /// 质合比例
        /// </summary>
        public string ZH_Proportion { get; set; }

        public int Red_0 { get; set; }
        public int Red_1 { get; set; }
        public int Red_2 { get; set; }
        public int Red_3 { get; set; }
        public int Red_4 { get; set; }
        public int Red_5 { get; set; }
        public int Red_6 { get; set; }
        public int Red_7 { get; set; }
        public int Red_8 { get; set; }
        public int Red_9 { get; set; }

        public int RedCan_0 { get; set; }
        public int RedCan_1 { get; set; }
        public int RedCan_2 { get; set; }
        public int RedCan_3 { get; set; }
        public int RedCan_4 { get; set; }
        public int RedCan_5 { get; set; }
        public int RedCan_6 { get; set; }
        public int RedCan_7 { get; set; }
        public int RedCan_8 { get; set; }
        public int RedCan_9 { get; set; }

        public int Type_DZ { set; get; }

        /// <summary>
        /// 大小比例遗漏
        /// </summary>
        public int O_DX_Proportion20 { get; set; }
        public int O_DX_Proportion11 { get; set; }
        public int O_DX_Proportion02 { get; set; }

        /// <summary>
        /// 奇偶比例遗漏
        /// </summary>
        public int O_JO_Proportion20 { get; set; }
        public int O_JO_Proportion11 { get; set; }
        public int O_JO_Proportion02 { get; set; }

        /// <summary>
        /// 质合比例遗漏
        /// </summary>
        public int O_ZH_Proportion20 { get; set; }
        public int O_ZH_Proportion11 { get; set; }
        public int O_ZH_Proportion02 { get; set; }
    }

     
    public class CQSSC_2X_ZuXZS_InfoCollection : List<CQSSC_2X_ZuXZS_Info>
    {
    }
    /// <summary>
    /// 重庆2星直选走势
    /// </summary>
     
    public class CQSSC_2X_ZXZS_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }

        public string Type { get; set; }

        public int Red_S0 { get; set; }
        public int Red_S1 { get; set; }
        public int Red_S2 { get; set; }
        public int Red_S3 { get; set; }
        public int Red_S4 { get; set; }
        public int Red_S5 { get; set; }
        public int Red_S6 { get; set; }
        public int Red_S7 { get; set; }
        public int Red_S8 { get; set; }
        public int Red_S9 { get; set; }

        public int Red_G0 { get; set; }
        public int Red_G1 { get; set; }
        public int Red_G2 { get; set; }
        public int Red_G3 { get; set; }
        public int Red_G4 { get; set; }
        public int Red_G5 { get; set; }
        public int Red_G6 { get; set; }
        public int Red_G7 { get; set; }
        public int Red_G8 { get; set; }
        public int Red_G9 { get; set; }

        public int Type_DZ { set; get; }
        /// <summary>
        /// 大小比例
        /// </summary>
        public string DX_Proportion { get; set; }

        /// <summary>
        /// 奇偶比例
        /// </summary>
        public string JO_Proportion { get; set; }

        /// <summary>
        /// 质合比例
        /// </summary>
        public string ZH_Proportion { get; set; }

        /// <summary>
        /// 大小比例遗漏
        /// </summary>
        public int O_DX_Proportion20 { get; set; }
        public int O_DX_Proportion11 { get; set; }
        public int O_DX_Proportion02 { get; set; }

        /// <summary>
        /// 奇偶比例遗漏
        /// </summary>
        public int O_JO_Proportion20 { get; set; }
        public int O_JO_Proportion11 { get; set; }
        public int O_JO_Proportion02 { get; set; }

        /// <summary>
        /// 质合比例遗漏
        /// </summary>
        public int O_ZH_Proportion20 { get; set; }
        public int O_ZH_Proportion11 { get; set; }
        public int O_ZH_Proportion02 { get; set; }
    }

     
    public class CQSSC_2X_ZXZS_InfoCollection : List<CQSSC_2X_ZXZS_Info>
    {
    }
    /// <summary>
    /// 除3
    /// </summary>
     
    public class CQSSC_3X_C3YS_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }

        public int O_Red1_0 { get; set; }
        public int O_Red1_1 { get; set; }
        public int O_Red1_2 { get; set; }

        public int O_Red2_0 { get; set; }
        public int O_Red2_1 { get; set; }
        public int O_Red2_2 { get; set; }

        public int O_Red3_0 { get; set; }
        public int O_Red3_1 { get; set; }
        public int O_Red3_2 { get; set; }

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

        public int Y1_Number0 { get; set; }
        public int Y1_Number1 { get; set; }
        public int Y1_Number2 { get; set; }
        public int Y1_Number3 { get; set; }

        public int Y2_Number0 { get; set; }
        public int Y2_Number1 { get; set; }
        public int Y2_Number2 { get; set; }
        public int Y2_Number3 { get; set; }
        /// <summary>
        /// 余数比例
        /// </summary>
        public string YS_Proportion { get; set; }
    }

     
    public class CQSSC_3X_C3YS_InfoCollection : List<CQSSC_3X_C3YS_Info>
    {
    }
    /// <summary>
    /// 大小走势
    /// </summary>
     
    public class CQSSC_3X_DXZS_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }


        public int D_Red1 { get; set; }
        public int X_Red1 { get; set; }
        public int D_Red2 { get; set; }
        public int X_Red2 { get; set; }
        public int D_Red3 { get; set; }
        public int X_Red3 { get; set; }
        /// <summary>
        /// 大小形态
        /// </summary>
        public string Dxxt { get; set; }

        public int DX_Q_D { get; set; }
        public int DX_DDX { get; set; }
        public int DX_DXD { get; set; }
        public int DX_XDD { get; set; }
        public int DX_DXX { get; set; }
        public int DX_XDX { get; set; }
        public int DX_XXD { get; set; }
        public int DX_Q_X { get; set; }

        /// <summary>
        /// 大小比例
        /// </summary>
        public string DX_Proportion { get; set; }

        /// <summary>
        /// 大小比例遗漏
        /// </summary>
        public int O_DX_Proportion30 { get; set; }
        public int O_DX_Proportion21 { get; set; }
        public int O_DX_Proportion12 { get; set; }
        public int O_DX_Proportion03 { get; set; }
    }

     
    public class CQSSC_3X_DXZS_InfoCollection : List<CQSSC_3X_DXZS_Info>
    {
    }
    /// <summary>
    /// 和值走势
    /// </summary>
     
    public class CQSSC_3X_HZZS_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }

        public int HZ_0 { get; set; }
        public int HZ_1 { get; set; }
        public int HZ_2 { get; set; }
        public int HZ_3 { get; set; }
        public int HZ_4 { get; set; }
        public int HZ_5 { get; set; }
        public int HZ_6 { get; set; }
        public int HZ_7 { get; set; }
        public int HZ_8 { get; set; }
        public int HZ_9 { get; set; }
        public int HZ_10 { get; set; }
        public int HZ_11 { get; set; }
        public int HZ_12 { get; set; }
        public int HZ_13 { get; set; }
        public int HZ_14 { get; set; }
        public int HZ_15 { get; set; }
        public int HZ_16 { get; set; }
        public int HZ_17 { get; set; }
        public int HZ_18 { get; set; }
        public int HZ_19 { get; set; }
        public int HZ_20 { get; set; }
        public int HZ_21 { get; set; }
        public int HZ_22 { get; set; }
        public int HZ_23 { get; set; }
        public int HZ_24 { get; set; }
        public int HZ_25 { get; set; }
        public int HZ_26 { get; set; }
        public int HZ_27 { get; set; }

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

     
    public class CQSSC_3X_HZZS_InfoCollection : List<CQSSC_3X_HZZS_Info>
    {
    }
    /// <summary>
    /// 奇偶走势
    /// </summary>
     
    public class CQSSC_3X_JOZS_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }


        public int J_Red1 { get; set; }
        public int O_Red1 { get; set; }
        public int J_Red2 { get; set; }
        public int O_Red2 { get; set; }
        public int J_Red3 { get; set; }
        public int O_Red3 { get; set; }
        /// <summary>
        /// 奇偶形态
        /// </summary>
        public string Joxt { get; set; }

        public int JO_Q_J { get; set; }
        public int JO_JJO { get; set; }
        public int JO_JOJ { get; set; }
        public int JO_OJJ { get; set; }
        public int JO_JOO { get; set; }
        public int JO_OJO { get; set; }
        public int JO_OOJ { get; set; }
        public int JO_Q_O { get; set; }

        /// <summary>
        /// 奇偶比例
        /// </summary>
        public string JO_Proportion { get; set; }

        /// <summary>
        /// 奇偶比例遗漏
        /// </summary>
        public int O_JO_Proportion30 { get; set; }
        public int O_JO_Proportion21 { get; set; }
        public int O_JO_Proportion12 { get; set; }
        public int O_JO_Proportion03 { get; set; }
    }

     
    public class CQSSC_3X_JOZS_InfoCollection : List<CQSSC_3X_JOZS_Info>
    {
    }
    /// <summary>
    /// 跨度
    /// </summary>
     
    public class CQSSC_3X_KD_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }

        public int KuaDu { set; get; }

        public int KD_0 { get; set; }
        public int KD_1 { get; set; }
        public int KD_2 { get; set; }
        public int KD_3 { get; set; }
        public int KD_4 { get; set; }
        public int KD_5 { get; set; }
        public int KD_6 { get; set; }
        public int KD_7 { get; set; }
        public int KD_8 { get; set; }
        public int KD_9 { get; set; }
    }

     
    public class CQSSC_3X_KD_InfoCollection : List<CQSSC_3X_KD_Info>
    {
    }
    /// <summary>
    /// 质合走势
    /// </summary>
     
    public class CQSSC_3X_ZHZS_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }

        public int Z_Red1 { get; set; }
        public int H_Red1 { get; set; }
        public int Z_Red2 { get; set; }
        public int H_Red2 { get; set; }
        public int Z_Red3 { get; set; }
        public int H_Red3 { get; set; }
        /// <summary>
        /// 质合形态
        /// </summary>
        public string Zhxt { get; set; }

        public int ZH_Q_Z { get; set; }
        public int ZH_ZZH { get; set; }
        public int ZH_ZHZ { get; set; }
        public int ZH_HZZ { get; set; }
        public int ZH_ZHH { get; set; }
        public int ZH_HZH { get; set; }
        public int ZH_HHZ { get; set; }
        public int ZH_Q_H { get; set; }

        /// <summary>
        /// 质合比例
        /// </summary>
        public string ZH_Proportion { get; set; }

        /// <summary>
        /// 质合比例遗漏
        /// </summary>
        public int O_ZH_Proportion30 { get; set; }
        public int O_ZH_Proportion21 { get; set; }
        public int O_ZH_Proportion12 { get; set; }
        public int O_ZH_Proportion03 { get; set; }
    }

     
    public class CQSSC_3X_ZHZS_InfoCollection : List<CQSSC_3X_ZHZS_Info>
    {
    }
    /// <summary>
    /// 重庆3星组选走势
    /// </summary>
     
    public class CQSSC_3X_ZuXZS_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// 大小比例
        /// </summary>
        public string DX_Proportion { get; set; }

        /// <summary>
        /// 奇偶比例
        /// </summary>
        public string JO_Proportion { get; set; }

        /// <summary>
        /// 质合比例
        /// </summary>
        public string ZH_Proportion { get; set; }

        public int Red_0 { get; set; }
        public int Red_1 { get; set; }
        public int Red_2 { get; set; }
        public int Red_3 { get; set; }
        public int Red_4 { get; set; }
        public int Red_5 { get; set; }
        public int Red_6 { get; set; }
        public int Red_7 { get; set; }
        public int Red_8 { get; set; }
        public int Red_9 { get; set; }

        public int RedCan_0 { get; set; }
        public int RedCan_1 { get; set; }
        public int RedCan_2 { get; set; }
        public int RedCan_3 { get; set; }
        public int RedCan_4 { get; set; }
        public int RedCan_5 { get; set; }
        public int RedCan_6 { get; set; }
        public int RedCan_7 { get; set; }
        public int RedCan_8 { get; set; }
        public int RedCan_9 { get; set; }

        public int Type_Z3 { set; get; }
        public int Type_Z6 { set; get; }
        public int Type_BZ { set; get; }

        /// <summary>
        /// 大小比例遗漏
        /// </summary>
        public int O_DX_Proportion30 { get; set; }
        public int O_DX_Proportion21 { get; set; }
        public int O_DX_Proportion12 { get; set; }
        public int O_DX_Proportion03 { get; set; }

        /// <summary>
        /// 奇偶比例遗漏
        /// </summary>
        public int O_JO_Proportion30 { get; set; }
        public int O_JO_Proportion21 { get; set; }
        public int O_JO_Proportion12 { get; set; }
        public int O_JO_Proportion03 { get; set; }

        /// <summary>
        /// 质合比例遗漏
        /// </summary>
        public int O_ZH_Proportion30 { get; set; }
        public int O_ZH_Proportion21 { get; set; }
        public int O_ZH_Proportion12 { get; set; }
        public int O_ZH_Proportion03 { get; set; }
    }

     
    public class CQSSC_3X_ZuXZS_InfoCollection : List<CQSSC_3X_ZuXZS_Info>
    {
    }
    /// <summary>
    /// 重庆3星直选走势
    /// </summary>
     
    public class CQSSC_3X_ZXZS_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }

        public string Type { get; set; }

        public int Red_B0 { get; set; }
        public int Red_B1 { get; set; }
        public int Red_B2 { get; set; }
        public int Red_B3 { get; set; }
        public int Red_B4 { get; set; }
        public int Red_B5 { get; set; }
        public int Red_B6 { get; set; }
        public int Red_B7 { get; set; }
        public int Red_B8 { get; set; }
        public int Red_B9 { get; set; }

        public int Red_S0 { get; set; }
        public int Red_S1 { get; set; }
        public int Red_S2 { get; set; }
        public int Red_S3 { get; set; }
        public int Red_S4 { get; set; }
        public int Red_S5 { get; set; }
        public int Red_S6 { get; set; }
        public int Red_S7 { get; set; }
        public int Red_S8 { get; set; }
        public int Red_S9 { get; set; }

        public int Red_G0 { get; set; }
        public int Red_G1 { get; set; }
        public int Red_G2 { get; set; }
        public int Red_G3 { get; set; }
        public int Red_G4 { get; set; }
        public int Red_G5 { get; set; }
        public int Red_G6 { get; set; }
        public int Red_G7 { get; set; }
        public int Red_G8 { get; set; }
        public int Red_G9 { get; set; }

        public int Type_Z3 { set; get; }
        public int Type_Z6 { set; get; }
        public int Type_BZ { set; get; }
    }

     
    public class CQSSC_3X_ZXZS_InfoCollection : List<CQSSC_3X_ZXZS_Info>
    {
    }
    /// <summary>
    /// 和值走势
    /// </summary>
     
    public class CQSSC_5X_HZZS_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }
        public string RedBall4 { get; set; }
        public string RedBall5 { get; set; }

        public int HeZhi { set; get; }

        public int HZ_0 { get; set; }
        public int HZ_1 { get; set; }
        public int HZ_2 { get; set; }
        public int HZ_3 { get; set; }
        public int HZ_4 { get; set; }
        public int HZ_5 { get; set; }
        public int HZ_6 { get; set; }
        public int HZ_7 { get; set; }
        public int HZ_8 { get; set; }
        public int HZ_9 { get; set; }
        public int HZ_10 { get; set; }
        public int HZ_11 { get; set; }
        public int HZ_12 { get; set; }
        public int HZ_13 { get; set; }
        public int HZ_14 { get; set; }
        public int HZ_15 { get; set; }
        public int HZ_16 { get; set; }
        public int HZ_17 { get; set; }
        public int HZ_18 { get; set; }
        public int HZ_19 { get; set; }
        public int HZ_20 { get; set; }
        public int HZ_21 { get; set; }
        public int HZ_22 { get; set; }
        public int HZ_23 { get; set; }
        public int HZ_24 { get; set; }
        public int HZ_25 { get; set; }
        public int HZ_26 { get; set; }
        public int HZ_27 { get; set; }
        public int HZ_28 { get; set; }
        public int HZ_29 { get; set; }
        public int HZ_30 { get; set; }
        public int HZ_31 { get; set; }
        public int HZ_32 { get; set; }
        public int HZ_33 { get; set; }
        public int HZ_34 { get; set; }
        public int HZ_35 { get; set; }
        public int HZ_36 { get; set; }
        public int HZ_37 { get; set; }
        public int HZ_38 { get; set; }
        public int HZ_39 { get; set; }
        public int HZ_40 { get; set; }
        public int HZ_41 { get; set; }
        public int HZ_42 { get; set; }
        public int HZ_43 { get; set; }
        public int HZ_44 { get; set; }
        public int HZ_45 { get; set; }
    }

     
    public class CQSSC_5X_HZZS_InfoCollection : List<CQSSC_5X_HZZS_Info>
    {
    }
    /// <summary>
    /// 重庆5星基本走势
    /// </summary>
     
    public class CQSSC_5X_JBZS_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }
        public string RedBall4 { get; set; }
        public string RedBall5 { get; set; }

        public int Red_W0 { get; set; }
        public int Red_W1 { get; set; }
        public int Red_W2 { get; set; }
        public int Red_W3 { get; set; }
        public int Red_W4 { get; set; }
        public int Red_W5 { get; set; }
        public int Red_W6 { get; set; }
        public int Red_W7 { get; set; }
        public int Red_W8 { get; set; }
        public int Red_W9 { get; set; }

        public int Red_Q0 { get; set; }
        public int Red_Q1 { get; set; }
        public int Red_Q2 { get; set; }
        public int Red_Q3 { get; set; }
        public int Red_Q4 { get; set; }
        public int Red_Q5 { get; set; }
        public int Red_Q6 { get; set; }
        public int Red_Q7 { get; set; }
        public int Red_Q8 { get; set; }
        public int Red_Q9 { get; set; }

        public int Red_B0 { get; set; }
        public int Red_B1 { get; set; }
        public int Red_B2 { get; set; }
        public int Red_B3 { get; set; }
        public int Red_B4 { get; set; }
        public int Red_B5 { get; set; }
        public int Red_B6 { get; set; }
        public int Red_B7 { get; set; }
        public int Red_B8 { get; set; }
        public int Red_B9 { get; set; }

        public int Red_S0 { get; set; }
        public int Red_S1 { get; set; }
        public int Red_S2 { get; set; }
        public int Red_S3 { get; set; }
        public int Red_S4 { get; set; }
        public int Red_S5 { get; set; }
        public int Red_S6 { get; set; }
        public int Red_S7 { get; set; }
        public int Red_S8 { get; set; }
        public int Red_S9 { get; set; }

        public int Red_G0 { get; set; }
        public int Red_G1 { get; set; }
        public int Red_G2 { get; set; }
        public int Red_G3 { get; set; }
        public int Red_G4 { get; set; }
        public int Red_G5 { get; set; }
        public int Red_G6 { get; set; }
        public int Red_G7 { get; set; }
        public int Red_G8 { get; set; }
        public int Red_G9 { get; set; }

        /// <summary>
        /// 大小比例
        /// </summary>
        public string DX_Proportion { get; set; }

        /// <summary>
        /// 奇偶比例
        /// </summary>
        public string JO_Proportion { get; set; }
    }

     
    public class CQSSC_5X_JBZS_InfoCollection : List<CQSSC_5X_JBZS_Info>
    {
    }
    /// <summary>
    /// 大小单双
    /// </summary>
     
    public class CQSSC_DXDS_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }

        public int D_Red_S { get; set; }
        public int X_Red_S { get; set; }
        public int Dan_Red_S { get; set; }
        public int S_Red_S { get; set; }
        public int D_Red_G { get; set; }
        public int X_Red_G { get; set; }
        public int Dan_Red_G { get; set; }
        public int S_Red_G { get; set; }

        public int DD { get; set; }
        public int DX { get; set; }
        public int DDan { get; set; }
        public int DS { get; set; }
        public int XD { get; set; }
        public int XX { get; set; }
        public int XDan { get; set; }
        public int XS { get; set; }
        public int DanD { get; set; }
        public int DanX { get; set; }
        public int DanDan { get; set; }
        public int DanS { get; set; }
        public int SD { get; set; }
        public int SX { get; set; }
        public int SDan { get; set; }
        public int SS { get; set; }
    }

     
    public class CQSSC_DXDS_InfoCollection : List<CQSSC_DXDS_Info>
    {
    }
}
