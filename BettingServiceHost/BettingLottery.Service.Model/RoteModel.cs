﻿using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace BettingLottery.Service.Model
{
    [ProtoContract]
    public class RoteModel
    {
        [ProtoMember(1)]
        public string ServiceId { get; set; }
    }
}
