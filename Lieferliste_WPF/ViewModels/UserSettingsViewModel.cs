using ControlzEx.Theming;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using MaterialDesignColors;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Lieferliste_WPF.ViewModels
{
    internal class UserSettingsViewModel : ViewModelBase
    {
        private ObservableCollection<string> _ExplorerFilter = new();
        public ICollectionView ExplorerFilter { get; }
        private IUserSettingsService _settingsService;
        public ICommand SaveCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand ReloadCommand { get; }
        public ICommand ChangeThemeCommand { get; }
        public string Title { get; } = "Einstellungen";
        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (_isDarkTheme != value)
                {
                    _isDarkTheme = value;
                    NotifyPropertyChanged(() => IsDarkTheme);
                    //ApplyBase(value);
                }
            }
        }
        private Theme? selectedTheme;
        public Theme? SelectedTheme 
        {
            get { return selectedTheme; }
            set
            {
                if (selectedTheme != value)
                {
                    selectedTheme = value;
                    if (value != null) _settingsService.Theme = value.Name;
                }
            }
        }
        public MeasureFirstPartInfo FirstPartInfo { get; private set; }
        public VmpbDocumentInfo VmpbDocumentInfo { get; private set; }
        public WorkareaDocumentInfo WorkareaDocumentInfo { get; private set; }
        public string Froot { get; set; }
        public string Ftemplate { get; set; }
        public string Fregex { get; set; }
        public string Vroot { get; set; }
        public string Vtemplate { get; set; }
        public string Vregex { get; set; }
        public string Wroot { get; set; }
        public string Wregex { get; set; }
        public WorkareaDocumentInfo WorkareaDocument { get; private set; }
        public UserSettingsViewModel(IUserSettingsService settingsService, IContainerExtension container)
        {

            _settingsService = settingsService;
            var br = new BrushConverter();
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            ResetCommand = new ActionCommand(OnResetExecuted, OnResetCanExecute);
            ReloadCommand = new ActionCommand(OnReloadExecuted, OnReloadCanExecute);
            ChangeThemeCommand = new ActionCommand(OnChangeThemeExecuted, OnChangeThemeCanExecute);
            ExplorerFilter = CollectionViewSource.GetDefaultView(_ExplorerFilter);
            SelectedTheme = ThemeManager.Current.DetectTheme(App.Current.MainWindow);
            FirstPartInfo = new MeasureFirstPartInfo(container);
            var Fdocu = FirstPartInfo.CreateDocumentInfos();
            VmpbDocumentInfo = new VmpbDocumentInfo(container);
            var Vdocu = VmpbDocumentInfo.CreateDocumentInfos();
            WorkareaDocumentInfo = new WorkareaDocumentInfo(container);
            var Workdocu = WorkareaDocumentInfo.CreateDocumentInfos();
            Froot = Fdocu[DocumentPart.RootPath];
            Ftemplate = Fdocu[DocumentPart.Template];
            Fregex = Fdocu[DocumentPart.RegularEx];
            Vroot = Vdocu[DocumentPart.RootPath];
            Vtemplate = Vdocu[DocumentPart.Template];
            Vregex = Vdocu[DocumentPart.RegularEx];
            Wroot = Workdocu[DocumentPart.RootPath];
            Wregex = Workdocu[DocumentPart.RegularEx];
        }

        private bool OnChangeThemeCanExecute(object arg)
        {
            throw new NotImplementedException();
        }

        private void OnChangeThemeExecuted(object obj)
        {

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
            _settingsService.Reset();
        }

        private bool OnSaveCanExecute(object arg)
        {
            return _settingsService.IsChanged;
        }

        private void OnSaveExecuted(object obj)
        { 
            _settingsService.Save();
            bool changed = false;
            var docu = FirstPartInfo.GetDocument();
            if (docu[DocumentPart.RootPath] != Froot) { docu[DocumentPart.RootPath] = Froot; changed = true; }
            if (docu[DocumentPart.Template] != Ftemplate) { docu[DocumentPart.Template] = Ftemplate; changed = true; }
            if (docu[DocumentPart.RegularEx] != Fregex) { docu[DocumentPart.RegularEx] = Fregex; changed = true; }

            if (changed) { FirstPartInfo.SaveDocumentData(); }

            changed = false;
            docu = VmpbDocumentInfo.GetDocument();
            if (docu[DocumentPart.RootPath] != Vroot) { docu[DocumentPart.RootPath] = Vroot; changed = true; }
            if (docu[DocumentPart.Template] != Vtemplate) { docu[DocumentPart.Template] = Vtemplate; changed = true; }
            if (docu[DocumentPart.RegularEx] != Vregex) { docu[DocumentPart.RegularEx] = Vregex; changed = true; }

            if (changed) { VmpbDocumentInfo.SaveDocumentData(); }

            changed = false;
            docu = WorkareaDocumentInfo.GetDocument();
            if (docu[DocumentPart.RootPath] != Wroot) { docu[DocumentPart.RootPath] = Wroot; changed = true; }
            if (docu[DocumentPart.RegularEx] != Wregex) { docu[DocumentPart.RegularEx] = Wregex; changed = true; }

            if (changed) { WorkareaDocumentInfo.SaveDocumentData(); }
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
