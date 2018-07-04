using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.ExceptionExtend;
using EntityModel.Interface;


namespace KaSon.FrameWork.ORM.Helper.AnalyzerFactory.Model
{
    /// <summary>
    /// 玩法分析 - 传统足球
    /// </summary>
    internal class GameTypeAnalyzer_传统足球 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_传统足球()
        {
            Spliter = ',';
            Wildcard = "*";
        }
        public char Spliter { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        // 是否允许通配符
        public bool IsEnableWildcard { get; set; }
        // 通配符
        public string Wildcard { get; set; }
        // 总场数
        public int TotalNumber { get; set; }
        public int BallNumber { get; set; }
        /// <summary>
        /// 投注号码切分后的数字列表。执行CheckAntecode或AnalyzeAnteCode或CaculateBonus以后有值
        /// </summary>
        public string[] AnteCodeNumbers { get; private set; }
        public int[] DanNumbers { get; private set; }

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

            var list = antecode.Split('|');
            if (list.Length > 1)
            {
                var dans = list[1].Split(new char[] { Spliter }, StringSplitOptions.RemoveEmptyEntries);
                DanNumbers = new int[dans.Length];
                for (int i = 0; i < dans.Length; i++)
                {
                    DanNumbers[i] = int.Parse(dans[i]);
                }
                if (DanNumbers.GroupBy(i => i).Count() != DanNumbers.Length)
                {
                    errMsg = "胆码重复";
                    return false;
                }
            }
            else
            {
                DanNumbers = new int[0];
            }
            AnteCodeNumbers = list[0].Split(Spliter);
            if (AnteCodeNumbers.Length != TotalNumber)
            {
                errMsg = string.Format("投注号码必须是被\"{0}\"切分成 {1}个部分的字符串", Spliter, TotalNumber);
                return false;
            }
            var orderAnalyzer = AnalyzerFactory.GetOrderAnalyzer(GameCode, GameType);
            var tmp = AnteCodeNumbers.Clone() as string[];
            if (IsEnableWildcard)
            {
                tmp = AnteCodeNumbers.Where(a =>
                {
                    return !a.Equals(Wildcard.ToString());
                }).ToArray();
                if (tmp.Length < BallNumber || tmp.Length > TotalNumber)
                {
                    errMsg = string.Format("有效投注号码必须是\"{0} - {1}\"个号码", BallNumber, TotalNumber);
                    return false;
                }
            }
            if (tmp.Length == BallNumber && DanNumbers.Length > 0)
            {
                errMsg = "胆码设置错误 - " + antecode;
                return false;
            }
            if (DanNumbers.Length >= BallNumber)
            {
                errMsg = "胆码设置错误，胆码必须小于 " + BallNumber + "个 - " + antecode;
                return false;
            }
            foreach (var danIndex in DanNumbers)
            {
                var dan = AnteCodeNumbers[danIndex];
                if (dan == Wildcard.ToString())
                {
                    errMsg = "胆码设置错误，对应胆码为通配符 - " + antecode;
                    return false;
                }
            }
            foreach (var item in tmp)
            {
                if (!orderAnalyzer.CheckOneAntecodeNumber(item, out errMsg)) { return false; }
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
            string msg;
            if (!CheckAntecode(antecode, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, antecode, "投注号码格式错误 - " + msg);
            }
            var totalCount = 0;
            var danList = new List<string>();
            var tmp = new List<string>();
            for (int i = 0; i < TotalNumber; i++)
            {
                if (DanNumbers.Contains(i))
                {
                    danList.Add(AnteCodeNumbers[i]);
                }
                else
                {
                    if (AnteCodeNumbers[i] != Wildcard.ToString())
                    {
                        tmp.Add(AnteCodeNumbers[i]);
                    }
                }
            }
            var c = new Combination();
            c.Calculate(tmp.ToArray(), BallNumber - DanNumbers.Length, (item) =>
            {
                var count = 1;
                foreach (var t in item)
                {
                    count *= t.Length;
                }
                totalCount += count;
            });
            foreach (var dan in danList)
            {
                totalCount *= dan.Length;
            }
            return totalCount;
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
            var bonusLevelList = new List<int>();
            switch (GameType)
            {
                case "T14C":
                    CaculateBonus_14C(checker, bonusLevelList);
                    break;
                case "TR9":
                    CaculateBonus_R9(checker, bonusLevelList);
                    break;
                default:
                    CaculateBonus(checker, bonusLevelList);
                    break;
            }
            return bonusLevelList;
        }
        private void CaculateBonus_R9(IWinNumberAnalyzable checker, List<int> bonusLevelList)
        {
            var numHitCount = 0;
            var danHitCount = 0;
            var zhu = 1;

            var danList = new List<string>();
            var numList = new List<string>();
            for (int i = 0; i < TotalNumber; i++)
            {
                var ante = AnteCodeNumbers[i];
                var win = checker.WinNumbers[i];
                if (ante != Wildcard && (ante.Contains(win) || (IsEnableWildcard && win == Wildcard)))
                {
                    if (DanNumbers.Contains(i))
                    {
                        danHitCount++;
                        danList.Add(ante);
                    }
                    else
                    {
                        numHitCount++;
                        numList.Add(ante);
                    }
                    if (ante.Length > 1 && win == Wildcard)
                        zhu *= ante.Length;
                }
            }
            var hitCount = numHitCount + danHitCount;
            if (danHitCount >= DanNumbers.Length)
            {
                if (hitCount >= BallNumber)
                {
                    var c = new Combination();
                    c.Calculate(numList.ToArray(), BallNumber - DanNumbers.Length, (item) =>
                    {
                        for (int i = 0; i < zhu; i++)
                        {
                            bonusLevelList.Add(1);
                        }
                    });
                }
            }
        }
        private void CaculateBonus_14C(IWinNumberAnalyzable checker, List<int> bonusLevelList)
        {
            var hitCount = 0;
            var winCount = 0;
            var loseCount = 0;
            var zhu = 1;
            for (int i = 0; i < TotalNumber; i++)
            {
                var ante = AnteCodeNumbers[i];
                var win = checker.WinNumbers[i];
                if (ante.Contains(win) || (IsEnableWildcard && win == Wildcard))
                {
                    hitCount++;
                    if (win != Wildcard)
                        winCount += ante.Length - win.Length;
                    if (ante.Length > 1 && win == Wildcard)
                        zhu *= ante.Length;
                }
                else
                {
                    loseCount = ante.Length;
                }
            }
            if (hitCount == TotalNumber)
            {
                for (int k = 0; k < zhu; k++)
                {
                    bonusLevelList.Add(1);
                }
                //for (int j = 0; j < winCount * zhu; j++)
                //if (checker.WinNumbers.Contains(Wildcard))
                //{
                //    var count = CaculateBonus_14C_Zhong2(checker);
                //    for (int j = 0; j < count; j++)
                //    {
                //        bonusLevelList.Add(2);
                //    }
                //}
                //else
                //{
                for (int j = 0; j < winCount * zhu; j++)
                {
                    bonusLevelList.Add(2);
                }
                //}
            }
            else if (hitCount == TotalNumber - 1)
            {
                //for (int j = 0; j < loseCount * zhu; j++)
                for (int j = 0; j < loseCount * zhu; j++)
                {
                    bonusLevelList.Add(2);
                }
            }
        }

        private int CaculateBonus_14C_Zhong2(IWinNumberAnalyzable checker)
        {
            var count = 0;
            //var anteCodeArray = "310,310,310,310,310,310,310,310,310,310,310,310,310,310".Split(',');
            var strL = new List<string>();
            for (int i = 0; i < AnteCodeNumbers.Count(); i++)
            {
                var valu = AnteCodeNumbers[i];
                AnteCodeNumbers[i] = "*";
                strL.Add(string.Join(",", AnteCodeNumbers));
                AnteCodeNumbers[i] = valu;
            }
            foreach (var item in strL)
            {
                var winCount = 0;
                var zhong = 0;
                var zhu = 1;
                var anteList = item.Split(',');
                for (int i = 0; i < TotalNumber; i++)
                {
                    var ante = anteList[i];
                    if (ante == "*") continue;
                    var win = checker.WinNumbers[i];
                    if (ante.Contains(win) || (IsEnableWildcard && win == Wildcard))
                    {
                        zhong++;
                        winCount += ante.Length - win.Length;
                        if (ante.Length > 1 && win == Wildcard)
                            zhu *= ante.Length;
                    }
                }

                if (zhong == TotalNumber - 1)
                {
                    for (int i = 0; i < zhu * winCount; i++)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private void CaculateBonus(IWinNumberAnalyzable checker, List<int> bonusLevelList)
        {
            var hitCount = 0;
            for (int i = 0; i < TotalNumber; i++)
            {
                var ante = AnteCodeNumbers[i];
                var win = checker.WinNumbers[i];
                if (ante.Contains(win) || (IsEnableWildcard && win == Wildcard))
                {
                    hitCount++;
                }
            }
            if (hitCount == TotalNumber)
            {
                bonusLevelList.Add(1);
            }
        }
    }
}
