using El2Core.Utils;
using El2Core.Models;
using Lieferliste_WPF.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.CompilerServices;
using CompositeCommands.Core;

namespace Lieferliste_WPF
{
    class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfiguration  Configuration = builder.Build();
            var defaultconnection = Configuration.GetConnectionString("ConnectionHome");
            var builderopt = new DbContextOptionsBuilder<DB_COS_LIEFERLISTE_SQLContext>().UseSqlServer(defaultconnection);

            containerRegistry.RegisterInstance(builderopt.Options);
            containerRegistry.Register<DB_COS_LIEFERLISTE_SQLContext>();
            containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
            containerRegistry.RegisterForNavigation<LoadingView>();

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
