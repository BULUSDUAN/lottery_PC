using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModel.CoreModel
{
    /// <summary>
    /// 用户手机信息
    /// </summary>
    public class UserMobileInfo
    {
        /// <summary>
        /// 认证来源
        /// </summary>
        public string AuthFrom { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        public string UserId { get; set; }
        public bool IsSettedMobile { get; set; }
        public int RequestTimes { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        /// <summary>
        /// 是否可以换
        /// </summary>
        public bool CanBeChanged { get; set; }
        /// <summary>
        /// 剩余秒数
        /// </summary>
        public int Seconds { get; set; }
    }
    public class UserMobile_Collection
    {
        public UserMobile_Collection()
        {
            MobileList = new List<UserMobileInfo>();
        }
        public List<UserMobileInfo> MobileList { get; set; }
    }
}
