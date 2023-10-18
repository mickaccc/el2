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
 
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            string defaultConnection = Configuration.GetConnectionString("ConnectionHome");
            services.AddDbContextFactory<DB_COS_LIEFERLISTE_SQLContext>(
                options =>
                    options.UseSqlServer(defaultConnection));

            services.AddTransient(typeof(MainWindow))
            .AddSingleton<AppStatic>()
            .AddSingleton<MainWindowViewModel>()
            .AddScoped<LieferViewModel>()
            .AddScoped<MachineEditViewModel>()
            .AddScoped<RoleEditViewModel>()
            .AddScoped<UserViewModel>()
            .AddScoped<MachinePlanViewModel>();
          
        }
       
    }
}
