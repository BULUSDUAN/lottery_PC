using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Common.Algorithms
{
    /// <summary>
    /// 数组的组合算法
    /// </summary>
    public class ArrayCombination
    {
        public void Calculate(string[][] array, Action<string[]> callback)
        {
            int length = array.Length;
            if (array.Length < 1)
            {
                return;
            }
            string[] tmp = new string[length];
            CoreCompute(array, tmp, 0, callback);
        }
        private void CoreCompute(string[][] all, string[] arr, int position, Action<string[]> resultIndexItemHandler)
        {
            string[] tmp = all[position];
            for (int i = 0; i < tmp.Length; i++)
            {
                arr[position] = tmp[i];
                if (position >= all.Length - 1)
                {
                    resultIndexItemHandler(arr);
                }
                else
                {
                    CoreCompute(all, arr, position + 1, resultIndexItemHandler);
                }
            }
        }


        public void Calculate<T>(T[][] array, Action<T[]> callback)
        {
            int length = array.Length;
            if (array.Length < 1)
            {
                return;
            }
            T[] tmp = new T[length];
            CoreComputeT(array, tmp, 0, callback);
        }
        private void CoreComputeT<T>(T[][] all, T[] arr, int position, Action<T[]> resultIndexItemHandler)
        {
            T[] tmp = all[position];
            for (int i = 0; i < tmp.Length; i++)
            {
                arr[position] = tmp[i];
                if (position >= all.Length - 1)
                {
                    resultIndexItemHandler(arr);
                }
                else
                {
                    CoreComputeT(all, arr, position + 1, resultIndexItemHandler);
                }
            }
        }
    }
}
