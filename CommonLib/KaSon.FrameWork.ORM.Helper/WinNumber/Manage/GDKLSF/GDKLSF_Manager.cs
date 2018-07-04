using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class GDKLSF_Manager : DBbase
    {

        #region 前台查询数据

        /// <summary>
        /// 查询基本走势实体类的所有字段
        /// </summary>
        public List<GDKLSF_JBZS> QueryGDKLSF_JBZS(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<GDKLSF_JBZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询定位第一位实体类的所有字段
        /// </summary>
        public List<GDKLSF_DW1> QueryGDKLSF_DW1(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<GDKLSF_DW1>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询定位第二位实体类的所有字段
        /// </summary>
        public List<GDKLSF_DW2> QueryGDKLSF_DW2(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<GDKLSF_DW2>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询定位第三位实体类的所有字段
        /// </summary>
        public List<GDKLSF_DW3> QueryGDKLSF_DW3(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<GDKLSF_DW3>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询大小实体类的所有字段
        /// </summary>
        public List<GDKLSF_DX> QueryGDKLSF_DX(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<GDKLSF_DX>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询奇偶实体类的所有字段
        /// </summary>
        public List<GDKLSF_JO> QueryGDKLSF_JO(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<GDKLSF_JO>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询质和实体类的所有字段
        /// </summary>
        public List<GDKLSF_ZH> QueryGDKLSF_ZH(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<GDKLSF_ZH>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        #endregion

        #region 初始化数据

        /// <summary>
        /// 添加基本走势数据
        /// </summary>
        public void AddGDKLSF_JBZS(GDKLSF_JBZS entity)
        {
            LottertDataDB.GetDal<GDKLSF_JBZS>().Add(entity);
        }

        /// <summary>
        ///  查询基本走势最新一条数据
        /// </summary>
        public GDKLSF_JBZS QueryGDKLSF_JBZS()
        {
             
            return LottertDataDB.CreateQuery<GDKLSF_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询GDKLSF_JBZS本期是否生成
        /// </summary>
        public int QueryGDKLSF_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GDKLSF_JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加定位第一位数据
        /// </summary>
        public void AddGDKLSF_DW1(GDKLSF_DW1 entity)
        {
            LottertDataDB.GetDal<GDKLSF_DW1>().Add(entity);
        }

        /// <summary>
        ///  查询定位第一位最新一条数据
        /// </summary>
        public GDKLSF_DW1 QueryGDKLSF_DW1()
        {
             
            return LottertDataDB.CreateQuery<GDKLSF_DW1>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询GDKLSF_DW1本期是否生成
        /// </summary>
        public int QueryGDKLSF_DW1IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GDKLSF_DW1>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加定位第二位数据
        /// </summary>
        public void AddGDKLSF_DW2(GDKLSF_DW2 entity)
        {
            LottertDataDB.GetDal<GDKLSF_DW2>().Add(entity);
        }

        /// <summary>
        ///  查询定位第二位最新一条数据
        /// </summary>
        public GDKLSF_DW2 QueryGDKLSF_DW2()
        {
             
            return LottertDataDB.CreateQuery<GDKLSF_DW2>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询GDKLSF_DW2本期是否生成
        /// </summary>
        public int QueryGDKLSF_DW2IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GDKLSF_DW2>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加定位第三位数据
        /// </summary>
        public void AddGDKLSF_DW3(GDKLSF_DW3 entity)
        {
            LottertDataDB.GetDal<GDKLSF_DW3>().Add(entity);
        }

        /// <summary>
        ///  查询定位第三位最新一条数据
        /// </summary>
        public GDKLSF_DW3 QueryGDKLSF_DW3()
        {
             
            return LottertDataDB.CreateQuery<GDKLSF_DW3>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询GDKLSF_DW3本期是否生成
        /// </summary>
        public int QueryGDKLSF_DW3IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GDKLSF_DW3>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加大小数据
        /// </summary>
        public void AddGDKLSF_DX(GDKLSF_DX entity)
        {
            LottertDataDB.GetDal<GDKLSF_DX>().Add(entity);
        }

        /// <summary>
        ///  查询大小最新一条数据
        /// </summary>
        public GDKLSF_DX QueryGDKLSF_DX()
        {
             
            return LottertDataDB.CreateQuery<GDKLSF_DX>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询GDKLSF_DX本期是否生成
        /// </summary>
        public int QueryGDKLSF_DXIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GDKLSF_DX>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加奇偶数据
        /// </summary>
        public void AddGDKLSF_JO(GDKLSF_JO entity)
        {
            LottertDataDB.GetDal<GDKLSF_JO>().Add(entity);
        }

        /// <summary>
        ///  查询奇偶最新一条数据
        /// </summary>
        public GDKLSF_JO QueryGDKLSF_JO()
        {
             
            return LottertDataDB.CreateQuery<GDKLSF_JO>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询GDKLSF_JO本期是否生成
        /// </summary>
        public int QueryGDKLSF_JOIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GDKLSF_JO>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加质和数据
        /// </summary>
        public void AddGDKLSF_ZH(GDKLSF_ZH entity)
        {
            LottertDataDB.GetDal<GDKLSF_ZH>().Add(entity);
        }

        /// <summary>
        ///  查询质和最新一条数据
        /// </summary>
        public GDKLSF_ZH QueryGDKLSF_ZH()
        {
             
            return LottertDataDB.CreateQuery<GDKLSF_ZH>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询GDKLSF_ZH本期是否生成
        /// </summary>
        public int QueryGDKLSF_ZHIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GDKLSF_ZH>().Count(p => p.IssuseNumber == issuseNumber);
        }

        #endregion

    }
}
