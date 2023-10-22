using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;

namespace Lieferliste_WPF.ViewModels
{
    class UserSettingsViewModel : Base.ViewModelBase
    {
        string _ExplorerPathPattern;
        ObservableCollection<string> _ExplorerFilter = new();
        public ICollectionView ExplorerFilter { get; }
        string _ExplorerRoot;
        public Brush OutOfDate { get; set; }
        public Brush InOfDate { get; set; }

        public UserSettingsViewModel()
        {
            var br = new BrushConverter();
            ExplorerPathPattern = Properties.Settings.Default.ExplorerPath;
     
            ExplorerFilter = CollectionViewSource.GetDefaultView(_ExplorerFilter);          
            ExplorerRoot = Properties.Settings.Default.ExplorerRoot;
  
        }
        public string ExplorerPathPattern
        {
            get { return _ExplorerPathPattern; }
            set
            {
                if (_ExplorerPathPattern != value)
                {
                    _ExplorerPathPattern = value;
                    NotifyPropertyChanged(() => ExplorerPathPattern);
                }
            }
        }

        public string ExplorerExt
        {
            get => _ExplorerRoot;
            set
            {
                if (_ExplorerRoot != value)
                { _ExplorerRoot = value;
                    NotifyPropertyChanged(() => ExplorerExt);
                }
            }
        }
        public string ExplorerRoot
        {
            get => _ExplorerRoot;
            set
            {
                if (value != _ExplorerRoot)
                {
                    _ExplorerRoot = value;
                    NotifyPropertyChanged(() => ExplorerRoot);
                }
            }
        }
    }
}
