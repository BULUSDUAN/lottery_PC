using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    [Serializable]
    public abstract class RegisterInfo
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 注册IP
        /// </summary>
        public string RegisterIp { get; set; }
        /// <summary>
        /// 注册引用页面
        /// </summary>
        public string ReferrerUrl { get; set; }
        /// <summary>
        /// 注册引用
        /// </summary>
        public string Referrer { get; set; }
        /// <summary>
        /// 注册类型
        /// </summary>
        public string RegType { get; set; }
        /// <summary>
        /// 代理商编码
        /// </summary>
        public string AgentId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 是否允许注册
        /// </summary>
        public bool IsAllowRegist { get; set; }
    }
    /// <summary>
    /// 首页活动注册信息
    /// </summary>
    public class RegisterInfo_Local_Index : RegisterInfo
    {
        public string ComeFrom { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }
        public string Idcode { get; set; }
        public string Realname { get; set; }
        public string Mobile { get; set; }
    }
    /// <summary>
    /// 用户注册信息
    /// </summary>
    [Serializable]
    public class RegisterInfo_Local : RegisterInfo
    {
        public string ComeFrom { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }
        public string Mobile { get; set; }
    }
    /// <summary>
    /// 后台用户注册信息
    /// </summary>
    public class RegisterInfo_Admin : RegisterInfo
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 所属角色
        /// </summary>
        public List<string> RoleIdList { get; set; }
    }
    /// <summary>
    /// 支付宝用户注册信息
    /// </summary>
    [Serializable]
    public class RegisterInfo_Alipay : RegisterInfo
    {
        /// <summary>
        /// 支付宝登录帐号
        /// </summary>
        public string LoginAccount { get; set; }
        public string DisplayName { get; set; }
        public string OpenId { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// 是否是支付宝金帐户
        /// </summary>
        public bool IsGoldAccount { get; set; }
        /// <summary>
        /// 帐户等级
        /// </summary>
        public string AccountLevel { get; set; }
        /// <summary>
        /// 支付宝用户状态
        /// </summary>
        public string UserStatus { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 是否认证手机
        /// </summary>
        public bool IsAuthMobile { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 是否证件认证
        /// </summary>
        public bool IsAuthRealName { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string CardNumber { get; set; }
        /// <summary>
        /// 是否邮箱认证
        /// </summary>
        public bool IsAuthEmail { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
    }
    /// <summary>
    /// QQ用户注册信息
    /// </summary>
    public class RegisterInfo_QQ : RegisterInfo
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string DisplayName { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// 开放编号
        /// </summary>
        public string OpenId { get; set; }
    }
}
