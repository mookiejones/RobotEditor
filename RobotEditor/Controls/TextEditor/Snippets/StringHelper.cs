using System.Collections.Generic;
using System.Linq;

namespace RobotEditor.Controls.TextEditor.Snippets;

public static class StringHelper
{
    public static bool ContainsDeclaration(this string code, Dictionary<string, Declaration> declarations) => declarations.Values.Select(current => code.Contains(current.Id)).Any(flag => flag);

    public static string GetTheNextId(this string code, Dictionary<string, Declaration> declarations)
    {
        string result = null;
        int num = 2147483647;
        foreach (Declaration current in declarations.Values)
        {
            int num2 = code.IndexOf(current.Id, System.StringComparison.Ordinal);
            if (num2 == -1 || num2 >= num)
            {
                continue;
            }

            num = num2;
            result = current.Id;
        }
        return result;
    }
}