﻿using ProtoBuf;
using Kason.Sg.Core.CPlatform;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Model
{
    [ProtoContract]
    public class IdentityUser:RequestData
    {
        [ProtoMember(1)]
        public string RoleId { get; set; }
    }
}
