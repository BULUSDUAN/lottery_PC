using MessagePack;
using ProtoBuf;
using Kason.Sg.Core.System.Intercept;

namespace OrderLottery.Service.Model
{ 
    [ProtoContract]
    public class UserModel
    {

        [ProtoMember(1)]
        [CacheKey(1)]
        public int UserId { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public int Age { get; set; }

    }
}
