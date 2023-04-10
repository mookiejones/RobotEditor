using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RobotEditor.UI.Selectors;

public sealed class MenuTemplateSelector : DataTemplateSelector
{
    public DataTemplate KUKATemplate { get; set; }
    public DataTemplate FanucTemplate { get; set; }
    public DataTemplate NachiTemplate { get; set; }
    public DataTemplate ABBTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (container is FrameworkElement && item != null && item is Task)
        {
        }
        return null;
    }
}