using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal sealed class QueryParameterColletion : ICollection<QueryParameter>, IEnumerable<QueryParameter>, IEnumerable
    {
        private string _cacheKey;
        private bool _locked;
        private readonly List<QueryParameter> _parameters = new List<QueryParameter>();

        internal QueryParameterColletion()
        {
        }

        public void Add(QueryParameter parameter)
        {
            if (this.Contains(parameter))
            {
                throw new Exception("已存在此参数");
            }
            if (this.Contains(parameter.Name))
            {
                throw new Exception("已存在此参数名");
            }
            this._parameters.Add(parameter);
            this._cacheKey = null;
        }

        private void CheckUnlocked()
        {
            if (this._locked)
            {
                throw new ArgumentNullException();
            }
        }

        public void Clear()
        {
            this.CheckUnlocked();
            this._parameters.Clear();
            this._cacheKey = null;
        }

        public bool Contains(QueryParameter parameter)
        {
            return this._parameters.Contains(parameter);
        }

        public bool Contains(string name)
        {
            return (this.IndexOf(name) != -1);
        }
        public bool ContainsEx(string name,out QueryParameter qp) {
            qp = null;
            int index = this.IndexOf(name);
            if (index == -1)
            {
                return false;
            }
            qp= this._parameters[index]; 
            return true;
        }
        public void CopyTo(QueryParameter[] array, int index)
        {
            this._parameters.CopyTo(array, index);
        }

        internal static QueryParameterColletion DeepCopy(QueryParameterColletion copyParams)
        {
            if (copyParams == null)
            {
                return null;
            }
            QueryParameterColletion colletion = new QueryParameterColletion();
            foreach (QueryParameter parameter in (IEnumerable<QueryParameter>)copyParams)
            {
                colletion.Add(parameter.ShallowCopy());
            }
            return colletion;
        }

        internal string GetCacheKey()
        {
            if ((this._cacheKey == null) && (this._parameters.Count > 0))
            {
                if (1 == this._parameters.Count)
                {
                    QueryParameter parameter = this._parameters[0];
                    this._cacheKey = "@@1" + parameter.Name + ":" + parameter.ParameterType.FullName;
                }
                else
                {
                    StringBuilder builder = new StringBuilder(this._parameters.Count * 20);
                    builder.Append("@@");
                    builder.Append(this._parameters.Count);
                    for (int i = 0; i < this._parameters.Count; i++)
                    {
                        if (i > 0)
                        {
                            builder.Append(";");
                        }
                        QueryParameter parameter2 = this._parameters[i];
                        builder.Append(parameter2.Name);
                        builder.Append(":");
                        builder.Append(parameter2.ParameterType.FullName);
                    }
                    this._cacheKey = builder.ToString();
                }
            }
            return this._cacheKey;
        }

        private int IndexOf(string name)
        {
            int num = 0;
            foreach (QueryParameter parameter in this._parameters)
            {
                if (string.Compare(name, parameter.Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return num;
                }
                num++;
            }
            return -1;
        }

        public bool Remove(QueryParameter parameter)
        {
            this.CheckUnlocked();
            bool flag = this._parameters.Remove(parameter);
            if (flag)
            {
                this._cacheKey = null;
            }
            return flag;
        }

        public void Remove(string providerId)
        {
            this._parameters.RemoveAll(where => where.ProviderId == providerId);
        }

        public void SafeAdd(string name, string providerId, Type type)
        {
            //&& type.CustomAttributes.Count()>0
            if (!this.Contains(name) )
            {
                this.Add(new QueryParameter(name, providerId, type));
            }
        }

        internal void SetReadOnly(bool isReadOnly)
        {
            this._locked = isReadOnly;
        }

        IEnumerator<QueryParameter> IEnumerable<QueryParameter>.GetEnumerator()
        {
            return this._parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._parameters.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return this._parameters.Count;
            }
        }

        public QueryParameter this[string name]
        {
            get
            {
                int index = this.IndexOf(name);
                if (index == -1)
                {
                    throw new NullReferenceException("没有此参数！" + name);
                }
                return this._parameters[index];
            }
        }

        bool ICollection<QueryParameter>.IsReadOnly
        {
            get
            {
                return this._locked;
            }
        }
    }
}
