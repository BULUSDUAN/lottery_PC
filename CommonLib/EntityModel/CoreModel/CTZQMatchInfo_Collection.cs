using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
namespace EntityModel.CoreModel
{
    [ProtoContract]
    public class CTZQMatchInfo_Collection : Page
    {

        public CTZQMatchInfo_Collection()
        {
            ListInfo = new List<CTZQMatchInfo>();
        }
        [ProtoMember(1)]
        public List<CTZQMatchInfo> ListInfo { get; set; }
    } 
}
