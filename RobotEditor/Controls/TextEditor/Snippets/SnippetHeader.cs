using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RobotEditor.Controls.TextEditor.Snippets
{
    public class SnippetHeader
    {
        public string Title
        {
            get;
            set;
        }
        public string Text
        {
            get;
            set;
        }
        public List<string> Shortcuts
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public List<string> Extensions
        {
            get;
            set;
        }
        public string Author
        {
            get;
            set;
        }
        public List<SnippetType> Types
        {
            get;
            set;
        }
        public SnippetHeader(XElement headerElement)
        {

            throw new NotImplementedException();
            /*        Title = headerElement.ElementsValue("Title", "No title");
            Description = headerElement.ElementsValue("Description", "No description");
            Author = headerElement.ElementsValue("Author", "No author");
            Text = headerElement.ElementsValue("Text", Title);
     * */
            Shortcuts = new List<string>();
            foreach (XElement current in headerElement.Elements("Shortcut"))
            {
                if (!string.IsNullOrEmpty(current.Value) && !Shortcuts.Contains(current.Value))
                {
                    Shortcuts.Add(current.Value);
                }
            }
            Types = new List<SnippetType>();
            XElement xElement = headerElement.Elements("SnippetTypes").FirstOrDefault<XElement>();
            if (xElement != null)
            {
                foreach (XElement current2 in xElement.Elements("SnippetType"))
                {
                    try
                    {
                        SnippetType item = (SnippetType)Enum.Parse(typeof(SnippetType), current2.Value);
                        Types.Add(item);
                    }
                    catch
                    {
                    }
                }
            }
            LoadExtension(headerElement);
        }


        private void LoadExtension(XElement headerElement)
        {
            if (headerElement == null)
            {
                throw new ArgumentNullException("headerElement");
            }
            Extensions = new List<string>();
            IEnumerable<XElement> enumerable = headerElement.Elements("Extensions");
            foreach (XElement current in enumerable)
            {
                string value = current.Value;
                string[] array = value.Split(new char[]
                {
                    ' '
                });
                string[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    string text = array2[i];
                    if (!string.IsNullOrEmpty(text) && !Extensions.Contains(text))
                    {
                        string text2 = text;
                        if (!text.StartsWith("."))
                        {
                            text2 += ".";
                        }
                        Extensions.Add(text2);
                    }
                }
            }
        }
    }
}