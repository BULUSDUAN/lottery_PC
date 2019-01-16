using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Services.Attribute
{
    /// <summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EntityAttribute : System.Attribute
    {
        private EntityType _type = EntityType.Table;

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        public EntityAttribute(string name)
        {
            this.Name = name;
            this.EmptyIsUpdate = true;
        }
        public EntityAttribute(string name,bool emptyIsUpdate)
        {
            this.Name = name;
            this.EmptyIsUpdate = emptyIsUpdate;
        }
        /// <summary>
        /// 实体对应的表名/视图名
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        /// 整个实体更新时，是否空字段要更新
        /// </summary>
        public bool EmptyIsUpdate { get; set; }

        /// <summary>
        /// 实体类型
        /// </summary>
        public EntityType Type
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }
    }
}
