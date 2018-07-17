using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Domain.Entities;
using System.Data;
using KaSon.FrameWork.ORM.Helper;
using EntityModel;

namespace GameBiz.Business.Domain.Managers
{
   public class TotalSingleTreasureManager : DBbase
    {
       public void AddTotalSingleTreasure(C_TotalSingleTreasure entity)
       {
           DB.GetDal<C_TotalSingleTreasure>().Add(entity);
       }
       public void UpdateTotalSingleTreasure(C_TotalSingleTreasure entity)
       {
            DB.GetDal<C_TotalSingleTreasure>().Update(entity);
       }
       public C_TotalSingleTreasure QueryTotalSingleTreasureBySchemeId(string schemeId)
       {
           return DB.CreateQuery<C_TotalSingleTreasure>().Where(s => s.SchemeId == schemeId).FirstOrDefault();
       }

       public C_BDFX_RecordSingleCopy QueryBDFXRecordSingleCopyBySchemeId(string singleCopyId)
       {
           return DB.CreateQuery<C_BDFX_RecordSingleCopy>().Where(s => s.SingleCopySchemeId == singleCopyId).FirstOrDefault();
       }
       public void AddBDFXRecordSingleCopy(C_BDFX_RecordSingleCopy entity)
       {
           DB.GetDal<C_BDFX_RecordSingleCopy>().Add(entity);
       }
    }
}
