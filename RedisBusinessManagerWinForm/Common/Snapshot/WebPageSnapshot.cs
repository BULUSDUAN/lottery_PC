using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace Common.Snapshot
{
    /// <summary>
    /// web 页面快照类
    /// </summary>
    public class WebPageSnapshot : IDisposable
    {
        string url = "about:blank";

        /// <summary>
        /// 简单构造一个 WebBrowser 对象
        /// 更灵活的应该是直接引用浏览器的com对象实现稳定控制
        /// </summary>
        WebBrowser wb = null;
        /// <summary>
        /// URL 地址
        /// http://www.cnblogs.com
        /// </summary>
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        int width = 1024;
        /// <summary>
        /// 图象宽度
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        int height = 768;
        /// <summary>
        /// 图象高度
        /// </summary>
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        Bitmap m_Bitmap;

        /// <summary>
        /// 初始化
        /// </summary>
        protected void InitComobject()
        {
            wb=new WebBrowser();
            try
            {
                wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wb_DocumentCompleted);
                wb.ScriptErrorsSuppressed = false;
                wb.ScrollBarsEnabled = false;
                //wb.Size = new Size(1024, 768);
                wb.Navigate(this.url);
                //因为没有窗体，所以必须如此
                while (wb.ReadyState != WebBrowserReadyState.Complete)
                    System.Windows.Forms.Application.DoEvents();
                wb.Stop();
                if (wb.ActiveXInstance == null)
                    throw new Exception("实例不能为空");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wb.DocumentTitle.Contains("分析器错误")||wb.DocumentTitle.IndexOf("错误")>=0)
            {
                return;
            }
            if (e.Url.ToString() == this.url)
            {
                this.Width = wb.Document.Body.ScrollRectangle.Width;
                this.Height = wb.Document.Body.ScrollRectangle.Height;
            }
            wb.ClientSize = new Size(this.Width, this.Height);
            wb.ScrollBarsEnabled = false;
            m_Bitmap = new Bitmap(wb.Bounds.Width, wb.Bounds.Height);
            wb.BringToFront();
            wb.DrawToBitmap(m_Bitmap, wb.Bounds);
            m_Bitmap = (Bitmap)m_Bitmap.GetThumbnailImage(wb.Width, wb.Height, null, IntPtr.Zero);
        }

        /// <summary>
        /// 获取快照
        /// </summary>
        /// <returns>Bitmap</returns>
        public Bitmap TakeSnapshot()
        {
            try
            {
                //InitComobject();
                Thread th = new Thread(new ThreadStart(InitComobject));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                th.Join();
                return m_Bitmap;
                //构造snapshot类，抓取浏览器ActiveX的图象
                //var snap = new Snapshot();
                //return snap.TakeSnapshot(wb.ActiveXInstance, new Rectangle(0, 0, this.Width, this.Height));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public void Dispose()
        {
            wb.Dispose();
        }

    }
}
