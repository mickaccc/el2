using El2Core.Models;
using Prism.Commands;
using Prism.Dialogs;

namespace Lieferliste_WPF.Dialogs.ViewModels
{
    public class ProcessTimeDialogVM : IDialogAware
    {
        private double? correctValue;
        public double? CorrectValue
        {
            get { return correctValue; }
            set { correctValue = value; }
        }
        private EmployeeNote? emplNote;
        public string Title => "Zeit ändern";

        private DelegateCommand<string?>? _closeDialogCommand;
        public DelegateCommand<string?> CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string?>(CloseDialog));

        public DialogCloseListener RequestClose { get; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }
        protected virtual void CloseDialog(string? parameter)
        {
            ButtonResult result = ButtonResult.None;
            IDialogParameters param = new DialogParameters();

            if (parameter == null)
                result = ButtonResult.Cancel;
            else
            {
                result = ButtonResult.OK;
                param.Add("correct", correctValue);
                param.Add("correction", emplNote);
            }
            RequestClose.Invoke(param, result);
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            emplNote = parameters.GetValue<EmployeeNote>("correction");
            correctValue = emplNote.Processingtime;
            
        }
    }

}
