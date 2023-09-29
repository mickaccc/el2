using Lieferliste_WPF.Data.Models;
using Lieferliste_WPF.Data;
using Lieferliste_WPF.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;

namespace Lieferliste_WPF.ViewModels
{
    class SettingsViewModel : Base.ViewModelBase
    {
        string _ExplorerPathPattern;
        ObservableCollection<string> _ExplorerFilter = new();
        public ICollectionView ExplorerFilter { get; }
        string _ExplorerRoot;
        public Brush OutOfDate { get; set; }
        public Brush InOfDate { get; set; }

        public SettingsViewModel()
        {
            var br = new BrushConverter();
            ExplorerPathPattern = Properties.Settings.Default.ExplorerPath;
            foreach (var item in Properties.Settings.Default.ExplorerFilter)
            {
              _ExplorerFilter.Add(item);
            }
            ExplorerFilter = CollectionViewSource.GetDefaultView(_ExplorerFilter);
            ExplorerExt = Properties.Settings.Default.ExplorerExt;
            ExplorerRoot = Properties.Settings.Default.ExplorerRoot;
            OutOfDate = (Brush?)br.ConvertFromString(Properties.Settings.Default.outOfDate.Name);
            InOfDate = (Brush?)br.ConvertFromString(Properties.Settings.Default.inDate.Name);
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
