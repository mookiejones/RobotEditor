using System.Windows;
using System.Windows.Controls;
using RobotEditor.Interfaces;
using RobotEditor.ViewModel;

namespace RobotEditor.Selectors
{
    public sealed class PanesStyleSelector : StyleSelector
    {
        public Style ToolStyle { private get; set; }
        public Style FileStyle { private get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            Style result;
            if (item is ToolViewModel)
            {
                result = ToolStyle;
            }
            else
            {
                if (item is IEditorDocument)
                {
                    result = FileStyle;
                }
                else
                {
                    result = base.SelectStyle(item, container);
                }
            }
            return result;
        }
    }
}