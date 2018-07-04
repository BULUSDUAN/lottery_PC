using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection
{
    /// <summary>
    /// 基本走势
    /// </summary>
     
    public class SDKLPK3_JBZS_Info : ImportInfoBase
    {
        public int D1_1 { get; set; }
        public int D1_2 { get; set; }
        public int D1_3 { get; set; }
        public int D1_4 { get; set; }
        public int D1_5 { get; set; }
        public int D1_6 { get; set; }
        public int D1_7 { get; set; }
        public int D1_8 { get; set; }
        public int D1_9 { get; set; }
        public int D1_10 { get; set; }
        public int D1_11 { get; set; }
        public int D1_12 { get; set; }
        public int D1_13 { get; set; }

        public int D2_1 { get; set; }
        public int D2_2 { get; set; }
        public int D2_3 { get; set; }
        public int D2_4 { get; set; }
        public int D2_5 { get; set; }
        public int D2_6 { get; set; }
        public int D2_7 { get; set; }
        public int D2_8 { get; set; }
        public int D2_9 { get; set; }
        public int D2_10 { get; set; }
        public int D2_11 { get; set; }
        public int D2_12 { get; set; }
        public int D2_13 { get; set; }

        public int D3_1 { get; set; }
        public int D3_2 { get; set; }
        public int D3_3 { get; set; }
        public int D3_4 { get; set; }
        public int D3_5 { get; set; }
        public int D3_6 { get; set; }
        public int D3_7 { get; set; }
        public int D3_8 { get; set; }
        public int D3_9 { get; set; }
        public int D3_10 { get; set; }
        public int D3_11 { get; set; }
        public int D3_12 { get; set; }
        public int D3_13 { get; set; }
    }

     
    public class SDKLPK3_JBZS_InfoCollection : List<SDKLPK3_JBZS_Info>
    {
    }

    /// <summary>
    /// 组选走势
    /// </summary>
     
    public class SDKLPK3_ZHXZS_Info : ImportInfoBase
    {
        public int S_1 { get; set; }
        public int S_2 { get; set; }
        public int S_3 { get; set; }
        public int S_4 { get; set; }
        public int S_5 { get; set; }
        public int S_6 { get; set; }
        public int S_7 { get; set; }
        public int S_8 { get; set; }
        public int S_9 { get; set; }
        public int S_10 { get; set; }
        public int S_11 { get; set; }
        public int S_12 { get; set; }
        public int S_13 { get; set; }

        public int DX_30 { get; set; }
        public int DX_21 { get; set; }
        public int DX_12 { get; set; }
        public int DX_03 { get; set; }

        public int JO_30 { get; set; }
        public int JO_21 { get; set; }
        public int JO_12 { get; set; }
        public int JO_03 { get; set; }

        public int ZH_30 { get; set; }
        public int ZH_21 { get; set; }
        public int ZH_12 { get; set; }
        public int ZH_03 { get; set; }
    }

     
    public class SDKLPK3_ZHXZS_InfoCollection : List<SDKLPK3_ZHXZS_Info>
    {
    }

    /// <summary>
    /// 花色
    /// </summary>
     
    public class SDKLPK3_HSZS_Info : ImportInfoBase
    {
        public int D1_1 { get; set; }
        public int D1_2 { get; set; }
        public int D1_3 { get; set; }
        public int D1_4 { get; set; }

        public int D2_1 { get; set; }
        public int D2_2 { get; set; }
        public int D2_3 { get; set; }
        public int D2_4 { get; set; }

        public int D3_1 { get; set; }
        public int D3_2 { get; set; }
        public int D3_3 { get; set; }
        public int D3_4 { get; set; }

        public int TH { get; set; }
        public int THS { get; set; }
        public int SZ { get; set; }
    }

     
    public class SDKLPK3_HSZS_InfoCollection : List<SDKLPK3_HSZS_Info>
    {
    }

    /// <summary>
    /// 大小
    /// </summary>
     
    public class SDKLPK3_DXZS_Info : ImportInfoBase
    {
        public int D1_D { get; set; }
        public int D1_X { get; set; }
        public int D2_D { get; set; }
        public int D2_X { get; set; }
        public int D3_D { get; set; }
        public int D3_X { get; set; }

        public int DXB_30 { get; set; }
        public int DXB_21 { get; set; }
        public int DXB_12 { get; set; }
        public int DXB_03 { get; set; }

        public int DDD { get; set; }
        public int DDX { get; set; }
        public int DXD { get; set; }
        public int XDD { get; set; }
        public int DXX { get; set; }
        public int XDX { get; set; }
        public int XXD { get; set; }
        public int XXX { get; set; }
    }
     
    public class SDKLPK3_DXZS_InfoCollection : List<SDKLPK3_DXZS_Info>
    {
    }

    /// <summary>
    /// 奇偶
    /// </summary>
     
    public class SDKLPK3_JOZS_Info : ImportInfoBase
    {
        public int D1_J { get; set; }
        public int D1_O { get; set; }
        public int D2_J { get; set; }
        public int D2_O { get; set; }
        public int D3_J { get; set; }
        public int D3_O { get; set; }

        public int JOB_30 { get; set; }
        public int JOB_21 { get; set; }
        public int JOB_12 { get; set; }
        public int JOB_03 { get; set; }

        public int JJJ { get; set; }
        public int JJO { get; set; }
        public int JOJ { get; set; }
        public int OJJ { get; set; }
        public int JOO { get; set; }
        public int OJO { get; set; }
        public int OOJ { get; set; }
        public int OOO { get; set; }
    }
     
    public class SDKLPK3_JOZS_InfoCollection : List<SDKLPK3_JOZS_Info>
    {
    }

    /// <summary>
    /// 质合
    /// </summary>
     
    public class SDKLPK3_ZHZS_Info : ImportInfoBase
    {
        public int D1_Z { get; set; }
        public int D1_H { get; set; }
        public int D2_Z { get; set; }
        public int D2_H { get; set; }
        public int D3_Z { get; set; }
        public int D3_H { get; set; }

        public int ZHB_30 { get; set; }
        public int ZHB_21 { get; set; }
        public int ZHB_12 { get; set; }
        public int ZHB_03 { get; set; }

        public int ZZZ { get; set; }
        public int ZZH { get; set; }
        public int ZHZ { get; set; }
        public int HZZ { get; set; }
        public int ZHH { get; set; }
        public int HZH { get; set; }
        public int HHZ { get; set; }
        public int HHH { get; set; }
    }
     
    public class SDKLPK3_ZHZS_InfoCollection : List<SDKLPK3_ZHZS_Info>
    {
    }

    /// <summary>
    /// 除3余
    /// </summary>
     
    public class SDKLPK3_C3YZS_Info : ImportInfoBase
    {
        public int D1_0 { get; set; }
        public int D1_1 { get; set; }
        public int D1_2 { get; set; }

        public int D2_0 { get; set; }
        public int D2_1 { get; set; }
        public int D2_2 { get; set; }

        public int D3_0 { get; set; }
        public int D3_1 { get; set; }
        public int D3_2 { get; set; }

        public string C3YB { get; set; }

        public int Y0_0 { get; set; }
        public int Y0_1 { get; set; }
        public int Y0_2 { get; set; }
        public int Y0_3 { get; set; }

        public int Y1_0 { get; set; }
        public int Y1_1 { get; set; }
        public int Y1_2 { get; set; }
        public int Y1_3 { get; set; }

        public int Y2_0 { get; set; }
        public int Y2_1 { get; set; }
        public int Y2_2 { get; set; }
        public int Y2_3 { get; set; }
    }

     
    public class SDKLPK3_C3YZS_InfoCollection : List<SDKLPK3_C3YZS_Info>
    {
    }


    /// <summary>
    /// 和值
    /// </summary>
     
    public class SDKLPK3_HZZS_Info : ImportInfoBase
    {
        public int HZ { get; set; }

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
    }

     
    public class SDKLPK3_HZZS_InfoCollection : List<SDKLPK3_HZZS_Info>
    {
    }

    /// <summary>
    /// 类型
    /// </summary>
     
    public class SDKLPK3_LXZS_Info : ImportInfoBase
    {
        public int S_1 { get; set; }
        public int S_2 { get; set; }
        public int S_3 { get; set; }
        public int S_4 { get; set; }
        public int S_5 { get; set; }
        public int S_6 { get; set; }
        public int S_7 { get; set; }
        public int S_8 { get; set; }
        public int S_9 { get; set; }
        public int S_10 { get; set; }
        public int S_11 { get; set; }
        public int S_12 { get; set; }
        public int S_13 { get; set; }

        public int TH { get; set; }
        public int THS { get; set; }
        public int SZ { get; set; }
        public int DZ { get; set; }
        public int BZ { get; set; }
    }

     
    public class SDKLPK3_LXZS_InfoCollection : List<SDKLPK3_LXZS_Info>
    {
    }
}
