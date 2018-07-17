using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace EntityModel
{
    [ProtoContract]
   public class Page 
    {
        /// <summary>
        /// 当前页数
        /// </summary>
        [ProtoMember(1)]
        public int pageIndex { get; set; }
        /// <summary>
        /// 每页数据条数
        /// </summary>
        [ProtoMember(2)]
        public int pageSize { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        [ProtoMember(3)]
        public int TotalCount { get; set; }
        /// <summary>
        /// 允许最大数
        /// </summary>
        [ProtoMember(4)]
        public int MaxPageSize = 200;
        [ProtoMember(5)]
        public string OrderBy { get; set; }
        [ProtoMember(6)]
        public string SortBy { get; set; }
    }
}
