using KaSon.FrameWork.ORM.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.IBuilder
{
    internal interface IVisitorBuilder
    {
        AnyLinqVisitor Any();
        AverageLinqVisitor Average();
        ContainsLinqVisitor Contains();
        CountLinqVisitor Count();
        DistinctLinqVisitor Distinct();
        FirstLinqVisitor First();
        GroupByLinqVisitor GroupBy();
        GroupByKeyVistor GroupByKey();
       // HasCountLinqVisitor HasCount();
        JoinLinqVisitor Join();
        MaxLinqVisitor Max();
        MinLinqVisitor Min();
        OrderByLinqVisitor OrderBy();
     //   PageLinqVisitor Page();
        SelectLinqVisitor Select();

      

        SelectManyLinqVisitor SelectMany();
        SetJoinModeLinqVisitor SetJoinMode();
        SumLinqVisitor Sum();
        TakeLinqVisitor Take();
        UnionLinqVisitor Union();
        WhereLinqVisitor Where();


        PageLinqVisitor Page();

        /// <summary>
        /// 跳过前几条
        /// </summary>
        /// <returns></returns>
        SkipLinqVisitor Skip();
    }
}
