using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection
{
     
    public class HC1_JBZS_Info : ImportInfoBase
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
        public int Red_16 { get; set; }
        public int Red_17 { get; set; }
        public int Red_18 { get; set; }
        public int Red_19 { get; set; }

        public int Red_20 { get; set; }
        public int Red_21 { get; set; }
        public int Red_22 { get; set; }
        public int Red_23 { get; set; }
        public int Red_24 { get; set; }
        public int Red_25 { get; set; }
        public int Red_26 { get; set; }
        public int Red_27 { get; set; }
        public int Red_28 { get; set; }
        public int Red_29 { get; set; }

        public int Red_30 { get; set; }
        public int Red_31 { get; set; }
        public int Red_32 { get; set; }
        public int Red_33 { get; set; }
        public int Red_34 { get; set; }
        public int Red_35 { get; set; }
        public int Red_36 { get; set; }
    }
     
    public class HC1_JBZS_InfoCollection : List<HC1_JBZS_Info>
    {
    }

     
    public class HC1_SXJJFWZS_Info : ImportInfoBase
    {
        public string ShengX { set; get; }
        public string JiJie { set; get; }
        public string FangWei { set; get; }


        public int SX_shu { set; get; }
        public int SX_niu { set; get; }
        public int SX_hu { set; get; }
        public int SX_tu { set; get; }
        public int SX_long { set; get; }
        public int SX_she { set; get; }
        public int SX_ma { set; get; }
        public int SX_yang { set; get; }
        public int SX_hou { set; get; }
        public int SX_ji { set; get; }
        public int SX_gou { set; get; }
        public int SX_zhu { set; get; }

        public int JJ_chun { set; get; }
        public int JJ_xia { set; get; }
        public int JJ_qiu { set; get; }
        public int JJ_dong { set; get; }

        public int FW_dong { set; get; }
        public int FW_nan { set; get; }
        public int FW_xi { set; get; }
        public int FW_bei { set; get; }
    }
     
    public class HC1_SXJJFWZS_InfoCollection : List<HC1_SXJJFWZS_Info>
    {
    }
}
