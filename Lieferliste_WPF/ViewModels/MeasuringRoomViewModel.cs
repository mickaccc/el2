using CompositeCommands.Core;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lieferliste_WPF.ViewModels
{
    internal class MeasuringRoomViewModel : ViewModelBase, IDropTarget
    {
        public string Title { get; } = "Messraum Zuteilung";
        private IContainerProvider _container;
        private IApplicationCommands _applicationCommands;
        public MeasuringRoomViewModel(IContainerProvider container, IApplicationCommands applicationCommands)
        { 
            _container = container;
            _applicationCommands = applicationCommands;
        }
        public void DragOver(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        public void Drop(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }
    }
}
