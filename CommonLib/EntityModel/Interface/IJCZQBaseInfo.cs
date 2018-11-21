using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Interface
{
    public interface IBallBaseInfo
    {
      //  string GameType { get; }
        string MatchId { get; }
        string MatchData { get; }
        bool Equals(object obj);
    }
    public interface IMatchData
    {
        //  string GameType { get; }
      
        string MatchData { get; }
       
    }
    public interface IBJDCBallBaseInfo
    {
        //  string GameType { get; }
        string IssuseNumber { get; }
        int MatchOrderId { get; }

    //    int MatchState { get; }

    }
}
