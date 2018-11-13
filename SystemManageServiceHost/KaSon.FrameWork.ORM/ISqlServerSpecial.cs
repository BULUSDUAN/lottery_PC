using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    using System;
    using System.Data;

    internal interface ISqlServerSpecial
    {
        int BulkAdd(DataTable dataSource, BulkAddParam param);
    }
}
