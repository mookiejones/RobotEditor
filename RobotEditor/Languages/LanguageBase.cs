using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using RobotEditor.Abstract;
using RobotEditor.Classes;
using RobotEditor.Enums;
using RobotEditor.ViewModel;
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

        protected override string ShiftRegex
        {
            get { throw new NotImplementedException(); }
        }

        internal override string SourceFile
        {
            get { throw new NotImplementedException(); }
        }

        internal override IList<ICompletionData> CodeCompletion => new List<ICompletionData>
                {
                    new CodeCompletion("Item1")
                };

        public override Regex MethodRegex => new Regex(string.Empty);

        public override Regex StructRegex => new Regex(string.Empty);

        public override Regex FieldRegex => new Regex(string.Empty);

        public override Regex EnumRegex => new Regex(string.Empty);

        public override void Initialize(string filename) => Initialize();

        public override string CommentChar
        {
            get { throw new NotImplementedException(); }
        }

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
            var array = Regex.Split(section.Title, "�");
            var offset = section.StartOffset + array[0].Length;
            var length = section.Length - array[0].Length;
            return doc.GetText(offset, length);
        }

        public override string ExtractXYZ(string positionstring)
        {
            var positionBase = new PositionBase(positionstring);
            return positionBase.ExtractFromMatch();
        }
    }
}