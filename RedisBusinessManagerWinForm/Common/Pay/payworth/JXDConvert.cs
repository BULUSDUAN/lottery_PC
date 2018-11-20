using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Newtonsoft.Json;
using System.Web.Script.Serialization;


namespace Common.Pay.payworth
{
    public class JXDConvert
    {
        public static String XmlToJson(String XmlString){
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(XmlString);
            String JsonStr = JsonConvert.SerializeXmlNode(XmlDoc);
            return JsonStr;
        }
    

    public static  Dictionary<String, Object> JsonToDictionary(String jsonData){
        //实例化JavaScriptSerializer类的新实例
        JavaScriptSerializer JsonStr = new JavaScriptSerializer();
        try{
            //将指定的 JSON 字符串转换为 Dictionary<string, object> 类型的对象            
            return JsonStr.Deserialize<Dictionary<String, Object>>(jsonData);
        }
        catch (Exception ex){
            throw new Exception(ex.Message);
            }
        }
    }
}