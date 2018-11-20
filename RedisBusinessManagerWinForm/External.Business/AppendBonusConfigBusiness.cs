using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using External.Core.AppendBonus;
using External.Domain.Managers.AppendBonusConfig;
using Common.Communication;

namespace External.Business
{
    public class AppendBonusConfigBusiness
    {
        public AppendBonusConfigInfo_QueryCollection GetAppendBonusConfigList()
        {
            return new AppendBonusConfigManager().GetAppendBonusConfigList();
        }

        public CommonActionResult UpdateAppendBonusConfig(AppendBonusConfigInfo_QueryCollection configList)
        {
            return new AppendBonusConfigManager().UpdateAppendBonusConfig(configList);
        }

        public CommonActionResult DeleteAppendBonusConfig(string gameCode)
        {
            return new AppendBonusConfigManager().DeleteAppendBonusConfig(gameCode);
        }
    }
}
