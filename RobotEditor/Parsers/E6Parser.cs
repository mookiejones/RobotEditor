using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using RobotEditor.Abstract;
using RobotEditor.Enums;

namespace RobotEditor.Parsers
{
    [Localizable(false)]
    public class E6Parser : AbstractParser
    {
        private readonly Dictionary<Tokens, MatchCollection> _regExMatchCollection;
        private readonly Dictionary<Tokens, string> _tokens;
        private int _index;
        private string _inputString;

        public E6Parser()
        {
            _tokens = new Dictionary<Tokens, string>();
            _regExMatchCollection = new Dictionary<Tokens, MatchCollection>();
            _index = 0;
            _inputString = string.Empty;
            _tokens.Add(Tokens.DECLARATION, "[Dd][Ee][Cc][Ll]");
            _tokens.Add(Tokens.E6POS, "[Ee][6][Pp][Oo][Ss]");
            _tokens.Add(Tokens.IDENTIFIER, "[a-zA-Z_][a-zA-Z0-9_]*");
            _tokens.Add(Tokens.END, "[Ee][Nn][Dd]");
            _tokens.Add(Tokens.COLON, "\\:");
            _tokens.Add(Tokens.EQUALS, "=");
            _tokens.Add(Tokens.STRING, "\".*?\"");
            _tokens.Add(Tokens.WHITESPACE, "[ \\t]+");
            _tokens.Add(Tokens.NEWLINE, "[\\r\\n]+");
            _tokens.Add(Tokens.REAL, "[\\d.-]*");
            _tokens.Add(Tokens.INTEGER, "[\\d]+");
            _tokens.Add(Tokens.APOSTROPHE, "'.*");
            _tokens.Add(Tokens.LPAREN, "\\(");
            _tokens.Add(Tokens.RPAREN, "\\)");
            _tokens.Add(Tokens.LBRACE, "\\}");
            _tokens.Add(Tokens.RBRACE, "\\{");
            _tokens.Add(Tokens.ASTERISK, "\\*");
            _tokens.Add(Tokens.SLASH, "\\/");
            _tokens.Add(Tokens.PLUS, "\\+");
            _tokens.Add(Tokens.COMMA, ",");
            _tokens.Add(Tokens.MINUS, "\\-");
        }

        public string InputString
        {
            set
            {
                _inputString = value;
                PrepareRegex();
            }
        }

        private void PrepareRegex()
        {
            _regExMatchCollection.Clear();
            foreach (var current in _tokens)
            {
                _regExMatchCollection.Add(current.Key, Regex.Matches(_inputString, current.Value));
            }
        }

        public void ResetParser()
        {
            _index = 0;
            _inputString = string.Empty;
            _regExMatchCollection.Clear();
        }

        public Token GetToken()
        {
            Token result;
            if (_index >= _inputString.Length)
            {
                result = null;
            }
            else
            {
                foreach (var current in _regExMatchCollection)
                {
                    foreach (Match match in current.Value)
                    {
                        if (match.Index == _index)
                        {
                            _index += match.Length;
                            result = new Token(current.Key, match.Value);
                            return result;
                        }
                        if (match.Index > _index)
                        {
                            break;
                        }
                    }
                }
                _index++;
                result = new Token(Tokens.UNDEFINED, string.Empty);
            }
            return result;
        }

        public PeekToken Peek() => Peek(new PeekToken(_index, new Token(Tokens.UNDEFINED, string.Empty)));

        public PeekToken Peek(PeekToken peekToken)
        {
            var index = _index;
            _index = peekToken.TokenIndex;
            PeekToken result;
            if (_index >= _inputString.Length)
            {
                _index = index;
                result = null;
            }
            else
            {
                foreach (var current in _tokens)
                {
                    var regex = new Regex(current.Value);
                    var match = regex.Match(_inputString, _index);
                    if (match.Success && match.Index == _index)
                    {
                        _index += match.Length;
                        var peekToken2 = new PeekToken(_index, new Token(current.Key, match.Value));
                        _index = index;
                        result = peekToken2;
                        return result;
                    }
                }
                var peekToken3 = new PeekToken(_index + 1, new Token(Tokens.UNDEFINED, string.Empty));
                _index = index;
                result = peekToken3;
            }
            return result;
        }
    }
}