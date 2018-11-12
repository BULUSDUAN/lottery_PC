using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Services.Attribute
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class OracleTypeAttribute : System.Attribute
    {
        private OracleDataType _type;

        public OracleTypeAttribute(OracleDataType type)
        {
            this._type = type;
        }

        public OracleDataType DataType
        {
            get
            {
                return this._type;
            }
        }
    }
}
