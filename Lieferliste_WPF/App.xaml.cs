using El2Utilities.Models;
using El2Utilities.Utils;
using Lieferliste_WPF.Interfaces;
using Lieferliste_WPF.Properties;
using Lieferliste_WPF.View;
using Lieferliste_WPF.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace Lieferliste_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            ServiceProvider.GetRequiredService<AppStatic>();
            ServiceProvider.GetRequiredService<MainWindowViewModel>();
            ServiceProvider.GetRequiredService<LieferViewModel>();
            ServiceProvider.GetRequiredService<MachineEditViewModel>();
            _ = ServiceProvider.GetRequiredService<MachinePlanViewModel>();
            ServiceProvider.GetRequiredService<RoleEditViewModel>();
            ServiceProvider.GetRequiredService<UserViewModel>();
            ServiceProvider.GetRequiredService<SettingsViewModel>();


            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            string defaultConnection = Configuration.GetConnectionString("ConnectionBosch");
            services.AddDbContextFactory<DB_COS_LIEFERLISTE_SQLContext>(
                options =>
                    options.UseSqlServer(defaultConnection));

 
            services.AddTransient(typeof(MainWindow));
            services.AddSingleton<AppStatic>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<LieferViewModel>();
            services.AddSingleton<MachineEditViewModel>();
            services.AddSingleton<RoleEditViewModel>();
            services.AddSingleton<UserViewModel>();
            services.AddSingleton<MachinePlanViewModel>();
            services.AddSingleton<SettingsViewModel>();

           
        }
       
    }
}
