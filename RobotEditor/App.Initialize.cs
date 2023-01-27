using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RobotEditor.Model;
using RobotEditor.ViewModel;
using System;

namespace RobotEditor
{
    public partial class App
    {
        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        public App()

        {
            Services = ConfigureServices();
            Ioc.Default.ConfigureServices(Services);
        }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            ServiceCollection services = new ServiceCollection();
            // Services
            _ = services.AddSingleton<IDataService, DataService>();

            // ViewModels
            _ = services.AddSingleton<MainViewModel>();
            _ = services.AddSingleton<MessageViewModel>();

            _ = services.AddSingleton<StatusBarViewModel>();
            _ = services.AddSingleton<ObjectBrowserViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
