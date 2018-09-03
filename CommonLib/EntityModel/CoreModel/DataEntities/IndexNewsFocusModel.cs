using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    public class IndexNewsFocusModel
    {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Category { get; set; }
            public DateTime CreateTime { get; set; }
            public bool IsRedTitle { get; set; }
            //public string NewsType { get; set; }
    }
}
