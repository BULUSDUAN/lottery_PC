using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class SDKLPK3_Manager : DBbase
    {
        #region 导入数据

        /// <summary>
        /// 添加基本走势图表
        /// </summary>
        public void AddSDKLPK3_JBZS(SDKLPK3_JBZS entity)
        {
            LottertDataDB.GetDal<SDKLPK3_JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势本期是否生成
        /// </summary>
        public int QuerySDKLPK3_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_JBZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  查询基本走势（遗漏）最新一条数据
        /// </summary>
        public SDKLPK3_JBZS QuerySDKLPK3_JBZS()
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 添加组选走势
        /// </summary>
        public void AddSDKLPK3_ZHXZS(SDKLPK3_ZHXZS entity)
        {
            LottertDataDB.GetDal<SDKLPK3_ZHXZS>().Add(entity);
        }

        /// <summary>
        /// 查询组选走势本期是否生成
        /// </summary>
        public int QuerySDKLPK3_ZHXZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_ZHXZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  查询组选走势（遗漏）最新一条数据
        /// </summary>
        /// <returns></returns>
        public SDKLPK3_ZHXZS QuerySDKLPK3_ZHXZS()
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_ZHXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }


        /// <summary>
        /// 添加花色
        /// </summary>
        public void AddSDKLPK3_HSZS(SDKLPK3_HSZS entity)
        {
            LottertDataDB.GetDal<SDKLPK3_HSZS>().Add(entity);
        }
        /// <summary>
        /// 查询花色走势本期是否生成
        /// </summary>
        public int QuerySDKLPK3_HSZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_HSZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  查询花色走势（遗漏）最新一条数据
        /// </summary>
        /// <returns></returns>
        public SDKLPK3_HSZS QuerySDKLPK3_HSZS()
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_HSZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }


        /// <summary>
        /// 添加大小
        /// </summary>
        public void AddSDKLPK3_DXZS(SDKLPK3_DXZS entity)
        {
            LottertDataDB.GetDal<SDKLPK3_DXZS>().Add(entity);
        }
        /// <summary>
        /// 查询大小走势本期是否生成
        /// </summary>
        public int QuerySDKLPK3_DXZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_DXZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  查询大小走势（遗漏）最新一条数据
        /// </summary>
        /// <returns></returns>
        public SDKLPK3_DXZS QuerySDKLPK3_DXZS()
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_DXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }


        /// <summary>
        /// 添加奇偶
        /// </summary>
        public void AddSDKLPK3_JOZS(SDKLPK3_JOZS entity)
        {
            LottertDataDB.GetDal<SDKLPK3_JOZS>().Add(entity);
        }
        /// <summary>
        /// 查询奇偶走势本期是否生成
        /// </summary>
        public int QuerySDKLPK3_JOZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_JOZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  查询奇偶走势（遗漏）最新一条数据
        /// </summary>
        /// <returns></returns>
        public SDKLPK3_JOZS QuerySDKLPK3_JOZS()
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_JOZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 添加质合
        /// </summary>
        public void AddSDKLPK3_ZHZS(SDKLPK3_ZHZS entity)
        {
            LottertDataDB.GetDal<SDKLPK3_ZHZS>().Add(entity);
        }
        /// <summary>
        /// 查询质合走势本期是否生成
        /// </summary>
        public int QuerySDKLPK3_ZHZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_ZHZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  查询质合走势（遗漏）最新一条数据
        /// </summary>
        /// <returns></returns>
        public SDKLPK3_ZHZS QuerySDKLPK3_ZHZS()
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_ZHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 添加除3余
        /// </summary>
        public void AddSDKLPK3_C3YZS(SDKLPK3_C3YZS entity)
        {
            LottertDataDB.GetDal<SDKLPK3_C3YZS>().Add(entity);
        }
        /// <summary>
        /// 查询除3余走势本期是否生成
        /// </summary>
        public int QuerySDKLPK3_C3YZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_C3YZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  查询除3余走势（遗漏）最新一条数据
        /// </summary>
        /// <returns></returns>
        public SDKLPK3_C3YZS QuerySDKLPK3_C3YZS()
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_C3YZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 添加和值
        /// </summary>
        public void AddSDKLPK3_HZZS(SDKLPK3_HZZS entity)
        {
            LottertDataDB.GetDal<SDKLPK3_HZZS>().Add(entity);
        }
        /// <summary>
        /// 查询和值走势本期是否生成
        /// </summary>
        public int QuerySDKLPK3_HZZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_HZZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  查询和值走势（遗漏）最新一条数据
        /// </summary>
        /// <returns></returns>
        public SDKLPK3_HZZS QuerySDKLPK3_HZZS()
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_HZZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 添加类型
        /// </summary>
        public void AddSDKLPK3_LXZS(SDKLPK3_LXZS entity)
        {
            LottertDataDB.GetDal<SDKLPK3_LXZS>().Add(entity);
        }
        /// <summary>
        /// 查询类型走势本期是否生成
        /// </summary>
        public int QuerySDKLPK3_LXZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_LXZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }


        /// <summary>
        ///  查询类型走势（遗漏）最新一条数据
        /// </summary>
        /// <returns></returns>
        public SDKLPK3_LXZS QuerySDKLPK3_LXZS()
        {
             
            return LottertDataDB.CreateQuery<SDKLPK3_LXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        #endregion

        #region 前台查询数据

        /// <summary>
        /// 查询基本走势实体类的所有字段
        /// </summary>
        public List<SDKLPK3_JBZS> QuerySDKLPK3_JBZS_Info(int length)
        {
           
            var query = from r in LottertDataDB.CreateQuery<SDKLPK3_JBZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询组选走势实体类的所有字段
        /// </summary>
        public List<SDKLPK3_ZHXZS> QuerySDKLPK3_ZHXZS_Info(int length)
        {
            var query = from r in LottertDataDB.CreateQuery<SDKLPK3_ZHXZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询花色走势实体类的所有字段
        /// </summary>
        public List<SDKLPK3_HSZS> QuerySDKLPK3_HSZS_Info(int length)
        {
            var query = from r in LottertDataDB.CreateQuery<SDKLPK3_HSZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询大小走势实体类的所有字段
        /// </summary>
        public List<SDKLPK3_DXZS> QuerySDKLPK3_DXZS_Info(int length)
        {
            var query = from r in LottertDataDB.CreateQuery<SDKLPK3_DXZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询奇偶走势实体类的所有字段
        /// </summary>
        public List<SDKLPK3_JOZS> QuerySDKLPK3_JOZS_Info(int length)
        {
            var query = from r in LottertDataDB.CreateQuery<SDKLPK3_JOZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询质合走势实体类的所有字段
        /// </summary>
        public List<SDKLPK3_ZHZS> QuerySDKLPK3_ZHZS_Info(int length)
        {
            var query = from r in LottertDataDB.CreateQuery<SDKLPK3_ZHZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询除3余走势实体类的所有字段
        /// </summary>
        public List<SDKLPK3_C3YZS> QuerySDKLPK3_C3YZS_Info(int length)
        {
            var query = from r in LottertDataDB.CreateQuery<SDKLPK3_C3YZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询和值走势实体类的所有字段
        /// </summary>
        public List<SDKLPK3_HZZS> QuerySDKLPK3_HZZS_Info(int length)
        {
            var query = from r in LottertDataDB.CreateQuery<SDKLPK3_HZZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询类型走势实体类的所有字段
        /// </summary>
        public List<SDKLPK3_LXZS> QuerySDKLPK3_LXZS_Info(int length)
        {
            var query = from r in LottertDataDB.CreateQuery<SDKLPK3_LXZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        #endregion
    }
}
