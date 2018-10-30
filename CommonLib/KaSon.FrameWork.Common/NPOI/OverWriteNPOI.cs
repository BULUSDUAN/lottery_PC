using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KaSon.FrameWork.Common.NPOI
{
   public class OverWriteNPOI: MemoryStream
    {
            public OverWriteNPOI()
            {
                AllowClose = true;
            }

            public bool AllowClose { get; set; }

            public override void Close()
            {
                if (AllowClose)
                    base.Close();
            }
        }
}
