using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
   public class IndexMatch
    {
       /// <summary>
       /// 主键Id
       /// </summary>
       public virtual int Id { get; set; }
       /// <summary>
       /// 比赛编号
       /// </summary>
       public virtual string MatchId { get; set; }
       /// <summary>
       /// 比赛名字
       /// </summary>
       public virtual string MatchName { get; set; }
       /// <summary>
       /// 队伍图片路径
       /// </summary>
       public virtual string ImgPath { get; set; }
       /// <summary>
       /// 创建时间
       /// </summary>
       public virtual DateTime CreateTime { get; set; }
    }
}
