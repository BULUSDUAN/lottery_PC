using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using System.Collections;
using Common.Utilities;

namespace External.Core.SiteMessage
{
    /// <summary>
    /// 文章资讯
    /// </summary>
    [CommunicationObject]
    public class ArticleInfo_Add
    {
        /// <summary>
        /// 对应彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWords { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string DescContent { get; set; }
        /// <summary>
        /// 是否标红
        /// </summary>
        public bool IsRedTitle { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 创建者编号
        /// </summary>
        public string CreateUserKey { get; set; }
        /// <summary>
        /// 创建者显示名称
        /// </summary>
        public string CreateUserDisplayName { get; set; }
    }
    /// <summary>
    /// 文章资讯
    /// </summary>
    [CommunicationObject]
    public class ArticleInfo_Query
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 对应彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWords { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string DescContent { get; set; }
        /// <summary>
        /// 是否标红
        /// </summary>
        public bool IsRedTitle { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 显示次序
        /// </summary>
        public int ShowIndex { get; set; }
        /// <summary>
        /// 阅读次数
        /// </summary>
        public int ReadCount { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建者编号
        /// </summary>
        public string CreateUserKey { get; set; }
        /// <summary>
        /// 创建者显示名称
        /// </summary>
        public string CreateUserDisplayName { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 更新者编号
        /// </summary>
        public string UpdateUserKey { get; set; }
        /// <summary>
        /// 更新者显示名称
        /// </summary>
        public string UpdateUserDisplayName { get; set; }
        /// <summary>
        /// 静态文件地址
        /// </summary>
        public string StaticPath { get; set; }
        /// <summary>
        /// 上一条
        /// </summary>
        public string PreId { get; set; }
        public string PreTitle { get; set; }
        public string PreStaticPath { get; set; }
        /// <summary>
        /// 下一条
        /// </summary>
        public string NextId { get; set; }
        public string NextTitle { get; set; }
        public string NextStaticPath { get; set; }

        internal void LoadArray(object[] dataArray)
        {
            if (dataArray.Length == 19)
            {
                Id = (string)dataArray[0];
                GameCode = (string)dataArray[1];
                Title = (string)dataArray[2];
                Category = (string)dataArray[3];
                ShowIndex = (int)dataArray[4];
                ReadCount = (int)dataArray[5];
                CreateTime = (DateTime)dataArray[6];
                CreateUserKey = (string)dataArray[7];
                CreateUserDisplayName = (string)dataArray[8];
                UpdateTime = (DateTime)dataArray[9];
                UpdateUserKey = (string)dataArray[10];
                UpdateUserDisplayName = (string)dataArray[11];
                IsRedTitle = (bool)dataArray[12];
                KeyWords = UsefullHelper.GetDbValue<string>(dataArray[13]);
                DescContent = UsefullHelper.GetDbValue<string>(dataArray[14]);
                StaticPath = UsefullHelper.GetDbValue<string>(dataArray[15]);
                PreStaticPath = UsefullHelper.GetDbValue<string>(dataArray[16]);
                NextStaticPath = UsefullHelper.GetDbValue<string>(dataArray[17]);
                Description = UsefullHelper.GetDbValue<string>(dataArray[18]);
            }
            else
            {
                throw new ArgumentException("转换成此ArticleInfo_Query对象的数据数组长度不正确，传入数组为" + dataArray.Length + "。" + string.Join(", ", dataArray), "dataArray");
            }
        }
    }
    /// <summary>
    /// 文章资讯列表
    /// </summary>
    [CommunicationObject]
    public class ArticleInfo_QueryCollection
    {
        public ArticleInfo_QueryCollection()
        {
            ArticleList = new List<ArticleInfo_Query>();
        }
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 公告列表
        /// </summary>
        public IList<ArticleInfo_Query> ArticleList { get; set; }
        /// <summary>
        /// 加载存储过程返回数组列表
        /// </summary>
        /// <param name="list">数组列表</param>
        public void LoadList(IList list)
        {
            foreach (object[] item in list)
            {
                var info = new ArticleInfo_Query();
                info.LoadArray(item);
                ArticleList.Add(info);
            }
        }
    }
}
