using EntityModel.Enum;
using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Domain.Entities
{
    [Serializable]
    [Entity("C_Lottery_Game", Type = EntityType.Table)]
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
