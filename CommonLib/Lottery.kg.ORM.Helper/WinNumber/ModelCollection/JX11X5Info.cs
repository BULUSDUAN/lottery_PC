using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.ModelCollection;

namespace Lottery.Kg.ORM.Helper.WinNumber.ModelCollection
{
    /// <summary>
    /// 江西11选5任选基本走势info
    /// </summary>
     
    public class JX11X5_RXJBZS_Info : ImportInfoBase
    {
        /// <summary>
        /// 任选基本走势分布
        /// </summary>
        public int RXJB_01 { get; set; }
        public int RXJB_02 { get; set; }
        public int RXJB_03 { get; set; }
        public int RXJB_04 { get; set; }
        public int RXJB_05 { get; set; }
        public int RXJB_06 { get; set; }
        public int RXJB_07 { get; set; }
        public int RXJB_08 { get; set; }
        public int RXJB_09 { get; set; }
        public int RXJB_10 { get; set; }
        public int RXJB_11 { get; set; }

        /// <summary>
        /// 遗漏奇数的个数
        /// </summary>
        public int J_0 { get; set; }
        public int J_1 { get; set; }
        public int J_2 { get; set; }
        public int J_3 { get; set; }
        public int J_4 { get; set; }
        public int J_5 { get; set; }

        /// <summary>
        /// 遗漏小的个数
        /// </summary>
        public int X_0 { get; set; }
        public int X_1 { get; set; }
        public int X_2 { get; set; }
        public int X_3 { get; set; }
        public int X_4 { get; set; }
        public int X_5 { get; set; }

        /// <summary>
        /// 遗漏质数的个数
        /// </summary>
        public int Z_0 { get; set; }
        public int Z_1 { get; set; }
        public int Z_2 { get; set; }
        public int Z_3 { get; set; }
        public int Z_4 { get; set; }
        public int Z_5 { get; set; }

    }

     
    public class JX11X5_RXJBZS_InfoCollection : List<JX11X5_RXJBZS_Info>
    {
    }

    /// <summary>
    /// 江西11选5任选大小走势info
    /// </summary>
     
    public class JX11X5_RXDX_Info : ImportInfoBase
    {
        /// <summary>
        /// 任选基本走势分布
        /// </summary>
        public int NO1_D { get; set; }
        public int NO1_X { get; set; }
        public int NO2_D { get; set; }
        public int NO2_X { get; set; }
        public int NO3_D { get; set; }
        public int NO3_X { get; set; }
        public int NO4_D { get; set; }
        public int NO4_X { get; set; }
        public int NO5_D { get; set; }
        public int NO5_X { get; set; }

        public string DXqualifying { get; set; }
        public string DaoXiaoBi { get; set; }

        public int Bi5_0 { get; set; }
        public int Bi4_1 { get; set; }
        public int Bi3_2 { get; set; }
        public int Bi2_3 { get; set; }
        public int Bi1_4 { get; set; }
        public int Bi0_5 { get; set; }
    }

     
    public class JX11X5_RXDX_InfoCollection : List<JX11X5_RXDX_Info>
    {
    }





    /// <summary>
    /// 江西11选5任选奇偶走势info
    /// </summary>
     
    public class JX11X5_RXJO_Info : ImportInfoBase
    {
        /// <summary>
        /// 任选奇偶走势分布
        /// </summary>
        public int NO1_J { get; set; }
        public int NO1_O { get; set; }
        public int NO2_J { get; set; }
        public int NO2_O { get; set; }
        public int NO3_J { get; set; }
        public int NO3_O { get; set; }
        public int NO4_J { get; set; }
        public int NO4_O { get; set; }
        public int NO5_J { get; set; }
        public int NO5_O { get; set; }

        public string JOqualifying { get; set; }
        public string JiOuBi { get; set; }

        public int Bi5_0 { get; set; }
        public int Bi4_1 { get; set; }
        public int Bi3_2 { get; set; }
        public int Bi2_3 { get; set; }
        public int Bi1_4 { get; set; }
        public int Bi0_5 { get; set; }
    }

     
    public class JX11X5_RXJO_InfoCollection : List<JX11X5_RXJO_Info>
    {

    }

    /// <summary>
    /// 江西11选5任选质和走势info
    /// </summary>
     
    public class JX11X5_RXZH_Info : ImportInfoBase
    {
        /// <summary>
        /// 任选质和走势分布
        /// </summary>
        public int NO1_Z { get; set; }
        public int NO1_H { get; set; }
        public int NO2_Z { get; set; }
        public int NO2_H { get; set; }
        public int NO3_Z { get; set; }
        public int NO3_H { get; set; }
        public int NO4_Z { get; set; }
        public int NO4_H { get; set; }
        public int NO5_Z { get; set; }
        public int NO5_H { get; set; }

        public string ZHqualifying { get; set; }
        public string ZhiHeBi { get; set; }

        public int Bi5_0 { get; set; }
        public int Bi4_1 { get; set; }
        public int Bi3_2 { get; set; }
        public int Bi2_3 { get; set; }
        public int Bi1_4 { get; set; }
        public int Bi0_5 { get; set; }

    }

     
    public class JX11X5_RXZH_InfoCollection : List<JX11X5_RXZH_Info>
    {
    }

    /// <summary>
    /// 江西11选5任选和值走势info
    /// </summary>
     
    public class JX11X5_RXHZ_Info : ImportInfoBase
    {
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

     
    public class JX11X5_RXHZ_InfoCollection : List<JX11X5_RXHZ_Info>
    {

    }

    /// <summary>
    /// 江西11选5任选除3走势info
    /// </summary>
     
    public class JX11X5_Chu3_Info : ImportInfoBase
    {
        /// <summary>
        /// 除3余数比
        /// </summary>
        public string Chu3Bi { get; set; }
        /// <summary>
        /// 除3余数遗漏
        /// </summary>
        public int NO1_0 { get; set; }
        public int NO1_1 { get; set; }
        public int NO1_2 { get; set; }
        public int NO2_0 { get; set; }
        public int NO2_1 { get; set; }
        public int NO2_2 { get; set; }
        public int NO3_0 { get; set; }
        public int NO3_1 { get; set; }
        public int NO3_2 { get; set; }
        public int NO4_0 { get; set; }
        public int NO4_1 { get; set; }
        public int NO4_2 { get; set; }
        public int NO5_0 { get; set; }
        public int NO5_1 { get; set; }
        public int NO5_2 { get; set; }
        /// <summary>
        /// 除3余数个数
        /// </summary>
        public int Yu0_0 { get; set; }
        public int Yu0_1 { get; set; }
        public int Yu0_2 { get; set; }
        public int Yu0_3 { get; set; }
        public int Yu0_4 { get; set; }
        public int Yu0_5 { get; set; }

        public int Yu1_0 { get; set; }
        public int Yu1_1 { get; set; }
        public int Yu1_2 { get; set; }
        public int Yu1_3 { get; set; }
        public int Yu1_4 { get; set; }
        public int Yu1_5 { get; set; }

        public int Yu2_0 { get; set; }
        public int Yu2_1 { get; set; }
        public int Yu2_2 { get; set; }
        public int Yu2_3 { get; set; }
        public int Yu2_4 { get; set; }
        public int Yu2_5 { get; set; }
    }

     
    public class JX11X5_Chu3_InfoCollection : List<JX11X5_Chu3_Info>
    {
    }

    /// <summary>
    /// 江西11选5任选第一位走势info
    /// </summary>
     
    public class JX11X5_RX1_Info : ImportInfoBase
    {
        /// <summary>
        /// 任选基本走势分布
        /// </summary>
        public int NO_1 { get; set; }
        public int NO_2 { get; set; }
        public int NO_3 { get; set; }
        public int NO_4 { get; set; }
        public int NO_5 { get; set; }
        public int NO_6 { get; set; }
        public int NO_7 { get; set; }
        public int NO_8 { get; set; }
        public int NO_9 { get; set; }
        public int NO_10 { get; set; }
        public int NO_11 { get; set; }

        public int DX_D { get; set; }
        public int DX_X { get; set; }

        public int JO_J { get; set; }
        public int JO_O { get; set; }

        public int ZH_Z { get; set; }
        public int ZH_H { get; set; }

        public int Yu_0 { get; set; }
        public int Yu_1 { get; set; }
        public int Yu_2 { get; set; }
    }

     
    public class JX11X5_RX1_InfoCollection : List<JX11X5_RX1_Info>
    {
    }

    /// <summary>
    /// 江西11选5任选第二位走势info
    /// </summary>
     
    public class JX11X5_RX2_Info : ImportInfoBase
    {
        /// <summary>
        /// 任选基本走势分布
        /// </summary>
        public int NO_1 { get; set; }
        public int NO_2 { get; set; }
        public int NO_3 { get; set; }
        public int NO_4 { get; set; }
        public int NO_5 { get; set; }
        public int NO_6 { get; set; }
        public int NO_7 { get; set; }
        public int NO_8 { get; set; }
        public int NO_9 { get; set; }
        public int NO_10 { get; set; }
        public int NO_11 { get; set; }

        public int DX_D { get; set; }
        public int DX_X { get; set; }

        public int JO_J { get; set; }
        public int JO_O { get; set; }

        public int ZH_Z { get; set; }
        public int ZH_H { get; set; }

        public int Yu_0 { get; set; }
        public int Yu_1 { get; set; }
        public int Yu_2 { get; set; }
    }

     
    public class JX11X5_RX2_InfoCollection : List<JX11X5_RX2_Info>
    {
    }


    /// <summary>
    /// 江西11选5任选第三位走势info
    /// </summary>
     
    public class JX11X5_RX3_Info : ImportInfoBase
    {
        /// <summary>
        /// 任选基本走势分布
        /// </summary>
        public int NO_1 { get; set; }
        public int NO_2 { get; set; }
        public int NO_3 { get; set; }
        public int NO_4 { get; set; }
        public int NO_5 { get; set; }
        public int NO_6 { get; set; }
        public int NO_7 { get; set; }
        public int NO_8 { get; set; }
        public int NO_9 { get; set; }
        public int NO_10 { get; set; }
        public int NO_11 { get; set; }

        public int DX_D { get; set; }
        public int DX_X { get; set; }

        public int JO_J { get; set; }
        public int JO_O { get; set; }

        public int ZH_Z { get; set; }
        public int ZH_H { get; set; }

        public int Yu_0 { get; set; }
        public int Yu_1 { get; set; }
        public int Yu_2 { get; set; }
    }

     
    public class JX11X5_RX3_InfoCollection : List<JX11X5_RX3_Info>
    {
    }


    /// <summary>
    /// 江西11选5任选第四位走势info
    /// </summary>
     
    public class JX11X5_RX4_Info : ImportInfoBase
    {
        /// <summary>
        /// 任选基本走势分布
        /// </summary>
        public int NO_1 { get; set; }
        public int NO_2 { get; set; }
        public int NO_3 { get; set; }
        public int NO_4 { get; set; }
        public int NO_5 { get; set; }
        public int NO_6 { get; set; }
        public int NO_7 { get; set; }
        public int NO_8 { get; set; }
        public int NO_9 { get; set; }
        public int NO_10 { get; set; }
        public int NO_11 { get; set; }

        public int DX_D { get; set; }
        public int DX_X { get; set; }

        public int JO_J { get; set; }
        public int JO_O { get; set; }

        public int ZH_Z { get; set; }
        public int ZH_H { get; set; }

        public int Yu_0 { get; set; }
        public int Yu_1 { get; set; }
        public int Yu_2 { get; set; }
    }

     
    public class JX11X5_RX4_InfoCollection : List<JX11X5_RX4_Info>
    {
    }


    /// <summary>
    /// 江西11选5任选第五位走势info
    /// </summary>
     
    public class JX11X5_RX5_Info : ImportInfoBase
    {
        /// <summary>
        /// 任选基本走势分布
        /// </summary>
        public int NO_1 { get; set; }
        public int NO_2 { get; set; }
        public int NO_3 { get; set; }
        public int NO_4 { get; set; }
        public int NO_5 { get; set; }
        public int NO_6 { get; set; }
        public int NO_7 { get; set; }
        public int NO_8 { get; set; }
        public int NO_9 { get; set; }
        public int NO_10 { get; set; }
        public int NO_11 { get; set; }

        public int DX_D { get; set; }
        public int DX_X { get; set; }

        public int JO_J { get; set; }
        public int JO_O { get; set; }

        public int ZH_Z { get; set; }
        public int ZH_H { get; set; }

        public int Yu_0 { get; set; }
        public int Yu_1 { get; set; }
        public int Yu_2 { get; set; }
    }

     
    public class JX11X5_RX5_InfoCollection : List<JX11X5_RX5_Info>
    {
    }

    /// <summary>
    /// 江西11选5任选基本走势info
    /// </summary>
     
    public class JX11X5_Q3ZS_Info : ImportInfoBase
    {
        public int WW_1 { get; set; }
        public int WW_2 { get; set; }
        public int WW_3 { get; set; }
        public int WW_4 { get; set; }
        public int WW_5 { get; set; }
        public int WW_6 { get; set; }
        public int WW_7 { get; set; }
        public int WW_8 { get; set; }
        public int WW_9 { get; set; }
        public int WW_10 { get; set; }
        public int WW_11 { get; set; }

        public int QW_1 { get; set; }
        public int QW_2 { get; set; }
        public int QW_3 { get; set; }
        public int QW_4 { get; set; }
        public int QW_5 { get; set; }
        public int QW_6 { get; set; }
        public int QW_7 { get; set; }
        public int QW_8 { get; set; }
        public int QW_9 { get; set; }
        public int QW_10 { get; set; }
        public int QW_11 { get; set; }

        public int BW_1 { get; set; }
        public int BW_2 { get; set; }
        public int BW_3 { get; set; }
        public int BW_4 { get; set; }
        public int BW_5 { get; set; }
        public int BW_6 { get; set; }
        public int BW_7 { get; set; }
        public int BW_8 { get; set; }
        public int BW_9 { get; set; }
        public int BW_10 { get; set; }
        public int BW_11 { get; set; }

        public int DXBi3_0 { get; set; }
        public int DXBi2_1 { get; set; }
        public int DXBi1_2 { get; set; }
        public int DXBi0_3 { get; set; }

        public int JOBi3_0 { get; set; }
        public int JOBi2_1 { get; set; }
        public int JOBi1_2 { get; set; }
        public int JOBi0_3 { get; set; }
    }

     
    public class JX11X5_Q3ZS_InfoCollection : List<JX11X5_Q3ZS_Info>
    {
    }

    /// <summary>
    /// 江西11选5任选基本走势info
    /// </summary>
     
    public class JX11X5_Q3ZUS_Info : ImportInfoBase
    {
        public int Q3_1 { get; set; }
        public int Q3_2 { get; set; }
        public int Q3_3 { get; set; }
        public int Q3_4 { get; set; }
        public int Q3_5 { get; set; }
        public int Q3_6 { get; set; }
        public int Q3_7 { get; set; }
        public int Q3_8 { get; set; }
        public int Q3_9 { get; set; }
        public int Q3_10 { get; set; }
        public int Q3_11 { get; set; }

        public int DXBi3_0 { get; set; }
        public int DXBi2_1 { get; set; }
        public int DXBi1_2 { get; set; }
        public int DXBi0_3 { get; set; }

        public int JOBi3_0 { get; set; }
        public int JOBi2_1 { get; set; }
        public int JOBi1_2 { get; set; }
        public int JOBi0_3 { get; set; }

        public int ZHBi3_0 { get; set; }
        public int ZHBi2_1 { get; set; }
        public int ZHBi1_2 { get; set; }
        public int ZHBi0_3 { get; set; }
    }

     
    public class JX11X5_Q3ZUS_InfoCollection : List<JX11X5_Q3ZUS_Info>
    {
    }

    /// <summary>
    /// 江西11选5任选基本走势info
    /// </summary>
     
    public class JX11X5_Q3DX_Info : ImportInfoBase
    {
        /// <summary>
        ///位置大小
        /// </summary>
        public int NO1_D { get; set; }
        public int NO1_X { get; set; }
        public int NO2_D { get; set; }
        public int NO2_X { get; set; }
        public int NO3_D { get; set; }
        public int NO3_X { get; set; }
        /// <summary>
        /// 大小比
        /// </summary>
        public int Bi3_0 { get; set; }
        public int Bi2_1 { get; set; }
        public int Bi1_2 { get; set; }
        public int Bi0_3 { get; set; }
        /// <summary>
        /// 大大大
        /// </summary>
        public int DX_DDD { get; set; }
        public int DX_DDX { get; set; }
        public int DX_DXD { get; set; }
        public int DX_XDD { get; set; }
        public int DX_DXX { get; set; }
        public int DX_XDX { get; set; }
        public int DX_XXD { get; set; }
        public int DX_XXX { get; set; }
    }

     
    public class JX11X5_Q3DX_InfoCollection : List<JX11X5_Q3DX_Info>
    {
    }


    /// <summary>
    /// 江西11选5任选基本走势info
    /// </summary>
     
    public class JX11X5_Q3JO_Info : ImportInfoBase
    {
        /// <summary>
        ///位置大小
        /// </summary>
        public int NO1_J { get; set; }
        public int NO1_O { get; set; }
        public int NO2_J { get; set; }
        public int NO2_O { get; set; }
        public int NO3_J { get; set; }
        public int NO3_O { get; set; }
        /// <summary>
        /// 大小比
        /// </summary>
        public int Bi3_0 { get; set; }
        public int Bi2_1 { get; set; }
        public int Bi1_2 { get; set; }
        public int Bi0_3 { get; set; }
        /// <summary>
        /// 奇奇奇
        /// </summary>
        public int JO_JJJ { get; set; }
        public int JO_JJO { get; set; }
        public int JO_JOJ { get; set; }
        public int JO_OJJ { get; set; }
        public int JO_JOO { get; set; }
        public int JO_OJO { get; set; }
        public int JO_OOJ { get; set; }
        public int JO_OOO { get; set; }
    }

     
    public class JX11X5_Q3JO_InfoCollection : List<JX11X5_Q3JO_Info>
    {
    }


    /// <summary>
    /// 江西11选5前3质和走势info
    /// </summary>
     
    public class JX11X5_Q3ZH_Info : ImportInfoBase
    {
        /// <summary>
        ///位置质和
        /// </summary>
        public int NO1_Z { get; set; }
        public int NO1_H { get; set; }
        public int NO2_Z { get; set; }
        public int NO2_H { get; set; }
        public int NO3_Z { get; set; }
        public int NO3_H { get; set; }
        /// <summary>
        /// 质和比
        /// </summary>
        public int Bi3_0 { get; set; }
        public int Bi2_1 { get; set; }
        public int Bi1_2 { get; set; }
        public int Bi0_3 { get; set; }
        /// <summary>
        /// 质质质
        /// </summary>
        public int ZH_ZZZ { get; set; }
        public int ZH_ZZH { get; set; }
        public int ZH_ZHZ { get; set; }
        public int ZH_HZZ { get; set; }
        public int ZH_ZHH { get; set; }
        public int ZH_HZH { get; set; }
        public int ZH_HHZ { get; set; }
        public int ZH_HHH { get; set; }
    }

     
    public class JX11X5_Q3ZH_InfoCollection : List<JX11X5_Q3ZH_Info>
    {
    }

    /// <summary>
    /// 江西11选5前3除3走势info
    /// </summary>
     
    public class JX11X5_Q3Chu3_Info : ImportInfoBase
    {
        /// <summary>
        /// 除3余数遗漏
        /// </summary>
        public int NO1_0 { get; set; }
        public int NO1_1 { get; set; }
        public int NO1_2 { get; set; }
        public int NO2_0 { get; set; }
        public int NO2_1 { get; set; }
        public int NO2_2 { get; set; }
        public int NO3_0 { get; set; }
        public int NO3_1 { get; set; }
        public int NO3_2 { get; set; }
        /// <summary>
        /// 除3余数比
        /// </summary>
        public virtual string Chu3Bi { get; set; }
        /// <summary>
        /// 除3余数个数
        /// </summary>
        public int Yu0_0 { get; set; }
        public int Yu0_1 { get; set; }
        public int Yu0_2 { get; set; }
        public int Yu0_3 { get; set; }

        public int Yu1_0 { get; set; }
        public int Yu1_1 { get; set; }
        public int Yu1_2 { get; set; }
        public int Yu1_3 { get; set; }

        public int Yu2_0 { get; set; }
        public int Yu2_1 { get; set; }
        public int Yu2_2 { get; set; }
        public int Yu2_3 { get; set; }

    }

     
    public class JX11X5_Q3Chu3_InfoCollection : List<JX11X5_Q3Chu3_Info>
    {
    }

    /// <summary>
    /// 江西11选5前3和值走势info
    /// </summary>
     
    public class JX11X5_Q3HZ_Info : ImportInfoBase
    {
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

        public int Yu_0 { get; set; }
        public int Yu_1 { get; set; }
        public int Yu_2 { get; set; }
    }

     
    public class JX11X5_Q3HZ_InfoCollection : List<JX11X5_Q3HZ_Info>
    {
    }

    /// <summary>
    /// 江西11选5前2直选走势info
    /// </summary>
     
    public class JX11X5_Q2ZS_Info : ImportInfoBase
    {
        /// <summary>
        /// 实际是开奖号码第一位（万位）
        /// </summary>
        public int WW_1 { get; set; }
        public int WW_2 { get; set; }
        public int WW_3 { get; set; }
        public int WW_4 { get; set; }
        public int WW_5 { get; set; }
        public int WW_6 { get; set; }
        public int WW_7 { get; set; }
        public int WW_8 { get; set; }
        public int WW_9 { get; set; }
        public int WW_10 { get; set; }
        public int WW_11 { get; set; }
        /// <summary>
        /// 实际是开奖号码第一位（千位）
        /// </summary>
        public int QW_1 { get; set; }
        public int QW_2 { get; set; }
        public int QW_3 { get; set; }
        public int QW_4 { get; set; }
        public int QW_5 { get; set; }
        public int QW_6 { get; set; }
        public int QW_7 { get; set; }
        public int QW_8 { get; set; }
        public int QW_9 { get; set; }
        public int QW_10 { get; set; }
        public int QW_11 { get; set; }

        public int DXBi2_0 { get; set; }
        public int DXBi1_1 { get; set; }
        public int DXBi0_2 { get; set; }

        public int JOBi2_0 { get; set; }
        public int JOBi1_1 { get; set; }
        public int JOBi0_2 { get; set; }
    }

     
    public class JX11X5_Q2ZS_InfoCollection : List<JX11X5_Q2ZS_Info>
    {
    }

    /// <summary>
    /// 江西11选5前2组选走势info
    /// </summary>
     
    public class JX11X5_Q2ZUS_Info : ImportInfoBase
    {
        public int Q2_1 { get; set; }
        public int Q2_2 { get; set; }
        public int Q2_3 { get; set; }
        public int Q2_4 { get; set; }
        public int Q2_5 { get; set; }
        public int Q2_6 { get; set; }
        public int Q2_7 { get; set; }
        public int Q2_8 { get; set; }
        public int Q2_9 { get; set; }
        public int Q2_10 { get; set; }
        public int Q2_11 { get; set; }

        public int DXBi2_0 { get; set; }
        public int DXBi1_1 { get; set; }
        public int DXBi0_2 { get; set; }

        public int JOBi2_0 { get; set; }
        public int JOBi1_1 { get; set; }
        public int JOBi0_2 { get; set; }

        public int ZHBi2_0 { get; set; }
        public int ZHBi1_1 { get; set; }
        public int ZHBi0_2 { get; set; }


        public int Yu0_0 { get; set; }
        public int Yu0_1 { get; set; }
        public int Yu0_2 { get; set; }

        public int Yu1_0 { get; set; }
        public int Yu1_1 { get; set; }
        public int Yu1_2 { get; set; }

        public int Yu2_0 { get; set; }
        public int Yu2_1 { get; set; }
        public int Yu2_2 { get; set; }

    }

     
    public class JX11X5_Q2ZUS_InfoCollection : List<JX11X5_Q2ZUS_Info>
    {

    }

    /// <summary>
    /// 江西11选5前2和值走势info
    /// </summary>
     
    public class JX11X5_Q2HZ_Info : ImportInfoBase
    {
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

        public int DX_D { get; set; }
        public int DX_X { get; set; }

        public int JO_J { get; set; }
        public int JO_O { get; set; }

        public int ZH_Z { get; set; }
        public int ZH_H { get; set; }

        public int Yu_0 { get; set; }
        public int Yu_1 { get; set; }
        public int Yu_2 { get; set; }
    }

     
    public class JX11X5_Q2HZ_InfoCollection : List<JX11X5_Q2HZ_Info>
    {

    }
}
