using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Lieferliste_WPF.ViewModels;

namespace Lieferliste_WPF.Planning
{
    class testMachine
    {
        public InternalMachine machine { get; private set; }

        public testMachine()
        {
            MachineCreator mCreator = new MachineCreator();
           // MachineContainer mContainer = new MachineContainerViewModel();
            //mContainer.BID = 1;
            mCreator.setFactory(new InternalMachineFactory());
            //mCreator.fillMachines(mContainer);

            //machine = (InternalMachine)mContainer.First(x => x.RID == 3);
        }

    }
}
