﻿using EntityModel.ExceptionExtend;
using EntityModel.Xml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KaSon.FrameWork.Common.Gateway
{
    public class CommandDefinition
    {
        private static readonly Dictionary<string, CommandDefinition> _commandDictionary;

        static CommandDefinition()
        {
            _commandDictionary = new Dictionary<string, CommandDefinition>();

            _commandDictionary.Add("001",
                new CommandDefinition
                {
                    CommandCode = "001",
                    Direction = "request",
                    Description = "期号查询",
                    RequestObjectType = typeof(IssuseQueryRequestInfo)
                });
            _commandDictionary.Add("002",
                new CommandDefinition
                {
                    CommandCode = "002",
                    Direction = "request",
                    Description = "提交投注记录",
                    RequestObjectType = typeof(TicketRequestInfo)
                });
            _commandDictionary.Add("003",
                new CommandDefinition
                {
                    CommandCode = "003",
                    Direction = "request",
                    Description = "查询交易结果",
                    RequestObjectType = typeof(QueryTicketRequestInfo)
                });
            _commandDictionary.Add("004",
                new CommandDefinition
                {
                    CommandCode = "004",
                    Direction = "request",
                    Description = "奖金查询",
                    RequestObjectType = typeof(QueryPrizeRequestInfo)
                });
            _commandDictionary.Add("005",
                new CommandDefinition
                {
                    CommandCode = "005",
                    Direction = "request",
                    Description = "合作商帐户查询",
                    RequestObjectType = typeof(BalanceRequestInfo)
                });
            _commandDictionary.Add("006",
                new CommandDefinition
                {
                    CommandCode = "006",
                    Direction = "request",
                    Description = "北单比赛列表查询",
                    RequestObjectType = typeof(BJDC_GameQueryRequestInfo)
                });
            _commandDictionary.Add("009",
                new CommandDefinition
                {
                    CommandCode = "009",
                    Direction = "request",
                    Description = "获取比赛结果",
                    RequestObjectType = typeof(GameResultQueryRequestInfo)
                });
            _commandDictionary.Add("014",
                new CommandDefinition
                {
                    CommandCode = "014",
                    Direction = "request",
                    Description = "获取销售的竞彩对阵",
                    RequestObjectType = typeof(JC_GameQueryRequestInfo)
                });
            // 自动通知反馈
            _commandDictionary.Add("107",
                new CommandDefinition
                {
                    CommandCode = "107",
                    Direction = "request",
                    Description = "交易自动发送出票结果响应",
                    RequestObjectType = typeof(AutoTicketResultResponseInfo)
                });
            _commandDictionary.Add("010",
                new CommandDefinition
                {
                    CommandCode = "010",
                    Direction = "request",
                    Description = "数字彩开奖号码查询",
                    RequestObjectType = typeof(WinNumberRequestInfo)
                });
        }

        public string CommandCode { get; set; }
        public string Direction { get; set; }
        public string Description { get; set; }
        public Type RequestObjectType { get; set; }

        public static CommandDefinition GetCommandByRequestObjectType(Type type)
        {
            var result = _commandDictionary.Single(item => { return type.Equals(item.Value.RequestObjectType); });
            return result.Value;
        }

        private static CommandDefinition CreateInstance(string cmdCode, string direction, string description)
        {
            return new CommandDefinition
            {
                CommandCode = cmdCode,
                Direction = direction,
                Description = description
            };
        }

        public static string GetCommandCategory(string code)
        {
            switch (code)
            {
                case "002":
                case "102":
                case "003":
                case "103":
                case "007":
                case "107":
                    return "Ticket";
            }
            return code;
        }
    }

    public static class ResultCodeAnalyzer
    {
        static ResultCodeAnalyzer()
        {
            CodeDictionary = new Dictionary<string, string>();
            CodeDictionary.Add("000", "操作成功");

            CodeDictionary.Add("999", "系统错误");
            CodeDictionary.Add("901", "没有此用户");
            CodeDictionary.Add("902", "数据格式错误");
            CodeDictionary.Add("903", "当前没有投注期");
            CodeDictionary.Add("904", "数据校验失败");

            CodeDictionary.Add("905", "数据金额错误");
            CodeDictionary.Add("906", "超过倍数上限");
            CodeDictionary.Add("907", "超过最大金额上限");
            CodeDictionary.Add("908", "账户余额不足");
            CodeDictionary.Add("909", "投注订单ID重复");
            CodeDictionary.Add("910", "下单过期");
            CodeDictionary.Add("911", "发单票数超过上限");
            CodeDictionary.Add("912", "方案编号过长");
            CodeDictionary.Add("913", "下单失败因包含有取消场次");
            CodeDictionary.Add("914", "未开售");
            CodeDictionary.Add("915", "无此种玩法");

            CodeDictionary.Add("920", "没有此种彩种权限");
            CodeDictionary.Add("921", "玩法中包含不支持比赛场次");
            CodeDictionary.Add("922", "玩法不支持单式方案");
            CodeDictionary.Add("923", "含有未开售或者已截止场次");
            CodeDictionary.Add("924", "暂停销售");
        }

        public static Dictionary<string, string> CodeDictionary { get; }

        public static string GetResultDescription(string resultId)
        {
            if (CodeDictionary.ContainsKey(resultId))
                return CodeDictionary[resultId];
            return resultId;
        }

        public static void CheckResultCode(string resultId)
        {
            if (resultId != "000")
                throw new LogicException(GetResultDescription(resultId));
        }
    }
}