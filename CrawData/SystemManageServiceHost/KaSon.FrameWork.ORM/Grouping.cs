using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    public class Grouping<K, T> : IGrouping<K, T>, IGrouping
    {
        private readonly IList<object> list = new List<object>();
        public Grouping(K key, IEnumerable<T> group)
        {
            _key = key;
            _group = group;
        }
        public Grouping(K key)
        {
            this._key = key;
        }

        private readonly K _key;
        private readonly IEnumerable<T> _group;

        public K Key
        {
            get { return _key; }
        }
        public void Add(object item)
        {
            list.Add(item);
        }
        public IEnumerable<T> Group
        {
            get { return _group; }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (null == _group)
            {
                yield break;
            }
            foreach (T member in _group)
            {
                yield return member;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }


        public object GetMe()
        {
            return this;
        }
    }
}
