using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.Common
{
    public interface IKgLog
    {
          void Log(string name,Exception ex);
         void Log(string ex);
    }
}
