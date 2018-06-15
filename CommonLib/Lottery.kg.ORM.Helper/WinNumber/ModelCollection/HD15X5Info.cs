using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.ModelCollection;

namespace Lottery.Kg.ORM.Helper.WinNumber.ModelCollection
{
    /// <summary>
    /// 华东15选5和值走势info
    /// </summary>
     
    public class HD15X5_HZ_Info : ImportInfoBase
    {
        public int HeZhi { get; set; }
        public int HeWei { get; set; }

        public int HZ_15_17 { get; set; }
        public int HZ_18_20 { get; set; }
        public int HZ_21_23 { get; set; }
        public int HZ_24_26 { get; set; }
        public int HZ_27_29 { get; set; }
        public int HZ_30_32 { get; set; }
        public int HZ_33_35 { get; set; }
        public int HZ_36_38 { get; set; }
        public int HZ_39_41 { get; set; }
        public int HZ_42_44 { get; set; }
        public int HZ_45_47 { get; set; }
        public int HZ_48_50 { get; set; }
        public int HZ_51_53 { get; set; }
        public int HZ_54_56 { get; set; }
        public int HZ_57_59 { get; set; }
        public int HZ_60_62 { get; set; }
        public int HZ_63_65 { get; set; }

    }

     
    public class HD15X5_HZ_InfoCollection : List<HD15X5_HZ_Info>
    {
    }

    /// <summary>
    /// 华东15选5基本走势info
    /// </summary>
     
    public class HD15X5_JBZS_Info : ImportInfoBase
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

        public int Hezhi { get; set; }
        public int HW { get; set; }
        public string DaXiaobi { get; set; }
        public string Jobi { get; set; }
        public string ZHbi { get; set; }
    }

     
    public class HD15X5_JBZS_InfoCollection : List<HD15X5_JBZS_Info>
    {
    }
    /// <summary>
    /// 华东15选5基本走势info
    /// </summary>
     
    public class HD15X5_CH_Info : ImportInfoBase
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
        public int Red_12 { get; set; }
        public int Red_13 { get; set; }
        public int Red_14 { get; set; }
        public int Red_15 { get; set; }
        /// <summary>
        /// 重号
        /// </summary>
        public int Duplicate { get; set; }
        public int HeZhi { get; set; }

        /// <summary>
        /// 奇偶比例
        /// </summary>
        public string JO_Proportion { get; set; }
        /// <summary>
        /// 大小比例
        /// </summary>
        public string DX_Proportion { get; set; }
    }
     
    public class HD15X5_CH_InfoCollection : List<HD15X5_CH_Info>
    {
    }
     
    public class HD15X5_LH_Info : ImportInfoBase
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
        public int Red_1112 { get; set; }
        public int Red_1213 { get; set; }
        public int Red_1314 { get; set; }
        public int Red_1415 { get; set; }

        public int GHao { get; set; }

        public string XLH { get; set; }
        public string DLH { get; set; }

        public int GH_0 { get; set; }
        public int GH_1 { get; set; }
        public int GH_2 { get; set; }
        public int GH_3 { get; set; }
        public int GH_4 { get; set; }
    }
     
    public class HD15X5_LH_InfoCollection : List<HD15X5_LH_Info>
    {
    }
    /// <summary>
    /// 华东15选5大小走势info
    /// </summary>
     
    public class HD15X5_DX_Info : ImportInfoBase
    {
        /// <summary> 
        /// 任选大小走势分布
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

     
    public class HD15X5_DX_InfoCollection : List<HD15X5_DX_Info>
    {
    }

    /// <summary>
    /// 华东15选5奇偶走势info
    /// </summary>
     
    public class HD15X5_JO_Info : ImportInfoBase
    {
        /// <summary> 
        /// 奇偶走势分布
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

     
    public class HD15X5_JO_InfoCollection : List<HD15X5_JO_Info>
    {
    }

    /// <summary>
    /// 华东15选5质和走势info
    /// </summary>
     
    public class HD15X5_ZH_Info : ImportInfoBase
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

     
    public class HD15X5_ZH_InfoCollection : List<HD15X5_ZH_Info>
    {
    }

}
