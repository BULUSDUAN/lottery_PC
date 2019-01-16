using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaSon.FrameWork.ORM.Helper
{
    public class JSON_kaiJiang
    {
        //#region 文件路径
        ///// <summary>
        ///// 奖期数据文件
        ///// </summary>
        //private static string IssuseFile(string type, string issuseId)
        //{
        //    if (type.StartsWith("CTZQ"))
        //    {
        //        var strs = type.Split('_');
        //        var gameCode = strs[0];
        //        //var gameType = strs[1];
        //        return string.Format("/matchdata/{0}/{1}/{2}_BonusLevel.json?_={3}", gameCode, issuseId, type, DateTime.Now.ToString("yyyyMMddHHmmss"));
        //    }
        //    return string.Format("/matchdata/{0}/{0}_{1}.json?_={2}", type, issuseId, DateTime.Now.ToString("yyyyMMddHHmmss"));
        //}
        //#endregion


        //public static List<KaiJiang> GetKaiJiang()
        //{
        //    var entitys = WCFClients.ChartClient.QueryAllGameNewWinNumber("JX11X5|CQSSC|SSQ|DLT|FC3D|PL3|CTZQ_T14C|CTZQ_T6BQC|CTZQ_T4CJQ|CTZQ_TR9");
        //    List<KaiJiang> list = new List<KaiJiang>();
        //    foreach (var item in entitys.List)
        //    {
        //        var poolInfo = GetPoolInfo(item.GameCode, item.IssuseNumber);
        //        list.Add(new KaiJiang()
        //        {
        //            result = item.WinNumber,
        //            prizepool = poolInfo != null ? poolInfo.TotalPrizePoolMoney.ToString("###,##0.00") : "",
        //            nums = Getnums(poolInfo),
        //            name = item.GameCode.ToUpper() == "CTZQ" ? item.GameType : item.GameCode,
        //            termNo = item.IssuseNumber,
        //            ver = "1",
        //            grades = Getgrades(poolInfo),
        //            date = item.CreateTime.ToString("yyyy-MM-dd"),
        //            type = GetGameName(item.GameCode, item.GameType),
        //            sale = poolInfo != null ? poolInfo.TotalSellMoney.ToString("###,##0.00") : ""
        //        });

        //    }

        //    list[list.Count - 1].name = "tr9";
        //    list[list.Count - 1].type = "任选9";

        //    return list;
        //}

        //public static PrizelevelInfo GetKaiJingInfo(string version, string type, string term)
        //{
        //    type = type.ToUpper();
        //    //"JX11X5|CQSSC|SSQ|DLT|FC3D|PL3|CTZQ_T14C|CTZQ_T6BQC|CTZQ_T4CJQ|CTZQ_TR9"
        //    var entity = WCFClients.GameIssuseClient.QueryAllGameCurrentIssuse(true);
        //    var entitys = entity as List<LotteryIssuse_QueryInfo>;
        //    var bjdcentity = WCFClients.GameIssuseClient.QueryBJDCCurrentIssuseInfo();//北单
        //    PrizelevelInfo info = new PrizelevelInfo();
        //    info.prizeLevel = new List<Prizelevel>();
        //    var gameCode = type;
        //    if (type.StartsWith("CTZQ"))
        //    {
        //        var strs = type.Split('_');
        //        gameCode = strs[0];
        //        //var gameType = strs[1];
        //        var poolInfo = GetPoolInfo_CTZQ(type.ToUpper(), term);
        //        foreach (var item in poolInfo)
        //        {
        //            info.prizeLevel.Add(new Prizelevel()
        //            {
        //                    betnum = item.BonusCount.ToString(),
        //                    prize = item.BonusMoney.ToString("###,##0.00"),
        //                    name = item.BonusLevelDisplayName
        //            });
        //        }
        //    }
        //    else
        //    {
        //        var poolInfo = GetPoolInfo(type.ToUpper(), term);
        //        if (poolInfo.GradeList != null)
        //        {
        //            foreach (var item in poolInfo.GradeList)
        //            {
        //                info.prizeLevel.Add(new Prizelevel()
        //                {
        //                    betnum = item.BonusCount.ToString(),
        //                    prize = item.BonusMoney.ToString("###,##0.00"),
        //                    name = item.GradeName
        //                });
        //            }
        //        }
        //    }
        //    var model = entitys.Find(a => a.GameCode == gameCode);
        //    info.stopSendPrizeTime = model != null ? model.LocalStopTime.ToString() : DateTime.Now.ToString();
            
        //    return info;
        //}

        ///// <summary>
        ///// 查询开奖历史
        ///// </summary>
        //public static List<KaiJiangHistory> GetHistory(string gameCode, int page)
        //{
        //    gameCode = gameCode.ToUpper();
        //    var startTime = DateTime.Now.AddYears(-1);
        //    var endTime = DateTime.Now;
        //    List<KaiJiangHistory> list = new List<KaiJiangHistory>();
        //    string[] arr = { "T14C", "TR9", "T6BQC", "T4CJQ" };
        //    if (arr.Count(a => a == gameCode) == 1)
        //    {
        //        var numberHistoryList = WCFClients.ChartClient.QueryGameWinNumberByDate(startTime, endTime, string.Format("{0}_{1}", "CTZQ", gameCode), page, MaxIssuseCount("CTZQ"));
        //        foreach (var item in numberHistoryList.List)
        //        {
        //            list.Add(new KaiJiangHistory()
        //            {
        //                result = item.WinNumber,
        //                time = item.CreateTime.ToString("yyyy-MM-dd"),
        //                prizepool = "",
        //                term = item.IssuseNumber,
        //                type = GetGameName(item.GameCode, item.GameType),
        //                sale = ""
        //            });
        //        }
        //    }
        //    else
        //    {
        //        var numberHistoryList = WCFClients.ChartClient.QueryGameWinNumberByDateDesc(startTime, endTime, gameCode, page, MaxIssuseCount("CTZQ"));
        //        foreach (var item in numberHistoryList.List)
        //        {
        //            list.Add(new KaiJiangHistory()
        //            {
        //                result = item.WinNumber,
        //                time = item.CreateTime.ToString("yyyy-MM-dd"),
        //                prizepool = "",
        //                term = item.IssuseNumber,
        //                type = GetGameName(item.GameCode, item.GameType),
        //                sale = ""
        //            });
        //        }
        //    }
        //    return list;
        //}


        ///// <summary>
        ///// 3.4对阵详情：是在传统足球开奖详情页内请求
        ///// </summary>
        //public static List<KaiJiangOpenMatch> GetphoneOpenMatch(string version, string type, string term)
        //{
        //    List<KaiJiangOpenMatch> list = new List<KaiJiangOpenMatch>();
        //    var match = WCFClients.GameClient.QueryCTZQMatchListByIssuseNumber(type, term, "");
        //    if (match == null || match.ListInfo == null)
        //        return list;
        //    int i = 1;
        //    foreach (var item in match.ListInfo)
        //    {
        //        list.Add(new KaiJiangOpenMatch()
        //        {
        //            result = GetResult(item.HomeTeamScore, item.GuestTeamScore),
        //            match_point = -1,
        //            whole_score = item.HomeTeamScore + ":" + item.GuestTeamScore,
        //            match_name = item.MatchName,
        //            away_team = item.GuestTeamName.Replace("\u0026nbsp;", "").Replace("&nbsp;", ""),
        //            match_state = "已完成",
        //            home_team = item.HomeTeamName.Replace("\u0026nbsp;", "").Replace("&nbsp;", ""),
        //            half_score = item.HomeTeamHalfScore + ":" + item.GuestTeamHalfScore,
        //            bout_index = (i++).ToString(),
        //            match_time = ""
        //        });
        //    }
        //    return list;
        //}

        ///// <summary>
        ///// 数字彩详情
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="issuseId"></param>
        ///// <returns></returns>
        //private static Web_SZC_BonusPoolInfo GetPoolInfo(string type, string issuseId)
        //{
        //    BusinessHelper bizHelper = new BusinessHelper();
        //    var poolInfo = bizHelper.GetSZCBonusPool(IssuseFile(type, issuseId));
        //    return poolInfo;
        //}

        ///// <summary>
        ///// 传统足球详情
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="issuseId"></param>
        ///// <returns></returns>
        //private static List<Web_CTZQ_BonusPoolInfo> GetPoolInfo_CTZQ(string type, string issuseId)
        //{
        //    BusinessHelper bizHelper = new BusinessHelper();
        //    var poolInfo = bizHelper.GetCTZQBonusPool(IssuseFile(type, issuseId));
        //    return poolInfo;
        //}

        //private static string Getnums(Web_SZC_BonusPoolInfo model)
        //{
        //    if (model == null || model.GradeList == null)
        //        return "";
        //    string msg = string.Empty;
        //    foreach (var item in model.GradeList)
        //    {
        //        msg += item.BonusCount + ",";
        //    }
        //    return msg.TrimEnd(',');
        //}
        //private static string Getgrades(Web_SZC_BonusPoolInfo model)
        //{
        //    if (model == null || model.GradeList == null)
        //        return "";
        //    string msg = string.Empty;
        //    foreach (var item in model.GradeList)
        //    {
        //        msg += item.BonusMoney.ToString("###,##0.00") + ",";
        //    }
        //    return msg.TrimEnd(',');
        //}

        //private static string GetResult(int homeTeamScore, int guestTeamScore)
        //{
        //    string flag = "[{0},{1},{2}-{3}]";
        //    if (homeTeamScore == guestTeamScore)
        //    {
        //        flag = string.Format(flag, "平", "2", homeTeamScore, guestTeamScore);
        //    }
        //    else if (homeTeamScore > guestTeamScore)
        //    {
        //        flag = string.Format(flag, "胜", "3", homeTeamScore, guestTeamScore);
        //    }
        //    else
        //    {
        //        flag = string.Format(flag, "负", "1", homeTeamScore, guestTeamScore);
        //    }
        //    return flag;
        //}

        ///// <summary>
        ///// 游戏名字
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //private static string GetGameName(string name, string gametype)
        //{
        //    //JX11X5|CQSSC|SSQ|DLT|FC3D|PL3|CTZQ_T14C|CTZQ_T6BQC|CTZQ_T4CJQ
        //    name = name.ToUpper() == "CTZQ" ? gametype : name;
        //    switch (name.ToUpper())
        //    {
        //        case "JX11X5":
        //            return "江西11选5";
        //        case "CQSSC":
        //            return "重庆时时彩";
        //        case "SSQ":
        //            return "双色球";
        //        case "DLT":
        //            return "大乐透";
        //        case "FC3D":
        //            return "福彩3D";
        //        case "PL3":
        //            return "排列3";
        //        case "T14C":
        //            return "胜负彩";
        //        case "T6BQC":
        //            return "半全场";
        //        case "T4CJQ":
        //            return "进球彩";
        //        case "TR9":
        //            return "任选九";
        //        case "CTZQ":
        //            return "传统足球";
        //        default:
        //            return "";
        //    }
        //}


        ////获取对应彩种的最大期数
        //private static int MaxIssuseCount(string gameCode)
        //{
        //    switch (gameCode.ToUpper())
        //    {
        //        case "CQSSC":
        //            return 120;
        //        case "JXSSC":
        //            return 84;
        //        case "JX11X5":
        //            return 84;
        //        case "SD11X5":
        //            return 78;
        //        case "GD11X5":
        //            return 84;
        //        case "FC3D":
        //        case "PL3":
        //        case "PL5":
        //            return 358;
        //        case "SDQYH":
        //            return 40;
        //        case "GDKLSF":
        //            return 84;
        //        case "GXKLSF":
        //            return 50;
        //        case "SSQ":
        //        case "DLT":
        //            return 156;
        //        case "JSKS":
        //            return 82;
        //        case "CTZQ":
        //            return 50;
        //        default:
        //            return 0;
        //    }
        //}
    }
}