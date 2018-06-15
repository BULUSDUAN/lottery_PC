using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class QLC_Manager : DBbase
    {

        #region 前台查询数据

        /// <summary>
        /// 查询基本走势实体类的所有字段
        /// </summary>
        public List<QLC_JBZS> QueryQLC_JBZS(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<QLC_JBZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询大小走势实体类的所有字段
        /// </summary>
        public List<QLC_DX> QueryQLC_DX(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<QLC_DX>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询奇偶走势实体类的所有字段
        /// </summary>
        public List<QLC_JO> QueryQLC_JO(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<QLC_JO>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询质和走势实体类的所有字段
        /// </summary>
        public List<QLC_ZH> QueryQLC_ZH(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<QLC_ZH>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询除3走势实体类的所有字段
        /// </summary>
        public List<QLC_Chu3> QueryQLC_Chu3(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<QLC_Chu3>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        #endregion

        #region 初始化数据

        /// <summary>
        /// 添加基本走势数据
        /// </summary>
        public void AddQLC_JBZS(QLC_JBZS entity)
        {
            LottertDataDB.GetDal<QLC_JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势最新一条数据
        /// </summary>
        public QLC_JBZS QueryQLC_JBZS()
        {
             
            return LottertDataDB.CreateQuery<QLC_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询QLC_JBZS本期是否生成
        /// </summary>
        public int QueryQLC_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<QLC_JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加大小走势数据
        /// </summary>
        public void AddQLC_DX(QLC_DX entity)
        {
            LottertDataDB.GetDal<QLC_DX>().Add(entity);
        }

        /// <summary>
        /// 查询大小走势最新一条数据
        /// </summary>
        public QLC_DX QueryQLC_DX()
        {
             
            return LottertDataDB.CreateQuery<QLC_DX>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询QLC_DX本期是否生成
        /// </summary>
        public int QueryQLC_DXIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<QLC_DX>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加奇偶走势数据
        /// </summary>
        public void AddQLC_JO(QLC_JO entity)
        {
            LottertDataDB.GetDal<QLC_JO>().Add(entity);
        }

        /// <summary>
        /// 查询奇偶走势最新一条数据
        /// </summary>
        public QLC_JO QueryQLC_JO()
        {
             
            return LottertDataDB.CreateQuery<QLC_JO>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询QLC_JO本期是否生成
        /// </summary>
        public int QueryQLC_JOIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<QLC_JO>().Count(p => p.IssuseNumber == issuseNumber);
        }
        /// <summary>
        /// 添加质和走势数据
        /// </summary>
        public void AddQLC_ZH(QLC_ZH entity)
        {
            LottertDataDB.GetDal<QLC_ZH>().Add(entity);
        }

        /// <summary>
        /// 添加质和走势最新一条数据
        /// </summary>
        public QLC_ZH QueryQLC_ZH()
        {
             
            return LottertDataDB.CreateQuery<QLC_ZH>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询QLC_ZH本期是否生成
        /// </summary>
        public int QueryQLC_ZHIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<QLC_ZH>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加除3走势数据
        /// </summary>
        public void AddQLC_Chu3(QLC_Chu3 entity)
        {
            LottertDataDB.GetDal<QLC_Chu3>().Add(entity);
        }

        /// <summary>
        /// 添加除3走势最新一条数据
        /// </summary>
        public QLC_Chu3 QueryQLC_Chu3()
        {
             
            return LottertDataDB.CreateQuery<QLC_Chu3>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询QLC_Chu3本期是否生成
        /// </summary>
        public int QueryQLC_Chu3IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<QLC_Chu3>().Count(p => p.IssuseNumber == issuseNumber);
        }

        #endregion
    }
}
