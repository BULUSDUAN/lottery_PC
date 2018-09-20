using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameBiz.Core;

namespace app.lottery.site.cbbao.Models
{
    public class ZixunComment
    {
        public string UserDisplayName { get; set; } //用户名
        public string TextNum { get; set; } //文章编号
        public string Content { get; set; } //评论内容
        public string CreateTime { get; set; } //评论发表时间
    }
}