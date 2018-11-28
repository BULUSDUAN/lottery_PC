using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using GameBiz.Domain.Entities;
using GameBiz.Core;
using Common.Utilities;
using System.Data;

namespace GameBiz.Business.Domain.Managers
{
   public class TotalSingleTreasureManager : GameBizEntityManagement
    {
       public void AddTotalSingleTreasure(TotalSingleTreasure entity)
       {
           this.Add<TotalSingleTreasure>(entity);
       }
       public void UpdateTotalSingleTreasure(TotalSingleTreasure entity)
       {
           this.Update<TotalSingleTreasure>(entity);
       }
       public TotalSingleTreasure QueryTotalSingleTreasureBySchemeId(string schemeId)
       {
           Session.Clear();
           return Session.Query<TotalSingleTreasure>().FirstOrDefault(s => s.SchemeId == schemeId);
       }

       public BDFXRecordSingleCopy QueryBDFXRecordSingleCopyBySchemeId(string singleCopyId)
       {
           Session.Clear();
           return Session.Query<BDFXRecordSingleCopy>().FirstOrDefault(s => s.SingleCopySchemeId == singleCopyId);
       }
       public void AddBDFXRecordSingleCopy(BDFXRecordSingleCopy entity)
       {
           this.Add<BDFXRecordSingleCopy>(entity);
       }
    }
}
