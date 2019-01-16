using KaSon.FrameWork.Services.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal class PropertyMap
    {
        private Func<object, object> _get = null;
        private Action<object, object> _set = null;

        public bool AutoConvert { get; set; }

        public FieldMap FieldMap { get; set; }

        public Func<object, object> Get
        {
            get
            {
                return this._get;
            }
            set
            {
                this._get = delegate(object entity)
                {
                    object obj2;
                    try
                    {
                        obj2 = value(entity);
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("获取字段" + this.Info.Name + "的值时发生错误", exception);
                    }
                    return obj2;
                };
            }
        }

        public PropertyInfo Info { get; set; }

        public Action<object, object> Set
        {
            get
            {
                return this._set;
            }
            set
            {
                this._set = delegate(object entity, object obj)
                {
                    try
                    {
                        value(entity, OperateCommon.AutoConvert(obj, this.Info.PropertyType));
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("设置字段" + this.Info.Name + "的值时发生错误", exception);
                    }
                };
            }
        }
    }
}
