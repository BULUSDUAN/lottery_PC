using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.Common.ExceptionEx
{
    public static class ExceptionExtension
    {
        public static string ToGetMessage(this Exception ex)
        {
            return ex.Message.Split('★')[0];
        }
    }
}
