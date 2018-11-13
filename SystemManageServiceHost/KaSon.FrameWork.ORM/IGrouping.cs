using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal interface IGrouping
    {
        /// <summary>
        /// Adds an object to the current group.
        /// </summary>
        /// <param name="item">The <see cref="T:System.Object"/> to add.</param>
        void Add(object item);

        object GetMe();
    }
}
