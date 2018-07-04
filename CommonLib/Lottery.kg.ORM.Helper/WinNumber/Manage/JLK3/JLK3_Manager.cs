using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JLK3_Manager : DBbase
    {

        #region 前台查询数据

        /// <summary>
        /// 查询基本走势实体类的所有字段
        /// </summary>
        public List<JLK3_JBZS> QueryJLK3_JBZS_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JLK3_JBZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询和值实体类的所有字段
        /// </summary>
        public List<JLK3_HZ> QueryJLK3_HZ_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JLK3_HZ>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询形态实体类的所有字段
        /// </summary>
        public List<JLK3_XT> QueryJLK3_XT_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JLK3_XT>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询组合实体类的所有字段
        /// </summary>
        public List<JLK3_ZH> QueryJLK3_ZH_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JLK3_ZH>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询综合和走势实体类的所有字段
        /// </summary>
        public List<JLK3_ZHZS> QueryJLK3_ZHZS_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JLK3_ZHZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        #endregion

        #region 初始化数据

        /// <summary>
        /// 添加基本走势数据
        /// </summary>
        public void AddJLK3_JBZS(JLK3_JBZS entity)
        {
            LottertDataDB.GetDal<JLK3_JBZS>().Add(entity);
        }

        /// <summary>
        ///  查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JLK3_JBZS QueryJLK3_JBZS()
        {
             
            return LottertDataDB.CreateQuery<JLK3_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JLK3_JBZS本期是否生成
        /// </summary>
        public int QueryJLK3_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JLK3_JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加和值走势数据
        /// </summary>
        public void AddJLK3_HZ(JLK3_HZ entity)
        {
            LottertDataDB.GetDal<JLK3_HZ>().Add(entity);
        }

        /// <summary>
        ///  查询和值走势（遗漏）最新一条数据
        /// </summary>
        public JLK3_HZ QueryJLK3_HZ()
        {
             
            return LottertDataDB.CreateQuery<JLK3_HZ>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JLK3_HZ本期是否生成
        /// </summary>
        public int QueryJLK3_HZIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JLK3_HZ>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加形态走势数据
        /// </summary>
        public void AddJLK3_XT(JLK3_XT entity)
        {
            LottertDataDB.GetDal<JLK3_XT>().Add(entity);
        }

        /// <summary>
        ///  查询形态走势（遗漏）最新一条数据
        /// </summary>
        public JLK3_XT QueryJLK3_XT()
        {
             
            return LottertDataDB.CreateQuery<JLK3_XT>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JLK3_XT本期是否生成
        /// </summary>
        public int QueryJLK3_XTIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JLK3_XT>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加组合走势数据
        /// </summary>
        public void AddJLK3_ZH(JLK3_ZH entity)
        {
            LottertDataDB.GetDal<JLK3_ZH>().Add(entity);
        }

        /// <summary>
        ///  查询组合走势（遗漏）最新一条数据
        /// </summary>
        public JLK3_ZH QueryJLK3_ZH()
        {
             
            return LottertDataDB.CreateQuery<JLK3_ZH>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JLK3_ZH本期是否生成
        /// </summary>
        public int QueryJLK3_ZHIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JLK3_ZH>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加综合走势数据
        /// </summary>
        public void AddJLK3_ZHZS(JLK3_ZHZS entity)
        {
            LottertDataDB.GetDal<JLK3_ZHZS>().Add(entity);
        }

        /// <summary>
        ///  查询综合走势（遗漏）最新一条数据
        /// </summary>
        public JLK3_ZHZS QueryJLK3_ZHZS()
        {
             
            return LottertDataDB.CreateQuery<JLK3_ZHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JLK3_ZHZS本期是否生成
        /// </summary>
        public int QueryJLK3_ZHZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JLK3_ZHZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

        #endregion

    }
}
