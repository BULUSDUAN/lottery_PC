using KaSon.FrameWork.ORM.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal class QueryProviderContext<T> : QueryProviderContext
    {
        private Func<QueryProviderContext, T> _func;

        public Type tp;
        public QueryProviderContext(DbProvider dp)
            : base(dp)
        {
           // tp =typeof( T);
        }

        public T Execute()
        {
            return this._func(this);
        }

        public void SetExecuteFunc(Func<QueryProviderContext, T> func)
        {
            this._func = func;
        }
     
    }
}
