﻿using ControlzEx.Theming;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.Extensions.Logging;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Lieferliste_WPF.ViewModels
{
    internal class UserSettingsViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private ObservableCollection<string> _ExplorerFilter = new();
        public ICollectionView ExplorerFilter { get; }
        private IUserSettingsService _settingsService;
        private IContainerExtension _container;
        private IEventAggregator _ea;
        ILogger _logger;
        public ICommand SaveCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand ReloadCommand { get; }
        public ICommand PersonalFilterAddCommand { get; }
        public ICommand PersonalFilterRemoveCommand { get; }
        public ICommand PersonalFilterNewCommand { get; }
        public ICommand ArchivatorRuleAddCommand { get; }
        public ICommand ArchivatorRuleRemoveCommand { get; }

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

        public double GlobalFontSize
        {
            get { return _settingsService.FontSize; }
            set
            {
                _settingsService.FontSize = value;
                if (FontSizeValidationRule(_settingsService.FontSize, out string? errorMessage))
                {
                    _errors.Clear();
                    App.GlobalFontSize = value;
                }
                else
                {
                    _errors[nameof(GlobalFontSize)] = [errorMessage];
                }
                if (ErrorsChanged != null)
                    ErrorsChanged(this, new DataErrorsChangedEventArgs(nameof(GlobalFontSize)));               
            }
        }
        public string EmployTimeFormat
        {
            get => Enum.GetName((Formats.TimeFormat)_settingsService.EmployTimeFormat) ?? string.Empty;
            set => _settingsService.EmployTimeFormat = (int)Enum.Parse<Formats.TimeFormat>(value);
        }
        public string[] TimeFormats { get { return Enum.GetNames<Formats.TimeFormat>(); } }
        public double SizePercent
        {
            get { return _settingsService.SizePercent; }
            set { _settingsService.SizePercent = value; }
        }
        public MeasureFirstPartInfo FirstPartInfo { get; private set; }
        public VmpbDocumentInfo VmpbDocumentInfo { get; private set; }
        public WorkareaDocumentInfo WorkareaDocumentInfo { get; private set; }
        public MeasureDocumentInfo MeasureDocumentInfo { get; private set; }
        public Document Fdocu { get; private set; }
        public Document Vdocu { get; private set; }
        public Document Wdocu { get; private set; }
        public Document Mdocu { get; private set; }
        private string _ruleInfoScan = string.Empty;

        public string RuleInfoScan
        {
            get => _ruleInfoScan;
            set { _ruleInfoScan = value; }
        }
        private string _ruleMsfDomain = string.Empty;

        public string RuleMsfDomain
        {
            get => _ruleMsfDomain; 
            set { _ruleMsfDomain = value; }
        }

        private string _DrawingLink;

        public string DrawingLink
        {
            get { return _DrawingLink; }
            set { _DrawingLink = value; }
        }
        private string _ArchivFileExt;

        public string ArchivFileExt
        {
            get { return _ArchivFileExt; }
            set { _ArchivFileExt = value;  Archivator.FileExtensions = value.Split(','); }
        }
        private int _ArchivDelayDays;

        public int ArchivDelayDays
        {
            get { return _ArchivDelayDays; }
            set { _ArchivDelayDays = Archivator.DelayDays = value; }
        }
        private List<ArchivatorRule> _ArchivatorRules = Archivator.ArchiveRules ?? [];

        public ICollectionView ArchivatorRules { get; }
                   
        public string ArchivatorRuleRegEx { get; set; }
        public string ArchivatorRuleFolder { get; set; }
        public Array ArchivRuleTargets { get; } = Enum.GetValues(typeof(Archivator.ArchivatorTarget));
        public string? ArchivRuleTargetSelect { get; set; } = Enum.GetName(Archivator.ArchivatorTarget.TTNR);
        public List<Tuple<string, string, int>> PropertyNames { get; } = [];
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
                    if (_filterContainer.Keys.Contains(_personalFilterName))
                        _filterContainer[_personalFilterName].Pattern = _personalFilterRegex;
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
                    if (_personalFilterField != null)
                        if (_filterContainer.Keys.Contains(_personalFilterName))
                            _filterContainer[_personalFilterName].Field = _personalFilterField.ToValueTuple();
                }
            }
        }
        public ImmutableArray<string> PlanedSetups { get; } =
            ImmutableArray.Create(new string[] { "Setup1", "Setup2" });
        public ICollectionView PersonalFilterView { get; private set; }
        public ObservableCollection<ProjectScheme> ProjectSchemes { get; private set; }
        
        public UserSettingsViewModel(IUserSettingsService settingsService, IContainerExtension container, IEventAggregator eva)
        {
                _ea = eva;
            _settingsService = settingsService;
                _container = container;
                var factory = container.Resolve<ILoggerFactory>();
                _logger = factory.CreateLogger<UserSettingsViewModel>();
            try
            { 
                var br = new BrushConverter();
                SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
                ResetCommand = new ActionCommand(OnResetExecuted, OnResetCanExecute);
                ReloadCommand = new ActionCommand(OnReloadExecuted, OnReloadCanExecute);
                PersonalFilterAddCommand = new ActionCommand(OnPersonalFilterAddExecuted, OnPersonalFilterAddCanExecute);
                PersonalFilterNewCommand = new ActionCommand(OnPersonalFilterNewExecuted, OnPersonalFilterNewCanExecute);
                PersonalFilterRemoveCommand = new ActionCommand(OnPersonalFilterRemoveExecuted, OnPersonalFilterRemoveCanExecute);
                ArchivatorRuleAddCommand = new ActionCommand(OnArchivatorRuleAddExecuted, OnArchivatorRuleAddCanExecute);
                ArchivatorRuleRemoveCommand = new ActionCommand(OnArchivatorRuleRemoveExecuted, OnArchivatorRuleRemoveCanExecute);
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
                ArchivatorRules = CollectionViewSource.GetDefaultView(_ArchivatorRules);
                LoadFilters();
                LoadProjectSchemes();
                LoadRules();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            
        }

        private void LoadRules()
        {

            if (RuleInfo.Rules.TryGetValue("MeasureMsfDomain", out Rule? msf))
                RuleMsfDomain = msf.RuleValue;
            if (RuleInfo.Rules.TryGetValue("MeasureScan", out Rule? scan))
                RuleInfoScan= scan.RuleValue;

            ArchivFileExt = (Archivator.FileExtensions != null) ? string.Join(',', Archivator.FileExtensions) : "";
            ArchivDelayDays = Archivator.DelayDays;
            Archivator.IsChanged = false;
        }
        private void LoadFilters()
        {
            try
            {
                _filterContainer = PersonalFilterContainer.GetInstance();
                _filterContainerKeys = _filterContainer.Keys.ToObservableCollection();
                PersonalFilterView = CollectionViewSource.GetDefaultView(_filterContainerKeys.Skip(1));
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
                PropertyNames.Add(PropertyPair.MarkerCode.ToTuple());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
        private void LoadProjectSchemes()
        {
            ProjectSchemes = RuleInfo.ProjectSchemes.Values.ToObservableCollection();
        }

        private void OnPersonalFilterChanged(object? sender, EventArgs e)
        {

            if (PersonalFilterView.CurrentItem != null)
            {
                var pf = PersonalFilterView.CurrentItem.ToString();
                if (_filterContainer[pf] != null)
                {
                    PersonalFilterName = _filterContainer[pf].Name;
                    PersonalFilterField = _filterContainer[pf].Field.ToTuple();
                    PersonalFilterRegex = _filterContainer[pf].Pattern;
                }
            }
            else
            {
                PersonalFilterName = string.Empty;
                PersonalFilterField = null;
                PersonalFilterRegex = string.Empty;
            }
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
            if (_settingsService != null && _filterContainer != null)
            {
                return _settingsService.IsChanged || _filterContainer.IsChanged || Archivator.IsChanged;
            }
            return false;
        }

        private void OnSaveExecuted(object obj)
        {
            _settingsService.Save();
            FirstPartInfo.SaveDocumentData();
            VmpbDocumentInfo.SaveDocumentData();
            WorkareaDocumentInfo.SaveDocumentData();
            MeasureDocumentInfo.SaveDocumentData();
            
            Globals.SaveRule("MeasureScan", RuleInfoScan);
            Globals.SaveRule("MeasureMsfDomain", RuleMsfDomain);
   
            Globals.SaveProjectSchemes([.. ProjectSchemes]);
            _filterContainer.Save();
            if (Archivator.IsChanged)
            {
                Globals.SaveArchivator();
                
            }
        }
        private bool OnPersonalFilterRemoveCanExecute(object arg)
        {
            return PersonalFilterView.CurrentItem != null;
        }

        private void OnPersonalFilterRemoveExecuted(object obj)
        {
            if (PersonalFilterView.CurrentItem is string curr)
            {
                _filterContainer.Remove(curr);
                _filterContainerKeys.Remove(curr);
                PersonalFilterView.Refresh();
                PersonalFilterView.MoveCurrentToFirst();               
            }
        }

        private bool OnPersonalFilterNewCanExecute(object arg)
        {
            return true;
        }

        private void OnPersonalFilterNewExecuted(object obj)
        {
            if (string.IsNullOrEmpty(PersonalFilterName) == false)
            {
                PersonalFilterName = string.Empty;
                PersonalFilterField = null;
                PersonalFilterRegex = string.Empty;
            }
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

            try
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
                    _logger.LogInformation("{message} ", pf.ToString());
                    PersonalFilterView.Refresh();
                }
            }
            catch (Exception e)
            {

                _logger.LogError("{message}", e.ToString());
            }
        }
        private bool OnArchivatorRuleRemoveCanExecute(object arg)
        {
            return ArchivatorRules.CurrentPosition != -1 && PermissionsProvider.GetInstance().GetUserPermission(Permissions.Archivate);
        }

        private void OnArchivatorRuleRemoveExecuted(object obj)
        {
            _ArchivatorRules.Remove((ArchivatorRule)ArchivatorRules.CurrentItem);
            Archivator.ArchiveRules = _ArchivatorRules;
            ArchivatorRules.Refresh();
        }

        private bool OnArchivatorRuleAddCanExecute(object arg)
        {
            return !string.IsNullOrEmpty(ArchivatorRuleFolder) && !string.IsNullOrEmpty(ArchivatorRuleRegEx)
                && PermissionsProvider.GetInstance().GetUserPermission(Permissions.Archivate);
        }

        private void OnArchivatorRuleAddExecuted(object obj)
        {
            var target = Enum.Parse(typeof(Archivator.ArchivatorTarget), ArchivRuleTargetSelect);
            var item = new ArchivatorRule(ArchivatorRuleRegEx, (Archivator.ArchivatorTarget)target, ArchivatorRuleFolder, ArchivFileExt);
            _ArchivatorRules.Add(item);
            Archivator.ArchiveRules = _ArchivatorRules;
            ArchivatorRules.Refresh();           
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
            set { _settingsService.IsAutoSave = value; _ea.GetEvent<EnableAutoSave>().Publish(value); }
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
        public string PlanedSetup
        {
            get { return _settingsService.PlanedSetup; }
            set { _settingsService.PlanedSetup = value; }
        }
        public int KWReview
        {
            get { return _settingsService.KWReview; }
            set
            {
                _settingsService.KWReview = value;
                if (KwValidationRule(_settingsService.KWReview, out string? errorMessage))
                {
                    _errors.Clear();
                }
                else
                {
                    _errors[nameof(KWReview)] = [errorMessage];
                }
                if (ErrorsChanged != null)
                    ErrorsChanged(this, new DataErrorsChangedEventArgs(nameof(KWReview)));
            }
        }
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName != null)
            {
                if (_errors.ContainsKey(propertyName))
                    return _errors[propertyName];
            }
            return null;

        }
        public bool HasErrors { get {  return _errors.Count > 0; } }
        Dictionary<string, List<string>> _errors = [];
        private bool KwValidationRule(int KW, out string ? errorMessage)
        {
            errorMessage = "";
            bool IsValid = true;

            if (KW < 0 || KW > 50)
            {
                errorMessage = "Der Wert muss zwischen 0 und 50 liegen";
                IsValid = false;
            }
            return IsValid;
        }
        private bool FontSizeValidationRule(double FontSize, out string? errorMessage)
        {
            errorMessage = string.Empty;
            bool IsValid = true;
            if (FontSize < 10 || FontSize > 30)
            {
                errorMessage = "Der Wert muss zwischen 10 und 30 liegen";
                IsValid = false;
            }
            return IsValid;
        }
    }
}
