using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.ORM.Helper.WinNumber.Manage;
using EntityModel;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;
using KaSon.FrameWork.Analyzer.AnalyzerFactory;

namespace KaSon.FrameWork.ORM.Helper.WinNumber
{
    public class LotteryDataBusiness_JX11X5 : LotteryDataBusiness, ILotteryDataBusiness
    {
        public string CurrentGameCode
        {
            get
            {
                return "JX11X5";
            }
        }

        public void ImportWinNumber(string issuseNumber, string winNumber)
        {
            if (string.IsNullOrEmpty(issuseNumber)) return;
            if (string.IsNullOrEmpty(winNumber)) return;

            var msg = string.Empty;
            AnalyzerFactory.GetWinNumberAnalyzer(this.CurrentGameCode).CheckWinNumber(winNumber, out msg);
            if (!string.IsNullOrEmpty(msg))
                throw new Exception(msg);

            //开启事务
            using (LottertDataDB)
            {
                LottertDataDB.Begin();

                this.ClearGameChartCache("QueryJX11X5_RXJBZS_Info");
                this.ClearGameChartCache("QueryJX11X5_RXDX_Info");
                this.ClearGameChartCache("QueryJX11X5_RXJO_Info");
                this.ClearGameChartCache("QueryJX11X5_RXZH_Info");
                this.ClearGameChartCache("QueryJX11X5_RXHZ_Info");
                this.ClearGameChartCache("QueryJX11X5_Chu3_Info");
                this.ClearGameChartCache("QueryJX11X5_RX1_Info");
                this.ClearGameChartCache("QueryJX11X5_RX2_Info");
                this.ClearGameChartCache("QueryJX11X5_RX3_Info");
                this.ClearGameChartCache("QueryJX11X5_RX4_Info");
                this.ClearGameChartCache("QueryJX11X5_RX5_Info");
                this.ClearGameChartCache("QueryJX11X5_Q3ZS_Info");
                this.ClearGameChartCache("QueryJX11X5_Q3ZUS_Info");
                this.ClearGameChartCache("QueryJX11X5_Q3DX_Info");
                this.ClearGameChartCache("QueryJX11X5_Q3JO_Info");
                this.ClearGameChartCache("QueryJX11X5_Q3ZH_Info");
                this.ClearGameChartCache("QueryJX11X5_Q3Chu3_Info");
                this.ClearGameChartCache("QueryJX11X5_Q3HZ_Info");
                this.ClearGameChartCache("QueryJX11X5_Q2ZS_Info");
                this.ClearGameChartCache("QueryJX11X5_Q2ZUS_Info");
                this.ClearGameChartCache("QueryJX11X5_Q2HZ_Info");
                this.ClearNewWinNumberCache("QueryJX11X5_GameWinNumber");

                Import_RXJBZS(issuseNumber, winNumber);
                Import_RXDX(issuseNumber, winNumber);
                Import_RXJO(issuseNumber, winNumber);
                Import_RXZH(issuseNumber, winNumber);
                Import_RXHZ(issuseNumber, winNumber);
                Import_RXChu3(issuseNumber, winNumber);
                Import_RX1(issuseNumber, winNumber);
                Import_RX2(issuseNumber, winNumber);
                Import_RX3(issuseNumber, winNumber);
                Import_RX4(issuseNumber, winNumber);
                Import_RX5(issuseNumber, winNumber);
                Import_Q3ZS(issuseNumber, winNumber);
                Import_Q3ZUS(issuseNumber, winNumber);
                Import_Q3DX(issuseNumber, winNumber);
                Import_Q3JO(issuseNumber, winNumber);
                Import_Q3ZH(issuseNumber, winNumber);
                Import_Q3Chu3(issuseNumber, winNumber);
                Import_Q3HZ(issuseNumber, winNumber);
                Import_Q2ZS(issuseNumber, winNumber);
                Import_Q2ZUS(issuseNumber, winNumber);
                Import_Q2HZ(issuseNumber, winNumber);
                Add_GameWinNumber(issuseNumber, winNumber);

                LottertDataDB.Commit();
            }
        }

        #region 前台查询数据

        /// <summary>
        ///查询任选基本走势列表按时间倒叙 
        /// </summary>
        public List<JX11X5_RXJBZS_Info> QueryJX11X5_RXJBZS_Info(int length)
        {
            List<JX11X5_RXJBZS_Info> Collection = new List<JX11X5_RXJBZS_Info>();
            var list = this.QueryGameChart<JX11X5_RXJBZS_Info>(string.Format("QueryJX11X5_RXJBZS_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_RXJBZS_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_RXJBZS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_RXJBZS>, JX11X5_RXJBZS, List<JX11X5_RXJBZS_Info>, JX11X5_RXJBZS_Info>(entityList, ref infoList,
                    () => { return new JX11X5_RXJBZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询任选大小列表按时间倒叙 
        /// </summary>
        public List<JX11X5_RXDX_Info> QueryJX11X5_RXDX_Info(int length)
        {
            List<JX11X5_RXDX_Info> Collection = new List<JX11X5_RXDX_Info>();
            var list = this.QueryGameChart<JX11X5_RXDX_Info>(string.Format("QueryJX11X5_RXDX_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_RXDX_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_RXDX_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_RXDX>, JX11X5_RXDX, List<JX11X5_RXDX_Info>, JX11X5_RXDX_Info>(entityList, ref infoList,
                    () => { return new JX11X5_RXDX_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询任选奇偶列表按时间倒叙 
        /// </summary>
        public List<JX11X5_RXJO_Info> QueryJX11X5_RXJO_Info(int length)
        {
            List<JX11X5_RXJO_Info> Collection = new List<JX11X5_RXJO_Info>();
            var list = this.QueryGameChart<JX11X5_RXJO_Info>(string.Format("QueryJX11X5_RXJO_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_RXJO_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_RXJO_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_RXJO>, JX11X5_RXJO, List<JX11X5_RXJO_Info>, JX11X5_RXJO_Info>(entityList, ref infoList,
                    () => { return new JX11X5_RXJO_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询任选质和列表按时间倒叙 
        /// </summary>
        public List<JX11X5_RXZH_Info> QueryJX11X5_RXZH_Info(int length)
        {
            List<JX11X5_RXZH_Info> Collection = new List<JX11X5_RXZH_Info>();
            var list = this.QueryGameChart<JX11X5_RXZH_Info>(string.Format("QueryJX11X5_RXZH_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_RXZH_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_RXZH_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_RXZH>, JX11X5_RXZH, List<JX11X5_RXZH_Info>, JX11X5_RXZH_Info>(entityList, ref infoList,
                    () => { return new JX11X5_RXZH_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询任选和值列表按时间倒叙 
        /// </summary>
        public List<JX11X5_RXHZ_Info> QueryJX11X5_RXHZ_Info(int length)
        {
            List<JX11X5_RXHZ_Info> Collection = new List<JX11X5_RXHZ_Info>();
            var list = this.QueryGameChart<JX11X5_RXHZ_Info>(string.Format("QueryJX11X5_RXHZ_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_RXHZ_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_RXHZ_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_RXHZ>, JX11X5_RXHZ, List<JX11X5_RXHZ_Info>, JX11X5_RXHZ_Info>(entityList, ref infoList,
                    () => { return new JX11X5_RXHZ_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询任选除3列表按时间倒叙 
        /// </summary>
        public List<JX11X5_Chu3_Info> QueryJX11X5_Chu3_Info(int length)
        {
            List<JX11X5_Chu3_Info> Collection = new List<JX11X5_Chu3_Info>();
            var list = this.QueryGameChart<JX11X5_Chu3_Info>(string.Format("QueryJX11X5_Chu3_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_Chu3_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_Chu3_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_Chu3>, JX11X5_Chu3, List<JX11X5_Chu3_Info>, JX11X5_Chu3_Info>(entityList, ref infoList,
                    () => { return new JX11X5_Chu3_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询任选第一位列表按时间倒叙 
        /// </summary>
        public List<JX11X5_RX1_Info> QueryJX11X5_RX1_Info(int length)
        {
            List<JX11X5_RX1_Info> Collection = new List<JX11X5_RX1_Info>();
            var list = this.QueryGameChart<JX11X5_RX1_Info>(string.Format("QueryJX11X5_RX1_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_RX1_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_RX1_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_RX1>, JX11X5_RX1, List<JX11X5_RX1_Info>, JX11X5_RX1_Info>(entityList, ref infoList,
                    () => { return new JX11X5_RX1_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询任选第二位列表按时间倒叙 
        /// </summary>
        public List<JX11X5_RX2_Info> QueryJX11X5_RX2_Info(int length)
        {
            List<JX11X5_RX2_Info> Collection = new List<JX11X5_RX2_Info>();
            var list = this.QueryGameChart<JX11X5_RX2_Info>(string.Format("QueryJX11X5_RX2_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_RX2_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_RX2_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_RX2>, JX11X5_RX2, List<JX11X5_RX2_Info>, JX11X5_RX2_Info>(entityList, ref infoList,
                    () => { return new JX11X5_RX2_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询任选第三位列表按时间倒叙 
        /// </summary>
        public List<JX11X5_RX3_Info> QueryJX11X5_RX3_Info(int length)
        {
            List<JX11X5_RX3_Info> Collection = new List<JX11X5_RX3_Info>();
            var list = this.QueryGameChart<JX11X5_RX3_Info>(string.Format("QueryJX11X5_RX3_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_RX3_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_RX3_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_RX3>, JX11X5_RX3, List<JX11X5_RX3_Info>, JX11X5_RX3_Info>(entityList, ref infoList,
                    () => { return new JX11X5_RX3_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询任选第四位列表按时间倒叙 
        /// </summary>
        public List<JX11X5_RX4_Info> QueryJX11X5_RX4_Info(int length)
        {
            List<JX11X5_RX4_Info> Collection = new List<JX11X5_RX4_Info>();
            var list = this.QueryGameChart<JX11X5_RX4_Info>(string.Format("QueryJX11X5_RX4_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_RX4_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_RX4_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_RX4>, JX11X5_RX4, List<JX11X5_RX4_Info>, JX11X5_RX4_Info>(entityList, ref infoList,
                    () => { return new JX11X5_RX4_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询任选第五位列表按时间倒叙 
        /// </summary>
        public List<JX11X5_RX5_Info> QueryJX11X5_RX5_Info(int length)
        {
            List<JX11X5_RX5_Info> Collection = new List<JX11X5_RX5_Info>();
            var list = this.QueryGameChart<JX11X5_RX5_Info>(string.Format("QueryJX11X5_RX5_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_RX5_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_RX5_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_RX5>, JX11X5_RX5, List<JX11X5_RX5_Info>, JX11X5_RX5_Info>(entityList, ref infoList,
                    () => { return new JX11X5_RX5_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询前三直选列表按时间倒叙 
        /// </summary>
        public List<JX11X5_Q3ZS_Info> QueryJX11X5_Q3ZS_Info(int length)
        {
            List<JX11X5_Q3ZS_Info> Collection = new List<JX11X5_Q3ZS_Info>();
            var list = this.QueryGameChart<JX11X5_Q3ZS_Info>(string.Format("QueryJX11X5_Q3ZS_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_Q3ZS_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_Q3ZS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_Q3ZS>, JX11X5_Q3ZS, List<JX11X5_Q3ZS_Info>, JX11X5_Q3ZS_Info>(entityList, ref infoList,
                    () => { return new JX11X5_Q3ZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询前三组选列表按时间倒叙 
        /// </summary>
        public List<JX11X5_Q3ZUS_Info> QueryJX11X5_Q3ZUS_Info(int length)
        {
            List<JX11X5_Q3ZUS_Info> Collection = new List<JX11X5_Q3ZUS_Info>();
            var list = this.QueryGameChart<JX11X5_Q3ZUS_Info>(string.Format("QueryJX11X5_Q3ZUS_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_Q3ZUS_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_Q3ZUS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_Q3ZUS>, JX11X5_Q3ZUS, List<JX11X5_Q3ZUS_Info>, JX11X5_Q3ZUS_Info>(entityList, ref infoList,
                    () => { return new JX11X5_Q3ZUS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询前三大小列表按时间倒叙 
        /// </summary>
        public List<JX11X5_Q3DX_Info> QueryJX11X5_Q3DX_Info(int length)
        {
            List<JX11X5_Q3DX_Info> Collection = new List<JX11X5_Q3DX_Info>();
            var list = this.QueryGameChart<JX11X5_Q3DX_Info>(string.Format("QueryJX11X5_Q3DX_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_Q3DX_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_Q3DX_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_Q3DX>, JX11X5_Q3DX, List<JX11X5_Q3DX_Info>, JX11X5_Q3DX_Info>(entityList, ref infoList,
                    () => { return new JX11X5_Q3DX_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询前三奇偶列表按时间倒叙 
        /// </summary>
        public List<JX11X5_Q3JO_Info> QueryJX11X5_Q3JO_Info(int length)
        {
            List<JX11X5_Q3JO_Info> Collection = new List<JX11X5_Q3JO_Info>();
            var list = this.QueryGameChart<JX11X5_Q3JO_Info>(string.Format("QueryJX11X5_Q3JO_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_Q3JO_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_Q3JO_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_Q3JO>, JX11X5_Q3JO, List<JX11X5_Q3JO_Info>, JX11X5_Q3JO_Info>(entityList, ref infoList,
                    () => { return new JX11X5_Q3JO_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询前三质和列表按时间倒叙 
        /// </summary>
        public List<JX11X5_Q3ZH_Info> QueryJX11X5_Q3ZH_Info(int length)
        {
            List<JX11X5_Q3ZH_Info> Collection = new List<JX11X5_Q3ZH_Info>();
            var list = this.QueryGameChart<JX11X5_Q3ZH_Info>(string.Format("QueryJX11X5_Q3ZH_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_Q3ZH_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_Q3ZH_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_Q3ZH>, JX11X5_Q3ZH, List<JX11X5_Q3ZH_Info>, JX11X5_Q3ZH_Info>(entityList, ref infoList,
                    () => { return new JX11X5_Q3ZH_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询前三除3列表按时间倒叙 
        /// </summary>
        public List<JX11X5_Q3Chu3_Info> QueryJX11X5_Q3Chu3_Info(int length)
        {
            List<JX11X5_Q3Chu3_Info> Collection = new List<JX11X5_Q3Chu3_Info>();
            var list = this.QueryGameChart<JX11X5_Q3Chu3_Info>(string.Format("QueryJX11X5_Q3Chu3_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_Q3Chu3_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_Q3Chu3_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_Q3Chu3>, JX11X5_Q3Chu3, List<JX11X5_Q3Chu3_Info>, JX11X5_Q3Chu3_Info>(entityList, ref infoList,
                    () => { return new JX11X5_Q3Chu3_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询前三和值列表按时间倒叙 
        /// </summary>
        public List<JX11X5_Q3HZ_Info> QueryJX11X5_Q3HZ_Info(int length)
        {
            List<JX11X5_Q3HZ_Info> Collection = new List<JX11X5_Q3HZ_Info>();
            var list = this.QueryGameChart<JX11X5_Q3HZ_Info>(string.Format("QueryJX11X5_Q3HZ_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_Q3HZ_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_Q3HZ_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_Q3HZ>, JX11X5_Q3HZ, List<JX11X5_Q3HZ_Info>, JX11X5_Q3HZ_Info>(entityList, ref infoList,
                    () => { return new JX11X5_Q3HZ_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询前2直选列表按时间倒叙 
        /// </summary>
        public List<JX11X5_Q2ZS_Info> QueryJX11X5_Q2ZS_Info(int length)
        {
            List<JX11X5_Q2ZS_Info> Collection = new List<JX11X5_Q2ZS_Info>();
            var list = this.QueryGameChart<JX11X5_Q2ZS_Info>(string.Format("QueryJX11X5_Q2ZS_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_Q2ZS_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_Q2ZS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_Q2ZS>, JX11X5_Q2ZS, List<JX11X5_Q2ZS_Info>, JX11X5_Q2ZS_Info>(entityList, ref infoList,
                    () => { return new JX11X5_Q2ZS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询前2组选列表按时间倒叙 
        /// </summary>
        public List<JX11X5_Q2ZUS_Info> QueryJX11X5_Q2ZUS_Info(int length)
        {
            List<JX11X5_Q2ZUS_Info> Collection = new List<JX11X5_Q2ZUS_Info>();
            var list = this.QueryGameChart<JX11X5_Q2ZUS_Info>(string.Format("QueryJX11X5_Q2ZUS_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_Q2ZUS_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_Q2ZUS_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_Q2ZUS>, JX11X5_Q2ZUS, List<JX11X5_Q2ZUS_Info>, JX11X5_Q2ZUS_Info>(entityList, ref infoList,
                    () => { return new JX11X5_Q2ZUS_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        /// <summary>
        ///查询前2和值列表按时间倒叙 
        /// </summary>
        public List<JX11X5_Q2HZ_Info> QueryJX11X5_Q2HZ_Info(int length)
        {
            List<JX11X5_Q2HZ_Info> Collection = new List<JX11X5_Q2HZ_Info>();
            var list = this.QueryGameChart<JX11X5_Q2HZ_Info>(string.Format("QueryJX11X5_Q2HZ_Info_{0}", length), () =>
            {
                var infoList = new List<JX11X5_Q2HZ_Info>();
                var entityList = new JX11X5_Manager().QueryJX11X5_Q2HZ_Info(length);

               ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_Q2HZ>, JX11X5_Q2HZ, List<JX11X5_Q2HZ_Info>, JX11X5_Q2HZ_Info>(entityList, ref infoList,
                    () => { return new JX11X5_Q2HZ_Info(); });
                return infoList;
            });
            Collection.AddRange(list);
            return Collection;
        }

        #endregion

        #region 生成走势图数据

        public void Add_GameWinNumber(string issuseNumber, string winNumber)
        {
            new KJGameIssuseBusiness().IssusePrize(this.CurrentGameCode, issuseNumber, winNumber);
            var manager = new JX11X5_GameWinNumberManager();
            var exist = manager.QueryWinNumber(issuseNumber);
            if (exist != null) return;

            manager.AddJX11X5_GameWinNumber(new JX11X5_GameWinNumber
            {
                GameCode = this.CurrentGameCode,
                IssuseNumber = issuseNumber,
                WinNumber = winNumber,
                CreateTime = DateTime.Now,
            });
        }

        /// <summary>
        /// 任选基本走势
        /// </summary>
        private void Import_RXJBZS(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_RXJBZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var arrayJi = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]) };

            #region 奇个数

            var win1 = string.Empty;
            var win2 = string.Empty;
            var win3 = string.Empty;
            var win4 = string.Empty;
            var win5 = string.Empty;
            if (arrayJi[0] % 2 == 1)
                win1 = "J";
            else
                win1 = "0";

            if (arrayJi[1] % 2 == 1)
                win2 = "J";
            else
                win2 = "0";

            if (arrayJi[2] % 2 == 1)
                win3 = "J";
            else
                win3 = "0";

            if (arrayJi[3] % 2 == 1)
                win4 = "J";
            else
                win4 = "0";

            if (arrayJi[4] % 2 == 1)
                win5 = "J";
            else
                win5 = "0";

            var arrayWinJi = new string[] { win1, win2, win3, win4, win5 };
            var joType = string.Join("", arrayWinJi);
            int jiCount = joType.Count(p => p == 'J');
            #endregion

            #region 小个数

            var winxiao1 = string.Empty;
            var winxiao2 = string.Empty;
            var winxiao3 = string.Empty;
            var winxiao4 = string.Empty;
            var winxiao5 = string.Empty;
            if (arrayJi[0] <= 5)
                winxiao1 = "X";
            else
                winxiao1 = "D";

            if (arrayJi[1] <= 5)
                winxiao2 = "X";
            else
                winxiao2 = "D";

            if (arrayJi[2] <= 5)
                winxiao3 = "X";
            else
                winxiao3 = "D";

            if (arrayJi[3] <= 5)
                winxiao4 = "X";
            else
                winxiao4 = "D";

            if (arrayJi[4] <= 5)
                winxiao5 = "X";
            else
                winxiao5 = "D";

            var arrayWinXiao = new string[] { winxiao1, winxiao2, winxiao3, winxiao4, winxiao5 };
            var XiaoType = string.Join("", arrayWinXiao);
            int XiaoCount = XiaoType.Count(p => p == 'X');
            #endregion

            #region 质个数

            var zhilist = new int[] { 1, 2, 3, 5, 7 };
            var winzhi1 = string.Empty;
            var winzhi2 = string.Empty;
            var winzhi3 = string.Empty;
            var winzhi4 = string.Empty;
            var winzhi5 = string.Empty;
            if (zhilist.Contains(arrayJi[0]))
                winzhi1 = "Z";
            else
                winzhi1 = "H";

            if (zhilist.Contains(arrayJi[1]))
                winzhi2 = "Z";
            else
                winzhi2 = "H";

            if (zhilist.Contains(arrayJi[2]))
                winzhi3 = "Z";
            else
                winzhi3 = "H";

            if (zhilist.Contains(arrayJi[3]))
                winzhi4 = "Z";
            else
                winzhi4 = "H";

            if (zhilist.Contains(arrayJi[4]))
                winzhi5 = "Z";
            else
                winzhi5 = "H";

            var arrayWinZhi = new string[] { winzhi1, winzhi2, winzhi3, winzhi4, winzhi5 };
            var ZhiType = string.Join("", arrayWinZhi);
            int ZhiCount = ZhiType.Count(p => p == 'Z');
            #endregion

            var last = manager.QueryJX11X5_RXJBZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<JX11X5_RXJBZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("RXJB_"))
                {
                    var order = p.Name.Replace("RXJB_", string.Empty);
                    lastValue = winRed.Contains(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("J_"))
                {
                    var order = p.Name.Replace("J_", string.Empty);
                    lastValue = jiCount == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("X_"))
                {
                    var order = p.Name.Replace("X_", string.Empty);
                    lastValue = XiaoCount == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Z_"))
                {
                    var order = p.Name.Replace("Z_", string.Empty);
                    lastValue = ZhiCount == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJX11X5_RXJBZS(entity);
        }

        /// <summary>
        /// 任选大小走势
        /// </summary>
        private void Import_RXDX(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_RXDXIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]) };

            #region 大小个数

            var winxiao1 = string.Empty;
            var winxiao2 = string.Empty;
            var winxiao3 = string.Empty;
            var winxiao4 = string.Empty;
            var winxiao5 = string.Empty;
            if (array[0] <= 5)
                winxiao1 = "X";
            else
                winxiao1 = "D";

            if (array[1] <= 5)
                winxiao2 = "X";
            else
                winxiao2 = "D";

            if (array[2] <= 5)
                winxiao3 = "X";
            else
                winxiao3 = "D";

            if (array[3] <= 5)
                winxiao4 = "X";
            else
                winxiao4 = "D";

            if (array[4] <= 5)
                winxiao5 = "X";
            else
                winxiao5 = "D";

            var arrayWinXiao = new string[] { winxiao1, winxiao2, winxiao3, winxiao4, winxiao5 };
            var XiaoType = string.Join("", arrayWinXiao);
            int DaCount = XiaoType.Count(p => p == 'D');
            #endregion

            var last = manager.QueryJX11X5_RXDX();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("DXqualifying", string.Join("", arrayWinXiao));
            dic.Add("DaoXiaoBi", string.Format("{0}:{1}", DaCount, 5 - DaCount));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<JX11X5_RXDX>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1_"))
                {
                    var order = p.Name.Replace("NO1_", string.Empty);
                    lastValue = arrayWinXiao[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2_"))
                {
                    var order = p.Name.Replace("NO2_", string.Empty);
                    lastValue = arrayWinXiao[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3_"))
                {
                    var order = p.Name.Replace("NO3_", string.Empty);
                    lastValue = arrayWinXiao[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO4_"))
                {
                    var order = p.Name.Replace("NO4_", string.Empty);
                    lastValue = arrayWinXiao[3] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO5_"))
                {
                    var order = p.Name.Replace("NO5_", string.Empty);
                    lastValue = arrayWinXiao[4] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var da = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(da[0]) == DaCount ? 0 : lastValue;
                }
                return lastValue;
            });
            manager.AddJX11X5_RXDX(entity);
        }

        /// <summary>
        /// 任选奇偶走势
        /// </summary>
        private void Import_RXJO(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_RXJOIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]) };

            #region 奇偶个数

            var winji1 = string.Empty;
            var winji2 = string.Empty;
            var winji3 = string.Empty;
            var winji4 = string.Empty;
            var winji5 = string.Empty;
            if (array[0] % 2 == 1)
                winji1 = "J";
            else
                winji1 = "O";

            if (array[1] % 2 == 1)
                winji2 = "J";
            else
                winji2 = "O";

            if (array[2] % 2 == 1)
                winji3 = "J";
            else
                winji3 = "O";

            if (array[3] % 2 == 1)
                winji4 = "J";
            else
                winji4 = "O";

            if (array[4] % 2 == 1)
                winji5 = "J";
            else
                winji5 = "O";

            var arraywinji = new string[] { winji1, winji2, winji3, winji4, winji5 };
            var JiType = string.Join("", arraywinji);
            int JiCount = JiType.Count(p => p == 'J');
            #endregion

            var last = manager.QueryJX11X5_RXJO();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("JOqualifying", string.Join("", arraywinji));
            dic.Add("JiOuBi", string.Format("{0}:{1}", JiCount, 5 - JiCount));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<JX11X5_RXJO>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1_"))
                {
                    var order = p.Name.Replace("NO1_", string.Empty);
                    lastValue = arraywinji[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2_"))
                {
                    var order = p.Name.Replace("NO2_", string.Empty);
                    lastValue = arraywinji[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3_"))
                {
                    var order = p.Name.Replace("NO3_", string.Empty);
                    lastValue = arraywinji[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO4_"))
                {
                    var order = p.Name.Replace("NO4_", string.Empty);
                    lastValue = arraywinji[3] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO5_"))
                {
                    var order = p.Name.Replace("NO5_", string.Empty);
                    lastValue = arraywinji[4] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var ji = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(ji[0]) == JiCount ? 0 : lastValue;
                }
                return lastValue;
            });
            manager.AddJX11X5_RXJO(entity);

        }

        /// <summary>
        /// 任选质和走势
        /// </summary>
        private void Import_RXZH(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_RXZHIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var array = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]), int.Parse(winRed[3]), int.Parse(winRed[4]) };

            #region 质和个数

            var zhilist = new int[] { 1, 2, 3, 5, 7 };
            var winzhi1 = string.Empty;
            var winzhi2 = string.Empty;
            var winzhi3 = string.Empty;
            var winzhi4 = string.Empty;
            var winzhi5 = string.Empty;
            if (zhilist.Contains(array[0]))
                winzhi1 = "Z";
            else
                winzhi1 = "H";

            if (zhilist.Contains(array[1]))
                winzhi2 = "Z";
            else
                winzhi2 = "H";

            if (zhilist.Contains(array[2]))
                winzhi3 = "Z";
            else
                winzhi3 = "H";

            if (zhilist.Contains(array[3]))
                winzhi4 = "Z";
            else
                winzhi4 = "H";

            if (zhilist.Contains(array[4]))
                winzhi5 = "Z";
            else
                winzhi5 = "H";

            var arraywinzhi = new string[] { winzhi1, winzhi2, winzhi3, winzhi4, winzhi5 };
            var zhiType = string.Join("", arraywinzhi);
            int ZhiCount = zhiType.Count(p => p == 'Z');
            #endregion

            var last = manager.QueryJX11X5_RXZH();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("ZHqualifying", string.Join("", arraywinzhi));
            dic.Add("ZhiHeBi", string.Format("{0}:{1}", ZhiCount, 5 - ZhiCount));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<JX11X5_RXZH>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1_"))
                {
                    var order = p.Name.Replace("NO1_", string.Empty);
                    lastValue = arraywinzhi[0] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2_"))
                {
                    var order = p.Name.Replace("NO2_", string.Empty);
                    lastValue = arraywinzhi[1] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3_"))
                {
                    var order = p.Name.Replace("NO3_", string.Empty);
                    lastValue = arraywinzhi[2] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO4_"))
                {
                    var order = p.Name.Replace("NO4_", string.Empty);
                    lastValue = arraywinzhi[3] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO5_"))
                {
                    var order = p.Name.Replace("NO5_", string.Empty);
                    lastValue = arraywinzhi[4] == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var zhi = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(zhi[0]) == ZhiCount ? 0 : lastValue;
                }

                return lastValue;
            });
            manager.AddJX11X5_RXZH(entity);

        }

        /// <summary>
        /// 任选和值走势
        /// </summary>
        private void Import_RXHZ(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_RXHZIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]) + int.Parse(winRed[3]) + int.Parse(winRed[4]);
            var hw = hz % 10;

            var last = manager.QueryJX11X5_RXHZ();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<JX11X5_RXHZ>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("HZ_"))
                {
                    var order = p.Name.Replace("HZ_", string.Empty);
                    lastValue = hz == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("HW_"))
                {
                    var order = p.Name.Replace("HW_", string.Empty);
                    lastValue = hw == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJX11X5_RXHZ(entity);
        }

        /// <summary>
        /// 任选基本走势
        /// </summary>
        private void Import_RXChu3(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_Chu3IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var chu3Arrary = new int[] { int.Parse(winRed[0]) % 3, int.Parse(winRed[1]) % 3, int.Parse(winRed[2]) % 3, int.Parse(winRed[3]) % 3, int.Parse(winRed[4]) % 3 };
            var count_0 = chu3Arrary.Count(p => p == 0);
            var count_1 = chu3Arrary.Count(p => p == 1);
            var count_2 = chu3Arrary.Count(p => p == 2);

            var last = manager.QueryJX11X5_Chu3();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("Chu3Bi", string.Format("{0}:{1}:{2}:{3}:{4}", chu3Arrary[0], chu3Arrary[1], chu3Arrary[2], chu3Arrary[3], chu3Arrary[4]));

            var entity = this.CreateNewEntity<JX11X5_Chu3>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1_"))
                {
                    var order = p.Name.Replace("NO1_", string.Empty);
                    lastValue = chu3Arrary[0] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2_"))
                {
                    var order = p.Name.Replace("NO2_", string.Empty);
                    lastValue = chu3Arrary[1] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3_"))
                {
                    var order = p.Name.Replace("NO3_", string.Empty);
                    lastValue = chu3Arrary[2] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO4_"))
                {
                    var order = p.Name.Replace("NO4_", string.Empty);
                    lastValue = chu3Arrary[3] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO5_"))
                {
                    var order = p.Name.Replace("NO5_", string.Empty);
                    lastValue = chu3Arrary[4] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu0_"))
                {
                    var order = p.Name.Replace("Yu0_", string.Empty);
                    lastValue = int.Parse(order) == count_0 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu1_"))
                {
                    var order = p.Name.Replace("Yu1_", string.Empty);
                    lastValue = int.Parse(order) == count_1 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu2_"))
                {
                    var order = p.Name.Replace("Yu2_", string.Empty);
                    lastValue = int.Parse(order) == count_2 ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJX11X5_Chu3(entity);
        }

        /// <summary>
        /// 任选第一位走势
        /// </summary>
        private void Import_RX1(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_RX1IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var rx1 = int.Parse(winRed[0]);

            var zhilist = new int[] { 1, 2, 3, 5, 7, 11 };
            var zhihe = string.Empty;
            if (zhilist.Contains(rx1))
                zhihe = "Z";
            else
                zhihe = "H";

            var winxiao1 = string.Empty;
            if (rx1 <= 5)
                winxiao1 = "X";
            else
                winxiao1 = "D";

            var winji1 = string.Empty;
            if (rx1 % 2 == 1)
                winji1 = "J";
            else
                winji1 = "O";

            var last = manager.QueryJX11X5_RX1();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", rx1.ToString());
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<JX11X5_RX1>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO_"))
                {
                    var order = p.Name.Replace("NO_", string.Empty);
                    lastValue = rx1 == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("DX_"))
                {
                    var order = p.Name.Replace("DX_", string.Empty);
                    lastValue = order == winxiao1 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("JO_"))
                {
                    var order = p.Name.Replace("JO_", string.Empty);
                    lastValue = order == winji1 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("ZH_"))
                {
                    var order = p.Name.Replace("ZH_", string.Empty);
                    lastValue = order == zhihe ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu_"))
                {
                    var order = p.Name.Replace("Yu_", string.Empty);
                    lastValue = rx1 % 3 == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJX11X5_RX1(entity);
        }

        /// <summary>
        /// 任选第二位走势
        /// </summary>
        private void Import_RX2(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_RX2IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var rx1 = int.Parse(winRed[1]);
            var zhilist = new int[] { 1, 2, 3, 5, 7, 11 };
            var zhihe = string.Empty;
            if (zhilist.Contains(rx1))
                zhihe = "Z";
            else
                zhihe = "H";

            var winxiao1 = string.Empty;
            if (rx1 <= 5)
                winxiao1 = "X";
            else
                winxiao1 = "D";

            var winji1 = string.Empty;
            if (rx1 % 2 == 1)
                winji1 = "J";
            else
                winji1 = "O";

            var last = manager.QueryJX11X5_RX2();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", rx1.ToString());
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<JX11X5_RX2>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO_"))
                {
                    var order = p.Name.Replace("NO_", string.Empty);
                    lastValue = rx1 == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("DX_"))
                {
                    var order = p.Name.Replace("DX_", string.Empty);
                    lastValue = order == winxiao1 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("JO_"))
                {
                    var order = p.Name.Replace("JO_", string.Empty);
                    lastValue = order == winji1 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("ZH_"))
                {
                    var order = p.Name.Replace("ZH_", string.Empty);
                    lastValue = order == zhihe ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu_"))
                {
                    var order = p.Name.Replace("Yu_", string.Empty);
                    lastValue = rx1 % 3 == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJX11X5_RX2(entity);
        }

        /// <summary>
        /// 任选第三位走势
        /// </summary>
        private void Import_RX3(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_RX3IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var rx1 = int.Parse(winRed[2]);
            var zhilist = new int[] { 1, 2, 3, 5, 7, 11 };
            var zhihe = string.Empty;
            if (zhilist.Contains(rx1))
                zhihe = "Z";
            else
                zhihe = "H";

            var winxiao1 = string.Empty;
            if (rx1 <= 5)
                winxiao1 = "X";
            else
                winxiao1 = "D";

            var winji1 = string.Empty;
            if (rx1 % 2 == 1)
                winji1 = "J";
            else
                winji1 = "O";

            var last = manager.QueryJX11X5_RX3();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", rx1.ToString());
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<JX11X5_RX3>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO_"))
                {
                    var order = p.Name.Replace("NO_", string.Empty);
                    lastValue = rx1 == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("DX_"))
                {
                    var order = p.Name.Replace("DX_", string.Empty);
                    lastValue = order == winxiao1 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("JO_"))
                {
                    var order = p.Name.Replace("JO_", string.Empty);
                    lastValue = order == winji1 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("ZH_"))
                {
                    var order = p.Name.Replace("ZH_", string.Empty);
                    lastValue = order == zhihe ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu_"))
                {
                    var order = p.Name.Replace("Yu_", string.Empty);
                    lastValue = rx1 % 3 == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJX11X5_RX3(entity);
        }

        /// <summary>
        /// 任选第四位走势
        /// </summary>
        private void Import_RX4(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_RX4IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var rx1 = int.Parse(winRed[3]);
            var zhilist = new int[] { 1, 2, 3, 5, 7, 11 };
            var zhihe = string.Empty;
            if (zhilist.Contains(rx1))
                zhihe = "Z";
            else
                zhihe = "H";

            var winxiao1 = string.Empty;
            if (rx1 <= 5)
                winxiao1 = "X";
            else
                winxiao1 = "D";

            var winji1 = string.Empty;
            if (rx1 % 2 == 1)
                winji1 = "J";
            else
                winji1 = "O";

            var last = manager.QueryJX11X5_RX4();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", rx1.ToString());
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<JX11X5_RX4>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO_"))
                {
                    var order = p.Name.Replace("NO_", string.Empty);
                    lastValue = rx1 == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("DX_"))
                {
                    var order = p.Name.Replace("DX_", string.Empty);
                    lastValue = order == winxiao1 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("JO_"))
                {
                    var order = p.Name.Replace("JO_", string.Empty);
                    lastValue = order == winji1 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("ZH_"))
                {
                    var order = p.Name.Replace("ZH_", string.Empty);
                    lastValue = order == zhihe ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu_"))
                {
                    var order = p.Name.Replace("Yu_", string.Empty);
                    lastValue = rx1 % 3 == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJX11X5_RX4(entity);
        }

        /// <summary>
        /// 任选第五位走势
        /// </summary>
        private void Import_RX5(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_RX5IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var rx1 = int.Parse(winRed[4]);
            var zhilist = new int[] { 1, 2, 3, 5, 7, 11 };
            var zhihe = string.Empty;
            if (zhilist.Contains(rx1))
                zhihe = "Z";
            else
                zhihe = "H";

            var winxiao1 = string.Empty;
            if (rx1 <= 5)
                winxiao1 = "X";
            else
                winxiao1 = "D";

            var winji1 = string.Empty;
            if (rx1 % 2 == 1)
                winji1 = "J";
            else
                winji1 = "O";

            var last = manager.QueryJX11X5_RX5();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", rx1.ToString());
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<JX11X5_RX5>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO_"))
                {
                    var order = p.Name.Replace("NO_", string.Empty);
                    lastValue = rx1 == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("DX_"))
                {
                    var order = p.Name.Replace("DX_", string.Empty);
                    lastValue = order == winxiao1 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("JO_"))
                {
                    var order = p.Name.Replace("JO_", string.Empty);
                    lastValue = order == winji1 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("ZH_"))
                {
                    var order = p.Name.Replace("ZH_", string.Empty);
                    lastValue = order == zhihe ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu_"))
                {
                    var order = p.Name.Replace("Yu_", string.Empty);
                    lastValue = rx1 % 3 == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJX11X5_RX5(entity);
        }

        /// <summary>
        /// 任选前三直选走势
        /// </summary>
        private void Import_Q3ZS(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_Q3ZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var win1 = string.Empty;
            var win2 = string.Empty;
            var win3 = string.Empty;
            if (int.Parse(winRed[0]) >= 6)
                win1 = "D";
            else
                win1 = "X";

            if (int.Parse(winRed[1]) >= 6)
                win2 = "D";
            else
                win2 = "X";

            if (int.Parse(winRed[2]) >= 6)
                win3 = "D";
            else
                win3 = "X";

            var arrayWin3 = new string[] { win1, win2, win3 };
            var dxType = string.Join("", arrayWin3);
            int daCount = dxType.Count(p => p == 'D');

            var winjo1 = string.Empty;
            var winjo2 = string.Empty;
            var winjo3 = string.Empty;

            if (int.Parse(winRed[0]) % 2 == 0)
                winjo1 = "O";
            else
                winjo1 = "J";

            if (int.Parse(winRed[1]) % 2 == 0)
                winjo2 = "O";
            else
                winjo2 = "J";

            if (int.Parse(winRed[2]) % 2 == 0)
                winjo3 = "O";
            else
                winjo3 = "J";

            var arrayWinjo = new string[] { winjo1, winjo2, winjo3 };
            var joType = string.Join("", arrayWinjo);
            int jiCount = joType.Count(p => p == 'J');

            var last = manager.QueryJX11X5_Q3ZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", string.Format("{0},{1},{2}", winRed[0], winRed[1], winRed[2]));
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<JX11X5_Q3ZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("WW_"))
                {
                    var order = p.Name.Replace("WW_", string.Empty);
                    lastValue = int.Parse(winRed[0]) == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("QW_"))
                {
                    var order = p.Name.Replace("QW_", string.Empty);
                    lastValue = int.Parse(winRed[1]) == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("BW_"))
                {
                    var order = p.Name.Replace("BW_", string.Empty);
                    lastValue = int.Parse(winRed[2]) == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("DXBi"))
                {
                    var order = p.Name.Replace("DXBi", string.Empty);
                    var dx = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(dx[0]) == daCount ? 0 : lastValue;
                }
                if (p.Name.StartsWith("JOBi"))
                {
                    var order = p.Name.Replace("JOBi", string.Empty);
                    var jo = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(jo[0]) == jiCount ? 0 : lastValue;
                }
                return lastValue;
            });
            manager.AddJX11X5_Q3ZS(entity);
        }

        /// <summary>
        /// 任选前三组选走势
        /// </summary>
        private void Import_Q3ZUS(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_Q3ZUSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var qian3 = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]) };
            var win1 = string.Empty;
            var win2 = string.Empty;
            var win3 = string.Empty;
            if (int.Parse(winRed[0]) >= 6)
                win1 = "D";
            else
                win1 = "X";

            if (int.Parse(winRed[1]) >= 6)
                win2 = "D";
            else
                win2 = "X";

            if (int.Parse(winRed[2]) >= 6)
                win3 = "D";
            else
                win3 = "X";

            var arrayWin3 = new string[] { win1, win2, win3 };
            var dxType = string.Join("", arrayWin3);
            int daCount = dxType.Count(p => p == 'D');

            var winjo1 = string.Empty;
            var winjo2 = string.Empty;
            var winjo3 = string.Empty;

            if (int.Parse(winRed[0]) % 2 == 0)
                winjo1 = "O";
            else
                winjo1 = "J";

            if (int.Parse(winRed[1]) % 2 == 0)
                winjo2 = "O";
            else
                winjo2 = "J";

            if (int.Parse(winRed[2]) % 2 == 0)
                winjo3 = "O";
            else
                winjo3 = "J";

            var arrayWinjo = new string[] { winjo1, winjo2, winjo3 };
            var joType = string.Join("", arrayWinjo);
            int jiCount = joType.Count(p => p == 'J');

            var winzh1 = string.Empty;
            var winzh2 = string.Empty;
            var winzh3 = string.Empty;

            var zhilist = new int[] { 1, 2, 3, 5, 7 };

            if (zhilist.Contains(int.Parse(winRed[0])))
                winzh1 = "Z";
            else
                winzh1 = "H";
            if (zhilist.Contains(int.Parse(winRed[1])))
                winzh2 = "Z";
            else
                winzh2 = "H";
            if (zhilist.Contains(int.Parse(winRed[2])))
                winzh3 = "Z";
            else
                winzh3 = "H";

            var arrayWinwinzh3 = new string[] { win1, win2, win3 };
            var zhType = string.Join("", arrayWinwinzh3);
            int zhiCount = zhType.Count(p => p == 'Z');

            var last = manager.QueryJX11X5_Q3ZUS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", string.Format("{0},{1},{2}", winRed[0], winRed[1], winRed[2]));
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<JX11X5_Q3ZUS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Q3_"))
                {
                    var order = p.Name.Replace("Q3_", string.Empty);
                    lastValue = qian3.Contains(int.Parse(order)) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("DXBi"))
                {
                    var order = p.Name.Replace("DXBi", string.Empty);
                    var dx = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(dx[0]) == daCount ? 0 : lastValue;
                }
                if (p.Name.StartsWith("JOBi"))
                {
                    var order = p.Name.Replace("JOBi", string.Empty);
                    var jo = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(jo[0]) == jiCount ? 0 : lastValue;
                }
                if (p.Name.StartsWith("ZHBi"))
                {
                    var order = p.Name.Replace("ZHBi", string.Empty);
                    var zh = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(zh[0]) == zhiCount ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJX11X5_Q3ZUS(entity);
        }

        /// <summary>
        /// 任选前三大小走势
        /// </summary>
        private void Import_Q3DX(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_Q3DXIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var qian3 = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]) };
            var win1 = string.Empty;
            var win2 = string.Empty;
            var win3 = string.Empty;
            if (int.Parse(winRed[0]) >= 6)
                win1 = "D";
            else
                win1 = "X";

            if (int.Parse(winRed[1]) >= 6)
                win2 = "D";
            else
                win2 = "X";

            if (int.Parse(winRed[2]) >= 6)
                win3 = "D";
            else
                win3 = "X";

            var arrayWin3 = new string[] { win1, win2, win3 };
            var dxType = string.Join("", arrayWin3);
            int daCount = dxType.Count(p => p == 'D');


            var last = manager.QueryJX11X5_Q3DX();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", string.Format("{0},{1},{2}", winRed[0], winRed[1], winRed[2]));
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<JX11X5_Q3DX>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1_"))
                {
                    var order = p.Name.Replace("NO1_", string.Empty);
                    lastValue = win1 == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2_"))
                {
                    var order = p.Name.Replace("NO2_", string.Empty);
                    lastValue = win2 == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3_"))
                {
                    var order = p.Name.Replace("NO3_", string.Empty);
                    lastValue = win3 == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("DX_"))
                {
                    var order = p.Name.Replace("DX_", string.Empty);
                    if (order == "DDD" && order == dxType)
                        lastValue = 0;
                    if (order == "DDX" && order == dxType)
                        lastValue = 0;
                    if (order == "DXD" && order == dxType)
                        lastValue = 0;
                    if (order == "XDD" && order == dxType)
                        lastValue = 0;
                    if (order == "DXX" && order == dxType)
                        lastValue = 0;
                    if (order == "XDX" && order == dxType)
                        lastValue = 0;
                    if (order == "XXD" && order == dxType)
                        lastValue = 0;
                    if (order == "XXX" && order == dxType)
                        lastValue = 0;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var dx = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(dx[0]) == daCount ? 0 : lastValue;
                }
                return lastValue;
            });
            manager.AddJX11X5_Q3DX(entity);
        }

        /// <summary>
        /// 任选前三奇偶走势
        /// </summary>
        private void Import_Q3JO(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_Q3JOIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var qian3 = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]) };
            var win1 = string.Empty;
            var win2 = string.Empty;
            var win3 = string.Empty;

            if (qian3[0] % 2 == 0)
                win1 = "O";
            else
                win1 = "J";

            if (qian3[1] % 2 == 0)
                win2 = "O";
            else
                win2 = "J";

            if (qian3[2] % 2 == 0)
                win3 = "O";
            else
                win3 = "J";

            var arrayWin3 = new string[] { win1, win2, win3 };
            var joType = string.Join("", arrayWin3);
            int jiCount = joType.Count(p => p == 'J');

            var last = manager.QueryJX11X5_Q3JO();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", string.Format("{0},{1},{2}", winRed[0], winRed[1], winRed[2]));
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<JX11X5_Q3JO>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1_"))
                {
                    var order = p.Name.Replace("NO1_", string.Empty);
                    lastValue = win1 == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2_"))
                {
                    var order = p.Name.Replace("NO2_", string.Empty);
                    lastValue = win2 == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3_"))
                {
                    var order = p.Name.Replace("NO3_", string.Empty);
                    lastValue = win3 == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("JO_"))
                {
                    var order = p.Name.Replace("JO_", string.Empty);
                    if (order == "JJJ" && order == joType)
                        lastValue = 0;
                    if (order == "JJO" && order == joType)
                        lastValue = 0;
                    if (order == "JOJ" && order == joType)
                        lastValue = 0;
                    if (order == "OJJ" && order == joType)
                        lastValue = 0;
                    if (order == "JOO" && order == joType)
                        lastValue = 0;
                    if (order == "OJO" && order == joType)
                        lastValue = 0;
                    if (order == "OOJ" && order == joType)
                        lastValue = 0;
                    if (order == "OOO" && order == joType)
                        lastValue = 0;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var jo = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(jo[0]) == jiCount ? 0 : lastValue;
                }
                return lastValue;
            });
            manager.AddJX11X5_Q3JO(entity);
        }

        /// <summary>
        /// 任选前三质和走势
        /// </summary>
        private void Import_Q3ZH(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_Q3ZHIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var qian3 = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]), int.Parse(winRed[2]) };
            var win1 = string.Empty;
            var win2 = string.Empty;
            var win3 = string.Empty;

            var zhilist = new int[] { 1, 2, 3, 5, 7 };

            if (zhilist.Contains(qian3[0]))
                win1 = "Z";
            else
                win1 = "H";
            if (zhilist.Contains(qian3[1]))
                win2 = "Z";
            else
                win2 = "H";
            if (zhilist.Contains(qian3[2]))
                win3 = "Z";
            else
                win3 = "H";

            var arrayWin3 = new string[] { win1, win2, win3 };
            var zhType = string.Join("", arrayWin3);
            int zhiCount = zhType.Count(p => p == 'Z');

            var last = manager.QueryJX11X5_Q3ZH();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", string.Format("{0},{1},{2}", winRed[0], winRed[1], winRed[2]));
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<JX11X5_Q3ZH>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1_"))
                {
                    var order = p.Name.Replace("NO1_", string.Empty);
                    lastValue = win1 == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2_"))
                {
                    var order = p.Name.Replace("NO2_", string.Empty);
                    lastValue = win2 == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3_"))
                {
                    var order = p.Name.Replace("NO3_", string.Empty);
                    lastValue = win3 == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("ZH_"))
                {
                    var order = p.Name.Replace("ZH_", string.Empty);
                    if (order == "ZZZ" && order == zhType)
                        lastValue = 0;
                    if (order == "ZZH" && order == zhType)
                        lastValue = 0;
                    if (order == "ZHZ" && order == zhType)
                        lastValue = 0;
                    if (order == "HZZ" && order == zhType)
                        lastValue = 0;
                    if (order == "ZHH" && order == zhType)
                        lastValue = 0;
                    if (order == "HZH" && order == zhType)
                        lastValue = 0;
                    if (order == "HHZ" && order == zhType)
                        lastValue = 0;
                    if (order == "HHH" && order == zhType)
                        lastValue = 0;
                }
                if (p.Name.StartsWith("Bi"))
                {
                    var order = p.Name.Replace("Bi", string.Empty);
                    var zhi = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(zhi[0]) == zhiCount ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJX11X5_Q3ZH(entity);
        }

        /// <summary>
        /// 任选前三除3走势
        /// </summary>
        private void Import_Q3Chu3(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_Q3Chu3IssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var chu3Arrary = new int[] { int.Parse(winRed[0]) % 3, int.Parse(winRed[1]) % 3, int.Parse(winRed[2]) % 3 };
            var count_0 = chu3Arrary.Count(p => p == 0);
            var count_1 = chu3Arrary.Count(p => p == 1);
            var count_2 = chu3Arrary.Count(p => p == 2);

            var last = manager.QueryJX11X5_Q3Chu3();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", winNumber);
            dic.Add("CreateTime", DateTime.Now);
            dic.Add("Chu3Bi", string.Format("{0}:{1}:{2}", chu3Arrary[0], chu3Arrary[1], chu3Arrary[2]));

            var entity = this.CreateNewEntity<JX11X5_Q3Chu3>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("NO1_"))
                {
                    var order = p.Name.Replace("NO1_", string.Empty);
                    lastValue = chu3Arrary[0] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO2_"))
                {
                    var order = p.Name.Replace("NO2_", string.Empty);
                    lastValue = chu3Arrary[1] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("NO3_"))
                {
                    var order = p.Name.Replace("NO3_", string.Empty);
                    lastValue = chu3Arrary[2] == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu0_"))
                {
                    var order = p.Name.Replace("Yu0_", string.Empty);
                    lastValue = int.Parse(order) == count_0 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu1_"))
                {
                    var order = p.Name.Replace("Yu1_", string.Empty);
                    lastValue = int.Parse(order) == count_1 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu2_"))
                {
                    var order = p.Name.Replace("Yu2_", string.Empty);
                    lastValue = int.Parse(order) == count_2 ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJX11X5_Q3Chu3(entity);
        }

        /// <summary>
        /// 前3和值走势
        /// </summary>
        private void Import_Q3HZ(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_Q3HZIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]) + int.Parse(winRed[2]);
            var hw = hz % 10;
            var yu = hz % 3;

            var last = manager.QueryJX11X5_Q3HZ();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", string.Format("{0},{1},{2}", winRed[0], winRed[1], winRed[2]));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<JX11X5_Q3HZ>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("HZ_"))
                {
                    var order = p.Name.Replace("HZ_", string.Empty);
                    lastValue = hz == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("HW_"))
                {
                    var order = p.Name.Replace("HW_", string.Empty);
                    lastValue = hw == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu_"))
                {
                    var order = p.Name.Replace("Yu_", string.Empty);
                    lastValue = yu == int.Parse(order) ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJX11X5_Q3HZ(entity);
        }

        /// <summary>
        /// 任选前三直选走势
        /// </summary>
        private void Import_Q2ZS(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_Q2ZSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var win1 = string.Empty;
            var win2 = string.Empty;
            if (int.Parse(winRed[0]) >= 6)
                win1 = "D";
            else
                win1 = "X";

            if (int.Parse(winRed[1]) >= 6)
                win2 = "D";
            else
                win2 = "X";

            var arrayWin3 = new string[] { win1, win2 };
            var dxType = string.Join("", arrayWin3);
            int daCount = dxType.Count(p => p == 'D');

            var winjo1 = string.Empty;
            var winjo2 = string.Empty;

            if (int.Parse(winRed[0]) % 2 == 0)
                winjo1 = "O";
            else
                winjo1 = "J";

            if (int.Parse(winRed[1]) % 2 == 0)
                winjo2 = "O";
            else
                winjo2 = "J";

            var arrayWinjo = new string[] { winjo1, winjo2 };
            var joType = string.Join("", arrayWinjo);
            int jiCount = joType.Count(p => p == 'J');

            var last = manager.QueryJX11X5_Q2ZS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", string.Format("{0},{1}", winRed[0], winRed[1]));
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<JX11X5_Q2ZS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("WW_"))
                {
                    var order = p.Name.Replace("WW_", string.Empty);
                    lastValue = int.Parse(winRed[0]) == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("QW_"))
                {
                    var order = p.Name.Replace("QW_", string.Empty);
                    lastValue = int.Parse(winRed[1]) == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("DXBi"))
                {
                    var order = p.Name.Replace("DXBi", string.Empty);
                    var da = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(da[0]) == daCount ? 0 : lastValue;
                }
                if (p.Name.StartsWith("JOBi"))
                {
                    var order = p.Name.Replace("JOBi", string.Empty);
                    var ji = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(ji[0]) == jiCount ? 0 : lastValue;
                }
                return lastValue;
            });
            manager.AddJX11X5_Q2ZS(entity);
        }

        /// <summary>
        /// 任选前2组选走势
        /// </summary>
        private void Import_Q2ZUS(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_Q2ZUSIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var qian3 = new int[] { int.Parse(winRed[0]), int.Parse(winRed[1]) };
            var chu3Arrary = new int[] { int.Parse(winRed[0]) % 3, int.Parse(winRed[1]) % 3 };
            var count_0 = chu3Arrary.Count(p => p == 0);
            var count_1 = chu3Arrary.Count(p => p == 1);
            var count_2 = chu3Arrary.Count(p => p == 2);

            #region 大小 奇偶 质和

            var win1 = string.Empty;
            var win2 = string.Empty;
            if (int.Parse(winRed[0]) >= 6)
                win1 = "D";
            else
                win1 = "X";

            if (int.Parse(winRed[1]) >= 6)
                win2 = "D";
            else
                win2 = "X";


            var arrayWin3 = new string[] { win1, win2 };
            var dxType = string.Join("", arrayWin3);
            int daCount = dxType.Count(p => p == 'D');

            var winjo1 = string.Empty;
            var winjo2 = string.Empty;
            if (int.Parse(winRed[0]) % 2 == 0)
                winjo1 = "O";
            else
                winjo1 = "J";

            if (int.Parse(winRed[1]) % 2 == 0)
                winjo2 = "O";
            else
                winjo2 = "J";


            var arrayWinjo = new string[] { winjo1, winjo2 };
            var joType = string.Join("", arrayWinjo);
            int jiCount = joType.Count(p => p == 'J');

            var winzh1 = string.Empty;
            var winzh2 = string.Empty;

            var zhilist = new int[] { 1, 2, 3, 5, 7, 11 };

            if (zhilist.Contains(int.Parse(winRed[0])))
                winzh1 = "Z";
            else
                winzh1 = "H";
            if (zhilist.Contains(int.Parse(winRed[1])))
                winzh2 = "Z";
            else
                winzh2 = "H";

            var arrayWinwinzh3 = new string[] { winzh1, winzh2 };
            var zhType = string.Join("", arrayWinwinzh3);
            int zhiCount = zhType.Count(p => p == 'Z');

            #endregion

            var last = manager.QueryJX11X5_Q2ZUS();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", string.Format("{0},{1}", winRed[0], winRed[1]));
            dic.Add("CreateTime", DateTime.Now);

            var entity = this.CreateNewEntity<JX11X5_Q2ZUS>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("Q2_"))
                {
                    var order = p.Name.Replace("Q2_", string.Empty);
                    lastValue = qian3.Contains(int.Parse(order)) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu0_"))
                {
                    var order = p.Name.Replace("Yu0_", string.Empty);
                    lastValue = int.Parse(order) == count_0 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu1_"))
                {
                    var order = p.Name.Replace("Yu1_", string.Empty);
                    lastValue = int.Parse(order) == count_1 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu2_"))
                {
                    var order = p.Name.Replace("Yu2_", string.Empty);
                    lastValue = int.Parse(order) == count_2 ? 0 : lastValue;
                }
                if (p.Name.StartsWith("DXBi"))
                {
                    var order = p.Name.Replace("DXBi", string.Empty);
                    var dx = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(dx[0]) == daCount ? 0 : lastValue;
                }
                if (p.Name.StartsWith("JOBi"))
                {
                    var order = p.Name.Replace("JOBi", string.Empty);
                    var jo = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(jo[0]) == jiCount ? 0 : lastValue;
                }
                if (p.Name.StartsWith("ZHBi"))
                {
                    var order = p.Name.Replace("ZHBi", string.Empty);
                    var zh = order.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    lastValue = int.Parse(zh[0]) == zhiCount ? 0 : lastValue;
                }
                return lastValue;
            });

            manager.AddJX11X5_Q2ZUS(entity);
        }

        /// <summary>
        /// 前3和值走势
        /// </summary>
        private void Import_Q2HZ(string issuseNumber, string winNumber)
        {
            var manager = new JX11X5_Manager();
            var issuse = manager.QueryJX11X5_Q2HZIssuseNumber(issuseNumber);
            if (issuse > 0)
                return;

            var winRed = winNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var hz = int.Parse(winRed[0]) + int.Parse(winRed[1]);
            var hw = hz % 10;
            var yu = hz % 3;

            var winji = string.Empty;
            if (hz % 2 == 1)
                winji = "J";
            else
                winji = "O";

            var winda = string.Empty;
            if (hz >= 12)
                winda = "D";
            else
                winda = "X";

            var zhilist = new int[] { 1, 2, 3, 5, 7 };
            var winzhi = string.Empty;
            if (zhilist.Contains(hz))
                winzhi = "Z";
            else
                winzhi = "H";

            var last = manager.QueryJX11X5_Q2HZ();
            var dic = new Dictionary<string, object>();
            dic.Add("IssuseNumber", issuseNumber);
            dic.Add("WinNumber", string.Format("{0},{1}", winRed[0], winRed[1]));
            dic.Add("CreateTime", DateTime.Now);
            var entity = this.CreateNewEntity<JX11X5_Q2HZ>(dic, (p) =>
            {
                //取上一期的数据+1
                var lastValue = (last == null ? 1 : int.Parse(p.GetValue(last, null).ToString()) + 1);
                //判断有没有开出此号
                if (p.Name.StartsWith("HZ_"))
                {
                    var order = p.Name.Replace("HZ_", string.Empty);
                    lastValue = hz == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("HW_"))
                {
                    var order = p.Name.Replace("HW_", string.Empty);
                    lastValue = hw == int.Parse(order) ? 0 : lastValue;
                }
                if (p.Name.StartsWith("DX_"))
                {
                    var order = p.Name.Replace("DX_", string.Empty);
                    lastValue = winda == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("JO_"))
                {
                    var order = p.Name.Replace("JO_", string.Empty);
                    lastValue = winji == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("ZH_"))
                {
                    var order = p.Name.Replace("ZH_", string.Empty);
                    lastValue = winzhi == order ? 0 : lastValue;
                }
                if (p.Name.StartsWith("Yu_"))
                {
                    var order = p.Name.Replace("Yu_", string.Empty);
                    lastValue = yu == int.Parse(order) ? 0 : lastValue;
                }

                return lastValue;
            });

            manager.AddJX11X5_Q2HZ(entity);
        }

        #endregion

        /// <summary>
        /// 查询开奖数据
        /// </summary>
        public GameWinNumber_InfoCollection QueryJX11X5_GameWinNumber(int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new JX11X5_GameWinNumberManager().QueryJX11X5_GameWinNumber(pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_GameWinNumber>, JX11X5_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryJX11X5_GameWinNumber_{0}_{1}", pageIndex, pageSize);
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new JX11X5_GameWinNumberManager().QueryJX11X5_GameWinNumber(pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_GameWinNumber>, JX11X5_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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
        public GameWinNumber_InfoCollection QueryJX11X5_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new JX11X5_GameWinNumberManager().QueryJX11X5_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_GameWinNumber>, JX11X5_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;

            //string key = string.Format("QueryJX11X5_GameWinNumber_{0}_{1}_{2}_{3}", pageIndex, pageSize, startTime.ToString("yyyyMMdd"), endTime.ToString("yyyyMMdd"));
            //return QueryNewWinNumber(key, () =>
            //{
            //    GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            //    var totalCount = 0;
            //    var infoList = new List<GameWinNumber_Info>();
            //    var entityList = new JX11X5_GameWinNumberManager().QueryJX11X5_GameWinNumber(startTime, endTime, pageIndex, pageSize, out totalCount);

            //   ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_GameWinNumber>, JX11X5_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
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


        public GameWinNumber_InfoCollection QueryJX11X5_GameWinNumberDesc(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            GameWinNumber_InfoCollection collection = new GameWinNumber_InfoCollection();
            var totalCount = 0;
            var infoList = new List<GameWinNumber_Info>();
            var entityList = new JX11X5_GameWinNumberManager().QueryJX11X5_GameWinNumberDesc(startTime, endTime, pageIndex, pageSize, out totalCount);

           ObjectConvert.ConvertEntityListToInfoList<List<JX11X5_GameWinNumber>, JX11X5_GameWinNumber, List<GameWinNumber_Info>, GameWinNumber_Info>(entityList, ref infoList,
                () => { return new GameWinNumber_Info(); },
                (entity, info) =>
                {
                    //处理info里面有，页entity里面没有的属性
                    //info.WinNumber = entity.WinNumber;
                });
            collection.TotalCount = totalCount;
            collection.List.AddRange(infoList);
            return collection;
        }

        public GameWinNumber_Info QueryWinNumber(string issuseNumber)
        {
            var manager = new JX11X5_GameWinNumberManager();
            var entity = manager.QueryWinNumber(issuseNumber);
            if (entity == null) return new GameWinNumber_Info();
            var info = new GameWinNumber_Info();
           ObjectConvert.ConverEntityToInfo<JX11X5_GameWinNumber, GameWinNumber_Info>(entity, ref info);
            return info;
        }
    }
}
