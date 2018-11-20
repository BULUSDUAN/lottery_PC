using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using External.Core.Login;
using GameBiz.Core;

namespace Activity.Business
{
    public class A20120915_测试阶段注册送10000测试金 : IRegister_AfterTranCommit
    {
        public void AfterRegisterTranCommit(string regType, string userId)
        {
            //GameBiz.Business.BusinessHelper.Payin_2Balance(GameBiz.Business.BusinessHelper.FundCategory_ManualFillMoney, userId, userId, false, "", "", 
            //    userId, GameBiz.Core.AccountType.Common, 20000M, "公测虚拟￥20,000.00测试金");
        }
        public object ExecPlugin(string type, object inputParam)
        {
            var paraList = inputParam as object[];
            switch (type)
            {
                case "IRegister_AfterTranCommit":
                    AfterRegisterTranCommit((string)paraList[0], (string)paraList[1]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("不支持的插件类型 - " + type);
            }
            return null;
        }
    }
}
