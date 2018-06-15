using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.ModelCollection;

namespace Lottery.Kg.ORM.Helper.WinNumber.ModelCollection
{
    /// <summary>
    /// 江苏快3基本走势info
    /// </summary>
     
    public class JLK3_JBZS_Info : ImportInfoBase
    {
        public int Red_1 { get; set; }
        public int Red_2 { get; set; }
        public int Red_3 { get; set; }
        public int Red_4 { get; set; }
        public int Red_5 { get; set; }
        public int Red_6 { get; set; }

        public int RedCan_1 { get; set; }
        public int RedCan_2 { get; set; }
        public int RedCan_3 { get; set; }
        public int RedCan_4 { get; set; }
        public int RedCan_5 { get; set; }
        public int RedCan_6 { get; set; }

        /// <summary>
        /// 奇的个数
        /// </summary>
        public int JiCount_0 { get; set; }
        public int JiCount_1 { get; set; }
        public int JiCount_2 { get; set; }
        public int JiCount_3 { get; set; }

        /// <summary>
        ///小的个数
        /// </summary>
        public int XiaoCount_0 { get; set; }
        public int XiaoCount_1 { get; set; }
        public int XiaoCount_2 { get; set; }
        public int XiaoCount_3 { get; set; }

        /// <summary>
        /// 和值
        /// </summary>
        public int He_3 { get; set; }
        public int He_4 { get; set; }
        public int He_5 { get; set; }
        public int He_6 { get; set; }
        public int He_7 { get; set; }
        public int He_8 { get; set; }
        public int He_9 { get; set; }
        public int He_10 { get; set; }
        public int He_11 { get; set; }
        public int He_12 { get; set; }
        public int He_13 { get; set; }
        public int He_14 { get; set; }
        public int He_15 { get; set; }
        public int He_16 { get; set; }
        public int He_17 { get; set; }
        public int He_18 { get; set; }

    }

     
    public class JLK3_JBZS_InfoCollection : List<JLK3_JBZS_Info>
    {
    }

    /// <summary>
    /// 江苏快3基本走势info
    /// </summary>
     
    public class JLK3_HZ_Info : ImportInfoBase
    {
        /// <summary>
        /// 和值
        /// </summary>
        public int He_3 { get; set; }
        public int He_4 { get; set; }
        public int He_5 { get; set; }
        public int He_6 { get; set; }
        public int He_7 { get; set; }
        public int He_8 { get; set; }
        public int He_9 { get; set; }
        public int He_10 { get; set; }
        public int He_11 { get; set; }
        public int He_12 { get; set; }
        public int He_13 { get; set; }
        public int He_14 { get; set; }
        public int He_15 { get; set; }
        public int He_16 { get; set; }
        public int He_17 { get; set; }
        public int He_18 { get; set; }
        /// <summary>
        /// 合尾
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

        /// <summary>
        ///除3余数
        /// </summary>
        public int Yu_0 { get; set; }
        public int Yu_1 { get; set; }
        public int Yu_2 { get; set; }
    }
     
    public class JLK3_HZ_InfoCollection : List<JLK3_HZ_Info>
    {
    }


    /// <summary>
    /// 江苏快3基本走势info
    /// </summary>
     
    public class JLK3_XT_Info : ImportInfoBase
    {
        public int Red_1 { get; set; }
        public int Red_2 { get; set; }
        public int Red_3 { get; set; }
        public int Red_4 { get; set; }
        public int Red_5 { get; set; }
        public int Red_6 { get; set; }

        public int RedCan_1 { get; set; }
        public int RedCan_2 { get; set; }
        public int RedCan_3 { get; set; }
        public int RedCan_4 { get; set; }
        public int RedCan_5 { get; set; }
        public int RedCan_6 { get; set; }

        /// <summary>
        /// 号码形态
        /// </summary>
        public int XT_3T { get; set; }
        public int XT_3BT { get; set; }
        public int XT_2T { get; set; }
        public int XT_2BT { get; set; }

        /// <summary>
        /// 跨度
        /// </summary>
        public int KD_0 { get; set; }
        public int KD_1 { get; set; }
        public int KD_2 { get; set; }
        public int KD_3 { get; set; }
        public int KD_4 { get; set; }
        public int KD_5 { get; set; }

        /// <summary>
        /// 和值
        /// </summary>
        public int He_3 { get; set; }
        public int He_4 { get; set; }
        public int He_5 { get; set; }
        public int He_6 { get; set; }
        public int He_7 { get; set; }
        public int He_8 { get; set; }
        public int He_9 { get; set; }
        public int He_10 { get; set; }
        public int He_11 { get; set; }
        public int He_12 { get; set; }
        public int He_13 { get; set; }
        public int He_14 { get; set; }
        public int He_15 { get; set; }
        public int He_16 { get; set; }
        public int He_17 { get; set; }
        public int He_18 { get; set; }
    }
     
    public class JLK3_XT_InfoCollection : List<JLK3_XT_Info>
    {
    }


    /// <summary>
    /// 江苏快3基本走势info
    /// </summary>
     
    public class JLK3_ZH_Info : ImportInfoBase
    {
        public int Red_1 { get; set; }
        public int Red_2 { get; set; }
        public int Red_3 { get; set; }
        public int Red_4 { get; set; }
        public int Red_5 { get; set; }
        public int Red_6 { get; set; }

        public int RedCan_1 { get; set; }
        public int RedCan_2 { get; set; }
        public int RedCan_3 { get; set; }
        public int RedCan_4 { get; set; }
        public int RedCan_5 { get; set; }
        public int RedCan_6 { get; set; }

        public int AA11 { get; set; }
        public int AA22 { get; set; }
        public int AA33 { get; set; }
        public int AA44 { get; set; }
        public int AA55 { get; set; }
        public int AA66 { get; set; }


        public int AB12 { get; set; }
        public int AB13 { get; set; }
        public int AB14 { get; set; }
        public int AB15 { get; set; }
        public int AB16 { get; set; }
        public int AB23 { get; set; }
        public int AB24 { get; set; }
        public int AB25 { get; set; }
        public int AB26 { get; set; }
        public int AB34 { get; set; }
        public int AB35 { get; set; }
        public int AB36 { get; set; }
        public int AB45 { get; set; }
        public int AB46 { get; set; }
        public int AB56 { get; set; }

        /// <summary>
        /// 跨度
        /// </summary>
        public int KD_0 { get; set; }
        public int KD_1 { get; set; }
        public int KD_2 { get; set; }
        public int KD_3 { get; set; }
        public int KD_4 { get; set; }
        public int KD_5 { get; set; }

        public int HZ { get; set; }
    }
     
    public class JLK3_ZH_InfoCollection : List<JLK3_ZH_Info>
    {
    }



    /// <summary>
    /// 江苏快3综合走势info
    /// </summary>
     
    public class JLK3_ZHZS_Info : ImportInfoBase
    {
        public int Red_1 { get; set; }
        public int Red_2 { get; set; }
        public int Red_3 { get; set; }
        public int Red_4 { get; set; }
        public int Red_5 { get; set; }
        public int Red_6 { get; set; }

        public int RedCan_1 { get; set; }
        public int RedCan_2 { get; set; }
        public int RedCan_3 { get; set; }
        public int RedCan_4 { get; set; }
        public int RedCan_5 { get; set; }
        public int RedCan_6 { get; set; }

        /// <summary>
        ///百位
        /// </summary>
        public int BW_1 { get; set; }
        public int BW_2 { get; set; }
        public int BW_3 { get; set; }
        public int BW_4 { get; set; }
        public int BW_5 { get; set; }
        public int BW_6 { get; set; }

        /// <summary>
        ///十位
        /// </summary>
        public int SW_1 { get; set; }
        public int SW_2 { get; set; }
        public int SW_3 { get; set; }
        public int SW_4 { get; set; }
        public int SW_5 { get; set; }
        public int SW_6 { get; set; }

        /// <summary>
        ///个位
        /// </summary>
        public int GW_1 { get; set; }
        public int GW_2 { get; set; }
        public int GW_3 { get; set; }
        public int GW_4 { get; set; }
        public int GW_5 { get; set; }
        public int GW_6 { get; set; }
    }
     
    public class JLK3_ZHZS_InfoCollection : List<JLK3_ZHZS_Info>
    {
    }



}
