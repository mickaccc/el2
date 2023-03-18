using Lieferliste_WPF.Data;
using Lieferliste_WPF.ViewModels;
using Lieferliste_WPF.Working;
using Lieferliste_WPF.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Lieferliste_WPF.ViewModels.Support;
using Lieferliste_WPF.Data.Models;

namespace Lieferliste_WPF.Planning
{
    class MachineCreator
    {
        private IMachineFactory _machineFactory = null;

        public void setFactory(IMachineFactory factoryRef)
        {
            this._machineFactory = factoryRef;
        }

        public void fillMachines(MachineContainerViewModel machineContainer)
        {
            using (var ctx = new DataContext())
            {


               

            }
        }

        public void fillParking(MachineContainer machineContainer)
        {

        }
    }
}
