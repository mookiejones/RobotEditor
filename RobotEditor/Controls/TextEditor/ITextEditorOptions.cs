using System.ComponentModel;

namespace RobotEditor.Interfaces;

public interface ITextEditorOptions : INotifyPropertyChanged
{
    string IndentationString { get; }
    bool AutoInsertBlockEnd { get; }
    bool ConvertTabsToSpaces { get; }
    int IndentationSize { get; }
    int VerticalRulerColumn { get; }
    bool UnderlineErrors { get; }
    string FontFamily { get; }
}