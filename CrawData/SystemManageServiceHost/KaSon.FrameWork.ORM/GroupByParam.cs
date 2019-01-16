using KaSon.FrameWork.Services.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal class GroupByParam
    {
        public GroupByParam()
        {
            this.KeyList = new Dictionary<string, GroupByParamKey>();
        }

        public Dictionary<string, GroupByParamKey> KeyList { get; private set; }
    }
}
