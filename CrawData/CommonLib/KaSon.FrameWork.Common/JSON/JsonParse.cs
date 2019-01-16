using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using Newtonsoft.Json;

namespace KaSon.FrameWork.Common.JSON
{
    public class JsonParse
    {
        public static Dictionary<string, object> GetObjDictionary(string json)
        {
          //  JavaScriptSerializer jss = new JavaScriptSerializer();

            return (Dictionary<string, object>)JsonConvert.DeserializeObject(json);
        }
        public static Dictionary<string, string> GetStrDictionary(string json)
        {
            Dictionary<string, object> dic = GetObjDictionary(json);
           // JavaScriptSerializer jss = new JavaScriptSerializer();
            Dictionary<string, string> dicResult = new Dictionary<string, string>();

            foreach(var obj in dic)
            {
                dicResult.Add(obj.Key.ToString(), JsonConvert.SerializeObject(obj.Value));
            }

            return dicResult;
        }
        public static Dictionary<string, string> GetStrDictionary(string json, int total)
        {
            Dictionary<string, object> dic = GetObjDictionary(json);
            //JavaScriptSerializer jss = new JavaScriptSerializer();
            Dictionary<string, string> dicResult = new Dictionary<string, string>();

            int i = 1;

            foreach (var obj in dic)
            {
                dicResult.Add(obj.Key.ToString(), JsonConvert.SerializeObject(obj.Value));

                if (i >= total)
                {
                    break;
                }

                i++;
            }

            return dicResult;
        }
        public static string GetNode(string key, string json)
        {
            Dictionary<string, string> dic = GetStrDictionary(json);

            if (dic.Count > 0)
            {
                foreach (var d in dic)
                {
                    if (d.Key.ToString() == key)
                    {
                        return d.Value.ToString();
                    }
                }
            }
            else
            {
                return string.Empty;
            }

            return string.Empty;
        }
        /// <summary>
        /// 将DataTable结果集转化为Json
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="appendAttr">smaple: 'totalRow':12,</param>
        /// <returns>Json结果集</returns>
        public static string DataTableToJson(DataTable dt, string appendAttr)
        {
            StringBuilder JsonString = new StringBuilder();
            //Exception Handling        
            if (dt != null && dt.Rows.Count > 0)
            {
                JsonString.Append("{ ");
                JsonString.Append(appendAttr);
                JsonString.Append("\"data\":[ ");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{ ");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j < dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\"");
                        }
                    }

                    /*end Of String*/
                    if (i == dt.Rows.Count - 1)
                    {
                        JsonString.Append("} ");
                    }
                    else
                    {
                        JsonString.Append("}, ");
                    }
                }
                JsonString.Append("]}");
                return JsonString.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
