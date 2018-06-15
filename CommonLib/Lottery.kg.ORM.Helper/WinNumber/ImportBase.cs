using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber
{
    public class ImportBase
    {
        public virtual long Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public virtual string IssuseNumber { get; set; }
        /// <summary>
        /// 开奖号
        /// </summary>
        public virtual string WinNumber { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    public class GameWinNumberBase
    {
        public virtual long Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public virtual string IssuseNumber { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 开奖号
        /// </summary>
        public virtual string WinNumber { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 用户表
    /// </summary>
    public class UserLogin
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public virtual string UserName { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public virtual string DisplayName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public virtual string PassWord { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
