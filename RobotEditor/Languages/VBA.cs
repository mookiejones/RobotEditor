﻿using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using RobotEditor.Controls.TextEditor.Folding;
using RobotEditor.Controls.TextEditor.Snippets.CompletionData;
using RobotEditor.Enums;
using RobotEditor.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using FileInfo = System.IO.FileInfo;

namespace RobotEditor.Languages;

[Localizable(false)]
public sealed class VBA : AbstractLanguageClass
{
    public VBA(string file)
        : base(file)
    {
        FoldingStrategy = new RegionFoldingStrategy();
    }

    public override List<string> SearchFilters => new()
    {
                "*.*",
                "*.dat",
                "*.src",
                "*.ini",
                "*.sub",
                "*.zip",
                "*.kfd"
            };

    internal override Typlanguage RobotType => Typlanguage.VBA;

    internal override string SourceFile => string.Empty;

    internal override AbstractFoldingStrategy FoldingStrategy { get; set; }

    protected override string ShiftRegex => "((RobTarget\\s*[\\w]*\\s*:=\\s*\\[\\[)([\\d.-]*),([\\d.-]*),([-.\\d]*))";

    internal override string FunctionItems => "((?<!END)()()PROC\\s([\\d\\w]*)[\\(\\)\\w\\d_. ]*)";

    internal override IList<ICompletionData> CodeCompletion => new List<ICompletionData>
            {
                new CodeCompletion("Item1")
            };

    public override Regex MethodRegex => new("( sub )", RegexOptions.IgnoreCase);

    public override Regex StructRegex => new("( struc )", RegexOptions.IgnoreCase);

    public override Regex FieldRegex => new("( boolean )", RegexOptions.IgnoreCase);

    public override Regex EnumRegex => new("( enum )", RegexOptions.IgnoreCase);

    public override void Initialize(string filename) => base.Initialize();

    public override string CommentChar => "'";

    public override Regex SignalRegex => new(string.Empty);

    public override Regex XYZRegex => new(string.Empty);

    protected override bool IsFileValid(FileInfo file) => false;

    internal override string FoldTitle(FoldingSection section, TextDocument doc)
    {
        string[] array = Regex.Split(section.Title, "æ");
        int offset = section.StartOffset + array[0].Length;
        int length = section.Length - (array[0].Length + array[1].Length);
        return doc.GetText(offset, length);
    }

    public override string ExtractXYZ(string positionstring) => string.Empty;

    public override DocumentViewModel GetFile(string filepath) => new(filepath);

    private sealed class RegionFoldingStrategy : AbstractFoldingStrategy
    {
        private IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
        {
            List<NewFolding> list = new();
            list.AddRange(CreateFoldingHelper(document, "public function", "end function", true));
            list.AddRange(CreateFoldingHelper(document, "private function", "end function", true));
            list.AddRange(CreateFoldingHelper(document, "public sub", "end sub", true));
            list.AddRange(CreateFoldingHelper(document, "private sub", "end sub", true));
            list.AddRange(CreateFoldingHelper(document, "property", "end property", true));
            list.AddRange(CreateFoldingHelper(document, "If", "End If", false));
            list.AddRange(CreateFoldingHelper(document, "Select Case", "End Select", false));
            list.Sort((NewFolding a, NewFolding b) => a.StartOffset.CompareTo(b.StartOffset));
            return list;
        }

        protected override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }
    }
}