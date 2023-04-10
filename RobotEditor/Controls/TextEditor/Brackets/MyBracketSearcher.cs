using System;
using System.Collections.Generic;
using System.Diagnostics;
using ICSharpCode.AvalonEdit.Document;

namespace RobotEditor.Controls.TextEditor.Brackets;

public sealed class MyBracketSearcher : IBracketSearcher
{
    public BracketSearchResult SearchBracket(TextDocument document, int offset)
    {
        BracketSearchResult result;
        if (offset > 0)
        {
            var charAt = document.GetCharAt(offset - 1);
            var num = "([{".IndexOf(charAt);
            var num2 = -1;
            if (num > -1)
            {
                num2 = SearchBracketForward(document, offset, "([{"[num], ")]}"[num]);
            }
            num = ")]}".IndexOf(charAt);
            if (num > -1)
            {
                num2 = SearchBracketBackward(document, offset - 2, "([{"[num], ")]}"[num]);
            }
            if (num2 > -1)
            {
                result = new BracketSearchResult(Math.Min(offset - 1, num2), 1, Math.Max(offset - 1, num2), 1);
                return result;
            }
        }
        result = null;
        return result;
    }

    private static int ScanLineStart(ITextSource document, int offset)
    {
        int result;
        for (var i = offset - 1; i > 0; i--)
        {
            if (document.GetCharAt(i) == '\n')
            {
                result = i + 1;
                return result;
            }
        }
        result = 0;
        return result;
    }

    private static int GetStartType(ITextSource document, int linestart, int offset)
    {
        var flag = false;
        var flag2 = false;
        var flag3 = false;
        var num = 0;
        for (var i = linestart; i < offset; i++)
        {
            var charAt = document.GetCharAt(i);
            if (charAt <= '\'')
            {
                if (charAt != '"')
                {
                    if (charAt == '\'')
                    {
                        if (!flag)
                        {
                            flag2 = !flag2;
                        }
                    }
                }
                else
                {
                    if (!flag2)
                    {
                        if (flag && flag3)
                        {
                            if (i + 1 < document.TextLength && document.GetCharAt(i + 1) == '"')
                            {
                                i++;
                                flag = false;
                            }
                            else
                            {
                                flag3 = false;
                            }
                        }
                        else
                        {
                            if (!flag && i > 0 && document.GetCharAt(i - 1) == '@')
                            {
                                flag3 = true;
                            }
                        }
                        flag = !flag;
                    }
                }
            }
            else
            {
                if (charAt != '/')
                {
                    if (charAt == '\\')
                    {
                        if (flag && !flag3 || flag2)
                        {
                            i++;
                        }
                    }
                }
                else
                {
                    if (!flag && !flag2 && i + 1 < document.TextLength)
                    {
                        if (document.GetCharAt(i + 1) == '/')
                        {
                            num = 1;
                        }
                    }
                }
            }
        }
        return flag || flag2 ? 2 : num;
    }

    private static int SearchBracketBackward(ITextSource document, int offset, char openBracket, char closingBracket)
    {
        int result;
        if (offset + 1 >= document.TextLength)
        {
            result = -1;
        }
        else
        {
            var num = QuickSearchBracketBackward(document, offset, openBracket, closingBracket);
            if (num >= 0)
            {
                result = num;
            }
            else
            {
                var linestart = ScanLineStart(document, offset + 1);
                var startType = GetStartType(document, linestart, offset + 1);
                if (startType == 1)
                {
                    result = -1;
                }
                else
                {
                    var stack = new Stack<int>();
                    var flag = false;
                    var flag2 = false;
                    var flag3 = false;
                    var flag4 = false;
                    var flag5 = false;
                    var i = 0;
                    while (i <= offset)
                    {
                        var charAt = document.GetCharAt(i);
                        var c = charAt;
                        if (c <= '"')
                        {
                            if (c != '\n' && c != '\r')
                            {
                                if (c != '"')
                                {
                                    goto IL_262;
                                }
                                if (!flag3 && !flag2 && !flag)
                                {
                                    if (flag4 && flag5)
                                    {
                                        if (i + 1 < document.TextLength && document.GetCharAt(i + 1) == '"')
                                        {
                                            i++;
                                            flag4 = false;
                                        }
                                        else
                                        {
                                            flag5 = false;
                                        }
                                    }
                                    else
                                    {
                                        if (!flag4 && offset > 0 && document.GetCharAt(i - 1) == '@')
                                        {
                                            flag5 = true;
                                        }
                                    }
                                    flag4 = !flag4;
                                }
                            }
                            else
                            {
                                flag2 = false;
                                flag3 = false;
                                if (!flag5)
                                {
                                    flag4 = false;
                                }
                            }
                        }
                        else
                        {
                            if (c != '\'')
                            {
                                if (c != '/')
                                {
                                    if (c != '\\')
                                    {
                                        goto IL_262;
                                    }
                                    if (flag4 && !flag5 || flag3)
                                    {
                                        i++;
                                    }
                                }
                                else
                                {
                                    if (flag)
                                    {
                                        Debug.Assert(i > 0);
                                        if (document.GetCharAt(i - 1) == '*')
                                        {
                                            flag = false;
                                        }
                                    }
                                    if (!flag4 && !flag3 && i + 1 < document.TextLength)
                                    {
                                        if (!flag && document.GetCharAt(i + 1) == '/')
                                        {
                                            flag2 = true;
                                        }
                                        if (!flag2 && document.GetCharAt(i + 1) == '*')
                                        {
                                            flag = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!flag4 && !flag2 && !flag)
                                {
                                    flag3 = !flag3;
                                }
                            }
                        }
                    IL_2DC:
                        i++;
                        continue;
                    IL_262:
                        if (charAt == openBracket)
                        {
                            if (!flag4 && !flag3 && !flag2 && !flag)
                            {
                                stack.Push(i);
                            }
                        }
                        else
                        {
                            if (charAt == closingBracket)
                            {
                                if (!flag4 && !flag3 && !flag2 && !flag)
                                {
                                    if (stack.Count > 0)
                                    {
                                        stack.Pop();
                                    }
                                }
                            }
                        }
                        goto IL_2DC;
                    }
                    if (stack.Count > 0)
                    {
                        result = stack.Pop();
                    }
                    else
                    {
                        result = -1;
                    }
                }
            }
        }
        return result;
    }

    private static int SearchBracketForward(ITextSource document, int offset, char openBracket, char closingBracket)
    {
        var flag = false;
        var flag2 = false;
        var flag3 = false;
        var flag4 = false;
        var flag5 = false;
        int result;
        if (offset < 0)
        {
            result = -1;
        }
        else
        {
            var num = QuickSearchBracketForward(document, offset, openBracket, closingBracket);
            if (num >= 0)
            {
                result = num;
            }
            else
            {
                var linestart = ScanLineStart(document, offset);
                var startType = GetStartType(document, linestart, offset);
                if (startType != 0)
                {
                    result = -1;
                }
                else
                {
                    var num2 = 1;
                    while (offset < document.TextLength)
                    {
                        var charAt = document.GetCharAt(offset);
                        var c = charAt;
                        if (c <= '"')
                        {
                            if (c != '\n' && c != '\r')
                            {
                                if (c != '"')
                                {
                                    goto IL_22C;
                                }
                                if (!flag2 && !flag4 && !flag5)
                                {
                                    if (flag && flag3)
                                    {
                                        if (offset + 1 < document.TextLength &&
                                            document.GetCharAt(offset + 1) == '"')
                                        {
                                            offset++;
                                            flag = false;
                                        }
                                        else
                                        {
                                            flag3 = false;
                                        }
                                    }
                                    else
                                    {
                                        if (!flag && offset > 0 && document.GetCharAt(offset - 1) == '@')
                                        {
                                            flag3 = true;
                                        }
                                    }
                                    flag = !flag;
                                }
                            }
                            else
                            {
                                flag4 = false;
                                flag2 = false;
                                if (!flag3)
                                {
                                    flag = false;
                                }
                            }
                        }
                        else
                        {
                            if (c != '\'')
                            {
                                if (c != '/')
                                {
                                    if (c != '\\')
                                    {
                                        goto IL_22C;
                                    }
                                    if (flag && !flag3 || flag2)
                                    {
                                        offset++;
                                    }
                                }
                                else
                                {
                                    if (flag5)
                                    {
                                        Debug.Assert(offset > 0);
                                        if (document.GetCharAt(offset - 1) == '*')
                                        {
                                            flag5 = false;
                                        }
                                    }
                                    if (!flag && !flag2 && offset + 1 < document.TextLength)
                                    {
                                        if (!flag5 && document.GetCharAt(offset + 1) == '/')
                                        {
                                            flag4 = true;
                                        }
                                        if (!flag4 && document.GetCharAt(offset + 1) == '*')
                                        {
                                            flag5 = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!flag && !flag4 && !flag5)
                                {
                                    flag2 = !flag2;
                                }
                            }
                        }
                    IL_29E:
                        offset++;
                        continue;
                    IL_22C:
                        if (charAt == openBracket)
                        {
                            if (!flag && !flag2 && !flag4 && !flag5)
                            {
                                num2++;
                            }
                        }
                        else
                        {
                            if (charAt == closingBracket)
                            {
                                if (!flag && !flag2 && !flag4 && !flag5)
                                {
                                    num2--;
                                    if (num2 == 0)
                                    {
                                        result = offset;
                                        return result;
                                    }
                                }
                            }
                        }
                        goto IL_29E;
                    }
                    result = -1;
                }
            }
        }
        return result;
    }

    private static int QuickSearchBracketBackward(ITextSource document, int offset, char openBracket,
        char closingBracket)
    {
        var num = -1;
        int result;
        for (var i = offset; i >= 0; i--)
        {
            var charAt = document.GetCharAt(i);
            if (charAt == openBracket)
            {
                num++;
                if (num == 0)
                {
                    result = i;
                    return result;
                }
            }
            else
            {
                if (charAt == closingBracket)
                {
                    num--;
                }
                else
                {
                    if (charAt == '"')
                    {
                        break;
                    }
                    if (charAt == '\'')
                    {
                        break;
                    }
                    if (charAt == '/' && i > 0)
                    {
                        if (document.GetCharAt(i - 1) == '/')
                        {
                            break;
                        }
                        if (document.GetCharAt(i - 1) == '*')
                        {
                            break;
                        }
                    }
                }
            }
        }
        result = -1;
        return result;
    }

    private static int QuickSearchBracketForward(ITextSource document, int offset, char openBracket,
        char closingBracket)
    {
        var num = 1;
        int result;
        for (var i = offset; i < document.TextLength; i++)
        {
            var charAt = document.GetCharAt(i);
            if (charAt == openBracket)
            {
                num++;
            }
            else
            {
                if (charAt == closingBracket)
                {
                    num--;
                    if (num == 0)
                    {
                        result = i;
                        return result;
                    }
                }
                else
                {
                    if (charAt == '"')
                    {
                        break;
                    }
                    if (charAt == '\'')
                    {
                        break;
                    }
                    if (charAt == '/' && i > 0)
                    {
                        if (document.GetCharAt(i - 1) == '/')
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (charAt == '*' && i > 0)
                        {
                            if (document.GetCharAt(i - 1) == '/')
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
        result = -1;
        return result;
    }
}