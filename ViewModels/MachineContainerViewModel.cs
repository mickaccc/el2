using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lieferliste_WPF.Planning;
using Lieferliste_WPF.Entities;

namespace Lieferliste_WPF.ViewModels
{
    class MachineContainerViewModel:Base.ViewModelBase
    {
        public String Title { get { return _perspective.Name; } }
        public List<MachineViewModel> Machines { get; private set; }
        private Perspective _perspective;
        private ObservableList<Process> _orderPool = new ObservableList<Process>();
        public Perspective Perspective
        {
            get { return _perspective; }
            set { _perspective = value; }
        }
        private int _selectedMachinesIndex=0;
        public MachineViewModel SelectedMachine
        {
            get { return Machines.ElementAt(_selectedMachinesIndex); }
            set { _selectedMachinesIndex = Machines.IndexOf(value); }
        }

        public int BID { get; set; }

        public ObservableList<Process> OrderPool { get { return _orderPool; } set { _orderPool = value; } }
        public MachineContainerViewModel(Perspective perspective)
        {
            Machines = new List<MachineViewModel>();
            
            MachineCreator mCreator = new MachineCreator();
            _perspective = perspective;
            this.BID = perspective.SubType;
            mCreator.setFactory(new InternalMachineFactory());
            mCreator.fillMachines(this);
            _perspective.AttatchedPanes.Add(new MachineWrapper{MachineViewModel=SelectedMachine});
       
        }

        public void ChangeActiveMachine(MachineViewModel machine)
        {
                MachineWrapper mw =(MachineWrapper)ToolCase.This.AttachedPanes.First(x => x.GetType() == typeof(MachineWrapper));
                mw.MachineViewModel = machine;
                ToolCase.This.PropertyModifieded();
                    
                    
                
        }
    }
}
