using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
namespace EntityModel.CoreModel
{
    [ProtoContract]
   public class GameWinNumber_InfoCollection:Page
    {
        public GameWinNumber_InfoCollection()
        {
            List = new List<GameWinNumber_Info>();
        }
        [ProtoMember(1)]
        public List<GameWinNumber_Info> List { get; set; }
    }
    [ProtoContract]
    public class GameWinNumber_Info
    {
        public GameWinNumber_Info(){ }
        [ProtoMember(1)]
        public long Id { get; set; }
        [ProtoMember(2)]
        public string IssuseNumber { get; set; }
        [ProtoMember(3)]
        public string GameCode { get; set; }
        [ProtoMember(4)]
        public string GameType { get; set; }
        [ProtoMember(5)]
        public string WinNumber { get; set; }
        [ProtoMember(6)]
        public DateTime CreateTime { get; set; }
    }
}
