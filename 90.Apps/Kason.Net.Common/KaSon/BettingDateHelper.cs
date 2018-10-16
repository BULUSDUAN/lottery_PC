using System;
using System.Collections.Generic;
using System.Text;

namespace Kason.Net.Common.KaSon
{
    /// <summary>
    /// 追加期号
    /// </summary>
  public  class BettingDateHelper
    {
        /// <summary>
        /// 彩种最大期号
        /// </summary>
        /// <param name="gameCode"></param>
        /// <returns></returns>
        public static int GetMaxDate(string gameCode) {

            switch (gameCode)
            {
                case "CQSSC":
                    return 120;
                case"JXSSC":
                    return 84;
                case "JX11X5":
                    return 84;
                case "SD11X5":
                    return 87;
                case "GD11X5":
                    return 84;
                case "SDQYH":
                    return 40;
                case "GDKLSF":
                    return 84;
                case "GXKLSF":
                    return 50;
                case "FC3D":
                case "PL3":
                    return 358;
                case "SSQ":
                case "DLT":
                    return 154;
                case "JSKS":
                    return 82;
                case "SDKLPK3":
                    return 88;
                default:
                    return 0;
            }
        }

        public static List<string>  GetUpdate(string IssuseNumber, int currentMaxDate,string gameCode, int plus1) {

            string Illdate = IssuseNumber;
            int maxIll = currentMaxDate;
            int Illamount = plus1;

            var tmpIssuse = "";
            var numberArr = Illdate.Split('-');
            string Issuse0 = numberArr[0];
            int Issuse1 = int.Parse(numberArr[0]); // 期号前半部分，表示日期
            int Issuse2 = int.Parse(numberArr[1]); // 期号后半部分，表示具体期数
            var list = new List<string>();// [];
            var minDate = DateTime.Parse("2019-02-04"); // new Date('2019/02/04'); // 春节第1天（除夕）
            var maxDate = DateTime.Parse("2019-02-10"); //new Date('2019/02/10'); // 春节最后天（初六）
            for (var i = 0; i < Illamount; i++)
            {
                if (tmpIssuse =="")
                {
                    tmpIssuse = Illdate;
                }
                else
                {
                    Issuse2++;
                    if (Issuse2 > maxIll)
                    {
                        if (gameCode == "FC3D" || gameCode == "PL3" || gameCode == "GXKLSF" || gameCode == "SSQ" || gameCode == "DLT")
                        {
                            Issuse1 = Issuse1 + 1;
                            Issuse2 = 1;
                        }
                        else
                        {
                            var issuseDate = DateTime.Parse(Issuse0.Substring(0, 4) + "-" + Issuse0.Substring(4, 2) + "-" + Issuse0.Substring(6, 2));
                            issuseDate = issuseDate.AddDays(1);
                            // 屏蔽春节7天奖期
                            if (issuseDate >= minDate && issuseDate <= maxDate)
                            {
                                //issuseDate.setDate(issuseDate.getDate() + 1);
                                issuseDate = issuseDate.AddDays(1);
                            }
                            var date = issuseDate.Day;
                            var month = issuseDate.Month;
                            Issuse0 = issuseDate.Year + ((month < 10) ? ("0" + month) : month.ToString()).ToString() + ((date < 10) ? ("0" + date) : date.ToString()).ToString();
                            Issuse1= int.Parse(Issuse0);
                            Issuse2 = 1;
                        }
                    }
                    switch (Issuse2.ToString().Length)
                    {
                        case 1:
                            tmpIssuse = Issuse1 + "-" + (maxIll > 99 ? "00" : "0") + Issuse2;
                            break;
                        case 2:
                            tmpIssuse = Issuse1 + "-" + (maxIll > 99 ? "0" : "") + Issuse2;
                            break;
                        default:
                            tmpIssuse = Issuse1.ToString() + "-" + Issuse2;
                            break;
                    }
                }
                list.Add(tmpIssuse);
            }
            return list;

        }
    }
}
