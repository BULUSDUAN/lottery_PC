using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Mdel
{

   
   internal class SelectRightNodeMdel
    {

        public string MemberName { get; set; }
        public bool RightIsClass { get; set; }

        public string FieldName { get; set; }

        public Type RightNodeType { get; set; }

        public IList<FieldMdel> ColumList { get; set; } = new List<FieldMdel>();


    }
}
