using System;
using System.Collections.Generic;
using System.Linq;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper
{
    public class MobileValidationManager : DBbase
    {
        public IList<E_Validation_Mobile> QueryMobileValidationList(string mobile)
        {
            return DB.CreateQuery<E_Validation_Mobile>().Where(x => x.Mobile == mobile).ToList();
        }
    }
}
