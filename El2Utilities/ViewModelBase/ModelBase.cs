using El2Core.Models;
using System.ComponentModel;

namespace El2Core.ViewModelBase
{
    public abstract class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void RunPropertyChanged((string?, string?, string?) focused = default)
        {
            var th = this as Vorgang;
            if (focused == default)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
            }
            else if (th != null)
            {
                foreach (var item in this.GetType().GetProperties())
                {
                    if (th.VorgangId != focused.Item1 || item.Name != focused.Item2)
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
