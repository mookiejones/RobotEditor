using ICSharpCode.AvalonEdit.Snippets;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace RobotEditor.Controls.TextEditor.Snippets
{
    public class Declaration
    {
        private static Dictionary<string, Declaration> _defaults;
        public static Dictionary<string, Declaration> Defaults
        {
            get
            {
                if (_defaults == null)
                {
                    _defaults = new Dictionary<string, Declaration>
                    {
                        {
                            "$end$", new Declaration
                            {
                                Id = "$end$"
                            }
                        },
                        {
                            "$selection$", new Declaration
                            {
                                Id = "$selection$"
                            }
                        }
                    };
                }
                return _defaults;
            }
        }
        public string Default
        {
            get;
            set;
        }
        public string Id
        {
            get;
            set;
        }
        public object ToolTip
        {
            get;
            set;
        }
    }

    internal static class SnippetParser
    {
        public static Snippet BuildSnippet(XElement element)
        {
            Snippet snippet = new();
            Dictionary<string, Declaration> decarations = GetDecarations(element);
            Dictionary<string, SnippetReplaceableTextElement> dictionary =
                new();
            string text = GetTheCode(element);
            while (text.ContainsDeclaration(decarations))
            {
                string theNextId = text.GetTheNextId(decarations);
                string text2 = text.Substring(0, text.IndexOf(theNextId, System.StringComparison.Ordinal));
                if (!string.IsNullOrEmpty(text2))
                {
                    snippet.Elements.Add(new SnippetTextElement
                    {
                        Text = text2
                    });
                    text = text.Remove(0, text2.Length);
                }
                text = text.Remove(0, theNextId.Length);
                if (theNextId == "$end$")
                {
                    snippet.Elements.Add(new SnippetCaretElement());
                }
                else
                {
                    if (theNextId == "$selection$")
                    {
                        snippet.Elements.Add(new SnippetSelectionElement());
                    }
                    else
                    {
                        if (dictionary.ContainsKey(theNextId))
                        {
                            snippet.Elements.Add(new SnippetBoundElement
                            {
                                TargetElement = dictionary[theNextId]
                            });
                        }
                        else
                        {
                            SnippetReplaceableTextElement snippetReplaceableTextElement = new()
                            {
                                Text = decarations[theNextId].Default
                            };
                            snippet.Elements.Add(snippetReplaceableTextElement);
                            dictionary.Add(theNextId, snippetReplaceableTextElement);
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(text))
            {
                snippet.Elements.Add(new SnippetTextElement
                {
                    Text = text
                });
            }
            return snippet;
        }

        private static Dictionary<string, Declaration> GetDecarations(XElement element)
        {
            Dictionary<string, Declaration> dictionary = new(Declaration.Defaults);
            XElement xElement = element.Elements("Declarations").FirstOrDefault<XElement>();
            if (xElement != null)
            {
                foreach (XElement current in xElement.Elements("Literal"))
                {
                    Literal literal = new(current);
                    dictionary.Add(literal.Id, literal);
                }
            }
            return dictionary;
        }

        private static string GetTheCode(XElement element)
        {
            XElement xElement = element.Element("Code");
            return xElement != null ? xElement.Value : throw new XmlException("The element 'Code' is required on element '" + element + "'");
        }
    }
}