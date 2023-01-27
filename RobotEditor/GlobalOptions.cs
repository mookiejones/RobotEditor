using System.ComponentModel;
using RobotEditor.Interfaces;
using RobotEditor.ViewModel;

namespace RobotEditor.Classes
{
    public sealed class GlobalOptions : IOptions
    {
        

        private static GlobalOptions _instance;
        public static GlobalOptions Instance => _instance ?? (_instance = new GlobalOptions());
        private GlobalOptionsViewModel _options = new GlobalOptionsViewModel();

        private GlobalOptions()
        {
            FlyoutOpacity = 0.85;
        }

     

        [DefaultValue(0.75)]
        public double FlyoutOpacity { get; set; }

        public GlobalOptionsViewModel Options
        {
            get => _options;
            set => _options = value;
        }

        [Localizable(false)]
        public string Title => "Global Options";
    }
}