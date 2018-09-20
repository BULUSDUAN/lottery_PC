using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using app.lottery.site.Controllers;
using Common.Utilities;
using Common.XmlAnalyzer;
using External.Core.SiteMessage;

namespace app.lottery.site.iqucai.Controllers
{
    public class HelpController : BaseController
    {
        //
        // GET: /Help/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 常见问题
        /// </summary>
        public ActionResult usual()
        {
            return View();
        }

        /// <summary>
        /// 购彩帮助
        /// </summary>
        public ActionResult buyguide()
        {
            return View();
        }

        /// <summary>
        /// 合买帮助
        /// </summary>
        public ActionResult hemai()
        {
            return View();
        }
        /// <summary>
        /// 代购
        /// </summary>
        public ActionResult daigou()
        {
            return View();
        }

        /// <summary>
        /// 关于我们
        /// </summary>
        public ActionResult abtus()
        {
            return View();
        }

        /// <summary>
        /// 用户协议
        /// </summary>
        public ActionResult yhxy()
        {
            return View();
        }

        /// <summary>
        /// 用户协议
        /// </summary>
        public ActionResult betpact()
        {
            return View();
        }
        public ActionResult betpact1()
        {
            return View();
        }
        /// <summary>
        /// 单式格式
        /// </summary>
        public ActionResult dsgs()
        {
            return View();
        }
        public ActionResult dsgs1()
        {
            return View();
        }
        /// <summary>
        /// 意见反馈
        /// </summary>
        /// <returns></returns>
        public ActionResult suggest()
        {
            ViewBag.User = CurrentUser;
            return View();
        }

        public JsonResult submitsuggestion()
        {
            try
            {
                var contentxt = PreconditionAssert.IsNotEmptyString(Request["contentxt"], "请输入评论内容");
                var PageOpenSpeed = string.IsNullOrEmpty(Request["PageOpenSpeed"]) ? "1" : Request["PageOpenSpeed"];//页面打开速度
                var InterfaceBeautiful = string.IsNullOrEmpty(Request["InterfaceBeautiful"]) ? "1" : Request["InterfaceBeautiful"];//界面设计美观
                var ComposingReasonable = string.IsNullOrEmpty(Request["ComposingReasonable"]) ? "1" : Request["ComposingReasonable"];//排版展示合理
                var OperationReasonable = string.IsNullOrEmpty(Request["OperationReasonable"]) ? "1" : Request["OperationReasonable"];//操作过程合理
                var ContentConveyDistinct = string.IsNullOrEmpty(Request["ContentConveyDistinct"]) ? "1" : Request["ContentConveyDistinct"];//内容传达清晰
                PreconditionAssert.IsFalse(SensitiveAnalyzer.IsHaveSensitive(contentxt), "反馈内容不允许包含敏感词");
                UserIdeaInfo_Add ideainfo = new UserIdeaInfo_Add()
                {
                    Description = contentxt,
                    Category = "网站反馈",
                    IsAnonymous = false,
                    CreateUserId = CurrentUser.LoginInfo.UserId,
                    CreateUserDisplayName = CurrentUser.LoginInfo.DisplayName,
                    CreateUserMoibile = "",
                    PageOpenSpeed = decimal.Parse(PageOpenSpeed),
                    InterfaceBeautiful = decimal.Parse(InterfaceBeautiful),
                    ComposingReasonable = decimal.Parse(ComposingReasonable),
                    OperationReasonable = decimal.Parse(OperationReasonable),
                    ContentConveyDistinct = decimal.Parse(ContentConveyDistinct),
                };
                var result = WCFClients.ExternalClient.SubmitUserIdea(ideainfo);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message });
            }
        }
        /// <summary>
        /// 左边
        /// </summary>
        public PartialViewResult left(string type, string name)
        {
            ViewBag.Name = name;
            ViewBag.Type = type;
            return PartialView();
        }

        public ActionResult moneyser()
        {

            return View();
        }

        #region 注册模版
        /// <summary>
        /// 注册
        /// </summary>
        public ActionResult login()
        {

            return View();
        }
        /// <summary>
        ///  会员
        /// </summary>
        /// <returns></returns>
        public ActionResult login1()
        {
            return View();
        }
        /// <summary>
        /// 注意事项
        /// </summary>
        public ActionResult notes()
        {
            return View();
        }
        /// <summary>
        /// 合作帐号
        /// </summary>
        public ActionResult cooperation()
        {
            return View();
        }
        /// <summary>
        /// 是否可以注册多个帐号
        /// </summary>
        public ActionResult isokregister()
        {
            return View();
        }
        /// <summary>
        /// 注册未收到验证邮件？
        /// </summary>
        public ActionResult validationMail()
        {

            return View();
        }
        /// <summary>
        /// 未收到手机验证码怎么办？
        /// </summary>
        public ActionResult validationPhone()
        {

            return View();
        }



        #endregion

        #region 充值模版

        #region  充值介绍
        /// <summary>
        /// 充值列表
        /// </summary>
        public ActionResult payintroduce()
        {
            return View();
        }
        /// <summary>
        /// 充值方式
        /// </summary>
        public ActionResult payway()
        {
            return View();
        }
        /// <summary>
        /// 充值多久到账
        /// </summary>
        public ActionResult payAccount()
        {
            return View();
        }
        /// <summary>
        /// 充值未到账
        /// </summary>
        public ActionResult payNotAccount()
        {
            return View();
        }
        /// <summary>
        /// 充值手续费
        /// </summary>
        public ActionResult paypoundage()
        {
            return View();
        }

        #endregion

        #region  网银充值
        /// <summary>
        /// 网银充值
        /// </summary>
        public ActionResult EbanksPay()
        {
            return View();
        }

        /// <summary>
        /// 网银充值未即时到账
        /// </summary>
        public ActionResult EbanksNotAccount()
        {
            return View();
        }
        /// <summary>
        /// 支付失败，调试错误
        /// </summary>
        public ActionResult EbanksDebugError()
        {
            return View();
        }
        /// <summary>
        /// 网上支付时提示此网站出具的安全证书已过期或还未生效
        /// </summary>
        public ActionResult EbanksSafetyLCE()
        {
            return View();

        }
        /// <summary>
        /// 充值页面右下方提示网页错误
        /// </summary>
        public ActionResult EbanksWebpageError()
        {
            return View();
        }
        /// <summary>
        ///  网银充值，为什么点击网上银行充值的时候，页面打不开，不能进入到银行的交易页面
        /// </summary>
        public ActionResult EbanksTradePage()
        {
            return View();
        }

        /// <summary>
        ///  为什么使用网上银行充值，选择银行后点击下一步没有反应？
        /// </summary>
        public ActionResult EbanksNotReaction()
        {
            return View();
        }
        /// <summary>
        ///  如何通过网上银行在线充值？
        /// </summary>
        public ActionResult EbanksOnlinePay()
        {
            return View();
        }
        /// <summary>
        ///  如何开通网上银行？
        /// </summary>
        public ActionResult EbanksDredge()
        {
            return View();
        }


        #endregion

        #region 支付宝
        /// <summary>
        /// 支付宝
        /// </summary>
        public ActionResult Alipay()
        {
            return View();
        }
        /// <summary>
        /// 支付宝付款
        /// </summary>
        public ActionResult AlipayPay()
        {
            return View();
        }

        public ActionResult AlipaySelfHelp()
        {
            return View();
        }


        #endregion

        #region 快捷支付
        /// <summary>
        /// 快捷支付
        /// </summary>
        public ActionResult Quickpayment()
        {
            return View();
        }
        /// <summary>
        /// 快捷支付付款
        /// </summary>
        public ActionResult QuickpaymentPay()
        {
            return View();
        }
        #endregion
        #endregion

        #region 我的账户

        #region  实名认证
        /// <summary>
        /// 实名认证
        /// </summary>
        public ActionResult Approve()
        {
            return View();
        }
        /// <summary>
        /// 账户如何实名认证？
        /// </summary>
        public ActionResult whatApprove()
        {
            return View();
        }
        /// <summary>
        /// 身份信息都是正确的，为什么不能实名认证成功？
        /// </summary>
        public ActionResult information()
        {
            return View();
        }
        /// <summary>
        /// 账户实名认证的相关注意事项
        /// </summary>
        public ActionResult Attention()
        {
            return View();
        }
        /// <summary>
        /// 未填写真实姓名可能会带来的风险
        /// </summary>
        public ActionResult NotApprove()
        {
            return View();
        }

        #endregion

        #region  账户密码
        /// <summary>
        /// 账户密码
        /// </summary>
        public ActionResult administrator()
        {
            return View();
        }
        /// <summary>
        /// 忘记密码怎么找回？如何修改密码？
        /// </summary>
        public ActionResult password()
        {
            return View();
        }
        /// <summary>
        /// 忘记用户名怎么办？
        /// </summary>
        public ActionResult username()
        {
            return View();
        }
        /// <summary>
        /// 在登录时为什么验证码输入是对的，却提示我输入错误？
        /// </summary>
        public ActionResult verificationcode()
        {
            return View();

        }
        /// <summary>
        /// 我看不到验证码的图片，怎么办？
        /// </summary>
        public ActionResult VerifyImage()
        {
            return View();
        }
        #endregion

        #region 账户安全
        /// <summary>
        /// 账户安全
        /// </summary>
        public ActionResult AccountSecurity()
        {
            return View();
        }
        /// <summary>
        /// 如何确保账户资金安全？
        /// </summary>
        public ActionResult safety()
        {
            return View();
        }

        /// <summary>
        /// 如何绑定手机？
        /// </summary>
        public ActionResult bindingPhone()
        {
            return View();
        }

        /// <summary>
        /// 成年人购彩限制的说明
        /// </summary>
        public ActionResult astrict()
        {
            return View();
        }

        /// <summary>
        /// 邮箱未收到投注信息的邮件？
        /// </summary>
        public ActionResult Email()
        {
            return View();
        }

        /// <summary>
        /// 手机收不到开奖通知？
        /// </summary>
        public ActionResult notification()
        {
            return View();
        }
        /// <summary>
        /// 如何修改及取消绑定的手机号码？
        /// </summary>
        public ActionResult NotbindingPhone()
        {
            return View();
        }
        /// <summary>
        /// 如何修改已绑定的银行卡（未绑定手机）？
        /// </summary>
        public ActionResult bindingBank()
        {
            return View();
        }
        /// <summary>
        /// 如何修改已绑定的银行卡（有绑定手机）？
        /// </summary>
        public ActionResult bindingBankNotPhone()
        {
            return View();
        }
        /// <summary>
        /// 如何修改个人基本资料？
        /// </summary>
        public ActionResult personalData()
        {
            return View();
        }
        #endregion

        #region 网银充值订单查询
        /// <summary>
        /// 网银充值订单查询
        /// </summary>
        public ActionResult Ebank()
        {
            return View();
        }
        /// <summary>
        /// 支付宝订单查询
        /// </summary>
        public ActionResult EbankAlipay()
        {
            return View();
        }
        /// <summary>
        /// 广东发展银行
        /// </summary>
        public ActionResult EbankGDB()
        {
            return View();
        }
        /// <summary>
        /// 光大银行
        /// </summary>
        public ActionResult EbankCEB()
        {
            return View();
        }
        /// <summary>
        /// 深圳发展银行
        /// </summary>
        public ActionResult EbankSDB()
        {
            return View();
        }
        /// <summary>
        /// 民生银行
        /// </summary>
        public ActionResult EbankCMBC()
        {
            return View();
        }
        /// <summary>
        /// 兴业银行
        /// </summary>
        public ActionResult EbankCIB()
        {
            return View();
        }
        /// <summary>
        /// 中信银行
        /// </summary>
        public ActionResult EbankCITIC()
        {
            return View();
        }
        /// <summary>
        /// 浦东银行
        /// </summary>
        public ActionResult EbankPDB()
        {
            return View();
        }
        /// <summary>
        /// 中国银行
        /// </summary>
        public ActionResult EbankBOC()
        {
            return View();
        }
        /// <summary>
        /// 交通银行
        /// </summary>
        public ActionResult EbankBOCOM()
        {
            return View();
        }
        /// <summary>
        /// 建设银行
        /// </summary>
        public ActionResult EbankCCB()
        {
            return View();
        }
        /// <summary>
        /// 招商银行
        /// </summary>
        public ActionResult EbankCMB()
        {
            return View();
        }
        /// <summary>
        /// 工商银行
        /// </summary>
        public ActionResult EbankICBC()
        {
            return View();
        }
        /// <summary>
        /// 农业银行
        /// </summary>
        public ActionResult EbankABC()
        {
            return View();
        }
        #endregion

        #region  隐私设置
        /// <summary>
        /// 隐私设置
        /// </summary>
        public ActionResult privacySet()
        {
            return View();
        }
        /// <summary>
        /// 什么是隐私设置？
        /// </summary>
        public ActionResult privacySet1()
        {
            return View();
        }
        /// <summary>
        /// 怎样设置隐私保护？
        /// </summary>
        public ActionResult privacySet2()
        {
            return View();
        }
        /// <summary>
        /// 设置隐私保护应注意些什么
        /// </summary>
        public ActionResult privacySet3()
        {
            return View();
        }
        /// <summary>
        /// 隐私设置后立即生效吗
        /// </summary>
        public ActionResult privacySet4()
        {
            return View();
        }

        #endregion

        #region  红包
        /// <summary>
        /// 红包
        /// </summary>
        public ActionResult Redpackets()
        {
            return View();
        }
        /// <summary>
        /// 什么是红包？
        /// </summary>
        public ActionResult Redpackets1()
        {
            return View();
        }
        /// <summary>
        /// 怎样使用红包购彩？
        /// </summary>
        public ActionResult Redpackets2()
        {
            return View();
        }
        /// <summary>
        /// 红包购彩撤单了投注金额会返还吗
        /// </summary>
        public ActionResult Redpackets3()
        {
            return View();
        }
        /// <summary>
        /// 红包买彩票中奖了怎么提款
        /// </summary>
        public ActionResult Redpackets4()
        {
            return View();
        }
        /// <summary>
        /// 怎样获得红包
        /// </summary>
        public ActionResult Redpackets5()
        {
            return View();
        }
        /// <summary>
        /// 怎么查看红包明细？红包余额是否可以提现
        /// </summary>
        public ActionResult Redpackets6()
        {
            return View();
        }

        #endregion
        #endregion

        #region 购买彩票

        #region  如何购彩
        /// <summary>
        /// 如何购彩
        /// </summary>
        public ActionResult ForTheColor()
        {
            return View();
        }
        /// <summary>
        /// 如何查看购彩记录？
        /// </summary>
        public ActionResult ForTheColor1()
        {
            return View();
        }
        /// <summary>
        /// 什么是代购？
        /// </summary>
        public ActionResult ForTheColor2()
        {
            return View();
        }
        /// <summary>
        /// 什么是机选？
        /// </summary>
        public ActionResult ForTheColor3()
        {
            return View();
        }
        /// <summary>
        /// 什么是胆拖？
        /// </summary>
        public ActionResult ForTheColor4()
        {
            return View();
        }
        /// <summary>
        /// 什么是直选？组三？组六？
        /// </summary>
        public ActionResult ForTheColor5()
        {
            return View();
        }
        /// <summary>
        /// 什么是单式上传？
        /// </summary>
        public ActionResult ForTheColor6()
        {
            return View();
        }
        /// <summary>
        /// 什么是限号？
        /// </summary>
        public ActionResult ForTheColor7()
        {
            return View();
        }
        /// <summary>
        /// 彩票有没有时间限制？
        /// </summary>
        public ActionResult ForTheColor8()
        {
            return View();
        }
        /// <summary>
        /// 什么是和值？
        /// </summary>
        public ActionResult ForTheColor9()
        {
            return View();
        }
        /// <summary>
        /// 什么是追号？
        /// </summary>
        public ActionResult ForTheColor10()
        {
            return View();
        }
        /// <summary>
        /// 追号有哪些方式？
        /// </summary>
        public ActionResult ForTheColor11()
        {
            return View();
        }
        /// <summary>
        /// 追号有哪些规则？
        /// </summary>
        public ActionResult ForTheColor12()
        {
            return View();
        }
        /// <summary>
        /// 什么是复式投注？
        /// </summary>
        public ActionResult ForTheColor13()
        {
            return View();
        }
        /// <summary>
        /// 什么是单式投注？
        /// </summary>
        public ActionResult ForTheColor14()
        {
            return View();
        }

        #endregion

        #region  合买
        /// <summary>
        /// 合买
        /// </summary>
        public ActionResult ShoppingService()
        {
            return View();
        }
        /// <summary>
        /// 合买奖金如何分配？
        /// </summary>
        public ActionResult ShoppingService1()
        {
            return View();
        }
        /// <summary>
        /// 什么是合买？
        /// </summary>
        public ActionResult ShoppingService2()
        {
            return View();
        }
        /// <summary>
        /// 什么是方案保底？
        /// </summary>
        public ActionResult ShoppingService3()
        {
            return View();
        }
        /// <summary>
        /// 合买方案如何尽快满员？
        /// </summary>
        public ActionResult ShoppingService4()
        {
            return View();
        }
        /// <summary>
        /// 合买提成？
        /// </summary>
        public ActionResult ShoppingService5()
        {
            return View();
        }
        /// <summary>
        /// 什么是合买战绩？
        /// </summary>
        public ActionResult ShoppingService6()
        {
            return View();
        }
        /// <summary>
        /// 竞技类彩票战绩规则
        /// </summary>
        public ActionResult ShoppingService7()
        {
            return View();
        }
        /// <summary>
        /// 数字类彩票战绩规则？
        /// </summary>
        public ActionResult ShoppingService8()
        {
            return View();
        }
        /// <summary>
        /// 什么叫方案置顶？
        /// </summary>
        public ActionResult ShoppingService9()
        {
            return View();
        }
        /// <summary>
        /// 方案置顶有哪些规则？
        /// </summary>
        public ActionResult ShoppingService10()
        {
            return View();
        }
        /// <summary>
        /// 什么是清保？
        /// </summary>
        public ActionResult ShoppingService11()
        {
            return View();
        }
        /// <summary>
        /// 什么是资金冻结？
        /// </summary>
        public ActionResult ShoppingService12()
        {
            return View();
        }
        /// <summary>
        /// 什么是网站保底？
        /// </summary>
        public ActionResult ShoppingService13()
        {
            return View();
        }
        /// <summary>
        /// 发起方案时没有设置方案保密，发起之后可以修改吗？？
        /// </summary>
        public ActionResult ShoppingService14()
        {
            return View();
        }
        /// <summary>
        /// 什么是方案保密？
        /// </summary>
        public ActionResult ShoppingService15()
        {
            return View();
        }
        /// <summary>
        /// 发错了方案如何处理？
        /// </summary>
        public ActionResult ShoppingService16()
        {
            return View();
        }
        /// <summary>
        /// 什么叫方案上传？
        /// </summary>
        public ActionResult ShoppingService17()
        {
            return View();
        }
        /// <summary>
        /// 单式上传支持哪些格式？
        /// </summary>
        public ActionResult ShoppingService18()
        {
            return View();
        }
        /// <summary>
        /// 如何参与合买？
        /// </summary>
        public ActionResult ShoppingService19()
        {
            return View();
        }
        /// <summary>
        /// 发起合买方案是否有相关限制？
        /// </summary>
        public ActionResult ShoppingService20()
        {
            return View();
        }
        /// <summary>
        /// 如何发起合买方案？
        /// </summary>
        public ActionResult ShoppingService21()
        {
            return View();
        }
        #endregion

        #region 自动跟单
        /// <summary>
        /// 自动跟单
        /// </summary>
        public ActionResult animationDocumentary()
        {
            return View();
        }
        /// <summary>
        /// 什么是自动跟单
        /// </summary>
        public ActionResult animationDocumentary1()
        {
            return View();
        }
        /// <summary>
        /// 设置了自动跟单是否还可以参与其他合买
        /// </summary>
        public ActionResult animationDocumentary2()
        {
            return View();
        }
        /// <summary>
        /// 怎么取消自动跟单
        /// </summary>
        public ActionResult animationDocumentary3()
        {
            return View();
        }
        /// <summary>
        /// 如何定制跟单
        /// </summary>
        public ActionResult animationDocumentary4()
        {
            return View();
        }
        /// <summary>
        /// 自动跟单会员需要注意什么
        /// </summary>
        public ActionResult animationDocumentary5()
        {
            return View();
        }

        #endregion

        #region 撤单
        /// <summary>
        /// 撤单
        /// </summary>
        public ActionResult Revoke()
        {
            return View();
        }
        /// <summary>
        /// 什么是方案撤单
        /// </summary>
        public ActionResult Revoke1()
        {
            return View();
        }
        /// <summary>
        /// 已购买的方案如何申请撤单
        /// </summary>
        public ActionResult Revoke2()
        {
            return View();
        }
        /// <summary>
        /// 什么是方案流产
        /// </summary>
        public ActionResult Revoke3()
        {
            return View();
        }
        /// <summary>
        /// 方案流产后怎么办
        /// </summary>
        public ActionResult Revoke4()
        {
            return View();
        }
        /// <summary>
        /// 已经满员的方案为何会被撤单
        /// </summary>
        public ActionResult Revoke5()
        {
            return View();
        }

        #endregion

        #endregion

        #region 兑奖提款

        #region  奖金领取
        /// <summary>
        /// 奖金领取
        /// </summary>
        public ActionResult bonusGet()
        {
            return View();
        }
        /// <summary>
        /// 中奖后怎么兑奖？
        /// </summary>
        public ActionResult bonusGet1()
        {
            return View();
        }
        /// <summary>
        /// 派奖时间是多久？
        /// </summary>
        public ActionResult bonusGet2()
        {
            return View();
        }
        /// <summary>
        /// 怎样查看我的方案有没有中奖？
        /// </summary>
        public ActionResult bonusGet3()
        {
            return View();
        }
        /// <summary>
        /// 中奖奖金如何扣税？
        /// </summary>
        public ActionResult bonusGet4()
        {
            return View();
        }
        /// <summary>
        /// 如何派奖？
        /// </summary>
        public ActionResult bonusGet5()
        {
            return View();
        }
        /// <summary>
        /// 怎么保护中奖者的个人信息？
        /// </summary>
        public ActionResult bonusGet6()
        {
            return View();
        }

        #endregion

        #region  提款
        /// <summary>
        /// 提款
        /// </summary>
        public ActionResult drawing()
        {
            return View();
        }
        /// <summary>
        /// 提款须知
        /// </summary>
        public ActionResult drawing1()
        {
            return View();
        }
        /// <summary>
        /// 如何绑定银行卡？
        /// </summary>
        public ActionResult drawing2()
        {
            return View();
        }
        /// <summary>
        /// 为什么我的提款不成功？
        /// </summary>
        public ActionResult drawing3()
        {
            return View();
        }
        /// <summary>
        /// 提款后为什么账户中有冻结的资金？
        /// </summary>
        public ActionResult drawing4()
        {
            return View();
        }
        /// <summary>
        /// 申请提款，网站是否有通知？
        /// </summary>
        public ActionResult drawing5()
        {
            return View();
        }
        /// <summary>
        /// 如何将钱从 帐号中提取出来（如何提款）？
        /// </summary>
        public ActionResult drawing6()
        {
            return View();
        }

        #endregion

        #region 提款手续费
        /// <summary>
        /// 提款手续费
        /// </summary>
        public ActionResult poundage()
        {
            return View();
        }
        /// <summary>
        /// 提款手续费
        /// </summary>
        public ActionResult poundage1()
        {
            return View();
        }

        #endregion

        #region 提款到账时间
        /// <summary>
        /// 提款到账时间
        /// </summary>
        public ActionResult poundageTime()
        {
            return View();
        }
        /// <summary>
        /// 提款处理及到账时间
        /// </summary>
        public ActionResult poundageTime1()
        {
            return View();
        }

        #endregion

        #endregion

        #region 交易规则

        #region 合买奖金分配
        /// <summary>
        /// 合买奖金分配
        /// </summary>
        public ActionResult bonusAllot()
        {
            return View();
        }
        /// <summary>
        /// 合买提成计算规则
        /// </summary>
        public ActionResult bonusAllot1()
        {
            return View();
        }
        /// <summary>
        /// 合买方案怎么分奖金？
        /// </summary>
        public ActionResult bonusAllot2()
        {
            return View();
        }

        #endregion

        #region  自动跟单
        /// <summary>
        /// 自动跟单
        /// </summary>
        public ActionResult customizeDocumentary()
        {
            return View();
        }
        /// <summary>
        /// 什么是定制跟单
        /// </summary>
        public ActionResult customizeDocumentary1()
        {
            return View();
        }
        /// <summary>
        /// 哪些彩种可以定制跟单，最低定制跟单金额是多少
        /// </summary>
        public ActionResult customizeDocumentary2()
        {
            return View();
        }
        /// <summary>
        /// 定制跟单有哪些规则？
        /// </summary>
        public ActionResult customizeDocumentary3()
        {
            return View();
        }

        #endregion

        #region 网站保底
        /// <summary>
        /// 网站保底
        /// </summary>
        public ActionResult website()
        {
            return View();
        }
        /// <summary>
        /// 什么是发起人保底?
        /// </summary>
        public ActionResult website1()
        {
            return View();
        }
        /// <summary>
        /// 什么是网站保底?
        /// </summary>
        public ActionResult website2()
        {
            return View();
        }

        #endregion

        #region 战绩规则
        /// <summary>
        /// 战绩规则
        /// </summary>
        public ActionResult recordRule()
        {
            return View();
        }
        /// <summary>
        /// 竞技类彩票战绩规则
        /// </summary>
        public ActionResult recordRule1()
        {
            return View();
        }
        /// <summary>
        /// 数字类彩票战绩规则
        /// </summary>
        public ActionResult recordRule2()
        {
            return View();
        }


        #endregion

        #endregion

        #region 各种玩法

        #region 足彩胜负彩
        /// <summary>
        /// 足彩胜负彩
        /// </summary>
        public ActionResult zcsfc()
        {
            return View();
        }
        /// <summary>
        /// 足彩胜负彩玩法规则
        /// </summary>
        public ActionResult zcsfc1()
        {
            return View();
        }

        #endregion

        #region 足彩四场进球
        /// <summary>
        /// 足彩四场进球
        /// </summary>
        public ActionResult zcscjq()
        {
            return View();
        }
        /// <summary>
        /// 足彩四场进球玩法规则
        /// </summary>
        public ActionResult zcscjq1()
        {
            return View();
        }

        #endregion

        #region 足彩六场半全
        /// <summary>
        /// 足彩六场半全
        /// </summary>
        public ActionResult zclcbq()
        {
            return View();
        }
        /// <summary>
        /// 足彩六场半全玩法规则
        /// </summary>
        public ActionResult zclcbq1()
        {
            return View();
        }

        #endregion

        #region 北京单场
        /// <summary>
        /// 北京单场
        /// </summary>
        public ActionResult bjdc()
        {
            return View();
        }
        /// <summary>
        /// 北京单场玩法规则
        /// </summary>
        public ActionResult bjdc1()
        {
            return View();
        }

        #endregion

        #region 竞彩足球
        /// <summary>
        /// 竞彩足球
        /// </summary>
        public ActionResult jczq()
        {
            return View();
        }
        /// <summary>
        /// 竞彩足球玩法规则
        /// </summary>
        public ActionResult jczq1()
        {
            return View();
        }

        #endregion

        #region 竞彩篮球
        /// <summary>
        /// 竞彩篮球
        /// </summary>
        public ActionResult jclq()
        {
            return View();
        }
        /// <summary>
        /// 竞彩篮球玩法规则
        /// </summary>
        public ActionResult jclq1()
        {
            return View();
        }

        #endregion

        #region 双色球
        /// <summary>
        /// 双色球
        /// </summary>
        public ActionResult ssq()
        {
            return View();
        }
        /// <summary>
        /// 双色球玩法规则
        /// </summary>
        public ActionResult ssq1()
        {
            return View();
        }

        #endregion

        #region 大乐透
        /// <summary>
        /// 大乐透
        /// </summary>
        public ActionResult dlt()
        {
            return View();
        }
        /// <summary>
        /// 大乐透玩法规则
        /// </summary>
        public ActionResult dlt1()
        {
            return View();
        }

        #endregion

        #region 福彩3D
        /// <summary>
        /// 福彩3D
        /// </summary>
        public ActionResult fc3d()
        {
            return View();
        }
        /// <summary>
        /// 福彩3D玩法规则
        /// </summary>
        public ActionResult fc3d1()
        {
            return View();
        }

        #endregion

        #region 排列3
        /// <summary>
        /// 排列3
        /// </summary>
        public ActionResult pl3()
        {
            return View();
        }
        /// <summary>
        /// 排列3玩法规则
        /// </summary>
        public ActionResult pl31()
        {
            return View();
        }

        #endregion

        #region 江西11选5
        /// <summary>
        /// 11、江西11选5
        /// </summary>
        public ActionResult jx11x5()
        {
            return View();
        }
        /// <summary>
        /// 11、江西11选5玩法规则
        /// </summary>
        public ActionResult jx11x51()
        {
            return View();
        }

        #endregion

        #region 重庆时时彩
        /// <summary>
        /// 重庆时时彩
        /// </summary>
        public ActionResult cqssc()
        {
            return View();
        }
        /// <summary>
        /// 重庆时时彩玩法规则
        /// </summary>
        public ActionResult cqssc1()
        {
            return View();
        }

        #endregion

        #region 山东11选5

        public ActionResult sd11x5()
        {
            return View();
        }

        public ActionResult sd11x51()
        {
            return View();
        }

        #endregion

        #region 广东11选5

        public ActionResult gd11x5()
        {
            return View();
        }

        public ActionResult gd11x51()
        {
            return View();
        }

        #endregion

        #region 广东快乐十分

        public ActionResult gdklsf()
        {
            return View();
        }

        public ActionResult gdklsf1()
        {
            return View();
        }

        #endregion

        #region 江苏快三

        public ActionResult jsks()
        {
            return View();
        }

        public ActionResult jsks1()
        {
            return View();
        }

        #endregion

        #region 山东快乐扑克3

        public ActionResult sdklpk3()
        {
            return View();
        }

        public ActionResult sdklpk31()
        {
            return View();
        }

        #endregion

        #endregion

        #region 手机购彩

        #region 彩票客户端购彩
        /// <summary>
        /// 彩票客户端购彩
        /// </summary>
        public ActionResult phoneForTheColor()
        {
            return View();
        }
        /// <summary>
        /// 彩票手机客户端的下载安装
        /// </summary>
        public ActionResult phoneForTheColor1()
        {
            return View();
        }
        /// <summary>
        /// 彩票手机客户端上的手机银联充值有哪些安全支付方式，怎么操作充值的？
        /// </summary>
        public ActionResult phoneForTheColor2()
        {
            return View();
        }
        /// <summary>
        /// 彩票手机客户端上的手机银联充值有哪些注意事项，支持哪些银行的银行卡充值？
        /// </summary>
        public ActionResult phoneForTheColor3()
        {
            return View();
        }
        /// <summary>
        /// 如何在 彩票手机客户端上购彩？
        /// </summary>
        public ActionResult phoneForTheColor4()
        {
            return View();
        }

        #endregion

        #endregion

        #region 安全保障

        #region 购彩安全
        /// <summary>
        /// 购彩安全
        /// </summary>
        public ActionResult forTheColorSafety()
        {
            return View();
        }
        /// <summary>
        /// 购彩安全
        /// </summary>
        public ActionResult forTheColorSafety1()
        {
            return View();
        }

        #endregion

        #region  业务资质
        /// <summary>
        /// 业务资质
        /// </summary>
        public ActionResult business()
        {
            return View();
        }
        /// <summary>
        /// 业务资质
        /// </summary>
        public ActionResult business1()
        {
            return View();
        }

        #endregion

        #region  购彩优势
        /// <summary>
        /// 购彩优势
        /// </summary>
        public ActionResult advantage()
        {
            return View();
        }
        /// <summary>
        /// 购彩优势
        /// </summary>
        public ActionResult advantage1()
        {
            return View();
        }

        #endregion

        #region 竞彩投注须知

        public ActionResult betNotice()
        {
            return View();
        }

        public ActionResult betNotice1()
        {
            return View();
        }

        #endregion

        #region  用户服务协议
        /// <summary>
        /// 用户服务协议
        /// </summary>
        public ActionResult serviceAgreement()
        {
            return View();
        }
        /// <summary>
        /// 用户服务协议
        /// </summary>
        public ActionResult serviceAgreement1()
        {
            return View();
        }

        #endregion

        #endregion

        #region 关于玩彩网

        #region 公司简介
        /// <summary>
        /// 公司简介
        /// </summary>
        public ActionResult Aboutsite()
        {
            return View();
        }
        /// <summary>
        /// 公司简介
        /// </summary>
        public ActionResult Aboutsite1()
        {
            return View();
        }

        #endregion

        #region  加盟合作
        /// <summary>
        /// 加盟合作
        /// </summary>
        public ActionResult Join()
        {
            return View();
        }
        /// <summary>
        /// 合作方式
        /// </summary>
        public ActionResult Join1()
        {
            return View();
        }
        /// <summary>
        /// 结算流程
        /// </summary>
        public ActionResult Join2()
        {
            return View();
        }

        #endregion

        #region  网站地图

        /// <summary>
        /// 网站地图
        /// </summary>
        public ActionResult websiteMap()
        {
            return View();
        }
        /// <summary>
        /// 网站地图
        /// </summary>
        public ActionResult websiteMap1()
        {
            return View();
        }

        #endregion

        #region  友情链接
        /// <summary>
        /// 友情链接
        /// </summary>
        public ActionResult links()
        {
            return View();
        }
        /// <summary>
        /// 友情链接
        /// </summary>
        public ActionResult links1()
        {
            return View();
        }

        #endregion

        #endregion

        /// <summary>
        /// 合买奖金如何分配
        /// </summary>
        /// <returns></returns>
        public ActionResult chippedBonus()
        {
            return View();
        }
        /// <summary>
        /// 世界杯
        /// </summary>
        /// <returns></returns>
        public ActionResult worldcup()
        {
            return View();
        }
        /// <summary>
        /// 2串1
        /// </summary>
        public ActionResult ecy()
        {
            return View();
        }
        /// <summary>
        /// 中奖后停止跟单
        /// </summary>
        public ActionResult zjtzzh()
        {
            return View();

        }
        /// <summary>
        /// 中奖后什么时候派奖
        /// </summary>
        public ActionResult zjsmshpj()
        {

            return View();
        }
        /// <summary>
        /// 单注奖金为什么只有两元
        /// </summary>
        public ActionResult azjjwsmzyly()
        {
            return View();
        }
        /// <summary>
        /// 奖金怎么计算
        /// </summary>
        public ActionResult jjzmjs()
        {
            return View();
        }
        /// <summary>
        /// 提款须知
        /// </summary>
        /// <returns></returns>
        public ActionResult tkxz()
        {
            return View();
        }
        public ActionResult tr9()
        {
            return View();
        }
        public ActionResult tr91()
        {
            return View();
        }
        public ActionResult Gswj()
        {
            return View();
        }
        #region 玩法规则

        public ActionResult W_SSQ()
        {
            return View();
        }

        public ActionResult W_DLT()
        {
            return View();
        }

        public ActionResult W_CQSSC()
        {
            return View();
        }

        public ActionResult W_FC3D()
        {
            return View();
        }

        public ActionResult W_GD11X5()
        {
            return View();
        }

        public ActionResult W_GDKLSF()
        {
            return View();
        }

        public ActionResult W_GXKLSF()
        {
            return View();
        }

        public ActionResult W_JSKS()
        {
            return View();
        }

        public ActionResult W_JX11X5()
        {
            return View();
        }

        public ActionResult W_JXSSC()
        {
            return View();
        }

        public ActionResult W_PL3()
        {
            return View();
        }

        public ActionResult W_SD11X5()
        {
            return View();
        }

        public ActionResult W_SDQYH()
        {
            return View();
        }

        public ActionResult w_jczq()
        {
            return View();
        }

        public ActionResult w_jclq()
        {
            return View();
        }

        public ActionResult w_ctzq()
        {
            return View();
        }

        public ActionResult w_t14c()
        {
            return View();
        }

        public ActionResult w_tr9()
        {
            return View();
        }

        public ActionResult w_t6bqc()
        {
            return View();
        }

        public ActionResult w_t4cjq()
        {
            return View();
        }

        public ActionResult w_bjdc()
        {
            return View();
        }
        #endregion

    }
}
