using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection
{
    /// <summary>
    /// 山东群英会任选奇偶走势info
    /// </summary>
     
    public class SDQYH_RXJO_Info : ImportInfoBase
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

     
    public class SDQYH_RXJO_InfoCollection : List<SDQYH_RXJO_Info>
    {

    }

    /// <summary>
    /// 山东群英会任选奇偶走势info
    /// </summary>
     
    public class SDQYH_RXZH_Info : ImportInfoBase
    {
        /// <summary>
        /// 任选基本走势分布
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

     
    public class SDQYH_RXZH_InfoCollection : List<SDQYH_RXZH_Info>
    {
    }

    /// <summary>
    /// 山东群英会任选大小走势info
    /// </summary>
     
    public class SDQYH_RXDX_Info : ImportInfoBase
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

     
    public class SDQYH_RXDX_InfoCollection : List<SDQYH_RXDX_Info>
    {
    }

    /// <summary>
    /// 山东群英会任选除3走势info
    /// </summary>
     
    public class SDQYH_Chu3_Info : ImportInfoBase
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

     
    public class SDQYH_Chu3_InfoCollection : List<SDQYH_Chu3_Info>
    {
    }


    /// <summary>
    /// 山东群英会顺选1走势info
    /// </summary>
     
    public class SDQYH_SX1_Info : ImportInfoBase
    {
        public int NO_01 { get; set; }
        public int NO_02 { get; set; }
        public int NO_03 { get; set; }
        public int NO_04 { get; set; }
        public int NO_05 { get; set; }
        public int NO_06 { get; set; }
        public int NO_07 { get; set; }
        public int NO_08 { get; set; }
        public int NO_09 { get; set; }
        public int NO_10 { get; set; }
        public int NO_11 { get; set; }
        public int NO_12 { get; set; }
        public int NO_13 { get; set; }
        public int NO_14 { get; set; }
        public int NO_15 { get; set; }
        public int NO_16 { get; set; }
        public int NO_17 { get; set; }
        public int NO_18 { get; set; }
        public int NO_19 { get; set; }
        public int NO_20 { get; set; }
        public int NO_21 { get; set; }
        public int NO_22 { get; set; }
        public int NO_23 { get; set; }

        /// <summary>
        /// 大小奇偶质和
        /// </summary>
        public int DX_D { get; set; }
        public int DX_X { get; set; }
        public int JO_J { get; set; }
        public int JO_O { get; set; }
        public int ZH_Z { get; set; }
        public int ZH_H { get; set; }
        public int YU_0 { get; set; }
        public int YU_1 { get; set; }
        public int YU_2 { get; set; }
    }

     
    public class SDQYH_SX1_InfoCollection : List<SDQYH_SX1_Info>
    {
    }

    /// <summary>
    /// 山东群英会顺选2走势info
    /// </summary>
     
    public class SDQYH_SX2_Info : ImportInfoBase
    {
        public int NO_01 { get; set; }
        public int NO_02 { get; set; }
        public int NO_03 { get; set; }
        public int NO_04 { get; set; }
        public int NO_05 { get; set; }
        public int NO_06 { get; set; }
        public int NO_07 { get; set; }
        public int NO_08 { get; set; }
        public int NO_09 { get; set; }
        public int NO_10 { get; set; }
        public int NO_11 { get; set; }
        public int NO_12 { get; set; }
        public int NO_13 { get; set; }
        public int NO_14 { get; set; }
        public int NO_15 { get; set; }
        public int NO_16 { get; set; }
        public int NO_17 { get; set; }
        public int NO_18 { get; set; }
        public int NO_19 { get; set; }
        public int NO_20 { get; set; }
        public int NO_21 { get; set; }
        public int NO_22 { get; set; }
        public int NO_23 { get; set; }

        /// <summary>
        /// 大小奇偶质和
        /// </summary>
        public int DX_D { get; set; }
        public int DX_X { get; set; }
        public int JO_J { get; set; }
        public int JO_O { get; set; }
        public int ZH_Z { get; set; }
        public int ZH_H { get; set; }
        public int YU_0 { get; set; }
        public int YU_1 { get; set; }
        public int YU_2 { get; set; }
    }

     
    public class SDQYH_SX2_InfoCollection : List<SDQYH_SX2_Info>
    {
    }

    /// <summary>
    /// 山东群英会顺选2走势info
    /// </summary>
     
    public class SDQYH_SX3_Info : ImportInfoBase
    {
        public int NO_01 { get; set; }
        public int NO_02 { get; set; }
        public int NO_03 { get; set; }
        public int NO_04 { get; set; }
        public int NO_05 { get; set; }
        public int NO_06 { get; set; }
        public int NO_07 { get; set; }
        public int NO_08 { get; set; }
        public int NO_09 { get; set; }
        public int NO_10 { get; set; }
        public int NO_11 { get; set; }
        public int NO_12 { get; set; }
        public int NO_13 { get; set; }
        public int NO_14 { get; set; }
        public int NO_15 { get; set; }
        public int NO_16 { get; set; }
        public int NO_17 { get; set; }
        public int NO_18 { get; set; }
        public int NO_19 { get; set; }
        public int NO_20 { get; set; }
        public int NO_21 { get; set; }
        public int NO_22 { get; set; }
        public int NO_23 { get; set; }

        /// <summary>
        /// 大小奇偶质和
        /// </summary>
        public int DX_D { get; set; }
        public int DX_X { get; set; }
        public int JO_J { get; set; }
        public int JO_O { get; set; }
        public int ZH_Z { get; set; }
        public int ZH_H { get; set; }
        public int YU_0 { get; set; }
        public int YU_1 { get; set; }
        public int YU_2 { get; set; }
    }

     
    public class SDQYH_SX3_InfoCollection : List<SDQYH_SX3_Info>
    {
    }
    ///// <summary>
    ///// 山东群英会任选奇偶走势info
    ///// </summary>
    // 
    //public class SDQYH_RXZH_Info : ImportInfoBase
    //{

    //}

    // 
    //public class SDQYH_RXZH_InfoCollection : List<SDQYH_RXZH_Info>
    //{
    //}


}
