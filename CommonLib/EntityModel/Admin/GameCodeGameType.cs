using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel
{
    public class GameList
    {
        public List<GameCodeinfo> _gameCodeInfo { get; set; }
        public List<GameTypeinfo> _gameTypeInfo { get; set; }

    }
   public class GameCodeinfo
    {
        public string GameCode { get; set; }
        public string GameName { get; set; }
    }
    public class GameTypeinfo
    {
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string GameTypeName { get; set; }
    }
}
