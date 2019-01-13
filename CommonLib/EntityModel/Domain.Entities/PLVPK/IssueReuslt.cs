using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    /// <summary>
    // ºÅÂë±í
    ///</summary>
  
    public class IssueReuslt
    {
        public IssueReuslt()
        {
            
        }
        public string lastactionNum { get; set; } = "";
        public string lastactionNo { get; set; } = "";

        public int id { get; set; } = 0;
        public int type { get; set; } = 0;
        public int sort { get; set; } = 0;

        public string gamecode { get; set; } = "";
        public string title { get; set; } = "";
        public string gamename { get; set; } = "";
        public string info { get; set; } = "";

        public int actionNum { get; set; } = 0;
        public ulong actionTime { get; set; } = 0;
        public ulong nowTime { get; set; } = 0;
        public int dataftime { get; set; } = 0;

        public ulong diffTime { get; set; } = 0;
        public ulong kjDiffTime { get; set; } = 0;

        public string actionNo { get; set; } = "";
        public string kjdata { get; set; } = "";
        public string actionTime1 { get; set; } = "";
        public string actionTime2 { get; set; } = "";

        public int enable { get; set; }

    }
}