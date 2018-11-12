using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal class Page
    {
        public int Index { get; set; }

        public string OrderBy { get; set; }

        public int Row { get; set; }


        public string AppendOrderBy { get; set; }

    }
}
