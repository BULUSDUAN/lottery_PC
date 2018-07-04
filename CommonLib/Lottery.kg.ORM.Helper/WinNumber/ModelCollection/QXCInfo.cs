using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection
{
    /// <summary>
    /// 基本走势info
    /// </summary>
     
    public class QXC_JBZS_Info : ImportInfoBase
    {
        /// <summary>
        /// 第一位
        /// </summary>
        public int NO1_0 { get; set; }
        public int NO1_1 { get; set; }
        public int NO1_2 { get; set; }
        public int NO1_3 { get; set; }
        public int NO1_4 { get; set; }
        public int NO1_5 { get; set; }
        public int NO1_6 { get; set; }
        public int NO1_7 { get; set; }
        public int NO1_8 { get; set; }
        public int NO1_9 { get; set; }

        public int NO2_0 { get; set; }
        public int NO2_1 { get; set; }
        public int NO2_2 { get; set; }
        public int NO2_3 { get; set; }
        public int NO2_4 { get; set; }
        public int NO2_5 { get; set; }
        public int NO2_6 { get; set; }
        public int NO2_7 { get; set; }
        public int NO2_8 { get; set; }
        public int NO2_9 { get; set; }

        public int NO3_0 { get; set; }
        public int NO3_1 { get; set; }
        public int NO3_2 { get; set; }
        public int NO3_3 { get; set; }
        public int NO3_4 { get; set; }
        public int NO3_5 { get; set; }
        public int NO3_6 { get; set; }
        public int NO3_7 { get; set; }
        public int NO3_8 { get; set; }
        public int NO3_9 { get; set; }

        public int NO4_0 { get; set; }
        public int NO4_1 { get; set; }
        public int NO4_2 { get; set; }
        public int NO4_3 { get; set; }
        public int NO4_4 { get; set; }
        public int NO4_5 { get; set; }
        public int NO4_6 { get; set; }
        public int NO4_7 { get; set; }
        public int NO4_8 { get; set; }
        public int NO4_9 { get; set; }

        public int NO5_0 { get; set; }
        public int NO5_1 { get; set; }
        public int NO5_2 { get; set; }
        public int NO5_3 { get; set; }
        public int NO5_4 { get; set; }
        public int NO5_5 { get; set; }
        public int NO5_6 { get; set; }
        public int NO5_7 { get; set; }
        public int NO5_8 { get; set; }
        public int NO5_9 { get; set; }

        public int NO6_0 { get; set; }
        public int NO6_1 { get; set; }
        public int NO6_2 { get; set; }
        public int NO6_3 { get; set; }
        public int NO6_4 { get; set; }
        public int NO6_5 { get; set; }
        public int NO6_6 { get; set; }
        public int NO6_7 { get; set; }
        public int NO6_8 { get; set; }
        public int NO6_9 { get; set; }

        public int NO7_0 { get; set; }
        public int NO7_1 { get; set; }
        public int NO7_2 { get; set; }
        public int NO7_3 { get; set; }
        public int NO7_4 { get; set; }
        public int NO7_5 { get; set; }
        public int NO7_6 { get; set; }
        public int NO7_7 { get; set; }
        public int NO7_8 { get; set; }
        public int NO7_9 { get; set; }

    }

     
    public class QXC_JBZS_InfoCollection : List<QXC_JBZS_Info>
    {
    }

    /// <summary>
    /// 大小走势info
    /// </summary>
     
    public class QXC_DX_Info : ImportInfoBase
    {
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
        public int NO6D { get; set; }
        public int NO6X { get; set; }
        public int NO7D { get; set; }
        public int NO7X { get; set; }
        /// <summary>
        /// 大小比
        /// </summary>
        public string DaoXiaoBi { get; set; }
        /// <summary>
        /// 大小比7比0
        /// </summary>
        public int Bi7_0 { get; set; }
        public int Bi6_1 { get; set; }
        public int Bi5_2 { get; set; }
        public int Bi4_3 { get; set; }
        public int Bi3_4 { get; set; }
        public int Bi2_5 { get; set; }
        public int Bi1_6 { get; set; }
        public int Bi0_7 { get; set; }

    }

     
    public class QXC_DX_InfoCollection : List<QXC_DX_Info>
    {
    }



    /// <summary>
    /// 奇偶走势info
    /// </summary>
     
    public class QXC_JO_Info : ImportInfoBase
    {
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
        public int NO6J { get; set; }
        public int NO6O { get; set; }
        public int NO7J { get; set; }
        public int NO7O { get; set; }
        /// <summary>
        /// 奇偶比
        /// </summary>
        public string JiOuBi { get; set; }
        /// <summary>
        /// 奇偶比7比0
        /// </summary>
        public int Bi7_0 { get; set; }
        public int Bi6_1 { get; set; }
        public int Bi5_2 { get; set; }
        public int Bi4_3 { get; set; }
        public int Bi3_4 { get; set; }
        public int Bi2_5 { get; set; }
        public int Bi1_6 { get; set; }
        public int Bi0_7 { get; set; }
    }

     
    public class QXC_JO_InfoCollection : List<QXC_JO_Info>
    {
    }

    /// <summary>
    /// 大小走势info
    /// </summary>
     
    public class QXC_ZH_Info : ImportInfoBase
    {
        /// <summary>
        /// 质和遗漏
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
        public int NO6Z { get; set; }
        public int NO6H { get; set; }
        public int NO7Z { get; set; }
        public int NO7H { get; set; }
        /// <summary>
        /// 质和比
        /// </summary>
        public string ZhiHeBi { get; set; }
        /// <summary>
        /// 质和比7比0
        /// </summary>
        public int Bi7_0 { get; set; }
        public int Bi6_1 { get; set; }
        public int Bi5_2 { get; set; }
        public int Bi4_3 { get; set; }
        public int Bi3_4 { get; set; }
        public int Bi2_5 { get; set; }
        public int Bi1_6 { get; set; }
        public int Bi0_7 { get; set; }
    }

     
    public class QXC_ZH_InfoCollection : List<QXC_ZH_Info>
    {
    }


    /// <summary>
    /// 除3走势info
    /// </summary>
     
    public class QXC_Chu3_Info : ImportInfoBase
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
        public int NO4_0 { get; set; }
        public int NO4_1 { get; set; }
        public int NO4_2 { get; set; }
        public int NO5_0 { get; set; }
        public int NO5_1 { get; set; }
        public int NO5_2 { get; set; }
        public int NO6_0 { get; set; }
        public int NO6_1 { get; set; }
        public int NO6_2 { get; set; }
        public int NO7_0 { get; set; }
        public int NO7_1 { get; set; }
        public int NO7_2 { get; set; }
        /// <summary>
        /// 除3余数比
        /// </summary>
        public string Chu3Bi { get; set; }
        /// <summary>
        /// 除3余数个数
        /// </summary>
        public int Yu0_0 { get; set; }
        public int Yu0_1 { get; set; }
        public int Yu0_2 { get; set; }
        public int Yu0_3 { get; set; }
        public int Yu0_4 { get; set; }
        public int Yu0_5 { get; set; }
        public int Yu0_6 { get; set; }
        public int Yu0_7 { get; set; }

        public int Yu1_0 { get; set; }
        public int Yu1_1 { get; set; }
        public int Yu1_2 { get; set; }
        public int Yu1_3 { get; set; }
        public int Yu1_4 { get; set; }
        public int Yu1_5 { get; set; }
        public int Yu1_6 { get; set; }
        public int Yu1_7 { get; set; }

        public int Yu2_0 { get; set; }
        public int Yu2_1 { get; set; }
        public int Yu2_2 { get; set; }
        public int Yu2_3 { get; set; }
        public int Yu2_4 { get; set; }
        public int Yu2_5 { get; set; }
        public int Yu2_6 { get; set; }
        public int Yu2_7 { get; set; }
    }

     
    public class QXC_Chu3_InfoCollection : List<QXC_Chu3_Info>
    {
    }
}
