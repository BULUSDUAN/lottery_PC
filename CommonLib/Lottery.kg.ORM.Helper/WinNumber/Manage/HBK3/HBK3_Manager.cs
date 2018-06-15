using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class HBK3_Manager : DBbase
    {

        #region 前台查询数据

        /// <summary>
        /// 查询基本走势实体类的所有字段
        /// </summary>
        public List<HBK3_JBZS> QueryHBK3_JBZS_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<HBK3_JBZS>()
                        orderby r.IssuseNumber ascending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询和值实体类的所有字段
        /// </summary>
        public List<HBK3_HZ> QueryHBK3_HZ_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<HBK3_HZ>()
                        orderby r.IssuseNumber ascending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询形态实体类的所有字段
        /// </summary>
        public List<HBK3_XT> QueryHBK3_XT_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<HBK3_XT>()
                        orderby r.IssuseNumber ascending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询组合实体类的所有字段
        /// </summary>
        public List<HBK3_ZH> QueryHBK3_ZH_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<HBK3_ZH>()
                        orderby r.IssuseNumber ascending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询综合和走势实体类的所有字段
        /// </summary>
        public List<HBK3_ZHZS> QueryHBK3_ZHZS_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<HBK3_ZHZS>()
                        orderby r.IssuseNumber ascending
                        select r;
            return query.Take(length).ToList();
        }

        #endregion

        #region 初始化数据

        /// <summary>
        /// 添加基本走势数据
        /// </summary>
        public void AddHBK3_JBZS(HBK3_JBZS entity)
        {
            LottertDataDB.GetDal<HBK3_JBZS>().Add(entity);
        }

        /// <summary>
        ///  查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HBK3_JBZS QueryHBK3_JBZS()
        {
             
            return LottertDataDB.CreateQuery<HBK3_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询HBK3_JBZS本期是否生成
        /// </summary>
        public int QueryHBK3_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HBK3_JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加和值走势数据
        /// </summary>
        public void AddHBK3_HZ(HBK3_HZ entity)
        {
            LottertDataDB.GetDal<HBK3_HZ>().Add(entity);
        }

        /// <summary>
        ///  查询和值走势（遗漏）最新一条数据
        /// </summary>
        public HBK3_HZ QueryHBK3_HZ()
        {
             
            return LottertDataDB.CreateQuery<HBK3_HZ>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询HBK3_HZ本期是否生成
        /// </summary>
        public int QueryHBK3_HZIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HBK3_HZ>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加形态走势数据
        /// </summary>
        public void AddHBK3_XT(HBK3_XT entity)
        {
            LottertDataDB.GetDal<HBK3_XT>().Add(entity);
        }

        /// <summary>
        ///  查询形态走势（遗漏）最新一条数据
        /// </summary>
        public HBK3_XT QueryHBK3_XT()
        {
             
            return LottertDataDB.CreateQuery<HBK3_XT>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询HBK3_XT本期是否生成
        /// </summary>
        public int QueryHBK3_XTIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HBK3_XT>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加组合走势数据
        /// </summary>
        public void AddHBK3_ZH(HBK3_ZH entity)
        {
            LottertDataDB.GetDal<HBK3_ZH>().Add(entity);
        }

        /// <summary>
        ///  查询组合走势（遗漏）最新一条数据
        /// </summary>
        public HBK3_ZH QueryHBK3_ZH()
        {
             
            return LottertDataDB.CreateQuery<HBK3_ZH>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询HBK3_ZH本期是否生成
        /// </summary>
        public int QueryHBK3_ZHIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HBK3_ZH>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加综合走势数据
        /// </summary>
        public void AddHBK3_ZHZS(HBK3_ZHZS entity)
        {
            LottertDataDB.GetDal<HBK3_ZHZS>().Add(entity);
        }

        /// <summary>
        ///  查询综合走势（遗漏）最新一条数据
        /// </summary>
        public HBK3_ZHZS QueryHBK3_ZHZS()
        {
             
            return LottertDataDB.CreateQuery<HBK3_ZHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询HBK3_ZHZS本期是否生成
        /// </summary>
        public int QueryHBK3_ZHZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HBK3_ZHZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

        #endregion

    }
}
