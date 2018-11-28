using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
   public class VirtualUserData
    {
       public virtual int Id { get; set; }
       public virtual string NickName { get; set; }
       public virtual string RealName { get; set; }
       public virtual string Mobile { get; set; }
       public virtual string IdCardNumber { get; set; }
       public virtual string CreateUserId { get; set; }
       public virtual bool IsUser { get; set; }
       public virtual DateTime CreateTime { get; set; }
    }
}
