using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel
{

    [ProtoContract]
    [Entity("test_relate", Type = EntityType.Table)]
    public class Relate
    {
        public Relate()
        {



        }
        [ProtoMember(1)]
        [Field("id", IsIdenty = true, IsPrimaryKey = true)]
        public int ID { get; set; }

        [ProtoMember(2)]
        [Field("relate_name")]
        public string Name { get; set; }



    }
}
