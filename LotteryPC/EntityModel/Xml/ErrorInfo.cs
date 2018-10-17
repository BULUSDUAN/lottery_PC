using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Xml
{
    public class ErrorInfo : XmlMappingObject
    {
        [XmlMapping("transcode", 0, MappingType = MappingType.Attribute)]
        public string TransCode { get; set; }

        [XmlMapping("message", 1, MappingType = MappingType.Attribute)]
        public string Message { get; set; }
    }
}
