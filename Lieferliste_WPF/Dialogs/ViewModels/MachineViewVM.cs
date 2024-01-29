using CompositeCommands.Core;
using El2Core.Models;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Lieferliste_WPF.Dialogs.ViewModels
{
    class MachineViewVM : IDialogAware
    {
        public string Title => "Maschinen Details";
        public string InventNo { get; private set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<int>? CostUnits { get; private set; }
        private List<Vorgang>? Processes { get; set; }
        public ICollectionView? ProcessesCV { get; private set; }
        public event Action<IDialogResult> RequestClose;
        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set
            {
                if (_applicationCommands != value)
                {
                    _applicationCommands = value;
                }
            }
        }
        public MachineViewVM(IApplicationCommands applicationCommands) { _applicationCommands = applicationCommands; }
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
           
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Name = parameters.GetValue<string>("Name");
            InventNo = parameters.GetValue<string>("InvNo");
            Description = parameters.GetValue<string>("Description");
            CostUnits = parameters.GetValue<List<int>>("CostUnits");
            Processes = parameters.GetValue<List<Vorgang>>("processList");
            ProcessesCV = CollectionViewSource.GetDefaultView(Processes);
        }
    }
}
