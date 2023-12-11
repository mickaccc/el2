using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace El2Core.ViewModelBase
{
    public abstract class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void RunPropertyChanged()
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(""));
        }

    }
}
