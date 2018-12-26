using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    /// <summary>
    // 
    ///</summary>

    public class playGroup
    {
        public playGroup()
        {

        }
        public string Key { get; set; }
        public string Name { get; set; }
        public List<blast_lhc_antecode> CodeList { get; set; }

        public List<blast_lhc_antecode> CodeList1 { get; set; }

        public List<blast_lhc_antecode> CodeList2 { get; set; }

        public List<blast_lhc_antecode> CodeList3 { get; set; }

        public List<blast_lhc_antecode> CodeList4 { get; set; }


        public List<blast_lhc_antecode> CodeList5 { get; set; }

        public List<blast_lhc_antecode> CodeList6 { get; set; }

        public List<blast_lhc_antecode> CodeList7 { get; set; }


        public List<blast_lhc_antecode> CodeList8 { get; set; }


        public List<blast_lhc_antecode> CodeList9 { get; set; }
    }
}