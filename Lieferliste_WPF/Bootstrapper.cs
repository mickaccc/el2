using CompositeCommands.Core;
using El2Core.Models;
using El2Core.Utils;
using Lieferliste_WPF.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ModuleDeliverList;
using ModuleDeliverList.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using System.IO;
using System.Windows;

namespace Lieferliste_WPF
{
    class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
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

                IConfiguration  Configuration = builder.Build();
            var defaultconnection = Configuration.GetConnectionString("ConnectionBosch");
            var builderopt = new DbContextOptionsBuilder<DB_COS_LIEFERLISTE_SQLContext>().UseSqlServer(defaultconnection);

            containerRegistry.RegisterInstance(builderopt.Options);
            containerRegistry.Register<DB_COS_LIEFERLISTE_SQLContext>();
            containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
            containerRegistry.RegisterScoped<IRegionManager, RegionManager>();
            containerRegistry.RegisterForNavigation<UserSettings>();
            containerRegistry.RegisterForNavigation<RoleEdit>();
            containerRegistry.RegisterForNavigation<MachinePlan>();
            containerRegistry.RegisterForNavigation<MachineEdit>();
            containerRegistry.RegisterForNavigation<UserEdit>();

            containerRegistry.RegisterForNavigation<Liefer>();

            Globals gl = new Globals(Container);
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
