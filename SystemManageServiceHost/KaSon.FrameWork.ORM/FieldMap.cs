using KaSon.FrameWork.Services.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal class FieldMap
    {
        public EncryptAttribute Encrypt { get; set; }

        public FieldAttribute Field { get; set; }

        public IdentityAttribute Identity { get; set; }

        public OracleTypeAttribute OracleType { get; set; }

        public PropertyMap Property { get; set; }

        public SequenceAttribute Sequence { get; set; }

        public SqlServerTypeAttribute SqlType { get; set; }

        public VersionAttribute Version { get; set; }
    }
}
