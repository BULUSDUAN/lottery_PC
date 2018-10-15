using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Enum
{
   
    /// <summary>
    /// 结果状态
    /// </summary>
    public enum ResultStatus
    {
        /// <summary>
        /// 成功的
        /// </summary>
        Successful = 1,
        /// <summary>
        /// 失败的
        /// </summary>
        Failed = 2,
        /// <summary>
        /// 未知的
        /// </summary>
        Unknown = 9,
    }

    #region XML解析

    /// <summary>
    /// 属性映射到XML节点类型
    /// </summary>
    public enum MappingType
    {
        /// <summary>
        /// 元素
        /// </summary>
        Element = 0,
        /// <summary>
        /// 属性
        /// </summary>
        Attribute = 1,
    }
    /// <summary>
    /// 对象类型
    /// </summary>
    public enum XmlObjectType
    {
        /// <summary>
        /// 单项
        /// </summary>
        Item = 0,
        /// <summary>
        /// 列表
        /// </summary>
        List = 1,
    }

    #endregion

    #region 关于三峡付

    public enum TransResult
    {
        /// <summary>
        /// 失败
        /// </summary>
        Fail = 0,
        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 超时
        /// </summary>
        OutTime = 2,

    }

    #endregion

    /// <summary>
    /// 微信订单状态
    /// </summary>
    public enum WXOrderStatus
    {
        /// <summary>
        /// 支付成功
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// 用户正在支付
        /// </summary>
        USERPAYING = 1,
        /// <summary>
        /// 订单不存在
        /// </summary>
        ORDERNOTEXIST = 2,
        /// <summary>
        /// 未支付
        /// </summary>
        NOTPAY = 3,
        /// <summary>
        /// 其它错误
        /// </summary>
        ERROR = 9,
    }
}
