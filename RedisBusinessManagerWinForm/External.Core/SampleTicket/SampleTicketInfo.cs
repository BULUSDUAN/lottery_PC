using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using External.Core.Login;
using GameBiz.Core;

namespace External.Core.SampleTicket
{
    /// <summary>
    /// 票样
    /// </summary>
    [CommunicationObject]
    public class SampleTicketInfo
    {
        /// <summary>
        /// 票样编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 订单编码
        /// </summary>
        public string OrderFormCode { get; set; }
        /// <summary>
        /// 申请人编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 彩种类型
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 彩种玩法
        /// </summary>
        public string PlayType { get; set; }
        /// <summary>
        /// 方案编码
        /// </summary>
        public string SchemeCode { get; set; }
        /// <summary>
        /// 快递地址
        /// </summary>
        public string ExpressAddress { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 操作用户
        /// </summary>
        public string OperationUser { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public DealWithType DealWithType { get; set; }
        /// <summary>
        /// 申请状态，0：申请中,1：派送中，2：已派送，3：已拒绝
        /// </summary>
        public ApplyState ApplyState { get; set; }
        /// <summary>
        /// 快递单号
        /// </summary>
        public string ExpressNumber { get; set; }
        /// <summary>
        /// 快递费用
        /// </summary>
        public decimal DeliveryCosts { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        private T GetDbValue<T>(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return default(T);
            }
            return (T)value;
        }
        public void LoadArray(object[] dataArray)
        {
            if (dataArray.Length == 14)
            {
                UserId = GetDbValue<string>(dataArray[0]);
                Id = GetDbValue<int>(dataArray[1]);
                OrderFormCode = GetDbValue<string>(dataArray[2]);
                CreateTime = GetDbValue<DateTime>(dataArray[3]);
                GameType = GetDbValue<string>(dataArray[4]);
                PlayType = GetDbValue<string>(dataArray[5]);
                SchemeCode = GetDbValue<string>(dataArray[6]);
                DealWithType = GetDbValue<DealWithType>(dataArray[7]);
                Description = GetDbValue<string>(dataArray[8]);
                OperationUser = GetDbValue<string>(dataArray[9]);
                ExpressAddress = GetDbValue<string>(dataArray[10]);
                ApplyState = GetDbValue<ApplyState>(dataArray[11]);
                ExpressNumber = GetDbValue<string>(dataArray[12]);
                DeliveryCosts = GetDbValue<decimal>(dataArray[13]);
            }
            else
            {
                throw new ArgumentException("数据数组长度不满足要求，不能转换成此SampleTicketInfo对象，传入数组为" + dataArray.Length + "。" + string.Join(", ", dataArray), "dataArray");
            }
        }
    }

    [CommunicationObject]
    public class SampleTicketInfoCollection
    {
        public SampleTicketInfoCollection()
        {
            List = new List<SampleTicketInfo>();
        }
        public int TotalCount { get; set; }
        public List<SampleTicketInfo> List { get; set; }
        /// <summary>
        /// 返回数组列表
        /// </summary>
        /// <param name="list">数组列表</param>
        public void LoadList(IList list)
        {
            foreach (object[] item in list)
            {
                var info = new SampleTicketInfo();
                info.LoadArray(item);
                List.Add(info);
            }
        }
    }
}
