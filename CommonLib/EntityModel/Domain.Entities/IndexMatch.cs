using EntityModel.Enum;
using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Domain.Entities
{
  
    [Entity("C_Index_Match", Type = EntityType.Table)]
    public class IndexMatch
    {
        /// <summary>
        /// 主键Id
        /// </summary
        [Field("Id", IsPrimaryKey = true,IsIdenty =true)]
        public virtual int Id { get; set; }
        /// <summary>
        /// 比赛编号
        /// </summary>
        [Field("MatchId")]
        public virtual string MatchId { get; set; }
        /// <summary>
        /// 比赛名字
        /// </summary
        [Field("MatchName")]
        public virtual string MatchName { get; set; }
        /// <summary>
        /// 队伍图片路径
        /// </summary>
        [Field("ImgPath")]
        public virtual string ImgPath { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public virtual DateTime CreateTime { get; set; }
    }
}
