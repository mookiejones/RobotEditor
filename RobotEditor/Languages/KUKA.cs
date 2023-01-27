using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Snippets;
using RobotEditor.Controls.TextEditor;
using RobotEditor.Controls.TextEditor.Folding;
using RobotEditor.Controls.TextEditor.Snippets;
using RobotEditor.Controls.TextEditor.Snippets.CompletionData;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Languages.Data;
using RobotEditor.Messages;
using RobotEditor.Utilities;
using RobotEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using MenuItem = System.Windows.Controls.MenuItem;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace RobotEditor.Languages
{
    public class KUKA : AbstractLanguageClass
    {

        private RelayCommand _systemFunctionCommand;

        public KUKA(string file)
            : base(file)
        {
            //TODO Trying out KUKAs folding Strategy
            FoldingStrategy = new RegionFoldingStrategy();
            //      FoldingStrategy = new KrlFoldingStrategy();
        }

        public override void Initialize(string filename)
        {
            base.Initialize();
        }

        public ICommand SystemFunctionCommand => _systemFunctionCommand ??
                       (_systemFunctionCommand = new RelayCommand(() => FunctionGenerator.GetSystemFunctions()));

        internal override Typlanguage RobotType => Typlanguage.KUKA;

        internal override string SourceFile => throw new NotImplementedException();

        public static string GetSystemFunctions => FunctionGenerator.GetSystemFunctions();

        public static List<string> Ext => new List<string>
                {
                    ".dat",
                    ".src",
                    ".ini",
                    ".sub",
                    ".zip",
                    ".kfd"
                };

        public override List<string> SearchFilters => Ext;

        public string Comment { get; set; }

        internal override IList<ICompletionData> CodeCompletion => new List<ICompletionData>
                {
                    new CodeCompletion("Item1")
                };

        internal sealed override AbstractFoldingStrategy FoldingStrategy { get; set; }

        public new MenuItem MenuItems
        {
            get
            {
                MenuItem menuItem = new MenuItem
                {
                    Header = "KUKA"
                };
                MenuItem newItem = new MenuItem
                {
                    Header = "Test 456"
                };
                _ = menuItem.Items.Add(newItem);
                return menuItem;
            }
        }

        private static Snippet ForSnippet => new Snippet
        {
            Elements =
                    {
                        new SnippetTextElement
                        {
                            Text = "for "
                        },
                        new SnippetReplaceableTextElement
                        {
                            Text = "item"
                        },
                        new SnippetTextElement
                        {
                            Text = " in range("
                        },
                        new SnippetReplaceableTextElement
                        {
                            Text = "from"
                        },
                        new SnippetTextElement
                        {
                            Text = ", "
                        },
                        new SnippetReplaceableTextElement
                        {
                            Text = "to"
                        },
                        new SnippetTextElement
                        {
                            Text = ", "
                        },
                        new SnippetReplaceableTextElement
                        {
                            Text = "step"
                        },
                        new SnippetTextElement
                        {
                            Text = "):backN\t"
                        },
                        new SnippetSelectionElement()
                    }
        };

        public override Regex EnumRegex => new Regex("^(ENUM)\\s+([\\d\\w]+)\\s+([\\d\\w,]+)",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public override Regex StructRegex => new Regex("DECL STRUC|^STRUC\\s([\\w\\d]+\\s*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public override Regex FieldRegex => new Regex(
                        "^[DECL ]*[GLOBAL ]*[CONST ]*(INT|REAL|BOOL|CHAR)\\s+([\\$0-9a-zA-Z_\\[\\],\\$]+)=?([^\\r\\n;]*);?([^\\r\\n]*)",
                        RegexOptions.IgnoreCase | RegexOptions.Multiline);

        protected override string ShiftRegex => "((E6POS [\\w]*={)X\\s([\\d.-]*)\\s*,*Y\\s*([-.\\d]*)\\s*,Z\\s*([-\\d.]*))";

        public override Regex MethodRegex => new Regex("^[GLOBAL ]*(DEF)+\\s+([\\w_\\d]+\\s*)\\(",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);

        internal override string FunctionItems => "((DEF|DEFFCT (BOOL|CHAR|INT|REAL|FRAME)) ([\\w_\\s]*)\\(([\\w\\]\\s:_\\[,]*)\\))";

        public override string CommentChar => ";";

        public override Regex SignalRegex => new Regex("^(SIGNAL+)\\s+([\\d\\w]+)\\s+([^\\r\\;]*)",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public override Regex XYZRegex => new Regex("^[DECL ]*[GLOBAL ]*(POS|E6POS|E6AXIS|FRAME) ([\\w\\d_\\$]+)=?\\{?([^}}]*)?\\}?",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public FileInfo GetFileInfo(string text)
        {
            FileInfo result = new FileInfo(text);
            return result;
            //            return _fi.GetFileInfo(text);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Dispose(true);
            }
        }

        public static bool OnlyDatExists(string filename)
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(filename),
                    Path.GetFileNameWithoutExtension(filename) + ".src"));
        }

        [Localizable(false)]
        public static string SystemFileName()
        {
            string result;
            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Filter = "All File (*.*)|*.*";
                openFileDialog.InitialDirectory = "C:\\krc\\bin\\";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    result = openFileDialog.FileName;
                    return result;
                }
            }
            result = string.Empty;
            return result;
        }

        protected override bool IsFileValid(System.IO.FileInfo file)
        {
            return FileIsValid(file);
        }

        internal bool FileIsValid(System.IO.FileInfo file)
        {
            foreach (string ext in Ext)
            {
                if (file.Extension.ToLower() == ext)
                {
                    return true;
                }
            }
            return false;
        }
        private static Collection<string> GetPositionFromFile(int line, ITextEditorComponent editor)
        {
            Collection<string> collection = new Collection<string>();
            while (true)
            {
                collection.Add(editor.Document.Lines[line].ToString());
                line++;
            }

            return collection;
        }

        public static Editor ReversePath(Editor editor)
        {
            Collection<Collection<string>> collection = new Collection<Collection<string>>();
            for (int i = 0; i <= editor.Document.Lines.Count - 1; i++)
            {
                if (
                    editor.Document.Lines[i].ToString()
                        .ToUpperInvariant()
                        .IndexOf(";FOLD LIN", StringComparison.OrdinalIgnoreCase) > -1 |
                    editor.Document.Lines[i].ToString()
                        .ToUpperInvariant()
                        .IndexOf(";FOLD PTP", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    collection.Add(GetPositionFromFile(i, editor));
                }
            }
            editor.Text = string.Empty;
            for (int j = collection.Count - 1; j >= 0; j--)
            {
                for (int k = 0; k < collection[j].Count; k++)
                {
                    Collection<string> collection2 = collection[j];
                    editor.AppendText(collection2[k] + "\r\n");
                }
            }
            return editor;
        }

        internal override string FoldTitle(FoldingSection section, TextDocument doc)
        {
            string[] array = Regex.Split(section.Title, "�");
            string text = section.TextContent.ToLower().Trim();
            string text2 = section.TextContent.Trim();
            int num = section.TextContent.Trim().IndexOf("%{PE}%", StringComparison.Ordinal) - "%{PE}%".Length;
            int num2 = section.TextContent.Trim().IndexOf("\r\n", StringComparison.Ordinal);
            _ = section.StartOffset + array[0].Length;
            text2 = text2.Substring(text.IndexOf(array[0], StringComparison.Ordinal) + array[0].Length);
            int num4 = text2.Length - array[0].Length;
            if (num > -1)
            {
                num4 = (num < num2) ? num : num4;
            }
            return text2.Substring(0, num4);
        }

        public override DocumentViewModel GetFile(string filepath)
        {
            ImageSource iconSource = null;
            string extension = Path.GetExtension(filepath.ToLower());
            if (extension != null)
            {
                if (!(extension == ".src"))
                {
                    if (!(extension == ".dat"))
                    {
                        if (extension == ".sub" || extension == ".sps" || extension == ".kfd")
                        {
                            GetInfo();
                            iconSource = ImageHelper.GetIcon("..\\..\\Resources\\spsfile.png");
                        }
                    }
                    else
                    {
                        GetInfo();
                        iconSource = ImageHelper.GetIcon("..\\..\\Resources\\datfile.png");
                    }
                }
                else
                {
                    GetInfo();
                    iconSource = ImageHelper.GetIcon("..\\..\\Resources\\srcfile.png");
                }
            }
            return new DocumentViewModel(filepath)
            {
                IconSource = iconSource
            };
        }

        private static void GetInfo()
        {
        }

        public SnippetCollection Snippets()
        {
            return new SnippetCollection
            {
                ForSnippet
            };
        }

        public override string ExtractXYZ(string positionstring)
        {
            PositionBase positionBase = new PositionBase(positionstring);
            return positionBase.ExtractFromMatch();
        }

        public static string GetDatFileName(string filename)
        {
            return filename.Substring(0, filename.LastIndexOf('.')) + ".dat";
        }

        public static List<string> GetModuleFileNames(string filename)
        {
            string str = filename.Substring(0, filename.LastIndexOf('.'));
            List<string> list = new List<string>();
            if (File.Exists(str + ".src"))
            {
                list.Add(str + ".src");
            }
            if (File.Exists(str + ".dat"))
            {
                list.Add(str + ".dat");
            }
            return list;
        }

        private static class FunctionGenerator
        {
            private static string _functionFile = string.Empty;

            private static string GetStruc(string filename)
            {
                return RemoveFromFile(filename, "((?<!_)STRUC [\\w\\s,\\[\\]]*)");
            }

            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static string GetSystemFunctions()
            {
                StringBuilder stringBuilder = new StringBuilder();
                SystemFunctionsViewModel systemFunctionsViewModel = new SystemFunctionsViewModel();
                Window window = new Window
                {
                    Content = systemFunctionsViewModel
                };
                string result;
                if (window.DialogResult.HasValue && window.DialogResult.Value)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    try
                    {
                        openFileDialog.Filter =
                            "KUKA VxWorks _file (vxWorks.rt;VxWorks.Debug;*.*)|vxWorks.rt;vxWorks.debug;*.*";
                        openFileDialog.Title = "Select file for reading System Functions";
                        openFileDialog.InitialDirectory = "C:\\krc\\bin\\";
                        if (openFileDialog.ShowDialog() == true)
                        {
                            if (!File.Exists(openFileDialog.FileName))
                            {
                                return null;
                            }
                            File.Copy(openFileDialog.FileName, "c:\\Temp.rt", true);
                            _functionFile = "c:\\Temp.rt";
                            if (systemFunctionsViewModel.Structures)
                            {
                                _ = stringBuilder.AppendFormat("{0}\r\n*** Structures  ******************\r\n{0}\r\n",
                                    "************************************************");
                                _ = stringBuilder.Append(GetStruc(_functionFile));
                            }
                            if (systemFunctionsViewModel.Programs)
                            {
                                _ = stringBuilder.AppendFormat("{0}\r\n*** Programs  ******************\r\n{0}\r\n",
                                    "************************************************");
                                _ = stringBuilder.Append(GetRegex(_functionFile,
                                    "(EXTFCTP|EXTDEF)([\\d\\w]*)([\\[\\]\\w\\d\\( :,]*\\))"));
                            }
                            if (systemFunctionsViewModel.Functions)
                            {
                                _ = stringBuilder.AppendFormat("{0}\r\n*** Functions  ******************\r\n{0}\r\n",
                                    "************************************************");
                                _ = stringBuilder.Append(GetRegex(_functionFile,
                                    "(EXTFCTP|EXTDEF)([\\d\\w]*)([\\[\\]\\w\\d\\( :,]*\\))"));
                            }
                            if (systemFunctionsViewModel.Variables)
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage msg = new ErrorMessage("GetSystemFiles", ex, MessageType.Error);
                        _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
                    }
                }
                result = stringBuilder.ToString();
                return result;
            }

            private static string RemoveFromFile(string functionFile, string matchString)
            {
                StringBuilder stringBuilder = new StringBuilder();
                string input;
                using (StreamReader streamReader = new StreamReader(_functionFile))
                {
                    input = streamReader.ReadToEnd();
                    Regex regex = new Regex(matchString, RegexOptions.IgnoreCase);
                    MatchCollection matchCollection = regex.Matches(input);
                    if (matchCollection.Count > 0)
                    {
                        foreach (Match match in matchCollection)
                        {
                            _ = stringBuilder.AppendLine(match.Value);
                        }
                    }
                }
                Regex regex2 = new Regex(matchString);
                string value = regex2.Replace(input, string.Empty);
                using (StreamWriter streamWriter = new StreamWriter(functionFile))
                {
                    streamWriter.Write(value);
                }
                return stringBuilder.ToString();
            }

            private static string GetRegex(string functionFile, string matchString)
            {
                string result;
                if (string.IsNullOrEmpty(functionFile))
                {
                    result = null;
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    using (StreamReader streamReader = new StreamReader(functionFile))
                    {
                        string input = streamReader.ReadToEnd();
                        Regex regex = new Regex(matchString, RegexOptions.IgnoreCase);
                        MatchCollection matchCollection = regex.Matches(input);
                        if (matchCollection.Count > 0)
                        {
                            foreach (Match match in matchCollection)
                            {
                                _ = stringBuilder.AppendLine(match.Value);
                            }
                        }
                    }
                    result = stringBuilder.ToString();
                }
                return result;
            }
        }

        private sealed class RegionFoldingStrategy : AbstractFoldingStrategy
        {
            protected override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
            {
                firstErrorOffset = -1;
                List<LanguageFold> list = new List<LanguageFold>();
                list.AddRange(CreateFoldingHelper(document, ";fold", ";endfold", true));
                list.AddRange(CreateFoldingHelper(document, "def", "end", true));
                list.AddRange(CreateFoldingHelper(document, "global def", "end", true));
                list.AddRange(CreateFoldingHelper(document, "global deffct", "endfct", true));
                list.AddRange(CreateFoldingHelper(document, "deftp", "endtp", true));
                list.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
                return list;
            }
        }
    }
}