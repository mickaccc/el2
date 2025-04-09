using El2Core.Converters;
using El2Core.Models;
using Prism.Commands;
using Prism.Dialogs;
using System;

namespace Lieferliste_WPF.Dialogs.ViewModels
{
    public class ProcessTimeDialogVM : IDialogAware
    {
        private string? correctValue;
        public string? CorrectValue
        {
            get { return correctValue; }
            set { correctValue = value; }
        }
        private EmployeeNote? emplNote;
        public string Title => "Zeit ändern";

        private DelegateCommand<string?>? _closeDialogCommand;
        public DelegateCommand<string?> CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string?>(CloseDialog));
        private DelegateCommand<string?>? _textChangeCommand;
        public DelegateCommand<string?> TextChangeCommand =>
            _textChangeCommand ?? (_textChangeCommand = new DelegateCommand<string?>(OnTextChange));



        public DialogCloseListener RequestClose { get; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }
        private void OnTextChange(string? obj)
        {
            correctValue = obj;
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
                param.Add("newTime", correctValue);
                param.Add("note", emplNote);
            }
            RequestClose.Invoke(param, result);
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var conv = new TimeConverter();
            emplNote = parameters.GetValue<EmployeeNote>("note");
            correctValue = (string?)conv.Convert(emplNote.Processingtime, null, null, null);
            
        }
    }

}
