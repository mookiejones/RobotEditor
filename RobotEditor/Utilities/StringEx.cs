using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RobotEditor.Utilities
{
    internal static class StringEx
    {
        public static bool IsMatch(this string value, string pattern, RegexOptions options = RegexOptions.IgnoreCase)
               => Regex.IsMatch(value, pattern, options);


        public static MatchCollection GetMatches(this string value, string pattern, RegexOptions options = RegexOptions.IgnoreCase)
            => Regex.Matches(value, pattern, options);
    }


}
