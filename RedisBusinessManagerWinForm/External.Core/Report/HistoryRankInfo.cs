using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using System.Collections;

namespace External.Core.Report
{
    [CommunicationObject]
    public class HistoryRankInfo
    {
        public int BonusCount { get; set; }
        public decimal BonusMoney { get; set; }
        public string DisplayName { get; set; }
        public string IdCardNumber { get; set; }
        public string LocalMobile { get; set; }
        public string RealName { get; set; }
        public string UserId { get; set; }
        public string UserKey { get; set; }

        internal void LoadArray(object[] dataArray)
        {
            if (dataArray.Length == 8)
            {
                BonusCount = (int)dataArray[0];
                BonusMoney = (decimal)dataArray[1];
                DisplayName = dataArray[2] == DBNull.Value ? "" : (string)dataArray[2];
                IdCardNumber = dataArray[3] == DBNull.Value ? "" : (string)dataArray[3];
                LocalMobile = dataArray[4] == DBNull.Value ? "" : (string)dataArray[4];
                RealName = dataArray[5] == DBNull.Value ? "" : (string)dataArray[5];
                UserId = (string)dataArray[6];
                UserKey = dataArray[7] == DBNull.Value ? "" : (string)dataArray[7];
            }
            else
            {
                throw new ArgumentException("数据数组长度不满足要求，不能转换成此HistoryRankInfo对象，传入数组为"
                    + dataArray.Length + "。" + string.Join(", ", dataArray), "dataArray");
            }
        }
    }
    [CommunicationObject]
    public class HistoryRankInfoCollection
    {
        public HistoryRankInfoCollection()
        {
            DetailList = new List<HistoryRankInfo>();
        }
        public int TotalCount { get; set; }
        public List<HistoryRankInfo> DetailList { get; set; }

        public void LoadList(IList list)
        {
            foreach (object[] item in list)
            {
                var info = new HistoryRankInfo();
                info.LoadArray(item);
                DetailList.Add(info);
            }
        }
    }
}
