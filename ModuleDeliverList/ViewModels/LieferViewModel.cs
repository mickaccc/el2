using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using ModuleDeliverList.UserControls;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ModuleDeliverList.ViewModels
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows7.0")]
    internal class LieferViewModel : ViewModelBase
    {

        public ICollectionView OrdersView { get; private set; }
        public ICommand TextSearchCommand => _textSearchCommand ??= new RelayCommand(OnTextSearch);
        //public ICommand OpenExplorerCommand => _openExplorerCommand ??= new RelayCommand(OnOpenExplorer);
        public ICommand FilterCommand => _filterCommand ??= new RelayCommand(OnFilter);
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
        private RelayCommand? _filterCommand;
        private string _searchFilterText = string.Empty;
        private string _searchFilterProj = string.Empty;
        private CmbFilter _cmbFilter;
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
        public CmbFilter SelectedFilter
        {

            get => _cmbFilter;
            set
            {
                if (_cmbFilter != value)
                {
                    _cmbFilter = value;
                    NotifyPropertyChanged(() => SelectedFilter);
                    OrdersView.Refresh();
                }
            }
        }
        public string SearchFilterProj
        {
            get => _searchFilterProj;
            set
            {
                if (value != _searchFilterProj)
                {
                    _searchFilterProj = value;
                    NotifyPropertyChanged(() => SearchFilterProj);
                    OrdersView?.Refresh();
                }
            }
        }

        public LieferViewModel(IContainerProvider container,
            IApplicationCommands applicationCommands)
        {
            _applicationCommands = applicationCommands;
            DBctx = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            SortAscCommand = new ActionCommand(OnAscSortExecuted, OnAscSortCanExecute);
            SortDescCommand = new ActionCommand(OnDescSortExecuted, OnDescSortCanExecute);

            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            ArchivateCommand = new ActionCommand(OnArchivateExecuted, OnArchivateCanExecute);
            OrderTask = new NotifyTaskCompletion<ICollectionView>(LoadDataAsync());
            _applicationCommands.ArchivateCommand.RegisterCommand(ArchivateCommand);

        }
        private bool OrdersView_FilterPredicate(object value)
        {

            var ord = (Vorgang)value;

            var accepted = true;

            if (!string.IsNullOrWhiteSpace(_searchFilterText))
            {
                if (!(accepted = ord.Aid.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase)))
                    if (!(accepted = ord.AidNavigation.Material?.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase) ?? false))
                        accepted = ord.AidNavigation.MaterialNavigation?.Bezeichng?.Contains(_searchFilterText, StringComparison.CurrentCultureIgnoreCase) ?? false;
            }
            if (accepted && _cmbFilter == CmbFilter.INVISIBLE) accepted = !ord.Visability;
            if (accepted && _cmbFilter == CmbFilter.READY) accepted = ord.AidNavigation.Fertig;
            if (accepted && _cmbFilter == CmbFilter.START) accepted = ord.Text?.Contains("STARTEN", StringComparison.CurrentCultureIgnoreCase) ?? false;
            if (accepted && _cmbFilter == CmbFilter.SALES) accepted = ord.Aid.StartsWith("VM");
            if (accepted && _cmbFilter == CmbFilter.DEVELOP) accepted = ord.Aid.StartsWith("EM");
            if (accepted) accepted = !ord.AidNavigation.Abgeschlossen;

            if (accepted && _searchFilterProj != string.Empty) accepted = ord.AidNavigation.ProId == _searchFilterProj;
            return accepted;

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

        private void OnFilter(object obj)
        {
            _cmbFilter = (CmbFilter)obj;
            OrdersView.Refresh();
        }
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
                MessageBox.Show(e.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
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
            var a = await DBctx.OrderRbs
               .Include(v => v.Vorgangs)
               .ThenInclude(r => r.RidNavigation)
               .Include(m => m.MaterialNavigation)
               .Include(d => d.DummyMatNavigation)
               .Where(x => x.Abgeschlossen == false)
               .ToListAsync();
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
                                _projects.Add(x.AidNavigation.ProId ?? string.Empty);
                            }
                            foreach (var r in ol.Where(x => x.Aktuell))
                            {
                                result.Add(r);

                            }
                        }
                    }
                }
                _orders.AddRange(result.OrderBy(x => x.SpaetEnd));

                OrdersView = CollectionViewSource.GetDefaultView(_orders);
                OrdersView.Filter += OrdersView_FilterPredicate;
            });
            return OrdersView;
        }
    }
}
