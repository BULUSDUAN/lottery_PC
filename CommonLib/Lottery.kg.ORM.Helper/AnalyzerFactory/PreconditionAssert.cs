using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using EntityModel.ExceptionExtend;

namespace KaSon.FrameWork.ORM.Helper.AnalyzerFactory
{
    /// <summary>
    /// 前置条件断言，辅助类
    /// </summary>
    public static class PreconditionAssert
    {
        /// <summary>
        /// 断言传入对象不为null
        /// </summary>
        /// <param name="value">被验证对象</param>
        /// <param name="message">验证失败的消息</param>
        public static object IsNotNull(object value, string message)
        {
            if (value == null)
            {
                throw new PreconditionException(message ?? "前置条件断言失败 - " + "要求不为null");
            }
            return value;
        }
        /// <summary>
        /// 断言传入字符串不是空字符串，包括只有空格的字符串
        /// </summary>
        /// <param name="value">被验证字符串</param>
        /// <param name="message">验证失败的消息</param>
        public static string IsNotEmptyString(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new PreconditionException(message ?? "前置条件断言失败 - " + "要求不为空字符串");
            }
            return value;
        }
        /// <summary>
        /// 断言传入值为真
        /// </summary>
        /// <param name="condition">被验证的布尔条件</param>
        /// <param name="message">验证失败的消息</param>
        public static bool IsTrue(bool condition, string message)
        {
            if (!condition)
            {
                throw new PreconditionException(message ?? "前置条件断言失败 - " + "要求为真");
            }
            return condition;
        }
        /// <summary>
        /// 断言传入值为假
        /// </summary>
        /// <param name="condition">被验证的布尔条件</param>
        /// <param name="message">验证失败的消息</param>
        public static bool IsFalse(bool condition, string message)
        {
            if (condition)
            {
                throw new PreconditionException(message ?? "前置条件断言失败 - " + "要求为假");
            }
            return condition;
        }

        public static void ContianKey(IDictionary dic, object key, string message)
        {
            if (!dic.Contains(key))
            {
                throw new ArgumentException(string.Format(message, key));
            }
        }

        public static void ContianKeys(IDictionary dic, object[] keys, string message)
        {
            foreach (var key in keys)
            {
                ContianKey(dic, key, message);
            }
        }
        public static void MaxLength(string value, int length, string message)
        {
            if (value.Length > length)
                throw new ArgumentException(message);
        }

        public static int IsInt32(string number, string message)
        {
            Int32 output;
            if (!Int32.TryParse(number, out output))
            {
                throw new ArgumentException(message);
            }
            return output;
        }
        public static decimal IsDecimal(string number, string message)
        {
            Decimal output;
            if (!Decimal.TryParse(number, out output))
            {
                throw new ArgumentException(message);
            }
            return output;
        }
        public static bool AsBool(string value, string message)
        {
            bool result;
            if (!bool.TryParse(value, out result))
            {
                throw new ArgumentException(message);
            }
            return result;
        }
        public static void CanFormatString(string str, object[] values, string message)
        {
            try
            {
                string.Format(str, values);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(message, ex);
            }
        }

        /// <summary>
        /// 判断传入数字是否small到big间（包括两端）
        /// </summary>
        /// <param name="value">被判断值</param>
        /// <param name="big">大</param>
        /// <param name="small">小</param>
        /// <param name="message">信息</param>
        /// <returns></returns>
        public static int IsIntegral(int value, int big, int small, string message)
        {
            if (small <= value && value <= big)
            {
                return value;
            }
            else
            {
                throw new PreconditionException(message);
            }

        }
    }
}
