using CompositeCommands.Core;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Planning;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;

namespace Lieferliste_WPF.Dialogs.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0")]
    class WorkerViewVM : IDialogAware, IDropTarget
    {
        public string Title => "Messraum Details";
        public PlanWorker? PlanWorker { get; private set; }
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

        public WorkerViewVM(IApplicationCommands applicationCommands) { _applicationCommands = applicationCommands; }
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
           
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            PlanWorker = parameters.GetValue<PlanWorker>("PlanWorker");
            SetMarkerCommand = PlanWorker.SetMarkerCommand;
        }
        public void Drop(IDropInfo dropInfo)
        {
            PlanWorker?.Drop(dropInfo);
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.IsSameDragDropContextAsSource) PlanWorker?.DragOver(dropInfo);

        }
    }
}
