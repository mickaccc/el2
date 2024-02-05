﻿using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
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
        private string _ExplorerPathPattern;
        private ObservableCollection<string> _ExplorerFilter = new();
        public ICollectionView ExplorerFilter { get; }
        private IUserSettingsService _settingsService;
        public ICommand SaveCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand ReloadCommand { get; }
        public ICommand ChangeThemeCommand { get; }
        public Brush OutOfDate { get; set; }
        public Brush InOfDate { get; set; }
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
        public UserSettingsViewModel(IUserSettingsService settingsService)
        {

            _settingsService = settingsService;
            var br = new BrushConverter();
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            ResetCommand = new ActionCommand(OnResetExecuted, OnResetCanExecute);
            ReloadCommand = new ActionCommand(OnReloadExecuted, OnReloadCanExecute);
            ChangeThemeCommand = new ActionCommand(OnChangeThemeExecuted, OnChangeThemeCanExecute);
            ExplorerFilter = CollectionViewSource.GetDefaultView(_ExplorerFilter);

            //Swatches = new SwatchesProvider().Swatches;

            //PaletteHelper paletteHelper = new PaletteHelper();
            //Theme theme = paletteHelper.GetTheme();

            //IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark;

        }


        //private static void ApplyStyle(bool alternate)
        //{
        //    var resourceDictionary = new ResourceDictionary
        //    {
        //        Source = new Uri(@"pack://application:,,,/Dragablz;component/Themes/materialdesign.xaml")
        //    };

        //    var styleKey = alternate ? "MaterialDesignAlternateTabablzControlStyle" : "MaterialDesignTabablzControlStyle";
        //    var style = (Style)resourceDictionary[styleKey];

        //    foreach (var tabablzControl in Dragablz.TabablzControl.GetLoadedInstances())
        //    {
        //        tabablzControl.Style = style;
        //    }
        //}

        //private static void ApplyBase(bool isDark)
        //    => ModifyTheme(theme => theme.SetBaseTheme(isDark ? BaseTheme.Dark : BaseTheme.Light));

        private static void ApplyPrimary(Swatch swatch)
            => ModifyTheme(theme => theme.SetPrimaryColor(swatch.ExemplarHue.Color));

        private static void ApplyAccent(Swatch swatch)
        {
            if (swatch.AccentExemplarHue is Hue accentHue)
            {
                ModifyTheme(theme => theme.SetSecondaryColor(accentHue.Color));
            }
        }

        private static void ModifyTheme(Action<Theme> modificationAction)
        {
            //PaletteHelper paletteHelper = new PaletteHelper();
            //Theme theme = paletteHelper.GetTheme();

            //modificationAction?.Invoke(theme);

            //paletteHelper.SetTheme(theme);
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
