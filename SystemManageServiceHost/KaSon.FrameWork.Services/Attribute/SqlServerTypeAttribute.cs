using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Services.Attribute
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SqlServerTypeAttribute : System.Attribute
    {
        private SqlServerDataType _type;

        public SqlServerTypeAttribute(SqlServerDataType type)
        {
            this._type = type;
        }

        public SqlServerDataType DataType
        {
            get
            {
                return this._type;
            }
        }
    }
}
