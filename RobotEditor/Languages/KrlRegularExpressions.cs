using System.Text.RegularExpressions;

namespace RobotEditor.Languages
{
    public static class KrlRegularExpressions
    {
        private static readonly Regex endforLineRegex = new Regex("^\\s*endfor(|\\s*|\\s*;.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex DefLineRegex { get; } = new Regex("^\\s*((global\\s+)?(def|deffct))\\s+.*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex EmptyLineRegex { get; } = new Regex("^\\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex Loop { get; } = new Regex("^\\s*loop(|\\s*|\\s*;.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex EndLoop { get; } = new Regex("^\\s*endloop(|\\s*|\\s*;.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex For { get; } = new Regex("^\\s*for\\s.*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex EndFor => endforLineRegex;
        public static Regex EndDefLineRegex { get; } = new Regex("^\\s*(end|endfct)((\\s+.*)|\\s*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex EndIf { get; } = new Regex("^\\s*endif(\\s*;{1,}.*)*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex EndWhile { get; } = new Regex("^\\s*endwhile(\\s*;{1,}.*)*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex FoldEndLineRegex { get; } = new Regex("^\\s*;\\s*endfold((\\s+.*)|\\s*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex FoldStartLineRegex { get; } = new Regex("^\\s*;\\s*fold((\\s+.*)|\\s*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex If { get; } = new Regex("^\\s*if\\s.*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex ValidNameRegex { get; } = new Regex("^(?<Name>[A-Za-z_$]{1}[A-Za-z_$0-9]{0,23})$", RegexOptions.Compiled);
        public static Regex While { get; } = new Regex("^\\s*while\\s.*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}
