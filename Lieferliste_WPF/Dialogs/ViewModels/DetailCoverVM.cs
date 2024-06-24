using El2Core.Models;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Dialogs.ViewModels
{
    internal class DetailCoverVM : IDialogAware
    {
        public string Title => "Cover Details";
        public ShiftCover Cover { get; set; }
        List<Tuple<TimeOnly,TimeOnly>> TimeList { get; set; }
        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
           
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Cover = (ShiftCover)parameters.GetValue<ShiftCover>("Cover");
            if (TimeList == null) TimeList = [];
            bool[] bit = new bool[1440];
            Cover.CoverMask?.CopyTo(bit, 0);
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
                    TimeList.Add(Tuple.Create(
                        new TimeOnly(0, start),
                        new TimeOnly(0, i)));
                }
                else if (high && bit[i + 1] == false)
                {
                    high = false;
                    TimeList.Add(Tuple.Create(
                       new TimeOnly(0, start),
                       new TimeOnly(0, i)));
                }
            }
        }
    }
}
