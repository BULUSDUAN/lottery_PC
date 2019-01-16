using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Mdel
{
  public  class FieldMdel
    {

        public string MemberName { get; set; }

        public string FieldName { get; set; }
        public string Alis { get; set; }
        public string FieldTypeName { get; set; }

        public Type FieldType { get; set; }
    }
}
