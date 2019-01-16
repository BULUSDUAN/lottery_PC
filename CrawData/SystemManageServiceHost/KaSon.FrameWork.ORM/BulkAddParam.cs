

namespace KaSon.FrameWork.ORM
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class BulkAddParam
    {
        private List<ColumnMapping> _columnMappings = new List<ColumnMapping>();

        public List<ColumnMapping> ColumnMappings
        {
            get
            {
                return this._columnMappings;
            }
            set
            {
                this._columnMappings = value;
            }
        }

        public string TableName { get; set; }
    }
}
