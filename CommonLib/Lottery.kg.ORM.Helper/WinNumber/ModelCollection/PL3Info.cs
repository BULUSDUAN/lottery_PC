using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.ModelCollection;

namespace Lottery.Kg.ORM.Helper.WinNumber.ModelCollection
{
    /// <summary>
    /// 排列3基本走势info
    /// </summary>
     
    public class PL3_JiBenZouSi_Info : ImportInfoBase
    {
        /// <summary>
        /// 组三
        /// </summary>
        public int ZH_3 { get; set; }
        public int ZH_6 { get; set; }
        public int ZH_Baozi { get; set; }
        /// <summary>
        /// 百十个位置的遗漏
        /// </summary>
        public int BW_0 { get; set; }
        public int BW_1 { get; set; }
        public int BW_2 { get; set; }
        public int BW_3 { get; set; }
        public int BW_4 { get; set; }
        public int BW_5 { get; set; }
        public int BW_6 { get; set; }
        public int BW_7 { get; set; }
        public int BW_8 { get; set; }
        public int BW_9 { get; set; }
        public int SW_0 { get; set; }
        public int SW_1 { get; set; }
        public int SW_2 { get; set; }
        public int SW_3 { get; set; }
        public int SW_4 { get; set; }
        public int SW_5 { get; set; }
        public int SW_6 { get; set; }
        public int SW_7 { get; set; }
        public int SW_8 { get; set; }
        public int SW_9 { get; set; }
        public int GW_0 { get; set; }
        public int GW_1 { get; set; }
        public int GW_2 { get; set; }
        public int GW_3 { get; set; }
        public int GW_4 { get; set; }
        public int GW_5 { get; set; }
        public int GW_6 { get; set; }
        public int GW_7 { get; set; }
        public int GW_8 { get; set; }
        public int GW_9 { get; set; }

    }

     
    public class PL3_JiBenZouSi_InfoCollection : List<PL3_JiBenZouSi_Info>
    {
    }



    /// <summary>
    /// 排列3组选走势info
    /// </summary>
     
    public class PL3_ZuXuanZouSi_Info : ImportInfoBase
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

     
    public class PL3_ZuXuanZouSi_InfoCollection : List<PL3_ZuXuanZouSi_Info>
    {
    }





    /// <summary>
    /// 排列3组选走势info
    /// </summary>
     
    public class PL3_DX_Info : ImportInfoBase
    {
        /// <summary>
        /// 大小形态
        /// </summary>
        public string DXType { get; set; }
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
        /// <summary>
        /// 大小比
        /// </summary>
        public string DaoXiaoBi { get; set; }
        /// <summary>
        /// 大小比0比5
        /// </summary>
        public int Bi3_0 { get; set; }
        public int Bi2_1 { get; set; }
        public int Bi1_2 { get; set; }
        public int Bi0_3 { get; set; }
    }

     
    public class PL3_DX_InfoCollection : List<PL3_DX_Info>
    {
    }


    /// <summary>
    /// 排列3大小号码走势info
    /// </summary>
     
    public class PL3_DXHM_Info : ImportInfoBase
    {
        /// <summary>
        /// 百十个位置的遗漏
        /// </summary>
        public int BW_0 { get; set; }
        public int BW_1 { get; set; }
        public int BW_2 { get; set; }
        public int BW_3 { get; set; }
        public int BW_4 { get; set; }
        public int BW_5 { get; set; }
        public int BW_6 { get; set; }
        public int BW_7 { get; set; }
        public int BW_8 { get; set; }
        public int BW_9 { get; set; }
        public int SW_0 { get; set; }
        public int SW_1 { get; set; }
        public int SW_2 { get; set; }
        public int SW_3 { get; set; }
        public int SW_4 { get; set; }
        public int SW_5 { get; set; }
        public int SW_6 { get; set; }
        public int SW_7 { get; set; }
        public int SW_8 { get; set; }
        public int SW_9 { get; set; }
        public int GW_0 { get; set; }
        public int GW_1 { get; set; }
        public int GW_2 { get; set; }
        public int GW_3 { get; set; }
        public int GW_4 { get; set; }
        public int GW_5 { get; set; }
        public int GW_6 { get; set; }
        public int GW_7 { get; set; }
        public int GW_8 { get; set; }
        public int GW_9 { get; set; }
        /// <summary>
        /// 大小比
        /// </summary>
        public string DaoXiaoBi { get; set; }
        /// <summary>
        /// 大小比0比5
        /// </summary>
        public int Bi3_0 { get; set; }
        public int Bi2_1 { get; set; }
        public int Bi1_2 { get; set; }
        public int Bi0_3 { get; set; }

    }

     
    public class PL3_DXHM_InfoCollection : List<PL3_DXHM_Info>
    {
    }


    /// <summary>
    ///  奇偶走势info
    /// </summary>
     
    public class PL3_JIOU_Info : ImportInfoBase
    {
        /// <summary>
        /// 大小形态
        /// </summary>
        public string JOType { get; set; }
        /// <summary>
        /// 大大大
        /// </summary>
        public int JO_JJJ { get; set; }
        public int JO_JJO { get; set; }
        public int JO_JOJ { get; set; }
        public int JO_OJJ { get; set; }
        public int JO_JOO { get; set; }
        public int JO_OJO { get; set; }
        public int JO_OOJ { get; set; }
        public int JO_OOO { get; set; }
        /// <summary> 
        /// 大小比
        /// </summary>
        public string JOBi { get; set; }
        /// <summary>
        /// 大小比0比5
        /// </summary>
        public int Bi3_0 { get; set; }
        public int Bi2_1 { get; set; }
        public int Bi1_2 { get; set; }
        public int Bi0_3 { get; set; }

    }

     
    public class PL3_JIOU_InfoCollection : List<PL3_JIOU_Info>
    {
    }


    /// <summary>
    /// 排列3奇偶大小号码走势info
    /// </summary>
     
    public class PL3_JOHM_Info : ImportInfoBase
    {
        /// <summary>
        /// 百十个位置的遗漏
        /// </summary>
        public int BW_0 { get; set; }
        public int BW_1 { get; set; }
        public int BW_2 { get; set; }
        public int BW_3 { get; set; }
        public int BW_4 { get; set; }
        public int BW_5 { get; set; }
        public int BW_6 { get; set; }
        public int BW_7 { get; set; }
        public int BW_8 { get; set; }
        public int BW_9 { get; set; }
        public int SW_0 { get; set; }
        public int SW_1 { get; set; }
        public int SW_2 { get; set; }
        public int SW_3 { get; set; }
        public int SW_4 { get; set; }
        public int SW_5 { get; set; }
        public int SW_6 { get; set; }
        public int SW_7 { get; set; }
        public int SW_8 { get; set; }
        public int SW_9 { get; set; }
        public int GW_0 { get; set; }
        public int GW_1 { get; set; }
        public int GW_2 { get; set; }
        public int GW_3 { get; set; }
        public int GW_4 { get; set; }
        public int GW_5 { get; set; }
        public int GW_6 { get; set; }
        public int GW_7 { get; set; }
        public int GW_8 { get; set; }
        public int GW_9 { get; set; }
        /// <summary>
        /// 大小比
        /// </summary> 
        public string JOBi { get; set; }
        /// <summary>
        /// 大小比0比5
        /// </summary>
        public int Bi3_0 { get; set; }
        public int Bi2_1 { get; set; }
        public int Bi1_2 { get; set; }
        public int Bi0_3 { get; set; }
    }

     
    public class PL3_JOHM_InfoCollection : List<PL3_JOHM_Info>
    {
    }

    /// <summary>
    /// 排列3质和走势info
    /// </summary>
     
    public class PL3_ZhiHe_Info : ImportInfoBase
    {
        /// <summary>
        /// 大小形态
        /// </summary>
        public string ZHType { get; set; }
        /// <summary>
        /// 大大大
        /// </summary>
        public int ZH_ZZZ { get; set; }
        public int ZH_ZZH { get; set; }
        public int ZH_ZHZ { get; set; }
        public int ZH_HZZ { get; set; }
        public int ZH_ZHH { get; set; }
        public int ZH_HZH { get; set; }
        public int ZH_HHZ { get; set; }
        public int ZH_HHH { get; set; }
        /// <summary> 
        /// 大小比
        /// </summary>
        public string ZHBi { get; set; }
        /// <summary>
        /// 大小比0比5 
        /// </summary>
        public int Bi3_0 { get; set; }
        public int Bi2_1 { get; set; }
        public int Bi1_2 { get; set; }
        public int Bi0_3 { get; set; }
    }

     
    public class PL3_ZhiHe_InfoCollection : List<PL3_ZhiHe_Info>
    {
    }


    /// <summary>
    /// 排列3组选走势info
    /// </summary>
     
    public class PL3_ZHHM_Info : ImportInfoBase
    {
        /// <summary>
        /// 百十个位置的遗漏
        /// </summary>
        public int BW_0 { get; set; }
        public int BW_1 { get; set; }
        public int BW_2 { get; set; }
        public int BW_3 { get; set; }
        public int BW_4 { get; set; }
        public int BW_5 { get; set; }
        public int BW_6 { get; set; }
        public int BW_7 { get; set; }
        public int BW_8 { get; set; }
        public int BW_9 { get; set; }
        public int SW_0 { get; set; }
        public int SW_1 { get; set; }
        public int SW_2 { get; set; }
        public int SW_3 { get; set; }
        public int SW_4 { get; set; }
        public int SW_5 { get; set; }
        public int SW_6 { get; set; }
        public int SW_7 { get; set; }
        public int SW_8 { get; set; }
        public int SW_9 { get; set; }
        public int GW_0 { get; set; }
        public int GW_1 { get; set; }
        public int GW_2 { get; set; }
        public int GW_3 { get; set; }
        public int GW_4 { get; set; }
        public int GW_5 { get; set; }
        public int GW_6 { get; set; }
        public int GW_7 { get; set; }
        public int GW_8 { get; set; }
        public int GW_9 { get; set; }
        /// <summary>
        /// 大小比
        /// </summary> 
        public string ZHBi { get; set; }
        /// <summary>
        /// 大小比0比5 
        /// </summary>
        public int Bi3_0 { get; set; }
        public int Bi2_1 { get; set; }
        public int Bi1_2 { get; set; }
        public int Bi0_3 { get; set; }
    }

     
    public class PL3_ZHHM_InfoCollection : List<PL3_ZHHM_Info>
    {
    }

    /// <summary>
    /// 排列3组选走势info
    /// </summary>
     
    public class PL3_HeiZhi_Info : ImportInfoBase
    {
        /// <summary> 
        /// 和值
        /// </summary>
        public int HZ { get; set; }
        /// <summary>
        /// 和值分布
        /// </summary>
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
        /// 合尾分布
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

     
    public class PL3_HeiZhi_InfoCollection : List<PL3_HeiZhi_Info>
    {
    }

    /// <summary>
    /// 排列3跨度12走势info
    /// </summary>
     
    public class PL3_KuaDu_12_Info : ImportInfoBase
    {
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
        /// <summary>
        /// 跨度大
        /// </summary>
        public int KDDX_D { get; set; }
        public int KDDX_X { get; set; }
        /// <summary>
        /// 跨度奇
        /// </summary>
        public int KDJ0_J { get; set; }
        public int KDJ0_O { get; set; }
        /// <summary>
        /// 跨度和
        /// </summary>
        public int KDZH_Z { get; set; }
        public int KDZH_H { get; set; }
        /// <summary>
        /// 除3余数
        /// </summary>
        public int KDChu3_0 { get; set; }
        public int KDChu3_1 { get; set; }
        public int KDChu3_2 { get; set; }
        /// <summary>
        /// 除4余数
        /// </summary>
        public int KDChu4_0 { get; set; }
        public int KDChu4_1 { get; set; }
        public int KDChu4_2 { get; set; }
        public int KDChu4_3 { get; set; }
        /// <summary>
        /// 除5余数
        /// </summary>
        public int KDChu5_0 { get; set; }
        public int KDChu5_1 { get; set; }
        public int KDChu5_2 { get; set; }
        public int KDChu5_3 { get; set; }
        public int KDChu5_4 { get; set; }
    }

     
    public class PL3_KuaDu_12_InfoCollection : List<PL3_KuaDu_12_Info>
    {
    }

    /// <summary>
    /// 排列3 跨度百位、个位走势info
    /// </summary>
     
    public class PL3_KuaDu_13_Info : ImportInfoBase
    {
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
        /// <summary>
        /// 跨度大
        /// </summary>
        public int KDDX_D { get; set; }
        public int KDDX_X { get; set; }
        /// <summary>
        /// 跨度奇
        /// </summary>
        public int KDJ0_J { get; set; }
        public int KDJ0_O { get; set; }
        /// <summary>
        /// 跨度和
        /// </summary>
        public int KDZH_Z { get; set; }
        public int KDZH_H { get; set; }
        /// <summary>
        /// 除3余数
        /// </summary>
        public int KDChu3_0 { get; set; }
        public int KDChu3_1 { get; set; }
        public int KDChu3_2 { get; set; }
        /// <summary>
        /// 除4余数
        /// </summary>
        public int KDChu4_0 { get; set; }
        public int KDChu4_1 { get; set; }
        public int KDChu4_2 { get; set; }
        public int KDChu4_3 { get; set; }
        /// <summary>
        /// 除5余数
        /// </summary>
        public int KDChu5_0 { get; set; }
        public int KDChu5_1 { get; set; }
        public int KDChu5_2 { get; set; }
        public int KDChu5_3 { get; set; }
        public int KDChu5_4 { get; set; }
    }

     
    public class PL3_KuaDu_13_InfoCollection : List<PL3_KuaDu_13_Info>
    {
    }

    /// <summary>
    /// 排列3跨度十位、个位走势info
    /// </summary>
     
    public class PL3_KuaDu_23_Info : ImportInfoBase
    {
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
        /// <summary>
        /// 跨度大
        /// </summary>
        public int KDDX_D { get; set; }
        public int KDDX_X { get; set; }
        /// <summary>
        /// 跨度奇
        /// </summary>
        public int KDJ0_J { get; set; }
        public int KDJ0_O { get; set; }
        /// <summary>
        /// 跨度和
        /// </summary>
        public int KDZH_Z { get; set; }
        public int KDZH_H { get; set; }
        /// <summary>
        /// 除3余数
        /// </summary>
        public int KDChu3_0 { get; set; }
        public int KDChu3_1 { get; set; }
        public int KDChu3_2 { get; set; }
        /// <summary>
        /// 除4余数
        /// </summary>
        public int KDChu4_0 { get; set; }
        public int KDChu4_1 { get; set; }
        public int KDChu4_2 { get; set; }
        public int KDChu4_3 { get; set; }
        /// <summary>
        /// 除5余数
        /// </summary>
        public int KDChu5_0 { get; set; }
        public int KDChu5_1 { get; set; }
        public int KDChu5_2 { get; set; }
        public int KDChu5_3 { get; set; }
        public int KDChu5_4 { get; set; }
    }

     
    public class PL3_KuaDu_23_InfoCollection : List<PL3_KuaDu_23_Info>
    {
    }




    /// <summary>
    /// 排列3组选走势info
    /// </summary>
     
    public class PL3_Chu31_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }

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
        /// 012比例
        /// </summary>
        public string P012_Proportion { set; get; }


        public int O_P012_Proportion300 { set; get; }
        public int O_P012_Proportion210 { set; get; }
        public int O_P012_Proportion201 { set; get; }
        public int O_P012_Proportion120 { set; get; }
        public int O_P012_Proportion111 { set; get; }
        public int O_P012_Proportion102 { set; get; }
        public int O_P012_Proportion030 { set; get; }
        public int O_P012_Proportion021 { set; get; }
        public int O_P012_Proportion012 { set; get; }
        public int O_P012_Proportion003 { set; get; }
    }

     
    public class PL3_Chu31_InfoCollection : List<PL3_Chu31_Info>
    {
    }

    /// <summary>
    /// 排列3组选走势info
    /// </summary>
     
    public class PL3_Chu32_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }

        public int C_AAA_000 { get; set; }
        public int C_AAA_111 { get; set; }
        public int C_AAA_222 { get; set; }

        public int C_AAB_001 { get; set; }
        public int C_AAB_010 { get; set; }
        public int C_AAB_100 { get; set; }
        public int C_AAB_110 { get; set; }
        public int C_AAB_101 { get; set; }
        public int C_AAB_011 { get; set; }
        public int C_AAB_002 { get; set; }
        public int C_AAB_020 { get; set; }
        public int C_AAB_200 { get; set; }
        public int C_AAB_220 { get; set; }
        public int C_AAB_202 { get; set; }
        public int C_AAB_022 { get; set; }
        public int C_AAB_112 { get; set; }
        public int C_AAB_121 { get; set; }
        public int C_AAB_211 { get; set; }
        public int C_AAB_221 { get; set; }
        public int C_AAB_212 { get; set; }
        public int C_AAB_122 { get; set; }

        public int C_ABC_012 { get; set; }
        public int C_ABC_120 { get; set; }
        public int C_ABC_201 { get; set; }
        public int C_ABC_102 { get; set; }
        public int C_ABC_021 { get; set; }
        public int C_ABC_210 { get; set; }
    }

     
    public class PL3_Chu32_InfoCollection : List<PL3_Chu32_Info>
    {
    }

    /// <summary>
    /// 排列3除3 3走势info
    /// </summary>
     
    public class PL3_Chu33_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }

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

        public string y3xt { set; get; }

        public int Y0_Number0 { get; set; }
        public int Y0_Number1 { get; set; }
        public int Y0_Number2 { get; set; }

        public int Y1_Number0 { get; set; }
        public int Y1_Number1 { get; set; }
        public int Y1_Number2 { get; set; }

        public int Y2_Number0 { get; set; }
        public int Y2_Number1 { get; set; }
        public int Y2_Number2 { get; set; }
    }

     
    public class PL3_Chu33_InfoCollection : List<PL3_Chu33_Info>
    {
    }


    /// <summary>
    /// 排列3 和值特征
    /// </summary>
     
    public class PL3_HZTZ_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }

        /// <summary>
        /// 和值
        /// </summary>
        public int HeZhi { get; set; }

        public int DX_HeZhi_D { get; set; }
        public int DX_HeZhi_X { get; set; }

        public int JO_HeZhi_J { get; set; }
        public int JO_HeZhi_O { get; set; }

        public int ZH_HeZhi_Z { get; set; }
        public int ZH_HeZhi_H { get; set; }

        public int QS_HeZhi_Sheng { get; set; }
        public int QS_HeZhi_Ping { get; set; }
        public int QS_HeZhi_Jiang { get; set; }

        public int O_C4_Y0 { set; get; }
        public int O_C4_Y1 { set; get; }
        public int O_C4_Y2 { set; get; }
        public int O_C4_Y3 { set; get; }

        public int O_C3_Y0 { set; get; }
        public int O_C3_Y1 { set; get; }
        public int O_C3_Y2 { set; get; }

        public int O_C5_Y0 { set; get; }
        public int O_C5_Y1 { set; get; }
        public int O_C5_Y2 { set; get; }
        public int O_C5_Y3 { set; get; }
        public int O_C5_Y4 { set; get; }
    }

     
    public class PL3_HZTZ_InfoCollection : List<PL3_HZTZ_Info>
    {
    }

    /// <summary>
    /// 排列3 和值合尾
    /// </summary>
     
    public class PL3_HZHW_Info : ImportInfoBase
    {
        public string RedBall1 { get; set; }
        public string RedBall2 { get; set; }
        public string RedBall3 { get; set; }

        /// <summary>
        /// 和值
        /// </summary>
        public virtual int HeZhi { get; set; }

        public int AVG_0 { get; set; }
        public int AVG_1 { get; set; }
        public int AVG_2 { get; set; }
        public int AVG_3 { get; set; }
        public int AVG_4 { get; set; }
        public int AVG_5 { get; set; }
        public int AVG_6 { get; set; }
        public int AVG_7 { get; set; }
        public int AVG_8 { get; set; }
        public int AVG_9 { get; set; }

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

     
    public class PL3_HZHW_InfoCollection : List<PL3_HZHW_Info>
    {
    }




}
