using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Algorithms
{
    /// <summary>
    /// 排列算法
    /// </summary>
    public class Permutation
    {
        public void Calculate(string[] _arr, int target, Action<string[]> resultItemHandler)
        {
            int[] a = new int[target];
            InitArr(a, 0);
            CoreCompute(a, 0, target, _arr.Length, (item) =>
            {
                string[] rlt = new string[target];
                for (int i = 0, j = 0; j < target; i++, j++)
                {
                    rlt[i] = _arr[item[j]];
                }
                resultItemHandler(rlt);
            });
        }
        private void CoreCompute(int[] arr, int position, int target, int total, Action<int[]> resultIndexItemHandler)
        {
            for (int i = 0; i < total; i++)
            {
                if (arr.Contains(i))
                {
                    continue;
                }
                arr[position] = i;
                if (position + 1 >= target)
                {
                    resultIndexItemHandler(arr);
                }
                else
                {
                    CoreCompute(arr, position + 1, target, total, resultIndexItemHandler);
                }
            }
            InitArr(arr, position);
        }
        private void InitArr(int[] a, int p)
        {
            for (int i = p; i < a.Length; i++)
            {
                a[i] = -1;
            }
        }
    }
}
