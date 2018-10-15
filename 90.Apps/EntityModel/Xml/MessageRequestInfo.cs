using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Xml
{
    public class MessageRequestInfo : XmlMappingObject
    {
        [XmlMapping("head", 0)]
        public MessageHead Head { get; set; }

        [XmlMapping("body", 1)]
        public string Body { get; set; }

        public class MessageHead : XmlMappingObject
        {
            [XmlMapping("transcode", 0, MappingType = MappingType.Attribute)]
            public string Transcode { get; set; }

            [XmlMapping("partnerid", 1, MappingType = MappingType.Attribute)]
            public string Partnerid { get; set; }

            [XmlMapping("version", 2, MappingType = MappingType.Attribute)]
            public string Version { get; set; }

            [XmlMapping("time", 3, MappingType = MappingType.Attribute)]
            public string DateTime { get; set; }
        }
    }

}
