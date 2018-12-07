using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using KaSon.FrameWork.Common.Hk6;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 生肖正肖
    /// </summary>
   public class DataInserterHelper 
    {
        private IDbProvider DB = null;
       
        public DataInserterHelper(IDbProvider _DB) {
            DB = _DB;
        }


        private void DataAdd两面(int playid) {
            //   519 1.98    特双  02  318 两面  1   2       TS

            List<dynamic> list = new List<dynamic>();
            int index = 0;
            list.Add(new { fag= "TD", Name= "特单" });
            list.Add(new { fag = "TS", Name = "特双" });
            list.Add(new { fag = "TDa", Name = "特大" });
            list.Add(new { fag = "TXiao", Name = "特小" });
            list.Add(new { fag = "THDa", Name = "特合大" });
            list.Add(new { fag = "THX", Name = "特合小" });
            list.Add(new { fag = "THDan", Name = "特合单" });
            list.Add(new { fag = "THShuang", Name = "特合双" });

            list.Add(new { fag = "TWDa", Name = "特尾大" });
            list.Add(new { fag = "TWXiao", Name = "特尾小" });

            list.Add(new { fag = "TDXiao", Name = "特地肖" });
            list.Add(new { fag = "TTXiao", Name = "特天肖" });
            list.Add(new { fag = "TQXiao", Name = "特前肖" });
            list.Add(new { fag = "THXiao", Name = "特后肖" });

            list.Add(new { fag = "TJiaXiao", Name = "特家肖" });
            list.Add(new { fag = "TYXiao", Name = "特野肖" });
            list.Add(new { fag = "ZongDa", Name = "总大" });
            list.Add(new { fag = "ZongXiao", Name = "总小" });
            list.Add(new { fag = "ZongDan", Name = "总单" });
            list.Add(new { fag = "ZongShuan", Name = "总双" });


            IList<blast_lhc_antecode> antList = new List<blast_lhc_antecode>();
            foreach (var item in list)
            {
                index++;
                blast_lhc_antecode ant = new blast_lhc_antecode()
                {
                    sort = index,
                    enable = true,
                    fag = item.fag,
                    AnteCode = item.fag,
                    displayName = item.Name,
                    odds = 1.98M,
                    playid = playid,
                    remark = "两面玩法",
                    createTime = DateTime.Now,
                    updateTime = DateTime.Now

                };
                antList.Add(ant);
            }
            Add(antList);


        }

        private void DataAdd特码正码(int payid,decimal odds=48.8M, string mark="特码")
        {
            //   519 1.98    特双  02  318 两面  1   2       TS
            string code = "";
            IList<blast_lhc_antecode> antList = new List<blast_lhc_antecode>();
            for (int i = 1; i < 50; i++)
            {
                if (i<10)
                {
                    code = "0" + i;
                }
                else 
                {
                    code = i + "";
                }
              
                blast_lhc_antecode ant = new blast_lhc_antecode()
                {
                    sort = i,
                    enable = true,
                    fag = code,
                    AnteCode = code,
                    displayName = code,
                    odds = odds,
                    playid = payid,
                    remark = mark,
                     createTime=DateTime.Now,
                     updateTime=DateTime.Now

                };
                antList.Add(ant);
            }

            Add(antList);
        }

        /// <summary>
        /// 特肖,11.6,一肖 2.1，总肖3.6
        /// </summary>
        /// <param name="payid"></param>
        /// <param name="odds"></param>
        /// <param name="mark"></param>
        private void DataAdd正肖特肖一肖(int payid, decimal odds = 48.8M, string mark = "正肖")
        {
            string[] shuxiang = { "鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };
            IList<blast_lhc_antecode> antList = new List<blast_lhc_antecode>();
            string code = "";
            int i = 0;
            foreach (var item in shuxiang)
            {
                i++;
                if (i < 10)
                {
                    code = "0" + i;
                }
                else
                {
                    code = i + "";
                }
                blast_lhc_antecode ant = new blast_lhc_antecode()
                {
                    sort = i,
                    enable = true,
                    fag = code,
                    AnteCode = code,
                    displayName = item,
                    odds = odds,
                    playid = payid,
                    remark = mark,
                    createTime = DateTime.Now,
                    updateTime = DateTime.Now

                };
                antList.Add(ant);
            }
            Add(antList);
        }
        private void DataAdd总肖(int payid)
        {
            string[] shuxiang = { "234肖", "5肖", "6肖", "7肖", "总肖单", "总肖双" };
            string[] oddsarr = { "15", "3.07", "1.96", "5.4", "1.98", "1.85" };
            string[] fagsarr = { "234X", "5X", "6X", "7X", "DX", "SX" };
            IList<blast_lhc_antecode> antList = new List<blast_lhc_antecode>();
            string code = "";
            int i = 0;
            var index = 0;
            foreach (var item in shuxiang)
            {
                i++;
                if (i < 10)
                {
                    code = "0" + i;
                }
                else
                {
                    code = i + "";
                }
                blast_lhc_antecode ant = new blast_lhc_antecode()
                {
                    sort = i,
                    enable = true,
                    fag = fagsarr[index],
                    AnteCode = fagsarr[index],
                    displayName = item,
                    odds =decimal.Parse( oddsarr[index]),
                    playid = payid,
                    remark = "总肖",
                    createTime = DateTime.Now,
                    updateTime = DateTime.Now

                };
                antList.Add(ant);
                index++;
            }

            Add(antList);

        }

        private void Add(IList<blast_lhc_antecode> antList) {
            DB.Begin();
            try
            {
                foreach (var item in antList)
                {
                    DB.GetDal<blast_lhc_antecode>().Add(item);
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private void DataAdd三色波(int payid)
        {
            string[] shuxiang = { "红波", "蓝波", "绿波" };
            string[] oddsarr = { "2.76", "2.86", "2.86" };
            string[] fagsarr = { "Red", "Blu", "Green" };
            IList<blast_lhc_antecode> antList = new List<blast_lhc_antecode>();
            string code = "";
            int i = 0;
            var index = 0;
            foreach (var item in shuxiang)
            {
                i++;
                if (i < 10)
                {
                    code = "0" + i;
                }
                else
                {
                    code = i + "";
                }
                blast_lhc_antecode ant = new blast_lhc_antecode()
                {
                    sort = i,
                    enable =true,
                    fag = fagsarr[index],
                    AnteCode = fagsarr[index],
                    displayName = item,
                    odds = decimal.Parse(oddsarr[index]),
                    playid = payid,
                    remark = "三色波",
                    createTime = DateTime.Now,
                    updateTime = DateTime.Now

                };
                antList.Add(ant);
                index++;
            }

            Add(antList);

        }
        private void DataAdd半波(int payid)
        {
            string[] shuxiang = {
                                    "红单", "红双", "红大",
                                    "红小", "绿单", "绿双",
                                    "绿大", "绿小", "蓝单",
                                    "蓝双", "蓝大", "蓝小",
                                   };
            string[] oddsarr = {
                "5.58", "5.06", "6.5",
                "4.5", "5.56", "6.45",
                "5.58", "6.52", "5.58",
                "5.58", "5", "6.58",
            };
            string[] fagsarr = {
                        "RedDan", "RedSua", "RedDa",
                        "RedXiao", "GreenDan", "GreenSua",
                        "GreenDa", "GreenXiao", "BluDan",
                        "BluSua", "BluDa", "BluXiao",
            };
            IList<blast_lhc_antecode> antList = new List<blast_lhc_antecode>();
            string code = "";
            int i = 0;
            var index = 0;
            foreach (var item in shuxiang)
            {
                i++;
                if (i < 10)
                {
                    code = "0" + i;
                }
                else
                {
                    code = i + "";
                }
                blast_lhc_antecode ant = new blast_lhc_antecode()
                {
                    sort = i,
                    enable = true,
                    fag = fagsarr[index],
                    AnteCode = fagsarr[index],
                    displayName = item,
                    odds = decimal.Parse(oddsarr[index]),
                    playid = payid,
                    remark = "半波",
                    createTime = DateTime.Now,
                    updateTime = DateTime.Now

                };
                antList.Add(ant);
                index++;
            }

            Add(antList);

        }

        private void DataAdd半半波(int payid)
        {
            string[] shuxiang = {
                                    "红大单", "红大双", "红小单",
                                    "红小双", "绿大单", "绿大双",
                                    "绿小单", "绿小双", "蓝大单",
                                    "蓝大双", "蓝小单", "蓝小双",
                                   };
            string[] oddsarr = {
                "14.8", "11.12", "8.92",
                "8.92", "11.12", "11.12",
                "11.12", "14.82", "8.92",
                "11.12", "14.82", "11.12",
            };
            string[] fagsarr = {
                        "RedDaDan", "RedDaSua", "RedXiaoDan",
                        "RedXiaoSua", "GreenDaDan", "GreenDaSua",
                        "GreenXiaoDan", "GreenXiaoSua", "BluDaDan",
                        "BluDaSua", "BluXiaoDan", "BluXiaoSua",
            };
            IList<blast_lhc_antecode> antList = new List<blast_lhc_antecode>();
            string code = "";
            int i = 0;
            var index = 0;
            foreach (var item in shuxiang)
            {
                i++;
                if (i < 10)
                {
                    code = "0" + i;
                }
                else
                {
                    code = i + "";
                }
                blast_lhc_antecode ant = new blast_lhc_antecode()
                {
                    sort = i,
                    enable = true,
                    fag = fagsarr[index],
                    AnteCode = fagsarr[index],
                    displayName = item,
                    odds = decimal.Parse(oddsarr[index]),
                    playid = payid,
                    remark = "半半波",
                    createTime = DateTime.Now,
                    updateTime = DateTime.Now

                };
                antList.Add(ant);
                index++;
            }

            Add(antList);

        }

        private void DataAdd七波(int payid)
        {
            string[] shuxiang = {
                                    "红波", "蓝波", "绿波",
                                    "和局"
                                   };
            string[] oddsarr = {
                "2.65", "3", "3",
                "25"
            };
            string[] fagsarr = {
                        "Red", "Blu", "Green",
                        "HeJu"
            };
            IList<blast_lhc_antecode> antList = new List<blast_lhc_antecode>();
            string code = "";
            int i = 0;
            var index = 0;
            foreach (var item in shuxiang)
            {
                i++;
                if (i < 10)
                {
                    code = "0" + i;
                }
                else
                {
                    code = i + "";
                }
                blast_lhc_antecode ant = new blast_lhc_antecode()
                {
                    sort = i,
                    enable = true,
                    fag = fagsarr[index],
                    AnteCode = fagsarr[index],
                    displayName = item,
                    odds = decimal.Parse(oddsarr[index]),
                    playid = payid,
                    remark = "七色" +
                    "波",
                    createTime = DateTime.Now,
                    updateTime = DateTime.Now

                };
                antList.Add(ant);
                index++;
            }

            Add(antList);

        }


        public void ADDDATA() {
            var list = DB.CreateQuery<blast_played>().ToList<blast_played>();
          var played= list.Where(b=>b.name=="特码").FirstOrDefault();
           

            //DataAdd特码正码(played.playId);
            //DataAdd特码正码(list.Where(b => b.name == "正码").FirstOrDefault().playId,8.02M, "正码");
            Console.WriteLine("DataAdd特码正码");
            DataAdd两面(list.Where(b => b.name == "两面").FirstOrDefault().playId);
            ///// 特肖,11.6,一肖 2.1，总肖3.6
            DataAdd正肖特肖一肖(list.Where(b => b.name == "正肖").FirstOrDefault().playId);
            DataAdd正肖特肖一肖(list.Where(b => b.name == "特肖").FirstOrDefault().playId, 11.6M, "特肖");
            DataAdd正肖特肖一肖(list.Where(b => b.name == "一肖").FirstOrDefault().playId, 3.6M, "一肖");

            DataAdd总肖(list.Where(b => b.name == "总肖").FirstOrDefault().playId);
            DataAdd三色波(list.Where(b => b.name == "三色波").FirstOrDefault().playId);
            DataAdd半波(list.Where(b => b.name == "半波").FirstOrDefault().playId);
            DataAdd半半波(list.Where(b => b.name == "半半波").FirstOrDefault().playId);
            DataAdd七波(list.Where(b => b.name == "七色波").FirstOrDefault().playId);
            Console.WriteLine("DataAdd特码正码*********");


        }
    }
}
