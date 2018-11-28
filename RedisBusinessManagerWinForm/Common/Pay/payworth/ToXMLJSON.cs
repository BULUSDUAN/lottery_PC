using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Runtime.Serialization;


namespace Common.Pay.payworth
{
    public class ToXMLJSON
    {
        public static String ObjectToXmlJson(Object tc,String data_type)
        {
            String retult = null;             
            if(data_type.ToLower().Equals("xml"))
            {
                retult = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                retult += "<data_content>";
                retult += XMLStr(tc);
                retult += "</data_content>";
            }
            if(data_type.ToLower().Equals("json"))
            {
                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();       //执行序列化
                retult = jsonSerializer.Serialize(tc);
            }

            return retult;

        }

        public static String XMLStr(Object TempObj) {

            String TempStr = null;

            if (typeof(List<Dictionary<String, Object>>) == TempObj.GetType()) {
                List<Dictionary<String, Object>> TempList = (List<Dictionary<String, Object>>)TempObj;
                for(int i =0;i<TempList.Count();i++){
                    TempStr += XMLStr(TempList[i]);
                }

            }
            else if (typeof(Dictionary<String, Object>) == TempObj.GetType())
            {
                Dictionary<String, Object> TempDic = (Dictionary<String, Object>)TempObj;
                foreach (String key in TempDic.Keys) {
                    if (TempDic[key] == null) {
                        TempStr += "<" + key + "/>";
                    }
                    else if (typeof(String) == TempDic[key].GetType())
                    {
                        if (Convert.IsDBNull(TempDic[key]) || String.IsNullOrEmpty(TempDic[key].ToString()))
                        {
                            TempStr += "<" + key + "/>";
                        }
                        else
                        {
                            TempStr += "<" + key + ">" + TempDic[key] + "</" + key + ">";
                        }
                    }
                    else if (typeof(List<Dictionary<String, Object>>) == TempDic[key].GetType())
                    {
                        TempStr += "<" + key + ">";
                        TempStr += XMLStr(TempDic[key]);
                        TempStr += "</" + key + ">";

                    }
                    else if (typeof(Dictionary<String, Object>) == TempDic[key].GetType())
                    {
                        TempStr += "<" + key + ">";
                        TempStr += XMLStr(TempDic[key]);
                        TempStr += "</" + key + ">";

                    }
                    else {
                        throw new ArgumentException("不能处理的数据类型[" + TempDic[key].GetType() + "]");
                    }
                        
                }

            }
            else {
                throw new ArgumentException("不能处理的数据类型[" + TempObj.GetType()+"]");
            }

            return TempStr;
        }

                
    }
}
