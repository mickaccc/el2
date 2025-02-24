using El2Core.Models;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace El2Core.ViewModelBase
{
    public abstract class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void RunPropertyChanged((string, string)? focused = null)
        {
            var th = this as Vorgang;
            if (focused == null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
            }
            else if (th != null)
            {
                foreach (var item in this.GetType().GetProperties())
                {
                    if (th.VorgangId != focused.Value.Item1 || item.Name != focused.Value.Item2)
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(item.Name));
                }
            }
            else
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
            }
        }
    }
}
