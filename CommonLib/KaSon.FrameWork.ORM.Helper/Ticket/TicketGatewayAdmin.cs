using EntityModel;
using EntityModel.Interface;
using EntityModel.Ticket;
using KaSon.FrameWork.Analyzer.AnalyzerFactory;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.JSON;
using KaSon.FrameWork.Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// BJDC
    /// </summary>
    public partial class TicketGatewayAdmin
    {
        private static string _baseDir;
        public void SetMatchConfigBaseDir(string dir)
        {
            _baseDir = dir;
        }
     
        private IList<T> GetOddsList_JingCai<T>(string gameCode, string gameType, string flag)where T : IMatchData
        {
            List<T> result = new List<T>();

            if (ConfigHelper.CrawDataBaseIsMongo)
            {
                switch (gameCode.ToUpper())
                {
                    case "JCZQ":
                        result = MgMatchDataHelper.JCZQ_SP<T>(gameType, null);

                        break;
                    case "JCLQ":
                        result = MgMatchDataHelper.JCLQ_SP<T>(gameType, null);

                        break;
                    default:
                        break;
                }
            }
            else
            {
                var fileName = string.Format(@"{3}/{0}/{1}_SP{2}.json", gameCode, gameType, flag, _baseDir);
                var json = ReadFileString(fileName);
                result = JsonSerializer.DeserializeOldDate<List<T>>(json);
            }




            return result;
        }
        private string ReadFileString(string fileName)
        {
            string strResult = PostManager.Get(fileName, Encoding.UTF8);

            if (!string.IsNullOrEmpty(strResult))
            {
                if (strResult.ToLower().StartsWith("var"))
                {
                    string[] strArray = strResult.Split('=');
                    if (strArray != null && strArray.Length == 2)
                    {
                        if (strArray[1].ToString().Trim().EndsWith(";"))
                        {
                            return strArray[1].ToString().Trim().TrimEnd(';');
                        }
                        return strArray[1].ToString().Trim();
                    }
                }
            }
            return strResult;
        }
        public List<string> RequestTicket_BJDCSingleScheme(GatewayTicketOrder_SingleScheme order, out List<string> realMatchIdArray)
        {
            var selectMatchIdArray = order.SelectMatchId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var allowCodeArray = order.AllowCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            //var codeText = File.ReadAllText(order.AnteCodeFullFileName, Encoding.UTF8);
            var codeText = Encoding.UTF8.GetString(order.FileBuffer);
            var matchIdList = new List<string>();
            var codeList = AnalyzerFactory.CheckSingleSchemeAnteCode(codeText, order.PlayType, order.ContainsMatchId, selectMatchIdArray, allowCodeArray, out matchIdList);
            var totalMoney = codeList.Count * 2M * order.Amount;
            if (totalMoney != order.TotalMoney)
                throw new ArgumentException(string.Format("订单金额不正确，应该为：{0}；实际为：{1}。订单号：{2}", totalMoney, order.TotalMoney, order.OrderId));
            realMatchIdArray = order.ContainsMatchId ? matchIdList : selectMatchIdArray.ToList();

            var manager = new Sports_Manager();
            if (manager.QuerySingleSchemeOrder(order.OrderId) == null)
            {
                manager.AddSingleSchemeOrder(new T_SingleScheme_Order
                {
                    AllowCodes = order.AllowCodes,
                    Amount = order.Amount,
                    FileBuffer = Encoding.UTF8.GetString(order.FileBuffer),
                    //AnteCodeFullFileName = order.AnteCodeFullFileName,
                    ContainsMatchId = order.ContainsMatchId,
                    CreateTime = DateTime.Now,
                    GameCode = order.GameCode,
                    GameType = order.GameType,
                    IssuseNumber = order.IssuseNumber,
                    IsVirtualOrder = order.IsVirtualOrder,
                    OrderId = order.OrderId,
                    PlayType = order.PlayType,
                    SelectMatchId = order.SelectMatchId,
                    TotalMoney = order.TotalMoney,
                });
            }

            return codeList;
        }

    }
}
