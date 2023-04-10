using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;

namespace RobotEditor.Controls.TextEditor.Snippets.CompletionData;

public interface ICompletionDataProvider : IDisposable
{
    IEnumerable<ICompletionData> ProvideData(CompletionContextInfo context);
}