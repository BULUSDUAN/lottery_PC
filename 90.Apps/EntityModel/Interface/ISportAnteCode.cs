using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Interface
{
    public interface ISportAnteCode
    {
        string GameType { get; }
        string MatchId { get; }
        string AnteCode { get; }
        string Odds { get; }
        int Length { get; }
        bool IsDan { get; }
        string GetMatchResult(string gameCode, string gameType, string score);
    }
}
