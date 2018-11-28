using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using System.Collections;

namespace External.Core.Login
{
    [CommunicationObject]
    [Serializable]
    public class AlipayLoginInfo : RegisterInfo_Alipay
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserKey { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        public DateTime CreateTime { get; set; }
        //public string ApliyCount { get; set; }

        internal void LoadArray(object[] dataArray)
        {
            if (dataArray.Length == 10)
            {
                UserId = (string)dataArray[0];
                this.LoginName = (string)dataArray[1];
                this.LoginAccount = dataArray[2] == DBNull.Value ? null : (string)dataArray[2];
                this.CreateTime = (DateTime)dataArray[3];
                this.RealName = dataArray[4] == DBNull.Value ? null : (string)dataArray[4];
                this.Mobile = dataArray[5] == DBNull.Value ? null : (string)dataArray[5];
                this.CardNumber = dataArray[6] == DBNull.Value ? null : (string)dataArray[6];
                UserKey = (string)dataArray[7];
                DisplayName = dataArray[8] == DBNull.Value ? null : (string)dataArray[8];
                this.RegisterIp = dataArray[9] == DBNull.Value ? null : (string)dataArray[9];
                //ApliyCount = dataArray[10].ToString();
            }
        }
    }

    [CommunicationObject]
    public class AlipayLoginInfoCollection
    {
        public AlipayLoginInfoCollection()
        {
            AlipayLoginList = new List<AlipayLoginInfo>();
        }

        public int TotalCount { get; set; }

        public IList<AlipayLoginInfo> AlipayLoginList { get; set; }

        public void LoadList(IList list)
        {
            foreach (object[] item in list)
            {
                var info = new AlipayLoginInfo();
                info.LoadArray(item);
                AlipayLoginList.Add(info);
            }
        }
    }
}
