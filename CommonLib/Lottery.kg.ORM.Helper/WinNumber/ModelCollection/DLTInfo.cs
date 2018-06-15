using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.ModelCollection;

namespace Lottery.Kg.ORM.Helper.WinNumber.ModelCollection
{
    /// <summary>
    /// 大乐透基本走势info
    /// </summary>
     
    public class DLT_JiBenZouSi_Info : ImportInfoBase
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
        public int Red34 { get; set; }
        public int Red35 { get; set; }

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

    }

     
    public class DLT_JiBenZouSi_InfoCollection : List<DLT_JiBenZouSi_Info>
    {
    }

    /// <summary>
    /// 大乐透大小info
    /// </summary>
     
    public class DLT_DX_Info : ImportInfoBase
    {
        /// <summary>
        /// 前区
        /// </summary>
        public string QianQu { get; set; }
        /// <summary>
        /// 大小排位
        /// </summary>
        public string DaoXiaoQualifying { get; set; }
        /// <summary>
        /// 大小比
        /// </summary>
        public string DaoXiaoBi { get; set; }
        /// <summary>
        /// 大小遗漏
        /// </summary>
        public int NO1D { get; set; }
        public int NO1X { get; set; }
        public int NO2D { get; set; }
        public int NO2X { get; set; }
        public int NO3D { get; set; }
        public int NO3X { get; set; }
        public int NO4D { get; set; }
        public int NO4X { get; set; }
        public int NO5D { get; set; }
        public int NO5X { get; set; }
        /// <summary>
        /// 大小比0比5
        /// </summary>
        public int Bi0_5 { get; set; }
        public int Bi1_4 { get; set; }
        public int Bi2_3 { get; set; }
        public int Bi3_2 { get; set; }
        public int Bi4_1 { get; set; }
        public int Bi5_0 { get; set; }
    }

     
    public class DLT_DX_InfoCollection : List<DLT_DX_Info>
    {
    }

    /// <summary>
    /// 大乐透奇偶info
    /// </summary>
     
    public class DLT_JiOu_Info : ImportInfoBase
    {
        /// <summary>
        /// 前区
        /// </summary>
        public string QianQu { get; set; }
        /// <summary>
        /// 奇偶排位
        /// </summary>
        public string JiOuQualifying { get; set; }
        /// <summary>
        /// 奇偶比
        /// </summary>
        public string JiOuBi { get; set; }
        /// <summary>
        /// 奇偶遗漏
        /// </summary>
        public int NO1J { get; set; }
        public int NO1O { get; set; }
        public int NO2J { get; set; }
        public int NO2O { get; set; }
        public int NO3J { get; set; }
        public int NO3O { get; set; }
        public int NO4J { get; set; }
        public int NO4O { get; set; }
        public int NO5J { get; set; }
        public int NO5O { get; set; }
        /// <summary>
        /// 奇偶比0比5
        /// </summary>
        public int Bi0_5 { get; set; }
        public int Bi1_4 { get; set; }
        public int Bi2_3 { get; set; }
        public int Bi3_2 { get; set; }
        public int Bi4_1 { get; set; }
        public int Bi5_0 { get; set; }
    }

     
    public class DLT_JiOu_InfoCollection : List<DLT_JiOu_Info>
    {
    }

    /// <summary>
    /// 大乐透质和info
    /// </summary>
     
    public class DLT_ZhiHe_Info : ImportInfoBase
    {
        /// <summary>
        /// 前区
        /// </summary>
        public string QianQu { get; set; }
        /// <summary>
        /// 质合排位
        /// </summary>
        public string ZhiHeQualifying { get; set; }
        /// <summary>
        /// 质合比
        /// </summary>
        public string ZhiHeBi { get; set; }
        /// <summary>
        /// 质合遗漏
        /// </summary>
        public int NO1Z { get; set; }
        public int NO1H { get; set; }
        public int NO2Z { get; set; }
        public int NO2H { get; set; }
        public int NO3Z { get; set; }
        public int NO3H { get; set; }
        public int NO4Z { get; set; }
        public int NO4H { get; set; }
        public int NO5Z { get; set; }
        public int NO5H { get; set; }
        /// <summary>
        /// 质合比0比5
        /// </summary> 
        public int Bi0_5 { get; set; }
        public int Bi1_4 { get; set; }
        public int Bi2_3 { get; set; }
        public int Bi3_2 { get; set; }
        public int Bi4_1 { get; set; }
        public int Bi5_0 { get; set; }
    }

     
    public class DLT_ZhiHe_InfoCollection : List<DLT_ZhiHe_Info>
    {
    }


    /// <summary>
    /// 大乐透和值info
    /// </summary>
     
    public class DLT_HeZhi_Info : ImportInfoBase
    {
        /// <summary>
        /// 前区
        /// </summary>
        public string QianQu { get; set; }
        /// <summary>
        /// 和值
        /// </summary>
        public int HeZhi { get; set; }
        /// <summary>
        /// 和尾
        /// </summary>
        public int HeWei { get; set; }
        /// <summary>
        /// 和值分布
        /// </summary>
        public int HZ_15_49 { get; set; }
        public int HZ_50_59 { get; set; }
        public int HZ_60_69 { get; set; }
        public int HZ_70_79 { get; set; }
        public int HZ_80_89 { get; set; }
        public int HZ_90_99 { get; set; }
        public int HZ_100_109 { get; set; }
        public int HZ_110_119 { get; set; }
        public int HZ_120_129 { get; set; }
        public int HZ_130_165 { get; set; }
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

     
    public class DLT_HeZhi_InfoCollection : List<DLT_HeZhi_Info>
    {
    }



    /// <summary>
    /// 大乐透除3info
    /// </summary>
     
    public class DLT_Chu3_Info : ImportInfoBase
    {
        /// <summary>
        /// 前区
        /// </summary>
        public string QianQu { get; set; }
        /// <summary>
        /// 除3余数排位
        /// </summary>
        public string Chu3Qualifying { get; set; }
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


     
    public class DLT_Chu3_InfoCollection : List<DLT_Chu3_Info>
    {
    }

    /// <summary>
    /// 大乐透首尾info
    /// </summary>
     
    public class DLT_KuaDu_SW_Info : ImportInfoBase
    {
        /// <summary>
        /// 前区
        /// </summary>
        public string QianQu { get; set; }
        /// <summary>
        /// 首尾跨度遗漏
        /// </summary>
        public int KD_4 { get; set; }
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
        public int KD_33 { get; set; }
        public int KD_34 { get; set; }
    }

     
    public class DLT_KuaDu_SW_InfoCollection : List<DLT_KuaDu_SW_Info>
    {
    }

    /// <summary>
    /// 大乐透12info
    /// </summary>
     
    public class DLT_KuaDu_12_Info : ImportInfoBase
    {
        /// <summary>
        /// 前区
        /// </summary>
        public string QianQu { get; set; }
        /// <summary> 
        /// 首尾跨度遗漏
        /// </summary>
        public int KD_1 { get; set; }
        public int KD_2 { get; set; }
        public int KD_3 { get; set; }
        public int KD_4 { get; set; }
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
    }

     
    public class DLT_KuaDu_12_InfoCollection : List<DLT_KuaDu_12_Info>
    {
    }

    /// <summary>
    /// 大乐透23info
    /// </summary>
     
    public class DLT_KuaDu_23_Info : ImportInfoBase
    {
        /// <summary>
        /// 前区 
        /// </summary>
        public string QianQu { get; set; }
        /// <summary> 
        /// 首尾跨度遗漏
        /// </summary>
        public int KD_1 { get; set; }
        public int KD_2 { get; set; }
        public int KD_3 { get; set; }
        public int KD_4 { get; set; }
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
    }

     
    public class DLT_KuaDu_23_InfoCollection : List<DLT_KuaDu_23_Info>
    {
    }

    /// <summary>
    /// 大乐透34info
    /// </summary>
     
    public class DLT_KuaDu_34_Info : ImportInfoBase
    {
        /// <summary>
        /// 前区 
        /// </summary>
        public string QianQu { get; set; }
        /// <summary> 
        /// 首尾跨度遗漏
        /// </summary>
        public int KD_1 { get; set; }
        public int KD_2 { get; set; }
        public int KD_3 { get; set; }
        public int KD_4 { get; set; }
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
    }

     
    public class DLT_KuaDu_34_InfoCollection : List<DLT_KuaDu_34_Info>
    {
    }

    /// <summary>
    /// 大乐透45info
    /// </summary>
     
    public class DLT_KuaDu_45_Info : ImportInfoBase
    {
        /// <summary>
        /// 前区 
        /// </summary>
        public string QianQu { get; set; }
        /// <summary> 
        /// 首尾跨度遗漏
        /// </summary>
        public int KD_1 { get; set; }
        public int KD_2 { get; set; }
        public int KD_3 { get; set; }
        public int KD_4 { get; set; }
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
    }

     
    public class DLT_KuaDu_45_InfoCollection : List<DLT_KuaDu_45_Info>
    {
    }

}
