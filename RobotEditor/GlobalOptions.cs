using RobotEditor.Interfaces;
using RobotEditor.ViewModel;
using System.ComponentModel;

namespace RobotEditor
{
    public sealed class GlobalOptions : IOptions
    {


        private static GlobalOptions _instance;
        public static GlobalOptions Instance => _instance ?? (_instance = new GlobalOptions());

        private GlobalOptions()
        {
            FlyoutOpacity = 0.85;
        }



        [DefaultValue(0.75)]
        public double FlyoutOpacity { get; set; }

        public GlobalOptionsViewModel Options { get; set; } = new GlobalOptionsViewModel();

        [Localizable(false)]
        public string Title => "Global Options";
    }
}