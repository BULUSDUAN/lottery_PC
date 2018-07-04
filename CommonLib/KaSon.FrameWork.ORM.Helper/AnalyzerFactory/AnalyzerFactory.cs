using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EntityModel.Interface;
using KaSon.FrameWork.ORM.Helper.AnalyzerFactory.Model;

namespace KaSon.FrameWork.ORM.Helper.AnalyzerFactory
{
    /// <summary>
    /// 分析器工厂
    /// </summary>
    public static class AnalyzerFactory
    {
        /// <summary>
        /// 获取订单分析器
        /// </summary>
        /// <param name="gameCode">彩种</param>
        /// <returns></returns>
        public static IOrderAnalyzable GetOrderAnalyzer(string gameCode, string gameType = "")
        {
            switch (gameCode)
            {
                case "CTZQ":
                    switch (gameType)
                    {
                        case "T14C":
                            return new GameAnalyzer_传统足球(gameCode) { EffectiveAnteCode = "310", EffectiveWinNumber = "310", IsEnableWildcard = true, BallNumber = 14, TotalNumber = 14, };
                        case "TR9":
                            return new GameAnalyzer_传统足球(gameCode) { EffectiveAnteCode = "310", EffectiveWinNumber = "310", IsEnableWildcard = true, BallNumber = 9, TotalNumber = 14, };
                        case "T6BQC":
                            return new GameAnalyzer_传统足球(gameCode) { EffectiveAnteCode = "310", EffectiveWinNumber = "310", IsEnableWildcard = true, TotalNumber = 12, BallNumber = 12, };
                        case "T4CJQ":
                            return new GameAnalyzer_传统足球(gameCode) { EffectiveAnteCode = "0123", EffectiveWinNumber = "0123", IsEnableWildcard = true, TotalNumber = 8, BallNumber = 8, };
                        default:
                            throw new ArgumentOutOfRangeException("不支持的彩种：" + gameCode + "-" + gameType);
                    }
                case "CQSSC":
                case "JXSSC":
                    return new GameAnalyzer_时时彩(gameCode);
                case "SD11X5":
                case "GD11X5":
                case "JX11X5":
                case "CQ11X5":
                case "LN11X5":
                    return new GameAnalyzer_十一选五(gameCode);
                case "SDQYH":
                    return new GameAnalyzer_群英会(gameCode);
                case "GDKLSF":
                    return new GameAnalyzer_广东快乐十分(gameCode);
                case "GXKLSF":
                    return new GameAnalyzer_广西快乐十分(gameCode);
                case "FC3D":
                    return new GameAnalyzer_时时彩(gameCode) { TotalNumber = 3 };
                case "PL3":
                    return new GameAnalyzer_时时彩(gameCode) { TotalNumber = 3 };
                case "SSQ":
                    return new GameAnalyzer_大盘数字彩(gameCode);
                case "DLT":
                    if (gameType.StartsWith("12X2"))
                    {
                        return new GameAnalyzer_十二选二(gameCode);
                    }
                    else
                    {
                        return new GameAnalyzer_大盘数字彩(gameCode) { MinNumber_First = 1, MaxNumber_First = 35, MinNumber_Last = 1, MaxNumber_Last = 12, TotalNumber_First = 5, TotalNumber_Last = 2 };
                    }
                case "QXC":
                    return new GameAnalyzer_时时彩(gameCode) { TotalNumber = 7 };
                case "QLC":
                    return new GameAnalyzer_大盘数字彩(gameCode) { MinNumber_First = 1, MaxNumber_First = 30, MinNumber_Last = 1, MaxNumber_Last = 30, TotalNumber_First = 7, TotalNumber_Last = 1 };
                case "JSKS":
                    return new GameAnalyzer_江苏快3(gameCode);
                case "SDKLPK3":
                    return new GameAnalyzer_快乐扑克3(gameCode);
                default:
                    throw new ArgumentOutOfRangeException("不支持的彩种：" + gameCode);
            }
        }
        /// <summary>
        /// 获取中奖号码分析器
        /// </summary>
        /// <param name="gameCode">彩种</param>
        /// <returns></returns>
        public static IWinNumberAnalyzable GetWinNumberAnalyzer(string gameCode, string gameType = "")
        {
            switch (gameCode)
            {
                case "CTZQ":
                    switch (gameType)
                    {
                        case "T14C":     // 胜负14场
                            return new GameAnalyzer_传统足球(gameCode) { EffectiveAnteCode = "310", EffectiveWinNumber = "310", IsEnableWildcard = true, BallNumber = 14, TotalNumber = 14, };
                        case "TR9":      // 任9
                            return new GameAnalyzer_传统足球(gameCode) { EffectiveAnteCode = "310", EffectiveWinNumber = "310", IsEnableWildcard = true, BallNumber = 9, TotalNumber = 14, };
                        case "T6BQC":    // 6场半全场
                            return new GameAnalyzer_传统足球(gameCode) { EffectiveAnteCode = "310", EffectiveWinNumber = "310", IsEnableWildcard = true, TotalNumber = 12, BallNumber = 12, };
                        case "T4CJQ":    // 4场进球
                            return new GameAnalyzer_传统足球(gameCode) { EffectiveAnteCode = "0123", EffectiveWinNumber = "0123", IsEnableWildcard = true, TotalNumber = 8, BallNumber = 8, };
                        default:
                            throw new ArgumentOutOfRangeException("不支持的彩种：" + gameCode + "-" + gameType);
                    }
                case "PL5":
                case "CQSSC":
                case "JXSSC":
                    return new GameAnalyzer_时时彩(gameCode);
                case "SD11X5":
                case "GD11X5":
                case "JX11X5":
                case "CQ11X5":
                case "LN11X5":
                    return new GameAnalyzer_十一选五(gameCode);
                case "HD15X5":
                case "SDQYH":
                    return new GameAnalyzer_群英会(gameCode);
                case "HNKLSF":
                case "CQKLSF":
                case "GDKLSF":
                    return new GameAnalyzer_广东快乐十分(gameCode);
                case "GXKLSF":
                    return new GameAnalyzer_广西快乐十分(gameCode);
                case "FC3D":
                    return new GameAnalyzer_时时彩(gameCode) { TotalNumber = 3 };
                case "HC1":
                    return new GameAnalyzer_好彩一(gameCode) { TotalNumber = 1 };
                case "PL3":
                    return new GameAnalyzer_时时彩(gameCode) { TotalNumber = 3 };
                case "SSQ":
                    return new GameAnalyzer_大盘数字彩(gameCode);
                case "DLT":
                    if (gameType.StartsWith("12X2"))
                    {
                        return new GameAnalyzer_十二选二(gameCode);
                    }
                    else
                    {
                        return new GameAnalyzer_大盘数字彩(gameCode) { MinNumber_First = 1, MaxNumber_First = 35, MinNumber_Last = 1, MaxNumber_Last = 12, TotalNumber_First = 5, TotalNumber_Last = 2 };
                    }
                case "QXC":
                    return new GameAnalyzer_时时彩(gameCode) { TotalNumber = 7 };
                case "QLC":
                    return new GameAnalyzer_大盘数字彩(gameCode) { MinNumber_First = 1, MaxNumber_First = 30, MinNumber_Last = 1, MaxNumber_Last = 30, TotalNumber_First = 7, TotalNumber_Last = 1 };
                case "JSKS":
                case "JLK3":
                case "HBK3":
                    return new GameAnalyzer_江苏快3(gameCode);
                case "DF6J1":
                    return new GameAnalyzer_数字彩(gameCode) { MinNumber_First = 0, MaxNumber_First = 9, MinNumber_Last = 1, MaxNumber_Last = 12, TotalNumber_First = 6, TotalNumber_Last = 1 };
                case "SDKLPK3":
                    return new GameAnalyzer_快乐扑克3(gameCode);
                default:
                    throw new ArgumentOutOfRangeException("不支持的彩种：" + gameCode);
            }
        }
        /// <summary>
        /// 获取投注号码分析器
        /// </summary>
        /// <param name="gameCode">彩种</param>
        /// <param name="gameType">玩法</param>
        /// <returns></returns>
        public static IAntecodeAnalyzable GetAntecodeAnalyzer(string gameCode, string gameType)
        {
            gameCode = gameCode.ToUpper();
            gameType = gameType.ToUpper();
            switch (gameCode)
            {
                #region 重庆时时彩

                case "CQSSC":
                    switch (gameType)
                    {
                        case "1XDX":    // 一星单选
                            return new GameTypeAnalyzer_单式() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 1, };
                        case "2XDX":    // 二星单选
                            return new GameTypeAnalyzer_单式() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 2, };
                        case "3XDX":    // 三星直选
                            return new GameTypeAnalyzer_单式() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 3, };
                        case "5XDX":    // 五星直选
                            return new GameTypeAnalyzer_单式() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 5, };
                        case "5XTX":    // 五星通选
                            return new GameTypeAnalyzer_五星通选() { GameCode = gameCode, GameType = gameType, };
                        case "DXDS":    // 大小单双
                            return new GameTypeAnalyzer_大小单双() { GameCode = gameCode, GameType = gameType, };
                        case "2XHZ":    // 二星和值
                            return new GameTypeAnalyzer_和值() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 2, MinValue = 0, MaxValue = 9, };
                        case "3XHZ":    // 三星和值
                            return new GameTypeAnalyzer_和值() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 3, MinValue = 0, MaxValue = 9, };
                        case "ZX3DS":   // 组三单式
                            return new GameTypeAnalyzer_组三单式() { GameCode = gameCode, GameType = gameType };
                        case "ZX3FS":   // 组三复式
                            return new GameTypeAnalyzer_组三复式() { GameCode = gameCode, GameType = gameType };
                        case "ZX6":     // 组选六
                            return new GameTypeAnalyzer_组选六() { GameCode = gameCode, GameType = gameType };
                        case "2XBAODAN":   // 二星组选包胆
                            return new GameTypeAnalyzer_包胆() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 2, };
                        case "3XBAODAN":   // 三星组选包胆
                            return new GameTypeAnalyzer_包胆() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 3, };
                        case "2XBAODIAN":   // 二星组选包点
                            return new GameTypeAnalyzer_包点() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 2, MinValue = 0, MaxValue = 9, };
                        case "3XBAODIAN":   // 三星组选包点
                            return new GameTypeAnalyzer_包点() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 3, MinValue = 0, MaxValue = 9, };
                        case "2XZXDS":   // 二星组选复式
                        case "2XZXFS":   // 二星组选复式
                            return new GameTypeAnalyzer_二星组选复式() { GameCode = gameCode, GameType = gameType, };
                        case "2XZXFW":   // 二星组选分位
                            return new GameTypeAnalyzer_二星组选分位() { GameCode = gameCode, GameType = gameType, };
                        case "3XZXZH":   // 三星直选组合
                            return new GameTypeAnalyzer_三星直选组合() { GameCode = gameCode, GameType = gameType, };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                #region 江西时时彩

                case "JXSSC":
                    switch (gameType)
                    {
                        case "1XDX":
                            return new GameTypeAnalyzer_单式() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 1, };
                        case "2XDX":
                            return new GameTypeAnalyzer_单式() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 2, };
                        case "3XDX":
                            return new GameTypeAnalyzer_单式() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 3, };
                        case "4XDX":
                            return new GameTypeAnalyzer_单式() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 4, };
                        case "5XDX":
                            return new GameTypeAnalyzer_单式() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 5, };
                        case "5XTX":    // 五星通选
                            return new GameTypeAnalyzer_五星通选() { GameCode = gameCode, GameType = gameType, };
                        case "DXDS":    // 大小单双
                            return new GameTypeAnalyzer_大小单双() { GameCode = gameCode, GameType = gameType, };
                        case "ZX3DS":   // 组三单式
                            return new GameTypeAnalyzer_组三单式() { GameCode = gameCode, GameType = gameType };
                        case "ZX3FS":   // 组三复式
                            return new GameTypeAnalyzer_组三复式() { GameCode = gameCode, GameType = gameType };
                        case "ZX6":     // 组选六
                            return new GameTypeAnalyzer_组选六() { GameCode = gameCode, GameType = gameType };
                        case "2XHZ":    // 二星和值
                            return new GameTypeAnalyzer_和值() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 2, MinValue = 0, MaxValue = 9, };
                        case "2XBAODIAN":   // 二星组选包点
                            return new GameTypeAnalyzer_包点() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 2, MinValue = 0, MaxValue = 9, };
                        case "2XZX":   // 二星组选
                            return new GameTypeAnalyzer_二星组选复式() { GameCode = gameCode, GameType = gameType, };
                        case "RX1":   // 任选一
                            return new GameTypeAnalyzer_时时彩任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 1, };
                        case "RX2":   // 任选二
                            return new GameTypeAnalyzer_时时彩任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 2, };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                #region 山东十一选五、广东十一选五、江西十一选五、重庆十一选五、辽宁十一选五

                case "SD11X5":
                case "GD11X5":
                case "JX11X5":
                case "CQ11X5":
                case "LN11X5":
                    switch (gameType)
                    {
                        case "RX1":
                            return new GameTypeAnalyzer_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 1, };
                        case "RX2":
                            return new GameTypeAnalyzer_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 2, };
                        case "RX3":
                            return new GameTypeAnalyzer_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 3, };
                        case "RX4":
                            return new GameTypeAnalyzer_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 4, };
                        case "RX5":
                            return new GameTypeAnalyzer_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 5, };
                        case "RX6":
                            return new GameTypeAnalyzer_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 6, };
                        case "RX7":
                            return new GameTypeAnalyzer_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 7, };
                        case "RX8":
                            return new GameTypeAnalyzer_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 8, };
                        case "Q2ZHIX":
                            return new GameTypeAnalyzer_前N直选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 2, };
                        case "Q3ZHIX":
                            return new GameTypeAnalyzer_前N直选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 3, };
                        case "Q2ZUX":
                            return new GameTypeAnalyzer_前N组选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 2, };
                        case "Q3ZUX":
                            return new GameTypeAnalyzer_前N组选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 3, };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                #region 山东群英会

                case "SDQYH":
                    switch (gameType)
                    {
                        case "RX1":
                            return new GameTypeAnalyzer_群英会任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 1, };
                        case "RX2":
                            return new GameTypeAnalyzer_群英会任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 2, };
                        case "RX3":
                            return new GameTypeAnalyzer_群英会任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 3, };
                        case "RX4":
                            return new GameTypeAnalyzer_群英会任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 4, };
                        case "RX5":
                            return new GameTypeAnalyzer_群英会任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 5, };
                        case "RX6":
                            return new GameTypeAnalyzer_群英会任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 6, };
                        case "RX7":
                            return new GameTypeAnalyzer_群英会任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 7, };
                        case "RX8":
                            return new GameTypeAnalyzer_群英会任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 8, };
                        case "RX9":
                            return new GameTypeAnalyzer_群英会任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 9, };
                        case "RX10":
                            return new GameTypeAnalyzer_群英会任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 10, };
                        case "W2":
                            return new GameTypeAnalyzer_前N组选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 2, };
                        case "W3":
                            return new GameTypeAnalyzer_前N组选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 3, };
                        case "W4":
                            return new GameTypeAnalyzer_前N组选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 4, };
                        case "S1":
                            return new GameTypeAnalyzer_前N直选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 1, };
                        case "S2":
                            return new GameTypeAnalyzer_前N直选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 2, };
                        case "S3":
                            return new GameTypeAnalyzer_前N直选() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 3, };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                #region 广东快乐十分

                case "GDKLSF":
                    switch (gameType)
                    {
                        case "X1ST":    // 选一数投
                            return new GameTypeAnalyzer_选一投() { GameCode = gameCode, GameType = gameType, TotalNumber = 8, BallNumber = 1, MinNumber = 1, MaxNumber = 18, IsRed = false, };
                        case "X1HT":     // 选一红投
                            return new GameTypeAnalyzer_选一投() { GameCode = gameCode, GameType = gameType, TotalNumber = 8, BallNumber = 1, MaxBallNumber = 1, MinNumber = 19, MaxNumber = 20, IsRed = true, };
                        case "RX2"://任选二
                            return new GameTypeAnalyzer_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 8, BallNumber = 2, };
                        case "RX3"://任选三
                            return new GameTypeAnalyzer_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 8, BallNumber = 3, };
                        case "RX4"://任选四
                            return new GameTypeAnalyzer_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 8, BallNumber = 4, };
                        case "RX5"://任选五
                            return new GameTypeAnalyzer_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 8, BallNumber = 5, };
                        case "X2LZHI"://选二连直
                            return new GameTypeAnalyzer_选N连直() { GameCode = gameCode, GameType = gameType, TotalNumber = 8, BallNumber = 2, };
                        case "X2LZU"://选二连组
                            return new GameTypeAnalyzer_选N连组() { GameCode = gameCode, GameType = gameType, TotalNumber = 8, BallNumber = 2, };
                        case "X3QZHI"://选三连直
                            return new GameTypeAnalyzer_前N直选() { GameCode = gameCode, GameType = gameType, TotalNumber = 8, BallNumber = 3, };
                        case "X3QZU"://选三连组
                            return new GameTypeAnalyzer_前N组选() { GameCode = gameCode, GameType = gameType, TotalNumber = 8, BallNumber = 3, };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                #region 广西快乐十分

                case "GXKLSF":
                    switch (gameType)
                    {
                        case "ZXHYT":
                            return new GameTypeAnalyzer_直选好运() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 1, IsLast = true, };
                        case "ZXHY1":    // 直选好运一
                            return new GameTypeAnalyzer_直选好运() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 1, };
                        case "ZXHY2":
                            return new GameTypeAnalyzer_直选好运() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 2, };
                        case "ZXHY3":
                            return new GameTypeAnalyzer_直选好运() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 3, };
                        case "ZXHY4":
                            return new GameTypeAnalyzer_直选好运() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 4, };
                        case "ZXHY5":
                            return new GameTypeAnalyzer_直选好运() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 5, };
                        case "TXHY3":
                            return new GameTypeAnalyzer_通选好运() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 3, };
                        case "TXHY4":
                            return new GameTypeAnalyzer_通选好运() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 4, };
                        case "TXHY5":
                            return new GameTypeAnalyzer_通选好运() { GameCode = gameCode, GameType = gameType, TotalNumber = 5, BallNumber = 5, };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                #region 福彩3D、排列三

                case "FC3D":
                case "PL3":
                    switch (gameType)
                    {
                        case "DS":
                        case "FS":
                            return new GameTypeAnalyzer_单式() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 3, };
                        case "HZ":
                            return new GameTypeAnalyzer_和值() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 3, MinValue = 0, MaxValue = 9, };
                        case "ZXHZ":
                            return new GameTypeAnalyzer_组选和值() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 3, MinValue = 0, MaxValue = 9, };
                        case "ZX3DS":   // 组三单式
                            return new GameTypeAnalyzer_组三单式() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, };
                        case "ZX3FS":   // 组三复式
                            return new GameTypeAnalyzer_组三复式() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, };
                        case "ZX6DS":   // 组六单式
                            return new GameTypeAnalyzer_组选六() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, };
                        case "ZX6":     // 组选六
                            return new GameTypeAnalyzer_组选六() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                #region 双色球

                case "SSQ":
                    switch (gameType)
                    {
                        case "DS":
                            return new GameTypeAnalyzer_分段单式() { GameCode = gameCode, GameType = gameType, MaxBallTotalLenght = 7, MinBallTotalLenght = 7, };
                        case "FS":
                            return new GameTypeAnalyzer_分段单式() { GameCode = gameCode, GameType = gameType, MinTotalNumber_First = 6, MaxTotalNumber_First = 20, MaxTotalNumber_Last = 16, MaxBallTotalLenght = 36, MinBallTotalLenght = 8, };
                        case "DT":
                            return new GameTypeAnalyzer_分段胆拖() { GameCode = gameCode, GameType = gameType, };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                #region 大乐透

                case "DLT":
                    switch (gameType)
                    {
                        case "DS":
                            return new GameTypeAnalyzer_分段单式() { GameCode = gameCode, GameType = gameType, BallNumber_First = 5, BallNumber_Last = 2, MaxNumber_First = 35, MaxNumber_Last = 12, MaxTotalNumber_First = 5, MaxTotalNumber_Last = 2, MinTotalNumber_First = 5, MinTotalNumber_Last = 2, MaxBallTotalLenght = 7, MinBallTotalLenght = 7, };
                        case "FS":
                            return new GameTypeAnalyzer_分段单式() { GameCode = gameCode, GameType = gameType, BallNumber_First = 5, BallNumber_Last = 2, MaxNumber_First = 35, MaxNumber_Last = 12, MaxTotalNumber_First = 18, MaxTotalNumber_Last = 12, MinTotalNumber_First = 5, MinTotalNumber_Last = 2, MaxBallTotalLenght = 30, MinBallTotalLenght = 8, };
                        case "DT":
                            return new GameTypeAnalyzer_分段胆拖() { GameCode = gameCode, GameType = gameType, NeedLastDan = true, BallNumber_First = 5, BallNumber_Last = 2, MaxLength_First = 35, MinLength_First = 6, MaxLength_Last = 12, MinLength_Last = 2, MaxNumber_First_Dan = 35, MinNumber_First_Dan = 1, MaxNumber_First_Tuo = 35, MinNumber_First_Tuo = 1, MaxNumber_Last_Dan = 12, MinNumber_Last_Dan = 1, MaxNumber_Last_Tuo = 12, MinNumber_Last_Tuo = 1, MaxTotalNumber_First_Dan = 4, MinTotalNumber_First_Dan = 1, MaxTotalNumber_First_Tuo = 35, MinTotalNumber_First_Tuo = 2, MaxTotalNumber_Last_Dan = 1, MinTotalNumber_Last_Dan = 0, MaxTotalNumber_Last_Tuo = 12, MinTotalNumber_Last_Tuo = 2, };
                        case "12X2DS":
                            return new GameTypeAnalyzer_任选() { GameCode = gameCode, GameType = gameType, BallNumber = 2, TotalNumber = 12, MinCount = 2, MaxCount = 2, };
                        case "12X2FS":
                            return new GameTypeAnalyzer_任选() { GameCode = gameCode, GameType = gameType, BallNumber = 2, TotalNumber = 12, MinCount = 3, MaxCount = 12, };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                #region 江苏快3

                case "JSKS":
                    //http://www.kuaicaile.com/rule/jsk3.jsp
                    switch (gameType)
                    {
                        case "HZ"://和值
                            return new GameTypeAnalyzer_江苏快3和值() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 3, MinValue = 1, MaxValue = 6, };
                        case "3THTX"://三同号通选
                            return new GameTypeAnalyzer_江苏快3三同三连() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 1, MinValue = 1, MaxValue = 6, };
                        case "3THDX"://三同号单选
                            return new GameTypeAnalyzer_江苏快3三同三连() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 1, MinValue = 1, MaxValue = 6, };
                        case "2THFX"://二同号复选
                            return new GameTypeAnalyzer_江苏快3二同号单复() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 3, MinValue = 1, MaxValue = 6 };
                        case "2THDX"://二同号单选
                            return new GameTypeAnalyzer_江苏快3二同号单复() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 3, MinValue = 1, MaxValue = 6 };
                        case "3BTH"://三不同号
                            return new GameTypeAnalyzer_江苏快3三不同号() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 3, MinValue = 1, MaxValue = 6 };
                        case "3BTHDT"://三不同号单选
                            return new GameTypeAnalyzer_江苏快3三不同号() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 3, MinValue = 1, MaxValue = 6 };
                        case "2BTH"://二不同号
                            return new GameTypeAnalyzer_江苏快3二不同号() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 2, MinValue = 1, MaxValue = 6 };
                        case "2BTHDT"://二不同号单选
                            return new GameTypeAnalyzer_江苏快3二不同号() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 2, MinValue = 1, MaxValue = 6 };
                        case "3LHTX"://三连号通选
                            return new GameTypeAnalyzer_江苏快3三同三连() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 1, MinValue = 1, MaxValue = 6, };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                #region 山东快乐扑克3

                case "SDKLPK3":
                    switch (gameType)
                    {
                        case "TH"://同花
                        case "SZ"://顺子
                        case "THS"://同花顺
                        case "DZ"://对子
                        case "BZ"://豹子
                            return new GameTypeAnalyzer_快乐扑克3() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 3, MinValue = 1, MaxValue = 3 };
                        case "RX1"://任选一
                            return new GameTypeAnalyzer_快乐扑克3_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 1, };
                        case "RX2"://任选二
                            return new GameTypeAnalyzer_快乐扑克3_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 2, };
                        case "RX3"://任选三
                            return new GameTypeAnalyzer_快乐扑克3_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 3, };
                        case "RX4"://任选四
                            return new GameTypeAnalyzer_快乐扑克3_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 4, };
                        case "RX5"://任选五
                            return new GameTypeAnalyzer_快乐扑克3_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 5, };
                        case "RX6"://任选六
                            return new GameTypeAnalyzer_快乐扑克3_任选() { GameCode = gameCode, GameType = gameType, TotalNumber = 3, BallNumber = 6, };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }


                #endregion

                #region 传统足球

                case "CTZQ":
                    switch (gameType)
                    {
                        case "T14C":
                            return new GameTypeAnalyzer_传统足球() { GameCode = gameCode, GameType = gameType, TotalNumber = 14, BallNumber = 14, IsEnableWildcard = true, };
                        case "TR9":
                            return new GameTypeAnalyzer_传统足球() { GameCode = gameCode, GameType = gameType, TotalNumber = 14, BallNumber = 9, IsEnableWildcard = true, };
                        case "T6BQC":
                            return new GameTypeAnalyzer_传统足球() { GameCode = gameCode, GameType = gameType, TotalNumber = 12, BallNumber = 12, IsEnableWildcard = true, };
                        case "T4CJQ":
                            return new GameTypeAnalyzer_传统足球() { GameCode = gameCode, GameType = gameType, TotalNumber = 8, BallNumber = 8, IsEnableWildcard = true, };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                #region 竞猜世界杯冠军 和 冠亚军

                case "JCSJBGJ":
                    switch (gameType)
                    {
                        case "JCSJBGJ":
                            return new GameTypeAnalyzer_竞彩世界杯冠军() { GameCode = gameCode, GameType = gameType, BallNumber = 1, };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }
                case "JCYJ":
                    switch (gameType)
                    {
                        case "JCYJ":
                            return new GameTypeAnalyzer_竞彩世界杯冠亚军() { GameCode = gameCode, GameType = gameType, BallNumber = 1, };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                #region 欧洲杯

                case "OZB":
                    switch (gameType)
                    {
                        case "GJ":
                            return new GameTypeAnalyzer_欧洲杯冠军();
                        case "GYJ":
                            return new GameTypeAnalyzer_欧洲杯冠亚军();
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                #region 世界杯

                case "SJB":
                    switch (gameType)
                    {
                        case "GJ":
                            return new GameTypeAnalyzer_世界杯冠军();
                        case "GYJ":
                            return new GameTypeAnalyzer_世界杯冠亚军();
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                default:
                    throw new NotSupportedException("不支持的彩种：" + gameCode);
            }
        }
        public static IAntecodeAnalyzable_Sport GetSportAnalyzer(string gameCode, string gameType, int baseCount)
        {
            switch (gameCode.ToUpper())
            {
                case "BJDC":    // 北京单场
                case "JCZQ":    // 竞彩足球
                    return new GameTypeAnalyzer_过关足球() { GameCode = gameCode, GameType = gameType, BaseCount = baseCount };
                case "JCLQ":    // 竞彩蓝球
                    switch (gameType.ToUpper())
                    {
                        case "RFSF":
                        case "DXF":
                        case "HH":
                            return new GameTypeAnalyzer_过关足球() { GameCode = gameCode, GameType = gameType, BaseCount = baseCount, IsResultFromAnteCode = true };
                        default:
                            return new GameTypeAnalyzer_过关足球() { GameCode = gameCode, GameType = gameType, BaseCount = baseCount };
                    }
                default:
                    throw new NotSupportedException("不支持的彩种：" + gameCode);
            }
        }
        public static IAnteCodeChecker_Sport GetSportAnteCodeChecker(string gameCode, string gameType)
        {
            switch (gameCode.ToUpper())
            {
                #region 北京单场

                case "BJDC":

                    switch (gameType.ToUpper())
                    {
                        case "SF":     // 胜负
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "3,0", EffectiveWinNumber = "3,0", };
                        case "SPF":     // 胜平负
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "3,1,0", EffectiveWinNumber = "3,1,0", };
                        case "ZJQ":     // 进球数
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "0,1,2,3,4,5,6,7", EffectiveWinNumber = "0,1,2,3,4,5,6,7", };
                        case "SXDS":    // 上下单双。上单 3；上双 2；下单 1；下双 0；
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "SD,SS,XD,XS", EffectiveWinNumber = "SD,SS,XD,XS", };
                        case "BF":      // 比分
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "10,20,30,40,21,31,41,32,42,X0,00,11,22,33,XX,01,02,03,04,12,13,14,23,24,0X", EffectiveWinNumber = "10,20,30,40,21,31,41,32,42,X0,00,11,22,33,XX,01,02,03,04,12,13,14,23,24,0X", };
                        case "BQC":     // 半全场
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "33,31,30,13,11,10,03,01,00", EffectiveWinNumber = "33,31,30,13,11,10,03,01,00", };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                #region 竞彩足球

                case "JCZQ":

                    switch (gameType.ToUpper())
                    {
                        case "SPF":     // 胜平负
                        case "BRQSPF": //不让球胜平负
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "3,1,0", EffectiveWinNumber = "3,1,0", };
                        case "ZJQ":     // 进球数
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "0,1,2,3,4,5,6,7", EffectiveWinNumber = "0,1,2,3,4,5,6,7", };
                        case "BF":      // 比分
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "10,20,30,40,21,31,41,32,42,50,51,52,X0,00,11,22,33,XX,01,02,03,04,12,13,14,23,24,05,15,25,0X", EffectiveWinNumber = "10,20,30,40,21,31,41,32,42,50,51,52,X0,00,11,22,33,XX,01,02,03,04,12,13,14,23,24,05,15,25,0X", };
                        case "BQC":     // 半全场
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "33,31,30,13,11,10,03,01,00", EffectiveWinNumber = "33,31,30,13,11,10,03,01,00", };
                        case "HH":     // 混合过关
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "", EffectiveWinNumber = "", };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                #region 竞彩篮球

                case "JCLQ":

                    switch (gameType.ToUpper())
                    {
                        case "SF":     // 胜负
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "3,0", EffectiveWinNumber = "3,0", };
                        case "RFSF":     // 让分胜负
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "3,0", EffectiveWinNumber = "3,0", };
                        case "SFC":      // 胜分差
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "01,02,03,04,05,06,11,12,13,14,15,16", EffectiveWinNumber = "01,02,03,04,05,06,11,12,13,14,15,16", };
                        case "DXF":     // 大小分
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "3,0", EffectiveWinNumber = "3,0", };
                        case "HH":     // 混合过关
                            return new GameAnalyzer_过关足球(gameCode) { GameType = gameType, EffectiveAnteCode = "", EffectiveWinNumber = "", };
                        default:
                            throw new NotSupportedException("不支持的玩法：" + gameCode + " - " + gameType);
                    }

                #endregion

                default:
                    throw new NotSupportedException("不支持的彩种：" + gameCode);
            }
        }
        public static int GetHitCount(string anteNumber, string winNumber)
        {
            var hitCount = 0;
            var antecodeList = anteNumber.Split('|')[0].Split(',');
            var winList = winNumber.Split(',');
            if (antecodeList.Length != winList.Length)
            {
                return -1;
            }
            for (int i = 0; i < antecodeList.Length; i++)
            {
                if (winList[i] == "*")
                {
                    hitCount++;
                    continue;
                }
                if (antecodeList[i].Contains(winList[i]))
                {
                    hitCount++;
                    continue;
                }
            }
            return hitCount;
        }

        /// <summary>
        /// 验证单式上传的投注号码是否正确
        /// </summary>
        public static List<string> CheckSingleSchemeAnteCode(string anteCodeText, string playType, bool containsMatchId, string[] selectMatchIdArray, string[] allowCodes, out List<string> matchIdList)
        {
            matchIdList = new List<string>();
            if (allowCodes.Length == 0)
                throw new Exception("允许投注的号码不为能空");
            //allowCodes = (string.Join(",", allowCodes) + ",*").Split(',');

            string placeholder = string.Empty;
            placeholder = placeholder.PadLeft(allowCodes[0].Length, '*');
            allowCodes = (string.Join(",", allowCodes) + "," + placeholder + "").Split(',');

            var passLEN = 0;
            var playTypeArray = playType.Split('_');
            if (playTypeArray.Length != 2)
                throw new Exception("串关方式不正确: " + playType);
            passLEN = int.Parse(playTypeArray[0]);
            var passName = playType == "1_1" ? "单关" : playType == "0_1" ? "混合过关" : playType.Replace("_", "串");

            //每一个号码的长度
            var numberLength = allowCodes[0].Length;
            var list = anteCodeText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var formatList = new List<string>();
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Contains(",") || list[i].Contains(" ") || passLEN == 1)
                {
                    formatList.Add(list[i]);
                }
                else
                {
                    List<string> tmps = new List<string>();
                    for (int j = 0; j < list[i].Length; j += numberLength)
                    {
                        tmps.Add(list[i].Substring(j, numberLength));

                    }
                    formatList.Add(string.Join(",", tmps));
                }
            }
            var hhList = new List<string>();
            var curPlayType = string.Empty;
            for (int i = 0; i < formatList.Count; i++)
            {
                var itemcodes = formatList[i].Split(new string[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);

                //上传文件里不包含场次信息
                if (!containsMatchId)
                {
                    //判断号码是否包含与场次数相同的号码
                    if (itemcodes.Length != selectMatchIdArray.Length)
                        throw new Exception("第" + (i + 1) + "行[" + formatList[i] + "]应该包含" + selectMatchIdArray.Length + "场比赛，实际包含" + itemcodes.Length + "场比赛");

                    //混合过关组合号码数组
                    if (passLEN == 0)
                    {
                        //混合过关判断号码里是否包含非法号码
                        if (itemcodes.Where(a => !allowCodes.Contains(a)).ToList().Count > 0)
                            throw new Exception("第" + (i + 1) + "行[" + formatList[i] + "]包含非法号码");
                    }
                    //非混合过关号码判断
                    else
                    {
                        //非混合过关判断投注号码过关方式
                        if (itemcodes.Where(a => !a.Contains("*")).ToList().Count != passLEN)
                            throw new Exception("第" + (i + 1) + "行[" + formatList[i] + "]不是" + passName);
                        //非混合过关判断号码里是否包含非法号码
                        if (itemcodes.Where(a => !allowCodes.Contains(a)).Count() > 0)
                            throw new Exception("第" + (i + 1) + "行[" + formatList[i] + "]包含非法号码");
                    }

                    #region 格式化投注号码，拼凑成投注格式
                    //当前行投注号码的过关方式
                    curPlayType = playType == "0_1" ? (itemcodes.Where(a => !a.Contains("*")).ToList().Count + "_1") : playType;
                    var tmpNum1 = new List<string>();
                    for (int j = 0; j < itemcodes.Length; j++)
                    {
                        string num = string.Format("{0}|{1}|{2}", selectMatchIdArray[j], itemcodes[j], curPlayType);
                        tmpNum1.Add(num);
                    }
                    hhList.Add(string.Join("#", tmpNum1));
                    #endregion
                    continue;
                }

                //判断每组号码是否包含场次
                if (itemcodes.Where(a => a.Contains("→")).ToList().Count != itemcodes.Length)
                    throw new Exception("第" + (i + 1) + "行[" + formatList[i] + "]格式错误，每注号码应该包含场次分隔符→");

                //非混合过关判断投注号码过关方式
                if (passLEN > 0)
                {
                    //非混合过关判断投注号码过关方式
                    if (itemcodes.Where(a => a != "*").ToList().Count != passLEN)
                        throw new Exception("第" + (i + 1) + "行[" + formatList[i] + "]不是" + passName);
                }

                var query = from s in itemcodes select s.Split('→')[0];
                if (query.ToList().Distinct().Count() != itemcodes.Length)
                    throw new Exception("场次重复,上传文件时每注号码的场次不能重复");
                //当前行投注号码的过关方式
                curPlayType = playType == "0_1" ? (itemcodes.Length + "_1") : playType;
                var tmpNum2 = new List<string>();
                //判断每组号码的场次信息是否合法以及投注号码是否合法
                for (int j = 0; j < itemcodes.Length; j++)
                {
                    var itemCodeArray = itemcodes[j].Split('→');
                    var matchId = itemCodeArray[0];
                    if (!matchIdList.Contains(matchId))
                        matchIdList.Add(matchId);

                    //PreconditionAssert.IsTrue(allowMatch.Contains(matchId), "第" + (i + 1) + "行[" + formatList[i] + "]所包含的比赛场次第[" + matchId + "]场不是本期比赛");
                    var ante = itemCodeArray[1];
                    if (!allowCodes.Contains(ante))
                        throw new Exception("第" + (i + 1) + "行[" + formatList[i] + "]的投注号码非法，应该在[" + string.Join(",", allowCodes) + "]范围内");

                    //拼凑投注号码
                    string num = string.Format("{0}|{1}|{2}", matchId, ante, curPlayType);
                    tmpNum2.Add(num);
                }
                hhList.Add(string.Join("#", tmpNum2));
            }

            return hhList;
        }

        /// <summary>
        /// 验证传统足球单式上传号码是否正确
        /// </summary>
        public static List<string> CheckCTZQSingleSchemeAnteCode(string anteCodeText, string gameType, string[] allowCodes, out List<string> matchIdList)
        {
            var codeList = new List<string>();
            var codeArray = anteCodeText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var mustLength = 0;
            var starsCount = 0;
            matchIdList = new List<string>();
            switch (gameType)
            {
                case "T14C":
                    mustLength = 14;
                    break;
                case "TR9":
                    mustLength = 14;
                    starsCount = 5;
                    break;
                case "T6BQC":
                    mustLength = 12;
                    break;
                case "T4CJQ":
                    mustLength = 8;
                    break;
            }
            for (int i = 0; i < mustLength; i++)
            {
                matchIdList.Add((i + 1).ToString());
            }
            for (int i = 0; i < codeArray.Length; i++)
            {
                //一行号码的数组 如任九的 1*31***31031*1
                var oneLineArray = codeArray[i].ToArray();
                if (oneLineArray.Length != mustLength)
                    throw new Exception(string.Format("第{0}行的字符长度应该为{1}，实际为{2}", i + 1, mustLength, oneLineArray.Length));
                var oneLineStarsCount = 0;
                for (int j = 0; j < oneLineArray.Length; j++)
                {
                    if (!allowCodes.Contains(oneLineArray[j].ToString()))
                        throw new Exception(string.Format("第{0}行第{1}个字符{2}为非法字符", i + 1, j + 1, oneLineArray[j]));
                    if (oneLineArray[j] == '*')
                        oneLineStarsCount++;
                }
                if (oneLineStarsCount != starsCount)
                    throw new Exception(string.Format("第{0}行号码格式错误，单式只能包含{1}个*字符", i + 1, starsCount));
                codeList.Add(codeArray[i]);
            }

            return codeList;
        }
    }
}
