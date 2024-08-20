using El2Core.Models;
using Prism.Commands;
using Prism.Dialogs;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lieferliste_WPF.Dialogs.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0")]
    internal class AddNewWorkAreaVM : BindableBase, IDialogAware
    {

        private DelegateCommand<string>? _closeDialogCommand;
        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string>(CloseDialog));

        private string _section = string.Empty;
        public string Section
        {
            get { return _section; }
            set { SetProperty(ref _section, value); }
        }
        private string _info = string.Empty;
        public string Info
        {
            get { return _info; }
            set { SetProperty(ref _info, value); }
        }
        private string _title = "neuer Bereich";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DialogCloseListener RequestClose { get; }

        private IList<WorkArea>? workA;

        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;
            IDialogParameters parameters = new DialogParameters();
            if (parameter?.ToLower() == "true")
            {

                var by = workA?.Max(x => x.Sort) + 1;
                var wa = new WorkArea() { Bereich = Section, Info = Info, Sort = (Convert.ToByte(by)) };
                
                parameters.Add("new", wa);
                result = ButtonResult.OK;
                
            }
            else if (parameter?.ToLower() == "false")
                result = ButtonResult.Cancel;

            RequestClose.Invoke(parameters, result);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            workA = parameters.GetValue<IList<WorkArea>>("SectionList");
        }
    }
}
