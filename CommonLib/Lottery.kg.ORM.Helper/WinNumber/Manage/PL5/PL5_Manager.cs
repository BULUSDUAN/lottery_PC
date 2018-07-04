using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class PL5_Manager : DBbase
    {

        #region 前台查询数据

        /// <summary>
        /// 查询基本走势实体类的所有字段
        /// </summary>
        public List<PL5_JBZS> QueryPL5_JBZS(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL5_JBZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询大小走势实体类的所有字段
        /// </summary>
        public List<PL5_DX> QueryPL5_DX(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL5_DX>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询奇偶走势实体类的所有字段
        /// </summary>
        public List<PL5_JO> QueryPL5_JO(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL5_JO>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询质和走势实体类的所有字段
        /// </summary>
        public List<PL5_ZH> QueryPL5_ZH(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL5_ZH>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询除3走势实体类的所有字段
        /// </summary>
        public List<PL5_Chu3> QueryPL5_Chu3(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL5_Chu3>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询和值走势实体类的所有字段
        /// </summary>
        public List<PL5_HZ> QueryPL5_HZ(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL5_HZ>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }
        #endregion

        #region 初始化数据

        /// <summary>
        /// 添加排列5基本走势数据
        /// </summary>
        public void AddPL5_JBZS(PL5_JBZS entity)
        {
            LottertDataDB.GetDal<PL5_JBZS>().Add(entity);
        }

        /// <summary>
        ///  查询基本走势（遗漏）最新一条数据
        /// </summary>
        public PL5_JBZS QueryPL5_JBZS()
        {
             
            return LottertDataDB.CreateQuery<PL5_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL5_JBZS本期是否生成
        /// </summary>
        public int QueryPL5_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL5_JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列5大小走势数据
        /// </summary>
        public void AddPL5_DX(PL5_DX entity)
        {
            LottertDataDB.GetDal<PL5_DX>().Add(entity);
        }

        /// <summary>
        ///  查询大小走势（遗漏）最新一条数据
        /// </summary>
        public PL5_DX QueryPL5_DX()
        {
             
            return LottertDataDB.CreateQuery<PL5_DX>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL5_DX本期是否生成
        /// </summary>
        public int QueryPL5_DXIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL5_DX>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列5奇偶走势数据
        /// </summary>
        public void AddPL5_JO(PL5_JO entity)
        {
            LottertDataDB.GetDal<PL5_JO>().Add(entity);
        }

        /// <summary>
        ///  查询奇偶走势（遗漏）最新一条数据
        /// </summary>
        public PL5_JO QueryPL5_JO()
        {
             
            return LottertDataDB.CreateQuery<PL5_JO>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL5_JO本期是否生成
        /// </summary>
        public int QueryPL5_JOIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL5_JO>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列5质和走势数据
        /// </summary>
        public void AddPL5_ZH(PL5_ZH entity)
        {
            LottertDataDB.GetDal<PL5_ZH>().Add(entity);
        }

        /// <summary>
        ///  查询质和走势（遗漏）最新一条数据
        /// </summary>
        public PL5_ZH QueryPL5_ZH()
        {
             
            return LottertDataDB.CreateQuery<PL5_ZH>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL5_ZH本期是否生成
        /// </summary>
        public int QueryPL5_ZHIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL5_ZH>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列5除3走势数据
        /// </summary>
        public void AddPL5_Chu3(PL5_Chu3 entity)
        {
            LottertDataDB.GetDal<PL5_Chu3>().Add(entity);
        }

        /// <summary>
        ///  查询除3走势（遗漏）最新一条数据
        /// </summary>
        public PL5_Chu3 QueryPL5_Chu3()
        {
             
            return LottertDataDB.CreateQuery<PL5_Chu3>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL5_Chu3本期是否生成
        /// </summary>
        public int QueryPL5_Chu3IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL5_Chu3>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列5和值走势数据
        /// </summary>
        public void AddPL5_HZ(PL5_HZ entity)
        {
            LottertDataDB.GetDal<PL5_HZ>().Add(entity);
        }

        /// <summary>
        ///  查询和值势（遗漏）最新一条数据
        /// </summary>
        public PL5_HZ QueryPL5_HZ()
        {
             
            return LottertDataDB.CreateQuery<PL5_HZ>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL5_HZ本期是否生成
        /// </summary>
        public int QueryPL5_HZIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL5_HZ>().Count(p => p.IssuseNumber == issuseNumber);
        }


        #endregion

    }
}
