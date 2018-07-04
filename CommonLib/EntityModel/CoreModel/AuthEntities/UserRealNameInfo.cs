using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    /// <summary>
    /// 用户实名信息
    /// </summary>
    [Serializable]
    public class UserRealNameInfo
    {
        /// <summary>
        /// 认证来源
        /// </summary>
        public string AuthFrom { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCardNumber { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }

        public bool IsSettedRealName { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateBy { get; set; }
    }
    public class UserRealName_Collection
    {
        public UserRealName_Collection()
        {
            RealNameList = new List<UserRealNameInfo>();
        }
        public int TotalCount { get; set; }
        public List<UserRealNameInfo> RealNameList { get; set; }
    }
}
