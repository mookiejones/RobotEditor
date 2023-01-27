/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:editor.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.DependencyInjection;
using RobotEditor.Model;

namespace RobotEditor.ViewModel
{
    /// <summary>
    ///     This class contains static references to all the view models in the
    ///     application and provides an entry point for the bindings.
    ///     <para>
    ///         See http://www.galasoft.ch/mvvm
    ///     </para>
    /// </summary>
    public sealed class ViewModelLocator
    {
        //static ViewModelLocator()
        //{
        //    ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
 
        //    SimpleIoc.Default.Register<IDataService, DataService>();
        //    SimpleIoc.Default.Register<StatusBarViewModel>();
        //    SimpleIoc.Default.Register<ObjectBrowserViewModel>();
        //    SimpleIoc.Default.Register<MainViewModel>(true);
        //}

        public ObjectBrowserViewModel ObjectBrowser => Ioc.Default.GetRequiredService<ObjectBrowserViewModel>();

        public StatusBarViewModel StatusBar => Ioc.Default.GetRequiredService<StatusBarViewModel>();

        /// <summary>
        ///     Gets the Main property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main => Ioc.Default.GetRequiredService<MainViewModel>();

        /// <summary>
        ///     Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}