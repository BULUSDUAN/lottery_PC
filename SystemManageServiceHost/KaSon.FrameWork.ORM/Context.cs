using KaSon.FrameWork.Services.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    public class Context
    {
        private readonly string _contextId = Guid.NewGuid().ToString();
        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

        public Context(object host)
        {
            this.Host = host;
        }

        public object GetData(string key)
        {
            return this._data[key];
        }

        public void SetData(string key, object obj)
        {
            if (this._data.ContainsKey(key))
            {
                throw new Exception("已有此键值！");
            }
            this._data.Add(key, obj);
        }

        public string ContextId
        {
            get
            {
                return this._contextId;
            }
        }

        public object Host { get; protected set; }

        public string RunId { get; set; }
    }
}
