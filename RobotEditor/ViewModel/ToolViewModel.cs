using RobotEditor.Enums;

namespace RobotEditor.ViewModel
{
    public class ToolViewModel : PaneViewModel
    {
        public DefaultToolPane DefaultPane = DefaultToolPane.None;
        private bool _isVisible = true;

        protected ToolViewModel(string name)
        {
            Name = name;
            Title = name;
        }

        public int Height { get; set; }
        public int Width { get; set; }
        public string Name { get; private set; }

        public bool IsVisible { get =>_isVisible; set=>SetProperty(ref _isVisible,value); }
    }
}