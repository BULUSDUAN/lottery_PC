using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.RequestModel
{
    public class QueryUserFollowRuleParam:Page
    {
        public QueryUserFollowRuleParam()
        {           
            this.byFollower = false;
            this.userId=string.Empty;
        }
        public bool byFollower { get; set; }
        public string userId { get; set; }
        public string gameCode { get; set; }
        public string gameType { get; set; }
        public string userToken { get; set; }
    }
}
