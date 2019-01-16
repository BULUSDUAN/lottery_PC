namespace KaSon.FrameWork.ORM.Dal
{
    using KaSon.FrameWork.Services.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal abstract class DalHelper
    {
        protected DalHelper()
        {
        }

        public virtual object ExcuteAddWithGetId(string sql, DbParameterCollection paramerters)
        {
            return null;
        }

        public virtual void ExcuteBulkAdd<T>(IList<T> entitys)
        {
        }

        public virtual IList<T> GetRandomList<T>(LambdaContext context, Expression where, int count)
        {
            return null;
        }
    }
}

