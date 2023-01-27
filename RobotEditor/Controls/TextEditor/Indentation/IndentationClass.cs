using ICSharpCode.AvalonEdit.Indentation.CSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RobotEditor.Controls.TextEditor.Indentation
{
    internal sealed class IndentationClass
    {
        private Block _block;
        private bool _blockComment;
        private Stack<Block> _blocks;
        private bool _escape;
        private bool _inChar;
        private bool _inString;
        private char _lastRealChar;
        private bool _lineComment;
        private bool _verbatim;
        private StringBuilder _wordBuilder;

        public void Reformat(IDocumentAccessor doc, IndentationSettings set)
        {
            Init();
            while (doc.MoveNext())
            {
                Step(doc, set);
            }
        }

        public void Init()
        {
            _wordBuilder = new StringBuilder();
            _blocks = new Stack<Block>();
            _block = new Block
            {
                InnerIndent = "",
                OuterIndent = "",
                Bracket = '{',
                Continuation = false,
                LastWord = "",
                OneLineBlock = 0,
                PreviousOneLineBlock = 0,
                StartLine = 0
            };
            _inString = false;
            _inChar = false;
            _verbatim = false;
            _escape = false;
            _lineComment = false;
            _blockComment = false;
            _lastRealChar = ' ';
        }

        [Localizable(false)]
        public void Step(IDocumentAccessor doc, IndentationSettings set)
        {
            string text = doc.Text;
            if (!set.LeaveEmptyLines || text.Length != 0)
            {
                text = text.TrimStart(new char[0]);
                StringBuilder stringBuilder = new StringBuilder();
                if (text.Length == 0)
                {
                    if (!_blockComment && (!_inString || !_verbatim))
                    {
                        _ = stringBuilder.Append(_block.InnerIndent);
                        _ = stringBuilder.Append(Repeat(set.IndentString, _block.OneLineBlock));
                        if (_block.Continuation)
                        {
                            _ = stringBuilder.Append(set.IndentString);
                        }
                        if (doc.Text != null && doc.Text != stringBuilder.ToString())
                        {
                            doc.Text = stringBuilder.ToString();
                        }
                    }
                }
                else
                {
                    if (TrimEnd(doc))
                    {
                        text = doc.Text.TrimStart(new char[0]);
                    }
                    Block block = _block;
                    bool blockComment = _blockComment;
                    bool flag = _inString && _verbatim;
                    _lineComment = false;
                    _inChar = false;
                    _escape = false;
                    if (!_verbatim)
                    {
                        _inString = false;
                    }
                    _lastRealChar = '\n';
                    char c = ' ';
                    char c2 = text[0];
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (_lineComment)
                        {
                            break;
                        }
                        char c3 = c;
                        c = c2;
                        c2 = i + 1 < text.Length ? text[i + 1] : '\n';
                        if (_escape)
                        {
                            _escape = false;
                        }
                        else
                        {
                            char c4 = c;
                            if (c4 <= '\'')
                            {
                                switch (c4)
                                {
                                    case '"':
                                        if (!_inChar && !_lineComment && !_blockComment)
                                        {
                                            _inString = !_inString;
                                            if (!_inString && _verbatim)
                                            {
                                                if (c2 == '"')
                                                {
                                                    _escape = true;
                                                    _inString = true;
                                                }
                                                else
                                                {
                                                    _verbatim = false;
                                                }
                                            }
                                            else
                                            {
                                                if (_inString && c3 == '@')
                                                {
                                                    _verbatim = true;
                                                }
                                            }
                                        }
                                        break;
                                    case '#':
                                        if (!_inChar && !_blockComment && !_inString)
                                        {
                                            _lineComment = true;
                                        }
                                        break;
                                    default:
                                        if (c4 == '\'')
                                        {
                                            if (!_inString && !_lineComment && !_blockComment)
                                            {
                                                _inChar = !_inChar;
                                            }
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                if (c4 != '/')
                                {
                                    if (c4 == '\\')
                                    {
                                        if ((_inString && !_verbatim) || _inChar)
                                        {
                                            _escape = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (_blockComment && c3 == '*')
                                    {
                                        _blockComment = false;
                                    }
                                    if (!_inString && !_inChar)
                                    {
                                        if (!_blockComment && c2 == '/')
                                        {
                                            _lineComment = true;
                                        }
                                        if (!_lineComment && c2 == '*')
                                        {
                                            _blockComment = true;
                                        }
                                    }
                                }
                            }
                            if (!_lineComment && !_blockComment && !_inString && !_inChar)
                            {
                                if (!char.IsWhiteSpace(c) && c != '[' && c != '/')
                                {
                                    if (_block.Bracket == '{')
                                    {
                                        _block.Continuation = true;
                                    }
                                }
                                if (char.IsLetterOrDigit(c))
                                {
                                    _ = _wordBuilder.Append(c);
                                }
                                else
                                {
                                    if (_wordBuilder.Length > 0)
                                    {
                                        _block.LastWord = _wordBuilder.ToString();
                                    }
                                    _wordBuilder.Length = 0;
                                }
                                c4 = c;
                                if (c4 <= ';')
                                {
                                    switch (c4)
                                    {
                                        case '(':
                                            goto IL_671;
                                        case ')':
                                            if (_blocks.Count == 0)
                                            {
                                                goto IL_873;
                                            }
                                            if (_block.Bracket == '(')
                                            {
                                                _block = _blocks.Pop();
                                                if (IsSingleStatementKeyword(_block.LastWord))
                                                {
                                                    _block.Continuation = false;
                                                }
                                            }
                                            goto IL_873;
                                        case '*':
                                        case '+':
                                            goto IL_873;
                                        case ',':
                                            break;
                                        default:
                                            switch (c4)
                                            {
                                                case ':':
                                                    if (_block.LastWord == "case" ||
                                                        text.StartsWith("case ", StringComparison.Ordinal) ||
                                                        text.StartsWith(_block.LastWord + ":", StringComparison.Ordinal))
                                                    {
                                                        _block.Continuation = false;
                                                        _block.ResetOneLineBlock();
                                                    }
                                                    goto IL_873;
                                                case ';':
                                                    break;
                                                default:
                                                    goto IL_873;
                                            }
                                            break;
                                    }
                                    _block.Continuation = false;
                                    _block.ResetOneLineBlock();
                                }
                                else
                                {
                                    switch (c4)
                                    {
                                        case '[':
                                            goto IL_671;
                                        case '\\':
                                            break;
                                        case ']':
                                            if (_blocks.Count != 0)
                                            {
                                                if (_block.Bracket == '[')
                                                {
                                                    _block = _blocks.Pop();
                                                }
                                            }
                                            break;
                                        default:
                                            switch (c4)
                                            {
                                                case '{':
                                                    _block.ResetOneLineBlock();
                                                    _blocks.Push(_block);
                                                    _block.StartLine = doc.LineNumber;
                                                    if (_block.LastWord == "switch")
                                                    {
                                                        _block.Indent(set.IndentString + set.IndentString);
                                                    }
                                                    else
                                                    {
                                                        _block.Indent(set);
                                                    }
                                                    _block.Bracket = '{';
                                                    break;
                                                case '}':
                                                    while (_block.Bracket != '{')
                                                    {
                                                        if (_blocks.Count == 0)
                                                        {
                                                            break;
                                                        }
                                                        _block = _blocks.Pop();
                                                    }
                                                    if (_blocks.Count != 0)
                                                    {
                                                        _block = _blocks.Pop();
                                                        _block.Continuation = false;
                                                        _block.ResetOneLineBlock();
                                                    }
                                                    break;
                                            }
                                            break;
                                    }
                                }
                            IL_873:
                                if (!char.IsWhiteSpace(c))
                                {
                                    _lastRealChar = c;
                                }
                                goto IL_88B;
                            IL_671:
                                _blocks.Push(_block);
                                if (_block.StartLine == doc.LineNumber)
                                {
                                    _block.InnerIndent = _block.OuterIndent;
                                }
                                else
                                {
                                    _block.StartLine = doc.LineNumber;
                                }
                                _block.Indent(Repeat(set.IndentString, block.OneLineBlock) +
                                              (block.Continuation ? set.IndentString : "") +
                                              (i == text.Length - 1 ? set.IndentString : new string(' ', i + 1)));
                                _block.Bracket = c;
                                goto IL_873;
                            }
                            if (_wordBuilder.Length > 0)
                            {
                                _block.LastWord = _wordBuilder.ToString();
                            }
                            _wordBuilder.Length = 0;
                        }
                    IL_88B:
                        ;
                    }
                    if (_wordBuilder.Length > 0)
                    {
                        _block.LastWord = _wordBuilder.ToString();
                    }
                    _wordBuilder.Length = 0;
                    if (!flag)
                    {
                        if (!blockComment || text[0] == '*')
                        {
                            if (!doc.Text.StartsWith("//\t", StringComparison.Ordinal) && !(doc.Text == "//"))
                            {
                                if (text[0] == '}')
                                {
                                    _ = stringBuilder.Append(block.OuterIndent);
                                    block.ResetOneLineBlock();
                                    block.Continuation = false;
                                }
                                else
                                {
                                    _ = stringBuilder.Append(block.InnerIndent);
                                }
                                if (stringBuilder.Length > 0 && block.Bracket == '(' && text[0] == ')')
                                {
                                    _ = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                                }
                                else
                                {
                                    if (stringBuilder.Length > 0 && block.Bracket == '[' && text[0] == ']')
                                    {
                                        _ = stringBuilder.Remove(stringBuilder.Length - 1, 1);
                                    }
                                }
                                if (text[0] == ':')
                                {
                                    block.Continuation = true;
                                }
                                else
                                {
                                    if (_lastRealChar == ':' && stringBuilder.Length >= set.IndentString.Length)
                                    {
                                        if (_block.LastWord == "case" ||
                                            text.StartsWith("case ", StringComparison.Ordinal) ||
                                            text.StartsWith(_block.LastWord + ":", StringComparison.Ordinal))
                                        {
                                            _ = stringBuilder.Remove(stringBuilder.Length - set.IndentString.Length,
                                                set.IndentString.Length);
                                        }
                                    }
                                    else
                                    {
                                        if (_lastRealChar == ')')
                                        {
                                            if (IsSingleStatementKeyword(_block.LastWord))
                                            {
                                                _block.OneLineBlock++;
                                            }
                                        }
                                        else
                                        {
                                            if (_lastRealChar == 'e' && _block.LastWord == "else")
                                            {
                                                _block.OneLineBlock = Math.Max(1, _block.PreviousOneLineBlock);
                                                _block.Continuation = false;
                                                block.OneLineBlock = _block.OneLineBlock - 1;
                                            }
                                        }
                                    }
                                }
                                if (doc.IsReadOnly)
                                {
                                    if (!block.Continuation && block.OneLineBlock == 0 &&
                                        block.StartLine == _block.StartLine && _block.StartLine < doc.LineNumber &&
                                        _lastRealChar != ':')
                                    {
                                        stringBuilder.Length = 0;
                                        text = doc.Text;
                                        foreach (char current in text.TakeWhile(char.IsWhiteSpace))
                                        {
                                            _ = stringBuilder.Append(current);
                                        }
                                        if (blockComment && stringBuilder.Length > 0 &&
                                            stringBuilder[stringBuilder.Length - 1] == ' ')
                                        {
                                            stringBuilder.Length--;
                                        }
                                        _block.InnerIndent = stringBuilder.ToString();
                                    }
                                }
                                else
                                {
                                    if (text[0] != '{')
                                    {
                                        if (text[0] != ')' && block.Continuation && block.Bracket == '{')
                                        {
                                            _ = stringBuilder.Append(set.IndentString);
                                        }
                                        _ = stringBuilder.Append(Repeat(set.IndentString, block.OneLineBlock));
                                    }
                                    if (blockComment)
                                    {
                                        _ = stringBuilder.Append(' ');
                                    }
                                    if (stringBuilder.Length != doc.Text.Length - text.Length ||
                                        !doc.Text.StartsWith(stringBuilder.ToString(), StringComparison.Ordinal) ||
                                        char.IsWhiteSpace(doc.Text[stringBuilder.Length]))
                                    {
                                        doc.Text = stringBuilder + text;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string Repeat(string text, int count)
        {
            string result;
            if (count == 0)
            {
                result = string.Empty;
            }
            else
            {
                if (count == 1)
                {
                    result = text;
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder(text.Length * count);
                    for (int i = 0; i < count; i++)
                    {
                        _ = stringBuilder.Append(text);
                    }
                    result = stringBuilder.ToString();
                }
            }
            return result;
        }

        private static bool IsSingleStatementKeyword(string keyword)
        {
            bool result;
            switch (keyword)
            {
                case "if":
                case "for":
                case "while":
                case "do":
                case "foreach":
                case "using":
                case "lock":
                    result = true;
                    return result;
            }
            result = false;
            return result;
        }

        [Localizable(false)]
        private static bool TrimEnd(IDocumentAccessor doc)
        {
            string text = doc.Text;
            bool result;
            if (!char.IsWhiteSpace(text[text.Length - 1]))
            {
                result = false;
            }
            else
            {
                if (text.EndsWith("// ", StringComparison.Ordinal) || text.EndsWith("* ", StringComparison.Ordinal))
                {
                    result = false;
                }
                else
                {
                    doc.Text = text.TrimEnd(new char[0]);
                    result = true;
                }
            }
            return result;
        }

        private struct Block
        {
            public char Bracket;
            public bool Continuation;
            public string InnerIndent;
            [Localizable(false)] public string LastWord;
            public int OneLineBlock;
            public string OuterIndent;
            public int PreviousOneLineBlock;
            public int StartLine;

            public void ResetOneLineBlock()
            {
                PreviousOneLineBlock = OneLineBlock;
                OneLineBlock = 0;
            }

            public void Indent(IndentationSettings set)
            {
                Indent(set.IndentString);
            }

            public void Indent(string indentationString)
            {
                OuterIndent = InnerIndent;
                InnerIndent += indentationString;
                Continuation = false;
                ResetOneLineBlock();
                LastWord = "";
            }

            [Localizable(false)]
            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "[Block StartLine={0}, LastWord='{1}', Continuation={2}, OneLineBlock={3}, PreviousOneLineBlock={4}]",
                    new object[]
                    {
                        StartLine,
                        LastWord,
                        Continuation,
                        OneLineBlock,
                        PreviousOneLineBlock
                    });
            }
        }
    }
}