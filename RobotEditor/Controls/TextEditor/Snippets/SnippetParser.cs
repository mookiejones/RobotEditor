using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.AvalonEdit.Snippets;

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
            var snippet = new Snippet();
            var decarations = GetDecarations(element);
            var dictionary =
                new Dictionary<string, SnippetReplaceableTextElement>();
            var text = GetTheCode(element);
            while (text.ContainsDeclaration(decarations))
            {
                var theNextId = text.GetTheNextId(decarations);
                var text2 = text.Substring(0, text.IndexOf(theNextId, System.StringComparison.Ordinal));
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
                            var snippetReplaceableTextElement = new SnippetReplaceableTextElement
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
            var dictionary = new Dictionary<string, Declaration>(Declaration.Defaults);
            var xElement = element.Elements("Declarations").FirstOrDefault<XElement>();
            if (xElement != null)
            {
                foreach (var current in xElement.Elements("Literal"))
                {
                    var literal = new Literal(current);
                    dictionary.Add(literal.Id, literal);
                }
            }
            return dictionary;
        }

        private static string GetTheCode(XElement element)
        {
            var xElement = element.Element("Code");
            if (xElement != null)
            {
                return xElement.Value;
            }
            throw new XmlException("The element 'Code' is required on element '" + element + "'");
        }
    }
}