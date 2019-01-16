using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 跨度
    /// </summary>
    public class CQSSC_3X_KD : ImportBase
    {
        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }
        public virtual string RedBall3 { get; set; }

        public virtual int KuaDu { set; get; }

        public virtual int KD_0 { get; set; }
        public virtual int KD_1 { get; set; }
        public virtual int KD_2 { get; set; }
        public virtual int KD_3 { get; set; }
        public virtual int KD_4 { get; set; }
        public virtual int KD_5 { get; set; }
        public virtual int KD_6 { get; set; }
        public virtual int KD_7 { get; set; }
        public virtual int KD_8 { get; set; }
        public virtual int KD_9 { get; set; }
    }
}
