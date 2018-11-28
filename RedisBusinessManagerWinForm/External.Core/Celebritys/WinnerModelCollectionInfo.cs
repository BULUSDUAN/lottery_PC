using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.Celebritys
{
    [CommunicationObject]
    public class WinnerModelCollectionInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ModelCollectionId { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 模型Id
        /// </summary>
        public string ModelId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class WinnerModelCollectionInfoList
    {
        public WinnerModelCollectionInfoList()
        {
            ModeCollectionList = new List<WinnerModelCollectionInfo>();
        }
        public int TotalCount { get; set; }
        public List<WinnerModelCollectionInfo> ModeCollectionList { get; set; }
    }
}
