﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class SysOpratorInfo
    {
        public string LoginName { get; set; }
        public string UserId { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
    }
    [CommunicationObject]
    public class SysOpratorInfo_Collection
    {
        public SysOpratorInfo_Collection()
        {
            OpratorListInfo = new List<SysOpratorInfo>();
        }
        public int TotalCount { get; set; }
        public List<SysOpratorInfo> OpratorListInfo { get; set; }
    }
}
