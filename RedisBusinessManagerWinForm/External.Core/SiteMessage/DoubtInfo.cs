using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using System.Collections;

namespace External.Core.SiteMessage
{
    /// <summary>
    /// 常见问题
    /// </summary>
    [CommunicationObject]
    public class DoubtInfo_Add
    {
        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public virtual string KeyWords { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string DescContent { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public virtual string Category { get; set; }
        /// <summary>
        /// 创建者编号
        /// </summary>
        public virtual string CreateUserKey { get; set; }
        /// <summary>
        /// 创建者显示名称
        /// </summary>
        public virtual string CreateUserDisplayName { get; set; }
    }
    /// <summary>
    /// 常用问题信息
    /// </summary>
    [CommunicationObject]
    public class DoubtInfo_Query
    {
        /// <summary>
        /// 编号
        /// </summary>
        public virtual string Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public virtual string KeyWords { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string DescContent { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public virtual string Category { get; set; }
        /// <summary>
        /// 显示次序
        /// </summary>
        public virtual int ShowIndex { get; set; }
        /// <summary>
        /// 顶总数
        /// </summary>
        public virtual int UpCount { get; set; }
        /// <summary>
        /// 踩总数
        /// </summary>
        public virtual int DownCount { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建者编号
        /// </summary>
        public virtual string CreateUserKey { get; set; }
        /// <summary>
        /// 创建者显示名称
        /// </summary>
        public virtual string CreateUserDisplayName { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
        /// <summary>
        /// 更新者编号
        /// </summary>
        public virtual string UpdateUserKey { get; set; }
        /// <summary>
        /// 更新者显示名称
        /// </summary>
        public virtual string UpdateUserDisplayName { get; set; }
        /// <summary>
        /// 已做操作
        /// </summary>
        public virtual string Operation { get; set; }

        internal void LoadArray(object[] dataArray)
        {
            if (dataArray.Length == 12 || dataArray.Length == 14)
            {
                Id = (string)dataArray[0];
                Title = (string)dataArray[1];
                Category = (string)dataArray[2];
                ShowIndex = (int)dataArray[3];
                UpCount = (int)dataArray[4];
                DownCount = (int)dataArray[5];
                CreateTime = (DateTime)dataArray[6];
                CreateUserKey = (string)dataArray[7];
                CreateUserDisplayName = (string)dataArray[8];
                UpdateTime = (DateTime)dataArray[9];
                UpdateUserKey = (string)dataArray[10];
                UpdateUserDisplayName = (string)dataArray[11];
                if (dataArray.Length == 14)
                {
                    Description = (string)dataArray[12];
                    if (dataArray[13] == null || dataArray[13] == DBNull.Value)
                    {
                        Operation = null;
                    }
                    else
                    {
                        Operation = (string)dataArray[13];
                    }
                }
            }
            else
            {
                throw new ArgumentException("转换成此DoubtInfo_Query对象的数据数组长度不满足要求，传入数组为" + dataArray.Length + "。" + string.Join(", ", dataArray), "dataArray");
            }
        }
    }
    /// <summary>
    /// 常用问题列表
    /// </summary>
    [CommunicationObject]
    public class DoubtInfo_QueryCollection
    {
        public DoubtInfo_QueryCollection()
        {
            DoubtList = new List<DoubtInfo_Query>();
        }
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 公告列表
        /// </summary>
        public IList<DoubtInfo_Query> DoubtList { get; set; }
        /// <summary>
        /// 加载存储过程返回数组列表
        /// </summary>
        /// <param name="list">数组列表</param>
        public void LoadList(IList list)
        {
            foreach (object[] item in list)
            {
                var info = new DoubtInfo_Query();
                info.LoadArray(item);
                DoubtList.Add(info);
            }
        }
    }
}
