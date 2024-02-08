using CompositeCommands.Core;
using ControlzEx.Theming;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using Lieferliste_WPF.Dialogs;
using Lieferliste_WPF.Dialogs.ViewModels;
using Lieferliste_WPF.Planning;
using Lieferliste_WPF.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ModuleDeliverList.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using System.IO;
using System.Windows;
using Unity;

namespace Lieferliste_WPF
{
    internal class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            var settingsService = Container.Resolve<UserSettingsService>();
            ThemeManager.Current.ChangeTheme(App.Current, settingsService.Theme);
        
            return Container.Resolve<MainWindow>();
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();

        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            var defaultconnection = configuration.GetConnectionString("ConnectionBosch");
            var builderopt = new DbContextOptionsBuilder<DB_COS_LIEFERLISTE_SQLContext>().UseSqlServer(defaultconnection)
                .EnableThreadSafetyChecks(true);

            containerRegistry.RegisterInstance(builderopt.Options);
            containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
            containerRegistry.RegisterScoped<IRegionManager, RegionManager>();
            containerRegistry.RegisterSingleton<IUserSettingsService, UserSettingsService>();
            containerRegistry.RegisterForNavigation<UserSettings>();
            containerRegistry.RegisterForNavigation<RoleEdit>();
            containerRegistry.RegisterForNavigation<MachinePlan>();
            containerRegistry.RegisterForNavigation<MachineEdit>();
            containerRegistry.RegisterForNavigation<UserEdit>();
            containerRegistry.RegisterForNavigation<Archive>();
            containerRegistry.RegisterForNavigation<Liefer>();
            containerRegistry.RegisterForNavigation<ShowWorkArea>();
            containerRegistry.RegisterForNavigation<ProjectEdit>();
            containerRegistry.RegisterForNavigation<MeasuringRoom>();

            containerRegistry.RegisterSingleton<IPlanMachineFactory, PlanMachineFactory>();
            containerRegistry.RegisterSingleton<IPlanWorkerFactory, PlanWorkerFactory>();
            containerRegistry.RegisterDialog<Order>();
            containerRegistry.RegisterDialog<MachineView, MachineViewVM>();
            containerRegistry.RegisterDialog<Projects>();
            containerRegistry.RegisterDialog<AddNewWorkArea, AddNewWorkAreaVM>();
            containerRegistry.RegisterDialog<HistoryDialog, HistoryDialogVM>();

            Globals gl = new(Container);
            UserInfo u = new();
            u.Initialize(gl.PC, gl.User);
            containerRegistry.RegisterInstance(u);

        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);

            moduleCatalog.AddModule<ModuleDeliverList.DeliverListModule>();

        }

    }
}
