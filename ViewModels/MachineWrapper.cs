using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lieferliste_WPF.ViewModels
{
    class MachineWrapper:Base.ViewModelBase
    {
        private MachineViewModel _machine;
        public MachineViewModel MachineViewModel
        {
            get
            { return _machine; }
            set
            {
                _machine = value;
                RaisePropertyChanged("MachineViewModel");
                RaisePropertyChanged("Title");
            }
        }
        public string Title => MachineViewModel.Title;
    }
}
