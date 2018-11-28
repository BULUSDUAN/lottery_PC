using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Common.Communication;
using System.Collections;
using GameBiz.Core;

namespace External.Core.SiteMessage
{
    /// <summary>
    /// 新增公告的信息对象
    /// </summary>
    [CommunicationObject]
    public class BulletinInfo_Publish
    {
        /// <summary>
        /// 公告标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 公告内容。可以是HTML
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 有效期从。如为null表示及时启用
        /// </summary>
        public DateTime? EffectiveFrom { get; set; }
        /// <summary>
        /// 有效期至。如为null表示不过期
        /// </summary>
        public DateTime? EffectiveTo { get; set; }
        /// <summary>
        /// 公告状态：正常，取消
        /// </summary>
        public EnableStatus Status { get; set; }
        /// <summary>
        /// 公告代理商
        /// </summary>
        public BulletinAgent BulletinAgent { get; set; }
        /// <summary>
        /// 优先级别
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public int IsPutTop { get; set; }
        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}[优先级:{1}][置顶:{2}]", Title, Priority, IsPutTop);
        }
    }
    /// <summary>
    /// 新增公告的信息对象，用于后台查询
    /// </summary>
    [CommunicationObject]
    public class BulletinInfo_Query
    {
        /// <summary>
        /// 编号
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 公告标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 公告内容。可以是HTML
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 有效期从。如为null表示及时启用
        /// </summary>
        public DateTime? EffectiveFrom { get; set; }
        /// <summary>
        /// 有效期至。如为null表示不过期
        /// </summary>
        public DateTime? EffectiveTo { get; set; }
        /// <summary>
        /// 公告状态：正常，取消
        /// </summary>
        public EnableStatus Status { get; set; }
        /// <summary>
        /// 公告代理商
        /// </summary>
        public BulletinAgent BulletinAgent { get; set; }
        /// <summary>
        /// 优先级别
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 是否置顶。1 为是，0 为否
        /// </summary>
        public int IsPutTop { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// 创建者显示名称
        /// </summary>
        public string CreatorDisplayName { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdateBy { get; set; }
        /// <summary>
        /// 更新者显示名称
        /// </summary>
        public string UpdatorDisplayName { get; set; }

        internal void LoadArray(object[] dataArray)
        {
            if (dataArray.Length == 8)
            {
                Id = (long)dataArray[0];
                Title = (string)dataArray[1];
                Status = (EnableStatus)dataArray[2];
                Priority = (int)dataArray[3];
                IsPutTop = (int)dataArray[4];
                EffectiveFrom = (DateTime)dataArray[5];
                EffectiveTo = (DateTime)dataArray[6];
                CreateTime = (DateTime)dataArray[7];
            }
            else
            {
                throw new ArgumentException("转换成此BulletinInfo_Query对象的数据数组长度不能识别，传入数组为" + dataArray.Length + "。" + string.Join(", ", dataArray), "dataArray");
            }
        }
    }
    /// <summary>
    /// 新增公告的信息对象，用于前台查询
    /// </summary>
    [CommunicationObject]
    public class BulletinInfo_Collection
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BulletinInfo_Collection()
        {
            BulletinList = new List<BulletinInfo_Query>();
        }
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 公告列表
        /// </summary>
        public List<BulletinInfo_Query> BulletinList { get; set; }
        /// <summary>
        /// 加载存储过程返回数组列表
        /// </summary>
        /// <param name="list">数组列表</param>
        public void LoadList(IList list)
        {
            foreach (object[] item in list)
            {
                var info = new BulletinInfo_Query();
                info.LoadArray(item);
                BulletinList.Add(info);
            }
        }
    }
}
