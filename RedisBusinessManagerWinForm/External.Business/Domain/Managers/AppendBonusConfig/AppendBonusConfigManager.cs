using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Auth.Domain.Entities;
using Common.Business;
using NHibernate.Criterion;
using External.Domain.Entities.Authentication;
using External.Core.AppendBonus;
using Common.Communication;
using External.Domain.Entities.AppendBonusConfig;
using GameBiz.Business;

namespace External.Domain.Managers.AppendBonusConfig
{
    public class AppendBonusConfigManager : GameBizEntityManagement
    {
        public AppendBonusConfigInfo_QueryCollection GetAppendBonusConfigList()
        {
            Session.Clear();
            var sql = @"SELECT distinct g.DisplayName as gameName, gt.DisplayName as gameTypeName, gt.GameCode, gt.GameType, 
                        ISNULL(abc.AppendBonusMoney, 0) as AppendBonusMoney, ISNULL(abc.AppendRatio, 0) as AppendRatio, 
                        ISNULL(abc.StartMultiple, 1) as StartMultiple, isnull(abc.ColorBeanNumber, 0) as ColorBeanNumber,
                        ISNULL(abc.ColorBeanRatio, 0) as ColorBeanRatio, ISNULL(abc.ColorBeanStartMultiple, 1) as ColorBeanStartMultiple,
                        isnull(ModifyTime, getdate()) as ModifyTime, isnull(StartIssueNumber, 0) as StartIssueNumber, 
						isnull(EndIssueNumber, 500) as EndIssueNumber, isnull(BonusMoneyStartMultiple, 0) as BonusMoneyStartMultiple 
                        FROM C_Lottery_GameType gt
                        INNER JOIN C_Lottery_Game g on g.GameCode=gt.GameCode and gt.EnableStatus=0 and g.EnableStatus=0
                        LEFT JOIN E_Activity_AppendBonusConfig abc on abc.GameCode=gt.GameCode and abc.GameType=gt.GameType
                        ORDER BY g.DisplayName, gt.DisplayName ";

            int totalCount = 0;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Core_Pager"));
            query = query.AddInParameter("sqlStr", sql);
            query = query.AddInParameter("currentPageIndex", 0);
            query = query.AddInParameter("pageSize", 1000);
            var collection = query.ToListByPaging(out totalCount);
            
            var coll = new AppendBonusConfigInfo_QueryCollection();

            var list = new List<AppendBonusConfigInfo>();
            foreach (var item in collection)
            {
                var array = item as object[];
                list.Add(new External.Core.AppendBonus.AppendBonusConfigInfo
                {
                    GameName = array[0].ToString(),
                    GameTypeName = array[1].ToString(),
                    GameCode = array[2].ToString(),
                    GameType = array[3].ToString(),
                    AppendBonusMoney = Convert.ToDecimal(array[4].ToString()),
                    AppendRatio = Convert.ToDecimal(array[5].ToString()),
                    StartMultiple = Int32.Parse(array[6].ToString()),
                    ColorBeanNumber = Int32.Parse(array[7].ToString()),
                    ColorBeanRatio = Convert.ToDecimal(array[8].ToString()),
                    ColorBeanStartMultiple = Int32.Parse(array[9].ToString()),
                    ModifyTime = Convert.ToDateTime(array[10].ToString()),

                    StartIssueNumber = Int32.Parse(array[11].ToString()),
                    EndIssueNumber = Int32.Parse(array[12].ToString()),
                    BonusMoneyStartMultiple = Int32.Parse(array[13].ToString()),
                });
            }

            coll.ConfigList = list;

            return coll;
        }

        public CommonActionResult UpdateAppendBonusConfig(AppendBonusConfigInfo_QueryCollection collection)
        {
            bool result = true;
            string strMsg = string.Empty;
            CommonActionResult resultObj = null;
            var configList = collection.ConfigList;

            Session.Clear();

            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                foreach (var item in configList)
                {
                    try
                    {
                        var configAll = Session.CreateCriteria<External.Domain.Entities.AppendBonusConfig.AppendBonusConfig>()
                            .Add(Restrictions.Eq("GameCode", item.GameCode))
                            .Add(Restrictions.Eq("GameType", item.GameType))
                            .List<External.Domain.Entities.AppendBonusConfig.AppendBonusConfig>();

                        if (configAll.Count == 0)
                        {
                            var config = new External.Domain.Entities.AppendBonusConfig.AppendBonusConfig
                            {
                                GameCode = item.GameCode,
                                GameType = item.GameType,
                                AppendBonusMoney = item.AppendBonusMoney,
                                AppendRatio = item.AppendRatio,
                                StartMultiple = item.StartMultiple,
                                ColorBeanNumber = item.ColorBeanNumber,
                                ColorBeanRatio = item.ColorBeanRatio,
                                ColorBeanStartMultiple = item.ColorBeanStartMultiple,
                                ModifyTime = DateTime.Now,

                                StartIssueNumber = item.StartIssueNumber,
                                EndIssueNumber = item.EndIssueNumber,
                                BonusMoneyStartMultiple = item.BonusMoneyStartMultiple
                            };

                            Add<External.Domain.Entities.AppendBonusConfig.AppendBonusConfig>(config);
                        }
                        else
                        {
                            var config = configAll[0];

                            config.AppendBonusMoney = item.AppendBonusMoney;
                            config.AppendRatio = item.AppendRatio;
                            config.StartMultiple = item.StartMultiple;
                            config.ColorBeanNumber = item.ColorBeanNumber;
                            config.ColorBeanRatio = item.ColorBeanRatio;
                            config.ColorBeanStartMultiple = item.ColorBeanStartMultiple;
                            config.ModifyTime = DateTime.Now;

                            config.StartIssueNumber = item.StartIssueNumber;
                            config.EndIssueNumber = item.EndIssueNumber;
                            config.BonusMoneyStartMultiple = item.BonusMoneyStartMultiple;

                            Update<External.Domain.Entities.AppendBonusConfig.AppendBonusConfig>(config);
                        }
                    }
                    catch
                    {
                        result = false;
                        strMsg += item.GameName + "-" + item.GameTypeName;

                        break;
                    }
                }

                if (result == false)
                {
                    biz.RollbackTran();

                    resultObj = new CommonActionResult(result, strMsg + ":加奖配置失败!");
                }
                else
                {
                    biz.CommitTran();

                    resultObj = new CommonActionResult(result, "加奖配置成功!");
                }
            }

            return resultObj;
        }

        public CommonActionResult DeleteAppendBonusConfig(string gameCode)
        {
            string strMsg = string.Empty;
            CommonActionResult resultObj = null;

            Session.Clear();

            int iDelete = Session.CreateQuery(string.Format("delete AppendBonusConfig c where c.GameCode='{0}' ", gameCode)).ExecuteUpdate();

            if (iDelete > 0)
            {
                resultObj = new CommonActionResult(true, "清除加奖配置成功!");
            }
            else
            {
                resultObj = new CommonActionResult(false, "清除加奖配置失败!");
            }

            return resultObj;
        }

        public External.Domain.Entities.AppendBonusConfig.AppendBonusConfig GetAppendBonusConfig(string gameCode, string gameType)
        {
            var configList = Session.CreateCriteria<External.Domain.Entities.AppendBonusConfig.AppendBonusConfig>()
                            .Add(Restrictions.Eq("GameCode", gameCode))
                            .Add(Restrictions.Eq("GameType", gameType))
                            .List<External.Domain.Entities.AppendBonusConfig.AppendBonusConfig>();

            if (configList.Count == 0)
            {
                return null;
            }
            else
            {
                return configList[0];
            }
        }
    }
}
