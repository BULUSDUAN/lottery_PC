using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    /// <summary>
    /// 基本走势
    /// </summary>
    public class PL3_DX : ImportBase
    {
        /// <summary>
        /// 大小形态
        /// </summary>
        public virtual string DXType { get; set; }
        /// <summary>
        /// 大大大
        /// </summary>
        public virtual int DX_DDD { get; set; }
        public virtual int DX_DDX { get; set; }
        public virtual int DX_DXD { get; set; }
        public virtual int DX_XDD { get; set; }
        public virtual int DX_DXX { get; set; }
        public virtual int DX_XDX { get; set; }
        public virtual int DX_XXD { get; set; }
        public virtual int DX_XXX { get; set; }
        /// <summary>
        /// 大小比
        /// </summary>
        public virtual string DaoXiaoBi { get; set; }
        /// <summary>
        /// 大小比
        /// </summary>
        public virtual int Bi3_0 { get; set; }
        public virtual int Bi2_1 { get; set; }
        public virtual int Bi1_2 { get; set; }
        public virtual int Bi0_3 { get; set; }
    }
}
