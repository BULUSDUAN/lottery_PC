using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 除3 1走势
    /// </summary>
    public class PL3_Chu32 : ImportBase
    {
        public virtual string RedBall1 { get; set; }
        public virtual string RedBall2 { get; set; }
        public virtual string RedBall3 { get; set; }

        public virtual int C_AAA_000 { get; set; }
        public virtual int C_AAA_111 { get; set; }
        public virtual int C_AAA_222 { get; set; }

        public virtual int C_AAB_001 { get; set; }
        public virtual int C_AAB_010 { get; set; }
        public virtual int C_AAB_100 { get; set; }
        public virtual int C_AAB_110 { get; set; }
        public virtual int C_AAB_101 { get; set; }
        public virtual int C_AAB_011 { get; set; }
        public virtual int C_AAB_002 { get; set; }
        public virtual int C_AAB_020 { get; set; }
        public virtual int C_AAB_200 { get; set; }
        public virtual int C_AAB_220 { get; set; }
        public virtual int C_AAB_202 { get; set; }
        public virtual int C_AAB_022 { get; set; }
        public virtual int C_AAB_112 { get; set; }
        public virtual int C_AAB_121 { get; set; }
        public virtual int C_AAB_211 { get; set; }
        public virtual int C_AAB_221 { get; set; }
        public virtual int C_AAB_212 { get; set; }
        public virtual int C_AAB_122 { get; set; }

        public virtual int C_ABC_012 { get; set; }
        public virtual int C_ABC_120 { get; set; }
        public virtual int C_ABC_201 { get; set; }
        public virtual int C_ABC_102 { get; set; }
        public virtual int C_ABC_021 { get; set; }
        public virtual int C_ABC_210 { get; set; }
    }
}
