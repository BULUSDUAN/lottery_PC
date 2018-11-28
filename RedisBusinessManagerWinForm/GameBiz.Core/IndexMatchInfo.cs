using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class IndexMatchInfo
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 比赛编号
        /// </summary>
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛名字
        /// </summary>
        public string MatchName { get; set; }
        /// <summary>
        /// 队伍图片路径
        /// </summary>
        public string ImgPath { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class IndexMatch_Collection
    {
        public IndexMatch_Collection()
        {
            IndexMatchList = new List<IndexMatchInfo>();
        }
        public int TotalCount { get; set; }
        public List<IndexMatchInfo> IndexMatchList { get; set; }
    }
}
