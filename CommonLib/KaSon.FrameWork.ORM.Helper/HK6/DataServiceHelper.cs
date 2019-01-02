using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 生肖正肖
    /// </summary>
   public class DataServiceHelper 
    {
        private IDbProvider DB = null;
       
        public DataServiceHelper(IDbProvider _DB) 
        {
            DB = _DB;
        }

        public blast_lhc_time GetissueNo() {
           // result.IsSuccess = true;
            DateTime date = DateTime.Parse(DateTime.Now.ToShortDateString());
            if (DateTime.Now.Hour > 21)
            {
                date = DateTime.Parse(DateTime.Now.AddDays(1).ToShortDateString());
            }
            var tquery = DB.CreateQuery<blast_lhc_time>();
            var mb = (from b in tquery
                      where b.actionTime >= date
                      orderby b.actionTime ascending
                      select b).FirstOrDefault();
            string num = (mb.actionNo + "").Replace(DateTime.Now.Year.ToString(), "");
           // mb.PreData = GetWinData(mb);
            return mb;

        }
        private blast_data GetWinData(blast_lhc_time btime) {
            int issueNo =int.Parse( btime.actionNo+"");
            if (issueNo<2000)
            {
                issueNo = int.Parse(btime.actionTime.Year + "" + issueNo);
            }
            var query = DB.CreateQuery<blast_data>();
            blast_data data = new blast_data();
            
                int year = int.Parse((issueNo + "").Substring(0, 4));
                int iNo = int.Parse((issueNo + "").Substring(4));
                if (iNo - 1 <= 0)
                {
                    string date = (year - 1) + "-12-01";
                    string ete = (year) + "-01-01";
                    DateTime sdate = DateTime.Parse(date);
                    DateTime edate = DateTime.Parse(ete);
                    //上一年的期号
                    data = (from b in query
                            where b.kjtime > sdate && b.kjtime < edate
                            orderby b.kjtime descending
                            select b).FirstOrDefault();
                    if (data == null)
                    {
                        blast_lhc_time datatime = (from b in DB.CreateQuery<blast_lhc_time>()
                                where b.actionTime > sdate && b.actionTime < edate
                                orderby b.actionTime descending
                                select b).FirstOrDefault();
                        data=   new blast_data() { issueNo = datatime.actionNo, kjdata = "", typeid = 1, kjnumber = "", kjtime = datatime.actionTime };
                    }
                }
                else {
                    int nissueNo = (issueNo - 1);
                    data = query.Where(b => b.issueNo == nissueNo).FirstOrDefault();
                    if (data == null)
                    {
                        blast_lhc_time datatime = DB.CreateQuery<blast_lhc_time>().Where(b => b.actionNo == nissueNo).FirstOrDefault();
                        data = new blast_data() { issueNo = datatime.actionNo , kjdata = "", typeid = 1, kjnumber = "", kjtime = datatime.actionTime };
                    }
                }
               
            
            string str = data.kjdata + "";


            if (str.Contains(",")&& !str.Contains(","))
            {
                int index = str.LastIndexOf(',');
                string temp = str.Substring(0, index);
                string temp1 = str.Substring(index);
                data.kjdata = temp + "+" + temp1.Replace(",", "");
            }
            return data;
        }
       
    }
}
