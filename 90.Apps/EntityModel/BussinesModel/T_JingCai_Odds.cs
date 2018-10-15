using EntityModel.Ticket;

namespace EntityModel
{
    public abstract class JingCai_Odds : I_JingCai_Odds
    {
        public abstract decimal GetOdds(string result);
        public abstract bool CheckIsValidate();
        public abstract void SetOdds(I_JingCai_Odds odds);
        public abstract bool Equals(I_JingCai_Odds odds);
        public abstract string GetOddsString();
    }
}