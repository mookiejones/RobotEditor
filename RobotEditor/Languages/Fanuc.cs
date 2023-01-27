using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using RobotEditor.Controls.TextEditor.Folding;
using RobotEditor.Controls.TextEditor.Snippets.CompletionData;
using RobotEditor.Enums;
using RobotEditor.Languages.Data;
using RobotEditor.ViewModel;
using FileInfo = System.IO.FileInfo;

namespace RobotEditor.Languages
{
    [Localizable(false)]
    public sealed class Fanuc : AbstractLanguageClass
    {
        public Fanuc(string file)
            : base(file)
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        public Fanuc()
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        public override List<string> SearchFilters => EXT;

        public static List<string> EXT => new List<string>
                {
                    ".ls"
                };

        internal override Typlanguage RobotType => Typlanguage.Fanuc;

        internal override IList<ICompletionData> CodeCompletion => new List<ICompletionData>
                {
                    new CodeCompletion("Item1")
                };

        protected override string ShiftRegex
        {
            get { throw new NotImplementedException(); }
        }

        internal override string SourceFile
        {
            get { throw new NotImplementedException(); }
        }

        internal override string FunctionItems => "(\\.Program [\\d\\w]*[\\(\\)\\w\\d_.]*)";

        internal override AbstractFoldingStrategy FoldingStrategy { get; set; }

        public override Regex MethodRegex => new Regex(string.Empty);

        public override Regex StructRegex => new Regex(string.Empty);

        public override Regex FieldRegex => new Regex(string.Empty);

        public override Regex EnumRegex => new Regex(string.Empty);

        public override void Initialize(string filename) => Initialize();

        [Localizable(false)]
        public override string CommentChar => "!";

        public override Regex SignalRegex => new Regex(string.Empty);

        public override Regex XYZRegex => new Regex(string.Empty);

        protected override bool IsFileValid(FileInfo file) => EXT.Any((string e) => file.Extension.ToLower() == e);

        internal override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            var array = Regex.Split(section.Title, "�");
            var offset = section.StartOffset + array[0].Length;
            var length = section.Length - (array[0].Length + array[1].Length);
            return doc.GetText(offset, length);
        }

        public override string CommentReplaceString(string text)
        {
            Regex regex = null;
            if (Regex.IsMatch(text, "^[\\s]*\\d*:\\s*!"))
            {
                regex = new Regex("^([\\s\\d]*:\\s*)!([^\\r\\n]*)");
            }
            if (Regex.IsMatch(text, "^[\\s]*!"))
            {
                regex = new Regex("^([\\s]*)!([^\\r\\n]*)");
            }
            string result;
            if (regex != null)
            {
                var match = regex.Match(text);
                if (match.Success)
                {
                    result = match.Groups[1] + match.Groups[2].ToString();
                    return result;
                }
            }
            result = text;
            return result;
        }

        public override int CommentOffset(string text)
        {
            var regex = new Regex("(^[\\s\\d:]+)");
            var match = regex.Match(text);
            var result = match.Success ? match.Groups[1].Length : 0;
            return result;
        }

        public override bool IsLineCommented(string text)
        {
            var regex = new Regex("^[\\s]*\\d*:\\s*!");
            var regex2 = new Regex("^[\\s]*!");
            return regex.IsMatch(text) || regex2.IsMatch(text);
        }

        public override string ExtractXYZ(string positionstring)
        {
            Debugger.Break();
            var positionBase = new PositionBase(positionstring);
            return positionBase.ExtractFromMatch();
        }

        public override DocumentViewModel GetFile(string filepath) => new DocumentViewModel(filepath);

        private sealed class RegionFoldingStrategy : AbstractFoldingStrategy
        {
            protected override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
            {
                firstErrorOffset = -1;
                return CreateNewFoldings(document);
            }

            public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
            {
                var list = new List<NewFolding>();
                list.AddRange(CreateFoldingHelper(document, ";fold", ";endfold", true));
                list.Sort((NewFolding a, NewFolding b) => a.StartOffset.CompareTo(b.StartOffset));
                return list;
            }
        }
    }
}