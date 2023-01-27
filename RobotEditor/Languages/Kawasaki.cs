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
using System.Linq;
using System.Text.RegularExpressions;
using FileInfo = System.IO.FileInfo;

namespace RobotEditor.Languages
{
    [Localizable(false)]
    public sealed class Kawasaki : AbstractLanguageClass
    {
        private const RegexOptions Ro = RegexOptions.IgnoreCase | RegexOptions.Multiline;

        public Kawasaki(string file)
            : base(file)
        {
            Filename = file;
            FoldingStrategy = new RegionFoldingStrategy();
        }

        public Kawasaki()
        {
            FoldingStrategy = new RegionFoldingStrategy();
        }

        public override List<string> SearchFilters => EXT;

        public static List<string> EXT => new List<string>
                {
                    ".as",
                    ".prg"
                };

        internal override Typlanguage RobotType => Typlanguage.KAWASAKI;

        internal override IList<ICompletionData> CodeCompletion => new List<ICompletionData>
                {
                    new CodeCompletion("Item1")
                };

        protected override string ShiftRegex => throw new NotImplementedException();

        internal override string SourceFile => throw new NotImplementedException();

        internal override string FunctionItems => "(\\\\.Program [\\\\d\\\\w]*[\\\\(\\\\)\\\\w\\\\d_.]*)";

        internal override AbstractFoldingStrategy FoldingStrategy { get; set; }

        public override Regex MethodRegex => new Regex("(\\.Program [\\d\\w]*[\\(\\)\\w\\d_.]*)",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public override Regex StructRegex => new Regex("(ISKAWASAKI)(ISKAWASAKI)(ISKAWASAKI)",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public override Regex FieldRegex => new Regex("(ISKAWASAKI)(ISKAWASAKI)(ISKAWASAKI)",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public override Regex EnumRegex => new Regex("^ENUM ", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public override Regex XYZRegex => new Regex("^(LINEAR|JOINT) ([^#])*#\\[([^\\]]*)",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public override void Initialize(string filename)
        {
            Initialize();
        }

        public override string CommentChar => ";";

        public override Regex SignalRegex => new Regex(string.Empty);

        protected override bool IsFileValid(FileInfo file)
        {
            return EXT.Any((string e) => file.Extension.ToLower() == e);
        }

        internal override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            string[] array = Regex.Split(section.Title, "�");
            int startOffset = section.StartOffset;
            int length = section.Length - (array[0].Length + array[1].Length);
            return doc.GetText(startOffset, length);
        }

        public override string ExtractXYZ(string positionstring)
        {
            _ = new PositionBase(positionstring);
            return positionstring.Substring(positionstring.IndexOf("#[", StringComparison.Ordinal) + 2);
        }

        public override DocumentViewModel GetFile(string filepath)
        {
            return new DocumentViewModel(filepath);
        }

        private sealed class RegionFoldingStrategy : AbstractFoldingStrategy
        {
            public new void UpdateFoldings(FoldingManager manager, TextDocument document)
            {
                throw new NotImplementedException();
            }

            protected override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
            {
                firstErrorOffset = -1;
                return CreateNewFoldings(document);
            }

            private IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
            {
                List<NewFolding> list = new List<NewFolding>();
                list.AddRange(CreateFoldingHelper(document, ".program", ".end", false));
                list.AddRange(CreateFoldingHelper(document, ".robotdata1", ".end", false));
                list.AddRange(CreateFoldingHelper(document, ".ope_info1", ".end", false));
                list.AddRange(CreateFoldingHelper(document, ".sysdata", ".end", false));
                list.AddRange(CreateFoldingHelper(document, ".auxdata", ".end", false));
                list.AddRange(CreateFoldingHelper(document, ".awdata", ".end", false));
                list.AddRange(CreateFoldingHelper(document, ".inter_panel_d", ".end", false));
                list.AddRange(CreateFoldingHelper(document, ".inter_panel_color_d", ".end", false));
                list.AddRange(CreateFoldingHelper(document, ".sig_comment", ".end", false));
                list.AddRange(CreateFoldingHelper(document, ".trans", ".end", true));
                list.AddRange(CreateFoldingHelper(document, ".real", ".end", true));
                list.AddRange(CreateFoldingHelper(document, ".strings", ".end", true));
                list.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
                return list;
            }
        }
    }
}