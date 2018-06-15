using KaSon.FrameWork.Helper.分析器工厂.接口;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Helper.分析器工厂.Model
{
    public class GameTypeAnalyzer_欧洲杯冠亚军 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_欧洲杯冠亚军()
        {
            Spliter = ',';
        }

        public char Spliter { get; set; }
        /// <summary>
        /// 投注号码切分后的数字列表。执行CheckAntecode或AnalyzeAnteCode或CaculateBonus以后有值
        /// </summary>
        public string[] AnteCodeNumbers { get; private set; }

        /// <summary>
        /// 检查投注号码格式是否正确
        /// </summary>
        /// <param name="antecode">投注号码</param>
        /// <param name="errMsg">错误消息</param>
        /// <returns>格式是否正确</returns>
        public bool CheckAntecode(string antecode, out string errMsg)
        {
            var contentArray = new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50" };
            AnteCodeNumbers = antecode.Split(Spliter);
            var dis = AnteCodeNumbers.Distinct().Count();
            if (AnteCodeNumbers.Length != dis)
            {
                errMsg = "投注内容中有重复的选项";
                return false;
            }
            foreach (var item in AnteCodeNumbers)
            {
                if (!(item.Length == 2))
                {
                    errMsg = string.Format("投注号码必须为2位数！");
                    return false;
                }
                if (!contentArray.Contains(item))
                {
                    errMsg = string.Format("投注号码必须在01-50之间号码！");
                    return false;
                }
            }
            errMsg = "";
            return true;
        }

        /// <summary>
        /// 分析一个投注号码，计算此号码所包含的注数
        /// </summary>
        /// <param name="antecode">投注号码</param>
        /// <returns>号码所包含的注数</returns>
        public int AnalyzeAnteCode(string antecode)
        {
            string msg;
            if (!CheckAntecode(antecode, out msg))
            {
                throw new Exception(string.Format("投注号码 {0} 格式错误 - {1}", antecode, msg));
            }
            return antecode.Split(Spliter).Length;
        }

        /// <summary>
        /// 计算投注号码的中奖状态，返回中奖的奖等列表。如果为空，表示未中奖；
        /// </summary>
        /// <param name="antecode">投注号码</param>
        /// <param name="winNumber">中奖号码</param>
        /// <returns>返回中奖的奖等列表</returns>
        public IList<int> CaculateBonus(string antecode, string winNumber)
        {
            string msg;
            if (!CheckAntecode(antecode, out msg))
                throw new Exception(string.Format("投注号码 {0} 格式错误 - {1}", antecode, msg));
            var contentArray = new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50" };
            if (!contentArray.Contains(winNumber))
                throw new Exception(string.Format("开奖号 {0} 格式不正确", winNumber));
            var bonusLevelList = new List<int>();
            if (AnteCodeNumbers.Contains(winNumber))
            {
                bonusLevelList.Add(0);
            }
            return bonusLevelList;
        }
    }
}
