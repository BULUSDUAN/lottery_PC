using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModel.CoreModel
{
    /// <summary>
    /// 用户邮箱信息
    /// </summary>
    public class UserEmailInfo
    {
        /// <summary>
        /// 认证来源
        /// </summary>
        public string AuthFrom { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        public string UserId { get; set; }
        public bool IsSettedEmail { get; set; }
        public int RequestTimes { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateBy { get; set; }
    }
}
