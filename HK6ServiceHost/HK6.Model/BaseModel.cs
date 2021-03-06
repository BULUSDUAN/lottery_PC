﻿using ProtoBuf;
using Kason.Sg.Core.System.Intercept;
using System;
using System.Collections.Generic;
using System.Text;

namespace SystemManage.Model
{
    [ProtoContract]
    public class BaseModel
    {
        [ProtoMember(1)]
        public Guid Id => Guid.NewGuid();
    }
}
