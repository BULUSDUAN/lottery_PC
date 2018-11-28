// @Description: 快钱VPOS-CNP网关接口范例
// @Copyright (c) 上海快钱信息服务有限公司
// @version 1.0
using System;
using System.Collections;

namespace Common.Gateway.KQ.Pay
{
    /// <summary>
    /// 根据应答码返回应答文本
    /// 使用方法：初始化返回代码 KQMsg Msg = new KQMsg(); 给变量赋值string kmsg = Msg.KuaiQianMsg["00"].ToString(); 变量值kmsg=交易成功。
    /// </summary>
    public class KQMsg
    {
        public static Hashtable KuaiQianMsg = new Hashtable();
        public KQMsg()
        {
            #region 根据应答码返回应答文本
            if (KuaiQianMsg.Count == 0)
            {
                KuaiQianMsg.Add("SK", "无效的卡");
                KuaiQianMsg.Add("Q2", "有效期错");
                KuaiQianMsg.Add("I0", "外部交易跟踪编号（如：商户订单号）发生重复");
                KuaiQianMsg.Add("I1", "请提供正确的持卡人姓名，必须与申请信用卡时的姓名一致");
                KuaiQianMsg.Add("I2", "请提供正确的验证码（CVV2），验证码在卡背面签名栏后的三位数字串");
                KuaiQianMsg.Add("I3", "请提供正确的证件号码，必须与申请信用卡时的证件号码一致");
                KuaiQianMsg.Add("I4", "请提供正确的卡有效期，卡有效期是在卡号下面的4位数字");
                KuaiQianMsg.Add("I5", "超出持卡人设置的交易限额，请持卡人联系发卡银行调高限额");
                KuaiQianMsg.Add("I6", "无效证件类型");
                KuaiQianMsg.Add("I7", "运单不存在：这笔运单没有导入到当前送货员使用终端所绑定的物流站中");
                KuaiQianMsg.Add("I8", "这笔运单已被收款");
                KuaiQianMsg.Add("C0", "处理中，请稍候......");
                KuaiQianMsg.Add("00", "交易成功");
                KuaiQianMsg.Add("01", "请联系发卡行，或核对卡信息后重新输入");
                KuaiQianMsg.Add("02", "查发卡方的特殊条件，请联系快钱公司");
                KuaiQianMsg.Add("03", "无效商户");
                KuaiQianMsg.Add("04", "无效终端");
                KuaiQianMsg.Add("06", "出错");
                KuaiQianMsg.Add("05", "不予承兑,或核对卡信息后重新输入");
                KuaiQianMsg.Add("07", "特定条件下没收卡");
                KuaiQianMsg.Add("09", "请求正在处理中");
                KuaiQianMsg.Add("11", "部分金额批准");
                KuaiQianMsg.Add("12", "无效交易，或核对卡信息后重新输入");
                KuaiQianMsg.Add("13", "13=无效金额，交易金额不在许可的范围内，疑问请联系快钱公司");
                KuaiQianMsg.Add("14", "无效卡号（无此号），或核对卡信息后重新输入");
                KuaiQianMsg.Add("15", "无此发卡方/拒绝");
                KuaiQianMsg.Add("16", "批准更新第三磁道");
                KuaiQianMsg.Add("17", "客户取消");
                KuaiQianMsg.Add("18", "商户保证金对应的可交易额度不足，请联系快钱公司");
                KuaiQianMsg.Add("19", "您的快钱账户余额不足以进行退货，请充值到快钱账户后，再进行退货操作");
                KuaiQianMsg.Add("20", "无效响应");
                KuaiQianMsg.Add("21", "不能采取行动");
                KuaiQianMsg.Add("22", "故障怀疑");
                KuaiQianMsg.Add("23", "不可接受的交易费");
                KuaiQianMsg.Add("25", "找不到原始交易");
                KuaiQianMsg.Add("30", "格式错误");
                KuaiQianMsg.Add("31", "交换中心不支持的银行");
                KuaiQianMsg.Add("32", "商户不受理的卡");
                KuaiQianMsg.Add("33", "过期的卡");
                KuaiQianMsg.Add("34", "有作弊嫌疑");
                KuaiQianMsg.Add("35", "请联系快钱公司");
                KuaiQianMsg.Add("36", "受限制的卡");
                KuaiQianMsg.Add("37", "风险卡，请联系快钱公司");
                KuaiQianMsg.Add("38", "超过允许的试输入");
                KuaiQianMsg.Add("39", "无贷记账户");
                KuaiQianMsg.Add("40", "请求的功能尚不支持");
                KuaiQianMsg.Add("41", "挂失卡");
                KuaiQianMsg.Add("42", "无此账户");
                KuaiQianMsg.Add("43", "被窃卡");
                KuaiQianMsg.Add("44", "无此投资账户");
                KuaiQianMsg.Add("51", "资金不足");
                KuaiQianMsg.Add("52", "无此支票账户");
                KuaiQianMsg.Add("53", "无此储蓄卡账户");
                KuaiQianMsg.Add("54", "过期的卡");
                KuaiQianMsg.Add("55", "密码错误");
                KuaiQianMsg.Add("56", "无此卡记录");
                KuaiQianMsg.Add("57", "不允许持卡人进行的交易");
                KuaiQianMsg.Add("58", "不允许终端进行的交易");
                KuaiQianMsg.Add("59", "有作弊嫌疑");
                KuaiQianMsg.Add("60", "受卡方与代理方联系");
                KuaiQianMsg.Add("61", "超出取款转账金额限制");
                KuaiQianMsg.Add("62", "受限制的卡");
                KuaiQianMsg.Add("63", "侵犯安全");
                KuaiQianMsg.Add("64", "原始金额错误");
                KuaiQianMsg.Add("65", "超出取款次数限制");
                KuaiQianMsg.Add("66", "受卡方通知受理方安全部门");
                KuaiQianMsg.Add("67", "强行受理（要求在自动会员机上没收此卡）");
                KuaiQianMsg.Add("68", "无法在正常时间内获得交易应答，请稍后重试");
                KuaiQianMsg.Add("75", "允许的输入PIN次数超限");
                KuaiQianMsg.Add("76", "无效账户");
                KuaiQianMsg.Add("80", "交易拒绝");
                KuaiQianMsg.Add("90", "正在日终处理（系统终止一天的活动，开始第二天的活动，交易在几分钟后可再次发送）");
                KuaiQianMsg.Add("91", "发卡方或交换中心不能操作");
                KuaiQianMsg.Add("92", "金融机构或中间网络设施找不到或无法达到、金融机构签退");
                KuaiQianMsg.Add("93", "交易违法、不能完成");
                KuaiQianMsg.Add("94", "重复交易");
                KuaiQianMsg.Add("95", "核对差错");
                KuaiQianMsg.Add("96", "系统异常、失效");
                KuaiQianMsg.Add("97", "ATM/POS 终端号找不到");
                KuaiQianMsg.Add("98", "交换中心收不到发卡方应答");
                KuaiQianMsg.Add("99", "密码格式错");
                KuaiQianMsg.Add("B.BIN.0001", " cardNo is error!无法识别的主账号(卡号)");

            }
            #endregion
        }
    }
}
