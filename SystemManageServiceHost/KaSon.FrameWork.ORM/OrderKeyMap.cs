using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
  internal  class OrderKeyMap
    {

      internal bool isAsc { get; set; }
      internal string Key { get; set; }

      internal string ModelAlis { get; set; }

      internal Type ModelType { get; set; }

    }
}
