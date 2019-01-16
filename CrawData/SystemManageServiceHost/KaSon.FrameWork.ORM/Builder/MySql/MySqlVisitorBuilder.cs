using KaSon.FrameWork.ORM.IBuilder;
using KaSon.FrameWork.ORM.Visitor;
using KaSon.FrameWork.ORM.Visitor.MySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KaSon.FrameWork.ORM.Builder
{
    internal class MySqlVisitorBuilder : IVisitorBuilder 
    {



        public AnyLinqVisitor Any()
        {
            return new AnyVisitor();
        }

        public AverageLinqVisitor Average()
        {
            return new AverageVisitor();
        }

        public ContainsLinqVisitor Contains()
        {
            return new ContainsVisitor();
        }

        public CountLinqVisitor Count()
        {
            return new CountVisitor();
        }

        public DistinctLinqVisitor Distinct()
        {
            return new KaSon.FrameWork.ORM.Visitor.DistinctVisitor();
        }

        public FirstLinqVisitor First()
        {
            return new FirstVisitor();
        }

        public GroupByLinqVisitor GroupBy()
        {
            return new GroupByVisitor();
        }

        public GroupByKeyVistor GroupByKey()
        {
            return new GroupByKeyVistor();
        }

        //public HasCountLinqVisitor HasCount()
        //{
        //    return new HasCountLinqVisitor();
        //}

        public JoinLinqVisitor Join()
        {
            return new JoinVisitor();
        }

        public MaxLinqVisitor Max()
        {
            return new MaxVisitor();
        }

        public MinLinqVisitor Min()
        {
            return new MinVisitor();
        }

        public OrderByLinqVisitor OrderBy()
        {
            return new OrderByVisitor();
        }

        //public PageLinqVisitor Page()
        //{
        //    return new PageVisitor();
        //}

        public SelectLinqVisitor Select()
        {
            return new SelectVisitor();
        }

        public SelectManyLinqVisitor SelectMany()
        {
            return new SelectManyVisitor();
        }

        public SetJoinModeLinqVisitor SetJoinMode()
        {
            return new SetJoinModeVisitor();
        }

        public SumLinqVisitor Sum()
        {
            return new SumVisitor();
        }

        public TakeLinqVisitor Take()
        {
            return new TakeVisitor();
        }

        public UnionLinqVisitor Union()
        {
            return new UnionVisitor();
        }

        public WhereLinqVisitor Where()
        {
            return new WhereVisitor();
        }


        public PageLinqVisitor Page()
        {
            return new PageVisitor();
        }


        public SkipLinqVisitor Skip()
        {
            return new SkipVisitor();
        }
    }
}
