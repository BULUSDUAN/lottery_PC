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
        private IDbProvider nDB = null;
        private IDbProvider xDB = null;
        public DataInserterHelper(IDbProvider _DB, IDbProvider _D) {
            DB = _DB;
            nDB = _DB;
            xDB = _D;
        }

        public   void DataAddPK期号() {
            //var xDB = DB.Init("MySql.Default1");
            //var nDB = DB.Init("MySql.Default");
            var list= xDB.CreateQuery<xingblast_data_time>().Where(b=>b.typeid==20).ToList();
            IList<blast_data_time> list2 = new List<blast_data_time>();
            foreach (var item in list)
            {
                blast_data_time bt = new blast_data_time() {
                     //actionNo=item.actionNo,
                     // actionhours=item.actionTime,
                     //  stophours=item.stopTime,
                     //   typeid=item.typeid,
                     //    isOpen=false,
                     //     winNum=""


                };
                list2.Add(bt);

            }
            foreach (var item in list2)
            {
                string value = item.actionTime;
               var p= nDB.CreateQuery<blast_data_time>().Where(b=>b.actionTime == value).FirstOrDefault();
                if (p==null)
                {
                    nDB.GetDal<blast_data_time>().Add(item);

                }
            }
        }

        /// <summary>
        /// pk 分组录入
        /// </summary>
        private void DataAddPK玩法()
        {
            IList<dynamic> list0 = new List<dynamic>();

            IList<dynamic> list1 = new List<dynamic>();

            list0.Add(new { name="前一",typeid=2,groupId=16,playId=58 });
            list0.Add(new { name = "前二", typeid = 2, groupId = 17, playId = 59 });
            list0.Add(new { name = "前三", typeid = 2, groupId = 18, playId = 60 });

            list0.Add(new { name = "第1~5名", typeid = 2, groupId = 19, playId = 61 });
            list0.Add(new { name = "第6~10名", typeid = 2, groupId = 19, playId = 62 });
            list0.Add(new { name = "定位胆", typeid = 2, groupId = 19, playId = 63 });

            list0.Add(new { name = "和值", typeid = 2, groupId = 20, playId = 64 });

            list0.Add(new { name = "冠军", typeid = 2, groupId = 21, playId = 65 });
            list0.Add(new { name = "亚军", typeid = 2, groupId = 21, playId = 66 });
            list0.Add(new { name = "季军", typeid = 2, groupId = 21, playId = 67 });
            list0.Add(new { name = "第西名", typeid = 2, groupId = 21, playId = 68 });
            list0.Add(new { name = "第五名", typeid = 2, groupId = 21, playId = 69 });


            list0.Add(new { name = "冠军", typeid = 2, groupId = 22, playId = 70 });
            list0.Add(new { name = "亚军", typeid = 2, groupId = 22, playId = 71 });
            list0.Add(new { name = "季军", typeid = 2, groupId = 22, playId = 72 });

            list0.Add(new { name = "大小冠军", typeid = 2, groupId = 23, playId = 73 });
            list0.Add(new { name = "大小亚军", typeid = 2, groupId = 23, playId = 74 });
            list0.Add(new { name = "大小季军", typeid = 2, groupId = 23, playId = 75 });


            list0.Add(new { name = "单双冠军", typeid = 2, groupId = 23, playId = 76 });
            list0.Add(new { name = "单双亚军", typeid = 2, groupId = 23, playId = 77 });
            list0.Add(new { name = "单双季军", typeid = 2, groupId = 23, playId = 78 });

            list0.Add(new { name = "冠亚和", typeid = 2, groupId = 23, playId = 79 });


          //  list1.Add(new { name = "01", typeid = 2, groupId = 16, playId = 58 });

            //var xDB = DB.Init("MySql.Default1");
            var nDB = DB.Init("MySql.Default");
            int index = 0;
            try
            {

                nDB.Begin();
            foreach (var item in list0)
            {
                index++;
                blast_played bp = new blast_played()
                {
                    groupId = item.groupId,
                    name = item.name,
                    enable = true,
                    simpleInfo="",
                    info="",
                    example="",
                    Odds = 0,
                    playId = item.playId,
                    sort = index,
                     typeid=item.typeid
                      
                };
                nDB.GetDal<blast_played>().Add(bp);
                }

              
                nDB.Commit();
            }
            catch( Exception ex)
            {
                Console.WriteLine(ex.ToString());
                nDB.Rollback();


            }
        }

        public void PKAnteCodeData() {
          //  DataAddPK玩法();
            //前-定位胆
          //  DataAddPK前();

           // DataAddPKAnteCode冠亚和();

            //DataAddPKAnteCode龙虎();

           // DataAddPKAnteCode五行();

           // DataAddPKAnteCode大小单双();


          //  DataAddPKAnteCode冠亚和1();
        }
        private void DataAddPK前() {

            List<int> list = new List<int>();
            blast_lhc_antecode antecode1 = new blast_lhc_antecode() {
                remark = "前一",
                odds = 9.8m,
                playid = 58
            };
            //list.Add(1);
            //DataAddPKAnteCode前2(list, antecode1);

            antecode1 = new blast_lhc_antecode()
            {
                remark = "前二",
                odds = 88.2M,
                playid = 59
            };
            list.Clear();
            list.Add(1);
            list.Add(2);
            DataAddPKAnteCode前2(list, antecode1);


             antecode1 = new blast_lhc_antecode()
            {
                remark = "前三",
                odds = 705.6M,
                playid = 60
            };
           
            list.Clear();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            DataAddPKAnteCode前2(list, antecode1);


            antecode1 = new blast_lhc_antecode()
            {
                remark = "第1~5名",
                odds = 9.8M,
                playid = 61
            };

            list.Clear();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);
            list.Add(5);
            DataAddPKAnteCode前2(list, antecode1);



            antecode1 = new blast_lhc_antecode()
            {
                remark = "第6~10名",
                odds = 9.8M,
                playid = 62
            };

            list.Clear();
            list.Add(6);
            list.Add(7);
            list.Add(8);
            list.Add(9);
            list.Add(10);
            DataAddPKAnteCode前2(list, antecode1);


            antecode1 = new blast_lhc_antecode()
            {
                remark = "定位胆",
                odds = 9.8M,
                playid = 63
            };

            list.Clear();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);
            list.Add(5);
            list.Add(6);
            list.Add(7);
            list.Add(8);
            list.Add(9);
            list.Add(10);
            DataAddPKAnteCode前2(list, antecode1);


        }
        private void DataAddPKAnteCode前(blast_lhc_antecode antecode)
        {
            var nDB = DB.Init("MySql.Default");
            try
            {
                for (int i = 1; i <= 10; i++)
                {
                    string temp = "0" + i;
                    if (i >= 10)
                    {
                        temp = i + "";
                    }
                    var P = new blast_lhc_antecode()
                    {
                        AnteCode = temp,
                        displayName = temp,
                        odds = antecode.odds,
                        playid = antecode.playid,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort = i,
                        enable = true,
                        remark = antecode.remark

                    };
                    nDB.GetDal<blast_lhc_antecode>().Add(P);
                }
              
                nDB.Commit();
            }
            catch (Exception)
            {

               
            }
           

        }

        private void DataAddPKAnteCode前2(List<int> list,blast_lhc_antecode antecode)
        {
            var nDB = DB.Init("MySql.Default");
            try
            {
                nDB.Begin();
                foreach (var item in list)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        string temp = "0" + i;
                        if (i >= 10)
                        {
                            temp = i + "";
                        }
                        var P = new blast_lhc_antecode()
                        {
                            AnteCode = item+"_" + temp,
                            displayName = temp,
                            odds = antecode.odds,
                            playid = antecode.playid,
                            createTime = DateTime.Now,
                            updateTime = DateTime.Now,
                            sort = i,
                            fag= temp,
                            cateNum =item,
                            enable = true,
                            remark = antecode.remark

                        };
                        nDB.GetDal<blast_lhc_antecode>().Add(P);
                    }
                }
              

                nDB.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                nDB.Rollback();
            }


        }


        private void DataAddPKAnteCode冠亚和()
        {
            var nDB = DB.Init("MySql.Default");
            try
            {
                List<dynamic> list = new List<dynamic>();
              //  int index = 0;
                nDB.Begin();
                //foreach (var item in list)
                //{
                for (int i = 3; i <= 19; i++)
                    {
                        string temp = "0" + i;
                     
                        var P = new blast_lhc_antecode()
                        {
                            AnteCode = temp,
                            displayName = temp,
                            odds = 14.7M,
                            playid = 64,
                            createTime = DateTime.Now,
                            updateTime = DateTime.Now,
                            sort = i,
                            cateNum = 0,
                            enable = true,
                            remark = "冠亚和"

                        };
                        nDB.GetDal<blast_lhc_antecode>().Add(P);
                    }
              //  }


                nDB.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                nDB.Rollback();

            }


        }

        private void DataAddPKAnteCode龙虎()
        {
            var nDB = DB.Init("MySql.Default");
            try
            {
                List<dynamic> list0 = new List<dynamic>() ;

                list0.Add(new { name = "冠军", typeid = 2, groupId = 21, playId = 65 });
                list0.Add(new { name = "亚军", typeid = 2, groupId = 21, playId = 66 });
                list0.Add(new { name = "季军", typeid = 2, groupId = 21, playId = 67 });
                list0.Add(new { name = "第西名", typeid = 2, groupId = 21, playId = 68 });
                list0.Add(new { name = "第五名", typeid = 2, groupId = 21, playId = 69 });
                nDB.Begin();

                foreach (var item in list0)
                {
                    var P = new blast_lhc_antecode()
                    {
                        AnteCode = "01",
                        displayName = "龙",
                        odds = 1.98M,
                        playid = item.playId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort = 0,
                        cateNum = 0,
                        enable = true,
                        remark = item.name

                    };
                    var P1 = new blast_lhc_antecode()
                    {
                        AnteCode = "02",
                        displayName = "虎",
                        odds = 1.98M,
                        playid = item.playId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort = 1,
                        cateNum = 0,
                        enable = true,
                        remark = item.name

                    };
                    nDB.GetDal<blast_lhc_antecode>().Add(P);
                    nDB.GetDal<blast_lhc_antecode>().Add(P1);
                }


                nDB.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                nDB.Rollback();

            }


        }

        private void DataAddPKAnteCode五行()
        {
            var nDB = DB.Init("MySql.Default");
            try
            {
                List<dynamic> list0 = new List<dynamic>();

                list0.Add(new { name = "冠军", typeid = 2, groupId = 22, playId = 70 });
                list0.Add(new { name = "亚军", typeid = 2, groupId = 22, playId = 71 });
                list0.Add(new { name = "季军", typeid = 2, groupId = 22, playId = 72 });
                nDB.Begin();
                foreach (var item in list0)
                {
                    var P = new blast_lhc_antecode()
                    {
                        AnteCode = "01",
                        displayName = "金",
                        odds = 4.85M,
                        playid = item.playId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort = 0,
                        cateNum = 0,
                        enable = true,
                        remark = item.name

                    };
                    var P1 = new blast_lhc_antecode()
                    {
                        AnteCode = "02",
                        displayName = "木",
                        odds = 1.98M,
                        playid = item.playId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort = 1,
                        cateNum = 0,
                        enable = true,
                        remark = item.name

                    };
                    var P2 = new blast_lhc_antecode()
                    {
                        AnteCode = "03",
                        displayName = "水",
                        odds = 1.98M,
                        playid = item.playId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort = 2,
                        cateNum = 0,
                        enable = true,
                        remark = item.name

                    };
                    var P3 = new blast_lhc_antecode()
                    {
                        AnteCode = "04",
                        displayName = "火",
                        odds = 1.98M,
                        playid = item.playId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort = 3,
                        cateNum = 0,
                        enable = true,
                        remark = item.name

                    };
                    var P4 = new blast_lhc_antecode()
                    {
                        AnteCode = "05",
                        displayName = "土",
                        odds = 1.98M,
                        playid = item.playId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort =4,
                        cateNum = 0,
                        enable = true,
                        remark = item.name

                    };
                    nDB.GetDal<blast_lhc_antecode>().Add(P);
                    nDB.GetDal<blast_lhc_antecode>().Add(P1);
                    nDB.GetDal<blast_lhc_antecode>().Add(P2);
                    nDB.GetDal<blast_lhc_antecode>().Add(P3);
                    nDB.GetDal<blast_lhc_antecode>().Add(P4);
                }


                nDB.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                nDB.Rollback();

            }


        }

        private void DataAddPKAnteCode大小单双()
        {
            var nDB = DB.Init("MySql.Default");
            try
            {
                List<dynamic> list0 = new List<dynamic>();
                List<dynamic> list1 = new List<dynamic>();

                list0.Add(new { name = "大小冠军", typeid = 2, groupId = 23, playId = 73 });
                list0.Add(new { name = "大小亚军", typeid = 2, groupId = 23, playId = 74 });
                list0.Add(new { name = "大小季军", typeid = 2, groupId = 23, playId = 75 });


                list1.Add(new { name = "单双冠军", typeid = 2, groupId = 23, playId = 76 });
                list1.Add(new { name = "单双亚军", typeid = 2, groupId = 23, playId = 77 });
                list1.Add(new { name = "单双季军", typeid = 2, groupId = 23, playId = 78 });

                nDB.Begin();
                foreach (var item in list0)
                {
                    var P = new blast_lhc_antecode()
                    {
                        AnteCode = "01",
                        displayName = "大",
                        odds = 1.98M,
                        playid = item.playId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort = 0,
                        cateNum = 0,
                        enable = true,
                        remark = item.name

                    };
                    var P1 = new blast_lhc_antecode()
                    {
                        AnteCode = "02",
                        displayName = "小",
                        odds = 1.98M,
                        playid = item.playId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort = 1,
                        cateNum = 0,
                        enable = true,
                        remark = item.name

                    };
                    nDB.GetDal<blast_lhc_antecode>().Add(P);
                    nDB.GetDal<blast_lhc_antecode>().Add(P1);
                }
                foreach (var item in list1)
                {
                    var P = new blast_lhc_antecode()
                    {
                        AnteCode = "01",
                        displayName = "单",
                        odds = 1.96M,
                        playid = item.playId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort = 0,
                        cateNum = 0,
                        enable = true,
                        remark = item.name

                    };
                    var P1 = new blast_lhc_antecode()
                    {
                        AnteCode = "02",
                        displayName = "双",
                        odds = 1.96M,
                        playid = item.playId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort = 1,
                        cateNum = 0,
                        enable = true,
                        remark = item.name

                    };
                    nDB.GetDal<blast_lhc_antecode>().Add(P);
                    nDB.GetDal<blast_lhc_antecode>().Add(P1);
                }


                nDB.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                nDB.Rollback();

            }


        }


        private void DataAddPKAnteCode冠亚和1()
        {
            var nDB = DB.Init("MySql.Default");
            try
            {
                List<dynamic> list0 = new List<dynamic>();

                list0.Add(new { name = "冠亚和", typeid = 2, groupId = 23, playId = 79 });

                nDB.Begin();
                foreach (var item in list0)
                {
                    var P = new blast_lhc_antecode()
                    {
                        AnteCode = "01",
                        displayName = "大",
                        odds = 1.98M,
                        playid = item.playId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort = 0,
                        cateNum = 0,
                        enable = true,
                        remark = item.name

                    };
                    var P1 = new blast_lhc_antecode()
                    {
                        AnteCode = "02",
                        displayName = "小",
                        odds = 1.98M,
                        playid = item.playId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort = 1,
                        cateNum = 0,
                        enable = true,
                        remark = item.name

                    };
                    var P2 = new blast_lhc_antecode()
                    {
                        AnteCode = "03",
                        displayName = "单",
                        odds = 1.98M,
                        playid = item.playId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort = 2,
                        cateNum = 0,
                        enable = true,
                        remark = item.name

                    };
                    var P3 = new blast_lhc_antecode()
                    {
                        AnteCode = "04",
                        displayName = "双",
                        odds = 1.98M,
                        playid = item.playId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        sort = 3,
                        cateNum = 0,
                        enable = true,
                        remark = item.name

                    };
                    nDB.GetDal<blast_lhc_antecode>().Add(P);
                    nDB.GetDal<blast_lhc_antecode>().Add(P1);
                    nDB.GetDal<blast_lhc_antecode>().Add(P2);
                    nDB.GetDal<blast_lhc_antecode>().Add(P3);
                }


                nDB.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                nDB.Rollback();

            }


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
