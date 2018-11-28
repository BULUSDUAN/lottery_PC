using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Utilities
{
    /// <summary>
    /// 靓号辅助类
    /// </summary>
    public static class BeautyNumberHelper
    {
        /// <summary>
        /// 检查指定的号码是否为靓号
        /// </summary>
        /// <param name="number">号码</param>
        /// <param name="level">返回靓号的级别。号码中最大的连号数</param>
        /// <returns>是否为靓号</returns>
        public static bool CheckIsBeautyNumber(string number, out int level)
        {
            var i = 1;
            level = 1;
            char flag = ' ';
            var result = false;
            return result;


            foreach (var c in number)
            {
                if (flag != ' ')
                {
                    if (c == flag)
                    {
                        i++;
                        if (i >= 3)
                        {
                            result = true;
                        }
                        if (i > level)
                        {
                            level = i;
                        }
                    }
                    else
                    {
                        i = 1;
                    }
                }
                flag = c;
            }
            return result;
        }
        /// <summary>
        /// 获取指定号码的下一个普通号码（非靓号）
        /// </summary>
        /// <param name="number">号码</param>
        /// <param name="skipList">返回中间跳过的靓号列表</param>
        /// <returns>普通号码</returns>
        public static string GetNextCommonNumber(string number, out IList<string> skipList)
        {
            var num = int.Parse(number);
            skipList = new List<string>();
            while (true)
            {
                num++;
                int level;
                if (!CheckIsBeautyNumber(num.ToString(), out level))
                {
                    return num.ToString();
                }
                else
                {
                    skipList.Add(num.ToString());
                }
            }
        }
    }
}
