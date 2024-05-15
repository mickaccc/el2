﻿using ControlzEx.Theming;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using ModulePlanning.Specials;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
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
        public ICommand PersonalFilterAddCommand { get; }
        public ICommand PersonalFilterRemoveCommand { get; }
        public ICommand PersonalFilterNewCommand { get; }
        public ICommand TlColumnAddCommand { get; }
        public ICommand TlColumnRemoveCommand { get; }
        private RelayCommand? _ColumnsChangedCommand;
        public ICommand ColumnsChangedCommand => _ColumnsChangedCommand ??= new RelayCommand(OnColumnChanged);

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
        public MeasureDocumentInfo MeasureDocumentInfo { get; private set; }
        public Document Fdocu {  get; private set; }
        public Document Vdocu { get; private set; }
        public Document Wdocu { get; private set; }
        public Document Mdocu { get; private set; }
        public List<Tuple<string, string, int>> PropertyNames { get; } = [];
        public IEnumerable<string> ColumnKeys { get; } = Constances.TLColumn.ColumnNames.Keys;
        public CollectionView TlColumnsView { get; private set; }
        public string TlColumnCurrent { get; set; }
        private PersonalFilterContainer _filterContainer;
        private ObservableCollection<string> _filterContainerKeys;
        private string _personalFilterName;
        public string PersonalFilterName
        {
            get { return _personalFilterName; }
            set
            {
                if (_personalFilterName != value)
                {
                    _personalFilterName = value;
                    NotifyPropertyChanged(() => PersonalFilterName);
                }
            }
        }
        private string _personalFilterRegex;
        public string PersonalFilterRegex
        {
            get { return _personalFilterRegex; }
            set
            {
                if (_personalFilterRegex != value)
                {
                    _personalFilterRegex = value;
                    NotifyPropertyChanged(() => PersonalFilterRegex);
                }
            }
        }
        private Tuple<string, string, int>? _personalFilterField;
        public Tuple<string, string, int>? PersonalFilterField
        {
            get { return _personalFilterField; }
            set
            {
                if (_personalFilterField != value)
                {
                    _personalFilterField = value;
                    NotifyPropertyChanged(() => PersonalFilterField);
                }
            }
        }
        public ICollectionView PersonalFilterView { get; private set; }
        public UserSettingsViewModel(IUserSettingsService settingsService, IContainerExtension container)
        {

            _settingsService = settingsService;
            var br = new BrushConverter();
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            ResetCommand = new ActionCommand(OnResetExecuted, OnResetCanExecute);
            ReloadCommand = new ActionCommand(OnReloadExecuted, OnReloadCanExecute);
            PersonalFilterAddCommand = new ActionCommand(OnPersonalFilterAddExecuted, OnPersonalFilterAddCanExecute);
            PersonalFilterNewCommand = new ActionCommand(OnPersonalFilterNewExecuted, OnPersonalFilterNewCanExecute);
            PersonalFilterRemoveCommand = new ActionCommand(OnPersonalFilterRemoveExecuted, OnPersonalFilterRemoveCanExecute);
            TlColumnAddCommand = new ActionCommand(OnTlColumnAddExecuted, OnTlColumnAddCanExecute);
            TlColumnRemoveCommand = new ActionCommand(OnTlColumnRemoveExecuted, OnTlColumnRemoveCanExecute);
            ExplorerFilter = CollectionViewSource.GetDefaultView(_ExplorerFilter);
            SelectedTheme = ThemeManager.Current.DetectTheme(App.Current.MainWindow);
            FirstPartInfo = new MeasureFirstPartInfo(container);
            Fdocu = FirstPartInfo.CreateDocumentInfos();
            VmpbDocumentInfo = new VmpbDocumentInfo(container);
            Vdocu = VmpbDocumentInfo.CreateDocumentInfos();
            WorkareaDocumentInfo = new WorkareaDocumentInfo(container);
            Wdocu = WorkareaDocumentInfo.CreateDocumentInfos();
            MeasureDocumentInfo = new MeasureDocumentInfo(container);
            Mdocu = MeasureDocumentInfo.CreateDocumentInfos();
            LoadFilters();
            TlColumnsView = (CollectionView?)CollectionViewSource.GetDefaultView(TlColumns);
        }



        private void LoadFilters()
        {
            _filterContainer = PersonalFilterContainer.GetInstance();
            _filterContainerKeys = _filterContainer.Keys.ToObservableCollection();
            PersonalFilterView = CollectionViewSource.GetDefaultView(_filterContainerKeys);
            PersonalFilterView.MoveCurrentToFirst();
            //if(PersonalFilterView.CurrentItem != null)
            //    PersonalFilterContainerItem = pfilter[PersonalFilterView.CurrentItem.ToString()];
            PersonalFilterView.CurrentChanged += OnPersonalFilterChanged;
            PropertyNames.Add(PropertyPair.OrderNumber.ToTuple());
            PropertyNames.Add(PropertyPair.ProcessDescription.ToTuple());
            PropertyNames.Add(PropertyPair.Material.ToTuple());
            PropertyNames.Add(PropertyPair.MaterialDescription.ToTuple());
            PropertyNames.Add(PropertyPair.LieferTermin.ToTuple());
            PropertyNames.Add(PropertyPair.PrioText.ToTuple());
            PropertyNames.Add(PropertyPair.RessourceName.ToTuple());
            PropertyNames.Add(PropertyPair.Project.ToTuple());
            PropertyNames.Add(PropertyPair.ProjectInfo.ToTuple());
        }
        private void OnColumnChanged(object obj)
        {
            var curr = TlColumnsView.CurrentItem;
            TlColumns = TlColumns; //UserSettingsService must Check Changes
        }
        private void OnPersonalFilterChanged(object? sender, EventArgs e)
        {

            if (PersonalFilterView.CurrentItem != null)
            {
                var pf = PersonalFilterView.CurrentItem.ToString();
                PersonalFilterName = _filterContainer[pf].Name;
                PersonalFilterField = _filterContainer[pf].Field.ToTuple();
                PersonalFilterRegex = _filterContainer[pf].Pattern;
            }
        }
        private bool OnTlColumnRemoveCanExecute(object arg)
        {
            return true;
        }

        private void OnTlColumnRemoveExecuted(object obj)
        {
            var tl = TlColumns;
            tl.RemoveAt(TlColumns.Count-1);
            TlColumns = tl;     //because UserSettingsService should be changed
            TlColumnsView.Refresh();
        }

        private bool OnTlColumnAddCanExecute(object arg)
        {
            return true;
        }

        private void OnTlColumnAddExecuted(object obj)
        {
            var t = Constances.TLColumn.ColumnNames.First();
            var tl = TlColumns;
            tl.Add(t.Key);
            TlColumns = tl;           //because UserSettingsService should be changed
            TlColumnsView.Refresh();
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

            return _settingsService.IsChanged || _filterContainer.IsChanged;
        }

        private void OnSaveExecuted(object obj)
        { 
            _settingsService.Save();
            FirstPartInfo.SaveDocumentData();
            VmpbDocumentInfo.SaveDocumentData();
            WorkareaDocumentInfo.SaveDocumentData();
            MeasureDocumentInfo.SaveDocumentData();
            _filterContainer.Save();
        }
        private bool OnPersonalFilterRemoveCanExecute(object arg)
        {
            return PersonalFilterView.CurrentItem != null;
        }

        private void OnPersonalFilterRemoveExecuted(object obj)
        {
            var curr = PersonalFilterView.CurrentItem as string;
            if (curr != null)
            {
                _filterContainer.Remove(curr);
                _filterContainerKeys.Remove(curr);
                PersonalFilterName = (string)PersonalFilterView.CurrentItem;
            }
            
        }

        private bool OnPersonalFilterNewCanExecute(object arg)
        {
            return true;
        }

        private void OnPersonalFilterNewExecuted(object obj)
        {
            PersonalFilterName = string.Empty;
            PersonalFilterField = null;
            PersonalFilterRegex = string.Empty;           
        }

        private bool OnPersonalFilterAddCanExecute(object arg)
        {
            var acc = !string.IsNullOrEmpty(PersonalFilterName) &&
                !string.IsNullOrEmpty(PersonalFilterRegex) &&
                PersonalFilterField != null &&
                !PersonalFilterView.Contains(PersonalFilterName);

            return acc;
        }

        private void OnPersonalFilterAddExecuted(object obj)
        {
            
            if (string.IsNullOrEmpty(PersonalFilterName) ||
                string.IsNullOrEmpty(PersonalFilterRegex) ||
                PersonalFilterField == null) return;
            
            PersonalFilter? pf = null;
            switch (PersonalFilterField.Item3)
            {
                case 1:
                    pf = new PersonalFilterVorgang(
                        PersonalFilterName, PersonalFilterRegex, (PersonalFilterField.Item1, PersonalFilterField.Item2, PersonalFilterField.Item3));
                    break;
                case 2:
                    pf = new PersonalFilterOrderRb(
                        PersonalFilterName, PersonalFilterRegex, (PersonalFilterField.Item1, PersonalFilterField.Item2, PersonalFilterField.Item3));
                    break;
                case 3:
                    pf = new PersonalFilterMaterial(
                        PersonalFilterName, PersonalFilterRegex, (PersonalFilterField.Item1, PersonalFilterField.Item2, PersonalFilterField.Item3));
                    break;
                case 4:
                    pf = new PersonalFilterRessource(
                        PersonalFilterName, PersonalFilterRegex, (PersonalFilterField.Item1, PersonalFilterField.Item2, PersonalFilterField.Item3));
                    break;
                case 5:
                    pf = new PersonalFilterProject(
                        PersonalFilterName, PersonalFilterRegex, (PersonalFilterField.Item1, PersonalFilterField.Item2, PersonalFilterField.Item3));
                    break;
            }
            if (pf != null)
            {
                _filterContainer.Add(pf.Name, pf);
                _filterContainerKeys.Add(pf.Name);
            }
        }

        public string ExplorerPathPattern
        {
            get { return _settingsService.ExplorerPath; }
            set { _settingsService.ExplorerPath = value; }
        }

        public string PersonalFolder
        {
            get { return _settingsService.PersonalFolder; }
            set { _settingsService.PersonalFolder = value; }
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
  
        public bool RowDetails
        {
            get { return _settingsService.IsRowDetails; }
            set { _settingsService.IsRowDetails = value; }
        }
        public StringCollection TlColumns
        {
            get { return _settingsService.TlColumns; }
            set
            {
                _settingsService.TlColumns = value;
                
            }
        }
    }
}
