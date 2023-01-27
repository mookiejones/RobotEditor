using System;
using System.Xml.Linq;

namespace RobotEditor.Controls.TextEditor.Snippets
{
    public sealed class Literal : Declaration
    {
        public Literal(XContainer literal)
        {
            if (literal == null)
            {
                throw new ArgumentNullException("literal");
            }
            XElement xElement = literal.Element("ID");
            if (xElement != null)
            {
                Id = "$" + xElement.Value + "$";
            }
            XElement xElement2 = literal.Element("ToolTip");
            if (xElement2 != null)
            {
                ToolTip = xElement2.Value;
            }
            XElement xElement3 = literal.Element("Default");
            if (xElement3 != null)
            {
                Default = xElement3.Value;
            }
        }
    }
}