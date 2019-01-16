using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.Enum;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection
{
    /// <summary>
    /// 文章信息
    /// </summary>
     
    public class News_Info
    {
        public int Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 点击次数
        /// </summary>
        public int Clicks { get; set; }
        /// <summary>
        /// 发布人Id
        /// </summary>
        public string PublisherUserId { get; set; }
        /// <summary>
        /// 新闻类别
        /// </summary>
        public NewsCategory NewsCategory { get; set; }
    }

     
    public class News_InfoCollection
    {
        public News_InfoCollection()
        {
            NewList = new List<News_Info>();
        }

        public List<News_Info> NewList { get; set; }
        public int TotalCount { get; set; }
    }
}
