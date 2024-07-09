using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lieferliste_WPF.Dialogs.ViewModels
{
    public class InputDialogVM : IDialogAware
    {
        public string Title => "Schichtplan";
        public string? InputText {  get; set; }
        private DelegateCommand? _closeDialogCommand;
        public DelegateCommand CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand(OnOkDialog));

        private DelegateCommand? _cancelDialogCommand;
        public DelegateCommand CancelDialogCommand =>
            _cancelDialogCommand ?? (_cancelDialogCommand = new DelegateCommand(OnCancelDialog));

        public event Action<IDialogResult> RequestClose;
        private void OnOkDialog()
        {
            throw new NotImplementedException();
        }
        private void OnCancelDialog()
        {
            throw new NotImplementedException();
        }
        public bool CanCloseDialog()
        {
            return !string.IsNullOrWhiteSpace(InputText);
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
    }
}
