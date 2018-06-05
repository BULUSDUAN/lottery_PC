using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class GameWinNumber_InfoCollection:Page
    {
        public GameWinNumber_InfoCollection(){ }

        public List<GameWinNumber_Info> List { get; set; }
        public int TotalCount { get; set; }
    }
    public class GameWinNumber_Info
    {
        public GameWinNumber_Info(){ }

        public long Id { get; set; }
        public string IssuseNumber { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string WinNumber { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
