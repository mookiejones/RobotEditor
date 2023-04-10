using ICSharpCode.AvalonEdit.CodeCompletion;
using RobotEditor.Controls.TextEditor.Snippets.CompletionData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace RobotEditor.Controls.TextEditor.Snippets;

public sealed class SnippetCompletionDataProvider : ICompletionDataProvider, IDisposable
{
    public IEnumerable<ICompletionData> ProvideData(CompletionContextInfo context)
    {


        if (context != null && context.Path != null && context.CompletionType != CompletionType.CompletionKey && context.CompletionType != CompletionType.ScopeChange)
        {
            string extension = Path.GetExtension(context.Path);
            if (extension != null && (extension.Equals(".src", StringComparison.InvariantCultureIgnoreCase) || extension.Equals(".dat", StringComparison.InvariantCultureIgnoreCase) || extension.Equals(".sub", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (context.StringBeforeCaret.Contains(";"))
                {
                    goto IL_170;
                }
                int num = context.StringBeforeCaret.Count(c => c == '"');
                if (num % 2 == 1)
                {
                    goto IL_170;
                }
            }
            foreach (ICompletionData current in SnippetManager.GetCompletionDataForExtension(extension))
            {
                yield return current;
            }
        }
    IL_170:
        yield break;
    }
    public void Dispose()
    {
    }


}