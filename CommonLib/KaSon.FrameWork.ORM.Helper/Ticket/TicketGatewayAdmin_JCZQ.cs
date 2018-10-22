using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Ticket;
using KaSon.FrameWork.Analyzer.AnalyzerFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// BJDC
    /// </summary>
    public partial class TicketGatewayAdmin : DBbase
    {
        public void UpdateOddsList_JCZQ_Manual()
        {
            UpdateOddsList_JCZQ_Manual_SPF("JCZQ", "SPF");
            UpdateOddsList_JCZQ_Manual_BRQSPF("JCZQ", "BRQSPF");
            UpdateOddsList_JCZQ_Manual_ZJQ("JCZQ", "ZJQ");
            UpdateOddsList_JCZQ_Manual_BQC("JCZQ", "BQC");
            UpdateOddsList_JCZQ_Manual_BF("JCZQ", "BF");
        }
        public void UpdateOddsList_JCZQ_Manual_SPF(string gameCode, string gameType)
        {
            var oddsList = GetOddsList_JingCai<T_JCZQ_Odds_SPF>(gameCode, gameType, string.Empty);
            try
            {
                DB.Begin();
                var oddsManager = new JCZQ_OddsManager();
                List<T_JCZQ_Odds_SPF> entityList = new List<T_JCZQ_Odds_SPF>();
                foreach (var odds in oddsList)
                {
                    if (!odds.CheckIsValidate())
                        continue;
                    var entity = oddsManager.GetLastOdds<T_JCZQ_Odds_SPF>(gameType, odds.MatchId, false);
                    if (entity == null)
                    {

                        entity = new T_JCZQ_Odds_SPF
                        {
                            MatchId = odds.MatchId,
                            CreateTime = DateTime.Now,
                        };
                        entity.SetOdds(odds);
                        entityList.Add(entity);
                    }
                }
                if (entityList != null && entityList.Any())
                {
                    oddsManager.AddOdds(entityList);
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            
        }
        public void UpdateOddsList_JCZQ_Manual_BRQSPF(string gameCode, string gameType)
        {
            var oddsList = GetOddsList_JingCai<T_JCZQ_Odds_BRQSPF>(gameCode, gameType, string.Empty);
            try
            {
                DB.Begin();
                var oddsManager = new JCZQ_OddsManager();
                foreach (var odds in oddsList)
                {
                    if (!odds.CheckIsValidate())
                        continue;
                    var entity = oddsManager.GetLastOdds<T_JCZQ_Odds_BRQSPF>(gameType, odds.MatchId, false);
                    if (entity == null)
                    {

                        entity = new T_JCZQ_Odds_BRQSPF
                        {
                            MatchId = odds.MatchId,
                            CreateTime = DateTime.Now,
                        };
                        entity.SetOdds(odds);
                        oddsManager.AddOdds(entity);
                    }
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw;
            }
           
        }
        public void UpdateOddsList_JCZQ_Manual_ZJQ(string gameCode, string gameType)
        {
            var oddsList = GetOddsList_JingCai<T_JCZQ_Odds_ZJQ>(gameCode, gameType, string.Empty);
            try
            {
                DB.Begin();
                var oddsManager = new JCZQ_OddsManager();
                foreach (var odds in oddsList)
                {
                    if (!odds.CheckIsValidate())
                        continue;
                    var entity = oddsManager.GetLastOdds<T_JCZQ_Odds_ZJQ>(gameType, odds.MatchId, false);
                    if (entity == null)
                    {

                        entity = new T_JCZQ_Odds_ZJQ
                        {
                            MatchId = odds.MatchId,
                            CreateTime = DateTime.Now,
                        };
                        entity.SetOdds(odds);
                        oddsManager.AddOdds(entity);
                    }
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw;
            }
           
        }
        public void UpdateOddsList_JCZQ_Manual_BQC(string gameCode, string gameType)
        {
            var oddsList = GetOddsList_JingCai<T_JCZQ_Odds_BQC>(gameCode, gameType, string.Empty);
            try
            {
                DB.Begin();
                var oddsManager = new JCZQ_OddsManager();
                foreach (var odds in oddsList)
                {
                    if (!odds.CheckIsValidate())
                        continue;
                    var entity = oddsManager.GetLastOdds<T_JCZQ_Odds_BQC>(gameType, odds.MatchId, false);
                    if (entity == null)
                    {

                        entity = new T_JCZQ_Odds_BQC
                        {
                            MatchId = odds.MatchId,
                            CreateTime = DateTime.Now,
                        };
                        entity.SetOdds(odds);
                        oddsManager.AddOdds(entity);
                    }
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw;
            }
            
        }
        public void UpdateOddsList_JCZQ_Manual_BF(string gameCode, string gameType)
        {
            var oddsList = GetOddsList_JingCai<T_JCZQ_Odds_BF>(gameCode, gameType, string.Empty);
            try
            {
                DB.Begin();
                var oddsManager = new JCZQ_OddsManager();
                foreach (var odds in oddsList)
                {
                    if (!odds.CheckIsValidate())
                        continue;
                    var entity = oddsManager.GetLastOdds<T_JCZQ_Odds_BF>(gameType, odds.MatchId, false);
                    if (entity == null)
                    {

                        entity = new T_JCZQ_Odds_BF
                        {
                            MatchId = odds.MatchId,
                            CreateTime = DateTime.Now,
                        };
                        entity.SetOdds(odds);
                        oddsManager.AddOdds(entity);
                    }
                }
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw;
            }
            
        }
        public List<string> RequestTicket_JCZQSingleScheme(GatewayTicketOrder_SingleScheme order, out List<string> realMatchIdArray)
        {
            var selectMatchIdArray = order.SelectMatchId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var allowCodeArray = order.AllowCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            //var codeText = File.ReadAllText(order.AnteCodeFullFileName, Encoding.UTF8);
            var codeText = Encoding.UTF8.GetString(order.FileBuffer);
            var matchIdList = new List<string>();
            var codeList = AnalyzerFactory.CheckSingleSchemeAnteCode(codeText, order.PlayType, order.ContainsMatchId, selectMatchIdArray, allowCodeArray, out matchIdList);
            var totalMoney = codeList.Count * 2M * order.Amount;
            if (totalMoney != order.TotalMoney)
                throw new Exception(string.Format("订单金额不正确，应该为：{0}；实际为：{1}。订单号：{2}", totalMoney, order.TotalMoney, order.OrderId));
            realMatchIdArray = order.ContainsMatchId ? matchIdList : selectMatchIdArray.ToList();

            if (!new JCZQ_OddsManager().CheckAllMatchUpdatedOdds(order.GameCode, order.GameType, realMatchIdArray.ToArray()))
            {
                throw new ArgumentException("订单中有比赛未开出赔率");
            }

            var manager = new Sports_Manager();
            if (manager.QuerySingleSchemeOrder(order.OrderId) == null)
            {
                manager.AddSingleSchemeOrder(new T_SingleScheme_Order
                {
                    AllowCodes = order.AllowCodes,
                    Amount = order.Amount,
                    FileBuffer = Encoding.UTF8.GetString(order.FileBuffer),
                    //AnteCodeFullFileName = order.AnteCodeFullFileName,
                    ContainsMatchId = order.ContainsMatchId,
                    CreateTime = DateTime.Now,
                    GameCode = order.GameCode,
                    GameType = order.GameType,
                    IssuseNumber = order.IssuseNumber,
                    IsVirtualOrder = order.IsVirtualOrder,
                    OrderId = order.OrderId,
                    PlayType = order.PlayType,
                    SelectMatchId = order.SelectMatchId,
                    TotalMoney = order.TotalMoney,
                });
            }

            return codeList;
        }

    }
}
