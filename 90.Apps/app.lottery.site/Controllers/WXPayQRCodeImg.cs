using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace app.lottery.site.Controllers
{
    /// <summary>
    /// 微信二维码图片
    /// </summary>
    public class WXPayQRCodeImg : ActionResult
    {
        /// <summary>
        /// 微信URL地址
        /// </summary>
        public string QRCodeUrl { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            OnPaint(context);
        }

        private void OnPaint(ControllerContext context)
        {
            try
            {
                //初始化二维码生成工具
                QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                qrCodeEncoder.QRCodeVersion = 0;
                qrCodeEncoder.QRCodeScale = 8;//大小

                //将字符串生成二维码图片
                Bitmap image = qrCodeEncoder.Encode(this.QRCodeUrl, Encoding.Default);

                //保存为PNG到内存流  
                MemoryStream ms = new MemoryStream();
                image.Save(ms, ImageFormat.Png);

                //输出二维码图片
                context.HttpContext.Response.BinaryWrite(ms.GetBuffer());
                context.HttpContext.Response.End();
            }
            catch (Exception)
            {
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.Write("Err!");
                context.HttpContext.Response.End();
            }
        }
    }
}