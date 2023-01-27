using System.Windows;
using System.Windows.Controls;
using AvalonDock.Layout;
using RobotEditor.ViewModel; 

namespace RobotEditor.Selectors
{
    public sealed class PanesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FunctionTemplate { private get; set; }
        public DataTemplate ObjectBrowserTemplate { private get; set; }
        public DataTemplate LocalVariablesTemplate { private get; set; }
        public DataTemplate NotesTemplate { private get; set; }
        public DataTemplate KukaTemplate { private get; set; }
        public DataTemplate FileTemplate { private get; set; }

        public DataTemplate AngleConverterTemplate { private get; set; }
        public DataTemplate MessageTemplate { private get; set; }

        public override DataTemplate SelectTemplate(object item,
            DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            if (item is ObjectBrowserViewModel)
                return ObjectBrowserTemplate;

            if (item is KukaViewModel)
                return KukaTemplate;

            if (item is NotesViewModel)
                return NotesTemplate;

            if (item is FunctionViewModel)
                return FunctionTemplate;

            if (item is DocumentViewModel)
                return FileTemplate;

            if (item is MessageViewModel)
                return MessageTemplate;

            if (item is AngleConvertorViewModel)
                return AngleConverterTemplate;
            return base.SelectTemplate(item, container);
        }
    }
}