using KaSon.FrameWork.Services.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal class EntityInfo
    {
        private readonly IList<string> _primaryKeys = new List<string>();

        /// <summary>
        /// 自动递增键
        /// </summary>
        public string Autorecode { get; set; }

        public EntityAttribute Entity { get; set; }

        public Type EntityType { get; set; }

        public IDictionary<string, FieldMap> Fields { get; set; }

        public bool IsEncrypt { get; set; }

        public bool IsEntity { get; set; }

        public IList<string> PrimaryKeys
        {
            get
            {
                return this._primaryKeys;
            }
        }

        public IDictionary<string, PropertyMap> Propertys { get; set; }
    }
}
