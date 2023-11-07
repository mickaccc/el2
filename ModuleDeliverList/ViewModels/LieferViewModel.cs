using CompositeCommands.Core;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ModuleDeliverList.ViewModels
{
    class LieferViewModel : ViewModelBase
    {
        
        public ICollectionView OrdersView { get; private set; }
        public ICommand TextSearchCommand => _textSearchCommand ??= new RelayCommand(OnTextSearch);
        //public ICommand OpenExplorerCommand => _openExplorerCommand ??= new RelayCommand(OnOpenExplorer);
        public ICommand FilterCommand => _filterCommand ??= new RelayCommand(OnFilter);
        private ConcurrentObservableCollection<Vorgang> _orders { get; } = new();

        public ActionCommand SortAscCommand { get; private set; }
        public ActionCommand SortDescCommand { get; private set; }
        public ActionCommand OrderViewCommand { get; private set; }
        public ActionCommand SaveCommand { get; private set; }
        public string Key { get; } = "lie";

        public string Title { get; } = "Lieferliste";

        private readonly Dictionary<string, string> _filterCriterias = new();
        private readonly string _sortField = string.Empty;
        private readonly string _sortDirection = string.Empty;
        private RelayCommand? _textSearchCommand;
        private RelayCommand? _openExplorerCommand;
        private RelayCommand? _filterCommand;
        private string _searchFilterText = string.Empty;
        private CmbFilter _cmbFilter;
        private static object _lock = new object();
        private IContainerExtension _container;
        private string _setting;
        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set 
            {
                if (_applicationCommands != value)
                _applicationCommands = value;
                NotifyPropertyChanged(() => ApplicationCommands);
            }
        }
        public NotifyTaskCompletion<ICollectionView>? OrderTask { get; private set; }
        
        internal CollectionViewSource OrdersViewSource {get; private set;} = new();

        public enum CmbFilter
        {
            [Description("")]
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
           
            get { return _cmbFilter; }
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
        
        public LieferViewModel(IContainerExtension container, IApplicationCommands applicationCommands)
        {
            _container = container;
            _applicationCommands = applicationCommands;
 
            SortAscCommand = new ActionCommand(OnAscSortExecuted, OnAscSortCanExecute);
            SortDescCommand = new ActionCommand(OnDescSortExecuted, OnDescSortCanExecute);
            OrderViewCommand = new ActionCommand(OnOrderViewExecuted, OnOrderViewCanExecute);
            SaveCommand = new ActionCommand(OnSaveExecuted, OnSaveCanExecute);
            OrderTask = new NotifyTaskCompletion<ICollectionView>(LoadDataAsync());        
        }


        private bool OrdersView_FilterPredicate(object value)
        {
            var ord = (Vorgang)value;

            var accepted = true;

            if (!string.IsNullOrWhiteSpace(_searchFilterText))
            {
                _searchFilterText = _searchFilterText.ToUpper();
                if (!(accepted = ord.Aid.ToUpper().Contains(_searchFilterText)))
                    if (!(accepted = ord.AidNavigation.Material?.ToUpper().Contains(_searchFilterText) ?? false))
                        accepted = ord.AidNavigation.MaterialNavigation?.Bezeichng?.ToUpper().Contains(_searchFilterText) ?? false;
            }
            if (accepted && _cmbFilter == CmbFilter.INVISIBLE) accepted = ord.Ausgebl;
            if (accepted && _cmbFilter == CmbFilter.READY) accepted = ord.AidNavigation.Fertig;
            if (accepted && _cmbFilter == CmbFilter.START) accepted = ord.Text.ToUpper().Contains("STARTEN");
            if (accepted && _cmbFilter == CmbFilter.SALES) accepted = ord.Aid.StartsWith("VM");
            if (accepted && _cmbFilter == CmbFilter.DEVELOP) accepted = ord.Aid.StartsWith("EM");
            return accepted;
        }

        public string ToolTip
        {
            get
            {
                var toolTip = new StringBuilder();
                if (_filterCriterias.Count > 0)
                {
                    toolTip.Append("gefiltert:\n");
                    foreach (KeyValuePair<String, String> c in _filterCriterias)
                    {
                        toolTip.Append(c.Key).Append(" = ").Append(c.Value).Append("\n");
                    }
                }
                if (_sortField != string.Empty)
                {
                    toolTip.Append("sortiert nach:\n").Append(_sortField).Append(" / ").Append(_sortDirection);
                }

                return toolTip.ToString();
                
            }
        }

        private void OnTextSearch(object commandParameter)
        {
            if (commandParameter is TextChangedEventArgs change)
            {
                var tb = (TextBox)change.OriginalSource;
                if (tb.Text.Length == 0 || tb.Text.Length >= 3)
                {
                    _searchFilterText = tb.Text;
                    var uiContext = SynchronizationContext.Current;
                    uiContext?.Send(x => OrdersView.Refresh(), null);
                }
            }
        }


        #region Commands
 
        private void OnFilter(object obj)
        {
            _cmbFilter = (CmbFilter) obj;
            OrdersView.Refresh();
        }
        private static bool OnOrderViewCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission("OOPEN01");
        }
        private static void OnOrderViewExecuted(object parameter)
        {
 
        }
        private void OnSaveExecuted(object obj)
        {
            using var Dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            Dbctx.SaveChangesAsync();
        }
        private bool OnSaveCanExecute(object arg)
        {

            try
            {
                using var Dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                return Dbctx.ChangeTracker.HasChanges();
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
            HashSet<Vorgang> result = new();
            BindingOperations.EnableCollectionSynchronization(_orders, _lock);
            await Task.Factory.StartNew(() =>
            {
                using (var Dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>())
                {
                    var a = Dbctx.OrderRbs
                        .Include(v => v.Vorgangs)
                        .Include(m => m.MaterialNavigation)
                        .Include(d => d.DummyMatNavigation)
                        .Where(x => x.Abgeschlossen == false)
                        .ToList();
                    HashSet<Vorgang> ol = new();
                    foreach (var v in a)
                    {
                        ol.Clear();
                        bool relev = false;
                        foreach (var x in v.Vorgangs)
                        {
                            ol.Add(x);
                            if (x.ArbPlSap.Length >= 3)
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
                            foreach (var r in ol.Where(x => x.Aktuell))
                            {
                                result.Add(r);
                            }
                        }
                    }
                    _orders.AddRange(result.OrderBy(x => x.SpaetEnd));
                }
                OrdersView = CollectionViewSource.GetDefaultView(_orders);
                OrdersView.Filter += OrdersView_FilterPredicate;
            });

            return OrdersView;
        }
 
    }
}
