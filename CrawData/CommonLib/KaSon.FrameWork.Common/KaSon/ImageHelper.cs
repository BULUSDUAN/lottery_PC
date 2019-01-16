namespace KaSon.FrameWork.Common
{
    using System;

    using System.DrawingCore;
    using System.DrawingCore.Drawing2D;
    using System.DrawingCore.Imaging;
    using System.IO;

    public static class ImageHelper
    {
        public static Bitmap CutImage(Bitmap img, int StartX, int StartY, int newW, int newH)
        {
            if (img == null)
            {
                return null;
            }
            int width = img.Width;
            int height = img.Height;
            if ((StartX >= width) || (StartY >= height))
            {
                return null;
            }
            if ((StartX + newW) > width)
            {
                newW = width - StartX;
            }
            if ((StartY + newH) > height)
            {
                newH = height - StartY;
            }
            try
            {
                Bitmap image = new Bitmap(newW, newH, PixelFormat.Format24bppRgb);
                Graphics graphics = Graphics.FromImage(image);
                graphics.DrawImage(img, new Rectangle(0, 0, newW, newH), new Rectangle(StartX, StartY, newW, newH), GraphicsUnit.Pixel);
                graphics.Dispose();
                return image;
            }
            catch
            {
                return null;
            }
        }

        public static Bitmap CutImage(string img, int StartX, int StartY, int newW, int newH)
        {
            Bitmap bitmap = new Bitmap(img);
            return CutImage(bitmap, StartX, StartY, newW, newH);
        }

        public static Bitmap ResizeImage(Bitmap img, int newW, int newH)
        {
            try
            {
                Bitmap image = new Bitmap(newW, newH);
                Graphics graphics = Graphics.FromImage(image);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(img, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                graphics.Dispose();
                return image;
            }
            catch
            {
                return null;
            }
        }

        public static Bitmap ResizeImage(string img, int newW, int newH)
        {
            Bitmap bitmap = new Bitmap(img);
            return ResizeImage(bitmap, newW, newH);
        }


        public static string ImageToBase64(Image image, System.DrawingCore.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray(); // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        public static string  ImgToBase64String(string Imagefilename)
        {
            String strbaser64 = "";
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);
              //  this.pictureBox1.Image = bmp;
               // FileStream fs = new FileStream(Imagefilename + ".txt", FileMode.Create);
             //   StreamWriter sw = new StreamWriter(fs);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.DrawingCore.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                 strbaser64 = Convert.ToBase64String(arr);
              //  sw.Write(strbaser64);

              //  sw.Close();
               // fs.Close();
                // MessageBox.Show("转换成功!");
            }
            catch (Exception ex)
            {
              //  MessageBox.Show("ImgToBase64String 转换失败\nException:" + ex.Message);
            }

            return strbaser64;
        }

    }
}

