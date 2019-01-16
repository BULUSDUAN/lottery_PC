using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
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
