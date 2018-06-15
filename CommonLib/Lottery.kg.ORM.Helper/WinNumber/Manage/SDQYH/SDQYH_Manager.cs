using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class SDQYH_Manager : DBbase
    {

        #region 前台查询数据

        /// <summary>
        /// 查询任选奇偶实体类的所有字段
        /// </summary>
        public List<SDQYH_RXJO> QuerySDQYH_RXJO_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<SDQYH_RXJO>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选质和实体类的所有字段
        /// </summary>
        public List<SDQYH_RXZH> QuerySDQYH_RXZH_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<SDQYH_RXZH>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选大小实体类的所有字段
        /// </summary>
        public List<SDQYH_RXDX> QuerySDQYH_RXDX_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<SDQYH_RXDX>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选除3实体类的所有字段
        /// </summary>
        public List<SDQYH_Chu3> QuerySDQYH_Chu3_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<SDQYH_Chu3>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询顺选一走势实体类的所有字段
        /// </summary>
        public List<SDQYH_SX1> QuerySDQYH_SX1_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<SDQYH_SX1>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询顺选二走势实体类的所有字段
        /// </summary>
        public List<SDQYH_SX2> QuerySDQYH_SX2_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<SDQYH_SX2>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询顺选三走势实体类的所有字段
        /// </summary>
        public List<SDQYH_SX3> QuerySDQYH_SX3_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<SDQYH_SX3>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }


        #endregion

        #region 初始化数据

        /// <summary>
        /// 添加任选奇偶数据
        /// </summary>
        public void AddSDQYH_RXJO(SDQYH_RXJO entity)
        {
            LottertDataDB.GetDal<SDQYH_RXJO>().Add(entity);
        }

        /// <summary>
        /// 查询任选奇偶（遗漏）最新一条数据
        /// </summary>
        public SDQYH_RXJO QuerySDQYH_RXJO()
        {
             
            return LottertDataDB.CreateQuery<SDQYH_RXJO>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询SDQYH_RXJO本期是否生成
        /// </summary>
        public int QuerySDQYH_RXJOIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDQYH_RXJO>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加任选质和数据
        /// </summary>
        public void AddSDQYH_RXZH(SDQYH_RXZH entity)
        {
            LottertDataDB.GetDal<SDQYH_RXZH>().Add(entity);
        }

        /// <summary>
        /// 查询任选质和最新一条数据
        /// </summary>
        public SDQYH_RXZH QuerySDQYH_RXZH()
        {
             
            return LottertDataDB.CreateQuery<SDQYH_RXZH>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询SDQYH_RXZH本期是否生成
        /// </summary>
        public int QuerySDQYH_RXZHIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDQYH_RXZH>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加任选大小数据
        /// </summary>
        public void AddSDQYH_RXDX(SDQYH_RXDX entity)
        {
            LottertDataDB.GetDal<SDQYH_RXDX>().Add(entity);
        }

        /// <summary>
        /// 查询任选大小最新一条数据
        /// </summary>
        public SDQYH_RXDX QuerySDQYH_RXDX()
        {
             
            return LottertDataDB.CreateQuery<SDQYH_RXDX>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询SDQYH_RXDX本期是否生成
        /// </summary>
        public int QuerySDQYH_RXDXIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDQYH_RXDX>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加任选大小数据
        /// </summary>
        public void AddSDQYH_Chu3(SDQYH_Chu3 entity)
        {
            LottertDataDB.GetDal<SDQYH_Chu3>().Add(entity);
        }

        /// <summary>
        /// 查询任选大小最新一条数据
        /// </summary>
        public SDQYH_Chu3 QuerySDQYH_Chu3()
        {
             
            return LottertDataDB.CreateQuery<SDQYH_Chu3>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询SDQYH_Chu3本期是否生成
        /// </summary>
        public int QuerySDQYH_Chu3IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDQYH_Chu3>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加顺选一数据
        /// </summary>
        public void AddSDQYH_SX1(SDQYH_SX1 entity)
        {
            LottertDataDB.GetDal<SDQYH_SX1>().Add(entity);
        }

        /// <summary>
        /// 查询顺选一最新一条数据
        /// </summary>
        public SDQYH_SX1 QuerySDQYH_SX1()
        {
             
            return LottertDataDB.CreateQuery<SDQYH_SX1>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询SDQYH_SX1本期是否生成
        /// </summary>
        public int QuerySDQYH_SX1IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDQYH_SX1>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加顺选二数据
        /// </summary>
        public void AddSDQYH_SX2(SDQYH_SX2 entity)
        {
            LottertDataDB.GetDal<SDQYH_SX2>().Add(entity);
        }

        /// <summary>
        /// 查询顺选二最新一条数据
        /// </summary>
        public SDQYH_SX2 QuerySDQYH_SX2()
        {
             
            return LottertDataDB.CreateQuery<SDQYH_SX2>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询SDQYH_SX2本期是否生成
        /// </summary>
        public int QuerySDQYH_SX2IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDQYH_SX2>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加顺选三数据
        /// </summary>
        public void AddSDQYH_SX3(SDQYH_SX3 entity)
        {
            LottertDataDB.GetDal<SDQYH_SX3>().Add(entity);
        }

        /// <summary>
        /// 查询顺选三最新一条数据
        /// </summary>
        public SDQYH_SX3 QuerySDQYH_SX3()
        {
             
            return LottertDataDB.CreateQuery<SDQYH_SX3>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询SDQYH_SX3本期是否生成
        /// </summary>
        public int QuerySDQYH_SX3IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDQYH_SX3>().Count(p => p.IssuseNumber == issuseNumber);
        }

        #endregion

    }
}
