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
}
}