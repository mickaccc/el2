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

namespace ModuleDeliverList.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows7.0")]
    internal class LieferViewModel : ViewModelBase, IViewModel
    {

        public ICollectionView OrdersView { get; private set; }
        public ICommand TextSearchCommand => _textSearchCommand ??= new RelayCommand(OnTextSearch);
        public ICommand FilterDeleteCommand => _filterDeleteCommand ??= new RelayCommand(OnFilterDelete);
        public ICommand ToggleFilterCommand => _toggleFilterCommand ??= new RelayCommand(OnToggleFilter);

        private ConcurrentObservableCollection<Vorgang> _orders { get; } = [];
        private DB_COS_LIEFERLISTE_SQLContext DBctx { get; set; }
        public ActionCommand SortAscCommand { get; private set; }
        public ActionCommand SortDescCommand { get; private set; }
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
        private string _searchFilterText = string.Empty;
        private string _selectedProjectFilter = string.Empty;
        private string _selectedSectionFilter = string.Empty;
        private static System.Timers.Timer? _timer;
        private static System.Timers.Timer? _autoSaveTimer;
        private IContainerProvider _container;
        IEventAggregator _ea;
        IUserSettingsService _settingsService;
        private CmbFilter _selectedDefaultFilter;
        private List<Ressource> _ressources = [];
        private SortedDictionary<byte, string> _sections =[];

        public SortedDictionary<byte, string> Sections
        {
            get { return _sections; }
            set
            {
                if (value != _sections)
                {
                    _sections = value;
                    NotifyPropertyChanged(() => Sections);
                }
            }
        }

        private List<string> _projects = new();
        public List<string> Projects
        {
            get => _projects;
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
                if(_filterInvers != value)
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

            SortAscCommand = new ActionCommand(OnAscSortExecuted, OnAscSortCanExecute);
            SortDescCommand = new ActionCommand(OnDescSortExecuted, OnDescSortCanExecute);
            InvisibilityCommand = new ActionCommand(OnInvisibilityExecuted, OnInvisibilityCanExecute);

            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            FilterSaveCommand = new ActionCommand(OnFilterSaveExecuted, OnFilterSaveCanExecute);

            OrderTask = new NotifyTaskCompletion<ICollectionView>(LoadDataAsync());

            _ea.GetEvent<MessageVorgangChanged>().Subscribe(MessageReceived);
            _ea.GetEvent<MessageOrderChanged>().Subscribe(MessageOrderReceived);
            SetTimer();
            if (_settingsService.IsAutoSave) SetAutoSave();
        }



        private bool OnInvisibilityCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.LieferVrgInvis);
        }

        private void OnInvisibilityExecuted(object obj)
        {
            if(obj is Vorgang v)
            {
                v.Visability = !v.Visability;
                OrdersView.Refresh();
            }
        }
        private void MessageOrderReceived(OrderRb rb)
        {
            var o = _orders.FirstOrDefault(x => x.Aid == rb.Aid);
            if(o != null)
            {
                o.AidNavigation.Abgeschlossen = rb.Abgeschlossen;
                o.RunPropertyChanged();
            }
        }
        private void MessageReceived(Vorgang vrg)
        {
            var v = _orders.FirstOrDefault(x => x.VorgangId == vrg.VorgangId);
            if (v != null)
            {
                v.ActualEndDate = vrg.ActualEndDate;
                v.ActualStartDate = vrg.ActualStartDate;
                v.Aktuell = vrg.Aktuell;
                v.SysStatus = vrg.SysStatus;
                v.Termin = vrg.Termin;
                v.Text = vrg.Text;
                v.BemM = vrg.BemM;
                v.BemMa = vrg.BemMa;
                v.BemT = vrg.BemT;
                v.CommentMach = vrg.CommentMach;
                v.QuantityMiss = vrg.QuantityMiss;
                v.QuantityRework = vrg.QuantityRework;
                v.QuantityScrap = vrg.QuantityScrap;
                v.QuantityYield = vrg.QuantityYield;

                v.RunPropertyChanged();
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
        private void SetTimer()
        {
            // Create a timer with a 30 seconds interval.
            _timer = new System.Timers.Timer(2000);
            // Hook up the Elapsed event for the timer. 
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
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
        private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            if (OrdersView != null)
            {
                Task.Factory.StartNew(() =>
                {
                    lock (_lock)
                    {
                        using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                        {
                            var m = db.Msgs
                                .Include(x => x.Onl)
                                .Where(x => x.Onl.PcId == UserInfo.PC && x.Onl.UserId == UserInfo.User.UserIdent)
                                .ToList();
                            foreach (var mess in m)
                            {
                                db.Database.ExecuteSqlRawAsync(@"DELETE FROM msg WHERE id={0}", mess.Id);
                                var o = _orders.FirstOrDefault(x => x.VorgangId == mess.PrimaryKey);
                                if (o != null)
                                {
                                    Vorgang vrg = db.Vorgangs.First(x => x.VorgangId == mess.PrimaryKey);
                                    if (mess.TableName == "Vorgang")
                                    {
                                        o.SysStatus = vrg.SysStatus;
                                        o.Spos = vrg.Spos;
                                        o.Text = vrg.Text;
                                        o.ActualEndDate = vrg.ActualEndDate;
                                        o.ActualStartDate = vrg.ActualStartDate;
                                        o.Aktuell = vrg.Aktuell;
                                        o.Bullet = vrg.Bullet;
                                        o.BemM = vrg.BemM;
                                        o.BemMa = vrg.BemMa;
                                        o.BemT = vrg.BemT;
                                        o.CommentMach = vrg.CommentMach;
                                        o.QuantityMiss = vrg.QuantityMiss;
                                        o.QuantityRework = vrg.QuantityRework;
                                        o.QuantityScrap = vrg.QuantityScrap;
                                        o.QuantityYield = vrg.QuantityYield;

                                    }
                                    else if (mess.TableName == "OrderRB")
                                    {
                                        o.AidNavigation.Abgeschlossen = vrg.AidNavigation.Abgeschlossen;
                                        o.AidNavigation.Dringend = vrg.AidNavigation.Dringend;
                                        o.AidNavigation.Fertig = vrg.AidNavigation.Fertig;
                                        o.AidNavigation.Mappe = vrg.AidNavigation.Mappe;
                                    }
                                    o.RunPropertyChanged();                                 
                                    _ea.GetEvent<MessageVorgangChanged>().Publish(o);
                                }
                            }
                        }
                    }
                });
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
        private bool OnDescSortCanExecute(object arg)
        {
            return true;
        }
        private void OnDescSortExecuted(object parameter)
        {

            if (parameter is LieferlisteControl lvc)
            {
                OrdersViewSource.SortDescriptions.Clear();
                OrdersViewSource.SortDescriptions.Add(new SortDescription(lvc.HasMouseOver, ListSortDirection.Descending));
                OrdersView.Refresh();
            }

        }
        private bool OnAscSortCanExecute(Object arg)
        { return true; }

        private void OnAscSortExecuted(object parameter)
        {

            var v = Translate();

            if (v != string.Empty)
            {
                OrdersViewSource.SortDescriptions.Clear();
                OrdersViewSource.SortDescriptions.Add(new SortDescription(v, ListSortDirection.Ascending));
                var uiContext = SynchronizationContext.Current;
                uiContext?.Send(x => OrdersView.Refresh(), null);
            }
        }
        #endregion
        public string HasMouse;
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
                            if(filt.Any(y => y.OrderNumber ==  x.Aid)) relev = true;
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
                                    if(!_sections.Keys.Contains(z.Sort))
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
                if(live.CanChangeLiveFiltering)
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
