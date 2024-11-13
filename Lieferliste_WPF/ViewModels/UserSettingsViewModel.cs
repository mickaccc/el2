using ControlzEx.Theming;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using System;
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
    internal class UserSettingsViewModel : ViewModelBase
    {
        private ObservableCollection<string> _ExplorerFilter = new();
        public ICollectionView ExplorerFilter { get; }
        private IUserSettingsService _settingsService;
        private IContainerExtension _container;
        ILogger _logger;
        public ICommand SaveCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand ReloadCommand { get; }
        public ICommand PersonalFilterAddCommand { get; }
        public ICommand PersonalFilterRemoveCommand { get; }
        public ICommand PersonalFilterNewCommand { get; }

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
                App.GlobalFontSize = value;
            }
        }

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
        private string _ruleInfoScan = RuleInfo.Rules["MeasureScan"].RuleValue;

        public string RuleInfoScan
        {
            get { return _ruleInfoScan = RuleInfo.Rules["MeasureScan"].RuleValue; }
            set { _ruleInfoScan = RuleInfo.Rules["MeasureScan"].RuleValue = value; }
        }

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

        public UserSettingsViewModel(IUserSettingsService settingsService, IContainerExtension container)
        {

            _settingsService = settingsService;
            _container = container;
            var factory = container.Resolve<ILoggerFactory>();
            _logger = factory.CreateLogger<UserSettingsViewModel>();
            var br = new BrushConverter();
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            ResetCommand = new ActionCommand(OnResetExecuted, OnResetCanExecute);
            ReloadCommand = new ActionCommand(OnReloadExecuted, OnReloadCanExecute);
            PersonalFilterAddCommand = new ActionCommand(OnPersonalFilterAddExecuted, OnPersonalFilterAddCanExecute);
            PersonalFilterNewCommand = new ActionCommand(OnPersonalFilterNewExecuted, OnPersonalFilterNewCanExecute);
            PersonalFilterRemoveCommand = new ActionCommand(OnPersonalFilterRemoveExecuted, OnPersonalFilterRemoveCanExecute);
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

        }

        private void LoadFilters()
        {
            _filterContainer = PersonalFilterContainer.GetInstance();
            _filterContainer.Remove("_keine");
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
            PropertyNames.Add(PropertyPair.MarkerCode.ToTuple());
        }


        private void OnPersonalFilterChanged(object? sender, EventArgs e)
        {

            if (PersonalFilterView.CurrentItem != null)
            {
                var pf = PersonalFilterView.CurrentItem.ToString();
                if (_filterContainer.Keys.Contains(pf))
                {
                    PersonalFilterName = _filterContainer[pf].Name;
                    PersonalFilterField = _filterContainer[pf].Field.ToTuple();
                    PersonalFilterRegex = _filterContainer[pf].Pattern;
                }
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

            return _settingsService.IsChanged || _filterContainer.IsChanged;
        }

        private void OnSaveExecuted(object obj)
        {
            _settingsService.Save();
            FirstPartInfo.SaveDocumentData();
            VmpbDocumentInfo.SaveDocumentData();
            WorkareaDocumentInfo.SaveDocumentData();
            MeasureDocumentInfo.SaveDocumentData();
            var gl = new Globals(_container);
            gl.SaveRule(RuleInfo.Rules["MeasureScan"]);
            _filterContainer.Save();

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
                PersonalFilterName = (string)PersonalFilterView.CurrentItem;
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
                }
            }
            catch (Exception e)
            {

                _logger.LogError("{message}", e.ToString());
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
        public string PlanedSetup
        {
            get { return _settingsService.PlanedSetup; }
            set { _settingsService.PlanedSetup = value; }
        }
    }
}
