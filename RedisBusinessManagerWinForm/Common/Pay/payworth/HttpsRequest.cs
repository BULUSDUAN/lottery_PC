using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;

namespace Common.Pay.payworth
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpsRequest
    {
        public static String HtmlPost(String Url, Dictionary<String, String> Params)
        {
            String FormString = "<body onLoad=\"document.actform.submit()\">正在处理请稍候.....................<form  id=\"actform\" name=\"actform\" method=\"post\" action=\"" + Url + "\">";

            foreach (string key in Params.Keys)
            {
                FormString += "<input name=\"" + key + "\" type=\"hidden\" value='" + Params[key] + "'>";

            }
            FormString += "</form></body>";
            return FormString;
        }

        ///<summary>
        ///采用https协议访问网络
        ///</summary>
        ///<param name="URL">url地址</param>
        ///<param name="strPostdata">发送的数据</param>
        ///<returns></returns>
        public static string OpenReadWithHttps(string URL, string strPostdata)
        {
            try
            {
                //Log.LogWrite("请求参数：" + URL + "?" + strPostdata);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "post";
                request.Accept = "text/html, application/xhtml+xml, */*";
                request.ContentType = "application/x-www-form-urlencoded";
                byte[] buffer = Encoding.UTF8.GetBytes(strPostdata);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(reader.ReadToEnd()));
                }
                response.Close();
                request = null;
            }
            catch (Exception ex)
            {
                return "[HttpsRequest.OpenReadWithHttps请求异常][" + ex.Message + "]";
            }
        }
        public static String URLencode(String DateStr)
        {
            String EnUserid = HttpUtility.UrlEncode(DateStr, Encoding.GetEncoding("UTF-8"));
            return EnUserid;
        }
        public static String GetParam(Dictionary<String, String> Param)
        {
            String AddString = "";
            int Start_N = 0;
            foreach (string key in Param.Keys)
            {

                AddString += key + "=" + Param[key];
                Start_N++;
                if (Start_N < Param.Count)
                {

                    AddString += "&";

                }
            }


            return AddString;

        }

        /// <summary>
        /// 华势代付支持的银行
        /// </summary>
        /// <param name="bankName"></param>
        /// <returns></returns>
        public static string checkBankName(string bankName)
        {
            string no = "0";
            switch (bankName)
            {
                case "中国工商银行":
                case "工商银行":
                    no = "10001";
                    break;
                case "中国农业银行":
                case "农业银行":
                    no = "10002";
                    break;
                case "中国银行":
                    no = "10003";
                    break;
                case "中国建设银行":
                case "建设银行":
                    no = "10004";
                    break;
                case "交通银行":
                case "中国交通银行":
                    no = "10005";
                    break;
                case "邮政储蓄银行":
                case "中国邮政储蓄银行":
                    no = "10006";
                    break;
                case "中信银行":
                case "中国中信银行":
                    no = "11001";
                    break;
                case "中国光大银行":
                case "光大银行":
                    no = "11002";
                    break;
                case "华夏银行":
                case "中国华夏银行":
                    no = "11003";
                    break;
                case "中国民生银行":
                case "民生银行":
                    no = "11004";
                    break;
                case "中国招商银行":
                case "招商银行":
                    no = "11005";
                    break;
                case "兴业银行":
                case "中国兴业银行":
                    no = "11006";
                    break;
                case "广发银行":
                case "中国广发银行":
                    no = "11007";
                    break;
                case "平安银行":
                case "中国平安银行":
                    no = "11008";
                    break;
                case "上海浦东发展银行":
                    no = "11009";
                    break;
                case "恒丰银行":
                case "中国恒丰银行":
                    no = "11010";
                    break;
                case "浙商银行":
                    no = "11011";
                    break;
                case "渤海银行":
                    no = "11012";
                    break;
                case "天津农村商业银行":
                    no = "11013";
                    break;
                case "外换银行":
                    no = "11014";
                    break;
                case "友利银行":
                    no = "11015";
                    break;
                case "新韩银行":
                    no = "11015";
                    break;
                case "企业银行":
                    no = "11015";
                    break;
                case "韩亚银行":
                    no = "11015";
                    break;
                case "天津银行":
                    no = "11015";
                    break;
                case "邯郸银行":
                    no = "11015";
                    break;
                case "邢台银行":
                    no = "11015";
                    break;
                case "张家口市商业银行":
                    no = "11015";
                    break;
                case "承德银行":
                    no = "11015";
                    break;
                case "沧州银行":
                    no = "11015";
                    break;
                case "廊坊银行":
                    no = "11015";
                    break;
                case "晋商银行":
                    no = "11015";
                    break;
                case "晋城银行":
                    no = "11015";
                    break;
                case "内蒙古银行":
                    no = "11015";
                    break;
                case "包商银行":
                    no = "11015";
                    break;
                case "鄂尔多斯银行":
                    no = "11015";
                    break;
                case "大连银行":
                    no = "11015";
                    break;
                case "鞍山市商业银行":
                    no = "11015";
                    break;
                case "锦州银行":
                    no = "11015";
                    break;
                case "葫芦岛银行":
                    no = "11015";
                    break;
                case "营口沿海银行":
                    no = "11015";
                    break;
                case "阜新银行":
                    no = "11015";
                    break;
                case "吉林银行":
                    no = "11015";
                    break;
                case "哈尔滨市商业银行":
                    no = "11015";
                    break;
                case "嘉兴银行":
                    no = "11015";
                    break;
                case "湖州银行":
                    no = "11015";
                    break;
                case "绍兴银行":
                    no = "11015";
                    break;
                case "浙江稠州商业银行":
                    no = "11015";
                    break;
                case "台州银行":
                    no = "11015";
                    break;
                case "浙江泰隆商业银行":
                    no = "11015";
                    break;
                case "浙江民泰商业银行":
                    no = "11015";
                    break;
                case "福建海峡银行":
                    no = "11015";
                    break;
                case "东营市商业银行":
                    no = "11015";
                    break;
                case "江西银行":
                case "南昌银行":
                    no = "11015";
                    break;
                case "赣州银行":
                    no = "11015";
                    break;
                case "烟台银行":
                    no = "11015";
                    break;
                case "潍坊银行":
                    no = "11015";
                    break;
                case "济宁银行":
                    no = "11015";
                    break;
                case "泰安市商业银行":
                    no = "11015";
                    break;
                case "莱商银行":
                    no = "11015";
                    break;
                case "威海市商业银行":
                    no = "11015";
                    break;
                case "德州银行":
                    no = "11015";
                    break;
                case "临商银行":
                    no = "11015";
                    break;
                case "南阳银行":
                    no = "11015";
                    break;
                case "郑州银行":
                    no = "11015";
                    break;
                case "开封市商业银行":
                    no = "11015";
                    break;
                case "洛阳银行":
                    no = "11015";
                    break;
                case "漯河银行":
                    no = "11015";
                    break;
                case "商丘银行":
                    no = "11015";
                    break;
                case "富滇银行":
                    no = "11015";
                    break;
                case "长沙银行":
                    no = "11015";
                    break;
                case "柳州银行":
                    no = "11015";
                    break;
                case "珠海华润银行":
                    no = "11015";
                    break;
                case "广东南粤银行":
                    no = "11015";
                    break;
                case "广西北部湾银行":
                    no = "11015";
                    break;
                case "重庆银行":
                    no = "11015";
                    break;
                case "自贡市商业银行":
                    no = "11015";
                    break;
                case "攀枝花市商业银行":
                    no = "11015";
                    break;
                case "德阳银行":
                    no = "11015";
                    break;
                case "绵阳市商业银行":
                    no = "11015";
                    break;
                case "贵阳银行":
                    no = "11015";
                    break;
                case "长安银行":
                    no = "11015";
                    break;
                case "兰州银行":
                    no = "11015";
                    break;
                case "青海银行":
                    no = "11015";
                    break;
                case "乌鲁木齐市商业银行":
                    no = "11015";
                    break;
                case "昆仑银行":
                    no = "11015";
                    break;
                case "成都银行":
                    no = "11015";
                    break;
                case "南充市商业银行":
                    no = "11015";
                    break;
                case "昆山农村商业银行":
                    no = "11015";
                    break;
                case "吴江农村商业银行":
                    no = "11015";
                    break;
                case "常熟农村商业银行":
                    no = "11015";
                    break;
                case "张家港农村商业银行":
                    no = "11015";
                    break;
                case "广州农村商业银行":
                    no = "11015";
                    break;
                case "顺德农村商业银行":
                    no = "11015";
                    break;
                case "吉林农村信用社":
                    no = "11015";
                    break;
                case "江苏省农村信用社联合社":
                    no = "11015";
                    break;
                case "浙江省农村信用社":
                    no = "11015";
                    break;
                case "鄞州银行":
                    no = "11015";
                    break;
                case "安徽省农村信用社联合社":
                    no = "11015";
                    break;
                case "福建福州农村商业银行":
                    no = "11015";
                    break;
                case "山东省农村信用社联合社":
                    no = "11015";
                    break;
                case "湖北农信":
                    no = "11015";
                    break;
                case "深圳农商行":
                case "深圳农村商业银行":
                    no = "11015";
                    break;
                case "东莞农村商业银行":
                    no = "11015";
                    break;
                case "广西壮族自治区农村信用社":
                    no = "11015";
                    break;
                case "海南省农村信用社":
                    no = "11015";
                    break;
                case "云南省农村信用社":
                    no = "11015";
                    break;
                case "黄河农村商业银行":
                    no = "11015";
                    break;
                case "北京银行":
                    no = "12001";
                    break;
                case "上海银行":
                    no = "12040";
                    break;
                case "南京银行":
                    no = "12042";
                    break;
                case "杭州银行":
                    no = "12045";
                    break;
                case "温州银行":
                    no = "12046";
                    break;
                case "宁波银行":
                    no = "12055";
                    break;
                case "北京农村商业银行":
                    no = "13001";
                    break;
                case "上海农商银行":
                    no = "13018";
                    break;
                case "河北银行":
                    no = "12003";
                    break;
                case "宁夏银行":
                    no = "12120";
                    break;
                case "厦门银行":
                    no = "12130";
                    break;
                case "青岛银行":
                    no = "12132";
                    break;
                case "江苏银行":
                    no = "12041";
                    break;
                case "苏州银行":
                    no = "12044";
                    break;
                case "徽商银行":
                    no = "12056";
                    break;
                case "九江银行":
                    no = "12060";
                    break;
                case "上饶银行":
                    no = "12062";
                    break;             
                case "齐商银行":
                    no = "12065";
                    break;
                case "日照银行":
                    no = "12070";
                    break;
                case "汉口银行":
                    no = "12083";
                    break;
                case "广州银行":
                    no = "12086";
                    break;
                case "东莞银行":
                    no = "12089";
                    break;
                case "重庆农村商业银行":
                    no = "13125";
                    break;
            }
            return no;
        }
    }




}