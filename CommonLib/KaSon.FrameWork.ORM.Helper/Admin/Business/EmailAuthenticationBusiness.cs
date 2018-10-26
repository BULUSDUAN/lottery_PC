using EntityModel;
using System;

namespace KaSon.FrameWork.ORM.Helper
{
    public class EmailAuthenticationBusiness
    {
        public E_Authentication_Email GetAuthenticatedEmail(string userId)
        {
            return new BettingPointManager().GetUserEmail(userId);
        }
    }
}
