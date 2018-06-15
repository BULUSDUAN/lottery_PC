using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.ModelCollection;

namespace Lottery.Kg.ORM.Helper.WinNumber.ModelCollection
{
    /// <summary>
    /// 2连走势
    /// </summary>
     
    public class LN11X5_2LZS_Info : ImportInfoBase
    {
        public int Red_12 { get; set; }
        public int Red_23 { get; set; }
        public int Red_34 { get; set; }
        public int Red_45 { get; set; }
        public int Red_56 { get; set; }
        public int Red_67 { get; set; }
        public int Red_78 { get; set; }
        public int Red_89 { get; set; }
        public int Red_910 { get; set; }
        public int Red_1011 { get; set; }

        public int GHao { get; set; }

        public int GH_0 { get; set; }
        public int GH_1 { get; set; }
        public int GH_2 { get; set; }
        public int GH_3 { get; set; }
        public int GH_4 { get; set; }
    }
     
    public class LN11X5_2LZS_InfoCollection : List<LN11X5_2LZS_Info>
    {
    }
    /// <summary>
    /// 重号走势
    /// </summary>
     
    public class LN11X5_CHZS_Info : ImportInfoBase
    {
        public int Red_01 { get; set; }
        public int Red_02 { get; set; }
        public int Red_03 { get; set; }
        public int Red_04 { get; set; }
        public int Red_05 { get; set; }
        public int Red_06 { get; set; }
        public int Red_07 { get; set; }
        public int Red_08 { get; set; }
        public int Red_09 { get; set; }
        public int Red_10 { get; set; }
        public int Red_11 { get; set; }

        /// <summary>
        /// 重号
        /// </summary>
        public int Duplicate { get; set; }

        public int D_0 { get; set; }
        public int D_1 { get; set; }
        public int D_2 { get; set; }
        public int D_3 { get; set; }
        public int D_4 { get; set; }
        public int D_5 { get; set; }
    }
     
    public class LN11X5_CHZS_InfoCollection : List<LN11X5_CHZS_Info>
    {
    }
    /// <summary>
    /// 多连走势
    /// </summary>
     
    public class LN11X5_DLZS_Info : ImportInfoBase
    {
        public int Red_123 { get; set; }
        public int Red_234 { get; set; }
        public int Red_345 { get; set; }
        public int Red_456 { get; set; }
        public int Red_567 { get; set; }
        public int Red_678 { get; set; }
        public int Red_789 { get; set; }
        public int Red_8910 { get; set; }
        public int Red_91011 { get; set; }

        public int Red_1234 { get; set; }
        public int Red_2345 { get; set; }
        public int Red_3456 { get; set; }
        public int Red_4567 { get; set; }
        public int Red_5678 { get; set; }
        public int Red_6789 { get; set; }
        public int Red_78910 { get; set; }
        public int Red_891011 { get; set; }

        public int Red_12345 { get; set; }
        public int Red_23456 { get; set; }
        public int Red_34567 { get; set; }
        public int Red_45678 { get; set; }
        public int Red_56789 { get; set; }
        public int Red_678910 { get; set; }
        public int Red_7891011 { get; set; }
    }
     
    public class LN11X5_DLZS_InfoCollection : List<LN11X5_DLZS_Info>
    {
    }
    /// <summary>
    /// 大小走势
    /// </summary>
     
    public class LN11X5_DXZS_Info : ImportInfoBase
    {
        public int D_Red1 { get; set; }
        public int X_Red1 { get; set; }
        public int D_Red2 { get; set; }
        public int X_Red2 { get; set; }
        public int D_Red3 { get; set; }
        public int X_Red3 { get; set; }
        public int D_Red4 { get; set; }
        public int X_Red4 { get; set; }
        public int D_Red5 { get; set; }
        public int X_Red5 { get; set; }


        public int DX_Q_D { get; set; }
        public int DX_1D4X { get; set; }
        public int DX_2D3X { get; set; }
        public int DX_3D2X { get; set; }
        public int DX_4D1X { get; set; }
        public int DX_Q_X { get; set; }

        /// <summary>
        /// 大小比例
        /// </summary>
        public string DX_Proportion { get; set; }
    }
     
    public class LN11X5_DXZS_InfoCollection : List<LN11X5_DXZS_Info>
    {
    }
    /// <summary>
    /// 隔号走势
    /// </summary>
     
    public class LN11X5_GHZS_Info : ImportInfoBase
    {
        public int Red_13 { get; set; }
        public int Red_24 { get; set; }
        public int Red_35 { get; set; }
        public int Red_46 { get; set; }
        public int Red_57 { get; set; }
        public int Red_68 { get; set; }
        public int Red_79 { get; set; }
        public int Red_810 { get; set; }
        public int Red_911 { get; set; }

        public int GHao { get; set; }

        public int GH_0 { get; set; }
        public int GH_1 { get; set; }
        public int GH_2 { get; set; }
        public int GH_3 { get; set; }
        public int GH_4 { get; set; }
    }
     
    public class LN11X5_GHZS_InfoCollection : List<LN11X5_GHZS_Info>
    {
    }
    /// <summary>
    /// 和值走势
    /// </summary>
     
    public class LN11X5_HZZS_Info : ImportInfoBase
    {
        public int HeZhi { get; set; }

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
     
    public class LN11X5_HZZS_InfoCollection : List<LN11X5_HZZS_Info>
    {
    }
    /// <summary>
    /// 基本走势
    /// </summary>
     
    public class LN11X5_JBZS_Info : ImportInfoBase
    {
        public int Red_01 { get; set; }
        public int Red_02 { get; set; }
        public int Red_03 { get; set; }
        public int Red_04 { get; set; }
        public int Red_05 { get; set; }
        public int Red_06 { get; set; }
        public int Red_07 { get; set; }
        public int Red_08 { get; set; }
        public int Red_09 { get; set; }
        public int Red_10 { get; set; }
        public int Red_11 { get; set; }

        /// <summary>
        /// 和值
        /// </summary>
        public int HeZhi { get; set; }
        /// <summary>
        /// 奇偶比例
        /// </summary>
        public string JO_Proportion { get; set; }
        /// <summary>
        /// 质合比例
        /// </summary>
        public string ZH_Proportion { get; set; }
        /// <summary>
        /// 跨度
        /// </summary>
        public int KuaDu { get; set; }
        /// <summary>
        /// 重号
        /// </summary>
        public int Duplicate { get; set; }
        /// <summary>
        /// 连号个数
        /// </summary>
        public int ContinuousNumber { get; set; }
    }
     
    public class LN11X5_JBZS_InfoCollection : List<LN11X5_JBZS_Info>
    {
    }
    /// <summary>
    /// 奇偶走势
    /// </summary>
     
    public class LN11X5_JOZS_Info : ImportInfoBase
    {
        public int J_Red1 { get; set; }
        public int O_Red1 { get; set; }
        public int J_Red2 { get; set; }
        public int O_Red2 { get; set; }
        public int J_Red3 { get; set; }
        public int O_Red3 { get; set; }
        public int J_Red4 { get; set; }
        public int O_Red4 { get; set; }
        public int J_Red5 { get; set; }
        public int O_Red5 { get; set; }

        public int JO_Q_J { get; set; }
        public int JO_1J4O { get; set; }
        public int JO_2J3O { get; set; }
        public int JO_3J2O { get; set; }
        public int JO_4J1O { get; set; }
        public int JO_Q_O { get; set; }

        /// <summary>
        /// 大小比例
        /// </summary>
        public string JO_Proportion { get; set; }
    }
     
    public class LN11X5_JOZS_InfoCollection : List<LN11X5_JOZS_Info>
    {
    }
    /// <summary>
    /// 前1走势
    /// </summary>
     
    public class LN11X5_Q1ZS_Info : ImportInfoBase
    {
        public int Red_01 { get; set; }
        public int Red_02 { get; set; }
        public int Red_03 { get; set; }
        public int Red_04 { get; set; }
        public int Red_05 { get; set; }
        public int Red_06 { get; set; }
        public int Red_07 { get; set; }
        public int Red_08 { get; set; }
        public int Red_09 { get; set; }
        public int Red_10 { get; set; }
        public int Red_11 { get; set; }

        public int DDan { get; set; }
        public int DS { get; set; }
        public int XDan { get; set; }
        public int XS { get; set; }

        public int C3_0 { get; set; }
        public int C3_1 { get; set; }
        public int C3_2 { get; set; }
    }
     
    public class LN11X5_Q1ZS_InfoCollection : List<LN11X5_Q1ZS_Info>
    {
    }
    /// <summary>
    /// 前2走势
    /// </summary>
     
    public class LN11X5_Q2ZS_Info : ImportInfoBase
    {
        public int DDan { get; set; }
        public int DS { get; set; }
        public int XDan { get; set; }
        public int XS { get; set; }

        public int C3_0 { get; set; }
        public int C3_1 { get; set; }
        public int C3_2 { get; set; }

        public int DDan1 { get; set; }
        public int DS1 { get; set; }
        public int XDan1 { get; set; }
        public int XS1 { get; set; }

        public int C31_0 { get; set; }
        public int C31_1 { get; set; }
        public int C31_2 { get; set; }
    }
     
    public class LN11X5_Q2ZS_InfoCollection : List<LN11X5_Q2ZS_Info>
    {
    }
    /// <summary>
    /// 前3走势
    /// </summary>
     
    public class LN11X5_Q3ZS_Info : ImportInfoBase
    {
        public int DDan { get; set; }
        public int DS { get; set; }
        public int XDan { get; set; }
        public int XS { get; set; }

        public int C3_0 { get; set; }
        public int C3_1 { get; set; }
        public int C3_2 { get; set; }

        public int DDan1 { get; set; }
        public int DS1 { get; set; }
        public int XDan1 { get; set; }
        public int XS1 { get; set; }

        public int C31_0 { get; set; }
        public int C31_1 { get; set; }
        public int C31_2 { get; set; }

        public int DDan2 { get; set; }
        public int DS2 { get; set; }
        public int XDan2 { get; set; }
        public int XS2 { get; set; }

        public int C32_0 { get; set; }
        public int C32_1 { get; set; }
        public int C32_2 { get; set; }
    }
     
    public class LN11X5_Q3ZS_InfoCollection : List<LN11X5_Q3ZS_Info>
    {
    }

}
