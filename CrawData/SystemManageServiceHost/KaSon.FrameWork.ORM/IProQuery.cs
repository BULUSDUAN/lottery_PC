using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    public interface IProQuery
    {
        IList<T> List<T>();
        T First<T>();

        int Excute();
    }
}
