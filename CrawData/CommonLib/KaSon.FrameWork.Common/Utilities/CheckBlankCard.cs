using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.Common.Utilities
{
    public class CheckBlankCard
    {

        public static bool MatchLuhn(String cardNo)
        {
            int[] cardNoArr = new int[cardNo.Length];
            IList<int> list = new List<int>();
            foreach (var item in cardNo.ToCharArray())
            {
                list.Add(int.Parse(item.ToString()));
            }

            //for (int i = 0; i < cardNo.Length; i++)
            //{
            //    cardNoArr[i] = int.Parse(String.valueOf(cardNo.(i)));
            //}
            //foreach (var item in list)
            //{

            //}
            for (int i = list.Count - 2; i >= 0; i -= 2)
            {
                list[i] <<= 1;
                list[i] = list[i] / 10 + list[i] % 10;
            }
            int sum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                sum += list[i];
            }
            return sum % 10 == 0;
        }

    }
}