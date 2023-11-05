using El2Core.Utils;
using Microsoft.Identity.Client;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.CodeDom;
using System.Threading.Tasks;
using System.Windows;

namespace ModuleLoadingView
{
    [ModuleDependency("MainWindowViewModel")]
    public class LoadingViewModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
   
        }
    }
}
