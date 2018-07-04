using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.ORM.Helper.WinNumber.Manage;
using EntityModel;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;

namespace KaSon.FrameWork.ORM.Helper.WinNumber
{
    public class LotteryDataBusiness_DF6_1 : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get
            {
                return "DF6J1";
            }
        }

        public void ImportWinNumber(string issuseNumber, string winNumber)
        {
            if (string.IsNullOrEmpty(issuseNumber)) return;
            if (string.IsNullOrEmpty(winNumber)) return;

            //var msg = string.Empty;
            //AnalyzerFactory.GetWinNumberAnalyzer(this.CurrentGameCode).CheckWinNumber(winNumber, out msg);
            //if (!string.IsNullOrEmpty(msg))
            //    throw new Exception(msg);

            //开启事务
            using (LottertDataDB)
            {
                LottertDataDB.Begin();

                this.ClearGameChartCache("QueryDF6_1_DXZS");
                this.ClearGameChartCache("QueryDF6_1_HZZS");
                this.ClearGameChartCache("QueryDF6_1_JBZS");
                this.ClearGameChartCache("QueryDF6_1_JOZS");
                this.ClearGameChartCache("QueryDF6_1_KDZS");
                this.ClearGameChartCache("QueryDF6_1_ZHZS");
                this.ClearNewWinNumberCache("QueryDF6_1_GameWinNumber");

                AddDF6_1_JBZS(issuseNumber, winNumber);
                AddDF6_1_HZZS(issuseNumber, winNumber);
                AddDF6_1_DXZS(issuseNumber, winNumber);
                AddDF6_1_JOZS(issuseNumber, winNumber);
                AddDF6_1_ZHZS(issuseNumber, winNumber);
                AddDF6_1_KDZS(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }

        public void Add_GameWinNumber(string issuseNumber, string winNumber)
        {
            new KJGameIssuseBusiness().IssusePrize(this.CurrentGameCode, issuseNumber, winNumber);
            var manager = new DF6_1_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddDF6_1_GameWinNumber(new DF6_1_GameWinNumber
            {
                GameCode = this.CurrentGameCode,
                IssuseNumber = issuseNumber,
                WinNumber = winNumber,
                CreateTime = DateTime.Now,
            });
        }
        /// <summary>
        /// 基本走势
        /// </summary>
        private void AddDF6_1_JBZS(string issuseNumber, string winNumber)
        {
            var manager = new DF6_1_JBZSManager();
            var issuse = manager.QueryDF6_1_JBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var last = manager.QueryLastDF6_1_JBZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<DF6_1_JBZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_"))
                {
                    var order = p.Name.Replace("Red_", string.Empty);
                    lastValue = winRed[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Red1_"))
                {
                    var order = p.Name.Replace("Red1_", string.Empty);
                    lastValue = winRed[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Red2_"))
                {
                    var order = p.Name.Replace("Red2_", string.Empty);
                    lastValue = winRed[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Red3_"))
                {
                    var order = p.Name.Replace("Red3_", string.Empty);
                    lastValue = winRed[3] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Red4_"))
                {
                    var order = p.Name.Replace("Red4_", string.Empty);
                    lastValue = winRed[4] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Red5_"))
                {
                    var order = p.Name.Replace("Red5_", string.Empty);
                    lastValue = winRed[5] == order ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddDF6_1_JBZS(entity);
        }
        /// <summary>
        /// 和值走势
        /// </summary>
        private void AddDF6_1_HZZS(string issuseNumber, string winNumber)
        {
            var manager = new DF6_1_HZZSManager();
            var issuse = manager.QueryDF6_1_HZZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hezhi = Convert.ToInt32(winRed[0]) + Convert.ToInt32(winRed[1]) + Convert.ToInt32(winRed[2]) + Convert.ToInt32(winRed[3]) + Convert.ToInt32(winRed[4]) + Convert.ToInt32(winRed[5]);
            var DaXiao = Convert.ToInt32(hezhi) > 30 ? "大" : "小";
            var JO = Convert.ToInt32(hezhi) % 2 == 0 ? "偶" : "奇";
            var ZH = (Convert.ToInt32(hezhi) == 1 || Convert.ToInt32(hezhi) == 2 || Convert.ToInt32(hezhi) == 3 || Convert.ToInt32(hezhi) == 5 || Convert.ToInt32(hezhi) == 7 || Convert.ToInt32(hezhi) == 11 || Convert.ToInt32(hezhi) == 13 || Convert.ToInt32(hezhi) == 17 || Convert.ToInt32(hezhi) == 19 || Convert.ToInt32(hezhi) == 23 || Convert.ToInt32(hezhi) == 29 || Convert.ToInt32(hezhi) == 31 || Convert.ToInt32(hezhi) == 37 || Convert.ToInt32(hezhi) == 41 || Convert.ToInt32(hezhi) == 43 || Convert.ToInt32(hezhi) == 47 || Convert.ToInt32(hezhi) == 53) ? "质" : "合";
            var C3Y = Convert.ToInt32(hezhi) % 3;

            var last = manager.QueryLastDF6_1_HZZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("HeZhi", hezhi);
            var entity = this.CreateNewEntity<DF6_1_HZZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Red_"))
                {
                    var order = p.Name.Replace("Red_", string.Empty);
                    if ((order == "06" && (hezhi >= 0 && hezhi <= 6)) || (order == "712" && (hezhi >= 7 && hezhi <= 12)) || (order == "1318" && (hezhi >= 13 && hezhi <= 18)) || (order == "1924" && (hezhi >= 19 && hezhi <= 24)) || (order == "2530" && (hezhi >= 25 && hezhi <= 30)) || (order == "3136" && (hezhi >= 31 && hezhi <= 36)) || (order == "3742" && (hezhi >= 37 && hezhi <= 42)) || (order == "4348" && (hezhi >= 43 && hezhi <= 48)) || (order == "4954" && (hezhi >= 49 && hezhi <= 54)))
                        lastValue = 0;
                }
                if (p.Name.StartsWith("C3_"))
                {
                    var order = p.Name.Replace("C3_", string.Empty);
                    lastValue = C3Y == Convert.ToInt32(order) ? 0 : lastValue;
                }
                if (p.Name == "JO_J")
                {
                    lastValue = JO == "奇" ? 0 : lastValue;
                }
                if (p.Name == "JO_O")
                {
                    lastValue = JO == "偶" ? 0 : lastValue;
                }
                if (p.Name == "DX_D")
                {
                    lastValue = DaXiao == "大" ? 0 : lastValue;
                }
                if (p.Name == "DX_X")
                {
                    lastValue = DaXiao == "小" ? 0 : lastValue;
                }
                if (p.Name == "ZH_Z")
                {
                    lastValue = ZH == "质" ? 0 : lastValue;
                }
                if (p.Name == "ZH_H")
                {
                    lastValue = ZH == "合" ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddDF6_1_HZZS(entity);
        }
        /// <summary>
        /// 大小走势
        /// </summary>
        private void AddDF6_1_DXZS(string issuseNumber, string winNumber)
        {
            var manager = new DF6_1_DXZSManager();
            var issuse = manager.QueryDF6_1_DXZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string DX_Proportion = string.Empty;
            int d = 0;
            int x = 0;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) > 4)
                {
                    d++;
                }
                else
                {
                    x++;
                }
            }
            DX_Proportion = string.Format("{0}:{1}", d, x);
            var last = manager.QueryLastDF6_1_DXZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("DX_Proportion", DX_Proportion);
            var entity = this.CreateNewEntity<DF6_1_DXZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("O_DX_"))
                {
                    var order = p.Name.Replace("O_DX_Proportion", string.Empty);
                    lastValue = DX_Proportion.Replace(":", "") == order ? 0 : lastValue;
                }
                if (p.Name == "DX_D")
                {
                    lastValue = Convert.ToInt32(winRed[0]) > 4 ? 0 : lastValue;
                }
                if (p.Name == "DX_X")
                {
                    lastValue = Convert.ToInt32(winRed[0]) > 4 ? lastValue : 0;
                }
                if (p.Name == "DX1_D")
                {
                    lastValue = Convert.ToInt32(winRed[1]) > 4 ? 0 : lastValue;
                }
                if (p.Name == "DX1_X")
                {
                    lastValue = Convert.ToInt32(winRed[1]) > 4 ? lastValue : 0;
                }
                if (p.Name == "DX2_D")
                {
                    lastValue = Convert.ToInt32(winRed[2]) > 4 ? 0 : lastValue;
                }
                if (p.Name == "DX2_X")
                {
                    lastValue = Convert.ToInt32(winRed[2]) > 4 ? lastValue : 0;
                }
                if (p.Name == "DX3_D")
                {
                    lastValue = Convert.ToInt32(winRed[3]) > 4 ? 0 : lastValue;
                }
                if (p.Name == "DX3_X")
                {
                    lastValue = Convert.ToInt32(winRed[3]) > 4 ? lastValue : 0;
                }
                if (p.Name == "DX4_D")
                {
                    lastValue = Convert.ToInt32(winRed[4]) > 4 ? 0 : lastValue;
                }
                if (p.Name == "DX4_X")
                {
                    lastValue = Convert.ToInt32(winRed[4]) > 4 ? lastValue : 0;
                }
                if (p.Name == "DX5_D")
                {
                    lastValue = Convert.ToInt32(winRed[5]) > 4 ? 0 : lastValue;
                }
                if (p.Name == "DX5_X")
                {
                    lastValue = Convert.ToInt32(winRed[5]) > 4 ? lastValue : 0;
                }
                return lastValue;
            });

            manager.AddDF6_1_DXZS(entity);
        }

        private void AddDF6_1_JOZS(string issuseNumber, string winNumber)
        {
            var manager = new DF6_1_JOZSManager();
            var issuse = manager.QueryDF6_1_JOZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string JO_Proportion = string.Empty;
            int J = 0;
            int O = 0;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) % 2 == 0)
                {
                    O++;
                }
                else
                {
                    J++;
                }
            }
            JO_Proportion = string.Format("{0}:{1}", J, O);
            var last = manager.QueryLastDF6_1_JOZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("JO_Proportion", JO_Proportion);
            var entity = this.CreateNewEntity<DF6_1_JOZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("O_JO_"))
                {
                    var order = p.Name.Replace("O_JO_Proportion", string.Empty);
                    lastValue = JO_Proportion.Replace(":", "") == order ? 0 : lastValue;
                }
                if (p.Name == "JO_J")
                {
                    lastValue = Convert.ToInt32(winRed[0]) % 2 != 0 ? 0 : lastValue;
                }
                if (p.Name == "JO_O")
                {
                    lastValue = Convert.ToInt32(winRed[0]) % 2 != 0 ? lastValue : 0;
                }
                if (p.Name == "JO1_J")
                {
                    lastValue = Convert.ToInt32(winRed[1]) % 2 != 0 ? 0 : lastValue;
                }
                if (p.Name == "JO1_O")
                {
                    lastValue = Convert.ToInt32(winRed[1]) % 2 != 0 ? lastValue : 0;
                }
                if (p.Name == "JO2_J")
                {
                    lastValue = Convert.ToInt32(winRed[2]) % 2 != 0 ? 0 : lastValue;
                }
                if (p.Name == "JO2_O")
                {
                    lastValue = Convert.ToInt32(winRed[2]) % 2 != 0 ? lastValue : 0;
                }
                if (p.Name == "JO3_J")
                {
                    lastValue = Convert.ToInt32(winRed[3]) % 2 != 0 ? 0 : lastValue;
                }
                if (p.Name == "JO3_O")
                {
                    lastValue = Convert.ToInt32(winRed[3]) % 2 != 0 ? lastValue : 0;
                }
                if (p.Name == "JO4_J")
                {
                    lastValue = Convert.ToInt32(winRed[4]) % 2 != 0 ? 0 : lastValue;
                }
                if (p.Name == "JO4_O")
                {
                    lastValue = Convert.ToInt32(winRed[4]) % 2 != 0 ? lastValue : 0;
                }
                if (p.Name == "JO5_J")
                {
                    lastValue = Convert.ToInt32(winRed[5]) % 2 != 0 ? 0 : lastValue;
                }
                if (p.Name == "JO5_O")
                {
                    lastValue = Convert.ToInt32(winRed[5]) % 2 != 0 ? lastValue : 0;
                }
                return lastValue;
            });

            manager.AddDF6_1_JOZS(entity);
        }

        private void AddDF6_1_ZHZS(string issuseNumber, string winNumber)
        {
            var manager = new DF6_1_ZHZSManager();
            var issuse = manager.QueryDF6_1_ZHZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string ZH_Proportion = string.Empty;
            int z = 0;
            int h = 0;
            foreach (var item in winRed)
            {
                if (Convert.ToInt32(item) == 1 || Convert.ToInt32(item) == 2 || Convert.ToInt32(item) == 3 || Convert.ToInt32(item) == 5 || Convert.ToInt32(item) == 7)
                {
                    z++;
                }
                else
                {
                    h++;
                }
            }
            ZH_Proportion = string.Format("{0}:{1}", z, h);
            var last = manager.QueryLastDF6_1_ZHZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("ZH_Proportion", ZH_Proportion);
            var entity = this.CreateNewEntity<DF6_1_ZHZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("O_ZH_"))
                {
                    var order = p.Name.Replace("O_ZH_Proportion", string.Empty);
                    lastValue = ZH_Proportion.Replace(":", "") == order ? 0 : lastValue;
                }
                if (p.Name == "ZH_Z")
                {
                    lastValue = (Convert.ToInt32(winRed[0]) == 1 || Convert.ToInt32(winRed[0]) == 2 || Convert.ToInt32(winRed[0]) == 3 || Convert.ToInt32(winRed[0]) == 5 || Convert.ToInt32(winRed[0]) == 7) ? 0 : lastValue;
                }
                if (p.Name == "ZH_H")
                {
                    lastValue = (Convert.ToInt32(winRed[0]) == 1 || Convert.ToInt32(winRed[0]) == 2 || Convert.ToInt32(winRed[0]) == 3 || Convert.ToInt32(winRed[0]) == 5 || Convert.ToInt32(winRed[0]) == 7) ? lastValue : 0;
                }
                if (p.Name == "ZH1_Z")
                {
                    lastValue = (Convert.ToInt32(winRed[1]) == 1 || Convert.ToInt32(winRed[1]) == 2 || Convert.ToInt32(winRed[1]) == 3 || Convert.ToInt32(winRed[1]) == 5 || Convert.ToInt32(winRed[1]) == 7) ? 0 : lastValue;
                }
                if (p.Name == "ZH1_H")
                {
                    lastValue = (Convert.ToInt32(winRed[1]) == 1 || Convert.ToInt32(winRed[1]) == 2 || Convert.ToInt32(winRed[1]) == 3 || Convert.ToInt32(winRed[1]) == 5 || Convert.ToInt32(winRed[1]) == 7) ? lastValue : 0;
                }
                if (p.Name == "ZH2_Z")
                {
                    lastValue = (Convert.ToInt32(winRed[2]) == 1 || Convert.ToInt32(winRed[2]) == 2 || Convert.ToInt32(winRed[2]) == 3 || Convert.ToInt32(winRed[2]) == 5 || Convert.ToInt32(winRed[2]) == 7) ? 0 : lastValue;
                }
                if (p.Name == "ZH2_H")
                {
                    lastValue = (Convert.ToInt32(winRed[2]) == 1 || Convert.ToInt32(winRed[2]) == 2 || Convert.ToInt32(winRed[2]) == 3 || Convert.ToInt32(winRed[2]) == 5 || Convert.ToInt32(winRed[2]) == 7) ? lastValue : 0;
                }
                if (p.Name == "ZH3_Z")
                {
                    lastValue = (Convert.ToInt32(winRed[3]) == 1 || Convert.ToInt32(winRed[3]) == 2 || Convert.ToInt32(winRed[3]) == 3 || Convert.ToInt32(winRed[3]) == 5 || Convert.ToInt32(winRed[3]) == 7) ? 0 : lastValue;
                }
                if (p.Name == "ZH3_H")
                {
                    lastValue = (Convert.ToInt32(winRed[3]) == 1 || Convert.ToInt32(winRed[3]) == 2 || Convert.ToInt32(winRed[3]) == 3 || Convert.ToInt32(winRed[3]) == 5 || Convert.ToInt32(winRed[3]) == 7) ? lastValue : 0;
                }
                if (p.Name == "ZH4_Z")
                {
                    lastValue = (Convert.ToInt32(winRed[4]) == 1 || Convert.ToInt32(winRed[4]) == 2 || Convert.ToInt32(winRed[4]) == 3 || Convert.ToInt32(winRed[4]) == 5 || Convert.ToInt32(winRed[4]) == 7) ? 0 : lastValue;
                }
                if (p.Name == "ZH4_H")
                {
                    lastValue = (Convert.ToInt32(winRed[4]) == 1 || Convert.ToInt32(winRed[4]) == 2 || Convert.ToInt32(winRed[4]) == 3 || Convert.ToInt32(winRed[4]) == 5 || Convert.ToInt32(winRed[4]) == 7) ? lastValue : 0;
                }
                if (p.Name == "ZH5_Z")
                {
                    lastValue = (Convert.ToInt32(winRed[5]) == 1 || Convert.ToInt32(winRed[5]) == 2 || Convert.ToInt32(winRed[5]) == 3 || Convert.ToInt32(winRed[5]) == 5 || Convert.ToInt32(winRed[5]) == 7) ? 0 : lastValue;
                }
                if (p.Name == "ZH5_H")
                {
                    lastValue = (Convert.ToInt32(winRed[5]) == 1 || Convert.ToInt32(winRed[5]) == 2 || Convert.ToInt32(winRed[5]) == 3 || Convert.ToInt32(winRed[5]) == 5 || Convert.ToInt32(winRed[5]) == 7) ? lastValue : 0;
                }
                return lastValue;
            });

            manager.AddDF6_1_ZHZS(entity);
        }
        private void AddDF6_1_KDZS(string issuseNumber, string winNumber)
        {
            var manager = new DF6_1_KDZSManager();
            var issuse = manager.QueryDF6_1_KDZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hezhi = Convert.ToInt32(winRed[0]) + Convert.ToInt32(winRed[1]) + Convert.ToInt32(winRed[2]) + Convert.ToInt32(winRed[3]) + Convert.ToInt32(winRed[4]) + Convert.ToInt32(winRed[5]);

            var KD = int.Parse(winRed.Max()) - int.Parse(winRed.Min());
            var C6 = (hezhi / 6).ToString("N0");
            var last = manager.QueryLastDF6_1_KDZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("HeZhi", hezhi);
            var entity = this.CreateNewEntity<DF6_1_KDZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("KD_"))
                {
                    var order = p.Name.Replace("KD_", string.Empty);
                    lastValue = KD == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("C_"))
                {
                    var order = p.Name.Replace("C_", string.Empty);
                    lastValue = C6 == order ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddDF6_1_KDZS(entity);
        }

        public DF6_1_DXZS_InfoCollection QueryDF6_1_DXZS(int index)
        {
            DF6_1_DXZS_InfoCollection Collection = new DF6_1_DXZS_InfoCollection();
            var list = this.QueryGameChart<DF6_1_DXZS_Info>(string.Format("QueryDF6_1_DXZS_{0}", index), () =>
            {
                var infoList = new List<DF6_1_DXZS_Info>();
                var entityList = new DF6_1_DXZSManager().QueryDF6_1_DXZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<DF6_1_DXZS>, DF6_1_DXZS, List<DF6_1_DXZS_Info>, DF6_1_DXZS_Info>(entityList, ref infoList,
                    () => { return new DF6_1_DXZS_Info(); },
                    (entity, info) =>
                    {
                        //处理info里面有，页entity里面没有的属性
                        //info.WinNumber = entity.WinNumber;
                    });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }
        public DF6_1_HZZS_InfoCollection QueryDF6_1_HZZS(int index)
        {
            DF6_1_HZZS_InfoCollection Collection = new DF6_1_HZZS_InfoCollection();
            var list = this.QueryGameChart<DF6_1_HZZS_Info>(string.Format("QueryDF6_1_HZZS_{0}", index), () =>
            {
                var infoList = new List<DF6_1_HZZS_Info>();
                var entityList = new DF6_1_HZZSManager().QueryDF6_1_HZZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<DF6_1_HZZS>, DF6_1_HZZS, List<DF6_1_HZZS_Info>, DF6_1_HZZS_Info>(entityList, ref infoList,
                    () => { return new DF6_1_HZZS_Info(); },
                    (entity, info) =>
                    {
                        //处理info里面有，页entity里面没有的属性
                        //info.WinNumber = entity.WinNumber;
                    });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }
        public DF6_1_JBZS_InfoCollection QueryDF6_1_JBZS(int index)
        {
            DF6_1_JBZS_InfoCollection Collection = new DF6_1_JBZS_InfoCollection();
            var list = this.QueryGameChart<DF6_1_JBZS_Info>(string.Format("QueryDF6_1_JBZS_{0}", index), () =>
            {
                var infoList = new List<DF6_1_JBZS_Info>();
                var entityList = new DF6_1_JBZSManager().QueryDF6_1_JBZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<DF6_1_JBZS>, DF6_1_JBZS, List<DF6_1_JBZS_Info>, DF6_1_JBZS_Info>(entityList, ref infoList,
                    () => { return new DF6_1_JBZS_Info(); },
                    (entity, info) =>
                    {
                        //处理info里面有，页entity里面没有的属性
                        //info.WinNumber = entity.WinNumber;
                    });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }
        public DF6_1_JOZS_InfoCollection QueryDF6_1_JOZS(int index)
        {
            DF6_1_JOZS_InfoCollection Collection = new DF6_1_JOZS_InfoCollection();
            var list = this.QueryGameChart<DF6_1_JOZS_Info>(string.Format("QueryDF6_1_JOZS_{0}", index), () =>
            {
                var infoList = new List<DF6_1_JOZS_Info>();
                var entityList = new DF6_1_JOZSManager().QueryDF6_1_JOZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<DF6_1_JOZS>, DF6_1_JOZS, List<DF6_1_JOZS_Info>, DF6_1_JOZS_Info>(entityList, ref infoList,
                    () => { return new DF6_1_JOZS_Info(); },
                    (entity, info) =>
                    {
                        //处理info里面有，页entity里面没有的属性
                        //info.WinNumber = entity.WinNumber;
                    });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }
        public DF6_1_KDZS_InfoCollection QueryDF6_1_KDZS(int index)
        {
            DF6_1_KDZS_InfoCollection Collection = new DF6_1_KDZS_InfoCollection();
            var list = this.QueryGameChart<DF6_1_KDZS_Info>(string.Format("QueryDF6_1_KDZS_{0}", index), () =>
            {
                var infoList = new List<DF6_1_KDZS_Info>();
                var entityList = new DF6_1_KDZSManager().QueryDF6_1_KDZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<DF6_1_KDZS>, DF6_1_KDZS, List<DF6_1_KDZS_Info>, DF6_1_KDZS_Info>(entityList, ref infoList,
                    () => { return new DF6_1_KDZS_Info(); },
                    (entity, info) =>
                    {
                        //处理info里面有，页entity里面没有的属性
                        //info.WinNumber = entity.WinNumber;
                    });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }
        public DF6_1_ZHZS_InfoCollection QueryDF6_1_ZHZS(int index)
        {
            DF6_1_ZHZS_InfoCollection Collection = new DF6_1_ZHZS_InfoCollection();
            var list = this.QueryGameChart<DF6_1_ZHZS_Info>(string.Format("QueryDF6_1_ZHZS_{0}", index), () =>
            {
                var infoList = new List<DF6_1_ZHZS_Info>();
                var entityList = new DF6_1_ZHZSManager().QueryDF6_1_ZHZS(index);

               ObjectConvert.ConvertEntityListToInfoList<List<DF6_1_ZHZS>, DF6_1_ZHZS, List<DF6_1_ZHZS_Info>, DF6_1_ZHZS_Info>(entityList, ref infoList,
                    () => { return new DF6_1_ZHZS_Info(); },
                    (entity, info) =>
                    {
                        //处理info里面有，页entity里面没有的属性
                        //info.WinNumber = entity.WinNumber;
                    });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }


        /// <summary>
        /// 查询开奖数据
        /// </summary>
        public GameWinNumber_InfoCollection QueryDF6_1_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new DF6_1_GameWinNumberManager().QueryDF6_1_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<DF6_1_GameWinNumber>, DF6_1_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryDF6_1_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new DF6_1_GameWinNumberManager().QueryDF6_1_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<DF6_1_GameWinNumber>, DF6_1_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
            //        () => { return new GameWinNumber_Info(); },
            //        (entity, info) =>
            //        {
            //            //处理info里面有，页entity里面没有的属性
            //            //info.WinNumber = entity.WinNumber;
            //        });
            //    collection.TotalCount = totalCount;
            //    collection.List.AddRange(infoList);
            //    return collection;
            //});
        }

        public GameWinNumber_Info QueryWinNumber(string issuseNumber)
        {
            var manager = new DF6_1_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<DF6_1_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }
    }
}
