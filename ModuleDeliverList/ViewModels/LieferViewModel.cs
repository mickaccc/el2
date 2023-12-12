using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace ModuleDeliverList.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows7.0")]
    internal class LieferViewModel : ViewModelBase
    {

        public ICollectionView OrdersView { get; private set; }
        public ICommand TextSearchCommand => _textSearchCommand ??= new RelayCommand(OnTextSearch);
        private ConcurrentObservableCollection<Vorgang> _orders { get; } = [];
        private DB_COS_LIEFERLISTE_SQLContext DBctx { get; set; }
        public ActionCommand SortAscCommand { get; private set; }
        public ActionCommand SortDescCommand { get; private set; }
        public ActionCommand SaveCommand { get; private set; }
        public ActionCommand ArchivateCommand { get; private set; }

        public string Title { get; } = "Lieferliste";

        private readonly Dictionary<string, string> _filterCriterias = new();
        private readonly string _sortField = string.Empty;
        private readonly string _sortDirection = string.Empty;
        private RelayCommand? _textSearchCommand;
        private string _searchFilterText = string.Empty;
        private string _selectedProjectFilter = string.Empty;
        private string _selectedSectionFilter = string.Empty;
        private static System.Timers.Timer? _timer;
        private IContainerProvider _container;
        IEventAggregator _ea;
        private CmbFilter _selectedDefaultFilter;
        private List<Ressource> _ressources = [];
        private HashSet<string> _sections =[];

        public HashSet<string> Sections
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

        private HashSet<string> _projects = new();
        public HashSet<string> Projects
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
        private static readonly object _lock = new();
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
            [Description("")]
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
            SALES
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
                    OrdersView?.Refresh();
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
                    OrdersView?.Refresh();
                }
            }
        }
        
        public LieferViewModel(IContainerProvider container,
            IApplicationCommands applicationCommands,
            IEventAggregator ea)
        {
            _applicationCommands = applicationCommands;
            DBctx = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            _container = container;
            _ea = ea;

            SortAscCommand = new ActionCommand(OnAscSortExecuted, OnAscSortCanExecute);
            SortDescCommand = new ActionCommand(OnDescSortExecuted, OnDescSortCanExecute);

            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            ArchivateCommand = new ActionCommand(OnArchivateExecuted, OnArchivateCanExecute);
            OrderTask = new NotifyTaskCompletion<ICollectionView>(LoadDataAsync());
            _applicationCommands.ArchivateCommand.RegisterCommand(ArchivateCommand);
            _ea.GetEvent<MessageVorgangChanged>().Subscribe(MessageReceived);
            SetTimer();

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

            if (!string.IsNullOrWhiteSpace(_searchFilterText))
            {
                if (!(accepted = ord.Aid.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase)))
                    if (!(accepted = ord.AidNavigation.Material?.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase) ?? false))
                        accepted = ord.AidNavigation.MaterialNavigation?.Bezeichng?.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase) ?? false;
            }
            if (accepted && _selectedDefaultFilter == CmbFilter.INVISIBLE) accepted = !ord.Visability;
            if (accepted && _selectedDefaultFilter == CmbFilter.READY) accepted = ord.AidNavigation.Fertig;
            if (accepted && _selectedDefaultFilter == CmbFilter.START) accepted = ord.Text?.Contains("STARTEN", StringComparison.CurrentCultureIgnoreCase) ?? false;
            if (accepted && _selectedDefaultFilter == CmbFilter.SALES) accepted = ord.Aid.StartsWith("VM");
            if (accepted && _selectedDefaultFilter == CmbFilter.DEVELOP) accepted = ord.Aid.StartsWith("EM");
            if (accepted) accepted = !ord.AidNavigation.Abgeschlossen;

            if (accepted && _selectedProjectFilter != string.Empty) accepted = ord.AidNavigation.ProId == _selectedProjectFilter;
            if (accepted && _selectedSectionFilter != string.Empty) accepted = _ressources?
                    .FirstOrDefault(x => x.Inventarnummer == ord.ArbPlSap?[3..])?
                    .WorkArea?.Bereich == _selectedSectionFilter;
            return accepted;

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
                                db.Database.ExecuteSqlRaw(@"DELETE FROM msg WHERE id={0}", mess.Id);
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
        private void OnTextSearch(object commandParameter)
        {
            if (commandParameter is not TextChangedEventArgs change) return;
            var tb = (TextBox)change.OriginalSource;
            if (tb.Text.Length != 0 && tb.Text.Length < 3) return;
            _searchFilterText = tb.Text;
            var uiContext = SynchronizationContext.Current;
            uiContext?.Send(x => OrdersView.Refresh(), null);
        }
        #region Commands
 
        private void OnArchivateExecuted(object obj)
        {
            if (obj is string onr)
            {
                _orders.First(x => x.Aid == onr).AidNavigation.Abgeschlossen = true;
                DBctx.SaveChangesAsync();
                OrdersView.Refresh();
            }
        }

        private bool OnArchivateCanExecute(object arg)
        {
            if (arg is string onr)
            {
                bool f = _orders.First(x => x.Aid == onr).AidNavigation.Fertig;
                return PermissionsProvider.GetInstance().GetUserPermission(Permissions.Archivate) && f;
            }
            return false;
        }

        private void OnSaveExecuted(object obj)
        {
            DBctx.SaveChangesAsync();
        }
        private bool OnSaveCanExecute(object arg)
        {

            try
            {
                return DBctx.ChangeTracker.HasChanges();
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.Message, "CanSave", MessageBoxButton.OK, MessageBoxImage.Error);
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
            _projects.Add(String.Empty);
            _sections.Add(String.Empty);
            var a = await DBctx.OrderRbs
               .Include(v => v.Vorgangs)
               .ThenInclude(r => r.RidNavigation)
               .Include(m => m.MaterialNavigation)
               .Include(d => d.DummyMatNavigation)
               .Where(x => x.Abgeschlossen == false)
               .ToListAsync();
            var ress = await DBctx.Ressources.AsNoTracking()
                .Include(x => x.WorkArea)
                .ToArrayAsync();
            _ressources.AddRange(ress);
            await Task.Factory.StartNew(() =>
            {
                HashSet<Vorgang> result = new();

                lock (_lock)
                {
                    HashSet<Vorgang> ol = new();
                    foreach (var v in a)
                    {
                        ol.Clear();
                        bool relev = false;
                        foreach (var x in v.Vorgangs)
                        {
                            ol.Add(x);
                            if (x.ArbPlSap?.Length >= 3)
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
                                    _projects.Add(p);
                            }

                            foreach (var r in ol.Where(x => x.Aktuell))
                            {
                                result.Add(r);
                                var inv = (r.ArbPlSap != null) ? r.ArbPlSap[3..] : string.Empty;
                                var z = ress.FirstOrDefault(x => x.Inventarnummer?.Trim() == inv)?.WorkArea?.Bereich;
                                if (z != null)
                                {
                                    _sections.Add(z);
                                }
                            }

                        }
                    }

                    _orders.AddRange(result.OrderBy(x => x.SpaetEnd));

                }
            });
            OrdersView = CollectionViewSource.GetDefaultView(_orders);
            OrdersView.Filter += OrdersView_FilterPredicate;
            return OrdersView;
        }
    }
}
