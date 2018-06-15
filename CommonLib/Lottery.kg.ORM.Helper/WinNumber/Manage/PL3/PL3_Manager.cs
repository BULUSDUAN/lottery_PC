using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class PL3_Manager : DBbase
    {

        #region 前台查询数据

        /// <summary>
        /// 查询基本走势实体类的所有字段
        /// </summary>
        public List<PL3_JiBenZouSi> QueryPL3_JiBenZouSi(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_JiBenZouSi>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询排列3组选走势实体类的所有字段
        /// </summary>
        public List<PL3_ZuXuanZouSi> QueryPL3_ZuXuanZouSi_info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_ZuXuanZouSi>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询排列3大小走势实体类的所有字段
        /// </summary>
        public List<PL3_DX> QueryPL3_DX_info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_DX>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询排列3大小号码走势实体类的所有字段
        /// </summary>
        public List<PL3_DXHM> QueryPL3_DXHM_info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_DXHM>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询排列3奇偶走势实体类的所有字段
        /// </summary>
        public List<PL3_JIOU> QueryPL3_JIOU_info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_JIOU>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询排列3奇偶号码走势实体类的所有字段
        /// </summary>
        public List<PL3_JOHM> QueryPL3_JOHM_info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_JOHM>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询排列3质和走势实体类的所有字段
        /// </summary>
        public List<PL3_ZhiHe> QueryPL3_ZhiHe_info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_ZhiHe>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询排列3质和号码走势实体类的所有字段
        /// </summary>
        public List<PL3_ZHHM> QueryPL3_ZHHM_info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_ZHHM>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询排列3和值走势实体类的所有字段
        /// </summary>
        public List<PL3_HeiZhi> QueryPL3_HeiZhi_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_HeiZhi>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询排列3跨度百位、十位实体类的所有字段
        /// </summary>
        public List<PL3_KuaDu_12> QueryPL3_KuaDu_12_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_KuaDu_12>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询排列3跨度百位、个位实体类的所有字段
        /// </summary>
        public List<PL3_KuaDu_13> QueryPL3_KuaDu_13_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_KuaDu_13>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询排列3跨度十位、个位实体类的所有字段
        /// </summary>
        public List<PL3_KuaDu_23> QueryPL3_KuaDu_23_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_KuaDu_23>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询除3 1走势实体类的所有字段
        /// </summary>
        public List<PL3_Chu31> QueryPL3_PL3_Chu31_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_Chu31>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询除3 2走势实体类的所有字段
        /// </summary>
        public List<PL3_Chu32> QueryPL3_PL3_Chu32_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_Chu32>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询除3 3走势实体类的所有字段
        /// </summary>
        public List<PL3_Chu33> QueryPL3_PL3_Chu33_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_Chu33>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询排列3 和值特征 实体类的所有字段
        /// </summary>
        public List<PL3_HZTZ> QueryPL3_PL3_HZTZ_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_HZTZ>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询排列3 和值合尾 实体类的所有字段
        /// </summary>
        public List<PL3_HZHW> QueryPL3_PL3_HZHW_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<PL3_HZHW>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }


        #endregion

        #region 初始化数据

        /// <summary>
        /// 添加排列3基本走势数据
        /// </summary>
        public void AddPL3_JiBenZouSi(PL3_JiBenZouSi entity)
        {
            LottertDataDB.GetDal<PL3_JiBenZouSi>().Add(entity);
        }

        /// <summary>
        ///  查询基本走势（遗漏）最新一条数据
        /// </summary>
        public PL3_JiBenZouSi QueryPL3_JiBenZouSi()
        {
             
            return LottertDataDB.CreateQuery<PL3_JiBenZouSi>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_JiBenZouSi本期是否生成
        /// </summary>
        public int QueryPL3_JiBenZouSiIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_JiBenZouSi>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列3大小走势数据
        /// </summary>
        public void AddPL3_DX(PL3_DX entity)
        {
            LottertDataDB.GetDal<PL3_DX>().Add(entity);
        }

        /// <summary>
        ///  查询排列3大小走势最新一条数据
        /// </summary>
        public PL3_DX QueryPL3_DX()
        {
             
            return LottertDataDB.CreateQuery<PL3_DX>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_DX本期是否生成
        /// </summary>
        public int QueryPL3_DXIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_DX>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列3组选走势数据
        /// </summary>
        public void AddPL3_ZuXuanZouSi(PL3_ZuXuanZouSi entity)
        {
            LottertDataDB.GetDal<PL3_ZuXuanZouSi>().Add(entity);
        }

        /// <summary>
        /// 查询排列3组选走势最新一条数据
        /// </summary>
        public PL3_ZuXuanZouSi QueryPL3_ZuXuanZouSi()
        {
             
            return LottertDataDB.CreateQuery<PL3_ZuXuanZouSi>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_ZuXuanZouSi本期是否生成
        /// </summary>
        public int QueryPL3_ZuXuanZouSiIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_ZuXuanZouSi>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列3大小号码分布走势数据
        /// </summary>
        public void AddPL3_DXHM(PL3_DXHM entity)
        {
            LottertDataDB.GetDal<PL3_DXHM>().Add(entity);
        }

        /// <summary>
        /// 查询排列3大小号码分布走势最新一条数据
        /// </summary>
        public PL3_DXHM QueryPL3_DXHM()
        {
             
            return LottertDataDB.CreateQuery<PL3_DXHM>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询CQ11X5_JBZS本期是否生成
        /// </summary>
        public int QueryPL3_DXHMIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_DXHM>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        ///  添加排列3奇偶分布走势数据
        /// </summary>
        public void AddPL3_JIOU(PL3_JIOU entity)
        {
            LottertDataDB.GetDal<PL3_JIOU>().Add(entity);
        }

        /// <summary>
        /// 查询排列3奇偶分布走势最新一条数据
        /// </summary>
        public PL3_JIOU QueryPL3_JIOU()
        {
             
            return LottertDataDB.CreateQuery<PL3_JIOU>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_JIOU本期是否生成
        /// </summary>
        public int QueryPL3_JIOUIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_JIOU>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列3奇偶号码分布走势数据
        /// </summary>
        public void AddPL3_JOHM(PL3_JOHM entity)
        {
            LottertDataDB.GetDal<PL3_JOHM>().Add(entity);
        }

        /// <summary>
        /// 查询排列3奇偶号码分布走势最新一条数据
        /// </summary>
        public PL3_JOHM QueryPL3_JOHM()
        {
             
            return LottertDataDB.CreateQuery<PL3_JOHM>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_JOHM本期是否生成
        /// </summary>
        public int QueryPL3_JOHMIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_JOHM>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列3质和形态分布走势数据
        /// </summary>
        public void AddPL3_ZhiHe(PL3_ZhiHe entity)
        {
            LottertDataDB.GetDal<PL3_ZhiHe>().Add(entity);
        }

        /// <summary>
        /// 查询排列3质和形态分布走势最新一条数据
        /// </summary>
        public PL3_ZhiHe QueryPL3_ZhiHe()
        {
             
            return LottertDataDB.CreateQuery<PL3_ZhiHe>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_ZhiHe本期是否生成
        /// </summary>
        public int QueryPL3_ZhiHeIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_ZhiHe>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列3质和号码分布走势数据
        /// </summary>
        public void AddPL3_ZHHM(PL3_ZHHM entity)
        {
            LottertDataDB.GetDal<PL3_ZHHM>().Add(entity);
        }

        /// <summary>
        /// 查询排列3质和号码分布走势最新一条数据
        /// </summary>
        public PL3_ZHHM QueryPL3_ZHHM()
        {
             
            return LottertDataDB.CreateQuery<PL3_ZHHM>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_ZHHM本期是否生成
        /// </summary>
        public int QueryPL3_ZHHMIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_ZHHM>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列3和值走势数据
        /// </summary>
        public void AddPL3_HeiZhi(PL3_HeiZhi entity)
        {
            LottertDataDB.GetDal<PL3_HeiZhi>().Add(entity);
        }

        /// <summary>
        ///  查询排列3和值走势最新一条数据
        /// </summary>
        public PL3_HeiZhi QueryPL3_HeiZhi()
        {
             
            return LottertDataDB.CreateQuery<PL3_HeiZhi>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_HeiZhi本期是否生成
        /// </summary>
        public int QueryPL3_HeiZhiIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_HeiZhi>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列3跨度百十位走势数据
        /// </summary>
        public void AddPL3_KuaDu_12(PL3_KuaDu_12 entity)
        {
            LottertDataDB.GetDal<PL3_KuaDu_12>().Add(entity);
        }

        /// <summary>
        ///  查询排列3跨度百十位最新一条数据
        /// </summary>
        public PL3_KuaDu_12 QueryPL3_KuaDu_12()
        {
             
            return LottertDataDB.CreateQuery<PL3_KuaDu_12>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_KuaDu_12本期是否生成
        /// </summary>
        public int QueryPL3_KuaDu_12IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_KuaDu_12>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列3跨度十个位走势数据
        /// </summary>
        public void AddPL3_KuaDu_23(PL3_KuaDu_23 entity)
        {
            LottertDataDB.GetDal<PL3_KuaDu_23>().Add(entity);
        }

        /// <summary>
        ///  查询排列3跨度十个位最新一条数据
        /// </summary>
        public PL3_KuaDu_23 QueryPL3_KuaDu_23()
        {
             
            return LottertDataDB.CreateQuery<PL3_KuaDu_23>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_KuaDu_23本期是否生成
        /// </summary>
        public int QueryPL3_KuaDu_23IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_KuaDu_23>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列3跨度百个位走势数据
        /// </summary>
        public void AddPL3_KuaDu_13(PL3_KuaDu_13 entity)
        {
            LottertDataDB.GetDal<PL3_KuaDu_13>().Add(entity);
        }

        /// <summary> 
        ///  查询排列3跨度百个位最新一条数据
        /// </summary>
        public PL3_KuaDu_13 QueryPL3_KuaDu_13()
        {
             
            return LottertDataDB.CreateQuery<PL3_KuaDu_13>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_KuaDu_13本期是否生成
        /// </summary>
        public int QueryPL3_KuaDu_13IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_KuaDu_13>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列3 除3 1走势势数据
        /// </summary>
        public void AddPL3_Chu31(PL3_Chu31 entity)
        {
            LottertDataDB.GetDal<PL3_Chu31>().Add(entity);
        }

        /// <summary>
        /// 添加排列3 除3 1走势数据
        /// </summary>
        public PL3_Chu31 QueryPL3_Chu31()
        {
             
            return LottertDataDB.CreateQuery<PL3_Chu31>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_Chu31本期是否生成
        /// </summary>
        public int QueryPL3_Chu31IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_Chu31>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列3 除3 2走势势数据
        /// </summary>
        public void AddPL3_Chu32(PL3_Chu32 entity)
        {
            LottertDataDB.GetDal<PL3_Chu32>().Add(entity);
        }

        /// <summary>
        /// 添加排列3 除3 2走势数据
        /// </summary>
        public PL3_Chu32 QueryPL3_Chu32()
        {
             
            return LottertDataDB.CreateQuery<PL3_Chu32>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_Chu32本期是否生成
        /// </summary>
        public int QueryPL3_Chu32IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_Chu32>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        ///  添加排列3 除3 3走势势数据
        /// </summary>
        public void AddPL3_Chu33(PL3_Chu33 entity)
        {
            LottertDataDB.GetDal<PL3_Chu33>().Add(entity);
        }

        /// <summary>
        /// 添加排列3 除3 3走势数据
        /// </summary>
        public PL3_Chu33 QueryPL3_Chu33()
        {
             
            return LottertDataDB.CreateQuery<PL3_Chu33>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_Chu33本期是否生成
        /// </summary>
        public int QueryPL3_Chu33IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_Chu33>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        /// 添加排列3 和值特征走势势数据
        /// </summary>
        public void AddPL3_HZTZ(PL3_HZTZ entity)
        {
            LottertDataDB.GetDal<PL3_HZTZ>().Add(entity);
        }

        /// <summary>
        ///  添加排列3 和值特征走势数据
        /// </summary>
        public PL3_HZTZ QueryPL3_HZTZ()
        {
             
            return LottertDataDB.CreateQuery<PL3_HZTZ>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_HZTZ本期是否生成
        /// </summary>
        public int QueryPL3_HZTZIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_HZTZ>().Count(p => p.IssuseNumber == issuseNumber);
        }

        /// <summary>
        ///  添加排列3 和值合尾走势势数据
        /// </summary>
        public void AddPL3_HZHW(PL3_HZHW entity)
        {
            LottertDataDB.GetDal<PL3_HZHW>().Add(entity);
        }

        /// <summary>
        /// 添加排列3 和值合尾走势数据
        /// </summary>
        public PL3_HZHW QueryPL3_HZHW()
        {
             
            return LottertDataDB.CreateQuery<PL3_HZHW>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询PL3_HZHW本期是否生成
        /// </summary>
        public int QueryPL3_HZHWIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<PL3_HZHW>().Count(p => p.IssuseNumber == issuseNumber);
        }

        #endregion

    }
}
