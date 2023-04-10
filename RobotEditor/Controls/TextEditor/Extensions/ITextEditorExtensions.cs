using System.Collections.Generic;

namespace RobotEditor.Controls.TextEditor.Extensions;

public interface ITextEditorExtensions
{
    IEnumerable<T> Get<T>();
    IEnumerable<T> Get<T>(string extension);
}