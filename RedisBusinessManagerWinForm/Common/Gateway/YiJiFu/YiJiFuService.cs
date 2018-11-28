using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Common.Net;

namespace Common.Gateway.YiJiFu
{
    /// <summary>
    /// 易极付接口服务
    /// </summary>
    public class YiJiFuService
    {
        /// <summary>
        /// 接口服务构造函数
        /// </summary>
        /// <param name="partnerid">商户ID，合作身份者ID，合作伙伴ID</param>
        /// <param name="key">商户密钥</param>
        /// <param name="istest">是否为测试状态，系统自动根据是否测试状态来选择网关地址</param>
        /// <remarks>进行全局变量初始化，在网站运行时初始化该服务接口</remarks>
        public YiJiFuService(string partnerid, string key, bool istest)
        {
            Config.PatnerId = partnerid;
            Config.Key = key;
            Config.IsTest = istest;
        }

        #region 手机绑定

        /// <summary>
        /// 接口函数 - 申请用户手机绑定
        /// </summary>
        /// <param name="userid">关联用户ID</param>
        /// <param name="mobile">手机号码</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <returns>用户手机绑定url地址</returns>
        public string MobileBinding(string userid, string mobile, string returnUrl, string notifyUrl, string errorNotifyUrl)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "applyMobileBindingCustomer");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userid", userid);
            dic.Add("mobile", mobile);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 更新用户手机绑定
        /// </summary>
        /// <param name="userid">关联用户ID</param>
        /// <param name="mobile">更新用户手机号码</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <returns>更新用户手机绑定url地址</returns>
        public string UpdateMobileBinding(string userid, string mobile, string returnUrl, string notifyUrl, string errorNotifyUrl)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "updateMobileBindingCustomer");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userid", userid);
            dic.Add("mobile", mobile);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        #endregion

        #region 用户注册激活

        /// <summary>
        /// 接口函数 - 注册服务
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">用户密码</param>
        /// <param name="realName">用户真实姓名</param>
        /// <param name="email">用户邮箱</param>
        /// <param name="registerFrom">注册来源E_TURNOVER（易极付)</param>
        /// <param name="originRegisterFrom">注册来源EXT_IMPORT(外部引入)</param>
        /// <param name="returnUrl">易极付处理完请求后，当前页面自动跳转到合作伙伴网站里指定页面的http路径</param>
        /// <param name="notifyUrl">易极付服务器主动通知合作伙伴网站里指定的页面http路径</param>
        /// <param name="errorNotifyUrl">当合作伙伴通过该接口发起请求时，如果出现提示报错，易极付会根据 “6.1 请求出错时的通知错误码”通过异步的方式发送通知给合作伙伴。该功能需要联系技术支持开通。</param>
        /// <param name="userType">用户类型，可选值：P(个人用户)B(企业用户)默认为个人用户</param>
        /// <param name="userStatus">用户状态，可选值:T（正常用户），默认为T。</param>
        /// <returns>用户注册url地址</returns>
        public string RegistgerUrl(string userName, string password, string realName, string email, string registerFrom, string originRegisterFrom, string returnUrl, string notifyUrl, string errorNotifyUrl, string userType = "P", string userStatus = "T")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "userRegister");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userName", userName);
            dic.Add("password", password);
            dic.Add("userType", userType);
            dic.Add("realName", realName);
            dic.Add("email", email);
            dic.Add("registerFrom", registerFrom);
            dic.Add("originRegisterFrom", originRegisterFrom);
            dic.Add("userStatus", userStatus);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 跳转激活
        /// 此功能在易极付官网进行激活，设置登陆密码和支付密码，没有同步通知，有异步通知
        /// </summary>
        /// <param name="userid">关联用户ID</param>
        /// <param name="returnUrl">同步处理地址 [可为空]</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <returns>跳转激活url地址</returns>
        public string RedirectActive(string userid, string returnUrl, string notifyUrl, string errorNotifyUrl)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "yzzActivate");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userid", userid);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 用户信息查询
        /// </summary>
        /// <param name="userid">关联用户ID</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <returns>用户信息查询url地址</returns>
        public string AccountQuery(string userid, string returnUrl, string notifyUrl, string errorNotifyUrl)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "userAccountQuery");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userid", userid);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        #endregion

        #region 银行卡绑定、查询、验证

        /// <summary>
        /// 接口函数 - 设置银行卡默认绑定服务
        /// </summary>
        /// <param name="userid">关联用户ID</param>
        /// <param name="bankCardNo">银行卡号</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <returns>设置银行卡默认绑定服务url地址</returns>
        public string BindBank_SetDefault(string userid, string bankCardNo, string returnUrl, string notifyUrl, string errorNotifyUrl)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "bankCodeBinding.setDefault");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userid", userid);
            dic.Add("bankCardNo", bankCardNo);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 添加银行卡绑定信息服务
        /// </summary>
        /// <param name="userid">关联用户ID</param>
        /// <param name="bankCardNo">银行卡号</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <param name="bankKey">联行号 [可选值]</param>
        /// <param name="bankType">银行简称 [可选值]</param>
        /// <param name="bankCardType">银行卡类型 [可选值]</param>
        /// <param name="bankAccountType">银行账户类型 [可选值]</param>
        /// <param name="name">开户人姓名 [可选值]</param>
        /// <param name="certType">开户人证件类型 [可选值]</param>
        /// <param name="certNo">证件号 [可选值]</param>
        /// <param name="gender">性别 [可选值]</param>
        /// <param name="province">省份 [可选值]</param>
        /// <param name="city">城市 [可选值]</param>
        /// <param name="address">详细地址 [可选值]</param>
        /// <returns>添加银行卡绑定信息服务url地址</returns>
        public string BindBank_AddInfo(string userid, string bankCardNo, string returnUrl, string notifyUrl, string errorNotifyUrl,
            string bankKey = "", string bankType = "", string bankCardType = "", string bankAccountType = "", string name = "", string certType = "", string certNo = "", string gender = "", string province = "", string city = "", string address = "")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "bankCodeBinding.addInfo");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userid", userid);
            dic.Add("bankCardNo", bankCardNo);
            dic.Add("bankKey", bankKey);
            dic.Add("bankType", bankType);
            dic.Add("bankCardType", bankCardType);
            dic.Add("bankAccountType", bankAccountType);
            dic.Add("name", name);
            dic.Add("certNo", certNo);
            dic.Add("gender", gender);
            dic.Add("province", province);
            dic.Add("city", city);
            dic.Add("address", address);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 解除银行卡的绑定信息
        /// </summary>
        /// <param name="userid">关联用户ID</param>
        /// <param name="bankCardNo">银行卡号</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <returns>解除银行卡的绑定信息url地址</returns>
        public string BindBank_Remove(string userid, string bankCardNo, string returnUrl, string notifyUrl, string errorNotifyUrl)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "bankCodeBindingRemove");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userid", userid);
            dic.Add("bankCardNo", bankCardNo);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 验证银行卡信息
        /// </summary>
        /// <param name="bankCode">银行简称</param>
        /// <param name="accountName">银行卡户名</param>
        /// <param name="accountNo">银行卡卡号</param>
        /// <param name="cardType">银行卡类型  "C"："信用卡类型"   "D"："借记卡类型" </param>
        /// <param name="certNo">证件号码</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <param name="extendId">流水号（唯一） [可选值]</param>
        /// <param name="channelApi">渠道号 [可选值]</param>
        /// <param name="certType">证件类型 
        /// "ID", "身份证"
        /// "ARMY_ID", "军官证"
        /// "PASSPORT", "护照"
        /// "HOME_RETURN", "回乡证"
        /// "TAIWAN", "台胞证"
        /// "OFFICERS_CARD", "警官证"
        /// "SOLDIER_CARD", "士兵证"
        /// "Other", "其它证件"
        ///  [可选值]
        /// </param>
        /// <param name="phoneNo">手机号码 [可选值]</param>
        /// <param name="validDate">有效期 贷记卡必输入，借记卡不需输入 4位长度 [可选值]</param>
        /// <param name="cvv2">cvv2 贷记卡必输入，借记卡不需输入 3位长度 [可选值]</param>
        /// <param name="rem1">备用字段 [可选值]</param>
        /// <returns>验证银行卡信息url地址</returns>
        public string FacadeVerify(string bankCode, string accountName, string accountNo, string cardType, string certNo, string returnUrl, string notifyUrl, string errorNotifyUrl,
            string extendId = "", string channelApi = "", string certType = "", string phoneNo = "", string validDate = "", string cvv2 = "", string rem1 = "")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "verifyFacade");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("bankCode", bankCode);
            dic.Add("accountName", accountName);
            dic.Add("accountNo", accountNo);
            dic.Add("cardType", cardType);
            dic.Add("certNo", certNo);
            dic.Add("extendId", extendId);
            dic.Add("channelApi", channelApi);
            dic.Add("certType", certType);
            dic.Add("phoneNo", phoneNo);
            dic.Add("validDate", validDate);
            dic.Add("cvv2", cvv2);
            dic.Add("rem1", rem1);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 根据userId查询已签约银行
        /// </summary>
        /// <param name="userid">关联用户ID</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <returns>根据userId查询已签约银行url地址</returns>
        public string QueryQuickBank(string userid, string returnUrl, string notifyUrl, string errorNotifyUrl)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "findQuickBank");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userid", userid);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        #endregion

        #region 短信验证

        /// <summary>
        /// 接口函数 - 短信发动验证服务
        /// </summary>
        /// <param name="userid">关联用户ID</param>
        /// <param name="smsContext">短信内容 注意，短信内容由你们传入，格式如下。“你好，此次验证码为C，如不是本人操作请勿回复”里面的C必填，是我们这边生成的验证码进行对C的替换</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <returns>短信发动验证服务url地址</returns>
        public string SMSCaptcha(string userid, string smsContext, string returnUrl, string notifyUrl, string errorNotifyUrl)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "smsCaptcha");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userid", userid);
            dic.Add("smsContext", smsContext);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 验证短信验证码服务
        /// </summary>
        /// <param name="checkCodeString">用户手机验证码</param>
        /// <param name="checkCodeUniqueId">验证码标识 由短信发送验证码服务返回给的同步参数</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <returns>验证短信验证码服务url地址</returns>
        public string SMSCaptchaVerify(string checkCodeString, string checkCodeUniqueId, string returnUrl, string notifyUrl, string errorNotifyUrl)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", " smsConfirmVerifyCode");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("checkCodeString", checkCodeString);
            dic.Add("checkCodeUniqueId", checkCodeUniqueId);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        #endregion

        #region 实名认证
        /// <summary>
        /// 接口函数 - 个人实名认证服务
        /// </summary>
        /// <param name="realName">真实姓名</param>
        /// <param name="cardid">身份证号</param>
        /// <param name="cardpic">身份证正面</param>
        /// <param name="cardpic1">身份证背面</param>
        /// <param name="cardoff">身份证到期时间 正常：20120911</param>
        /// <param name="userId">用户id</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <param name="userName">核心用户名 [可选值]</param>
        /// <param name="mobile">手机 [可选值]</param>
        /// <returns>个人实名认证服务url地址</returns>
        public string RealNameCert(string realName, string cardid, string cardpic, string cardpic1, string cardoff, string userId, string returnUrl, string notifyUrl, string errorNotifyUrl,
           string userName = "", string mobile = "")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", " realNameCert.save");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("realName", realName);
            dic.Add("cardid", cardid);
            dic.Add("cardpic", cardpic);
            dic.Add("cardpic1", cardpic1);
            dic.Add("cardoff", cardoff);
            dic.Add("userId", userId);
            dic.Add("userName", userName);
            dic.Add("mobile", mobile);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 企业实名认证服务
        /// </summary>
        /// <param name="comCycle">营业期限</param>
        /// <param name="comName">企业名称</param>
        /// <param name="conCardid">联系人身份证号</param>
        /// <param name="conMobile">联系人手机</param>
        /// <param name="conName">联系人姓名</param>
        /// <param name="conPhone">公司联系电话</param>
        /// <param name="licence">营业执照副本</param>
        /// <param name="licencenum">营业执照注册号</param>
        /// <param name="taxAuthority">税务登记号</param>
        /// <param name="nickname">真实姓名</param>
        /// <param name="organizationcode">组织机构代码</param>
        /// <param name="legalPersonCardPic">法人身份证正面</param>
        /// <param name="legalPersonCardPic1">法人身份证背面</param>
        /// <param name="legalPersonCardType">法人身份类型</param>
        /// <param name="legalPersonCardOff">法人身份到期时间</param>
        /// <param name="legalPersonCardid">法人身份证号</param>
        /// <param name="source">数据来源 1:易极付,2:猪八戒</param>
        /// <param name="coreCustomerUserId">核心用户id</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <param name="licencecopy">营业执照副本加盖公章 [可选值]</param>
        /// <param name="agentPersonName">经办人姓名 [可选值]</param>
        /// <param name="agentPersonCardid">经办人身份证号 [可选值]</param>
        /// <param name="backLetterPic">担保函 [可选值]</param>
        /// <param name="isLegalPerAudit">是否以经办人作为实名认证 [可选值]</param>
        /// <param name="agentPersonCardType">经办人身份证类型 [可选值]</param>
        /// <param name="agentPersonCardOff">经办人身份证到期时间 [可选值]</param>
        /// <param name="agentPersonCardPic">经办人身份正面图片 [可选值]</param>
        /// <param name="agentPersonCardPic1">经办人身份证背面图片 [可选值]</param>
        /// <param name="contextName">联系人姓名 [可选值]</param>
        /// <param name="contextPhone">联系人电话 [可选值]</param>
        /// <param name="coreCustomerUserName">核心用户名 [可选值]</param>
        /// <returns>企业实名认证服务url地址</returns>
        public string RealNameCert(string comCycle, string comName, string conCardid, string conMobile, string conName, string conPhone, string licence, string licencenum,
            string taxAuthority, string nickname, string organizationcode, string legalPersonCardPic, string legalPersonCardPic1, string legalPersonCardType, string legalPersonCardOff,
            string legalPersonCardid, string source, string coreCustomerUserId, string returnUrl, string notifyUrl, string errorNotifyUrl,
            string licencecopy = "", string agentPersonName = "", string agentPersonCardid = "", string backLetterPic = "", string isLegalPerAudit = "", string agentPersonCardType = "",
            string agentPersonCardOff = "", string agentPersonCardPic = "", string agentPersonCardPic1 = "", string contextName = "", string contextPhone = "", string coreCustomerUserName = "")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "realNameCert.save");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("comCycle", comCycle);
            dic.Add("comName", comName);
            dic.Add("conCardid", conCardid);
            dic.Add("conMobile", conMobile);
            dic.Add("conName", conName);
            dic.Add("conPhone", conPhone);
            dic.Add("licence", licence);
            dic.Add("licencenum", licencenum);
            dic.Add("taxAuthority", taxAuthority);
            dic.Add("nickname", nickname);
            dic.Add("organizationcode", organizationcode);
            dic.Add("legalPersonCardPic", legalPersonCardPic);
            dic.Add("legalPersonCardPic1", legalPersonCardPic1);
            dic.Add("legalPersonCardType", legalPersonCardType);
            dic.Add("legalPersonCardOff", legalPersonCardOff);
            dic.Add("legalPersonCardid", legalPersonCardid);
            dic.Add("source", source);
            dic.Add("coreCustomerUserId", coreCustomerUserId);
            dic.Add("licencecopy", licencecopy);
            dic.Add("agentPersonName", agentPersonName);
            dic.Add("agentPersonCardid", agentPersonCardid);
            dic.Add("backLetterPic", backLetterPic);
            dic.Add("isLegalPerAudit", isLegalPerAudit);
            dic.Add("agentPersonCardType", agentPersonCardType);
            dic.Add("agentPersonCardOff", agentPersonCardOff);
            dic.Add("agentPersonCardPic", agentPersonCardPic);
            dic.Add("agentPersonCardPic1", agentPersonCardPic1);
            dic.Add("contextName", contextName);
            dic.Add("contextPhone", contextPhone);
            dic.Add("coreCustomerUserName", coreCustomerUserName);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 实名状态查询服务
        /// </summary>
        /// <param name="userid">关联用户ID</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <returns>实名状态查询服务url地址</returns>
        public string RealNameCert_Query(string userid, string returnUrl, string notifyUrl, string errorNotifyUrl)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "realNameCert.query");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userid", userid);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        #endregion

        #region 充值代扣
        /// <summary>
        /// 接口函数 - 跳转充值页面
        /// </summary>
        /// <param name="userid">关联用户ID 易极付平台生成的用户ID，如：20121018888000015344</param>
        /// <param name="tradeBizProductCode">业务产品编号 由易极付提供</param>
        /// <param name="tradeMerchantId">平台商户：平台商，在带个交易之中提供平台服务的商家 [可为空]</param>
        /// <param name="tradePartnerId">参与者：一般指进行充值或者提现，帐内转账等单纯业务时的个人用户或商户 [可为空]</param>
        /// <param name="depositAmount">充值金额 [可为空]</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <returns>跳转充值页面url地址</returns>
        public string Deposit(string userid, string tradeBizProductCode, string tradeMerchantId, string tradePartnerId, string depositAmount, string returnUrl, string notifyUrl, string errorNotifyUrl)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "deposit");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userid", userid);
            dic.Add("tradeBizProductCode", tradeBizProductCode);
            dic.Add("tradeMerchantId", tradeMerchantId);
            dic.Add("tradePartnerId", tradePartnerId);
            dic.Add("depositAmount", depositAmount);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 充值记录查询服务
        /// </summary>
        /// <param name="userid">关联用户ID</param>
        /// <param name="currPage">当前页 [可选值]</param>
        /// <param name="pageSize">每页数量 [可选值]</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <returns>充值记录查询服务url地址</returns>
        public string QueryDeposit(string userid, string returnUrl, string notifyUrl, string errorNotifyUrl, string currPage = "", string pageSize = "")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "deposit.query");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userid", userid);
            dic.Add("currPage", currPage);
            dic.Add("pageSize", pageSize);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 跳转代扣签约
        /// </summary>
        /// <param name="userid">关联用户ID</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <returns>跳转代扣签约url地址</returns>
        public string DeductSign(string userid, string returnUrl, string notifyUrl, string errorNotifyUrl)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "deductSign");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userid", userid);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 同步/异步代扣充值服务
        /// </summary>
        /// <param name="deductType">代扣类型 sync：申请同步代扣 async：申请异步代扣</param>
        /// <param name="certType">证件类型  ID:身份证,ARMY_ID:军官证,PASSPORT:护照,HOME_RETURN:回乡证,TAIWAN:台胞证,OFFICERS_CARD:警官证,SOLDIER_CARD:士兵证,Other:其它证件;</param>
        /// <param name="userId">充值账户</param>
        /// <param name="amount">充值金额</param>
        /// <param name="certNo">证件号</param>
        /// <param name="bankCode">银行简写</param>
        /// <param name="bankAccountNo">银行开户账户号</param>
        /// <param name="bankAccountName">银行账户开户名</param>
        /// <param name="publicTag">对公对私 对公对私，可选值：Y(是) N(否) NY(特殊) 对公-Y，对私-N 银行卡开户性质，对公只单位用户，对私只个人用户</param>
        /// <param name="outBizNo">外部订单号 不传则默认以orderNo作为参数设置</param>
        /// <param name="tradeBizProductCode">业务产品编号</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <param name="mobileNo">手机号 [可选值]</param>
        /// <param name="provName">银行省名 [可选值]</param>
        /// <param name="cityName">银行市名 [可选值]</param>
        /// <param name="tradeMerchantId">平台商户 平台商，在带个交易之中提供平台服务的商家 [可选值]</param>
        /// <param name="tradePartnerId">参与者 一般指进行充值或者提现，帐内转账等单纯业务时的个人用户或商户 [可选值]</param>
        /// <returns>同步/异步代扣充值服务url地址</returns>
        public string DeductDeposit(string deductType, string certType, string userId, string amount, string certNo, string bankCode, string bankAccountNo, string bankAccountName, string publicTag, string outBizNo, string tradeBizProductCode, string returnUrl, string notifyUrl, string errorNotifyUrl,
            string mobileNo = "", string cityName = "", string tradeMerchantId = "", string tradePartnerId = "")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "deductDeposit.apply");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("deductType", deductType);
            dic.Add("certType", certType);
            dic.Add("userId", userId);
            dic.Add("amount", amount);
            dic.Add("certNo", certNo);
            dic.Add("bankCode", bankCode);
            dic.Add("bankAccountNo", bankAccountNo);
            dic.Add("bankAccountName", bankAccountName);
            dic.Add("publicTag", publicTag);
            dic.Add("outBizNo", outBizNo);
            dic.Add("tradeBizProductCode", tradeBizProductCode);
            dic.Add("mobileNo", mobileNo);
            dic.Add("cityName", cityName);
            dic.Add("tradeMerchantId", tradeMerchantId);
            dic.Add("tradePartnerId", tradePartnerId);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 网银充值
        /// </summary>
        /// <param name="type">类型 可选值：B2B，B2C（必须大写）</param>
        /// <param name="payChannelApi">充值支付渠道API 调用支付引擎api查询接口获取</param>
        /// <param name="userId">充值账户</param>
        /// <param name="amount">充值金额 >0</param>
        /// <param name="charge">手续费 填chargeRule时，不填charge</param>
        /// <param name="chargeRule">手续费规则 手续费规则 [default 不收费] 填chargeRule时，不填charge</param>
        /// <param name="bizIdentity">业务请求者身份标识码 业务请求者身份标识码，详细见快捷充值</param>
        /// <param name="bizNo">业务号</param>
        /// <param name="outBizNo">外部订单号</param>
        /// <param name="tradeBizProductCode">业务产品编号</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <param name="currency">币种 不填默认为CNY，可选值：CNY(人民币) USD(美元) JPY(日元) [可选值]</param>
        /// <param name="freezeType">冻结码 冻结码，见快捷充值freezeType [可选值]</param>
        /// <param name="depositType">充值类型 默认为DEPOSIT_ONLY(纯充值)，可选值：DEPOSIT_ONLY(纯充值) DEPOSIT_BATCH(批量纯充值) DEPOSIT_PAYMENT(转支付) DEPOSIT_PAYEMENT_BATCH(转支付批量) [可选值]</param>
        /// <param name="memo">备注 [可选值]</param>
        /// <param name="bankAccountNo">银行开户账户号 [可选值]</param>
        /// <param name="bankAccountName">银行账户开户名 [可选值]</param>
        /// <param name="bankAddress">开户行地址 [可选值]</param>
        /// <param name="cardType">卡类型 [可选值]</param>
        /// <param name="apiExtBizNo">支付渠道API扩展服务单据号 支付渠道API扩展服务单据号 [可选值]</param>
        /// <param name="extUrl">扩展跳转地址 [可选值]</param>
        /// <param name="tradeMerchantId">平台商户 平台商，在带个交易之中提供平台服务的商家 [可选值]</param>
        /// <param name="tradePartnerId">参与者 一般指进行充值或者提现，帐内转账等单纯业务时的个人用户或商户 [可选值]</param>
        /// <param name="reserve1">保留字段1 [可选值]</param>
        /// <param name="reserve2">保留字段2 [可选值]</param>
        /// <returns>网银充值url地址</returns>
        public string Payment(string type, string payChannelApi, string userId, string amount, string charge, string chargeRule, string bizIdentity, string bizNo, string outBizNo, string tradeBizProductCode, string returnUrl, string notifyUrl, string errorNotifyUrl,
            string currency = "", string freezeType = "", string depositType = "", string memo = "", string bankAccountNo = "", string bankAccountName = "", string bankAddress = "", string cardType = "",
            string apiExtBizNo = "", string extUrl = "", string tradeMerchantId = "", string tradePartnerId = "", string reserve1 = "", string reserve2 = "")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "applyDeposit");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("type", type);
            dic.Add("payChannelApi", payChannelApi);
            dic.Add("userId", userId);
            dic.Add("amount", amount);
            dic.Add("charge", charge);
            dic.Add("chargeRule", chargeRule);
            dic.Add("bizIdentity", bizIdentity);
            dic.Add("bizNo", bizNo);
            dic.Add("outBizNo", outBizNo);
            dic.Add("tradeBizProductCode", tradeBizProductCode);
            dic.Add("currency", currency);
            dic.Add("freezeType", freezeType);
            dic.Add("depositType", depositType);
            dic.Add("memo", memo);
            dic.Add("bankAccountNo", bankAccountNo);
            dic.Add("bankAccountName", bankAccountName);
            dic.Add("bankAddress", bankAddress);
            dic.Add("cardType", cardType);
            dic.Add("apiExtBizNo", apiExtBizNo);
            dic.Add("extUrl", extUrl);
            dic.Add("tradeMerchantId", tradeMerchantId);
            dic.Add("tradePartnerId", tradePartnerId);
            dic.Add("reserve1", reserve1);
            dic.Add("reserve2", reserve2);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 即时到账接口
        /// </summary>
        /// <param name="orderId">商户订单编号</param>
        /// <param name="userName">用户名称</param>
        /// <param name="realName">用户真实姓名</param>
        /// <param name="certType">用户身份类型 传”ID”</param>
        /// <param name="certNo">证件号</param>
        /// <param name="gender">性别</param>
        /// <param name="mobile">手机号码</param>
        /// <param name="payChannelApi">支付渠道</param>
        /// <param name="gatheringType">交易子类型 可选参数为：GOODS_BUY、SERVICE_BUY、MOBILE_PAY、REFUND分别代表商品购买，服务购买，手机支付，退款</param>
        /// <param name="tradeAmount">交易金额 单位为：RMB Yuan。取值范围为[0.01，100000000.00]，精确到小数点后两位。</param>
        /// <param name="sellerUserId">卖家id 如果合作伙伴是自卖，则填写合作伙伴身份ID。如果合作伙伴提供给其他买家，则需要在易极付注册 由易极付提供给软件商ID</param>
        /// <param name="tradeBizProductCode">业务产品编号 业务产品编号，由易极付提供 [网关及时到账 20130807_gyjyfasypay 转账到卡20130807_yjytocard]</param>
        /// <param name="phone">固定电话 [可空]</param>
        /// <param name="country">国籍 [可空]</param>
        /// <param name="zipcode">邮政编码 [可空]</param>
        /// <param name="email">邮箱 [可空]</param>
        /// <param name="address">详细地址 [可空]</param>
        /// <param name="tradeName">交易名称 商品的标题/交易标题/订单标题/订单关键字等。该参数最长为64个汉字。 [可空]</param>
        /// <param name="currency">币种 可选参数为：CNY、USD、JPY，分别代表人民币、美元，日元 [可空]</param>
        /// <param name="tradeMemo">交易备注 [可空]</param>
        /// <param name="outId">商品id 最大32位,支持字母和数据，不支持中文。 [可空]</param>
        /// <param name="name">商品名称 [可空]</param>
        /// <param name="memo">商品详情 商品详情，最大支持512个汉字 [可空]</param>
        /// <param name="price">商品单价 单位为：RMB Yuan。取值范围为[0.01，100000000.00]，精确到小数点后两位。 [可空]</param>
        /// <param name="quantity">商品数量 为正整数 [可空]</param>
        /// <param name="otherFee">商品其他费用 单位为：RMB Yuan。取值范围为[0.01，100000000.00]，精确到小数点后两位。 [可空]</param>
        /// <param name="unit">商品单位 [可空]</param>
        /// <param name="detailUrl">商品描述网址 [可空]</param>
        /// <param name="referUrl">商品来源网址 [可空]</param>
        /// <param name="category">商品类目 商品类目，最大支持16个汉字 [可空]</param>
        /// <param name="returnUrl">同步处理地址 [可空]</param>
        /// <param name="notifyUrl">异步处理地址 [可空]</param>
        /// <param name="errorNotifyUrl">错误处理地址 [可空]</param>
        /// <returns>即时到账接口url地址</returns>
        public string FastPay(string orderId, string userName, string realName, string certType, string certNo, string gender, string mobile,
            string payChannelApi, string gatheringType, string tradeAmount, string sellerUserId, string tradeBizProductCode, string returnUrl, string notifyUrl, string errorNotifyUrl,
            string phone = "", string country = "", string zipcode = "", string email = "", string address = "", string tradeName = "",
            string currency = "", string tradeMemo = "", string outId = "", string name = "", string memo = "",
            string price = "", string quantity = "", string otherFee = "", string unit = "", string detailUrl = "", string referUrl = "", string category = "")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "qFastPay");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", orderId);
            //dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userName", userName);
            dic.Add("realName", realName);
            dic.Add("certType", certType);
            dic.Add("certNo", certNo);
            dic.Add("gender", gender);
            dic.Add("mobile", mobile);
            dic.Add("payChannelApi", payChannelApi);
            dic.Add("gatheringType", gatheringType);
            dic.Add("tradeAmount", tradeAmount);
            dic.Add("sellerUserId", sellerUserId);
            dic.Add("tradeBizProductCode", tradeBizProductCode);
            dic.Add("phone", phone);
            dic.Add("country", country);
            dic.Add("zipcode", zipcode);
            dic.Add("email", email);
            dic.Add("address", address);
            dic.Add("tradeName", tradeName);
            dic.Add("currency", currency);
            dic.Add("tradeMemo", tradeMemo);
            dic.Add("outId", outId);
            dic.Add("name", name);
            dic.Add("memo", memo);
            dic.Add("price", price);
            dic.Add("quantity", quantity);
            dic.Add("otherFee", otherFee);
            dic.Add("unit", unit);
            dic.Add("detailUrl", detailUrl);
            dic.Add("referUrl", referUrl);
            dic.Add("category", category);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 转账
        /// </summary>
        /// <param name="tradeName">交易名称</param>
        /// <param name="buyerUserId">买家</param>
        /// <param name="sellerUserId">卖家</param>
        /// <param name="tradeBizProductCode">业务产品编号</param>
        /// <param name="gatheringType">交易子类型 交易子类型，可选值：GOODS_BUY商品购买) SERVICE_BUY(服务购买) MOBILE_PAY(手机支付) REFUND(退款)</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <param name="payerUserId"> [可选值]</param>
        /// <param name="tradeMerchantId">平台商户 平台商，在带个交易之中提供平台服务的商家 根据转账收费规则的不同进行传值 [可选值]</param>
        /// <param name="tradePartnerId">参与者 一般指进行充值或者提现，帐内转账等单纯业务时的个人用户或商户 根据转账收费规则的不同进行传值 [可选值]</param>
        /// <param name="parentNo">父交易号 [可选值]</param>
        /// <param name="tradeAmount">交易额 [可选值]</param>
        /// <param name="currency">币种 币种，默认为CNY(人民币)，可选值：CNY(人民币) USD(美元) JPY(日元") [可选值]</param>
        /// <param name="tradeMemo">交易备注 [可选值]</param>
        /// <param name="buyerMarkerMemo">买家备注 [可选值]</param>
        /// <param name="sellerMarkerMemo">卖家备注 [可选值]</param>
        /// <param name="goods">商品详情 以json格式传输 [可选值，一般不传值]</param>
        /// <returns>转账url地址</returns>
        public string TradeTransfer(string tradeName, string buyerUserId, string sellerUserId, string tradeBizProductCode, string gatheringType, string returnUrl, string notifyUrl, string errorNotifyUrl,
            string payerUserId = "", string tradeMerchantId = "", string tradePartnerId = "", string parentNo = "", string tradeAmount = "", string currency = "", string tradeMemo = "", string buyerMarkerMemo = "", string sellerMarkerMemo = "", string goods = "")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "tradeTransfer");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("tradeName", tradeName);
            dic.Add("buyerUserId", buyerUserId);
            dic.Add("sellerUserId", sellerUserId);
            dic.Add("tradeBizProductCode", tradeBizProductCode);
            dic.Add("gatheringType", gatheringType);
            dic.Add("payerUserId", payerUserId);
            dic.Add("tradeMerchantId", tradeMerchantId);
            dic.Add("tradePartnerId", tradePartnerId);
            dic.Add("parentNo", parentNo);
            dic.Add("tradeAmount", tradeAmount);
            dic.Add("currency", currency);
            dic.Add("tradeMemo", tradeMemo);
            dic.Add("buyerMarkerMemo", buyerMarkerMemo);
            dic.Add("sellerMarkerMemo", sellerMarkerMemo);
            dic.Add("goods", goods);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 支付渠道查询
        /// </summary>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <returns>支付渠道查询url地址</returns>
        public string PayChannelApi(string returnUrl, string notifyUrl, string errorNotifyUrl)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "queryPayChannelApi");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 银行联行号查询
        /// </summary>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <returns>银行联行号查询url地址</returns>
        public string BankNoQuery(string returnUrl, string notifyUrl, string errorNotifyUrl)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "bankNoQuery");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        #endregion

        #region 提现转账

        /// <summary>
        /// 接口函数 - 用户提现
        /// </summary>
        /// <param name="userId">关联用户ID</param>
        /// <param name="amount">提现金额</param>
        /// <param name="bankAccountNo">银行开户账户号</param>
        /// <param name="bankAccountName">银行账户开户名</param>
        /// <param name="bankCnapsNo">银行联行号</param>
        /// <param name="provName">银行省名</param>
        /// <param name="cityName">银行市名</param>
        /// <param name="tradeBizProductCode">业务产品编号 测试环境不传此值，线上环境根据合同收费规则进行传递</param>
        /// <param name="tradePartnerId">参与者 一般指进行充值或者提现，帐内转账等单纯业务时的个人用户或商户 测试环境不传此值，线上环境根据合同收费规则进行判断是否传入此值</param>
        /// <param name="bankCode">银行简写</param>
        /// <param name="publicTag">对公对私 对公对私，可选值：Y(是) N(否) NY(特殊) 对公-Y，对私-N 对公是单位用户，对私是个人用户。</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <param name="outBizNo">外部订单号 可以不传，则默认当前orderNo [可选值]</param>
        /// <param name="bankAddress">开户行地址 [可选值]</param>
        /// <param name="memo">提现指令备注 [可选值]</param>
        /// <param name="tradeMerchantId">平台商户 平台商，在带个交易之中提供平台服务的商家 测试环境不传此值，线上环境根据合同收费规则进行判断是否传入此值 [可选值]</param>
        /// <param name="applyIsInter">是否后台审核 是否审核，传“N”就不审核，不传或者其他值就审核 审核就走T+1提现，一天之后才能提现成功。不审核就是实时提现 [可选值]</param>
        /// <returns>用户提现url地址</returns>
        public string ApplyWithdraw(string userId, string amount, string bankAccountNo, string bankAccountName, string bankCnapsNo, string provName, string cityName, string tradeBizProductCode, string tradePartnerId, string bankCode, string publicTag, string returnUrl, string notifyUrl, string errorNotifyUrl,
            string outBizNo = "", string bankAddress = "", string memo = "", string tradeMerchantId = "", string applyIsInter = "")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "yzzApplyWithdraw");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userId", userId);
            dic.Add("amount", amount);
            dic.Add("bankAccountNo", bankAccountNo);
            dic.Add("bankAccountName", bankAccountName);
            dic.Add("bankCnapsNo", bankCnapsNo);
            dic.Add("provName", provName);
            dic.Add("cityName", cityName);
            dic.Add("tradeBizProductCode", tradeBizProductCode);
            dic.Add("tradePartnerId", tradePartnerId);
            dic.Add("bankCode", bankCode);
            dic.Add("publicTag", publicTag);
            dic.Add("outBizNo", outBizNo);
            dic.Add("bankAddress", bankAddress);
            dic.Add("memo", memo);
            dic.Add("tradeMerchantId", tradeMerchantId);
            dic.Add("applyIsInter", applyIsInter);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        /// <summary>
        /// 接口函数 - 提现记录查询服务
        /// </summary>
        /// <param name="userId">关联用户ID</param>
        /// <param name="returnUrl">同步处理地址</param>
        /// <param name="notifyUrl">异步处理地址</param>
        /// <param name="errorNotifyUrl">错误处理地址</param>
        /// <param name="currPage">当前页 [可选值]</param>
        /// <param name="pageSize">每页数量 [可选值]</param>
        /// <returns>提现记录查询服务url地址</returns>
        public string WithdrawQuery(string userId, string returnUrl, string notifyUrl, string errorNotifyUrl, string currPage = "", string pageSize = "")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            #region 基本参数
            dic.Add("service", "withdraw.query");//服务名称,这个是识别是何接口实现何功能的标识,请勿修改
            dic.Add("partnerId", Config.PatnerId);
            dic.Add("returnUrl", returnUrl);
            dic.Add("notifyUrl", notifyUrl);
            dic.Add("errorNotifyUrl", errorNotifyUrl);
            dic.Add("orderNo", OrderNo);
            #endregion

            #region 业务参数
            dic.Add("userId", userId);
            dic.Add("currPage", currPage);
            dic.Add("pageSize", pageSize);
            #endregion

            return Functions.BuildUrl(dic, Config.Key);
        }

        #endregion

        #region Notify回调验证
        /// <summary>
        /// 根据返回的通知url生成md5值
        /// </summary>
        /// <param name="coll">NameValues集合，通过Request.QueryString或Request.Form方式取得</param>
        /// <remarks>用于验证返回的通知是否匹配</remarks>
        /// <returns>返回sign串</returns>
        public string GetMD5SignWithNotifyUrl(System.Collections.Specialized.NameValueCollection coll)
        {
            return Functions.GetSign(coll, Config.Key);
        }
        #endregion

        #region OrderNo
        /// <summary>
        /// 生成OrderNo
        /// 易极付合作合作伙伴网站唯一订单号。生成规则：8位年月日+8位序列  如：2012121200000001
        /// </summary>
        public string OrderNo
        {
            get
            {
                return DateTime.Now.ToString("yyyyMMddHHmmssff");
            }
        }
        #endregion

        #region 银行编码
        private Dictionary<string, string> _bank_code = new Dictionary<string, string>();
        /// <summary>
        /// 银行编码
        /// </summary>
        public Dictionary<string, string> BANK_CODE
        {
            get
            {
                if (_bank_code.Count < 1)
                {
                    _bank_code.Add("ABC", "中国农业银行");
                    _bank_code.Add("BOC", "中国银行");
                    _bank_code.Add("COMM", "交通银行");
                    _bank_code.Add("CCB", "中国建设银行");
                    _bank_code.Add("CEB", "中国光大银行");
                    _bank_code.Add("CIB", "兴业银行");
                    _bank_code.Add("CMB", "招商银行");
                    _bank_code.Add("CMBC", "民生银行");
                    _bank_code.Add("CITIC", "中信银行");
                    _bank_code.Add("CQRCB", "重庆农村商业银行");
                    _bank_code.Add("ICBC", "中国工商银行");
                    _bank_code.Add("PSBC", "中国邮政储蓄银行");
                    _bank_code.Add("SPDB", "浦发银行");
                    _bank_code.Add("UNION", "中国银联");
                    _bank_code.Add("CQCB", "重庆银行");
                    _bank_code.Add("GDB", "广东发展银行");
                    _bank_code.Add("SDB", "深圳发展银行");
                    _bank_code.Add("HXB", "华夏银行");
                    _bank_code.Add("CQTGB", "重庆三峡银行");
                    _bank_code.Add("PINGANBANK", "平安银行");
                    _bank_code.Add("BANKSH", "上海银行");
                }

                return _bank_code;
            }
        }
        #endregion
    }
}
