using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.AnalyzerFactory
{
    /// <summary>
    /// 竞彩奖金算法
    /// </summary>
    public class SMGBonus
    {
        /// <summary>
        /// 四舍六入、五看奇偶(第二位小数是奇数进位、偶数舍弃
        /// </summary>
        /// <returns></returns>
        public decimal FourToSixHomesInFive(decimal number)
        {
            var num = number.ToString();
            var star = num.IndexOf(".");
            if (star == -1) return number;

            num = num.Substring(star + 1);
            if (num.Length <= 2) return number;
            //num=1454
            var arry = num.ToArray();
            //4
            var lastF = int.Parse(arry[1].ToString());
            //5
            var lastT = int.Parse(arry[2].ToString());

            if (lastT > 5 || (lastT == 5 && lastF % 2 != 0))
                number = Math.Truncate(number * 100) / 100 + 0.01M;
            else if (lastT < 5 || (lastT == 5 && lastF % 2 == 0))
                number = Math.Truncate(number * 100) / 100;

            return number;
        }
    }
}
