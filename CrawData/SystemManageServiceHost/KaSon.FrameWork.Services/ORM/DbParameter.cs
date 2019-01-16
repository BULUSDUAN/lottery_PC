using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace KaSon.FrameWork.Services.ORM
{
    /// <summary>
    /// DB参数
    /// </summary>
    public class DbParameter
    {
        private ParameterDirection _direction = ParameterDirection.Input;

        public System.Data.DbType? DbType { get; set; }

        /// <summary>
        /// </summary>
        public ParameterDirection Direction
        {
            get
            {
                return this._direction;
            }
            set
            {
                this._direction = value;
            }
        }

        public int? Length { get; set; }

        /// <summary>
        /// </summary>
        public string Name { get; set; }

        public OracleDataType? OracleType { get; set; }

        public SqlServerDataType? SqlType { get; set; }


        public MySqlDataType? MySqlType { get; set; }

        /// <summary>
        /// </summary>
        public object Value { get; set; }
    }
}
