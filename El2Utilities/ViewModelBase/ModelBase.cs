using System.ComponentModel;

namespace El2Core.ViewModelBase
{
    public abstract class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void RunPropertyChanged((string, string)? focused = null)
        {
            if (focused == null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
            }
            else
            {
                foreach (var item in this.GetType().GetProperties())
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(item.Name));
                }
            }
        }
    }
}
