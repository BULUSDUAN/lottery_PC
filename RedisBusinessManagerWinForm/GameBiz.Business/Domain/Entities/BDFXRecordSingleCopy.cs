using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
   public class BDFXRecordSingleCopy
    {
       public virtual int Id { get; set; }
       public virtual string BDXFSchemeId { get; set; }
       public virtual string SingleCopySchemeId { get; set; }
       public virtual DateTime CreateTime { get; set; }
    }
}
