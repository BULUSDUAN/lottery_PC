using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Model
{
    public class GD11X5_XTZS : ImportBase
    {
        public virtual int DX_Q_D { get; set; }
        public virtual int DX_1D4X { get; set; }
        public virtual int DX_2D3X { get; set; }
        public virtual int DX_3D2X { get; set; }
        public virtual int DX_4D1X { get; set; }
        public virtual int DX_Q_X { get; set; }

        public virtual int JO_Q_J { get; set; }
        public virtual int JO_1J4O { get; set; }
        public virtual int JO_2J3O { get; set; }
        public virtual int JO_3J2O { get; set; }
        public virtual int JO_4J1O { get; set; }
        public virtual int JO_Q_O { get; set; }

        public virtual int ZH_Q_Z { get; set; }
        public virtual int ZH_1Z4H { get; set; }
        public virtual int ZH_2Z3H { get; set; }
        public virtual int ZH_3Z2H { get; set; }
        public virtual int ZH_4Z1H { get; set; }
        public virtual int ZH_Q_H { get; set; }
    }
}
