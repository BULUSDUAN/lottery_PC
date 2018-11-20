using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using System.Collections;

namespace External.Core.Report
{
    /// <summary>
    /// 排行榜信息项
    /// </summary>
    [CommunicationObject]
    public class RankInfo
    {
        public int Index { get; set; }
        public string UserId { get; set; }
        public string UserKey { get; set; }
        public string UserDisplayName { get; set; }
        public string UserComeFrom { get; set; }
        public decimal RankMoney { get; set; }
        public string GameCode { get; set; }
        public string GameDispayName { get; set; }

        internal void LoadArray(object[] dataArray)
        {
            if (dataArray.Length != 5)
            {
                throw new ArgumentException("转换成此RankInfo对象的数据数组长度必须是5，传入数组为" + dataArray.Length + "。" + string.Join(", ", dataArray), "dataArray");
            }
            UserId = (string)dataArray[0];
            UserKey = (string)dataArray[1];
            UserDisplayName = (string)dataArray[2];
            UserComeFrom = (string)dataArray[3];
            RankMoney = (decimal)dataArray[4];
        }
    }
    [CommunicationObject]
    public class RankInfoCollection : List<RankInfo>
    {
        /// <summary>
        /// 加载存储过程返回数组列表
        /// </summary>
        /// <param name="list">数组列表</param>
        public void LoadList(IList list)
        {
            var i = 1;
            foreach (object[] item in list)
            {
                var info = new RankInfo
                {
                    Index = i++,
                };
                info.LoadArray(item);
                Add(info);
            }
        }
    }
}
