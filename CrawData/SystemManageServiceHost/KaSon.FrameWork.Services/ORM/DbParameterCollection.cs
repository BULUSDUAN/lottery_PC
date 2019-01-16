using KaSon.FrameWork.Services.Attribute;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace KaSon.FrameWork.Services.ORM
{
    /// <summary>
    ///  Db 参数
    /// </summary>
    public class DbParameterCollection : IEnumerable, IEnumerator
    {
        private readonly List<DbParameter> _parameters = new List<DbParameter>();
        private int _position = -1;

        public void Add(DbParameter param)
        {
            this._parameters.Add(param);
        }

        public void Clear()
        {
            this._parameters.Clear();
        }

        public int Count()
        {
            return this._parameters.Count<DbParameter>();
        }

        public DbParameterCollection GetEnumerator()
        {
            this._position = -1;
            return this;
        }

        public void Insert(string name, object value, ParameterDirection direction)
        {
            DbParameter item = new DbParameter
            {
                Name = name,
                Value = value,
                Direction = direction
            };
            this._parameters.Add(item);
        }

        public bool MoveNext()
        {
            this._position++;
            return (this._position < this._parameters.Count);
        }
      
        public void Remove(DbParameter param)
        {
            this._parameters.Remove(param);
        }

        public void Reset()
        {
            this._position = -1;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public DbParameter Current
        {
            get
            {
                DbParameter parameter;
                try
                {
                    parameter = this._parameters[this._position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
                return parameter;
            }
        }

        public DbParameter this[int index]
        {
            get
            {
                return this._parameters[index];
            }
            set
            {
                this._parameters[index] = value;
            }
        }

        public DbParameter this[string name]
        {
            get
            {
                return this._parameters.FirstOrDefault<DbParameter>(param => (param.Name == name));
            }
            set
            {
                foreach (DbParameter parameter in this._parameters)
                {
                    if (parameter.Name == name)
                    {
                        parameter.Value = value;
                    }
                }
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }
    }
}
