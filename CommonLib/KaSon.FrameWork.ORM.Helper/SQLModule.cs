using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
  public  class SQLModule
    {

        /// <summary>
        /// 用户系统模块  
        /// </summary>
      public IList<SQLModel> UserSystemModule { get; set; }
        /// <summary>
        /// 投注系统
        /// </summary>
        public IList<SQLModel> BettiongModule { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IList<SQLModel> DataModule { get; set; }

        public IList<SQLModel> AdminModule { get; set; }
    }
}
