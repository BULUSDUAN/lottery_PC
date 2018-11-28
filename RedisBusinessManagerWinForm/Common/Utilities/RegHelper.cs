using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Utilities
{
    public static class RegHelper
    {
        public static bool IsReg(string text, string pattern, RegexOptions options = RegexOptions.None)
        {
            var reg = new Regex(pattern, options);
            var match = reg.Match(text);
            return match.Success;
        }
        public static string GetRegText(string text, string pattern, RegexOptions options = RegexOptions.None)
        {
            var reg = new Regex(pattern, options);
            var match = reg.Match(text);
            if (!match.Success)
            {
                return null;
            }
            return match.Value;
        }
        public static IList<string> GetRegList(string text, string pattern, RegexOptions options = RegexOptions.None, Action<Regex, Match> action = null)
        {
            var list = new List<string>();
            var reg = new Regex(pattern, options);
            var match = reg.Match(text);
            while (match.Success)
            {
                list.Add(match.Value);
                if (action != null)
                {
                    action(reg, match);
                }

                match = match.NextMatch();
            }
            return list;
        }
        public static string RegReplace(string text, string pattern, string replacement, RegexOptions options = RegexOptions.None)
        {
            var reg = new Regex(pattern, options);
            return reg.Replace(text, replacement);
        }
    }
}
