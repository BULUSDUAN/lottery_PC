using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Lottery
{
    /// <summary>
    /// 票的拆分规则
    /// </summary>
    public static class TicketRuleGetter
    {
        /// <summary>
        /// 获取一张票能携带的最大号码数量
        /// </summary>
        /// <param name="gameCode">彩种</param>
        /// <param name="gameType">玩法</param>
        /// <returns>数量，一般是5</returns>
        public static int GetMaxAntecodeCountEachTicket(string gameCode, string gameType)
        {
            if (_getterHandler == null)
            {
                throw new SystemException("未配置票规则句柄");
            }
            return _getterHandler(gameCode, gameType);
        }
        private static Func<string, string, int> _getterHandler;
        /// <summary>
        /// 注册票规则句柄
        /// </summary>
        public static void RegisterRuleGetter(Func<string, string, int> getterHandler)
        {
            if (_getterHandler == null)
            {
                _getterHandler = getterHandler;
            }
        }
    }
}
