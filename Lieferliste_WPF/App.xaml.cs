using El2Utilities.Models;
using El2Utilities.Utils;
using Lieferliste_WPF.Interfaces;
using Lieferliste_WPF.Properties;
using Lieferliste_WPF.Views;
using Lieferliste_WPF.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using Prism.Modularity;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Internal;

namespace Lieferliste_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
    }
}
