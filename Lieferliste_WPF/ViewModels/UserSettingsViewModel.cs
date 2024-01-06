using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Prism.Ioc;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Lieferliste_WPF.ViewModels
{
    internal class UserSettingsViewModel : ViewModelBase
    {
        private string _ExplorerPathPattern;
        private ObservableCollection<string> _ExplorerFilter = new();
        public ICollectionView ExplorerFilter { get; }
        private IUserSettingsService _settingsService;
        public ICommand SaveCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand ReloadCommand { get; }
        public Brush OutOfDate { get; set; }
        public Brush InOfDate { get; set; }
        public string Title { get; } = "Einstellungen";
        public UserSettingsViewModel(IUserSettingsService settingsService)
        {
            
            _settingsService = settingsService;
            var br = new BrushConverter();
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            ResetCommand = new ActionCommand(OnResetExecuted, OnResetCanExecute);
            ReloadCommand = new ActionCommand(OnReloadExecuted, OnReloadCanExecute);
            ExplorerFilter = CollectionViewSource.GetDefaultView(_ExplorerFilter);

        }

        private bool OnReloadCanExecute(object arg)
        {
            return true;
        }

        private void OnReloadExecuted(object obj)
        {
            _settingsService.Reload();
        }

        private bool OnResetCanExecute(object arg)
        {
            return true;
        }

        private void OnResetExecuted(object obj)
        {
            _settingsService?.Reset();
        }

        private bool OnSaveCanExecute(object arg)
        {
            return true;
        }

        private void OnSaveExecuted(object obj)
        {
            _settingsService.Save();
        }

        public string ExplorerPathPattern
        {
            get { return _settingsService.ExplorerPath; }
            set { _settingsService.ExplorerPath = value; }
        }
 
        public string ExplorerRoot
        {
            get { return _settingsService.ExplorerRoot; }
            set { _settingsService.ExplorerRoot = value; }
        }

        public bool AutoSave
        {
            get { return _settingsService.IsAutoSave; }
            set { _settingsService.IsAutoSave = value; }
        }
        public bool SaveMessage
        {
            get { return _settingsService.IsSaveMessage; }
            set { _settingsService.IsSaveMessage = value; }
        }

    }
}
