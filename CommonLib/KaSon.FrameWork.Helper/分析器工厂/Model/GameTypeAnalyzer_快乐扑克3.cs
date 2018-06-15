using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.Helper.分析器工厂.接口;


namespace KaSon.FrameWork.Helper.分析器工厂.Model
{
    /// <summary>
    /// 处理同花、顺子、同花顺、对子、豹子玩法
    /// </summary>
    internal class GameTypeAnalyzer_快乐扑克3 : IAntecodeAnalyzable
    {
        public string GameCode { get; set; }
        public string GameType { get; set; }

        public int TotalNumber { get; set; }
        public int BallNumber { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
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
            // 前置验证 - 彩种、玩法、投注号码
            PreconditionAssert.IsNotEmptyString(GameCode, "检查投注号码格式前，必须设置彩种");
            PreconditionAssert.IsNotEmptyString(GameType, "检查投注号码格式前，必须设置玩法");
            PreconditionAssert.IsNotEmptyString(antecode, "必须传入非空的投注号码");
            var gameTypeArray = new string[] { "TH", "SZ", "THS", "DZ", "BZ" };
            if (!gameTypeArray.Contains(GameType))
            {
                errMsg = string.Format("玩法编码错误,必须是：{0}中的一种", string.Join(",", gameTypeArray));
                return false;
            }

            var rightNumberArray = new string[] { };
            switch (GameType)
            {
                case "TH":
                    //同花黑桃、同花红桃、同花梅花、同花方块、同花包选
                    rightNumberArray = new string[] { "TH1", "TH2", "TH3", "TH4", "THX" };
                    break;
                case "SZ":
                    //顺子1、顺子2...顺子包选
                    rightNumberArray = new string[] { "SZ1", "SZ2", "SZ3", "SZ4", "SZ5", "SZ6", "SZ7", "SZ8", "SZ9", "SZ10", "SZ11", "SZ12", "SZ13", "SZX" };
                    break;
                case "THS":
                    //同花顺黑桃、同花顺红桃、同花顺梅花、同花顺方块、同花顺包选
                    rightNumberArray = new string[] { "THS1", "THS2", "THS3", "THS4", "THSX" };
                    break;
                case "DZ":
                    //对子1、对子1...对子包选
                    rightNumberArray = new string[] { "DZ1", "DZ2", "DZ3", "DZ4", "DZ5", "DZ6", "DZ7", "DZ8", "DZ9", "DZ10", "DZ11", "DZ12", "DZ13", "DZX" };
                    break;
                case "BZ":
                    //豹子1、豹子2...豹子包选
                    rightNumberArray = new string[] { "BZ1", "BZ2", "BZ3", "BZ4", "BZ5", "BZ6", "BZ7", "BZ8", "BZ9", "BZ10", "BZ11", "BZ12", "BZ13", "BZX" };
                    break;
                default:
                    break;
            }

            if (rightNumberArray.Length == 0 || !rightNumberArray.Contains(antecode))
            {
                errMsg = string.Format("投注号码:{0} 不正确", antecode);
                return false;
            }


            errMsg = "";
            return true;
        }

        /// <summary>
        /// 分析一个投注号码，计算此号码所包含的注数。
        /// 投注号码格式错误时抛出异常AntecodeFormatException。
        /// </summary>
        /// <param name="antecode">投注号码</param>
        /// <returns>号码所包含的注数</returns>
        public int AnalyzeAnteCode(string antecode)
        {
            AnteCodeNumbers = antecode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in AnteCodeNumbers)
            {
                string msg;
                if (!CheckAntecode(item, out msg))
                {
                    throw new AntecodeFormatException(GameCode, GameType, antecode, "投注号码格式错误 - " + msg);
                }
            }

            return AnteCodeNumbers.Length;
        }

        /// <summary>
        /// 获取中奖注数
        /// </summary>
        public IList<int> CaculateBonus(string antecode, string winNumber)
        {
            //验证格式
            string msg;
            var checker = AnalyzerFactory.GetWinNumberAnalyzer(GameCode, GameType);
            if (!checker.CheckWinNumber(winNumber, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, winNumber, "中奖号码格式错误 - " + msg);
            }

            //计算中奖等级
            var bonusLevelList = new List<int>();
            var anteCodeArray = antecode.ToUpper().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var code in anteCodeArray)
            {
                if (!CheckAntecode(code, out msg))
                {
                    throw new AntecodeFormatException(GameCode, GameType, code, "投注号码格式错误 - " + msg);
                }

                switch (GameType)
                {
                    case "TH":
                        //同花
                        var th = GetWinNumber_TH(checker.WinNumbers);
                        if (string.IsNullOrEmpty(th))
                            break;

                        //是同花状态
                        if (code == th)
                        {
                            //选中中一等奖
                            bonusLevelList.Add(1);
                            continue;
                        }
                        if (code == "THX")
                        {
                            //包选中二等奖
                            bonusLevelList.Add(2);
                            continue;
                        }
                        break;
                    case "SZ":
                        //顺子
                        var sz = GetWinNumber_SZ(checker.WinNumbers);
                        if (string.IsNullOrEmpty(sz))
                            break;

                        //是顺子状态
                        if (code == sz)
                        {
                            //选中中一等奖
                            bonusLevelList.Add(1);
                            continue;
                        }
                        if (code == "SZX")
                        {
                            //包选中二等奖
                            bonusLevelList.Add(2);
                            continue;
                        }
                        break;
                    case "THS":
                        //同花顺
                        var ths = GetWinNumber_THS(checker.WinNumbers);
                        if (string.IsNullOrEmpty(ths))
                            break;

                        //是同花顺状态
                        if (code == ths)
                        {
                            //选中中一等奖
                            bonusLevelList.Add(1);
                            continue;
                        }
                        if (code == "THSX")
                        {
                            //包选中二等奖
                            bonusLevelList.Add(2);
                            continue;
                        }
                        break;
                    case "DZ":
                        //对子
                        var dz = GetWinNumber_DZ(checker.WinNumbers);
                        if (string.IsNullOrEmpty(dz))
                            break;

                        //是对子状态
                        if (code == dz)
                        {
                            //选中中一等奖
                            bonusLevelList.Add(1);
                            continue;
                        }
                        if (code == "DZX")
                        {
                            //包选中二等奖
                            bonusLevelList.Add(2);
                            continue;
                        }
                        break;
                    case "BZ":
                        //豹子
                        var bz = GetWinNumber_BZ(checker.WinNumbers);
                        if (string.IsNullOrEmpty(bz))
                            break;

                        //是豹子状态
                        if (code == bz)
                        {
                            //选中中一等奖
                            bonusLevelList.Add(1);
                            continue;
                        }
                        if (code == "BZX")
                        {
                            //包选中二等奖
                            bonusLevelList.Add(2);
                            continue;
                        }
                        break;
                    default:
                        break;
                }
            }

            return bonusLevelList;
        }

        /// <summary>
        /// 获取开奖号同花编码，返回空则不是同花状态
        /// </summary>
        private string GetWinNumber_TH(string[] winNumberArray)
        {
            var huaArray = winNumberArray.Select(p => p[0]).Distinct().ToArray();
            if (huaArray.Length > 1)
                return string.Empty;
            return string.Format("TH{0}", huaArray[0]);
        }

        /// <summary>
        /// 获取开奖号顺子编码，返回空则不是顺子状态
        /// </summary>
        private string GetWinNumber_SZ(string[] winNumberArray)
        {
            var numberArray = winNumberArray.Select(p => int.Parse(p.Substring(1))).OrderBy(p => p).ToArray();
            if (numberArray[0] == 1 && numberArray[1] == 12 && numberArray[2] == 13)
            {
                //特殊 AKQ
                return "SZ12";
            }
            if (numberArray[0] + 1 == numberArray[1] && numberArray[1] + 1 == numberArray[2])
            {
                //顺子状态
                return string.Format("SZ{0}", numberArray[0]);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取开奖号码同花顺编码，返回空则不是同花顺状态
        /// </summary>
        private string GetWinNumber_THS(string[] winNumberArray)
        {
            var th = GetWinNumber_TH(winNumberArray);
            if (string.IsNullOrEmpty(th))
                return string.Empty;

            var sz = GetWinNumber_SZ(winNumberArray);
            if (string.IsNullOrEmpty(sz))
                return string.Empty;

            return string.Format("THS{0}", winNumberArray[0][0]);
        }

        /// <summary>
        /// 获取开奖号码对子编码，返回空则不是对子状态
        /// </summary>
        private string GetWinNumber_DZ(string[] winNumberArray)
        {
            var group = winNumberArray.Select(p => int.Parse(p.Substring(1))).GroupBy(p => p);
            if (group.Count() == 2)
            {
                //对子状态
                foreach (var item in group.ToArray())
                {
                    if (item.Count() == 2)
                        return string.Format("DZ{0}", item.Key);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取开奖号码豹子编码，返回空则不是豹子状态
        /// </summary>
        private string GetWinNumber_BZ(string[] winNumberArray)
        {
            var group = winNumberArray.Select(p => int.Parse(p.Substring(1))).GroupBy(p => p);
            if (group.Count() == 1)
            {
                //豹子状态
                return string.Format("BZ{0}", group.ToArray()[0].Key);
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// 处理任选玩法
    /// </summary>
    internal class GameTypeAnalyzer_快乐扑克3_任选 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_快乐扑克3_任选()
        {
            Spliter = ',';
        }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public char Spliter { get; set; }
        public int TotalNumber { get; set; }
        public int BallNumber { get; set; }
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
            // 前置验证 - 彩种、玩法、投注号码
            PreconditionAssert.IsNotEmptyString(GameCode, "检查投注号码格式前，必须设置彩种");
            PreconditionAssert.IsNotEmptyString(GameType, "检查投注号码格式前，必须设置玩法");
            PreconditionAssert.IsNotEmptyString(antecode, "必须传入非空的投注号码");

            AnteCodeNumbers = antecode.Split(Spliter);

            if (AnteCodeNumbers.Length < BallNumber)
            {
                errMsg = string.Format("投注号码必须至少包含 {0}个数字", BallNumber);
                return false;
            }

            //var orderAnalyzer = AnalyzerFactory.GetOrderAnalyzer(GameCode, GameType);
            //if (!orderAnalyzer.CheckComboAntecodeNumber(antecode, ',', out errMsg))
            //{
            //    return false;
            //}
            errMsg = "";
            return true;
        }
        /// <summary>
        /// 分析一个投注号码，计算此号码所包含的注数。
        /// 投注号码格式错误时抛出异常AntecodeFormatException。
        /// </summary>
        /// <param name="antecode">投注号码</param>
        /// <returns>号码所包含的注数</returns>
        public int AnalyzeAnteCode(string antecode)
        {
            string msg;
            if (!CheckAntecode(antecode, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, antecode, "投注号码格式错误 - " + msg);
            }
            var c = new Combination();
            var i = 0;
            c.Calculate(AnteCodeNumbers, BallNumber, (num) => i++);
            return i;
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

            var f_number = AnteCodeNumbers.Select(p => int.Parse(p)).ToArray();
            var f_winNumber = checker.WinNumbers.Select(p => int.Parse(p.Substring(1))).Distinct().ToArray();

            var bonusLevelList = new List<int>();
            //var c = new Combination();
            //c.Calculate(f_number, BallNumber, (n) =>
            //{
            //    var hitCount = GetHitCount(f_winNumber, n);
            //    if (hitCount == f_winNumber.Length)
            //        bonusLevelList.Add(0);
            //});

            //return bonusLevelList;

            if (BallNumber <= TotalNumber)     // 任选的数量小于等于总号码数。如：任选一、任选二、任选三
            {
                var p = new Combination();
                p.Calculate(f_number, BallNumber, (n) =>
                {
                    var hitCount = GetHitCount(f_winNumber, n);
                    if (hitCount == BallNumber)
                        bonusLevelList.Add(0);
                });

                return bonusLevelList;
            }
            else    // 任选的数量大于等于总号码数。如：任选四、任选五、任选六
            {

                var p = new Combination();
                p.Calculate(f_number, BallNumber, (n) =>
                {
                    var hitCount = GetHitCount(f_winNumber, n);
                    if (hitCount == f_winNumber.Length)
                        bonusLevelList.Add(0);
                });

                return bonusLevelList;
            }
            //return bonusLevelList;
        }

        /// <summary>
        /// 计算命中号码个数
        /// </summary>
        private int GetHitCount(int[] winNumber, int[] anteNumber)
        {
            var c = 0;
            foreach (var win in winNumber)
            {
                if (anteNumber.Contains(win))
                    c++;
            }
            return c;
        }

    }
}
