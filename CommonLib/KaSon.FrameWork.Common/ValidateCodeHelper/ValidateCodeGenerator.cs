/*---------------------------
     验证码生成器        
     2010-11-01          
---------------------------*/
using System;
using System.Text;
using System.IO;
using System.DrawingCore;
using System.DrawingCore.Imaging;

namespace KaSon.FrameWork.Common
{
    public class ValidateCodeGenerator
    {
        /// <summary>
        /// 背景颜色
        /// </summary>
        public Color BackGroundColor { get; set; }
        /// <summary>
        /// 随机字符
        /// </summary>
        public string RandomWord { get; set; }
        /// <summary>
        /// 图片宽度
        /// </summary>
        public int ImageWidth { get; set; }
        /// <summary>
        /// 图片高度
        /// </summary>
        public int ImageHeight { get; set; }
        /// <summary>
        /// 字体大小
        /// </summary>
        public int fontSize { get; set; }

        //public override void ExecuteResult(ControllerContext context)
        //{
        //    OnPaint(context);
        //}

            public 


        static string[] FontItems = new string[] { "tahoma", "Verdana", "Consolas", "Times New Roman" };
        static Brush[] BrushItems = new Brush[] { Brushes.OliveDrab, Brushes.ForestGreen, Brushes.DarkCyan, Brushes.LightSlateGray, Brushes.RoyalBlue, Brushes.SlateBlue, Brushes.DarkViolet, Brushes.MediumVioletRed, Brushes.IndianRed, Brushes.Firebrick, Brushes.Chocolate, Brushes.Peru };
        static Color[] ColorItems = new Color[] { Color.Green, Color.Blue, Color.Gray, Color.Red, Color.Black, Color.Orange, Color.OrangeRed, Color.Silver };
        private int _brushNameIndex;

        Random _random = new Random(DateTime.Now.GetHashCode());

        /// <summary>
        /// 取一个随机字体
        /// </summary>
        /// <returns></returns>
        private Font GetFont()
        {
            int fontIndex = _random.Next(0, FontItems.Length);
            return new Font(FontItems[fontIndex], fontSize, GetFontStyle());
        }

        /// <summary>
        /// 取一个随机字体样式
        /// </summary>
        /// <returns></returns>
        private FontStyle GetFontStyle()
        {
            switch (DateTime.Now.Second % 2)
            {
                case 0:
                    return FontStyle.Regular | FontStyle.Bold;
                case 1:
                    return FontStyle.Italic | FontStyle.Bold;
                default:
                    return FontStyle.Regular | FontStyle.Bold | FontStyle.Strikeout;
            }
        }

        /// <summary>
        /// 取一个随机笔刷
        /// </summary>
        /// <returns></returns>
        private Brush GetBrush()
        {
            _brushNameIndex = _random.Next(0, BrushItems.Length);
            return BrushItems[_brushNameIndex];
        }

        /// <summary>
        /// 获取随机颜色
        /// </summary>
        /// <returns></returns>
        private Color GetColor()
        {
            int colorIndex = _random.Next(0, ColorItems.Length);
            return ColorItems[colorIndex];
        }

        /// <summary>
        /// 绘画背景色
        /// </summary>
        /// <param name="g"></param>
        private void Paint_Background(Graphics g)
        {
            g.Clear(BackGroundColor);
        }

        /// <summary>
        /// 绘画边框
        /// </summary>
        /// <param name="g"></param>
        private void Paint_Border(Graphics g)
        {
            g.DrawRectangle(Pens.DarkGray, 0, 0, ImageWidth - 1, ImageHeight - 1);
        }

        /// <summary>
        /// 绘画文字
        /// </summary>
        /// <param name="g"></param>
        private void Paint_Text(Graphics g, string text)
        {
            int x = 1, y = 1;
            Brush brush = GetBrush();
            for (int i = 0; i < text.Length; i++)
            {
                x = ImageWidth / text.Length * i - 2;
                y = _random.Next(0, 5);
                g.DrawString(text.Substring(i, 1), GetFont(), brush, x, y);
            }

        }

        /// <summary>
        /// 绘画噪音点
        /// </summary>
        /// <param name="b"></param>
        private void Paint_Stain(Bitmap b)
        {
            for (int n = 0; n < (ImageWidth * ImageHeight / 40); n++)
            {
                int x = _random.Next(0, ImageWidth);
                int y = _random.Next(0, ImageHeight);
                b.SetPixel(x, y, GetColor());
            }
        }

        /// <summary>
        /// 画笔事件
        /// </summary>
        /// <param name="context"></param>
        public byte[] OnPaint()
        {
            Bitmap oBitmap = null;
            Graphics g = null;

            try
            {
                using (oBitmap = new Bitmap(ImageWidth, ImageHeight))
                {
                    g = Graphics.FromImage(oBitmap);

                    Paint_Background(g);
                    Paint_Text(g, RandomWord);
                    Paint_Stain(oBitmap);
                    //Paint_Border(g);
                    var ms = new MemoryStream();
                    oBitmap.Save(ms, ImageFormat.Gif);
                    return ms.ToArray();
                    //context.HttpContext.Response.ContentType = "image/gif";
                    //oBitmap.Save(context.HttpContext.Response.OutputStream, ImageFormat.Gif);
                    //g.Dispose();
                    //oBitmap.Dispose();
                   
                }
                
            }
            catch
            {
                //context.HttpContext.Response.Clear();
                //context.HttpContext.Response.Write("Err!");
                //context.HttpContext.Response.End();
                return null;
            }
            //finally
            //{
            //    if (null != oBitmap)
            //        oBitmap.Dispose();
            //    if (null != g)
            //        g.Dispose();
            //}
        }

    }
}