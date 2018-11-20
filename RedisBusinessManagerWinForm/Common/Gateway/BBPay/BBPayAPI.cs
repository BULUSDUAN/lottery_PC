using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services.Protocols;
using Common.Cryptography;
using System.Collections.Generic;
using System.Collections;
using Common.JSON;


namespace Common.Gateway.BBPay
{
    public class BBPayAPI
    {

        public string Pay(BBPayAPIInfo info)
        {
            string formHtml = string.Empty;
            SortedDictionary<string, object> sd = new SortedDictionary<string, object>();
            sd.Add("pt", "02");//网银
            sd.Add("order", info.order);
            sd.Add("transtime", info.transtime);
            sd.Add("currency", info.currency);
            sd.Add("amount", info.amount);
            sd.Add("productcategory", info.productcategory);
            sd.Add("productname", info.productname);
            sd.Add("productdesc", info.productdesc);
            sd.Add("productprice", info.amount);
            sd.Add("productcount", info.productcount);
            sd.Add("merrmk", "");
            sd.Add("identityid", info.identityid);
            sd.Add("identitytype", info.identitytype);
            sd.Add("userip", info.userip);
            sd.Add("userua", info.userua);
            sd.Add("areturl", info.areturl);
            sd.Add("sreturl", info.sreturl);
            sd.Add("pnc", info.pnc.ToString());
            //生成Md5签名
            var source = string.Join("", (from d in sd select d.Value).ToArray()) + info.BBPayKey;
            var sign = Encipherment.MD5(source, Encoding.UTF8);
            sd.Add("sign", sign);
            string json = Common.JSON.JsonHelper.Serialize(sd);
            var data = System.Web.HttpUtility.UrlEncode(json);
            StringBuilder strBud = new StringBuilder();
            strBud.Append("<form name='formBBPay'  id='formBBPay' method='post' action='" + info.BBPayUrl + "'/>");
            strBud.Append("<input type='hidden' name='data'  id='data' value='" + data + "'/>");
            strBud.Append("<input type='hidden' name='merchantaccount'id='transtime' value='" + info.BBPayId + "'/>");
            strBud.Append("<input type='hidden' name='encryptkey' id='encryptkey' value='1'/>");
            strBud.Append("</form>");
            strBud.Append("<script language=javascript>document.getElementById('formBBPay').submit();</script>");
            formHtml = strBud.ToString();
            return formHtml;
        }

        public BBPayCallBackInfo GetBBPayCallBackSign(string data, string encryptkey, string md5key)
        {
            BBPayCallBackInfo info = new BBPayCallBackInfo();
            try
            {
                SortedDictionary<string, object> callinfo = Common.JSON.JsonHelper.Deserialize<SortedDictionary<string, object>>(HttpUtility.UrlDecode(data));
                if (!callinfo.ContainsKey("sign"))
                    throw new Exception("no sign");
                if (!callinfo.ContainsKey("amount"))
                    throw new Exception("no amount");
                if (!callinfo.ContainsKey("bborderid"))
                    throw new Exception("no bborderid");
                if (!callinfo.ContainsKey("order"))
                    throw new Exception("no order");
                if (!callinfo.ContainsKey("merrmk"))
                    throw new Exception("no merrmk");
                if (!callinfo.ContainsKey("merid"))
                    throw new Exception("no merid");
                if (!callinfo.ContainsKey("identityid"))
                    throw new Exception("no identityid");
                if (!callinfo.ContainsKey("identitytype"))
                    throw new Exception("no identitytype");
                if (!callinfo.ContainsKey("status"))
                    throw new Exception("no status");
                
                var source = string.Join("", (from d in callinfo where d.Key != "sign" select d.Value).ToArray());
                var sign = callinfo["sign"].ToString();
                var resultKey = Encipherment.MD5(source + md5key, Encoding.UTF8);
                info.amount = (int) (decimal.Parse(callinfo["amount"].ToString())/100);
                info.bborderid = callinfo["bborderid"].ToString();
                info.order = callinfo["order"].ToString();
                info.merrmk = callinfo["merrmk"].ToString();
                info.merid = callinfo["merid"].ToString();
                info.identityid = callinfo["identityid"].ToString();
                info.identitytype = callinfo["identitytype"].ToString();
                if (resultKey.Equals(sign))
                {
                    info.status = Convert.ToInt32(callinfo["status"]);
                    info.sign = sign;
                }
                else
                {
                    throw new Exception("sign not equals");
                    //info.status = 0;
                    //info.sign = "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return info;
        }

    }
}
