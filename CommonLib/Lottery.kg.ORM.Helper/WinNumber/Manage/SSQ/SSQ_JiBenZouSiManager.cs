using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;
using Lottery.Kg.ORM.Helper.WinNumber.ModelCollection;
namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class SSQ_JiBenZouSiManager : DBbase
    {
        /// <summary>
        /// 添加大乐透基本走势数据
        /// </summary>
        public void AddSSQ_JiBenZouSi(SSQ_JiBenZouSi entity)
        {
            LottertDataDB.GetDal<SSQ_JiBenZouSi>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public SSQ_JiBenZouSi QueryLastSSQ_JiBenZouSi()
        {
             
            return LottertDataDB.CreateQuery<SSQ_JiBenZouSi>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<SSQ_JiBenZouSi_Info> QuerySSQ_JiBenZouSiInfo(int index, DateTime startTime, DateTime endTime)
        {
             
            var query = from s in LottertDataDB.CreateQuery<SSQ_JiBenZouSi>()
                        where s.CreateTime >= startTime && s.CreateTime <= endTime
                        orderby s.CreateTime descending
                        select new SSQ_JiBenZouSi_Info
                        {
                            Blue01 = s.Blue01,
                            Blue02 = s.Blue02,
                            Blue03 = s.Blue03,
                            Blue04 = s.Blue04,
                            Blue05 = s.Blue05,
                            Blue06 = s.Blue06,
                            Blue07 = s.Blue07,
                            Blue08 = s.Blue08,
                            Blue09 = s.Blue09,
                            Blue10 = s.Blue10,
                            Blue11 = s.Blue11,
                            Blue12 = s.Blue12,
                            Blue13 = s.Blue13,
                            Blue14 = s.Blue14,
                            Blue15 = s.Blue15,
                            Blue16 = s.Blue16,
                            CreateTime = s.CreateTime,
                            Id = s.Id,
                            IssuseNumber = s.IssuseNumber,
                            Red01 = s.Red01,
                            Red02 = s.Red02,
                            Red03 = s.Red03,
                            Red04 = s.Red04,
                            Red05 = s.Red05,
                            Red06 = s.Red06,
                            Red07 = s.Red07,
                            Red08 = s.Red08,
                            Red09 = s.Red09,
                            Red10 = s.Red10,
                            Red11 = s.Red11,
                            Red12 = s.Red12,
                            Red13 = s.Red13,
                            Red14 = s.Red14,
                            Red15 = s.Red15,
                            Red16 = s.Red16,
                            Red17 = s.Red17,
                            Red18 = s.Red18,
                            Red19 = s.Red19,
                            Red20 = s.Red20,
                            Red21 = s.Red21,
                            Red22 = s.Red22,
                            Red23 = s.Red23,
                            Red24 = s.Red24,
                            Red25 = s.Red25,
                            Red26 = s.Red26,
                            Red27 = s.Red27,
                            Red28 = s.Red28,
                            Red29 = s.Red29,
                            Red30 = s.Red30,
                            Red31 = s.Red31,
                            Red32 = s.Red32,
                            Red33 = s.Red33,
                            WinNumber = s.WinNumber
                        };
            return query.ToList();
        }


        public List<SSQ_JiBenZouSi> QuerySSQ_JiBenZouSi(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<SSQ_JiBenZouSi>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询SSQ_JiBenZouSi本期是否生成
        /// </summary>
        public int QuerySSQ_JiBenZouSiIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SSQ_JiBenZouSi>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
