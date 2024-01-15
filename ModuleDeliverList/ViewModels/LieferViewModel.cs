using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using ModuleDeliverList.UserControls;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

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
        public ActionCommand SetMarkerCommand { get; private set; }
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
        private static System.Timers.Timer? _timer;
        private static System.Timers.Timer? _autoSaveTimer;
        private IContainerProvider _container;
        IEventAggregator _ea;
        IUserSettingsService _settingsService;
        private CmbFilter _selectedDefaultFilter;
        private static List<Ressource> _ressources = [];
        private static SortedDictionary<int, string> _sections = [];
        public SortedDictionary<int, string> Sections => _sections;
        private static List<string> _projects = new();
        public static List<string> Projects => _projects;
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
            SetMarkerCommand = new ActionCommand(OnSetMarkerExecuted, OnSetMarkerCanExecute);
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            FilterSaveCommand = new ActionCommand(OnFilterSaveExecuted, OnFilterSaveCanExecute);

            OrderTask = new NotifyTaskCompletion<ICollectionView>(LoadDataAsync());

            _ea.GetEvent<MessageVorgangChanged>().Subscribe(MessageReceived);
            _ea.GetEvent<MessageOrderChanged>().Subscribe(MessageOrderReceived);
            _ea.GetEvent<MessageOrderArchivated>().Subscribe(MessageOrderArchivated);

            if (_settingsService.IsAutoSave) SetAutoSave();

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

        private static bool OnSetMarkerCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.SETMARK);
        }

        private void OnSetMarkerExecuted(object obj)
        {
            try
            {
                if (obj == null) return;
                var values = (object[])obj;
                var name = (string)values[0];
                var desc = (Vorgang)values[1];

                if (name == "DelBullet") desc.Bullet = Brushes.White.ToString();
                if (name == "Bullet1") desc.Bullet = Brushes.Red.ToString();
                if (name == "Bullet2") desc.Bullet = Brushes.Green.ToString();
                if (name == "Bullet3") desc.Bullet = Brushes.Yellow.ToString();
                if (name == "Bullet4") desc.Bullet = Brushes.Blue.ToString();

                //ProcessesCV.Refresh();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "SetMarker", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void MessageOrderArchivated(OrderRb rb)
        {
            var o = _orders.FirstOrDefault(x => x.Aid == rb.Aid);
            if (o != null)
            {
                o.AidNavigation.Abgeschlossen = rb.Abgeschlossen;
                DBctx.ChangeTracker.Entries<OrderRb>().First(x => x.Entity.Aid == rb.Aid).State = EntityState.Unchanged;
                OrdersView.Refresh();
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

                        }
                    }
                });
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "MsgReceivedLieferlisteOrder", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void MessageReceived(List<string?> vrgIdList)
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
            if (accepted && _selectedDefaultFilter == CmbFilter.SALES) accepted = ord.Aid.StartsWith("VM") == !FilterInvers;
            if (accepted && _selectedDefaultFilter == CmbFilter.DEVELOP) accepted = ord.Aid.StartsWith("EM") == !FilterInvers;
            if (accepted && _selectedDefaultFilter == CmbFilter.EXERTN) accepted = (ord.ArbPlSap == "_EXTERN_") == !FilterInvers;

            if (accepted) accepted = !ord.AidNavigation.Abgeschlossen;
            if (accepted && !string.IsNullOrEmpty(_selectedProjectFilter)) accepted = ord.AidNavigation.ProId == _selectedProjectFilter;
            if (accepted && _selectedSectionFilter != string.Empty) accepted = _ressources?
                    .FirstOrDefault(x => x.Inventarnummer == ord.ArbPlSap?[3..])?
                    .WorkArea?.Bereich == _selectedSectionFilter;


            return accepted;
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
            if (DBctx.ChangeTracker.HasChanges()) DBctx.SaveChangesAsync();
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
            FilterInvers = false;
        }
        private void OnToggleFilter(object obj)
        {
            FilterInvers = !FilterInvers;
        }


        private void OnSaveExecuted(object obj)
        {
            DBctx.SaveChangesAsync();
        }
        private bool OnSaveCanExecute(object arg)
        {
            try
            {
                DBctx.ChangeTracker.DetectChanges();
                return DBctx.ChangeTracker.HasChanges();
            }
            catch (InvalidOperationException e)
            {
                //MessageBox.Show(e.Message, "CanSave", MessageBoxButton.OK, MessageBoxImage.Error);
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
            _projects.Add(string.Empty);
            _sections.Add(0, string.Empty);
            var a = await DBctx.OrderRbs
               .Include(v => v.Vorgangs)
               .ThenInclude(r => r.RidNavigation)
               .Include(m => m.MaterialNavigation)
               .Include(d => d.DummyMatNavigation)
               .Include(p => p.Pro)
               .Where(x => x.Abgeschlossen == false)
               .ToListAsync();
            var ress = await DBctx.Ressources.AsNoTracking()
                .Include(x => x.WorkArea)
                .ToArrayAsync();
            var filt = await DBctx.ProductionOrderFilters.AsNoTracking().ToArrayAsync();
            _ressources.AddRange(ress);
            await Task.Factory.StartNew(() =>
            {
                HashSet<Vorgang> result = new();

                lock (_lock)
                {

                    HashSet<string> proj = new();
                    HashSet<Vorgang> ol = new();
                    foreach (var v in a)
                    {
                        ol.Clear();
                        bool relev = false;
                        foreach (var x in v.Vorgangs)
                        {
                            ol.Add(x);
                            if (filt.Any(y => y.OrderNumber == x.Aid)) relev = true;
                            if (x.ArbPlSap?.Length >= 3 && !relev)
                            {
                                if (int.TryParse(x.ArbPlSap[..3], out int c))
                                    if (UserInfo.User.UserCosts.Any(y => y.CostId == c))
                                    {
                                        relev = true;
                                    }
                            }
                        }
                        if (relev)
                        {

                            foreach (var x in ol.Where(x => x.AidNavigation.ProId != null))
                            {
                                var p = x.AidNavigation.ProId;
                                if (p != null)
                                    proj.Add(p);
                            }

                            foreach (var r in ol.Where(x => x.Aktuell))
                            {
                                result.Add(r);
                                var inv = (r.ArbPlSap != null) ? r.ArbPlSap[3..] : string.Empty;
                                var z = ress.FirstOrDefault(x => x.Inventarnummer?.Trim() == inv)?.WorkArea;
                                if (z != null)
                                {
                                    if (!_sections.Keys.Contains(z.Sort))
                                        _sections.Add(z.Sort, z.Bereich);
                                }
                            }
                        }
                    }
                    _projects.AddRange(proj.Order());

                    _orders.AddRange(result.OrderBy(x => x.SpaetEnd));
 
                }
            });
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

        public void Closing()
        {
            if (DBctx.ChangeTracker.HasChanges())
            {
                if (_settingsService.IsSaveMessage)
                {
                    var result = MessageBox.Show("Sollen die Änderungen in Teamleiter-Zuteilungen gespeichert werden?",
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
}
