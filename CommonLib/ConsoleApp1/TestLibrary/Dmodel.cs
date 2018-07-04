using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestLibrary
{
    [Entity("Table_1", Type = EntityType.Table)]
  public  class Dmodel
    {
        [Field("id")]
        public int id { get; set; }
        [Field("name")]
        public string name { get; set; }
    }
}
