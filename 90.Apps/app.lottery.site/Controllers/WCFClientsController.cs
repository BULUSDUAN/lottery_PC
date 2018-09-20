using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Activity.Client;
using External.Client;
using GameBiz.Client;
using LotteryData.Client;

namespace app.lottery.site.Controllers
{
    /// <summary>
    /// WCF客户端类
    /// </summary>
    public class WCFClients
    {
        private static ExternalWcfClient _ExternalClient = null;
        /// <summary>
        /// 插件WCF客户端
        /// </summary>
        public static ExternalWcfClient ExternalClient
        {
            get
            {
                if (_ExternalClient == null)
                {
                    _ExternalClient = new ExternalWcfClient();
                }
                return _ExternalClient;
            }
        }
        private static ActivityWcfClient _ActivityClient = null;
        /// <summary>
        /// 活动WCF客户端
        /// </summary>
        public static ActivityWcfClient ActivityClient
        {
            get
            {
                if (_ActivityClient == null)
                {
                    _ActivityClient = new ActivityWcfClient();
                }
                return _ActivityClient;
            }
        }

        private static GameBizWcfClient_Core _gameBiz = null;
        /// <summary>
        /// 彩种WCF客户端
        /// </summary>
        public static GameBizWcfClient_Core GameClient
        {
            get
            {
                if (_gameBiz == null)
                {
                    _gameBiz = new GameBizWcfClient_Core();
                }
                return _gameBiz;
            }
        }

        private static GameBizWcfClient_Fund _gameFund = null;
        /// <summary>
        /// 彩种资金WCF客户端
        /// </summary>
        public static GameBizWcfClient_Fund GameFundClient
        {
            get
            {
                if (_gameFund == null)
                {
                    _gameFund = new GameBizWcfClient_Fund();
                }
                return _gameFund;
            }
        }

        private static GameBizWcfClient_Query _gameQuery = null;
        /// <summary>
        /// 查询WCF客户端
        /// </summary>
        public static GameBizWcfClient_Query GameQueryClient
        {
            get
            {
                if (_gameQuery == null)
                {
                    _gameQuery = new GameBizWcfClient_Query();
                }
                return _gameQuery;
            }
        }

        private static GameBizWcfClient_Experter _experterClient = null;
        /// <summary>
        /// 名家相关客户端
        /// </summary>
        public static GameBizWcfClient_Experter ExperterClient
        {
            get
            {
                if (_experterClient == null)
                {
                    _experterClient = new GameBizWcfClient_Experter();
                }
                return _experterClient;
            }
        }

        private static GameBizWcfClient_Issuse _gameIssuse = null;
        /// <summary>
        /// 彩种奖期WCF客户端
        /// </summary>
        public static GameBizWcfClient_Issuse GameIssuseClient
        {
            get
            {
                if (_gameIssuse == null)
                {
                    _gameIssuse = new GameBizWcfClient_Issuse();
                }
                return _gameIssuse;
            }
        }

        private static ChartWcfClient _chartClient = null;
        /// <summary>
        /// 彩票数据图表客户端
        /// </summary>
        public static ChartWcfClient ChartClient
        {
            get
            {
                if (_chartClient == null)
                {
                    _chartClient = new ChartWcfClient();
                }
                return _chartClient;
            }
        }

        public static ExternalIntegralBizWcfClient _IntegralExternalClient = null;
        /// <summary>
        /// 插件WCF客户端
        /// </summary>
        public static ExternalIntegralBizWcfClient IntegralExternalClient
        {
            get
            {
                if (_IntegralExternalClient == null)
                {
                    _IntegralExternalClient = new ExternalIntegralBizWcfClient();
                }
                return _IntegralExternalClient;
            }
        }
        
    }
}
