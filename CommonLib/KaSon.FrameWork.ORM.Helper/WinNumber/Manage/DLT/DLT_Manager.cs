using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class DLT_Manager : DBbase
    {

        #region 前台查询数据

        /// <summary>
        /// 查询基本走势实体类的所有字段
        /// </summary>
        public List<DLT_JiBenZouSi> QueryDLT_JiBenZouSi(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<DLT_JiBenZouSi>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询大小实体类的所有字段
        /// </summary>
        public List<DLT_DX> QueryDLT_DX(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<DLT_DX>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询奇偶实体类的所有字段
        /// </summary>
        public List<DLT_JiOu> QueryDLT_JiOu(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<DLT_JiOu>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询质和实体类的所有字段
        /// </summary>
        public List<DLT_ZhiHe> QueryDLT_ZhiHe(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<DLT_ZhiHe>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询和值实体类的所有字段
        /// </summary>
        public List<DLT_HeZhi> QueryDLT_HeZhi(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<DLT_HeZhi>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询除3实体类的所有字段
        /// </summary>
        public List<DLT_Chu3> QueryDLT_Chu3(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<DLT_Chu3>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询首尾实体类的所有字段
        /// </summary>
        public List<DLT_KuaDu_SW> QueryDLT_KuaDu_SW(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<DLT_KuaDu_SW>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询12实体类的所有字段
        /// </summary>
        public List<DLT_KuaDu_12> QueryDLT_KuaDu_12(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<DLT_KuaDu_12>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询23实体类的所有字段
        /// </summary>
        public List<DLT_KuaDu_23> QueryDLT_KuaDu_23(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<DLT_KuaDu_23>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }


        /// <summary>
        /// 查询34实体类的所有字段
        /// </summary>
        public List<DLT_KuaDu_34> QueryDLT_KuaDu_34(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<DLT_KuaDu_34>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询45实体类的所有字段
        /// </summary>
        public List<DLT_KuaDu_45> QueryDLT_KuaDu_45(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<DLT_KuaDu_45>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        #endregion

        #region 初始化数据

        /// <summary>
        /// 添加大乐透基本走势数据
        /// </summary>
        public void AddDLT_JiBenZouSi(DLT_JiBenZouSi entity)
        {
            LottertDataDB.GetDal<DLT_JiBenZouSi>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public DLT_JiBenZouSi QueryLastDLT_JiBenZouSi()
        {
             
            return LottertDataDB.CreateQuery<DLT_JiBenZouSi>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询DLT_JiBenZouSi本期是否生成
        /// </summary>
        public int QueryDLT_JiBenZouSiIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DLT_JiBenZouSi>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加大乐透大小走势
        /// </summary>
        public void AddDLT_DX(DLT_DX entity)
        {
            LottertDataDB.GetDal<DLT_DX>().Add(entity);
        }

        /// <summary>
        /// 查询大小走势最新一条数据
        /// </summary>
        public DLT_DX QueryDLT_DX()
        {
             
            return LottertDataDB.CreateQuery<DLT_DX>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询DLT_DX本期是否生成
        /// </summary>
        public int QueryDLT_DXIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DLT_DX>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加奇偶走势
        /// </summary>
        public void AddDLT_JiOu(DLT_JiOu entity)
        {
            LottertDataDB.GetDal<DLT_JiOu>().Add(entity);
        }

        /// <summary>
        /// 查询奇偶走势最新一条数据
        /// </summary>
        public DLT_JiOu QueryDLT_JiOu()
        {
             
            return LottertDataDB.CreateQuery<DLT_JiOu>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询DLT_JiOu本期是否生成
        /// </summary>
        public int QueryDLT_JiOuIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DLT_JiOu>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加质和走势
        /// </summary>
        public void AddDLT_ZhiHe(DLT_ZhiHe entity)
        {
            LottertDataDB.GetDal<DLT_ZhiHe>().Add(entity);
        }

        /// <summary>
        /// 查询质和走势最新一条数据
        /// </summary>
        public DLT_ZhiHe QueryDLT_ZhiHe()
        {
             
            return LottertDataDB.CreateQuery<DLT_ZhiHe>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询DLT_ZhiHe本期是否生成
        /// </summary>
        public int QueryDLT_ZhiHeIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DLT_ZhiHe>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加和值走势
        /// </summary>
        public void AddDLT_HeZhi(DLT_HeZhi entity)
        {
            LottertDataDB.GetDal<DLT_HeZhi>().Add(entity);
        }

        /// <summary>
        /// 查询和值走势最新一条数据
        /// </summary>
        public DLT_HeZhi QueryDLT_HeZhi()
        {
             
            return LottertDataDB.CreateQuery<DLT_HeZhi>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询DLT_HeZhi本期是否生成
        /// </summary>
        public int QueryDLT_HeZhiIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DLT_HeZhi>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加除3走势
        /// </summary>
        public void AddDLT_Chu3(DLT_Chu3 entity)
        {
            LottertDataDB.GetDal<DLT_Chu3>().Add(entity);
        }

        /// <summary>
        /// 查询除3走势最新一条数据
        /// </summary>
        public DLT_Chu3 QueryDLT_Chu3()
        {
             
            return LottertDataDB.CreateQuery<DLT_Chu3>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询DLT_Chu3本期是否生成
        /// </summary>
        public int QueryDLT_Chu3IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DLT_Chu3>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加首尾跨度走势
        /// </summary>
        public void AddDLT_KuaDu_SW(DLT_KuaDu_SW entity)
        {
            LottertDataDB.GetDal<DLT_KuaDu_SW>().Add(entity);
        }

        /// <summary>
        /// 查询首尾跨度走势最新一条数据
        /// </summary>
        public DLT_KuaDu_SW QueryDLT_KuaDu_SW()
        {
             
            return LottertDataDB.CreateQuery<DLT_KuaDu_SW>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询DLT_KuaDu_SW本期是否生成
        /// </summary>
        public int QueryDLT_KuaDu_SWIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DLT_KuaDu_SW>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加12跨度走势
        /// </summary>
        public void AddDLT_KuaDu_12(DLT_KuaDu_12 entity)
        {
            LottertDataDB.GetDal<DLT_KuaDu_12>().Add(entity);
        }

        /// <summary>
        /// 查询23跨度走势最新一条数据
        /// </summary>
        public DLT_KuaDu_12 QueryDLT_KuaDu_12()
        {
             
            return LottertDataDB.CreateQuery<DLT_KuaDu_12>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询DLT_KuaDu_12本期是否生成
        /// </summary>
        public int QueryDLT_KuaDu_12IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DLT_KuaDu_12>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加23跨度走势
        /// </summary>
        public void AddDLT_KuaDu_23(DLT_KuaDu_23 entity)
        {
            LottertDataDB.GetDal<DLT_KuaDu_23>().Add(entity);
        }

        /// <summary>
        /// 查询23跨度走势最新一条数据
        /// </summary>
        public DLT_KuaDu_23 QueryDLT_KuaDu_23()
        {
             
            return LottertDataDB.CreateQuery<DLT_KuaDu_23>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询DLT_KuaDu_23本期是否生成
        /// </summary>
        public int QueryDLT_KuaDu_23IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DLT_KuaDu_23>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加34跨度走势
        /// </summary>
        public void AddDLT_KuaDu_34(DLT_KuaDu_34 entity)
        {
            LottertDataDB.GetDal<DLT_KuaDu_34>().Add(entity);
        }

        /// <summary>
        /// 查询34跨度走势最新一条数据
        /// </summary>
        public DLT_KuaDu_34 QueryDLT_KuaDu_34()
        {
             
            return LottertDataDB.CreateQuery<DLT_KuaDu_34>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询DLT_KuaDu_34本期是否生成
        /// </summary>
        public int QueryDLT_KuaDu_34IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DLT_KuaDu_34>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加45跨度走势
        /// </summary>
        public void AddDLT_KuaDu_45(DLT_KuaDu_45 entity)
        {
            LottertDataDB.GetDal<DLT_KuaDu_45>().Add(entity);
        }

        /// <summary>
        /// 查询45跨度走势最新一条数据
        /// </summary>
        public DLT_KuaDu_45 QueryDLT_KuaDu_45()
        {
             
            return LottertDataDB.CreateQuery<DLT_KuaDu_45>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询DLT_KuaDu_45本期是否生成
        /// </summary>
        public int QueryDLT_KuaDu_45IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DLT_KuaDu_45>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        #endregion
    }
}
