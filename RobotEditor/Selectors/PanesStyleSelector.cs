using RobotEditor.Interfaces;
using RobotEditor.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace RobotEditor.Selectors;

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
            result = item is IEditorDocument ? FileStyle : base.SelectStyle(item, container);
        }
        return result;
    }
}