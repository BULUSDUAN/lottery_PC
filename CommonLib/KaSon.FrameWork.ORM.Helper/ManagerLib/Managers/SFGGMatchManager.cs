using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    public class SFGGMatchManager : DBbase
    {
        public void AddSFGGMatch(C_SFGG_Match entity)
        {
            this.DB.GetDal<C_SFGG_Match>().Add(entity);
        }
        public void UpdateSFGGMatch(C_SFGG_Match entity)
        {
            this.DB.GetDal<C_SFGG_Match>().Update(entity);
        }
        public List<C_SFGG_Match> QuerySFGGMatchList(string issuseNumber)
        {
            return this.DB.CreateQuery<C_SFGG_Match>().Where(s => s.IssuseNumber == issuseNumber).ToList();
        }
        public C_SFGG_Match QuerySFGGMatch(string issuseNumber, int matchOrderId)
        {
            return this.DB.CreateQuery<C_SFGG_Match>().Where(s => s.IssuseNumber == issuseNumber && s.MatchOrderId == matchOrderId).FirstOrDefault();
        }
    }
}
