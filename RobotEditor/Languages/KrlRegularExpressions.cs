using System.Text.RegularExpressions;

namespace RobotEditor.Languages
{
    public static class KrlRegularExpressions
    {
      
       
        private static readonly Regex defLineRegex = new Regex("^\\s*((global\\s+)?(def|deffct))\\s+.*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex emptyLineRegex = new Regex("^\\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex endDefLineRegex = new Regex("^\\s*(end|endfct)((\\s+.*)|\\s*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex endIfLineRegex = new Regex("^\\s*endif(\\s*;{1,}.*)*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex endWhileLineRegex = new Regex("^\\s*endwhile(\\s*;{1,}.*)*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex loopLine = new Regex("^\\s*loop(|\\s*|\\s*;.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex endloopLine = new Regex("^\\s*endloop(|\\s*|\\s*;.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex foldEndLineRegex = new Regex("^\\s*;\\s*endfold((\\s+.*)|\\s*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex foldStartLineRegex = new Regex("^\\s*;\\s*fold((\\s+.*)|\\s*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ifLineRegex = new Regex("^\\s*if\\s.*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex validNameRegex = new Regex("^(?<Name>[A-Za-z_$]{1}[A-Za-z_$0-9]{0,23})$", RegexOptions.Compiled);
        private static readonly Regex whileLineRegex = new Regex("^\\s*while\\s.*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex forLineRegex = new Regex("^\\s*for\\s.*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex endforLineRegex = new Regex("^\\s*endfor(|\\s*|\\s*;.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex DefLineRegex => defLineRegex;
        public static Regex EmptyLineRegex => emptyLineRegex;
        public static Regex Loop => loopLine;
        public static Regex EndLoop => endloopLine;
        public static Regex For => forLineRegex;
        public static Regex EndFor => endforLineRegex;
        public static Regex EndDefLineRegex => endDefLineRegex;
        public static Regex EndIf => endIfLineRegex;
        public static Regex EndWhile => endWhileLineRegex;
        public static Regex FoldEndLineRegex => foldEndLineRegex;
        public static Regex FoldStartLineRegex => foldStartLineRegex;
        public static Regex If => ifLineRegex;
        public static Regex ValidNameRegex => validNameRegex;
        public static Regex While => whileLineRegex;
    }
}
