using CompositeCommands.Core;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Planning;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;

namespace Lieferliste_WPF.Dialogs.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0")]
    class MachineViewVM : IDialogAware, IDropTarget
    {
        public string Title => "Maschinen Details";
        public PlanMachine? PlanMachine { get; private set; }
        public event Action<IDialogResult>? RequestClose;
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
        public ICommand? SetMarkerCommand { get; private set; }
        public ICommand? HistoryCommand { get; private set; }
        public ICommand? FastCopyCommand { get; private set; }
        public ICommand? CorrectionCommand { get; private set; }

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
            PlanMachine = parameters.GetValue<PlanMachine>("PlanMachine");
            SetMarkerCommand = PlanMachine.SetMarkerCommand;
            HistoryCommand = PlanMachine.HistoryCommand;  
            FastCopyCommand = PlanMachine.FastCopyCommand;
            CorrectionCommand = PlanMachine.CorrectionCommand;
        }
        public void Drop(IDropInfo dropInfo)
        {
            PlanMachine?.Drop(dropInfo);
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.IsSameDragDropContextAsSource) PlanMachine?.DragOver(dropInfo);

        }
    }
}
