using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Services.Attribute
{
    /// <summary>
    /// SqlServer 自增ID
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IdentityAttribute : System.Attribute
    {
    }
}
