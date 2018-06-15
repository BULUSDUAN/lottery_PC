using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.Helper.分析器工厂.接口;

namespace KaSon.FrameWork.Helper.分析器工厂.Model
{
    /// <summary>
    /// 只处理3THDX,3THTX,3LHTX这三种玩法
    /// </summary>
    internal class GameTypeAnalyzer_江苏快3三同三连 : IAntecodeAnalyzable
    {
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public char Spliter { get; set; }

        public int TotalNumber { get; set; }
        public int BallNumber { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string[] AnteCodeNumbers { get; private set; }

        public GameTypeAnalyzer_江苏快3三同三连()
        {
            Spliter = ',';
        }

        public bool CheckAntecode(string antecode, out string errMsg)
        {
            // 前置验证 - 彩种、玩法、投注号码
            PreconditionAssert.IsNotEmptyString(GameCode, "检查投注号码格式前，必须设置彩种");
            PreconditionAssert.IsNotEmptyString(GameType, "检查投注号码格式前，必须设置玩法");
            PreconditionAssert.IsNotEmptyString(antecode, "必须传入非空的投注号码");

            AnteCodeNumbers = antecode.Split(Spliter);

            if (GameType == "3THDX")
            {
                var thdx = new string[] { "111", "222", "333", "444", "555", "666" };
                if (!thdx.Contains(antecode))
                {
                    errMsg = string.Format("投注号码格式不正确 - {0}", antecode);
                    return false;
                }
            }
            if (GameType == "3THTX" && antecode != "XXX")
            {
                errMsg = string.Format("投注号码格式不正确 - {0}", antecode);
                return false;
            }
            if (GameType == "3LHTX" && antecode != "XYZ")
            {
                errMsg = string.Format("投注号码格式不正确 - {0}", antecode);
                return false;
            }
            errMsg = "";
            return true;
        }

        public int AnalyzeAnteCode(string antecode)
        {
            string msg;
            if (!CheckAntecode(antecode, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, antecode, "投注号码格式错误 - " + msg);
            }
            return 1;
        }

        public IList<int> CaculateBonus(string antecode, string winNumber)
        {
            string msg;
            if (!CheckAntecode(antecode, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, antecode, "投注号码格式错误 - " + msg);
            }
            var checker = AnalyzerFactory.GetWinNumberAnalyzer(GameCode, GameType);
            if (!checker.CheckWinNumber(winNumber, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, winNumber, "中奖号码格式错误 - " + msg);
            }
            winNumber = string.Join(string.Empty, checker.WinNumbers);
            var bonusLevelList = new List<int>();
            switch (GameType)
            {
                case "3THDX":
                    if (winNumber == antecode)
                        bonusLevelList.Add(0);
                    break;
                case "3THTX":
                    var thdx = new string[] { "111", "222", "333", "444", "555", "666" };
                    if (thdx.Contains(winNumber))
                        bonusLevelList.Add(0);
                    break;
                case "3LHTX":
                    if (int.Parse(checker.WinNumbers[2]) - int.Parse(checker.WinNumbers[1]) == 1 && int.Parse(checker.WinNumbers[1]) - int.Parse(checker.WinNumbers[0]) == 1)
                        bonusLevelList.Add(0);
                    break;
                default:
                    break;
            }

            return bonusLevelList;
        }
    }
}
