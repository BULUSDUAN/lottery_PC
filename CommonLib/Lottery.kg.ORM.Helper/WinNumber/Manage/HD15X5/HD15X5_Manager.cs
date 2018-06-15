using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class HD15X5_Manager : DBbase
    {

        #region 前台查询数据

        /// <summary>
        /// 查询基本走势实体类的所有字段
        /// </summary>
        public List<HD15X5_JBZS> QueryHD15X5_JBZS(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<HD15X5_JBZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询基本走势实体类的所有字段
        /// </summary>
        public List<HD15X5_HZ> QueryHD15X5_HZ(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<HD15X5_HZ>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询大小走势实体类的所有字段
        /// </summary>
        public List<HD15X5_DX> QueryHD15X5_DX(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<HD15X5_DX>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询奇偶走势实体类的所有字段
        /// </summary>
        public List<HD15X5_JO> QueryHD15X5_JO(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<HD15X5_JO>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询质和走势实体类的所有字段
        /// </summary>
        public List<HD15X5_ZH> QueryHD15X5_ZH(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<HD15X5_ZH>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        public List<HD15X5_CH> QueryHD15X5_CH(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HD15X5_CH>()
                        orderby s.IssuseNumber ascending
                        select s;
            return query.Take(index).ToList();
        }

        public List<HD15X5_LH> QueryHD15X5_LH(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HD15X5_LH>()
                        orderby s.IssuseNumber ascending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询HD15X5_LH本期是否生成
        /// </summary>
        public int QueryHD15X5_LHIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HD15X5_LH>().Count(p => p.IssuseNumber == issuseNumber);
        }

        #endregion

        #region 初始化数据

        /// <summary>
        /// 添加基本走势数据
        /// </summary>
        public void AddHD15X5_JBZS(HD15X5_JBZS entity)
        {
            LottertDataDB.GetDal<HD15X5_JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HD15X5_LH QueryLastHD15X5_LH()
        {
             
            return LottertDataDB.CreateQuery<HD15X5_LH>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HD15X5_CH QueryLastHD15X5_CH()
        {
             
            return LottertDataDB.CreateQuery<HD15X5_CH>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        ///  查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HD15X5_JBZS QueryHD15X5_JBZS()
        {
             
            return LottertDataDB.CreateQuery<HD15X5_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询HD15X5_JBZS本期是否生成
        /// </summary>
        public int QueryHD15X5_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HD15X5_JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加和值走势数据
        /// </summary>
        public void AddHD15X5_HZ(HD15X5_HZ entity)
        {
            LottertDataDB.GetDal<HD15X5_HZ>().Add(entity);
        }

        /// <summary>
        ///  查询和值走势（遗漏）最新一条数据
        /// </summary>
        public HD15X5_HZ QueryHD15X5_HZ()
        {
             
            return LottertDataDB.CreateQuery<HD15X5_HZ>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询HD15X5_HZ本期是否生成
        /// </summary>
        public int QueryHD15X5_HZIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HD15X5_HZ>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加大小走势数据
        /// </summary>
        public void AddHD15X5_DX(HD15X5_DX entity)
        {
            LottertDataDB.GetDal<HD15X5_DX>().Add(entity);
        }

        /// <summary>
        ///  查询大小走势（遗漏）最新一条数据
        /// </summary>
        public HD15X5_DX QueryHD15X5_DX()
        {
             
            return LottertDataDB.CreateQuery<HD15X5_DX>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询HD15X5_DX本期是否生成
        /// </summary>
        public int QueryHD15X5_DXIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HD15X5_DX>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加奇偶走势数据
        /// </summary>
        public void AddHD15X5_JO(HD15X5_JO entity)
        {
            LottertDataDB.GetDal<HD15X5_JO>().Add(entity);
        }

        /// <summary>
        ///  查询奇偶走势（遗漏）最新一条数据
        /// </summary>
        public HD15X5_JO QueryHD15X5_JO()
        {
             
            return LottertDataDB.CreateQuery<HD15X5_JO>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询HD15X5_JO本期是否生成
        /// </summary>
        public int QueryHD15X5_JOIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HD15X5_JO>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加质和走势数据
        /// </summary>
        public void AddHD15X5_ZH(HD15X5_ZH entity)
        {
            LottertDataDB.GetDal<HD15X5_ZH>().Add(entity);
        }

        /// <summary>
        ///  查询质和走势（遗漏）最新一条数据
        /// </summary>
        public HD15X5_ZH QueryHD15X5_ZH()
        {
             
            return LottertDataDB.CreateQuery<HD15X5_ZH>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询HD15X5_ZH本期是否生成
        /// </summary>
        public int QueryHD15X5_ZHIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HD15X5_ZH>().Count(p => p.IssuseNumber == issuseNumber);
        }

        public void AddHD15X5_CH(HD15X5_CH entity)
        {
            LottertDataDB.GetDal<HD15X5_CH>().Add(entity);
        }

        /// <summary>
        /// 查询HD15X5_CH本期是否生成
        /// </summary>
        public int QueryHD15X5_CHIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HD15X5_CH>().Count(p => p.IssuseNumber == issuseNumber);
        }

        public void AddHD15X5_LH(HD15X5_LH entity)
        {
            LottertDataDB.GetDal<HD15X5_LH>().Add(entity);
        }

        #endregion

    }
}
