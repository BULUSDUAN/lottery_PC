using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.HK6
{
    public class CalendarHelper
    {
       
        private int[] madd = new int[12];
        private string tgString = "甲乙丙丁戊己庚辛壬癸";
        private string dzString = "子丑寅卯辰巳午未申酉戌亥";
        private string numString = "一二三四五六七八九十";
        private string monString = "正二三四五六七八九十冬腊";
        private string weekString = "日一二三四五六";
        private string sx = "鼠牛虎兔龙蛇马羊猴鸡狗猪";
        private int cYear, cMonth, cDay ;
        public DateTime TheDate;
        private int[] CalendarData = new int[]{ 0xA4B, 0x5164B, 0x6A5, 0x6D4, 0x415B5, 0x2B6, 0x957, 0x2092F, 0x497, 0x60C96, 0xD4A, 0xEA5, 0x50DA9, 0x5AD, 0x2B6, 0x3126E, 0x92E, 0x7192D, 0xC95, 0xD4A, 0x61B4A, 0xB55, 0x56A, 0x4155B, 0x25D, 0x92D, 0x2192B, 0xA95, 0x71695, 0x6CA, 0xB55, 0x50AB5, 0x4DA, 0xA5B, 0x30A57, 0x52B, 0x8152A, 0xE95, 0x6AA, 0x615AA, 0xAB5, 0x4B6, 0x414AE, 0xA57, 0x526, 0x31D26, 0xD95, 0x70B55, 0x56A, 0x96D, 0x5095D, 0x4AD, 0xA4D, 0x41A4D, 0xD25, 0x81AA5, 0xB54, 0xB6A, 0x612DA, 0x95B, 0x49B, 0x41497, 0xA4B, 0xA164B, 0x6A5, 0x6D4, 0x615B4, 0xAB6, 0x957, 0x5092F, 0x497, 0x64B, 0x30D4A, 0xEA5, 0x80D65, 0x5AC, 0xAB6, 0x5126D, 0x92E, 0xC96, 0x41A95, 0xD4A, 0xDA5, 0x20B55, 0x56A, 0x7155B, 0x25D, 0x92D, 0x5192B, 0xA95, 0xB4A, 0x416AA, 0xAD5, 0x90AB5, 0x4BA, 0xA5B, 0x60A57, 0x52B, 0xA93, 0x40E95};

        public CalendarHelper()
        {
            madd[0] = 0;
            madd[1] = 31;
            madd[2] = 59;
            madd[3] = 90;
            madd[4] = 120;
            madd[5] = 151;
            madd[6] = 181;
            madd[7] = 212;
            madd[8] = 243;
            madd[9] = 273;
            madd[10] = 304;
            madd[11] = 334;

        }
        public int GetBit(int m, int n)
        {
            return (m >> n) & 1;
        }
        public void e2c(DateTime dt)
        {
           var TheDate = dt;
            int total; int m, n, k;
            var isEnd = false;
            var tmp = TheDate.Year;
            if (tmp < 1900)
            {
                tmp += 1900;
            }
            double db = (tmp - 1921) / 4;
            total =(int)( (tmp - 1921) * 365 + Math.Floor(db) + madd[TheDate.Month] + TheDate.Day - 38);
            if (TheDate.Year % 4 == 0 && TheDate.Month > 1)
            {
                total++;
            }
            for (m = 0; ; m++)
            {
                k = (CalendarData[m] < 0xfff) ? 11 : 12;
                for (n = k; n >= 0; n--)
                {
                    if (total <= 29 + GetBit(CalendarData[m], n))
                    {
                        isEnd = true;
                        break;
                    }
                    total = total - 29 - GetBit(CalendarData[m], n);
                }
                if (isEnd) break;
            }
           int cYear = 1921 + m;
            int cMonth = k - n + 1;
            int cDay = total;
            if (k == 12)
            {
                double tdb= CalendarData[m] / 0x10000;
                if (cMonth == Math.Floor(tdb) + 1)
                {
                    cMonth = 1 - cMonth;
                }
                tdb = CalendarData[m] / 0x10000;
                if (cMonth > Math.Floor(tdb) + 1)
                {
                    cMonth--;
                }
            }
        }

        public string GetcDateString()
        {
            string tmp = "";
            tmp += tgString.ToCharArray()[((cYear - 4) % 10)]+"";
            tmp += dzString.ToCharArray()[((cYear - 4) % 12)] + "";
            tmp += "(";
            tmp += sx.ToCharArray()[((cYear - 4) % 12)] + "";
            tmp += ")年 ";
            if (cMonth < 1)
            {
                tmp += "(闰)";
                tmp += monString.ToCharArray()[(-cMonth - 1)] + "";
            }
            else
            {
                tmp += monString.ToCharArray()[(cMonth - 1)] + "";
            }
            tmp += "月";
            tmp += (cDay < 11) ? "初" : ((cDay < 20) ? "十" : ((cDay < 30) ? "廿" : "三十"));
            if (cDay % 10 != 0 || cDay == 10)
            {
                tmp += numString.ToCharArray()[((cDay - 1) % 10)] + "";
            }
            return tmp;
        }

    }
}
