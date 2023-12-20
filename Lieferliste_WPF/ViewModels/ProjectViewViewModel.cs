using CompositeCommands.Core;
using El2Core.ViewModelBase;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lieferliste_WPF.ViewModels
{
    internal class ProjectViewViewModel : ViewModelBase
    {
        IContainerProvider _container;
        IApplicationCommands _applicationCommands;

        public ProjectViewViewModel(IContainerProvider container, IApplicationCommands applicationCommands)
        {
            _container = container;
            _applicationCommands = applicationCommands;
        }
    }
}
