using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.Common.Hk6
{

    public class XiaoModel {
        public int Index { get; set; }
        public string DisPlayName { get; set; }
        public string Name { get; set; }
        public List<string> CodeList { get; set; }


      

    }

    public class XiaoCollectionModel
    {
        /// <summary>
        /// 野兽
        /// </summary>
        public List<XiaoModel> YeShou { get; set; }
        public List<XiaoModel> JiaShou { get; set; }
        public List<XiaoModel> TianXiao { get; set; }
        public List<XiaoModel> DiXiao { get; set; }
        public List<XiaoModel> QianXiao { get; set; }
        public List<XiaoModel> HouXiao { get; set; }
    }
    /// <summary>
    /// 生肖计算
    /// </summary>
    public  class SXHelper
    {//

       // 红波 :01、02、07、08、12、13、18、19、23、24、29、30、34、35、40、45、46
            
        public static string[] RedBox = new string[] {
                      "01","02","07","08","12","13",
                      "18","19","23","24","29","30",
                      "34","35","40","45","46"
                    };

        //蓝波 :03、04、09、10、14、15、20、25、26、31、36、37、41、42、47、48
            
        public static string[] BluBox = new string[] {
                     "03","04","09","10","14",
                    "15","20","25","26","31",
                    "36","37","41","42","47","48"
                    };
        public static string[] GreenBox = new string[] {
            "05","06","11","16","17","21",
            "22","27","28","32","33","38",
            "39","43","44","49"

                    };
        //天肖
        // 
        // 牛（10 22 34 46）、兔（08 20 32 44）、龙（07 19 31 43）、
        // 马（05 17 29 41）、猴（03 15 27 39）、猪（12 24 36 48）
        //天肖 牛02,兔04,05龙,马07,猴09,猪12
        private int[] ttxiao = new int[] {
                        10, 22, 34, 46,
                        08 ,20, 32 ,44,
                        07 ,19 ,31, 43,
                        05 ,17 ,29 ,41,
                        03, 15 ,27 ,39,
                        12, 24 ,36 ,48
                    };
        //鼠（11 23 35 47）、虎（09 21 33 45）、蛇（06 18 30 42）、
        //羊（04 16 28 40）、鸡（02 14 26 38）、狗（01 13 25 37 49）
        //天肖 牛02,兔04,05龙,马07,猴09,猪12
        //地肖 鼠01,虎03,06蛇,羊08,鸡10,狗11
        private int[] tdxiao = new int[] {
                        11 ,23 ,35,47,
                        09, 21 ,33, 45,
                        06 ,18, 30 ,42,
                        04 ,16 ,28, 40,
                        02,14 ,26, 38,
                        01 ,13, 25, 37 ,49
                    };
        private static string  shuxiang()
        {
            string[] shuxiang = { "鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };
            string[] shuCode =  { "1",  "2",  "3",   "4",  "5",  "6",  "7",  "8",  "9", "10", "11", "12" };
            int tmp =DateTime.Now.Year - 2008;
            if (DateTime.Now.Year < 2008)
            {
                // Console.WriteLine(shuxiang[tmp % 12 + 12]);
                return shuCode[tmp % 12 + 12];
            }
            else
            {
                // Console.WriteLine(shuxiang[tmp % 12]);
                return shuCode[tmp % 12];
            }
        }
        private static int SCode(int code)
        {
           int t=int.Parse( shuxiang());
            int scode = 1; 
            if (code > t)
            {
                 scode = (code - t + 1);
            }
            else if (code < t)
            {
                  scode = 12 - t+code+1;
            }
            return scode;
        }

        public static List<string> ScodeArr(int code) {
           int scode= SCode(code);
            List<string> list = new List<string>();
            if (scode >= 10)
            {
                list.Add(scode.ToString());
            }
            else {
                list.Add("0"+scode.ToString());
            }
           
            int temp =scode;
            while (true)
            {
                temp = temp + 12;
                if (temp <= 49)
                {
                    list.Add(temp.ToString());
                }
                else {
                    break;
                }
            }
            temp = scode;
            while (true)
            {
                temp = temp - 12;
                if (temp >=1)
                {
                    if (temp >= 10)
                    {
                        list.Add(temp.ToString());
                    }
                    else {
                        list.Add("0" + temp.ToString());
                    }
                   
                }
                else
                {
                    break;
                }
            }

            return list;

        }

        public static List<XiaoModel> XiaoCollection12()
        {
            List<XiaoModel> XM = new List<XiaoModel>();
            for (int i = 1; i < 13; i++)
            {
                XiaoModel xl= new XiaoModel();
                xl.Index = i;
                xl.CodeList=ScodeArr(i);
                XM.Add(xl);
            }
            return XM;
        }
            public static XiaoCollectionModel XiaoCollection() {

            XiaoCollectionModel m = new XiaoCollectionModel();
            //天肖 牛02,兔04,05龙,马07,猴09,猪12
            //地肖 鼠01,虎03,06蛇,羊08,鸡10,狗11

            var tlist = new List<XiaoModel>();
            var dlist = new List<XiaoModel>();
            var yshou=new List<XiaoModel>();
            var jialist = new List<XiaoModel>();
            var qianlist = new List<XiaoModel>();
            var houlist = new List<XiaoModel>();
            #region txiao
            tlist.Add(new XiaoModel() {
                Name = "02",
                DisPlayName= "牛",
                CodeList = ScodeArr(2)
            });
            tlist.Add(new XiaoModel()
            {
                Name = "04",
                DisPlayName = "兔",
                CodeList = ScodeArr(4)
            });
            tlist.Add(new XiaoModel()
            {

                Name = "05",
                DisPlayName = "龙",
                CodeList = ScodeArr(5)
            });
            tlist.Add(new XiaoModel()
            {
                Name = "07",
                DisPlayName = "马",
                CodeList = ScodeArr(7)
            });
            tlist.Add(new XiaoModel()
            {
                Name = "09",
                DisPlayName = "猴",
                CodeList = ScodeArr(9)
            });
            tlist.Add(new XiaoModel()
            {
                Name = "12",
                DisPlayName = "猪",
                CodeList = ScodeArr(12)
            });
            #endregion
            #region dxiao
            dlist.Add(new XiaoModel()
            {
                Name = "01",
                DisPlayName = "鼠01",
                CodeList = ScodeArr(1)
            });
            dlist.Add(new XiaoModel()
            {
                Name = "03",
                DisPlayName = "虎03",
                CodeList = ScodeArr(3)
            });
            dlist.Add(new XiaoModel()
            {
                Name = "06",
                DisPlayName = "06蛇",
                CodeList = ScodeArr(6)
            });
            dlist.Add(new XiaoModel()
            {

                Name = "08",
                DisPlayName = "羊08",
                CodeList = ScodeArr(8)
            });
            dlist.Add(new XiaoModel()
            {
                Name = "10",
                DisPlayName = "鸡10",
                CodeList = ScodeArr(10)
            });
            dlist.Add(new XiaoModel()
            {
                Name = "11",
                DisPlayName = "狗11",
                CodeList = ScodeArr(11)
            });

            #endregion

            #region yshou
            yshou.Add(new XiaoModel()
            {
                Name = "01",
                DisPlayName = "鼠01",
                CodeList = ScodeArr(1)
            });
            yshou.Add(new XiaoModel()
            {
                Name = "03",
                DisPlayName = "虎03",
                CodeList = ScodeArr(3)
            });
            yshou.Add(new XiaoModel()
            {
                Name = "04",
                DisPlayName = "兔04",
                CodeList = ScodeArr(4)
            });
            yshou.Add(new XiaoModel()
            {

                Name = "05",
                DisPlayName = "05龙",
                CodeList = ScodeArr(5)
            });
            yshou.Add(new XiaoModel()
            {
                Name = "06",
                DisPlayName = "06蛇",
                CodeList = ScodeArr(6)
            });
            yshou.Add(new XiaoModel()
            {
                Name = "09",
                DisPlayName = "猴09",
                CodeList = ScodeArr(9)
            });

            #endregion

            #region jialist
            jialist.Add(new XiaoModel()
            {
                Name = "02",
                DisPlayName = "牛02",
                CodeList = ScodeArr(2)
            });
            jialist.Add(new XiaoModel()
            {
                Name = "07",
                DisPlayName = "马07",
                CodeList = ScodeArr(7)
            });
            jialist.Add(new XiaoModel()
            {
                Name = "羊08",
                DisPlayName = "羊08",
                CodeList = ScodeArr(8)
            });
            yshou.Add(new XiaoModel()
            {

                Name = "鸡10",
                DisPlayName = "鸡10",
                CodeList = ScodeArr(10)
            });
            jialist.Add(new XiaoModel()
            {
                Name = "狗11",
                DisPlayName = "狗11",
                CodeList = ScodeArr(11)
            });
            jialist.Add(new XiaoModel()
            {
                Name = "猪12",
                DisPlayName = "猪12",
                CodeList = ScodeArr(12)
            });

            #endregion

            #region qianlist
            qianlist.Add(new XiaoModel()
            {
                Name = "01",
                DisPlayName = "鼠01",
                CodeList = ScodeArr(1)
            });
            qianlist.Add(new XiaoModel()
            {
                Name = "牛02",
                DisPlayName = "牛02",
                CodeList = ScodeArr(2)
            });
            qianlist.Add(new XiaoModel()
            {
                Name = "虎03",
                DisPlayName = "虎03",
                CodeList = ScodeArr(3)
            });
            qianlist.Add(new XiaoModel()
            {

                Name = "兔04",
                DisPlayName = "兔04",
                CodeList = ScodeArr(4)
            });
            qianlist.Add(new XiaoModel()
            {
                Name = "05龙",
                DisPlayName = "05龙",
                CodeList = ScodeArr(5)
            });
            qianlist.Add(new XiaoModel()
            {
                Name = "06蛇",
                DisPlayName = "06蛇",
                CodeList = ScodeArr(6)
            });

            #endregion
            #region houlist
            houlist.Add(new XiaoModel()
            {
                Name = "马07",
                DisPlayName = "马07",
                CodeList = ScodeArr(7)
            });
            houlist.Add(new XiaoModel()
            {
                Name = "羊08",
                DisPlayName = "羊08",
                CodeList = ScodeArr(8)
            });
            houlist.Add(new XiaoModel()
            {
                Name = "猴09",
                DisPlayName = "猴09",
                CodeList = ScodeArr(9)
            });
           
            houlist.Add(new XiaoModel()
            {
                Name = "鸡10",
                DisPlayName = "鸡10",
                CodeList = ScodeArr(10)
            });
            houlist.Add(new XiaoModel()
            {
                Name = "狗11",
                DisPlayName = "狗11",
                CodeList = ScodeArr(11)
            });
            houlist.Add(new XiaoModel()
            {

                Name = "猪12",
                DisPlayName = "猪12",
                CodeList = ScodeArr(12)
            });
            #endregion
            #region
            m.DiXiao = dlist;
            m.TianXiao = tlist;
            m.JiaShou = jialist;
            m.QianXiao = qianlist;
            m.HouXiao = houlist;
            m.YeShou = yshou;
            #endregion
            return m;
        }
    }
}
