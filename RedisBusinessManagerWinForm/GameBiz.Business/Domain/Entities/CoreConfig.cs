using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class CoreConfig
    {
        public virtual int Id { get; set; }
        public virtual string ConfigKey { get; set; }
        public virtual string ConfigName { get; set; }
        public virtual string ConfigValue { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
