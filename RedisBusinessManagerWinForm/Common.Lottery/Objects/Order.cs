using System.Collections.Generic;

namespace Common.Lottery.Objects
{
    /// <summary>
    /// 订单
    /// </summary>
    public interface IOrder<A>
        where A : IAntecode
    {
        /// <summary>
        /// 游戏
        /// </summary>
        string GameCode { get; set; }
        /// <summary>
        /// 投注倍数 1--99
        /// </summary>
        int Amount { get; set; }
        /// <summary>
        /// 投注号码
        /// </summary>
        List<A> AntecodeList { get; set; }
    }
    /// <summary>
    /// 订单
    /// </summary>
    public class Order
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Order()
        {
            AntecodeList = new List<Antecode>();
            Amount = 1;
        }
        /// <summary>
        /// 游戏
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 投注倍数 1--99
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 投注号码
        /// </summary>
        public List<Antecode> AntecodeList { get; set; }
        /// <summary>
        /// 添加一个投注号码到列表
        /// </summary>
        /// <param name="gameTypeName">玩法</param>
        /// <param name="anteNumber">投注号码</param>
        /// <param name="amount">倍数</param>
        public void AddAntecode(string gameTypeName, string anteNumber, int amount = 1)
        {
            var antecode = new Antecode()
            {
                GameType = gameTypeName,
                AnteNumber = anteNumber,
            };
            AntecodeList.Add(antecode);
        }
    }
}
