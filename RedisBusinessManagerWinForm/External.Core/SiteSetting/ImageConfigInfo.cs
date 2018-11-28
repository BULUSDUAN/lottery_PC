using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.SiteSetting
{
    [CommunicationObject]
    public class ImageConfigInfo
    {
        public string Index { get; set; }
        public string Title { get; set; }
        public string PicUrl { get; set; }
        public string picDesc { get; set; }
        public string ArticleUrl { get; set; }
        public DateTime ModifyTime { get; set; }
    }                                                                                                                                                                                                                                                                                                                                                                          
    [CommunicationObject]
    public class ImageConfigInfo_QueryCollection
    {
        public ImageConfigInfo_QueryCollection()
        {
            ConfigList = new List<ImageConfigInfo>();
        }

        public IList<ImageConfigInfo> ConfigList { get; set; }
    }
}
