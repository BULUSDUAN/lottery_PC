using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Services.Enum
{
    /// <summary>
    /// 数据库提供者信息
    /// </summary>
    public enum WhereType
    {
        Equal,
        UnEqual,
        Less,
        LessOrEqual,
        Than,
        ThanOrEqual,
        LeftLike,
        RightLike,
        Like,
        In,
        NotIn,
        None
    }
}
