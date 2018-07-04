using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.Enum;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.ORM.Helper.WinNumber.Manage;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;
namespace KaSon.FrameWork.ORM.Helper.WinNumber
{
    public class KJGameIssuseBusiness:DBbase
    {
        public void AddKJLocalIssuseList(KJLocalIssuse_AddInfoCollection list, int localAdvanceSeconds)
        {
            using (LottertDataDB)
            {
                var manage = new KJGameIssuseManager();
                foreach (var item in list)
                {
                    var issuse = manage.QueryKJGameIssuse(item.GameCode, item.IssuseNumber);
                    if (issuse == null)
                    {
                        issuse = new KJGameIssuse
                        {
                            CreateTime = DateTime.Now,
                            GameCode = item.GameCode,
                            GameCode_IssuseNumber = string.Format("{0}|{1}", item.GameCode, item.IssuseNumber),
                            IssuseNumber = item.IssuseNumber,
                            StartTime = item.StartTime,
                            WinNumber = string.Empty,
                            Status = (IssuseStatus)IssuseStatus.OnSale,
                            GatewayStopTime = item.BettingStopTime,
                            OfficialStopTime = item.OfficialStopTime,
                            LocalStopTime = item.BettingStopTime.AddSeconds(localAdvanceSeconds),
                        };
                        manage.AddKJGameIssuse(issuse);
                    }
                }
            }
        }

        public KJIssuse_QueryInfo QueryCurrentIssuseInfoWithOffical(string gameCode)
        {
            var entity = new KJGameIssuseManager().QueryCurrentIssuseWithOffical(gameCode);
            if (entity == null) return null;
            var info = new KJIssuse_QueryInfo { Status = IssuseStatus.OnSale };
           ObjectConvert.ConverEntityToInfo<KJGameIssuse, KJIssuse_QueryInfo>(entity, ref info);
            var gameInfo = new KJGameInfo();
            gameInfo.GameCode = entity.GameCode;
            info.Game = gameInfo;
            return info;
        }

        public KJIssuse_QueryInfo QueryIssuseInfo(string gameCode, string issuseNumber)
        {
            var issuse = new KJGameIssuseManager().QueryKJGameIssuse(gameCode, issuseNumber);
            if (issuse == null) return new KJIssuse_QueryInfo { Status = IssuseStatus.OnSale };
            return new KJIssuse_QueryInfo
            {
                CreateTime = issuse.CreateTime,
                GameCode_IssuseNumber = issuse.GameCode_IssuseNumber,
                Game = new KJGameInfo
                {
                    GameCode = issuse.GameCode
                },
                GatewayStopTime = issuse.GatewayStopTime,
                IssuseNumber = issuse.IssuseNumber,
                LocalStopTime = issuse.LocalStopTime,
                OfficialStopTime = issuse.OfficialStopTime,
                StartTime = issuse.StartTime,
                Status = issuse.Status,
                WinNumber = issuse.WinNumber,
            };
        }

        public void IssusePrize(string gameCode, string issuseNumber, string winNumber)
        {
            var manager = new KJGameIssuseManager();
            var issuseEntity = manager.QueryKJGameIssuse(gameCode, issuseNumber);
            if (issuseEntity == null)
                return;

            issuseEntity.WinNumber = winNumber;
            issuseEntity.Status = IssuseStatus.Stopped;
            issuseEntity.AwardTime = DateTime.Now;
            manager.UpdateKJGameIssuse(issuseEntity);
        }

        public void DeleteIssuseData(string gameCode, string[] issuseArray)
        {
            if (issuseArray.Length <= 0) return;
            //开启事务
                LottertDataDB.Begin();
                var manager = new KJGameIssuseManager();
                var count = 1;
                if (issuseArray.Length >= 500)
                {
                    count = issuseArray.Length / 500;
                    if (issuseArray.Length % 500 > 0)
                        count++;
                }
                for (int i = 0; i < count; i++)
                {
                    var array = issuseArray.Skip(500 * i).Take(500).ToArray();
                    manager.DeleteIssuseData(gameCode, array);
                }
                LottertDataDB.Commit();            
        }
    }
}
