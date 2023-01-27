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
        internal static bool IsMatch(this string value, string pattern, RegexOptions options = RegexOptions.IgnoreCase)
               => Regex.IsMatch(value, pattern, options);


        internal static MatchCollection GetMatches(this string value, string pattern, RegexOptions options = RegexOptions.IgnoreCase)
            => Regex.Matches(value, pattern, options);


        internal static bool IsWhitespaceOrNewline(this char ch) => ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';

        internal static bool IsWordPart(this char ch) => char.IsLetterOrDigit(ch) || ch == '_';

    }


}
