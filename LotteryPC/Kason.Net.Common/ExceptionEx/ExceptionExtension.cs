using System;
using System.Collections.Generic;
using System.Text;

namespace Kason.Net.Common.ExceptionEx
{
    public static class ExceptionExtension
    {
        public static string ToGetMessage(this Exception ex)
        {
            return ex.Message.Split('★')[0].Replace("\r\n","");
        }
    }
}
