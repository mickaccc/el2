using El2Core.Models;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace Lieferliste_WPF.Dialogs.ViewModels
{
    internal class DetailCoverVM : IDialogAware
    {
        public string Title => "Cover Details";
        public ShiftCover Cover { get; set; }
        public bool IsEditable { get; } = false;
        public List<string[]> TimeList { get; set; }
        public event Action<IDialogResult> RequestClose;
        private DelegateCommand? _closeDialogCommand;
        public DelegateCommand CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand (OnDialogClosed));

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            ButtonResult result = ButtonResult.None;
            IDialogParameters param = new DialogParameters();

            //if (parameter == null)
            //    result = ButtonResult.Cancel;
            //else if (parameter is Vorgang v)
            //{
            //    result = ButtonResult.Yes;
            //    param.Add("Comment", v.BemT);
            //    param.Add("VID", _vid);
            //}
            RaiseRequestClose(new DialogResult(result, param));
        }
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            Cover = (ShiftCover)parameters.GetValue<ShiftCover>("Cover");
            if (TimeList == null) TimeList = [];
            bool[] bit = new bool[1440];
            BitArray bitArray = new BitArray(Cover.CoverMask);
            bitArray.CopyTo(bit, 0);
            int start = 0;
            bool high = false;
            for (int i = 0; i < bit.Length; i++)
            {
                if (bit[i])
                {
                    if (i == 0)
                    {
                        high = true;
                        start = i;
                    }
                    else if (bit[i - 1] == false)
                    {
                        high = true;
                        start = i;
                    }
                }
                else if (i == bit.Length && high)
                {
                    TimeList.Add([
                        new TimeOnly(start/60, start%60).ToString(),
                        new TimeOnly(i/60, i%60).ToString() ]);
                }
                else if (high && bit[i + 1] == false)
                {
                    high = false;
      
                    TimeList.Add([
                       new TimeOnly(start/60, start%60).ToString(),
                       new TimeOnly(i/60, i%60).ToString() ]);
                }
            }
        }
    }
}
