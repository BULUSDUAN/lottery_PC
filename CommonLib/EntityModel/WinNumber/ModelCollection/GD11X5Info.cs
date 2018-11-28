using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection
{
    /// <summary>
    /// 2连走势
    /// </summary>
     
    public class GD11X5_012DWZS_Info : ImportInfoBase
    {
        public int C31_0 { get; set; }
        public int C31_1 { get; set; }
        public int C31_2 { get; set; }

        public int C32_0 { get; set; }
        public int C32_1 { get; set; }
        public int C32_2 { get; set; }

        public int C33_0 { get; set; }
        public int C33_1 { get; set; }
        public int C33_2 { get; set; }

        public int C34_0 { get; set; }
        public int C34_1 { get; set; }
        public int C34_2 { get; set; }

        public int C35_0 { get; set; }
        public int C35_1 { get; set; }
        public int C35_2 { get; set; }

        public string C012XT { get; set; }
        public string C012BL { get; set; }
    }
     
    public class GD11X5_012DWZS_InfoCollection : List<GD11X5_012DWZS_Info>
    {
    }

     
    public class GD11X5_012LZZS_Info : ImportInfoBase
    {
        public int C_320 { get; set; }
        public int C_311 { get; set; }
        public int C_302 { get; set; }
        public int C_221 { get; set; }
        public int C_212 { get; set; }
        public int C_230 { get; set; }
        public int C_203 { get; set; }
        public int C_140 { get; set; }
        public int C_131 { get; set; }
        public int C_122 { get; set; }
        public int C_113 { get; set; }
        public int C_104 { get; set; }
        public int C_041 { get; set; }
        public int C_032 { get; set; }
        public int C_023 { get; set; }
        public int C_014 { get; set; }
    }
     
    public class GD11X5_012LZZS_InfoCollection : List<GD11X5_012LZZS_Info>
    {
    }
    /// <summary>
    /// 2连走势
    /// </summary>
     
    public class GD11X5_2LZS_Info : ImportInfoBase
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

        public string XLH { get; set; }
        public string DLH { get; set; }

        public int GH_0 { get; set; }
        public int GH_1 { get; set; }
        public int GH_2 { get; set; }
        public int GH_3 { get; set; }
        public int GH_4 { get; set; }
    }
     
    public class GD11X5_2LZS_InfoCollection : List<GD11X5_2LZS_Info>
    {
    }
    /// <summary>
    /// 重号走势
    /// </summary>
     
    public class GD11X5_CHZS_Info : ImportInfoBase
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

        public int D1_01_04 { get; set; }
        public int D2_05_08 { get; set; }
        public int D3_09_11 { get; set; }
    }
     
    public class GD11X5_CHZS_InfoCollection : List<GD11X5_CHZS_Info>
    {
    }
    /// <summary>
    /// 多连走势
    /// </summary>
     
    public class GD11X5_DLZS_Info : ImportInfoBase
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
     
    public class GD11X5_DLZS_InfoCollection : List<GD11X5_DLZS_Info>
    {
    }
    /// <summary>
    /// 隔号走势
    /// </summary>
     
    public class GD11X5_GHZS_Info : ImportInfoBase
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

        public int J_GHao { get; set; }
        public int O_GHao { get; set; }

        public int GH_0 { get; set; }
        public int GH_1 { get; set; }
        public int GH_2 { get; set; }
        public int GH_3 { get; set; }
        public int GH_4 { get; set; }
    }
     
    public class GD11X5_GHZS_InfoCollection : List<GD11X5_GHZS_Info>
    {
    }
    /// <summary>
    /// 和值走势
    /// </summary>
     
    public class GD11X5_HZZS_Info : ImportInfoBase
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
     
    public class GD11X5_HZZS_InfoCollection : List<GD11X5_HZZS_Info>
    {
    }

     
    public class GD11X5_JBZS_Info : ImportInfoBase
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
        /// 大小比例
        /// </summary>
        public string DX_Proportion { get; set; }
        /// <summary>
        /// 重号
        /// </summary>
        public int Duplicate { get; set; }
        /// <summary>
        /// 连号个数
        /// </summary>
        public int ContinuousNumber { get; set; }
    }
     
    public class GD11X5_JBZS_InfoCollection : List<GD11X5_JBZS_Info>
    {
    }
    /// <summary>
    /// 跨度走势
    /// </summary>
     
    public class GD11X5_KDZS_Info : ImportInfoBase
    {
        public int KD_4 { get; set; }
        public int KD_5 { get; set; }
        public int KD_6 { get; set; }
        public int KD_7 { get; set; }
        public int KD_8 { get; set; }
        public int KD_9 { get; set; }
        public int KD_10 { get; set; }


        public int KuaDu { get; set; }

        public int JO_J { get; set; }
        public int JO_O { get; set; }
        public int ZH_Z { get; set; }
        public int ZH_H { get; set; }


        public int C3_0 { get; set; }
        public int C3_1 { get; set; }
        public int C3_2 { get; set; }
    }
     
    public class GD11X5_KDZS_InfoCollection : List<GD11X5_KDZS_Info>
    {
    }
    /// <summary>
    /// 前1走势
    /// </summary>
     
    public class GD11X5_Q1JBZS_Info : ImportInfoBase
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

        public int HeZhi { get; set; }

        public int JO_J { get; set; }
        public int JO_O { get; set; }
        public int DX_D { get; set; }
        public int DX_X { get; set; }
        public int ZH_Z { get; set; }
        public int ZH_H { get; set; }


        public int C3_0 { get; set; }
        public int C3_1 { get; set; }
        public int C3_2 { get; set; }
    }
     
    public class GD11X5_Q1JBZS_InfoCollection : List<GD11X5_Q1JBZS_Info>
    {
    }
    /// <summary>
    /// 前1形态走势
    /// </summary>
     
    public class GD11X5_Q1XTZS_Info : ImportInfoBase
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

        public int DS_D { get; set; }
        public int DS_S { get; set; }

        public int JO_J { get; set; }
        public int JO_O { get; set; }
        public int DX_D { get; set; }
        public int DX_X { get; set; }
        public int ZH_Z { get; set; }
        public int ZH_H { get; set; }

        public int DDan { get; set; }
        public int DS { get; set; }
        public int XDan { get; set; }
        public int XS { get; set; }

        public int C3_0 { get; set; }
        public int C3_1 { get; set; }
        public int C3_2 { get; set; }
    }
     
    public class GD11X5_Q1XTZS_InfoCollection : List<GD11X5_Q1XTZS_Info>
    {
    }
    /// <summary>
    /// 前2走势
    /// </summary>
     
    public class GD11X5_Q2JBZS_Info : ImportInfoBase
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

        public int JO_J { get; set; }
        public int JO_O { get; set; }
        public int DX_D { get; set; }
        public int DX_X { get; set; }
        public int ZH_Z { get; set; }
        public int ZH_H { get; set; }

        public int Red1_01 { get; set; }
        public int Red1_02 { get; set; }
        public int Red1_03 { get; set; }
        public int Red1_04 { get; set; }
        public int Red1_05 { get; set; }
        public int Red1_06 { get; set; }
        public int Red1_07 { get; set; }
        public int Red1_08 { get; set; }
        public int Red1_09 { get; set; }
        public int Red1_10 { get; set; }
        public int Red1_11 { get; set; }

        public int JO1_J { get; set; }
        public int JO1_O { get; set; }
        public int DX1_D { get; set; }
        public int DX1_X { get; set; }
        public int ZH1_Z { get; set; }
        public int ZH1_H { get; set; }
    }
     
    public class GD11X5_Q2JBZS_InfoCollection : List<GD11X5_Q2JBZS_Info>
    {
    }
    /// <summary>
    /// 前2形态走势
    /// </summary>
     
    public class GD11X5_Q2XTZS_Info : ImportInfoBase
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

        public int DX_DD { set; get; }
        public int DX_DX { set; get; }
        public int DX_XD { set; get; }
        public int DX_XX { set; get; }

        public int DS_DD { set; get; }
        public int DS_DS { set; get; }
        public int DS_SD { set; get; }
        public int DS_SS { set; get; }
    }
     
    public class GD11X5_Q2XTZS_InfoCollection : List<GD11X5_Q2XTZS_Info>
    {
    }
    /// <summary>
    /// 前3走势
    /// </summary>
     
    public class GD11X5_Q3JBZS_Info : ImportInfoBase
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

        public int Red1_01 { get; set; }
        public int Red1_02 { get; set; }
        public int Red1_03 { get; set; }
        public int Red1_04 { get; set; }
        public int Red1_05 { get; set; }
        public int Red1_06 { get; set; }
        public int Red1_07 { get; set; }
        public int Red1_08 { get; set; }
        public int Red1_09 { get; set; }
        public int Red1_10 { get; set; }
        public int Red1_11 { get; set; }

        public int Red2_01 { get; set; }
        public int Red2_02 { get; set; }
        public int Red2_03 { get; set; }
        public int Red2_04 { get; set; }
        public int Red2_05 { get; set; }
        public int Red2_06 { get; set; }
        public int Red2_07 { get; set; }
        public int Red2_08 { get; set; }
        public int Red2_09 { get; set; }
        public int Red2_10 { get; set; }
        public int Red2_11 { get; set; }
    }
     
    public class GD11X5_Q3JBZS_InfoCollection : List<GD11X5_Q3JBZS_Info>
    {
    }
    /// <summary>
    /// 前3形态走势
    /// </summary>
     
    public class GD11X5_Q3XTZS_Info : ImportInfoBase
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
     
    public class GD11X5_Q3XTZS_InfoCollection : List<GD11X5_Q3XTZS_Info>
    {
    }

     
    public class GD11X5_XTZS_Info : ImportInfoBase
    {
        public int DX_Q_D { get; set; }
        public int DX_1D4X { get; set; }
        public int DX_2D3X { get; set; }
        public int DX_3D2X { get; set; }
        public int DX_4D1X { get; set; }
        public int DX_Q_X { get; set; }

        public int JO_Q_J { get; set; }
        public int JO_1J4O { get; set; }
        public int JO_2J3O { get; set; }
        public int JO_3J2O { get; set; }
        public int JO_4J1O { get; set; }
        public int JO_Q_O { get; set; }

        public int ZH_Q_Z { get; set; }
        public int ZH_1Z4H { get; set; }
        public int ZH_2Z3H { get; set; }
        public int ZH_3Z2H { get; set; }
        public int ZH_4Z1H { get; set; }
        public int ZH_Q_H { get; set; }
    }
     
    public class GD11X5_XTZS_InfoCollection : List<GD11X5_XTZS_Info>
    {
    }
}
