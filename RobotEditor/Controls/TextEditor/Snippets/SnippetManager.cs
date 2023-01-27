using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.Messaging;
using ICSharpCode.AvalonEdit.Snippets;
using RobotEditor.Enums;
using RobotEditor.Messages;

namespace RobotEditor.Controls.TextEditor.Snippets
{
    public class SnippetManager
    {
        private static readonly Dictionary<string, SnippetInfo> Snippets = new Dictionary<string, SnippetInfo>();
        private static readonly Dictionary<string, List<SnippetInfo>> SnippetsByExtension = new Dictionary<string, List<SnippetInfo>>();

        public static IList<SnippetCompletionData> CompletionData
        {
            get
            {
                var dictionary = new Dictionary<SnippetInfo, string>();
                var list = new List<SnippetCompletionData>();
                foreach (var current in Snippets.Values)
                {
                    if (!dictionary.ContainsKey(current))
                    {
                        list.Add(new SnippetCompletionData(current));
                        dictionary.Add(current, string.Empty);
                    }
                }
                return list;
            }
        }
        public static IEnumerable<SnippetCompletionData> GetCompletionDataForExtension(string extension)
        {
            if (SnippetsByExtension.ContainsKey(extension))
            {
                foreach (var current in SnippetsByExtension[extension])
                {
                    yield return new SnippetCompletionData(current);
                }
            }
            yield break;
        }
        public static SnippetInfo GetSnippetForShortcut(string shortCut)
        {
            if (Snippets.ContainsKey(shortCut))
            {
                return Snippets[shortCut];
            }
            return null;
        }
        public static IEnumerable<SnippetInfo> GetSnippetsForExtension(string extension)
        {
            if (SnippetsByExtension.ContainsKey(extension))
            {
                foreach (var current in SnippetsByExtension[extension])
                {
                    yield return current;
                }
            }
            yield break;
        }
        public static bool HasSnippetsForExtension(string extension) => SnippetsByExtension.ContainsKey(extension);
        public static bool HasSnippetsFor(string shortCut, string extension)
        {
            if (Snippets.ContainsKey(shortCut))
            {
                var snippetInfo = Snippets[shortCut];
                if (snippetInfo.Header.Extensions.Contains(extension))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool KnowsShortCut(string shortCut) => Snippets.ContainsKey(shortCut);
        public static bool LoadSnippet(string file)
        {
            file = Path.GetFullPath(file);
            if (!File.Exists(file))
            {
                return false;
            }
            bool result;
            try
            {
                var xElement = XElement.Load(file);
                if (xElement.Name != "CodeSnippets")
                {
                    result = false;
                }
                else
                {
                    if (xElement.Elements("CodeSnippet").Count<XElement>() == 0)
                    {
                        result = false;
                    }
                    else
                    {
                        foreach (var current in xElement.Elements("CodeSnippet"))
                        {
                            var snippetInfo = BuildSnippet(current, file);
                            foreach (var current2 in snippetInfo.Header.Shortcuts)
                            {
                                if (!Snippets.ContainsKey(current2))
                                {
                                    Snippets.Add(current2, snippetInfo);
                                }
                                else
                                {
                                    var msg = new ErrorMessage(string.Format("Duplicate Shortcut :", file), null,
                                        MessageType.Error);
                                    WeakReferenceMessenger.Default.Send(msg);

                                }
                            }
                            foreach (var current3 in snippetInfo.Header.Extensions)
                            {
                                if (SnippetsByExtension.ContainsKey(current3))
                                {
                                    var list = SnippetsByExtension[current3];
                                    if (!list.Contains(snippetInfo))
                                    {
                                        list.Add(snippetInfo);
                                    }
                                }
                                else
                                {
                                    SnippetsByExtension[current3] = new List<SnippetInfo>
                                    {
                                        snippetInfo
                                    };
                                }
                            }
                        }
                        result = true;
                    }
                }
            }           
            catch (Exception ex2)
            {
                var msg = new ErrorMessage("ErrorOnLoadingSnippet", ex2, MessageType.Error);
                WeakReferenceMessenger.Default.Send(msg);
               
                result = false;
            }
            return result;
        }
        public static void LoadSnippets(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }
            foreach (var current in
                from x in Directory.GetFiles(directory)
                where x.ToLowerInvariant().EndsWith(".snippet")
                select x)
            {
                LoadSnippet(current);
            }
        }
        public static void ImportSnippet(string sourceFilePath, string targetDirectory)
        {
            if (!Path.IsPathRooted(targetDirectory))
            {
                targetDirectory = Path.GetFullPath(targetDirectory);
            }
            if (!Path.IsPathRooted(sourceFilePath))
            {
                sourceFilePath = Path.GetFullPath(sourceFilePath);
            }
            if (File.Exists(targetDirectory))
            {
                throw new ArgumentException("Target directory is an existing file.");
            }
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            var name = FileExtended.GetName(sourceFilePath);
            var destFileName = Path.Combine(targetDirectory, name);
            if (LoadSnippet(sourceFilePath))
            {
                File.Copy(sourceFilePath, destFileName, true);
            }
        }
        private static SnippetInfo BuildSnippet(XElement element, string path)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            var xAttribute = element.Attribute("Format");
            if (xAttribute == null)
            {
                throw new XmlException("The Attribute 'Format' is missing on element'" + element + "'");
            }
            var value = xAttribute.Value;
            var headerElement = element.Element("Header");
            var header = new SnippetHeader(headerElement);
            var element2 = element.Element("Snippet");
            Snippet snippet = SnippetParser.BuildSnippet(element2);
            return new SnippetInfo(path)
            {
                Version = value,
                Snippet = snippet,
                Header = header
            };
        }
    }
}