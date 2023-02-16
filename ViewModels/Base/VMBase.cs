using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lieferliste_WPF.ViewModels.Base
{
    public class VMBase: ViewModelBase
    {
    private bool isNew = true;
    public bool IsNew
    {
        get { return isNew; }
        set
        {
            isNew = value;
            RaisePropertyChanged();
        }
    }
    private bool isSelected = false;

    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            RaisePropertyChanged();
        }
    }
    private bool isDeleted = false;

    public bool IsDeleted
    {
        get { return isDeleted; }
        set
        {
            isDeleted = value;
            RaisePropertyChanged();
        }
    }
        private bool isModified = false;

        public bool IsModified
        {
            get { return isModified; }
            set
            {
                isModified = value;
                RaisePropertyChanged();
            }
        }
}
}
