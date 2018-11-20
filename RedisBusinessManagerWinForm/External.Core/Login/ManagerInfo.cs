using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using Common;
using System.Collections;

namespace External.Core.Login
{
    /// <summary>
    /// 后台管理人员查询信息
    /// </summary>
    [CommunicationObject]
    [Serializable]
    public class ManagerQueryInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 可用状态
        /// </summary>
        public EnableStatus Status { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserKey { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 角色编号列表
        /// </summary>
        public string RoleIdList { get; set; }
        /// <summary>
        /// 所属角色
        /// </summary>
        public string RoleDisplayName { get; set; }

        public void LoadArray(object[] dataArray)
        {
            if (dataArray.Length == 6)
            {
                UserId = (string)dataArray[0];
                LoginName = (string)dataArray[1];
                Status = (EnableStatus)dataArray[2];
                UserKey = dataArray[3] == DBNull.Value ? null : (string)dataArray[3];
                DisplayName = dataArray[4] == DBNull.Value ? null : (string)dataArray[4];
                RoleDisplayName = dataArray[5] == DBNull.Value ? null : (string)dataArray[5];
            }
            else
            {
                throw new ArgumentException("数据数组长度不满足要求，不能转换成此ManagerQueryInfo对象，传入数组为" + dataArray.Length + "。" + string.Join(", ", dataArray), "dataArray");
            }
        }
    }
    /// <summary>
    /// 后台管理人员查询列表
    /// </summary>
    [CommunicationObject]
    public class ManagerQueryInfoCollection
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ManagerQueryInfoCollection()
        {
            ManagerList = new List<ManagerQueryInfo>();
        }
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 用户列表
        /// </summary>
        public IList<ManagerQueryInfo> ManagerList { get; set; }
        /// <summary>
        /// 加载存储过程返回数组列表
        /// </summary>
        /// <param name="list">数组列表</param>
        public void LoadList(IList list)
        {
            foreach (object[] item in list)
            {
                var info = new ManagerQueryInfo();
                info.LoadArray(item);
                ManagerList.Add(info);
            }
        }
    }
}
