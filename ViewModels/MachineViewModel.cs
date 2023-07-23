using Lieferliste_WPF.Data.Models;
using Lieferliste_WPF.Planning;
using Lieferliste_WPF.Utilities;
using Lieferliste_WPF.ViewModels.Support;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lieferliste_WPF.ViewModels
{
    class MachineViewModel : Support.CrudVM
    {
        private IMachine _machine;
        public IMachine Machine
        {
            get { return _machine; }
            set
            {
                _machine = value;
                GetData();
                RaisePropertyChanged("Title");
            }
        }

        public ObservableCollection<DayLine> KappaLine { get; private set; }
        public ObservableLinkedList<Process> Processes { get; private set; }
        public MachineContainerViewModel MachineContainerViewModel { get; set; }
        public String Title { get { return Machine.MachineName; } }

        public Int32 RID { get { return Machine.RID; } }

        public MachineViewModel()
        {
            KappaLine = new ObservableCollection<DayLine>();
            Processes = new ObservableLinkedList<Process>();


        }

        protected override void GetData()
        {
            
        }

        internal bool addProcess(Process dragged)
        {
            return false;
        }
        internal void moveProcess(Process dragged, Process target)
        {
           
        }
    }
}
