using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;

namespace EntityModel
{
    [ProtoContract]
    [Entity("test_user", Type = EntityType.Table)]
    public class User
    {
        // [Serializable]
        public User()
        {



        }


        [ProtoMember(1)]
        [Field("id", IsIdenty = true, IsPrimaryKey = true)]
        public int ID { get; set; }

        [ProtoMember(1)]
        [Field("test_name")]
        public string Name { get; set; }

        [ProtoMember(1)]
        [Field("relate_id")]
        public int Relate_ID { get; set; }


        public string RelateName { get; set; }

        
    }
}
