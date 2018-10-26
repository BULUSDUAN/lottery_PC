using EntityModel;
using System;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{
    public class BettingPointManager : DBbase
    {
        public E_Authentication_Email GetUserEmail(string userId)
        {
            return DB.CreateQuery<E_Authentication_Email>().Where(x=>x.UserId==userId).FirstOrDefault();
        }
    }
}
