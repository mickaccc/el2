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
        string _ExplorerPath;
        ObservableCollection<string> _ExplorerFilter = new();
        public ICollectionView ExplorerFilter { get; }
        string _ExplorerExt;
        public Brush OutOfDate { get; set; }
        public Brush InOfDate { get; set; }

        public SettingsViewModel()
        {
            var br = new BrushConverter();
            ExplorerPath = Properties.Settings.Default.ExplorerPath;
            foreach (var item in Properties.Settings.Default.ExplorerFilter)
            {
              _ExplorerFilter.Add(item);
            }
            ExplorerFilter = CollectionViewSource.GetDefaultView(_ExplorerFilter);
            ExplorerExt = Properties.Settings.Default.ExplorerExt;
            OutOfDate = (Brush?)br.ConvertFromString(Properties.Settings.Default.inDate.Name);
            InOfDate = (Brush?)br.ConvertFromString(Properties.Settings.Default.inDate.Name);
        }
        public string ExplorerPath
        {
            get { return _ExplorerPath; }
            set
            {
                if (_ExplorerPath != value)
                {
                    _ExplorerPath = value;
                    NotifyPropertyChanged(() => ExplorerPath);
                }
            }
        }

        public string ExplorerExt
        {
            get => _ExplorerExt;
            set
            {
                if (_ExplorerExt != value)
                { _ExplorerExt = value;
                    NotifyPropertyChanged(() => ExplorerExt);
                }
            }
        }
    }
}
