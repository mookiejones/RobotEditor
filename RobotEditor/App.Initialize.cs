using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RobotEditor.Model;
using RobotEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var services = new ServiceCollection();
            // Services
            services.AddSingleton<IDataService,DataService>();

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MessageViewModel>();

            services.AddSingleton<StatusBarViewModel>();
            services.AddSingleton<ObjectBrowserViewModel>();
            
            return services.BuildServiceProvider();
        }
    }
}
