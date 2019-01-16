using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JSK3_Manager : DBbase
    {

        #region 前台查询数据

        /// <summary>
        /// 查询基本走势实体类的所有字段
        /// </summary>
        public List<JSK3_JBZS> QueryJSK3_JBZS_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JSK3_JBZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询和值实体类的所有字段
        /// </summary>
        public List<JSK3_HZ> QueryJSK3_HZ_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JSK3_HZ>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询形态实体类的所有字段
        /// </summary>
        public List<JSK3_XT> QueryJSK3_XT_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JSK3_XT>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询组合实体类的所有字段
        /// </summary>
        public List<JSK3_ZH> QueryJSK3_ZH_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JSK3_ZH>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询综合和走势实体类的所有字段
        /// </summary>
        public List<JSK3_ZHZS> QueryJSK3_ZHZS_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JSK3_ZHZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        #endregion

        #region 初始化数据

        /// <summary>
        /// 添加基本走势数据
        /// </summary>
        public void AddJSK3_JBZS(JSK3_JBZS entity)
        {
            LottertDataDB.GetDal<JSK3_JBZS>().Add(entity);
        }

        /// <summary>
        ///  查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JSK3_JBZS QueryJSK3_JBZS()
        {
             
            return LottertDataDB.CreateQuery<JSK3_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JSK3_JBZS本期是否生成
        /// </summary>
        public int QueryJSK3_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JSK3_JBZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加和值走势数据
        /// </summary>
        public void AddJSK3_HZ(JSK3_HZ entity)
        {
            LottertDataDB.GetDal<JSK3_HZ>().Add(entity);
        }

        /// <summary>
        ///  查询和值走势（遗漏）最新一条数据
        /// </summary>
        public JSK3_HZ QueryJSK3_HZ()
        {
             
            return LottertDataDB.CreateQuery<JSK3_HZ>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JSK3_HZ本期是否生成
        /// </summary>
        public int QueryJSK3_HZIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JSK3_HZ>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加形态走势数据
        /// </summary>
        public void AddJSK3_XT(JSK3_XT entity)
        {
            LottertDataDB.GetDal<JSK3_XT>().Add(entity);
        }

        /// <summary>
        ///  查询形态走势（遗漏）最新一条数据
        /// </summary>
        public JSK3_XT QueryJSK3_XT()
        {
             
            return LottertDataDB.CreateQuery<JSK3_XT>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JSK3_XT本期是否生成
        /// </summary>
        public int QueryJSK3_XTIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JSK3_XT>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加组合走势数据
        /// </summary>
        public void AddJSK3_ZH(JSK3_ZH entity)
        {
            LottertDataDB.GetDal<JSK3_ZH>().Add(entity);
        }

        /// <summary>
        ///  查询组合走势（遗漏）最新一条数据
        /// </summary>
        public JSK3_ZH QueryJSK3_ZH()
        {
             
            return LottertDataDB.CreateQuery<JSK3_ZH>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JSK3_ZH本期是否生成
        /// </summary>
        public int QueryJSK3_ZHIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JSK3_ZH>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加综合走势数据
        /// </summary>
        public void AddJSK3_ZHZS(JSK3_ZHZS entity)
        {
            LottertDataDB.GetDal<JSK3_ZHZS>().Add(entity);
        }

        /// <summary>
        ///  查询综合走势（遗漏）最新一条数据
        /// </summary>
        public JSK3_ZHZS QueryJSK3_ZHZS()
        {
             
            return LottertDataDB.CreateQuery<JSK3_ZHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JSK3_ZHZS本期是否生成
        /// </summary>
        public int QueryJSK3_ZHZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JSK3_ZHZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        #endregion

    }
}
