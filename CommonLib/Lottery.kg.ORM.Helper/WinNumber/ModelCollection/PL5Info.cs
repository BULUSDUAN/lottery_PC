using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection
{
    /// <summary>
    /// 排列5基本走势info
    /// </summary>
     
    public class PL5_JBZS_Info : ImportInfoBase
    {
        /// <summary>
        /// 第一位
        /// </summary>
        public int W_0 { get; set; }
        public int W_1 { get; set; }
        public int W_2 { get; set; }
        public int W_3 { get; set; }
        public int W_4 { get; set; }
        public int W_5 { get; set; }
        public int W_6 { get; set; }
        public int W_7 { get; set; }
        public int W_8 { get; set; }
        public int W_9 { get; set; }

        public int Q_0 { get; set; }
        public int Q_1 { get; set; }
        public int Q_2 { get; set; }
        public int Q_3 { get; set; }
        public int Q_4 { get; set; }
        public int Q_5 { get; set; }
        public int Q_6 { get; set; }
        public int Q_7 { get; set; }
        public int Q_8 { get; set; }
        public int Q_9 { get; set; }

        public int B_0 { get; set; }
        public int B_1 { get; set; }
        public int B_2 { get; set; }
        public int B_3 { get; set; }
        public int B_4 { get; set; }
        public int B_5 { get; set; }
        public int B_6 { get; set; }
        public int B_7 { get; set; }
        public int B_8 { get; set; }
        public int B_9 { get; set; }

        public int S_0 { get; set; }
        public int S_1 { get; set; }
        public int S_2 { get; set; }
        public int S_3 { get; set; }
        public int S_4 { get; set; }
        public int S_5 { get; set; }
        public int S_6 { get; set; }
        public int S_7 { get; set; }
        public int S_8 { get; set; }
        public int S_9 { get; set; }

        public int G_0 { get; set; }
        public int G_1 { get; set; }
        public int G_2 { get; set; }
        public int G_3 { get; set; }
        public int G_4 { get; set; }
        public int G_5 { get; set; }
        public int G_6 { get; set; }
        public int G_7 { get; set; }
        public int G_8 { get; set; }
        public int G_9 { get; set; }
    }

     
    public class PL5_JBZS_InfoCollection : List<PL5_JBZS_Info>
    {
    }

    /// <summary>
    /// 排列5基本走势info
    /// </summary>
     
    public class PL5_DX_Info : ImportInfoBase
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

     
    public class PL5_DX_InfoCollection : List<PL5_DX_Info>
    {
    }



    /// <summary>
    /// 排列5基本走势info
    /// </summary>
     
    public class PL5_JO_Info : ImportInfoBase
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

     
    public class PL5_JO_InfoCollection : List<PL5_JO_Info>
    {
    }

    /// <summary>
    /// 排列5质和走势info
    /// </summary>
     
    public class PL5_ZH_Info : ImportInfoBase
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

     
    public class PL5_ZH_InfoCollection : List<PL5_ZH_Info>
    {
    }

    /// <summary>
    /// 排列5除3走势info
    /// </summary>
     
    public class PL5_Chu3_Info : ImportInfoBase
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

     
    public class PL5_Chu3_InfoCollection : List<PL5_Chu3_Info>
    {
    }

    /// <summary>
    /// 排列5和值走势info
    /// </summary>
     
    public class PL5_HZ_Info : ImportInfoBase
    {
        /// <summary>
        /// 任选和值走势
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

     
    public class PL5_HZ_InfoCollection : List<PL5_HZ_Info>
    {
    }

}
