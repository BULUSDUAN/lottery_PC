using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    
    public class IndexMatchInfo
    {
       

        public int Id { get; set; }
        public string MatchId { get; set; }
        public string MatchName { get; set; }
        public string ImgPath { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
