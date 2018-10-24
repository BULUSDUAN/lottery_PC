using EntityModel.Enum;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EntityModel
{
    /// <summary>
    /// 用户查询信息
    /// </summary>

    [Serializable]
    public class UserQueryInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// VIP等级
        /// </summary>
        public int VipLevel { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 注册源
        /// </summary>
        public string ComeFrom { get; set; }
        /// <summary>
        /// 注册IP
        /// </summary>
        public string RegisterIp { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegTime { get; set; }
        /// <summary>
        /// 可用状态
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 可用状态
        /// </summary>
        public bool IsAgent { get; set; }
        /// <summary>
        /// 可用状态
        /// </summary>
        public bool IsFillMoney { get; set; }
        /// <summary>
        /// 经销商
        /// </summary>
        public string AgentId { get; set; }
        /// <summary>
        /// 代理总类（分/总）
        /// </summary>
        public OCAgentCategory OCAgentCategory { get; set; }
        /// <summary>
        /// 支付宝
        /// </summary>
        public string ApliyCount { get; set; }
        /// <summary>
        /// QQ号码
        /// </summary>
        public string QQNumber { get; set; }
        /// <summary>
        /// 充值账户余额
        /// </summary>
        public decimal FillMoneyBalance { get; set; }
        /// <summary>
        /// 奖金账户，中奖后返到此账户，可提现
        /// </summary>
        public decimal BonusBalance { get; set; }
        /// <summary>
        /// 佣金账户，为代理商计算佣金时，转到此账户
        /// </summary>
        public decimal CommissionBalance { get; set; }
        /// <summary>
        /// 名家余额
        /// </summary>
        public decimal ExpertsBalance { get; set; }
        /// <summary>
        /// 冻结账户，提现、追号、异常手工冻结
        /// </summary>
        public decimal FreezeBalance { get; set; }
        /// <summary>
        /// 红包余额
        /// </summary>
        public decimal RedBagBalance { get; set; }

        public bool IsSettedMobile { get; set; }
        public string Mobile { get; set; }
        public bool IsSettedRealName { get; set; }
        public string RealName { get; set; }
        public string CardType { get; set; }
        public string IdCardNumber { get; set; }
        public bool IsSettedEmail { get; set; }
        public string Email { get; set; }
        public DateTime AgentAddTime { get; set; }
        public int XjYh { get; set; }
        public int XjDl { get; set; }
        public DateTime HotAddTime { get; set; }
        public CPSMode CPSMode { get; set; }
        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 重复密码
        /// </summary>
        public string RepeatPassword { get; set; }
        /// <summary>
        /// 当前豆豆
        /// </summary>
        public int CurrentDouDou { get; set; }
        /// <summary>
        /// 用户类别:0:网站普通用户；1：内部员工用户；
        /// </summary>
        public int UserType { get; set; }
        /// <summary>
        /// 成长值
        /// </summary>
        public int UserGrowth { get; set; }
        /// <summary>
        /// CPS金额
        /// </summary>
        public decimal CPSBalance { get; set; }
        /// <summary>
        /// 信用类型（0正常，1可疑）
        /// </summary>
        public int UserCreditType { get; set; }
        private T GetDbValue<T>(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return default(T);
            }
            return (T)value;
        }
        public void LoadArray(object[] dataArray)
        {
            if (dataArray.Length == 30)
            {
                UserId = GetDbValue<string>(dataArray[0]);
                DisplayName = GetDbValue<string>(dataArray[1]);
                ComeFrom = GetDbValue<string>(dataArray[2]);
                RegisterIp = GetDbValue<string>(dataArray[3]);
                RegTime = GetDbValue<DateTime>(dataArray[4]);
                IsEnable = GetDbValue<bool>(dataArray[5]);
                IsAgent = GetDbValue<bool>(dataArray[6]);
                IsFillMoney = GetDbValue<bool>(dataArray[7]);
                AgentId = GetDbValue<string>(dataArray[8]);

                FillMoneyBalance = GetDbValue<decimal>(dataArray[9]);
                BonusBalance = GetDbValue<decimal>(dataArray[10]);
                FreezeBalance = GetDbValue<decimal>(dataArray[11]);
                CommissionBalance = GetDbValue<decimal>(dataArray[12]);
                RedBagBalance = GetDbValue<decimal>(dataArray[13]);
                ExpertsBalance = GetDbValue<decimal>(dataArray[14]);

                IsSettedMobile = GetDbValue<bool>(dataArray[15]);
                Mobile = GetDbValue<string>(dataArray[16]);
                IsSettedRealName = GetDbValue<bool>(dataArray[17]);
                RealName = GetDbValue<string>(dataArray[18]);
                CardType = GetDbValue<string>(dataArray[19]);
                IdCardNumber = GetDbValue<string>(dataArray[20]);
                IsSettedEmail = GetDbValue<bool>(dataArray[21]);
                Email = GetDbValue<string>(dataArray[22]);
                VipLevel = GetDbValue<int>(dataArray[23]);
                UserType = GetDbValue<int>(dataArray[24]);
                ApliyCount = GetDbValue<string>(dataArray[25]);
                QQNumber = GetDbValue<string>(dataArray[26]);
                OCAgentCategory = GetDbValue<OCAgentCategory>(dataArray[27]);
                CPSBalance = GetDbValue<decimal>(dataArray[28]);
                CPSMode = GetDbValue<CPSMode>(dataArray[29]);
            }
            else if (dataArray.Length == 31)
            {
                UserId = GetDbValue<string>(dataArray[0]);
                DisplayName = GetDbValue<string>(dataArray[1]);
                ComeFrom = GetDbValue<string>(dataArray[2]);
                RegisterIp = GetDbValue<string>(dataArray[3]);
                RegTime = GetDbValue<DateTime>(dataArray[4]);
                IsEnable = GetDbValue<bool>(dataArray[5]);
                IsAgent = GetDbValue<bool>(dataArray[6]);
                IsFillMoney = GetDbValue<bool>(dataArray[7]);
                AgentId = GetDbValue<string>(dataArray[8]);

                FillMoneyBalance = GetDbValue<decimal>(dataArray[9]);
                BonusBalance = GetDbValue<decimal>(dataArray[10]);
                FreezeBalance = GetDbValue<decimal>(dataArray[11]);
                CommissionBalance = GetDbValue<decimal>(dataArray[12]);
                RedBagBalance = GetDbValue<decimal>(dataArray[13]);
                ExpertsBalance = GetDbValue<decimal>(dataArray[14]);

                IsSettedMobile = GetDbValue<bool>(dataArray[15]);
                Mobile = GetDbValue<string>(dataArray[16]);
                IsSettedRealName = GetDbValue<bool>(dataArray[17]);
                RealName = GetDbValue<string>(dataArray[18]);
                CardType = GetDbValue<string>(dataArray[19]);
                IdCardNumber = GetDbValue<string>(dataArray[20]);
                IsSettedEmail = GetDbValue<bool>(dataArray[21]);
                Email = GetDbValue<string>(dataArray[22]);
                VipLevel = GetDbValue<int>(dataArray[23]);
                UserType = GetDbValue<int>(dataArray[24]);
                ApliyCount = GetDbValue<string>(dataArray[25]);
                QQNumber = GetDbValue<string>(dataArray[26]);
                OCAgentCategory = GetDbValue<OCAgentCategory>(dataArray[27]);
                CPSBalance = GetDbValue<decimal>(dataArray[28]);
                CPSMode = GetDbValue<CPSMode>(dataArray[29]);
                ChannelName = GetDbValue<string>(dataArray[30]);
            }
            else if (dataArray.Length == 24)
            {
                UserId = GetDbValue<string>(dataArray[0]);
                DisplayName = GetDbValue<string>(dataArray[1]);
                ComeFrom = GetDbValue<string>(dataArray[2]);
                RegisterIp = GetDbValue<string>(dataArray[3]);
                RegTime = GetDbValue<DateTime>(dataArray[4]);
                IsEnable = GetDbValue<bool>(dataArray[5]);
                IsAgent = GetDbValue<bool>(dataArray[6]);
                IsFillMoney = GetDbValue<bool>(dataArray[7]);
                AgentId = GetDbValue<string>(dataArray[8]);

                FillMoneyBalance = GetDbValue<decimal>(dataArray[9]);
                BonusBalance = GetDbValue<decimal>(dataArray[10]);
                FreezeBalance = GetDbValue<decimal>(dataArray[11]);
                CommissionBalance = GetDbValue<decimal>(dataArray[12]);
                RedBagBalance = GetDbValue<decimal>(dataArray[13]);
                ExpertsBalance = GetDbValue<decimal>(dataArray[14]);

                IsSettedMobile = GetDbValue<bool>(dataArray[15]);
                Mobile = GetDbValue<string>(dataArray[16]);
                IsSettedRealName = GetDbValue<bool>(dataArray[17]);
                RealName = GetDbValue<string>(dataArray[18]);
                CardType = GetDbValue<string>(dataArray[19]);
                IdCardNumber = GetDbValue<string>(dataArray[20]);
                IsSettedEmail = GetDbValue<bool>(dataArray[21]);
                Email = GetDbValue<string>(dataArray[22]);
                VipLevel = GetDbValue<int>(dataArray[23]);
            }
            else if (dataArray.Length == 18)
            {
                //和数据库顺序一致
                UserId = GetDbValue<string>(dataArray[0]);
                DisplayName = GetDbValue<string>(dataArray[1]);
                ComeFrom = GetDbValue<string>(dataArray[2]);
                RegisterIp = GetDbValue<string>(dataArray[3]);
                RegTime = GetDbValue<DateTime>(dataArray[4]);
                VipLevel = GetDbValue<int>(dataArray[5]);
                IsSettedEmail = GetDbValue<bool>(dataArray[6]);
                Mobile = GetDbValue<string>(dataArray[7]);
                IsSettedRealName = GetDbValue<bool>(dataArray[8]);

                RealName = GetDbValue<string>(dataArray[9]);
                CardType = GetDbValue<string>(dataArray[10]);
                IdCardNumber = GetDbValue<string>(dataArray[11]);
                AgentId = GetDbValue<string>(dataArray[12]);
                AgentAddTime = GetDbValue<DateTime>(dataArray[13]);
                XjYh = GetDbValue<int>(dataArray[14]);
                XjDl = GetDbValue<int>(dataArray[15]);
                IsAgent = GetDbValue<bool>(dataArray[16]);
                IsEnable = GetDbValue<bool>(dataArray[17]);
            }
            else if (dataArray.Length == 16)
            {
                //和数据库顺序一致
                UserId = GetDbValue<string>(dataArray[0]);
                DisplayName = GetDbValue<string>(dataArray[1]);
                ComeFrom = GetDbValue<string>(dataArray[2]);
                RegisterIp = GetDbValue<string>(dataArray[3]);
                RegTime = GetDbValue<DateTime>(dataArray[4]);
                VipLevel = GetDbValue<int>(dataArray[5]);
                IsSettedEmail = GetDbValue<bool>(dataArray[6]);
                Mobile = GetDbValue<string>(dataArray[7]);
                IsSettedRealName = GetDbValue<bool>(dataArray[8]);

                RealName = GetDbValue<string>(dataArray[9]);
                CardType = GetDbValue<string>(dataArray[10]);
                IdCardNumber = GetDbValue<string>(dataArray[11]);
                AgentId = GetDbValue<string>(dataArray[12]);
                HotAddTime = GetDbValue<DateTime>(dataArray[13]);
                IsAgent = GetDbValue<bool>(dataArray[14]);
                IsEnable = GetDbValue<bool>(dataArray[15]);
            }
            else if (dataArray.Length == 32)
            {
                UserId = GetDbValue<string>(dataArray[0]);
                DisplayName = GetDbValue<string>(dataArray[1]);
                ComeFrom = GetDbValue<string>(dataArray[2]);
                RegisterIp = GetDbValue<string>(dataArray[3]);
                RegTime = GetDbValue<DateTime>(dataArray[4]);
                IsEnable = GetDbValue<bool>(dataArray[5]);
                IsAgent = GetDbValue<bool>(dataArray[6]);
                IsFillMoney = GetDbValue<bool>(dataArray[7]);
                AgentId = GetDbValue<string>(dataArray[8]);

                FillMoneyBalance = GetDbValue<decimal>(dataArray[9]);
                BonusBalance = GetDbValue<decimal>(dataArray[10]);
                FreezeBalance = GetDbValue<decimal>(dataArray[11]);
                CommissionBalance = GetDbValue<decimal>(dataArray[12]);
                RedBagBalance = GetDbValue<decimal>(dataArray[13]);
                ExpertsBalance = GetDbValue<decimal>(dataArray[14]);

                IsSettedMobile = GetDbValue<bool>(dataArray[15]);
                Mobile = GetDbValue<string>(dataArray[16]);
                IsSettedRealName = GetDbValue<bool>(dataArray[17]);
                RealName = GetDbValue<string>(dataArray[18]);
                CardType = GetDbValue<string>(dataArray[19]);
                IdCardNumber = GetDbValue<string>(dataArray[20]);
                IsSettedEmail = GetDbValue<bool>(dataArray[21]);
                Email = GetDbValue<string>(dataArray[22]);
                VipLevel = GetDbValue<int>(dataArray[23]);
                UserType = GetDbValue<int>(dataArray[24]);
                ApliyCount = GetDbValue<string>(dataArray[25]);
                QQNumber = GetDbValue<string>(dataArray[26]);
                OCAgentCategory = GetDbValue<OCAgentCategory>(dataArray[27]);
                CPSBalance = GetDbValue<decimal>(dataArray[28]);
                CPSMode = GetDbValue<CPSMode>(dataArray[29]);
                UserCreditType = GetDbValue<int>(dataArray[30]);
            }
            else
            {
                throw new ArgumentException("数据数组长度不满足要求，不能转换成此UserQueryInfo对象，传入数组为" + dataArray.Length + "。" + string.Join(", ", dataArray), "dataArray");
            }
        }
    }
    /// <summary>
    /// 用户查询列表
    /// </summary>
    public class UserQueryInfoCollection
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public UserQueryInfoCollection()
        {
            UserList = new List<UserQueryInfo>();
        }
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 总总值账户金额
        /// </summary>
        public decimal TotalFillMoneyBalance { get; set; }
        /// <summary>
        /// 总奖金账户金额
        /// </summary>
        public decimal TotalBonusBalance { get; set; }
        /// <summary>
        /// 总佣金账户
        /// </summary>
        public decimal TotalCommissionBalance { get; set; }
        /// <summary>
        /// 总名家金额
        /// </summary>
        public decimal TotalExpertsBalance { get; set; }
        /// <summary>
        /// 总冻结金额
        /// </summary>
        public decimal TotalFreezeBalance { get; set; }
        /// <summary>
        /// 总红包金额
        /// </summary>
        public decimal TotalRedBagBalance { get; set; }
        /// <summary>
        /// 总的豆豆
        /// </summary>
        public int TotalDouDou { get; set; }
        /// <summary>
        /// 总CPS金额
        /// </summary>
        public decimal TotalCPSBalance { get; set; }
        /// <summary>
        /// 用户列表
        /// </summary>
        public IList<UserQueryInfo> UserList { get; set; }
        /// <summary>
        /// 加载存储过程返回数组列表
        /// </summary>
        /// <param name="list">数组列表</param>
        public void LoadList(IList list)
        {
            foreach (object[] item in list)
            {
                var info = new UserQueryInfo();
                info.LoadArray(item);
                UserList.Add(info);
            }
        }
    }
}
