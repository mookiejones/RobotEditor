using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Snippets;
using RobotEditor.Controls.TextEditor.Folding;
using RobotEditor.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace RobotEditor.Languages;

public interface ILanguageClass
{
    DirectoryInfo RootPath { get; set; }
    string FileName { get; set; }

    MenuItem RobotMenuItems { get; set; }

    string Name { get; }

    string SnippetPath { get; }
    string Intellisense { get; }
    string SnippetFilePath { get; }
    string Filename { get; }
    string RawText { set; }
    Regex MethodRegex { get; }
    Regex StructRegex { get; }
    Regex FieldRegex { get; }
    Regex SignalRegex { get; }
    Regex EnumRegex { get; }
    Regex XYZRegex { get; }
    ReadOnlyObservableCollection<Snippet> Snippets { get; }
    AbstractFoldingStrategy FoldingStrategy { get; }
    string CommentChar { get; }
    IEnumerable<IVariable> Fields { get; set; }
    void GetRootDirectory(string path);
    string FoldTitle(FoldingSection section, TextDocument document);
}