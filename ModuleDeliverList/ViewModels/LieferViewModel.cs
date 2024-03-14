using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModuleDeliverList.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0")]
    internal class LieferViewModel : ViewModelBase, IViewModel
    {

        public ICollectionView OrdersView { get; private set; }
        public ICommand TextSearchCommand => _textSearchCommand ??= new RelayCommand(OnTextSearch);
        public ICommand FilterDeleteCommand => _filterDeleteCommand ??= new RelayCommand(OnFilterDelete);
        public ICommand ToggleFilterCommand => _toggleFilterCommand ??= new RelayCommand(OnToggleFilter);
        public ICommand SortAscCommand => _sortAscCommand ??= new RelayCommand(OnAscSortExecuted);
        public ICommand SortDescCommand => _sortDescCommand ??= new RelayCommand(OnDescSortExecuted);
        public ICommand ProjectPrioCommand { get; private set; }
        private ConcurrentObservableCollection<Vorgang> _orders { get; } = [];
        private DB_COS_LIEFERLISTE_SQLContext DBctx { get; set; }
        public ActionCommand SaveCommand { get; private set; }
        public ActionCommand FilterSaveCommand { get; private set; }
        public ActionCommand InvisibilityCommand { get; private set; }
        public string Title { get; } = "Lieferliste";
        public bool HasChange => DBctx.ChangeTracker.HasChanges();

        private readonly Dictionary<string, string> _filterCriterias = new();
        private readonly string _sortField = string.Empty;
        private readonly string _sortDirection = string.Empty;
        private RelayCommand? _textSearchCommand;
        private RelayCommand? _filterDeleteCommand;
        private RelayCommand? _toggleFilterCommand;
        private RelayCommand? _sortAscCommand;
        private RelayCommand? _sortDescCommand;
        private string _searchFilterText = string.Empty;
        private string _selectedProjectFilter = string.Empty;
        private string _selectedSectionFilter = string.Empty;
        private string _markerCode = string.Empty;
        private static System.Timers.Timer? _timer;
        private static System.Timers.Timer? _autoSaveTimer;
        private IContainerProvider _container;
        private IEventAggregator _ea;
        private IUserSettingsService _settingsService;
        private CmbFilter _selectedDefaultFilter;
        private static List<Ressource> _ressources = [];
        private static SortedDictionary<int, string> _sections = [];
        public SortedDictionary<int, string> Sections => _sections;
        private ObservableCollection<ProjectStruct> _projects = new();
        public ObservableCollection<ProjectStruct> Projects
        {
            get { return _projects; }
            set
            {
                if (_projects != value)
                {
                    _projects = value;
                    NotifyPropertyChanged(() => Projects);
                }
            }
        }

        private bool _filterInvers;

        public bool FilterInvers
        {
            get { return _filterInvers; }
            set
            {
                if (_filterInvers != value)
                {
                    _filterInvers = value;
                    NotifyPropertyChanged(() => FilterInvers);
                    OrdersView.Refresh();
                }
            }
        }
        public string SearchFilterText
        {
            get { return _searchFilterText; }
            set
            {
                if (value != _searchFilterText)
                {
                    _searchTxtLock = true;
                    _searchFilterText = value;
                    NotifyPropertyChanged(() => SearchFilterText);
                    OrdersView.Refresh();
                    _searchTxtLock = false;
                }
            }
        }
        public string MarkerCode
        {
            get { return _markerCode; }
            set
            {
                if (value != _markerCode)
                {
                    _markerCode = value;
                    NotifyPropertyChanged(() => MarkerCode);
                    OrdersView.Refresh();
                }
            }
        }
        private static readonly object _lock = new();
        private static bool _searchTxtLock;
        private IApplicationCommands _applicationCommands;

        public IApplicationCommands ApplicationCommands
        {
            get => _applicationCommands;
            set
            {
                if (_applicationCommands != value)
                    _applicationCommands = value;
                NotifyPropertyChanged(() => ApplicationCommands);
            }
        }
        public NotifyTaskCompletion<ICollectionView>? OrderTask { get; private set; }

        internal CollectionViewSource OrdersViewSource { get; private set; } = new();

        public enum CmbFilter
        {
            [Description("Leer")]
            // ReSharper disable once InconsistentNaming
            NOT_SET = 0,
            [Description("ausgeblendet")]
            INVISIBLE,
            [Description("zum ablegen")]
            READY,
            [Description("Aufträge zum Starten")]
            START,
            [Description("Entwicklungsmuster")]
            DEVELOP,
            [Description("Verkaufsmuster")]
            SALES,
            [Description("Projekte mit Verzug")]
            PROJECTS_LOST,
            [Description("rote Aufträge")]
            ORDERS_RED,
            [Description("rote Projekte")]
            PROJECTS_RED,
            [Description("EXTERN")]
            EXERTN
        }
        public CmbFilter SelectedDefaultFilter
        {
            get => _selectedDefaultFilter;
            set
            {
                if (_selectedDefaultFilter != value)
                {
                    _selectedDefaultFilter = value;
                    NotifyPropertyChanged(() => SelectedDefaultFilter);
                    OrdersView.Refresh();
                }
            }
        }
        public string SelectedProjectFilter
        {
            get => _selectedProjectFilter;
            set
            {
                if (value != _selectedProjectFilter)
                {
                    _selectedProjectFilter = value;
                    NotifyPropertyChanged(() => SelectedProjectFilter);
                    OrdersView.Refresh();
                }
            }
        }

        public string SelectedSectionFilter
        {
            get { return _selectedSectionFilter; }
            set
            {
                if (_selectedSectionFilter != value)
                {
                    _selectedSectionFilter = value;
                    NotifyPropertyChanged(() => SelectedSectionFilter);
                    OrdersView.Refresh();
                }
            }
        }


        public LieferViewModel(IContainerProvider container,
            IApplicationCommands applicationCommands,
            IEventAggregator ea,
            IUserSettingsService settingsService)
        {
            _applicationCommands = applicationCommands;
            DBctx = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            _container = container;
            _ea = ea;
            _settingsService = settingsService;

            InvisibilityCommand = new ActionCommand(OnInvisibilityExecuted, OnInvisibilityCanExecute);
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            FilterSaveCommand = new ActionCommand(OnFilterSaveExecuted, OnFilterSaveCanExecute);
            ProjectPrioCommand = new ActionCommand(OnSetProjectPrioExecuted, OnSetProjectPrioCanExecute);
            OrderTask = new NotifyTaskCompletion<ICollectionView>(LoadDataAsync());

            _ea.GetEvent<MessageVorgangChanged>().Subscribe(MessageVorgangReceived);
            _ea.GetEvent<MessageOrderChanged>().Subscribe(MessageOrderReceived);
            _ea.GetEvent<MessageOrderArchivated>().Subscribe(MessageOrderArchivated);

            if (_settingsService.IsAutoSave) SetAutoSave();

        }

        private bool OnSetProjectPrioCanExecute(object arg)
        {
            if(arg is Vorgang v)
                if(v.AidNavigation.Pro != null)
                    return PermissionsProvider.GetInstance().GetUserPermission(Permissions.ProjectPrio);
            return false;
        }

        private void OnSetProjectPrioExecuted(object obj)
        {
            if (obj is Vorgang v)
            {
                if (v.AidNavigation.Pro != null)
                    v.AidNavigation.Pro.ProjectPrio = !v.AidNavigation.Pro.ProjectPrio;
            }
        }

        private bool OnInvisibilityCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.LieferVrgInvis);
        }

        private void OnInvisibilityExecuted(object obj)
        {
            if (obj is Vorgang v)
            {
                v.Visability = !v.Visability;
                OrdersView.Refresh();
            }
        }

        private void MessageOrderArchivated(OrderRb rb)
        {
            try
            {
                lock (_lock)
                {
                    var o = _orders.FirstOrDefault(x => x.Aid == rb.Aid);
                    if (o != null)
                    {

                        if (rb.Abgeschlossen)
                        {
                            _orders.Remove(o);
                        }
                        DBctx.ChangeTracker.Entries<OrderRb>().First(x => x.Entity.Aid == rb.Aid).State = EntityState.Unchanged;
                        OrdersView.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MsgReceivedArchivated", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void MessageOrderReceived(List<string?> rb)
        {
            try
            {
                Task.Run(() =>
                {
                    lock (_lock)
                    {
                        foreach (string rbId in rb.Where(x => x != null))
                        {
                            var o = _orders.FirstOrDefault(x => x.Aid == rbId);
                            if (o != null)
                            {
                                if (DBctx.ChangeTracker.Entries<Vorgang>().First(x => x.Entity.VorgangId == o.VorgangId).State != EntityState.Unchanged)
                                {
                                    DBctx.ChangeTracker.Entries<Vorgang>().First(x => x.Entity.VorgangId == o.VorgangId).State = EntityState.Unchanged;
                                }
                                DBctx.ChangeTracker.Entries<Vorgang>().First(x => x.Entity.VorgangId == o.VorgangId).Reload();
                                o.RunPropertyChanged();
                            }
                            else
                            {
                                foreach (var v in DBctx.Vorgangs.Where(x => x.Aid == rbId))
                                {
                                    Application.Current.Dispatcher.Invoke(AddRelevantProcess, v.VorgangId);
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MsgReceivedLieferlisteOrder", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void MessageVorgangReceived(List<string?> vrgIdList)
        {
            try
            {
                Task.Run(() =>
         {
             lock (_lock)
             {
                 foreach (var vrg in vrgIdList.Where(x => x != null))
                 {
                     var v = _orders.FirstOrDefault(x => x.VorgangId == vrg);
                     if (v != null)
                     {
                         DBctx.ChangeTracker.Entries<Vorgang>().First(x => x.Entity.VorgangId == v.VorgangId).Reload();
                         v.RunPropertyChanged();
                         if (v.Aktuell == false)
                         {
                             _orders.Remove(v);
                             DBctx.ChangeTracker.Entries<Vorgang>().First(x => x.Entity.VorgangId == v.VorgangId).State = EntityState.Unchanged;
                         }
                     }
                     else if (DBctx.Vorgangs.First(x => x.VorgangId.Trim() == vrg).Aktuell)
                     {
                         if (vrg != null)
                             Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, AddRelevantProcess, vrg);
                     }
                 }
             }
         });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MsgReceivedLieferlisteVorgang", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool OrdersView_FilterPredicate(object value)
        {

            var ord = (Vorgang)value;

            var accepted = ord.Aktuell;

            if (accepted && _selectedDefaultFilter == CmbFilter.NOT_SET) accepted = ord.Visability;

            if (!string.IsNullOrWhiteSpace(_searchFilterText))
            {
                if (!(accepted = ord.Aid.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase)))
                    if (!(accepted = ord.AidNavigation.Material?.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase) ?? false))
                        accepted = ord.AidNavigation.MaterialNavigation?.Bezeichng?.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase) ?? false;
            }

            if (accepted && _selectedDefaultFilter == CmbFilter.INVISIBLE) accepted = !ord.Visability == !FilterInvers;
            if (accepted && _selectedDefaultFilter == CmbFilter.READY) accepted = ord.AidNavigation.Fertig == !FilterInvers;
            if (accepted && _selectedDefaultFilter == CmbFilter.START)
                accepted = (ord.Text?.Contains("STARTEN", StringComparison.CurrentCultureIgnoreCase) ?? false) == !FilterInvers;
            if (accepted && _selectedDefaultFilter == CmbFilter.SALES) accepted = (ord.AidNavigation.Pro?.ProjectType ==
                    (int)ProjectTypes.ProjectType.SaleSpecimen) == !FilterInvers;
            if (accepted && _selectedDefaultFilter == CmbFilter.DEVELOP) accepted = (ord.AidNavigation.Pro?.ProjectType ==
                    (int)ProjectTypes.ProjectType.DevelopeSpecimen) == !FilterInvers;
            if (accepted && _selectedDefaultFilter == CmbFilter.EXERTN) accepted = (ord.ArbPlSap == "_EXTERN_") == !FilterInvers;
            if (accepted) accepted = !ord.AidNavigation.Abgeschlossen;
            if (accepted && !string.IsNullOrEmpty(_selectedProjectFilter)) accepted = ord.AidNavigation.ProId == _selectedProjectFilter;
            if (accepted && _selectedSectionFilter != string.Empty) accepted = _ressources?
                    .FirstOrDefault(x => x.Inventarnummer == ord.ArbPlSap?[3..])?
                    .WorkArea?.Bereich == _selectedSectionFilter;
            if (accepted && _markerCode != string.Empty) accepted = ord.AidNavigation.MarkCode?.Contains(_markerCode, StringComparison.InvariantCultureIgnoreCase) ?? false;
            if (accepted && _selectedDefaultFilter == CmbFilter.PROJECTS_LOST) accepted = ProjectsLost(ord.AidNavigation.Pro) == !FilterInvers;
            if (accepted && _selectedDefaultFilter == CmbFilter.ORDERS_RED) accepted = ord.AidNavigation.Prio?.Length > 0 == !FilterInvers;
            if (accepted && _selectedDefaultFilter == CmbFilter.PROJECTS_RED) accepted = ord.AidNavigation.Pro?.ProjectPrio == !FilterInvers;
            return accepted;
        }

        private bool ProjectsLost(Project? pro)
        {
            if (pro != null)
            {
                foreach(var item in pro.OrderRbs.Where(x => x.Abgeschlossen == false))
                {
                    if(item.Vorgangs.Any(x => x.SpaetEnd < DateTime.Now && x.QuantityMiss > 0)) return true;
                }
            }
            return false;
        }

        private void OnTextSearch(object commandParameter)
        {
            if (commandParameter is string search)

                if (!_searchTxtLock && search.Length >= 3)
                    SearchFilterText = search;
        }

        private void SetAutoSave()
        {
            _autoSaveTimer = new System.Timers.Timer(60000);
            _autoSaveTimer.Elapsed += OnAutoSave;
            _autoSaveTimer.AutoReset = true;
            _autoSaveTimer.Enabled = true;
        }

        private void OnAutoSave(object? sender, ElapsedEventArgs e)
        {
            try
            {
                if (DBctx.ChangeTracker.HasChanges()) DBctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "AutoSave", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Commands
        private bool OnFilterSaveCanExecute(object arg)
        {
            return false;
        }

        private void OnFilterSaveExecuted(object obj)
        {
            throw new NotImplementedException();
        }
        private void OnFilterDelete(object obj)
        {
            SearchFilterText = string.Empty;
            SelectedDefaultFilter = CmbFilter.NOT_SET;
            SelectedProjectFilter = string.Empty;
            SelectedSectionFilter = string.Empty;
            MarkerCode = string.Empty;
            FilterInvers = false;
        }
        private void OnToggleFilter(object obj)
        {
            FilterInvers = !FilterInvers;
        }


        private void OnSaveExecuted(object obj)
        {
            try
            {
                DBctx.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}\nInnerEx\n{1}",e.Message,e.InnerException), "OnSave Liefer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool OnSaveCanExecute(object arg)
        {
            try
            {
                return DBctx.ChangeTracker.HasChanges();
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.Message, "CanSave Liefer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }

        private void OnDescSortExecuted(object parameter)
        {
            var v = Translate();
            if (v != string.Empty)
            {
                OrdersViewSource.SortDescriptions.Clear();
                OrdersViewSource.SortDescriptions.Add(new SortDescription(v, ListSortDirection.Descending));
                OrdersView.Refresh();
            }
        }

        private void OnAscSortExecuted(object parameter)
        {

            var v = Translate();

            if (v != string.Empty)
            {
                OrdersView.SortDescriptions.Clear();
                OrdersView.SortDescriptions.Add(new SortDescription(v, ListSortDirection.Ascending));
                OrdersView.Refresh();
            }
        }
        #endregion

        public string HasMouse { get; set; }
        private string Translate()
        {
            string v = HasMouse switch
            {
                "txtOrder" => "Aid",
                "txtVnr" => "Vorgangs.Vnr",
                "txtMatText" => "MaterialNavigation.Bezeichng",
                "txtMatTTNR" => "Material",
                "txtVrgText" => "Vorgangs.Text",
                "txtPlanT" => "Termin",
                "txtProj" => "ProId",
                "txtPojInfo" => "ProInfo",
                "txtQuant" => "Quantity",
                "txtWorkArea" => "Vorgangs.ArbplSap",
                "txtQuantYield" => "Vorgangs.QuantityYield",
                "txtScrap" => "Vorgangs.QuantityScrap",
                "txtOpen" => "Vorgangs.QuantityMiss",
                "txtRessTo" => "Vorgangs.RidNavigation.RessName",
                _ => string.Empty,
            };

            return v;
        }

        public async Task<ICollectionView> LoadDataAsync()
        {
            //_projects.Add(new Project() { ProjectPsp = "leer"});

            if (!_sections.Keys.Contains(0)) _sections.Add(0, string.Empty);
            var a = await DBctx.Vorgangs
               .Include(v => v.AidNavigation)
               .ThenInclude(x => x.MaterialNavigation)
               .Include(r => r.RidNavigation)
               .Include(m => m.AidNavigation.DummyMatNavigation)
               .Include(d => d.AidNavigation.Pro)
               .ThenInclude(x => x.ProjectAttachments)
               .Include(v => v.ArbPlSapNavigation)
               .Where(x => x.AidNavigation.Abgeschlossen == false)
               .ToListAsync();
            var ress = await DBctx.Ressources.AsNoTracking()
                .Include(x => x.WorkArea)
                .ToArrayAsync();
            var filt = await DBctx.ProductionOrderFilters.AsNoTracking().ToArrayAsync();
            _ressources.AddRange(ress);
            SortedSet<ProjectStruct> pl = [new(string.Empty, ProjectTypes.ProjectType.None, string.Empty)];
            await Task.Factory.StartNew(() =>
            {
                HashSet<Vorgang> result = new();

                lock (_lock)
                {
                    SortedDictionary<string, ProjectTypes.ProjectType> proj = new();
 
                        bool relev = false;
                    foreach (var group in a.GroupBy(x => x.Aid))
                    {
                        if (filt.Any(y => y.OrderNumber == group.Key))
                        {
                            relev = true;
                        }
                        else
                        {

                            foreach (var vorg in group)
                            {

                                if (vorg.ArbPlSap?.Length >= 3 && !relev)
                                {
                                    if (int.TryParse(vorg.ArbPlSap[..3], out int c))
                                        if (UserInfo.User.UserCosts.Any(y => y.CostId == c))
                                        {
                                            relev = true;
                                            break;
                                        }
                                }

                            }
                            if (relev)
                            {
                                foreach (var vorg in group)
                                {
                                    if (vorg.AidNavigation.ProId != null)
                                    {
                                        var p = vorg.AidNavigation.Pro;
                                        if (p != null)
                                            pl.Add(new(p.ProjectPsp.Trim(), (ProjectTypes.ProjectType)p.ProjectType, p.ProjectInfo));
                                    }

                                    if (vorg.Aktuell)
                                    {
                                        result.Add(vorg);
                                        var inv = (vorg.ArbPlSap != null) ? vorg.ArbPlSap[3..] : string.Empty;
                                        var z = ress.FirstOrDefault(x => x.Inventarnummer?.Trim() == inv)?.WorkArea;
                                        if (z != null)
                                        {
                                            if (!_sections.Keys.Contains(z.Sort))
                                                _sections.Add(z.Sort, z.Bereich);
                                        }
                                    }
                                }
                            }
                        }
                        relev = false;
                                               
                    }
                    _orders.AddRange(result.OrderBy(x => x.SpaetEnd));
                }
            });
            Projects.AddRange(pl);
            OrdersView = CollectionViewSource.GetDefaultView(_orders);
            OrdersView.Filter += OrdersView_FilterPredicate;
            ICollectionViewLiveShaping? live = OrdersView as ICollectionViewLiveShaping;
            if (live != null)
            {
                if (live.CanChangeLiveFiltering)
                {
                    live.LiveFilteringProperties.Add("Aktuell");
                    live.LiveFilteringProperties.Add("AidNavigation.Abgeschlossen");
                    live.IsLiveFiltering = true;
                    live.IsLiveSorting = false;
                }
            }
            return OrdersView;
        }
        private bool AddRelevantProcess(string vid)
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var vrgAdd = db.Vorgangs
               .Include(x => x.AidNavigation)
               .ThenInclude(x => x.MaterialNavigation)
               .Include(x => x.AidNavigation.DummyMatNavigation)
               .Include(x => x.RidNavigation)
               .First(x => x.VorgangId.Trim() == vid);

            if (vrgAdd.ArbPlSap?.Length >= 3)
            {
                if (int.TryParse(vrgAdd.ArbPlSap[..3], out int c))
                    if (UserInfo.User.UserCosts.Any(y => y.CostId == c))
                    {
                        _orders.Add(vrgAdd);
                        return true;
                    }
            }
            return false;
        }
        public void Closing()
        {
            if (DBctx.ChangeTracker.HasChanges())
            {
                if (_settingsService.IsSaveMessage)
                {
                    var result = MessageBox.Show("Sollen die Änderungen in Lieferliste gespeichert werden?",
                        Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        DBctx.SaveChanges();
                    }
                }
                else DBctx.SaveChanges();
            }
        }
    }
    public class ProjectStruct : IComparable
    {
        public string ProjectPsp { get; }
        public ProjectTypes.ProjectType ProjectType { get; }
        public string? ProjectInfo { get; }

        public ProjectStruct() { }
        public ProjectStruct(string ProjectPsp, ProjectTypes.ProjectType ProjectType, string? projectInfo)
        {
            this.ProjectPsp = ProjectPsp;
            this.ProjectType = ProjectType;
            this.ProjectInfo = projectInfo;
        }
        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            ProjectStruct? otherProjectStruct = obj as ProjectStruct;
            if (otherProjectStruct != null)
                return this.ProjectPsp.CompareTo(otherProjectStruct.ProjectPsp);
            else
                throw new ArgumentException("Object is not a ProjectStruct");
        }
    }
}
