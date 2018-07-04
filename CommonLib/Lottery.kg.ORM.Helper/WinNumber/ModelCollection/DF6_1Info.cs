using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;
namespace KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection
{
     
    public class DF6_1_DXZS_Info : ImportInfoBase
    {
        public int DX_D { get; set; }
        public int DX_X { get; set; }

        public int DX1_D { get; set; }
        public int DX1_X { get; set; }

        public int DX2_D { get; set; }
        public int DX2_X { get; set; }

        public int DX3_D { get; set; }
        public int DX3_X { get; set; }

        public int DX4_D { get; set; }
        public int DX4_X { get; set; }

        public int DX5_D { get; set; }
        public int DX5_X { get; set; }

        /// <summary>
        /// 大小比例
        /// </summary>
        public string DX_Proportion { get; set; }

        /// <summary>
        /// 大小比例遗漏
        /// </summary>
        public int O_DX_Proportion06 { get; set; }
        public int O_DX_Proportion15 { get; set; }
        public int O_DX_Proportion24 { get; set; }
        public int O_DX_Proportion33 { get; set; }
        public int O_DX_Proportion42 { get; set; }
        public int O_DX_Proportion51 { get; set; }
        public int O_DX_Proportion60 { get; set; }
    }

     
    public class DF6_1_DXZS_InfoCollection : List<DF6_1_DXZS_Info>
    {
    }

     
    public class DF6_1_HZZS_Info : ImportInfoBase
    {
        public int Red_06 { get; set; }
        public int Red_712 { get; set; }
        public int Red_1318 { get; set; }
        public int Red_1924 { get; set; }
        public int Red_2530 { get; set; }
        public int Red_3136 { get; set; }
        public int Red_3742 { get; set; }
        public int Red_4348 { get; set; }
        public int Red_4954 { get; set; }

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

     
    public class DF6_1_HZZS_InfoCollection : List<DF6_1_HZZS_Info>
    {
    }

     
    public class DF6_1_JBZS_Info : ImportInfoBase
    {
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

        public int Red1_0 { get; set; }
        public int Red1_1 { get; set; }
        public int Red1_2 { get; set; }
        public int Red1_3 { get; set; }
        public int Red1_4 { get; set; }
        public int Red1_5 { get; set; }
        public int Red1_6 { get; set; }
        public int Red1_7 { get; set; }
        public int Red1_8 { get; set; }
        public int Red1_9 { get; set; }

        public int Red2_0 { get; set; }
        public int Red2_1 { get; set; }
        public int Red2_2 { get; set; }
        public int Red2_3 { get; set; }
        public int Red2_4 { get; set; }
        public int Red2_5 { get; set; }
        public int Red2_6 { get; set; }
        public int Red2_7 { get; set; }
        public int Red2_8 { get; set; }
        public int Red2_9 { get; set; }

        public int Red3_0 { get; set; }
        public int Red3_1 { get; set; }
        public int Red3_2 { get; set; }
        public int Red3_3 { get; set; }
        public int Red3_4 { get; set; }
        public int Red3_5 { get; set; }
        public int Red3_6 { get; set; }
        public int Red3_7 { get; set; }
        public int Red3_8 { get; set; }
        public int Red3_9 { get; set; }

        public int Red4_0 { get; set; }
        public int Red4_1 { get; set; }
        public int Red4_2 { get; set; }
        public int Red4_3 { get; set; }
        public int Red4_4 { get; set; }
        public int Red4_5 { get; set; }
        public int Red4_6 { get; set; }
        public int Red4_7 { get; set; }
        public int Red4_8 { get; set; }
        public int Red4_9 { get; set; }

        public int Red5_0 { get; set; }
        public int Red5_1 { get; set; }
        public int Red5_2 { get; set; }
        public int Red5_3 { get; set; }
        public int Red5_4 { get; set; }
        public int Red5_5 { get; set; }
        public int Red5_6 { get; set; }
        public int Red5_7 { get; set; }
        public int Red5_8 { get; set; }
        public int Red5_9 { get; set; }
    }

     
    public class DF6_1_JBZS_InfoCollection : List<DF6_1_JBZS_Info>
    {
    }
     
    public class DF6_1_JOZS_Info : ImportInfoBase
    {
        public int JO_J { get; set; }
        public int JO_O { get; set; }

        public int JO1_J { get; set; }
        public int JO1_O { get; set; }

        public int JO2_J { get; set; }
        public int JO2_O { get; set; }

        public int JO3_J { get; set; }
        public int JO3_O { get; set; }

        public int JO4_J { get; set; }
        public int JO4_O { get; set; }

        public int JO5_J { get; set; }
        public int JO5_O { get; set; }

        /// <summary>
        /// 奇偶比例
        /// </summary>
        public string JO_Proportion { get; set; }

        /// <summary>
        /// 奇偶比例遗漏
        /// </summary>
        public int O_JO_Proportion06 { get; set; }
        public int O_JO_Proportion15 { get; set; }
        public int O_JO_Proportion24 { get; set; }
        public int O_JO_Proportion33 { get; set; }
        public int O_JO_Proportion42 { get; set; }
        public int O_JO_Proportion51 { get; set; }
        public int O_JO_Proportion60 { get; set; }
    }

     
    public class DF6_1_JOZS_InfoCollection : List<DF6_1_JOZS_Info>
    {
    }
     
    public class DF6_1_KDZS_Info : ImportInfoBase
    {
        public int HeZhi { set; get; }


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


        public int C_0 { get; set; }
        public int C_1 { get; set; }
        public int C_2 { get; set; }
        public int C_3 { get; set; }
        public int C_4 { get; set; }
        public int C_5 { get; set; }
        public int C_6 { get; set; }
        public int C_7 { get; set; }
        public int C_8 { get; set; }
        public int C_9 { get; set; }
    }

     
    public class DF6_1_KDZS_InfoCollection : List<DF6_1_KDZS_Info>
    {
    }
     
    public class DF6_1_ZHZS_Info : ImportInfoBase
    {
        public int ZH_Z { get; set; }
        public int ZH_H { get; set; }

        public int ZH1_Z { get; set; }
        public int ZH1_H { get; set; }

        public int ZH2_Z { get; set; }
        public int ZH2_H { get; set; }

        public int ZH3_Z { get; set; }
        public int ZH3_H { get; set; }

        public int ZH4_Z { get; set; }
        public int ZH4_H { get; set; }

        public int ZH5_Z { get; set; }
        public int ZH5_H { get; set; }

        /// <summary>
        /// 质合比例
        /// </summary>
        public string ZH_Proportion { get; set; }

        /// <summary>
        /// 质合比例遗漏
        /// </summary>
        public int O_ZH_Proportion06 { get; set; }
        public int O_ZH_Proportion15 { get; set; }
        public int O_ZH_Proportion24 { get; set; }
        public int O_ZH_Proportion33 { get; set; }
        public int O_ZH_Proportion42 { get; set; }
        public int O_ZH_Proportion51 { get; set; }
        public int O_ZH_Proportion60 { get; set; }
    }

     
    public class DF6_1_ZHZS_InfoCollection : List<DF6_1_ZHZS_Info>
    {
    }
}
