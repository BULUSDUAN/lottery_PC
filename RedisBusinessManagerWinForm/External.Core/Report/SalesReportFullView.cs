using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using System.Collections;

namespace External.Core.Report
{
    [CommunicationObject]
    public class SalesReportFullViewItem
    {
        public string DateOrTime { get; set; }
        public string DateType { get; set; }
        public int TotalOrderCount { get; set; }
        public decimal TotalOrderMoney { get; set; }
        public int TotalPayCount { get; set; }
        public decimal TotalPayMoney { get; set; }

        internal void LoadArray(object[] dataArray)
        {
            if (dataArray.Length != 6)
            {
                throw new ArgumentException("转换成此SalesReportFullViewItem对象的数据数组长度必须是6，传入数组为" + dataArray.Length + "。" + string.Join(", ", dataArray), "dataArray");
            }
            DateOrTime = (string)dataArray[0];
            DateType = (string)dataArray[1];
            TotalOrderCount = (int)dataArray[2];
            TotalOrderMoney = (decimal)dataArray[3];
            TotalPayCount = (int)dataArray[4];
            TotalPayMoney = (decimal)dataArray[5];
        }
    }
    [CommunicationObject]
    public class SalesReportFullView : List<SalesReportFullViewItem>
    {
        /// <summary>
        /// 加载存储过程返回数组列表
        /// </summary>
        /// <param name="list">数组列表</param>
        public void LoadList(IList list)
        {
            foreach (object[] item in list)
            {
                var info = new SalesReportFullViewItem();
                info.LoadArray(item);
                Add(info);
            }
        }
    }

    [CommunicationObject]
    public class ConditionSummaryItem
    {
        public int FillMoneyCount { get; set; }
        public decimal FillMoney { get; set; }
        public int FirstFillMoneyCount { get; set; }
        public decimal FirstFillMoney { get; set; }
        public int UserCount { get; set; }
        public int OrderCount { get; set; }
        public decimal OrderMoney { get; set; }
        public decimal BonusMoney { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public void LoadArray(IList dataArray)
        {
            if (dataArray.Count != 10)
            {
                throw new ArgumentException("转换成此ConditionSummaryItem对象的数据数组长度必须是10，传入数组为" + dataArray.Count + "。" + string.Join(", ", dataArray), "dataArray");
            }
            FillMoneyCount = (int)dataArray[0];
            FillMoney = (decimal)dataArray[1];
            FirstFillMoneyCount = (int)dataArray[2];
            FirstFillMoney = (decimal)dataArray[3];
            UserCount = (int)dataArray[4];
            OrderCount = (int)dataArray[5];
            OrderMoney = (decimal)dataArray[6];
            BonusMoney = (decimal)dataArray[7];
            StartTime = (string)dataArray[8];
            EndTime = (string)dataArray[9];
        }
    }

}
