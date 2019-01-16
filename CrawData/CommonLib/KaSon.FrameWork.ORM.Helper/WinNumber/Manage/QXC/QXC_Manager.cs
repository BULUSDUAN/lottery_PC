using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class QXC_Manager : DBbase
    {

        #region 前台查询数据

        /// <summary>
        /// 查询基本走势实体类的所有字段
        /// </summary>
        public List<QXC_JBZS> QueryQXC_JBZS(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<QXC_JBZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询大小走势实体类的所有字段
        /// </summary>
        public List<QXC_DX> QueryQXC_DX(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<QXC_DX>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询奇偶 走势实体类的所有字段
        /// </summary>
        public List<QXC_JO> QueryQXC_JO(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<QXC_JO>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询质和走势实体类的所有字段
        /// </summary>
        public List<QXC_ZH> QueryQXC_ZH(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<QXC_ZH>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询除3 走势实体类的所有字段
        /// </summary>
        public List<QXC_Chu3> QueryQXC_Chu3(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<QXC_Chu3>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }
        #endregion

        #region 初始化数据

        /// <summary>
        /// 添加基本走势数据
        /// </summary>
        public void AddQXC_JBZS(QXC_JBZS entity)
        {
            LottertDataDB.GetDal<QXC_JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势最新一条数据
        /// </summary>
        public QXC_JBZS QueryQXC_JBZS()
        {
             
            return LottertDataDB.CreateQuery<QXC_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询QXC_JBZS本期是否生成
        /// </summary>
        public int QueryQXC_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<QXC_JBZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加大小走势数据
        /// </summary>
        public void AddQXC_DX(QXC_DX entity)
        {
            LottertDataDB.GetDal<QXC_DX>().Add(entity);
        }

        /// <summary>
        /// 查询大小走势最新一条数据
        /// </summary>
        public QXC_DX QueryQXC_DX()
        {
             
            return LottertDataDB.CreateQuery<QXC_DX>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询QXC_DX本期是否生成
        /// </summary>
        public int QuerQXC_DXIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<QXC_DX>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加奇偶走势数据
        /// </summary>
        public void AddQXC_JO(QXC_JO entity)
        {
            LottertDataDB.GetDal<QXC_JO>().Add(entity);
        }

        /// <summary>
        /// 查询奇偶走势最新一条数据
        /// </summary>
        public QXC_JO QueryQXC_JO()
        {
             
            return LottertDataDB.CreateQuery<QXC_JO>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询QXC_JO本期是否生成
        /// </summary>
        public int QueryQXC_JOIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<QXC_JO>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加质和走势数据
        /// </summary>
        public void AddQXC_ZH(QXC_ZH entity)
        {
            LottertDataDB.GetDal<QXC_ZH>().Add(entity);
        }

        /// <summary>
        /// 查询质和走势最新一条数据
        /// </summary>
        public QXC_ZH QueryQXC_ZH()
        {
             
            return LottertDataDB.CreateQuery<QXC_ZH>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询QXC_ZH本期是否生成
        /// </summary>
        public int QueryQXC_ZHIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<QXC_ZH>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加除3 走势数据
        /// </summary>
        public void AddQXC_Chu3(QXC_Chu3 entity)
        {
            LottertDataDB.GetDal<QXC_Chu3>().Add(entity);
        }

        /// <summary>
        /// 查询除3走势最新一条数据
        /// </summary>
        public QXC_Chu3 QueryQXC_Chu3()
        {
             
            return LottertDataDB.CreateQuery<QXC_Chu3>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询QXC_Chu3本期是否生成
        /// </summary>
        public int QueryQXC_Chu3IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<QXC_Chu3>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        #endregion
    }
}
