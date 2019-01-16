namespace KaSon.FrameWork.Common
{
    using System;

    using System.DrawingCore;
    using System.DrawingCore.Drawing2D;
    using System.DrawingCore.Imaging;
    using System.IO;

    public static class RandomHelper
    {
        public static byte[] GetRandomGraphic(string str)
        {
            byte[] buffer;
            Bitmap image = new Bitmap((int) Math.Ceiling((double) (str.Length * 18.0)), 0x1c);
            Graphics graphics = Graphics.FromImage(image);
            try
            {
                int num;
                Color[] colorArray = new Color[] { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.Brown, Color.DarkBlue };
                string[] strArray = new string[] { "Times New Roman", "MS Mincho", "Book Antiqua", "Gungsuh", "PMingLiU", "Impact" };
                Random random = new Random();
                graphics.Clear(Color.White);
                for (num = 0; num < 3; num++)
                {
                    int num2 = random.Next(image.Width);
                    int num3 = random.Next(image.Height);
                    int num4 = random.Next(image.Width);
                    int num5 = random.Next(image.Height);
                    Color color = colorArray[random.Next(colorArray.Length)];
                    graphics.DrawLine(new Pen(color), num2, num3, num4, num5);
                }
                for (num = 0; num < str.Length; num++)
                {
                    string familyName = strArray[random.Next(strArray.Length)];
                    Font font = new Font(familyName, 15f);
                    LinearGradientBrush brush = new LinearGradientBrush(new Point(num * 0x12, 1), new Point((num * 0x12) + 0x12, 1), Color.FromArgb(random.Next(0xff), random.Next(0xff), random.Next(0xff)), Color.FromArgb(random.Next(0xff), random.Next(0xff), random.Next(0xff)));
                    float x = (num * 15) - random.Next(-2, 2);
                    float y = random.Next(1, 8);
                    graphics.DrawString(str[num].ToString(), font, brush, x, y);
                }
                for (num = 0; num < 50; num++)
                {
                    int num8 = random.Next(image.Width);
                    int num9 = random.Next(image.Height);
                    image.SetPixel(num8, num9, Color.FromArgb(random.Next()));
                }
                using (MemoryStream stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Jpeg);
                    buffer = stream.ToArray();
                }
            }
            finally
            {
                graphics.Dispose();
                image.Dispose();
            }
            return buffer;
        }

        public static string GetRandomNumber(int length)
        {
            int num3;
            if (length < 1)
            {
                throw new ApplicationException("随机数长度必须大于0");
            }
            int[] numArray = new int[length];
            int[] numArray2 = new int[length];
            string str = "";
            int ticks = (int) DateTime.Now.Ticks;
            int num2 = new Random(ticks).Next(0, 0x7fffffff - (length * 0x2710));
            int[] numArray3 = new int[length];
            for (num3 = 0; num3 < length; num3++)
            {
                numArray3[num3] = num2 + 0x2710;
            }
            for (num3 = 0; num3 < length; num3++)
            {
                Random random2 = new Random(numArray3[num3]);
                int minValue = (int) Math.Pow(10.0, (double) length);
                numArray[num3] = random2.Next(minValue, 0x7fffffff);
            }
            for (num3 = 0; num3 < length; num3++)
            {
                string str2 = numArray[num3].ToString();
                int num5 = str2.Length;
                int startIndex = new Random().Next(0, num5 - 1);
                numArray2[num3] = int.Parse(str2.Substring(startIndex, 1));
            }
            for (num3 = 0; num3 < length; num3++)
            {
                str = str + numArray2[num3].ToString();
            }
            return str;
        }

        public static string GetRandomString(int length)
        {
            if (length < 1)
            {
                throw new ApplicationException("随机数长度必须大于0");
            }
            string str = string.Empty;
            char[] chArray = new char[] { 
                '0','1', '2', '3', '4', '5', '6', '8', '9', 
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 
                'K', 'L', 'M', 'N','O', 'P', 'R', 'S', 'T','U','V', 'W', 'X', 'Y','Z'
             };
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                str = str + chArray[random.Next(chArray.Length)];
            }
            return str;
        }
    }
}

