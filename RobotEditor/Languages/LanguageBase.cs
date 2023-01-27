using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using RobotEditor.Controls.TextEditor.Folding;
using RobotEditor.Controls.TextEditor.Snippets.CompletionData;
using RobotEditor.Enums;
using RobotEditor.Languages.Data;
using RobotEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using FileInfo = System.IO.FileInfo;

namespace RobotEditor.Languages
{
    [Localizable(false)]
    public sealed class LanguageBase : AbstractLanguageClass
    {
        public LanguageBase()
        {
        }

        public LanguageBase(string file)
            : base(file)
        {
        }

        public override List<string> SearchFilters => DefaultSearchFilters;

        private static List<string> DefaultSearchFilters => new List<string>
                {
                    "*.*"
                };

        internal override Typlanguage RobotType => Typlanguage.None;

        internal override string FunctionItems => string.Empty;

        internal override AbstractFoldingStrategy FoldingStrategy { get; set; }

        protected override string ShiftRegex => throw new NotImplementedException();

        internal override string SourceFile => throw new NotImplementedException();

        internal override IList<ICompletionData> CodeCompletion => new List<ICompletionData>
                {
                    new CodeCompletion("Item1")
                };

        public override Regex MethodRegex => new Regex(string.Empty);

        public override Regex StructRegex => new Regex(string.Empty);

        public override Regex FieldRegex => new Regex(string.Empty);

        public override Regex EnumRegex => new Regex(string.Empty);

        public override void Initialize(string filename) => Initialize();

        public override string CommentChar => throw new NotImplementedException();

        public override Regex SignalRegex => new Regex(string.Empty);

        public override Regex XYZRegex => new Regex(string.Empty);

        protected override bool IsFileValid(FileInfo file) => false;

        public override DocumentViewModel GetFile(string filename) => new DocumentViewModel(filename);

        internal override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }
            string[] array = Regex.Split(section.Title, "�");
            int offset = section.StartOffset + array[0].Length;
            int length = section.Length - array[0].Length;
            return doc.GetText(offset, length);
        }

        public override string ExtractXYZ(string positionstring)
        {
            PositionBase positionBase = new PositionBase(positionstring);
            return positionBase.ExtractFromMatch();
        }
    }
}