using Prism.Ioc;
using Prism.Modularity;
using ModuleRoleEdit.Views;
using System;
using Prism.Regions;

namespace ModuleRoleEdit
{
    public class RoleEditModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<RoleEdit>();
        }
    }
}
