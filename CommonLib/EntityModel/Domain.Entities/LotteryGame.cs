﻿using EntityModel.Enum;
using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Domain.Entities
{
    [Entity("C_Auth_UserFunction", Type = EntityType.Table)]
    public class LotteryGame
    {
        [Field("GameCode")]
        public  string GameCode { get; set; }
        [Field("DisplayName")]
        public  string DisplayName { get; set; }

        [Field("EnableStatus")]
        public  EnableStatus EnableStatus { get; set; }
    }
}
