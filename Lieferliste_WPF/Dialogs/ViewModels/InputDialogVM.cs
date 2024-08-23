using Prism.Commands;
using Prism.Dialogs;

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

        public DialogCloseListener RequestClose { get; }

        private void OnOkDialog()
        {
            if (string.IsNullOrWhiteSpace(InputText))
            { return; }
            else
            {
                IDialogParameters param = new DialogParameters();

                param.Add("InputText", InputText);

                RequestClose.Invoke(param, ButtonResult.OK);
            }

        }
        private void OnCancelDialog()
        {
           RequestClose.Invoke(new DialogParameters(), ButtonResult.Cancel); 
        }
        public bool CanCloseDialog()
        {
            return !string.IsNullOrWhiteSpace(InputText);
        }

        public void OnDialogClosed()
        {
            
        }
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
    }
}
