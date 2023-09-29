using System.ComponentModel;

namespace Lieferliste_WPF.ViewModels
{
    /// <summary>
    /// Abstract base-class for a view-model.
    /// </summary>
    public abstract class AbstractViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Event raised when a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
