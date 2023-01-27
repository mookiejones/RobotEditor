using RobotEditor.Enums;

namespace RobotEditor.Parsers
{
    public abstract class AbstractParser
    {
        public string Parse(Token value) => string.Empty;

        public sealed class PeekToken
        {
            public PeekToken(int index, Token value)
            {
                TokenIndex = index;
                TokenPeek = value;
            }

            public int TokenIndex { get; set; }
            public Token TokenPeek { get; set; }
        }

        public sealed class Token
        {
            public Token(Tokens name, string value)
            {
                TokenName = name;
                TokenValue = value;
            }

            public Tokens TokenName { get; set; }
            public string TokenValue { get; set; }
        }
    }
}