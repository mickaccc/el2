using El2Core.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lieferliste_WPF.Dialogs.ViewModels
{
    internal class AddNewWorkAreaVM : BindableBase, IDialogAware
    {

        private DelegateCommand<string> _closeDialogCommand;
        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string>(CloseDialog));

        private string _section;
        public string Section
        {
            get { return _section; }
            set { SetProperty(ref _section, value); }
        }
        private string _info;
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
        private IList<WorkArea> workA;
        public event Action<IDialogResult> RequestClose;

        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;
            IDialogParameters parameters = new DialogParameters();
            if (parameter?.ToLower() == "true")
            {

                var by = workA.Max(x => x.Sort) + 1;
                var wa = new WorkArea() { Bereich = Section, Info = Info, Sort = (Convert.ToByte(by)) };
                
                parameters.Add("new", wa);
                result = ButtonResult.OK;
                
            }
            else if (parameter?.ToLower() == "false")
                result = ButtonResult.Cancel;

            RaiseRequestClose(new DialogResult(result, parameters));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
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
