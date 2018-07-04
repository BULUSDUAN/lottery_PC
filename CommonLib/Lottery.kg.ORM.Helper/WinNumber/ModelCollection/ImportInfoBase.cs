using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection
{
    public class ImportInfoBase
    {
        public long Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 开奖号
        /// </summary>
        public string WinNumber { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }    

    public class UserLogin_Info
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }


}
