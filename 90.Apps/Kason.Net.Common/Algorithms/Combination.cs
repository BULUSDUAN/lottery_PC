using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kason.Net.Common.Algorithms
{
    /// <summary>
    /// 组合算法
    /// </summary>
    public class Combination
    {
        /// <summary>
        /// 计算a取b的组合数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int Calculate(int a, int b)
        {
            if (a < b)
            {
                return 0;
            }
            var sub = b;
            if (a - b < b)
            {
                sub = a - b;
            }
            var r = 1;
            var s = 1;
            for (int i = 0; i < sub; i++)
            {
                r *= a - i;
                s *= i + 1;
            }
            return r / s;
        }
        /// <summary>
        /// 给一个数组，从数组中获取 K 个元素的组合，并回调指定函数
        /// </summary>
        /// <param name="array">指定原始字符串数组</param>
        /// <param name="k">从数组中获取 K 个元素的组合</param>
        /// <param name="callBack">回调函数</param>
        public void Calculate(string[] array, int k, Action<string[]> callBack)
        {
            Calculate<string>(array, k, callBack);
        }
        public void Calculate<T>(T[] array, int k, Action<T[]> callBack)
        {
            int[] indexList = new int[k + 1];
            indexList[0] = k;
            int m = array.Length;
            CoreCompute(indexList, m, k, (item) =>
            {
                var rlt = new T[k];
                for (int i = 0, j = 0; j < k; i++, j++)
                {
                    rlt[i] = array[item[j] - 1];
                }
                callBack(rlt);
                return true;
            });
        }
        /// <summary>
        /// 给一个数组，从数组中获取 K 个元素的组合，并回调指定函数
        /// </summary>
        /// <param name="array">指定原始字符串数组</param>
        /// <param name="k">从数组中获取 K 个元素的组合</param>
        /// <param name="callBack">回调函数</param>
        public void Calculate(string[] array, int k, Func<string[], bool> callBack)
        {
            Calculate<string>(array, k, callBack);
        }
        public void Calculate<T>(T[] array, int k, Func<T[], bool> callBack)
        {
            int[] indexList = new int[k + 1];
            indexList[0] = k;
            int m = array.Length;
            CoreCompute(indexList, m, k, (item) =>
            {
                var rlt = new T[k];
                for (int i = 0, j = 0; j < k; i++, j++)
                {
                    rlt[i] = array[item[j] - 1];
                }
                return callBack(rlt);
            });
        }


        private void CoreCompute(int[] a, int m, int k, Func<int[], bool> resultIndexItemHandler)
        {
            if (k == 0)
            {
                resultIndexItemHandler(a);
                return;
            }
            for (int i = m; i >= k; i--)
            {
                a[k] = i;
                if (k > 1)
                {
                    CoreCompute(a, i - 1, k - 1, resultIndexItemHandler);
                }
                else
                {
                    int[] rlt = a.Skip(1).Take(a[0]).ToArray();
                    if (!resultIndexItemHandler(rlt))
                    {
                        break;
                    }
                }
            }
        }
    }
}
