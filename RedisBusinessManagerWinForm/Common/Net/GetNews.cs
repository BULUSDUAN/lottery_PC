using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Xml;

namespace Common.Net
{
    [ServiceContract]
    interface IGetNewsService
    {
        /// <summary>
        /// 获取文章列表
        /// </summary>
        [OperationContract(Action = "http://tempuri.org/GetNewsListFromClass")]
        string GetNewsListFromClass(string key, string className, int type, int pageIndex, int pageSize);
        /// <summary>
        /// 获取文章列表
        /// </summary>
        [OperationContract(Action = "http://tempuri.org/GetNewsListWithContentFromClass")]
        string GetNewsListWithContentFromClass(string key, string className, int type, int pageIndex, int pageSize);
    }
   
    class GetNewsFactory
    {
        /// <summary>
        /// 文章列表接口地址
        /// </summary>
        private static string ServiceURL = "http://192.168.1.107:19700/services/GetNews.asmx";

        /// <summary>
        /// 初始化文章获取列表
        /// </summary>
        /// <param name="serviceUrl">文章获取列表地址</param>
        public GetNewsFactory(string serviceUrl)
        {
            ServiceURL = serviceUrl;
        }

        public IGetNewsService WebService()
        {
            return GetServiceChannel();
        }

        private static IGetNewsService GetServiceChannel()
        {
            System.ServiceModel.Channels.Binding bind = GetBinding();
            EndpointAddress address = new EndpointAddress(ServiceURL);
            ChannelFactory<IGetNewsService> factory = new ChannelFactory<IGetNewsService>(bind, address);
            IGetNewsService channel = factory.CreateChannel();
            return channel;
        }

        private static System.ServiceModel.Channels.Binding GetBinding()
        {
            BasicHttpBinding bind = new BasicHttpBinding(BasicHttpSecurityMode.None);
            bind.TransferMode = TransferMode.StreamedResponse;
            bind.MaxReceivedMessageSize = long.MaxValue;
            bind.MaxBufferSize = int.MaxValue;
            bind.ReaderQuotas.MaxArrayLength = int.MaxValue;
            bind.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            bind.ReceiveTimeout = new TimeSpan(1, 0, 0);
            bind.SendTimeout = new TimeSpan(1, 0, 0);
            return bind;
        }
    }  

    /// <summary>
    /// 文章列表获取
    /// </summary>
    public static class ArticleGetor
    {
        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="serviceUrl">接口地址</param>
        /// <param name="key">MD5加密串，showmethemoney万能。普通格式：DateTime.Now.ToString("yyyy/mm/dd")</param>
        /// <param name="className">文章所属栏目 
        /// ssq:双色球 dlt:大乐透 klsf:快乐十分 sdqyh:山东群英会 ssc:时时彩 x11x5:11选5 jc:竞彩 x22x5:22选5 fc3d:福彩3D pl3pl5:排列3/排列5 qlc:七乐彩 qxc:七星彩 
        /// cqssc:重庆时时彩 jxssc:江西时时彩 wzhd:网站活动 gdklsf:广东快乐十分 gxklsf:广西快乐十分 sd11x5:十一运夺金 gd11x5:广东11选5 jx11x5:江西11选5 pl3:排列3 pl5:排列5 zqbb:足球宝贝 wfjq:玩法技巧 zjyc:专家预测
        /// gycp:公益彩票 wzgg:网站公告
        /// </param>
        /// <param name="type">文章类型 1.推荐-专家预测 2.滚动 3.热点 4.头条 5.公告 6.WAP 7.精彩-玩法技巧</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        public static string GetArticlesList(string serviceUrl, string key, string className, int type, int pageIndex, int pageSize)
        {
            GetNewsFactory ft = new GetNewsFactory(serviceUrl);
            key = string.IsNullOrEmpty(key) ? "showmethemoney" : key;
            className = string.IsNullOrEmpty(className) ? "all" : className;
            return ft.WebService().GetNewsListFromClass(key, className, type, pageIndex, pageSize);
        }




        class GetNewsFactoryWithContent
        {
            /// <summary>
            /// 投注技巧列表接口地址
            /// </summary>
            private static string ServiceWithContentURL = "http://192.168.1.107:19700/services/GetNews.asmx";

            /// <summary>
            /// 初始化投注技巧获取列表
            /// </summary>
            /// <param name="serviceUrl">文章获取列表地址</param>
            public GetNewsFactoryWithContent(string serviceUrl)
            {
                ServiceWithContentURL = serviceUrl;
            }

            public IGetNewsService WebService()
            {
                return GetServiceChannelWithContent();
            }

            private static IGetNewsService GetServiceChannelWithContent()
            {
                System.ServiceModel.Channels.Binding bind = GetBindingWithContent();
                EndpointAddress address = new EndpointAddress(ServiceWithContentURL);
                ChannelFactory<IGetNewsService> factory = new ChannelFactory<IGetNewsService>(bind, address);
                IGetNewsService channel = factory.CreateChannel();
                return channel;
            }

            private static System.ServiceModel.Channels.Binding GetBindingWithContent()
            {
                BasicHttpBinding bind = new BasicHttpBinding(BasicHttpSecurityMode.None);
                bind.TransferMode = TransferMode.StreamedResponse;
                bind.MaxReceivedMessageSize = long.MaxValue;
                bind.MaxBufferSize = int.MaxValue;
                bind.ReaderQuotas.MaxArrayLength = int.MaxValue;
                bind.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                bind.ReceiveTimeout = new TimeSpan(1, 0, 0);
                bind.SendTimeout = new TimeSpan(1, 0, 0);
                return bind;
            }
        }
        /// <summary>
        /// 获取技巧列表
        /// </summary>
        /// <param name="serviceUrl"></param>
        /// <param name="key"></param>
        /// <param name="className"></param>
        /// <param name="type"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static string GetArticlesListWithContent(string serviceUrl, string key, string className, int type, int pageIndex, int pageSize)
        {
            GetNewsFactoryWithContent ft = new GetNewsFactoryWithContent(serviceUrl);
            key = string.IsNullOrEmpty(key) ? "showmethemoney" : key;
            className = string.IsNullOrEmpty(className) ? "all" : className;
            return ft.WebService().GetNewsListWithContentFromClass(key, className, type, pageIndex, pageSize);
        }
    }
}
