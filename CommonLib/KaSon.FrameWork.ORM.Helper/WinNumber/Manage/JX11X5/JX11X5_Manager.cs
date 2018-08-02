using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JX11X5_Manager : DBbase
    {

        #region 前台查询数据

        /// <summary>
        /// 查询任选基本走势实体类的所有字段
        /// </summary>
        public List<JX11X5_RXJBZS> QueryJX11X5_RXJBZS_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_RXJBZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选大小实体类的所有字段
        /// </summary>
        public List<JX11X5_RXDX> QueryJX11X5_RXDX_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_RXDX>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选大小实体类的所有字段
        /// </summary>
        public List<JX11X5_RXJO> QueryJX11X5_RXJO_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_RXJO>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选质和实体类的所有字段
        /// </summary>
        public List<JX11X5_RXZH> QueryJX11X5_RXZH_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_RXZH>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选和值实体类的所有字段
        /// </summary>
        public List<JX11X5_RXHZ> QueryJX11X5_RXHZ_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_RXHZ>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选除3实体类的所有字段
        /// </summary>
        public List<JX11X5_Chu3> QueryJX11X5_Chu3_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_Chu3>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选第一位实体类的所有字段
        /// </summary>
        public List<JX11X5_RX1> QueryJX11X5_RX1_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_RX1>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选第二位实体类的所有字段
        /// </summary>
        public List<JX11X5_RX2> QueryJX11X5_RX2_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_RX2>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选第三位实体类的所有字段
        /// </summary>
        public List<JX11X5_RX3> QueryJX11X5_RX3_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_RX3>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选第四位实体类的所有字段
        /// </summary>
        public List<JX11X5_RX4> QueryJX11X5_RX4_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_RX4>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选第五位实体类的所有字段
        /// </summary>
        public List<JX11X5_RX5> QueryJX11X5_RX5_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_RX5>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选前三直选实体类的所有字段
        /// </summary>
        public List<JX11X5_Q3ZS> QueryJX11X5_Q3ZS_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_Q3ZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选前三组选实体类的所有字段
        /// </summary>
        public List<JX11X5_Q3ZUS> QueryJX11X5_Q3ZUS_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_Q3ZUS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选前三大小实体类的所有字段
        /// </summary>
        public List<JX11X5_Q3DX> QueryJX11X5_Q3DX_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_Q3DX>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选前三奇偶实体类的所有字段
        /// </summary>
        public List<JX11X5_Q3JO> QueryJX11X5_Q3JO_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_Q3JO>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选前三质和实体类的所有字段
        /// </summary>
        public List<JX11X5_Q3ZH> QueryJX11X5_Q3ZH_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_Q3ZH>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选前三除3实体类的所有字段
        /// </summary>
        public List<JX11X5_Q3Chu3> QueryJX11X5_Q3Chu3_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_Q3Chu3>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选前三和值实体类的所有字段
        /// </summary>
        public List<JX11X5_Q3HZ> QueryJX11X5_Q3HZ_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_Q3HZ>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选前2直选实体类的所有字段
        /// </summary>
        public List<JX11X5_Q2ZS> QueryJX11X5_Q2ZS_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_Q2ZS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选前2组选实体类的所有字段
        /// </summary>
        public List<JX11X5_Q2ZUS> QueryJX11X5_Q2ZUS_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_Q2ZUS>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        /// <summary>
        /// 查询任选前2和值实体类的所有字段
        /// </summary>
        public List<JX11X5_Q2HZ> QueryJX11X5_Q2HZ_Info(int length)
        {
             
            var query = from r in LottertDataDB.CreateQuery<JX11X5_Q2HZ>()
                        orderby r.IssuseNumber descending
                        select r;
            return query.Take(length).ToList();
        }

        #endregion

        #region 初始化数据

        /// <summary>
        /// 添加排列3基本走势数据
        /// </summary>
        public void AddJX11X5_RXJBZS(JX11X5_RXJBZS entity)
        {
            LottertDataDB.GetDal<JX11X5_RXJBZS>().Add(entity);
        }

        /// <summary>
        ///  查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JX11X5_RXJBZS QueryJX11X5_RXJBZS()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RXJBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_RXJBZS本期是否生成
        /// </summary>
        public int QueryJX11X5_RXJBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RXJBZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加任选大小数据
        /// </summary>
        public void AddJX11X5_RXDX(JX11X5_RXDX entity)
        {
            LottertDataDB.GetDal<JX11X5_RXDX>().Add(entity);
        }

        /// <summary>
        ///  查询任选大小（遗漏）最新一条数据
        /// </summary>
        public JX11X5_RXDX QueryJX11X5_RXDX()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RXDX>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_RXDX本期是否生成
        /// </summary>
        public int QueryJX11X5_RXDXIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RXDX>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加任选奇偶数据
        /// </summary>
        public void AddJX11X5_RXJO(JX11X5_RXJO entity)
        {
            LottertDataDB.GetDal<JX11X5_RXJO>().Add(entity);
        }

        /// <summary>
        /// 查询任选奇偶（遗漏）最新一条数据
        /// </summary>
        public JX11X5_RXJO QueryJX11X5_RXJO()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RXJO>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_RXJO本期是否生成
        /// </summary>
        public int QueryJX11X5_RXJOIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RXJO>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  添加任选质和数据
        /// </summary>
        public void AddJX11X5_RXZH(JX11X5_RXZH entity)
        {
            LottertDataDB.GetDal<JX11X5_RXZH>().Add(entity);
        }

        /// <summary>
        /// 查询任选质和（遗漏）最新一条数据
        /// </summary>
        public JX11X5_RXZH QueryJX11X5_RXZH()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RXZH>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_RXZH本期是否生成
        /// </summary>
        public int QueryJX11X5_RXZHIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RXZH>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  添加任选和值数据
        /// </summary>
        public void AddJX11X5_RXHZ(JX11X5_RXHZ entity)
        {
            LottertDataDB.GetDal<JX11X5_RXHZ>().Add(entity);
        }

        /// <summary>
        /// 查询任选和值（遗漏）最新一条数据
        /// </summary>
        public JX11X5_RXHZ QueryJX11X5_RXHZ()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RXHZ>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_RXHZ本期是否生成
        /// </summary>
        public int QueryJX11X5_RXHZIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RXHZ>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加任选和值数据
        /// </summary>
        public void AddJX11X5_Chu3(JX11X5_Chu3 entity)
        {
            LottertDataDB.GetDal<JX11X5_Chu3>().Add(entity);
        }

        /// <summary>
        ///  查询任选和值（遗漏）最新一条数据
        /// </summary>
        public JX11X5_Chu3 QueryJX11X5_Chu3()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Chu3>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_Chu3本期是否生成
        /// </summary>
        public int QueryJX11X5_Chu3IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Chu3>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  添加任选第一位数据
        /// </summary>
        public void AddJX11X5_RX1(JX11X5_RX1 entity)
        {
            LottertDataDB.GetDal<JX11X5_RX1>().Add(entity);
        }

        /// <summary>
        /// 查询任选第一位（遗漏）最新一条数据
        /// </summary>
        public JX11X5_RX1 QueryJX11X5_RX1()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RX1>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_RX1本期是否生成
        /// </summary>
        public int QueryJX11X5_RX1IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RX1>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  添加任选第二位数据
        /// </summary>
        public void AddJX11X5_RX2(JX11X5_RX2 entity)
        {
            LottertDataDB.GetDal<JX11X5_RX2>().Add(entity);
        }

        /// <summary>
        /// 查询任选第二位（遗漏）最新一条数据
        /// </summary>
        public JX11X5_RX2 QueryJX11X5_RX2()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RX2>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_RX2本期是否生成
        /// </summary>
        public int QueryJX11X5_RX2IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RX2>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  添加任选第三位数据
        /// </summary>
        public void AddJX11X5_RX3(JX11X5_RX3 entity)
        {
            LottertDataDB.GetDal<JX11X5_RX3>().Add(entity);
        }

        /// <summary>
        /// 查询任选第三位（遗漏）最新一条数据
        /// </summary>
        public JX11X5_RX3 QueryJX11X5_RX3()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RX3>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_RX3本期是否生成
        /// </summary>
        public int QueryJX11X5_RX3IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RX3>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  添加任选第四位数据
        /// </summary>
        public void AddJX11X5_RX4(JX11X5_RX4 entity)
        {
            LottertDataDB.GetDal<JX11X5_RX4>().Add(entity);
        }

        /// <summary>
        /// 查询任选第四位（遗漏）最新一条数据
        /// </summary>
        public JX11X5_RX4 QueryJX11X5_RX4()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RX4>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_RX4本期是否生成
        /// </summary>
        public int QueryJX11X5_RX4IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RX4>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  添加任选第五位数据
        /// </summary>
        public void AddJX11X5_RX5(JX11X5_RX5 entity)
        {
            LottertDataDB.GetDal<JX11X5_RX5>().Add(entity);
        }

        /// <summary>
        /// 查询任选第五位（遗漏）最新一条数据
        /// </summary>
        public JX11X5_RX5 QueryJX11X5_RX5()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RX5>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_RX5本期是否生成
        /// </summary>
        public int QueryJX11X5_RX5IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_RX5>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }


        /// <summary>
        /// 添加任选前三直选数据
        /// </summary>
        public void AddJX11X5_Q3ZS(JX11X5_Q3ZS entity)
        {
            LottertDataDB.GetDal<JX11X5_Q3ZS>().Add(entity);
        }

        /// <summary>
        /// 查询任选前三直选最新一条数据
        /// </summary>
        public JX11X5_Q3ZS QueryJX11X5_Q3ZS()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q3ZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_Q3ZS本期是否生成
        /// </summary>
        public int QueryJX11X5_Q3ZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q3ZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加任选前三组选数据
        /// </summary>
        public void AddJX11X5_Q3ZUS(JX11X5_Q3ZUS entity)
        {
            LottertDataDB.GetDal<JX11X5_Q3ZUS>().Add(entity);
        }

        /// <summary>
        /// 查询任选前三组选最新一条数据
        /// </summary>
        public JX11X5_Q3ZUS QueryJX11X5_Q3ZUS()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q3ZUS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_Q3ZUS本期是否生成
        /// </summary>
        public int QueryJX11X5_Q3ZUSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q3ZUS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加任选前三大小数据
        /// </summary>
        public void AddJX11X5_Q3DX(JX11X5_Q3DX entity)
        {
            LottertDataDB.GetDal<JX11X5_Q3DX>().Add(entity);
        }

        /// <summary>
        /// 查询任选前三大小最新一条数据
        /// </summary>
        public JX11X5_Q3DX QueryJX11X5_Q3DX()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q3DX>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_Q3DX本期是否生成
        /// </summary>
        public int QueryJX11X5_Q3DXIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q3DX>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加任选前三奇偶数据
        /// </summary>
        public void AddJX11X5_Q3JO(JX11X5_Q3JO entity)
        {
            LottertDataDB.GetDal<JX11X5_Q3JO>().Add(entity);
        }

        /// <summary>
        /// 查询任选前三奇偶最新一条数据
        /// </summary>
        public JX11X5_Q3JO QueryJX11X5_Q3JO()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q3JO>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_Q3JO本期是否生成
        /// </summary>
        public int QueryJX11X5_Q3JOIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q3JO>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加任选前三质和数据
        /// </summary>
        public void AddJX11X5_Q3ZH(JX11X5_Q3ZH entity)
        {
            LottertDataDB.GetDal<JX11X5_Q3ZH>().Add(entity);
        }

        /// <summary>
        /// 查询任选前三质和最新一条数据
        /// </summary>
        public JX11X5_Q3ZH QueryJX11X5_Q3ZH()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q3ZH>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_Q3ZH本期是否生成
        /// </summary>
        public int QueryJX11X5_Q3ZHIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q3ZH>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加任选前三除3数据
        /// </summary>
        public void AddJX11X5_Q3Chu3(JX11X5_Q3Chu3 entity)
        {
            LottertDataDB.GetDal<JX11X5_Q3Chu3>().Add(entity);
        }

        /// <summary>
        ///  查询任选前三除3最新一条数据
        /// </summary>
        public JX11X5_Q3Chu3 QueryJX11X5_Q3Chu3()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q3Chu3>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_Q3Chu3本期是否生成
        /// </summary>
        public int QueryJX11X5_Q3Chu3IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q3Chu3>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加任选前三和值数据
        /// </summary>
        public void AddJX11X5_Q3HZ(JX11X5_Q3HZ entity)
        {
            LottertDataDB.GetDal<JX11X5_Q3HZ>().Add(entity);
        }

        /// <summary>
        /// 查询任选前三和值最新一条数据
        /// </summary>
        public JX11X5_Q3HZ QueryJX11X5_Q3HZ()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q3HZ>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_Q3HZ本期是否生成
        /// </summary>
        public int QueryJX11X5_Q3HZIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q3HZ>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加任选前2直选数据
        /// </summary>
        public void AddJX11X5_Q2ZS(JX11X5_Q2ZS entity)
        {
            LottertDataDB.GetDal<JX11X5_Q2ZS>().Add(entity);
        }

        /// <summary>
        /// 查询任选前2直选最新一条数据
        /// </summary>
        public JX11X5_Q2ZS QueryJX11X5_Q2ZS()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q2ZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_Q2ZS本期是否生成
        /// </summary>
        public int QueryJX11X5_Q2ZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q2ZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        /// 添加任选前2组选数据
        /// </summary>
        public void AddJX11X5_Q2ZUS(JX11X5_Q2ZUS entity)
        {
            LottertDataDB.GetDal<JX11X5_Q2ZUS>().Add(entity);
        }

        /// <summary>
        /// 查询任选前2组选最新一条数据
        /// </summary>
        public JX11X5_Q2ZUS QueryJX11X5_Q2ZUS()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q2ZUS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_Q2ZUS本期是否生成
        /// </summary>
        public int QueryJX11X5_Q2ZUSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q2ZUS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        /// <summary>
        ///  添加任选前2和值数据
        /// </summary>
        public void AddJX11X5_Q2HZ(JX11X5_Q2HZ entity)
        {
            LottertDataDB.GetDal<JX11X5_Q2HZ>().Add(entity);
        }

        /// <summary>
        ///  查询任选前2和值最新一条数据
        /// </summary>
        public JX11X5_Q2HZ QueryJX11X5_Q2HZ()
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q2HZ>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 查询JX11X5_Q2HZ本期是否生成
        /// </summary>
        public int QueryJX11X5_Q2HZIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JX11X5_Q2HZ>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

        #endregion

    }
}
